#pragma once

#include <map>
#include "DateTime.h"
#include "SerializeHelpers.h"

namespace RP
{
	class PVPSummary
	{
	private:
		std::map<float, DateTime> m_HighestRank;
		int m_ActivePVPWeeks = 0;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, m_ActivePVPWeeks);
			WriteBinary<int>(*_ResultOutputStream, m_HighestRank.size());
			for (auto& ranks : m_HighestRank)
			{
				WriteBinary<float>(*_ResultOutputStream, ranks.first);
				ranks.second.Serialize(_ResultOutputStream);
			}
		}
		static PVPSummary Deserialize(std::ifstream& _InputStream)
		{
			PVPSummary result;
			ReadBinary<int>(_InputStream, &result.m_ActivePVPWeeks);
			int highestRankCount = 0;
			ReadBinary<int>(_InputStream, &highestRankCount);
			for (int i = 0; i < highestRankCount; ++i)
			{
				std::pair<float, DateTime> pairData;
				ReadBinary<float>(_InputStream, &pairData.first);
				pairData.second = DateTime::Deserialize(_InputStream);
				result.m_HighestRank.insert(pairData);
			}
			return result;
		}
	};
	class PlayerSummaryDatabase
	{
	private:
		std::map<std::string, PVPSummary> m_PVPSummaries;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, m_PVPSummaries.size());
			for (auto& pvpSummary : m_PVPSummaries)
			{
				WriteBinary<std::string>(*_ResultOutputStream, pvpSummary.first);
				pvpSummary.second.Serialize(_ResultOutputStream);
			}
		}
		static PlayerSummaryDatabase Deserialize(std::ifstream& _InputStream)
		{
			PlayerSummaryDatabase result;
			int pvpSummaryCount = 0;
			ReadBinary<int>(_InputStream, &pvpSummaryCount);
			for (int i = 0; i < pvpSummaryCount; ++i)
			{
				std::pair<std::string, PVPSummary> pairData;
				ReadBinary<std::string>(_InputStream, &pairData.first);
				pairData.second = PVPSummary::Deserialize(_InputStream);
				result.m_PVPSummaries.insert(pairData);
			}
			return result;
		}
	};
}