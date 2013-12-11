/****
    Mentor Web Blocks for Umbraco
    Copyright (C) 2013 Mentor Digital 
    (Mentor Communications Consultancy Ltd, 4 West End, Somerset Street, Bristol, BS2 8NE)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
****/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using umbraco.cms.businesslogic.macro;
using umbraco.interfaces;
using umbraco.NodeFactory;
using WebBlocks.BusinessLogic.Exceptions;
using WebBlocks.BusinessLogic.Helpers;
using WebBlocks.BusinessLogic.Interface;
using WebBlocks.BusinessLogic.Model;
using WebBlocks.BusinessLogic.Views.RenderingEngine;
using WebBlocks.Model;
using WebBlocks.Model.Helpers;

namespace WebBlocks.BusinessLogic.Views
{
    public class BlockView
    {
        protected Block block;
        protected Node currentPageNode = null;

        /// <summary>
        /// Prepares a BlockView for a specified Block.
        /// </summary>
        /// <param name="block"></param>
        public BlockView(Block block, Node currentPageNode = null)
        {
            this.block = block;
            this.currentPageNode = currentPageNode;
        }

        /// <summary>
        /// Renders the block
        /// </summary>
        /// <returns>rendered html for the block</returns>
        public void Render(HtmlGenericControl control)
        {
            if (!HttpContext.Current.Response.IsClientConnected)
                return;

            if (block.BlockType == null)
                return;

            IRenderingEngine renderingEngine = InitRenderingEngine(control);
            

            //store the current block being rendered 
            //(Razor API can add classes to this control etc)
            CacheHelper<HtmlGenericControl>.Add("wbCurrentBlockControl", control);
            //store the current block id (used by helper methods - e.g. xslt GetCurrentBlock())
            CacheHelper<Block>.Add("wbCurrentBlock", block);

            //rendering engine may be null if it is a usercontrol
            if (renderingEngine != null)
            {
                renderingEngine.CurrentNode = block.Node;
                //set the rendered content from the engine
                control.InnerHtml = renderingEngine.Render();
            }
        }

        /// <summary>
        /// Initialises the rendering engine
        /// Works out what rendering engine to use
        /// </summary>
        /// <param name="control">the control that is the block div</param>
        /// <returns>the engine set up to call render</returns>
        protected IRenderingEngine InitRenderingEngine(HtmlGenericControl control)
        {
            //Most options are razor so we declare it as razor engine first
            IRenderingEngine engine = new RazorRenderingEngine();
            
            //if the block is an embedded resource (compiled razor)
            //if (EmbeddedResources.IsEmbeddedBlock(block.BlockType))
            //{
            //    //setting scriptname to empty forces the macroengine to render the script code
            //    engine.Macro.ScriptName = "";
            //    engine.Macro.ScriptLanguage = "cshtml";
            //    //get the script code from the embeedded resource
            //    engine.Macro.ScriptCode = EmbeddedResources.GetEmbeddedScript(block.BlockType);
            //}
            ////if the block is a razor script block - a block that is not specialised
            //else 
            if (block.BlockType == BlockType.RazorScriptBlock)
            {
                //get the script name
                engine.Macro.ScriptName = block.Node.GetProperty("scriptName").Value;
                engine.Macro.ScriptLanguage = "cshtml";
                engine.Macro.ScriptCode = "";
            }
            //otherwise we will find the associated macro and render it as Razor or WebBlocksXslt
            //dev-note: usercontrols are rendered in the Block.ascx so that viewstate etc can be saved
            else
            {
                engine = RenderBlockBasedOnType(control, engine);
            }

            return engine;
        }

        private IRenderingEngine RenderBlockBasedOnType(HtmlGenericControl control, IRenderingEngine engine)
        {
            //get the macro
            Macro macroType = Macro.GetByAlias(block.BlockType);

            if(macroType == null)
                throw new MacroNotFoundException();

            string fileName = "";
            //get the correct rendering engine (razor or xslt) and will give us the fileName
            engine = GetEngineByMacroSetting(macroType, ref fileName);

            //if engine is not null (is XSLT or Razor)
            if (engine != null)
            {
                engine.Macro.ScriptName = fileName;
                engine.Macro.ScriptLanguage = fileName.Split('.').Last();
            }
            else //it is a usercontrol
            {
                RenderUserControl(macroType, control);
            }

            return engine;
        }

        protected IRenderingEngine GetEngineByMacroSetting(Macro macro, ref string fileName)
        {
            IRenderingEngine engine = new RazorRenderingEngine();
            

            //if a razor script is selected
            if (!string.IsNullOrEmpty(macro.ScriptingFile))
            {
                engine = new RazorRenderingEngine();
                fileName = macro.ScriptingFile;
            }
            //else if a xslt script is selected
            else if (!string.IsNullOrEmpty(macro.Xslt))
            {
                engine = new XsltRenderingEngine();
                engine.Macro.Alias = macro.Alias;
                fileName = macro.Xslt;
            }
            //else set it to null (this will then cause it to render as a usercontrol
            else if (!string.IsNullOrEmpty(macro.Type))
            {
                engine = null;
            }

            return engine;
        }

        protected void RenderUserControl(Macro macro, HtmlGenericControl control)
        {
            Page pageHolder = new Page();
            UserControl usercontrol = (UserControl)pageHolder.LoadControl("~/" + macro.Type);
            Type type = usercontrol.GetType().BaseType.UnderlyingSystemType;

            IProperty parameterProperty = block.Node.GetProperty("parameters");
            if (parameterProperty != null)
            {
                string json = parameterProperty.Value;
                List<UserControlProperty> properties = json.FromJson<List<UserControlProperty>>();
                if (properties != null)
                {
                    //FieldInfo[] infos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (UserControlProperty property in properties)
                    {
                        PropertyInfo customPropertyInfo = type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                        if (customPropertyInfo != null)
                        {
                            customPropertyInfo.SetValue(usercontrol,
                                                  GetConvertedValue(property.Value, customPropertyInfo.PropertyType), null);
                        }
                    }
                }
            }

            //add the current block
            PropertyInfo propertyInfo = type.GetProperty("Block", BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(usercontrol, Helpers.WebBlocks.GetNode(block.NodeId), null);
            }

            control.Controls.Add(usercontrol);
        }

        protected dynamic GetConvertedValue(string input, Type outputType)
        {
            //invoke generic method with out passed in type
            return typeof (ValueConverter)
                .GetMethod("ConvertType")
                .MakeGenericMethod(outputType)
                .Invoke(null, new object[] { input });
        }
    }
}
