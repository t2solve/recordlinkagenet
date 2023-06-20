using RecordLinkageNet.Core.Compare.State;
using System;
using System.Collections.Generic;
using System.Data;
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
        private Dictionary<CompareState, bool> compareStatesHistory = new Dictionary<CompareState, bool>();

        public CompareProcess()
        {
            this.stateNow = new StateInit();
            this.stateNow.SetContext(this);

            this.storageFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
        }

        public string ProcessStorageFolder { get => storageFolder; set => storageFolder = value; }

        public void TransitionTo(CompareState state)
        {
            Trace.WriteLine($"Context: Transition to {state.GetType().Name}.");
            this.stateNow = state;
            this.stateNow.SetContext(this);

            if (compareStatesHistory.ContainsKey(state))
            {
                compareStatesHistory[state] = false;
            }
            else
            {
                compareStatesHistory.Add(state, false);
            }
        }

        public bool Save()
        {
            bool success = false;

            foreach(var pair in this.compareStatesHistory)
            {
               bool partSuccess = pair.Key.Save();
                if (!partSuccess)
                {
                    Trace.WriteLine("warning 394898 error during save state: " + state.Name); 
                }

            }

            return success;
        }
    }
}
