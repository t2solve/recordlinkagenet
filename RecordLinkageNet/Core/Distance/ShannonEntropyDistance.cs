﻿using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;

namespace RecordLinkageNet.Core.Distance
{
    public static class ShannonEntropy
    {
        public static double ShannonEntropyDistanceNormalizedToRange0To1(this string s1, string s2)
        {
            double a = Calc(s1.AsMemory());
            double b = Calc(s2.AsMemory());
           
            return (1f - NumberNormalizeHelper.NormalizeNumberToRange0to1(Math.Abs(a - b), Math.Max(a, b)));
        }

        public static double ShannonEntropyDistance(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {
            double a = Calc(s1);
            double b = Calc(s2);
            return Math.Abs(a - b);
        }
        public static double ShannonEntropyDistance(this string s1, string s2)
        {
            double a = Calc(s1.AsMemory());
            double b = Calc(s2.AsMemory());
            return Math.Abs(a - b);
        }

        /// <summary>
        /// https://codereview.stackexchange.com/questions/868/calculating-entropy-of-a-string
        /// returns bits of entropy represented in a given string, per 
        /// http://en.wikipedia.org/wiki/Entropy_(information_theory) 
        /// </summary>
        public static double Calc(ReadOnlyMemory<Char> s)
        {
            var map = new Dictionary<char, int>();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.Slice(i, 1).ToArray()[0]; //what ?? the normal way of to char ??
                //see disscuision why not iterable direct https://github.com/dotnet/runtime/issues/23950
                if (!map.ContainsKey(c))
                    map.Add(c, 1);
                else
                    map[c] += 1;
            }

            double result = 0.0;
            int len = s.Length;
            foreach (var item in map)
            {
                var frequency = (double)item.Value / len;
                result -= frequency * (Math.Log(frequency) / Math.Log(2));
            }

            return result;
        }

    }

}
