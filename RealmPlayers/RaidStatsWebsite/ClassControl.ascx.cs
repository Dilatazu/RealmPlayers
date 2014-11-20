using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using PlayerClass = VF_RealmPlayersDatabase.PlayerClass;
using PlayerFaction = VF_RealmPlayersDatabase.PlayerFaction;

namespace VF_RaidDamageWebsite
{
    public partial class ClassControl : System.Web.UI.UserControl
    {
        public static Dictionary<string, PlayerClass> ClassLimitConverter = new Dictionary<string, PlayerClass>{
            {"Wr", PlayerClass.Warrior},
            {"Wl", PlayerClass.Warlock},
            {"Ma", PlayerClass.Mage},
            {"Pr", PlayerClass.Priest},
            {"Sh", PlayerClass.Shaman},
            {"Ro", PlayerClass.Rogue},
            {"Pa", PlayerClass.Paladin},
            {"Dr", PlayerClass.Druid},
            {"Hu", PlayerClass.Hunter},
        };
        public static Dictionary<PlayerClass, string> ClassLimitConverterReverse = new Dictionary<PlayerClass, string>{
            {PlayerClass.Warrior, "Wr"},
            {PlayerClass.Warlock, "Wl"},
            {PlayerClass.Mage, "Ma"},
            {PlayerClass.Priest, "Pr"},
            {PlayerClass.Shaman, "Sh"},
            {PlayerClass.Rogue, "Ro"},
            {PlayerClass.Paladin, "Pa"},
            {PlayerClass.Druid, "Dr"},
            {PlayerClass.Hunter, "Hu"},
        };


        public static string GetClassLimitStr(List<PlayerClass> _Classes)
        {
            string retString = "";
            foreach (var _Class in _Classes)
            {
                retString = retString + GetClassLimitStr(_Class) + "I";
            }
            return retString;
        }
        public static string GetClassLimitStr(PlayerClass _Class)
        {
            var firstOrDefault = ClassLimitConverter.FirstOrDefault((_Value) => _Value.Value == _Class);
            if (firstOrDefault.Equals(default(KeyValuePair<string, PlayerClass>)) == false)
                return firstOrDefault.Key;
            return "";
        }
        public static List<PlayerClass> GetClassLimits(string _ClassLimitStr)
        {
            if (_ClassLimitStr == "" || _ClassLimitStr == "All")
                return null;
            List<PlayerClass> retList = new List<PlayerClass>();
            string[] classLimits = _ClassLimitStr.Split('I');
            foreach (var classLimit in classLimits)
            {
                PlayerClass playerClass;
                if (ClassLimitConverter.TryGetValue(classLimit, out playerClass) == true)
                    retList.Add(playerClass);
            }
            if (retList.Count == 0)
                return null;
            else if (retList.Count >= ClassLimitConverter.Count)
            {
                foreach (var classLimit in ClassLimitConverter)
                {
                    if (retList.Contains(classLimit.Value) == false)
                        return retList;
                }
                return null;
            }
            return retList;
        }
        public static List<PlayerFaction> GetFactionLimits(string _FactionLimitStr)
        {
            if (_FactionLimitStr == "" || _FactionLimitStr == null || _FactionLimitStr == "All")
                return null;
            List<PlayerFaction> retList = new List<PlayerFaction>();
            if (_FactionLimitStr.Contains("Ho"))
                retList.Add(PlayerFaction.Horde);
            if (_FactionLimitStr.Contains("Al"))
                retList.Add(PlayerFaction.Alliance);

            if (retList.Count == 1) 
                return retList;
            else //If 0 return All(null), if both return All(null)
                return null;
        }
        public static string GetNoFactionLimitStr()
        {
            return "All";
        }
        public static string GetFactionLimitStr(PlayerFaction _Faction)
        {
            if (_Faction == PlayerFaction.Horde)
                return "Ho";
            else if (_Faction == PlayerFaction.Alliance)
                return "Al";
            else
                return "All";
        }

