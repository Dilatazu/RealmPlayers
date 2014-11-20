using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace RealmPlayersServer
{
    public partial class Index : System.Web.UI.Page
    {
        public MvcHtmlString m_VisitorsHTML = null;
        public MvcHtmlString m_DonationsHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            string visitorExtraText = "";
            
            var userActivityStats = Hidden.ApplicationInstance.Instance.GetUserActivityStats();
            if (userActivityStats != null)
            {
                m_VisitorsHTML = new MvcHtmlString(visitorExtraText + "<h5><span class='text-center'>" + userActivityStats.GetStat(Hidden.UserActivityStats.IntervalStat.Last24Hours) + " unique visitors last 24 hours</span></h5>");
            }
            else
            {
                m_VisitorsHTML = new MvcHtmlString(visitorExtraText);
            }

            var donationsStr = "";
            var donations = Donators.GetDonations();
            for(int i = donations.Count - 1; i >= donations.Count - 5 && i >= 0; --i)
            {
                donationsStr += "<tr><td class='left'>" + donations[i].Item3 + "</td><td>" + donations[i].Item2 + "</td><td class='right'>" + donations[i].Item1.ToString("yyyy-MM-dd") + "</td></tr>";
            }
            m_DonationsHTML = new MvcHtmlString(donationsStr);
        }
        protected void SearchBox_Submit_Click(object sender, EventArgs e)
        {
            //if (ddlSearchType.SelectedValue == "Players")
            {
                string redirectAddress = "CharacterList.aspx?search=" + System.Web.HttpUtility.HtmlEncode(SearchBox.Text);
                if (ddlRealm.SelectedValue != "Any")
                    redirectAddress += "&realm=" + ddlRealm.SelectedValue;
                if (ddlRace.SelectedValue != "All")
                    redirectAddress += "&race=" + ddlRace.SelectedValue;
                if (ddlClass.SelectedValue != "All")
                    redirectAddress += "&class=" + ddlClass.SelectedValue;
                if (ddlLevel.SelectedValue != "All")
                    redirectAddress += "&level=" + ddlLevel.SelectedValue;
                Response.Redirect(redirectAddress);
            }
            //else
            {
                //string redirectAddress = "GuildList.aspx?search=" + System.Web.HttpUtility.HtmlEncode(SearchBox.Text);
                //if (ddlRealm.SelectedValue != "Any")
                //    redirectAddress += "&realm=" + ddlRealm.SelectedValue;
                //if (ddlFaction.SelectedValue != "Any")
                //    redirectAddress += "&faction=" + ddlFaction.SelectedValue;
                //Response.Redirect(redirectAddress);
                Response.Redirect("GuildList.aspx");
            }
        }
    }
}