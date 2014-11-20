using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace RealmPlayersServer
{
    public partial class AdminInfo : System.Web.UI.Page
    {
        public MvcHtmlString m_InfoHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Admin | RealmPlayers";

            string userStr = Request.QueryString.Get("user");
            string passStr = Request.QueryString.Get("pass");

            if (userStr != "Viktor" || passStr != "***REMOVED***")
                Response.Redirect("Index.aspx");

            int count = PageUtility.GetQueryInt(Request, "count", 500);
            string detailsStr = Request.QueryString.Get("details");
            string commandStr = Request.QueryString.Get("command");
            string queryUrlStr = PageUtility.GetQueryString(Request, "queryurl", "");


            var appInstance = Hidden.ApplicationInstance.Instance;
            if (commandStr == "reload")
            {
                appInstance.m_LastLoadedDateTime = DateTime.MinValue;
                Response.Redirect(Request.Url.AbsolutePath + "?user=Viktor&pass=***REMOVED***");
            }
            string info = "<h2><a href='Admininfo.aspx?user=Viktor&pass=***REMOVED***'>Admin Info</a></h2>";
            info += "<h3><a href='Log.aspx?user=Viktor&pass=***REMOVED***'>Event Log</a></h3><br />";
            info += "<div class='row'><div class='span10'>";
            info += "<h2>User Activity</h2><br />Last complete restart: " + appInstance.m_StartTime.ToString();
            info += "<br />Last database load time: " + appInstance.m_LastLoadedDateTime.ToLocalTime().ToString() + " <a href='" + Request.RawUrl + "&command=reload" + "'>(Reload now?)</a>";
            if (detailsStr == null)
            {
                System.Threading.Monitor.Enter(appInstance.m_RealmPlayersMutex);
                try
                {
                    var activityStats = appInstance.GetUserActivityStats();
                    DateTime nowMinus30s = DateTime.Now.AddSeconds(-30);
                    DateTime nowMinus5m = DateTime.Now.AddMinutes(-5);
                    DateTime nowMinus10m = DateTime.Now.AddMinutes(-10);
                    DateTime nowMinus30m = DateTime.Now.AddMinutes(-30);
                    DateTime nowMinus60m = DateTime.Now.AddMinutes(-60);
                    DateTime nowMinus2h = DateTime.Now.AddHours(-2);
                    DateTime nowMinus3h = DateTime.Now.AddHours(-3);
                    DateTime nowMinus6h = DateTime.Now.AddHours(-6);
                    DateTime nowMinus12h = DateTime.Now.AddHours(-12);
                    DateTime nowMinus24h = DateTime.Now.AddHours(-24);
                    DateTime nowMinus2d = DateTime.Now.AddDays(-2);
                    DateTime nowMinus4d = DateTime.Now.AddDays(-4);
                    DateTime nowMinus7d = DateTime.Now.AddDays(-7);
                    DateTime nowMinus1M = DateTime.Now.AddMonths(-1);
                    info += "<br />Unique Users Last:";
                    info += "<br />30 secs: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last30Sec);
                    info += "<br />5 mins: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last5Min);
                    info += "<br />10 mins: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last10Min);
                    info += "<br />30 mins: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last30Min);
                    info += "<br />hour: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.LastHour);
                    info += "<br />2 hours: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last2Hours);
                    info += "<br />3 hours: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last3Hours);
                    info += "<br />6 hours: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last6Hours);
                    info += "<br />12 hours: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last12Hours);
                    info += "<br />24 hours: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last24Hours);
                    info += "<br />2 days: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last2Days);
                    info += "<br />4 days: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last4Days);
                    info += "<br />7 days: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last7Days);
                    info += "<br />1 month: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.LastMonth);
                    info += "<br />total: " + activityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Total);
                    //if (appInstance.m_UsersOnSite.Count > 0)
                    //    info += "<br />" + appInstance.m_UsersOnSite.First().Key;
                    info += "<br /><br /><h4>IPs: DateTime</h4>";
                    var latestVisits = UserActivityDB.GetLatestVisits(DateTime.UtcNow.AddDays(-0.5));
                    var orderedLatestVisits = latestVisits.OrderByDescending((_Value) => _Value.Value.VisitTime);
                    //string[] urlReferrerSeparator = { " @<@ " };
                    foreach (var latestVisit in orderedLatestVisits)
                    {
                        //if(queryUrlStr == "")
                        //    userNavObj = latestVisit.Value;
                        //else if (queryUrlStr[0] == '!')
                        //{
                        //    try
                        //    {
                        //        string[] negativeQuerys = queryUrlStr.Split(new char[] { '!' }, StringSplitOptions.RemoveEmptyEntries);
                        //        userNavObj = user.Value.Last((_Value) =>
                        //        { 
                        //            foreach(string negQuery in negativeQuerys)
                        //            {
                        //                if (_Value.Item2.Contains(negQuery) == true)
                        //                    return false;
                        //            }
                        //            return true;
                        //        });
                        //    }
                        //    catch (Exception)
                        //    {}
                        //}
                        //else
                        //{
                        //    try
                        //    {
                        //        string[] positiveQuerys = queryUrlStr.Split(new char[]{'!'}, StringSplitOptions.RemoveEmptyEntries);
                        //        userNavObj = user.Value.Last((_Value) =>
                        //        {
                        //            foreach (string posQuery in positiveQuerys)
                        //            {
                        //                if (_Value.Item2.Contains(posQuery) == true)
                        //                    return true;
                        //            }
                        //            return false;
                        //        });
                        //    }
                        //    catch (Exception)
                        //    {}
                        //}

                        //if (userNavObj == null)
                        //    continue;

                        if (--count < 0)
                            break;

                        //string userUrl = userNavObj.Item2.Split(urlReferrerSeparator, StringSplitOptions.None).First();
                        //string userUrlReferrer = userNavObj.Item2.Split(urlReferrerSeparator, StringSplitOptions.None).Last();

                        //int comparisonCount = userUrlReferrer.Length/3;
                        //if (userUrl.Length < userUrlReferrer.Length)
                        //    comparisonCount = userUrl.Length/3;

                        //if (userUrl.Substring(0, comparisonCount) == userUrlReferrer.Substring(0, comparisonCount))
                        //    userUrlReferrer = "";
                        //else
                        //    userUrlReferrer = " from \"" + userUrlReferrer + "\"";

                        if ((DateTime.UtcNow - latestVisit.Value.VisitTime).TotalHours >= 6)
                        {
                            info += "<a class='nav' href='http://www.ip-adress.com/ip_tracer/" + latestVisit.Key + "'>" + latestVisit.Key + "</a>: "
                                + "<a class='nav' href='AdminInfo.aspx?user=Viktor&pass=***REMOVED***&details=" + latestVisit.Key + "'>" + latestVisit.Value.VisitTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "</a>" + " = <a class='nav' href='" + latestVisit.Value.URL + "'>" + latestVisit.Value.URL + "</a>"
                                + (latestVisit.Value.FromURL != "" ? " from \"" + latestVisit.Value.FromURL + "\"" : "") + "<br />";
                        }
                        else
                        {
                            info += "<a class='nav' href='http://www.ip-adress.com/ip_tracer/" + latestVisit.Key + "'>" + latestVisit.Key + "</a>: "
                                + "<a class='nav' href='AdminInfo.aspx?user=Viktor&pass=***REMOVED***&details=" + latestVisit.Key + "'>" + latestVisit.Value.VisitTime.ToLocalTime().ToString("HH:mm:ss") + "</a>" + " = <a class='nav' href='" + latestVisit.Value.URL + "'>" + latestVisit.Value.URL + "</a>"
                                + (latestVisit.Value.FromURL != "" ? " from \"" + latestVisit.Value.FromURL + "\"" : "") + "<br />";
                        }
                    }
                }
                catch (Exception ex)
                {
                    VF_RealmPlayersDatabase.Logger.LogException(ex);
                }
                System.Threading.Monitor.Exit(appInstance.m_RealmPlayersMutex);
            }
            else
            {
                //string[] urlReferrerSeparator = { " @<@ " };
                //List<Tuple<DateTime, string>> detailsData;
                var userActivity = UserActivityDB.GetActivity(detailsStr);
                if (userActivity != null)//appInstance.m_UsersOnSite.TryGetValue(detailsStr, out detailsData) == true)
                {
                    info += "<br /><br /><h4>Details for: <a class='nav' href='http://www.ip-adress.com/ip_tracer/" + detailsStr + "'>" + detailsStr + "</a></h4>";
                    string listData = "";
                    foreach (var urlVisit in userActivity.URLVisits)
                    {
                        //string userUrl = data.Item2.Split(urlReferrerSeparator, StringSplitOptions.None).First();
                        //string userUrlReferrer = data.Item2.Split(urlReferrerSeparator, StringSplitOptions.None).Last();

                        //int comparisonCount = userUrlReferrer.Length / 3;
                        //if (userUrl.Length < userUrlReferrer.Length)
                        //    comparisonCount = userUrl.Length / 3;

                        //if (userUrl.Substring(0, comparisonCount) == userUrlReferrer.Substring(0, comparisonCount))
                        //    userUrlReferrer = "";
                        //else
                        //    userUrlReferrer = " from \"" + userUrlReferrer + "\"";

                        listData = urlVisit.VisitTime.ToString("yyyy-MM-dd HH:mm:ss") + " = " + "<a class='nav' href='" + urlVisit.URL + "'>" + urlVisit.URL + "</a>" 
                            + (urlVisit.FromURL != "" ? " from \"" + urlVisit.FromURL + "\"" : "") + "<br />" + listData;
                    }
                    info += listData;
                }
            }
                
            info += "</div><div class='span2'>";
            info += "<h2>Commands</h2>";
            info += "<a href='" + Request.RawUrl + "&command=savestats" + "'>Save UserActivity</a>";
            info += "</div'></div>";
            m_InfoHTML = new MvcHtmlString(info);
        }
    }
}