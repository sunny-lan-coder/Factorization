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
        public List<long>[] PrimeStarts;//The starting indexes for each prime factor (each prime factor is repeated at a regular interval because of quadratic residues)
        public long[] PrimeIntervals;//The primes that are going to be used in the seive
        public long StartIdx;//The index to start sieving at
        public long L;//The value of the highest prime in PrimeIntervals
        public long AStart;//The value of a to start sieving at for the polynomial function f(a)
        public PolynomialFunction PolyFunction;//The polynomial function f(a) to seive for smooth things
    }

    public delegate BigInteger PolynomialFunction(long x);

    //Container holding data used while sieving (obselete)
    public class SievingData
    {
        public BigInteger[] V;//stores all the outputs of the f(a), will get divided down as seive progresses
        public BinaryVector[] Coefficients;//the matrix containing the prime powers mod 2 of each result
    }

    //Container holding the results from sieving
    public class SieveResult
    {
        public SieveRequest SourceRequest;//The request that produced this result
        public List<long> V;//The values of a
        public List<BinaryVector> SmoothRelations;//Contains all the smooth relations
    }

    //Container holding data required to perform guassian elimination on seive results
    public class SolveRequest
    {
        public int B;
        public long L;
        public BinaryVector[] Coefficients;
        public List<long> V;
    }

    public class SolveResult
    {
        public List<BinaryVector> solutions;
    }

    public class BinaryVector
    {
        public readonly long Length;

        public bit this[long key]
        {
            get
            {
                int modulus = (int)(key % sizeof(long));
                long quotient = (key - modulus) / sizeof(long);
                return (bit)((data[quotient] >> modulus) & 1);
            }
            set
            {
                int modulus = (int)(key % sizeof(long));
                long quotient = (key - modulus) / sizeof(long);
                long mask = 1L << modulus;
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


        private long val;

        private bit(long val)
        {
            this.val = val;
        }

        public static implicit operator bit(long i)
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
