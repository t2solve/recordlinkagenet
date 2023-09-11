using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class which does the comparision, internal iterating over DataSet A and B 
    /// </summary>
    [DataContract(Name = "ConditionList", Namespace = "RecordLinkageNet")]
    public class ConditionList : IEnumerable<Condition>
    {
        //jobId, result
        //private Queue<Task<Tuple<long, float>>> myTaskList = new Queue<Task<Tuple<long, float>>>(); //really ?? 
        [DataMember(Name = "ConditionListData")]
        private List<Condition> conditionList = new();//condition list 
        [DataMember(Name = "ColNamesMappingNewToCond")]
        private Dictionary<string, Tuple<string, string>> colNamesMappingNewToCond = new();

        public Tuple<string, string> GetOldNamesOfColumn(string name)
        {
            Tuple<string, string> retVal = null;

            if (colNamesMappingNewToCond == null)
            {
                Trace.WriteLine("error 2837878787 GetOldNamesOfColumn");
                return retVal;
            }
            if (colNamesMappingNewToCond.ContainsKey(name))
            {
                retVal = colNamesMappingNewToCond[name];
            }

            return retVal;
        }
        public void AddExisting(Condition con)
        {
            //TODO chekc if con name New already exists !! 
            RememberConditionNamesForTranslation(con);

            if (conditionList.Count() > 250)
                throw new Exception("error 293898 interal maximum for condition reached");

            con.ConditionIndex = (byte)this.conditionList.Count();

            this.conditionList.Add(con);
        }

        public int GetAmountConditions()
        {
            return conditionList.Count;
        }

        public void Clear()
        {
            conditionList.Clear();
        }



        private void RememberConditionNamesForTranslation(Condition j)
        {
            if (j == null)
            {
                Trace.WriteLine("error 239898  Condition is null");
                throw new ArgumentException("error 239898");

            }
            if (string.IsNullOrEmpty(j.NameColNewLabel))
            {
                Trace.WriteLine("warning 3984989 no NameColNewLabel set");
                return;
            }
            if (colNamesMappingNewToCond == null)
            {
                Trace.WriteLine("warning 26745345 no colNamesMappingNewToCond ");
                return;
            }
            //we remember what we renames 
            if (colNamesMappingNewToCond.ContainsKey(j.NameColNewLabel))
            {
                Trace.WriteLine("error 2932983 doubled condition names are not allowed ");
                throw new ArgumentException();
            }
            colNamesMappingNewToCond.Add(j.NameColNewLabel, new Tuple<string, string>(j.NameColA, j.NameColB));

        }

        public Condition String(string dataAColName, string dataBColName, Condition.StringMethod method, string newColLabel = null)
        {

            //Debug.Assert(canList.CheckDataAHasColWithName(dataAColName), "error 2342346534 col name not found");
            //Debug.Assert(canList.CheckDataBHasColWithName(dataBColName), "error 2342355456 col name not found");

            //TODO also test types => string = string etc.

            //we save the have 
            Condition j = new Condition
            {
                Mode = Condition.CompareType.String,
                NameColA = dataAColName,
                NameColB = dataBColName,
                MyStringMethod = method,
                //j.threshold = threshold;
                NameColNewLabel = newColLabel
            };

            j.SetNewColName();


            //conditionList.Add(j);
            //RememberConditionNamesForTranslation(j);

            AddExisting(j);

            return j;
        }

        //public void SortByScoreWeight()
        //{
        //    conditionList.Sort(); //do default sort
        //}

        public IEnumerator<Condition> GetEnumerator()
        {
            return conditionList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Condition GetConditionByIndex(byte index)
        {
            return this.conditionList.ElementAt((int)index);
        }

        public byte GetConditionIndexByName(string name)
        {
            byte index = byte.MaxValue;
            byte counter = 0;
            foreach (Condition con in this)
            {
                if (string.Compare(con.NameColNewLabel, name) == 0) return counter;
                counter += 1;
            }

            return index;
        }
    }
}
