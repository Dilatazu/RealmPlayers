#pragma once
#include "CharacterData.h"
#include "GuildData.h"
#include "HonorData.h"
#include "GearData.h"
#include "ArenaData.h"
#include "DateTime.h"
#include "UploadID.h"

namespace RP
{
	class PlayerData
	{
	public:
		std::string m_Name;
		WowRealm m_Realm;
		CharacterData m_Character;
		GuildData m_Guild;
		HonorData m_Honor;
		GearData m_Gear;
		ArenaData m_Arena;
		DateTime m_LastSeen;
		UploadID m_Uploader;
	public:

		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			WriteBinary<std::string>(*_ResultOutputStream, m_Name);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Realm);
			m_Character.Serialize(_ResultOutputStream);
			m_Guild.Serialize(_ResultOutputStream);
			m_Honor.Serialize(_ResultOutputStream);
			m_Gear.Serialize(_ResultOutputStream);
			m_Arena.Serialize(_ResultOutputStream);
			m_LastSeen.Serialize(_ResultOutputStream);
			m_Uploader.Serialize(_ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, PlayerData* _OutputData)
		{
			//std::cout << "PlayerData::Deserialize\n";
			ReadBinary<std::string>(_InputStream, &_OutputData->m_Name);
			ReadBinary_As<int>(_InputStream, &_OutputData->m_Realm);
			CharacterData::Deserialize(_InputStream, &_OutputData->m_Character);
			GuildData::Deserialize(_InputStream, &_OutputData->m_Guild);
			HonorData::Deserialize(_InputStream, &_OutputData->m_Honor);
			GearData::Deserialize(_InputStream, &_OutputData->m_Gear);
			ArenaData::Deserialize(_InputStream, &_OutputData->m_Arena);
			DateTime::Deserialize(_InputStream, &_OutputData->m_LastSeen);
			UploadID::Deserialize(_InputStream, &_OutputData->m_Uploader);
		}
	};
}