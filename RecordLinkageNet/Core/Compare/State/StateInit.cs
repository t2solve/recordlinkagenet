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
    [DataContract(Name = "StateInit", Namespace = "RecordLinkageNet")]
    public class StateInit : CompareState
    {
        public StateInit() : base()
        {
            this.name = "Init";

        }
        public override bool Load()
        {
            bool success = false; 
            string file = GetSpecificFileName();
            if (!CheckFileIsPresent(file))
            {
                Trace.WriteLine("error 92384938498 during read file");
                return success ;
            }
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(StateInit));
                FileStream fs = new FileStream(file, FileMode.Open);
                StateInit si = (StateInit)serializer.ReadObject(fs);
                fs.Close();
                //success = true;
                //
                //we copy all member 
                //TODO use reflection
                this.name = si.name;
                this.time = si.time;
                success = true; 
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 25675452354 during read xml:  " + e.ToString());
            }
            return success; 

        }

        public override bool Save()
        {
            bool success = false;
            string file = GetSpecificFileName();
            try
            {
                var ds = new DataContractSerializer(typeof(StateInit));
                var settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
                using (var w = XmlWriter.Create(file, settings))
                {
                    ds.WriteObject(w, this);
                }
                success = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("error e324234 during write:  " + e.ToString());
            }

            return success;
        }
    }
}
