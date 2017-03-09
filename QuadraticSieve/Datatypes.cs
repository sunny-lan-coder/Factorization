using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QuadraticSieve
{
    public class SieveRequest
    {
        public BigInteger N;
        public int B;
        public List<int>[] PrimeStarts;
        public int[] PrimeIntervals;
        public int StartIdx;
        public int L;
        public int AStart;
        public PolynomialFunction polyFunction;
    }

    public delegate BigInteger PolynomialFunction(int x);

    public class SievingData
    {
        public BigInteger[] V;
        public BinaryVector[] Coefficients;
    }

    public class SieveResult
    {
        public SieveRequest SourceRequest;
        public List<BigInteger> V;
        public List<BinaryVector> SmoothRelations;
    }

    public class SolveRequest
    {
        public int B;
        public int L;
        public BinaryVector[] Coefficients;
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
                acumalator.data[idx] ^= acumalator.data[idx];
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
