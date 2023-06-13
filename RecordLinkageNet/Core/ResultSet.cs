using Microsoft.ML;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Core.Transpose;
//using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DataRow = RecordLinkageNet.Core.Data.DataRow;

namespace RecordLinkageNet.Core
{

    /// <summary>
    /// no Dataframe in ml net yet starts with 2.0 ?, so we use a own resultset structure
    /// </summary>
    //[ProtoContract]
    [DataContract(Name = "ResultSet", Namespace = "DataContracts")]
    public class ResultSet
    {
        [DataMember(Name = "MatchingScoreCompareResulList")]
        //public Dictionary<IndexPair, int> indexMap = null;
        public List<MatchingScore> MatchingScoreCompareResultList = new List<MatchingScore>();
        [DataMember(Name = "GroupedMatchingResultList")]
        ResultGroup GroupedMatchingResultList = new ResultGroup();

        private void SortAllCandidateListByTotalScoreTopDown(ResultGroup resultGroupCandidates)
        {
            if (resultGroupCandidates == null)
                return; 

            foreach(MatchingResultGroup c in resultGroupCandidates.Data)
            {
                c.CandidateList.Sort();
            }
        }

        private void CalcEuclidianDistanceToTop()
        {
            if (GroupedMatchingResultList == null)
                return;

            foreach (MatchingResultGroup group in GroupedMatchingResultList.Data)
            {
                MatchingScore topScore = group.GetTopScore();

                if (topScore == null)
                {
                    Trace.WriteLine("warning 238928398: found empty result group, will be ignored"); 
                    continue;
                }
                //we calc all distance
                foreach( MatchingScore score in group.CandidateList)
                {
                    float distance = topScore.CalcEuclidianDistanceOverAllMatchScores(score);
                    group.CandidateListDistancesToTopScore.Add(distance);
                }

            }
        }

        public ResultGroup GroupResultAsMatchingBlocks(MatchingResultGroup.GroupingDirection direction = MatchingResultGroup.GroupingDirection.IndexAIsKeyForGroup)
        {

            //group via linq
            //everything in one 
            ResultGroup resultGroupCandidates = new ResultGroup();
            if (direction == MatchingResultGroup.GroupingDirection.IndexAIsKeyForGroup)
            {
                resultGroupCandidates.Data = (from p in MatchingScoreCompareResultList
                         group p by p.Pair.aIdx into g
                         select new MatchingResultGroup
                         {
                             MyGroupingDirection = direction,
                             IndexKey = g.Key,
                             CandidateList = g.ToList()
                         }).ToList();
                //select new { KeyId = g.Key, Set = g.ToList() };
            }
            if (direction == MatchingResultGroup.GroupingDirection.IndexBIsKeyGorGroup)
            {
                resultGroupCandidates.Data = (from p in MatchingScoreCompareResultList
                                         group p by p.Pair.bIdx into g
                                         select new MatchingResultGroup
                                         {
                                             MyGroupingDirection = direction,
                                             IndexKey = g.Key,
                                             CandidateList = g.ToList()
                                         }).ToList();
            }
            //save ref global
            GroupedMatchingResultList = resultGroupCandidates;

            //TODO seperate this steps
            //we sort
            SortAllCandidateListByTotalScoreTopDown(resultGroupCandidates);
            //we calc distance 
            CalcEuclidianDistanceToTop();

            //we save it 
            return GroupedMatchingResultList; 
        }

        public ResultGroup FilterByMinScore(Configuration config, float scoreMinValueRelative)//, float distanceMinValueRelative )
        {

            //TODO move  distance separate filter
            //

            ResultGroup selectedList =  new ResultGroup();
            if (GroupedMatchingResultList == null)
                return null;

            //we get a head 
            if (GroupedMatchingResultList.Data.Count == 0)
                return null; 
            MatchingResultGroup firstGroup = GroupedMatchingResultList.Data[0];

            //we get a head 
            if (firstGroup.CandidateList.Count == 0)
                return null;

            MatchingScore scoreFirst = firstGroup.CandidateList[0];

            //we do calc max 
            float scoreMinValueAbs = 0.4f;//TODO fix
            //config.ScoreProducer.CalcScoreAbsoluteValueFromPercentage(scoreMinValueRelative);

            //we do a abs distance calc //TODO use function
            float distanceMinValueAbs = scoreFirst.MatchScoreColumnByName.Keys.Count * byte.MaxValue; 

            ////filter for distance 
            //var listDistance = GroupedMatchingResultList.Where(m => m.CandidateListDistances.Count > 0)
            //    .Select(m => new MatchingResultGroup
            //    {
            //        GroupingDirection = m.GroupingDirection,
            //        CandidateList = m.CandidateList,
            //        CandidateListDistances = m.CandidateListDistances.Where(u => u > scoreMinValue).ToList()
            //    }).ToList();

            ////we filter for the score
            ////filter for distance 
            //var  listScore = GroupedMatchingResultList.Where(m => m.CandidateListDistances.Count > 0)
            //    .Select(m => new MatchingResultGroup
            //    {
            //        GroupingDirection = m.GroupingDirection,
            //        CandidateList = m.CandidateList.Where(u => u > scoreMinValue).ToList()
            //        CandidateListDistances = m.CandidateListDistances.Where(u => u > scoreMinValue).ToList()
            //    }).ToList();

            //TODO do it in linq
            foreach ( MatchingResultGroup groupOld in GroupedMatchingResultList.Data )
            {
                MatchingResultGroup newGroup = new MatchingResultGroup();
                bool flagFoundAtLeastOneElement = false;
                //we do a member copy 
                newGroup.IndexKey = groupOld.IndexKey;
                newGroup.MyGroupingDirection = groupOld.MyGroupingDirection;

                //and item pairs if needed
                for (int i=0;i<groupOld.CandidateList.Count; i++)
                {
                    //new test
                    float score = groupOld.CandidateList[i].ScoreTotal;
                    //float distance = groupOld.CandidateListDistancesToTopScore[i];
                    if (score >= scoreMinValueAbs )// && distance <= distanceMinValueAbs) //TODO: check again assumption
                    {
                        flagFoundAtLeastOneElement = true;
                        newGroup.CandidateList.Add(groupOld.CandidateList[i]);
                        newGroup.CandidateListDistancesToTopScore.Add(groupOld.CandidateListDistancesToTopScore[i]);

                    }
                 }
                if( flagFoundAtLeastOneElement)
                    selectedList.Data.Add(newGroup);
            }

            return selectedList; 
        }

    }

}
