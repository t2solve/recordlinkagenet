using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Compare.State;
using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class StateMachine
    {
        private bool CheckFileIsNotEmpyt(string file)
        {
            long length = new FileInfo(file).Length;
            if (length > 0)
                return true;
            return false;
        }
        private void DeleteFileIfExists(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        public void BasicSaveTest()
        {
            CompareProcess process = new CompareProcess();

            //List<CompareState> compareStates = new List<CompareState>();

            //testing vars
            Dictionary<string, CompareState> helper = new Dictionary<string, CompareState>();
            string fileNameInitState = Path.Combine(Environment.GetFolderPath(
               Environment.SpecialFolder.LocalApplicationData), "state-init.xml");

            helper.Add(fileNameInitState, new StateInit());

            //we delete all old file 
            foreach (string fileName in helper.Keys)
                DeleteFileIfExists(fileName);

            foreach (var p in helper)
            {
                p.Value.SetContext(process);
                Assert.IsTrue(p.Value.Save());

            }

            //we check the file exists
            foreach (string fileName in helper.Keys)
            {
                Assert.IsTrue(File.Exists(fileName));
                Assert.IsTrue(CheckFileIsNotEmpyt(fileName));
            }

            //we load our class
            var firstEntryInitState = helper.First();
            CompareState stateWeStored = firstEntryInitState.Value;
            DateTime before = stateWeStored.Time;
            //string fileNameInitClass = p.Key;
            CompareState stateWeLoad = new StateInit();
            //stateWeLoad.Name = "foobar";
            stateWeLoad.SetContext(process);
            DateTime after = stateWeLoad.Time;

            int resultDateTimeCompare = DateTime.Compare(before, after);
            Assert.IsTrue(resultDateTimeCompare < 0);//we check order of time
            //we load
            Assert.IsTrue(stateWeLoad.Load());
            //compare 
            Assert.IsTrue(string.Compare(stateWeLoad.Name, "Init") == 0);
            //now its the same
            Assert.IsTrue(DateTime.Compare(before, stateWeLoad.Time) == 0);


        }

        [TestMethod]
        public void TestConfState()
        {
            int amountData = 10;
            DataTableFeather tabA = TestDataGenerator.GenTestData(amountData);

            //build a simle configuration
            ConditionList conList = new ConditionList();
            Condition.StringMethod testMethod = Condition.StringMethod.JaroWinklerSimilarity;
            conList.String("NameFirst", "NameFirst", testMethod);
            conList.String("Street", "Street", testMethod);
            conList.String("PostalCode", "PostalCode", testMethod);
            conList.String("NameLast", "NameLast", testMethod);

            //add weight
            foreach (Condition c in conList)
            {
                c.ScoreWeight = 1.0f;
            }

            Configuration config = Configuration.Instance;
            config.AddIndex(new IndexFeather().Create(tabA, tabA));
            config.AddConditionList(conList);
            config.SetStrategy(Configuration.CalculationStrategy.WeightedConditionSum);

            CompareProcess process = new CompareProcess();
            StateConfiguration conState = new StateConfiguration();
            conState.SetContext(process);
            conState.DoLogDataTabA = true;
            conState.DoLogDataTabB = true;

            Assert.IsTrue(conState.Save());

            //clear old values 
            config.Reset();
            //Assert.IsFalse(config.IsValide());

            Assert.IsTrue(conState.Load());
            Assert.IsTrue(config.IsValide());

        }
    }
}
