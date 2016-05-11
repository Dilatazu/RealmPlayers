using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

using UtilityClass = VF_RealmPlayersDatabase.PlayerData.PlayerHistoryUtility;
namespace VF_RealmPlayersDatabase.PlayerData
{
    [ProtoContract]
    [Serializable]
    public class PlayerHistory : ISerializable
    {
        [ProtoMember(1)]
        public List<CharacterDataHistoryItem> CharacterHistory = new List<CharacterDataHistoryItem>();
        [ProtoMember(2)]
        public List<GuildDataHistoryItem> GuildHistory = new List<GuildDataHistoryItem>();
        [ProtoMember(3)]
        public List<HonorDataHistoryItem> HonorHistory = new List<HonorDataHistoryItem>();
        [ProtoMember(4)]
        public List<GearDataHistoryItem> GearHistory = new List<GearDataHistoryItem>();
        [ProtoMember(5)]
        public List<ArenaDataHistoryItem> ArenaHistory = null;
        [ProtoMember(6)]
        public List<TalentsDataHistoryItem> TalentsHistory = null;

        public PlayerHistory()
        { }

        public bool HaveValidHistory()
        {
            return (CharacterHistory.Count > 0 && GuildHistory.Count > 0 && HonorHistory.Count > 0 && GearHistory.Count > 0);
        }

        public bool IsEqual(PlayerHistory _PlayerHistory)
        {
            if (CharacterHistory.Count != _PlayerHistory.CharacterHistory.Count
            || GuildHistory.Count != _PlayerHistory.GuildHistory.Count
            || HonorHistory.Count != _PlayerHistory.HonorHistory.Count
            || GearHistory.Count != _PlayerHistory.GearHistory.Count
            || (ArenaHistory == null && _PlayerHistory.ArenaHistory != null)
            || (ArenaHistory != null && _PlayerHistory.ArenaHistory == null)
            || (ArenaHistory != null && _PlayerHistory.ArenaHistory != null && ArenaHistory.Count != _PlayerHistory.ArenaHistory.Count)
            || (TalentsHistory == null && _PlayerHistory.TalentsHistory != null)
            || (TalentsHistory != null && _PlayerHistory.TalentsHistory == null)
            || (TalentsHistory != null && _PlayerHistory.TalentsHistory != null && TalentsHistory.Count != _PlayerHistory.TalentsHistory.Count))
                return false;

            for (int i = 0; i < CharacterHistory.Count; ++i)
            {
                if (CharacterHistory[i].Data.IsSame(_PlayerHistory.CharacterHistory[i].Data) == false)
                    return false;
                if (CharacterHistory[i].Uploader.GetTime() != _PlayerHistory.CharacterHistory[i].Uploader.GetTime()
                || CharacterHistory[i].Uploader.GetContributorID() != _PlayerHistory.CharacterHistory[i].Uploader.GetContributorID())
                    return false;
            }
            for (int i = 0; i < GuildHistory.Count; ++i)
            {
                if (GuildHistory[i].Data.IsSame(_PlayerHistory.GuildHistory[i].Data) == false)
                    return false;
                if (GuildHistory[i].Uploader.GetTime() != _PlayerHistory.GuildHistory[i].Uploader.GetTime()
                || GuildHistory[i].Uploader.GetContributorID() != _PlayerHistory.GuildHistory[i].Uploader.GetContributorID())
                    return false;
            }
            for (int i = 0; i < HonorHistory.Count; ++i)
            {
                if (HonorHistory[i].Data.IsSame(_PlayerHistory.HonorHistory[i].Data) == false)
                    return false;
                if (HonorHistory[i].Uploader.GetTime() != _PlayerHistory.HonorHistory[i].Uploader.GetTime()
                || HonorHistory[i].Uploader.GetContributorID() != _PlayerHistory.HonorHistory[i].Uploader.GetContributorID())
                    return false;
            }
            for (int i = 0; i < GearHistory.Count; ++i)
            {
                if (GearHistory[i].Data.IsSame(_PlayerHistory.GearHistory[i].Data) == false)
                    return false;
                if (GearHistory[i].Uploader.GetTime() != _PlayerHistory.GearHistory[i].Uploader.GetTime()
                || GearHistory[i].Uploader.GetContributorID() != _PlayerHistory.GearHistory[i].Uploader.GetContributorID())
                    return false;
            }
            if (ArenaHistory != null)
            {
                for (int i = 0; i < ArenaHistory.Count; ++i)
                {
                    if (ArenaHistory[i].Data.IsSame(_PlayerHistory.ArenaHistory[i].Data) == false)
                        return false;
                    if (ArenaHistory[i].Uploader.GetTime() != _PlayerHistory.ArenaHistory[i].Uploader.GetTime()
                    || ArenaHistory[i].Uploader.GetContributorID() != _PlayerHistory.ArenaHistory[i].Uploader.GetContributorID())
                        return false;
                }
            }
            if (TalentsHistory != null)
            {
                for (int i = 0; i < TalentsHistory.Count; ++i)
                {
                    if (TalentsHistory[i].Data != _PlayerHistory.TalentsHistory[i].Data)
                        return false;
                    if (TalentsHistory[i].Uploader.GetTime() != _PlayerHistory.TalentsHistory[i].Uploader.GetTime()
                    || TalentsHistory[i].Uploader.GetContributorID() != _PlayerHistory.TalentsHistory[i].Uploader.GetContributorID())
                        return false;
                }
            }

            return true;
        }

