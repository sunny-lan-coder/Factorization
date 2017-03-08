using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EllipticCurveFactorization
{
    class Factor
    {

        public static void FactorLenstras(BigInteger n, BigInteger b)
        {
            Stopwatch s=new Stopwatch();
            s.Start();
            Console.WriteLine("Removing small primes...");
            n = RemoveSmallPrimes(n);
            LenstrasFactorization fac = new LenstrasFactorization(n);
            LenstrasFactorization.GenRandomElliptic(fac, b);
            LenstrasFactorization.SetBValueConventional(fac);
            Console.WriteLine("Factoring with B="+fac.b);
            int failcount = 0;
            while (n != 1)
            {
                BigInteger f;
                if (!fac.Factor(out f))
                {
                    LenstrasFactorization.GenRandomElliptic(fac, b);
                    failcount++;
                    if (failcount >= LenstrasFactorization.ConventionalCurves(fac.N))
                   {
                        Console.WriteLine("{0} is likely prime. Curve was run {1} times without finding factor.", fac.N, failcount);
                        break;
                   }
                    continue;
                }

                n /= f;
                if (n == 0)
                    break;
                Console.WriteLine("/" + f + "=" + n);
                failcount = 0;
                fac.SetProduct(n);
                LenstrasFactorization.GenRandomElliptic(fac, b);
                LenstrasFactorization.SetBValueConventional(fac);
            }
            s.Stop();
            Console.WriteLine("Find took "+s.ElapsedMilliseconds+"ms");
        }

        public static BigInteger RemoveSmallPrimes(BigInteger n)
        {
            for (int i = 0; i < Util.SmallPrimes.Length; i++)
            {
                while (n % Util.SmallPrimes[i] == 0)
                {
                    n /= Util.SmallPrimes[i];
                    Console.WriteLine("/" + Util.SmallPrimes[i] + "=" + n);
                }
            }
            return n;
        }
    }
}
