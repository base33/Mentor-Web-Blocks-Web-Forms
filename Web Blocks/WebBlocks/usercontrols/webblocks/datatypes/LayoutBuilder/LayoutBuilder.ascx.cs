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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebBlocks.BusinessLogic.Helpers;
using umbraco.cms.businesslogic.template;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;
using WebBlocks.BusinessLogic.Extensions;
using WebBlocks.Model.Helpers;
using WebBlocks.usercontrols.webblocks;
using WebBlocks.usercontrols.webblocks.datatypes.LayoutBuilder;
using umbraco.editorControls.tinyMCE3;
using umbraco.editorControls.tinyMCE3.webcontrol;
using umbraco.editorControls.userControlGrapper;
using umbraco.interfaces;

namespace WebBlocks.usercontrols.data_types
{
    public partial class layout_builder : BaseUserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
    {
        public object value
        {
            get;
            set;
        }

        public WebBlocksPreValuesAccessor PreValueAccessor { get; set; }

        #region Front End Properties
        protected string SelectedRichTextCss = "";
        protected string SelectedRichTextStyles = "";
        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Literal lit = new Literal();
            string text = File.ReadAllText(HttpContext.Current.Server.MapPath("~/MacroScripts/WebBlocksBackendScriptIncludes.cshtml"));
            lit.Text = text;
            Page.Header.Controls.Add(lit);

            RenderCss();
            RenderJs();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();

            SetInBuilder();

            CacheHelper<string>.Add("pageID", Request.QueryString["id"]);
            
            LoadRichTextEditorStyles();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Page.IsPostBack)
            {
                txtHiddenLayout.Text = HtmlHelper.ResizeImages(txtHiddenLayout.Text);
                value = txtHiddenLayout.Text;
            }
            else
            {
                txtHiddenLayout.Text = value.ToString();
            }

            HttpContext.Current.Items["builderJson"] = txtHiddenLayout.Text;

            RegisterCurrentPageNode();

            Template template = null;
            
            if (currentNode != null)
            {
                if (currentNode.template > 0)
                {
                    template = new Template(currentNode.template);
                }
            }
            else
            {
                if (currentDocument.Template > 0)
                {
                    template = new Template(currentDocument.Template);
                }
            }

            string controlText = "";

            if (template != null)
            {
                controlText = template.GetLayoutContent("LayoutContent");
            }
            
            // Finally render and add the parsed template to the builder.
            // Seamless integration of .NET templates within the builder!! :)
            canvasRender.Controls.Add(Page.ParseControl(controlText));

