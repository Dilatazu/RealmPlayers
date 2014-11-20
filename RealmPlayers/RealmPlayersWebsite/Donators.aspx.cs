using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace RealmPlayersServer
{
    public partial class Donators : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_DonatorsInfoHTML = null;
        public MvcHtmlString m_TableHeadHTML = null;
        public MvcHtmlString m_TableBodyHTML = null;

        public static List<Tuple<DateTime, string, string>> GetDonations()
        {
            string donatorsText = DynamicFileData.GetTextFile(Constants.RPPDbDir + "RPPWebsitePages\\Donators.txt");
            var donatorsArray = donatorsText.SplitVF("\r\n");

            List<Tuple<DateTime, string, string>> donations = new List<Tuple<DateTime, string, string>>();
            foreach (var donator in donatorsArray)
            {
                try
                {
                    var donatorArgs = donator.Split(',');
                    if (donatorArgs.Length == 3)
                    {
                        var player = donatorArgs[0].Replace("\t", "");
                        DateTime date = DateTime.Parse(donatorArgs[2].Replace("\t", ""));
                        string amount = donatorArgs[1].Replace("\t", "");
                        //if(donatorArgs[1].Replace("\t", "").TryParseFloat(out amount) == false)
                        //    throw new Exception("TryParseFloat failed!");

                        donations.Add(Tuple.Create(date, amount, player));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            return donations;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Donators | RealmPlayers";
            
            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome() + PageUtility.BreadCrumb_AddFinish("Donators"));

            m_TableHeadHTML = new MvcHtmlString(PageUtility.CreateTableRow("", PageUtility.CreateTableColumnHead("#Nr") 
                + PageUtility.CreateTableColumnHead("Player") 
                + PageUtility.CreateTableColumnHead("Amount") 
                + PageUtility.CreateTableColumnHead("Date")));

            var donations = GetDonations();

            string tableBody = "";
            for (int i = donations.Count - 1; i >= 0; --i)
            {
                tableBody += PageUtility.CreateTableRow("", PageUtility.CreateTableColumn("#" + (i + 1)) 
                    + PageUtility.CreateTableColumn(donations[i].Item3)
                    + PageUtility.CreateTableColumn(donations[i].Item2)
                    + PageUtility.CreateTableColumn(donations[i].Item1.ToString("yyyy-MM-dd")));
            }
            m_TableBodyHTML = new MvcHtmlString(tableBody);

            m_DonatorsInfoHTML = new MvcHtmlString(
                "<h1>Donators<span class='badge badge-inverse'>" + donations.Count + " Donations</span></h1>"
                + "<p>List displays the donations of money to this project. Sorted by date of the donation.</p>"
                + "<br/><p>Donations help me keep the server up to be able to provide a good and stable service. Donations are also a big motivator for me to continueing improving the service with new features, solving stability issues, adding contributors and other things related to the project."
                + "<br/>You can read more about donations and how to make one at the <a href='Donate.aspx'>Donate</a> page.</p><br/>");

        }
    }
}