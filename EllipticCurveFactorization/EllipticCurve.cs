using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EllipticCurveFactorization
{
    //represents an elliptic curve mod n
    class EllipticCurveMod
    {
        public BigInteger a;
        public BigInteger b;
        public BigInteger n;

        public EllipticCurveMod(BigInteger a, BigInteger b, BigInteger n)
        {
            this.a = a;
            this.b = b;
            this.n = n;
        }

        public EllipticCurveMod(BigInteger x, BigInteger y, BigInteger a, BigInteger n)
        {
            this.a = a;
            this.n = n;
            this.b = Util.Mod(y * y - x * x * x - a * x, n);
        }

        private bool trySlopeDistinct(BigInteger x1, BigInteger y1, BigInteger x2, BigInteger y2, out BigInteger slope)
        {
            BigInteger deltaX = Util.Mod(x1 - x2, n);
            BigInteger deltaY = Util.Mod(y1 - y2, n);

            BigInteger modInverse;

            if (!Util.TryModInverse(deltaX, n, out modInverse))
            {
                slope = modInverse;
                return false;
            }

            slope = Util.Mod( deltaY * modInverse , n);
            return true;
        }

        private bool trySlopeDerivative(BigInteger x, BigInteger y, out BigInteger slope)
        {
            BigInteger top = Util.Mod(3 * x * x + a, n);

            BigInteger bottom = Util.Mod(2 * y, n);
            BigInteger modInverse;
            if (!Util.TryModInverse(bottom, n, out modInverse))
            {
                slope = modInverse;
                return false;
            }
            slope = Util.Mod(top * modInverse , n);
            return true;
        }

        public bool TryAdd(BigInteger x1, BigInteger y1, BigInteger x2, BigInteger y2, out BigInteger x3, out BigInteger y3)
        {
            BigInteger slope;
            if (Util.PointsEqual(x1, y1, x2, y2))
            {
                if (!trySlopeDerivative(x1, y1, out slope))
                {
                    x3 = slope;
                    y3 = -1;
                    return false;
                }
            }
            else
            {
                if (x1 == x2)
                {
                    x3 = -1;
                    y3 = -1;
                    return false;
                }
                if (!trySlopeDistinct(x1, y1, x2, y2, out slope))
                {
                    x3 = slope;
                    y3 = -1;
                    return false;
                }
            }

            x3 = Util.Mod(slope * slope - x1 - x2, n);
            y3 = Util.Mod(slope * (x1 - x3) - y1, n);
            return true;
        }

        public bool TryMultiply(BigInteger x, BigInteger y, long f, out BigInteger x1, out BigInteger y1)
        {
            bool set = false;
            x1 = -1;
            y1 = -1;
            BigInteger xpow = x;
            BigInteger ypow = y;
            while (f > 0)
            {
                if ((f & 1) == 1)
                {
                    if (!set)
                    {
                        x1 = xpow;
                        y1 = ypow;
                        set = true;
                    }
                    else
                    {
                        if (!TryAdd(x1, y1, xpow, ypow, out x1, out y1))
                        {
                            return false;
                        }
                    }
                }
                f = f >> 1;
                if (!TryAdd(xpow, ypow, xpow, ypow, out xpow, out ypow))
                {
                    x1 = xpow;
                    return false;
                }
            }
            return true;
        }
    }
}
