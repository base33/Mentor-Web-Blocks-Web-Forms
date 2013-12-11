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
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;

namespace WebBlocks.BusinessLogic.UmbracoExtensions
{
    public class ContourExtension
    {
        public ContourExtension()
        {
            Umbraco.Forms.Data.Storage.FormStorage.FormCreated += new EventHandler<Umbraco.Forms.Core.FormEventArgs>(FormStorage_FormCreated);
        }

        void  FormStorage_FormCreated(object sender, Umbraco.Forms.Core.FormEventArgs e)
        {
            Document contour = Document.MakeNew(e.Form.Name, DocumentType.GetByAlias("ContourBlock"), new User(0), 1134);
            contour.getProperty("formGuid").Value = e.Form.Id.ToString();

            try
            {
                contour.Publish(new User(0));
                umbraco.library.UpdateDocumentCache(contour.Id);
            }
            catch
            {
                contour.Save();
            }
            
        }
    }
}
