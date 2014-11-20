<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="BossList.aspx.cs" Inherits="VF_RaidDamageWebsite.BossList" %>

<%@OutputCache Duration="600" VaryByParam="*" %>

<%@ Register Src="RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>
<%@ Register Src="ClassControl.ascx" TagPrefix="uc1" TagName="ClassControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->
    
    <header class="page-header">  
        <div class="row">
          <div class="span8">
              <%= m_BossListInfoHTML %>
          </div>
          <div class="span4" style="min-width:200px;">
              <div style="margin: 0px 0px 0px 10px; float:right; ">
                    <uc1:RealmControl runat="server" ID="RealmControl" />
                </div>
              <div style="margin: 0px 0px 0px 0px; float:right; ">
                    <uc1:ClassControl runat="server" ID="ClassControl" />
                </div>
          </div>
        </div>
    </header>

    <table id="characters-table" class="table">
        <thead>
            <%= m_TableHeadHTML %>
        </thead>
        <tbody>
            <%= m_TableBodyHTML %>
        </tbody>
    </table>
</asp:Content>
