﻿namespace SqlAnalysis.Helpers
{
    public static class StringExtensions
    {
        public static int IndexOfNth(this string str, string value, int nth = 0)
        {
            if (nth < 0)
            {
                throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");
            }

            var offset = str.IndexOf(value, StringComparison.Ordinal);
            for (var i = 0; i < nth; i++)
            {
                if (offset == -1)
                {
                    return -1;
                }

                offset = str.IndexOf(value, offset + 1, StringComparison.Ordinal);
            }

            return offset;
        }
    }
}