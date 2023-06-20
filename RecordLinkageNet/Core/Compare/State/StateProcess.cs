using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RecordLinkageNet.Core.Compare.State
{

    public class StateProcess : CompareState
    {
        private MatchCandidateList listOfMachtes = null; 

        public StateProcess() : base()
        {
            this.Name = "Process";
            this.type = Type.Process;
        }

        public MatchCandidateList ListOfMachtes { get => listOfMachtes; set => listOfMachtes = value; }

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

                MatchCandidateList si = null;
                success = ClassReaderFromXML.ReadClassInstanceFromXml(out si, file);

                if (si != null && success)
                {
                    listOfMachtes = si; 
                    success = true;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 23623434423 during read xml:  " + e.ToString());
            }
            return success;
        }

        public override bool Save()
        {
            bool success = false; 
            if(listOfMachtes != null) 
            {
                try
                {
                    string file = GetSpecificFileName();
                    success = ClassWriterToXML.WriteClassInstanceToXml(listOfMachtes, file);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("error 544234234 during write:  " + e.ToString());
                }
            }

            return success; 
        }
    }
}