        List<PlayerClass> m_ClassLimits = null;
        List<PlayerFaction> m_FactionLimits = null;
        public List<PlayerClass> GetClassLimits()
        {
            return m_ClassLimits;
        }
        public List<PlayerFaction> GetFactionLimits()
        {
            return m_FactionLimits;
        }
        public bool HasClassLimits()
        {
            return m_ClassLimits != null;
        }
        public bool HasFactionLimits()
        {
            return m_FactionLimits != null;
        }
        public string GetColorClassesStr()
        {
            if (m_ClassLimits == null)
                return "";

            string colorClasses = "";
            foreach (var classLimit in m_ClassLimits)
            {
                colorClasses = colorClasses + PageUtility.CreateColorCodedName(classLimit.ToString(), classLimit) + ", ";
            }
            return colorClasses.Substring(0, colorClasses.Length - 2);
        }
        public string GetColorFactionStr()
        {
            if (m_FactionLimits == null)
                return "";

            if (m_FactionLimits.Contains(PlayerFaction.Horde))
                return PageUtility.CreateColorCodedName("Horde", PlayerFaction.Horde);
            else if (m_FactionLimits.Contains(PlayerFaction.Alliance))
                return PageUtility.CreateColorCodedName("Alliance", PlayerFaction.Alliance);
            else
                return "";
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                string classStr = PageUtility.GetQueryString(Request, "ClassLimit", "All");

                var classLimits = GetClassLimits(classStr);
                if (classLimits == null)
                {
                    cblClass.Items.FindByValue("All").Selected = true;
                }
                else
                {
                    foreach(var c in classLimits)
                    {
                        var cParam = ClassLimitConverterReverse[c];
                        cblClass.Items.FindByValue(cParam).Selected = true;
                    }
                }

                string factionStr = PageUtility.GetQueryString(Request, "FactionLimit", null);
                var factionLimits = GetFactionLimits(factionStr);
                if (factionLimits == null)
                {
                    cblFaction.Items.FindByValue("Ho").Selected = true;
                    cblFaction.Items.FindByValue("Al").Selected = true;
                }
                else
                {
                    foreach (var faction in factionLimits)
                    {
                        if (faction == PlayerFaction.Horde)
                            cblFaction.Items.FindByValue("Ho").Selected = true;
                        else if (faction == PlayerFaction.Alliance)
                            cblFaction.Items.FindByValue("Al").Selected = true;
                    }
                }
            }
            Page.PreLoad += Page_PreLoad;
        }
        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (IsPostBack == true)
            {
                string classStr = PageUtility.GetQueryString(Request, "ClassLimit", "All");

                var classLimitsArray = GetClassLimits(classStr);

                string classLimits = "";
                foreach (ListItem item in cblClass.Items)
                {
                    if (item.Selected == true)
                    {
                        if (item.Value == "All")
                        {
                            if (classStr != "All" && classLimitsArray != null)
                            {
                                classLimits = "AllI";
                                break;
                            }
                        }
                        else
                        {
                            classLimits += item.Value + "I";
                        }
                    }
                }
                if (classLimits == "")
                {
                    cblClass.Items.FindByValue("All").Selected = true;
                }
                if (classLimits != "")
                    classLimits = classLimits.Substring(0, classLimits.Length - 1);
                else
                    classLimits = "All";

                if (classLimits.Contains("AllI"))
                    classLimits = classLimits.Replace("AllI", "");

                if (classLimits != "All" && GetClassLimits(classLimits) == null)
                    classLimits = "All";

                string factionStr = PageUtility.GetQueryString(Request, "FactionLimit", null);

                string factionLimits = "";
                foreach (ListItem item in cblFaction.Items)
                {
                    if (item.Selected == false)//Ful lösning för just Faction fallet där det bara finns 2 val! lös lite snyggare i framtiden
                    {
                        factionLimits += item.Value + "I";
                    }
                }
                if (factionLimits == "")
                {
                    cblFaction.Items.FindByValue("Ho").Selected = true;
                    cblFaction.Items.FindByValue("Al").Selected = true;
                }
                if (factionLimits != "")
                    factionLimits = factionLimits.Substring(0, factionLimits.Length - 1);
                else
                    factionLimits = null;

                if (factionLimits != null && GetFactionLimits(factionLimits) == null)
                    factionLimits = null;

                if (classStr != classLimits || factionStr != factionLimits)
                {
                    Response.Redirect(PageUtility.CreateUrlWithNewQueryValues(Request
                        , new KeyValuePair<string, string>[2] { 
                            new KeyValuePair<string, string>("ClassLimit", classLimits), 
                            new KeyValuePair<string, string>("FactionLimit", factionLimits) 
                        }));
                }
            }
            m_ClassLimits = GetClassLimits(PageUtility.GetQueryString(Request, "ClassLimit", "WrIWaIWlIMaIPrIShIRoIPaIDrIHu"));
            m_FactionLimits = GetFactionLimits(PageUtility.GetQueryString(Request, "FactionLimit", "All"));
        }
        protected void Page_InitComplete(object sender, EventArgs e)
        {
        }
    }
}