<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="Changelog.aspx.cs" Inherits="RealmPlayersServer.Changelog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <li><a href="Index.aspx">Home</a> <span class="divider">/</span></li>
        <li class="active">Changelog</li>
      </ul><!--/.breadcrumb -->
    <header class="page-header"></header>
    <%--<header class="page-header">  --%>
        <div class="row">
          <div class="span8">
            <%= m_ChangelogTextHTML %>
          </div>
          <div class="span4">
              <div class="changelogrightside">
                <%= m_ChangelogListHTML %>
              </div>
          </div>
        </div>
    <%--</header>--%>
</asp:Content>
