<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="UserPage.aspx.cs" Inherits="VF.RaidDamageWebsite.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
	<ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->
    
    <header class="page-header">  
        <div class="row">
          <div class="span12">
            <asp:Label ID="lblStatus" Text="Login using your UserID" Font-Size="Large" runat="server"></asp:Label>
            <br />
            <br />
            <asp:Panel ID="pnlLogin" runat="server">
                <asp:Label ID="lblUserID" Text="UserID:" Font-Size="Large" runat="server"></asp:Label>
                <asp:TextBox ID="txtUserID" style="margin-bottom:0px;" runat="server"></asp:TextBox>
                <asp:Button ID="btnLogin" class="btn" runat="server" Text="Login" OnClick="btnLogin_Click"/>
            </asp:Panel>
              
            <asp:Panel ID="pnlLogout" runat="server">
                <asp:Button ID="btnLogout" class="btn" runat="server" Text="Logout" OnClick="btnLogout_Click"/>
            </asp:Panel>
          </div>
        </div>
    </header>

</asp:Content>
