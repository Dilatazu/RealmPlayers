<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="OnlineStats.aspx.cs" Inherits="RealmPlayersServer.OnlineStats" %>

<%@ Register Src="~/RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="assets/js/jquery-1.10.2.min.js"></script>
    <script src='assets/js/charts/raphael-min.js'></script>
    <script src='assets/js/charts/popup.js'></script>
    <%= RealmPlayersServer.PageUtilityExtension.HTMLGetSiteVersion("assets/js/charts/chart.js") %>
    <script src='assets/js/charts/chart.js?version=18'></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span9">
              <%= m_PageInfoHTML %>
          </div>
          <div class="span3">
              <uc1:RealmControl runat="server" ID="RealmControl" />
          </div>
        </div>
    </header>
    <div class="row">
          <div class="span12">
            <div class="fame">
                <%= m_BodyHTML %>
        </div>
        </div>
        </div>
</asp:Content>