        public UploadID GetEarliestUploader()
        {
            UploadID earlierUploader = UploadID.NullMax();
            if (CharacterHistory.Count > 0)
            {
                UploadID uploader = CharacterHistory.First().Uploader;
                if (uploader.GetTime() < earlierUploader.GetTime())
                    earlierUploader = uploader;
            }
            if (GuildHistory.Count > 0)
            {
                UploadID uploader = GuildHistory.First().Uploader;
                if (uploader.GetTime() < earlierUploader.GetTime())
                    earlierUploader = uploader;
            }
            if (HonorHistory.Count > 0)
            {
                UploadID uploader = HonorHistory.First().Uploader;
                if (uploader.GetTime() < earlierUploader.GetTime())
                    earlierUploader = uploader;
            }
            if (GearHistory.Count > 0)
            {
                UploadID uploader = GearHistory.First().Uploader;
                if (uploader.GetTime() < earlierUploader.GetTime())
                    earlierUploader = uploader;
            }
            if (ArenaHistory != null && ArenaHistory.Count > 0)
            {
                UploadID uploader = ArenaHistory.First().Uploader;
                if (uploader.GetTime() < earlierUploader.GetTime())
                    earlierUploader = uploader;
            }
            if (TalentsHistory != null && TalentsHistory.Count > 0)
            {
                UploadID uploader = TalentsHistory.First().Uploader;
                if (uploader.GetTime() < earlierUploader.GetTime())
                    earlierUploader = uploader;
            }
            return earlierUploader;
        }
        public DateTime GetEarliestDateTime()
        {
            return GetEarliestUploader().GetTime();
        }
        public DateTime GetLatestDateTime()
        {
            DateTime latestDateTime = DateTime.MinValue;
            if (CharacterHistory.Count > 0)
            {
                DateTime dateTime = CharacterHistory.Last().Uploader.GetTime();
                if (dateTime > latestDateTime)
                    latestDateTime = dateTime;
            }
            if (GuildHistory.Count > 0)
            {
                DateTime dateTime = GuildHistory.Last().Uploader.GetTime();
                if (dateTime > latestDateTime)
                    latestDateTime = dateTime;
            }
            if (HonorHistory.Count > 0)
            {
                DateTime dateTime = HonorHistory.Last().Uploader.GetTime();
                if (dateTime > latestDateTime)
                    latestDateTime = dateTime;
            }
            if (GearHistory.Count > 0)
            {
                DateTime dateTime = GearHistory.Last().Uploader.GetTime();
                if (dateTime > latestDateTime)
                    latestDateTime = dateTime;
            }
            if (ArenaHistory != null && ArenaHistory.Count > 0)
            {
                DateTime dateTime = ArenaHistory.Last().Uploader.GetTime();
                if (dateTime > latestDateTime)
                    latestDateTime = dateTime;
            }
            if (TalentsHistory != null && TalentsHistory.Count > 0)
            {
                DateTime dateTime = TalentsHistory.Last().Uploader.GetTime();
                if (dateTime > latestDateTime)
                    latestDateTime = dateTime;
            }
            return latestDateTime;
        }
        public DateTime GetDateAtUploadNr(int _UploadNR, DateTime _DefaultValue)
        {
            var updates = GetUpdates();
            if (_UploadNR >= updates.Count)
                _UploadNR = updates.Count - 1;

            return updates[_UploadNR].GetTime();
        }
        public PlayerHistory ExtractOldHistory(DateTime _NewestData, bool _RemoveOld)
        {
            PlayerHistory extractedHistory = new PlayerHistory();
            extractedHistory.CharacterHistory = ExtractOldHistoryItems_T(CharacterHistory, (_Item) => { return (_Item.Uploader.GetTime() < _NewestData); }, _RemoveOld);
            extractedHistory.GuildHistory = ExtractOldHistoryItems_T(GuildHistory, (_Item) => { return (_Item.Uploader.GetTime() < _NewestData); }, _RemoveOld);
            extractedHistory.HonorHistory = ExtractOldHistoryItems_T(HonorHistory, (_Item) => { return (_Item.Uploader.GetTime() < _NewestData); }, _RemoveOld);
            extractedHistory.GearHistory = ExtractOldHistoryItems_T(GearHistory, (_Item) => { return (_Item.Uploader.GetTime() < _NewestData); }, _RemoveOld);
            if (ArenaHistory != null)
                extractedHistory.ArenaHistory = ExtractOldHistoryItems_T(ArenaHistory, (_Item) => { return (_Item.Uploader.GetTime() < _NewestData); }, _RemoveOld);
            if (TalentsHistory != null)
                extractedHistory.TalentsHistory = ExtractOldHistoryItems_T(TalentsHistory, (_Item) => { return (_Item.Uploader.GetTime() < _NewestData); }, _RemoveOld);
            return extractedHistory;
        }
        public void AddOldHistory(PlayerHistory _OldHistory)
        {
            AddOldHistoryItems_T(CharacterHistory, _OldHistory.CharacterHistory, CharacterDataHistoryItem.Time1BiggerThan2);
            AddOldHistoryItems_T(GuildHistory, _OldHistory.GuildHistory, GuildDataHistoryItem.Time1BiggerThan2);
            AddOldHistoryItems_T(HonorHistory, _OldHistory.HonorHistory, HonorDataHistoryItem.Time1BiggerThan2);
            AddOldHistoryItems_T(GearHistory, _OldHistory.GearHistory, GearDataHistoryItem.Time1BiggerThan2);
            if (_OldHistory.ArenaHistory != null)
            {
                if (ArenaHistory == null)
                    ArenaHistory = new List<ArenaDataHistoryItem>();
                AddOldHistoryItems_T(ArenaHistory, _OldHistory.ArenaHistory, ArenaDataHistoryItem.Time1BiggerThan2);
            }
            if (_OldHistory.TalentsHistory != null)
            {
                if (TalentsHistory == null)
                    TalentsHistory = new List<TalentsDataHistoryItem>();
                AddOldHistoryItems_T(TalentsHistory, _OldHistory.TalentsHistory, TalentsDataHistoryItem.Time1BiggerThan2);
            }
        }
        private void AddOldHistoryItems_T<T_HistoryItem>(List<T_HistoryItem> _HistoryArray, List<T_HistoryItem> _OldHistoryArray, Func<T_HistoryItem, T_HistoryItem, bool> _Time1BiggerThan2)
        {
            if (_OldHistoryArray.Count() == 0)
                return;
            if (_HistoryArray.Count() == 0)
            {
                _HistoryArray.AddRange(_OldHistoryArray);
                return;
            }
            if (_Time1BiggerThan2(_HistoryArray.First(), _OldHistoryArray.Last())
            && _Time1BiggerThan2(_HistoryArray.First(), _OldHistoryArray.First()))
            {
                _HistoryArray.InsertRange(0, _OldHistoryArray);
            }
            //else
            //{
            //    _HistoryArray.Find((historyItem) {return _Time1BiggerThan2(historyItem
            //}
        }
        private List<T_HistoryItem> ExtractOldHistoryItems_T<T_HistoryItem>(List<T_HistoryItem> _HistoryArray, Predicate<T_HistoryItem> _Predicate, bool _RemoveOld)
        {
            int lastIndex = _HistoryArray.FindLastIndex(_Predicate);
            if (lastIndex != -1)
            {
                List<T_HistoryItem> newHistory = _HistoryArray.GetRange(0, lastIndex + 1);
                if (_RemoveOld == true)
                    _HistoryArray.RemoveRange(0, lastIndex + 1);
                return newHistory;
            }
            return new List<T_HistoryItem>();
        }
        public void AddToHistory(CharacterData _CharacterData, UploadID _Uploader)
        {
            UtilityClass.AddToHistory.RunGeneric(CharacterHistory, _CharacterData, _Uploader);
        }
        public void AddToHistory(GuildData _GuildData, UploadID _Uploader)
        {
            UtilityClass.AddToHistory.RunGeneric(GuildHistory, _GuildData, _Uploader);
        }
        public void AddToHistory(HonorData _HonorData, UploadID _Uploader)
        {
            UtilityClass.AddToHistory.RunGeneric(HonorHistory, _HonorData, _Uploader);
        }
        public void AddToHistory(GearData _GearData, UploadID _Uploader)
        {
            var extraGear = _GearData._GenerateExtraItemSets();
            if (extraGear != null)
            {
                for (int i = 0; i < extraGear.Count; ++i)
                {
                    UploadID slightyOlderUploadId = new UploadID(_Uploader.GetContributorID(), _Uploader.GetTime().AddMilliseconds(0 - i));
                    UtilityClass.AddToHistory.RunGeneric(GearHistory, extraGear[i], slightyOlderUploadId);
                }
            }
            UtilityClass.AddToHistory.RunGeneric(GearHistory, _GearData, _Uploader);
        }
        public void AddToHistory(ArenaData _ArenaData, UploadID _Uploader)
        {
            if (_ArenaData == null)
                return;
            if (ArenaHistory == null)
                ArenaHistory = new List<ArenaDataHistoryItem>();
            UtilityClass.AddToHistory.RunGeneric(ArenaHistory, _ArenaData, _Uploader);
        }
        public void AddTalentsToHistory(string _TalentsData, UploadID _Uploader)
        {
            if (_TalentsData == null)
                return;
            if (TalentsHistory == null)
                TalentsHistory = new List<TalentsDataHistoryItem>();
            UtilityClass.AddToHistory.RunGeneric(TalentsHistory, _TalentsData, _Uploader);
        }
        private T_HistoryItem GetItemAtTime_T<T_HistoryItem>(List<T_HistoryItem> _HistoryArray, DateTime _DateTime, Predicate<T_HistoryItem> _Predicate)
        {
            if (_HistoryArray.Count == 0)
                return _HistoryArray.FirstOrDefault();
                //throw new Exception("GetItemAtTime_T failed, Array was empty");
            var itemIndex = _HistoryArray.FindLastIndex(_Predicate);
            if (itemIndex == -1) itemIndex = 0;
            return _HistoryArray[itemIndex];
        }
        public CharacterDataHistoryItem GetCharacterItemAtTime(DateTime _DateTime)
        {
            return GetItemAtTime_T(CharacterHistory, _DateTime, (CharacterDataHistoryItem _Item) => { return (_Item.Uploader.GetTime() <= _DateTime); });
        }
        public GuildDataHistoryItem GetGuildItemAtTime(DateTime _DateTime)
        {
            return GetItemAtTime_T(GuildHistory, _DateTime, (GuildDataHistoryItem _Item) => { return (_Item.Uploader.GetTime() <= _DateTime); });
        }
        public HonorDataHistoryItem GetHonorItemAtTime(DateTime _DateTime)
        {
            return GetItemAtTime_T(HonorHistory, _DateTime, (HonorDataHistoryItem _Item) => { return (_Item.Uploader.GetTime() <= _DateTime); });
        }
        public GearDataHistoryItem GetGearItemAtTime(DateTime _DateTime)
        {
            return GetItemAtTime_T(GearHistory, _DateTime, (GearDataHistoryItem _Item) => { return (_Item.Uploader.GetTime() <= _DateTime); });
        }
        public ArenaDataHistoryItem GetArenaItemAtTime(DateTime _DateTime)
        {
            if (ArenaHistory == null)
            {
                if (CharacterHistory.Count > 0)
                    return new ArenaDataHistoryItem(null, CharacterHistory.First().Uploader);
                else
                    throw new Exception("Error There was no ArenaHistory and no CharacterHistory for DateTime: \"" + _DateTime.ToString() + "\"");
            }
            return GetItemAtTime_T(ArenaHistory, _DateTime, (ArenaDataHistoryItem _Item) => { return (_Item.Uploader.GetTime() <= _DateTime); });
        }
        public TalentsDataHistoryItem GetTalentsItemAtTime(DateTime _DateTime)
        {
            if (TalentsHistory == null)
            {
                if (CharacterHistory.Count > 0)
                    return new TalentsDataHistoryItem(null, CharacterHistory.First().Uploader);
                else
                    throw new Exception("Error There was no TalentsHistory and no CharacterHistory for DateTime: \"" + _DateTime.ToString() + "\"");
            }
            return GetItemAtTime_T(TalentsHistory, _DateTime, (TalentsDataHistoryItem _Item) => { return (_Item.Uploader.GetTime() <= _DateTime); });
        }
        public bool GetPlayerAtTime(string _Name, WowRealm _Realm, DateTime _DateTime, out Player _RetPlayer)
        {
            try 
	        {
                if (HaveValidHistory() == false)
                {
                    Logger.ConsoleWriteLine("Player \"" + _Name + "\" did not have Valid History!", ConsoleColor.Red);
                    _RetPlayer = null;
                    return false;
                }
                _RetPlayer = new Player(_Name, _Realm
                    , GetCharacterItemAtTime(_DateTime)
                    , GetGuildItemAtTime(_DateTime)
                    , GetHonorItemAtTime(_DateTime)
                    , GetGearItemAtTime(_DateTime)
                    , GetArenaItemAtTime(_DateTime)
                    , GetTalentsItemAtTime(_DateTime));
                return true;
	        }
	        catch (Exception ex)
	        {
                Logger.LogException(ex);
                _RetPlayer = null;
                return false;
	        }
        }
        //public class GearHistoryEqualityComparer : IEqualityComparer<GearData>
        //{
        //    public bool Equals(GearData x, GearData y)
        //    {
        //        return x.IsSame(y);
        //    }

