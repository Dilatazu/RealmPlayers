using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace RealmPlayersServer
{
    public partial class ItemTooltip : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var queryString = PageUtility.GetQueryString(Request, "item", "null");
            if (queryString == "null")
                return;

            Response.ClearContent();
            Response.ContentType = "";
            
            Response.Write(RealmPlayersServer.ItemInfo.GetAjaxTooltip(queryString, 
            (int _ItemID, WowVersionEnum _WowVersion) =>
            {
                if (Hidden.ApplicationInstance.Instance.IsItemInfoCacheLoaded(_WowVersion) == false)
                    return null;
                return Hidden.ApplicationInstance.Instance.GetItemInfo(_ItemID, _WowVersion);
            }));
            Response.End();
        }
    }
}