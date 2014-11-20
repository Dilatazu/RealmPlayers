using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VF_RaidDamageWebsite
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
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}