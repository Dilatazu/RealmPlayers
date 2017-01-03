<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="Ranking.aspx.cs" Inherits="VF.RaidDamageWebsite.Ranking" %>

<%@OutputCache Duration="1000" VaryByParam="*" %>

<%@ Register Src="RealmControl.ascx" TagPrefix="uc1" TagName="RealmControl" %>
<%@ Register Src="ClassControl.ascx" TagPrefix="uc1" TagName="ClassControl" %>
<%@ Register Src="BossesControl.ascx" TagPrefix="uc1" TagName="BossesControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="assets/js/jquery-1.10.2.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <%= m_BreadCrumbHTML %>
    </ul><!--/.breadcrumb -->
    <header class="page-header">  
        <div class="row">
          <div class="span8" style="min-width:300px;">
              <div style="margin: 0px 0px 0px 0px; float:left; ">
              <uc1:BossesControl runat="server" ID="BossesControl" />
            </div>
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
    <%= m_InfoTextHTML %>
    <div class="row">
        <div class="span12">
            <div class="fame">
                <%= m_GraphSection %>
            </div>
        </div>
    </div>
</asp:Content>
