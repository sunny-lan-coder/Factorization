using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuadraticSieve2
{
    class Program
    {
        static void Main(string[] args)
        {
            QuadraticSieve.primeSupply = new derpy();
            MyQuadraticsTest();
            Console.ReadLine();
        }

        static void MyQuadraticsTest()
        {
            BigInteger N = 90283876;
            int B = 20;
            SieveInitInfo req = new SieveInitInfo();
            req.B = B;
            req.AStart = (int)N.Sqrt() + 1;
            req.PolyFunction = x => x * x - N;
            //the max amount of data we need to find the quadratic residues is the Bth prime (the highest one)
            QuadraticSieve.FindQuadraticResidues(req);
            SieveResult res = new SieveResult();
            QuadraticSieve.Sieve(req, res, 0, 60);
            res.V.ForEach(x => Console.WriteLine(x * x - N));

            SolveRequest sreq = new SolveRequest(req.B, res.SmoothsFound);
            sreq.AddDataToSolveRequest(res);
            SolveResult sres = new SolveResult();
            QuadraticSieve.Gaussian(sreq, sres);
            printarr(sreq.Coefficients);

        }

        public static void printarr(BinaryVector[] arr)
        {
            int rowLength = arr.Length;
            long colLength = 1;

            for (int i = 0; i < rowLength; i++)
            {
                if (!Object.ReferenceEquals(arr[i], null))
                {
                    colLength = arr[i].Length;
                    for (int j = 0; j < colLength; j++)
                    {
                        Console.Write(string.Format("{0} ", arr[i][j]));
                    }
                }
                else
                {

                    for (int j = 0; j < colLength; j++)
                    {
                        Console.Write(string.Format("{0} ", 0));
                    }
                }
                Console.WriteLine();
            }
        }

        static void printarr(BigInteger[] arr)
        {
            int rowLength = arr.Length;

            for (int i = 0; i < rowLength; i++)
            {
                Console.Write(string.Format("{0} ", arr[i]));

                Console.WriteLine();
            }
        }

        static void printarr(List<BigInteger> arr)
        {
            int rowLength = arr.Count;

            for (int i = 0; i < rowLength; i++)
            {
                Console.Write(string.Format("{0} ", arr[i]));

                Console.WriteLine();
            }
        }

        static void printarr(List<BinaryVector> arr)
        {
            int rowLength = arr.Count;
            long colLength = 1;

            for (int i = 0; i < rowLength; i++)
            {
                if (!Object.ReferenceEquals(arr[i], null))
                {
                    colLength = arr[i].Length;
                    for (int j = 0; j < colLength; j++)
                    {
                        Console.Write(string.Format("{0} ", arr[i][j]));
                    }
                }
                else
                {

                    for (int j = 0; j < colLength; j++)
                    {
                        Console.Write(string.Format("{0} ", 0));
                    }
                }
                Console.WriteLine();
            }
        }
    }




    public class derpy : PrimeSupply
    {
        private static readonly int[] hooves = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541 };
        public int this[int key]
        {
            get
            {
                return hooves[key];
            }
        }
    }
}
