using System;
using System.Collections.Generic;
using System.Text;

namespace RecordLinkageNet.Core.Distance
{
    //see https://www.geeksforgeeks.org/jaro-and-jaro-winkler-similarity/
    public static class JaroWinkler
    {
        public static double JaroDistance(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {
            int len1 = s1.Length,
                len2 = s2.Length;

            //we check if even, short cut
            if (len1 == len2)
            {
                if(s1.Span.SequenceEqual(s2.Span))
                    return 1.0;
            }
            //else no event
            if (len1 == 0 || len2 == 0)
                return 0.0;

            // Maximum distance , we compare with
            int maxDist = (int)Math.Floor((double)
                            Math.Max(len1, len2) / 2) - 1;

            // Count of matches
            int match = 0;

            // Hash for matches
            int[] hashS1 = new int[s1.Length];
            int[] hashS2 = new int[s2.Length];

            // Traverse through the first string
            for (int i = 0; i < len1; i++)
            {

                // Check if there is any matches
                for (int j = Math.Max(0, i - maxDist);
                    j < Math.Min(len2, i + maxDist + 1); j++)

                    // If there is a match
                    if (s1.Slice(i,1 ).Span.SequenceEqual(s2.Slice(j, 1).Span) &&
                        hashS2[j] == 0)
                    {
                        hashS1[i] = 1;
                        hashS2[j] = 1;
                        match++;
                        break;
                    }
            }

            // If there is no match
            if (match == 0)
                return 0.0;

            // Number of transpositions
            double t = 0;

            int point = 0;

            // Count number of occurrences
            // where two characters match but
            // there is a third matched character
            // in between the indices
            for (int i = 0; i < len1; i++)
                if (hashS1[i] == 1)
                {

                    // Find the next matched character
                    // in second string
                    while (hashS2[point] == 0)
                        point++;

                    if(s1.Slice(i, 1).Span.SequenceCompareTo(s2.Slice(point++,1).Span)!=0)
                    //if (s1[i] != s2[point++])
                        t++;
                }
            t /= 2;

            // Return the Jaro Similarity
            return (match / (double)len1
                    + match / (double)len2
                    + (match - t) / match)
                / 3.0;
        }

        public static double JaroDistance(this string s1, string s2)
        {
            // If the strings are equal
            return (s1.AsMemory().JaroDistance(s1.AsMemory()));
            //    return 1.0;

            //// Length of two strings
            //int len1 = s1.Length,
            //    len2 = s2.Length;

            //if (len1 == 0 || len2 == 0)
            //    return 0.0;

            //// Maximum distance upto which matching
            //// is allowed
            //int maxDist = (int)Math.Floor((double)
            //                Math.Max(len1, len2) / 2) - 1;

            //// Count of matches
            //int match = 0;

            //// Hash for matches
            //int[] hashS1 = new int[s1.Length];
            //int[] hashS2 = new int[s2.Length];

            //// Traverse through the first string
            //for (int i = 0; i < len1; i++)
            //{

            //    // Check if there is any matches
            //    for (int j = Math.Max(0, i - maxDist);
            //        j < Math.Min(len2, i + maxDist + 1); j++)

            //        // If there is a match
            //        if (s1[i] == s2[j] &&
            //            hashS2[j] == 0)
            //        {
            //            hashS1[i] = 1;
            //            hashS2[j] = 1;
            //            match++;
            //            break;
            //        }
            //}

            //// If there is no match
            //if (match == 0)
            //    return 0.0;

            //// Number of transpositions
            //double t = 0;

            //int point = 0;

            //// Count number of occurrences
            //// where two characters match but
            //// there is a third matched character
            //// in between the indices
            //for (int i = 0; i < len1; i++)
            //    if (hashS1[i] == 1)
            //    {

            //        // Find the next matched character
            //        // in second string
            //        while (hashS2[point] == 0)
            //            point++;

            //        if (s1[i] != s2[point++])
            //            t++;
            //    }
            //t /= 2;

            //// Return the Jaro Similarity
            //return (match / (double)len1
            //        + match / (double)len2
            //        + (match - t) / match)
            //    / 3.0;
        }

        public static double JaroWinklerSimilarity(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {
            double jaroDist = JaroDistance(s1, s2);

            // If the jaro Similarity is above a threshold
            if (jaroDist > 0.7)//TODO double check threshold
            {

                // Find the length of common prefix
                int prefix = 0;

                for (int i = 0; i < Math.Min(s1.Length,
                                            s2.Length); i++)
                {

                    // If the characters match
//                    if (s1[i] == s2[i])
                    if (s1.Slice(i,1).Span.SequenceEqual(s2.Slice(i,1).Span))
                        prefix++;

                    // Else break
                    else
                        break;
                }

                // Maximum of 4 characters are allowed in prefix
                prefix = Math.Min(4, prefix);

                // Calculate jaro winkler Similarity
                jaroDist += 0.1 * prefix * (1 - jaroDist);
            }
            return jaroDist;
        }

        public static double JaroWinklerSimilarity(this string s1, string s2)
        {
            return s1.AsMemory().JaroWinklerSimilarity(s2.AsMemory());  

            //double jaroDist = JaroDistance(s1, s2);

            //// If the jaro Similarity is above a threshold
            //if (jaroDist > 0.7)//TODO double check threshold
            //{

            //    // Find the length of common prefix
            //    int prefix = 0;

            //    for (int i = 0; i < Math.Min(s1.Length,
            //                                s2.Length); i++)
            //    {

            //        // If the characters match
            //        if (s1[i] == s2[i])
            //            prefix++;

            //        // Else break
            //        else
            //            break;
            //    }

            //    // Maximum of 4 characters are allowed in prefix
            //    prefix = Math.Min(4, prefix);

            //    // Calculate jaro winkler Similarity
            //    jaroDist += 0.1 * prefix * (1 - jaroDist);
            //}
            //return jaroDist;
        }
    }
}
