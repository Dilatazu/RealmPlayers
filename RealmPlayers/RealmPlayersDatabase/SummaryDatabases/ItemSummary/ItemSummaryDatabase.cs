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
        [ProtoMember(16)]
        public UInt64 m_EntityCounter_KronosII = 0L;
        [ProtoMember(17)]
        public UInt64 m_EntityCounter_Vengeance_Wildhammer = 0L;
        [ProtoMember(18)]
        public UInt64 m_EntityCounter_ExcaliburTBC = 0L;
        [ProtoMember(19)]
        public UInt64 m_EntityCounter_L4G_Hellfire = 0L;
        [ProtoMember(20)]
        public UInt64 m_EntityCounter_Warsong2 = 0L;
        [ProtoMember(21)]
        public UInt64 m_EntityCounter_Vengeance_Stonetalon = 0L;
        [ProtoMember(22)]
        public UInt64 m_EntityCounter_Elysium = 0L;
        [ProtoMember(23)]
        public UInt64 m_EntityCounter_Elysium2 = 0L;
        [ProtoMember(24)]
        public UInt64 m_EntityCounter_Zeth_Kur = 0L;
        [ProtoMember(25)]
        public UInt64 m_EntityCounter_Nemesis = 0L;
        [ProtoMember(26)]
        public UInt64 m_EntityCounter_HellGround = 0L;
        [ProtoMember(27)]
        public UInt64 m_EntityCounter_Nostralia = 0L;
        [ProtoMember(28)]
        public UInt64 m_EntityCounter_Hellfire2 = 0L;

        [ProtoMember(29)]
        public Dictionary<WowRealm, UInt64> m_EntityCounters_Realm = new Dictionary<WowRealm, UInt64>();

        private UInt64 GetRealmBitNR(WowRealm _Realm)
        {
            switch (_Realm)
            {
                case WowRealm.Emerald_Dream: return 1UL;
                case WowRealm.Warsong: return 2UL;
                case WowRealm.Al_Akir: return 3UL;
                case WowRealm.Valkyrie: return 4UL;
                case WowRealm.VanillaGaming: return 5UL;
                case WowRealm.Rebirth: return 6UL;
                case WowRealm.Archangel: return 7UL;
                case WowRealm.Nostalrius: return 8UL;
                case WowRealm.Kronos: return 9UL;
                case WowRealm.NostalGeek: return 10UL;
                case WowRealm.Nefarian: return 11UL;
                case WowRealm.NostalriusPVE: return 12UL;
                case WowRealm.WarsongTBC: return 13UL;
                case WowRealm.KronosII: return 14UL;
                case WowRealm.Vengeance_Wildhammer: return 15UL;
                case WowRealm.ExcaliburTBC: return 16UL;
                case WowRealm.L4G_Hellfire: return 17UL;
                case WowRealm.Warsong2: return 18UL;
                case WowRealm.Vengeance_Stonetalon: return 19UL;
                case WowRealm.Elysium: return 20UL;
                case WowRealm.Elysium2: return 21UL;
                case WowRealm.Zeth_Kur: return 22UL;
                case WowRealm.Nemesis: return 23UL;
                case WowRealm.HellGround: return 24UL;
                case WowRealm.Nostralia: return 25UL;
                case WowRealm.Hellfire2: return 26UL;
                case WowRealm.Outland: return 27UL;
                case WowRealm.Medivh: return 28UL;
                case WowRealm.Firemaw: return 29UL;
                case WowRealm.Felmyst: return 30UL;
            }
            return 0UL;
        }
        private void CalcRealmBits(WowRealm _Realm, out UInt64 _BitMask, out UInt64 _RealmValue)
        {
            _BitMask = 0xFFUL << 56;
            _RealmValue = GetRealmBitNR(_Realm) << 56;
            if (_RealmValue == 0) _RealmValue = _BitMask;
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

            entityID = GetRealmBitNR(_Realm) << 56;
            //switch (_Realm)
            //{
            //    //case WowRealm.Emerald_Dream:    entityID |= m_EntityCounter_Emerald_Dream++; break;
            //    //case WowRealm.Warsong:          entityID |= m_EntityCounter_Warsong++; break;
            //    //case WowRealm.Al_Akir:          entityID |= m_EntityCounter_Al_Akir++; break;
            //    //case WowRealm.Valkyrie:         entityID |= m_EntityCounter_Valkyrie++;  break;
            //    //case WowRealm.VanillaGaming:    entityID |= m_EntityCounter_VanillaGaming++; break;
            //    //case WowRealm.Rebirth:          entityID |= m_EntityCounter_Rebirth++; break;
            //    //case WowRealm.Archangel:        entityID |= m_EntityCounter_Archangel++; break;
            //    //case WowRealm.Nostalrius:       entityID |= m_EntityCounter_Nostalrius++; break;
            //    //case WowRealm.Kronos:           entityID |= m_EntityCounter_Kronos++;  break;
            //    //case WowRealm.NostalGeek:       entityID |= m_EntityCounter_NostalGeek++;  break;
            //    //case WowRealm.Nefarian:         entityID |= m_EntityCounter_Nefarian++;break;
            //    //case WowRealm.NostalriusPVE:    entityID |= m_EntityCounter_NostalriusPVE++; break;
            //    //case WowRealm.WarsongTBC:       entityID |= m_EntityCounter_WarsongTBC++; break;
            //    //case WowRealm.KronosII:         entityID |= m_EntityCounter_KronosII++; break;
            //    //case WowRealm.Vengeance_Wildhammer:     entityID |= m_EntityCounter_Vengeance_Wildhammer++; break;
            //    //case WowRealm.ExcaliburTBC:     entityID |= m_EntityCounter_ExcaliburTBC++; break;
            //    //case WowRealm.L4G_Hellfire:     entityID |= m_EntityCounter_L4G_Hellfire++; break;
            //    //case WowRealm.Warsong2:         entityID |= m_EntityCounter_Warsong2++;  break;
            //    //case WowRealm.Vengeance_Stonetalon:     entityID |= m_EntityCounter_Vengeance_Stonetalon++;  break;
            //    //case WowRealm.Elysium:          entityID |= m_EntityCounter_Elysium++;break;
            //    //case WowRealm.Elysium2:         entityID |= m_EntityCounter_Elysium2++;   break;
            //    //case WowRealm.Zeth_Kur:         entityID |= m_EntityCounter_Zeth_Kur++; break;
            //    //case WowRealm.Nemesis:          entityID |= m_EntityCounter_Nemesis++; break;
            //    //case WowRealm.HellGround:       entityID |= m_EntityCounter_HellGround++; break;
            //    //case WowRealm.Nostralia:        entityID |= m_EntityCounter_Nostralia++; break;
            //    //case WowRealm.Hellfire2:        entityID |= m_EntityCounter_Hellfire2++; break;
            //    default:
                    m_EntityCounters_Realm.AddIfKeyNotExist(_Realm, (UInt64)0);
                    UInt64 currEntityCounterValue = m_EntityCounters_Realm[_Realm];
                    entityID |= currEntityCounterValue;
                    m_EntityCounters_Realm[_Realm] = currEntityCounterValue + 1;
            //        break;
            //}
            m_PlayerIDs.Add(entityLinkStr, entityID);
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
                case 0UL: return WowRealm.Unknown;
                case 1UL: return WowRealm.Emerald_Dream;
                case 2UL: return WowRealm.Warsong;
                case 3UL: return WowRealm.Al_Akir;
                case 4UL: return WowRealm.Valkyrie;
                case 5UL: return WowRealm.VanillaGaming;
                case 6UL: return WowRealm.Rebirth;
                case 7UL: return WowRealm.Archangel;
                case 8UL: return WowRealm.Nostalrius;
                case 9UL: return WowRealm.Kronos;
                case 10UL: return WowRealm.NostalGeek;
                case 11UL: return WowRealm.Nefarian;
                case 12UL: return WowRealm.NostalriusPVE;
                case 13UL: return WowRealm.WarsongTBC;
                case 14UL: return WowRealm.KronosII;
                case 15UL: return WowRealm.Vengeance_Wildhammer;
                case 16UL: return WowRealm.ExcaliburTBC;
                case 17UL: return WowRealm.L4G_Hellfire;
                case 18UL: return WowRealm.Warsong2;
                case 19UL: return WowRealm.Vengeance_Stonetalon;
                case 20UL: return WowRealm.Elysium;
                case 21UL: return WowRealm.Elysium2;
                case 22UL: return WowRealm.Zeth_Kur;
                case 23UL: return WowRealm.Nemesis;
                case 24UL: return WowRealm.HellGround;
                case 25UL: return WowRealm.Nostralia;
                case 26UL: return WowRealm.Hellfire2;
                case 27UL: return WowRealm.Outland;
                case 28UL: return WowRealm.Medivh;
                case 29UL: return WowRealm.Firemaw;
                case 30UL: return WowRealm.Felmyst;
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
            VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Failed to add " + failedMountItemNames.Count + " mounts due to unknown names");// with names: " + String.Join("\", \"", failedMountItemNames.ToArray()) + "\"");
            VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Failed to add " + failedMountItemNames.Count + " companions due to unknown names");// with names: " + String.Join("\", \"", failedCompanionItemNames.ToArray()) + "\"");
        }

        public static ItemSummaryDatabase LoadSummaryDatabase(string _RootDirectory)
        {
            ItemSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\ItemSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database, 10000, true) == false)
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
        public void MigrateToSQL()
        {
            Dictionary<ulong, VF.SQLPlayerID> playerIDConverter = new Dictionary<ulong, VF.SQLPlayerID>();
            using (VF.SQLComm comm = new VF.SQLComm())
            {
                int totalItemsCount = m_Items.Count;
                int itemsProcessedCounter = 0;
                DateTime startTime = DateTime.UtcNow;
                DateTime prevLogTime = DateTime.UtcNow;
                foreach (var itemSummary in m_Items)
                {
                    ++itemsProcessedCounter;
                    foreach (var itemOwner in itemSummary.Value.m_ItemOwners)
                    {
                        VF.SQLPlayerID playerID;
                        if (playerIDConverter.TryGetValue(itemOwner.Item1, out playerID) == false)
                        {
                            string playerName = GetPlayerName(itemOwner.Item1);
                            WowRealm realm = GetPlayerRealm(itemOwner.Item1);

                            if (comm.GetPlayerID(realm, playerName, out playerID) == true)
                            {
                                playerIDConverter.Add(itemOwner.Item1, playerID);
                            }
                            else
                            {
                                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("playerID was not valid for player \"" + playerName + "\"", ConsoleColor.Red);
                            }
                        }
                        if(playerID.IsValid() == true)
                        {
                            if (comm.UpsertItemOwner(itemSummary.Value.m_ItemID, itemSummary.Value.m_SuffixID, playerID, null, itemOwner.Item2) == false)
                            {
                                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("UpsertItemOwner Failed???", ConsoleColor.Red);
                            }
                        }
                    }
                    if((DateTime.UtcNow - prevLogTime).TotalSeconds > 5)
                    {
                        VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Processed " + itemsProcessedCounter + " / " + totalItemsCount + " items");
                        prevLogTime = DateTime.UtcNow;
                    }
                }
                VF_RealmPlayersDatabase.Logger.ConsoleWriteLine("Done with processing " + totalItemsCount + " items, it took " + (DateTime.UtcNow - startTime).ToString());
            }
        }
        public static void UpdateSummaryDatabase(string _RootDirectory, RPPDatabase _Database, bool _UpdateAllHistory = false)
        {
            ItemSummaryDatabase database = null;
            string databaseFile = _RootDirectory + "\\SummaryDatabase\\ItemSummaryDatabase.dat";
            if (System.IO.File.Exists(databaseFile) == true)
            {
                if (VF.Utility.LoadSerialize(databaseFile, out database, 10000, true) == false)
                    database = null;

                //Update the new dictionary realm with all the old entitycounters!
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Emerald_Dream, database.m_EntityCounter_Emerald_Dream);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Warsong, database.m_EntityCounter_Warsong);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Al_Akir, database.m_EntityCounter_Al_Akir);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Valkyrie, database.m_EntityCounter_Valkyrie);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.VanillaGaming, database.m_EntityCounter_VanillaGaming);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Rebirth, database.m_EntityCounter_Rebirth);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Archangel, database.m_EntityCounter_Archangel);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Nostalrius, database.m_EntityCounter_Nostalrius);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Kronos, database.m_EntityCounter_Kronos);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.NostalGeek, database.m_EntityCounter_NostalGeek);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Nefarian, database.m_EntityCounter_Nefarian);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.NostalriusPVE, database.m_EntityCounter_NostalriusPVE);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.WarsongTBC, database.m_EntityCounter_WarsongTBC);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.KronosII, database.m_EntityCounter_KronosII);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Vengeance_Wildhammer, database.m_EntityCounter_Vengeance_Wildhammer);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.ExcaliburTBC, database.m_EntityCounter_ExcaliburTBC);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.L4G_Hellfire, database.m_EntityCounter_L4G_Hellfire);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Warsong2, database.m_EntityCounter_Warsong2);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Vengeance_Stonetalon, database.m_EntityCounter_Vengeance_Stonetalon);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Elysium, database.m_EntityCounter_Elysium);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Elysium2, database.m_EntityCounter_Elysium2);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Zeth_Kur, database.m_EntityCounter_Zeth_Kur);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Nemesis, database.m_EntityCounter_Nemesis);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.HellGround, database.m_EntityCounter_HellGround);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Nostralia, database.m_EntityCounter_Nostralia);
                database.m_EntityCounters_Realm.AddIfKeyNotExist(WowRealm.Hellfire2, database.m_EntityCounter_Hellfire2);
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
