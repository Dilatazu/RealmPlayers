using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RealmPlayersServer
{
    public partial class MasterFrame : System.Web.UI.MasterPage
    {
        //public static class PageUtilityExtension
        //{
        //    public static string SiteVersion = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
        //    public static System.Web.Mvc.MvcHtmlString HTMLGetSiteVersion(string _Filename)
        //    {
        //        if (_Filename.EndsWith("js"))
        //            return new System.Web.Mvc.MvcHtmlString("<script src='" + _Filename + "?version=" + SiteVersion + "' type='text/javascript'></script>");
        //        else if (_Filename.EndsWith("css"))
        //            return new System.Web.Mvc.MvcHtmlString("<link href='" + _Filename + "?version=" + SiteVersion + "' rel='stylesheet'/>");
        //        else
        //            return new System.Web.Mvc.MvcHtmlString(_Filename);
        //    }
        //    public static System.Web.Mvc.MvcHtmlString InitializeItemTooltip()
        //    {
        //        return new System.Web.Mvc.MvcHtmlString("<script>"
        //            + "g_WowIconImageHoster = '" + Hidden.ApplicationInstance.Instance.GetCurrentItemDatabaseAddress() + "';"
        //            + "</script>");
        //    }
        //}
        protected void Page_Init(object sender, EventArgs e)
        {
            Constants.AssertInitialize();
            var dataBase = Hidden.ApplicationInstance.Instance._GetRPPDatabase(false);
            var itemInfoCache = Hidden.ApplicationInstance.Instance.GetItemInfoCache(VF_RealmPlayersDatabase.WowVersionEnum.Vanilla, false);
            var itemInfoCacheTBC = Hidden.ApplicationInstance.Instance.GetItemInfoCache(VF_RealmPlayersDatabase.WowVersionEnum.TBC, false);
            if ((dataBase == null || itemInfoCache == null || itemInfoCacheTBC == null) && Request.Url.AbsolutePath.ToLower().StartsWith("/error.aspx") == false)
                Response.Redirect("Error.aspx?reason=loading&return=" + System.Web.HttpUtility.UrlEncode(Request.RawUrl));
            else
                Hidden.ApplicationInstance.Instance.AddUserActivity(Request.UserHostAddress, Request.RawUrl, (Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : ""));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //var dataBase = ApplicationInstance.Instance.GetRPPDatabase(false);
            //var itemInfoCache = ApplicationInstance.Instance.GetItemInfoCache(false);
            //if ((dataBase == null || itemInfoCache == null) && Request.Url.AbsolutePath.ToLower().StartsWith("/error.aspx") == false)
            //    Response.Redirect("Error.aspx?reason=loading&return=" + System.Web.HttpUtility.UrlEncode(Request.RawUrl));
            //else
            //    ApplicationInstance.Instance.AddUserToSite(Request.UserHostAddress);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            DateTime startLoadTime = Context.Timestamp.ToUniversalTime();
            DateTime startRenderTime = DateTime.UtcNow;
            Context.Items["PreRenderTime"] = (int)(startRenderTime - startLoadTime).TotalMilliseconds;
            System.IO.StringWriter customWriter = new System.IO.StringWriter();
            HtmlTextWriter localWriter = new HtmlTextWriter(customWriter);
            base.Render(localWriter);

            //m_PageSize = customWriter.GetStringBuilder().Length;
            Context.Items["PageSize"] = customWriter.ToString().Length;
            writer.Write(customWriter.ToString());
            Context.Items["RenderTime"] = (int)(DateTime.UtcNow - startRenderTime).TotalMilliseconds;
        }
        //protected void Page_PreRender(object sender, EventArgs e)
        //{
        //    //Logger.ConsoleWriteLine(Request.Url.AbsolutePath + " loaded in " + (DateTime.UtcNow - m_StartTime).TotalSeconds.ToStringDot("0.000") + " seconds", ConsoleColor.White);
        //    //Logger.ConsoleWriteLine(Request.Url.AbsolutePath + " loaded in " + (DateTime.UtcNow - m_StartTime).TotalSeconds.ToStringDot("0.000") + " seconds, fileSize=" + m_PageSize + " bytes", ConsoleColor.White);
        //}
        //protected void Page_Unload(object sender, EventArgs e)
        //{

        //    //Logger.ConsoleWriteLine(Request.Url.AbsolutePath + " loaded in " + (DateTime.UtcNow - m_StartTime).TotalSeconds.ToStringDot("0.000") + " seconds", ConsoleColor.White);
        //}
        protected void Top_SearchBox_Submit_Click(object sender, EventArgs e)
        {
            if (Top_SearchBox.Text != "")
            {
                Response.Redirect("CharacterList.aspx?search=" + System.Web.HttpUtility.UrlEncode(Top_SearchBox.Text));
            }
        }
    }
}