<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassControl.ascx.cs" Inherits="VF_RaidDamageWebsite.ClassControl" %>

<style>
    .ccclcbx .checkbox label {
        width:30px;
    }
</style>
<div class="servers ccclcbx" style="min-width: 140px; max-width: 140px;"><h4>Classes</h4><div style="margin-left: 15px;">
    <asp:CheckBoxList ID="cblClass" runat="server" RepeatColumns="2" AutoPostBack="True" Width="160px" RepeatDirection="Horizontal" CssClass="checkbox">
        <asp:ListItem Text="All" Value="All"/>
        <asp:ListItem Text="<font color='#c79c6e'>Warrior</font>" Value="Wr"/>
        <asp:ListItem Text="<font color='#ff7d0a'>Druid</font>" Value="Dr"/>
        <asp:ListItem Text="<font color='#fff569'>Rogue</font>" Value="Ro"/>
        <asp:ListItem Text="<font color='#ffffff'>Priest</font>" Value="Pr"/>
        <asp:ListItem Text="<font color='#abd473'>Hunter</font>" Value="Hu"/>
        <asp:ListItem Text="<font color='#69ccf0'>Mage</font>" Value="Ma"/>
        <asp:ListItem Text="<font color='#0070DE'>Shaman</font>" Value="Sh"/>
        <asp:ListItem Text="<font color='#9482ca'>Warlock</font>" Value="Wl"/>
        <asp:ListItem Text="<font color='#f58cba'>Paladin</font>" Value="Pa"/>
    </asp:CheckBoxList></div><h4>Faction</h4><div style="margin-left: 15px;">
    <asp:CheckBoxList ID="cblFaction" runat="server" RepeatColumns="2" AutoPostBack="True" Width="160px" RepeatDirection="Horizontal" CssClass="checkbox">
        <asp:ListItem Text="<font color='#f70002'>Horde</font>" Value="Ho"/>
        <asp:ListItem Text="<font color='#007df7'>Alliance</font>" Value="Al"/>
    </asp:CheckBoxList></div>
</div>