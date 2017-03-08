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

        private BigInteger b;

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

        public bool Step()
        {
            factored = !elliptic.TryMultiply(currX, currY, f, out currX, out currY);
            if (factored && currX == -1)
                failed = true;
            f++;
            return factored;
        }

        public BigInteger Factor()
        {
            while (!factored)
            {
                Step();
                if (failed)
                    throw new InvalidOperationException("factorization failed");
            }
            return currX;
        }
    }

    partial class LenstrasFactorization
    {
        public static void GenRandomElliptic(LenstrasFactorization factorization)
        {
            GenRandomElliptic(factorization,factorization.N);
        }
        public static void GenRandomElliptic(LenstrasFactorization factorization, BigInteger b)
        {
            BigInteger x = Util.GetRandom(b);
            BigInteger y = Util.GetRandom(b);
            factorization.elliptic = new EllipticCurveMod(x, y, factorization.N);
            factorization.currX = x;
            factorization.currY = y;
        }

    }
}
