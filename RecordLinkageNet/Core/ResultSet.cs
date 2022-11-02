using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    
    /// <summary>
    /// no Dataframe in ml net yet starts with 2.0 ?, so we use a own resultset structure
    /// </summary>
    public class ResultSet
    {
        public List<Tuple<int, int>> indexList = null;
        public List<string> colNames = null;
        public float[,] data = null; 

        public ResultSet(List<Tuple<int, int>> indexList, List<string> colNames, float[,] data)
        {
            this.indexList = indexList;
            this.colNames = colNames;
            this.data = data;
        }

        public int GetConditionIndexByName(string name)
        {
            int retVal = -1;

            if (colNames.Contains(name))
            {
                retVal = colNames.IndexOf(name);
            }
            else
                Trace.WriteLine("warning 29839283, no condition found of name: " + name + " found in resultsset");

            return retVal;
        }

        public float[] GetResultByConditonName(string name)
        {
            float[] result = { -1 }; //default
            int indexNumber = GetConditionIndexByName(name); 
            if(indexNumber>0)
            {
               result = Enumerable.Range(0, indexList.Count)
                .Select(x => data[x, indexNumber])
                .ToArray();
            }

            return result; 
        }
        
        public void PrintReadableDebug(int lineAmount = 100)
        {
            string sep = ";";
            int limit = lineAmount;
            if (limit > indexList.Count)
                limit = indexList.Count;

            //headline
            StringBuilder headLine = new StringBuilder();
            headLine.Append("idx<a,b>\t");
            for (int j = 0; j < colNames.Count; j++)
            {
                headLine.Append(colNames[j]);
                headLine.Append("\t");
            }
            Console.WriteLine(headLine.ToString());
            for (int i = 0; i < limit; i++)
            {
                StringBuilder b = new StringBuilder();
                b.Append("<");
                b.Append(indexList[i].Item1);
                b.Append(",");
                b.Append(indexList[i].Item2);
                b.Append(">");
                b.Append("\t");
                for (int j=0;j<colNames.Count;j++)
                {
                    b.AppendFormat("{0,12}", data[i, j]);
                    b.Append(sep);
                    b.Append("\t");
                }

                Console.WriteLine(b.ToString());

            }
        }






        //public IDataView GetAsDataView()
        //{
        //    MLContext mlContext = new MLContext();
        //    int m = indexList.Count;
        //    int n = colNames.Count;
        //    //BinaryData[] inMemoryCollection = new BinaryData[m];
        //    //for (int i = 0; i < m; i++)
        //    //{

        //    //    inMemoryCollection[i] = new BinaryData
        //    //    {
        //    //         = data[0, i],
        //    //        Label = Convert.ToBoolean(Convert.ToInt64(data[1, i]))
        //    //    };
        //    //}


        //    IDataView dataView = mlContext.Data.LoadFromEnumerable<BinaryData>(inMemoryCollection);


        //    return null;
        //}
    }
}
