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
using System.Web;
using System.Xml;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;
using umbraco.presentation.preview;

namespace WebBlocks.Model.Helpers
{
    public class PreviewNode
    {
        public static Node GetNode(int id = 0)
        {
            string cacheCurrentNodeId = "";
            
            id = id == 0 && (cacheCurrentNodeId = CacheHelper<string>.Get("wbCurrentPageNodeId")) != default(string)
                     ? int.Parse(cacheCurrentNodeId)
                     : id;

            if (id == 0 && !int.TryParse(HttpContext.Current.Request.QueryString["id"], out id))
            {
                return null;
            }

            try
            {
                Document d = new Document(id);
                User u = User.GetCurrent();
                PreviewContent previewContent = new PreviewContent(u);
                previewContent.PrepareDocument(u, d, true);
                XmlDocument xmlDoc = (XmlDocument)previewContent.XmlContent.Clone();
                XmlNode xmlNode = xmlDoc.GetElementById(id.ToString());
                return new Node(xmlNode);
            }
            catch (Exception)
            {
                return null;
            }
            

            
        }
    }
}
