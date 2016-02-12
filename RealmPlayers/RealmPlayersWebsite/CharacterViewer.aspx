<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true"
    CodeBehind="CharacterViewer.aspx.cs" Inherits="RealmPlayersServer.CharacterViewer" %>

<%@ Register Src="~/ModelViewerOptions.ascx" TagPrefix="uc1" TagName="ModelViewerOptions" %>

<%@OutputCache Duration="300" VaryByParam="*" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul>
    <!--/.breadcrumb -->

    <div class="row">
        <%= m_CharInfoHTML %>
        <div class="span2 fame">
            <h4>Navigate</h4>
            <%= Generate.Mvc(m_RaidStatsPlayerOverviewLink) %><br />
            <%= Generate.Mvc(m_CharacterDesignerLink) %><%--Character Designer--%>
        </div>
        <%--<div class="span2"></div>--%>
        <%--<div class="span2 fame">
          <h4 class="success">Fame</h4>
            <%= m_FameInfoHTML %>
        </div>--%>
    </div>
    <div class="row">
        <div class="span12" style="min-width:595px;">
            <%--<div style="min-height:612px; float:left; min-width: 555px; min-height:612px;">--%>
            <div class="blackframe" style="margin: 5px 5px 5px 5px; float:left; width: 555px; min-width: 555px; max-width:555px;">
                <div class="inventory">
                    <%= m_InventoryInfoHTML %>
                    <div style="z-index: 10; position: absolute; margin: 546px 33px; width: 140px; height: 40px;">
                        <uc1:ModelViewerOptions runat="server" ID="ModelViewerOptions" />
                    </div>
                    <div style="z-index: 10; position: absolute; margin: 551px 383px; width: 140px; height: 40px;">
                        <select style="width: 140px;" onchange="navigateWithNewQuery('itemset', this.options[this.selectedIndex].value)">
                            <%= m_ItemSetsDropdownOptions %>
                        </select>
                    </div>
                    <script>
                        function navigateWithNewQuery(_QueryName, _QueryValue) {
                            var querySearchStr = "&" + _QueryName + "=";
                            var itemSetIndex = window.location.href.indexOf(querySearchStr);
                            var nextAndSign = window.location.href.indexOf("&", itemSetIndex + 1)
                            if (itemSetIndex == -1) {
                                window.location.href = window.location.href + querySearchStr + _QueryValue;
                            }
                            else {
                                if (nextAndSign == -1)
                                    nextAndSign = window.location.href.length;
                                var replaceStr = window.location.href.substr(itemSetIndex, nextAndSign - itemSetIndex);
                                window.location.href = window.location.href.replace(replaceStr, querySearchStr + _QueryValue);
                            }
                        }
                    </script>
                </div>
                <%= m_GearStatsHTML %>
                <%= m_ExtraDataHTML %>
            </div>
            <div class="blackframe" style="margin: 5px 5px 5px 5px; float:left; width: 225px; min-width:225px; max-width:225px;">
                <%= m_StatsInfoHTML %>
            </div>
            <div class="blackframe" style="margin: 5px 5px 5px 5px; float:right; width: 270px; min-width:270px; max-width:270px;">
                <table id="characters-table" class="table">
                    <thead>
                    <tr><th>Latest Items</th><th>Date</th></tr>
                    </thead>
                    <tbody>
                    <%= m_ReceivedItemsHTML %>
                    </tbody>
                </table>
            </div>
            <div class="blackframe" style="margin: 5px 5px 5px 5px; float:left; width: 555px; min-width: 555px; max-width:555px;">
                <%= m_ChartSection %>
            </div>
            <%--<div class="span3">
                <%--<div class="characterstats" style="min-width:300px;">
                <table id="characters-table" class="table">
                  <thead>
                    <tr><th>Latest Items</th><th>Date</th></tr>
                  </thead>
                  <tbody>
                    <%= m_ReceivedItemsHTML %>
                  </tbody>
                </table>
                </div>--%>
            <%--</div>
            <div class="span4"></div>--%>
    </div>
    </div>
    <%--<div class="row">
        <div class="span12 fame">
          <h4 class="success">Fame</h4>
            <%= m_FameInfoHTML %>
        </div>
      </div>--%>
</asp:Content>
