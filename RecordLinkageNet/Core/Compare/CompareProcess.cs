using RecordLinkageNet.Core.Compare.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public class CompareProcess
    {
        private string storageFolder = String.Empty; 
        private CompareState stateNow = null;

        public CompareProcess()
        {
            this.stateNow = new StateInit();
            this.storageFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
        }

        public string ProcessStorageFolder { get => storageFolder; set => storageFolder = value; }

        public void TransitionTo(CompareState state)
        {
            Trace.WriteLine($"Context: Transition to {state.GetType().Name}.");
            this.stateNow = state;
            this.stateNow.SetContext(this);
        }
    }
}
