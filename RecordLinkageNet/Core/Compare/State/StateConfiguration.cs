using RecordLinkage.Core;
using RecordLinkageNet.Core.Data;
using RecordLinkageNet.Util;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using static RecordLinkageNet.Core.Configuration;

namespace RecordLinkageNet.Core.Compare.State
{
    //class to save and load confiuration#
    //but data is stored as sqlite file
    public class StateConfiguration : CompareState
    {
        private DataTableFeather tabA = null;
        private DataTableFeather tabB = null;

        private bool doLogDataTabA = false;
        private bool doLogDataTabB = false;
        private string defaultNameA = "tabAartefactSpec.xml";
        private string defaultNameB = "tabBartefactSpec.xml";

        //TODO implement a container or helper for all of this 
        //TODO config  FilterParameterThresholdRelativMinScore
        //TODO NumberTransposeModus .. Strategy not stored right now

        public StateConfiguration() : base()
        {
            this.Name = "Configuration";
            this.type = Type.Configuration;
        }

        public bool DoLogDataTabA { get => doLogDataTabA; set => doLogDataTabA = value; }
        public bool DoLogDataTabB { get => doLogDataTabB; set => doLogDataTabB = value; }
        public DataTableFeather TabA { get => tabA; set => tabA = value; }
        public DataTableFeather TabB { get => tabB; set => tabB = value; }

