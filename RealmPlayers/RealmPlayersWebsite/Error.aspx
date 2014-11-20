<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="RealmPlayersServer.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div class="row index">
        <div class="span12 text-center">
            <%= m_ErrorTextData %>
        </div>
    </div>
</asp:Content>
