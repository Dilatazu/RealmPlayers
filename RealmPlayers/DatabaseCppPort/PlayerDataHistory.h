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
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			static_assert(false, "Not implemented");
		}
		static void Deserialize(std::ifstream& _InputStream, PlayerDataHistory* _OutputData)
		{
			*_OutputData = PlayerDataHistory();

			static_assert(false, "Not implemented");
		}
	};
}