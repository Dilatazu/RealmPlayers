#pragma once
#include "SerializeHelpers.h"

#include "WowEnums.h"

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
			ReadBinary<int>(_InputStream, &result.m_Level);
			return result;
		}
	};
}