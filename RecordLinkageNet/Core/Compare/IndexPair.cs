using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class for index tuple 
    /// 
    /// </summary>
    public struct IndexPair : IEqualityComparer<IndexPair>
    {

        public uint aIdx = uint.MaxValue;
        public uint bIdx = uint.MaxValue;
        ////  aIdx  bIdx  //ulong is to huge 
        ////public Tuple<uint, uint> value = null;  //TODO check Z dimension ? 
        ////public long resultIdx = -1;
        ////public short conditionIndex = -1;
        //public IndexPair()
        //{
        //    //value = new Tuple<uint, uint>(uint.MaxValue, uint.MaxValue);
        //}
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
        public int GetHashCode([DisallowNull] IndexPair obj)
        {
            return HashCode.Combine(obj.aIdx, obj.bIdx);
        }

        public override string ToString()
        {
            return "Pair<" + aIdx + "," + bIdx + ">";
        }
        ////public IndexPair(uint x, uint y, short conIndex)
        ////{
        ////    this.value = new Tuple<uint, uint>(x, y);
        ////    this.conditionIndex = conIndex;
        ////}

        //public override bool Equals(object obj) => Equals(obj as IndexPair);

        //public bool Equals(IndexPair p)
        //{
        //    //bool success=false;
        //    //if(this.conditionIndex==p.conditionIndex)
        //    //if (value.Item2 == p.value.Item2)
        //    //    if (value.Item1 == p.value.Item1)
        //    //        return true;
        //    if (this.aIdx == p.aIdx)
        //        if (this.bIdx == p.bIdx)
        //            return true;
        //    return false;
        //}

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(aIdx,bIdx);
        //}

        //public int GetResultIndex(uint amountRowsInB)
        //{

        //    return (int)((value.Item1 * (amountRowsInB)) + value.Item2); 
        //}

    }
}
