using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WowRealm = VF_RealmPlayersDatabase.WowRealm;

namespace VF.RaidDamageWebsite
{
    public partial class RealmControl : System.Web.UI.UserControl
    {
        private WowRealm m_Realm = WowRealm.Unknown;
        public WowRealm Realm
        {
            get
            {
                return m_Realm;
            }
            set
            {
                m_Realm = value;
                rblRealm.Items.FindByValue(GetRealmParam()).Selected = true;
            }
        }
        public string GetRealmParam()
        {
            return RealmPlayersServer.StaticValues.ConvertRealmParam(m_Realm);
        }
        public string GetRealmViewing()
        {
            return RealmPlayersServer.StaticValues.ConvertRealmViewing(m_Realm);
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                string realmStr = Request.QueryString.Get("realm");
                if (realmStr == null)
                {
                    realmStr = "All";
                    HttpCookie realmControlCookie = Request.Cookies["RSRealmControl"];
                    if (realmControlCookie != null)
                        realmStr = realmControlCookie["Realm"];
                    if(Request.Url.Query == "")
                        Response.Redirect(Request.Url + "?realm=" + realmStr);
                    else
                        Response.Redirect(Request.Url + "&realm=" + realmStr);
                }
                m_Realm = VF_RealmPlayersDatabase.StaticValues.ConvertRealm(realmStr);
                if (m_Realm == WowRealm.Unknown)
                    m_Realm = WowRealm.All;
                rblRealm.Items.FindByValue(GetRealmParam()).Selected = true;
            }
            Page.PreLoad += Page_PreLoad;
        }
        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (IsPostBack == true)
            {
                string realmChoice = rblRealm.SelectedItem.Value;
                m_Realm = VF_RealmPlayersDatabase.StaticValues.ConvertRealm(realmChoice);

                HttpCookie cookie = new HttpCookie("RSRealmControl");
                cookie.Expires = DateTime.Now.AddDays(30);
                cookie["Realm"] = GetRealmParam();
                Response.Cookies.Add(cookie);
                if (Request.QueryString.Get("realm") != GetRealmParam())
                {
                    var newQuery = Request.Url.Query.Replace("realm=" + Request.QueryString.Get("realm"), "realm=" + GetRealmParam());
                    Response.Redirect(Request.Url.AbsolutePath + newQuery);
                }
            }
        }
        public static WowRealm GetRealmFromCookie(HttpRequest _Request)
        {
            string realmStr = null;
            HttpCookie realmControlCookie = _Request.Cookies["RSRealmControl"];
            if (realmControlCookie != null)
                realmStr = realmControlCookie["Realm"];
            if (realmStr == null)
                return WowRealm.Unknown;
            return VF_RealmPlayersDatabase.StaticValues.ConvertRealm(realmStr);
        }
        protected void Page_InitComplete(object sender, EventArgs e)
        {
            //if (IsPostBack == true)
            //{
            //    string realmChoice = RealmControl_RadioList.SelectedItem.Value;
            //    Realm = VF_RealmPlayersDatabase.StaticValues.ConvertRealm(realmChoice);

            //    HttpCookie cookie = new HttpCookie("RealmControl");
            //    cookie.Expires = DateTime.Now.AddDays(30);
            //    cookie["Realm"] = GetRealmParam();
            //    Response.Cookies.Add(cookie);
            //}
        }
    }
}