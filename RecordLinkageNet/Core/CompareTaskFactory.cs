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
                case CompareCondition.StringMethod.HammingDistance:
                    task = new Task<Tuple<long, float>>(() =>
                        CompareFunctionOneOrZero(Hamming.HammingDistance, jobId, textValueA, textValueB, job.threshold));

                    //task = new Task<Tuple<long, float>>(() => CompHammingDistance(jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.JaroDistance:
                    task = new Task<Tuple<long, float>>(() => CompJaroDistance(jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.JaroWinklerSimilarity:
                    task = new Task<Tuple<long, float>>(() => CompJaroWinklerSim(jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.DamerauLevenshteinDistance:
                    task = new Task<Tuple<long, float>>(() => CompCompDamerauLevenshteinDistance(jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.Exact:
                    task = new Task<Tuple<long, float>>(() => CompareExact(jobId, textValueA, textValueB));
                    break;
                case CompareCondition.StringMethod.ShannonEntropyDistance:
                    task = new Task<Tuple<long, float>>(() => CompShannonEntropyDistance(jobId, textValueA, textValueB, job.threshold));
                    break;
                case CompareCondition.StringMethod.MyCustomizedDistance:
                    task = new Task<Tuple<long, float>>(() => CompMyCustomDistance(jobId, textValueA, textValueB, job.threshold));
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
            if (a.Span.SequenceEqual(b.Span))
            {
                //yes
                result = Tuple.Create(id, 1.0f);
            }
            return result; 
        }

        //TODO reduce code fo one function with Action<> ??? 
            //jarowinkler job
        private static Tuple<long, float> CompJaroWinklerSim(long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //we calc it 
            {
                float value = (float)a.JaroWinklerSimilarity(b);
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

        private static Tuple<long, float> CompCompDamerauLevenshteinDistance(long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //we calc it 
            {
                float value = (float)a.DamerauLevenshteinDistance(b);
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
        private static Tuple<long, float> CompHammingDistance(long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //we calc it 
            {
                float value = (float)a.HammingDistance(b); //we set here the default
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

        private static Tuple<long, float> CompJaroDistance(long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //we calc it 
            {
                float value = (float)a.JaroDistance(b); //we set here the default
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

        private static Tuple<long, float> CompMyCustomDistance(long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //we calc it 
            {
                float value = (float)a.MyCustomizedDistance(b); //we set here the default
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

        private static Tuple<long, float> CompShannonEntropyDistance(long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //we calc it 
            {
                float value =  (float)a.ShannonEntropyDistance(b);
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


        private static Tuple<long, float> CompareFunctionOneOrZero(Func<ReadOnlyMemory<char>, ReadOnlyMemory<char>, double> action, long id, ReadOnlyMemory<char> a, ReadOnlyMemory<char> b, float threshold)
        {
            Tuple<long, float> result = Tuple.Create(id, -1f);  //default
            //calc
            {
                float value = (float) action.Invoke(a,b);
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