<%@ Page Title="" Language="C#" MasterPageFile="~/RaidDamageMasterFrame.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="VF.RaidDamageWebsite.About" %>
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
                <h2>About RaidStats</h2>
                    RaidStats is a "world of logs" like website for world of warcraft vanilla private-servers.
                    It was long requested that something like it should be made. Since i had made something similar(but very basic) back in spring 2013 for my own guild i figured i should make something more generic that can be used by everyone. 
                    The goal has always been to focus on making a strong platform which allows players/guilds to directly compare themselves to other players/guilds. 
                    Therefore i spent a very long time perfecting the way i log the data to make sure the values are reliable and not artificially high due to a bug/the way the boss is fighted. There are still a few bugs to fix, but most of it is server sided.
                    <h4>How does it work</h4>
                    There is 1 player that needs to have an addon called VF_RaidDamage(to be renamed VF_RaidStats in the future one day!). 
                    The addon uses SW_Stats 2.0 Beta 7 to read damage and healing done by raidmembers, this means the more players in the raid that has SW_Stats the more accurate the log will be.
                    The addon is completely automatic and records every change that has happened every 5-10 seconds(possibly changed in the future) if you are in a raid.
                    The SavedVariables file is later sent to the server and parsed, just like how RealmPlayers works this is done using the <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=16&t=4">VF_WoWLauncher</a><span class="team-info">.
                    <h4>Recording your guilds raid</h4>
                    Contact <a class="team-member" href="http://forum.realmplayers.com/memberlist.php?mode=viewprofile&u=51">Sethzer</a> on realmplayers forums. Tell him about the characters you play, the same requirements to becoming a RealmPlayers uploader applies to RaidStats. Make sure you read more about the requirements here: <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=14&t=15">http://forum.realmplayers.com/viewtopic.php?f=14&t=15</a>.
                <br />
                <h4>Thanks to</h4>
                <ul class="unstyled">
	            <li><a class="team-member" href="http://realmplayers.com/CharacterViewer.aspx?realm=WSG&player=Opilol">Opilol</a><span class="team-info"> - Alpha/Beta tester from the guild Belligerent(WSG). Helped me improve the service with support for AQ40 and Naxxramas bosses.</span></li>
	            <li><a class="team-member" href="http://realmplayers.com/CharacterViewer.aspx?realm=ED&player=Zey">Zey</a><span class="team-info"> - Alpha/Beta tester from the guild Dreamstate(ED). Backup data logger for Dreamstate for those days that the modified-every-day version i use myself didnt work at all due to bugs.</span></li>
	            <li><a class="team-member" href="http://realmplayers.com/CharacterViewer.aspx?realm=ED&player=Showtime">Showtime</a><span class="team-info"> - Alpha/Beta tester from the guild Ridin Dirty(ED). Helped me improve the addon and the reliability a lot by bringing a data logging perspective of a healing class and a crazy guild.</span></li>
	            <li><a class="team-member" href="http://realmplayers.com/CharacterViewer.aspx?realm=ED&player=Sethzer">Sethzer</a><span class="team-info"> - Alpha/Beta tester from the guild Team Plague(ED). Helped me improve the addon and the reliability by bringing a data logging perspective of a ranged damage class and alliance guild.</span></li>
                </ul>
            </div></div></div>
            <div class="row"><div class="span12"><div class="fame">
                <h2>About everything</h2>
                <h4>Existing projects</h4>
                <ul class="unstyled">
                    <li><a class="team-member" href="http://realmplayers.com">RealmPlayers</a><span class="team-info"> - Started 2013-07-16. Armory, pvp standings, guild progress</span></li>
                    <li><a class="team-member" href="http://realmplayers.com/RaidStats">RaidStats</a><span class="team-info"> - Started 2013-09-29. Released Publicly 2013-12-05. Recorded raids, can view damage, healing, threat and analyse raids overall.</span></li>
                    <li><a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=16&t=4">VF_WoWLauncher</a><span class="team-info"> - Started 2013-12-08. Released Publicly 2014-02-01. A WoW launcher for 1.12.1 private server realms, WTF config management, wow-one forum news, addon updates.</span></li>
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
                Donations will go towards supporting and improving RealmPlayers, RaidStats and future interesting public projects. For more details you can <a class="team-member" href="http://realmplayers.com/Donate.aspx">visit the Donation page</a><br />
            </div></div></div>
          </div>
        </div>
    </header>
</asp:Content>
