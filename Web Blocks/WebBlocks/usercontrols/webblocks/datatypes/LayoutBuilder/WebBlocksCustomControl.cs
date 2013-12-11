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
using System.Web.UI.WebControls;
using WebBlocks.usercontrols.data_types;
using WebBlocks.usercontrols.webblocks.datatypes.LayoutBuilder;

namespace WebBlocks.usercontrols.webblocks.datatypes
{
    /// <summary>
    /// This custom control is a wrapper for the Layout Builder usercontrol
    /// </summary>
    public class WebBlocksCustomControl : PlaceHolder
    {
        protected object InternalValue = null;

        public object value { get { return builder.value; } set { InternalValue = value; } }
        public WebBlocksPrevalueEditor PreValueEditor { get; set; }

        public layout_builder builder;
        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            builder = (layout_builder) Page.LoadControl("~/usercontrols/webblocks/datatypes/LayoutBuilder/LayoutBuilder.ascx");
            builder.value = InternalValue;
            builder.PreValueAccessor = new WebBlocksPreValuesAccessor(PreValueEditor);

            Controls.Add(builder);
        }
    }
}