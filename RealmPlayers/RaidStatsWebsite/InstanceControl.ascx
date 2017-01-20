<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InstanceControl.ascx.cs" Inherits="VF.RaidDamageWebsite.InstanceControl" %>

<style>
    .icclcbx .checkbox label {
        width:130px;
    }
</style>
<div class="servers icclcbx" style="min-width: 150px; max-width: 150px;"><h4>Instances</h4>
    <asp:CheckBoxList ID="cblInstance" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="160px" RepeatDirection="Horizontal" CssClass="checkbox">
        <asp:ListItem Text="All" Value="All"/>
        <asp:ListItem Text="Zul'Gurub" Value="ZG"/>
        <asp:ListItem Text="Ruins of Ahn'Qiraj" Value="RAQ"/>
        <asp:ListItem Text="Onyxia's Lair" Value="Ony"/>
        <asp:ListItem Text="Molten Core" Value="MC"/>
        <asp:ListItem Text="Blackwing Lair" Value="BWL"/>
        <asp:ListItem Text="Temple of Ahn'Qiraj" Value="TAQ"/>
        <asp:ListItem Text="Naxxramas" Value="Naxx"/>
        
        <asp:ListItem Text="Karazhan" Value="KZ"/>
        <asp:ListItem Text="Zul'Aman" Value="ZA"/>
        <asp:ListItem Text="Magtheridon's Lair" Value="ML"/>
        <asp:ListItem Text="Gruul's Lair" Value="GL"/>
        <asp:ListItem Text="Serpentshrine Cavern" Value="SSC"/>
        <asp:ListItem Text="Tempest Keep" Value="TK"/>
        <asp:ListItem Text="Black Temple" Value="BT"/>
        <asp:ListItem Text="Hyjal Summit" Value="MH"/>
        <asp:ListItem Text="Sunwell Plateau" Value="SWP"/>
    </asp:CheckBoxList>
</div>