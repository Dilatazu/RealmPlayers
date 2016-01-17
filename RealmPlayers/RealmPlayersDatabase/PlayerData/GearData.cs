using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

using WowVersionEnum = VF_RealmPlayersDatabase.WowVersionEnum;

namespace VF_RealmPlayersDatabase.PlayerData
{
    [ProtoContract]
    public class ItemInfo
    {
        [ProtoMember(1)]
        public ItemSlot Slot = ItemSlot.Main_Hand;
        [ProtoMember(2)]
        public int ItemID = 0;
        [ProtoMember(3)]
        public int EnchantID = 0;
        [ProtoMember(4)]
        public int SuffixID = 0;
        [ProtoMember(5)]
        public int UniqueID = 0;
        [ProtoMember(6)]
        public int[] GemIDs = null;

        public string GetAsString()
        {
            return "{" + Slot.ToString() + ", " + ItemID + ", " + EnchantID + ", " + SuffixID + ", " + UniqueID + ", " + (GemIDs == null ? "null" : ("(" + GemIDs[0].ToString() + ", " + GemIDs[1].ToString() + ", " + GemIDs[2].ToString() + ", " + GemIDs[3].ToString() + ")")) + "}";
        }

        public int GetGemIDCount()
        {
            if (GemIDs == null)
                return 0;

            int count = 0;
            for (int i = 0; i < GemIDs.Length; ++i)
            {
                if (GemIDs[i] != 0)
                {
                    count = i + 1;
                }
            }
            return count;
        }
        public ItemInfo()
        { }
        public ItemInfo(string _ItemLink, WowVersionEnum _WowVersion)
        {
            if (_WowVersion == WowVersionEnum.Vanilla)
            {
                string[] itemLinkData = _ItemLink.Split(new char[] { ':' });
                Slot = (ItemSlot)int.Parse(itemLinkData[0]);
                ItemID = int.Parse(itemLinkData[1]);
                EnchantID = int.Parse(itemLinkData[2]);
                SuffixID = int.Parse(itemLinkData[3]);
                UniqueID = int.Parse(itemLinkData[4]);
            }
            else if (_WowVersion == WowVersionEnum.TBC)
            {
                string[] itemLinkData = _ItemLink.Split(new char[] { ':' });
                Slot = (ItemSlot)int.Parse(itemLinkData[0]);
                ItemID = int.Parse(itemLinkData[1]);
                EnchantID = int.Parse(itemLinkData[2]);
                GemIDs = new int[4];
                bool anyGems = false;
                for(int i = 0; i < 4; ++i)
                {
                    GemIDs[i] = int.Parse(itemLinkData[3 + i]);
                    if (GemIDs[i] != 0)
                        anyGems = true;
                }
                if (anyGems == false)
                    GemIDs = null;
                SuffixID = int.Parse(itemLinkData[7]);
                UniqueID = int.Parse(itemLinkData[8]);
            }
        }
        public bool IsSame(ItemInfo _ItemInfo)
        {
            //if (Slot == ItemSlot.Finger_1 || Slot == ItemSlot.Finger_2)
            //{
            //    if(_ItemInfo.Slot == ItemSlot.Finger_1 || _ItemInfo.Slot == ItemSlot.Finger_2)
            //}
            //else if (Slot == ItemSlot.Trinket_1 || Slot == ItemSlot.Trinket_2)
            //{

            //}
            //else if (Slot != _ItemInfo.Slot) return false;

            if (ItemID != _ItemInfo.ItemID) return false;
            if (EnchantID != _ItemInfo.EnchantID) return false;
            if (SuffixID != _ItemInfo.SuffixID) return false;
            if (UniqueID != _ItemInfo.UniqueID) return false;
            if(GemIDs == null && _ItemInfo.GemIDs != null) return false;
            if (GemIDs != null && _ItemInfo.GemIDs == null) return false;
            if (GemIDs != null && _ItemInfo.GemIDs != null)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (GemIDs[i] != _ItemInfo.GemIDs[i])
                        return false;
                }
            }
            return true;
        }
    }
    [ProtoContract]
    public class GearData
    {
        [ProtoMember(1)]
        public Dictionary<ItemSlot, ItemInfo> Items = new Dictionary<ItemSlot, ItemInfo>();
        private List<ItemInfo> ExtraItems = null;//NOT SAVED THROUGH PROTOBUF

        public string GetAsString()
        {
            string itemsData = "{";
            foreach(var item in Items)
            {
                itemsData += item.Key.ToString() + "=" + item.Value.GetAsString() + ", ";
            }
            return itemsData + "}";
        }
        public string GetDiffString(GearData _Gear)
        {
            string gearItemsDebugInfo = "";
            foreach (ItemSlot slot in Enum.GetValues(typeof(ItemSlot)))
            {
                ItemInfo myItem = null;
                ItemInfo otherItem = null;
                if (Items.TryGetValue(slot, out myItem) == false) myItem = null;
                if (_Gear.Items.TryGetValue(slot, out otherItem) == false) otherItem = null;

                if (myItem == null && otherItem != null)
                {
                    gearItemsDebugInfo += "My" + otherItem.GetAsString() + "!=" +
                        "Other{null}, ";
                }
                else if (myItem != null && otherItem == null)
                {
                    gearItemsDebugInfo += "My{null}!=" +
                        "Other" + myItem.GetAsString() + ", ";
                }
                else if (myItem != null && otherItem != null)
                {
                    if (otherItem.IsSame(myItem) == false)
                    {
                        gearItemsDebugInfo += "My" + otherItem.GetAsString() + "!=" +
                            "Other" + myItem.GetAsString() + ", ";
                    }
                }
                else
                {
                    if (myItem != null || otherItem != null)
                    {
                        Logger.ConsoleWriteLine("ERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR\n, this is unexpected and should never happen!\nERROR\nERROR\nERROR\nERROR\nERROR\nERROR");
                    }
                }
            }
            return "{" + gearItemsDebugInfo + "}";
        }

        public ItemInfo GetItem(ItemSlot _ItemSlot)
        {
            if (Items.ContainsKey(_ItemSlot) == false)
                return null;
            return Items[_ItemSlot];
        }
        private void SetItem(ItemInfo _Item)
        {
            if (Items.ContainsKey(_Item.Slot) == false)
                Items.Add(_Item.Slot, _Item);
            else
                Items[_Item.Slot] = _Item;
        }

        //Can only be used accurately during PlayerHistory ADDING, this is only using temporary data generated ONCE and never saved!
        public List<GearData> _GenerateExtraItemSets() 
        {
            if (ExtraItems == null || ExtraItems.Count == 0)
                return null;

            List<GearData> newGearDataSets = new List<GearData>();
            GearData lastGearData = this;
            GearData newGearData = new GearData();
            for (int i = 0; i < ExtraItems.Count + 1; ++i)
            {
                ItemInfo extraItem = null;
                if(i < ExtraItems.Count)
                    extraItem = ExtraItems[i];
                if (extraItem == null || newGearData.Items.ContainsKey(extraItem.Slot) == true)
                {
                    foreach (var lastItemData in lastGearData.Items)
                    {
                        if (newGearData.Items.ContainsKey(lastItemData.Key) == false)
                            newGearData.Items.Add(lastItemData.Key, lastItemData.Value);
                    }
                    newGearDataSets.Add(newGearData);
                    lastGearData = newGearData;
                    if (extraItem == null)//Last ExtraItem, and we allready added it. so just exit the for loop here
                        break;
                    newGearData = new GearData();
                }
                newGearData.Items.Add(extraItem.Slot, extraItem);
            }
            return newGearDataSets;
        }

        public GearData()
        { }
        public GearData(System.Xml.XmlNode _PlayerNode, WowVersionEnum _WowVersion)
        {
            InitData(XMLUtility.GetChildValue(_PlayerNode, "ItemsData", ""), _WowVersion);
        }
        public GearData(string _GearDataString, WowVersionEnum _WowVersion)
        {
            InitData(_GearDataString, _WowVersion);
        }
        private void InitData(string _GearDataString, WowVersionEnum _WowVersion)
        {
            try
            {
                if (_GearDataString == "") return;
                string[] itemData = _GearDataString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (itemData.Length >= 4)
                    Items.Clear();
               
                ExtraItems = new List<ItemInfo>();
                foreach (string itemLink in itemData)
                {
                    var newItem = new ItemInfo(itemLink, _WowVersion);
                    if (Items.ContainsKey(newItem.Slot) == false)
                        Items.Add(newItem.Slot, newItem);
                    else
                        ExtraItems.Add(newItem);
                }
            }
            catch (Exception)
            { }
        }
        public float GetPercentageSameItems(GearData _GearData)
        {
            int sameCount = 0;
            foreach (var item in _GearData.Items)
            {
                if (Items[item.Key].IsSame(item.Value) == false)
                {
                    if (item.Key == ItemSlot.Finger_1 || item.Key == ItemSlot.Finger_2)
                    {
                        if (Items[ItemSlot.Finger_1].IsSame(item.Value)
                        || Items[ItemSlot.Finger_2].IsSame(item.Value))
                        {
                            sameCount++;
                        }
                    }
                    else if (item.Key == ItemSlot.Trinket_1 || item.Key == ItemSlot.Trinket_2)
                    {
                        if (Items[ItemSlot.Trinket_1].IsSame(item.Value)
                        || Items[ItemSlot.Trinket_2].IsSame(item.Value))
                        {
                            sameCount++;
                        }
                    }
                }
                else
                    sameCount++;
            }
            return ((float)sameCount / (float)Items.Count);
        }
        public bool IsSame(GearData _GearData)
        {
            try
            {
                //if (Items[ItemSlot.Finger_1].IsSame(_GearData.Items[ItemSlot.Finger_1]) == false)
                //{
                //    if(Items[ItemSlot.Finger_2].IsSame(_GearData.Items[ItemSlot.Finger_1]) == true)
                //}
                //if (Items[ItemSlot.Trinket_1].IsSame(_GearData.Items[ItemSlot.Trinket_1]) == false)
                //{

                //}
                //if (item.Key == ItemSlot.Finger_1 || item.Key == ItemSlot.Finger_2)
                //{
                //    if (Items[ItemSlot.Finger_1].IsSame(_GearData.Items[ItemSlot.Finger_2]) == false)
                //        return false;
                //}
                //else if (item.Key == ItemSlot.Trinket_1 || item.Key == ItemSlot.Trinket_2)
                //{
                //    if (Items[ItemSlot.Trinket_1].IsSame(_GearData.Items[ItemSlot.Trinket_2]) == false)
                //        return false;
                //}
                foreach (var item in _GearData.Items)
                {
                    if (Items[item.Key].IsSame(item.Value) == false)
                    {
                        if (item.Key == ItemSlot.Finger_1 || item.Key == ItemSlot.Finger_2)
                        {
                            if (Items[ItemSlot.Finger_1].IsSame(item.Value)
                            || Items[ItemSlot.Finger_2].IsSame(item.Value))
                            {}
                            else
                                return false;
                        }
                        else if (item.Key == ItemSlot.Trinket_1 || item.Key == ItemSlot.Trinket_2)
                        {
                            if(Items[ItemSlot.Trinket_1].IsSame(item.Value)
                            || Items[ItemSlot.Trinket_2].IsSame(item.Value))
                            {}
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
