#pragma once
#include <string>
#include "SerializeHelpers.h"

#include "WowEnums.h"

namespace RP
{
	class GuildData
	{
	public:
		std::string m_GuildName = "None";
		std::string m_GuildRank = "None";
		int m_GuildRankNr = 0;
	public:
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			WriteBinary<std::string>(*_ResultOutputStream, m_GuildName);
			WriteBinary<std::string>(*_ResultOutputStream, m_GuildRank);
			WriteBinary<int>(*_ResultOutputStream, m_GuildRankNr);
		}
		static void Deserialize(std::ifstream& _InputStream, GuildData* _OutputData)
		{
			//std::cout << "GuildData::Deserialize\n";
			ReadBinary<std::string>(_InputStream, &_OutputData->m_GuildName);
			ReadBinary<std::string>(_InputStream, &_OutputData->m_GuildRank);
			ReadBinary<int>(_InputStream, &_OutputData->m_GuildRankNr);
		}
	};
}