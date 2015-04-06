<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="InstanceList.aspx.cs" Inherits="VF.RaidDamageWebsite.InstanceList" %>

<%@ Register Src="RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="assets/js/jquery-1.10.2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->
    
    <header class="page-header">  
        <div class="row">
          <div class="span10">
            <%= m_PageInfoHTML %>
          </div>
          <div class="span2">
              <uc1:RealmControl runat="server" ID="RealmControl" />
          </div>
        </div>
    </header>
    
    <%= m_GraphsHTML %>
</asp:Content>
