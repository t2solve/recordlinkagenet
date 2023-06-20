using RecordLinkageNet.Core.Compare.State;
using System;
using System.CodeDom;
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
        private CompareState stateLast = null;
        private CompareState stateNow = null;
        private Dictionary<CompareState, DateTime> compareStatesHistory = new Dictionary<CompareState, DateTime>();

        public CompareProcess()
        {

            this.stateNow = new StateInit();
            this.stateNow.SetContext(this);
            this.stateLast = this.stateNow; 

            this.storageFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
        }

        public string ProcessStorageFolder { get => storageFolder; set => storageFolder = value; }

        public void Add(CompareState stateNew)
        {
            //Trace.WriteLine($"Context: Transition to {stateNew.GetType().Name}.");
            this.stateNow = stateNew;
            this.stateNow.SetContext(this);

            //we add to history
            if (compareStatesHistory.ContainsKey(stateNew))
            {
                compareStatesHistory[stateNew] = DateTime.UtcNow;
            }
            else
            {
                compareStatesHistory.Add(stateNew, DateTime.UtcNow);
            }

            //if (stateNew.GetHashCode() == stateLast.GetHashCode()) //we do not have TODO anything
            //    return;

            //if(!stateNew.Save())
            //{
            //    Trace.WriteLine("error 2398989 during save state");
            //    throw new Exception("error 2398989"); 
            //}
            //switch(stateNew.StateType)
            //{
            //    case CompareState.Type.Configuration:
            //        break;
            //    case CompareState.Type.Process:
            //        if(Configuration.Instance.IsValide)
                     
            //    case CompareState.Type.Uknown:
            //    default:
            //        Trace.WriteLine("error 2093929 wrong type");
            //        break;
            //}

        }

        public bool Save()
        {
            bool success = false;

            foreach(var pair in this.compareStatesHistory)
            {
                //we check if save it needed 
                //if (pair.Value == DateTime.MinValue)
                {
                    bool partSuccess = pair.Key.Save();
                    if (!partSuccess)
                    {
                        Trace.WriteLine("warning 394898 error during save state: " + pair.Key.Name);
                        return false; 
                    }
                }
            }
            success = true;

            return success;
        }
    }
}
