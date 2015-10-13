<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RealmControl.ascx.cs" Inherits="RealmPlayersServer.RealmControl" %>

<%--<form id="Form1" class="servers" runat="server">--%><div class="servers" style="min-width:240px;max-width:240px;"><h4>Realm</h4><div style="margin-left: 15px;">
    <asp:RadioButtonList ID="RealmControl_RadioList" runat="server" RepeatColumns="2" AutoPostBack="True" Width="240px" CssClass="radio">
        <asp:ListItem Text="Emerald Dream" Value="ED" />
        <asp:ListItem Text="Warsong" Value="WSG" />
        <asp:ListItem Text="Al'Akir" Value="AlA" />
        <asp:ListItem Text="Nostalrius" Value="NRB" />
        <%--<asp:ListItem Text="NostalriusPVE" Value="NBE" />--%>
        <asp:ListItem Text="Kronos" Value="KRO" />
        <asp:ListItem Text="Archangel(TBC)" Value="ArA" />
        <asp:ListItem Text="Rebirth" Value="REB" />
        <asp:ListItem Text="VanillaGaming" Value="VG" />
        <asp:ListItem Text="Valkyrie" Value="VAL" />
        <asp:ListItem Text="Nefarian(DE)" Value="NEF" />
        <asp:ListItem Text="NostalGeek(FR)" Value="NG" />
    </asp:RadioButtonList></div>
</div>
<%--</form>--%>