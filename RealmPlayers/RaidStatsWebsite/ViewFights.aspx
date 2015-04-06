<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="ViewFights.aspx.cs" Inherits="VF.RaidDamageWebsite.ViewFights" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="assets/js/jquery-1.10.2.min.js"></script>
    <script src='assets/js/charts/raphael-min.js'></script>
    <script src='assets/js/charts/popup.js'></script>
    <script src='assets/js/charts/chart.js?version=15'></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    
    <script>
        function navigateWithNewQuery(_QueryName, _QueryValue) {
            var querySearchStr = "&" + _QueryName + "=";
            var itemSetIndex = window.location.href.indexOf(querySearchStr);
            var nextAndSign = window.location.href.indexOf("&", itemSetIndex + 1)
            if(itemSetIndex == -1)
            {
                querySearchStr = "?" + _QueryName + "=";
                itemSetIndex = window.location.href.indexOf(querySearchStr);
                nextAndSign = window.location.href.indexOf("&", itemSetIndex + 1)
                if (itemSetIndex == -1) {
                    querySearchStr = "&" + _QueryName + "=";
                }
            }
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

    <script>
        var g_RaphaelBarsDrawer = null;
        var g_BarLeft = 10;
        var g_BarTop = 10;
        var g_BarHeight = 15;
        var g_BarMargin = 5;
        var g_BarMaxWidth = 680;
        var g_BarTextSize = 12;
        var g_Bars = new Array();
        function VF_CreateDmgBar(_WidthPercentage, _Color, _LeftText, _TextColor, _RightText) {
            if (g_RaphaelBarsDrawer == null) {
                g_RaphaelBarsDrawer = Raphael('diagramDiv', 700, 20000);
            }
            var newBarTop = g_BarTop + g_Bars.length * (g_BarHeight + g_BarMargin);
            var newBar = g_RaphaelBarsDrawer.rect(g_BarLeft, newBarTop, g_BarMaxWidth * _WidthPercentage, g_BarHeight);
            newBar.attr({ 'fill': _Color, 'stroke': _Color, 'stroke-width': 0 });
            var leftText = null;
            if (_LeftText.search("<") != -1) {
                var queryName = null;
                var queryValue = null;
                queryName = _LeftText.split("<")[1];
                queryValue = queryName.split(",")[1].split(">")[0];
                queryName = queryName.split(",")[0];
                _LeftText = _LeftText.split(">")[1];

                leftText = g_RaphaelBarsDrawer.text(g_BarLeft + 5, newBarTop + g_BarMargin + (g_BarHeight / 2 - g_BarTextSize / 2) + 1, _LeftText).attr({
                    font: g_BarTextSize + 'px Arial',
                    fill: _TextColor,
                    cursor: 'pointer',
                    'text-anchor': 'start',
                }).click(function () {
                    navigateWithNewQuery(queryName, queryValue);
                });
            }
            else {
                leftText = g_RaphaelBarsDrawer.text(g_BarLeft + 5, newBarTop + g_BarMargin + (g_BarHeight / 2 - g_BarTextSize / 2) + 1, _LeftText).attr({
                    font: g_BarTextSize + 'px Arial',
                    fill: _TextColor,
                    'text-anchor': 'start',
                }).toFront();
            }
            var rightText = g_RaphaelBarsDrawer.text(g_BarLeft + g_BarMaxWidth * _WidthPercentage - 5, newBarTop + g_BarMargin + (g_BarHeight / 2 - g_BarTextSize / 2) + 1, _RightText).attr({
                font: g_BarTextSize + 'px Arial',
                fill: _TextColor,
                'text-anchor': 'end',
            }).toFront();
            g_Bars.push(newBar);
        }
        //function VF_AddTextToLastBar(_Text, _TextColor, _TextAnchor) {
        //    g_Bars[g_Bars.length-1].
        //    var title = g_RaphaelBarsDrawer.text(g_BarLeft + 5, newBarTop + g_BarMargin + (g_BarHeight / 2 - g_BarTextSize / 2) + 1, _Text).attr({
        //        font: g_BarTextSize + 'px Arial',
        //        fill: _TextColor,
        //        'text-anchor': 'start',
        //    }).toFront();
        //}
    </script>
    <%= m_ChartSection %>
</asp:Content>
