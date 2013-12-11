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
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Xml;
using WebBlocks.Model;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.MacroEngines;
using umbraco.NodeFactory;
using umbraco.presentation;
using umbraco.presentation.preview;
using WebBlocks.BusinessLogic.Interface;
using WebBlocks.BusinessLogic.Views.RenderingEngine;
using WebBlocks.Model.Helpers;

namespace WebBlocks.BusinessLogic.Helpers
{
    /// <summary>
    /// This is a Razor helper class / API to interact with the container block and also to get the current node
    /// </summary>
    public static class WebBlocks
    {
        /// <summary>
        /// Gets the current node for the page (whether in preview mode
        /// </summary>
        /// <returns></returns>
        public static DynamicNode GetCurrentNode()
        {
            DynamicNode dNode;

            try
            {
                int id = Node.getCurrentNodeId();
                dNode = WebBlocksHelper.IsInBuilder() | UmbracoContext.Current.InPreviewMode ?
                    GetDynamicNodeFromPreviewCache(id) :
                    new DynamicNode(id);
            }
            catch
            {
                dNode = GetDynamicNodeFromPreviewCache();
            }

            return dNode;
        }

        /// <summary>
        /// Gets the current node for the page (whether in preview mode
        /// </summary>
        /// <returns></returns>
        public static DynamicNode GetNode(int id)
        {
            DynamicNode dNode;

            try
            {
                dNode = WebBlocksHelper.IsInBuilder() || UmbracoContext.Current.InPreviewMode ?
                    GetDynamicNodeFromPreviewCache(id) :
                    new DynamicNode(id);
            }
            catch
            {
                dNode = GetDynamicNodeFromPreviewCache();
            }

            return dNode;
        }

        public static DynamicNode GetDynamicNodeFromPreviewCache(int id = 0)
        {
            string cacheCurrentNodeId = "";
            //TODO: Add this fix elsewhere
            id = id == 0 && (cacheCurrentNodeId = CacheHelper<string>.Get("wbCurrentPageNodeId")) != default(string)
                     ? int.Parse(cacheCurrentNodeId)
                     : id;

            if (id == 0 && !int.TryParse(HttpContext.Current.Request.QueryString["id"], out id))
            {
                return null;
            }

            Document d = new Document(id);
            User u = User.GetCurrent();
            PreviewContent previewContent = new PreviewContent(u);
            previewContent.PrepareDocument(u, d, true);
            XmlDocument xmlDoc = (XmlDocument)previewContent.XmlContent.Clone();
            XmlNode xmlNode = xmlDoc.GetElementById(id.ToString());

            return new DynamicNode(new Node(xmlNode));
        }

        public static bool IsInWebBlocksBuilder()
        {
            return WebBlocksHelper.IsInBuilder();
        }

        /// <summary>
        /// Gets a string resource from a url (use for twitter or other feed systems)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetWebResource(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string resource = string.Empty;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    resource = reader.ReadToEnd();
                }
            }

            return resource;
        }

        public static string AddBlockClass(string name)
        {
            HtmlGenericControl control = CacheHelper<HtmlGenericControl>.Get("wbCurrentBlockControl");
            string[] classes = control.Attributes["class"].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string classString = "";
            foreach (string aClass in classes)
            {
                classString += aClass + " ";
            }
            classString += name;
            control.Attributes.Add("class", classString);

            return "";
        }

        public static string RemoveBlockClass(string name)
        {
            HtmlGenericControl control = CacheHelper<HtmlGenericControl>.Get("wbCurrentBlockControl");
            string[] classes = control.Attributes["class"].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string classString = "";
            foreach (string aClass in classes)
            {
                if (name != aClass)
                    classString += aClass + " ";
            }
            classString = classString.TrimEnd(' ');
            control.Attributes.Add("class", classString);
            return "";
        }

        public static string SetBlockElement(string name)
        {
            HtmlGenericControl control = CacheHelper<HtmlGenericControl>.Get("wbCurrentBlockControl");
            control.TagName = name;
            return "";
        }

        public static HtmlGenericControl GetBlockParentContainer()
        {
            return CacheHelper<HtmlGenericControl>.Get("wbCurrentContainer");
        }

        public static string RenderCompiledRazorscript(string key, dynamic model)
        {
            IRenderingEngine engine = new RazorRenderingEngine();
            engine.Macro.ScriptName = "";
            engine.Macro.ScriptLanguage = "cshtml";
            //get the script code from the embeedded resource
            engine.Macro.ScriptCode = EmbeddedResources.GetEmbeddedScript(key);
            engine.CurrentNode = new Node(model.Id);
            return engine.Render();
        }
    }
}
