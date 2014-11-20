<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="CharacterList.aspx.cs" Inherits="RealmPlayersServer.CharacterList" %>

<%@ Register Src="~/RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
      <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
      </ul><!--/.breadcrumb -->

      <div class="row">
        <div class="span10">
              <%= m_CharListInfoHTML %>
        </div>
        <%--<div class="span2">
            <uc1:RealmControl runat="server" ID="RealmControl" />
        </div>--%>
      </div>
    
    <%= m_PaginationHTML %>

    <%= m_GuildResultTableHTML %>

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
