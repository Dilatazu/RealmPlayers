<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="VF.RaidDamageWebsite.Log" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div class="row">
        <div class="span12 fame">
            <%= m_LogHTML %>
        </div>
      </div>
</asp:Content>
