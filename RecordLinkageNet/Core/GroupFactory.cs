using RecordLinkageNet.Core.Data;
using System;
using System.Diagnostics;
using System.Linq;

namespace RecordLinkageNet.Core
{
    public class GroupFactory
    {
        public enum Type
        {
            Unknown,
            IndexAIsKey,
            IndexBIsKey,
            ColumnValueIsKey
        }
        public static MatchGroupOrderedList GroupResultAsMatchingBlocks(MatchCandidateList matchCandidates, Type groupType, DataCell cell = null)
        {

            MatchGroupOrderedList matchGroupList = new MatchGroupOrderedList();

            //if(cell!=null)
            //{
            //    //TODO
            //}

            switch (groupType)
            {
                case Type.IndexAIsKey:

                    matchGroupList.Data = (from p in matchCandidates
                                           group p by p.GetIndexPair().aIdx into g
                                           select new MatchGroupOrdered()
                                           {
                                               Type = groupType,
                                               Criteria = new GroupCriteriaContainer(g.Key),
                                               Data = g.ToList()
                                           }).ToList();
                    break;
                case Type.IndexBIsKey:
                    matchGroupList.Data = (from p in matchCandidates
                                           group p by p.GetIndexPair().bIdx into g
                                           select new MatchGroupOrdered()
                                           {
                                               Type = groupType,
                                               Criteria = new GroupCriteriaContainer(g.Key),
                                               Data = g.ToList()
                                           }).ToList();
                    break;
                case Type.ColumnValueIsKey:

                    throw new NotImplementedException("error 234576345");
                //break;
                case Type.Unknown:
                default:

                    Trace.WriteLine("error 3984934589 please set criteria");
                    throw new ArgumentException("error 3984934589");
                    //break;
            }

            //if (criteria.CritType == GroupCriteriaContainer.Type.IndexAIsKey)
            //{
            //    matchGroupList.Data = (from p in matchCandidates
            //                           group p by p.GetIndexPair().aIdx into g
            //                           select new MatchGroupOrdered(criteria)
            //                           {
            //                               MyGroupingDirection = direction,
            //                               IndexKey = g.Key,
            //                               Data = g.ToList()
            //                           }).ToList();
            //}
            //if (criteria.CritType == GroupCriteriaContainer.Type.IndexBIsKey)
            //{
            //    matchGroupList.Data = (from p in matchCandidates
            //                           group p by p.GetIndexPair().bIdx into g
            //                           select new MatchGroupOrdered(criteria)
            //                           {
            //                               Data = g.ToList()
            //                           }).ToList();
            //}
            return matchGroupList;
        }
    }
}
