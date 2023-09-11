using RecordLinkageNet.Util;
using System;

namespace RecordLinkageNet.Core.Distance
{
    public static class JaroWinkler
    {
        public static double JaroDistance(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {

            //see best explanation https://www.geeksforgeeks.org/jaro-and-jaro-winkler-similarity/

            int s1Len = s1.Length;
            int s2Len = s2.Length;

            //we check if even, short cut
            if (s1Len == s2Len)
            {
                //is empty empty == 1.0 meaningful ? 
                //no => return 0
                if (s1Len == 0)
                    return 0;

                if (s1.Span.SequenceEqual(s2.Span))
                    return 1.0;
            }
            //else no event
            if (s1Len == 0 || s2Len == 0)
                return 0.0;

            //  calc maximum distance , we compare with
            int maxDist = (int)Math.Floor((double)
                            Math.Max(s1Len, s2Len) / 2) - 1;

            int match = 0;
            int[] hashS1 = new int[s1.Length];
            int[] hashS2 = new int[s2.Length];
            double t = 0;
            int point = 0;

            // iterate through the first string
            for (int i = 0; i < s1Len; i++)
            {
                for (int j = Math.Max(0, i - maxDist);
                    j < Math.Min(s2Len, i + maxDist + 1); j++)

                    // if match we mark it
                    if (s1.Slice(i, 1).Span.SequenceEqual(s2.Slice(j, 1).Span) &&
                        hashS2[j] == 0)
                    {
                        hashS1[i] = 1;
                        hashS2[j] = 1;
                        match++;
                        break;
                    }
            }

            if (match == 0)//no match at all
                return 0.0;

            // count number of occurrences
            // where two characters match but
            // there is a third matched character
            // in between the indices
            for (int i = 0; i < s1Len; i++)
                if (hashS1[i] == 1)
                {

                    // find the next matched character
                    // in second string
                    while (hashS2[point] == 0)
                        point++;

                    if (s1.Slice(i, 1).Span.SequenceCompareTo(s2.Slice(point++, 1).Span) != 0)
                        t++;
                }
            t /= 2;

            // calc Jaro Similarity, see wiki for formular
            return (match / (double)s1Len
                    + match / (double)s2Len
                    + (match - t) / match)
                / 3.0;
        }

        public static double JaroDistance(this string s1, string s2)
        {
            return JaroDistance(s1.AsMemory(), s2.AsMemory());

        }


        public static double JaroWinklerSimilarity(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {
            double jaroDist = JaroDistance(s1, s2);

            // if the jaro Similarity is above a threshold
            if (jaroDist > 0.7)//TODO double check threshold
            {
                // find the length of common prefix
                int prefix = 0;
                for (int i = 0; i < Math.Min(s1.Length,
                                            s2.Length); i++)
                {
                    //match
                    if (s1.Slice(i, 1).Span.SequenceEqual(s2.Slice(i, 1).Span))
                        prefix++;

                    else
                        break;
                }

                // maximum of 4 characters are allowed in prefix
                prefix = Math.Min(4, prefix);

                // calculate jaro winkler Similarity
                jaroDist += 0.1 * prefix * (1 - jaroDist);
            }
            return jaroDist;
        }

        public static double JaroWinklerSimilarity(this string s1, string s2)
        {
            return JaroWinklerSimilarity(s1.AsMemory(), s2.AsMemory());

        }
    }
}
