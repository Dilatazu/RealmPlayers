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
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			WriteBinary<int>(*_ResultOutputStream, (int)m_Race);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Class);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Sex);
			WriteBinary<int>(*_ResultOutputStream, (int)m_Level);
		}
		static void Deserialize(std::ifstream& _InputStream, CharacterData* _OutputData)
		{
			//std::cout << "CharacterData::Deserialize\n";
			ReadBinary_As<int>(_InputStream, &_OutputData->m_Race);
			ReadBinary_As<int>(_InputStream, &_OutputData->m_Class);
			ReadBinary_As<int>(_InputStream, &_OutputData->m_Sex);
			ReadBinary<int>(_InputStream, &_OutputData->m_Level);
		}
	};
}