<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="AdminInfo.aspx.cs" Inherits="RealmPlayersServer.AdminInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        
      </ul>

      <div class="row">
        <div class="span12">
            <%= m_InfoHTML %>
        </div>
      </div>
</asp:Content>
