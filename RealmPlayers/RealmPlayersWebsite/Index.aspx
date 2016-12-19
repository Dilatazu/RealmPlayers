<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="RealmPlayersServer.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="assets/js/extras.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <script type="text/javascript">
        function VF_SetSearchType(searchType) {
            if (searchType == "Players") {
                $("#BodyContent_SearchBox").attr("placeholder", "Search a Player");
                $("#BodyContent_ddlFaction").hide();
                $("#BodyContent_ddlRace").show();
                $("#BodyContent_ddlClass").show();
                $("#BodyContent_ddlLevel").show();
            }
            else if (searchType == "Guilds") {
                $("#BodyContent_SearchBox").attr("placeholder", "Search a Guild");
                $("#BodyContent_ddlRace").hide();
                $("#BodyContent_ddlClass").hide();
                $("#BodyContent_ddlLevel").hide();
                $("#BodyContent_ddlFaction").show();
            }
        }
  </script>
    <div class="row index">
        <div class="span12 text-center">
            <h1><span class="text-center">RealmPlayers</span><%--<br /><span class='badge badge-inverse'>by Dilatazu</span>--%></h1>
            <div class="form-search text-center">
                <%--<input type="text" class="span6" placeholder="Search a Guild or Player"/>--%>
                <asp:Panel ID="SearchBox_Panel" runat="server" DefaultButton="SearchBox_Submit">
                    <div class="row">
                        <div class="span12">
                            <asp:TextBox ID="SearchBox" CssClass="span6" style="margin: 10px 2.5px 0px 5px" runat="server" placeholder="Press Enter to Search" />
                            <%--<asp:Button ID="SearchButton" CssClass="btn" style="margin: 10px 2.5px 0px 5px" runat="server" Text="Search!" OnClick="SearchBox_Submit_Click" />--%>
                            <%--<asp:DropDownList style="width:130px; margin: 10px 5px 0px 2.5px" ID="ddlSearchType" runat="server" onchange="VF_SetSearchType(this.options[this.selectedIndex].value);">
                                <asp:ListItem Text="Search Players" Value="Players" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Search Guilds" Value="Guilds"/>
                                <asp:ListItem Text="Items" Value="Items"/>
                            </asp:DropDownList>--%>
                        </div>
                    </div>
                    <div id="AdvancedOptions" class="row">
                        <div class="span12">
                            <asp:DropDownList style="width:140px; margin: 10px 2.5px 0px 2.5px" ID="ddlRealm" runat="server">
                                <asp:ListItem Text="Any Realm" Value="Any" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Rebirth" Value="REB"/>
                                <asp:ListItem Text="Kronos" Value="KRO"/>
                                <asp:ListItem Text="Kronos II" Value="KR2"/>
                                <asp:ListItem Text="VanillaGaming" Value="VG"/>
                                <asp:ListItem Text="Elysium" Value="ELY"/>
                                <asp:ListItem Text="Valkyrie" Value="VAL"/>
                                <asp:ListItem Text="Nostalrius" Value="NRB"/>
                                <asp:ListItem Text="NostalriusPVE" Value="NBE" />
                                <asp:ListItem Text="Nefarian(DE)" Value="NEF"/>
                                <asp:ListItem Text="NostalGeek(FR)" Value="NG"/>
                                <asp:ListItem Text="Warsong(Feenix)" Value="WS2"/>
                                <asp:ListItem Text="Archangel(TBC)" Value="ArA"/>
                                <asp:ListItem Text="Wildhammer(TBC)" Value="VWH" />
                                <asp:ListItem Text="Stonetalon(TBC)" Value="VST" />
                                <asp:ListItem Text="ExcaliburTBC" Value="EXC" />
                                <asp:ListItem Text="Hellfire(TBC)" Value="HLF" />
                                <asp:ListItem Text="WarsongTBC" Value="WBC"/>
                                <asp:ListItem Text="Emerald Dream" Value="ED"/>
                                <asp:ListItem Text="Warsong" Value="WSG"/>
                                <asp:ListItem Text="Al'Akir" Value="AlA"/>
                            </asp:DropDownList>
                            <asp:DropDownList style="width:100px; margin: 10px 2.5px 0px 2.5px" ID="ddlRace" runat="server" onchange="this.style.color = this.options[this.selectedIndex].style.color;">
                                <asp:ListItem Text="Any Race" style="color:#ffffff" Value="All" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Alliance" style="color:#45a3ff" Value="Alliance"/>
                                <asp:ListItem Text="Horde" style="color:#ff4546" Value="Horde"/>
                                <asp:ListItem Text="Human" style="color:#45a3ff" Value="Human"/>
                                <asp:ListItem Text="Undead" style="color:#ff4546" Value="Undead"/>
                                <asp:ListItem Text="Dwarf" style="color:#45a3ff" Value="Dwarf"/>
                                <asp:ListItem Text="Orc" style="color:#ff4546" Value="Orc"/>
                                <asp:ListItem Text="Gnome" style="color:#45a3ff" Value="Gnome"/>
                                <asp:ListItem Text="Tauren" style="color:#ff4546" Value="Tauren"/>
                                <asp:ListItem Text="Night Elf" style="color:#45a3ff" Value="NightElf"/>
                                <asp:ListItem Text="Troll" style="color:#ff4546" Value="Troll"/>
                            </asp:DropDownList>
                            <asp:DropDownList style="width:110px; margin: 10px 2.5px 0px 2.5px; display: none;" ID="ddlFaction" runat="server" onchange="this.style.color = this.options[this.selectedIndex].style.color;">
                                <asp:ListItem Text="Any Faction" style="color:#ffffff" Value="All" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Alliance" style="color:#45a3ff" Value="Alliance"/>
                                <asp:ListItem Text="Horde" style="color:#ff4546" Value="Horde"/>
                            </asp:DropDownList>
                            <asp:DropDownList style="width:100px; margin: 10px 2.5px 0px 2.5px" ID="ddlClass" runat="server" onchange="this.style.color = this.options[this.selectedIndex].style.color;">
                                <asp:ListItem Text="Any Class" style="color:#ffffff" Value="All" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Warrior" style="color:#c79c6e" Value="Warrior"/>
                                <asp:ListItem Text="Druid" style="color:#ff7d0a" Value="Druid"/>
                                <asp:ListItem Text="Rogue" style="color:#fff569" Value="Rogue"/>
                                <asp:ListItem Text="Priest" style="color:#ffffff" Value="Priest"/>
                                <asp:ListItem Text="Hunter" style="color:#abd473" Value="Hunter"/>
                                <asp:ListItem Text="Mage" style="color:#69ccf0" Value="Mage"/>
                                <asp:ListItem Text="Shaman" style="color:#0070DE" Value="Shaman"/>
                                <asp:ListItem Text="Warlock" style="color:#9482ca" Value="Warlock"/>
                                <asp:ListItem Text="Paladin" style="color:#f58cba" Value="Paladin"/>
                            </asp:DropDownList>
                            <asp:DropDownList style="width:100px; margin: 10px 2.5px 0px 2.5px" ID="ddlLevel" runat="server">
                                <asp:ListItem Text="Any Level" Value="All" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Lvl 10-19" Value="10to19"/>
                                <asp:ListItem Text="Lvl 20-29" Value="20to29"/>
                                <asp:ListItem Text="Lvl 30-39" Value="30to39"/>
                                <asp:ListItem Text="Lvl 40-49" Value="40to49"/>
                                <asp:ListItem Text="Lvl 50-59" Value="50to59"/>
                                <asp:ListItem Text="Lvl 60-60" Value="60to60"/>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:Button ID="SearchBox_Submit" runat="server" style="display:none" OnClick="SearchBox_Submit_Click" />
                </asp:Panel>
            </div>
            <br />
            <%= m_VisitorsHTML %>
            <br />
            <h5><span class='text-center'>Thanks to all the project supporters! (for full list <a href="Donators.aspx">click here</a>)</span></h5>
            <style>
                .table th,
                .table td {
                    text-align: center;
                }
                .table .left{
                    text-align: left;
                }
                .table .right{
                    text-align: right;
                }
            </style>
            <table class="table" style="width:300px; left: calc(50% - 150px); position: absolute;">
              <thead></thead>
              <tbody><%= m_DonationsHTML %></tbody>
            </table>
            <br/><br/>
        </div>
    </div>
   <%-- </div>--%>
    <%--<script>
        $("#BodyContent_SearchBox").after(
            '<div id="search_menu" style="padding-top: 10px; display: none;">'
                + '<select style="width: 100px; margin: 0px 10px 0px 0px">'
                    + '<option>-Class-</option>'
                    + '<option>Mage</option>'
                    + '<option>Warrior</option>'
                    + '<option>etc</option>'
                + '</select>'
                + '<select style="width: 100px; margin: 0px 10px 0px 0px">'
                    + '<option>-Race-</option>'
                    + '<option>Tauren</option>'
                    + '<option>Human</option>'
                    + '<option>etc</option>'
                + '</select>'
                + '<input type="text" placeholder="Level range..." style="width:100px">'
            + '</div>').focus(function () { $("#search_menu").slideDown(); });

    </script>--%>
</asp:Content>
