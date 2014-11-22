#pragma once
#include "SerializeHelpers.h"
#include "GearData.h"
#include "UploadID.h"

namespace RP
{
	class GearDataHistoryItem
	{
	public:

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			static_assert(false, "Not implemented");
		}
		static void Deserialize(std::ifstream& _InputStream, GearDataHistoryItem* _OutputData)
		{
			*_OutputData = GearDataHistoryItem();

			static_assert(false, "Not implemented");
		}
	};
}