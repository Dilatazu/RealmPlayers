#pragma once
#include "SerializeHelpers.h"
#include "ArenaData.h"
#include "UploadID.h"

namespace RP
{
	class ArenaDataHistoryItem
	{
	public:
		
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			static_assert(false, "Not implemented");
		}
		static void Deserialize(std::ifstream& _InputStream, ArenaDataHistoryItem* _OutputData)
		{
			*_OutputData = ArenaDataHistoryItem();

			static_assert(false, "Not implemented");
		}
	};
}