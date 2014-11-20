using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VF_RaidDamageWebsite
{
    public partial class BossesControl : System.Web.UI.UserControl
    {
        const string MC_ALL = "00010203040506070809";
        const string ONY_ALL = "0A";
        const string BWL_ALL = "0B0C0D0E0F0G0H0I";
        const string ZG_ALL = "0J0K0L0M0N0O0P0Q";
        const string AQ20_ALL = "0W0X0Y0Z1A1B";
        const string AQ40_ALL = "1C1D1E1F1G1H1I1J1K";
        const string AQ40_NOOPTIONAL = "1C1D1E1F1G1H";
        const string NAXX_ALL = "1L1M1N1O1P1Q1R1S1T1U1V1W1X1Y1Z";
        const string NAXX_ALL_QUARTERS = "1L1M1N1O1P1Q1R1S1T1U1V1W1X";
        const string NAXX_ARACHNID_QUARTERS = "1L1M1N";
        const string NAXX_CONSTRUCT_QUARTERS = "1O1P1Q1R";
        const string NAXX_PLAGUE_QUARTERS = "1S1T1U";
        const string NAXX_MILITARY_QUARTERS = "1V1W1X";

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
        public static Dictionary<string, string> BossFilterConverter = new Dictionary<string, string>
        {
            {"Lucifron", "00"},
            {"Magmadar", "01"},
	        {"Gehennas", "02"},
	        {"Garr", "03"},
	        {"Baron Geddon", "04"},
	        {"Shazzrah", "05"},
	        {"Sulfuron Harbinger", "06"},
	        {"Golemagg the Incinerator", "07"},
	        {"Majordomo Executus", "08"},
	        {"Ragnaros","09"},
	
	        //Onyxia
	        {"Onyxia", "0A"},
	
	        //BWL
	        {"Razorgore the Untamed", "0B"},
	        {"Vaelastrasz the Corrupt", "0C"},
	        {"Broodlord Lashlayer", "0D"},
	        {"Firemaw", "0E"},
	        {"Ebonroc", "0F"},
	        {"Flamegor", "0G"},
	        {"Chromaggus", "0H"},
	        {"Nefarian", "0I"},
	
	        //ZG
	        {"High Priestess Jeklik", "0J"},
	        {"High Priest Venoxis", "0K"},
	        {"High Priestess Mar'li", "0L"},
	        {"High Priest Thekal", "0M"},
	        {"High Priestess Arlokk", "0N"},
	        //{"Hakkar the Soulflayer", "0O"},//inte rätt
	        {"Hakkar", "0O"},
	        {"Bloodlord Mandokir", "0P"},
	        {"Jin'do the Hexxer", "0Q"},
	        /*{"Gahz'ranka", "0R"},
	        {"Gri'lek", "0S"},
	        {"Renataki", "0T"},
	        {"Hazza'rah", "0U"},
	        {"Wushoolay", "0V"},*/
	
	        //AQ20
	        {"Kurinnaxx", "0W"},
	        {"General Rajaxx", "0X"},
	        {"Moam", "0Y"},
	        {"Buru the Gorger", "0Z"},
	        {"Ayamiss the Hunter", "1A"},
	        {"Ossirian the Unscarred", "1B"},
	
	        //AQ40
	        {"The Prophet Skeram", "1C"},
	        {"Battleguard Sartura", "1D"},
	        {"Fankriss the Unyielding", "1E"},
	        {"Princess Huhuran", "1F"},
	        {"Twin Emperors", "1G"},
	        {"C'Thun", "1H"},
	        {"Three Bugs", "1I"},
	        {"Ouro", "1J"},
	        {"Viscidus", "1K"},
	        //{"Anubisath Defenders", "1H"}, //whaaat? varför har jag denna här?
            //{"Vem", "Ahn'Qiraj Temple"},
            //{"Lord Kri", "Ahn'Qiraj Temple"},
            //{"Princess Yauj", "Ahn'Qiraj Temple"},
	
            //Naxx
            {"Anub'Rekhan", "1L"},
            {"Grand Widow Faerlina", "1M"},
            {"Maexxna", "1N"},
            {"Patchwerk", "1O"},
            {"Grobbulus", "1P"},
            {"Gluth", "1Q"},
            {"Thaddius", "1R"},
            {"Noth the Plaguebringer", "1S"},
            {"Heigan the Unclean", "1T"},
            {"Loatheb", "1U"},
            {"Instructor Razuvious", "1V"},
            {"Gothik the Harvester", "1W"},
            {"The Four Horsemen", "1X"},
            //{"Highlord Mograine", "Naxxramas"},
            //{"Thane Korth'azz", "Naxxramas"},
            //{"Lady Blaumeux", "Naxxramas"},
            //{"Sir Zeliek", "Naxxramas"},
            {"Sapphiron", "1Y"},
            {"Kel'Thuzad", "1Z"},

	        //UBRS
            //{"Pyroguard Emberseer", "2A"},
            //{"Warchief Rend Blackhand", "2B"},
            //{"The Beast", "2C"},
            //{"General Drakkisath", "2D"},
        };
        public static void InitializeBossFilters()
        {
            
        }
        public bool IsFiltered(string _Boss)
        {
            if (m_BossFilter == null)
                return true;
            if (m_BossFilter.Contains(_Boss))
                return true;
            string bossAcronym = "";
            if (BossFilterConverter.TryGetValue(_Boss, out bossAcronym) && m_BossFilter.Contains(bossAcronym))
                return true;
            if (m_BossFilter.Contains("All"))
                return true;
            return false;
        }
        public List<string> GetBossFilter() 
        {
            return m_BossFilter;
        }
        public MvcHtmlString m_MostViewedBossesLists = null;

        private string m_BossFilterType;
        public string GetBossFilterType()
        {
            return m_BossFilterType;
        }

        List<string> m_BossFilter = new List<string>();
        protected void Page_Init(object sender, EventArgs e)
        {
            foreach(var bossAcronym in BossFilterConverter)
            {
                var instance = VF_RaidDamageDatabase.BossInformation.BossFights[bossAcronym.Key];
                if (instance == "Zul'Gurub")
                    cblBoss1.Items.Add(new ListItem(bossAcronym.Key, bossAcronym.Value));
                else if (instance == "Ruins of Ahn'Qiraj")
                    cblBoss2.Items.Add(new ListItem(bossAcronym.Key, bossAcronym.Value));
                else if (instance == "Molten Core")
                    cblBoss3.Items.Add(new ListItem(bossAcronym.Key, bossAcronym.Value));
                else if (instance == "Blackwing Lair")
                    cblBoss4.Items.Add(new ListItem(bossAcronym.Key, bossAcronym.Value));
                else if (instance == "Ahn'Qiraj Temple")
                    cblBoss5.Items.Add(new ListItem(bossAcronym.Key, bossAcronym.Value));
                else if (instance == "Naxxramas")
                    cblBoss6.Items.Add(new ListItem(bossAcronym.Key, bossAcronym.Value));
            }
            //if (IsPostBack == false) //Disabled for now, caused a weird bug sometimes and things did not get selected correctly.
            {
                string bossesStr = PageUtility.GetQueryString(Request, "Bosses", BWL_ALL);

                var bosses = bossesStr.SplitListVF(2);
                CheckBoxList[] cblLists = new CheckBoxList[] { cblBoss1, cblBoss2, cblBoss3, cblBoss4, cblBoss5, cblBoss6 };
               
                foreach (var boss in bosses)
                {
                    foreach (var cblList in cblLists)
                    {
                        var item = cblList.Items.FindByValue(boss);
                        if (item != null) item.Selected = true;
                    }
                }
            }
            Page.PreLoad += Page_PreLoad;
        }
        protected void Page_PreLoad(object sender, EventArgs e)
        {
            string bossesStr = PageUtility.GetQueryString(Request, "Bosses", BWL_ALL);
            //PageUtility.GetQueryString(Request, "Mode", "mostviewed");
            var bosses = bossesStr.SplitListVF(2);
            if (IsPostBack == true)
            {
                bool listChanged = false;
                CheckBoxList[] cblLists = new CheckBoxList[] { cblBoss1, cblBoss2, cblBoss3, cblBoss4, cblBoss5, cblBoss6 };
                foreach (var cblList in cblLists)
                {
                    foreach (ListItem item in cblList.Items)
                    {
                        if (item.Selected == true)
                        {
                            if (bosses.AddUnique(item.Value) == true)
                                listChanged = true;
                        }
                        else
                        {
                            if (bosses.Remove(item.Value) == true)
                                listChanged = true;
                        }
                    }
                }
                if (listChanged == true)
                {
                    var orderedBosses = bosses.OrderBy((_Value) => _Value);
                    string newBossesStr = orderedBosses.MergeToStringVF();

                    Response.Redirect(PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", newBossesStr));
                }
            }
            foreach (var boss in bosses)
            {
                m_BossFilter.Add(BossFilterConverter.First((_Value) => _Value.Value == boss).Key);
            }
            {
                if (bossesStr == MC_ALL + ZG_ALL + BWL_ALL + AQ20_ALL + AQ40_ALL + NAXX_ALL)
                    m_BossFilterType = "All Instances";
                else if (bossesStr == ZG_ALL + AQ20_ALL)
                    m_BossFilterType = "ZG+AQ20";
                else if (bossesStr == ZG_ALL+MC_ALL)
                    m_BossFilterType = "ZG+MC";
                else if (bossesStr == MC_ALL+BWL_ALL)
                    m_BossFilterType = "MC+BWL";
                else if (bossesStr == BWL_ALL + AQ40_ALL)
                    m_BossFilterType = "BWL+AQ40";
                else if (bossesStr == AQ40_ALL + NAXX_ALL)
                    m_BossFilterType = "AQ40+Naxx";
                else if (bossesStr == BWL_ALL + AQ40_ALL + NAXX_ALL)
                    m_BossFilterType = "BWL+AQ40+Naxx";
                else if (bossesStr == MC_ALL + BWL_ALL + AQ40_ALL + NAXX_ALL)
                    m_BossFilterType = "MC+BWL+AQ40+Naxx";
                else if (bossesStr == ZG_ALL)
                    m_BossFilterType = "Zul'Gurub";
                else if (bossesStr == AQ20_ALL)
                    m_BossFilterType = "Ruins of Ahn'Qiraj";
                else if (bossesStr == MC_ALL)
                    m_BossFilterType = "Molten Core";
                else if (bossesStr == BWL_ALL)
                    m_BossFilterType = "Blackwing Lair";
                else if (bossesStr == AQ40_ALL)
                    m_BossFilterType = "Temple of Ahn'Qiraj";
                else if (bossesStr == AQ40_NOOPTIONAL)
                    m_BossFilterType = "AQ40 No Optional";
                else if (bossesStr == NAXX_ALL)
                    m_BossFilterType = "Full Naxxramas";
                else if (bossesStr == NAXX_ALL_QUARTERS)
                    m_BossFilterType = "All Quarters, Naxxramas";
                else if (bossesStr == NAXX_ARACHNID_QUARTERS)
                    m_BossFilterType = "Arachnid Quarter, Naxxramas";
                else if (bossesStr == NAXX_CONSTRUCT_QUARTERS)
                    m_BossFilterType = "Construct Quarter, Naxxramas";
                else if (bossesStr == NAXX_PLAGUE_QUARTERS)
                    m_BossFilterType = "Plague Quarter, Naxxramas";
                else if (bossesStr == NAXX_MILITARY_QUARTERS)
                    m_BossFilterType = "Military Quarter, Naxxramas";
                else
                    m_BossFilterType = "Specific Bosses";
            }
            System.Text.StringBuilder mostViewedBossesLists = new System.Text.StringBuilder();
            mostViewedBossesLists.Append("<div class='span3' style='min-width: 180px; max-width: 180px;'>");
            mostViewedBossesLists.Append("<h5>Multi Instance</h5>");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", MC_ALL + ZG_ALL + BWL_ALL + AQ20_ALL + AQ40_ALL + NAXX_ALL) + "'>All</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", ZG_ALL+AQ20_ALL) + "'>ZG+AQ20</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", ZG_ALL+MC_ALL) + "'>ZG+MC</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", MC_ALL+BWL_ALL) + "'>MC+BWL</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", BWL_ALL+AQ40_ALL) + "'>BWL+AQ40</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", AQ40_ALL+NAXX_ALL) + "'>AQ40+Naxx</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", BWL_ALL+AQ40_ALL+NAXX_ALL) + "'>BWL+AQ40+Naxx</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", MC_ALL+BWL_ALL+AQ40_ALL+NAXX_ALL) + "'>MC+BWL+AQ40+Naxx</a>");
            mostViewedBossesLists.Append("</div>");
            mostViewedBossesLists.Append("<div class='span3' style='min-width: 180px; max-width: 180px;'>");
            mostViewedBossesLists.Append("<h5>Zul'Gurub</h5>");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", ZG_ALL) + "'>All</a>");
            mostViewedBossesLists.Append("<h5>Ruins of Ahn'Qiraj</h5>");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", AQ20_ALL) + "'>All</a>");
            mostViewedBossesLists.Append("<h5>Molten Core</h5>");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", MC_ALL) + "'>All</a>");
            mostViewedBossesLists.Append("<h5>Blackwing Lair</h5>");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", BWL_ALL) + "'>All</a>");
            mostViewedBossesLists.Append("</div>");
            mostViewedBossesLists.Append("<div class='span3' style='min-width: 180px; max-width: 180px;'>");
            mostViewedBossesLists.Append("<h5>Temple of Ahn'Qiraj</h5>");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", AQ40_ALL) + "'>All</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", AQ40_NOOPTIONAL) + "'>No Optional</a>");
            mostViewedBossesLists.Append("<h5>Naxxramas</h5>");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", NAXX_ALL) + "'>All</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", NAXX_ALL_QUARTERS) + "'>All Quarters</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", NAXX_ARACHNID_QUARTERS) + "'>Arachnid Quarter</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", NAXX_CONSTRUCT_QUARTERS) + "'>Construct Quarter</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", NAXX_PLAGUE_QUARTERS) + "'>Plague Quarter</a><br />");
            mostViewedBossesLists.Append("<a href='" + PageUtility.CreateUrlWithNewQueryValue(Request, "Bosses", NAXX_MILITARY_QUARTERS) + "'>Military Quarter</a>");
            mostViewedBossesLists.Append("</div>");

            m_MostViewedBossesLists = new MvcHtmlString(mostViewedBossesLists.ToString());
        }
    }
}