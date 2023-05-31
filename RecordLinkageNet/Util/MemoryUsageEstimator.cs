using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Util
{
    public static class MemoryUsageEstimator
    {
        private static readonly long converionFactorByteToMB = 1048576;
        //amount limit of elements allowed in array, is magic number , found at:
        //https://stackoverflow.com/a/59730621/14105642
        private static readonly long magicNumberMaxElementsInArray = 2146435071;//2 billion

        //found here
        //https://stackoverflow.com/questions/750574/how-to-get-memory-available-or-used-in-c-sharp
        public static float GetRamUsedInMegaByte()
        {
            float memory = 0.0f;
            try
            {
                //TODO check difference GetAmountMemoryWeUseFromGCInMiB
                using (Process proc = Process.GetCurrentProcess())
                {
                    // The proc.PrivateMemorySize64 will returns the private memory usage in byte.
                    memory = proc.PrivateMemorySize64 / (converionFactorByteToMB);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 29382983 during get mem info :" + e.ToString());
            }
            return memory;
        }
        public static bool CheckGCAllowsHugeObjects()
        {
            if (Environment.Is64BitProcess)
            {
                //TODO read app.config ??? and parse manualy ? 
                //we do check the app.config
                //docu:  https://learn.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/gcallowverylargeobjects-element
                //string value = ConfigurationManager.OpenMachineConfiguration();
                //["gcAllowVeryLargeObjects"];
                //Trace.WriteLine("gcAllowVeryLargeObjects: " + value);
                return true;
            }
            else
            {
                Trace.WriteLine("warnig 203989839, not compiled as 64 Bit, disallowed to use gcAllowVeryLargeObjects");
                return false;
            }

        }
        public static bool CheckCreateArrayPossible(long amountElement, int approxmiatedSizeOfElementInBytes )
        {
            long memLimitMaxInMiB = 2000;
            //A) constraint a, limit elements in array
            if (amountElement>=magicNumberMaxElementsInArray)
            {
                Trace.WriteLine("waring 299283 to much elements will not fit in one array, exceeds limit");
                return false; 
            }

            //long memLimitPhysicalInMB = GetPhysicalAvailableMemoryInMiB(); //alias mem left
            long memLimitTotallInMiB = (long) GetRamUsedInMegaByte();// GetTotalMemoryInMiB();
            //long memWeUseInMiB = GetAmountMemoryWeUseFromGCInMiB();
            //TODO repair
            if (CheckGCAllowsHugeObjects())
            {
                memLimitMaxInMiB = memLimitTotallInMiB;
            }

            long ramNeedInByte = amountElement * (long)approxmiatedSizeOfElementInBytes;
            long ramNeedInMB = ramNeedInByte / converionFactorByteToMB;
            //ulong ramNeedInGB = ramNeedInByte / 1073741824;
            //B) more ram than allowed, see gcAllowVeryLargeObjects
            if (ramNeedInMB > memLimitMaxInMiB)
            {
                Trace.WriteLine("warning 234234 try to allocate to much memory");
                return false;
            }
            //C) more than we have
            if (ramNeedInMB > memLimitTotallInMiB)
            {
                Trace.WriteLine("warning 123123234 try to allocate more memory than phyiscal present");
                return false;
            }

            return true;
        }

        public static long GetAmountMemoryWeUseFromGCInMiB()
        {
            //Retrieves the number of bytes currently thought to be allocated
            long amountInBytes =   GC.GetTotalMemory(false);
            return amountInBytes / converionFactorByteToMB; 
        }

        //get system infos found at: 
        // https://stackoverflow.com/a/10028263/14105642

        ////TODO change to make linux useable 
        //[DllImport("psapi.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct PerformanceInformation
        //{
        //    public int Size;
        //    public IntPtr CommitTotal;
        //    public IntPtr CommitLimit;
        //    public IntPtr CommitPeak;
        //    public IntPtr PhysicalTotal;
        //    public IntPtr PhysicalAvailable;
        //    public IntPtr SystemCache;
        //    public IntPtr KernelTotal;
        //    public IntPtr KernelPaged;
        //    public IntPtr KernelNonPaged;
        //    public IntPtr PageSize;
        //    public int HandlesCount;
        //    public int ProcessCount;
        //    public int ThreadCount;
        //}

        //public static long GetPhysicalAvailableMemoryInMiB()
        //{
        //    PerformanceInformation pi = new PerformanceInformation();
        //    if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
        //    {
        //        return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / converionFactorByteToMB)); 
        //    }
        //    else
        //    {
        //        return -1;
        //    }

        //}

        //dismiss is non- functioning
        //public static long GetTotalMemoryInMiB()
        //{
        //    PerformanceInformation pi = new PerformanceInformation();
        //    if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
        //    {
        //        return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / converionFactorByteToMB));
        //    }
        //    else
        //    {
        //        return -1;
        //    }

        //}
    }
}