        //    public int GetHashCode(GearData obj)
        //    {
        //        foreach(var item in obj.Items)
        //        {
        //            item.
        //        }
        //        obj.Items
        //    }
        //}


        public Dictionary<string, List<UploadID>> GetUsedTalentSpeccs()
        {
            Dictionary<string, List<UploadID>> result = new Dictionary<string, List<UploadID>>();
            if (TalentsHistory == null)
                return result;
            foreach (var talentHistoryItem in TalentsHistory)
            {
                if (talentHistoryItem.Data != null)
                {
                    result.AddToList(talentHistoryItem.Data, talentHistoryItem.Uploader);
                }
            }
            return result;
        }
        public struct GearSet
        {
            public GearData Gear;
            public int Count;
            public DateTime LastUsed;
            public void Add(GearDataHistoryItem _HistoryItem)
            {
                Count += 1;
                if (_HistoryItem.Uploader.GetTime() > LastUsed)
                    LastUsed = _HistoryItem.Uploader.GetTime();
            }
            public GearSet(GearDataHistoryItem _HistoryItem)
            {
                Gear = _HistoryItem.Data;
                Count = 0;
                LastUsed = _HistoryItem.Uploader.GetTime();
            }
        }
        public List<GearSet> GetMostCommonGearSets()
        {
            List<GearSet> gearData = new List<GearSet>();
            List<GearSet> mostUsedGearsets = new List<GearSet>();
            foreach (var gearHistoryItem in GearHistory)
            {
                var dataIndex = mostUsedGearsets.FindIndex((GearSet _Item) => { return _Item.Gear.IsSame(gearHistoryItem.Data); });
                if (dataIndex != -1)
                {
                    var data = mostUsedGearsets[dataIndex];
                    data.Add(gearHistoryItem);
                    mostUsedGearsets[dataIndex] = data;
                }
                else
                    mostUsedGearsets.Add(new GearSet(gearHistoryItem));
            }
            var orderedList = mostUsedGearsets.OrderByDescending((GearSet _Item) => { return _Item.Count; });
            foreach (var data in orderedList)
            {
                gearData.Add(data);
            }
            return gearData;
        }
        public bool HaveItem(ItemInfo _Item)
        {
            foreach (var gear in GearHistory)
            {
                if (gear.Data.Items.ContainsKey(_Item.Slot) == true)
                {
                    if (gear.Data.Items[_Item.Slot].ItemID == _Item.ItemID)
                        return true;
                }
            }
            return false;
        }
        public List<CharacterDataHistoryItem> GetRaceOrSexChanges()
        {
            List<CharacterDataHistoryItem> result = new List<CharacterDataHistoryItem>();
            var firstCharHistory = CharacterHistory.First();

            result.Add(firstCharHistory);
            var prevCharHistory = firstCharHistory;
            var prevHonorHistory = HonorHistory.First();
            foreach (var charHistory in CharacterHistory)
            {
                if (charHistory.Data.Race != prevCharHistory.Data.Race || charHistory.Data.Sex != prevCharHistory.Data.Sex)
                {
                    var honorHistory = GetHonorItemAtTime(charHistory.Uploader.GetTime());
                    if (charHistory.Data.Level < prevCharHistory.Data.Level
                    || honorHistory.Data.LifetimeHighestRank < prevHonorHistory.Data.LifetimeHighestRank
                    || honorHistory.Data.LifetimeHK * 1.25 < prevHonorHistory.Data.LifetimeHK
                    || prevCharHistory.Data.Level < 30)
                    {
                        //Restart the results, inconsistency in the honor or levels, character must have been deleted and new character with same name created
                        result.Clear();
                    }
                    result.Add(charHistory);
                    prevHonorHistory = honorHistory;
                }
                prevCharHistory = charHistory;
            }
            return result;
        }
        public List<UploadID> GetUpdates()
        {
            List<UploadID> uploads = new List<UploadID>();

            foreach (var historyItem in CharacterHistory)
                uploads.Add(historyItem.Uploader);
            foreach (var historyItem in GearHistory)
                uploads.Add(historyItem.Uploader);
            foreach (var historyItem in HonorHistory)
                uploads.Add(historyItem.Uploader);
            foreach (var historyItem in GuildHistory)
                uploads.Add(historyItem.Uploader);
            if (ArenaHistory != null)
            {
                foreach (var historyItem in ArenaHistory)
                    uploads.Add(historyItem.Uploader);
            }
            if (TalentsHistory != null)
            {
                foreach (var historyItem in TalentsHistory)
                    uploads.Add(historyItem.Uploader);
            }

            return uploads.Distinct().ToList();
        }
        public int RollbackPlayer(Player _Player, DateTime _DateTime)
        {
            int removedHistoryData = 0;
            try
            {
                removedHistoryData += CharacterHistory.RemoveAll((CharacterDataHistoryItem _Item) => { return _Item.Uploader.GetTime() > _DateTime; });
                removedHistoryData += GuildHistory.RemoveAll((GuildDataHistoryItem _Item) => { return _Item.Uploader.GetTime() > _DateTime; });
                removedHistoryData += HonorHistory.RemoveAll((HonorDataHistoryItem _Item) => { return _Item.Uploader.GetTime() > _DateTime; });
                removedHistoryData += GearHistory.RemoveAll((GearDataHistoryItem _Item) => { return _Item.Uploader.GetTime() > _DateTime; });
                if (ArenaHistory != null)
                {
                    removedHistoryData += ArenaHistory.RemoveAll((ArenaDataHistoryItem _Item) => { return _Item.Uploader.GetTime() > _DateTime; });
                }
                if (TalentsHistory != null)
                {
                    removedHistoryData += TalentsHistory.RemoveAll((TalentsDataHistoryItem _Item) => { return _Item.Uploader.GetTime() > _DateTime; });
                }
                if (removedHistoryData > 0)
                    _RollbackPlayer(_Player, (_UploadID) => { if (_UploadID.GetTime() > _DateTime) return true; return false; });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return -removedHistoryData;
            }
            return removedHistoryData;
        }
        public int RollbackPlayer(Player _Player, Contributor _Contributor)
        {
            var contributorID = _Contributor.GetContributorID();
            int removedHistoryData = 0;
            try
            {
                removedHistoryData += CharacterHistory.RemoveAll((CharacterDataHistoryItem _Item) => { return _Item.Uploader.GetContributorID() == contributorID; });
                removedHistoryData += GuildHistory.RemoveAll((GuildDataHistoryItem _Item) => { return _Item.Uploader.GetContributorID() == contributorID; });
                removedHistoryData += HonorHistory.RemoveAll((HonorDataHistoryItem _Item) => { return _Item.Uploader.GetContributorID() == contributorID; });
                removedHistoryData += GearHistory.RemoveAll((GearDataHistoryItem _Item) => { return _Item.Uploader.GetContributorID() == contributorID; });
                if (ArenaHistory != null)
                {
                    removedHistoryData += ArenaHistory.RemoveAll((ArenaDataHistoryItem _Item) => { return _Item.Uploader.GetContributorID() == contributorID; });
                }
                if (TalentsHistory != null)
                {
                    removedHistoryData += TalentsHistory.RemoveAll((TalentsDataHistoryItem _Item) => { return _Item.Uploader.GetContributorID() == contributorID; });
                }
                if(removedHistoryData > 0)
                    _RollbackPlayer(_Player, (_UploadID) => { if (_UploadID.GetContributorID() == contributorID) return true; return false; });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return -removedHistoryData;
            }
            return removedHistoryData;
        }
        public int RollbackPlayer(Player _Player, Contributor _Contributor, DateTime _DateTime)
        {
            var contributorID = _Contributor.GetContributorID();
            int removedHistoryData = 0;
            try
            {
                removedHistoryData += CharacterHistory.RemoveAll((CharacterDataHistoryItem _Item) => { return (_Item.Uploader.GetContributorID() == contributorID) && (_Item.Uploader.GetTime() > _DateTime); });
                removedHistoryData += GuildHistory.RemoveAll((GuildDataHistoryItem _Item) => { return (_Item.Uploader.GetContributorID() == contributorID) && (_Item.Uploader.GetTime() > _DateTime); });
                removedHistoryData += HonorHistory.RemoveAll((HonorDataHistoryItem _Item) => { return (_Item.Uploader.GetContributorID() == contributorID) && (_Item.Uploader.GetTime() > _DateTime); });
                removedHistoryData += GearHistory.RemoveAll((GearDataHistoryItem _Item) => { return (_Item.Uploader.GetContributorID() == contributorID) && (_Item.Uploader.GetTime() > _DateTime); });
                if (ArenaHistory != null)
                {
                    removedHistoryData += ArenaHistory.RemoveAll((ArenaDataHistoryItem _Item) => { return (_Item.Uploader.GetContributorID() == contributorID) && (_Item.Uploader.GetTime() > _DateTime); });
                }
                if (TalentsHistory != null)
                {
                    removedHistoryData += TalentsHistory.RemoveAll((TalentsDataHistoryItem _Item) => { return (_Item.Uploader.GetContributorID() == contributorID) && (_Item.Uploader.GetTime() > _DateTime); });
                }
                if (removedHistoryData > 0)
                    _RollbackPlayer(_Player, (_UploadID) => { if (_UploadID.GetContributorID() == contributorID && (_UploadID.GetTime() > _DateTime)) return true; return false; });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return -removedHistoryData;
            }
            return removedHistoryData;
        }
        private void _RollbackPlayer(Player _Player, Func<UploadID, bool> _IsOutdated)
        {
            if (_IsOutdated(_Player.Uploader))
            {
                if (CharacterHistory.Count > 0)
                {
                    var lastData = CharacterHistory.Last();
                    _Player.Character = lastData.Data;
                    _Player.LastSeen = lastData.Uploader.GetTime();
                    _Player.Uploader = lastData.Uploader;
                }
                if (GuildHistory.Count > 0)
                {
                    var lastData = GuildHistory.Last();
                    _Player.Guild = lastData.Data;
                    if (_Player.LastSeen < lastData.Uploader.GetTime())
                    {
                        _Player.LastSeen = lastData.Uploader.GetTime();
                        _Player.Uploader = lastData.Uploader;
                    }
                }
                if (HonorHistory.Count > 0)
                {
                    var lastData = HonorHistory.Last();
                    _Player.Honor = lastData.Data;
                    if (_Player.LastSeen < lastData.Uploader.GetTime())
                    {
                        _Player.LastSeen = lastData.Uploader.GetTime();
                        _Player.Uploader = lastData.Uploader;
                    }
                }
                if (GearHistory.Count > 0)
                {
                    var lastData = GearHistory.Last();
                    _Player.Gear = lastData.Data;
                    if (_Player.LastSeen < lastData.Uploader.GetTime())
                    {
                        _Player.LastSeen = lastData.Uploader.GetTime();
                        _Player.Uploader = lastData.Uploader;
                    }
                }
                if (ArenaHistory != null)
                {
                    if (ArenaHistory.Count > 0)
                    {
                        var lastData = ArenaHistory.Last();
                        _Player.Arena = lastData.Data;
                        if (_Player.LastSeen < lastData.Uploader.GetTime())
                        {
                            _Player.LastSeen = lastData.Uploader.GetTime();
                            _Player.Uploader = lastData.Uploader;
                        }
                    }
                }
                else
                {
                    _Player.Arena = null;
                }
                if (TalentsHistory != null)
                {
                    if (TalentsHistory.Count > 0)
                    {
                        var lastData = TalentsHistory.Last();
                        _Player.TalentPointsData = lastData.Data;
                        if (_Player.LastSeen < lastData.Uploader.GetTime())
                        {
                            _Player.LastSeen = lastData.Uploader.GetTime();
                            _Player.Uploader = lastData.Uploader;
                        }
                    }
                }
                else
                {
                    _Player.TalentPointsData = null;
                }
            }
        }

        #region Serializing
        public PlayerHistory(SerializationInfo _Info, StreamingContext _Context)
        {
            CharacterHistory = (List<CharacterDataHistoryItem>)_Info.GetValue("CharacterHistory", typeof(List<CharacterDataHistoryItem>));
            GuildHistory = (List<GuildDataHistoryItem>)_Info.GetValue("GuildHistory", typeof(List<GuildDataHistoryItem>));
            HonorHistory = (List<HonorDataHistoryItem>)_Info.GetValue("HonorHistory", typeof(List<HonorDataHistoryItem>));
            GearHistory = (List<GearDataHistoryItem>)_Info.GetValue("GearHistory", typeof(List<GearDataHistoryItem>));
        }
        public void GetObjectData(SerializationInfo _Info, StreamingContext _Context)
        {
            _Info.AddValue("CharacterHistory", CharacterHistory);
            _Info.AddValue("GuildHistory", GuildHistory);
            _Info.AddValue("HonorHistory", HonorHistory);
            _Info.AddValue("GearHistory", GearHistory);
        }
        #endregion

    }
}
