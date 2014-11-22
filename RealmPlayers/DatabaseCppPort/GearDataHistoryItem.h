#pragma once
#include "SerializeHelpers.h"
#include "GearData.h"
#include "UploadID.h"

namespace RP
{
	class GearDataHistoryItem
	{
	public:
		GearData m_Data;
		UploadID m_Uploader;
	public:
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			m_Data.Serialize(_ResultOutputStream);
			m_Uploader.Serialize(_ResultOutputStream);
		}
		static void Deserialize(std::ifstream& _InputStream, GearDataHistoryItem* _OutputData)
		{
			GearData::Deserialize(_InputStream, &_OutputData->m_Data);
			UploadID::Deserialize(_InputStream, &_OutputData->m_Uploader);
		}
	};
}