using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using RPPDatabase = VF_RealmPlayersDatabase.Database;
using WowRealm = VF_RealmPlayersDatabase.WowRealm;
using ItemSlot = VF_RealmPlayersDatabase.ItemSlot;
using ItemInfo = VF_RealmPlayersDatabase.PlayerData.ItemInfo;

namespace VF_RPDatabase
{
    [ProtoContract]
    public class ItemSummary
    {
        [ProtoMember(1)]
        public int m_ItemID;
        [ProtoMember(2)]
        public int m_SuffixID;
        [ProtoMember(3)]
        public List<Tuple<UInt64, DateTime>> m_ItemOwners = new List<Tuple<UInt64, DateTime>>();

        public ItemSummary() { }
        public ItemSummary(ItemInfo _Item)
        {
            m_ItemID = _Item.ItemID;
            m_SuffixID = _Item.SuffixID;
        }
        public ItemSummary(int _ItemID, int _SuffixID)
        {
            m_ItemID = _ItemID;
            m_SuffixID = _SuffixID;
        }

        public bool AddOwner(UInt64 _PlayerID, DateTime _DateTime, bool _AllowDuplicate = false)
        {
            var entries = m_ItemOwners.Where((_Value) => _Value.Item1 == _PlayerID);
            int entriesCount = entries.Count();
            if (entriesCount == 0 || (entriesCount == 1 && _AllowDuplicate))
            {
                m_ItemOwners.Add(Tuple.Create(_PlayerID, _DateTime));
                return true;
            }
            return false;
        }

        public List<Tuple<UInt64, DateTime>> ItemOwners
        {
            get { return m_ItemOwners; }
        }
    }
    [ProtoContract]
    public class ItemSummaryDatabase
    {
        [ProtoMember(1)]
        public Dictionary<UInt64, ItemSummary> m_Items = new Dictionary<UInt64, ItemSummary>();
        [ProtoMember(2)]
        private Dictionary<string, UInt64> m_PlayerIDs = new Dictionary<string, UInt64>();

        [ProtoMember(3)]
        public UInt64 m_EntityCounter_Emerald_Dream = 0L;
        [ProtoMember(4)]
        public UInt64 m_EntityCounter_Warsong = 0L;
        [ProtoMember(5)]
        public UInt64 m_EntityCounter_Al_Akir = 0L;
        [ProtoMember(6)]
        public UInt64 m_EntityCounter_Valkyrie = 0L;
        [ProtoMember(7)]
        public UInt64 m_EntityCounter_VanillaGaming = 0L;
        [ProtoMember(8)]
        public UInt64 m_EntityCounter_Rebirth = 0L;
        [ProtoMember(9)]
        public UInt64 m_EntityCounter_Archangel = 0L;
        [ProtoMember(10)]
        public UInt64 m_EntityCounter_Nostalrius = 0L;
        [ProtoMember(11)]
        public UInt64 m_EntityCounter_Kronos = 0L;
        [ProtoMember(12)]
        public UInt64 m_EntityCounter_NostalGeek = 0L;
        [ProtoMember(13)]
        public UInt64 m_EntityCounter_Nefarian = 0L;
        [ProtoMember(14)]
        public UInt64 m_EntityCounter_NostalriusPVE = 0L;
        [ProtoMember(15)]
        public UInt64 m_EntityCounter_WarsongTBC = 0L;

