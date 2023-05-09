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
            pyroSolver.CalculateInitialBlockMatrices();
            pyroSolver.CalculateInitialBlockVector();
            Console.WriteLine(pyroSolver.DerBlockMatrix);

            Matrix cBiForm = pyroSolver.DerBlockMatrix * c;
            Matrix eBiForm = pyroSolver.DerBlockMatrix * e;
            Matrix gBiForm = pyroSolver.DerBlockMatrix * g;
            Matrix kBiForm = pyroSolver.DerBlockMatrix * lambda;
            Matrix yBiForm = pyroSolver.DerFuncBlockMatrix * alpha * c;
            Matrix piBiForm = pyroSolver.DerFuncBlockMatrix * pi;

            double[] rArray = new double[2 * PyroSolver.N + 1];
            // Array.Fill(rArray, );
            // Vector rLinFunctional = new Vector();



        }
    }
}

