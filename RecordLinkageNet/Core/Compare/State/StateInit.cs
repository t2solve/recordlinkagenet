using RecordLinkageNet.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RecordLinkageNet.Core.Compare.State
{
    public class StateInit : CompareState
    {
        //TODO check why we have this stupid class ???
        private InitArtefact artefact = null; 
        public StateInit() : base()
        {
            this.name = "Init";
            this.type = Type.Init;
            artefact = new InitArtefact();
        }
        public override bool Load()
        {
            bool success = LoadDefaultDataMemeber(out artefact); 
            if(success)
            {
                this.Name = artefact.Name;
                this.Time = artefact.Time;
            }
            return success;
        }

        public override bool Save()
        {
            artefact.Name = this.name;
            artefact.Time = this.Time;
            return SaveDefaultDataMemeber(artefact) ;
        }

        [DataContract(Name = "InitArtefact", Namespace = "RecordLinkageNet")]
        private class InitArtefact
        {
            [DataMember(Name = "Time")]
            public DateTime Time { get; set; }
            [DataMember(Name = "Name")]
            public string Name{ get; set; }
        }

    }
}