        private void CalcRealmBits(WowRealm _Realm, out UInt64 _BitMask, out UInt64 _RealmValue)
        {
            _BitMask = 0xFFUL << 56;
            _RealmValue = _BitMask;
            switch (_Realm)
	        {
                case WowRealm.Emerald_Dream:
                    _RealmValue = 1UL << 56;
                    break;
                case WowRealm.Warsong:
                    _RealmValue = 2UL << 56;
                    break;
                case WowRealm.Al_Akir:
                    _RealmValue = 3UL << 56;
                    break;
                case WowRealm.Valkyrie:
                    _RealmValue = 4UL << 56;
                    break;
                case WowRealm.VanillaGaming:
                    _RealmValue = 5UL << 56;
                    break;
                case WowRealm.Rebirth:
                    _RealmValue = 6UL << 56;
                    break;
                case WowRealm.Archangel:
                    _RealmValue = 7UL << 56;
                    break;
                case WowRealm.Nostalrius:
                    _RealmValue = 8UL << 56;
                    break;
                case WowRealm.Kronos:
                    _RealmValue = 9UL << 56;
                    break;
                case WowRealm.NostalGeek:
                    _RealmValue = 10UL << 56;
                    break;
                case WowRealm.Nefarian:
                    _RealmValue = 11UL << 56;
                    break;
                case WowRealm.NostalriusPVE:
                    _RealmValue = 12UL << 56;
                    break;
                case WowRealm.WarsongTBC:
                    _RealmValue = 13UL << 56;
                    break;
            }
        }
        public int GetItemUsageCount(WowRealm _Realm, ItemInfo _Item)
        {
            return GetItemUsageCount(_Realm, _Item.ItemID, _Item.SuffixID);
        }
        public int GetItemUsageCount(WowRealm _Realm, int _ItemID, int _SuffixID)
        {
            int usageCount = 0;
            var itemOwners = GetItemSummary(_ItemID, _SuffixID).ItemOwners;

            UInt64 bitMask;
            UInt64 realmValue;
            CalcRealmBits(_Realm, out bitMask, out realmValue);
            foreach (var itemOwner in itemOwners)
            {
                if ((itemOwner.Item1 & bitMask) == realmValue)
                    ++usageCount;
            }
            return usageCount;
        }

        public List<Tuple<DateTime, string>> GetItemUsage(WowRealm _Realm, int _ItemID, int _SuffixID)
        {
            List<Tuple<DateTime, string>> retList = new List<Tuple<DateTime,string>>();
            var itemOwners = GetItemSummary(_ItemID, _SuffixID).ItemOwners;

            UInt64 bitMask;
            UInt64 realmValue;
            CalcRealmBits(_Realm, out bitMask, out realmValue);
            foreach (var itemOwner in itemOwners)
            {
                if ((itemOwner.Item1 & bitMask) == realmValue)
                {
                    var player = m_PlayerIDs.First((_Value) => _Value.Value == itemOwner.Item1);
                    if (player.Key.StartsWith("R"))
                        retList.Add(Tuple.Create(itemOwner.Item2, player.Key.Substring(3)));
                    else
                        retList.Add(Tuple.Create(itemOwner.Item2, player.Key.Substring(1)));
                }
            }
            return retList;
        }

