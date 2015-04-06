using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VF.RaidDamageWebsite
{
    public partial class InstanceControl : System.Web.UI.UserControl
    {
        public static Dictionary<string, string> InstanceFilterConverter = new Dictionary<string, string>
        {
            {"Onyxia's Lair", "Ony"},
            {"Molten Core", "MC"},
            {"Blackwing Lair", "BWL"},
            {"Zul'Gurub", "ZG"},
            {"Ruins of Ahn'Qiraj", "RAQ"},
            {"Ahn'Qiraj Temple", "TAQ"},
            {"Temple of Ahn'Qiraj", "TAQ"},
            {"Naxxramas", "Naxx"},
        };
        public bool IsFiltered(string _Instance)
        {
            if (m_InstanceFilter == null)
                return true;
            if (m_InstanceFilter.Contains(_Instance))
                return true;
            string instanceAcronym = "";
            if (InstanceFilterConverter.TryGetValue(_Instance, out instanceAcronym) && m_InstanceFilter.Contains(instanceAcronym))
                return true;
            if (m_InstanceFilter.Contains("All"))
                return true;
            return false;
        }
        List<string> m_InstanceFilter = null;
        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                string instanceStr = PageUtility.GetQueryString(Request, "InstanceFilter", "All");

                string[] instancesFilterArray = instanceStr.Split('I');
                foreach (var instance in instancesFilterArray)
                {
                    cblInstance.Items.FindByValue(instance).Selected = true;
                }
            }
            Page.PreLoad += Page_PreLoad;
        }
        protected void Page_PreLoad(object sender, EventArgs e)
        {
            string instanceStr = PageUtility.GetQueryString(Request, "InstanceFilter", "All");

            if (IsPostBack == true)
            {
                string[] instancesFilterArray = instanceStr.Split('I');
                string instanceFilter = "";
                foreach (ListItem item in cblInstance.Items)
                {
                    if (item.Selected == true)
                    {
                        if (item.Value == "All")
                        {
                            if (instanceStr != "All" && (instancesFilterArray.Contains("All") == false && instancesFilterArray.Length != 7))
                            {
                                instanceFilter = "AllI";
                                break;
                            }
                        }
                        else
                        {
                            instanceFilter += item.Value + "I";
                        }
                    }
                }
                if(instanceFilter == "")
                {
                    cblInstance.Items.FindByValue("All").Selected = true;
                    instanceFilter = "All";
                }
                else
                {
                    instanceFilter = instanceFilter.Substring(0, instanceFilter.Length - 1);
                }
                if (instanceFilter.Contains("AllI"))
                    instanceFilter = instanceFilter.Replace("AllI", "");

                if (instanceFilter != "All" && instanceFilter.Split('I').Length > 6)
                    instanceFilter = "All";

                if(instanceStr != instanceFilter)
                {
                    Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "InstanceFilter", instanceFilter));
                }
            }
            m_InstanceFilter = instanceStr.Split('I').ToList();

        }
    }
}