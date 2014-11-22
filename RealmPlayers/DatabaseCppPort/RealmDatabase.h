#pragma once
#include "PlayerData.h"
#include "PlayerDataHistory.h"

namespace RP
{
	class RealmDatabaseHistory
	{
	public:
		std::map<std::string, PlayerDataHistory> m_PlayersHistory;
	public:

		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, m_PlayersHistory.size());
			for (auto& playerHistory : m_PlayersHistory)
			{
				WriteBinary<std::string>(*_ResultOutputStream, playerHistory.first);
				playerHistory.second.Serialize(_ResultOutputStream);
			}
		}
		static void Deserialize(std::ifstream& _InputStream, RealmDatabaseHistory* _OutputData)
		{
			*_OutputData = RealmDatabaseHistory();
			int playersCount = 0;
			ReadBinary<int>(_InputStream, &playersCount);
			for (int i = 0; i < playersCount; ++i)
			{
				std::pair<std::string, PlayerDataHistory> dataPair;
				ReadBinary<std::string>(_InputStream, &dataPair.first);
				PlayerDataHistory::Deserialize(_InputStream, &dataPair.second);
				_OutputData->m_PlayersHistory.insert(dataPair);
			}
		}
	};

	class RealmDatabase
	{
	public:
		WowRealm m_WowRealm;

		std::map<std::string, PlayerData> m_Players;

		RealmDatabaseHistory m_History;
	public:

		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, (int)m_WowRealm);
			WriteBinary<int>(*_ResultOutputStream, m_Players.size());
			for (auto& player : m_Players)
			{
				WriteBinary<std::string>(*_ResultOutputStream, player.first);
				player.second.Serialize(_ResultOutputStream);
			}
			m_History.Serialize(_ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, RealmDatabase* _OutputData)
		{
			*_OutputData = RealmDatabase();
			ReadBinary_As<int>(_InputStream, &_OutputData->m_WowRealm);
			int playersCount = 0;
			ReadBinary<int>(_InputStream, &playersCount);
			for (int i = 0; i < playersCount; ++i)
			{
				std::pair<std::string, PlayerData> dataPair;
				ReadBinary<std::string>(_InputStream, &dataPair.first);
				PlayerData::Deserialize(_InputStream, &dataPair.second);
				_OutputData->m_Players.insert(dataPair);
			}
			RealmDatabaseHistory::Deserialize(_InputStream, &_OutputData->m_History);
		}
	};
}