using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace RealmPlayersServer
{
    public partial class Log : System.Web.UI.Page
    {
        public MvcHtmlString m_LogHTML;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Log | RealmPlayers";

            string userStr = Request.QueryString.Get("user");
            string passStr = Request.QueryString.Get("pass");
            if (userStr == "Viktor" && passStr == VF.HiddenStrings.CreateUserID_AdminPassword)
            {
                if (PageUtility.GetQueryString(Request, "save", "null") != "null")
                {
                    Logger.SaveToDisk();
                    Response.Redirect(Request.Url.AbsolutePath + "?user=Viktor&pass=***REMOVED***");
                }

                var log = Logger.GetCopyOfLog(100);
                log.Reverse();
                string logText = "";
                foreach (var line in log)
                {
                    logText += line;
                }

                logText = "<h3>" + PageUtility.CreateLink(PageUtility.CreateUrlWithNewQueryValue(Request, "save", "true"), "Save LogFile") + "</h3>" + logText;
                
                m_LogHTML = new MvcHtmlString(logText);
            }
            else
                Response.Redirect("Index.aspx");
        }
    }
}