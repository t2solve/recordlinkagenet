using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System;
using RecordLinkageNet.Core.Distance;
using System.Threading.Tasks;
using RecordLinkageNet.Core.Compare;

namespace UnitTest
{
    [TestClass]
    public class DistanceFunctionTests
    {
        //[TestMethod]
        //public void TestHamming()
        //{
        //    Assert.AreEqual(0, "bar".HammingDistance("bar"), "HammingDistance no correct output");
        //    Assert.AreEqual(3, "foo".HammingDistance("bar"), "HammingDistance no correct output");
        //    Assert.AreEqual(3, "foo1".HammingDistance("bar1"), "HammingDistance no correct output");

        //    //test non even length strings
        //    Assert.AreEqual(3, "foobar".HammingDistance("bar"), "HammingDistance no correct output");
        //    string s1 = "jellyfish";
        //    string s2 = "smellyfish";
        //    double result = 8;
        //    Assert.AreEqual(result, s1.HammingDistance(s2), "HammingDistance no correct output");

        //}

        //[TestMethod]
        //public async Task TestExactStringComparePositive()
        //{

        //    Condition job = new Condition();
        //    //job.Mode = CompareCondition.CompareType.Exact;
        //    job.MyStringMethod = Condition.StringMethod.Exact;

        //    //eve
        //    Task<Tuple<long, float>> taskResult1 = CompareTaskFactory.CreateStringCompare(12, job, "a".AsMemory(), "a".AsMemory());
        //    taskResult1.Start();
        //    await taskResult1;
        //    Assert.AreEqual(1, taskResult1.Result.Item2, "TestExactStringCompare no correct output");


        //}

        //[TestMethod]
        //public async Task TestExactStringCompareEmpty()
        //{
        //    Condition job = new Condition();
        //    job.MyStringMethod = Condition.StringMethod.Exact;
        //    Task<Tuple<long, float>> taskResult2 = CompareTaskFactory.CreateStringCompare(12, job, "".AsMemory(), "".AsMemory());
        //    taskResult2.Start();
        //    await taskResult2;
        //    Assert.AreEqual(0, taskResult2.Result.Item2, "TestExactStringCompare no correct output");
        //}

        //[TestMethod]
        //public async Task TestExactStringCompareNegativ()
        //{
        //    Condition job = new Condition();
        //    job.MyStringMethod = Condition.StringMethod.Exact;
        //    Task<Tuple<long, float>> taskResult3 = CompareTaskFactory.CreateStringCompare(12, job, "a".AsMemory(), "b".AsMemory());
        //    taskResult3.Start();
        //    await taskResult3;
        //    Assert.AreEqual(0, taskResult3.Result.Item2, "TestExactStringCompare no correct output");
        //}
        [TestMethod]
        public void TestHammingJelly()
        {
            Assert.AreEqual(0, "".HammingDistance(""), "HammingDistance no correct output");
            Assert.AreEqual(0, "bar".HammingDistance("bar"), "HammingDistance no correct output");
            Assert.AreEqual(3, "foo".HammingDistance("bar"), "HammingDistance no correct output");
            Assert.AreEqual(3, "foo1".HammingDistance("bar1"), "HammingDistance no correct output");

            //test non even length strings
            //this is true ? no substring match ??  all letter are non distance if string not equal
            Assert.AreEqual(6, "foobar".HammingDistance("bar"), "HammingDistance no correct output");
            Assert.AreEqual(9, "jellyfish".HammingDistance("smellyfish"), "HammingDistance no correct output");
            Assert.AreEqual(9, "jelly".HammingDistance("smellyfish"), "HammingDistance no correct output");

        }

        [TestMethod]
        public void TestDamerauLevenshteinDistance()
        {
            //empty
            Assert.AreEqual(0, "".DamerauLevenshteinDistance(""), "TestDamerauLevenshteinDistance error no correct output");

            //equal
            Assert.AreEqual(0, "foo".DamerauLevenshteinDistance("foo"), "TestDamerauLevenshteinDistance error no correct output");
            //insert
            Assert.AreEqual(1, "foo".DamerauLevenshteinDistance("fooo"), "TestDamerauLevenshteinDistance error no correct output");
            //delete
            Assert.AreEqual(1, "foobar".DamerauLevenshteinDistance("fobar"), "TestDamerauLevenshteinDistance error no correct output");

            Assert.AreEqual(3, "foo".DamerauLevenshteinDistance("bar"), "TestDamerauLevenshteinDistance error no correct output");

            Assert.AreEqual(1, "foo".DamerauLevenshteinDistance("foo1"), "TestDamerauLevenshteinDistance error no correct output");
            Assert.AreEqual(1, "foo".DamerauLevenshteinDistance("1foo"), "TestDamerauLevenshteinDistance error no correct output");
            Assert.AreEqual(2, "foo".DamerauLevenshteinDistance("23foo"), "TestDamerauLevenshteinDistance error no correct output");
            Assert.AreEqual(8, "foo".DamerauLevenshteinDistance("funkyfoobar"), "TestDamerauLevenshteinDistance error no correct output");
        }


