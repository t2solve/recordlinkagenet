using RecordLinkageNet.Core.Data;
using System.Runtime.Serialization;

namespace RecordLinkageNet.Core
{
    [DataContract(Name = "GroupCriteriaContainer", Namespace = "RecordLinkageNet")]
    public class GroupCriteriaContainer
    {

        [DataMember(Name = "IndexKey")]

        private uint indexKey = uint.MaxValue;
        [DataMember(Name = "IndexCell")]
        private DataCell indexCell = null;

        public GroupCriteriaContainer(uint indexKey)
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