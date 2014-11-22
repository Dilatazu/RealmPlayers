#pragma once
#include "SerializeHelpers.h"
#include "GuildData.h"
#include "UploadID.h"

namespace RP
{
	class GuildDataHistoryItem
	{
	public:

	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			static_assert(false, "Not implemented");
		}
		static void Deserialize(std::ifstream& _InputStream, GuildDataHistoryItem* _OutputData)
		{
			*_OutputData = GuildDataHistoryItem();

			static_assert(false, "Not implemented");
		}
	};
}