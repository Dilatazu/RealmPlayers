#pragma once
#include "CharacterData.h"
#include "GuildData.h"
#include "HonorData.h"
#include "GearData.h"
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
}