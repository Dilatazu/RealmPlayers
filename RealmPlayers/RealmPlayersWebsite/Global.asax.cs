using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace RealmPlayersServer
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            PageUtility.HOSTURL_Armory = "";
            PageUtility.HOSTURL_RaidStats = "RaidStats/";
            Constants.AssertInitialize();
            Logger.Initialize();
            Logger.ConsoleWriteLine("Application_Start()", ConsoleColor.Magenta);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                string fullURL = Request.Url.ToString();
                string lowerCaseURL = fullURL.ToLower();
                var charIndex = lowerCaseURL.IndexOf("vchar/");
                if (charIndex != -1 && lowerCaseURL.EndsWith(".aspx"))
                {
                    var dataStr = fullURL.Substring(charIndex + "vchar/".Length);
                    dataStr = dataStr.Substring(0, dataStr.Length - 5);

                    Response.Redirect("/CharacterDesigner.aspx?data=" + dataStr);
                }
            }
            catch (Exception)
            {}
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var currContext = HttpContext.Current;
            //int totalRequestTime = -1;
            int preRenderTimeMs = -1; 
            int renderTimeMs = -1;
            int pageSize = -1;
            try
            {
                //totalRequestTime = (int)(DateTime.UtcNow - currContext.Timestamp.ToUniversalTime()).TotalMilliseconds;
                preRenderTimeMs = (int)currContext.Items["PreRenderTime"];
                renderTimeMs = (int)currContext.Items["RenderTime"];
                pageSize = (int)currContext.Items["PageSize"];
            }
            catch (Exception)
            {
                //if (currContext.Response.IsRequestBeingRedirected == false)
                //    Logger.ConsoleWriteLine("Application_EndRequest(): catched error when trying to calculate statistics", ConsoleColor.Red);
                //else
                //    Logger.ConsoleWriteLine("Application_EndRequest(): was a redirect", ConsoleColor.Red);
            }
            if(currContext.Response.IsRequestBeingRedirected == true || pageSize != -1)
                PerformanceStatistics.AddStatistics(currContext, preRenderTimeMs, renderTimeMs, pageSize);

            //Logger.ConsoleWriteLine(Request.Url.AbsolutePath + " loaded in " + totalRequestTime + " milliseconds(" + preRenderTime + "," + renderTime + "), fileSize=" + pageSize + " bytes", ConsoleColor.White);
            //currContext.Response.Write("<!-- Render Time: " + renderTime + " -->");
            
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

            if (Request.RawUrl.Contains("Error.aspx"))
                return;
            Response.Redirect("/Error.aspx?reason=website_error&return=Index.aspx");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Logger.ConsoleWriteLine("Application_End()", ConsoleColor.Magenta);
            Logger.SaveToDisk();
            //PerformanceStatistics.SaveStatistics();
            //try
            //{
            //    Npgsql.NpgsqlConnection.ClearAllPools();
            //}
            //catch (Exception)
            //{

            //}
        }
    }
}