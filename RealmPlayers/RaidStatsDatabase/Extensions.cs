//USING VF LIBRARY EXTENSIONS NOW!!!

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace VF_RaidDamageDatabase
//{
//    class Extensions
//    {
//    }
//    public static class DictionaryExtensions
//    {
//        public static void RemoveAll<TKey, TValue>(
//            this Dictionary<TKey, TValue> _Dictionary,
//            Func<KeyValuePair<TKey, TValue>, bool> _Predicate)
//        {
//            var toRemoveList = _Dictionary.Where(_Predicate).ToList();
//            foreach (var toRemove in toRemoveList)
//            {
//                _Dictionary.Remove(toRemove.Key);
//            }
//        }
//        public static void RemoveKeys<TKey, TValue>(
//            this Dictionary<TKey, TValue> _Dictionary,
//            Func<TKey, bool> _Predicate)
//        {
//            var toRemoveList = _Dictionary.Keys.Where(_Predicate).ToList();
//            foreach (var toRemove in toRemoveList)
//            {
//                _Dictionary.Remove(toRemove);
//            }
//        }
//        public static bool AddIfKeyNotExist<TKey, TValue>(
//            this Dictionary<TKey, TValue> _Dictionary,
//            TKey _Key, TValue _Value)
//        {
//            if (_Dictionary.ContainsKey(_Key) == false)
//            {
//                _Dictionary.Add(_Key, _Value);
//                return true;
//            }
//            return false;
//        }
//        public static void SetKeyValue<TKey, TValue>(
//            this Dictionary<TKey, TValue> _Dictionary,
//            TKey _Key, TValue _Value)
//        {
//            if (_Dictionary.ContainsKey(_Key) == false)
//            {
//                _Dictionary.Add(_Key, _Value);
//                return;
//            }
//            _Dictionary[_Key] = _Value;
//        }
//        //public static void VF_IncremementValue<TKey>(
//        //    this Dictionary<TKey, int> _Dictionary, TKey _Key)
//        //{
//        //    if (_Dictionary.ContainsKey(_Key))
//        //        _Dictionary[_Key] = _Dictionary[_Key] + 1;
//        //    else
//        //        _Dictionary.Add(_Key, 1);
//        //}

//        public static void AddOrSet<TKey, TValue>(
//            this Dictionary<TKey, TValue> _Dictionary,
//            TKey _Key, TValue _Value)
//        {
//            if (_Dictionary.ContainsKey(_Key) == false)
//                _Dictionary.Add(_Key, _Value);
//            else
//                _Dictionary[_Key] = _Value;
//        }
//        public static void AddToList<TKey, TValue>(
//            this Dictionary<TKey, List<TValue>> _Dictionary,
//            TKey _Key, TValue _Value)
//        {
//            if (_Dictionary.ContainsKey(_Key) == false)
//                _Dictionary.Add(_Key, new List<TValue>());
//            _Dictionary[_Key].Add(_Value);
//        }
        
//    }
//    public static class FakeListDictionaryExtensions
//    {
//        public static bool ContainsKey<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> _List, TKey _Key) where TKey : IComparable<TKey>
//        {
//            if (_List.FindIndex((KeyValuePair<TKey, TValue> _Value) => _Value.Key.CompareTo(_Key) == 0) != -1)
//                return true;
//            return false;
//        }
//        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this List<Tuple<TKey, TValue>> _List) where TKey : IComparable<TKey>
//        {
//            return _List.ToDictionary(_Value => _Value.Item1, _Value => _Value.Item2);
//        }
//    }
//    public static class StringExtensions
//    {
//        public static string[] SplitVF(this string _This, string _Delimiter, StringSplitOptions _StringSplitOptions = StringSplitOptions.None)
//        {
//            return _This.Split(new string[] { _Delimiter }, _StringSplitOptions);
//        }
//        public static string[] SplitVF(this string _This, string _Delimiter, int _Count, StringSplitOptions _StringSplitOptions = StringSplitOptions.None)
//        {
//            return _This.Split(new string[] { _Delimiter }, _Count, _StringSplitOptions);
//        }
//        public static bool TryParseDouble(this string _This, out double _Result)
//        {
//            return double.TryParse(_This, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _Result);
//        }
//        public static bool TryParseFloat(this string _This, out float _Result)
//        {
//            return float.TryParse(_This, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _Result);
//        }
//    }
//}
