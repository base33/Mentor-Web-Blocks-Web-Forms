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
using System.Web.UI.HtmlControls;
using WebBlocks.Model;
using WebBlocks.Model.Helpers;

namespace WebBlocks.usercontrols.webblocks
{
    public partial class container : BaseUserControl
    {
        protected Blocks blocks = new Blocks();

        public string Class = string.Empty;
        public bool UseContainerClass = true;

        public Blocks Blocks
        {
            get
            {
                return blocks;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //get the list of containers on this page - will be available if any blocks have been dragged onto the page
            List<Container> containerList = GetCurrentPageContainers();

            //check if the container list is not null
            if (containerList != null)
            {
                //get the current container associated with the current container which is being rendered
                Container currentContainer = (from c in containerList
                                              where c.Id == ID
                                              select c).FirstOrDefault();

                //if an associated container is found
                if (currentContainer != null)
                {
                    // loop through every block that has been predefined
                    // and apply a sort order to it or remove it if it
                    // has been marked for deletion.
                    foreach (Block predefinedBlock in 
                        currentContainer.Blocks.Where(b => b.Predefined))
                    {
                        block templateBlock = Blocks.FirstOrDefault(b => b.BlockId == predefinedBlock.NodeId);

                        if (templateBlock != null)
                        {
                            //if the predefined block is a page wysiwyg block
                            if (templateBlock.BlockType == BlockType.PageWysiwygBlock)
                            {
                                //store the content and sort order into the template blocks
                                templateBlock.Content = predefinedBlock.Content;
                            }
                            
                            //otherwise if the block has been deleted
                            if (predefinedBlock.Deleted)
                            {
                                //remove the block from the template to be rendered
                                Blocks.Remove(templateBlock);
                            }
                            else
                            {
                                //otherwise set the sort order
                                templateBlock.SortOrder = predefinedBlock.SortOrder;
                            }
                        }
                    }

                    //loop through all blocks dragged into it
                    foreach (Block currentBlock in currentContainer.Blocks.Where(b => b.Predefined == false && b.Deleted == false))
                    {
                        //create a block user control and add to the containers list of blocks to render
                        block blockUserControl = new block
                            {
                                BlockId = currentBlock.NodeId,
                                Content = currentBlock.Content,
                                BlockType = currentBlock.BlockType,
                                SortOrder = currentBlock.SortOrder,
                                IsPredefined = false
                            };
                        Blocks.Add(blockUserControl);
                    }
                }
            }

            //create the container div
            HtmlGenericControl container = new HtmlGenericControl("div");
            
            //set up and add the class and rel tag
            string containerClass = Class;

            if (UseContainerClass)
                containerClass += " container";

            container.Attributes.Add("class", containerClass);

            if (WebBlocksHelper.IsInBuilder())
                container.Attributes.Add("rel", this.ID);

            Blocks.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
            Blocks.ForEach(b => b.ParentContainer = container);

            //loop through each block added to the container
            foreach (block control in this.Blocks)
            {
                //add block usercontrol to list of controls to render
                container.Controls.Add(control);
            }

            //add the container div to the placeholder
            plcContent.Controls.Add(container);
        }

        
    }
}