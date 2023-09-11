using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace RecordLinkageNet.Util
{
    public class ClassReaderFromXML
    {
        public static bool ReadClassInstanceFromXml<T>(out T instance, string file)
        {
            bool success = false;
            instance = default(T);

            if (!File.Exists(file))
            {
                Trace.WriteLine("error 436345435 no file found : " + file);
                throw new ArgumentException("error 436345435");
            }
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                FileStream fs = new FileStream(file, FileMode.Open);
                instance = (T)serializer.ReadObject(fs);
                fs.Close();

                success = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 29838748762764 during read xml:  " + e.ToString());
            }
            return success;
        }
    }
}
