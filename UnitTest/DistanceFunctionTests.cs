﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using RecordLinkageNet.Core;
using System;
using RecordLinkageNet.Core.Distance;


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

            Assert.AreEqual(1, "foo".DamerauLevenshteinDistance( "foo1"), "TestDamerauLevenshteinDistance error no correct output");
            Assert.AreEqual(1, "foo".DamerauLevenshteinDistance("1foo"), "TestDamerauLevenshteinDistance error no correct output");
            Assert.AreEqual(2, "foo".DamerauLevenshteinDistance("23foo"),"TestDamerauLevenshteinDistance error no correct output");
            Assert.AreEqual(8, "foo".DamerauLevenshteinDistance("funkyfoobar"), "TestDamerauLevenshteinDistance error no correct output");
        }
     

        [TestMethod]
        public void TestShannonEntropy()
        {
            //ref of results are: https://codereview.stackexchange.com/questions/868/calculating-entropy-of-a-string

            Assert.AreEqual(4.00f, ShannonEntropyDistance.ShannonEntropy("abcdefghijklmnop".AsMemory()), "error wrong entropy calculation");
            Assert.AreEqual(3.18f, ShannonEntropyDistance.ShannonEntropy("Hello, World!".AsMemory()), 0.01f, "error wrong entropy calculation");
            Assert.AreEqual(2.85f, ShannonEntropyDistance.ShannonEntropy("hello world".AsMemory()), 0.01f, "error wrong entropy calculation");
            Assert.AreEqual(0.0f, ShannonEntropyDistance.ShannonEntropy("aaaa".AsMemory()), "error wrong entropy calculation");

        }
        [TestMethod]
        public void TestShannonEntropyDistance()
        {
            string aaaa = "aaaa";
            string aabb = "aabb";
            double result = aaaa.EntropyDistance(aabb);
            Assert.AreEqual(1.0f, result, "error failed distance calc"); 
        }


        [TestMethod]
        public void TestJaroWinklerMemory()
        {
            char[] arr1 = new char[3] {'b', 'a', 'r'};
            ReadOnlyMemory<Char> bar = new ReadOnlyMemory<char> (arr1);
            char[] arr2 = new char[3] { 'f', 'o', 'o' };
            ReadOnlyMemory<Char> foo = new ReadOnlyMemory<char>(arr2);
            char[] arr3 = new char[4] { 'b', 'a', 'r' ,'1'};
            ReadOnlyMemory<Char> bar1 = new ReadOnlyMemory<char>(arr3);
            char[] arr4 = new char[4] { 'f', 'o', 'o','1' };
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

            Assert.AreEqual(0, "".JaroWinklerSimilarity(""), "JaroWinklerSimilarity no correct output");

            Assert.AreEqual(1, "bar".JaroWinklerSimilarity("bar"), "JaroWinklerSimilarity no correct output");
            Assert.AreEqual(0, "foo".JaroWinklerSimilarity("bar"), "JaroWinklerSimilarity no correct output");
            Assert.AreEqual(0.5, "foo1".JaroWinklerSimilarity("bar1"), "JaroWinklerSimilarity no correct output");

            string s1="jellyfish";
            string s2 = "smellyfish";
            double result = 0.8962962962962964;
            Assert.AreEqual(result, s1.JaroWinklerSimilarity(s2), "JaroWinklerSimilarity no correct output");
       
        }
    }
}
