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
using umbraco.NodeFactory;
using umbraco.presentation;
using WebBlocks.Model.Helpers;

namespace WebBlocks.Model
{
    [Serializable]
    public class Block
    {
        protected int nodeId = 0;
        protected int sortOrder = 0;
        protected string blockType = string.Empty;
        protected string content = string.Empty;
        protected bool predefined = true;
        protected bool deleted = false;
        protected string extraClass = string.Empty;

        public int NodeId { get { return nodeId; } set { nodeId = value; } }
        public int SortOrder { get { return sortOrder; } set { sortOrder = value; } }
        public string BlockType { get { return blockType; } set { blockType = value; } }
        public string Content { get { return content; } set { content = value; } }
        public bool Predefined { get { return predefined; } set { predefined = value; } }
        public bool Deleted { get { return deleted; } set { deleted = value; } }
        public string Class { get { return extraClass; } set { extraClass = value; } }

        [NonSerialized]
        public Node Node = null;

        public Block() { }

        public Block(int nodeId)
        {
            if(!UmbracoContext.Current.InPreviewMode && !WebBlocksHelper.IsInBuilder())
                Node = new Node(nodeId);
            else
            {
                Node = PreviewNode.GetNode(nodeId);
            }

            if(Node == null || (Node.Parent != null && Node.Parent.Id == -20))
                return;

            content = (Node.GetProperty("content") != null
                            && Node.GetProperty("content").Value != null
                            && !string.IsNullOrEmpty(Node.GetProperty("content").Value))
                            ? Node.GetProperty("content").Value : "";
            blockType = Node.NodeTypeAlias;
            this.nodeId = Node.Id;
            extraClass = (Node.GetProperty("classes") != null) ? Node.GetProperty("classes").Value : string.Empty;
        }
    }
}
