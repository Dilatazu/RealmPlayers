using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ContributorUtility = VF_RealmPlayersDatabase.ContributorUtility;
using ContributorDB = VF_RealmPlayersDatabase.ContributorDB;

namespace RealmPlayersServer
{
    public class User
    {
        public string Name;
        public string UserID;
        public ContributorDB.ContributorDBElement DBElement;
        public User(ContributorDB.ContributorDBElement _DBElement)
        {
            DBElement = _DBElement;
            Name = _DBElement.Name;
            UserID = _DBElement.UserID;
        }
        public bool IsAdmin()
        {
            return Name == "Dilatazu" || Name == "Sethzer";
        }
        public bool IsPremium()
        {
            return IsAdmin();
        }
    }
    public class Authentication
    {
        public static void Initialize()
        {
            ContributorDB.Initialize();
            try
            {
                if (ContributorDB.GetMongoDB().IsConnected() == false)
                {
                    System.Threading.Thread.Sleep(500);
                    ContributorDB.GetMongoDB().IsConnected();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public static User GetSessionUser(System.Web.SessionState.HttpSessionState _Session)
        {
            return (User)_Session["User"];
        }
        public static void SetIsUserCookie(HttpResponse _Response, bool _IsUser)
        {
            HttpCookie cookie = new HttpCookie("IsUser");
            cookie.Expires = DateTime.Now.AddDays(1);
            cookie.Value = _IsUser == true ? "true" : "false";
            _Response.Cookies.Add(cookie);
        }
        public static User GetSessionUser(System.Web.UI.Page _CurrentPage, bool _RedirectOnLoginFailure = false)
        {
            if(_CurrentPage.Session["UserIP"] == null
            || ((string)_CurrentPage.Session["UserIP"] != _CurrentPage.Request.UserHostAddress))
            {
                //Någon försöker hijacka sessionen! logga ut Usern för säkerhetsskull
                LogoutUser(_CurrentPage.Session);
            }
            User user = (User)_CurrentPage.Session["User"];
            if(user == null)
            {
                SetIsUserCookie(_CurrentPage.Response, false);
                if (_RedirectOnLoginFailure == true && _CurrentPage.AppRelativeVirtualPath != "~/UserPage.aspx")
                    _CurrentPage.Response.Redirect("~/UserPage.aspx?LoginReturnUrl=" + _CurrentPage.Server.HtmlEncode(_CurrentPage.Request.RawUrl));
                return null;
            }
            else if(_CurrentPage.Request.QueryString["LoginReturnUrl"] != null)
            {
                SetIsUserCookie(_CurrentPage.Response, true);
                _CurrentPage.Response.Redirect(_CurrentPage.Server.HtmlDecode(_CurrentPage.Request.QueryString["LoginReturnUrl"]));
            }
            return user;
        }
        public static User LoginUser(System.Web.UI.Page _CurrentPage, string _UserID)
        {
            if (_CurrentPage.Session["User"] != null)
                return null; //Måste logga ut användare innan man kan logga in igen

            if(ContributorDB.CheckContributor(_UserID, _CurrentPage.Request.UserHostAddress) != ContributorDB.CheckContributorResult.UserID_Success_Login)
                return null;
            var contributor = ContributorDB.GetContributor(_UserID);
            if(contributor == null)
                return null;
            
            //Skapa User klassen och returnera eftersom inloggningen lyckades!
            User newUser = new User(contributor);
            _CurrentPage.Session["User"] = newUser;//Spara undan usern i sessionen så att vi kommer ihåg den
            _CurrentPage.Session["UserIP"] = _CurrentPage.Request.UserHostAddress;//Spara undan IP på den som loggade in. endast denna IP är nu valid med denna Sessionen!
            SetIsUserCookie(_CurrentPage.Response, true);
            return newUser;
        }
        public static void LogoutUser(System.Web.SessionState.HttpSessionState _Session)
        {
            _Session["UserIP"] = null;
            _Session["User"] = null;
        }
        public static void LogoutUser(System.Web.UI.Page _CurrentPage)
        {
            LogoutUser(_CurrentPage.Session);
            SetIsUserCookie(_CurrentPage.Response, false);
        }
    }
}