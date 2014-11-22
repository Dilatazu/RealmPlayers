#pragma once

#include <malloc.h>
#include "SerializeHelpers.h"

namespace VF
{
	template<typename T_Data>
	class TinyArray
	{
	private:
		T_Data* m_Data = nullptr;

		void* _GetBasePointer() const
		{
			if (m_Data == nullptr)
				return nullptr;
			return (((int*)m_Data) - 1);
		}
	public:
		TinyArray()
		{}
		TinyArray(const TinyArray& _Copy)
		{
			int size = _Copy.GetSize();
			SetSize(size);
			for (int i = 0; i < size; ++i)
			{
				m_Data[i] = _Copy.m_Data[i];
			}
		}
		TinyArray(TinyArray&& _Move)
		{
			m_Data = _Move.m_Data;
			_Move.m_Data = nullptr;
		}
		~TinyArray()
		{
			if (m_Data != nullptr)
				free(_GetBasePointer());
			m_Data = nullptr;
		}
		void SetSize(int _Size)
		{
			TinyArray oldArray = *this;

			void* dataPointer = malloc(sizeof(T_Data) * _Size + 4);
			((int*)dataPointer)[0] = _Size;
			m_Data = (T_Data*)&((int*)dataPointer)[1];

			if (oldArray.GetSize() != 0)
			{
				for (int i = 0; i < oldArray.GetSize() && i < _Size; ++i)
				{
					m_Data[i] = oldArray.m_Data[i];
				}
				free(oldArray._GetBasePointer());
			}
		}
		T_Data& operator[](int _Index) { return m_Data[_Index]; }
		const T_Data& operator[](int _Index) const { return m_Data[_Index]; }

		T_Data& At(int _Index) { return m_Data[_Index]; }
		const T_Data& At(int _Index) const { return m_Data[_Index]; }

		int GetSize() const
		{
			if (m_Data == nullptr)
				return 0;
			return *(((int*)m_Data) - 1);
		}
	public:
		void Serialize(std::ofstream* _ResultOutputStream) const
		{
			int size = GetSize();
			WriteBinary<int>(*_ResultOutputStream, size);
			if (size > 0)
			{
				for (int i = 0; i < size; ++i)
				{
					WriteBinary<T_Data>(*_ResultOutputStream, m_Data[i]);
				}
			}
		}
		static TinyArray<T_Data> Deserialize(std::ifstream& _InputStream)
		{
			TinyArray<T_Data> result;
			int size = 0;
			ReadBinary<int>(_InputStream, &size);
			if (size > 0)
			{
				result.SetSize(size);
				for (int i = 0; i < size; ++i)
				{
					ReadBinary<T_Data>(_InputStream, &result.m_Data[i]);
				}
			}
			return result;
		}
	};

}