using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace RecordLinkageNet.Util
{
    public class ClassWriterToXML
    {
        public static bool WriteClassInstanceToXml<T>(T instance, string file, bool overrideIfExist = true)
        {
            bool success = false;

            if (instance == null)
            {
                Trace.WriteLine("error 293893898 instance is null");
                return false;
            }
            if (instance.GetType() != typeof(T))
            {
                Trace.WriteLine("error 344898928 instance is NOT right type");
                throw new ArgumentException("error 344898928");
            }

            try
            {
                var ds = new DataContractSerializer(typeof(T));
                var settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
                using (var w = XmlWriter.Create(file, settings))
                {
                    ds.WriteObject(w, instance);
                }
                success = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 28374823748 during write:  " + e.ToString());
            }
            return success;
        }
    }
}
