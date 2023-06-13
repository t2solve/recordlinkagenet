using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Compare.State
{
    [DataContract(Name = "CompareState", Namespace = "RecordLinkageNet")]
    public abstract class CompareState
    {
        [IgnoreDataMember]
        protected CompareProcess process;
        [DataMember(Name = "Time")]
        protected DateTime time;
        [DataMember(Name = "Name")]
        protected string name;

        public string Name { get => name; set => name = value; }
        public DateTime Time { get => time; set => time = value; }

        public CompareState()
        {
            time = DateTime.UtcNow;
            name = "foo";
        }

        public void SetContext(CompareProcess process)
        {
            this.process = process;
        }

        public abstract bool Save();

        public abstract bool Load();

        public string GetSpecificFileName()
        {
            string fileName = string.Empty;

            fileName = Path.Combine(process.ProcessStorageFolder,
                "state-" + name + ".xml");

            //if (File.Exists(fileName))
            //{
            //    Trace.WriteLine("warning 2938938 will override file: " + fileName);
            //}

            return fileName;
        }

        public bool CheckFileIsPresent(string file)
        {

            if (file == null)
                return false;

            if (file == String.Empty)
                return false;

            //we do exists check 
            if (!File.Exists(file))
            {
                Trace.WriteLine("error 298392839898, file  " + file + " not found");
                return false;
            }

            return true;
        }
    }
}
