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
using umbraco.cms.businesslogic.datatype;

namespace WebBlocks.usercontrols.webblocks.datatypes
{
    public class WebBlocksDataType : AbstractDataEditor
    {
        private WebBlocksPrevalueEditor webBlocksPrevalueEditor;
        private WebBlocksCustomControl webBlocksControl = new WebBlocksCustomControl();

        public override Guid Id
        {
            get
            {
                return new Guid("849f2844-33c5-45d6-870c-87aa7e51d55e");
            }
        }

        public WebBlocksDataType()
        {
            webBlocksPrevalueEditor = webBlocksPrevalueEditor ?? (webBlocksPrevalueEditor = new WebBlocksPrevalueEditor(this));
            RenderControl = webBlocksControl;
            webBlocksControl.Init += WebBlocks_Init;
            DataEditorControl.OnSave += DataEditorControlOnOnSave;
            webBlocksControl.PreValueEditor = webBlocksPrevalueEditor;
        }

        protected void WebBlocks_Init(object sender, EventArgs e)
        {
            webBlocksControl.value = base.Data.Value ?? "";
        }

        private void DataEditorControlOnOnSave(EventArgs eventArgs)
        {
            base.Data.Value = webBlocksControl.value;
        }

        public override string DataTypeName
        {
            get { return "Web Blocks: Builder"; }
        }

        public override umbraco.interfaces.IDataPrevalue PrevalueEditor
        {
            get { return webBlocksPrevalueEditor; }
        }


    }
}