<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RealmControl.ascx.cs" Inherits="VF.RaidDamageWebsite.RealmControl" %>

<%--<form id="Form1" class="servers" runat="server">--%><div class="servers rcclcbx"><h4>Realm</h4>
    <asp:RadioButtonList ID="rblRealm" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="100px" RepeatDirection="Horizontal" CssClass="radio">
        <asp:ListItem Text="All" Value="All" />
        <asp:ListItem Text="Rebirth" Value="REB" />
        <asp:ListItem Text="Kronos" Value="KRO" />
        <asp:ListItem Text="Kronos II" Value="KR2" />
        <asp:ListItem Text="VanillaGaming" Value="VG" />
        <asp:ListItem Text="Elysium" Value="EL2" />
        <asp:ListItem Text="Zeth'Kur" Value="ZeK"/>
        <asp:ListItem Text="Nostalrius" Value="NRB" />
        <asp:ListItem Text="NostalriusPVE" Value="NBE" />
        <asp:ListItem Text="Nefarian(DE)" Value="NEF" />
        <asp:ListItem Text="NostalGeek(FR)" Value="NG" />
        <asp:ListItem Text="Elysium(Old)" Value="ELY" />
        <asp:ListItem Text="Warsong(Feenix)" Value="WS2" />
        <asp:ListItem Text="Archangel(TBC)" Value="ArA" />
        <asp:ListItem Text="Wildhammer(TBC)" Value="VWH" />
        <asp:ListItem Text="Stonetalon(TBC)" Value="VST" />
        <asp:ListItem Text="ExcaliburTBC" Value="EXC" />
        <asp:ListItem Text="Hellfire(TBC)" Value="HLF" />
        <asp:ListItem Text="Valkyrie" Value="VAL" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Emerald Dream" Value="ED" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Warsong" Value="WSG" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Al'Akir" Value="AlA" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
    </asp:RadioButtonList>
</div>
<%--</form>--%>