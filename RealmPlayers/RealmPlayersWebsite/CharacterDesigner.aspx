<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="CharacterDesigner.aspx.cs" Inherits="RealmPlayersServer.CharacterDesigner" %>

<%@ Register Src="~/ModelViewerOptions.ascx" TagPrefix="uc1" TagName="ModelViewerOptions" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul>
    <!--/.breadcrumb -->

    <div class="row">
        <div class="span12" style="min-width:595px;">
            <%--<div style="min-height:612px; float:left; min-width: 555px; min-height:612px;">--%>
            <div class="blackframe" style="margin: 5px 5px 5px 5px; float:left; width: 555px; min-width: 555px; max-width:555px;">
                    <%= m_CharacterDesignerInfo %>
                <div class="inventory">
                    <%= m_InventoryInfoHTML %>
                    <div style="z-index: 10; position: absolute; margin: 546px 33px; width: 140px; height: 40px;">
                        <uc1:ModelViewerOptions runat="server" ID="ModelViewerOptions" />
                    </div>
                </div>
                <%= m_GearStatsHTML %>
                <%--<%= m_ExtraDataHTML %>--%>
            </div>
            <div class="blackframe" style="margin: 5px 5px 5px 5px; float:left; width: 225px; min-width:225px; max-width:225px;">
                <asp:Panel ID="CharacterDesigner_Panel" runat="server" DefaultButton="CharacterDesignerRefresh">
                <asp:Table ID="Table1" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:DropDownList style="width:100px; margin: 10px 2.5px 0px 2.5px; color:#45a3ff" ID="ddlRace" runat="server" onchange="this.style.color = this.options[this.selectedIndex].style.color;">
                                <asp:ListItem Text="Human" style="color:#45a3ff" Value="Human" Selected="true"/>
                                <asp:ListItem Text="Undead" style="color:#ff4546" Value="Undead"/>
                                <asp:ListItem Text="Dwarf" style="color:#45a3ff" Value="Dwarf"/>
                                <asp:ListItem Text="Orc" style="color:#ff4546" Value="Orc"/>
                                <asp:ListItem Text="Gnome" style="color:#45a3ff" Value="Gnome"/>
                                <asp:ListItem Text="Tauren" style="color:#ff4546" Value="Tauren"/>
                                <asp:ListItem Text="Night Elf" style="color:#45a3ff" Value="Night_Elf"/>
                                <asp:ListItem Text="Troll" style="color:#ff4546" Value="Troll"/>
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:DropDownList style="width:100px; margin: 10px 2.5px 0px 2.5px; color:#fff" ID="ddlSex" runat="server">
                                <asp:ListItem Text="Male" Value="Male" Selected="true"/>
                                <asp:ListItem Text="Female" Value="Female"/>
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:DropDownList style="width:100px; margin: 10px 2.5px 0px 2.5px; color:#c79c6e" ID="ddlClass" runat="server" onchange="this.style.color = this.options[this.selectedIndex].style.color;">
                                <asp:ListItem Text="Warrior" style="color:#c79c6e" Value="Warrior" Selected="true"/>
                                <asp:ListItem Text="Druid" style="color:#ff7d0a" Value="Druid"/>
                                <asp:ListItem Text="Rogue" style="color:#fff569" Value="Rogue"/>
                                <asp:ListItem Text="Priest" style="color:#ffffff" Value="Priest"/>
                                <asp:ListItem Text="Hunter" style="color:#abd473" Value="Hunter"/>
                                <asp:ListItem Text="Mage" style="color:#69ccf0" Value="Mage"/>
                                <asp:ListItem Text="Shaman" style="color:#0070DE" Value="Shaman"/>
                                <asp:ListItem Text="Warlock" style="color:#9482ca" Value="Warlock"/>
                                <asp:ListItem Text="Paladin" style="color:#f58cba" Value="Paladin"/>
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label1" runat="server" Text="Head Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtHeadSlot" runat="server" placeholder="Head Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label3" runat="server" Text="Neck Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtNeckSlot" runat="server" placeholder="Neck Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label2" runat="server" Text="Shoulder Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtShoulderSlot" runat="server" placeholder="Shoulder Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label4" runat="server" Text="Back Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtBackSlot" runat="server" placeholder="Back Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label5" runat="server" Text="Chest Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtChestSlot" runat="server" placeholder="Chest Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label6" runat="server" Text="Shirt Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtShirtSlot" runat="server" placeholder="Shirt Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label7" runat="server" Text="Tabard Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtTabardSlot" runat="server" placeholder="Tabard Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label8" runat="server" Text="Wrist Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtWristSlot" runat="server" placeholder="Wrist Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label9" runat="server" Text="Gloves Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtGlovesSlot" runat="server" placeholder="Gloves Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label10" runat="server" Text="Belt Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtBeltSlot" runat="server" placeholder="Belt Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label11" runat="server" Text="Legs Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtLegsSlot" runat="server" placeholder="Legs Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label12" runat="server" Text="Feet Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtFeetSlot" runat="server" placeholder="Feet Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label13" runat="server" Text="Ring Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtRing1Slot" runat="server" placeholder="Ring Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label14" runat="server" Text="Ring Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtRing2Slot" runat="server" placeholder="Ring Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label15" runat="server" Text="Trinket Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtTrinket1Slot" runat="server" placeholder="Trinket Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label16" runat="server" Text="Trinket Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtTrinket2Slot" runat="server" placeholder="Trinket Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label17" runat="server" Text="Mainhand Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtMainhandSlot" runat="server" placeholder="Mainhand Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label18" runat="server" Text="Offhand Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtOffhandSlot" runat="server" placeholder="Offhand Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell><asp:Label ID="Label19" runat="server" Text="Ranged Slot" Width="100"></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="txtRangedSlot" runat="server" placeholder="Ranged Slot" Width="110" Height="15" style="margin-top:7px"></asp:TextBox></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:Button ID="CharacterDesignerRefresh" runat="server" style="display:none" OnClick="CharacterDesignerRefresh_Click" />
                </asp:Panel>
            </div>
            <%--<div class="blackframe" style="margin: 5px 5px 5px 5px; float:right; width: 270px; min-width:270px; max-width:270px;">
                <table id="characters-table" class="table">
                    <thead>
                    <tr><th>Latest Items</th><th>Date</th></tr>
                    </thead>
                    <tbody>
                    <%= m_ReceivedItemsHTML %>
                    </tbody>
                </table>
            </div>--%>
    </div>
    </div>
</asp:Content>
