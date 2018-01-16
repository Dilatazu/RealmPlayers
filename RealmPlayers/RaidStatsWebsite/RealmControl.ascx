<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RealmControl.ascx.cs" Inherits="VF.RaidDamageWebsite.RealmControl" %>

<%--<form id="Form1" class="servers" runat="server">--%><div class="servers rcclcbx"><h4>Realm</h4>
    <asp:RadioButtonList ID="rblRealm" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="100px" RepeatDirection="Horizontal" CssClass="radio">
        <asp:ListItem Text="All" Value="All" />
        <asp:ListItem Text="Rebirth" Value="REB" />
        <asp:ListItem Text="Kronos" Value="KRO" />
        <asp:ListItem Text="Kronos II" Value="KR2" />
        <asp:ListItem Text="VanillaGaming" Value="VG" />
        <asp:ListItem Text="Lightbringer" Value="LB"/>
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
        <asp:ListItem Text="Outland" Value="OUT"/>
        <asp:ListItem Text="Medivh" Value="MDV"/>
        <asp:ListItem Text="Felmyst" Value="FLM"/>
        <asp:ListItem Text="Firemaw" Value="FMW"/>
        <asp:ListItem Text="Ares" Value="AR"/>
        <asp:ListItem Text="Nighthaven" Value="NH"/>
        <asp:ListItem Text="Valkyrie" Value="VAL" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Emerald Dream" Value="ED" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Warsong" Value="WSG" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
        <asp:ListItem Text="Al'Akir" Value="AlA" style="visibility:hidden; height:0px; line-height: 0px; overflow:hidden; display:block;"/>
    </asp:RadioButtonList>
</div>
<%--</form>--%>