using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QuadraticSieve2
{
    public partial class QuadraticSieve
    {

        public static void FindQuadraticResidues(SieveInitInfo sievereq)
        {
            
            long L = primeSupply[sievereq.B];

            BigInteger[] V = new BigInteger[L];
            for (int i = 0; i < L; i++)
            {
                V[i] = sievereq.PolyFunction.F(i + sievereq.AStart);
            }

            List<List<long>> tmpPrimeStarts = new List<List<long>>();
            List<long> tmpPrimeIntervals = new List<long>();

            for (int pI = 0; pI < sievereq.B; pI++)
            {
                int p = primeSupply[pI];
                List<long> tmp = new List<long>(2);
                for (int a = 0; a < p; a++)
                {
                    if (V[a] % p == 0)
                    {
                        //we found a quadratic residue (there are two for each prime that isn't 2)
                        tmp.Add(a);
                    }
                }
                if (tmp.Count > 0)
                {
                    //if the ith value is divisible by the prime k, then every k results after that will also be divisible
                    tmpPrimeIntervals.Add(p);
                    tmpPrimeStarts.Add(tmp);
                }
            }

            sievereq.PrimeIntervals = tmpPrimeIntervals.ToArray();
            sievereq.PrimeStarts = tmpPrimeStarts.ToArray();
            //remove primes with no quadratic residues
            sievereq.B = sievereq.PrimeIntervals.Length;
        }

        //memory saving method - is slower, but better for multithreading
        public static void Sieve(SieveInitInfo sievereq, SieveResult sieveres, long startIdx, long L)
        {
            //want to optimize this further (predefine size of SmoothRelations to approximated B value)
            sieveres.SmoothRelations = new List<BinaryVector>();
            sieveres.V = new List<long>();
            sieveres.B = sievereq.B;
            sieveres.VOut = new List<BigInteger>();

            long[] nextIdxA = new long[sievereq.B];
            long[] nextIdxB = new long[sievereq.B];
            for (int i = 0; i < sievereq.B; i++)
            {
                long interval = sievereq.PrimeIntervals[i];
                long primeStart = sievereq.PrimeStarts[i][0];
                long unoffset = startIdx - primeStart;
                long rounded = (long)Math.Ceiling((unoffset) * 1D / interval) * interval;
                long remappedStart = rounded + primeStart;
                nextIdxA[i] = remappedStart;

                if (sievereq.PrimeStarts[i].Count == 1)
                {
                    nextIdxB[i] = -1;
                }
                else
                {
                    interval = sievereq.PrimeIntervals[i];
                    primeStart = sievereq.PrimeStarts[i][1];
                    unoffset = startIdx - primeStart;
                    rounded = (long)Math.Ceiling((unoffset) * 1D / interval) * interval;
                    remappedStart = rounded + primeStart;
                    nextIdxB[i] = remappedStart;
                }
            }
            BinaryVector currVect = new BinaryVector(sievereq.B);
            BigInteger currVal;
            for (long i = startIdx; i < L + startIdx; i++)
            {
                currVal = sievereq.PolyFunction.F(i + sievereq.AStart);

                for (int j = 0; j < sievereq.B; j++)
                {
                    if (nextIdxA[j] == i)
                    {
                        while (currVal % sievereq.PrimeIntervals[j] == 0)
                        {
                            currVal /= sievereq.PrimeIntervals[j];
                            currVect[j] = currVect[j] + 1;
                        }
                        nextIdxA[j] += sievereq.PrimeIntervals[j];
                    }

                    if (nextIdxB[j] == i)
                    {
                        while (currVal % sievereq.PrimeIntervals[j] == 0)
                        {
                            currVal /= sievereq.PrimeIntervals[j];
                            currVect[j] = currVect[j] + 1;
                        }
                        nextIdxB[j] += sievereq.PrimeIntervals[j];
                    }
                }

                if (currVal == 1)
                {
                    sieveres.SmoothRelations.Add(currVect);
                    currVect = new BinaryVector(sievereq.B);
                    sieveres.V.Add(i + sievereq.AStart);
                    sieveres.VOut.Add(sievereq.PolyFunction.F(i + sievereq.AStart));
                    sieveres.SmoothsFound++;
                }
                else
                {
                    BinaryVector.FastFill(0, currVect);
                }

            }
        }
    }
}

