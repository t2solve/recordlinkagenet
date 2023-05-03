using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Transpose
{
    public class DataTableMerger
    {
        private string tablePrefixA = "";
        private string tablePrefixB = "";

        private string GetRandomString(int length = 3)
        {
            if (length <= 0)
                length = 3;
            var rnd = new Random();
            return new string(Enumerable.Range(0, length).Select(n => (char)rnd.Next(97, 122)).ToArray());
        }


        private string UnifyColumName(string name, List<string> nameList ,string definedPrefix = null)
        {
            string newName = name; 
            if(nameList.Contains(name))
            {
                //we add a prefix 
                string newPrefix = definedPrefix;
                if (newPrefix == null)
                    newPrefix = GetRandomString();
                newName = newPrefix + name;
                newName = UnifyColumName(newName, nameList); // call recursive
            }

            return newName;
        }

        public DataTableFeather MergeDataTableBy(ResultGroup resultGroup, DataTableFeather dA, DataTableFeather dB,bool flagWriteNonMatchedAsRow = false)
        {
            //TODO informa abount A linked to B 

            DataTableFeather dMerged = new DataTableFeather();

            //get some infos we need

            int amountDataA = dA.GetAmountRows();
            int amountDataB = dB.GetAmountRows();

            List<string> colNamesA=dA.GetColumnNames();
            List<string> colNamesB = dB.GetColumnNames();
            //we unfiy all col a name
            //we do a translation for equal names
            Dictionary<string ,string > namesMapTabA = new Dictionary<string ,string>();
            foreach(string nameA  in colNamesA)
            {
                string newNameA = UnifyColumName(nameA, colNamesB);
                namesMapTabA[nameA] = newNameA; 
            }

            //we create all columns *A 
            foreach(string nameAOld in namesMapTabA.Keys)
            {
                //we get the data type from a 
                Type oldType = dA.GetColumnByName(nameAOld).GetDataTypeOfCol();
                if (oldType == null)
                    Trace.WriteLine("foo");
                DataColumn col = new DataColumn(amountDataA, oldType);
                string nameANew = namesMapTabA[nameAOld];

                dMerged.AddColumn(nameANew, col);
            }
            //we create all columns *B 
            foreach (string nameB in colNamesB)
            {
                //we get the data type from a 
                Type oldType = dB.GetColumnByName(nameB).GetDataTypeOfCol();
                if (oldType == null)
                    Trace.WriteLine("foo");
                DataColumn col = new DataColumn(amountDataA, oldType);
             
                dMerged.AddColumn(nameB, col);
            }

            //TODO count several things
            Dictionary<int, bool> rowIndexMemory = new Dictionary<int, bool>();

            //we do need to add all data 
            foreach(var rsGroup in resultGroup.Data)
            {
                foreach(var rsMatch in rsGroup.CandidateList)
                {
                    if(rsMatch.AcceptanceLvl== MatchingScore.AcceptanceLevel.MatchAccepted)
                    {
                        //we do add it to our  
                        int rowIndexOfA = (int)rsMatch.Pair.aIdx;
                        DataRow rowA = dA.GetRow(rowIndexOfA);
                        DataRow rowB = dB.GetRow((int)rsMatch.Pair.bIdx);

                        //we translate a data
                        DataRow rowATranslated = TranslateAllColumnNames(rowA, namesMapTabA);

                        DataRow mergedRow = MergeTwoRows(rowATranslated, rowB);

                        //we need to add the row
                        if(rowIndexMemory.ContainsKey(rowIndexOfA))
                        {
                            Trace.WriteLine("warning 2938982398 doubled matches for 1 row, will be ignore atm"); 
                        }
                        else
                        {
                            rowIndexMemory.Add(rowIndexOfA, true);  

                            dMerged.AddRow(rowIndexOfA, mergedRow);
                        }
                    }
                }
            }

            //
            if(flagWriteNonMatchedAsRow)
            {
                for (int i=0;i< amountDataA;i++)
                {
                    if(! rowIndexMemory.ContainsKey(i))
                    {
                        //we add a A row 
                        DataRow rowA = dA.GetRow(i);
                        DataRow rowATranslated = TranslateAllColumnNames(rowA, namesMapTabA);

                        dMerged.AddRow(i, rowATranslated);
                    }
                }
            }

            return dMerged; 
        }

        private DataRow TranslateAllColumnNames(DataRow d, Dictionary<string,string> namesMap)
        {
            DataRow newRow = new DataRow(null);
            
            foreach(string colName  in d.Data.Keys)
            {
                string colNameNew = "";
                if (namesMap.TryGetValue(colName, out colNameNew))
                {
                    newRow.Data.Add(colNameNew, d.Data[colName]);
                }
                else Trace.WriteLine("warning 2873827387 name not found : " + colName);
            }
            return newRow;
        }

        private DataRow MergeTwoRows(DataRow r1 ,DataRow r2)
        {
            //TODO do not use fore
            DataRow newRow = new DataRow(null);
            foreach (KeyValuePair<string, DataCell> d in r1.Data)
                newRow.Data.Add(d.Key, d.Value);

            foreach (KeyValuePair<string, DataCell> d in r2.Data)
                newRow.Data.Add(d.Key, d.Value);

            return newRow;
        }
    }
}
