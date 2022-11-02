using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecordLinkageNet.Core.Distance;

namespace RecordLinkageNet.Core
{
    public class CompareTaskFactory
    {
        public static Task<Tuple<long, float>> CreateStringCompare(long jobId, CompareCondition job, ReadOnlyMemory<char> textValueA, ReadOnlyMemory<char> textValueB)
        {
            Task<Tuple<long, float>> task = null;

            //we create a compare task 
            switch (job.MyStringMethod)
            {
                case CompareCondition.StringMethod.Exact:
                    task = new Task<Tuple<long, float>>(() =>
                        CompareExact(jobId, textValueA, textValueB));
                    break;
                case CompareCondition.StringMethod.HammingDistance:
                    task = new Task<Tuple<long, float>>(() =>
                        CompareFunctionOneOrZero(
                            Hamming.HammingDistance, jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.JaroDistance:
                    task = new Task<Tuple<long, float>>(() =>
                        CompareFunctionOneOrZero(
                            JaroWinkler.JaroDistance, jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.JaroWinklerSimilarity:
                    task = new Task<Tuple<long, float>>(() =>
                       CompareFunctionOneOrZero(
                           JaroWinkler.JaroWinklerSimilarity, jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.DamerauLevenshteinDistance:
                    task = new Task<Tuple<long, float>>(() =>
                        CompareFunctionOneOrZero(
                           DamerauLevenshtein.DamerauLevenshteinDistance, jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.ShannonEntropyDistance:
                    task = new Task<Tuple<long, float>>(() =>
                       CompareFunctionOneOrZero(
                          ShannonEntropy.ShannonEntropyDistance, jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.MyCustomizedDistance:
                    task = new Task<Tuple<long, float>>(() =>
                        CompareFunctionOneOrZero(
                           CustomizedDistance.MyCustomizedDistance, jobId, textValueA, textValueB, job.threshold));
                    break;

                case CompareCondition.StringMethod.Unknown:
                default:
                    Trace.WriteLine("error 232323298 compare method not implemented");
                    throw new NotImplementedException("compare method not implemted");
                    break;
            }

            return task;
        }

        private static Tuple<long, float> CompareExact(long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b)
        {
            Tuple<long, float> result = Tuple.Create(id, 0.0f);  //default not eqaul

            //case both empty 
            if(a.Length==0 && b.Length==0)
            {
                result = Tuple.Create(id, 0.0f);
                return result; //abort 
            }

            //compare seqs
            if (a.Span.SequenceEqual(b.Span))
            {
                //yes
                result = Tuple.Create(id, 1.0f);
            }
            return result;
        }

        private static Tuple<long, float> CompareFunctionOneOrZero(Func<ReadOnlyMemory<char>, ReadOnlyMemory<char>, double> action, long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //calc
            {
                float value = (float)action.Invoke(a, b);
                if (threshold >= 0.0)//we do have a thresold
                {
                    if (value >= threshold)
                        value = 1.0f;
                    else
                        value = 0.0f;
                }
                result = Tuple.Create(id, value);
            }

            return result;
        }

    }
}