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
        public enum ForumType
        {
            FeenixForum,
            RealmPlayersForum,
            NostalriusForum,
            KronosForum,
            RSS_RealmPlayersForum,
            RSS_NostalriusForum,
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
        private static List<ForumPost> GetThreadPosts(string _ThreadURL, string _LastPostURL, DateTime _EarliestPostDate, ForumType _ForumType)
        {
            if(_ForumType == ForumType.FeenixForum)
            {
                List<ForumPost> threadPosts = new List<ForumPost>();

                string website = _GetHTMLFile(_LastPostURL);
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
                        catch (Exception) { }

                        string postBody = websitePart[i].SplitVF("<div class='post_body'>", 2).Last();
                        string dateString = postBody.SplitVF("<abbr class=\"published\" title=\"", 2).Last().SplitVF("\">").First();
                        DateTime postDate = DateTime.Parse(dateString);
                        if (_EarliestPostDate > postDate)
                        {
                            break;
                        }
                        string postContent = websitePart[i].SplitVF("<div class='post entry-content '>").Last().SplitVF("</div>").First().SplitVF("-->", 2).Last();

                        string[] postContentParts = postContent.Replace("<br />", "\n").Split('<');

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
            else
            {
                string forumBaseURL = "http://forum.realmplayers.com/";
                if(_ForumType == ForumType.NostalriusForum)
                {
                    forumBaseURL = "http://forum.nostalrius.org/";
                }
                List<ForumPost> threadPosts = new List<ForumPost>();

                string website = _GetHTMLFile(_LastPostURL);
                string threadContent = website.SplitVF("<div id=\"page-body\">", 2).Last();
                string threadName = System.Net.WebUtility.HtmlDecode(threadContent.SplitVF("viewtopic.php", 2).Last().SplitVF(">", 2).Last().SplitVF("</a></h2>", 2).First());


                string[] websitePart;
                if(_ForumType == ForumType.NostalriusForum)
                {
                    websitePart = website.SplitVF("<div class=\"postbody\">");//"<p class=\"author\">by <strong><a href=\"");
                }
                else
                {
                    websitePart = website.SplitVF("<p class=\"author\"><a href=\"");
                }

                for (int i = websitePart.Length - 1; i >= 1; --i)
                {
                    try
                    {
                        string postURL;
                        
                        if(_ForumType == ForumType.NostalriusForum)
                        {
                            string postNumber = websitePart[i].SplitVF("><a href=\"#p", 2).Last();
                            postNumber = postNumber.SplitVF("\">", 2).First();
                            //postURL = _ThreadURL + "#" + System.Net.WebUtility.HtmlDecode(postURL);
                            if (_LastPostURL.Contains("&sid") == true)
                            {
                                //postURL = _LastPostURL.SplitVF("&sid=", 2).First() + "#" + _LastPostURL.SplitVF("#", 2).Last();
                                postURL = _LastPostURL.SplitVF("&sid=", 2).First() + "#p" + postNumber;
                            }
                            else
                            {
                                postURL = _LastPostURL + "&p=" + postNumber + "#p" + postNumber;
                            }
                        }
                        else
                        {
                            postURL = websitePart[i].SplitVF("\">", 2).First();
                            postURL = postURL.Replace("./", forumBaseURL);
                            postURL = System.Net.WebUtility.HtmlDecode(postURL);
                        }
                        string posterInfo;
                        if(_ForumType == ForumType.NostalriusForum)
                        {
                            posterInfo = websitePart[i].SplitVF("<p class=\"author\">by <strong><a href=\"").Last().SplitVF("<dl class=\"postprofile\"", 2).Last().SplitVF("</dt>").First();
                        }
                        else
                        {
                            posterInfo = websitePart[i].SplitVF("<dl class=\"postprofile\"", 2).Last().SplitVF("</dt>").First();
                        }
                        string posterName;
                        if (_ForumType == ForumType.NostalriusForum)
                        {
                            posterName = posterInfo.SplitVF("</a>\n ").First().SplitVF(">").Last();
                        }
                        else
                        {
                            posterName = posterInfo.SplitVF("</a>\r\n").First().SplitVF(">").Last();
                        }
                        string posterImageURL = posterInfo.SplitVF("User avatar\" /></a><br />").First().SplitVF("<img src=\"", 2).Last().SplitVF("\"").First();
                        posterImageURL = posterImageURL.Replace("./", forumBaseURL);

                        string dateString = websitePart[i].SplitVF("</strong> &raquo; ", 2).Last().SplitVF(" </p>", 2).First();
                        DateTime postDate = ParseDateString(dateString, DateTime.MinValue);
                        if (_EarliestPostDate > postDate)
                        {
                            break;
                        }
                        string postContent = websitePart[i].SplitVF("<div class=\"content\">", 2).Last().SplitVF("<dl class", 2).First();

                        string realContent = ParsePostHTMLContent(postContent);
                        var newForumPost = new ForumPost { m_ThreadName = threadName, m_ThreadURL = _ThreadURL, m_PostURL = postURL, m_PosterName = posterName, m_PosterImageURL = posterImageURL, m_PostContent = realContent, m_PostDate = postDate };
                        threadPosts.Add(newForumPost);
                    }
                    catch (Exception)
                    { }
                }
                return threadPosts;
            }
        }

        private static string ParsePostHTMLContent(string postContent)
        {
            string[] postContentParts = postContent.Replace("<br />", "\r\n").Split('<');

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

            string[] cn = realContent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            realContent = "";
            foreach (var c in cn)
            {
                realContent += c + "\n";
            }
            return realContent;
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

            public void UpdateThread(string _ThreadURL, string _LastPostURL, DateTime _LatestPost, Action<ForumPost> _RetPosts, ForumType _ForumType)
            {
                if (m_LastUpdatedThreads.ContainsKey(_ThreadURL) == false)
                    m_LastUpdatedThreads.Add(_ThreadURL, DateTime.MinValue.AddDays(10));

                if (m_LastUpdatedThreads[_ThreadURL] < _LatestPost)
                {
                    try
                    {
                        var newThreadPosts = GetThreadPosts(_ThreadURL, _LastPostURL, m_LastUpdatedThreads[_ThreadURL].AddDays(-1), _ForumType);

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
            public void UpdateThread(ForumPost _Post, Action<ForumPost> _RetPosts, ForumType _ForumType)
            {
                if (m_LastUpdatedThreads.ContainsKey(_Post.m_ThreadURL) == false)
                    m_LastUpdatedThreads.Add(_Post.m_ThreadURL, DateTime.MinValue.AddDays(10));

                if (m_LastUpdatedThreads[_Post.m_ThreadURL] < _Post.m_PostDate)
                {
                    try
                    {
                        if (m_ForumPosts.FirstOrDefault((_Value) => _Value.m_PostDate == _Post.m_PostDate && _Value.m_PostContent == _Post.m_PostContent) == default(ForumPost))
                        {
                            //Post doesnt allready exist, add it!
                            m_ForumPosts.Add(_Post);
                            try
                            {
                                _RetPosts(_Post);
                            }
                            catch (Exception)
                            { }
                        }

                        m_LastUpdatedThreads[_Post.m_ThreadURL] = _Post.m_PostDate;
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
        public static void GetLatestPosts(string[] _ForumAddresses, Action<ForumPost> _RetPosts, ForumType _ForumType, bool _OnlyNewest = false) //"http://www.wow-one.com/forum/117-server-updates/"
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
                    _GetLatestPosts(forumSection, _RetPosts, _ForumType);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            SaveForumSections();
        }
        public static void GetLatestPosts(string _ForumAddress, Action<ForumPost> _RetPosts, ForumType _ForumType, bool _OnlyNewest = false) //"http://www.wow-one.com/forum/117-server-updates/"
        {
            ForumSection forumSection = GetForumSection(_ForumAddress);

            if (_OnlyNewest == false)
            {
                foreach (var threadPost in forumSection.m_ForumPosts)
                {
                    _RetPosts(threadPost);
                }
            }
            _GetLatestPosts(forumSection, _RetPosts, _ForumType);
            SaveForumSections();
        }
        private static void _GetLatestPosts(ForumSection _ForumSection, Action<ForumPost> _RetPosts, ForumType _ForumType)
        {
            if ((DateTime.Now - _ForumSection.m_LastPollDatTime).TotalMinutes < 5)
                return;

            string website = _GetHTMLFile(_ForumSection.m_ForumSectionURL);

            if(_ForumType == ForumType.FeenixForum)
            {
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
                            latestPostDate = DateTime.Parse(dateStr.Replace("Today,", "" + refDate.Day + " " + refDate.ToString("MMM") + " " + refDate.Year));
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

                    if ((DateTime.Now - latestPostDate).TotalDays < 14)
                        _ForumSection.UpdateThread(threadLink, threadLink + "page__view__getlastpost", latestPostDate, _RetPosts, _ForumType);

                    //topics.Add(Tuple.Create(System.Net.WebUtility.HtmlDecode(topicName), topicLink));
                }
            }
            else if(_ForumType == ForumType.KronosForum)
            {
                //Kronos forum is through RSS feed example: http://forum.twinstar.cz/external.php?type=RSS2&forumids=969
                try
                {
                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    xmlDocument.LoadXml(website);
                    var rssNode = xmlDocument.DocumentElement;
                    if (rssNode.Name == "rss")
                    {
                        var channelNode = rssNode.FirstChild;
                        if(channelNode.Name == "channel")
                        {
                            for (int i = 0; i < channelNode.ChildNodes.Count; ++i)
                            {
                                if(channelNode.ChildNodes[i].Name == "item")
                                {
                                    ForumPost fPost = new ForumPost();
                                    fPost.m_PosterImageURL = "";
                                    var postNode = channelNode.ChildNodes[i];
                                    foreach(System.Xml.XmlElement node in postNode.ChildNodes)
                                    {
                                        if(node.Name == "title")
                                        {
                                            fPost.m_ThreadName = node.InnerText;
                                        }
                                        else if (node.Name == "content:encoded")
                                        {
                                            fPost.m_PostContent = ParsePostHTMLContent(node.InnerText.Replace("<![CDATA[", "").Replace("]]>", ""));
                                        }
                                        else if (node.Name == "link")
                                        {
                                            fPost.m_PostURL = node.InnerText.Replace("?goto=newpost", "");
                                            fPost.m_ThreadURL = fPost.m_PostURL;
                                        }
                                        else if (node.Name == "pubDate")
                                        {
                                            fPost.m_PostDate = ParseDateString(node.InnerText, DateTime.MinValue);
                                        }
                                        else if (node.Name == "dc:creator")
                                        {
                                            fPost.m_PosterName = node.InnerText;
                                        }
                                    }

                                    if (fPost.m_ThreadName != "" && fPost.m_PostContent != "" 
                                        && fPost.m_ThreadURL != "" && fPost.m_PosterName != "" && fPost.m_PostDate != DateTime.MinValue)
                                    {
                                        _ForumSection.UpdateThread(fPost, _RetPosts, _ForumType);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {}
            }
            else if (_ForumType == ForumType.RSS_RealmPlayersForum || _ForumType == ForumType.RSS_NostalriusForum)
            {
                //RSS example: http://realmplayers.com:5555/feed.php?f=14
                //https://www.phpbb.com/support/docs/en/3.1/kb/article/faq-phpbb-atom-feeds/
                try
                {
                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    xmlDocument.LoadXml(website);
                    var rssNode = xmlDocument.DocumentElement;
                    if (rssNode.Name == "feed")
                    {
                        for (int i = 0; i < rssNode.ChildNodes.Count; ++i)
                        {
                            if(rssNode.ChildNodes[i].Name == "entry")
                            {
                                var entryNode = rssNode.ChildNodes[i];

                                ForumPost fPost = new ForumPost();
                                foreach (System.Xml.XmlElement node in entryNode.ChildNodes)
                                {
                                    if(node.Name == "author")
                                    {
                                        if (node.FirstChild.Name == "name")
                                        {
                                            fPost.m_PosterName = node.FirstChild.InnerText.Replace("<![CDATA[", "").Replace("]]>", "");
                                        }
                                    }
                                    else if(node.Name == "published")
                                    {
                                        fPost.m_PostDate = ParseDateString(node.InnerText, DateTime.MinValue);
                                    }
                                    else if(node.Name == "link")
                                    {
                                        fPost.m_PostURL = node.Attributes["href"].Value;
                                        fPost.m_ThreadURL = fPost.m_PostURL;
                                    }
                                    else if (node.Name == "title")
                                    {
                                        fPost.m_ThreadName = node.InnerText.Replace("<![CDATA[", "").Replace("]]>", "");
                                        fPost.m_ThreadName = fPost.m_ThreadName.SplitVF(" • ").Last();
                                    }
                                    else if (node.Name == "content")
                                    {
                                        fPost.m_PostContent = ParsePostHTMLContent(node.InnerText.Replace("<![CDATA[", "").Replace("]]>", ""));
                                    }
                                }

                                if (fPost.m_ThreadName != "" && fPost.m_PostContent != ""
                                    && fPost.m_ThreadURL != "" && fPost.m_PosterName != "" && fPost.m_PostDate != DateTime.MinValue)
                                {
                                    _ForumSection.UpdateThread(fPost, _RetPosts, _ForumType);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                { }
            }
            else
            {
                string forumBaseURL = "http://forum.realmplayers.com/";
                if (_ForumType == ForumType.NostalriusForum)
                {
                    forumBaseURL = "http://forum.nostalrius.org/";
                }

                string[] websitePart = website.SplitVF("<dt title=\"");

                for (int i = 1; i < websitePart.Length; ++i)
                {
                    //string topicName = websitePart[i].SplitVF("class=\"topictitle\">", 2).Last().SplitVF("</a>").First();
                    string dateStr = websitePart[i].SplitVF("\"View the latest post\" /></a> <br />", 2).Last().SplitVF("</span>").First();
                    DateTime latestPostDate = ParseDateString(dateStr, DateTime.MinValue);

                    string threadLink = websitePart[i].SplitVF("<a href=\"", 2).Last().SplitVF("\" class", 2).First().SplitVF("&amp;sid=").First();

                    if (threadLink.StartsWith("./viewtopic.php?") == false)
                        continue;

                    DateTime createdThreadDate = ParseDateString(websitePart[i].SplitVF("&raquo; ", 2).Last().SplitVF("\n", 2).First(), DateTime.MinValue);

                    string lastPostLink = threadLink + websitePart[i].SplitVF("<dd class=\"lastpost\"", 2).Last().SplitVF(threadLink).Last().SplitVF("\"><img src=", 2).First();
                    if ((DateTime.Now - createdThreadDate).TotalDays < 14)
                    {
                        _ForumSection.UpdateThread(System.Net.WebUtility.HtmlDecode(threadLink.Replace("./viewtopic.php", forumBaseURL + "viewtopic.php")), System.Net.WebUtility.HtmlDecode(threadLink.Replace("./viewtopic.php", forumBaseURL + "viewtopic.php")), createdThreadDate, _RetPosts, _ForumType);
                    }
                    if ((DateTime.Now - latestPostDate).TotalDays < 14)
                    {
                        _ForumSection.UpdateThread(System.Net.WebUtility.HtmlDecode(threadLink.Replace("./viewtopic.php", forumBaseURL + "viewtopic.php")), System.Net.WebUtility.HtmlDecode(lastPostLink.Replace("./viewtopic.php", forumBaseURL + "viewtopic.php")), latestPostDate, _RetPosts, _ForumType);
                    }
                }

            }
            _ForumSection.m_LastPollDatTime = DateTime.Now;
        }

        private static DateTime ParseDateString(string dateStr, DateTime _ErrorDate)
        {
            DateTime date = _ErrorDate;
            try
            {
                if (dateStr.StartsWith("Today") == true)
                {
                    DateTime refDate = DateTime.UtcNow;
                    date = DateTime.Parse(dateStr.Replace("Today,", "" + refDate.Day + " " + refDate.ToString("MMM") + " " + refDate.Year));
                }
                else if (dateStr.StartsWith("Yesterday") == true)
                {
                    DateTime refDate = DateTime.UtcNow.AddDays(-1);
                    date = DateTime.Parse(dateStr.Replace("Yesterday,", "" + refDate.Day + " " + refDate.ToString("MMM") + " " + refDate.Year));
                }
                else
                {
                    date = DateTime.Parse(dateStr);
                }
            }
            catch (Exception)
            {
                date = _ErrorDate;
            }
            return date;
        }
    }
}
