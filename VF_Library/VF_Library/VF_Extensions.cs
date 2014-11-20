using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class VF_DoubleAndFloatExtensions
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
public static class VF_DateTimeExtensions
{
    public static string ToDateStr(this DateTime _DateTime)
    {
        return _DateTime.ToString("yyyy-MM-dd");
    }
    public static string ToTimeStr(this DateTime _DateTime)
    {
        return _DateTime.ToString("HH:mm:ss");
    }
    public static string ToDateTimeStr(this DateTime _DateTime)
    {
        return _DateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
public static class VF_ListExtensions
{
    public static string MergeToStringVF(this IEnumerable<string> _List, string _Between = null)
    {
        StringBuilder str = new StringBuilder("", _List.Count() * 5);
        foreach (var item in _List)
        {
            str.Append(item);
            if (_Between != null)
                str.Append(_Between);
        }
        if (_Between != null) //Remove last one
            str.Remove(str.Length - _Between.Length, _Between.Length);
        return str.ToString();
    }
}