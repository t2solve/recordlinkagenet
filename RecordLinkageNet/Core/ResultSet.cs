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

        //public Dictionary<IndexPair, int> indexMap = null;
        public List<MatchingScore> MatchingScoreCompareResulList = new List<MatchingScore>();
        List<MatchingResultGroup> GroupedMatchingResultList = new List<MatchingResultGroup>();

        public enum GroupingDirection
        {
            Unknown,
            IndexAIsKeyForGroup,
            IndexBIsKeyGorGroup,
        }

        public class MatchingResultGroup
        {
            public GroupingDirection GroupingDirection = GroupingDirection.Unknown;
            public uint IndexKey   = uint.MaxValue;
            public List<MatchingScore> CandidateList = new List<MatchingScore>();
            public List<float> CandidateListDistancesToTopScore = new List<float>(); //TODO refactor to dictio is dangerous, when resort 
            public MatchingScore GetTopScore()
            {
                if (CandidateList.Count > 0)
                    return CandidateList.First();
                return null; 
            }
        }

        private void SortAllCandidateListByTotalScoreTopDown(List<MatchingResultGroup> resultGroupCandidates)
        {
            if (resultGroupCandidates == null)
                return; 

            foreach(MatchingResultGroup c in resultGroupCandidates)
            {
                c.CandidateList.Sort();
            }
        }

        private void CalcEuclidianDistanceToTop()
        {
            if (GroupedMatchingResultList == null)
                return;

            foreach (MatchingResultGroup group in GroupedMatchingResultList)
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

        public List<MatchingResultGroup> GroupResultAsMatchingBlocks(GroupingDirection direction = GroupingDirection.IndexAIsKeyForGroup)
        {
            //List<ResultGroupCandidates> resultGroupCandidates = new List<ResultGroupCandidates>();
            //IEnumerable groups;
            //group via linq
            //everything in one 
            List < MatchingResultGroup > resultGroupCandidates = null;
            if (direction == GroupingDirection.IndexAIsKeyForGroup)
            {
                resultGroupCandidates = (from p in MatchingScoreCompareResulList
                         group p by p.Pair.aIdx into g
                         select new MatchingResultGroup
                         {
                             GroupingDirection = direction,
                             IndexKey = g.Key,
                             CandidateList = g.ToList()
                         }).ToList();
                //select new { KeyId = g.Key, Set = g.ToList() };
            }
            if (direction == GroupingDirection.IndexBIsKeyGorGroup)
            {
                resultGroupCandidates = (from p in MatchingScoreCompareResulList
                                         group p by p.Pair.bIdx into g
                                         select new MatchingResultGroup
                                         {
                                             GroupingDirection = direction,
                                             IndexKey = g.Key,
                                             CandidateList = g.ToList()
                                         }).ToList();
            }
            GroupedMatchingResultList = resultGroupCandidates;

            //TODO seperate this steps

            //we sort
            SortAllCandidateListByTotalScoreTopDown(resultGroupCandidates);

            //we calc distance 
            CalcEuclidianDistanceToTop();

            //TODO tests ? 

            //we save it 
            return GroupedMatchingResultList; 
        }

        public List<MatchingResultGroup> FilterByConditon(Configuration config, float scoreMinValueRelative, float distanceMinValueRelative )
        {


            List<MatchingResultGroup> selectedList =  new List<MatchingResultGroup>();
            if (GroupedMatchingResultList == null)
                return null;

            //we get a head 
            if (GroupedMatchingResultList.Count == 0)
                return null; 
            MatchingResultGroup firstGroup = GroupedMatchingResultList[0];

            //we get a head 
            if (firstGroup.CandidateList.Count == 0)
                return null;

            MatchingScore scoreFirst = firstGroup.CandidateList[0];

            //we do calc max 
            float scoreMinValueAbs = config.ScoreProducer.CalcScoreAbsoluteValueFromPercentage(scoreMinValueRelative);

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

            foreach ( MatchingResultGroup groupOld in GroupedMatchingResultList )
            {
                MatchingResultGroup newGroup = new MatchingResultGroup();
                bool flagFoundAtLeastOneElement = false;
                //we do a member copy 
                newGroup.IndexKey = groupOld.IndexKey;
                newGroup.GroupingDirection = groupOld.GroupingDirection;

                //and item pairs if needed
                for (int i=0;i<groupOld.CandidateList.Count; i++)
                {
                    //new test
                    float score = groupOld.CandidateList[i].ScoreTotal;
                    float distance = groupOld.CandidateListDistancesToTopScore[i];
                    if (score >= scoreMinValueAbs && distance <= distanceMinValueAbs)
                    {
                        flagFoundAtLeastOneElement = true;
                        newGroup.CandidateList.Add(groupOld.CandidateList[i]);
                        newGroup.CandidateListDistancesToTopScore.Add(groupOld.CandidateListDistancesToTopScore[i]);

                    }
                 }
                if( flagFoundAtLeastOneElement)
                    selectedList.Add(newGroup);
            }

            return selectedList; 
        }


        //              [a,b]    
        //[ProtoMember(1)]
        //public FLatIndexPair[] indexArray = null;

        ////! we need a own managed structure, because of huge data
        //public HugeMatrix<byte> compareData = null;
        ////[ProtoMember(2)]
        ////public ulong indexCount = 0;
        ////[ProtoMember(3)]
        ////public long indexCountB = 0;
        ////[ProtoMember(4)]
        //public List<string> colNames = null;
        //[ProtoMember(5)]
        //public byte[,] data = null; //compare data

        //save data for fast analayse
        //public float[] scoresValuesAbsolute = null;
        //public float[] scoresValuesRelativToMax = null;


        //public HugeMatrix<byte> data = null;


    }
}
