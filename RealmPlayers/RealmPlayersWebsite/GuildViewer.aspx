<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="GuildViewer.aspx.cs" Inherits="RealmPlayersServer.GuildViewer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <%= RealmPlayersServer.PageUtilityExtension.HTMLAddResources("assets/css/bootstrap-tooltip.css", "assets/js/guildprogress.js") %>
    <%--<link href="assets/css/bootstrap-tooltip.css?version=1.0" rel="stylesheet"/>--%>
    <%--<script src="assets/js/jquery-1.10.2.min.js"></script>
    <script src="assets/js/bootstrap.js"></script>
    <script src="assets/js/default.js"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
            <div class="span9">
                <%= m_GuildInfoHTML %>
            </div>
            <div class="span3">
                <div class="guildprogressframe" style="float:right; height:240px; min-height:240px; max-height:240px; width: 170px; min-width: 170px; max-width: 170px;">
                    <h4>Progress</h4>
                    <%= m_ProgressInfoHTML %>
                </div>
            </div>
        </div>
    </header>
    <% if (RealmPlayersServer.PageUtility.GetQueryString(Request, "view", "players") == "latestitems") { %>
        <div class="row"><div class="span4">
            <div class="characterstats" style="min-width:400px;">
                <div><h3>Latest Items</h3>
                <p>Shows the 100 latest items aquired by guild members. <br />Only shows items with a quality of epic or better.</p></div>
                <table id="recvitems-table" class="table">
                    <thead>
                        <tr><th>Latest Items</th><th>Player</th><th>Date</th></tr>
                    </thead>
                    <tbody>
                        <%= m_LatestItemsHTML %>
                    </tbody>
                </table>
                </div>
        </div></div>
    <% }
       else if (RealmPlayersServer.PageUtility.GetQueryString(Request, "view", "players") == "latestevents")
       { %>
        <div class="row"><div class="span4">
            <div class="characterstats" style="min-width:400px;">
                <div><h3>Latest Items</h3>
                <p>Shows the 100 latest items aquired by guild members. <br />Only shows items with a quality of epic or better.</p></div>
                <table id="Table3" class="table">
                    <thead>
                        <tr><th>Latest Items</th><th>Player</th><th>Date</th></tr>
                    </thead>
                    <tbody>
                        <%= m_LatestItemsHTML %>
                    </tbody>
                </table>
                </div>
        </div><div class="span1"></div><div class="span7">
            <div class="characterstats">
                <div><h3>Latest Member Changes</h3>
                <p>Shows the latest member changes that occured the last 14 days.</p></div>
                <table id="Table2" class="table">
                    <thead>
                        <tr><th>Player</th><th>Status Change</th><th>Date</th></tr>
                    </thead>
                    <tbody>
                        <%= m_LatestMembersHTML %>
                    </tbody>
                </table>
                </div>
        </div></div>
    <% } else if (RealmPlayersServer.PageUtility.GetQueryString(Request, "view", "players") == "latestmembers") { %>
        <div class="row"><div class="span4">
            <div class="characterstats" style="min-width:400px;">
                <div><h3>Latest Members</h3>
                <p>Shows the 40 latest members that have joined the guild. <br />Only shows members that are active and still in the guild.</p></div>
                <table id="Table1" class="table">
                    <thead>
                        <tr><th>Player</th><th>Status Change</th><th>Date</th></tr>
                    </thead>
                    <tbody>
                        <%= m_LatestMembersHTML %>
                    </tbody>
                </table>
                </div>
        </div></div>
    <% } else { %>
        <div class="row"><div class="span12">
            <div class="characterstats" style="min-width:600px;">
        <div><h3>Members</h3>
        <p>Only shows players that was seen less than 30 days ago</p></div>
        <table id="characters-table" class="table">
            <thead>
            <%= m_CharactersTableHeadHTML %>
            </thead>
            <tbody>
            <%= m_CharactersTableBodyHTML %>
            </tbody>
        </table>
        </div></div></div>
    <%} %>
    <%--<div class="pagination text-center">
      <ul>
        <li class="disabled"><a href="#">First</a></li>
        <li class="disabled"><a href="#">Prev</a></li>
        <li class="active"><a href="#">1</a></li>
        <li><a href="#">2</a></li>
        <li><a href="#">3</a></li>
        <li><a href="#">4</a></li>
        <li><a href="#">5</a></li>
        <li><a href="#">...</a></li>
        <li><a href="#">Next</a></li>
        <li><a href="#">Last</a></li>
      </ul>
    </div> <!-- /pagination -->--%>

    <%= m_GuildScriptData %>
    <script>
        for (key in guildProgress) {
            if (guildProgress.hasOwnProperty(key)) {
                document.getElementById(key + "-Progress").innerHTML = generateProgressGuildView(key, "ZG")
                    + generateProgressGuildView(key, "AQ20")
                    + generateProgressGuildView(key, "MC")
                    + generateProgressGuildView(key, "Ony")
                    + generateProgressGuildView(key, "BWL")
                    + generateProgressGuildView(key, "AQ40")
                    + generateProgressGuildView(key, "Naxx")
                    + generateProgressGuildView(key, "WB")
            }
        }

        jQuery(function ($) {;
            for (key in guildProgress) {
                if (guildProgress.hasOwnProperty(key)) {
                    createProgressTooltipGuildView($, key, "MC");
                    createProgressTooltipGuildView($, key, "Ony");
                    createProgressTooltipGuildView($, key, "BWL");
                    createProgressTooltipGuildView($, key, "ZG");
                    createProgressTooltipGuildView($, key, "AQ20");
                    createProgressTooltipGuildView($, key, "AQ40");
                    createProgressTooltipGuildView($, key, "Naxx");
                    createProgressTooltipGuildView($, key, "WB");
                }
            }
        });
    </script>
</asp:Content>
