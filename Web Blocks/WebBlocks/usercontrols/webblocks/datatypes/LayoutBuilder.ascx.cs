using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Hosting;
using System.IO;
using System.Text.RegularExpressions;
using WebBlocks.usercontrols.webblocks;
using umbraco.cms.businesslogic.template;
using WebBlocks.BusinessLogic;
using WebBlocks.Model;
using WebBlocks.BusinessLogic.Helpers;
using umbraco.NodeFactory;
using System.ComponentModel;
using umbraco.uicontrols;
using umbraco.editorControls.tinyMCE3;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.web;

namespace WebBlocks.usercontrols.data_types
{
    public partial class layout_builder : BaseUserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
    {
        private TinyMCE txtLongDesc;
        protected string layoutJson = "";

        public object value
        {
            get
            {
                var serializer = new JavaScriptSerializer();

                var containers = serializer.Deserialize<List<WebBlocks.Model.Container>>(layoutJson);

                foreach (var container in containers)
                {
                    foreach (var block in container.Blocks)
                    {
                        if (!string.IsNullOrEmpty(block.Content))
                        {
                            block.Content = TinyMCEImageHelper.CleanImages(block.Content);
                        }
                    }
                }

                return serializer.Serialize(containers);
            }
            set
            {
                if(value != null)
                    this.layoutJson = value.ToString();
            }
        }

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

        protected void Page_Init(object sender, EventArgs e)
        {
            // Set a developer flag for the layout builder
            SetInBuilder();

            HttpContext.Current.Items["pageID"] = Request.QueryString["id"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                this.value = txtHiddenLayout.Text;
                HttpContext.Current.Items["builderJson"] = txtHiddenLayout.Text;
            }
            else
            {
                txtHiddenLayout.Text = this.value.ToString();
            }

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
            canvasRender.Controls.Add(this.Page.ParseControl(controlText));

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
            // Pull in all blocks and folders from the blocks source Node
            Node blocksSourceNode = new Node(GlobalSettings.BlocksSourceNodeId);
            StringBuilder sb = new StringBuilder();

            // Loop through each children and output accordingly.
            foreach (Node child in blocksSourceNode.Children)
            {
                RenderAllBlocksInList(child, sb);
            }

            plcBlocksContextMenu.Controls.Add(new LiteralControl(sb.ToString()));
            plcBlocksContextMenu2.Controls.Add(new LiteralControl(sb.ToString()));
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
            string scriptName = "layoutBuilderScript";
            Type csType = this.GetType();

            // Instantiate a string builder to form the basis of the script
            StringBuilder sbScript = new StringBuilder();

            // Get a ClientScriptManager reference from the page class
            ClientScriptManager cs = Page.ClientScript;

            bool hasWysiwyg = false;

            if (currentNode != null && currentNode.Id > 0)
            {
                Document d = currentDocument;

                if (d != null)
                {
                    foreach (Property p in currentNode.Properties)
                    {
                        PropertyType pt = null;

                        if (d.getProperty(p.Alias) != null)
                        {
                            pt = d.getProperty(p.Alias).PropertyType;
                        }

                        if (pt != null && (pt.Id == -87 || pt.Name.Contains("Richtext editor")))
                        {
                            hasWysiwyg = true;
                            break;
                        }
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
            //cs.RegisterClientScriptInclude("jqueryContextMenu", basePath + "/scripts/jquery.contextMenu.js");
            //cs.RegisterClientScriptInclude("jqueryContextMenu", basePath + "/scripts/jquery.ui.position.js");

            sbScript.AppendFormat("var wbCurrentNodeId = '{0}';", currentDocument.Id);
            sbScript.AppendFormat("var wbServiceUrl = '{0}';", "/base/webblocks");
            sbScript.AppendFormat("var txtHiddenLayoutClientId = '{0}';", txtHiddenLayout.ClientID);
            sbScript.AppendFormat("var plcCotnfextMenuClientId = '{0}';", plcContextMenu.ClientID);
            
            cs.RegisterStartupScript(csType, scriptName, sbScript.ToString(), true);
        }
    }
}