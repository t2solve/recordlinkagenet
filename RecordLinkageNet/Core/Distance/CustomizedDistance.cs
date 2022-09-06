using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Distance
{
    public static class CustomizedDistance
    {
        /// <summary>
        ///  stub to implement a a user definied distance
        /// </summary>
        /// <param name="s1">this</param>
        /// <param name="s2">string we compare to</param>
        /// <returns>distance as double</returns>
        public static double MyDistance(this string s1, string s2)
        {
            double distance = 42;

            //TODO implement here
            throw new NotImplementedException();

            return distance;
        }
    }
}
