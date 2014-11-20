using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace VF_RaidDamageWebsite
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
    }
}