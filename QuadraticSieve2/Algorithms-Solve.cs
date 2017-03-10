using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QuadraticSieve2
{
    public partial class QuadraticSieve
    {
       private class GuassianSolveResult : SolveResult
        {
            public List<long> freeVariables;

            public GuassianSolveResult()
            {
                freeVariables = new List<long>();
            }

            public BinaryVector GetNextSolution()
            {
                throw new NotImplementedException();
            }
        }

        public static SolveResult Gaussian(SolveRequest solvereq)
        {
            GuassianSolveResult result = new GuassianSolveResult();
            for (int i = 0; i < Math.Min(solvereq.B, solvereq.L); i++)
            {
                if (solvereq.Coefficients[i][i] == 0)
                {
                    for (int j = i + 1; j < solvereq.B; j++)
                    {
                        if (solvereq.Coefficients[j][i] == 1)
                        {
                            BinaryVector tmp = solvereq.Coefficients[j];
                            solvereq.Coefficients[j] = solvereq.Coefficients[i];
                            solvereq.Coefficients[i] = tmp;
                            break;
                        }
                    }
                }
                if (solvereq.Coefficients[i][i] == 1)
                {
                    for (int j = 0; j < solvereq.B; j++)
                    {
                        if (j != i)
                            if (solvereq.Coefficients[j][i] == 1)
                            {
                                BinaryVector.FastAdd(solvereq.Coefficients[j], solvereq.Coefficients[i]);
                            }
                    }
                }
                else
                {
                    result.freeVariables.Add(i);
                }
            }
            return result;
        }
    }
}

