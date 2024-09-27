using System;
using System.Linq;

public static class EnumExtensions
{
    public static T Next<T>(this T v) where T : struct
    {
        return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] {default(T)}).SkipWhile(e => !v.Equals(e)).Skip(1)
            .First();
    }

    public static T Previous<T>(this T v) where T : struct
    {
        return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] {default(T)}).Reverse()
            .SkipWhile(e => !v.Equals(e)).Skip(1).First();
    }

    // NOTE : 동일한 Enum 형식으로만 체크 가능
    public static bool IsEquals<T>(this T v, T e) where T : struct
    {
        return v.Equals(e);
    }
}