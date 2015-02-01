using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using User = RealmPlayersServer.User;

namespace VF_RaidDamageWebsite
{
    public partial class Login : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Authentication.Initialize();
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome() + PageUtility.BreadCrumb_AddFinish("UserPage"));
            if (IsPostBack == false)
            {
                var user = Authentication.GetSessionUser(Page);//Detta ser till att om vi redan är inloggad skickas vi till rätt sektion direkt istället
                HandleUser(user);
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Authentication.LoginUser(Page, txtUserID.Text) == null)
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "UserID \"" + txtUserID.Text + "\" is not valid!";
                return;
            }
            var user = Authentication.GetSessionUser(Page);
            HandleUser(user);
        }

        public void HandleUser(User _User)
        {
            if(_User == null)
            {
                pnlLogin.Visible = true;
                pnlLogout.Visible = false;
                if (Page.Request.QueryString["LoginReturnUrl"] != null)
                {
                    lblStatus.Text = "The page you tried to visit requires a UserID. <br/>Please login using your UserID below.";
                }
                else
                {
                    lblStatus.Text = "Welcome. Please login using your UserID below.";
                }
                return;
            }

            pnlLogin.Visible = false;
            pnlLogout.Visible = true;
            if(_User.IsPremium())
            {
                lblStatus.Text = "Welcome " + _User.Name + ", you are a premium user.";
            }
            else
            {
                lblStatus.Text = "Welcome " + _User.Name + ", you are a normal user.";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Authentication.LogoutUser(Page);
            HandleUser(null);
        }
    }
}