using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;

namespace Lab3
{
    public class SplineData
    {
        private V1DataArray knots { get; set; }
        private int SplineKnotCount { get; set; }
        private int maxIterations { get; set; }
        private int stopType { get; set; }
        private double minResidual { get; set; }
        public List<SplineDataItem> results { get; set; }
        public int SupportSplineKnotCount { get; set; }
        public List<double[]> supportResults { get; set; }

        private double[] supportX;
        private double[] supportY;
        private int iterationCount { get; set; }

        public SplineData(V1DataArray knots, int SplineKnotCount, int maxIterations, double[] interval, int amount)
        {
            this.knots = knots;
            this.SplineKnotCount = SplineKnotCount;
            this.maxIterations = maxIterations;
            
            supportResults = new List<double[]>();
            supportX = new double[amount];
            supportY = new double[amount];

            for (int i = 0; i < amount - 1; i++)
            {
                supportX[i] = interval[0] + (interval[1] - interval[0]) / (amount - 1) * i;
            }
            supportX[amount - 1] = interval[1];
        }

        public void calculate(double norm)
        {
            double[] interval = {knots.grid.Min(), knots.grid.Max()};
            int iterationCount = 0;
            int criteria = 0;
            double minResidual = 0;
            double[] y = new double[knots.grid.Length];
            int error = 0;
            
            calculateSpline(knots.grid.Length, 
                            knots.grid, 
                            knots[0], 
                            SplineKnotCount, 
                            norm, 
                            maxIterations, 
                            interval, 
                            ref iterationCount, 
                            ref criteria, 
                            ref minResidual, 
                            y, 
                            ref error,
                            supportX,
                            supportX.Length,
                            supportY);

            if(error != 0) {
                throw new Exception("Произошла ошибка " + error + " во время вычисления сплайна");
                System.Console.WriteLine("During calculation error " + error + " occured") ;
            }

            results = new List<SplineDataItem>();
            for (int i = 0; i < knots.grid.Length; i++)
            {
                results.Add(new SplineDataItem(knots.grid[i], knots[0][i], y[i]));
            }
            this.iterationCount = iterationCount;
            this.minResidual = minResidual;
            stopType = criteria;
            
            for (int i = 0; i < supportX.Length; i++)
            {
                supportResults.Add(new double[] { supportX[i], supportY[i] });
            }
        }
        [DllImport("\\..\\..\\..\\..\\x64\\Debug\\SplineCalculation.dll",
        CallingConvention = CallingConvention.Cdecl)]
        public static extern void calculateSpline(int initialN,
                                                double[] initialX,
                                                double[] initialY,
                                                int splineN,
                                                double minResidual,
                                                int maxIterations,
                                                double[] interval,
                                                ref int iterationsCount,
                                                ref int criteria,
                                                ref double min,
                                                double[] vector,
                                                ref int error,
                                                double[] supportX,
                                                int supportVectorN,
                                                double[] supportY);

        public string ToLongString(string format)
        {
            string result = knots.ToLongString(format) + "\n\nSpline:\n";

            foreach (var dot in results)
            {
                result += dot.ToString(format) + "\n";
            }

            result += "\nMin of residual - " + minResidual + "\nStopping reason - " + stopType + "\nIterations - " + iterationCount;

            return result;
        }

        public void Save(string filename, string format)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(filename, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(file);
                writer.WriteLine(ToLongString(format));
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }
    }
}