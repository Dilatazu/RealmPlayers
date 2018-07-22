using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using VF_RealmPlayersDatabase;

namespace RealmPlayersServer
{
    public partial class OnlineStats : System.Web.UI.Page
    {
        public MvcHtmlString m_BreadCrumbHTML = null;
        public MvcHtmlString m_PageInfoHTML = null;
        public MvcHtmlString m_BodyHTML = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            var realm = PageUtility.GetQueryRealm(Request);
            if (realm == WowRealm.Unknown) return;
            var wowVersion = StaticValues.GetWowVersion(realm);

            var onlinePlayersDB = DatabaseAccess.GetRealmOnlinePlayers(this, realm);
            
            this.Title = "OnlineStats @ " + StaticValues.ConvertRealmParam(realm) + " | RealmPlayers";

            m_BreadCrumbHTML = new MvcHtmlString(PageUtility.BreadCrumb_AddHome()
                + PageUtility.BreadCrumb_AddRealm(realm)
                + PageUtility.BreadCrumb_AddFinish("OnlineStats"));

            m_PageInfoHTML = new MvcHtmlString("<h1>OnlineStats"
                + "</h1><p>OnlineStats for realm " + StaticValues.ConvertRealmViewing(realm) + "</p>"
                + "<p>" + onlinePlayersDB.OnlineEntries.Count + " nr of onlinestats entries!</p>");

            string graphSection = "";
            {
                List<int> dataX = new List<int>();
                List<int> dataY1 = new List<int>();
                List<int> dataY2 = new List<int>();
                List<int> dataY3 = new List<int>();
                List<int> dataY4 = new List<int>();
                List<int> dataY5 = new List<int>();
                List<string> labels = new List<string>();

                DateTime currOnlineDateTime = DateTime.UtcNow.AddDays(-14);
                currOnlineDateTime = currOnlineDateTime.AddMinutes(-currOnlineDateTime.Minute).AddSeconds(-currOnlineDateTime.Second).AddHours(1);
                //Remove minute and second significants

                int day = 0;

                HashSet<string> onlinePlayers = new HashSet<string>();
                HashSet<string> playersInRaidAndDungeons = new HashSet<string>();
                HashSet<string> playersInBattlegrounds = new HashSet<string>();
                HashSet<string> playersAtLvl60 = new HashSet<string>();

                Action addGraphEntry = () =>
                {
                    labels.Add(currOnlineDateTime.ToDateStr() + "<br />Players online: " + PageUtility.CreateColorString(onlinePlayers.Count.ToString(), System.Drawing.Color.Green)
                            + "<br />Players in Raids/Dungeons: " + PageUtility.CreateColorString(playersInRaidAndDungeons.Count.ToString() 
                            + (onlinePlayers.Count != 0 ? " ( " + (((double)playersInRaidAndDungeons.Count / (double)onlinePlayers.Count) * 100).ToStringDot("0.0") + "% )" : ""), System.Drawing.Color.Yellow)
                            + "<br />Players in Battlegrounds: " + PageUtility.CreateColorString(playersInBattlegrounds.Count.ToString()
                            + (onlinePlayers.Count != 0 ? " ( " + (((double)playersInBattlegrounds.Count / (double)onlinePlayers.Count) * 100).ToStringDot("0.0") + "% )" : ""), System.Drawing.Color.Red)
                            + "<br />Players below lvl 60: " + PageUtility.CreateColorString((onlinePlayers.Count - playersAtLvl60.Count).ToString()
                            + (onlinePlayers.Count != 0 ? " ( " + (((double)(onlinePlayers.Count - playersAtLvl60.Count) / (double)onlinePlayers.Count) * 100).ToStringDot("0.0") + "% )" : ""), System.Drawing.Color.Cyan)
                            + "<br />Players at lvl 60: " + PageUtility.CreateColorString(playersAtLvl60.Count.ToString()
                            + (onlinePlayers.Count != 0 ? " ( " + (((double)playersAtLvl60.Count / (double)onlinePlayers.Count) * 100).ToStringDot("0.0") + "% )" : ""), System.Drawing.Color.Purple));
                    dataX.Add(++day);
                    dataY1.Add(onlinePlayers.Count);
                    dataY2.Add(playersInRaidAndDungeons.Count);
                    dataY3.Add(playersInBattlegrounds.Count);
                    dataY4.Add(playersAtLvl60.Count);
                    dataY5.Add(onlinePlayers.Count - playersAtLvl60.Count);
                    onlinePlayers.Clear();
                    playersInRaidAndDungeons.Clear();
                    playersInBattlegrounds.Clear();
                    playersAtLvl60.Clear();
                };
                foreach (var onlineEntry in onlinePlayersDB.OnlineEntries)
                {
                    if(onlineEntry.DateTime_EndSpan < currOnlineDateTime)
                        continue;

                    while (currOnlineDateTime <= onlineEntry.DateTime_StartSpan.AddHours(-8))
                    {
                        addGraphEntry();
                        currOnlineDateTime = currOnlineDateTime.AddHours(8);
                    }

                    foreach (var entry in onlineEntry.OnlinePlayers)
                    {
                        onlinePlayers.Add(entry.Key);
                        if (StaticValues.IsZoneRaid(entry.Value.Zone) || StaticValues.IsZoneDungeon(entry.Value.Zone))
                            playersInRaidAndDungeons.Add(entry.Key);
                        if (StaticValues.IsZoneBattleground(entry.Value.Zone))
                            playersInBattlegrounds.Add(entry.Key);
                        if (entry.Value.Level == 60)
                            playersAtLvl60.Add(entry.Key);
                    }
                    foreach (var entry in onlineEntry.OnlinePlayers_Duplicates)
                    {
                        if (StaticValues.IsZoneRaid(entry.Zone) || StaticValues.IsZoneDungeon(entry.Zone))
                            playersInRaidAndDungeons.Add(entry.Name);
                        if (StaticValues.IsZoneBattleground(entry.Zone))
                            playersInBattlegrounds.Add(entry.Name);
                        if (entry.Level == 60)
                            playersAtLvl60.Add(entry.Name);
                    }
                }

                addGraphEntry();

                graphSection += "<div class='fame' style='min-width: 1100px; max-width: 1100px'>" + PageUtility.CreateGraph(dataX,
                        new List<Tuple<List<int>, System.Drawing.Color>>
                        {
                            new Tuple<List<int>, System.Drawing.Color>(dataY5, System.Drawing.Color.Cyan),
                            new Tuple<List<int>, System.Drawing.Color>(dataY4, System.Drawing.Color.Purple),
                            new Tuple<List<int>, System.Drawing.Color>(dataY3, System.Drawing.Color.Red),
                            new Tuple<List<int>, System.Drawing.Color>(dataY2, System.Drawing.Color.Yellow),
                            new Tuple<List<int>, System.Drawing.Color>(dataY1, System.Drawing.Color.Green)
                        },
                        labels
                        ,1100, 400) + "</div>";
            }
            m_BodyHTML = new MvcHtmlString(graphSection);
        }
    }
}