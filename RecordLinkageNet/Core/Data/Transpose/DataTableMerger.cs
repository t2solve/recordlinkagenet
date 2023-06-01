using RecordLinkageNet.Core.Compare;
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
        //private string tablePrefixA = "";
        //private string tablePrefixB = "";

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

        private bool CheckTableDataIsPresent(DataTableFeather dA, DataTableFeather dB)
        {
            bool succes = true; 
            if (dA == null || dB == null)
            {
                Trace.WriteLine("warning 345346346 DataTableFeather is null ");
                return false; 
            }
            if (dA.GetAmountColumns() == 0 || dB.GetAmountColumns() == 0)
            {
                Trace.WriteLine("warning 3463462457785 DataTableFeather has NO columns ");
                return false; 
            }

            return succes; 
        }

        public bool CheckTableColumnNamesAreEqual(DataTableFeather dA, DataTableFeather dB)
        {
            bool success = false;

            if (CheckTableDataIsPresent(dA, dB))
            {

                foreach (string nameA in dA.GetColumnNames())
                {
                    foreach (string nameB in dB.GetColumnNames())
                        if (String.Compare(nameA, nameB) == 0)
                            return true;
                }
            }
            return success;
        }
        private bool CheckIndexListIsPresent(List<IndexPair> indexList)
        {
            if (indexList == null)
            {
                Trace.WriteLine("warning 234562364 indexlist is null");
                return false;
            }
            if (indexList.Count == 0)
            {
                Trace.WriteLine("warning 2346246 indexlist is empty");
                return false;
            }
            return true; 
        }

        public int CountAmountOfRowsFromIndexList(List<IndexPair> indexList)
        {
            int amount = -1;
            if (CheckIndexListIsPresent(indexList))
            {
                return indexList.Count();
            }
            return amount; 
        }

        public DataTableFeather MergeColumnsDataByIndexList(List<IndexPair> indexList, DataTableFeather dA, DataTableFeather dB, bool allowColumnRenamingByConflict = false)
        {
            //TODO change to indexfeather ? 
            DataTableFeather dMerged = null;

           
            if (CheckIndexListIsPresent(indexList) && CheckTableDataIsPresent(dA, dB))
            {

                if (!allowColumnRenamingByConflict && CheckTableColumnNamesAreEqual(dA, dB))
                {
                    Trace.WriteLine("warning 2346598239898 column names have name conflict, use allowColumnRenamingByConflict = True ");
                    return null;
                }

                int amountRows = CountAmountOfRowsFromIndexList(indexList);
                if (amountRows==-1)
                {
                    Trace.WriteLine("warning 2352523 error during calc index length");
                    return null;
                }

                //TODO test what we got as parameter  
                Dictionary<string, string> namesMapTabB = null; 
                dMerged = CreateTableHeader( dB, dA, amountRows, out namesMapTabB);

                //we do add it to our  
                int rowCounter = 0;
                foreach (IndexPair indexPair in indexList)
                {
                    DataRow rowA = dA.GetRow((int)indexPair.aIdx);
                    DataRow rowB = dB.GetRow((int)indexPair.bIdx); ; 
                    //if indexRowB is max we add empty row ??? TODO remove againg
                    //uint indexRowB = indexPair.bIdx; 
                    //if (indexRowB == uint.MaxValue)
                    //{
                    //    foreach (KeyValuePair<string, DataCell> d in rowB.Data)
                    //        d.Value.Value = "";
                    //}
                    

                    //we translate a data
                    DataRow rowATranslated = TranslateAllColumnNames(rowB, namesMapTabB);
                    //merge
                    DataRow mergedRow = MergeTwoRows(rowATranslated, rowA);
                    dMerged.AddRow(rowCounter, mergedRow);
                    rowCounter += 1;
                }
            }
            return dMerged;
        }

        private DataTableFeather CreateTableHeader(DataTableFeather dA, DataTableFeather dB, int amountRowsForFutureTable, out Dictionary<string, string> namesMapTabA)
        {
            DataTableFeather dMerged = new DataTableFeather();

            List<string> colNamesA = dA.GetColumnNames();
            List<string> colNamesB = dB.GetColumnNames();
            //we unfiy all col a name
            //we do a translation for equal names
            namesMapTabA = new Dictionary<string, string>();
            foreach (string nameA in colNamesA)
            {
                string newNameA = UnifyColumName(nameA, colNamesB);
                namesMapTabA[nameA] = newNameA;
            }

            //we create all columns *A 
            foreach (string nameAOld in namesMapTabA.Keys)
            {
                //we get the data type from a 
                Type oldType = dA.GetColumnByName(nameAOld).GetDataTypeOfCol();
                if (oldType == null)
                {
                    //Trace.WriteLine("foo");
                    throw new ArgumentOutOfRangeException("error 25636346 column " + nameAOld + " not found");

                }
                DataColumn col = new DataColumn(amountRowsForFutureTable, oldType);
                string nameANew = namesMapTabA[nameAOld];

                dMerged.AddColumn(nameANew, col);
            }
            //we create all columns *B 
            foreach (string nameB in colNamesB)
            {
                //we get the data type from a 
                Type oldType = dB.GetColumnByName(nameB).GetDataTypeOfCol();
                if (oldType == null)
                {
                    Trace.WriteLine("foo");
                    throw new ArgumentOutOfRangeException("error 92839823 column " + nameB + " not found");

                }
                DataColumn col = new DataColumn(amountRowsForFutureTable, oldType);

                dMerged.AddColumn(nameB, col);
            }

            return dMerged;
        }


        //public DataTableFeather MergeDataTableBy(ResultGroup resultGroup, DataTableFeather dA, DataTableFeather dB,bool flagWriteNonMatchedAsRow = false, bool flagWriteDoubleAcceptedAsDoubleRows = false)
        //{
        //    //TODO informa abount A linked to B 

        //    DataTableFeather dMerged = new DataTableFeather();
        //    List<string> colNamesA = dA.GetColumnNames();
        //    List<string> colNamesB = dB.GetColumnNames();
        //    //we unfiy all col a name
        //    //we do a translation for equal names
        //    Dictionary<string, string> namesMapTabA = new Dictionary<string, string>();
        //    foreach (string nameA in colNamesA)
        //    {
        //        string newNameA = UnifyColumName(nameA, colNamesB);
        //        namesMapTabA[nameA] = newNameA;
        //    }

        //    //get some infos we need

        //    int amountDataA = dA.GetAmountRows();
        //    int amountDataB = dB.GetAmountRows();

        //    int amountRowsForFutureTable = amountDataA;

        //    if (flagWriteDoubleAcceptedAsDoubleRows)
        //    {
        //        amountRowsForFutureTable = 0;
        //        //we need to count all accepted rows 
        //        foreach (var rsGroup in resultGroup.Data)
        //        {
        //            bool oneEntryAccepted = false;
        //            foreach (var rsMatch in rsGroup.CandidateList)
        //            {

        //                if (rsMatch.AcceptanceLvl == MatchingScore.AcceptanceLevel.MatchAccepted)
        //                {
        //                    if (flagWriteDoubleAcceptedAsDoubleRows && !oneEntryAccepted)
        //                    {

        //                        amountRowsForFutureTable++;
        //                        oneEntryAccepted = true;
        //                    }
        //                }
        //            }
        //            if (!oneEntryAccepted && flagWriteNonMatchedAsRow)
        //                amountRowsForFutureTable++;
        //        }
        //    }

        //    //create the rows
        //    //dMerged = CreateColumnFromTwoTables(dA, dB, amountRowsForFutureTable);
           

        //    //TODO count several things
        //    Dictionary<int, bool> rowIndexMemory = new Dictionary<int, bool>();

        //    int rowCounter = 0;
        //    //we do need to add all data 
        //    foreach(var rsGroup in resultGroup.Data)
        //    {
        //        foreach(var rsMatch in rsGroup.CandidateList)
        //        {
        //            if(rsMatch.AcceptanceLvl== MatchingScore.AcceptanceLevel.MatchAccepted)
        //            {
        //                //we do add it to our  
        //                int rowIndexOfA = (int)rsMatch.Pair.aIdx;
        //                DataRow rowA = dA.GetRow(rowIndexOfA);
        //                DataRow rowB = dB.GetRow((int)rsMatch.Pair.bIdx);

        //                //we translate a data
        //                DataRow rowATranslated = TranslateAllColumnNames(rowA, namesMapTabA);

        //                DataRow mergedRow = MergeTwoRows(rowATranslated, rowB);

        //                //we need to add the row
        //                if(rowIndexMemory.ContainsKey(rowIndexOfA))
        //                {
        //                    if(flagWriteDoubleAcceptedAsDoubleRows)
        //                        dMerged.AddRow(rowCounter, mergedRow); 
        //                    //else 
        //                    //    Trace.WriteLine("warning 2938982398 doubled matches for 1 row, will be ignore atm"); 

        //                }
        //                else //first entry we add
        //                {
        //                    rowIndexMemory.Add(rowIndexOfA, true);  

        //                    dMerged.AddRow(rowCounter, mergedRow);
        //                }
                      
        //            }
        //            rowCounter++;//count
        //        }
        //    }

        //    //
        //    if(flagWriteNonMatchedAsRow)
        //    {
        //        for (int i=0;i< amountDataA;i++)
        //        {
        //            if(! rowIndexMemory.ContainsKey(i))
        //            {
        //                //we add a A row 
        //                DataRow rowA = dA.GetRow(i);
        //                DataRow rowATranslated = TranslateAllColumnNames(rowA, namesMapTabA);

        //                dMerged.AddRow(rowCounter++, rowATranslated);
        //            }
        //        }
        //    }

        //    return dMerged; 
        //}

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
