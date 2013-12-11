<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Installer.ascx.cs" Inherits="WebBlocks.Install.Installer" %>

<div style="background: transparent url(/umbraco/plugins/WebBlocks/images/Logo.png) no-repeat right 8px; padding-top: 40px;">
    <div>
        <h2>Congratulations!  You've now installed Mentor Web Blocks</h2>
        <p>Wait! There's a few more things to do before you get going!</p>
    </div>
    <div>
        <p>Add the following lines to the web.config…</p>
        <div style="color:blue;font-family: Terminus,Consolas;font-size:11pt;">
            &lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="wb"  <span style="color:red">namespace</span>="WebBlocks.usercontrols" <span style="color:red">assembly</span>="WebBlocks" /&gt;<br/>
            &lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="wb" <span style="color:red">src</span>="~/usercontrols/webblocks/container.ascx" <span style="color:red">tagName</span>="container" /&gt;<br/>
            &lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="wb" <span style="color:red">src</span>="~/usercontrols/webblocks/block.ascx" <span style="color:red">tagName</span>="block" /&gt;<br/>
        </div>
        <p>
            ...add it to around line 80 as per this example. The bold text is the code to add.
        </p>
        <div style="color:blue;font-family: Terminus,Consolas;font-size:11pt;">
            &lt;<span style="color:rgb(139, 13, 13)">pages</span> <span style="color:red">enableEventValidation</span>="false"&gt;<br/>
              &nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">controls</span>&gt;<br/>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="asp" <span style="color:red">namespace</span>="System.Web.UI" <span style="color:red">assembly</span>="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" /&gt;<br/>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="umbraco" <span style="color:red">namespace</span>="umbraco.presentation.templateControls" <span style="color:red">assembly</span>="umbraco" /&gt;<br/>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="asp" <span style="color:red">namespace</span>="System.Web.UI.WebControls" <span style="color:red">assembly</span>="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" /&gt;<br/>
               <b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="wb"  <span style="color:red">namespace</span>="WebBlocks.usercontrols" <span style="color:red">assembly</span>="WebBlocks" /&gt;<br/>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="wb" <span style="color:red">src</span>="~/usercontrols/webblocks/container.ascx" <span style="color:red">tagName</span>="container" /&gt;<br/>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">tagPrefix</span>="wb" <span style="color:red">src</span>="~/usercontrols/webblocks/block.ascx" <span style="color:red">tagName</span>="block" /&gt;</b><br/>
              &nbsp;&nbsp;&nbsp;&lt;/<span style="color:rgb(139, 13, 13)">controls</span>&gt;<br/>
            &lt;/<span style="color:rgb(139, 13, 13)">pages</span>&gt;
        </div>
        <p>Just one more...</p>
        
        <div style="color:blue;font-family: Terminus,Consolas;font-size:11pt;">
            &lt;<span style="color:rgb(139, 13, 13)">add</span> namespace="WebBlocks.BusinessLogic.Helpers" /&gt;
        </div>
        <div>
            <p>...add it to around line 263 as per this example. Again, the bold text is the code to add.</p>
        </div>
        <div style="color:blue;font-family: Terminus,Consolas;font-size:11pt;">
            &lt;<span style="color:rgb(139, 13, 13)">system.web.webPages.razor</span>&gt;<br/>
                &nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">host</span> <span style="color:red">factoryType</span>="umbraco.MacroEngines.RazorUmbracoFactory, umbraco.MacroEngines" /&gt;<br/>
                &nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">pages</span> <span style="color:red">pageBaseType</span>="umbraco.MacroEngines.DynamicNodeContext"&gt;<br/>
                  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">namespaces</span>&gt;<br/>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">namespace</span>="Microsoft.Web.Helpers" /&gt;<br/>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">namespace</span>="umbraco" /&gt;<br/>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">namespace</span>="Examine" /&gt;<br/>
                    <b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span style="color:rgb(139, 13, 13)">add</span> <span style="color:red">namespace</span>="WebBlocks.BusinessLogic.Helpers" /&gt;</b><br/>
                  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/<span style="color:rgb(139, 13, 13)">namespaces</span>&gt;<br/>
                &nbsp;&nbsp;&nbsp;&lt;/<span style="color:rgb(139, 13, 13)">pages</span>&gt;<br/>
            &lt;/<span style="color:rgb(139, 13, 13)">system.web.webPages.razor</span>&gt;<br/>
        </div>

        
    </div>
</div>