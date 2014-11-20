using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace RealmPlayersServer
{
    public partial class Changelog : System.Web.UI.Page
    {
        public MvcHtmlString m_ChangelogTextHTML = null;
        public MvcHtmlString m_ChangelogListHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("http://forum.realmplayers.com");
            TimeSpan outDateTimeSpan = new TimeSpan(1, 0, 0);
            if (PageUtility.GetQueryString(Request, "ForceReload", "false") != "false")
                outDateTimeSpan = new TimeSpan(0, 0, 0);
            string[] changelogFiles = System.IO.Directory.GetFiles(Constants.RPPDbDir + "RPPWebsitePages\\Changelogs\\");
            List<Tuple<string, DateTime>> changelogs = new List<Tuple<string, DateTime>>();
            foreach (var changelogFile in changelogFiles)
            {
                string dateTimeStr = changelogFile.Split(new char[] { '/', '\\' }).Last().Split('.').First();
                string[] dateTimeComponents = dateTimeStr.Split('_');
                if (dateTimeComponents.Count() == 5)
                {
                    DateTime dateTime = new DateTime(int.Parse(dateTimeComponents[0])
                        , int.Parse(dateTimeComponents[1])
                        , int.Parse(dateTimeComponents[2])
                        , int.Parse(dateTimeComponents[3])
                        , int.Parse(dateTimeComponents[4])
                        , 0, DateTimeKind.Local);

                    string changelogEntry = DynamicFileData.GetTextFile(changelogFile, outDateTimeSpan);
                    if (changelogEntry != "")
                    {//<div class='row'><div class='span12'></div></div>
                        changelogEntry = "<div class='changelogentry'><h3>" + dateTime.ToString("yyyy-MM-dd") + "</h3>" + changelogEntry + "</div>";
                        changelogs.Add(new Tuple<string, DateTime>(changelogEntry, dateTime));
                    }
                }
            }
            var orderedChangelogs = changelogs.OrderByDescending((_Value) => { return _Value.Item2; });

            string changelogText = "";
            foreach (var changelog in orderedChangelogs)
            {
                changelogText += changelog.Item1;
            }
            m_ChangelogTextHTML = new MvcHtmlString(changelogText);
        }
    }
}