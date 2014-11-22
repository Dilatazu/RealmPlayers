#pragma once

#include <fstream>
#include <type_traits>

namespace
{
	template<typename T_Data>
	inline void WriteBinary(std::ofstream& _Stream, const T_Data& _Data)
	{
		static_assert(std::is_pod<T_Data>::value, "Dont try to write complex data!!!");
		_Stream.write((char*)&_Data, sizeof(T_Data));
	}
	template<typename T_Data>
	inline void WriteBinary(std::ofstream& _Stream, T_Data* _Data)
	{
		static_assert(false, "Dont try to write pointers!!!");
	}
	template<>
	inline void WriteBinary(std::ofstream& _Stream, const std::string& _Data)
	{
		WriteBinary<int>(_Stream, _Data.size());
		_Stream.write(_Data.c_str(), _Data.size());
	}

	template<typename T_Data>
	inline void WriteBinaryArray(std::ofstream& _Stream, const T_Data* _Data, int _Count)
	{
		static_assert(std::is_pod<T_Data>::value, "Dont try to write complex data!!!");
		_Stream.write((char*)_Data, sizeof(T_Data) * _Count);
	}
	template<typename T_Data>
	inline void WriteBinaryArray(std::ofstream& _Stream, T_Data** _Data, int _Count)
	{
		static_assert(false, "Dont try to write pointers!!!");
	}

	template<typename T_Data>
	inline void ReadBinary(std::ifstream& _Stream, T_Data* _ResultData)
	{
		static_assert(std::is_pod<T_Data>::value, "Dont try to read complex data!!!");
		_Stream.read((char*)_ResultData, sizeof(T_Data));
	}
	template<typename T_Data>
	inline void ReadBinary(std::ifstream& _Stream, T_Data** _ResultData)
	{
		static_assert(false, "Dont try to read pointers!!!");
	}
	template<>
	inline void ReadBinary(std::ifstream& _Stream, std::string* _ResultData)
	{
		int charCount = 0;
		ReadBinary<int>(_Stream, &charCount);
		_ResultData->resize(charCount);
		_Stream.read(&(*_ResultData)[0], charCount);
	}

	template<typename T_ReadAs, typename T_Data>
	inline void ReadBinary_As(std::ifstream& _Stream, T_Data* _ResultData)
	{
		T_ReadAs readVariable;
		ReadBinary<T_ReadAs>(_Stream, &readVariable);
		*_ResultData = (T_Data)readVariable;
	}

	template<typename T_Data>
	inline void ReadBinaryArray(std::ifstream& _Stream, T_Data* _ArrayOutput, int _Count)
	{
		static_assert(std::is_pod<T_Data>::value, "Dont try to read complex data!!!");
		_Stream.read((char*)_ArrayOutput, sizeof(T_Data) * _Count);
	}
	template<typename T_Data>
	inline void ReadBinaryArray(std::ifstream& _Stream, T_Data** _ArrayOutput)
	{
		static_assert(false, "Dont try to read pointers!!!");
	}
}
