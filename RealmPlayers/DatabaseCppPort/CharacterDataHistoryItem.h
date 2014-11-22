#pragma once
#include "SerializeHelpers.h"
#include "CharacterData.h"
#include "UploadID.h"

namespace RP
{
	class CharacterDataHistoryItem
	{
	public:
		CharacterData m_Data;
		UploadID m_Uploader;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			m_Data.Serialize(_ResultOutputStream);
			m_Uploader.Serialize(_ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, CharacterDataHistoryItem* _OutputData)
		{
			*_OutputData = CharacterDataHistoryItem();
			CharacterData::Deserialize(_InputStream, &_OutputData->m_Data);
			UploadID::Deserialize(_InputStream, &_OutputData->m_Uploader);
		}
	};
}