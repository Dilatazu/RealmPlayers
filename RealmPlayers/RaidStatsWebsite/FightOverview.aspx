<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="FightOverview.aspx.cs" Inherits="VF_RaidDamageWebsite.FightOverview" %>

<%@OutputCache Duration="600" VaryByParam="*" %>

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
    
        <div class="row">
          <div class="span12">
            <div class="fame">
                <%= m_FightOverviewInfoHTML %>
            </div>
          </div>
          </div>
</asp:Content>
