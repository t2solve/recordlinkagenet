﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RecordLinkageNet.Core.Data
{
    [DataContract(Name = "DataCellString", Namespace = "RecordLinkageNet")]
    public class DataCellString : DataCell, IEqualityComparer<DataCellString>
    {
        //TODO refactor eveything is a string here
        [DataMember(Name = "MyValue")]
        private string myValue = "";
        public override string Value 
        { 
            get
            {
                return myValue;
            }
            set 
            {
                myValue = value;
            } 
        } 

        public bool Equals(DataCellString x, DataCellString y)
        {
            if ( ReferenceEquals(x, y)) return true;

            if(x !=null && y !=null)
            {
                if (x.GetHashCode() == y.GetHashCode())
                    return true; 
            }

            return false; 
        }

        public int GetHashCode( DataCellString obj)
        {
           return obj.myValue.GetHashCode();
        }
    }
}