            RenderAllBlocksInList();


        }

        /// <summary>
        /// Render css specific to this user control and append to the page header.
        /// </summary>
        protected void RenderCss()
        {
            AddCssToHead(basePath + "/css/webblocks.css");
            AddCssToHead(basePath + "/css/jquery-ui.custom.css");
            // Css to the context menu
            AddCssToHead(basePath + "/scripts/skins/cm_default/style.css");
        }

        /// <summary>
        /// Renders all blocks from the source so they can be displayed in a list for 
        /// use in a context menu for example.
        /// </summary>
        protected void RenderAllBlocksInList()
        {
            int blockSourceNodeId = PreValueAccessor.BlockSourceNodeId;

            try
            {
                // Pull in all blocks and folders from the blocks source Node
                Node blocksSourceNode = new Node(blockSourceNodeId);
                StringBuilder sb = new StringBuilder();

                // Loop through each children and output accordingly.
                foreach (Node child in blocksSourceNode.Children)
                {
                    RenderAllBlocksInList(child, sb);
                }

                plcBlocksContextMenu.Controls.Add(new LiteralControl(sb.ToString()));
                plcBlocksContextMenu2.Controls.Add(new LiteralControl(sb.ToString()));
            }
            catch (Exception)
            {

            }
            
        }

        protected void RenderAllBlocksInList(Node node, StringBuilder sb)
        {
            if (node.NodeTypeAlias == "BlocksFolder")
            {
                sb.Append("<li class='icon'>");
                sb.Append("<span class='icon folder'></span>");
                sb.AppendFormat("{0}", node.Name);
                sb.Append("<ul>");
            }
            else
            {
                sb.AppendFormat("<li class='icon blockItem' rel='{0}'>", node.Id);
                sb.Append("<span class='icon file'></span>");
                sb.AppendFormat("{0}", node.Name);
            }

            if (node.Children.Count > 0)
            {
                foreach (Node child in node.Children)
                {
                    RenderAllBlocksInList(child, sb);
                }
            }

            if (node.NodeTypeAlias == "BlocksFolder")
            {
                sb.Append("</ul>");
            }
        }

        /// <summary>
        /// Render javascript specific to this user control.
        /// </summary>
        protected void RenderJs()
        {
            // Define the name and type of the client scripts on the page
            const string scriptName = "layoutBuilderScript";
            Type csType = GetType();

            // Instantiate a string builder to form the basis of the script
            StringBuilder sbScript = new StringBuilder();

            // Get a ClientScriptManager reference from the page class
            ClientScriptManager cs = Page.ClientScript;

            bool hasWysiwyg = false;

            if (currentDocument != null && currentDocument.Id > 0)
            {
                if (currentDocument != null)
                {
                    if (currentDocument.GenericProperties.Any(p => p.Id == -87 || p.PropertyType.Name.Contains("Richtext editor")))
                    {
                        hasWysiwyg = true;
                    }
                }
            }
            
            if (!hasWysiwyg)
            {
                cs.RegisterClientScriptInclude("tinyMceCompress", "/umbraco/plugins/tinymce3/tinymce3tinymceCompress.aspx?themes=umbraco&plugins=contextmenu,umbracomacro,noneditable,inlinepopups,table,advlink,media,paste,spellchecker,umbracoimg,umbracocss&languages=en");
            }

            // Add client script includes
            cs.RegisterClientScriptInclude("json2", basePath + "/scripts/json2.js");
            cs.RegisterClientScriptInclude("base", basePath + "/scripts/base.js");
            cs.RegisterClientScriptInclude("webblocks", basePath + "/scripts/webblocks.js");
            cs.RegisterClientScriptInclude("jeegoocontext", basePath + "/scripts/jquery.jeegoocontext.js");

            sbScript.AppendFormat("var wbCurrentNodeId = '{0}';", currentDocument.Id);
            sbScript.AppendFormat("var wbBlockSourceNodeId = '{0}';", PreValueAccessor.BlockSourceNodeId);
            sbScript.Append("var wbServiceUrl = '/base/webblocks';");
            sbScript.AppendFormat("var txtHiddenLayoutClientId = '{0}';", txtHiddenLayout.ClientID);
            sbScript.AppendFormat("var plcCotnfextMenuClientId = '{0}';", plcContextMenu.ClientID);
            
            cs.RegisterStartupScript(csType, scriptName, sbScript.ToString(), true);
        }


        private void LoadRichTextEditorStyles()
        {
            string ids = PreValueAccessor.RichTextEditorStylesheet;

            if (ids != "")
            {
                string[] stylesheetIds = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var stylesheet in stylesheetIds.Select(s => new StyleSheet(Convert.ToInt32(s))))
                {
                    SelectedRichTextCss += string.Format("/css/{0}.css,", stylesheet.Text);
                
                    foreach (var style in stylesheet.Properties)
                    {
                        SelectedRichTextStyles += string.Format("{0}={1};", style.Text, style.Alias);
                    }
                }

                SelectedRichTextCss = SelectedRichTextCss.TrimEnd(',');
                SelectedRichTextStyles = SelectedRichTextStyles.TrimEnd(',');
            }
        }

        /// <summary>
        /// Registers the current page node so that 
        /// </summary>
        private void RegisterCurrentPageNode()
        {
            //the web blocks base class may have cached the current page node already
            if (WebBlocksHelper.IsInBuilder() && CacheHelper<Node>.Get("wbCurrentPageNode") == null)
            {
                int pageId = int.Parse(Request.QueryString["id"]);
                CacheHelper<Node>.Add("wbCurrentPageNode", PreviewNode.GetNode(pageId));
            }
        }

    }
}