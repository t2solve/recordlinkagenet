using System.Collections.Generic;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class for index tuple 
    /// 
    /// </summary>
    //[Serializable]
    public struct IndexPair : IEqualityComparer<IndexPair>
    {
        public uint aIdx = uint.MaxValue;
        public uint bIdx = uint.MaxValue;

        public IndexPair()
        {
            aIdx = uint.MaxValue; ;
            bIdx = uint.MaxValue; ;
        }
        public IndexPair(uint x, uint y)
        {
            aIdx = x;
            bIdx = y;
            //value = new Tuple<uint, uint>(x, y);
        }

        public bool Equals(IndexPair x, IndexPair y)
        {
            if (x.aIdx == y.aIdx)
                if (x.bIdx == y.bIdx)
                    return true;
            return false;
        }
        public int GetHashCode(IndexPair obj)
        {
            // .net core version:
            //return HashCode.Combine(obj.aIdx, obj.bIdx);
            //see
            //https://stackoverflow.com/questions/60956569/hashcode-equivalent-in-net-framework
            return new { obj.aIdx, obj.bIdx }.GetHashCode();
        }

        public override string ToString()
        {
            return "Pair<" + aIdx + "," + bIdx + ">";
        }


    }
}
