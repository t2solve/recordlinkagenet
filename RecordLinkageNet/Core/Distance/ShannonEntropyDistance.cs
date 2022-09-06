using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Distance
{
    public static class ShannonEntropyDistance
    {

        public static double EntropyDistance(this ReadOnlyMemory<Char> s1, ReadOnlyMemory<Char> s2)
        {
            double a = ShannonEntropy(s1);
            double b = ShannonEntropy(s2);
            return Math.Abs(a - b);
        }
        public static double EntropyDistance(this string s1, string s2)
        {
            double a = ShannonEntropy(s1.AsMemory());
            double b = ShannonEntropy(s2.AsMemory());
            return Math.Abs(a - b);
        }


        /// <summary>
        /// https://codereview.stackexchange.com/questions/868/calculating-entropy-of-a-string
        /// returns bits of entropy represented in a given string, per 
        /// http://en.wikipedia.org/wiki/Entropy_(information_theory) 
        /// </summary>
        public static double ShannonEntropy(ReadOnlyMemory<Char> s)
        {
            var map = new Dictionary<char, int>();

            //foreach (Char c in s)
            for(int i=0;i<s.Length; i++)
            //foreach (char c in s.Slice(0))
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
