<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="Donate.aspx.cs" Inherits="RealmPlayersServer.Donate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <li><a href="Index.aspx">Home</a> <span class="divider">/</span></li>
        <li><a href="About.aspx">About</a> <span class="divider">/</span></li>
        <li class="active">Donate</li>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span12">
            <div class="row"><div class="span12"><div class="fame">
                <h2>Donate for stability and improvement</h2>
                <h3>for RealmPlayers, RaidStats and VF_WoWLauncher</h3>
                <p>Please consider donating if you like the projects i develop and want to support and help make them even better. Leave a comment as either anonymous or with your ingame name to be listed on <a href="Donators.aspx">the Donators page</a></p>
                
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
                    <div class='PayPalButton' style='margin-top: -25px; margin-left: 242px; height: 30px;'>
                    <a href='https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6BGRWFLQ2VBFQ' target='_blank'></a>
                    </div>
                </div>
                <h4>Donations will go towards the following:</h4>
                <ul>
                <li><b><font color='#fff'>Server costs</font></b><br />
                    I host the server myself but the electricity and good internet connection is costing approximately 60 euro every month. Donations help me being able to continue hosting the website with as good, reliable and stable service as possible.</li>
                <li><b><font color='#fff'>Working</font></b><br />
                    There is always a lot of work to do and time is money. I have spent the majority of my free time working on these projects since i started back in august 2013. <br />
                    There are a lot of features that i want to implement and i will continue to work as hard as i can on these projects.<br />
                    Donations are a big motivator for me to continueing improving the service with new features, solving stability issues, adding contributors and other things related to the projects.
                </li>
                </ul>
                <%--<h4>Here are some of the main things that i currently work on(last updated 2014-03-15):</h4>
                <ul>
                <li><b><font color='#fff'>Improving backend for RealmPlayers(Website/Database)</font></b><br />
                    The database for RealmPlayers is really fast, but more things need to be saved into the database, such as item usage information. <br />
                    At the moment all this data is generated dynamically everytime the website reloads(sometimes as often as every 10 minutes). <br />
                    The data needs to be generated once every now and then instead and saved into a format that can easily be extended with the newest data to give something that is as dynamic as possible but only the latest month character changes needs to be calculated instead of every character change since the beginning of project.<br />
                </li>
                <li><b><font color='#fff'>New features for VF_WoWLauncher(Program/Database)</font></b><br />
                    The plan is to make a library of the best version of addons available. Such that a completely new player can download the Launcher and instantly find addons that works and 1 click to install them.<br />
                    Other then that there are loads of ideas that i have. Possibly some kind of optional SavedVariables synchronization service for players that use multiple computers.<br />
                    I am as always very open to any ideas on how to improve my projects and especially in this case as i know there is great potential on what can be possible to achieve.
                </li>
                <li><b><font color='#fff'>New features for VF_RealmPlayers(Addon)</font></b><br />
                    Recording data such as mount and pet/companion when possible.<br />
                    Recording multiple data sets for 1 player each session to give the database more accuracy for history.
                </li>
                <li><b><font color='#fff'>New features for RealmPlayers(Website)</font></b><br />
                    Using data from RaidStats to determine server firsts etc.<br />
                    Achievments, such as server firsts, but also fun things such as "player has more than 5 epic weapons" or amount of mounts/pets etc.
                </li>
                <li><b><font color='#fff'>New features for VF_RaidStats(Addon)</font></b><br />
                    Recording item drops from bosses.<br />
                    Recording more details and improving the compactness of the data.
                </li>
                <li><b><font color='#fff'>New features for RaidStats(Website)</font></b><br />
                    Statistics and data for individual players such as a list of raids attended and things such as average DPS, average deaths for each boss fights.<br />
                    Able to look at phases between the various fights and possible even between any 2 time periods you want.
                </li>
                <li><b><font color='#fff'>Suggestions and bug reports posted at the forum</font></b><br />
                    Forum can be found here: <a href="http://forum.realmplayers.com">http://forum.realmplayers.com</a>
                </li>
                <li><b><font color='#fff'>Possibly future work/new project for RealmPlayers</font></b><br />
                    Armory for WoW 2.4.3
                </li>
                </ul>--%>
                <h4>For more information on what i work on you can take a look at the <a href="http://forum.realmplayers.com">forum</a></h4>
                Project changelogs can be found here: <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=14&t=10">RealmPlayers</a>, <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=15&t=8">RaidStats</a>, <a class="team-member" href="http://forum.realmplayers.com/viewtopic.php?f=16&t=7">VF_WoWLauncher</a>
            </div></div></div>
          </div>
        </div>
    </header>
</asp:Content>
