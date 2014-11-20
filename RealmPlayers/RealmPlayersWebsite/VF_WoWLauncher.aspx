<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="VF_WoWLauncher.aspx.cs" Inherits="RealmPlayersServer.VF_WoWLauncher" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <li><a href="Index.aspx">Home</a> <span class="divider">/</span></li>
        <li class="active">VF_WoWLauncher</li>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span12">
            <div class="row"><div class="span12"><div class="fame">
                <h2>VF_WoWLauncher</h2>

                <p>After not getting the official feenix launcher to work good at all and seeing how outdated it was i decided to take matters into my own hands.</p>
                <p>I present to you VF_WoWLauncher. A new WoW Launcher designed for the 1.12.1 feenix realms. <font color='#fff'>Scroll to the bottom of the page for a download link.</font><br />
                (Note that this is a static picture that may not be updated to reflect the latest version updates)
                <img src="assets/img/VF_WoWLauncher.png" />
                    <br />
                <h4>Features:</h4>
                <ul>
                <li><b><font color='#fff'>Launch multiple clients with different configurations easily</font></b><br />
                    In the Launcher you can easily create config profiles such as different resolutions to let you easily launch a "bank alt" WoW with the lowest resolution etc.
                    Configurations are easily chosen from a dropdownlist in the main window.
                </li>
                <li><b><font color='#fff'>Play any feenix realm easily(Archangel(TBC) aswell if installed)</font></b><br />
                    In the Launcher you can easily change what realm you want to connect to using a dropdownlist.
                </li>
                <li><b><font color='#fff'>Option to clear WDB</font></b><br />
                    If wanted the Launcher clears the WDB folder when wow is started.
                </li>
                <li><b><font color='#fff'>Easily find out about the latest feenix forum news</font></b><br />
                    In the Launcher you can get a fast sneakpeek of the latest posts done to the "1.12.1 Changelogs" and "News and Announcements"/"Information and Releases"/"Server Updates" wow-one subforums.
                    Forum posts are sorted by date and can be hidden to further improve the visible impact a new post will have when you start the Launcher.
                </li>
                <li><b><font color='#fff'>VF_RealmPlayers and VF_RaidStats uploading</font></b><br />
                    The Launcher is direct replacement for the old VF_RealmPlayersUploader application used to upload VF_RealmPlayers data. 
                    If you have the addon installed and a UserID configured the data will automatically be uploaded after WoW is closed, just like it worked using the old WoWAndRPUploader shortcut.
                    The Launcher also automatically uploads any raids recorded using VF_RaidStats.
                    Note that if you do not have VF_RealmPlayers or VF_RaidStats addon installed or no UserID configured, nothing will get uploaded.
                </li>
                <li><b><font color='#fff'>Easily update your favourite addons to the latest versions</font></b><br />
                    As an example updates to the addon VF_RealmPlayers and VF_RaidStats can easily be installed by clicking a button on an update notification.
                    This will make it much easier for me to make updates to said addons as they can be distributed easily.
                    The addon updating framework is generic and can be used for any addons people want. 
                    Contact me if you are an addon maker and want your newest versions to easily be distributed to users.
                </li>
                <li><b><font color='#fff'>VF_WoWLauncher can be updated easily</font></b><br />
                    If any new updates exist to the VF_WoWLauncher you will be asked if you want to update.
                    If you chose to update it will automatically download the new version and install it for you.
                    When done with the update you will be able to use the latest new features added.
                </li>
                </ul>
                More features and improvements are going to get added over time.
                    <br /><br />
                <b><a class="team-member" href="ftp://realmplayers.com:5511/VF_WoWLauncherInstaller.exe">The VF_WoWLauncher installer can now be downloaded by clicking this link.</a></b><br />
                For any questions/feedback/suggestions or problems you can always contact me through my mail <a class="team-member" href="mailto:Dilatazu@gmail.com?Subject=Realmplayers.com" target="_top">Dilatazu@gmail.com</a> or PM @ <a class="team-member" href="http://forum.realmplayers.com/memberlist.php?mode=viewprofile&u=2">Dilatazu</a> on realmplayers forums.<br />
                </p>
            </div></div></div>
          </div>
        </div>
    </header>
</asp:Content>
