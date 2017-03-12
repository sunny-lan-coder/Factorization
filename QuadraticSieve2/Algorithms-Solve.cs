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
            public List<BinaryVector> reducedMatrix;
            private BinaryVector currSolution;
            private BinaryVector freeMult;
            private bool done;

            public GuassianSolveResult(long B, long L)
            {
                currSolution = new BinaryVector(L);
                done = false;
                reducedMatrix = new List<BinaryVector>();
            }

            public void End()
            {
                freeMult = new BinaryVector(reducedMatrix.Count);
            }

            public BinaryVector GetNextSolution()
            {
                incrementBit(0);
                return currSolution;
            }

            private void incrementBit(int idx)
            {
                if (idx == freeMult.Length)
                {
                    done = true;
                    return;
                }

                if (freeMult[idx] == 1)
                {
                    freeMult[idx] = 0;
                    incrementBit(idx + 1);
                }
                else
                {
                    freeMult[idx] = 1;
                }

                if (freeMult[idx] == 1)
                {
                    BinaryVector.FastAdd(currSolution, reducedMatrix[idx]);
                }
            }

            public bool HasNext()
            {
                return done;
            }

            public void ResetSolution()
            {
                currSolution = new BinaryVector(currSolution.Length);
                freeMult = new BinaryVector(reducedMatrix.Count);
                done = false;
            }
        }

        public static SolveResult Gaussian(SolveRequest solvereq)
        {
            GuassianSolveResult result = new GuassianSolveResult(solvereq.B, solvereq.L);
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
                    solvereq.Coefficients[i][i] = 1;//<-this may or may not work
                    BinaryVector col = new BinaryVector(solvereq.B);
                    for (int j = 0; j < solvereq.B; j++)
                    {
                        col[j] = solvereq.Coefficients[j][i];
                    }
                    result.reducedMatrix.Add(col);
                }
            }
            result.End();
            return result;
        }

        public static bool GetFactor(BigInteger n, SolveRequest req, SolveResult res, out BigInteger factor)
        {
            while (!res.HasNext())
            {
                BinaryVector v = res.GetNextSolution();
                // Program.printarr(v);
                BigInteger S = 1;
                BigInteger A = 1;
                for (int i = 0; i < req.L; i++)
                {
                    if (v[i] == 1)
                    {
                        S *= req.VOut[i];
                        A *= req.V[i];
                    }

                }

                BigInteger sqrtS = S.Sqrt();
                BigInteger f1 = A - sqrtS;
                BigInteger f2 = A + sqrtS;

                //  Console.WriteLine("A={0}, S={1}, sqrt(S)={2}, f1={3}, f2={4}",A,S,sqrtS,f1,f2);

                factor = BigInteger.GreatestCommonDivisor(f1, n);
                if (factor != n && factor != 1)
                    return true;
                factor = BigInteger.GreatestCommonDivisor(f2, n);
                if (factor != n && factor != 1)
                    return true;
            }
            factor = -1;

            return false;
        }
    }
}
