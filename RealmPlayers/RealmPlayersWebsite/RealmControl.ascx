﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RealmControl.ascx.cs" Inherits="RealmPlayersServer.RealmControl" %>

<%--<form id="Form1" class="servers" runat="server">--%><div class="servers" style="min-width:240px;max-width:240px;"><h4>Realm</h4><div style="margin-left: 15px;">
    <asp:RadioButtonList ID="RealmControl_RadioList" runat="server" RepeatColumns="2" AutoPostBack="True" Width="240px" CssClass="radio">
        <asp:ListItem Text="Rebirth" Value="REB" />
        <asp:ListItem Text="Kronos" Value="KRO" />
        <asp:ListItem Text="Kronos II" Value="KR2" />
        <asp:ListItem Text="VanillaGaming" Value="VG" />
        <asp:ListItem Text="Elysium" Value="Ely"/>
        <asp:ListItem Text="Zeth'Kur" Value="ZeK"/>
        <asp:ListItem Text="Anathema" Value="Ana"/>
        <asp:ListItem Text="Darrowshire" Value="Dar" />
        <asp:ListItem Text="Nefarian(DE)" Value="NEF" />
        <asp:ListItem Text="NostalGeek(FR)" Value="NG" />
        <asp:ListItem Text="Nemesis" Value="NES" />
        <asp:ListItem Text="Nostralia" Value="NST" />
        <asp:ListItem Text="Elysium(Old)" Value="ELY" />
        <asp:ListItem Text="Warsong(Feenix)" Value="WS2" />
        <asp:ListItem Text="Archangel" Value="ArA" />
        <asp:ListItem Text="Wildhammer" Value="VWH" />
        <asp:ListItem Text="Stonetalon" Value="VST" />
        <asp:ListItem Text="ExcaliburTBC" Value="EXC" />
        <asp:ListItem Text="WarGate" Value="HG" />
        <asp:ListItem Text="Hellfire I" Value="HLF" />
        <asp:ListItem Text="Hellfire II" Value="HF2" />
        <asp:ListItem Text="WarsongTBC" Value="WBC" />
        <asp:ListItem Text="Valkyrie" Value="VAL" />
        <asp:ListItem Text="Emerald Dream" Value="ED" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Warsong" Value="WSG" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Al'Akir" Value="AlA" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
    </asp:RadioButtonList></div>
</div>
<%--</form>--%>