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
    public partial class LauncherWindow : Form
    {
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
                    if (Settings.HaveTBC == true)
                    {
                        if (c_ddlRealm.Items.Contains("Archangel(TBC)") == false)
                            c_ddlRealm.Items.Add("Archangel(TBC)");
                    }
                    else
                    {
                        if (c_ddlRealm.Items.Contains("Archangel(TBC)") == true)
                        {
                            if ((string)c_ddlRealm.SelectedItem == "Archangel(TBC)")
                                c_ddlRealm.SelectedItem = "Emerald Dream";
                            c_ddlRealm.Items.Remove("Archangel(TBC)");
                        }
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
                    if (oldUserID != Settings.UserID
                    || oldWowFolder != Settings.Instance._WowDirectory
                    || oldWowTBCFolder != Settings.Instance._WowTBCDirectory)
                    {
                        GetLatestAddonUpdates();
                        GetLatestUserIDAddons();
                    }
                })));
                fileMenu.MenuItems.Add(new MenuItem("Close", new EventHandler((o, ea) => Close())));

                this.Menu.MenuItems.Add(fileMenu);
            }

            {
                var linksMenu = new MenuItem("Links");
                {
                    var feenixLinks = new MenuItem("Feenix Links");
                    feenixLinks.MenuItems.Add(new MenuItem("Goto forum", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://www.wow-one.com/forum");
                    })));
                    feenixLinks.MenuItems.Add(new MenuItem("Goto server updates", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://www.wow-one.com/forum/117-server-updates/");
                    })));
                    feenixLinks.MenuItems.Add(new MenuItem("Goto changelogs", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://www.wow-one.com/forum/32-1121-changelogs/");
                    })));
                    feenixLinks.MenuItems.Add(new MenuItem("Goto issue tracker", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://github.com/FeenixServerProject/Phoenix_1.12.1_Issue_tracker/issues");
                    })));
                    feenixLinks.MenuItems.Add(new MenuItem("Goto database", new EventHandler((o, ea) =>
                    {
                        System.Diagnostics.Process.Start("http://database.wow-one.com/");
                    })));
                    linksMenu.MenuItems.Add(feenixLinks);
                }
                linksMenu.MenuItems.Add("-");
                linksMenu.MenuItems.Add(new MenuItem("Goto RealmPlayers", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://realmplayers.com");
                })));
                linksMenu.MenuItems.Add(new MenuItem("Goto RaidStats", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://realmplayers.com/RaidStats");
                })));
                linksMenu.MenuItems.Add(new MenuItem("Goto RealmPlayers Forum", new EventHandler((o, ea) =>
                {
                    System.Diagnostics.Process.Start("http://forum.realmplayers.com");
                })));
                this.Menu.MenuItems.Add(linksMenu);
            }
            {
                var toolsMenu = new MenuItem("Tools");
                //toolsMenu.MenuItems.Add(new MenuItem("Addons Settings(Experimental)", new EventHandler((o, ea) => AccountSettings.ShowAccountSettings())));
                toolsMenu.MenuItems.Add(new MenuItem("Addons Manager", new EventHandler((o, ea) => AddonsSettings.ShowAddonsSettings())));
                toolsMenu.MenuItems.Add("-");
                toolsMenu.MenuItems.Add(new MenuItem("Create AddonPackage(Experimental)", 
                    new EventHandler((o, ea) => {
                        CreateAddonPackageForm newForm = new CreateAddonPackageForm();
                        newForm.ShowDialog();
                    })));
                toolsMenu.MenuItems.Add(new MenuItem("Poll wow-one for News", new EventHandler((o, ea) => GetLatestNews(false))));
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
                    DateTime minDate = DateTime.Now.AddDays(-14);
                    ForumReader.GetLatestPosts(new string[]{"http://www.wow-one.com/forum/117-server-updates/"
                        , "http://www.wow-one.com/forum/192-information-and-releases/"
                        , "http://www.wow-one.com/forum/3-news-and-announcements/"
                        , "http://www.wow-one.com/forum/32-1121-changelogs/"}
                        , (ForumReader.ForumPost _NewPost) =>
                        {
                            string threadNameLower = _NewPost.m_ThreadName.ToLower();
                            if (Settings.HaveTBC == false && (threadNameLower.Contains("archangel") || threadNameLower.Contains("2.4.3")))
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
                                    var image = Properties.Resources.f_icon_read;
                                    if (_NewPost.m_State == ForumReader.ForumPost.State.NewThisSession || (DateTime.Now - _NewPost.m_PostDate).TotalHours < 48)
                                    {
                                        _NewPost.m_State = ForumReader.ForumPost.State.Unread;
                                        image = Properties.Resources.f_icon;
                                        ForumReader.TriggerSaveForumCache();
                                    }
                                    c_dlAddons.AddItem(int.MinValue + (int)((_NewPost.m_PostDate - minDate).TotalMinutes)
                                        , image
                                        , _NewPost.m_ThreadName, (_NewPost.m_PostContent.Length > 1200 ? "This post is long! Click \"Goto thread\" to read the full post on the forum.\r\n\r\n" + _NewPost.m_PostContent.Substring(0, 1024) + "..." : _NewPost.m_PostContent), _NewPost.m_PostDate.ToString("yyyy-MM-dd HH:mm:ss") + " by " + _NewPost.m_PosterName
                                        , new DetailedList.RightSideForumPost(() =>
                                        {
                                            System.Diagnostics.Process.Start(_NewPost.m_ThreadURL + "page__view__getlastpost");
                                        }, (_ListItem) =>
                                        {
                                            _NewPost.m_State = ForumReader.ForumPost.State.Read;
                                            ForumReader.TriggerSaveForumCache();
                                            c_dlAddons.RemoveItem(_ListItem);
                                        }));
                                }));
                            }
                        }, onlyNewest);
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
                    foreach (var wowVersion in new WowVersion[] { WowVersion.Vanilla, WowVersion.TBC })
                    {
                        var installedAddons = InstalledAddons.GetInstalledAddons(wowVersion);
                        var addonUpdateInfos = ServerComm.GetAddonUpdateInfos(installedAddons, wowVersion);
                        foreach (var addonUpdateInfo in addonUpdateInfos)
                        {
                            System.Threading.Thread.Sleep(20);
                            c_dlAddons.BeginInvoke(new Action(() =>
                            {
                                c_dlAddons.AddItem((int)addonUpdateInfo.UpdateImportance
                                    , GetAddonUpdateImage(addonUpdateInfo.UpdateImportance)
                                    , addonUpdateInfo.AddonName + " " + addonUpdateInfo.CurrentVersion + "->" + addonUpdateInfo.UpdateVersion
                                    , addonUpdateInfo.UpdateDescription, "Update by " + addonUpdateInfo.UpdateSubmitter
                                    , new DetailedList.RightSideAddonUpdate((_ListItem, _SetProgressBarFunc) =>
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
                                                Utility.MessageBoxShow("Addon Update " + addonUpdateInfo.UpdateVersion + " for addon " + addonUpdateInfo.AddonName + " was successfully installed!");
                                                c_dlAddons.BeginInvoke(new Action(() =>
                                                {
                                                    c_dlAddons.RemoveItem(_ListItem);
                                                }));
                                            }
                                        }
                                        else
                                        {
                                            Utility.MessageBoxShow("Could not install update for addon " + addonUpdateInfo.AddonName + "\r\nReason: Download of addon failed");
                                        }
                                        //c_dlAddons.BeginInvoke(new Action(() =>
                                        //{
                                        //    c_dlAddons.RemoveItem(_ListItem);
                                        //}));
                                    }, () =>
                                    {
                                        if (addonUpdateInfo.MoreInfoSite == "") 
                                            Utility.MessageBoxShow("Could not find more info for this addon update, full changelog is always available on the forum: forum.realmplayers.com");
                                        else if (addonUpdateInfo.MoreInfoSite.StartsWith("http://"))
                                            System.Diagnostics.Process.Start(addonUpdateInfo.MoreInfoSite);
                                        else
                                            Utility.MessageBoxShow("Unknown \"More Info\" format, Your Launcher may be outdated.");
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
        void GetLatestUserIDAddons()
        {
            if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == true)
            {
                var userIDAddons = new List<string>();
                if (Settings.HaveClassic == true && InstalledAddons.GetAddonInfo("VF_RealmPlayers", WowVersion.Vanilla) == null)
                {
                    userIDAddons.Add("VF_RealmPlayers");
                }
                if (Settings.HaveClassic == true 
                    && InstalledAddons.GetAddonInfo("VF_RaidDamage", WowVersion.Vanilla) == null
                    && InstalledAddons.GetAddonInfo("VF_RaidStats", WowVersion.Vanilla) == null)
                {
                    //if (InstalledAddons.GetAddonInfo("SW_Stats", WowVersion.Vanilla) != null && InstalledAddons.GetAddonInfo("KLHThreatMeter", WowVersion.Vanilla) != null)
                    //{
                    userIDAddons.Add("VF_RaidDamage");
                    userIDAddons.Add("VF_RaidStats");
                    //}
                }
                if (Settings.HaveTBC == true && InstalledAddons.GetAddonInfo("VF_RealmPlayersTBC", WowVersion.TBC) == null)
                {
                    userIDAddons.Add("VF_RealmPlayersTBC");
                }
                if (Settings.HaveTBC == true && InstalledAddons.GetAddonInfo("VF_RaidStatsTBC", WowVersion.TBC) == null)
                {
                    userIDAddons.Add("VF_RaidStatsTBC");
                }
                if(userIDAddons.Count > 0)
                {
                    (new System.Threading.Tasks.Task(() =>
                    {
                        try
                        {
                            var addonUpdateInfosVanilla = ServerComm.GetAddonUpdateInfos(userIDAddons, WowVersion.Vanilla);
                            foreach (var addonUpdateInfo in addonUpdateInfosVanilla)
                            {
                                string addonDescription = "";
                                int sortIndex = 0;
                                if (addonUpdateInfo.AddonName == "VF_RealmPlayers")
                                {
                                    addonDescription = "Latest addon version for gathering data and contribute to the armory at realmplayers.com";
                                    sortIndex = int.MaxValue - 1;
                                }
                                else if (addonUpdateInfo.AddonName == "VF_RaidDamage")
                                {
                                    addonDescription = "Latest addon version for automatically logging data in raids. Logged raids will automatically be uploaded to RaidStats";
                                    sortIndex = int.MaxValue - 2;
                                }
                                else if (addonUpdateInfo.AddonName == "VF_RaidStats")
                                {
                                    addonDescription = "Latest addon version for automatically logging data in raids. Logged raids will automatically be uploaded to RaidStats";
                                    sortIndex = int.MaxValue - 3;
                                }
                                else if (addonUpdateInfo.AddonName == "VF_BGStats")
                                {
                                    addonDescription = "Latest addon version for automatically logging data in battlegrounds. Logged bgs will automatically be uploaded to BGStats";
                                    sortIndex = int.MaxValue - 4;
                                }
                                else
                                {
                                    continue;
                                }

                                c_dlAddons.BeginInvoke(new Action(() =>
                                {
                                    c_dlAddons.AddItem(sortIndex
                                        , GetAddonUpdateImage(ServerComm.UpdateImportance.Good)
                                        , addonUpdateInfo.AddonName
                                        , addonDescription, "Made by Dilatazu"
                                        , new DetailedList.RightSideAddonUpdate("Install", (_ListItem, _SetProgressBarFunc) =>
                                        {
                                            _SetProgressBarFunc(0.0f);
                                            string addonPackageFile = ServerComm.DownloadAddonPackage(addonUpdateInfo.AddonPackageDownloadFTP, addonUpdateInfo.AddonPackageFileSize, (float _DownloadPercentage) => { _SetProgressBarFunc(0.5f * _DownloadPercentage); });
                                            _SetProgressBarFunc(0.5f);
                                            if (addonPackageFile != "")
                                            {
                                                var updateAddons = InstalledAddons.GetAddonsInAddonPackage(addonPackageFile);
                                                var updatedAddons = InstalledAddons.InstallAddonPackage(addonPackageFile, WowVersion.Vanilla, (float _InstallPercentage) => { _SetProgressBarFunc(0.5f + 0.5f * _InstallPercentage); }, addonUpdateInfo.ClearAccountSavedVariablesRequired || addonUpdateInfo.ClearCharacterSavedVariablesRequired);
                                                if (updatedAddons != null && updatedAddons.Count > 0)
                                                {
                                                    _SetProgressBarFunc(1.0f);
                                                    Utility.MessageBoxShow(addonUpdateInfo.AddonName + " was successfully installed!");
                                                    c_dlAddons.BeginInvoke(new Action(() =>
                                                    {
                                                        c_dlAddons.RemoveItem(_ListItem);
                                                    }));
                                                }
                                            }
                                            else
                                            {
                                                Utility.MessageBoxShow("Could not install " + addonUpdateInfo.AddonName + "\r\nReason: Download of addon failed");
                                            }
                                            //c_dlAddons.BeginInvoke(new Action(() =>
                                            //{
                                            //    c_dlAddons.RemoveItem(_ListItem);
                                            //}));
                                        }, () =>
                                        {
                                            if (addonUpdateInfo.MoreInfoSite == "")
                                                Utility.MessageBoxShow("Could not find more info for this addon, Possibly more info and changelog on the forum: forum.realmplayers.com");
                                            else if (addonUpdateInfo.MoreInfoSite.StartsWith("http://"))
                                                System.Diagnostics.Process.Start(addonUpdateInfo.MoreInfoSite);
                                            else
                                                Utility.MessageBoxShow("Unknown \"More Info\" format, Your Launcher may be outdated.");
                                        })
                                    );
                                }));
                            }

                            #region TBC Copy pasted
                            var addonUpdateInfosTBC = ServerComm.GetAddonUpdateInfos(userIDAddons, WowVersion.TBC);
                            foreach (var addonUpdateInfo in addonUpdateInfosTBC)
                            {
                                string addonDescription = "";
                                int sortIndex = 0;
                                if (addonUpdateInfo.AddonName == "VF_RealmPlayersTBC")
                                {
                                    addonDescription = "Latest addon version for gathering data and contribute to the armory at realmplayers.com";
                                    sortIndex = int.MaxValue - 1;
                                }
                                else if (addonUpdateInfo.AddonName == "VF_RaidStatsTBC")
                                {
                                    addonDescription = "Latest addon version for automatically logging data in raids. Logged raids will automatically be uploaded to RaidStats";
                                    sortIndex = int.MaxValue - 1;
                                }
                                else if (addonUpdateInfo.AddonName == "VF_BGStatsTBC")
                                {
                                    addonDescription = "Latest addon version for automatically logging data in battlegrounds. Logged bgs will automatically be uploaded to BGStats";
                                    sortIndex = int.MaxValue - 1;
                                }
                                else
                                {
                                    continue;
                                }

                                c_dlAddons.BeginInvoke(new Action(() =>
                                {
                                    c_dlAddons.AddItem(sortIndex
                                        , GetAddonUpdateImage(ServerComm.UpdateImportance.Good)
                                        , addonUpdateInfo.AddonName
                                        , addonDescription, "Made by Dilatazu"
                                        , new DetailedList.RightSideAddonUpdate("Install", (_ListItem, _SetProgressBarFunc) =>
                                        {
                                            _SetProgressBarFunc(0.0f);
                                            string addonPackageFile = ServerComm.DownloadAddonPackage(addonUpdateInfo.AddonPackageDownloadFTP, addonUpdateInfo.AddonPackageFileSize, (float _DownloadPercentage) => { _SetProgressBarFunc(0.5f * _DownloadPercentage); });
                                            _SetProgressBarFunc(0.5f);
                                            if (Settings.HaveTBC == true && addonPackageFile != "")
                                            {
                                                var updateAddons = InstalledAddons.GetAddonsInAddonPackage(addonPackageFile);
                                                var updatedAddons = InstalledAddons.InstallAddonPackage(addonPackageFile, WowVersion.TBC, (float _InstallPercentage) => { _SetProgressBarFunc(0.5f + 0.5f * _InstallPercentage); }, addonUpdateInfo.ClearAccountSavedVariablesRequired || addonUpdateInfo.ClearCharacterSavedVariablesRequired);
                                                if (updatedAddons != null && updatedAddons.Count > 0)
                                                {
                                                    _SetProgressBarFunc(1.0f);
                                                    Utility.MessageBoxShow(addonUpdateInfo.AddonName + " was successfully installed!");
                                                    c_dlAddons.BeginInvoke(new Action(() =>
                                                    {
                                                        c_dlAddons.RemoveItem(_ListItem);
                                                    }));
                                                }
                                            }
                                            else
                                            {
                                                Utility.MessageBoxShow("Could not install " + addonUpdateInfo.AddonName + "\r\nReason: Download of addon failed");
                                            }
                                            //c_dlAddons.BeginInvoke(new Action(() =>
                                            //{
                                            //    c_dlAddons.RemoveItem(_ListItem);
                                            //}));
                                        }, () =>
                                        {
                                            if (addonUpdateInfo.MoreInfoSite == "")
                                                Utility.MessageBoxShow("Could not find more info for this addon, Possibly more info and changelog on the forum: forum.realmplayers.com");
                                            else if (addonUpdateInfo.MoreInfoSite.StartsWith("http://"))
                                                System.Diagnostics.Process.Start(addonUpdateInfo.MoreInfoSite);
                                            else
                                                Utility.MessageBoxShow("Unknown \"More Info\" format, Your Launcher may be outdated.");
                                        })
                                    );
                                }));
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                    })).Start();
                }
            }
        }
        void LauncherWindow_PostLoad(object sender, EventArgs e)
        {
            //Only run first time, sort of as a delayed Load
            this.Activated -= LauncherWindow_PostLoad;
            if (Settings.GetWowDirectory(WowVersion.Vanilla) == "")
            {
                SetupWowDirectory.ShowSetupWowDirectory();
                if (Settings.GetWowDirectory(WowVersion.Vanilla) == "")
                {
                    Application.Exit();
                    return;
                }
            }
            if (RealmPlayersUploader.IsValidUserID(Settings.UserID) == false)
            {
                if ((Settings.FirstTimeRunning == true && (WowUtility.IsAddonInstalled("VF_RealmPlayers", WowVersion.Vanilla) || WowUtility.IsAddonInstalled("VF_RaidDamage", WowVersion.Vanilla)))
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
            GetLatestUserIDAddons();
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
            if (Settings.HaveTBC == true)
                c_ddlRealm.Items.Add("Archangel(TBC)");
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
            c_btnLaunch.Enabled = false;

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            
            var wowVersion = WowVersion.Vanilla;
            if ((string)c_ddlRealm.SelectedItem == "Archangel(TBC)")
            {
                wowVersion = WowVersion.TBC;
                if (c_cbClearWDB.Checked == true)
                {
                    Utility.DeleteDirectory(Settings.GetWowDirectory(wowVersion) + "Cache");
                }
            }
            else
            {
                if (c_cbClearWDB.Checked == true)
                {
                    string[] files = System.IO.Directory.GetFiles(Settings.GetWowDirectory(wowVersion) + "WDB\\");
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
                    startInfo.Arguments = "\"" + Settings.GetWowDirectory(wowVersion) + "\"";
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
                                + " " + snuff + Settings.GetWowDirectory(wowVersion) + "\\" + snuff
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
                startInfo.FileName = Settings.GetWowDirectory(wowVersion) + "WoW.exe";
                startInfo.WorkingDirectory = Settings.GetWowDirectory(wowVersion);
            }

            LaunchFunctions.LaunchWow((string)c_ddlConfigProfile.SelectedItem, (string)c_ddlRealm.SelectedItem, startInfo);
            c_btnLaunch.Enabled = true;
        }

        private void c_btnConfig_Click(object sender, EventArgs e)
        {
            if ((string)c_ddlConfigProfile.SelectedItem == "Active Wow Config")
            {
                ConfigSettings.EditWTFConfigSettings(WowVersion.Vanilla);
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
            if (Settings.Instance.DefaultRealm == "Archangel(TBC)")
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
    }
}
