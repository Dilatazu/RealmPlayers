﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace VF.RaidDamageWebsite
{
    public partial class RaidDamageMasterFrame : System.Web.UI.MasterPage
    {
        public static class PageUtilityExtension
        {
            public static string SiteVersion = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
            public static System.Web.Mvc.MvcHtmlString HTMLGetSiteVersion(string _Filename)
            {
                if (_Filename.EndsWith("js"))
                    return new System.Web.Mvc.MvcHtmlString("<script src='" + _Filename + "?version=" + SiteVersion + "' type='text/javascript'></script>");
                else if (_Filename.EndsWith("css"))
                    return new System.Web.Mvc.MvcHtmlString("<link href='" + _Filename + "?version=" + SiteVersion + "' rel='stylesheet'/>");
                else
                    return new System.Web.Mvc.MvcHtmlString(_Filename);
            }
            public static System.Web.Mvc.MvcHtmlString InitializeItemTooltip()
            {
                return new System.Web.Mvc.MvcHtmlString("<script>"
                    + "g_WowIconImageHoster = '" + RealmPlayersServer.Hidden.ApplicationInstance.Instance.GetCurrentItemDatabaseAddress() + "';"
                    + "</script>");
            }
        }

        public MvcHtmlString m_UserInfoHTML;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
#if false
            var user = Authentication.GetSessionUser(Page);
            if (user != null)
            {
                m_UserInfoHTML = new MvcHtmlString("<li><a href='UserPage.aspx'>Logged in as " + user.Name + "</a></li>");
            }
            else
            {
                m_UserInfoHTML = new MvcHtmlString("<li><a href='UserPage.aspx'>Login</a></li>");
            }
#endif
        }
    }
}