using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core;
using RecordLinkageNet.Core.Compare;
using RecordLinkageNet.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static RecordLinkageNet.Core.WorkScheduler;

namespace UnitTest
{
    [TestClass]
    public class TestConfig
    {
        [TestMethod]
        public async Task TestConfigEstiamte()
        {
            int amountTestValue = 10000; 
            var testDataA = TestDataGenerator.CreateTestPersons(amountTestValue);
            DataTable tabA = new DataTable();

            //TODO change to generic containter add
            // like tab.AddListAndCreate

            tabA.AddDataClassAsColumns(new TestDataPerson(), testDataA.Count);
            foreach (TestDataPerson p in testDataA)//we add all cells 
            {
                tabA.AddRow(p);
            }

            ConditionList con = new ConditionList();
            con.String("NameLast", "NameLast", Condition.StringMethod.Exact);

            con.String("NameFirst", "NameLast", Condition.StringMethod.Exact);

            con.String("Street", "Street", Condition.StringMethod.Exact);
            con.String("PostalCode", "PostalCode", Condition.StringMethod.Exact);

            var index = new RecordLinkageNet.Core.Compare.Index();
            index.Create(tabA);

            Configuration config = new Configuration();
            config.Index = index;
            config.ConditionList = con; 

            WorkScheduler workScheduler = new WorkScheduler(config);
            workScheduler.EstimateWork();

           //var options = new DataflowBlockOptions() 
            // Define the mesh.
            var queue = new BufferBlock<JobSet>();

            // Start the producer and consumer.
            WorkScheduler.ProduceCompareJobs(queue,config);
            int processorCount = Environment.ProcessorCount;
            var consumerOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = processorCount
            };

//    https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-implement-a-producer-consumer-dataflow-pattern

            List<ResultSet> results = new List<ResultSet>(); 
            var consumer = new ActionBlock<JobSet>(x => results.Add(WorkScheduler.ProcessJob(x)), consumerOptions);
            queue.LinkTo(consumer, new DataflowLinkOptions { PropagateCompletion = true, });

            //old: var consumer = WorkScheduler.Consume(queue);

            // Wait for everything to complete.
            await Task.WhenAll(consumer.Completion, queue.Completion);

            // Ensure the consumer got what the producer sent (in the correct order).
            //var results = await consumer;
            Trace.WriteLine(results);
            int foobar = 0;
            foreach (ResultSet rs in results)
                foobar += rs.indexMap.Count;

            Assert.IsTrue(foobar == (amountTestValue * amountTestValue),"wrong amount of results");
           
        }
    }
}
