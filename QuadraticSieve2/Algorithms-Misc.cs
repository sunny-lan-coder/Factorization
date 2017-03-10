using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QuadraticSieve2
{
    public interface PrimeSupply
    {
        int this[int key]
        {
            get;
        }
    }

    public partial class QuadraticSieve
    {
        public static PrimeSupply primeSupply;
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

