using RecordLinkageNet.Core.Compare;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Index = RecordLinkageNet.Core.Compare.Index;

namespace RecordLinkageNet.Core
{
    public class Configuration
    {

        public ConditionList ConditionList { get; set; }
        public Index Index { get; set; }
        //public int AmountCPUtoUse { get; set; }


        //found here
        //https://stackoverflow.com/questions/750574/how-to-get-memory-available-or-used-in-c-sharp
        public float GetRamUsedInMegaByte()
        {
            float memory = 0.0f;
            using (Process proc = Process.GetCurrentProcess())
            {
                // The proc.PrivateMemorySize64 will returns the private memory usage in byte.
                // Would like to Convert it to Megabyte? divide it by 2^20
                memory = proc.PrivateMemorySize64 / (1024 * 1024);
            }
            return memory;
        }
    }
}
