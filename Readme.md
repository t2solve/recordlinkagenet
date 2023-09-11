![dotnet 6.0 build](https://github.com/t2solve/recordlinkagenet/actions/workflows/dotnet.yml/badge.svg)
![code quality check](https://github.com/t2solve/recordlinkagenet/actions/workflows/codeql.yml/badge.svg)
![code coverage](https://github.com/t2solve/recordlinkagenet/actions/workflows/codecoverage.yml/badge.svg)

# Overview

**aim:** opensource library which offers help to compare datasets (csv, database tables,classes) in a memory-limited environment  

**license** BSD 2-Clause

This project is a pure c# port of the super useful python package [recordlinkage](https://recordlinkage.readthedocs.io/en/latest/about.html).
Besides it tries to use the effective parts of the c# language (e.g. linq, dataflow).

## features
- string comparision with multiple string metrics
- uses own datatable struture to reduce memory footprint (in comparsison to system.data.datatable)
- uses dataflow to reduce memory footprint
- uses parallelism to reduce runtime
- limits: right now every datacell is string

## plattforms:
all plattform which supports [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
so:

- Linux
- MacOs
- Windows

## minimal examples
This project should look and feel like using the pyhton equivalent:
```c#       
//we create some testdata //see UnitTest.TestDataPerson
List<TestDataPerson> testDataPeopleA = new List<TestDataPerson>
{
    new TestDataPerson("Thomas", "Mueller", "Lindetrasse", "Testhausen", "12345"),
    new TestDataPerson("Thomas", "Mueller", "Lindenstrasse", "Testcity", "012345"),
    new TestDataPerson("Thomas", "Müller", "Lindenstrasse", "Testcity", "012345"),
    new TestDataPerson("Tomas", "Müller", "Lindenstroad", "Testhausen", "012342"),
    new TestDataPerson("Tomas", "Müller", "Lindenstroad", "Dorf", "012342")
};
DataTableFeather tabA = TableConverter.CreateTableFeatherFromDataObjectList(testDataPeopleA);

//we load some data from sqlite file
DataTableFeather tabB = RecordLinkageNet.Util.SqliteReader.ReadTableFromSqliteFile("filenameof.sqlite","testtablename");

ConditionList conList = new ConditionList();
Condition.StringMethod testMethod = Condition.StringMethod.JaroWinklerSimilarity;
conList.String("NameFirst", "NameFirst", testMethod);
conList.String("Street", "Street", testMethod);
conList.String("PostalCode", "PostalCode", Condition.StringMethod.Exact);
conList.String("NameLast", "NameLast", testMethod);

//configure comparison
Configuration config = Configuration.Instance;
config.AddIndex(new IndexFeather().Create(tabB, tabA));
config.AddConditionList(conList);
config.SetStrategy(Configuration.CalculationStrategy.WeightedConditionSum);
config.SetNumberTransposeModus(NumberTransposeHelper.TransposeModus.LOG10); ;

//we init a worker
WorkScheduler workScheduler = new WorkScheduler();
var pipeLineCancellation = new CancellationTokenSource();//for optional cancellation
var resultTask = workScheduler.Compare(pipeLineCancellation.Token);

await resultTask;

int amount = resultTask.Result.Count();
```

The project implements mutliple metrics for string comparision as extensions:

- HammingDistance
- DamerauLevenshteinDistance
- JaroDistance
- JaroWinklerSimilarity
- ShannonEntropyDistance

```c# 
using RecordLinkageNet.Core.Distance;
 
var result1 = "foo".HammingDistance("bar");//3
var result2 = "foo".DamerauLevenshteinDistance("bar");//3
var result3 = "foo".JaroWinklerSimilarity("bar");//0
```
The distances metrics are well tested with results from python lib [jellyfish](https://github.com/jamesturk/jellyfish).

## structure:

| folder | description |
| ----------- | ----------- |
| RecordLinkageNet | c# library code  |
| UnitTest | test for the lib  |

## thanks to
- [jamesturk](https://github.com/jamesturk) for [jellyfish](https://github.com/jamesturk/jellyfish) and his c implementation of string metrics
- [jeff-atwood](https://codereview.stackexchange.com/users/136/jeff-atwood) for [Shannon Entropy](https://codereview.stackexchange.com/a/909)
- [wickedshimmy](https://gist.github.com/wickedshimmy) and [joannaksk](https://gist.github.com/joannaksk) for [basic Damerau Levenshtein Distance](https://gist.github.com/joannaksk/da110f9b05ff38d3f4ea4d149a0eb55e)
