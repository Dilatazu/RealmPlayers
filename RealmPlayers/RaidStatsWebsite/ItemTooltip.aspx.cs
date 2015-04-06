using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VF.RaidDamageWebsite
{
    public partial class ItemTooltip : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var queryString = PageUtility.GetQueryString(Request, "item", "null");
            if (queryString == "null")
                return;

            //Response.Clear();
            //Response.ClearHeaders();

            //Response.ContentType = "text/json";//("Content-Type", "text/json");
            //byte[] b = Response.ContentEncoding.GetBytes("alert('test successfull!');");
            //Response.AddHeader("Content-Length", b.Length.ToString());
            //Response.BinaryWrite(b);
            //try
            //{
            //    Response.Flush();
            //    Response.Close();
            //}
            //catch (Exception)
            //{}
            //return;

            Response.ClearContent();
            Response.ContentType = "";
            Response.Write(RealmPlayersServer.ItemInfo.GetAjaxTooltip(queryString, ApplicationInstance.Instance.GetItemInfo));
            Response.End();
        }
    }
}