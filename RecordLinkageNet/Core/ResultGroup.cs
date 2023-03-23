using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RecordLinkageNet.Core
{
    [XmlRoot("ResultGroupRoot")]
    public class ResultGroup
    {
        [XmlArray("Data")]
        [XmlArrayItem("MatchingResultGroup")]
        public List<MatchingResultGroup> Data = new List<MatchingResultGroup>();
        public ResultGroup()
        {
        }
        public ResultGroup(List<MatchingResultGroup> group)
        {
            Data = group;
        }

        public bool WriteMeToFile(string file)
        {
            bool success = false; 
            try
            {
                var serializer = new XmlSerializer(typeof(ResultGroup));
                var memoryStream = new MemoryStream();
                var streamWriter = XmlWriter.Create(memoryStream, new()
                {
                    Encoding = Encoding.UTF8,
                    Indent = true
                });
                serializer.Serialize(streamWriter, this);
                var result = Encoding.UTF8.GetString(memoryStream.ToArray());

                //we have to rewind the MemoryStream before copying
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    memoryStream.CopyTo(fs);
                    fs.Flush();
                }

                success = true; 

            }
            catch (Exception e)
            {
                Trace.WriteLine("error 2352352646 during write:  " + e.ToString());
            }
            return success; 
        }
    }
}
