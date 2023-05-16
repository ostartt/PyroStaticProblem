namespace Pyro
{
    public class Application
    {
        public static void Main()
        {
            double lambda = 1.16;
            double pi = 0.00027;
            double alpha = 0.000002;
            double ro = 7500;
            double c = 139000000000;
            double e = -15.1;
            double g = 0.000000646;
            double sigmaL = 0;
            double DL = 0;
            double hL = 100;

            PyroSolver pyroSolver = new PyroSolver();

            pyroSolver.LinearFuncVector = new Vector(2 * PyroSolver.N + 1);
            pyroSolver.PhiDerMatrix = new Matrix(2 * PyroSolver.N + 1, 5);
            pyroSolver.PhiDerFuncMatrix = new Matrix(2 * PyroSolver.N + 1, 5);

            pyroSolver.CalculateInitialBiForm(pyroSolver.PhiDerMatrix, PyroSolver.DerBlock);
            pyroSolver.CalculateInitialBiForm(pyroSolver.PhiDerFuncMatrix, PyroSolver.DerFuncBlock);
            // pyroSolver.CalculateInitialLinearFunc(pyroSolver.LinearFuncVector);

            pyroSolver.CBiForm = new Matrix(pyroSolver.PhiDerMatrix) * c;
            pyroSolver.EBiForm = new Matrix(pyroSolver.PhiDerMatrix) * e;
            pyroSolver.GBiForm = new Matrix(pyroSolver.PhiDerMatrix) * g;
            pyroSolver.KBiForm = new Matrix(pyroSolver.PhiDerMatrix) * lambda;
            pyroSolver.YBiForm = new Matrix(pyroSolver.PhiDerFuncMatrix) * alpha * c;
            pyroSolver.PiBiForm = new Matrix(pyroSolver.PhiDerFuncMatrix) * pi;

            pyroSolver.LFunc = new Vector(2 * PyroSolver.N + 1);
            pyroSolver.RFunc = new Vector(2 * PyroSolver.N + 1);
            pyroSolver.MuFunc = new Vector(2 * PyroSolver.N + 1);

            pyroSolver.LFunc[2 * PyroSolver.N] = sigmaL;
            pyroSolver.RFunc[2 * PyroSolver.N] = DL;
            pyroSolver.MuFunc[2 * PyroSolver.N] = hL;

            Matrix[,] finalMatrix = pyroSolver.FormFinalMatrix();
            Vector[] finalVector = pyroSolver.FormFinalVector();

            Vector[] solution = PyroSolver.FiveDiagonalLowerUpperMethod(finalMatrix, finalVector);

            // for (int i = 0; i < solution.Length; i++)
            // {
            //     Console.WriteLine(solution[i]);
            // }
        }
    }
}