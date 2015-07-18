using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PlayerHistory = VF_RealmPlayersDatabase.PlayerData.PlayerHistory;
namespace RealmPlayersServer
{
    public class HistoryGenerator
    {

        public enum BasedOn
        {
            Nothing,
            ThisWeekHK,
            LastWeekHK,
            TodayHK,
            YesterdayHK,
            TodayHK_And_YesterdayHK,
        }
        public struct HonorHKDay
        {
            public DateTime m_DateTime;
            public BasedOn m_BasedOn;
            public int m_Accuracy;
            public int m_HonorValue;
            public int m_HKValue;
        }
        public static List<HonorHKDay> GeneratePlayerHonor(PlayerHistory _PlayerHistory)
        {
            DateTime dayAccuracyLoggingDate = DateTime.Parse("2013-08-02 01:00:00");
            DateTime firstTime = _PlayerHistory.HonorHistory.First().Uploader.GetTime().ToLocalTime();
            DateTime lastTime = _PlayerHistory.HonorHistory.Last().Uploader.GetTime().ToLocalTime();

            firstTime = firstTime.AddHours(-firstTime.Hour);
            firstTime = firstTime.AddMinutes(-firstTime.Minute);
            //firstTime = firstTime.ToLocalTime();

            //lastTime = lastTime.AddHours(-lastTime.Hour);
            //lastTime = lastTime.AddMinutes(-lastTime.Minute);
            //lastTime = lastTime.ToLocalTime();

            List<HonorHKDay> dataList = new List<HonorHKDay>();
            int totalDays = (int)((lastTime - firstTime).TotalDays + 1);
            DateTime currentDate = firstTime;
            for (int i = 0; i < totalDays; ++i)
            {
                bool todayAddedHKDay = false;
                DateTime researchDate = currentDate.AddHours(23.59);
                var honorItem = _PlayerHistory.GetHonorItemAtTime(researchDate);
                if ((researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalHours < 23.59)
                {
                    dataList.Add(new HonorHKDay
                        { m_DateTime = currentDate
                        , m_BasedOn = BasedOn.TodayHK
                        , m_Accuracy = (int)(researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalHours
                        , m_HonorValue = -1
                        , m_HKValue = honorItem.Data.TodayHK
                        });
                    todayAddedHKDay = true;
                }
                researchDate = researchDate.AddHours(24.0);
                honorItem = _PlayerHistory.GetHonorItemAtTime(researchDate);
                if ((researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalHours < 23.59)
                {
                    if (todayAddedHKDay == true)
                    {
                        var dataLast = dataList.Last();
                        if (dataLast.m_HKValue == honorItem.Data.YesterdayHK)
                        {
                            dataLast.m_HonorValue = honorItem.Data.YesterdayHonor;
                            dataLast.m_BasedOn = BasedOn.TodayHK_And_YesterdayHK;
                            dataLast.m_Accuracy = (int)(researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalHours;
                            dataList[dataList.Count - 1] = dataLast;
                        }
                    }
                    dataList.Add(new HonorHKDay
                        { m_DateTime = currentDate
                        , m_BasedOn = BasedOn.YesterdayHK
                        , m_Accuracy = (int)(researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalHours
                        , m_HonorValue = honorItem.Data.YesterdayHonor
                        , m_HKValue = honorItem.Data.YesterdayHK
                        });
                    todayAddedHKDay = true;
                }
                if (todayAddedHKDay == false)
                {
                    dataList.Add(new HonorHKDay
                    { m_DateTime = currentDate
                    , m_BasedOn = BasedOn.Nothing
                    , m_Accuracy = 0
                    , m_HonorValue = 0
                    , m_HKValue = 0
                    });
                }
                currentDate = currentDate.AddDays(1.0);
            }
            for (int i = 0; i < dataList.Count; ++i)
            {
                var currData = dataList[i];
                if (currData.m_BasedOn == BasedOn.Nothing || (currData.m_DateTime < dayAccuracyLoggingDate && currData.m_HKValue == 0))
                {
                    DateTime weekUpdateDate = DateTime_GotoSunday(currData.m_DateTime.AddDays(7));
                    DateTime researchDate = weekUpdateDate.AddDays(6.95);
                    var honorItem = _PlayerHistory.GetHonorItemAtTime(researchDate);
                    if ((researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalDays < 6.95)
                    {
                        currData.m_Accuracy = (int)(researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalDays;
                        currData.m_HKValue = honorItem.Data.LastWeekHK / 7;
                        currData.m_HonorValue = honorItem.Data.LastWeekHonor / 7;
                        currData.m_BasedOn = BasedOn.LastWeekHK;
                        dataList[i] = currData;
                    }
                    else
                    {
                        double currentDaysInThisWeek = (honorItem.Uploader.GetTime().ToLocalTime() - DateTime_GotoSunday(currData.m_DateTime)).TotalDays;
                        if (currentDaysInThisWeek < 1.0)
                            currentDaysInThisWeek = 1.0;
                        currData.m_Accuracy = (int)(researchDate - honorItem.Uploader.GetTime().ToLocalTime()).TotalDays;
                        currData.m_HKValue = (int)(honorItem.Data.ThisWeekHK / currentDaysInThisWeek);
                        currData.m_HonorValue = (int)(honorItem.Data.ThisWeekHonor / currentDaysInThisWeek);
                        currData.m_BasedOn = BasedOn.ThisWeekHK;
                        dataList[i] = currData;
                    }
                }
                if (currData.m_HonorValue == -1)
                {
                    DateTime weekUpdateDate = DateTime_GotoSunday(currData.m_DateTime.AddDays(7));
                    DateTime researchDate = weekUpdateDate.AddDays(6.95);
                    var honorItem = _PlayerHistory.GetHonorItemAtTime(researchDate);
                    double currentDaysInThisWeek = (honorItem.Uploader.GetTime().ToLocalTime() - DateTime_GotoSunday(currData.m_DateTime)).TotalDays;
                    if (currentDaysInThisWeek < 1.0)
                        currentDaysInThisWeek = 1.0;
                    currData.m_HonorValue = (int)(honorItem.Data.ThisWeekHonor / currentDaysInThisWeek);
                    currData.m_BasedOn = BasedOn.ThisWeekHK;
                    dataList[i] = currData;
                }
            }
            //dataList.Distinct();
            return dataList.Distinct().ToList();
        }
        public static DateTime DateTime_GotoSunday(DateTime _DateTime)
        {
            DateTime rankDate = _DateTime;
            rankDate = rankDate.AddHours(-rankDate.Hour);
            rankDate = rankDate.AddMinutes(-rankDate.Minute);
            rankDate = rankDate.AddDays(DayOfWeek.Sunday - rankDate.DayOfWeek);
            return rankDate;
        }

        public static Dictionary<DateTime, List<VF_RealmPlayersDatabase.PlayerData.ItemInfo>> GenerateLatestReceivedItems(PlayerHistory _PlayerHistory, DateTime _EarliestDate)
        {
            return GenerateLatestReceivedItems(_PlayerHistory, null, _EarliestDate);
        }
        public static Dictionary<DateTime, List<VF_RealmPlayersDatabase.PlayerData.ItemInfo>> GenerateLatestReceivedItems(PlayerHistory _PlayerHistory, VF_RealmPlayersDatabase.PlayerData.ExtraData _ExtraData, DateTime _EarliestDate)
        {
            Dictionary<DateTime, List<VF_RealmPlayersDatabase.PlayerData.ItemInfo>> recvItems = new Dictionary<DateTime, List<VF_RealmPlayersDatabase.PlayerData.ItemInfo>>();
            List<int> itemIDs = new List<int>();
            List<int> duplicateItemIDs = new List<int>();
            foreach (var gearHistoryItem in _PlayerHistory.GearHistory)
            {
                DateTime historyDate = gearHistoryItem.Uploader.GetTime().Date;

                int oneRingID = -1;
                int oneTrinketID = -1;
                int oneWeaponID = -1;
                foreach (var item in gearHistoryItem.Data.Items)
                {
                    int currItemID = item.Value.ItemID;
                    if (itemIDs.Contains(currItemID) == false)
                    {
                        itemIDs.Add(currItemID);
                        if(historyDate > _EarliestDate)
                            recvItems.AddToList(historyDate, item.Value);
                    }
                    else
                    {
                        if (currItemID == oneRingID || currItemID == oneTrinketID || currItemID == oneWeaponID)
                        {
                            if (duplicateItemIDs.Contains(currItemID) == false)
                            {
                                duplicateItemIDs.Add(currItemID);
                                if (historyDate > _EarliestDate)
                                    recvItems.AddToList(historyDate, item.Value);
                            }
                        }
                    }
                    if (item.Key == VF_RealmPlayersDatabase.ItemSlot.Finger_1 || item.Key == VF_RealmPlayersDatabase.ItemSlot.Finger_2)
                        oneRingID = currItemID;
                    else if (item.Key == VF_RealmPlayersDatabase.ItemSlot.Trinket_1 || item.Key == VF_RealmPlayersDatabase.ItemSlot.Trinket_2)
                        oneTrinketID = currItemID;
                    else if (item.Key == VF_RealmPlayersDatabase.ItemSlot.Main_Hand || item.Key == VF_RealmPlayersDatabase.ItemSlot.Off_Hand)
                        oneWeaponID = currItemID;
                }
            }
            if (_ExtraData != null)
            {
                try
                {
                    foreach(var mount in _ExtraData.Mounts)
                    {
                        var mountRecvDateTime = mount.GetEarliestUpload().GetTime();
                        var mountItem = new VF_RealmPlayersDatabase.PlayerData.ItemInfo { ItemID = VF.ItemTranslations.FindItemID(mount.Mount), SuffixID = 0, EnchantID = 0, UniqueID = 0, GemIDs = null, Slot = VF_RealmPlayersDatabase.ItemSlot.Unknown };
                        recvItems.AddToList(mountRecvDateTime, mountItem);
                    }
                }
                catch (Exception ex)
                { 
                    Logger.LogException(ex); 
                }
            }
            return recvItems;
        }
    }
}