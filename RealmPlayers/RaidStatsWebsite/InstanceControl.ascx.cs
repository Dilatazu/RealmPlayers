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

            {"Karazhan", "KZ"},
            {"Zul'Aman", "ZA"},
            {"Magtheridon's Lair", "ML"},
            {"Gruul's Lair", "GL"},
            {"Serpentshrine Cavern", "SSC"},
            {"Tempest Keep", "TK"},
            {"Black Temple", "BT"},
            {"Hyjal Summit", "MH"},
            {"Sunwell Plateau", "SWP"},
        };
        public void SetAllMode()
        {
            foreach(ListItem item in cblInstance.Items)
            {
                item.Attributes.CssStyle.Remove("visibility");
                item.Attributes.CssStyle.Remove("height");
                item.Attributes.CssStyle.Remove("line-height");
                item.Attributes.CssStyle.Remove("overflow");
                item.Attributes.CssStyle.Remove("display");
            }
        }
        public void SetVanillaMode()
        {
            foreach (ListItem item in cblInstance.Items)
            {
                if (item.Value == "All")
                    continue;

                if (item.Value == "Ony" || item.Value == "MC" || item.Value == "BWL" || item.Value == "ZG" || item.Value == "RAQ" || item.Value == "TAQ" || item.Value == "Naxx")
                {
                    item.Attributes.CssStyle.Remove("visibility");
                    item.Attributes.CssStyle.Remove("height");
                    item.Attributes.CssStyle.Remove("line-height");
                    item.Attributes.CssStyle.Remove("overflow");
                    item.Attributes.CssStyle.Remove("display");
                }
                else
                {
                    item.Attributes.CssStyle.Add("visibility", "hidden");
                    item.Attributes.CssStyle.Add("height", "0px");
                    item.Attributes.CssStyle.Add("line-height", "0px");
                    item.Attributes.CssStyle.Add("overflow", "hidden");
                    item.Attributes.CssStyle.Add("display", "block");
                }
            }
        }
        public void SetTBCMode()
        {
            foreach (ListItem item in cblInstance.Items)
            {
                if (item.Value == "All")
                    continue;

                if (item.Value == "Ony" || item.Value == "MC" || item.Value == "BWL" || item.Value == "ZG" || item.Value == "RAQ" || item.Value == "TAQ" || item.Value == "Naxx")
                {
                    item.Attributes.CssStyle.Add("visibility", "hidden");
                    item.Attributes.CssStyle.Add("height", "0px");
                    item.Attributes.CssStyle.Add("line-height", "0px");
                    item.Attributes.CssStyle.Add("overflow", "hidden");
                    item.Attributes.CssStyle.Add("display", "block");
                }
                else
                {
                    item.Attributes.CssStyle.Remove("visibility");
                    item.Attributes.CssStyle.Remove("height");
                    item.Attributes.CssStyle.Remove("line-height");
                    item.Attributes.CssStyle.Remove("overflow");
                    item.Attributes.CssStyle.Remove("display");
                }
            }
        }
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