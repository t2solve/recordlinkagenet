using RecordLinkageNet.Core.Data;

namespace RecordLinkageNet.Core
{
    public class GroupCriteriaContainer
    {

       
        private uint indexKey = uint.MaxValue;
        private DataCell indexCell = null;

        public GroupCriteriaContainer(uint indexKey )
        {
            this.indexKey = indexKey; 
        }
        public GroupCriteriaContainer(DataCell indexCell)
        {
            this.indexCell = indexCell;
        }

        public uint IndexKey { get => indexKey; set => indexKey = value; }
        public DataCell IndexCell { get => indexCell; set => indexCell = value; }
    }
}