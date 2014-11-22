#pragma once
#include "SerializeHelpers.h"
#include "HonorData.h"
#include "UploadID.h"

namespace RP
{
	class HonorDataHistoryItem
	{
	public:

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			static_assert(false, "Not implemented");
		}
		static void Deserialize(std::ifstream& _InputStream, HonorDataHistoryItem* _OutputData)
		{
			*_OutputData = HonorDataHistoryItem();

			static_assert(false, "Not implemented");
		}
	};
}