using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare.State
{
    public class StateEvalute : CompareState
    {
        private MatchGroupOrderedList matchGroupOrderedList = null; 
        public StateEvalute() : base()
        {
            this.Name = "Evaluate";
            this.type = Type.Evaluate;
        }

        public MatchGroupOrderedList MatchGroupOrderedList { get => matchGroupOrderedList; set => matchGroupOrderedList = value; }

        public override bool Load()
        {
            bool success = false;
            string file = GetSpecificFileName();
            if (!CheckFileIsPresent(file))
            {
                Trace.WriteLine("error 92384938498 during read file");
                return success;
            }
            try
            {

                MatchGroupOrderedList si = null;
                success = ClassReaderFromXML.ReadClassInstanceFromXml(out si, file);

                if (si != null && success)
                {
                    this.matchGroupOrderedList = si;
                    success = true;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 25675452354 during read xml:  " + e.ToString());
            }
            return success;

        }

        public override bool Save()
        {
            bool success = false;
            string file = GetSpecificFileName();
            try
            {
                success = ClassWriterToXML.WriteClassInstanceToXml(matchGroupOrderedList, file);
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 234623 during write:  " + e.ToString());
            }
            return success;
        }
    }
}