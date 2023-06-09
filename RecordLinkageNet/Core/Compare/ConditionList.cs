using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    /// <summary>
    /// Class which does the comparision, internal iterating over DataSet A and B 
    /// </summary>
    [DataContract(Name = "ConditionList", Namespace = "DataContracts")]
    public class ConditionList : IEnumerable<Condition>
    {
        //jobId, result
        //private Queue<Task<Tuple<long, float>>> myTaskList = new Queue<Task<Tuple<long, float>>>(); //really ?? 
        [DataMember(Name = "ConditionListData")]
        private List<Condition> conditionList = new List<Condition>();//condition list 
        private Dictionary<string,Tuple<string,string>> colNamesMappingNewToCond = new Dictionary<string, Tuple<string, string>> ();
     
        public Tuple<string, string> GetOldNamesOfColumn(string name)
        {
            Tuple<string, string> retVal = null;

            if(colNamesMappingNewToCond.ContainsKey(name))
            {
                retVal = colNamesMappingNewToCond[name];    
            }

            return retVal; 
        }
        public void AddExisting(Condition con )
        {
            RememberConditionNamesForTranslation(con);

            if (conditionList.Count() > 250)
                throw new Exception("error 293898 interal maximum for condition reached"); 

            con.ConditionIndex = (byte) this.conditionList.Count();

            this.conditionList.Add(con );
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
            //we remember what we renames 
            if (colNamesMappingNewToCond.ContainsKey(j.NameColNewLabel))
            {
                Trace.WriteLine("error 2932983 doubled condition names are not allowed ");
                throw new ArgumentException();
            }
            colNamesMappingNewToCond.Add(j.NameColNewLabel, new Tuple<string, string>(j.NameColA, j.NameColB));

        }

        public Condition String(string dataAColName, string dataBColName, Condition.StringMethod method,  string newColLabel = null)
        {

            //Debug.Assert(canList.CheckDataAHasColWithName(dataAColName), "error 2342346534 col name not found");
            //Debug.Assert(canList.CheckDataBHasColWithName(dataBColName), "error 2342355456 col name not found");

            //TODO also test types => string = string etc.

            //we save the have 
            Condition j = new Condition();
            

            j.Mode = Condition.CompareType.String;
            j.NameColA = dataAColName;
            j.NameColB = dataBColName;
            j.MyStringMethod = method;
            //j.threshold = threshold;
            j.NameColNewLabel = newColLabel;

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
            foreach(Condition con in this)
            {
                if (string.Compare(con.NameColNewLabel,name)==0) return counter;
                counter += 1;
            }

            return index; 
        }
    }
}
