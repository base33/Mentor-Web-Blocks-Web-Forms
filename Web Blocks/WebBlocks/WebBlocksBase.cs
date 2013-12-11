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
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using umbraco;
using umbraco.presentation.umbracobase;
using WebBlocks.Model.Helpers;
using WebBlocks.usercontrols.webblocks;

namespace WebBlocks
{
    [RestExtension("webblocks")]
    public class WebBlocksBase
    {
        [RestExtensionMethod(allowAll = true, returnXml = false)]
        public static string RenderBlock(string rawBlockId)
        {
            
            umbraco.BusinessLogic.User u = umbraco.BusinessLogic.User.GetCurrent();
            
            string contents = "";

            if (u != null)
            {
                
                try
                {
                    string[] parts = rawBlockId.Split(new[] {'?'});
                    int blockId = Convert.ToInt32(parts[0]);

                    CacheHelper<bool>.Add("wbIsInBuilder", true);
                    CacheHelper<string>.Add("wbCurrentPageNodeId", HttpContext.Current.Request.QueryString["id"]);

                    Page pageHolder = new Page();

                    HtmlForm htmlForm = new HtmlForm();

                    block viewControl = (block) pageHolder.LoadControl("~/usercontrols/webblocks/block.ascx");

                    htmlForm.Controls.Add(viewControl);
                    pageHolder.Controls.Add(htmlForm);

                    Type viewControlType = viewControl.GetType();
                    FieldInfo field = viewControlType.GetField("BlockId");

                    if (field != null)
                    {
                        field.SetValue(viewControl, blockId);
                    }

                    try
                    {
                        using (StringWriter sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pageHolder, sw, false);
                            contents = sw.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        //get our exception
                        throw ex.GetBaseException();
                    }
                    
                }
                catch (Exception ex)
                {
                    contents = GlobalSettings.DebugMode ? ex.Message : "Exception: This action is not allowed";
                }
                
            }

            return contents;
        }
    }
}