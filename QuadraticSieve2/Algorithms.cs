using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QuadraticSieve
{
    public interface PrimeSupply
    {
        int this[int key]
        {
            get;
        }
    }

    public class QuadraticSieve
    {
        public static PrimeSupply primeSupply;

        public static void InitSievingRequest(BigInteger N, int B, PolynomialFunction f, SieveRequest sievereq)
        {
            sievereq.AStart = (int)N.Sqrt() + 1;
            sievereq.StartIdx = 0;
            sievereq.PolyFunction = f;
            //the max amount of data we need to find the quadratic residues is the Bth prime (the highest one)
            sievereq.L = primeSupply[B];

            SievingData sievedat = new SievingData();

            EvaluatePoly(sievereq, sievedat);

            List<List<long>> tmpPrimeStarts = new List<List<long>>();
            List<long> tmpPrimeIntervals = new List<long>();

            for (int pI = 0; pI < B; pI++)
            {
                int p = primeSupply[pI];
                List<long> tmp = new List<long>(2);
                for (int a = 0; a < p; a++)
                {
                    if (sievedat.V[a] % p == 0)
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

        public static void SegmentSievingRequest(int startIdx, int L, SieveRequest sievereq)
        {
            sievereq.StartIdx = startIdx;
            sievereq.L = L;
        }

        public static void EvaluatePoly(SieveRequest sievereq, SievingData sievedat)
        {
            sievedat.V = new BigInteger[sievereq.L];
            for (int i = 0; i < sievereq.L; i++)
            {
                sievedat.V[i] = sievereq.PolyFunction(i + sievereq.AStart + sievereq.StartIdx);
            }
        }

        //memory saving method - is slower, but better for multithreading
        public static void Sieve(SieveRequest sievereq, SieveResult sieveres)
        {
            //want to optimize this further (predefine size of SmoothRelations to approximated B value)
            sieveres.SourceRequest = sievereq;
            sieveres.SmoothRelations = new List<BinaryVector>();
            sieveres.V = new List<long>();

            long[] nextIdxA = new long[sievereq.B];
            long[] nextIdxB = new long[sievereq.B];
            for (int i = 0; i < sievereq.B; i++)
            {
                long interval = sievereq.PrimeIntervals[i];
                long remappedStart = (interval - (sievereq.StartIdx - sievereq.PrimeStarts[i][0])) % interval;
                if (remappedStart < 0)
                    remappedStart = remappedStart + interval;
                nextIdxA[i] = remappedStart;

                remappedStart = (interval - (sievereq.StartIdx - sievereq.PrimeStarts[i][1])) % interval;
                if (remappedStart < 0)
                    remappedStart = remappedStart + interval;
                nextIdxB[i] = remappedStart;
            }
            BinaryVector currVect = new BinaryVector(sievereq.B);
            BigInteger currVal;
            for (long i = sievereq.StartIdx; i < sievereq.L + sievereq.StartIdx; i++)
            {
                currVal = sievereq.PolyFunction(i);

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
                    sieveres.V.Add(i);
                }
                else
                {
                    BinaryVector.FastFill(0, currVect);
                }

            }
        }

        public static void InitSolveRequest(int rows, int columns, SolveRequest solvereq)
        {
            solvereq.B = rows;
            solvereq.L = columns;
            solvereq.Coefficients = new BinaryVector[rows];
            solvereq.V = new List<long>();
            for (int i = 0; i < rows; i++)
            {
                solvereq.Coefficients[i] = new BinaryVector(columns);
            }
        }

        public static void AddDataToSolveRequest(SieveResult data, SolveRequest solvereq)
        {
            //transpose matrix, add it to the coefficients (making sure to offset by startIdx of data)
            solvereq.V.AddRange(solvereq.V);
            for (int i = 0; i < data.V.Count; i++)
            {
                for (int j = 0; j < data.SourceRequest.B; j++)
                {
                    solvereq.Coefficients[j][i + solvereq.V.Count] = data.SmoothRelations[i][j];
                }
            }
        }

        public static void Gaussian(SolveRequest solvereq)
        {
            for (int i = 0; i < Math.Min(solvereq.B, solvereq.L); i++)
            {
                if (solvereq.Coefficients[i][i] == 0)
                {
                    for (int j = i + 1; j < solvereq.B; j++)
                    {
                        if (solvereq.Coefficients[j][i] == 1)
                        {
                            BinaryVector tmp = solvereq.Coefficients[j];
                            solvereq.Coefficients[j] = solvereq.Coefficients[i];
                            solvereq.Coefficients[i] = tmp;
                            break;
                        }
                    }
                }
                if (solvereq.Coefficients[i][i] == 1)
                {
                    for (int j = 0; j < solvereq.B; j++)
                    {
                        if (j != i)
                            if (solvereq.Coefficients[j][i] == 1)
                            {
                                BinaryVector.FastAdd(solvereq.Coefficients[j], solvereq.Coefficients[i]);
                            }
                    }
                }
            }
        }

        public static void BackSubsitution(SolveRequest rowEchelon, SolveResult solveres)
        {

        }
    }

    public static class NumberTheory
    {
        // Newton's Method for finding sqrt
        public static BigInteger Sqrt(this BigInteger n)
        {
            var prev = n + 1; var cur = n;
            while (true)
            {
                prev = cur;
                cur = (n / cur + cur) / 2;
                if (BigInteger.Abs(prev - cur) <= 1)
                {
                    var test = cur * cur - n;
                    if (test == 0)
                        return cur;
                    else if (BigInteger.Abs(test) <= 2 * n)
                    {
                        if (test > 0) return cur - 1;
                        else return cur;
                    }
                }
            }
        }
    }
}

