using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EllipticCurveFactorization
{
    partial class LenstrasFactorization
    {
        public BigInteger N;

        private EllipticCurveMod elliptic;

        private long f;

        private BigInteger currX;
        private BigInteger currY;

        private bool factored;
        private bool failed;

        public long? b;
        private BigInteger tmp;

        public LenstrasFactorization(BigInteger n)
        {
            SetProduct(n);
        }

        public LenstrasFactorization(BigInteger n, EllipticCurveMod elliptic, BigInteger x, BigInteger y) : this(n)
        {
            this.elliptic = elliptic;
            this.currX = x;
            this.currY = y;
        }

        public void SetProduct(BigInteger n)
        {
            this.N = n;
            factored = false;
            failed = false;
            f = 2;
        }

        public void SetBValue(long b)
        {
            this.b = b;
        }

        public bool Step()
        {
            factored = !elliptic.TryMultiply(currX, currY, f, out currX, out currY);
            if (factored && currX == -1)
                failed = true;
            f++;
            return factored;
        }

        public bool Factor(out BigInteger factor)
        {
            if (b == null)
            {
                return FactorUntilFound(out factor);
            }
            else
            {
                return FactorLimited(out factor);
            }
        }

        public bool FactorLimited(out BigInteger factor)
        {
            return !elliptic.TryMultiply(currX, currY, b.Value, out factor, out tmp);
        }

        public bool FactorUntilFound(out BigInteger factor)
        {
            while (!factored)
            {
                Step();
                if (failed)
                {
                    factor = -1;
                    return false;
                }
            }
            factor = currX;
            return true;
        }
    }

    partial class LenstrasFactorization
    {
        public static void GenRandomElliptic(LenstrasFactorization factorization)
        {
            GenRandomElliptic(factorization, factorization.N);
        }
        public static void GenRandomElliptic(LenstrasFactorization factorization, BigInteger b)
        {
            BigInteger x = Util.GetRandom(b);
            BigInteger y = Util.GetRandom(b);
            factorization.elliptic = new EllipticCurveMod(x, y, factorization.N);
            factorization.currX = x;
            factorization.currY = y;
        }

        public static void SetBValueFactorial(LenstrasFactorization factorization, long b)
        {
            long factorial = 1;
            for (long i = 2; i <= b; i++)
            {
                factorial *= i;
            }
            factorization.SetBValue(b);
        }

        public static void SetBValueSmallPrimes(LenstrasFactorization factorization, long b)
        {
            long sol = 1;
            int i = 0;
            long currPrime;
            while (i < Util.SmallPrimes.Length && (currPrime = Util.SmallPrimes[i]) < b)
            {
                sol *= (long)Math.Pow(currPrime, Math.Round(Math.Log(b, currPrime)));
                i++;
            }
            factorization.SetBValue(sol);
        }

        private static long[] conventionalB = {
           2000,2000,2000,11000,50000,250000,1000000,3000000,11000000,43000000,110000000,260000000,850000000,2900000000
        };

        private static int[] conventionalCurves =
        {
            25,90,300,700,1800,5100,10600,19300,49000,124000,210000,340000
        };

        private static int productMagnitude(BigInteger n)
        {
            return (int)Math.Round(BigInteger.Log10(n) / 5);
        }

        public static int ConventionalCurves(BigInteger n)
        {
            return conventionalCurves[productMagnitude(n)];
        }

        public static long ConventionalB(BigInteger n)
        {
            return conventionalB[productMagnitude(n)];
        }

        public static void SetBValueConventional(LenstrasFactorization factorization)
        {
            SetBValueSmallPrimes(factorization, ConventionalB(factorization.N));
        }

    }
}
