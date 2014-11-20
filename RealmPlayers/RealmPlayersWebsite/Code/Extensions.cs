using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealmPlayersServer
{
    public class Extensions
    {
    }
    //public static class DictionaryExtensions
    //{
    //    public static void RemoveAll<TKey, TValue>(
    //        this Dictionary<TKey, TValue> _Dictionary,
    //        Func<KeyValuePair<TKey, TValue>, bool> _Predicate)
    //    {
    //        var toRemoveList = _Dictionary.Where(_Predicate).ToList();
    //        foreach (var toRemove in toRemoveList)
    //        {
    //            _Dictionary.Remove(toRemove.Key);
    //        }
    //    }
    //    public static void RemoveKeys<TKey, TValue>(
    //        this Dictionary<TKey, TValue> _Dictionary,
    //        Func<TKey, bool> _Predicate)
    //    {
    //        var toRemoveList = _Dictionary.Keys.Where(_Predicate).ToList();
    //        foreach (var toRemove in toRemoveList)
    //        {
    //            _Dictionary.Remove(toRemove);
    //        }
    //    }
    //    public static void AddOrSet<TKey, TValue>(
    //        this Dictionary<TKey, TValue> _Dictionary,
    //        TKey _Key, TValue _Value)
    //    {
    //        if (_Dictionary.ContainsKey(_Key) == false)
    //            _Dictionary.Add(_Key, _Value);
    //        else
    //            _Dictionary[_Key] = _Value;
    //    }
    //    public static void AddToList<TKey, TValue>(
    //        this Dictionary<TKey, List<TValue>> _Dictionary,
    //        TKey _Key, TValue _Value)
    //    {
    //        if (_Dictionary.ContainsKey(_Key) == false)
    //            _Dictionary.Add(_Key, new List<TValue>());
    //        _Dictionary[_Key].Add(_Value);
    //    }
    //}
    //public static class DoubleAndFloatExtensions
    //{
    //    public static string ToStringDot(this float _Float)
    //    {
    //        return _Float.ToString(System.Globalization.CultureInfo.InvariantCulture);
    //    }
    //    public static string ToStringDot(this float _Float, string _Format)
    //    {
    //        return _Float.ToString(_Format, System.Globalization.CultureInfo.InvariantCulture);
    //    }

    //    public static string ToStringDot(this double _Double)
    //    {
    //        return _Double.ToString(System.Globalization.CultureInfo.InvariantCulture);
    //    }
    //    public static string ToStringDot(this double _Double, string _Format)
    //    {
    //        return _Double.ToString(_Format, System.Globalization.CultureInfo.InvariantCulture);
    //    }
    //}
}