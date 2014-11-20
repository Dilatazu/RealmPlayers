using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class VF_StringExtensions
{
    public static string[] SplitVF(this string _This, string _Delimiter, StringSplitOptions _StringSplitOptions = StringSplitOptions.None)
    {
        return _This.Split(new string[] { _Delimiter }, _StringSplitOptions);
    }
    public static string[] SplitVF(this string _This, string _Delimiter, int _Count, StringSplitOptions _StringSplitOptions = StringSplitOptions.None)
    {
        return _This.Split(new string[] { _Delimiter }, _Count, _StringSplitOptions);
    }
    public static bool TryParseDouble(this string _This, out double _Result)
    {
        return double.TryParse(_This, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _Result);
    }
    public static bool TryParseFloat(this string _This, out float _Result)
    {
        return float.TryParse(_This, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _Result);
    }
    public static List<string> SplitListVF(this string _This, int _ItemSize)
    {
        List<string> items = new List<string>();

        StringBuilder strBuilder = new StringBuilder("", _ItemSize);
        for (int i = 0; i < _This.Length; ++i)
        {
            strBuilder.Append(_This[i]);
            if (strBuilder.Length == _ItemSize)
            {
                items.Add(strBuilder.ToString());
                strBuilder.Clear();
            }
        }
        if(strBuilder.Length != 0)
        {
            items.Add(strBuilder.ToString());
        }
        return items;
    }
}