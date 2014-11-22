#pragma once

#include "SerializeHelpers.h"

namespace RP
{
	class DateTime
	{
	public:
		unsigned __int64 Value;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<unsigned __int64>(*_ResultOutputStream, Value);
		}
		static DateTime Deserialize(std::ifstream& _InputStream)
		{
			DateTime result;
			ReadBinary<unsigned __int64>(_InputStream, &result.Value);
			return result;
		}
		static void Deserialize(std::ifstream& _InputStream, DateTime* _OutputData)
		{
			ReadBinary<unsigned __int64>(_InputStream, &_OutputData->Value);
		}
	};
}