<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="ItemUsageInfo.aspx.cs" Inherits="RealmPlayersServer.ItemUsageInfo" %>

<%@ Register Src="~/RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>

<%@OutputCache Duration="600" VaryByParam="*" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->

    <div class="row">
        <div class="span9">
            <%= m_ItemUsageInfoHTML %>
        </div>
        <div class="span3">
            <uc1:RealmControl runat="server" ID="RealmControl" />
        </div>
    </div>
    
    <%= m_PaginationHTML %>

    <table id="characters-table" class="table">
        <thead>
            <%= m_TableHeadHTML %>
        </thead>
        <tbody>
            <%= m_TableBodyHTML %>
        </tbody>
    </table>

    <%= m_PaginationHTML %>

</asp:Content>
