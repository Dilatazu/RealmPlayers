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
    <style> .clear {
         clear: both;
     }</style>
    <div id="layers"></div>
    <div id="main">
            <div id="tc-classes" class="choose">
		        <div id="tc-classes-outer">
			        <div id="tc-classes-inner"><p>Choose a class:</p></div>
		        </div>
	        </div>
	    <div id="main-contents" class="main-contents">

            <div id="tc-itself"></div>

            <script type="text/javascript">
	            tc_init();
	        </script>

	        <div class="clear"></div>
        </div>
	</div>
</asp:Content>
