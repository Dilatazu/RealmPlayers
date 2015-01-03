<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="RealmPlayersServer.About" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <li><a href="Index.aspx">Home</a> <span class="divider">/</span></li>
        <li class="active">About</li>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span12">
            <div class="row"><div class="span12"><div class="fame">
                <h2>About Realmplayers</h2>
                    RealmPlayers is an armory website developed by Dilatazu for vanilla WoW private-servers.
                    It was originally an idea about having pvp lists which existed in the old days of retail vanilla. 
                    The pvp lists are still part of the website but the website is nowadays a full WoW armory with character and guild data supporting multiple private server realms.
                    <h4>How does it work</h4>
                    The armory works by having several people running around with an addon which saves a database of players that are targeted.
                    This database is later sent to the server by using a small simple uploader program which you either manually or automaticly run everytime you close wow. 
                    The server parses the lua database and saves the data in a binary format. 
                    The website reads the latest version of the binary database once every 10 minutes and keeps it in memory.
                    <h4>Helping out</h4>
                    I am always looking for more data contributors, the more we have the more accurate the website will be.
                    Using the addon and uploader is extremely easy and only requires a maximum of 2 minutes installation time and is bundled with various ways to run the uploader application without effort.
                    <p>If you are interested in helping out, <a href="http://forum.realmplayers.com/viewtopic.php?f=14&t=15">you can read more here</a></p>
                <h4>Top Data Contributors</h4>
                <ul class="unstyled">
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Sethzer">Sethzer</a><span class="team-info"> - Active ED(Alliance) since day one, collects data for all realms</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Tosicx">Tosicx</a><span class="team-info"> - Active ED(Horde) contributor since 2013-12-09</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Azuros">Azuros</a><span class="team-info"> - Active ED(Alliance) contributor since 2013-09-09</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Zey">Zey</a><span class="team-info"> - Active ED(Horde) contributor since 2013-08-31</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=WSG&player=Meandawg">Meandawg</a><span class="team-info"> - Active WSG(Alliance) contributor since 2013-10-26</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Shadyrules">Shadyrules</a><span class="team-info"> - Active ED(Horde) contributor since 2013-08-16</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=WSG&player=Gladys">Gladys</a><span class="team-info"> - Active ED(Horde) contributor since 2013-12-03</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=WSG&player=Soge">Soge</a><span class="team-info"> - Active WSG(Alliance) contributor since 2013-09-07</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=WSG&player=Bigfella">Bigfella</a><span class="team-info"> - Active WSG(Horde) contributor since 2013-12-03</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Oejesten">Oejesten</a><span class="team-info"> - Active ED(Horde) contributor since 2014-01-01</span></li>
                    <%--<li><a class="team-member" href="/CharacterViewer.aspx?realm=WSG&player=Virose">Lyq</a><span class="team-info"> - Active WSG(Alliance) contributor since 2013-11-04</span></li>
                    <li><a class="team-member" href="/CharacterViewer.aspx?realm=AlA&player=Goromi">Goromi</a><span class="team-info"> - Active AlA(Horde) contributor since 2013-08-29</span></li>--%>
                    <li><a class="team-member" href="/Contributors.aspx">Everyone else</a><span class="team-info"> - Thanks all!</span></li>
                </ul>
                <h4>Extra thanks</h4>
                <ul class="unstyled">
	            <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Sethzer">Sethzer</a><span class="team-info"> - Alpha/Beta tester and general supporter with great data contributions since day one</span></li>
	            <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Ateni">Ateni</a><span class="team-info"> - Help with website design template</span></li>        
	            <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Sixt">Sixt</a><span class="team-info"> - Thanks for converting those tables of Enchant and Suffix IDs to something useful!</span></li>
	            <li><a class="team-member" href="/CharacterViewer.aspx?realm=ED&player=Medelane">Medelane</a><span class="team-info"> - Thanks for the great idea of making donations more visible!</span></li>
	            <li><a class="team-member" href="/Donators.aspx">All Donators</a><span class="team-info"> - Thanks a lot for all the donations! I appreciate the support a lot :)</span></li>
                </ul>
            </div></div></div>
            <div class="row"><div class="span12"><div class="fame">
                <h2>About everything</h2>
                <h4>Existing projects</h4>
                <ul class="unstyled">
                    <li><a class="team-member" href="http://realmplayers.com">RealmPlayers</a><span class="team-info"> - Started 2013-07-16. Armory, pvp standings, guild progress</span></li>
                    <li><a class="team-member" href="http://realmplayers.com/RaidStats">RaidStats</a><span class="team-info"> - Started 2013-09-29. Released Publicly 2013-12-05. Recorded raids, can view damage, healing, threat and analyse raids overall.</span></li>
                    <li><a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=16&t=4">VF_WoWLauncher</a><span class="team-info"> - Started 2013-12-08. Released Publicly 2014-02-01. A WoW launcher for 1.12.1 private server realms, WTF config management, feenixserver forum news, addon updates.</span></li>
                </ul>
                <h4>Project Changelogs</h4>
                You can find the changelog for Realmplayers here: <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=14&t=10">http://forum.realmplayers.com/viewtopic.php?f=14&t=10</a><br />
                You can find the changelog for RaidStats here: <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=15&t=8">http://forum.realmplayers.com/viewtopic.php?f=15&t=8</a><br />
                You can find the changelog for VF_WoWLauncher here: <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=16&t=7">http://forum.realmplayers.com/viewtopic.php?f=16&t=7</a>
                <h4>Bug report and Issues</h4>
                Please create a post about any issues or bugs you find related to the projects at <a class="team-member" href="http://forum.realmplayers.com">http://forum.realmplayers.com</a>.
                <h4>Contact</h4>
                For any questions regarding data contribution and getting a UserID you can contact <a class="team-member" href='http://forum.realmplayers.com/memberlist.php?mode=viewprofile&u=51'>Sethzer</a> by PM on the realmplayers forums.
                <br />For any questions regarding anything else you can always contact me through PM @ <a class="team-member" href="http://forum.realmplayers.com/memberlist.php?mode=viewprofile&u=2">Dilatazu</a> on realmplayers forums.
                <h4>Donate</h4>
                If you like the projects i would be very happy for any donations.<br />
                Donations will go towards supporting and improving RealmPlayers, RaidStats and future interesting public projects. For more details you can <a class="team-member" href="Donate.aspx">visit the Donation page</a><br />
            <div style='margin-top: 5px; height: 30px;'>
            <%--<script src="/Assets/CoinWidget/coin.js?version=1"></script>
            <script>
                CoinWidgetCom.go({
                    wallet_address: "1EdLMCosekTQqwgmhJkj3fFCNP42YhubJw"
                    , currency: "bitcoin"
                    , counter: "hide"
                    , alignment: "al"
                    , qrcode: false
                    , auto_show: false
                    , lbl_button: "Donate Bitcoin"
                    , lbl_address: "Realmplayers Bitcoin Address:"
                    , lbl_count: "donations"
                    , lbl_amount: "BTC"
                });
            </script>
            <script>
                CoinWidgetCom.go({
                    wallet_address: "Len839XVS8vzYGZUh8kQ6zcRhvwvLCXY9d"
                    , currency: "litecoin"
                    , counter: "hide"
                    , alignment: "al"
                    , qrcode: false
                    , auto_show: false
                    , lbl_button: "Donate Litecoin"
                    , lbl_address: "Realmplayers Litecoin Address:"
                    , lbl_count: "donations"
                    , lbl_amount: "LTC"
                });
            </script>--%>
            <style>
            .PayPalButton a {
                width:  166px;
                height: 30px;
                display: block;
            background-image: url('/assets/img/PaypalPay_NotClicked_Smaller.png');
            }
            .PayPalButton a:hover {
            background-image: url('/assets/img/PaypalPay_Clicked_Smaller.png');
            }
            </style>
            <div class='PayPalButton' style='242px; height: 30px;'>
            <a href='https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6BGRWFLQ2VBFQ' target='_blank'></a>
            </div>
            </div>
            </div></div></div>
          </div>
        </div>
    </header>
</asp:Content>
