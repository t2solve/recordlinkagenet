using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare
{
    public abstract class CompareState
    {
        protected CompareProcess process;
        protected DateTime time;
        protected string name;

        public CompareState()
        {
            time =  DateTime.UtcNow;
        }

        public void SetContext(CompareProcess process)
        {
            this.process = process;

        }

        public abstract void Save();

        public abstract void Load();


    }
}