        [TestMethod]
        public void TestShannonEntropy()
        {
            //ref of results are: https://codereview.stackexchange.com/questions/868/calculating-entropy-of-a-string

            Assert.AreEqual(4.00f, ShannonEntropy.Calc("abcdefghijklmnop".AsMemory()), "error wrong entropy calculation");
            Assert.AreEqual(3.18f, ShannonEntropy.Calc("Hello, World!".AsMemory()), 0.01f, "error wrong entropy calculation");
            Assert.AreEqual(2.85f, ShannonEntropy.Calc("hello world".AsMemory()), 0.01f, "error wrong entropy calculation");
            Assert.AreEqual(0.0f, ShannonEntropy.Calc("aaaa".AsMemory()), "error wrong entropy calculation");

        }
        [TestMethod]
        public void TestShannonEntropyDistance()
        {
            string aaaa = "aaaa";
            string aabb = "aabb";
            double result = aaaa.ShannonEntropyDistance(aabb);
            Assert.AreEqual(1.0f, result, "error failed distance calc");

            Assert.AreEqual(0, "a".ShannonEntropyDistance("a"), "error failed distance calc");
            Assert.AreEqual(1, "a".ShannonEntropyDistance("ab"), "error failed distance calc");

        }


        [TestMethod]
        public void TestJaroWinklerMemory()
        {
            char[] arr1 = new char[3] { 'b', 'a', 'r' };
            ReadOnlyMemory<Char> bar = new ReadOnlyMemory<char>(arr1);
            char[] arr2 = new char[3] { 'f', 'o', 'o' };
            ReadOnlyMemory<Char> foo = new ReadOnlyMemory<char>(arr2);
            char[] arr3 = new char[4] { 'b', 'a', 'r', '1' };
            ReadOnlyMemory<Char> bar1 = new ReadOnlyMemory<char>(arr3);
            char[] arr4 = new char[4] { 'f', 'o', 'o', '1' };
            ReadOnlyMemory<Char> foo1 = new ReadOnlyMemory<char>(arr4);

            Assert.AreEqual(1, bar.JaroDistance(bar), "JaroDistance no correct output");

            Assert.AreEqual(1, bar.JaroWinklerSimilarity(bar), "JaroWinklerSimilarity no correct output");
            Assert.AreEqual(0, foo.JaroWinklerSimilarity(bar), "JaroWinklerSimilarity no correct output");
            Assert.AreEqual(0.5, foo1.JaroWinklerSimilarity(bar1), "JaroWinklerSimilarity no correct output");

            ReadOnlyMemory<char> s1 = "jellyfish".AsMemory();
            ReadOnlyMemory<char> s2 = "smellyfish".AsMemory();

            double result = 0.8962962962962964;
            Assert.AreEqual(result, s1.JaroWinklerSimilarity(s2), "JaroWinklerSimilarity no correct output");

        }

        [TestMethod]
        public void TestJaroWinkler()
        {

            //compare a set of results with python jellyfish pip package
            //see:
            //https://jamesturk.github.io/jellyfish/

            //TODO double check default behaviour empty empty => wrong not even
            Assert.AreEqual(0, "".JaroWinklerSimilarity(""), "JaroWinklerSimilarity no correct output");

            Assert.AreEqual(1, "bar".JaroWinklerSimilarity("bar"), "JaroWinklerSimilarity no correct output");
            Assert.AreEqual(0, "foo".JaroWinklerSimilarity("bar"), "JaroWinklerSimilarity no correct output");
            Assert.AreEqual(0.5, "foo1".JaroWinklerSimilarity("bar1"), "JaroWinklerSimilarity no correct output");

            string s1 = "jellyfish";
            string s2 = "smellyfish";
            double result = 0.8962962962962964;
            Assert.AreEqual(result, s1.JaroWinklerSimilarity(s2), "JaroWinklerSimilarity no correct output");

        }
    }
}
