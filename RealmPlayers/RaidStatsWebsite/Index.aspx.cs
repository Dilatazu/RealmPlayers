using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

using DamageDataSession = VF_RaidDamageDatabase.DamageDataSession;

namespace VF.RaidDamageWebsite
{
    public partial class Default : System.Web.UI.Page
    {
        public MvcHtmlString m_FightLinks = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("RaidList.aspx");
            var fightCollections = ApplicationInstance.Instance.GetFightsFileList();

            string fightLinks = "";

            foreach(var fightCollection in fightCollections)
            {
                fightLinks += "<a href='ViewFights.aspx?FightCollection=" + fightCollection + "'>" + fightCollection + "</a><br />";
            }

            m_FightLinks = new MvcHtmlString(fightLinks);

            //MongoDB branch(?)
        }
    }
}