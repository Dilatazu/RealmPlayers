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
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<std::string>(*_ResultOutputStream, m_GuildName);
			WriteBinary<std::string>(*_ResultOutputStream, m_GuildRank);
			WriteBinary<int>(*_ResultOutputStream, m_GuildRankNr);
		}
		static GuildData Deserialize(std::ifstream& _InputStream)
		{
			GuildData result;
			ReadBinary<std::string>(_InputStream, &result.m_GuildName);
			ReadBinary<std::string>(_InputStream, &result.m_GuildRank);
			ReadBinary<int>(_InputStream, &result.m_GuildRankNr);
			return result;
		}
	};
}