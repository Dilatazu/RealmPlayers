#pragma once
#include "SerializeHelpers.h"

#include "WowEnums.h"

namespace RP
{
	class ArenaPlayerData
	{
	public:
		std::string m_TeamName = "None";
		int m_TeamRating = 0;
		int m_GamesPlayed = 0;
		int m_GamesWon = 0;
		int m_PlayerPlayed = 0;
		int m_PlayerRating = 0;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<std::string>(*_ResultOutputStream, m_TeamName);
			WriteBinary<int>(*_ResultOutputStream, m_TeamRating);
			WriteBinary<int>(*_ResultOutputStream, m_GamesPlayed);
			WriteBinary<int>(*_ResultOutputStream, m_GamesWon);
			WriteBinary<int>(*_ResultOutputStream, m_PlayerPlayed);
			WriteBinary<int>(*_ResultOutputStream, m_PlayerRating);
		}
		static void Deserialize(std::ifstream& _InputStream, ArenaPlayerData* _OutputData)
		{
			ReadBinary<std::string>(_InputStream, &_OutputData->m_TeamName);
			ReadBinary<int>(_InputStream, &_OutputData->m_TeamRating);
			ReadBinary<int>(_InputStream, &_OutputData->m_GamesPlayed);
			ReadBinary<int>(_InputStream, &_OutputData->m_GamesWon);
			ReadBinary<int>(_InputStream, &_OutputData->m_PlayerPlayed);
			ReadBinary<int>(_InputStream, &_OutputData->m_PlayerRating);
		}
	};
