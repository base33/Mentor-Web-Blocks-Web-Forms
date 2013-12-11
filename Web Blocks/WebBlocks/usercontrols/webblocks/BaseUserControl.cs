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
using System.Web;
using System.Web.UI;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;
using WebBlocks.BusinessLogic.Helpers;
using WebBlocks.Model;
using WebBlocks.Model.Helpers;

namespace WebBlocks.usercontrols.webblocks
{
    public class BaseUserControl : System.Web.UI.UserControl
    {
        private Node node = null;
        private Document document = null;

        /// <summary>
        /// The base path to the web blocks plugin.
        /// </summary>
        protected string basePath
        {
            get
            {
                return Page.ResolveUrl(umbraco.GlobalSettings.Path) + "/plugins/webblocks";
            }
        }

        /// <summary>
        /// Returns the umbraco Node belonging to the current page.
        /// </summary>
        protected Node currentNode
        {
            get
            {
                if (this.node == null)
                {
                    try
                    {
                        this.node = Node.GetCurrent();
                    }
                    catch
                    { }
                }

                return node;
            }
        }

        protected Document currentDocument
        {
            get
            {
                if (document == null)
                {
                    int docId = 0;
                    int.TryParse(Request.QueryString["id"], out docId);

                    if (docId > 0)
                    {
                        document = new Document(docId);
                    }
                }

                return document;
            }
        }

        /// <summary>
        /// Adds a link html tag to the head of the page along with it's attributes.
        /// </summary>
        /// <param name="rel"></param>
        /// <param name="href"></param>
        /// <param name="type"></param>
        /// <param name="media"></param>
        protected void AddLinkToHead(string rel, string href, string type, string media = "")
        {
            media = (string.IsNullOrEmpty(media)) ? media : " media=\"" + media + "\"";

            Page.Header.Controls.Add(new LiteralControl(string.Format(
                "<link rel=\"{0}\" href=\"{1}\" type=\"{2}\"{3} />",
                rel,
                href,
                type,
                media
            )));
        }

        /// <summary>
        /// Adds a CSS link tag to the head of the page.
        /// </summary>
        /// <param name="filename"></param>
        protected void AddCssToHead(string filename)
        {
            AddLinkToHead("stylesheet", filename, "text/css");
        }

        protected void SetInBuilder()
        {
            CacheHelper<bool>.Add("wbIsInBuilder", true);
        }

        /// <summary>
        /// Gets the list of containers from the current page
        /// </summary>
        /// <returns>The list of containers</returns>
        protected List<Container> GetCurrentPageContainers()
        {
            List<Container> containers = null;

            //get the json for the containers
            string json = "";

            if (HttpContext.Current.Items["builderJson"] != null)
            {
                json = HttpContext.Current.Items["builderJson"].ToString();
            }
            else
            {
                if (currentNode != null && currentNode.GetProperty("builder") != null)
                {
                    json = currentNode.GetProperty("builder").Value;
                }
                else if(currentDocument.getProperty("builder") != null)
                {
                    json = currentDocument.getProperty("builder").Value.ToString();
                }
            }

            //deserialise the list
            containers = json.FromJson<List<Container>>();

            return containers;
        }
    }
}