        public override bool Load()
        {
            bool success = false;
            //clear before load
            Configuration.Instance.Reset();
            //DataTableFeather tabA = null;
            //DataTableFeather tabB= null;

            //we load tabA 
            if (doLogDataTabA)
            {
                string fileASpec = GetFileNameWithPath(defaultNameA);
                this.tabA = ReadDataTab(fileASpec);
                if (tabA == null)
                {
                    Trace.WriteLine("error 343489898 during tabA");

                    //return success; 
                }
            }
            //we load tabB
            if (doLogDataTabB)
            {
                string fileBSpec = GetFileNameWithPath(defaultNameB);
                tabB = ReadDataTab(fileBSpec);
                if (tabB == null)
                {
                    Trace.WriteLine("warning 23948938498 might no tabB loaded");
                }
            }

            //create index
            if (tabA != null && tabB != null)
                Configuration.Instance.AddIndex(new IndexFeather().Create(tabA, tabB));
            else
                Trace.WriteLine("warning 989898 27874 index in config not created");

            //load parameter via wrapper
            ConfigSingeltonWrapper confWrap = new ConfigSingeltonWrapper();
            string fileNameConfigFileAndPath = GetSpecificFileName();
            if (ClassReaderFromXML.ReadClassInstanceFromXml(out confWrap, fileNameConfigFileAndPath))
            {
                //copy by hand 
                Configuration.Instance.AddConditionList(confWrap.ConditionList)
                    .SetNumberTransposeModus(confWrap.NumberTransposeModus)
                    .SetStrategy(confWrap.Strategy)
                    .SetAmountCPUtoUse(confWrap.AmountCPUtoUse)
                    .SetFilterParameterThresholdRelativMinAllowedDistanceToTopScoree(
                   confWrap.FilterParameterThresholdRelativMinAllowedDistanceToTopScore)
                    .SetFilterParameterThresholdRelativMinScore(confWrap.FilterParameterThresholdRelativMinScore);

                //is here to much, what if we want to store a half ready config ?? 
                //if (Configuration.Instance.IsValide())
                //    success = true; 
                //var foobar = Configuration.Instance.IsValide();
                success = true;
            }
            else Trace.WriteLine("error 9384948 during read conifg wrapper");

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

                        tab = SqliteReader.ReadTableFromSqliteFile(fileToLoad, arte.TableName);
                        if (tab != null)
                            return tab;
                        else
                            Trace.WriteLine("error 293898398 during load sqlite data to tab");
                    }
                }

            }
            return tab;
        }

        public override bool Save()
        {
            bool successA = false;
            bool successB = false;
            bool sucConList = false;

            if (Configuration.Instance.IsValide())
            {
                //TODO check if they are the same
                if (doLogDataTabA)
                {
                    successA = WriteDataTabAndArtefact(defaultNameA, "tabAdata.sqlite", "tabA",
                        Configuration.Instance.Index.dataTabA);
                }
                else
                {
                    successA = true;
                }
                if (doLogDataTabB)
                {
                    successB = WriteDataTabAndArtefact(defaultNameB, "tabBdata.sqlite", "tabB",
                        Configuration.Instance.Index.dataTabB);
                }
                else
                {
                    successB = true;
                }
                string fileConfig = GetSpecificFileName();


                ConfigSingeltonWrapper confWrap = new ConfigSingeltonWrapper();
                try
                {
                    //we copy all with same field name
                    //https://stackoverflow.com/questions/8181484/copy-object-properties-reflection-or-serialization-which-is-faster
                    var fields = Configuration.Instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        var value = field.GetValue(Configuration.Instance);
                        field.SetValue(confWrap, value);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("error 2938989 during copy all of the config properties to class " + e.ToString());
                    return false;
                }
                //hand copy 
                confWrap.ConditionList = Configuration.Instance.ConditionList;

                string fileConfigConditionList = GetSpecificFileName();
                sucConList = ClassWriterToXML.WriteClassInstanceToXml(confWrap, fileConfigConditionList);


                //string fileConfigConditionList = GetFileNameWithPath(defaultNameConditionList);
                //sucConList = ClassWriterToXML.WriteClassInstanceToXml(Configuration.Instance.ConditionList, fileConfigConditionList);


            }

            //tr 
            //ClassWriterToXML.WriteClassInstanceToXml(Configuration.Instance, GetFileNameWithPath("foo.xml"));

            //TODO write all parameter of class


            return successA && successB && sucConList;
        }

        private bool WriteDataTabAndArtefact(string artefactName, string relateFileNameSqlite, string tableName, DataTableFeather tab)
        {
            bool success = false;
            DataTableArtefactHelper tabArtefact = new DataTableArtefactHelper();
            tabArtefact.RelativFilename = relateFileNameSqlite;
            tabArtefact.TableName = tableName;
            //we write the table 
            string file = GetFileNameWithPath(tabArtefact.RelativFilename);
            if (SqliteWriter.WriteDataFeatherToSqlite(tab, tabArtefact.TableName, file, true))
            {
                tabArtefact.Sha512HashValue = HashValueFactory.GetSha512Value(file);
                if (tabArtefact.Sha512HashValue != null)
                {
                    //we do write the artefact
                    success = WriteArtefact(artefactName, tabArtefact);
                }
                else
                {
                    Trace.WriteLine("error 23998 not able to sha512 file");
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


        private bool WriteArtefact(string name, DataTableArtefactHelper a)
        {
            bool success = false;
            string file = GetFileNameWithPath(name);
            try
            {
                success = ClassWriterToXML.WriteClassInstanceToXml(a, file);
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
                success = ClassReaderFromXML.ReadClassInstanceFromXml(out artefact, file);
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
            public string Sha512HashValue { get; set; } = "";
        }


        [DataContract(Name = "ConfigSingeltonWrapper", Namespace = "RecordLinkageNet")]
        private class ConfigSingeltonWrapper
        {
            //TODO overthink this !! is doubled code atm, see configuration class
            //data parts 
            [DataMember(Name = "Strategy")]
            public CalculationStrategy Strategy { get; set; } = CalculationStrategy.WeightedConditionSum;
            [DataMember(Name = "ConditionList")]
            public ConditionList ConditionList { get; set; } = null;
            [DataMember(Name = "NumberTransposeModus")]
            public NumberTransposeHelper.TransposeModus NumberTransposeModus { get; set; } = NumberTransposeHelper.TransposeModus.LINEAR;
            //computational 
            [DataMember(Name = "AmountCPUtoUse")]
            public int AmountCPUtoUse { get; set; } = Environment.ProcessorCount;

            //filter parameter 
            [DataMember(Name = "FilterParameterThresholdRelativMinScore")]
            public float FilterParameterThresholdRelativMinScore { get; set; } = 0.7f;//used by FilterRelativMinScore
            [DataMember(Name = "FilterParameterThresholdRelativMinAllowedDistanceToTopScore")]
            public float FilterParameterThresholdRelativMinAllowedDistanceToTopScore { get; set; } = 0.2f;

            //[DataMember(Name = "FilterParameter")]
            //public Dictionary<string, float> FilterParameter = new Dictionary<string, float>();
        }



    }
}
