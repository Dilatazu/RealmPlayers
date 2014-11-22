#pragma once

#include <vector>
#include <map>
#include <string>
#include <iostream>
#include "DateTime.h"
#include "SerializeHelpers.h"
#include "WowEnums.h"
#include "TinyArray.h"

namespace RP
{
	class CharacterData
	{
	public:
		PlayerRace m_Race = PlayerRace::Unknown;
		PlayerClass m_Class = PlayerClass::Unknown;
		PlayerSex m_Sex = PlayerSex::Male;
		int m_Level = 0;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, (int)m_Race);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Class);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Sex);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Level);
		}
		static CharacterData Deserialize(std::ifstream& _InputStream)
		{
			CharacterData result;
			ReadBinary_As<int>(_InputStream, &result.m_Race);
			ReadBinary_As<int>(_InputStream, &result.m_Class);
			ReadBinary_As<int>(_InputStream, &result.m_Sex);
			ReadBinary_As<int>(_InputStream, &result.m_Level);
			return result;
		}
	};
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
	class HonorData
	{
	public:

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{

			throw "Not implemented!";
		}
		static HonorData Deserialize(std::ifstream& _InputStream)
		{
			HonorData result;

			throw "Not implemented!";
			return result;
		}
	};

	class GearItem
	{
	public:
		ItemSlot m_Slot = ItemSlot::Main_Hand;
		int m_ItemID = 0;
		int m_EnchantID = 0;
		int m_SuffixID = 0;
		int m_UniqueID = 0;
		VF::TinyArray<int> m_GemIDs;

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, (int)m_Slot);
			WriteBinary<int>(*_ResultOutputStream, (int)m_ItemID);
			WriteBinary<int>(*_ResultOutputStream, (int)m_EnchantID);
			WriteBinary<int>(*_ResultOutputStream, (int)m_SuffixID);
			WriteBinary<int>(*_ResultOutputStream, (int)m_UniqueID);
			m_GemIDs.Serialize(_ResultOutputStream);
		}
		static GearItem Deserialize(std::ifstream& _InputStream)
		{
			GearItem result;
			throw "Not implemented!";
			return result;
		}
	};
	class GearData
	{
	public:

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{

			throw "Not implemented!";
		}
		static GearData Deserialize(std::ifstream& _InputStream)
		{
			GearData result;
			throw "Not implemented!";
			return result;
		}
	};
	class UploadID
	{
	public:

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{

			throw "Not implemented!";
		}
		static UploadID Deserialize(std::ifstream& _InputStream)
		{
			UploadID result;
			throw "Not implemented!";
			return result;
		}
	};

	class PlayerData
	{
	public:
		std::string m_Name;
		WowRealm m_Realm;
		CharacterData m_Character;
		GuildData m_Guild;
		HonorData m_Honor;
		GearData m_Gear;
		DateTime m_LastSeen;
		UploadID m_Uploader;
	public:

		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<std::string>(*_ResultOutputStream, m_Name);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Realm);
			m_Character.Serialize(_ResultOutputStream);
			m_Guild.Serialize(_ResultOutputStream);
			m_Honor.Serialize(_ResultOutputStream);
			m_Gear.Serialize(_ResultOutputStream);
			m_LastSeen.Serialize(_ResultOutputStream);
			m_Uploader.Serialize(_ResultOutputStream);
		}
		static PlayerData Deserialize(std::ifstream& _InputStream)
		{
			PlayerData result;
			
			return result;
		}
	};

	class RealmDatabase
	{
	public:
		WowRealm m_WowRealm;

		std::map<std::string, PlayerData> m_Players;
	public:
	};
}