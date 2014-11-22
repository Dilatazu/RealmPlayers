#pragma once

#include <vector>
#include <map>
#include <iostream>
#include "DateTime.h"
#include "SerializeHelpers.h"
//#include <Windows.h>

namespace RP
{
	class ItemSummary
	{
	public:
		int m_ItemID;
		int m_SuffixID;
		std::vector<std::pair<unsigned __int64, DateTime>> m_ItemOwners;
	public:

		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			WriteBinary<int>(*_ResultOutputStream, m_ItemID);
			WriteBinary<int>(*_ResultOutputStream, m_SuffixID);
			WriteBinary<int>(*_ResultOutputStream, m_ItemOwners.size());
			for (auto& itemOwners : m_ItemOwners)
			{
				WriteBinary<unsigned __int64>(*_ResultOutputStream, itemOwners.first);
				itemOwners.second.Serialize(_ResultOutputStream);
			}
		}
		static ItemSummary Deserialize(std::ifstream& _InputStream)
		{
			ItemSummary result;
			ReadBinary<int>(_InputStream, &result.m_ItemID);
			ReadBinary<int>(_InputStream, &result.m_SuffixID);
			int itemOwnersCount = 0;
			ReadBinary<int>(_InputStream, &itemOwnersCount);
			
			result.m_ItemOwners.reserve(itemOwnersCount);
			for (int i = 0; i < itemOwnersCount; ++i)
			{
				std::pair<unsigned __int64, DateTime> pairData;
				ReadBinary<unsigned __int64>(_InputStream, &pairData.first);
				pairData.second = DateTime::Deserialize(_InputStream);
				result.m_ItemOwners.push_back(pairData);
			}
			return result;
		}
	};
	class ItemSummaryDatabase
	{
	public:
		std::map<unsigned __int64, ItemSummary> m_Items;
		std::map<std::string, unsigned __int64> m_PlayerIDs;

		unsigned __int64 m_EntityCounter_Emerald_Dream = 0;
		unsigned __int64 m_EntityCounter_Warsong = 0;
		unsigned __int64 m_EntityCounter_Al_Akir = 0;
		unsigned __int64 m_EntityCounter_Valkyrie = 0;
		unsigned __int64 m_EntityCounter_VanillaGaming = 0;
		unsigned __int64 m_EntityCounter_Rebirth = 0;
		unsigned __int64 m_EntityCounter_Archangel = 0;
	public:
#pragma optimize("", off)
		void PrintSomeContent()
		{
			std::cout << "\nPrintSomeContent()\n------------------------------";
			std::cout << "\nm_Items.size=" << m_Items.size();
			std::cout << "\nm_PlayerIDs.size=" << m_PlayerIDs.size();
			std::cout << "\nm_EntityCounter_Emerald_Dream=" << m_EntityCounter_Emerald_Dream;
			std::cout << "\nm_EntityCounter_Warsong=" << m_EntityCounter_Warsong;
			std::cout << "\nm_EntityCounter_Al_Akir=" << m_EntityCounter_Al_Akir;
			std::cout << "\nm_EntityCounter_Valkyrie=" << m_EntityCounter_Valkyrie;
			std::cout << "\nm_EntityCounter_VanillaGaming=" << m_EntityCounter_VanillaGaming;
			std::cout << "\nm_EntityCounter_Rebirth=" << m_EntityCounter_Rebirth;
			std::cout << "\nm_EntityCounter_Archangel=" << m_EntityCounter_Archangel;

			int i = 0;
			for (auto& item : m_Items)
			{
				if (i == 0 || i % 3523 == 1713 || i == m_Items.size() - 1)
				{
					std::cout << "\nm_Items[" << i << "]=" << item.first << ",{" << item.second.m_ItemID << "," << item.second.m_SuffixID << ",m_ItemOwners[].size=" << item.second.m_ItemOwners.size() << "}";
				}
				++i;
			}

			i = 0;
			for (auto& playerID : m_PlayerIDs)
			{
				if (i == 0 || i % 5123 == 2847 || i == m_PlayerIDs.size() - 1)
				{
					std::cout << "\nm_PlayerIDs[" << i << "]=\"" << playerID.first.c_str() << "\"," << playerID.second;
				}
				++i;
			}
			std::cout << "\n------------------------------\n";
		}
#pragma optimize("", on)
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			WriteBinary<int>(*_ResultOutputStream, m_Items.size());
			for (auto& item : m_Items)
			{
				WriteBinary<unsigned __int64>(*_ResultOutputStream, item.first);
				item.second.Serialize(_ResultOutputStream);
			}
			WriteBinary<int>(*_ResultOutputStream, m_PlayerIDs.size());
			for (auto& playerID : m_PlayerIDs)
			{
				WriteBinary<std::string>(*_ResultOutputStream, playerID.first);
				WriteBinary<unsigned __int64>(*_ResultOutputStream, playerID.second);
			}

			WriteBinary<unsigned __int64>(*_ResultOutputStream, m_EntityCounter_Emerald_Dream);
			WriteBinary<unsigned __int64>(*_ResultOutputStream, m_EntityCounter_Warsong);
			WriteBinary<unsigned __int64>(*_ResultOutputStream, m_EntityCounter_Al_Akir);
			WriteBinary<unsigned __int64>(*_ResultOutputStream, m_EntityCounter_Valkyrie);
			WriteBinary<unsigned __int64>(*_ResultOutputStream, m_EntityCounter_VanillaGaming);
			WriteBinary<unsigned __int64>(*_ResultOutputStream, m_EntityCounter_Rebirth);
			WriteBinary<unsigned __int64>(*_ResultOutputStream, m_EntityCounter_Archangel);
		}
		static ItemSummaryDatabase Deserialize(std::ifstream& _InputStream)
		{
			ItemSummaryDatabase result;
			int itemsCount = 0;
			ReadBinary<int>(_InputStream, &itemsCount);
			for (int i = 0; i < itemsCount; ++i)
			{
				std::pair<unsigned __int64, ItemSummary> pairData;
				ReadBinary<unsigned __int64>(_InputStream, &pairData.first);
				pairData.second = ItemSummary::Deserialize(_InputStream);
				result.m_Items.insert(pairData);
			}
			int playerIDsCount = 0;
			ReadBinary<int>(_InputStream, &playerIDsCount);
			for (int i = 0; i < playerIDsCount; ++i)
			{
				std::pair<std::string, unsigned __int64> pairData;
				ReadBinary<std::string>(_InputStream, &pairData.first);
				ReadBinary<unsigned __int64>(_InputStream, &pairData.second);
				result.m_PlayerIDs.insert(pairData);
			}

			ReadBinary<unsigned __int64>(_InputStream, &result.m_EntityCounter_Emerald_Dream);
			ReadBinary<unsigned __int64>(_InputStream, &result.m_EntityCounter_Warsong);
			ReadBinary<unsigned __int64>(_InputStream, &result.m_EntityCounter_Al_Akir);
			ReadBinary<unsigned __int64>(_InputStream, &result.m_EntityCounter_Valkyrie);
			ReadBinary<unsigned __int64>(_InputStream, &result.m_EntityCounter_VanillaGaming);
			ReadBinary<unsigned __int64>(_InputStream, &result.m_EntityCounter_Rebirth);
			ReadBinary<unsigned __int64>(_InputStream, &result.m_EntityCounter_Archangel);
			return result;
		}
	};
}