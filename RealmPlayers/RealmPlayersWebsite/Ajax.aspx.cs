using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace RealmPlayersServer
{
    public partial class Ajax : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var queryString = PageUtility.GetQueryString(Request, "item", "null");
            if (queryString != "null")
            {
                Response.ClearContent();
                Response.ContentType = "";

                Response.Write(RealmPlayersServer.ItemInfo.GetAjaxTooltip(Request.RawUrl,
                (int _ItemID, WowVersionEnum _WowVersion) =>
                {
                    if (Hidden.ApplicationInstance.Instance.IsItemInfoCacheLoaded(_WowVersion) == false)
                        return null;
                    return Hidden.ApplicationInstance.Instance.GetItemInfo(_ItemID, _WowVersion);
                }));
                Response.End();
                return;
            }
            queryString = PageUtility.GetQueryString(Request, "data", "null");
            if (queryString == "talents")
            {
                Response.Clear();
                Response.ContentType = "application/x-javascript";
                if (Request.QueryString.Get("class") == "1")
                {
                    Response.Write(TalentCalculator.TalentInfo_Warrior);
                }
                else if (Request.QueryString.Get("class") == "2")
                {
                    Response.Write(TalentCalculator.TalentInfo_Paladin);
                }
                else if (Request.QueryString.Get("class") == "3")
                {
                    Response.Write(TalentCalculator.TalentInfo_Hunter);
                }
                else if (Request.QueryString.Get("class") == "4")
                {
                    Response.Write(TalentCalculator.TalentInfo_Rogue);
                }
                else if (Request.QueryString.Get("class") == "5")
                {
                    Response.Write(TalentCalculator.TalentInfo_Priest);
                }
                else if (Request.QueryString.Get("class") == "6")
                {
                    Response.Write("");
                }
                else if (Request.QueryString.Get("class") == "7")
                {
                    Response.Write(TalentCalculator.TalentInfo_Shaman);
                }
                else if (Request.QueryString.Get("class") == "8")
                {
                    Response.Write(TalentCalculator.TalentInfo_Mage);
                }
                else if (Request.QueryString.Get("class") == "9")
                {
                    Response.Write(TalentCalculator.TalentInfo_Warlock);
                }
                else if (Request.QueryString.Get("class") == "10")
                {
                    Response.Write("");
                }
                else if (Request.QueryString.Get("class") == "11")
                {
                    Response.Write(TalentCalculator.TalentInfo_Druid);
                }
                else if (Request.QueryString.Get("class") == "21")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Warrior);
                }
                else if (Request.QueryString.Get("class") == "22")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Paladin);
                }
                else if (Request.QueryString.Get("class") == "23")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Hunter);
                }
                else if (Request.QueryString.Get("class") == "24")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Rogue);
                }
                else if (Request.QueryString.Get("class") == "25")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Priest);
                }
                else if (Request.QueryString.Get("class") == "26")
                {
                    Response.Write("");
                }
                else if (Request.QueryString.Get("class") == "27")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Shaman);
                }
                else if (Request.QueryString.Get("class") == "28")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Mage);
                }
                else if (Request.QueryString.Get("class") == "29")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Warlock);
                }
                else if (Request.QueryString.Get("class") == "30")
                {
                    Response.Write("");
                }
                else if (Request.QueryString.Get("class") == "31")
                {
                    Response.Write(TalentCalculator.TalentInfoTBC_Druid);
                }
                Response.End();
                return;
            }
            return;
        }
    }
}