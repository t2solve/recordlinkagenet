﻿using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public class MatchCandidate
    {
        private IndexPair idxPair = new IndexPair(); 
        private IScore score = null;

        //TODO why by name ? , is ram inefficient
        //private Dictionary<string, byte> MatchScoreColumnByName = new Dictionary<string, byte>();
        //index 
        //private Dictionary<byte, byte> MatchScoreColumnByConditionIndex = new Dictionary<byte, byte>();
        public IScore GetScore()
        {
            return score; 
        }

        public IndexPair GetIndexPair()
        {
            return idxPair;
        }

    }
}
