<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BossesControl.ascx.cs" Inherits="VF.RaidDamageWebsite.BossesControl" %>

<style>
    .bcclcbx .checkbox label {
        width: 160px;
    }
    .bcclcbx a {
        color: mediumpurple;
        text-decoration: none;
    }
</style>
<script>
    //I know how shit all this code is, website coding part of RealmPlayers is poor due to not being my area of expertise and limited time.

    function Toggle_Expand_BossList(_BossList) {
        if (_BossList == 'specificbosses') {
            $('#specificbosses').slideDown();
            $('#mostviewed').slideUp();
        }
        else {
            $('#specificbosses').slideUp();
            $('#mostviewed').slideDown();
        }
    }
</script>
<div class="servers bcclcbx">
    <h4><a href='javascript:void(0)' onclick="Toggle_Expand_BossList('mostviewed');">Default lists</a></h4>
    <div class="row" id="mostviewed">
        <%= m_MostViewedBossesLists %>
    </div>
    <h4><a href='javascript:void(0)' onclick="Toggle_Expand_BossList('specificbosses');">View for specific bosses</a></h4>
    <div class="row" id="specificbosses" style="display:none;">
        <div class="span3" style="min-width: 180px; max-width: 180px;">
            <h5>Zul'Gurub</h5>
            <asp:CheckBoxList ID="cblBoss1" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="170px" RepeatDirection="Horizontal" CssClass="checkbox">
                <asp:ListItem Text="All" Value="0J0K0L0M0N0O0P0Q" />
            </asp:CheckBoxList>
            <h5>Ruins of Ahn'Qiraj</h5>
            <asp:CheckBoxList ID="cblBoss2" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="170px" RepeatDirection="Horizontal" CssClass="checkbox">
                <asp:ListItem Text="All" Value="0W0X0Y0Z1A1B" />
            </asp:CheckBoxList>
        </div>
        <div class="span3" style="min-width: 180px; max-width: 180px;">
            <h5>Molten Core</h5>
            <asp:CheckBoxList ID="cblBoss3" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="170px" RepeatDirection="Horizontal" CssClass="checkbox">
                <asp:ListItem Text="All" Value="00010203040506070809" />
            </asp:CheckBoxList>
            <h5>Blackwing Lair</h5>
            <asp:CheckBoxList ID="cblBoss4" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="170px" RepeatDirection="Horizontal" CssClass="checkbox">
                <asp:ListItem Text="All" Value="0B0C0D0E0F0G0H0I" />
            </asp:CheckBoxList>
        </div>
        <div class="span3" style="min-width: 180px; max-width: 180px;">
            <h5>Temple of Ahn'Qiraj</h5>
            <asp:CheckBoxList ID="cblBoss5" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="170px" RepeatDirection="Horizontal" CssClass="checkbox">
                <asp:ListItem Text="All" Value="1C1D1E1F1G1H1I1J1K" />
            </asp:CheckBoxList>
            <h5>Naxxramas</h5>
            <asp:CheckBoxList ID="cblBoss6" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="170px" RepeatDirection="Horizontal" CssClass="checkbox">
                <asp:ListItem Text="All" Value="1L1M1N1O1P1Q1R1S1T1U1V1W1X1Y1Z" />
            </asp:CheckBoxList>
        </div>
    </div>
</div>
