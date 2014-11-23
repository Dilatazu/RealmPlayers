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
			//std::cout << "RealmDatabaseHistory::Deserialize\n";
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
		void PrintSomeContent()
		{
			std::cout << "\nPrintSomeContent()\n------------------------------";
			std::cout << "\nm_Players.size=" << m_Players.size();
			std::cout << "\nm_PlayersHistory.size=" << m_History.m_PlayersHistory.size();

			int i = 0;
			for (auto& player : m_Players)
			{
				if (i == 0 || i % 3523 == 1713 || i == m_Players.size() - 1)
				{
					std::cout << "\nm_Players[" << i << "]=\"" << player.first 
						<< "\",{" << (int)player.second.m_Character.m_Race << "," << (int)player.second.m_Character.m_Class 
						<< "," << (int)player.second.m_Character.m_Sex << "}";
				}
				++i;
			}

			i = 0;
			int k = 0;
			for (auto& player : m_History.m_PlayersHistory)
			{
				if (i == 0 || i % 3523 == 1831 || i + k == m_History.m_PlayersHistory.size() - 1)
				{
					if (player.second.m_GuildHistory.size() > 0
						&& player.second.m_GuildHistory.front().m_Data.m_GuildName != player.second.m_GuildHistory.back().m_Data.m_GuildName)
					{
						std::cout << "\nm_PlayersGuildHistory[" << i + k << "]=\"" << player.first << "\","
							<< player.second.m_GuildHistory.front().m_Data.m_GuildName << "=>"
							<< player.second.m_GuildHistory.back().m_Data.m_GuildName;
					}
					else if (player.second.m_CharacterHistory.size() > 0
						&& player.second.m_CharacterHistory.front().m_Data.m_Level != player.second.m_CharacterHistory.back().m_Data.m_Level)
					{
						std::cout << "\nm_PlayersCharacterHistory[" << i + k << "]=\"" << player.first << "\","
							<< player.second.m_CharacterHistory.front().m_Data.m_Level << "=>"
							<< player.second.m_CharacterHistory.back().m_Data.m_Level;
					}
					else if (player.second.m_HonorHistory.size() > 0)
					{
						std::cout << "\nm_PlayersHonorHistory[" << i + k << "]=\"" << player.first << "\","
							<< player.second.m_HonorHistory.front().m_Data.m_CurrentRankProgress << "=>"
							<< player.second.m_HonorHistory.back().m_Data.m_CurrentRankProgress;
					}
					else
					{
						--i;
						++k;
					}
				}
				++i;
			}
			std::cout << "\n------------------------------\n";
		}
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, (int)m_WowRealm);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Players.size());
			for (auto& player : m_Players)
			{
				WriteBinary<std::string>(*_ResultOutputStream, player.first);
				player.second.Serialize(_ResultOutputStream);
			}
			m_History.Serialize(_ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, RealmDatabase* _OutputData)
		{
			std::cout << "RealmDatabase::Deserialize\n";
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