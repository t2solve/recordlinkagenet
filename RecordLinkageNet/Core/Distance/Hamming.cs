using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RecordLinkageNet.Core.Distance
{
    // TODO need an extem speedup, is supler slow so far
    public static class Hamming
    { 
        public static double HammingDistance(this string s1, string s2)
        {
            return s1.Zip(s2, (x, y) => AreEqual(x, y) ? 0 : 1).Sum();
        }

        //less performant
        private static bool AreEqual(char one, char other)
        {
            IEqualityComparer<char> equalityComparer = EqualityComparer<char>.Default; //get the default
            return equalityComparer.Equals(one, other);
        }

        public static double HammingDistance2(this string s1, string s2)
        {
            if (s1.Length != s2.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            double distance =
                s1.ToCharArray()
                .Zip(s2.ToCharArray(), (c1, c2) => new { c1, c2 })
                .Count(m => m.c1 != m.c2);

            return distance;
        }
    }
}