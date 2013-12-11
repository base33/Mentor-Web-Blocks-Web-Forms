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
using System.Web.UI.HtmlControls;
using WebBlocks.BusinessLogic.Views;
using WebBlocks.Model;
using WebBlocks.Model.Helpers;

namespace WebBlocks.usercontrols.webblocks
{
    public partial class block : BaseUserControl
    {
        public int BlockId = 0;
        public string Content = string.Empty;
        public string BlockType = WebBlocks.Model.BlockType.MacroBlock;
        public int SortOrder = 0;
        public bool IsPredefined = true;
        public string Class = string.Empty;
        public HtmlGenericControl ParentContainer = new HtmlGenericControl();

        protected void Page_Load(object sender, EventArgs e)
        {
            //create a div htmlgenericcontrol to hold the html
            HtmlGenericControl control = new HtmlGenericControl("div");

            string cssClass = "block";
            
            if(Class != string.Empty)
                cssClass += " " + Class;

            // If the block id is greater than 0 it is not a content only block
            if (BlockId > 0 || BlockType != WebBlocks.Model.BlockType.PageWysiwygBlock)
            {
                Block block = new Block(BlockId);


                CacheHelper<HtmlGenericControl>.Add("wbCurrentContainer", ParentContainer);

                // Don't do anything if the node doesn't exist anymore or if it's 
                // in the recycle bin (-20).
                if (block.BlockType == null || 
                    block.Node == null || 
                    (block.Node.Parent != null && block.Node.Parent.Id == -20))
                    return;

                if (block.Class != string.Empty)
                    cssClass += " " + block.Class;

                // Used by the front-end to determine if a block is predefined.
                // Predefined blocks can't be deleted.
                if (IsPredefined)
                {
                    cssClass += " predefined";
                }

                if(WebBlocksHelper.IsInBuilder())
                     control.Attributes.Add("rel", BlockId.ToString());

                BlockView blockView = new BlockView(block);
                control.Attributes.Add("class", cssClass);
                //render the block
                blockView.Render(control);
            }
            else
            {
                //This is a page rich text box
                control.Attributes.Add("class", cssClass + " pageWysiwygBlock");

                // Only add rel tags in builder mode.
                if (WebBlocksHelper.IsInBuilder())
                    control.Attributes.Add("rel", "0");

                control.InnerHtml = Content;
            }

            //add the control to place holder
            Controls.Add(control);
        }
    }
}