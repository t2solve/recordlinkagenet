using RecordLinkageNet.Core.Compare.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public class CompareProcess
    {
        private string processsStorageFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private CompareState state = null;

        public CompareProcess()
        {
            this.state = new StateInit();
        }

        public void TransitionTo(CompareState state)
        {
            Console.WriteLine($"Context: Transition to {state.GetType().Name}.");
            this.state = state;
            this.state.SetContext(this);
        }
    }
}
