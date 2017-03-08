using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EllipticCurveFactorization
{
    class Program
    {
        static void Main(string[] args)
        {
            BigInteger r, x, y;
            r = Util.ExtendedGCD(482, 1180, out x, out y);
            Console.WriteLine("gcd={0} x={1} y={2}",r,x,y);

            if(Util.TryModInverse(17,43,out x))
            {
                Console.WriteLine("multinv=" + x);
            }
            Console.WriteLine();
            EllipticCurveMod ell1 = new EllipticCurveMod(1,1, 493);
           
           if( ell1.TryAdd(316, 29, 316, 29, out x, out y))
            {
                Console.WriteLine("2(2,4) on curve=({0},{1})",x,y);
            }else
            {
                Console.WriteLine("Unable to add. Factor: "+x);
            }

          
            LenstrasFactorization fac = new LenstrasFactorization(BigInteger.Parse("10967535067"));
            LenstrasFactorization.GenRandomElliptic(fac);
            Console.WriteLine("factor 493=" + fac.Factor()+"*"+fac.N/fac.Factor());

            Factor.FactorLenstras(BigInteger.Parse(Console.ReadLine()), BigInteger.Parse(Console.ReadLine()));

            Console.ReadLine();

        }
    }
}
