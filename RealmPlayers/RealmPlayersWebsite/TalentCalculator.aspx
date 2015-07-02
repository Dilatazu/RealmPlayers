<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="TalentCalculator.aspx.cs" Inherits="RealmPlayersServer.TalentCalculator" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
	
    <%--//https://github.com/mangostools/aowow
    //https://github.com/Sarjuuk/aowow
    //https://github.com/mangoszero
    // https://github.com/mangostools--%>
	<link rel="stylesheet" type="text/css" href="assets/temp/global.css?16.7">
	<link rel="stylesheet" type="text/css" href="assets/css/talentcalc.css?16.7">
	<link rel="stylesheet" type="text/css" href="assets/css/talent.css?16.7">
		
    <script src="assets/temp/fx.js?16.7" type="text/javascript"></script>
    <script src="assets/temp/locale_enus.js?16.7" type="text/javascript"></script>
	<script src="assets/temp/locale_enus_0.js?16.7" type="text/javascript"></script>
	<script src="assets/temp/Markup-2.js?16.7" type="text/javascript"></script>
	<script src="assets/temp/global.js?16.7" type="text/javascript"></script>
	<script src="assets/js/TalentCalc_enus.js?16.7" type="text/javascript"></script>
	<script src="assets/js/TalentCalc.js?16.7" type="text/javascript"></script>
	<script src="assets/js/talent.js?16.7" type="text/javascript"></script>

	<script type="text/javascript">
		var g_serverTime = new Date('2015/06/29 12:36:36');
		g_locale = { id: 0, name: 'enus' };
		g_glyphs = [];
ss_conf = 3;	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div id="main-precontents"></div>
	<div id="main-contents" class="main-contents">

    <div id="tc-classes" class="choose">
		<div id="tc-classes-outer">
			<div id="tc-classes-inner"><p>Choose a class:</p><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_druid.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_hunter.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_mage.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_paladin.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_priest.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_rogue.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_shaman.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_warlock.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="iconmedium"><ins style="background-image: url(http://db.vanillagaming.org/images/icons/medium/class_warrior.jpg);"></ins><del></del><a href="javascript:;"></a></div><div class="clear"></div></div>
		</div>
	</div>
    <div id="tc-itself"></div>

	<script type="text/javascript">
		tc_init();
	</script>

	<div class="clear"></div>
	</div>
    <script defer="defer">aowow.init();</script>
</asp:Content>