        private ItemSummary GetItemSummary(ItemInfo _Item)
        {
            return GetItemSummary(_Item.ItemID, _Item.SuffixID);
        }
        private ItemSummary GetItemSummary(int _ItemID, int _SuffixID)
        {
            if (_SuffixID < 0)
                _SuffixID = 0;
            if (_ItemID < 0)
                _ItemID = 0;
            UInt64 uniqueitemID = (((UInt64)_SuffixID) << 32) | ((UInt64)(UInt32)_ItemID);
            ItemSummary itemSummary;
            if (m_Items.TryGetValue(uniqueitemID, out itemSummary) == false)
            {
                itemSummary = new ItemSummary(_ItemID, _SuffixID);
                m_Items.Add(uniqueitemID, itemSummary);
            }
            return itemSummary;
        }
        private UInt64 GetEntityID(WowRealm _Realm, string _PlayerName)
        {
            UInt64 entityID = UInt64.MaxValue;
            
            string entityLinkStr = VF_RealmPlayersDatabase.Utility.GetRealmPreString(_Realm) + _PlayerName;
            if(m_PlayerIDs.TryGetValue(entityLinkStr, out entityID) == true)
                return entityID;
            
            switch (_Realm)
            {
                case WowRealm.Emerald_Dream:
                    entityID = (1UL << 56) | m_EntityCounter_Emerald_Dream++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Warsong:
                    entityID = (2UL << 56) | m_EntityCounter_Warsong++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Al_Akir:
                    entityID = (3UL << 56) | m_EntityCounter_Al_Akir++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Valkyrie:
                    entityID = (4UL << 56) | m_EntityCounter_Valkyrie++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.VanillaGaming:
                    entityID = (5UL << 56) | m_EntityCounter_VanillaGaming++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Rebirth:
                    entityID = (6UL << 56) | m_EntityCounter_Rebirth++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Archangel:
                    entityID = (7UL << 56) | m_EntityCounter_Archangel++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Nostalrius:
                    entityID = (8UL << 56) | m_EntityCounter_Nostalrius++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Kronos:
                    entityID = (9UL << 56) | m_EntityCounter_Kronos++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.NostalGeek:
                    entityID = (10UL << 56) | m_EntityCounter_NostalGeek++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.Nefarian:
                    entityID = (11UL << 56) | m_EntityCounter_Nefarian++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.NostalriusPVE:
                    entityID = (12UL << 56) | m_EntityCounter_NostalriusPVE++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
                case WowRealm.WarsongTBC:
                    entityID = (13UL << 56) | m_EntityCounter_WarsongTBC++;
                    m_PlayerIDs.Add(entityLinkStr, entityID);
                    break;
            }
            return entityID;
        }
        public void GetEntityInfo(UInt64 _EntityID, out string _PlayerName, out WowRealm _Realm)
        {
            _Realm = GetPlayerRealm(_EntityID);
            _PlayerName = GetPlayerName(_EntityID);
        }
        public WowRealm GetPlayerRealm(UInt64 _EntityID)
        {
            UInt64 realm = _EntityID >> 56;
            switch (realm)
            {
                case 0UL:
                    return WowRealm.Unknown;
                case 1UL:
                    return WowRealm.Emerald_Dream;
                case 2UL:
                    return WowRealm.Warsong;
                case 3UL:
                    return WowRealm.Al_Akir;
                case 4UL:
                    return WowRealm.Valkyrie;
                case 5UL:
                    return WowRealm.VanillaGaming;
                case 6UL:
                    return WowRealm.Rebirth;
                case 7UL:
                    return WowRealm.Archangel;
                case 8UL:
                    return WowRealm.Nostalrius;
                case 9UL:
                    return WowRealm.Kronos;
                case 10UL:
                    return WowRealm.NostalGeek;
                case 11UL:
                    return WowRealm.Nefarian;
                case 12UL:
                    return WowRealm.NostalriusPVE;
                case 13UL:
                    return WowRealm.WarsongTBC;
                default:
                    VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Error GetPlayerRealm failed. Realm(" + realm + ") was not valid!!!");
                    return WowRealm.Unknown;
            }
        }
        public string GetPlayerName(UInt64 _EntityID)
        {
            var key = m_PlayerIDs.First((_Value) => _Value.Value == _EntityID).Key;
            if (key.StartsWith("R"))
                return key.Substring(3);
            else
                return key.Substring(1);
        }
        public void UpdateDatabase(RPPDatabase _Database)
        {
            UpdateDatabase(_Database, DateTime.MinValue);
        }
        public void UpdateDatabase(RPPDatabase _Database, DateTime _EarliestDateTime)
        {
            List<string> failedMountItemNames = new List<string>();
            List<string> failedCompanionItemNames = new List<string>();

            var realmDBs = _Database.GetRealms();
            foreach (var realmDB in realmDBs)
            {
                realmDB.Value.WaitForLoad(VF_RealmPlayersDatabase.RealmDatabase.LoadStatus.PlayersHistoryLoaded);
                foreach (var playerHistory in realmDB.Value.PlayersHistory)
                {
                    try
                    {
                        if (playerHistory.Value.GearHistory.Count < 1 || playerHistory.Value.GearHistory.Last().Uploader.GetTime() < _EarliestDateTime)
                            continue;

                        UInt64 playerID = GetEntityID(realmDB.Value.Realm, playerHistory.Key);
                    
                        foreach (var gearHistory in playerHistory.Value.GearHistory)
                        {
                            if (gearHistory.Uploader.GetTime() < _EarliestDateTime)
                                continue;
                            
                            foreach (var item in gearHistory.Data.Items)
                            {
                                var itemSummary = GetItemSummary(item.Value);
                                var optionalSlot = item.Value.Slot;
                                if(item.Value.Slot      == ItemSlot.Finger_1)  optionalSlot = ItemSlot.Finger_2;
                                else if(item.Value.Slot == ItemSlot.Finger_2)  optionalSlot = ItemSlot.Finger_1;
                                else if(item.Value.Slot == ItemSlot.Trinket_1) optionalSlot = ItemSlot.Trinket_2;
                                else if(item.Value.Slot == ItemSlot.Trinket_2) optionalSlot = ItemSlot.Trinket_1;
                                else if(item.Value.Slot == ItemSlot.Main_Hand) optionalSlot = ItemSlot.Off_Hand;
                                else if(item.Value.Slot == ItemSlot.Off_Hand)  optionalSlot = ItemSlot.Main_Hand;

                                if(item.Value.Slot != optionalSlot)
                                {
                                    ItemInfo optionalItemInfo;
                                    if(gearHistory.Data.Items.TryGetValue(optionalSlot, out optionalItemInfo) == true)
                                    {
                                        //Possible duplicate, check it out
                                        if(optionalItemInfo.ItemID == item.Value.ItemID 
                                        && optionalItemInfo.SuffixID == item.Value.SuffixID)
                                        {
                                            //Add duplicate
                                            itemSummary.AddOwner(playerID, gearHistory.Uploader.GetTime(), true);
                                        }
                                        else
                                        {
                                            //No duplicates
                                            itemSummary.AddOwner(playerID, gearHistory.Uploader.GetTime());
                                        }
                                    }
                                    else
                                    {
                                        //No duplicates
                                        itemSummary.AddOwner(playerID, gearHistory.Uploader.GetTime());
                                    }
                                }
                                else
                                {
                                    //No duplicates
                                    itemSummary.AddOwner(playerID, gearHistory.Uploader.GetTime());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VF_RealmPlayersDatabase.Logger.LogException(ex);
                    }
                }
                foreach (var playerExtraData in realmDB.Value.PlayersExtraData)
                {
                    try
                    {
                        UInt64 playerID = GetEntityID(realmDB.Value.Realm, playerExtraData.Key);
                    
                        foreach(var mount in playerExtraData.Value.Mounts)
                        {
                            int mountID = VF.ItemTranslations.FindItemID(mount.Mount);
                            if(mountID > 0)
                            {
                                var itemSummary = GetItemSummary(mountID, 0);
                                itemSummary.AddOwner(playerID, mount.GetEarliestUpload().GetTime());
                            }
                            else
                            {
                                failedMountItemNames.AddUnique(mount.Mount);
                            }
                        }
                        
                        foreach(var companion in playerExtraData.Value.Companions)
                        {
                            int companionID = VF.ItemTranslations.FindItemID(companion.Name);
                            if (companionID > 0)
                            {
                                var itemSummary = GetItemSummary(companionID, 0);
                                itemSummary.AddOwner(playerID, companion.GetEarliestUpload().GetTime());
                            }
                            else
                            {
                                failedCompanionItemNames.AddUnique(companion.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VF_RealmPlayersDatabase.Logger.LogException(ex);
                    }
                }
            }
            VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Failed to add Mounts with names: " + String.Join("\", \"", failedMountItemNames.ToArray()) + "\"");
            VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Failed to add Companions with names: " + String.Join("\", \"", failedCompanionItemNames.ToArray()) + "\"");
        }

        public static ItemSummaryDatabase LoadSummaryDatabase(string _RootDirectory)
        {
            ItemSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\ItemSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            //if (database != null)
            //{
            //    foreach (var guild in database.m_Guilds)
            //    {
            //        guild.Value.InitCache();
            //    }
            //}
            return database;
        }
        public static void UpdateSummaryDatabase(string _RootDirectory, RPPDatabase _Database, bool _UpdateAllHistory = false)
        {
            ItemSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\ItemSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database) == false)
                    database = null;
            }
            //if (database != null)
            //{
            //    foreach (var guild in database.m_Guilds)
            //    {
            //        guild.Value.InitCache();
            //    }
            //}
            if (database == null)
            {
                database = new ItemSummaryDatabase();
                database.UpdateDatabase(_Database);
            }
            else
            {
                if (_UpdateAllHistory == true)
                {
                    database.UpdateDatabase(_Database);
                }
                else
                {
                    database.UpdateDatabase(_Database, DateTime.UtcNow.AddDays(-8));
                }
            }
            VF.Utility.SaveSerialize(databaseFile, database);
        }
    }
}
