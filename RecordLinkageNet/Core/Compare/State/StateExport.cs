using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RecordLinkageNet.Core.Compare.State
{
    public class StateExport : CompareState
    {
        private ExportArtifact exportArtifact = null;
        public StateExport() : base()
        {
            this.Name = "Export";
            this.type = Type.Export;
            this.exportArtifact = new ExportArtifact(); 
        }

        public override bool Load()
        {
            return LoadDefaultDataMemeber(out exportArtifact); 
        }

        public override bool Save()
        {
            return SaveDefaultDataMemeber(exportArtifact);
        }

        public string FileName { get => exportArtifact.FileName; set => exportArtifact.FileName = value; }
        public bool FlagDoEmptyRows { get => exportArtifact.FlagDoEmptyRows; set => exportArtifact.FlagDoEmptyRows = value; }
        
        public string GetFileNameAndPathToExport()
        {
            return GetFileNameWithPath(exportArtifact.FileName);
        }

        [DataContract(Name = "ExportArtifact", Namespace = "RecordLinkageNet")]
        private class ExportArtifact
        {
            [DataMember(Name = "FileName")]
            public string FileName { get; set; } = "export.csv";
            [DataMember(Name = "FlagDoEmptyRows")]
            public bool FlagDoEmptyRows { get; set; } = false;

        }
    }


}