#define _ArenaData_FLAG_TEAM2V2 0x1
#define _ArenaData_FLAG_TEAM3V3 0x2
#define _ArenaData_FLAG_TEAM5V5 0x4

	class ArenaData
	{
	public:
		ArenaPlayerData* m_Team2v2 = nullptr;
		ArenaPlayerData* m_Team3v3 = nullptr;
		ArenaPlayerData* m_Team5v5 = nullptr;
	public:
		ArenaData()
		{}
		ArenaData(const ArenaData& _Copy)
		{
			int teamFlags = 0;
			int teamCount = 0;
			if (_Copy.m_Team2v2 != nullptr)
			{
				teamFlags |= _ArenaData_FLAG_TEAM2V2;
				++teamCount;
			}
			if (_Copy.m_Team3v3 != nullptr)
			{
				teamFlags |= _ArenaData_FLAG_TEAM3V3;
				++teamCount;
			}
			if (_Copy.m_Team5v5 != nullptr)
			{
				teamFlags |= _ArenaData_FLAG_TEAM5V5;
				++teamCount;
			}

			if (teamCount == 0) return;

			if (teamFlags & _ArenaData_FLAG_TEAM2V2)
			{
				m_Team2v2 = new ArenaPlayerData[teamCount];
				int teamIndex = 0;
				if (teamFlags & _ArenaData_FLAG_TEAM3V3) m_Team3v3 = &m_Team2v2[++teamIndex];
				if (teamFlags & _ArenaData_FLAG_TEAM5V5) m_Team5v5 = &m_Team2v2[++teamIndex];
			}
			else if (teamFlags & _ArenaData_FLAG_TEAM3V3)
			{
				m_Team3v3 = new ArenaPlayerData[teamCount];
				if (teamFlags & _ArenaData_FLAG_TEAM5V5) m_Team5v5 = &m_Team3v3[1];
			}
			else if (teamFlags & _ArenaData_FLAG_TEAM5V5)
			{
				m_Team5v5 = new ArenaPlayerData[teamCount];
			}

			if (_Copy.m_Team2v2 != nullptr)	*m_Team2v2 = *_Copy.m_Team2v2;
			if (_Copy.m_Team3v3 != nullptr)	*m_Team3v3 = *_Copy.m_Team3v3;
			if (_Copy.m_Team5v5 != nullptr)	*m_Team5v5 = *_Copy.m_Team5v5;
		}
		ArenaData(ArenaData&& _Move)
		{
			m_Team2v2 = _Move.m_Team2v2;
			m_Team3v3 = _Move.m_Team3v3;
			m_Team5v5 = _Move.m_Team5v5;
			_Move.m_Team2v2 = nullptr;
			_Move.m_Team3v3 = nullptr;
			_Move.m_Team5v5 = nullptr;
		}
		~ArenaData()
		{
			if (m_Team2v2 != nullptr)
				delete[] m_Team2v2;
			else if (m_Team3v3 != nullptr)
				delete[] m_Team3v3;
			else if (m_Team5v5 != nullptr)
				delete[] m_Team5v5;
			m_Team2v2 = nullptr;
			m_Team3v3 = nullptr;
			m_Team5v5 = nullptr;
		}
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			int teamFlags = 0;
			if (m_Team2v2 != nullptr) teamFlags |= _ArenaData_FLAG_TEAM2V2;
			if (m_Team3v3 != nullptr) teamFlags |= _ArenaData_FLAG_TEAM3V3;
			if (m_Team5v5 != nullptr) teamFlags |= _ArenaData_FLAG_TEAM5V5;
				
			WriteBinary<int>(*_ResultOutputStream, teamFlags);

			if (teamFlags & _ArenaData_FLAG_TEAM2V2) m_Team2v2->Serialize(_ResultOutputStream);
			if (teamFlags & _ArenaData_FLAG_TEAM3V3) m_Team2v2->Serialize(_ResultOutputStream);
			if (teamFlags & _ArenaData_FLAG_TEAM5V5) m_Team2v2->Serialize(_ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, ArenaData* _OutputData)
		{
			*_OutputData = ArenaData(); //IMPORTANT IN THIS CASE so that if any old data it gets destructed properly!!!
			int teamFlags = 0;
			ReadBinary<int>(_InputStream, &teamFlags);

			int teamCount = 0;
			if (teamFlags & _ArenaData_FLAG_TEAM2V2) ++teamCount;
			if (teamFlags & _ArenaData_FLAG_TEAM3V3) ++teamCount;
			if (teamFlags & _ArenaData_FLAG_TEAM5V5) ++teamCount;

			if (teamCount == 0)
				return;

			if (teamFlags & _ArenaData_FLAG_TEAM2V2)
			{
				_OutputData->m_Team2v2 = new ArenaPlayerData[teamCount];
				int teamIndex = 0;
				if (teamFlags & _ArenaData_FLAG_TEAM3V3) _OutputData->m_Team3v3 = &_OutputData->m_Team2v2[++teamIndex];
				if (teamFlags & _ArenaData_FLAG_TEAM5V5) _OutputData->m_Team5v5 = &_OutputData->m_Team2v2[++teamIndex];
			}
			else if (teamFlags & _ArenaData_FLAG_TEAM3V3)
			{
				_OutputData->m_Team3v3 = new ArenaPlayerData[teamCount];
				if (teamFlags & _ArenaData_FLAG_TEAM5V5) _OutputData->m_Team5v5 = &_OutputData->m_Team3v3[1];
			}
			else if (teamFlags & _ArenaData_FLAG_TEAM5V5)
			{
				_OutputData->m_Team5v5 = new ArenaPlayerData[teamCount];
			}

			if (teamFlags & _ArenaData_FLAG_TEAM2V2)
				ArenaPlayerData::Deserialize(_InputStream, _OutputData->m_Team2v2);
			if (teamFlags & _ArenaData_FLAG_TEAM3V3)
				ArenaPlayerData::Deserialize(_InputStream, _OutputData->m_Team3v3);
			if (teamFlags & _ArenaData_FLAG_TEAM5V5)
				ArenaPlayerData::Deserialize(_InputStream, _OutputData->m_Team5v5);
		}
	};
}