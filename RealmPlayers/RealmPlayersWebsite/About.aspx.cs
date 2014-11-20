using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace RealmPlayersServer
{
    public partial class About : System.Web.UI.Page
    {
        //public MvcHtmlString m_AboutTextHTML = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "About | RealmPlayers";
            //m_AboutTextHTML = new MvcHtmlString(DynamicFileData.GetTextFile(Constants.RPPDbDir + "RPPWebsitePages\\About.txt"));
        }
    }
}