# Overview


**aim:** opensource library which offers help to compare of datasets (csv, database tables,classes) in a memory-limited enviroment 

**status:** non-stable but useable , ( no nuget package yet)

**license** BSD 2-Clause

This project is a c# port of the super usefull python package [recordlinkage](https://recordlinkage.readthedocs.io/en/latest/about.html).
Besides it tries to use the effectiv parts of the c# language (e.g. linq, ml-net).

## plattforms:
all plattform which supports [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
so:

- Linux
- MacOs
- Windows
 |

## minimal examples
This project should look and feel like using the pyhton equivalent:
```c# 
//we load the ml context for data parsing
MLContext mlContext = new MLContext();

//load data howToDo wit ml.net
//https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/load-data-ml-net
IDataView dataA = mlContext.Data.LoadFromEnumerable<TestDataPerson>(inMemoryCollection);
IDataView dataB = mlContext.Data.LoadFromTextFile<TestDataPerson>("file.csv", separatorChar: ';', hasHeader: true);

//create index
RecordLinkageNet.Core.Index indexer = new RecordLinkageNet.Core.Index();
indexer.full();

//create candidate list
CandidateList can = indexer.index(dataA, dataB);
Compare compare = new Compare(can);

//          name in dataA , name in dataB
compare.Exact("PostalCode", "PostalCode");
compare.String("NameFirst", "NameFirst", CompareCondition.StringMethod.JaroWinklerSimilarity, 0.9f);
compare.String("NameLast", "NameLast", CompareCondition.StringMethod.JaroWinklerSimilarity);
bool success = compare.Compute();
if (success)
{
    ResultSet res = compare.PackedResult;
    res.PrintReadableDebug(5);//print debug output

    //define lamda function: how to sum up the score
    Func<float, float, float, float> sumUpScore = (con1, con2, con3) => con1 + con2 + con3;

    int amountDataValues = res.indexList.Count;
    float[] scoreValue = new float[amountDataValues];
    //we dot sum up for all 
    Parallel.For(0, amountDataValues,
            i => scoreValue[i] = sumUpScore(res.data[i, 0], res.data[i, 1], res.data[i, 2]));

    float scoreMinThreshold = 2.5f; //define what is min score
    //we print the index if do have the same 
    for (int i = 0; i < amountDataValues; i++) //TODO make this Parallel ? 
    {
        if (scoreValue[i] > scoreMinThreshold)
        {
            Console.WriteLine("almost same same, but different:");
            Tuple<int, int> indexTup = res.indexList[i];
            Console.WriteLine("<indexA,indexB>:" + indexTup.Item1 + "," + indexTup.Item2);
        }
    }
}
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
The distances metrics are well tested with results from python lib [jellfish](https://github.com/jamesturk/jellyfish).

For further reading or an executable example, please take a look into the 
other project RecordLinkageNetExamples


## structure:

| folder | description |
| ----------- | ----------- |
| RecordLinkageNet | c# library code  |
| UnitTest | test for the lib 

## thanks to
- [jamesturk](https://github.com/jamesturk) for [jellfish](https://github.com/jamesturk/jellyfish) and his c implementation of string metrics
- [jeff-atwood](https://codereview.stackexchange.com/users/136/jeff-atwood) for [Shannon Entropy](https://codereview.stackexchange.com/a/909)
- [wickedshimmy](https://gist.github.com/wickedshimmy) and [joannaksk](https://gist.github.com/joannaksk) for [basic Damerau Levenshtein Distance](https://gist.github.com/joannaksk/da110f9b05ff38d3f4ea4d149a0eb55e)