using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuadraticSieve2
{
    public interface SieveReqGen
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        bool NextSegment(out long start, out long len, out SieveInitInfo req);
    }

    class SinglePolynomialGen : SieveReqGen
    {
        private SieveInitInfo req;
        private long currIdx;
        private long bs;

        public SinglePolynomialGen(SieveInitInfo req, long len)
        {
            this.req = req;
            bs = len;
        }

        public bool NextSegment(out long start, out long len, out SieveInitInfo req)
        {
            len = bs;
            req = this.req;
            start = currIdx;
            currIdx += bs;
            return true;
        }
    }

    partial class QuadraticSieve
    {
        public static SolveRequest MultiThreadSieve(long smoothsNeeded, int B, SieveReqGen reqGen, int numThreads, int milliswait)
        {
            SolveRequest ret = new SolveRequest(B, smoothsNeeded);

            Thread[] t = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                t[i] = new Thread(() => SingleThread(reqGen, smoothsNeeded, ret));
                t[i].Start();

            }

            bool flag = true;

            while (flag)
            {
                flag = false;
                for (int i = 0; i < numThreads; i++)
                {
                    if (t[i].IsAlive)
                    {
                        flag = true;
                    }
                }
                Thread.Sleep(milliswait);
            }

            return ret;
        }

        private static void SingleThread(SieveReqGen gen, long smoothsNeeded, SolveRequest o)
        {
            long start;
            long len;
            SieveInitInfo req;

            while (true)
            {
                if (!gen.NextSegment(out start, out len, out req))
                    break;
                
                var currResult = new SieveResult();
                Sieve(req, currResult, start, len);
                lock (o)
                {
                    if (!o.AddDataToSolveRequestMax(currResult))
                        break;
                    Console.WriteLine("{0} out of {1} smooth numbers found...", o.L, smoothsNeeded);
                }
            }
        }
    }
}
