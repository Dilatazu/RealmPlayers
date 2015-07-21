<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="TalentCalculator.aspx.cs" Inherits="RealmPlayersServer.TalentCalculator" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
	
    <%--//https://github.com/mangostools/aowow
    //https://github.com/Sarjuuk/aowow
    //https://github.com/mangoszero
    // https://github.com/mangostools--%>

    <%= RealmPlayersServer.PageUtilityExtension.HTMLAddResources("assets/css/talentcalc.css", "assets/css/talent.css", "assets/js/TalentCalc_enus.js", "assets/js/TalentCalc.js", "assets/js/talent.js") %>

	<script type="text/javascript">
		g_locale = { id: 0, name: 'enus' };
		g_glyphs = [];
ss_conf = 3;	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <li><a href="Index.aspx">Home</a></li><li class="active"><span class="divider">/</span> Tools</li><li class="active"><span class="divider">/</span>Talent Calculator</li>
    </ul>
    <style> .clear {
         clear: both;
     }</style>

    <div class="row">
        <div class="span2" style="min-width:135px;max-width:135px">
			<div id="tc-classes-inner"></div>
        </div>
        <div class="span10" style="min-width:650px;max-width:650px">
            <div class="blackframe">
                <div id="tc-itself"></div>

            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        tc_init();
	</script>
</asp:Content>
