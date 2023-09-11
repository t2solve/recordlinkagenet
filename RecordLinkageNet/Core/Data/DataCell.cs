using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RecordLinkageNet.Core.Data
{
    [DataContract(Name = "DataCell", Namespace = "RecordLinkageNet")]
    [KnownType(typeof(DataCellString))]
    public abstract class DataCell : IEqualityComparer<DataCell> //, IComparable<DataCell>
    {
        [DataMember(Name = "Id")]
        public uint Id { get; set; } = uint.MaxValue; //aka index id 

        //public virtual string Value { get; set; } = "";

        //private string _value;

        public virtual string Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Equals(DataCell x, DataCell y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(DataCell obj)
        {
            throw new NotImplementedException();
        }


    }
}
