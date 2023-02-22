using Microsoft.ML;
using RecordLinkageNet.Core.Compare;
//using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    
    /// <summary>
    /// no Dataframe in ml net yet starts with 2.0 ?, so we use a own resultset structure
    /// </summary>
    //[ProtoContract]
    public class ResultSet
    {

        public Dictionary<IndexPair, int> indexMap = null;
        //              [a,b]    
        //[ProtoMember(1)]
        //public FLatIndexPair[] indexArray = null;

        //! we need a own managed structure, because of huge data
        public HugeMatrix<byte> compareData = null;
        //[ProtoMember(2)]
        //public ulong indexCount = 0;
        //[ProtoMember(3)]
        //public long indexCountB = 0;
        //[ProtoMember(4)]
        public List<string> colNames = null;
        //[ProtoMember(5)]
        //public byte[,] data = null; //compare data

        //analys data
        public float[] scoresValuesAbsolute = null;
        public float[] scoresValuesRelativToMax = null;


        //public HugeMatrix<byte> data = null;

      
    }
}
