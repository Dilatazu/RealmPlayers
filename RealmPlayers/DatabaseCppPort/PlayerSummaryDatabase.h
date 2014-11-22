#pragma once

#include <map>
#include <fstream>
#include <type_traits>


template<typename T_Data>
void WriteBinary(std::ofstream& _Stream, const T_Data& _Data)
{
	static_assert(std::is_pod<T_Data>::value, "Dont try to write complex data!!!");
	_Stream.write((char*)&_Data, sizeof(T_Data));
}
template<typename T_Data>
void WriteBinary(std::ofstream& _Stream, T_Data* _Data)
{
	static_assert(false, "Dont try to write pointers!!!");
}
template<>
void WriteBinary(std::ofstream& _Stream, const std::string& _Data)
{
	WriteBinary<int>(_Stream, _Data.size());
	_Stream.write(_Data.c_str(), _Data.size());
}
template<typename T_Data>
void ReadBinary(std::ifstream& _Stream, T_Data* _ResultData)
{
	static_assert(std::is_pod<T_Data>::value, "Dont try to read complex data!!!");
	_Stream.read((char*)_ResultData, sizeof(T_Data));
}
template<typename T_Data>
void ReadBinary(std::ifstream& _Stream, T_Data** _ResultData)
{
	static_assert(false, "Dont try to read pointers!!!");
}
template<>
void ReadBinary(std::ifstream& _Stream, std::string* _ResultData)
{
	int charCount = 0;
	ReadBinary<int>(_Stream, &charCount);
	_ResultData->resize(charCount);
	_Stream.read(&(*_ResultData)[0], charCount);
}

namespace RPDB
{
	class PVPSummary
	{
	private:
		std::map<float, unsigned __int64> m_HighestRank;
		int m_ActivePVPWeeks = 0;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, m_ActivePVPWeeks);
			WriteBinary<int>(*_ResultOutputStream, m_HighestRank.size());
			for (auto& ranks : m_HighestRank)
			{
				WriteBinary<float>(*_ResultOutputStream, ranks.first);
				WriteBinary<unsigned __int64>(*_ResultOutputStream, ranks.second);
			}
		}
		static PVPSummary Deserialize(std::ifstream& _InputStream)
		{
			PVPSummary result;
			ReadBinary<int>(_InputStream, &result.m_ActivePVPWeeks);
			int highestRankCount = 0;
			ReadBinary<int>(_InputStream, &highestRankCount);
			for (int i = 0; i < highestRankCount; ++i)
			{
				std::pair<float, unsigned __int64> pairData;
				ReadBinary<float>(_InputStream, &pairData.first);
				ReadBinary<unsigned __int64>(_InputStream, &pairData.second);
				result.m_HighestRank.insert(pairData);
			}
			return result;
		}
	};
	class PlayerSummaryDatabase
	{
	private:
		std::map<std::string, PVPSummary> m_PVPSummaries;
	public:
		void Serialize(std::ofstream* _ResultOutputStream)
		{
			WriteBinary<int>(*_ResultOutputStream, m_PVPSummaries.size());
			for (auto& pvpSummary : m_PVPSummaries)
			{
				WriteBinary<std::string>(*_ResultOutputStream, pvpSummary.first);
				pvpSummary.second.Serialize(_ResultOutputStream);
			}
		}
		static PlayerSummaryDatabase Deserialize(std::ifstream& _InputStream)
		{
			int pvpSummaryCount = 0;
			ReadBinary<int>(_InputStream, &pvpSummaryCount);
			for (int i = 0; i < pvpSummaryCount; ++i)
			{
				std::pair<std::string, PVPSummary> pairData;
				ReadBinary<std::string>(_InputStream, &pairData.first);
				pairData.second = PVPSummary::Deserialize(_InputStream);
			}
		}
	};
}