﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QuadraticSieve2
{
    // Container holding data required to prior to seiving
    public class SieveInitInfo
    {
        public int B;//The size of PrimeIntervals
        public List<long>[] PrimeStarts;//The starting indexes for each prime factor (each prime factor is repeated at a regular interval because of quadratic residues)
        public long[] PrimeIntervals;//The primes that are going to be used in the seive
        public long AStart;//The value of a to start sieving at for the polynomial function f(a)
        public PolynomialFunction PolyFunction;//The polynomial function f(a) to seive for smooth things

        public SieveInitInfo(int B, PolynomialFunction f)
        {
            this.B = B;
            PolyFunction = f;
            AStart = f.PositivePoint();
        }
    }

    public interface PolynomialFunction
    {
        BigInteger F(long x);//should return value of f(x)
        long PositivePoint();//should return first x value at which output >0
    }

    //defult polynomial function for sieving A Squared Minus N (needs better naming)
    public class AS2MNPolyFunc : PolynomialFunction
    {
        private BigInteger n;
        public AS2MNPolyFunc(BigInteger n)
        {
            this.n = n;
        }
        public BigInteger F(long x)
        {
            return x * x - n;
        }

        public long PositivePoint()
        {
            return (long)n.Sqrt() + 1;
        }
    }


    //Container holding the results from sieving
    public class SieveResult
    {
        public int B;//size of an individual Smooth relation
        public int SmoothsFound;//used for multithreading, to determine how many have been found so far
        public List<long> V;//The values of a
        public List<BigInteger> VOut;//Output values of function given a
        public List<BinaryVector> SmoothRelations;//Contains all the smooth relations
    }

    //Container holding data required to perform get solution from seive data
    public class SolveRequest
    {
        public int B;
        public long L;
        public BinaryVector[] Coefficients;
        public List<long> V;
        public List<BigInteger> VOut;
        private long columns;

        public SolveRequest(int rows, long columns)
        {
            this.columns = columns;
            B = rows;
            L = 0;
            Coefficients = new BinaryVector[rows];
            V = new List<long>();
            VOut = new List<BigInteger>();
            for (int i = 0; i < rows; i++)
            {
                Coefficients[i] = new BinaryVector(columns);
            }
        }

        public void AddDataToSolveRequest(SieveResult data)
        {
            //transpose matrix, add it to the coefficients (making sure to offset by startIdx of data)
            for (int i = 0; i < data.V.Count; i++)
            {
                for (int j = 0; j < data.B; j++)
                {
                    Coefficients[j][i + V.Count] = data.SmoothRelations[i][j];
                }
            }
            V.AddRange(data.V);
            VOut.AddRange(data.VOut);
            L += data.SmoothsFound;
        }

        public bool AddDataToSolveRequestMax(SieveResult data)
        {
            long tmp = L;
            //transpose matrix, add it to the coefficients (making sure to offset by startIdx of data)
            for (int i = 0; i < data.V.Count; i++)
            {
                for (int j = 0; j < data.B; j++)
                {
                    Coefficients[j][i + tmp] = data.SmoothRelations[i][j];
                }
                V.Add(data.V[i]);
                VOut.Add(data.VOut[i]);
                L++;
                if (L >= columns)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public interface SolveResult
    {
        BinaryVector GetNextSolution();
        //need better name for this: the number of time the solutions loop around
        bool HasNext();

        void ResetSolution();
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
            for (int idx = 0; idx < Math.Min(acumalator.data.Length, addent.data.Length); idx++)
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

        private long[] data;

        public BinaryVector(long length)
        {
            data = new long[(long)Math.Ceiling((double)length / sizeof(long))];
            this.Length = length;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
