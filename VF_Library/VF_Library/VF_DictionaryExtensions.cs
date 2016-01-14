using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class VF_DictionaryExtensions
{
    public static void RemoveAll<TKey, TValue>(
        this Dictionary<TKey, TValue> _Dictionary,
        Func<KeyValuePair<TKey, TValue>, bool> _Predicate)
    {
        var toRemoveList = _Dictionary.Where(_Predicate).ToList();
        foreach (var toRemove in toRemoveList)
        {
            _Dictionary.Remove(toRemove.Key);
        }
    }
    public static void RemoveKeys<TKey, TValue>(
        this Dictionary<TKey, TValue> _Dictionary,
        Func<TKey, bool> _Predicate)
    {
        var toRemoveList = _Dictionary.Keys.Where(_Predicate).ToList();
        foreach (var toRemove in toRemoveList)
        {
            _Dictionary.Remove(toRemove);
        }
    }
    public static bool AddIfKeyNotExist<TKey, TValue>(
        this Dictionary<TKey, TValue> _Dictionary,
        TKey _Key, TValue _Value)
    {
        if (_Dictionary.ContainsKey(_Key) == false)
        {
            _Dictionary.Add(_Key, _Value);
            return true;
        }
        return false;
    }
    public static void SetKeyValue<TKey, TValue>(
        this Dictionary<TKey, TValue> _Dictionary,
        TKey _Key, TValue _Value)
    {
        if (_Dictionary.ContainsKey(_Key) == false)
        {
            _Dictionary.Add(_Key, _Value);
            return;
        }
        _Dictionary[_Key] = _Value;
    }
    //public static void VF_IncremementValue<TKey>(
    //    this Dictionary<TKey, int> _Dictionary, TKey _Key)
    //{
    //    if (_Dictionary.ContainsKey(_Key))
    //        _Dictionary[_Key] = _Dictionary[_Key] + 1;
    //    else
    //        _Dictionary.Add(_Key, 1);
    //}

    public static void AddOrSet<TKey, TValue>(
        this Dictionary<TKey, TValue> _Dictionary,
        TKey _Key, TValue _Value)
    {
        if (_Dictionary.ContainsKey(_Key) == false)
            _Dictionary.Add(_Key, _Value);
        else
            _Dictionary[_Key] = _Value;
    }
    public static void AddToList<TKey, TValue>(
        this Dictionary<TKey, List<TValue>> _Dictionary,
        TKey _Key, TValue _Value)
    {
        if (_Dictionary.ContainsKey(_Key) == false)
            _Dictionary.Add(_Key, new List<TValue>());
        _Dictionary[_Key].Add(_Value);
    }
    public static void RemoveFromList<TKey, TValue>(
        this Dictionary<TKey, List<TValue>> _Dictionary,
        TKey _Key, TValue _Value)
    {
        if (_Dictionary.ContainsKey(_Key) == false)
            _Dictionary.Add(_Key, new List<TValue>());
        _Dictionary[_Key].Remove(_Value);
    }
    public static void AddToDistinctList<TKey>(
        this Dictionary<TKey, List<int>> _Dictionary,
        TKey _Key, int _Value)
    {
        if (_Dictionary.ContainsKey(_Key) == false)
            _Dictionary.Add(_Key, new List<int>());
        if (_Dictionary[_Key].Contains(_Value) == false)
            _Dictionary[_Key].Add(_Value);
    }
    public static void AddToDistinctList<TKey>(
        this Dictionary<TKey, List<string>> _Dictionary,
        TKey _Key, string _Value)
    {
        if (_Dictionary.ContainsKey(_Key) == false)
            _Dictionary.Add(_Key, new List<string>());
        if(_Dictionary[_Key].Contains(_Value) == false)
            _Dictionary[_Key].Add(_Value);
    }

    public static void MergeAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> _Dictionary,
        Dictionary<TKey, TValue> _OtherDictionary, Func<TKey, TValue, TValue, bool> _ReplaceValueFunc = null)
    {
        foreach (var item in _OtherDictionary)
        {
            if (_Dictionary.ContainsKey(item.Key) == true)
            {
                if (_ReplaceValueFunc == null || _ReplaceValueFunc(item.Key, _Dictionary[item.Key], item.Value) == true)
                    _Dictionary[item.Key] = item.Value;
            }
            else
            {
                _Dictionary.Add(item.Key, item.Value);
            }
        }
    }

    public static bool AddUnique<TValue>(this List<TValue> _List, TValue _Value)
    {
        if (_List.Contains(_Value) == true)
            return false;
        _List.Add(_Value);
        return true;
    }
    class StuffComparer<T> : IEqualityComparer<T>
    {
        public StuffComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            this.equals = equals;
            this.getHashCode = getHashCode;
        }

        readonly Func<T, T, bool> equals;
        public bool Equals(T x, T y)
        {
            return equals(x, y);
        }

        readonly Func<T, int> getHashCode;
        public int GetHashCode(T obj)
        {
            return getHashCode(obj);
        }
    }
    public static bool AddUnique<TValue>(this List<TValue> _List, TValue _Value, Func<TValue, TValue, bool> _EqualFunc)
    {
        if (_List.FindIndex(_CurrValue => _EqualFunc(_CurrValue, _Value)) != -1)
            return false;
        _List.Add(_Value);
        return true;
    }
    public static int AddRangeUnique<TValue>(this List<TValue> _List, IEnumerable<TValue> _OtherList)
    {
        int addCounter = 0;
        foreach (TValue item in _OtherList)
        {
            if (_List.Contains(item) == false)
            {
                _List.Add(item);
                ++addCounter;
            }
        }
        return addCounter;
    }
}
public static class VF_FakeListDictionaryExtensions
{
    public static bool ContainsKey<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> _List, TKey _Key) where TKey : IComparable<TKey>
    {
        if (_List.FindIndex((KeyValuePair<TKey, TValue> _Value) => _Value.Key.CompareTo(_Key) == 0) != -1)
            return true;
        return false;
    }
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this List<Tuple<TKey, TValue>> _List) where TKey : IComparable<TKey>
    {
        return _List.ToDictionary(_Value => _Value.Item1, _Value => _Value.Item2);
    }
}