#pragma once
#include <map>
#include "TinyArray.h"
#include "SerializeHelpers.h"

#include "WowEnums.h"

namespace RP
{
	class GearItem
	{
	public:
		ItemSlot m_Slot = ItemSlot::Main_Hand;
		int m_ItemID = 0;
		int m_EnchantID = 0;
		int m_SuffixID = 0;
		int m_UniqueID = 0;
		VF::TinyArray<int> m_GemIDs;

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, (int)m_Slot);
			WriteBinary<int>(*_ResultOutputStream, (int)m_ItemID);
			WriteBinary<int>(*_ResultOutputStream, (int)m_EnchantID);
			WriteBinary<int>(*_ResultOutputStream, (int)m_SuffixID);
			WriteBinary<int>(*_ResultOutputStream, (int)m_UniqueID);
			m_GemIDs.Serialize(_ResultOutputStream);
		}
		static GearItem Deserialize(std::ifstream& _InputStream)
		{
			GearItem result;
			ReadBinary_As<int>(_InputStream, &result.m_Slot);
			ReadBinary<int>(_InputStream, &result.m_ItemID);
			ReadBinary<int>(_InputStream, &result.m_EnchantID);
			ReadBinary<int>(_InputStream, &result.m_SuffixID);
			ReadBinary<int>(_InputStream, &result.m_UniqueID);
			result.m_GemIDs = VF::TinyArray<int>::Deserialize(_InputStream);
			return result;
		}
	};
	class GearData
	{
	public:
		std::map<ItemSlot, GearItem> m_Items;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, (int)m_Items.size());
			for (auto& item : m_Items)
			{
				WriteBinary<int>(*_ResultOutputStream, (int)item.first);
				item.second.Serialize(_ResultOutputStream);
			}
		}
		static GearData Deserialize(std::ifstream& _InputStream)
		{
			GearData result;
			int itemCount = 0;
			ReadBinary<int>(_InputStream, &itemCount);
			for (int i = 0; i < itemCount; ++i)
			{
				std::pair<ItemSlot, GearItem> dataPair;
				ReadBinary_As<int>(_InputStream, &dataPair.first);
				dataPair.second = GearItem::Deserialize(_InputStream);
				result.m_Items.insert(dataPair);
			}
			return result;
		}
	};
}