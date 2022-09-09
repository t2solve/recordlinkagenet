using System;
using System.Collections.Generic;
using System.Linq;

namespace RecordLinkageNet.Core.Distance
{
    // TODO need an extem speedup, is supler slow so far
    public static class Hamming
    {

        public static double HammingDistance(this string s1, string s2)
        {
            return HammingDistance(s1.AsMemory(), s2.AsMemory());
        }

        public static double HammingDistance(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {
            //port of https://github.com/jamesturk/cjellyfish/blob/66c48ae2998af3364b0653f74634b507fa5be823/hamming.c
            double distance = 0; 
            int s1Len = s1.Length;
            int s2Len = s2.Length;
            int i1 = 0;
            int i2 = 0;
            for (; i1 < s1Len && i2 < s2Len; i1++, i2++)
            {
                if(!(s1.Slice(i1, 1).Span.SequenceEqual(s2.Slice(i2, 1).Span)))
                //if (*s1 != *s2)
                {
                    distance++;
                }
            }
            for (; i1 < s1Len; i1++) //count end as diff
            {
                distance++;
            }
            for (; i2 < s2Len; i2++)//count end as diff
            {
                distance++;
            }
            return distance;
        }

        //public static double HammingDistance(this string s1, string s2)
        //{
        //    return s1.Zip(s2, (x, y) => AreEqual(x, y) ? 0 : 1).Sum();
        //}

      
        //private static bool AreEqual(char one, char other)
        //{
        //    IEqualityComparer<char> equalityComparer = EqualityComparer<char>.Default; //get the default
        //    return equalityComparer.Equals(one, other);
        //}

        ////less performant
        //public static double HammingDistance2(this string s1, string s2)
        //{
        //    if (s1.Length != s2.Length)
        //    {
        //        throw new Exception("Strings must be equal length");
        //    }

        //    double distance =
        //        s1.ToCharArray()
        //        .Zip(s2.ToCharArray(), (c1, c2) => new { c1, c2 })
        //        .Count(m => m.c1 != m.c2);

        //    return distance;
        //}
    }
}