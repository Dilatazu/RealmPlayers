using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace VF_WoWLauncher
{
    class ForumReader
    {
        private static string _GetHTMLFile(string _Address)
        {
            if(Settings.DebugMode == true)
                Logger.ConsoleWriteLine("Downloading HTML file \"" + _Address + "\"");
            try
            {
                var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_Address);
                webRequest.Timeout = 10000;
                webRequest.ReadWriteTimeout = 10000;
                webRequest.Proxy = null;
                using (var webResponse = webRequest.GetResponse())
                {
                    var responseStream = webResponse.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(responseStream);

                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                if (Settings.DebugMode == true)
                    Logger.ConsoleWriteLine("Failed to Download HTML file \"" + _Address + "\"\nException: " + ex.ToString(), ConsoleColor.Red);
                throw;
            }
        }
        [ProtoContract]
        public class ForumPost
        {
            public enum State
            {
                NewThisSession = 0,
                Unread = 1,
                Read = 2,
            }
            [ProtoMember(1)]
            public string m_ThreadName;
            [ProtoMember(2)]
            public string m_ThreadURL;
            [ProtoMember(3)]
            public string m_PostURL;
            [ProtoMember(4)]
            public string m_PosterName;
            [ProtoMember(5)]
            public string m_PosterImageURL;
            [ProtoMember(6)]
            public string m_PostContent;
            [ProtoMember(7)]
            public DateTime m_PostDate = DateTime.MinValue;
            [ProtoMember(8)]
            public State m_State = State.NewThisSession;
        }
        private static List<ForumPost> GetThreadPosts(string _ThreadURL, DateTime _EarliestPostDate)
        {
            List<ForumPost> threadPosts = new List<ForumPost>();

            string website = _GetHTMLFile(_ThreadURL + "page__view__getlastpost");
            string threadName = website.SplitVF("<h1 class='ipsType_pagetitle'>", 2).Last().SplitVF("</h1>").First();
            threadName = System.Net.WebUtility.HtmlDecode(threadName.Replace("\t", "").Replace("\n", ""));

            string[] websitePart = website.SplitVF("<div class='post_wrap' >");

            for (int i = websitePart.Length - 1; i >= 1; --i)
            {
                try
                {
                    string postURL = websitePart[i].SplitVF("<span class='post_id right ipsType_small desc blend_links'><a href='", 2).Last().SplitVF("'").First();
                    string posterName = websitePart[i].SplitVF("title='View Profile'>", 2).Last().SplitVF("</a>").First();
                    string gravatarURL = "null";
                    try 
                    { 
                        gravatarURL = "http://www.gravatar.com" + websitePart[i].SplitVF("<img src='http://www.gravatar.com", 2).Last().SplitVF("'").First(); 
                    }
                    catch(Exception){}

                    string postBody = websitePart[i].SplitVF("<div class='post_body'>", 2).Last();
                    string dateString = postBody.SplitVF("<abbr class=\"published\" title=\"", 2).Last().SplitVF("\">").First();
                    DateTime postDate = DateTime.Parse(dateString);
                    if (_EarliestPostDate > postDate)
                    {
                        break;
                    }
                    string postContent = websitePart[i].SplitVF("<div class='post entry-content '>").Last().SplitVF("</div>").First().SplitVF("-->", 2).Last();

                    string[] postContentParts = postContent.Split('<');

                    string realContent = "";
                    try
                    {
                        foreach (var postContentPart in postContentParts)
                        {
                            if (postContentPart.Contains('>'))
                            {
                                if (postContentPart.StartsWith("li>") == true)
                                    realContent += "\n*";
                                realContent += postContentPart.Substring(postContentPart.IndexOf('>') + 1);
                            }
                            else
                                realContent += postContentPart;
                        }
                    }
                    catch (Exception)
                    {
                        realContent += "\n!!!COULD NOT READ REST OF THE POST!!!";
                    }
                    realContent = System.Net.WebUtility.HtmlDecode(realContent);
                    realContent = realContent.Replace("\t", "");

                    string[] cn = realContent.SplitVF("\n", StringSplitOptions.RemoveEmptyEntries);

                    realContent = "";
                    foreach (var c in cn)
                    {
                        realContent += c + "\n";
                    }
                    var newForumPost = new ForumPost { m_ThreadName = threadName, m_ThreadURL = _ThreadURL, m_PostURL = postURL, m_PosterName = posterName, m_PosterImageURL = gravatarURL, m_PostContent = realContent, m_PostDate = postDate };
                    threadPosts.Add(newForumPost);
                }
                catch (Exception)
                { }
            }
            return threadPosts;
        }
        [ProtoContract]
        public class ForumSection
        {
            [ProtoMember(1)]
            public string m_ForumSectionURL;
            [ProtoMember(2)]
            public DateTime m_LastPollDatTime = DateTime.MinValue;
            [ProtoMember(3)]
            public List<ForumPost> m_ForumPosts = new List<ForumPost>();
            [ProtoMember(4)]
            public Dictionary<string, DateTime> m_LastUpdatedThreads = new Dictionary<string, DateTime>();
            public bool m_DataUpdated = false;//No need to save this

            public void UpdateThread(string _ThreadURL, DateTime _LatestPost, Action<ForumPost> _RetPosts)
            {
                if (m_LastUpdatedThreads.ContainsKey(_ThreadURL) == false)
                    m_LastUpdatedThreads.Add(_ThreadURL, DateTime.MinValue.AddDays(10));

                if (m_LastUpdatedThreads[_ThreadURL] < _LatestPost)
                {
                    try
                    {
                        var newThreadPosts = GetThreadPosts(_ThreadURL, m_LastUpdatedThreads[_ThreadURL].AddDays(-1));

                        foreach (var newThreadPost in newThreadPosts)
                        {
                            if (m_ForumPosts.FirstOrDefault((_Value) => _Value.m_PostDate == newThreadPost.m_PostDate && _Value.m_PostContent == newThreadPost.m_PostContent) == default(ForumPost))
                            {
                                //Post doesnt allready exist, add it!
                                m_ForumPosts.Add(newThreadPost);
                                try
                                {
                                    _RetPosts(newThreadPost);
                                }
                                catch (Exception)
                                { }
                            }
                        }

                        m_LastUpdatedThreads[_ThreadURL] = _LatestPost;
                        m_DataUpdated = true;
                    }
                    catch (Exception)
                    { }
                }
            }
        }
        private static Dictionary<string, ForumSection> _sm_ForumSections = null;//new Dictionary<string, ForumSection>();
        private static object _sm_ForumSectionLock = new object();
        private static ForumSection GetForumSection(string _ForumAddress)
        {
            lock(_sm_ForumSectionLock)
            {
                if (_sm_ForumSections == null)
                {
                    if (System.IO.File.Exists(StaticValues.LauncherSettingsDirectory + "DownloadedForumCache.dat") == true)
                    {
                        if (Utility.LoadSerialize(StaticValues.LauncherSettingsDirectory + "DownloadedForumCache.dat", out _sm_ForumSections) == false)
                            _sm_ForumSections = null;
                    }
                    if (_sm_ForumSections == null)
                        _sm_ForumSections = new Dictionary<string, ForumSection>();
                }
                if (_sm_ForumSections.ContainsKey(_ForumAddress) == false)
                    _sm_ForumSections.Add(_ForumAddress, new ForumSection { m_ForumSectionURL = _ForumAddress });
            }
            return _sm_ForumSections[_ForumAddress];
        }
        public static void SaveForumSections()
        {
            if (_sm_ForumSections != null)
            {
                lock (_sm_ForumSectionLock)
                {
                    bool saveData = false;
                    foreach (var forumSection in _sm_ForumSections)
                    {
                        if (forumSection.Value.m_DataUpdated == true)
                            saveData = true;
                        forumSection.Value.m_DataUpdated = false;
                    }
                    if (saveData == true)
                    {
                        Utility.AssertFilePath(StaticValues.LauncherSettingsDirectory + "DownloadedForumCache.dat");
                        Utility.SaveSerialize(StaticValues.LauncherSettingsDirectory + "DownloadedForumCache.dat", _sm_ForumSections, false);
                    }
                }
            }
        }
        public static void TriggerSaveForumCache()
        {
            lock (_sm_ForumSectionLock)
            {
                foreach (var forumSection in _sm_ForumSections)
                {
                    forumSection.Value.m_DataUpdated = true;
                    break;
                }
            }
        }
        public static void GetLatestPosts(string[] _ForumAddresses, Action<ForumPost> _RetPosts, bool _OnlyNewest = false) //"http://www.wow-one.com/forum/117-server-updates/"
        {
            if (_OnlyNewest == false)
            {
                foreach (var forumAddress in _ForumAddresses)
                {
                    try
                    {
                        ForumSection forumSection = GetForumSection(forumAddress);
                        foreach (var threadPost in forumSection.m_ForumPosts)
                        {
                            _RetPosts(threadPost);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }
            }
            foreach (var forumAddress in _ForumAddresses)
            {
                try
                {
                    ForumSection forumSection = GetForumSection(forumAddress);
                    _GetLatestPosts(forumSection, _RetPosts);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            SaveForumSections();
        }
        public static void GetLatestPosts(string _ForumAddress, Action<ForumPost> _RetPosts, bool _OnlyNewest = false) //"http://www.wow-one.com/forum/117-server-updates/"
        {
            ForumSection forumSection = GetForumSection(_ForumAddress);

            if (_OnlyNewest == false)
            {
                foreach (var threadPost in forumSection.m_ForumPosts)
                {
                    _RetPosts(threadPost);
                }
            }
            _GetLatestPosts(forumSection, _RetPosts);
            SaveForumSections();
        }
        private static void _GetLatestPosts(ForumSection _ForumSection, Action<ForumPost> _RetPosts)
        {
            if ((DateTime.Now - _ForumSection.m_LastPollDatTime).TotalMinutes < 5)
                return;

            {
                string website = _GetHTMLFile(_ForumSection.m_ForumSectionURL);
                string[] websitePart = website.SplitVF("<td class='col_f_content '>");


                for (int i = 1; i < websitePart.Length; ++i)
                {
                    string currContent = websitePart[i].SplitVF("</td>", 2).First();
                    //<h4><a id="tid-link-66240" href="http://www.wow-one.com/forum/topic/66240-maintenance-notification-ed/" title='View topic, started  18 December 2013 - 08:27 AM' class='topic_title'>Maintenance Notification - ED</a></h4>
                    //<br />
                    //<span class='desc lighter blend_links'>
                    //Started by <a hovercard-ref="member" hovercard-id="36033" class="_hovertrigger url fn " href='http://www.wow-one.com/forum/user/36033-danut/'>Danut</a>, 18 Dec 2013
                    //</span>

                    string topicName = currContent.SplitVF("class='topic_title'>", 2).Last().SplitVF("</a>").First();
                    DateTime latestPostDate = DateTime.Now;
                    try
                    {
                        string dateStr = websitePart[i].SplitVF("page__view__getlastpost' title='Go to last post'>", 2).Last().SplitVF("</a>").First();
                        if (dateStr.StartsWith("Today") == true)
                        {
                            DateTime refDate = DateTime.Now;
                            latestPostDate = DateTime.Parse(dateStr.Replace("Yesterday,", "" + refDate.Day + " " + refDate.ToString("MMM") + " " + refDate.Year));
                        }
                        else if (dateStr.StartsWith("Yesterday") == true)
                        {
                            DateTime refDate = DateTime.Now.AddDays(-1);
                            latestPostDate = DateTime.Parse(dateStr.Replace("Yesterday,", "" + refDate.Day + " " + refDate.ToString("MMM") + " " + refDate.Year));
                        }
                        else
                        {
                            latestPostDate = DateTime.Parse(dateStr);
                        }
                    }
                    catch (Exception)
                    {
                        latestPostDate = DateTime.Now;
                    }

                    string threadLink = currContent.SplitVF("href=\"", 2).Last().SplitVF("\"").First();
                    //http://www.wow-one.com/forum/topic/66240-maintenance-notification-ed/

                    if (threadLink.StartsWith("http://www.wow-one.com/forum/topic") == false)
                        continue;

                    if((DateTime.Now - latestPostDate).TotalDays < 14)
                        _ForumSection.UpdateThread(threadLink, latestPostDate, _RetPosts);

                    //topics.Add(Tuple.Create(System.Net.WebUtility.HtmlDecode(topicName), topicLink));
                }
            }
            _ForumSection.m_LastPollDatTime = DateTime.Now;

            //foreach (var topic in topics)
            //{
            //    string website = _GetHTMLFile(topic.Item2);
            //    string[] websitePart = website.SplitVF("<div class='post_body'>");

            //    for (int i = websitePart.Length - 1; i >= 1; --i)
            //    {
            //        try
            //        {
            //            string dateString = websitePart[i].SplitVF("<abbr class=\"published\" title=\"", 2).Last().SplitVF("\">").First();
            //            DateTime postDate = DateTime.Parse(dateString);
            //            if (_EarliestPostDate > postDate)
            //            {
            //                if(i == (websitePart.Length - 1))
            //                {
            //                    //We have found all the posts
            //                    return;
            //                }
            //                else
            //                {
            //                    //There may be more new posts in other threads
            //                    break;
            //                }
            //            }
            //            string postContent = websitePart[i].SplitVF("<div class='post entry-content '>").Last().SplitVF("</div>").First().SplitVF("-->", 2).Last();

            //            string[] postContentParts = postContent.Split('<');

            //            string realContent = "";
            //            try
            //            {
            //                foreach (var postContentPart in postContentParts)
            //                {
            //                    if (postContentPart.Contains('>'))
            //                    {
            //                        if(postContentPart.StartsWith("li>") == true)
            //                            realContent += "\n*";
            //                        realContent += postContentPart.Substring(postContentPart.IndexOf('>') + 1);
            //                    }
            //                    else
            //                        realContent += postContentPart;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                realContent += "\n!!!COULD NOT READ REST OF THE POST!!!";
            //            }
            //            realContent = System.Net.WebUtility.HtmlDecode(realContent);
            //            realContent = realContent.Replace("\t", "");

            //            string[] cn = realContent.SplitVF("\n", StringSplitOptions.RemoveEmptyEntries);

            //            realContent = "";
            //            foreach(var c in cn)
            //            {
            //                realContent += c + "\n";
            //            }
            //            var newForumPost = new ForumPost { m_ThreadName = topic.Item1, m_ThreadURL = topic.Item2, m_PostContent = realContent, m_PostDate = postDate };
            //            m_ForumPosts.AddToList(_ForumAddress, newForumPost);
            //            _RetNewPost(newForumPost);
            //        }
            //        catch (Exception ex)
            //        {}
            //    }
            //}
        }
    }
}
