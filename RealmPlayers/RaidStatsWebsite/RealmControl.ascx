<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RealmControl.ascx.cs" Inherits="VF_RaidDamageWebsite.RealmControl" %>

<%--<form id="Form1" class="servers" runat="server">--%><div class="servers rcclcbx"><h4>Realm</h4>
    <asp:RadioButtonList ID="rblRealm" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="100px" RepeatDirection="Horizontal" CssClass="radio">
        <asp:ListItem Text="All" Value="All" />
        <asp:ListItem Text="Emerald Dream" Value="ED" />
        <asp:ListItem Text="Warsong" Value="WSG" />
        <asp:ListItem Text="Al'Akir" Value="AlA" />
        <asp:ListItem Text="Rebirth" Value="REB" />
    </asp:RadioButtonList>
</div>
<%--</form>--%>