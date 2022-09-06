using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //no Dataframe yet, so we use a resultset
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
    }
}
