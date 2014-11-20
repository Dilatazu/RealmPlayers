using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase
{
    public partial class ItemDatabase
    { 
        public class ItemInfo
        {
            int m_ItemID;
            string m_ItemIcon;
            string m_ItemName;
            int m_ItemQuality;
            ItemSlot m_ItemSlot;
            int m_ItemModel;
            int m_ItemModelViewerID;
            int m_ItemModelViewerSlot;
            string m_AjaxTooltip;
            string m_WowHeadJSONData;
            string m_WowHeadJSONEquipData;
            private ItemInfo(int _ItemID, string _WowOneDBData, string _WowHeadXMLData)
            {
                m_ItemID = _ItemID;
                ParseAndInitializeAjaxTooltip(_WowOneDBData);

                ParseAndInitializeWowHeadXMLData(_WowHeadXMLData);
                //m_ItemSet = GenerateItemSetData();

            }

            private void ParseAndInitializeWowHeadXMLData(string _WowHeadXMLData)
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(_WowHeadXMLData);
                var rootElement = doc.DocumentElement;
                var itemElement = rootElement["item"];
                try
                {
                    m_WowHeadJSONData = itemElement["json"].InnerText;
                    try
                    {
                        int displayIDIndex = m_WowHeadJSONData.IndexOf("\"displayid\":") + "\"displayid\":".Length;
                        string displayID = m_WowHeadJSONData.Substring(displayIDIndex, m_WowHeadJSONData.IndexOf(',', displayIDIndex) - displayIDIndex);
                        m_ItemModelViewerID = int.Parse(displayID);
                    }
                    catch (Exception)
                    {
                        m_ItemModelViewerID = 0;
                    }
                    try
                    {
                        int slotbakIndex = m_WowHeadJSONData.IndexOf("\"slotbak\":") + "\"slotbak\":".Length;
                        string slotbak = m_WowHeadJSONData.Substring(slotbakIndex, m_WowHeadJSONData.IndexOf(',', slotbakIndex) - slotbakIndex);
                        m_ItemModelViewerSlot = int.Parse(slotbak);
                    }
                    catch (Exception)
                    {
                        m_ItemModelViewerSlot = 0;
                    }
                }
                catch (Exception)
                {
                    m_WowHeadJSONData = "";
                }
                try
                {
                    m_WowHeadJSONEquipData = itemElement["jsonEquip"].InnerText;
                }
                catch (Exception)
                {
                    m_WowHeadJSONEquipData = "";
                }
            }

            private void ParseAndInitializeAjaxTooltip(string _AjaxTooltip)
            {
                m_AjaxTooltip = _AjaxTooltip;

                string[] itemData = _AjaxTooltip.Split('{', '}');
                string[] itemDataFields = itemData[1].Split(',');

                m_ItemName = "";
                m_ItemQuality = 0;
                m_ItemIcon = "";
                foreach (string itemDataField in itemDataFields)
                {
                    try
                    {
                        if (itemDataField.Contains(':'))
                        {
                            string[] itemDataFieldValues = itemDataField.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                            if (itemDataFieldValues[0] == "name_enus")
                            {
                                int firstSnuf = itemDataFieldValues[1].IndexOf('\'');
                                int secondSnuf = itemDataFieldValues[1].LastIndexOf('\'');
                                m_ItemName = itemDataFieldValues[1].Substring(firstSnuf + 1, secondSnuf - firstSnuf - 1);
                            }
                            else if (itemDataFieldValues[0] == "quality")
                                m_ItemQuality = int.Parse(itemDataFieldValues[1]);
                            else if (itemDataFieldValues[0] == "icon")
                                m_ItemIcon = itemDataFieldValues[1].Replace("\'", "");
                            else if (itemDataFieldValues[0] == "tooltip_enus")
                            {
                                break;
                                //itemInfo.ItemTooltipHtml = itemDataFieldValues[1].Replace("\'", "").Replace("\\", "");
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
                try
                {
                    m_ItemModel = 0;
                    if (itemDataFields.Last().Contains(':'))
                    {
                        string[] itemDataFieldValues = itemDataFields.Last().Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        if (itemDataFieldValues[0] == " model")
                        {
                            m_ItemModel = int.Parse(itemDataFieldValues[1].Replace("'", ""));
                        }
                    }
                }
                catch (Exception)
                { }
            }
            public static ItemInfo GenerateVanilla(int _ItemID, ItemInfoDownloader _ItemInfoDownloader)
            {
                string wowOneDBData = _ItemInfoDownloader.DownloadWoWOneDB_Vanilla(_ItemID);
                string wowHeadXMLData = _ItemInfoDownloader.DownloadWoWHeadXMLData(_ItemID);
                if (wowOneDBData == null || wowHeadXMLData == null)
                    return null;
                return new ItemInfo(_ItemID, wowOneDBData, wowHeadXMLData);
            }
            public static ItemInfo GenerateTBC(int _ItemID, ItemInfoDownloader _ItemInfoDownloader)
            {
                string wowOneDBData = _ItemInfoDownloader.DownloadWoWOneDB_TBC(_ItemID);
                string wowHeadXMLData = _ItemInfoDownloader.DownloadWoWHeadXMLData(_ItemID);
                if (wowOneDBData == null || wowHeadXMLData == null)
                    return null;
                return new ItemInfo(_ItemID, wowOneDBData, wowHeadXMLData);
            }
        }
    }
}
