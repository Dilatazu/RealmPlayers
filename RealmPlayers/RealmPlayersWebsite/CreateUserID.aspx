<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="CreateUserID.aspx.cs" Inherits="RealmPlayersServer.CreateUserID" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <asp:Label ID="txtStatus" runat="server" Text=""></asp:Label><br />
    <asp:TextBox ID="txtCreateUserID" runat="server"></asp:TextBox><asp:Button ID="btnCreateUserID" runat="server" Text="Create UserID" OnClick="btnCreateUserID_Click" />
    <%= Generate.Mvc(m_InfoHTML) %>
</asp:Content>
