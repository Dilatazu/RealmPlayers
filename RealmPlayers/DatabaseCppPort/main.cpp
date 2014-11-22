#include <fstream>

#include "PlayerSummaryDatabase.h"
#include "ItemSummaryDatabase.h"
#include "RealmDatabase.h"

struct DLLData
{
	std::ofstream* m_SerializeOutput = nullptr;
};
DLLData* g_DLLData = nullptr;
extern "C"
{
	__declspec(dllexport) void Begin_Serialize(const char* _Filename)
	{
		if (g_DLLData == nullptr)
			g_DLLData = new DLLData();
		if (g_DLLData->m_SerializeOutput == nullptr)
			g_DLLData->m_SerializeOutput = new std::ofstream(_Filename, std::ios::binary);
	}
	__declspec(dllexport) void Serialize_Int32(__int32 _Value)
	{
		WriteBinary<int>(*g_DLLData->m_SerializeOutput, _Value);
	}
	__declspec(dllexport) void Serialize_UInt64(unsigned __int64 _Value)
	{
		WriteBinary<unsigned __int64>(*g_DLLData->m_SerializeOutput, _Value);
	}
	__declspec(dllexport) void Serialize_String(const char* _Value)
	{
		WriteBinary<std::string>(*g_DLLData->m_SerializeOutput, _Value);
	}
	__declspec(dllexport) void End_Serialize()
	{
		delete g_DLLData->m_SerializeOutput;
		g_DLLData->m_SerializeOutput = nullptr;
	}

	__declspec(dllexport) void Validate_ItemSummaryDatabase(const char* _Filename)
	{
		try
		{
			std::ifstream summaryDBFile(_Filename, std::ios::binary);
			auto summaryDB = RP::ItemSummaryDatabase::Deserialize(summaryDBFile);
			summaryDB.PrintSomeContent();
		}
		catch (std::exception ex)
		{
			std::cout << ex.what();
		}
	}
}