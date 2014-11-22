#pragma once
#include "SerializeHelpers.h"
#include "GuildData.h"
#include "UploadID.h"

namespace RP
{
	class GuildDataHistoryItem
	{
	public:
		GuildData m_Data;
		UploadID m_Uploader;
	public:
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			m_Data.Serialize(_ResultOutputStream);
			m_Uploader.Serialize(_ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, GuildDataHistoryItem* _OutputData)
		{
			GuildData::Deserialize(_InputStream, &_OutputData->m_Data);
			UploadID::Deserialize(_InputStream, &_OutputData->m_Uploader);
		}
	};
}