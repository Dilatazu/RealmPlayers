<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModelViewerOptions.ascx.cs" Inherits="RealmPlayersServer.ModelViewerOptions" %>

<div class="noMultilineChb">
    <asp:CheckBox ID="ModelViewerChb" runat="server" AutoPostBack="True" Text="View 3D Model" Width="140px"/>
    <asp:CheckBox ID="HideHeadChb" runat="server" AutoPostBack="True" Text="Hide Head" Width="140px"/>
</div>