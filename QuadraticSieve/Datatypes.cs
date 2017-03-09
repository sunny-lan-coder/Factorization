using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QuadraticSieve
{
    // Container holding data required to perform seiving
    public class SieveRequest
    {
        public BigInteger N;//The number to factor
        public int B;//The size of PrimeIntervals
        public List<int>[] PrimeStarts;//The starting indexes for each prime factor (each prime factor is repeated at a regular interval because of quadratic residues)
        public int[] PrimeIntervals;//The primes that are going to be used in the seive
        public int StartIdx;//The index to start sieving at
        public int L;//The value of the highest prime in PrimeIntervals
        public int AStart;//The value of a to start sieving at for the polynomial function f(a)
        public PolynomialFunction polyFunction;//The polynomial function f(a) to seive for smooth things
    }

    public delegate BigInteger PolynomialFunction(int x);

    //Container holding data used while sieving
    public class SievingData
    {
        public BigInteger[] V;//stores all the outputs of the f(a), will get divided down as seive progresses
        public BinaryVector[] Coefficients;//the matrix containing the prime powers mod 2 of each result
    }

    //Container holding the results from sieving
    public class SieveResult
    {
        public SieveRequest SourceRequest;//The request that produced this result
        public List<BigInteger> V;//The values of f(a)
        public List<BinaryVector> SmoothRelations;//Contains all the smooth relations
    }

    //Container holding data required to perform guassian elimination on seive results
    public class SolveRequest
    {
        public int B;
        public int L;
        public int FirstFree;
        public BinaryVector[] Coefficients;
        public List<BigInteger> V;
    }

    public class SolveResult
    {
        public List<BinaryVector> solutions;
    }

    public class BinaryVector
    {
        public readonly int Length;

        public bit this[int key]
        {
            get
            {
                int modulus = key % sizeof(long);
                int quotient = (key - modulus) / sizeof(long);
                return (bit)((data[quotient] >> modulus) & 1);
            }
            set
            {
                int modulus = key % sizeof(long);
                int quotient = (key - modulus) / sizeof(long);
                long mask = 1 << modulus;
                if (value == bit.ONE)
                {
                    data[quotient] |= mask;
                }
                else if (value == bit.ZERO)
                {
                    data[quotient] &= ~mask;
                }
                else
                {
                    throw new ArgumentException("Input can only be ONE or ZERO");
                }
            }
        }

        public static bool operator ==(BinaryVector a, BinaryVector b)
        {

            for (int idx = 0; idx < a.data.Length; idx++)
            {
                if (a.data[idx] != b.data[idx])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(BinaryVector a, BinaryVector b)
        {
            for (int idx = 0; idx < a.data.Length; idx++)
            {
                if (a.data[idx] != b.data[idx])
                {
                    return true;
                }
            }
            return false;
        }

        public static void FastAdd(BinaryVector acumalator, BinaryVector addent)
        {
            for (int idx = 0; idx < acumalator.data.Length; idx++)
            {
                acumalator.data[idx] ^= addent.data[idx];
            }
        }

        public static void FastFill(bit bit, BinaryVector vector)
        {
            long fill = 0;
            if (bit == bit.ONE)
            {
                fill = ~fill;
            }
            for (int idx = 0; idx < vector.data.Length; idx++)
            {
                vector.data[idx] = fill;
            }
        }

        public static long CircularLeftShift(long input, int n)
        {
            return input << n | input >> (sizeof(long) - n);
        }

        private long[] data;

        public BinaryVector(int length)
        {
            data = new long[(long)Math.Ceiling((double)length / sizeof(long))];
            this.Length = length;
        }
    }

    /// <summary>
    ///  A 1 bit number
    /// </summary>
    public struct bit
    {
        public static readonly bit ONE = new bit(1);
        public static readonly bit ZERO = new bit(0);


        private int val;

        private bit(int val)
        {
            this.val = val;
        }

        public static implicit operator bit(int i)
        {
            return new bit(i);
        }

        public static bit operator +(bit b1, bit b2)
        {
            return new bit(b1.val ^ b2.val);
        }

        public static bit operator -(bit b1, bit b2)
        {
            return new bit(b1.val ^ b2.val);
        }

        public static bit operator *(bit b1, bit b2)
        {
            return new bit(b1.val & b2.val);
        }

        public static bool operator ==(bit b1, bit b2)
        {
            return b1.val == b2.val;
        }

        public static bool operator !=(bit b1, bit b2)
        {
            return b1.val != b2.val;
        }

        public static bool operator >(bit b1, bit b2)
        {
            return b1.val > b2.val;
        }

        public static bool operator <(bit b1, bit b2)
        {
            return b1.val < b2.val;
        }

        public static bit operator /(bit b1, bit b2)
        {
            if (b2 == ZERO)
                throw new ArithmeticException("Cannot divide by zero");
            return new bit(b1.val & b2.val);
        }

        public override string ToString()
        {
            return "" + val;
        }
    }
}
