<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactUs.ascx.cs" Inherits="WebBlocks.Runway.usercontrols.ContactUs" %>

<asp:PlaceHolder runat="server" ID="plcSend">
    <div><asp:Label ID="lblName" runat="server" Text="Name: "></asp:Label><asp:TextBox runat="server" ID="txtName"></asp:TextBox></div>
    <div><asp:Label ID="lblEmail" runat="server" Text="Email: "></asp:Label><asp:TextBox runat="server" ID="txtEmail"></asp:TextBox></div>
    <div>
        <asp:Label ID="lblMessage" runat="server" Text="Message: "></asp:Label>
        <asp:TextBox runat="server" ID="txtMessage" TextMode="MultiLine"></asp:TextBox>
    </div>
    <div><asp:Button runat="server" ID="btnSend" Text="Submit" OnClick="btnSubmit_OnClick"/></div>
</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="plcSuccess">
    <div class="contactUsSuccess"><%= SuccessMessage %></div>
</asp:PlaceHolder>