#pragma once
#include "CharacterDataHistoryItem.h"
#include "GuildDataHistoryItem.h"
#include "HonorDataHistoryItem.h"
#include "GearDataHistoryItem.h"
#include "ArenaDataHistoryItem.h"

namespace RP
{
	class PlayerDataHistory
	{
	public:
		std::vector<CharacterDataHistoryItem> m_CharacterHistory;
		std::vector<GuildDataHistoryItem> m_GuildHistory;
		std::vector<HonorDataHistoryItem> m_HonorHistory;
		std::vector<GearDataHistoryItem> m_GearHistory;
		std::vector<ArenaDataHistoryItem> m_ArenaHistory;
	public:

	private:
		template<typename T_ItemType>
		static void _SerializeItemList(const std::vector<T_ItemType>& _Data, std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, (int)_Data.size());
			for (auto& item : _Data)
			{
				item.Serialize(_ResultOutputStream);
			}
		}
		template<typename T_ItemType>
		static void _DeserializeItemList(std::ifstream& _InputStream, std::vector<T_ItemType>* _OutputData)
		{
			int itemCount = 0;
			ReadBinary<int>(_InputStream, &itemCount);
			_OutputData->reserve(itemCount);
			for (int i = 0; i < itemCount; ++i)
			{
				_OutputData->push_back(T_ItemType());
				T_ItemType::Deserialize(_InputStream, &_OutputData->back());
			}
		}
	public:
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			_SerializeItemList(m_CharacterHistory, _ResultOutputStream);
			_SerializeItemList(m_GuildHistory, _ResultOutputStream);
			_SerializeItemList(m_HonorHistory, _ResultOutputStream);
			_SerializeItemList(m_GearHistory, _ResultOutputStream);
			_SerializeItemList(m_ArenaHistory, _ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, PlayerDataHistory* _OutputData)
		{
			*_OutputData = PlayerDataHistory(); //Important in this case
			_DeserializeItemList(_InputStream, &_OutputData->m_CharacterHistory);
			_DeserializeItemList(_InputStream, &_OutputData->m_GuildHistory);
			_DeserializeItemList(_InputStream, &_OutputData->m_HonorHistory);
			_DeserializeItemList(_InputStream, &_OutputData->m_GearHistory);
			_DeserializeItemList(_InputStream, &_OutputData->m_ArenaHistory);
		}
	};
}