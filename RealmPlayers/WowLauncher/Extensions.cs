using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace VF_WoWLauncher
{
    class Extensions
    {
    }
    public static class DictionaryExtensions
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
        public static bool AddOrSet<TKey, TValue>(
            this Dictionary<TKey, TValue> _Dictionary,
            TKey _Key, TValue _Value)
        {
            if (_Dictionary.ContainsKey(_Key) == false)
            {
                _Dictionary.Add(_Key, _Value);
                return true;
            }
            _Dictionary[_Key] = _Value;
            return false;
        }
        public static void AddToList<TKey, TValue>(
            this Dictionary<TKey, List<TValue>> _Dictionary,
            TKey _Key, TValue _Value)
        {
            if (_Dictionary.ContainsKey(_Key) == false)
                _Dictionary.Add(_Key, new List<TValue>());
            _Dictionary[_Key].Add(_Value);
        }
    }
    public static class DoubleAndFloatExtensions
    {
        public static string ToStringDot(this float _Float)
        {
            return _Float.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringDot(this float _Float, string _Format)
        {
            return _Float.ToString(_Format, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string ToStringDot(this double _Double)
        {
            return _Double.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringDot(this double _Double, string _Format)
        {
            return _Double.ToString(_Format, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
    public static class StringExtensions
    {
        public static string[] SplitVF(this string _This, string _Delimiter, StringSplitOptions _StringSplitOptions = StringSplitOptions.None)
        {
            return _This.Split(new string[] { _Delimiter }, _StringSplitOptions);
        }
        public static string[] SplitVF(this string _This, string _Delimiter, int _Count, StringSplitOptions _StringSplitOptions = StringSplitOptions.None)
        {
            return _This.Split(new string[] { _Delimiter }, _Count, _StringSplitOptions);
        }
        public static int LevenshteinDistance(this string _This, string _CompareString)
        {
            return LevenshteinAlgorithm.FasterLD(_This, _CompareString);
        }
        public static bool TryParseDouble(this string _This, out double _Result)
        {
            return double.TryParse(_This, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _Result);
        }
        public static bool TryParseFloat(this string _This, out float _Result)
        {
            return float.TryParse(_This, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _Result);
        }
    }
    public static class ZipExtensions
    {
        public static void AddDirectoryFilesRecursive(this ICSharpCode.SharpZipLib.Zip.ZipFile _ZipFile, string _Root, string _Directory)
        {
            string relative = _Directory.Substring(_Root.Length);
            if (relative.Length > 0)
            {
                _ZipFile.AddDirectory(relative);
            }

            foreach (string file in System.IO.Directory.GetFiles(_Directory))
            {
                relative = file.Substring(_Root.Length);
                _ZipFile.Add(file, relative);
            }

            foreach (string subFolder in System.IO.Directory.GetDirectories(_Directory))
            {
                _ZipFile.AddDirectoryFilesRecursive(_Root, subFolder);
            }
        }
    }
    static class ControlExtensions
    {
        public static FieldInfo GetEventField2(this Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                /* Find events defined as field */
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                    break;

                /* Find events defined as property { add; remove; } */
                field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
        }
        public static FieldInfo GetEventField(this Type type, string eventName)
        {
            FieldInfo f1 = type.GetField("Event" + eventName, BindingFlags.Static | BindingFlags.NonPublic);
            return f1;
        }

        //public static void RemoveEvents<T>(this T target, string Event)
        //{
        //    FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
        //    object obj = f1.GetValue(target);
        //    PropertyInfo pi = target.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
        //    EventHandlerList list = (EventHandlerList)pi.GetValue(target, null);
        //    list.RemoveHandler(obj, list[obj]);
        //}
        //public static int CheckEvents_T<T>(this T target, string Event)
        //{
        //    FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
        //    object obj = f1.GetValue(target);
        //    PropertyInfo pi = target.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
        //    EventHandlerList list = (EventHandlerList)pi.GetValue(target, null);
        //    //list.RemoveHandler(obj, list[obj]);
        //    if (list[obj] == null)
        //        return 0;
        //    return list[obj].GetInvocationList().Length;
        //}
        public static int GetEventDelegatesCount(this Control _Target, string _Event)
        {
            FieldInfo f1 = typeof(Control).GetField("Event" + _Event, BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(_Target);
            PropertyInfo pi = _Target.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(_Target, null);
            if (list[obj] == null)
                return 0;
            return list[obj].GetInvocationList().Length;
        }
        public static Delegate[] GetEventDelegates(this Control _Target, string _Event)
        {
            FieldInfo f1 = typeof(Control).GetField("Event" + _Event, BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(_Target);
            PropertyInfo pi = _Target.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(_Target, null);
            if (list[obj] == null)
                return new Delegate[0] { };
            return list[obj].GetInvocationList();
        }
        public static void AddEventDelegates(this Control _Target, string _Event, Delegate[] _Delegates)
        {
            FieldInfo f1 = typeof(Control).GetField("Event" + _Event, BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(_Target);
            PropertyInfo pi = _Target.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(_Target, null);

            foreach (var currDelegate in _Delegates)
            {
                list.AddHandler(obj, currDelegate);
            }
        }
        public static void RemoveEventDelegates(this Control _Target, string _Event, Delegate[] _Delegates)
        {
            FieldInfo f1 = typeof(Control).GetField("Event" + _Event, BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(_Target);
            PropertyInfo pi = _Target.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(_Target, null);

            foreach (var currDelegate in _Delegates)
            {
                list.RemoveHandler(obj, currDelegate);
            }
        }
        public static void BlockEventDelegates(this Control _Target, String _Event)
        {
            var eventDelegatesTagData = _Target.GetTagData("PreviousEvent" + _Event + "Delegates");
            if (eventDelegatesTagData == null)
            {
                var eventDelegates = _Target.GetEventDelegates(_Event);
                _Target.SetTagData("PreviousEvent" + _Event + "Delegates", eventDelegates);
                _Target.RemoveEventDelegates(_Event, eventDelegates);
            }
            else
            {
                if (_Target.GetEventDelegates(_Event).Length > 0)
                    throw new Exception("If Event is modified while Blocked everything can get messed up!");
            }
        }
        public static void UnBlockEventDelegates(this Control _Target, String _Event)
        {
            var eventDelegatesTagData = _Target.GetTagData("PreviousEvent" + _Event + "Delegates");
            if (eventDelegatesTagData != null)
            {
                if (_Target.GetEventDelegates(_Event).Length > 0)
                    throw new Exception("If Event is modified while Blocked everything can get messed up!");
                _Target.AddEventDelegates(_Event, (Delegate[])eventDelegatesTagData);
                _Target.SetTagData("PreviousEvent" + _Event + "Delegates", null);
            }
        }
        public static void SetTagData(this Control _Target, string _DataName, object _Data)
        {
            if (_Target.Tag == null)
                _Target.Tag = new Dictionary<string, object>();

            var tagDictionary = (Dictionary<string, object>)_Target.Tag;
            if (tagDictionary.ContainsKey(_DataName) == true)
                tagDictionary[_DataName] = _Data;
            else
                tagDictionary.Add(_DataName, _Data);
        }
        public static object GetTagData(this Control _Target, string _DataName)
        {
            if (_Target.Tag == null)
                _Target.Tag = new Dictionary<string, object>();

            var tagDictionary = (Dictionary<string, object>)_Target.Tag;
            if (tagDictionary.ContainsKey(_DataName) == true)
                return tagDictionary[_DataName];
            return null;
        }
    }
}
