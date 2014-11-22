#pragma once
#include "SerializeHelpers.h"
#include "DateTime.h"

namespace RP
{
	class UploadID
	{
	public:
		int m_ContributorID;
		DateTime m_Time;

	public:
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			WriteBinary<int>(*_ResultOutputStream, m_ContributorID);
			m_Time.Serialize(_ResultOutputStream);
		}
		static UploadID Deserialize(std::ifstream& _InputStream)
		{
			UploadID result;
			ReadBinary<int>(_InputStream, &result.m_ContributorID);
			result.m_Time = DateTime::Deserialize(_InputStream);
			return result;
		}
		static void Deserialize(std::ifstream& _InputStream, UploadID* _OutputData)
		{
			ReadBinary<int>(_InputStream, &_OutputData->m_ContributorID);
			DateTime::Deserialize(_InputStream, &_OutputData->m_Time);
		}
	};
}