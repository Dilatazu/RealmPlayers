using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Configuration;

namespace VF.RaidDamageWebsite
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            PageUtility.HOSTURL_Armory = "http://realmplayers.com/";
            PageUtility.HOSTURL_RaidStats = "";
            Logger.Initialize();
            Logger.ConsoleWriteLine("Application_Start()", ConsoleColor.Magenta);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            System.Web.HttpContext context = HttpContext.Current;
            System.Exception ex = Context.Server.GetLastError();

            Logger.ConsoleWriteLine(ex.ToString() + " from URL: \"" + Request.RawUrl + "\"", ConsoleColor.Red);
            context.Server.ClearError();
            Response.Redirect("RaidList.aspx");
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            Logger.ConsoleWriteLine("Application_End()", ConsoleColor.Magenta);
            Logger.SaveToDisk();
        }

        private static SessionStateSection SessionStateSection = (System.Web.Configuration.SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (arg == "UserID")
            {
                string customValue = "UserID=Unknown";
                //Logger.ConsoleWriteLine("GetVaryByCustomString called???");
                var isUserCookie = context.Request.Cookies.Get("IsUser");
                bool IsUser = isUserCookie != null && isUserCookie.Value == "true";
                //Logger.ConsoleWriteLine("GetVaryByCustomString called2???");
                if(IsUser == true)
                {
                    var sessionCookie = context.Request.Cookies.Get(SessionStateSection.CookieName);
                    if (sessionCookie != null && sessionCookie.Value != null)
                    {
                        customValue = "UserID=" + sessionCookie.Value;
                    }
                }
                //Logger.ConsoleWriteLine("GetVaryByCustomString called!!! CustomValue:" + customValue);
                return customValue;
            }
            return base.GetVaryByCustomString(context, arg);
        }
    }
}