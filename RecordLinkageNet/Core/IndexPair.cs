using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    /// <summary>
    /// Class for index tuple 
    /// 
    /// </summary>
    public class IndexPair
    {
      
                 //  aIdx  bIdx
        public Tuple<long, long> value = null;  //TODO check Z dimension ? 
        //public long resultIdx = -1;
        public short conditionIndex = -1;
        public IndexPair()
        {
            value = new Tuple<long, long>(-1, -1);
        }
        public IndexPair(long x, long y)
        {
            value = new Tuple<long, long>(x, y);
        }
        public IndexPair(long x, long y, short conIndex)
        {
            this.value = new Tuple<long, long>(x, y);
            this.conditionIndex = conIndex;
        }

        public override bool Equals(object obj) => this.Equals(obj as IndexPair);

        public bool Equals(IndexPair p)
        {
            //bool success=false;
            if(this.conditionIndex==p.conditionIndex)
                if (this.value.Item2 == p.value.Item2)
                    if (this.value.Item1 == p.value.Item1)
                    return true;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value.Item1,value.Item2, conditionIndex);
        }

        public int GetResultIndex(long amountRowsInB)
        {

            return (int)((value.Item1 * (amountRowsInB)) + value.Item2); 
        }

    }
}
