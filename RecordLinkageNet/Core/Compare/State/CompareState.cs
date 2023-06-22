using RecordLinkageNet.Util;
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
    public abstract class CompareState
    {
        public enum Type //TODO should i use this ?
        {
            Uknown,
            Init,
            Configuration,
            Process,
            Evaluate,
            Export,
            End
        }

        protected CompareProcess process;
        [DataMember(Name = "Time")]
        protected DateTime time;
        [DataMember(Name = "Name")]
        protected string name;
        [DataMember(Name = "Type")]
        protected Type type;

        public string Name { get => name; set => name = value; }
        public DateTime Time { get => time; set => time = value; }
        public Type StateType { get => type; set => type = value; }

        public CompareState()
        {
            time = DateTime.UtcNow;
            name = "BaseClass";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name);
        }

        public void SetContext(CompareProcess process)
        {
            this.process = process;
        }

        public abstract bool Save();

        public abstract bool Load();

        protected string GetSpecificFileName()
        {
            string fileName = string.Empty;

            fileName = Path.Combine(process.ProcessStorageFolder,
                "State-" + name + ".xml");

            //if (File.Exists(fileName))
            //{
            //    Trace.WriteLine("warning 2938938 will override file: " + fileName);
            //}

            return fileName;
        }

        protected string GetFileNameWithPath(string f)
        {
            return Path.Combine(this.process.ProcessStorageFolder, f);
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
        protected bool LoadDefaultDataMemeber<T> ( out T data)
        {
            bool success = false;
            string file = GetSpecificFileName();
            data = default(T);
            
            if (!CheckFileIsPresent(file))
            {
                Trace.WriteLine("error 2523522132 during read file");
                return success;
            }
            try
            {

                success = ClassReaderFromXML.ReadClassInstanceFromXml(out data, file);

                if (data != null && success)
                {
                    success = true;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 5672154574 during read xml:  " + e.ToString());
            }
            return success;
        }

        public  bool SaveDefaultDataMemeber<T>(T member)
        {
            if (member == null)//shortcut nothing to save so we dont do it
                return true;

            bool success = false;
            if (member != null)
            {
                try
                {
                    string file = GetSpecificFileName();
                    success = ClassWriterToXML.WriteClassInstanceToXml(member, file);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("error 3453463436674 during write:  " + e.ToString());
                }
            }

            return success;
        }

        //protected void CopyAllMyProperties<T>()
        //{
        //    //TODOhttps://stackoverflow.com/questions/8181484/copy-object-properties-reflection-or-serialization-which-is-faster
        //    //var newPerson = Activator.CreateInstance<Person>();
        //    //var fields = newPerson.GetType().GetFields(BindingFlags.Public
        //    //    | BindingFlags.Instance);
        //    //foreach (var field in fields)
        //    //{
        //    //    var value = field.GetValue(person);
        //    //    field.SetValue(newPerson, value);
        //    //}
        //}
    }
}
