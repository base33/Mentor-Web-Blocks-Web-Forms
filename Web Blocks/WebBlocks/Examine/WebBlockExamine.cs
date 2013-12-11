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
using System.Collections.Generic;
using System.Linq;
using Examine;
using umbraco.BusinessLogic;
using umbraco.NodeFactory;
using WebBlocks.BusinessLogic.Helpers;
using WebBlocks.Model;

namespace WebBlocks.Examine
{
    public class WebBlockExamine : ApplicationBase
    {
        public WebBlockExamine()
        {
            foreach(var indexerSite in ExamineManager.Instance.IndexProviderCollection.ToList())
            {
                indexerSite.GatheringNodeData += SetBuilderWysiwygField;
            }
        }

        /// <summary>
        /// Indexes all wysiwyg blocks in the page containers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SetBuilderWysiwygField(object sender, IndexingNodeDataEventArgs e)
        {
            //get the current node id
            int nodeId;

            if (int.TryParse(e.Fields["id"], out nodeId))
            {
                Node node = new Node(nodeId);

                var layoutProperty = node.GetProperty("builder");

                if (layoutProperty != null && layoutProperty.Value != null)
                {

                    string wysiwyg = "";

                    List<Container> containers = layoutProperty.Value.FromJson<List<Container>>();
                    foreach (Container container in containers)
                    {
                        Block block = container.Blocks.FirstOrDefault(b => b.BlockType == "" && b.NodeId == 0);
                        string blockContent = "";

                        if (block != null)
                        {
                            blockContent = block.Content;
                        }

                        e.Fields.Add("WBContainer_" + container.Id, blockContent);
                        wysiwyg += " " + block;
                    }
                    e.Fields.Add("WBWysiwyg", wysiwyg);
                }
            }
        }
    }
}