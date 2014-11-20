<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InstanceControl.ascx.cs" Inherits="VF_RaidDamageWebsite.InstanceControl" %>

<style>
    .icclcbx .checkbox label {
        width:120px;
    }
</style>
<div class="servers icclcbx" style="min-width: 140px; max-width: 140px;"><h4>Instances</h4>
    <asp:CheckBoxList ID="cblInstance" runat="server" RepeatLayout="Flow" AutoPostBack="True" Width="160px" RepeatDirection="Horizontal" CssClass="checkbox">
        <asp:ListItem Text="All" Value="All"/>
        <asp:ListItem Text="Zul'Gurub" Value="ZG"/>
        <asp:ListItem Text="Ruins of Ahn'Qiraj" Value="RAQ"/>
        <asp:ListItem Text="Onyxia's Lair" Value="Ony"/>
        <asp:ListItem Text="Molten Core" Value="MC"/>
        <asp:ListItem Text="Blackwing Lair" Value="BWL"/>
        <asp:ListItem Text="Temple of Ahn'Qiraj" Value="TAQ"/>
        <asp:ListItem Text="Naxxramas" Value="Naxx"/>
    </asp:CheckBoxList>
</div>