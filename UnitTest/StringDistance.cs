using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordLinkageNet.Core.Distance;
using System;

namespace UnitTest
{
    [TestClass]
    public class StringDistance
    {
        [TestMethod]
        public void TestHamming()
        {
            Assert.AreEqual(0, "bar".HammingDistance("bar"), "HammingDistance no correct output");
            Assert.AreEqual(3, "foo".HammingDistance("bar"), "HammingDistance no correct output");
            Assert.AreEqual(3, "foo1".HammingDistance("bar1"), "HammingDistance no correct output");

        }

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
            double value1 = aaaa.ShannonEntropyDistance(aabb);
            Assert.AreEqual(1.0f, value1, "error ShannonEntropyDistance failed distance calc");

            Assert.AreEqual(0, "a".ShannonEntropyDistance("a"), "error ShannonEntropyDistance failed distance calc");
            Assert.AreEqual(1, "a".ShannonEntropyDistance("ab"), "error ShannonEntropyDistance failed distance calc");


            string s1 = "Hello, World!";
            string s2 = "hello world";
            Assert.AreEqual(0.33f, s1.ShannonEntropyDistance(s2), 0.01f, "error ShannonEntropyDistance failed distance calc");


            string s3 = "jellyfish";
            string s4 = "smellyfish";
            double value2 = s3.ShannonEntropyDistance(s4);
            double result2 = 0.025f;
            Assert.AreEqual(result2, value2, 0.001f, "error ShannonEntropyDistance failed distance calc");

            string s5 = "1234567890";
            double value3 = s5.ShannonEntropyDistance(s3);
            double result3 = 0.37f;
            Assert.AreEqual(result3, value3, 0.01f, "error ShannonEntropyDistance failed distance calc");

            double value4 = s5.ShannonEntropyDistance(aaaa);
            double result4 = 3.32f;
            Assert.AreEqual(result4, value4, 0.01f, "error ShannonEntropyDistance failed distance calc");


        }
        [TestMethod]
        public void TestShannonEntropyDistanceNormalized()
        {
            string s1 = "1234567890";
            string s2 = "aaaaa";
            double value1 = s1.ShannonEntropyDistanceNormalizedToRange0To1(s2);
            double result1 = 0f;
            Assert.AreEqual(result1, value1, 0.01f, "error ShannonEntropyDistance failed distance calc");

            string s3 = "jellyfish";
            string s4 = "smellyfish";
            double value2 = s3.ShannonEntropyDistanceNormalizedToRange0To1(s4);
            double result2 = 0.99f;
            Assert.AreEqual(result2, value2, 0.01f, "error ShannonEntropyDistance failed distance calc");
            double value3 = s1.ShannonEntropyDistanceNormalizedToRange0To1(s3);
            double result3 = 0.887f;
            Assert.AreEqual(result3, value3, 0.01f, "error ShannonEntropyDistance failed distance calc");

            double value4 = s1.ShannonEntropyDistanceNormalizedToRange0To1(s1);
            double result4 = 1f;
            Assert.AreEqual(result4, value4, "error ShannonEntropyDistance failed distance calc");

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
        public void TestJaroWinklerSimilarity()
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

        [TestMethod]
        public void TestJaroDistance()
        {
            string s1 = "jellyfish";
            string s2 = "smellyfish";
            double value1 = s1.JaroDistance(s2);
            double result1 = 0.8962962962962964;
            Assert.AreEqual(result1, value1, "JaroDistance no correct output");

            string s3 = "398493898989333";
            double value2 = s3.JaroDistance(s1);
            double result2 = 0.0;
            Assert.AreEqual(result2, value2, "JaroDistance no correct output");

            string s4 = "1";
            double value3 = s4.JaroDistance(s3);
            double result3 = 0.0;
            Assert.AreEqual(result3, value3, "JaroDistance no correct output");

            string s5 = s3;
            double value4 = s5.JaroDistance(s3);
            double result4 = 1.0;
            Assert.AreEqual(result4, value4, "JaroDistance no correct output");



        }

        //generate test for hamming distance normalized 
        [TestMethod]
        public void TestHammingDistanceNormalized()
        {
            double allowedDelta = 0.01f;
            string s1 = "jellyfish";
            string s2 = "smellyfish";
            double value1 = s1.HammingDistanceNormalizedToRange0To1(s2);
            double result1 = 0.09f;
            Assert.AreEqual(result1, value1, allowedDelta, "HammingDistanceNormalized no correct output");

            string s3 = "398493898989333";
            double value2 = s3.HammingDistanceNormalizedToRange0To1(s1);
            double result2 = 0.0f;
            Assert.AreEqual(result2, value2, "HammingDistanceNormalized no correct output");

            string s4 = "fish";
            double value3 = s4.HammingDistanceNormalizedToRange0To1(s1);
            double result3 = 0.0f;
            Assert.AreEqual(result3, value3, "HammingDistanceNormalized no correct output");

            string s5 = s4;
            double value4 = s4.HammingDistanceNormalizedToRange0To1(s5);
            double result4 = 1f;
            Assert.AreEqual(result4, value4, "HammingDistanceNormalized no correct output");

            string s6 = "jelly";
            double value5 = s6.HammingDistanceNormalizedToRange0To1(s1);
            double result5 = 0.555f;
            Assert.AreEqual(result5, value5, allowedDelta, "HammingDistanceNormalized no correct output");

            string s7 = "jellyfis";
            double value6 = s7.HammingDistanceNormalizedToRange0To1(s1);
            double result6 = 0.88f;
            Assert.AreEqual(result6, value6, allowedDelta, "HammingDistanceNormalized no correct output");
        }

        //test methode for hamming distance normalized
        [TestMethod]
        public void TestDamerauLevenshteinDistanceNormalized()
        {
            double allowedDelta = 0.01f;
            var s1 = "jellyfish";
            var s2 = "smellyfish";
            double value1 = s1.DamerauLevenshteinDistanceNormalizedToRange0To1(s2);
            double result1 = 0.8f;
            Assert.AreEqual(result1, value1, allowedDelta, "DamerauLevenshteinDistanceNormalized no correct output");

            var s3 = "123456789";
            double value2 = s3.DamerauLevenshteinDistanceNormalizedToRange0To1(s1);
            double result2 = 0.0f;
            Assert.AreEqual(result2, value2, "DamerauLevenshteinDistanceNormalized no correct output");

            var s4 = "fish";
            double value3 = s4.DamerauLevenshteinDistanceNormalizedToRange0To1(s1);
            double result3 = 0.444f;
            Assert.AreEqual(result3, value3, allowedDelta, "DamerauLevenshteinDistanceNormalized no correct output");

            var s5 = s4;
            double value4 = s4.DamerauLevenshteinDistanceNormalizedToRange0To1(s5);
            double result4 = 1f;
            Assert.AreEqual(result4, value4, "DamerauLevenshteinDistanceNormalized no correct output");

        }


    }
}
