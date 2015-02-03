using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VF_RaidDamageDatabase
{
    class ThreadSafeCache
    {
        private Dictionary<string, object> m_Data = new Dictionary<string,object>();

        private T_Return _Get<T_Return>(Func<T_Return> _CreateFunc, string _DataKey)
        {
            T_Return retValue;

            lock(m_Data)
            {
                if (m_Data.ContainsKey(_DataKey) == false)
                {
                    Monitor.Exit(m_Data);
                    retValue = _CreateFunc();
                    Monitor.Enter(m_Data);
                    if (m_Data.ContainsKey(_DataKey) == false)
                        m_Data.Add(_DataKey, retValue);
                    else
                        retValue = (T_Return)m_Data[_DataKey];
                }
                else
                {
                    retValue = (T_Return)m_Data[_DataKey];
                }
            }
            return retValue;
        }
        public T_Return Get<T_Return>(string _UniqueIdentifier, Func<T_Return> _CreateFunc)
        {
            return _Get(_CreateFunc, _UniqueIdentifier);
        }
        public T_Return Get<T_Param1, T_Return>(string _UniqueIdentifier, Func<T_Param1, T_Return> _CreateFunc, T_Param1 _Param1)
        {
            string dataKey = _UniqueIdentifier + "_" + _Param1;
            return _Get(() => { return _CreateFunc(_Param1); }, dataKey);
        }
        public T_Return Get<T_Param1, T_Param2, T_Return>(string _UniqueIdentifier, Func<T_Param1, T_Param2, T_Return> _CreateFunc, T_Param1 _Param1, T_Param2 _Param2)
        {
            string dataKey = _UniqueIdentifier + "_" + _Param1 + "_" + _Param2;
            return _Get(() => {return _CreateFunc(_Param1, _Param2);}, dataKey);
        }
        public T_Return Get<T_Param1, T_Param2, T_Param3, T_Return>(string _UniqueIdentifier, Func<T_Param1, T_Param2, T_Param3, T_Return> _CreateFunc, T_Param1 _Param1, T_Param2 _Param2, T_Param3 _Param3)
        {
            string dataKey = _UniqueIdentifier + "_" + _Param1 + "_" + _Param2 + "_" + _Param3;
            return _Get(() => { return _CreateFunc(_Param1, _Param2, _Param3); }, dataKey);
        }
        public T_Return Get<T_Param1, T_Param2, T_Param3, T_Param4, T_Return>(string _UniqueIdentifier, Func<T_Param1, T_Param2, T_Param3, T_Param4, T_Return> _CreateFunc, T_Param1 _Param1, T_Param2 _Param2, T_Param3 _Param3, T_Param4 _Param4)
        {
            string dataKey = typeof(T_Return).ToString() + "_" + _Param1 + "_" + _Param2 + "_" + _Param3 + "_" + _Param4;
            return _Get(() => { return _CreateFunc(_Param1, _Param2, _Param3, _Param4); }, dataKey);
        }
    }
}
