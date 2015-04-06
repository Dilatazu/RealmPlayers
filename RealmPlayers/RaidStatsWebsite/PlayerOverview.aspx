<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="PlayerOverview.aspx.cs" Inherits="VF.RaidDamageWebsite.PlayerOverview" %>

<%@OutputCache Duration="600" VaryByParam="*" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->
    <div class="row">
        <div class="span12">
            <div class="fame">
                <%= m_PageHTML %>
            </div>
        </div>
    </div>
</asp:Content>
