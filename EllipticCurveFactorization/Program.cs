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

            Factor.FactorLenstras(BigInteger.Parse(Console.ReadLine()), BigInteger.Parse(Console.ReadLine()));

            Console.ReadLine();

        }
    }
}
