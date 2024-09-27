using System;

public static class StringExtension
{
    public static bool SpecialContains(this string value, string target,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
    {
        return value.IndexOf(target, stringComparison) >= 0;
    }
}
