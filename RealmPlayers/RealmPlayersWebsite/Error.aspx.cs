using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace RealmPlayersServer
{
    public partial class Error : System.Web.UI.Page
    {
        public MvcHtmlString m_ErrorTextData = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Error | RealmPlayers";
            string reasonStr = Request.QueryString.Get("reason");
            if (reasonStr == "loading")
            {
                Response.AppendHeader("Refresh", "10");

                string returnStr = Request.QueryString.Get("return");
                if (returnStr != null)
                {
                    returnStr = System.Web.HttpUtility.UrlDecode(returnStr);
                }
                else
                    returnStr = "";

                var rppDatabase = Hidden.ApplicationInstance.Instance.GetRPPDatabase(false);
                var itemInfoCache = Hidden.ApplicationInstance.Instance.GetItemInfoCache(VF_RealmPlayersDatabase.WowVersionEnum.Vanilla, false);
                var itemInfoCacheTBC = Hidden.ApplicationInstance.Instance.GetItemInfoCache(VF_RealmPlayersDatabase.WowVersionEnum.TBC, false);

                m_ErrorTextData = new MvcHtmlString("<h1>Website Database Loading</h1>" +
                    "<p>The website is currently loading the database which means you will unfortunately have to wait a bit before it is done."
                    + "<br />Depending on what section you are trying to visit this may take anywhere between 10 seconds and 5 minutes."
                    + "<br />You can have a look around other parts of the website while the database is loading" + (rppDatabase != null ? ", may i suggest <a class='nav' href='PVPList.aspx?section=lifetime_kills'>Lifetime Kills</a>?" : "")
                    + "<br /><br />The website auto refreshes every 10 seconds and when loading is done will redirect you automatically to"
                    + "<br /><a class='nav' href='" + returnStr + "'>" + returnStr + "</a></p>");

                if (rppDatabase != null && itemInfoCache != null && itemInfoCacheTBC != null)
                {
                    string waitStr = Request.QueryString.Get("wait");
                    if (waitStr != null)
                    {
                        string[] waitParts = waitStr.Split('-');
                        var realm = StaticValues.ConvertRealm(waitParts[0]);
                        if (realm != VF_RealmPlayersDatabase.WowRealm.Unknown)
                        {
                            var realmDatabase = rppDatabase.GetRealm(realm);
                            if (realmDatabase.IsPlayersLoadComplete() == false)
                                return;
                            if (waitParts.Length > 1)
                            {
                                if (waitParts[1] == "history")
                                {
                                    if (realmDatabase.IsPlayersHistoryLoadComplete() == false)
                                        return;
                                }
                                //else if (waitParts[1] == "itemsused")
                                //{
                                //    if (realmDatabase.IsLoadComplete() == false)
                                //        return;
                                //    var cacheDB = realmDatabase.GetCacheDatabase(false, true);
                                //    if (cacheDB == null || cacheDB.IsItemsUsedLoaded() == false)
                                //        return;
                                //}
                                else if (waitParts[1] == "guilds")
                                {
                                    if (realmDatabase.IsLoadComplete() == false)
                                        return;
                                }
                            }
                        }
                        else
                        {
                            if (waitParts[0] == "itemdropdatabase")
                            {
                                if (DatabaseAccess.GetItemDropDatabase(this, VF_RealmPlayersDatabase.WowVersionEnum.Vanilla, NotLoadedDecision.ReturnNull) == null)
                                    return;
                            }
                        }
                    }
                    Response.Redirect(returnStr);
                }
            }
            else if (reasonStr == "website_error")
            {
                string returnStr = Request.QueryString.Get("return");
                if (returnStr != null)
                {
                    returnStr = System.Web.HttpUtility.UrlDecode(returnStr);
                }
                else
                    returnStr = "";

                Response.AppendHeader("Refresh", "20;URL=" + returnStr);

                m_ErrorTextData = new MvcHtmlString("<h1>Website Error</h1>" +
                    "<p>There was some error that caused you to be redirect to this site."
                    + "<br />The error you encountered has been logged. Please report this error at the forum <a href='http://forum.realmplayers.com'>http://forum.realmplayers.com</a> so i can investigate it and solve it as soon as possible."
                    + "<br />You will get redirected to another section of the website to avoid further errors."
                    + "<br /><br />The website will refresh after 20 seconds and will redirect you automatically to"
                    + "<br /><a class='nav' href='" + returnStr + "'>" + returnStr + "</a></p>");
            }
        }
    }
}