using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DataRow = RecordLinkageNet.Core.Data.DataRow;

namespace RecordLinkageNet.Core
{
    [DataContract(Name = "ResultGroup", Namespace = "DataContracts")]
    public class ResultGroup //TODOD: find a better name, 
    {
        [DataMember(Name = "Data")]
        public List<MatchingResultGroup> Data = new List<MatchingResultGroup>();
        public ResultGroup()
        {
        }
        public ResultGroup(List<MatchingResultGroup> group)
        {
            Data = group;
        }

        private class EvenCandidatesObject
        {
            private string key = "";
            private List<DataRow> rowDataList = new List<DataRow>();
            private List<MatchingScore> rowMatchScore = new List<MatchingScore>();
            public EvenCandidatesObject(string key)
            {
                key = key; 
            }

            public void AddRow(DataRow row, MatchingScore ms)
            {
                rowDataList.Add(row);
                rowMatchScore.Add(ms);
            }

            public MatchingScore GetRootMatchingScore()
            {
                if (rowMatchScore.Count > 0)
                    return rowMatchScore[0];
                else return null; 
            }
        }

        public ResultGroup GroupBResultsForUniqueMatchedRows(Configuration config)
        {
            //TODO we filter by what ??? 
            //ResultGroup resultGroup = new ResultGroup();

            ////Dictionary<string, List<string>> myList = new Dictionary<string, List<string>>();
            //DataTableMerger merger = new DataTableMerger();

            ////TODO maybe check if to big ?? 
            //DataTableFeather data = merger.MergeDataTableBy(this.GroupedMatchingResultList ,config.Index.dataTabA , config.Index.dataTabB, true);


            //TODO check direction !! 
            //if (  if (direction == MatchingResultGroup.GroupingDirection.IndexBIsKeyGorGroup).)
            DataTableFeather dataTab = config.Index.dataTabB;
            //int amountData = dataTab.GetAmountRows();
            //List<string> colNamesInDataTab = dataTab.GetColumnNames();

            List<string> colNamesOfComapre = new List<string>();
            foreach (Condition con in config.ConditionList)
            {
                colNamesOfComapre.Add(con.NameColNewLabel);
            }


            foreach (MatchingResultGroup rs in this.Data)
            {
                //we iterate all group 
                //Dictionary<string, List<DataRow>> candidateDoubles = new Dictionary<string, List<DataRow>>();
                Dictionary<string, EvenCandidatesObject> candidateDoubles = new Dictionary<string, EvenCandidatesObject>();
                List<IndexPair> deleteTODOlist = new List<IndexPair>();
                foreach (MatchingScore rsMatch in rs.CandidateList)
                {
                    DataRow row = dataTab.GetRow((int)rsMatch.Pair.bIdx);
                    //we get a hash vor column names 
                    string hashValue = row.GetHashValueForColumList(colNamesOfComapre);
                    EvenCandidatesObject cObj = null; 
                    //List<DataRow> rowList = null;  //new List<DataRow>();

                    if (candidateDoubles.ContainsKey(hashValue))
                    {
                        Trace.WriteLine("found double");
                        //get old list
                        cObj = candidateDoubles[hashValue];
                        cObj.AddRow(row, rsMatch);

                        //rowList.Add(row);

                        //we move the MatchScore as child
                        MatchingScore root = cObj.GetRootMatchingScore();
                        if (root != null)
                            root.AddEvenChild(rsMatch);
                        else
                            Trace.WriteLine("error 293892389 8");
                        //save to delete
                        deleteTODOlist.Add(rsMatch.Pair);
                    }
                    else
                    {
                        cObj = new EvenCandidatesObject(hashValue);
                        cObj.AddRow(row, rsMatch); 
                        //rowList = new List<DataRow>();
                        //rowList.Add(row);
                        candidateDoubles.Add(hashValue, cObj);
                    }
                }//end for each candidate
                 //we remember all important ids
                 //foreach (KeyValuePair<string, List<DataRow>> keyValuePair in candidateDoubles)
                 //{
                 //    if (keyValuePair.Value.Count > 1)
                 //    {
                 //        Trace.WriteLine("230293092093 ");
                 //        //add id list 
                 //        foreach (string importantID in config.ImportantIdList)
                 //        {
                 //            //if(rs.ImportantIDList.TryGetValue(importantID)
                 //            rs.ImportantIDList.Add(importantID, new List<string>());
                 //        }
                 //        //we add alll
                 //        foreach (DataRow row in keyValuePair.Value)
                 //        {
                 //            foreach (string importantID in config.ImportantIdList)
                 //            {
                 //                if (row.Data.ContainsKey(importantID))
                 //                {
                 //                    string value = row.Data[importantID].Value;
                 //                    rs.ImportantIDList[importantID].Add(value); 
                 //                }

                //            }
                //        }
                //    }
                //}
                //TODO kill all same 
                foreach (IndexPair idx in deleteTODOlist)
                {
                    if (!rs.DeleteCandidate(idx))
                        Trace.WriteLine("warning 29892839 index not found");
                }
            }//end for group


            return this;
        }
    }
}
