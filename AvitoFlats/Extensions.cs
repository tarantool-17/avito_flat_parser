using System;

namespace AvitoFlats
{
    public static class Extension
    {
        public static string Clean(this string str)
        {
            return str?
                .Replace("\r\n", string.Empty)
                .Replace("\n", String.Empty)
                .Replace("&nbsp;", " ")
                .Replace("?", " ")
                .Trim();
        }
    }
}