<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="DungeonOverview.aspx.cs" Inherits="VF.RaidDamageWebsite.DungeonOverview" %>

<%@OutputCache Duration="60" VaryByParam="*" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->
    
    <header class="page-header">  
        <div class="row">
          <div class="span10">
              <%= new System.Web.Mvc.MvcHtmlString(m_RaidOverviewInfoHTML) %>
          </div>
          <div class="span2">
          </div>
        </div>
    </header>
    
    <div class="row">
        <div class="span12">
            <table id="characters-table" class="table">
                <thead>
                    <%= m_TableHeadHTML %>
                </thead>
                <tbody>
                    <%= m_TableBodyHTML %>
                </tbody>
            </table>
        </div>
    </div>
    <%= m_TrashHTML %>
    <div class="row">
        <div class="span12">
            <%= m_GraphSection %>
        </div>
    </div>
</asp:Content>
