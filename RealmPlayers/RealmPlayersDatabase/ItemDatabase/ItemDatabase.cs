using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase
{
    public partial class ItemDatabase
    {
        public Dictionary<uint, ItemInfo> m_Items = new Dictionary<uint, ItemInfo>();

        private uint GetDictionaryItemID(int _ItemID, WowRealm _Realm)
        {
            return GetDictionaryItemID((uint)_ItemID, _Realm);
        }
        private uint GetDictionaryItemID(uint _ItemID, WowRealm _Realm)
        {
            if(_ItemID > 0xFFFFFF)
                throw new Exception("(GetDictionaryItemID) ItemID: " + _ItemID.ToString() + " is too large");

            switch (_Realm)
            {
                case WowRealm.Unknown:
                    throw new Exception("(GetDictionaryItemID) Could not get dictionary ItemID for Unknown Realm");
                case WowRealm.Emerald_Dream:
                    return (1U << 24) + _ItemID;
                case WowRealm.Al_Akir:
                    return (1U << 24) + _ItemID;
                case WowRealm.Warsong:
                case WowRealm.Warsong2:
                case WowRealm.Nemesis:
                    return (1U << 24) + _ItemID;
                case WowRealm.All:
                    return uint.MaxValue;
                case WowRealm.Archangel:
                case WowRealm.WarsongTBC:
                    return (0x10U << 24) + _ItemID;
                case WowRealm.Valkyrie:
                case WowRealm.Elysium:
                case WowRealm.Elysium2:
                case WowRealm.Zeth_Kur:
                    return (2U << 24) + _ItemID;
                case WowRealm.Rebirth:
                    return (3U << 24) + _ItemID;
                case WowRealm.VanillaGaming:
                    return (4U << 24) + _ItemID;
                case WowRealm.Nostalrius:
                case WowRealm.NostalriusPVE:
                case WowRealm.Nostralia:
                    return (5U << 24) + _ItemID;
                case WowRealm.Kronos:
                case WowRealm.KronosII:
                    return (6U << 24) + _ItemID;
                case WowRealm.NostalGeek:
                    return (7U << 24) + _ItemID;
                case WowRealm.Nefarian:
                    return (8U << 24) + _ItemID;
                case WowRealm.Vengeance_Wildhammer:
                case WowRealm.Vengeance_Stonetalon:
                    return (0x11U << 24) + _ItemID;
                case WowRealm.ExcaliburTBC:
                    return (0x12U << 24) + _ItemID;
                case WowRealm.L4G_Hellfire:
                case WowRealm.Hellfire2:
                case WowRealm.HellGround: //Should have own ID
                    return (0x13U << 24) + _ItemID;
                default:
                    break;
            }
            throw new Exception("(GetDictionaryItemID) Could not get dictionary ItemID for Realm: " + _Realm.ToString());
        }
        public ItemInfo GetItemInfo(int _ItemID, WowRealm _Realm)
        {
            uint dictionaryID = GetDictionaryItemID(_ItemID, _Realm);
            ItemInfo itemInfo = null;
            if (dictionaryID == uint.MaxValue || m_Items.TryGetValue(dictionaryID, out itemInfo) == false)
            {
                if (m_Items.TryGetValue(GetDictionaryItemID(_ItemID, WowRealm.Emerald_Dream), out itemInfo) == true)
                    return itemInfo;
                return null;
            }
            return itemInfo;
        }

        public void InitializeItemInfo(int _ItemID, WowRealm _Realm, ItemInfoDownloader _ItemInfoDownloader)
        {
            uint dictionaryID = GetDictionaryItemID(_ItemID, _Realm);
            if (m_Items.ContainsKey(dictionaryID) == true)
                throw new Exception("(InitializeItemInfo) Could not Initialize ItemInfo for dictionaryID(" + dictionaryID.ToString() + "), it already exists");

            ItemInfo itemInfo = null;
            if(_Realm == WowRealm.Emerald_Dream)
            {
                itemInfo = ItemInfo.GenerateVanilla(_ItemID, _ItemInfoDownloader);
            }
            else if(_Realm == WowRealm.Archangel || _Realm == WowRealm.WarsongTBC)
            {
                itemInfo = ItemInfo.GenerateTBC(_ItemID, _ItemInfoDownloader);
            }
            if(itemInfo != null)
                m_Items.Add(dictionaryID, itemInfo);
            else
                throw new Exception("(InitializeItemInfo) Could not Initialize ItemInfo for dictionaryID(" + dictionaryID.ToString() + "), ItemInfo generation failed");
        }
    }
}
