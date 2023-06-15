using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Util;
using Simsala.Tool;
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
    public class StateConfiguration : CompareState
    {
        private bool doLogDataTabA = false;
        private bool doLogDataTabB = false;
        private string defaultNameA = "tabAartefactSpec.xml";
        private string defaultNameB = "tabBartefactSpec.xml";

        public StateConfiguration():base()
        {
            this.Name = "Configuration";
        }

        public bool DoLogDataTabA { get => doLogDataTabA; set => doLogDataTabA = value; }
        public bool DoLogDataTabB { get => doLogDataTabB; set => doLogDataTabB = value; }


        public override bool Load()
        {
            bool success = false;


            //we load tabA 
            string fileASpec = GetFileNameWithPath(defaultNameA);
            DataTableFeather tabA = ReadDataTab(fileASpec);
            
            //we load tabB
            string fileBSpec = GetFileNameWithPath(defaultNameB);
            DataTableFeather tabB = ReadDataTab(fileBSpec);

            if (tabA == null || tabB == null)
            {
                success = true;
            }
            //we set it in any way
            Configuration.Instance.AddIndex(new IndexFeather().Create(tabA, tabB)); 

            return success;
        }

        public DataTableFeather ReadDataTab(string artefactName)
        {
            DataTableFeather tab = null;
            if (CheckFileIsPresent(artefactName))
            {
                DataTableArtefactHelper arte = null;
                LoadArtefact(artefactName, out arte);
                //we check the arfecat
                string fileToLoad = GetFileNameWithPath(arte.RelativFilename);
                if (CheckFileIsPresent(fileToLoad))
                {
                    //we compare the hash
                    if (HashValueFactory.CheckFileHasSha512Value(arte.Sha512HashValue, fileToLoad))
                    {
                
                        bool success = ClassReaderFromXML.ReadClassInstanceFromXml(out tab, fileToLoad);
                        if (success && tab != null)
                            return tab;
                    }
                }

            }
            return tab; 
        }

        public override bool Save()
        {
            bool success1 = false;
            bool success2 = false;
            if (Configuration.Instance.IsValide())
            {
                //TODO check if they are the same
                if(doLogDataTabA)
                {
                    success1 = WriteDataTabAndArtefact(defaultNameA, "tabAdata.sqlite", "tabA",
                        Configuration.Instance.Index.dataTabA);
                }
                if (doLogDataTabB)
                {
                    success2 = WriteDataTabAndArtefact(defaultNameB, "tabBdata.sqlite", "tabB",
                        Configuration.Instance.Index.dataTabB);
                }
            }
            return success1 && success2; 
        }

        private bool WriteDataTabAndArtefact(string artefactName, string relateFileNameSqlite,string tableName,DataTableFeather tab)
        {
            bool success = false;
            DataTableArtefactHelper tabAfact = new DataTableArtefactHelper();
            tabAfact.RelativFilename = relateFileNameSqlite;
            tabAfact.TableName = tableName;
            //we write the table 
            string file = GetFileNameWithPath(tabAfact.RelativFilename);
            if (SqliteWriter.WriteDataFeatherToSqlite(tab,tabAfact.TableName, file))
            {
                Task.Delay(1000);
                tabAfact.Sha512HashValue = HashValueFactory.GetSha512Value(file);
                if (tabAfact.Sha512HashValue != null)
                {
                    //we do write the artefact
                    success = WriteArtefact(artefactName, tabAfact);
                }
                else
                {
                    Trace.WriteLine("erro 23998 not able to sha512 file");
                    return false;
                }
            }
            else
            {
                Trace.WriteLine("error 35235 during write dataTabA");
                return false;
            }
            return success; 
        }

        private string GetFileNameWithPath(string f)
        {
            return Path.Combine(this.process.ProcessStorageFolder,f);
        }

        private bool WriteArtefact(string name , DataTableArtefactHelper a )
        {
            bool success = false; 
            string file = GetFileNameWithPath(name); 
            try
            {
                success = ClassWriterToXML.WriteClassInstanceToXml(a,file);
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 322535234 during write:  " + e.ToString());
            }
            return success;
        }

        private bool LoadArtefact(string file, out DataTableArtefactHelper artefact)
        {
            bool success = false;
            artefact = null; 
            if (!CheckFileIsPresent(file))
            {
                Trace.WriteLine("error 23525345 during read file");
                return success;
            }
            try
            {
                success = ClassReaderFromXML.ReadClassInstanceFromXml(out artefact, file); ;
            }
            catch (Exception e)
            {
                Trace.WriteLine("error 325235 during read xml:  " + e.ToString());
            }
            return success;
        }

        [DataContract(Name = "DataTableArtefactHelper", Namespace = "RecordLinkageNet")]
        private class DataTableArtefactHelper
        {
            [DataMember(Name = "RelativFileName")]
            public string RelativFilename { get; set; } = "";
            [DataMember(Name = "TableName")]
            public string TableName { get; set; } = "";

            [DataMember(Name = "Sha512HashValue")]
            public string Sha512HashValue { get; set; } = "a";
        }


    }
}
