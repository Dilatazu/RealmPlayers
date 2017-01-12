using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VF_WoWLauncher
{
    internal enum LinkType
    {
        Homepage,
        RealmStatus,
        Forum,
        Bugtracker,
        Changelog,
        Subreddit,
        Twitter,
        Database,
        TalentCalculator,
    }
    public partial class LauncherWindow : Form
    {
        internal string GetLinkTypeString(LinkType linkType)
        {
            switch (linkType)
            {
                case LinkType.Homepage:
                    return "Homepage";
                case LinkType.RealmStatus:
                    return "Realm Status";
                case LinkType.Forum:
                    return "Forum";
                case LinkType.Bugtracker:
                    return "Bugtracker";
                case LinkType.Changelog:
                    return "Changelog";
                case LinkType.Subreddit:
                    return "Subreddit";
                case LinkType.Twitter:
                    return "Twitter";
                case LinkType.Database:
                    return "Database";
                case LinkType.TalentCalculator:
                    return "Talent Calculator";
                default:
                    return "";
            }
            return "";
        }
        public LauncherWindow()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.VF_WL_ICON_32;
            this.FormClosing += LauncherWindow_FormClosing;
            this.Move += LauncherWindow_Move;
            this.Activated += LauncherWindow_PostLoad;

            this.Menu = new MainMenu();

            {
                var fileMenu = new MenuItem("File");
                fileMenu.MenuItems.Add(new MenuItem("Settings", new EventHandler((o, ea) => {
                    var oldUserID = Settings.UserID;
                    var oldWowFolder = Settings.Instance._WowDirectory;
                    var oldWowTBCFolder = Settings.Instance._WowTBCDirectory;
                    ApplicationSettings.ShowApplicationSettings();
                    UpdatePossibleRealmConfigurations();
                    if (Settings.Instance.AutoRefreshNews == true)
                    {
                        c_tRefreshNews.Interval = 1000 * 3600; //Once every hour
                        c_tRefreshNews.Enabled = true;
                    }
                    else
                    {
                        c_tRefreshNews.Enabled = false;
                    }
                    if (oldUserID != Settings.UserID
                    || oldWowFolder != Settings.Instance._WowDirectory
                    || oldWowTBCFolder != Settings.Instance._WowTBCDirectory)
                    {
                        GetLatestAddonUpdates();
                        //GetLatestUserIDAddons();
                    }
                })));
                fileMenu.MenuItems.Add(new MenuItem("Close", new EventHandler((o, ea) => Close())));

                this.Menu.MenuItems.Add(fileMenu);
            }

            Dictionary<string, Dictionary<LinkType,string>> vanillaServersMenuTree = new Dictionary<string, Dictionary<LinkType, string>>
            {
                { "Elysium Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "https://elysium-project.org/"},
                        { LinkType.RealmStatus, "https://elysium-project.org/status/"},
                        { LinkType.Forum, "https://forum.elysium-project.org/"},
                        { LinkType.Subreddit, "https://www.reddit.com/r/elysiumproject"},
                        { LinkType.Twitter, "https://twitter.com/elysium_dev"}
                    }
                },
                { "Kronos Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://www.kronos-wow.com/"},
                        { LinkType.Forum, "http://forum.twinstar.cz/"},
                        { LinkType.Subreddit, "https://www.reddit.com/r/kronoswow"},
                        { LinkType.Twitter, "https://twitter.com/KronosWoW"}
                    }
                },
                { "VanillaGaming Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://vanillagaming.org/"},
                        { LinkType.Forum, "http://vanillagaming.org/forum/"},
                        { LinkType.Database, "http://db.vanillagaming.org/"},
                        { LinkType.TalentCalculator, "http://db.vanillagaming.org/?talent"},
                    }
                },
                { "Rebirth Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://www.therebirth.net/"},
                        { LinkType.Forum, "http://www.therebirth.net/forum/"},
                        { LinkType.Subreddit, "http://www.reddit.com/r/rebirthwow/"},
                    }
                },
                { "RetroWoW Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://retro-wow.com/"},
                        { LinkType.Forum, "http://retro-wow.com/forum/"},
                    }
                },
                { "MagicWoW Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://www.magicwow.co.uk/"},
                        { LinkType.Forum, "http://www.magicwow.co.uk/forums.php"},
                        { LinkType.Bugtracker, "http://www.magicwow.co.uk/index.php?page=bugtracker"},
                    }
                },
                { "SiegeWoW Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://siegewow.com/"},
                        { LinkType.Subreddit, "https://www.reddit.com/r/SiegeWoW/"},
                    }
                },
                { "NostalGeek(FR) Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://nostalgeek-serveur.com/"},
                        { LinkType.Forum, "http://nostalgeek-serveur.com/forums/"},
                    }
                },
                { "Nefarian(DE) Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://www.classic-wow.org/"},
                        { LinkType.Forum, "http://community.mmobase.de/forum.php#classic-wow-1-12-1"},
                    }
                },
                { "Nostalrius Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://en.nostalrius.org/"},
                        { LinkType.Forum, "http://forum.nostalrius.org/"},
                        { LinkType.Subreddit, "https://www.reddit.com/r/nostalriusbegins"},
                        { LinkType.Twitter, "https://twitter.com/NostalBegins"}
                    }
                },
            };
            Dictionary<string, Dictionary<LinkType, string>> tbcServersMenuTree = new Dictionary<string, Dictionary<LinkType, string>>
            {
                { "Feenix Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://www.wow-one.com"},
                        { LinkType.Forum, "http://www.wow-one.com/forum"},
                        { LinkType.Database, "http://database.wow-one.com/"},
                    }
                },
                { "VengeanceWoW Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "https://www.vengeancewow.com/"},
                        { LinkType.Forum, "https://www.vengeancewow.com/forum/"},
                        { LinkType.Changelog, "https://www.vengeancewow.com/forum/forum43/"},
                    }
                },
                { "Excalibur Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://www.excalibur-server.com/"},
                        { LinkType.Forum, "http://www.excalibur-nw.com/forum"},
                    }
                },
                { "Looking4Group Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "http://www.looking4group.eu/"},
                        { LinkType.Bugtracker, "https://bitbucket.org/looking4group_b2tbc/looking4group/issues?status=new&status=open"},
                        { LinkType.Subreddit, "https://www.reddit.com/r/looking4group"},
                    }
                },
                { "Back2Basics(DE) Links",
                    new Dictionary<LinkType, string> {
                        { LinkType.Homepage, "https://www.back2basics-wow.eu/"},
                        { LinkType.Forum, "https://www.back2basics-wow.eu/community/index.php?page=Index"},
                        { LinkType.Database, "https://tbcdb.rising-gods.de/"},
                    }
                },
            };
          
            {
                var linksMenu = new MenuItem("Links");
                var vanillaRealmsMenu = new MenuItem("Vanilla Realms");
                var tbcRealmsMenu = new MenuItem("TBC Realms");
                linksMenu.MenuItems.Add(vanillaRealmsMenu);
                linksMenu.MenuItems.Add(tbcRealmsMenu);

                foreach (var menuItem in vanillaServersMenuTree)
                {
                    var links = new MenuItem(menuItem.Key);
                    foreach(var link in menuItem.Value)
                    {
                        links.MenuItems.Add(new MenuItem("Goto " + GetLinkTypeString(link.Key), new EventHandler((o, ea) =>
                        {
                            System.Diagnostics.Process.Start(link.Value);
                        })));
                    }
                    vanillaRealmsMenu.MenuItems.Add(links);
                }
                foreach (var menuItem in tbcServersMenuTree)
                {
                    var links = new MenuItem(menuItem.Key);
                    foreach (var link in menuItem.Value)
                    {
                        links.MenuItems.Add(new MenuItem("Goto " + GetLinkTypeString(link.Key), new EventHandler((o, ea) =>
                        {
                            System.Diagnostics.Process.Start(link.Value);
                        })));
                    }
                    tbcRealmsMenu.MenuItems.Add(links);
                }

                linksMenu.MenuItems.Add("-");
                {
                    var links = new MenuItem("Other Links");
                    links.MenuItems.Add(new MenuItem("Wowservers Subreddit", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://www.reddit.com/r/wowservers/");
                    })));
                    links.MenuItems.Add(new MenuItem("Talent Calculator", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://realmplayers.com/Talents.aspx");
                    })));
                    links.MenuItems.Add(new MenuItem("Character Designer", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://realmplayers.com/CharacterDesigner.aspx");
                    })));
                    {
                        var itemdbLinks = new MenuItem("Item Databases");
                        itemdbLinks.MenuItems.Add(new MenuItem("Goto Feenix", new EventHandler((o, ea) =>
                        {
                            System.Diagnostics.Process.Start("http://database.wow-one.com/");
                        })));
                        itemdbLinks.MenuItems.Add(new MenuItem("Goto Valkyrie", new EventHandler((o, ea) =>
                        {
                            System.Diagnostics.Process.Start("http://db.valkyrie-wow.org/");
                        })));
                        itemdbLinks.MenuItems.Add(new MenuItem("Goto VanillaGaming", new EventHandler((o, ea) =>
                        {
                            System.Diagnostics.Process.Start("http://db.vanillagaming.org/");
                        })));
                        links.MenuItems.Add(itemdbLinks);
                    }
                    linksMenu.MenuItems.Add(links);
                }
                linksMenu.MenuItems.Add("-");
                linksMenu.MenuItems.Add(new MenuItem("RealmPlayers Armory", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://realmplayers.com");
                })));
                linksMenu.MenuItems.Add(new MenuItem("RaidStats", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://realmplayers.com/RaidStats");
                })));
                linksMenu.MenuItems.Add(new MenuItem("Talent Calculator", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://realmplayers.com/Talents.aspx");
                })));
                linksMenu.MenuItems.Add(new MenuItem("RealmPlayers Forum", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://forum.realmplayers.com");
                })));
                linksMenu.MenuItems.Add(new MenuItem("RealmPlayers Patreon", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("https://www.patreon.com/realmplayers");
                })));
                linksMenu.MenuItems.Add(new MenuItem("RealmPlayers Discord", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("https://discord.gg/NrZAVFE");
                })));
                linksMenu.MenuItems.Add(new MenuItem("RealmPlayers Twitter", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://twitter.com/realmplayers");
                })));
                this.Menu.MenuItems.Add(linksMenu);
            }
            {
                var toolsMenu = new MenuItem("Tools");
                //toolsMenu.MenuItems.Add(new MenuItem("Addons Settings(Experimental)", new EventHandler((o, ea) => AccountSettings.ShowAccountSettings())));
                toolsMenu.MenuItems.Add(new MenuItem("Addons Manager", new EventHandler((o, ea) => AddonsSettings.ShowAddonsSettings())));
                toolsMenu.MenuItems.Add(new MenuItem("RealmList Manager", new EventHandler((o, ea) => 
                {
                    RealmListSettings.ShowRealmListSettings();
                    UpdatePossibleRealmConfigurations();
                })));
                toolsMenu.MenuItems.Add("-");
                toolsMenu.MenuItems.Add(new MenuItem("Create AddonPackage(Experimental)", 
                    new EventHandler((o, ea) => {
                        CreateAddonPackageForm newForm = new CreateAddonPackageForm();
                        newForm.ShowDialog();
                    })));
                toolsMenu.MenuItems.Add(new MenuItem("Poll forums for News", new EventHandler((o, ea) => GetLatestNews(false))));
                toolsMenu.MenuItems.Add("-");
                toolsMenu.MenuItems.Add(new MenuItem("Show Log(Debug)", new EventHandler(c_btnShowLog_Click)));
                this.Menu.MenuItems.Add(toolsMenu);
            }
            {
                var helpMenu = new MenuItem("About");
                helpMenu.MenuItems.Add(new MenuItem("Donate", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://realmplayers.com/Donate.aspx");
                })));
                helpMenu.MenuItems.Add(new MenuItem("About", new EventHandler((o, ea) => (new AboutWindow()).ShowDialog())));

                this.Menu.MenuItems.Add(helpMenu);
            }

            if (Settings.Instance.AutoRefreshNews == true)
            {
                c_tRefreshNews.Interval = 1000 * 3600; //Once every hour
                c_tRefreshNews.Enabled = true;
            }
            else
            {
                c_tRefreshNews.Enabled = false;
            }

            c_tRefreshAddonUpdates.Interval = 1000 * 600; //Once every 10 minutes
            c_tRefreshAddonUpdates.Enabled = true;

            //JumpListCustomCategory userActionsCategory = new JumpListCustomCategory("Actions");
            //string notepadPath = Path.Combine(Environment.SystemDirectory, "notepad.exe");
            //JumpListLink jlNotepad = new JumpListLink(notepadPath, "Notepad");
            //jlNotepad.IconReference = new IconReference(notepadPath, 0);
            //list.AddUserTasks(jlNotepad);
            //list.Refresh();
            //JumpList list = JumpList.CreateJumpList();
            //JumpListCustomCategory category = new JumpListCustomCategory("Links"); 
            //category.AddJumpListItems(new JumpListLink("http://www.microsoft.com", "Microsoft"));
            //list.AddCustomCategories(category);
            //list.Refresh();

            Program.g_LauncherApp.InitiateJumpList();
        }

        private void UpdatePossibleRealmConfigurations()
        {
            if (Settings.HaveTBC == true)
            {
                foreach (var realmInfo in Settings.Instance.RealmLists)
                {
                    if (realmInfo.Value.WowVersion == WowVersionEnum.TBC)
                    {
                        if (c_ddlRealm.Items.Contains(realmInfo.Key) == false)
                            c_ddlRealm.Items.Add(realmInfo.Key);
                    }
                }
            }
            else
            {
                foreach (var realmInfo in Settings.Instance.RealmLists)
                {
                    if (realmInfo.Value.WowVersion == WowVersionEnum.TBC)
                    {
                        if (c_ddlRealm.Items.Contains(realmInfo.Key) == true)
                        {
                            if ((string)c_ddlRealm.SelectedItem == realmInfo.Key)
                                c_ddlRealm.SelectedIndex = 0;
                            c_ddlRealm.Items.Remove(realmInfo.Key);
                        }
                    }
                }
            }
            foreach (var realmInfo in Settings.Instance.RealmLists)
            {
                if (realmInfo.Value.WowVersion == WowVersionEnum.Vanilla)
                {
                    if (c_ddlRealm.Items.Contains(realmInfo.Key) == false)
                        c_ddlRealm.Items.Add(realmInfo.Key);
                }
            }
        }

        //void FilterNews()
        //{
        //    for(int i = 0; i < c_dlAddons.flpListBox.Controls.Count; ++i)
        //    {

        //    }
        //}

        bool m_PollingForNews = false;
        void GetLatestNews(bool _FirstTime)
        {
            bool onlyNewest = (_FirstTime == false);
            if (m_PollingForNews == true)
                return;
            (new System.Threading.Tasks.Task(() =>
            {
                m_PollingForNews = true;
                try
                {
                    DateTime absoluteMinDate = DateTime.Now.AddDays(-30);
                    Action<ForumReader.ForumPost> newPostLambda = (ForumReader.ForumPost _NewPost) =>
                    {
                        DateTime minDate = DateTime.Now.AddDays(-14);
                        ForumReader.ForumType forumType = ForumReader.ForumType.FeenixForum;
                        if (_NewPost.m_PostURL.Contains("forum.realmplayers.com") == true)
                        {
                            minDate = DateTime.Now.AddDays(-28);
                            forumType = ForumReader.ForumType.RealmPlayersForum;
                            if (_NewPost.m_PosterName != "Dilatazu" || _NewPost.m_PostContent.Contains("- -"))
                                return;
                        }
                        else if (_NewPost.m_PostURL.Contains("forum.nostalrius.org") == true)
                        {
                            minDate = DateTime.Now.AddDays(-28);
                            forumType = ForumReader.ForumType.NostalriusForum;
                            if (_NewPost.m_PosterName != "Viper" && _NewPost.m_PosterName != "Daemon" && _NewPost.m_PosterName != "Syrah")
                                return;
                        }
                        else if (_NewPost.m_PostURL.Contains("forum.twinstar.cz") == true)
                        {
                            minDate = DateTime.Now.AddDays(-28);
                            forumType = ForumReader.ForumType.KronosForum;
                            //Not needed for Kronos forum
                            //if (_NewPost.m_PosterName != "Chero")
                            //    return;
                        }
                        else if (_NewPost.m_PostURL.Contains("wow-one.com") == true)
                        {
                            minDate = DateTime.Now.AddDays(-14);
                            forumType = ForumReader.ForumType.FeenixForum;
                            if (_NewPost.m_PosterName != "Athairne"
                                && _NewPost.m_PosterName != "PermaFrost"
                                && _NewPost.m_PosterName != "Feenixes"
                                && _NewPost.m_PosterName != "VSupport"
                                && _NewPost.m_PosterName != "Forcy"
                                && _NewPost.m_PosterName != "Danut"
                                && _NewPost.m_PosterName != "ender"
                                && _NewPost.m_PosterName != "Aidas"
                                && _NewPost.m_PosterName != "voldemort"
                                && _NewPost.m_PosterName != "Thetruecrow"
                                && _NewPost.m_PosterName != "Emphar"
                                && _NewPost.m_PosterName != "Mardant"
                                && _NewPost.m_PosterName != "technique"
                                && _NewPost.m_PosterName != "Sideways"
                                && _NewPost.m_PosterName != "Bronson"
                                && _NewPost.m_PosterName != "Neforai"
                                && _NewPost.m_PosterName != "Database"
                                && _NewPost.m_PosterName != "zeroxas"
                                && _NewPost.m_PosterName != "anaaz"
                                && _NewPost.m_PosterName != "Doxis"
                                && _NewPost.m_PosterName != "Mr.Justice"
                                && _NewPost.m_PosterName != "TheDoctor"
                                && _NewPost.m_PosterName != "Ribbit"
                                && _NewPost.m_PosterName != "Vidotrieth"
                                && _NewPost.m_PosterName != "Tyale"
                                && _NewPost.m_PosterName != "64bytes"
                                && _NewPost.m_PosterName != "programuotojas"
                                && _NewPost.m_PosterName != "Ksenovia")
                                return;
                        }
                        else if (_NewPost.m_PostURL.Contains("twitter") == true)
                        {
                            minDate = DateTime.Now.AddDays(-7);
                            absoluteMinDate = DateTime.Now.AddDays(-14);
                            forumType = ForumReader.ForumType.Twitter;
                        }
                        else
                        { 
                            return;//Unknown forum type
                        }
                        string threadNameLower = _NewPost.m_ThreadName.ToLower();
                        if (forumType == ForumReader.ForumType.FeenixForum && Settings.HaveTBC == false && (threadNameLower.Contains("archangel") || threadNameLower.Contains("2.4.3") || threadNameLower.Contains("area 52")))
                        {
                            if (threadNameLower.Contains("emerald dream") || threadNameLower.Contains("warsong") || threadNameLower.Contains("al'akir") || threadNameLower.Contains("1.12.1") || _NewPost.m_ThreadName.Contains("ED"))
                            {

                            }
                            else
                            {
                                //Skip this Thread
                                Logger.ConsoleWriteLine("Skipped thread " + _NewPost.m_ThreadName + " because it seems to be about Archangel/2.4.3 server only");
                                return;
                            }
                        }
                        if (_NewPost.m_PostDate > minDate && _NewPost.m_State != ForumReader.ForumPost.State.Read)
                        {
                            c_dlAddons.BeginInvoke(new Action(() =>
                            {
                                Image image;

                                if (_NewPost.m_State == ForumReader.ForumPost.State.NewThisSession || (DateTime.Now - _NewPost.m_PostDate).TotalHours < 48)
                                {
                                    _NewPost.m_State = ForumReader.ForumPost.State.Unread;

                                    if (forumType == ForumReader.ForumType.FeenixForum)
                                    {
                                        image = Properties.Resources.f_icon;
                                    }
                                    else if (forumType == ForumReader.ForumType.RealmPlayersForum)
                                    {
                                        image = Properties.Resources.topic_unread;
                                    }
                                    else if (forumType == ForumReader.ForumType.NostalriusForum)
                                    {
                                        image = Properties.Resources.topic_unread_nos;
                                    }
                                    else if (forumType == ForumReader.ForumType.KronosForum)
                                    {
                                        image = Properties.Resources.newkronosthread32x33;
                                    }
                                    else if (forumType == ForumReader.ForumType.Twitter)
                                    {
                                        if (_NewPost.m_PosterName.StartsWith("realmplayers"))
                                            image = Properties.Resources.realmplayers32;
                                        else if (_NewPost.m_PosterName.StartsWith("kronos"))
                                            image = Properties.Resources.kronos32;
                                        else if (_NewPost.m_PosterName.StartsWith("elysium"))
                                            image = Properties.Resources.elysium32;
                                        else
                                            image = Properties.Resources.twitter32;
                                    }
                                    else
                                    {
                                        image = Properties.Resources.topic_unread;
                                    }
                                    ForumReader.TriggerSaveForumCache();
                                }
                                else
                                {
                                    if (forumType == ForumReader.ForumType.FeenixForum)
                                    {
                                        image = Properties.Resources.f_icon_read;
                                    }
                                    else if (forumType == ForumReader.ForumType.RealmPlayersForum)
                                    {
                                        image = Properties.Resources.topic_read;
                                    }
                                    else if (forumType == ForumReader.ForumType.NostalriusForum)
                                    {
                                        image = Properties.Resources.topic_read_nos;
                                    }
                                    else if (forumType == ForumReader.ForumType.KronosForum)
                                    {
                                        image = Properties.Resources.oldkronosthread32x33;
                                    }
                                    else if (forumType == ForumReader.ForumType.Twitter)
                                    {
                                        if (_NewPost.m_PosterName.StartsWith("realmplayers"))
                                            image = Properties.Resources.realmplayers32read;
                                        else if (_NewPost.m_PosterName.StartsWith("kronos"))
                                            image = Properties.Resources.kronos32read;
                                        else if (_NewPost.m_PosterName.StartsWith("elysium"))
                                            image = Properties.Resources.elysium32_read;
                                        else
                                            image = Properties.Resources.twitter32read;
                                    }
                                    else
                                    {
                                        image = Properties.Resources.topic_read;
                                    }
                                }

                                c_dlAddons.AddItem(int.MinValue + (int)((_NewPost.m_PostDate - absoluteMinDate).TotalMinutes)
                                    , image
                                    , (_NewPost.m_ThreadName.Length > 56 ? _NewPost.m_ThreadName.Substring(0, 50) + "..." : _NewPost.m_ThreadName), (_NewPost.m_PostContent.Length > 1200 ? "This post is long! Click \"Goto thread\" to read the full post on the forum.\r\n\r\n" + _NewPost.m_PostContent.Substring(0, 1024) + "..." : _NewPost.m_PostContent), _NewPost.m_PostDate.ToString("yyyy-MM-dd HH:mm:ss") + " by " + _NewPost.m_PosterName
                                    , new DetailedList.RightSideForumPost(() =>
                                    {
                                        //System.Diagnostics.Process.Start(_NewPost.m_ThreadURL + "page__view__getlastpost");
                                        System.Diagnostics.Process.Start(_NewPost.m_PostURL);
                                    }, (_ListItem) =>
                                    {
                                        _NewPost.m_State = ForumReader.ForumPost.State.Read;
                                        ForumReader.TriggerSaveForumCache();
                                        c_dlAddons.RemoveItem(_ListItem);
                                    }));
                            }));
                        }
                    };
                    if (Settings.Instance.NewsSources_Kronos == true)
                    {
                        ForumReader.GetLatestPosts(new string[] { "kronoswow" }
                            , newPostLambda, ForumReader.ForumType.Twitter, onlyNewest);
                    }
                    if (Settings.Instance.NewsSources_Nostalrius == true)
                    {
                        ForumReader.GetLatestPosts(new string[] { "elysium_dev" }
                            , newPostLambda, ForumReader.ForumType.Twitter, onlyNewest);
                    }

                    ForumReader.GetLatestPosts(new string[] { "realmplayers" }
                        , newPostLambda, ForumReader.ForumType.Twitter, onlyNewest);

                    ForumReader.GetLatestPosts(new string[]{"http://forum.realmplayers.com/viewforum.php?f=14"
                        , "http://forum.realmplayers.com/viewforum.php?f=15"
                        , "http://forum.realmplayers.com/viewforum.php?f=16"
                        , "http://forum.realmplayers.com/viewforum.php?f=17"
                        , "http://forum.realmplayers.com/viewforum.php?f=13"
                        , "http://forum.realmplayers.com/viewforum.php?f=6"
                        , "http://forum.realmplayers.com/viewforum.php?f=8"
                        , "http://forum.realmplayers.com/viewforum.php?f=10"}
                        , newPostLambda, ForumReader.ForumType.RealmPlayersForum, onlyNewest);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                m_PollingForNews = false;
            })).Start();
        }
        void GetLatestAddonUpdates()
        {
            (new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    foreach (var wowVersion in new WowVersionEnum[] { WowVersionEnum.Vanilla, WowVersionEnum.TBC })
                    {
                        var installedAddons = InstalledAddons.GetInstalledAddons(wowVersion);

                        if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == true)
                        {
                            var userIDAddons = new List<string>();
                            if (wowVersion == WowVersionEnum.Vanilla)
                            {
                                if (Settings.HaveClassic == true && InstalledAddons.GetAddonInfo("VF_RealmPlayers", WowVersionEnum.Vanilla) == null)
                                {
                                    if (Settings.Instance.ContributeRealmPlayers == true)
                                    {
                                        installedAddons.Add("VF_RealmPlayers");
                                    }
                                }
                                if (Settings.HaveClassic == true
                                    && InstalledAddons.GetAddonInfo("VF_RaidDamage", WowVersionEnum.Vanilla) == null
                                    && InstalledAddons.GetAddonInfo("VF_RaidStats", WowVersionEnum.Vanilla) == null)
                                {
                                    if (Settings.Instance.ContributeRaidStats == true)
                                    {
                                        //if (InstalledAddons.GetAddonInfo("SW_Stats", WowVersion.Vanilla) != null && InstalledAddons.GetAddonInfo("KLHThreatMeter", WowVersion.Vanilla) != null)
                                        //{
                                        installedAddons.Add("VF_RaidDamage");
                                        installedAddons.Add("VF_RaidStats");
                                        //}
                                    }
                                }
                            }
                            else if (wowVersion == WowVersionEnum.TBC)
                            {
                                if (Settings.HaveTBC == true && InstalledAddons.GetAddonInfo("VF_RealmPlayersTBC", WowVersionEnum.TBC) == null)
                                {
                                    if (Settings.Instance.ContributeRealmPlayers == true)
                                    {
                                        installedAddons.Add("VF_RealmPlayersTBC");
                                    }
                                }
                                if (Settings.HaveTBC == true && InstalledAddons.GetAddonInfo("VF_RaidStatsTBC", WowVersionEnum.TBC) == null)
                                {
                                    if (Settings.Instance.ContributeRaidStats == true)
                                    {
                                        installedAddons.Add("VF_RaidStatsTBC");
                                    }
                                }
                            }
                        }
                        var addonUpdateInfos = ServerComm.GetAddonUpdateInfos(installedAddons, wowVersion);
                        foreach (var addonUpdateInfo in addonUpdateInfos)
                        {
                            string addonVersionTitle = addonUpdateInfo.AddonName;
                            string updateOrInstallBy;
                            string updateOrInstallSuccessfullMessage;
                            string installFailedText;
                            string updateButtonText;
                            if (addonUpdateInfo.CurrentVersion != "0.0")
                            {
                                addonVersionTitle += " " + addonUpdateInfo.CurrentVersion + "->" + addonUpdateInfo.UpdateVersion;
                                updateOrInstallBy = "Update by " + addonUpdateInfo.UpdateSubmitter;
                                updateOrInstallSuccessfullMessage = "Addon Update " + addonUpdateInfo.UpdateVersion + " for addon " + addonUpdateInfo.AddonName + " was successfully installed!";
                                installFailedText = "Could not install update for addon " + addonUpdateInfo.AddonName + "\r\nReason: Download of addon failed";
                                updateButtonText = "Update";
                            }
                            else
                            {
                                updateOrInstallBy = "Made by " + addonUpdateInfo.UpdateSubmitter;
                                updateOrInstallSuccessfullMessage = addonUpdateInfo.AddonName + " was successfully installed!";
                                installFailedText = "Could not install " + addonUpdateInfo.AddonName + "\r\nReason: Download of addon failed";
                                updateButtonText = "Install";
                            }
                            System.Threading.Thread.Sleep(20);
                            c_dlAddons.BeginInvoke(new Action(() =>
                            {
                                c_dlAddons.AddItem((int)addonUpdateInfo.UpdateImportance
                                    , GetAddonUpdateImage(addonUpdateInfo.UpdateImportance)
                                    , addonVersionTitle
                                    , addonUpdateInfo.UpdateDescription, updateOrInstallBy
                                    , new DetailedList.RightSideAddonUpdate(updateButtonText, (_ListItem, _SetProgressBarFunc) =>
                                    {
                                        try
                                        {
                                            _SetProgressBarFunc(0.0f);
                                            string addonPackageFile = ServerComm.DownloadAddonPackage(addonUpdateInfo.AddonPackageDownloadFTP, addonUpdateInfo.AddonPackageFileSize, (float _DownloadPercentage) => { _SetProgressBarFunc(0.5f * _DownloadPercentage); });
                                            _SetProgressBarFunc(0.5f);
                                            if (addonPackageFile != "")
                                            {
                                                var updateAddons = InstalledAddons.GetAddonsInAddonPackage(addonPackageFile);
                                                var updatedAddons = InstalledAddons.InstallAddonPackage(addonPackageFile, wowVersion, (float _InstallPercentage) => { _SetProgressBarFunc(0.5f + 0.5f * _InstallPercentage); }, addonUpdateInfo.ClearAccountSavedVariablesRequired || addonUpdateInfo.ClearCharacterSavedVariablesRequired);
                                                if (updatedAddons != null && updatedAddons.Count > 0)
                                                {
                                                    _SetProgressBarFunc(1.0f);
                                                    Utility.MessageBoxShow(updateOrInstallSuccessfullMessage);
                                                    c_dlAddons.BeginInvoke(new Action(() =>
                                                    {
                                                        c_dlAddons.RemoveItem(_ListItem);
                                                    }));
                                                }
                                            }
                                            else
                                            {
                                                Utility.MessageBoxShow(installFailedText);
                                            }
                                            //c_dlAddons.BeginInvoke(new Action(() =>
                                            //{
                                            //    c_dlAddons.RemoveItem(_ListItem);
                                            //}));
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.LogException(ex);
                                        }
                                    }, () =>
                                    {
                                        try
                                        {
                                            if (addonUpdateInfo.MoreInfoSite == "") 
                                                Utility.MessageBoxShow("Could not find more info for this addon" + (addonUpdateInfo.AddonName.StartsWith("VF_") ? ", full changelog is always available on the forum: forum.realmplayers.com" : "."));
                                            else if (addonUpdateInfo.MoreInfoSite.StartsWith("http://"))
                                                System.Diagnostics.Process.Start(addonUpdateInfo.MoreInfoSite);
                                            else
                                                Utility.MessageBoxShow("Unknown \"More Info\" format, Your Launcher may be outdated.");
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.LogException(ex);
                                        }
                                    }));
                                //Utility.MessageBoxShow("Updated" + addonUpdateInfo.AddonName);
                                //c_dlAddons.Refresh();
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            })).Start();
        }
        void LauncherWindow_PostLoad(object sender, EventArgs e)
        {
            //Only run first time, sort of as a delayed Load
            this.Activated -= LauncherWindow_PostLoad;
            if (Settings.GetWowDirectory(WowVersionEnum.Vanilla) == "")
            {
                SetupWowDirectory.ShowSetupWowDirectory();
                if (Settings.GetWowDirectory(WowVersionEnum.Vanilla) == "")
                {
                    Application.Exit();
                    return;
                }
            }
            if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == false)
            {
                if ((Settings.FirstTimeRunning == true && (WowUtility.IsAddonInstalled("VF_RealmPlayers", WowVersionEnum.Vanilla) || WowUtility.IsAddonInstalled("VF_RaidDamage", WowVersionEnum.Vanilla)))
                /*|| RealmPlayersUploader.IsDatabasesNotUploaded() == true*/)
                {
                    SetupUserID.ShowSetupUserID();
                }
            }

            if (StaticValues.StartupArguments["LaunchWow"] != null)
            {
                string useRealm = StaticValues.StartupArguments["LaunchWow"];
                foreach (string item in c_ddlRealm.Items)
                {
                    if (item == useRealm)
                    {
                        c_ddlRealm.SelectedItem = item;
                        c_btnLaunch_Click(null, null);
                        return;
                    }
                }
                Utility.MessageBoxShow("There was no Realm named \"" + useRealm + "\"");
            }
            GetLatestNews(true);

            GetLatestAddonUpdates();
            //GetLatestUserIDAddons();
            //InstalledAddons.BackupAddon("VF_RealmPlayers");

            this.Activated += LauncherWindow_Activated;
            LauncherWindow_Activated(sender, e); //Have to manually call it the first time
        }

        void LauncherWindow_Activated(object sender, EventArgs e)
        {
            c_dlAddons.Focus();
        }

        protected override void WndProc(ref Message _Message)
        {
            if (Program.g_LauncherApp.HandleJumpListMessages(this, ref _Message) == false)
                base.WndProc(ref _Message);
        }

        private static Image GetAddonUpdateImage(ServerComm.UpdateImportance _UpdateImportance)
        {
            if (_UpdateImportance == ServerComm.UpdateImportance.Critical || _UpdateImportance == ServerComm.UpdateImportance.Very_Important)
            {
                return Properties.Resources.update_critical32;
            }
            else if (_UpdateImportance == ServerComm.UpdateImportance.Important)
            {
                return Properties.Resources.update_recommended32;
            }
            else if (_UpdateImportance == ServerComm.UpdateImportance.Good)
            {
                return Properties.Resources.update_misc32;
            }
            else
            {
                return Properties.Resources.update_misc32;
            }
        }
        void LauncherWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Instance.LauncherWindow_Top = this.Top;
            Settings.Instance.LauncherWindow_Left = this.Left;
        }

        private void LauncherWindow_Load(object sender, EventArgs e)
        {
            if (Settings.Instance.LauncherWindow_Left == -1 && Settings.Instance.LauncherWindow_Top == -1)
            {
                this.Top = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
                this.Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
            }
            else
            {
                this.Top = Settings.Instance.LauncherWindow_Top;
                this.Left = Settings.Instance.LauncherWindow_Left;
            }
            (new System.Threading.Tasks.Task(() =>
            {
                string updateData = "";
                var updateAvailable = wyUpdateControl.CheckIsUpdateAvailable(out updateData);
                if (updateAvailable == true)
                {
                    c_dlAddons.BeginInvoke(new Action(() =>
                    {
                        c_dlAddons.AddItem(int.MaxValue - 1
                            , GetAddonUpdateImage(ServerComm.UpdateImportance.Important)
                            , "New VF_WoWLauncher Version Available!"
                            , updateData, "Made by Dilatazu"
                            , new DetailedList.RightSideAddonUpdate("Install", (_ListItem, _SetProgressBarFunc) =>
                            {
                                wyUpdateControl.UpdateNow();
                            }, () =>
                            {
                                System.Diagnostics.Process.Start("http://forum.realmplayers.com/viewtopic.php?f=16&t=7");
                            })
                        );
                    }));
                }
            })).Start();
            this.Text = "VF_WoWLauncher " + StaticValues.LauncherVersion;
            //this.TopMost = true;

            UpdatePossibleRealmConfigurations();
            c_ddlRealm.SelectedItem = Settings.Instance.DefaultRealm;
            c_cbClearWDB.Checked = Settings.Instance.ClearWDB;
            InitializeConfigDDL(Settings.Instance.DefaultConfig);
            c_frmLogWindow = new LogWindow(this);

        }
        private void InitializeConfigDDL(string _SelectedConfig)
        {
            c_ddlConfigProfile.Items.Clear();
            c_ddlConfigProfile.Items.Add("Active Wow Config");
            c_ddlConfigProfile.Items.AddRange(ConfigProfiles.GetProfileNames().ToArray());
            c_ddlConfigProfile.SelectedItem = _SelectedConfig;
            
            if (StaticValues.StartupArguments["ConfigProfile"] != null)
            {
                string useConfigProfile = StaticValues.StartupArguments["ConfigProfile"];
                foreach (string item in c_ddlConfigProfile.Items)
                {
                    if (item == useConfigProfile)
                    {
                        c_ddlConfigProfile.SelectedItem = item;
                        return;
                    }
                }
                Utility.MessageBoxShow("There was no ConfigProfile named \"" + useConfigProfile + "\"");
            }
        }
        private void c_btnLaunch_Click(object sender, EventArgs e)
        {
            if(c_ddlRealm.SelectedItem == null)
            {
                Utility.MessageBoxShow("You have to choose a realm first!");
                return;
            }
            if (Settings.Instance.RealmLists.ContainsKey((string)c_ddlRealm.SelectedItem) == false)
                return;

            c_btnLaunch.Enabled = false;

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            var realmInfo = Settings.Instance.RealmLists[(string)c_ddlRealm.SelectedItem];

            if (realmInfo.WowVersion == WowVersionEnum.TBC)
            {
                if (c_cbClearWDB.Checked == true)
                {
                    Utility.DeleteDirectory(Settings.GetWowDirectory(realmInfo.WowVersion) + "Cache");
                }
            }
            else
            {
                if (c_cbClearWDB.Checked == true)
                {
                    string[] files = System.IO.Directory.GetFiles(Settings.GetWowDirectory(realmInfo.WowVersion) + "WDB\\");
                    foreach (var file in files)
                    {
                        System.IO.File.Delete(file);
                        Logger.LogText("ClearWDB: Deleted File: " + file);
                    }
                }
            }

            if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == true)
            {
                if (System.IO.File.Exists(StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat") == false
                    || System.IO.File.ReadAllText(StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat") != StaticValues.RunWowAndUploaderBatFileData)
                {
                    Utility.AssertDirectory(StaticValues.LauncherToolsDirectory);
                    System.IO.File.WriteAllText(StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat", StaticValues.RunWowAndUploaderBatFileData);
                }

                if (Settings.Instance.RunWoWNotAdmin == false)
                {
                    //startInfo.FileName = Settings.WowDirectory + "WoW.exe";
                    //startInfo.WorkingDirectory = Settings.WowDirectory;

                    startInfo.FileName = StaticValues.LauncherToolsDirectory + "RunWowAndUploader.bat";
                    //startInfo.FileName = Settings.WowDirectory + "22VF_RealmPlayersUploader 1.5\\RunWoWAndUploaderNoCMDWindow.vbs";
                    //startInfo.WorkingDirectory = Settings.WowDirectory + "22VF_RealmPlayersUploader 1.5\\";
                    startInfo.Arguments = "\"" + Settings.GetWowDirectory(realmInfo.WowVersion) + "\"";
                }
                else
                {
                    string slash = "\\\\";
                    string snuff = "\\\"";

                    startInfo.FileName = StaticValues.LauncherToolsDirectory + "NotAdmin.exe";
                    startInfo.Arguments = "\"cmd.exe\" \".\\\\\" " 
                        + "\"/c " 
                            + snuff
                                + snuff + StaticValues.LauncherWorkDirectory.Replace("\\", slash) + "/" + StaticValues.LauncherToolsDirectory.Replace("\\", slash) + "RunWowAndUploader.bat" + snuff
                                + " " + snuff + Settings.GetWowDirectory(realmInfo.WowVersion) + "\\" + snuff
                            + snuff 
                        + "\" nowindow";
                }
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = false;
                startInfo.CreateNoWindow = true;


                //NotAdmin.exe "cmd.exe" ".\\" "/c \"\"D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Release\\VF_WowLauncherTools\\RunWowAndUploader.bat\" \"D:\Program\World of Warcraft Classic\\\" nowindow\"";
            }
            else
            {
                startInfo.FileName = Settings.GetWowDirectory(realmInfo.WowVersion) + "WoW.exe";
                startInfo.WorkingDirectory = Settings.GetWowDirectory(realmInfo.WowVersion);
            }

            LaunchFunctions.LaunchWow((string)c_ddlConfigProfile.SelectedItem, (string)c_ddlRealm.SelectedItem, startInfo);
            c_btnLaunch.Enabled = true;
        }

        private void c_btnConfig_Click(object sender, EventArgs e)
        {
            if ((string)c_ddlConfigProfile.SelectedItem == "Active Wow Config")
            {
                ConfigSettings.EditWTFConfigSettings(WowVersionEnum.Vanilla);
                InitializeConfigDDL((string)c_ddlConfigProfile.SelectedItem);
            }
            else
            {
                ConfigSettings.EditProfileConfig((string)c_ddlConfigProfile.SelectedItem);
                InitializeConfigDDL((string)c_ddlConfigProfile.SelectedItem);
            }
        }

        private void c_ddlConfigProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance.DefaultConfig = (string)c_ddlConfigProfile.SelectedItem;
        }

        private void c_ddlRealm_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance.DefaultRealm = (string)c_ddlRealm.SelectedItem;

            if (Settings.Instance.RealmLists.ContainsKey(Settings.Instance.DefaultRealm) == false)
                return;

            var realmInfo = Settings.Instance.RealmLists[Settings.Instance.DefaultRealm];

            if (realmInfo.WowVersion == WowVersionEnum.TBC)
            {
                c_ddlConfigProfile.SelectedItem = "Active Wow Config";
                c_ddlConfigProfile.Enabled = false;
            }
            else
            {
                c_ddlConfigProfile.Enabled = true;
            }
        }

        private void c_cbClearWDB_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.ClearWDB = c_cbClearWDB.Checked;
        }

        void LauncherWindow_Move(object sender, EventArgs e)
        {
            if (c_frmLogWindow != null)
            {
                if(c_frmLogWindow.Visible == true)
                    c_frmLogWindow.MoveTo(this.Top - 5, this.Left + this.Width + 5);
            }
        }
        public void MoveTo(int _Top, int _Left)
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.Move -= LauncherWindow_Move;
                    this.Top = _Top;
                    this.Left = _Left;
                    this.Move += LauncherWindow_Move;
                }));
            }
            catch (Exception)
            {}
        }

        LogWindow c_frmLogWindow = null;
        private void c_btnShowLog_Click(object sender, EventArgs e)
        {
            if (c_frmLogWindow == null)
                c_frmLogWindow = new LogWindow(this);
            if (c_frmLogWindow.Visible == false)
            {
                c_frmLogWindow.Show();
                LauncherWindow_Move(this, e);
            }
            else
                c_frmLogWindow.Hide();
        }

        private void c_btnManageAddons_Click(object sender, EventArgs e)
        {
            AddonsSettings.ShowAddonsSettings();
        }

        private void c_dlAddons_Load(object sender, EventArgs e)
        {

        }

        private void c_tRefreshNews_Tick(object sender, EventArgs e)
        {
            GetLatestNews(false);
        }

        private void c_tRefreshAddonUpdates_Tick(object sender, EventArgs e)
        {
            (new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    if (ServerComm.PeekAddonUpdates(15/*15 is for a reason even though the interval this timer ticks at is 10 minutes*/) == true)
                    {
                        GetLatestAddonUpdates();
                        //GetLatestUserIDAddons();
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                }
            })).Start();
        }

        private void c_btnManageRealmLists_Click(object sender, EventArgs e)
        {
            RealmListSettings.ShowRealmListSettings();
            UpdatePossibleRealmConfigurations();
        }
    }
}
