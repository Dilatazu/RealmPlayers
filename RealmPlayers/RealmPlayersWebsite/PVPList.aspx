<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="PVPList.aspx.cs" Inherits="RealmPlayersServer.PVPList" %>

<%@ Register Src="~/RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span10">
            <%= m_CharListInfoHTML %>
          </div>
          <div class="span2">
              <uc1:RealmControl runat="server" ID="RealmControl" />
          </div>
        </div>
    </header>
    
    <%= m_PaginationHTML %>

    <%= m_PageHTML %>

    <%= m_PaginationHTML %>
</asp:Content>
