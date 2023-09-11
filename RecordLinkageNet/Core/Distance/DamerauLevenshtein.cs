using RecordLinkageNet.Util;
using System;
using System.Linq;

namespace RecordLinkageNet.Core.Distance
{
    public static class DamerauLevenshtein
    {
        public static double DamerauLevenshteinDistanceNormalizedToRange0To1(this string s1, string s2)
        {
            double value = DamerauLevenshteinDistance(s1.AsMemory(), s2.AsMemory());
            if (value == 0.0f) //short cut for speed up
                return 1.0f; //flip, because 0 changed are 100% equal

            double max = Math.Max(s1.Length, s2.Length);

            if (value == max)
                return 0.0f; //flip, because max changes are 100% unequal

            return (1.0f - NumberNormalizeHelper.NormalizeNumberToRange0to1(value, max, 0));
        }

        public static double DamerauLevenshteinDistance(this string s1, string s2)
        {
            return DamerauLevenshteinDistance(s1.AsMemory(), s2.AsMemory());
        }
        public static double DamerauLevenshteinDistance(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {
            //ground found here: https://gist.github.com/joannaksk/da110f9b05ff38d3f4ea4d149a0eb55e

            //TODO: rewrite to dictio based compare, its faster ??
            //or tree base like jelly

            if (s1.Span.SequenceEqual(s2.Span))
                return 0;

            int s1Len = s1.Length;
            int s2Len = s2.Length;

            if (s1Len == 0)
                return s2Len;

            if (s2Len == 0)
                return s1Len;

            var matrix = new int[s1Len + 1, s2Len + 1];

            for (int i = 1; i <= s1Len; i++)
            {
                matrix[i, 0] = i;
                for (int j = 1; j <= s2Len; j++)
                {
                    int cost = s2.Slice(j - 1, 1).Span.SequenceEqual(s1.Slice(i - 1, 1).Span) ? 0 : 1;

                    if (i == 1)
                        matrix[0, j] = j;

                    var vals = new int[] {
                    matrix[i - 1, j] + 1,
                    matrix[i, j - 1] + 1,
                    matrix[i - 1, j - 1] + cost
                };
                    matrix[i, j] = vals.Min();
                    if (i > 1 &&
                        j > 1 &&
                        s1.Slice(i - 1, 1).Span.SequenceEqual(s2.Slice(j - 2, 1).Span) &&
                        s1.Slice(i - 2, 1).Span.SequenceEqual(s2.Slice(j - 1, 1).Span))
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
                }
            }
            return matrix[s1Len, s2Len];
        }
    }
}
