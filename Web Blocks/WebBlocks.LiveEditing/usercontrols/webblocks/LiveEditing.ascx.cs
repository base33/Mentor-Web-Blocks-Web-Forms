using System;
using System.Linq;
using System.Web.UI;
using WebBlocks.BusinessLogic.Helpers;
using WebBlocks.Model.Helpers;
using WebBlocks.BusinessLogic.Views.RenderingEngine;
using umbraco.BusinessLogic;
using umbraco.NodeFactory;
using System.Text;
using umbraco.cms.businesslogic.web;
using umbraco.presentation;
using WebBlocks.LiveEditing.Helper;
using WebBlocks.usercontrols.webblocks.datatypes;
using WebBlocks.usercontrols.webblocks.datatypes.LayoutBuilder;

namespace WebBlocks.LiveEditing.usercontrols.webblocks
{
    public partial class LiveEditing : System.Web.UI.UserControl
    {
        /// <summary>
        /// The current node.  This will be retreived using PreviewNode.GetNode(id) so that preview content is retrieved
        /// </summary>
        protected Node currentPreviewNode;

        /// <summary>
        /// Predicate if in live edit mode
        /// </summary>
        protected bool liveEditMode = false;

        /// <summary>
        /// The current user permissions (update, publish, send to publish etc)
        /// </summary>
        protected string userPermissions = "";

        /// <summary>
        /// Shown in the tiny init markup in the front end and specifies the filename of the selected css
        /// </summary>
        protected string SelectedRichTextCss;

        /// <summary>
        /// Shown in the tiny init markup in the front end and specifies available styles from the rich text css files
        /// </summary>
        protected string SelectedRichTextStyles;

        /// <summary>
        /// An accessor class to get the current page builders prevalue editor settings
        /// </summary>
        protected WebBlocksPreValuesAccessor PreValueAccessor;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //if live edit mode is enabled and the user is logged in
            if (Request.QueryString["liveedit"] != null && UmbracoContext.Current.UmbracoUser != null)
            {
                LoadPreValueAccessor();
                currentPreviewNode = PreviewNode.GetNode(Node.getCurrentNodeId());
                CacheHelper<bool>.Add("wbIsInBuilder", true);
                liveEditMode = true;
                userPermissions = UmbracoContext.Current.UmbracoUser.GetPermissions(Node.GetCurrent().Path);
                SetRichTextStyles();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if live edit mode is enabled
            if (liveEditMode && UserPermissions.AllowedToUpdate(userPermissions))
            {
                //hide the not live edit mode panel (that will contain the edit button)
                pnlNotLiveEditMode.Visible = false;
                //show the live edit mode panel that will show the code necessary for live editing
                pnlLiveEditMode.Visible = true;
                //display the context menu
                plcContextMenu.Visible = true;
                //should we show the save button
                btnSave.Visible = true;
                btnSavePublish.Visible = UserPermissions.AllowedToPublish(userPermissions);
                btnSendToPublish.Visible = UserPermissions.AllowedToSendToPublish(userPermissions);
                lnkExitLiveEdit.Visible = true;
                if(string.IsNullOrEmpty(lnkExitLiveEdit.Attributes["href"]))
                    lnkExitLiveEdit.Attributes.Add("href", Request.Path);
                txtHiddenLayout.Text = txtHiddenLayout.Text != "" ?
                    txtHiddenLayout.Text : currentPreviewNode.GetProperty("builder").Value;
                CacheHelper<string>.Add("builderJson", txtHiddenLayout.Text);
            }
            else
            {
                pnlLiveEditMode.Visible = false;
                pnlNotLiveEditMode.Visible = UmbracoContext.Current.UmbracoUser != null;
                btnSave.Visible = false;
                btnSavePublish.Visible = false;
                lnkExitLiveEdit.Visible = false;
                btnSendToPublish.Visible = false;
                plcContextMenu.Visible = false;
            }
        }

