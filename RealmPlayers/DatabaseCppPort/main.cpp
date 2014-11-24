#include <fstream>
//#include <Windows.h>

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
	__declspec(dllexport) void __stdcall Begin_Serialize(const char* _Filename)
	{
		if (g_DLLData == nullptr)
			g_DLLData = new DLLData();
		if (g_DLLData->m_SerializeOutput == nullptr)
			g_DLLData->m_SerializeOutput = new std::ofstream(_Filename, std::ios::binary);
	}
	__declspec(dllexport) void __stdcall Serialize_Int32(__int32 _Value)
	{
		WriteBinary<int>(*g_DLLData->m_SerializeOutput, _Value);
	}
	__declspec(dllexport) void __stdcall Serialize_Float(float _Value)
	{
		WriteBinary<float>(*g_DLLData->m_SerializeOutput, _Value);
	}
	__declspec(dllexport) void __stdcall Serialize_UInt64(unsigned __int64 _Value)
	{
		WriteBinary<unsigned __int64>(*g_DLLData->m_SerializeOutput, _Value);
	}
	__declspec(dllexport) void __stdcall Serialize_String(const char* _Value)
	{
		WriteBinary<std::string>(*g_DLLData->m_SerializeOutput, _Value);
	}
	__declspec(dllexport) void __stdcall End_Serialize()
	{
		delete g_DLLData->m_SerializeOutput;
		g_DLLData->m_SerializeOutput = nullptr;
	}

	__declspec(dllexport) void __stdcall Validate_ItemSummaryDatabase(const char* _Filename)
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
	__declspec(dllexport) void __stdcall Validate_RealmDatabase(const char* _Filename)
	{
		try
		{
			std::ifstream realmDBFile(_Filename, std::ios::binary);
			RP::RealmDatabase realmDB;
			RP::RealmDatabase::Deserialize(realmDBFile, &realmDB);
			realmDB.PrintSomeContent();
		}
		catch (std::exception ex)
		{
			std::cout << ex.what();
		}
	}
}