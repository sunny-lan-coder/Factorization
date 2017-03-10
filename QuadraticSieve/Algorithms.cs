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
            StartIdx = 0;
            sievereq.polyFunction = f;
            //the max amount of data we need to find the quadratic residues is the Bth prime (the highest one)
            sievereq.L = primeSupply[B];

            SievingData sievedat = new SievingData();

            EvaluatePoly(sievereq, sievedat);

            List<List<int>> tmpPrimeStarts = new List<List<int>>();
            List<int> tmpPrimeIntervals = new List<int>();

            for (int pI = 0; pI < B; pI++)
            {
                int p = primeSupply[pI];
                List<int> tmp = new List<int>();
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
            StartIdx = startIdx;
            sievereq.L = L;
        }

        public static void EvaluatePoly(SieveRequest sievereq, SievingData sievedat)
        {
            sievedat.V = new BigInteger[sievereq.L];
            for (int i = 0; i < sievereq.L; i++)
            {
                sievedat.V[i] = sievereq.polyFunction(i + sievereq.AStart + StartIdx);
            }
        }

        //really weird sieving method
        public static void Sieve(SieveRequest sievereq, SievingData sievedat)
        {
            //get data first
            EvaluatePoly(sievereq, sievedat);

            //make transposed matrix (each BinaryVector holds the factorization for a number)
            sievedat.Coefficients = new BinaryVector[sievereq.L];

            //loop through each prime
            for (int i = 0; i < sievereq.B; i++)
            {
                //get value of prime
                int interval = sievereq.PrimeIntervals[i];
                //loop through each solution of the quadratic residue
                for (int j = 0; j < sievereq.PrimeStarts[i].Count; j++)
                {
                    //remap start location to this sieving interval
                    int remappedStart = (interval - (StartIdx - sievereq.PrimeStarts[i][j])) % interval;
                    if (remappedStart < 0)
                        remappedStart = remappedStart + interval;

                    //sieve out each prime
                    for (int k = remappedStart; k < sievereq.L; k += interval)
                    {
                        //attempt to save memory by not initializating the arrayelements that haven't been used yet
                        //NOTE: this doesn't work, because at least half of the elements will have been visited
                        if (Object.ReferenceEquals(sievedat.Coefficients[k], null))
                            sievedat.Coefficients[k] = new BinaryVector(sievereq.B);

                        //divide out the whole prime power
                        while (sievedat.V[k] % interval == 0)
                        {
                            sievedat.Coefficients[k][i] = sievedat.Coefficients[k][i] + 1;
                            sievedat.V[k] = sievedat.V[k] / interval;
                        }
                    }
                }
            }
        }

        public static void CreateFormattedSievingResult(SieveRequest sievereq, SievingData sievedat, SieveResult sieveres)
        {
            sieveres.SourceRequest = sievereq;
            sieveres.SmoothRelations = new List<BinaryVector>();
            sieveres.V = new List<BigInteger>();
            for (int i = 0; i < sievereq.L; i++)
            {
                //if number only consisted of primes that were sieved, then it is smooth
                if (sievedat.V[i] == 1)
                {
                    sieveres.V.Add(sievereq.polyFunction(i + sievereq.AStart + StartIdx));
                    sieveres.SmoothRelations.Add(sievedat.Coefficients[i]);
                }
            }
        }

        public static void InitSolveRequest(int rows, int columns, SolveRequest solvereq)
        {
            solvereq.B = rows;
            solvereq.L = columns;
            solvereq.Coefficients = new BinaryVector[rows];
            solvereq.V = new List<BigInteger>();
            solvereq.FirstFree = 0;
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
                    solvereq.FirstFree = i;
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