        protected override void  OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (liveEditMode)
            {
                RazorRenderingEngine renderingEngine = new RazorRenderingEngine();
                renderingEngine.CurrentNode = Node.GetCurrent();
                renderingEngine.Macro.ScriptName = "WebBlocksFrontendScriptIncludes.cshtml";
                renderingEngine.Macro.ScriptLanguage = "cshtml";
                renderingEngine.Macro.ScriptCode = "";
                frontEndScriptInclude.InnerHtml = renderingEngine.Render();
                frontEndScriptInclude.InnerHtml += RenderScripts();
                
                //register variables
                string variables = "<script type='text/javascript'>";
                variables += string.Format("var wbCurrentNodeId = '{0}';", currentPreviewNode.Id);
                variables += string.Format("var wbBlockSourceNodeId = '{0}';", 1078);
                variables += string.Format("var wbServiceUrl = '/base/webblocks';");
                variables += string.Format("var txtHiddenLayoutClientId = '{0}';", txtHiddenLayout.ClientID);
                variables += string.Format("var plcCotnfextMenuClientId = '{0}';", plcContextMenu.ClientID);
                variables += "</script>";
                scriptVariables.InnerHtml = variables;

                StringBuilder sb = new StringBuilder();
                foreach (Node child in new Node(PreValueAccessor.BlockSourceNodeId).Children)
                {
                    RenderAllBlocksInList(child, sb);
                }

                plcBlocksContextMenu.Controls.Add(new LiteralControl(sb.ToString()));
                plcBlocksContextMenu2.Controls.Add(new LiteralControl(sb.ToString()));
            }
        }

        /// <summary>
        /// Renders the required scripts/css based on whether the users device is desktop or mobile/tablet
        /// </summary>
        /// <returns></returns>
        protected string RenderScripts()
        {
            string scripts;

            if (!MobileHelper.IsMobile(Request))
            {
                scripts = @"<script type='text/javascript' src='/umbraco/plugins/webblocks/scripts/jquery.jeegoocontext.liveedit.js'></script>
                            <script type='text/javascript' src='/umbraco/plugins/webblocks/scripts/webblocks.liveedit.js'></script>";
            }
            else
            {
                scripts = @"<script type='text/javascript' src='/umbraco/plugins/webblocks/scripts/jquery.jeegoocontext.liveedit.js'></script>
                            <script type='text/javascript' src='/umbraco/plugins/webblocks/scripts/webblocks.liveedit.mobile.js'></script>
                            <script type='text/javascript' src='/umbraco/plugins/webblocks/scripts/jgestures.min.js'></script>
                            <script type='text/javascript' src='https://raw.github.com/furf/jquery-ui-touch-punch/master/jquery.ui.touch-punch.min.js'></script>";
            }

            return scripts;
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (User.GetCurrent() == null) return;
            txtHiddenLayout.Text = HtmlHelper.ResizeImages(txtHiddenLayout.Text);
            Document document = new Document(Node.getCurrentNodeId());
            document.getProperty("builder").Value = txtHiddenLayout.Text;
            document.Save();
            WriteAlertToResponse("Update saved!", true);
        }

        protected void btnSavePublish_Click(object sender, EventArgs e)
        {
            //var service = ApplicationContext.Current.Services.ContentService;
            //var node = service.GetById(Node.getCurrentNodeId());
            //node.SetValue("builder", txtHiddenLayout.Text);
            //service.SaveAndPublish(node, User.GetCurrent().Id);
            if (User.GetCurrent() == null) return;
            txtHiddenLayout.Text = HtmlHelper.ResizeImages(txtHiddenLayout.Text);
            Document document = new Document(Node.getCurrentNodeId());
            document.getProperty("builder").Value = txtHiddenLayout.Text;
            document.Publish(UmbracoContext.Current.UmbracoUser);
            umbraco.library.UpdateDocumentCache(document.Id);
            WriteAlertToResponse("Updates published!", true);
        }

        protected void btnSendToPublish_Click(object sender, EventArgs e)
        {
            if (User.GetCurrent() == null) return;
            txtHiddenLayout.Text = HtmlHelper.ResizeImages(txtHiddenLayout.Text);
            Document document = new Document(Node.getCurrentNodeId());
            document.getProperty("builder").Value = txtHiddenLayout.Text;
            document.Save();
            document.SendToPublication(UmbracoContext.Current.UmbracoUser);
            WriteAlertToResponse("Update sent to publication", true);
        }

        /// <summary>
        /// Temporary notification for confirmation of events
        /// </summary>
        /// <param name="message"></param>
        protected void WriteAlertToResponse(string message, bool success)
        {
            //Response.Write("<script type='text/javascript'>alert('" + message + "');");
        }

        /// <summary>
        /// Gets the rich text editor styles set in prevalue editor of the pages builder
        /// </summary>
        protected void SetRichTextStyles()
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
        /// Retrieves the prevalue editor accessor for the builder datatype
        /// </summary>
        /// <returns></returns>
        protected void LoadPreValueAccessor()
        {
            Document currentDoc = new Document(Node.getCurrentNodeId());
            WebBlocksDataType dataType = (WebBlocksDataType)currentDoc.getProperty("builder").PropertyType.DataTypeDefinition.DataType;
            PreValueAccessor = new WebBlocksPreValuesAccessor((WebBlocksPrevalueEditor)dataType.PrevalueEditor);
        }
    }
}

