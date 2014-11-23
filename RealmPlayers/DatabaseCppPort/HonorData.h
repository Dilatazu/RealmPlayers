#pragma once
#include "SerializeHelpers.h"

namespace RP
{
	class HonorData
	{
	public:
		int m_CurrentRank = 0;
		float m_CurrentRankProgress = 0.0f;
		int m_TodayHK = 0;
		int m_TodayDK = 0;
		int m_YesterdayHK = 0;
		int m_YesterdayHonor = 0;
		int m_ThisWeekHK = 0;
		int m_ThisWeekHonor = 0;
		int m_LastWeekHK = 0;
		int m_LastWeekHonor = 0;
		int m_LastWeekStanding = INT_MAX;
		int m_LifetimeHK = 0;
		int m_LifetimeDK = 0;
		int m_LifetimeHighestRank = 0;
	public:
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
#			define _HONORDATA_MEMBER_INTS_COUNT ((&((HonorData*)0)->m_LifetimeHighestRank - &((HonorData*)0)->m_CurrentRank) + 1)
			//Since everything is just int and floats we just treat it as a POD
			static_assert(sizeof(HonorData) == 14 * 4, "HonorData changed. MAKE SURE EVERYTHING IS 100% CORRECT");
			static_assert(_HONORDATA_MEMBER_INTS_COUNT == 14, "HonorData changed. MAKE SURE EVERYTHING IS 100% CORRECT");

			WriteBinaryArray<int>(*_ResultOutputStream, (int*)this, _HONORDATA_MEMBER_INTS_COUNT);
		}
		static void Deserialize(std::ifstream& _InputStream, HonorData* _OutputData)
		{
			//std::cout << "GuildData::HonorData\n";
			ReadBinaryArray<int>(_InputStream, (int*)_OutputData, _HONORDATA_MEMBER_INTS_COUNT);
		}
	};
}