<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="GuildList.aspx.cs" Inherits="RealmPlayersServer.GuildList" %>

<%@ Register Src="~/RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <%= RealmPlayersServer.PageUtilityExtension.HTMLAddResources("assets/css/bootstrap-tooltip.css", "assets/js/guildprogress.js") %>
    <%--<link href="assets/css/bootstrap-tooltip.css" rel="stylesheet"/>--%>
    <%--<script src="assets/js/jquery-1.10.2.min.js"></script>--%>
    <%--<script src="assets/js/jquery-cookie.js"></script>--%>
    <%--<script src="assets/js/bootstrap.js"></script>
    <script src="assets/js/default.js"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span10">
              <%= m_GuildListInfoHTML %>
          </div>
          <div class="span2">
              <uc1:RealmControl runat="server" ID="RealmControl" />
          </div>
        </div>
    </header>
    
        <%--<div class="row"><div class="span12">
            <div class="characterstats" style="min-width:600px;">--%>
    <%= m_PaginationHTML %>
    <table id="guild-table" class="table">
      <thead>
        <%= m_TableHeadHTML %>
      </thead>
      <tbody>
        <%= m_TableBodyHTML %>
      </tbody>
    </table>
    <%= m_PaginationHTML %>
        <%--</div></div></div>--%>
    
    
    <%= m_GuildScriptData %>
    <script>
        for (key in guildProgress) {
            if (guildProgress.hasOwnProperty(key)) {
                var progressString = generateProgress(key, "MC")
                    + generateProgress(key, "Ony")
                    + generateProgress(key, "BWL");

                var gPWidth = 3;
                if (g_AQReleased == true) {
                    progressString = generateProgress(key, "ZG")
                        + generateProgress(key, "AQ20")
                        + progressString
                        + generateProgress(key, "AQ40");
                    gPWidth = gPWidth + 3;
                }
                else {
                    progressString = generateProgress(key, "ZG") + progressString;
                    gPWidth = gPWidth + 1;
                }
                
                if (g_NaxxReleased == true) {
                    progressString = progressString + generateProgress(key, "Naxx");
                    gPWidth = gPWidth + 1;
                }

                progressString = progressString + generateProgress(key, "WB");
                gPWidth = gPWidth + 1;
                document.getElementById(key + "-Progress").innerHTML = "<div style='width: " + (gPWidth * 35) + "px; height: 40px;'>" + progressString + "</div>";
            }
        }

        jQuery(function ($) {;
            for (key in guildProgress) {
                if (guildProgress.hasOwnProperty(key)) {
                    createProgressTooltip($, key, "MC");
                    createProgressTooltip($, key, "Ony");
                    createProgressTooltip($, key, "BWL");
                    createProgressTooltip($, key, "ZG");
                    if (g_AQReleased == true) {
                        createProgressTooltip($, key, "AQ20");
                        createProgressTooltip($, key, "AQ40");
                        if (g_NaxxReleased == true) {
                            createProgressTooltip($, key, "Naxx");
                        }
                    }
                    createProgressTooltip($, key, "WB");
                }
            }
        });
    </script>  
</asp:Content>
