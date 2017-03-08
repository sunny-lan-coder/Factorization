using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace EllipticCurveFactorization
{

    static class Util
    {
        private static Random random = new Random();
        public static BigInteger Mod(BigInteger n, BigInteger m)
        {
            return (n % m + m) % m;
        }

        public static bool PointsEqual(BigInteger x1, BigInteger y1, BigInteger x2, BigInteger y2)
        {
            return x1 == x2 && y1 == y2;
        }

        public static BigInteger GetRandom(BigInteger n)
        {
            long length = (long)Math.Ceiling(Math.Ceiling(BigInteger.Log(n, 2)) / 8);
            byte[] data = new byte[length];
            random.NextBytes(data);
            return Mod(new BigInteger(data) , n);
        }

        public static BigInteger ExtendedGCD(BigInteger bigger, BigInteger smaller, out BigInteger x, out BigInteger y)
        {
            //should only happen in top level call
            if (smaller > bigger)
                return ExtendedGCD(smaller, bigger, out y, out x);

            BigInteger coeff = bigger / smaller;
            BigInteger remainder = bigger % smaller;

            if (remainder == 0)
            {
                x = 0;
                y = 1;
                return smaller;
            }

            x = 0;
            y = 0;
            BigInteger r, rx, ry;
            r = ExtendedGCD(smaller, remainder, out rx, out ry);
            y = rx + ry * -coeff;
            x = ry;
            return r;
        }

        public static bool TryModInverse(BigInteger a, BigInteger m, out BigInteger x)
        {
            BigInteger r, rx, ry;
            r = ExtendedGCD(a, m, out rx, out ry);
            if (r != 1)
            {
                x = r;
                return false;
            }
            x = Util.Mod(rx, m);
            return true;
        }
    }
}
