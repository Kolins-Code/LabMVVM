using FluentAssertions;
using Lab3;

namespace Lab3Tests
{
    public class SplineCalculationTests
    {
        private void arrayGenerate (double x, ref double y1, ref double y2)
        {
            y1 = x * x;
            y2 = 1;
        }
        
        [Fact]
        public void arrayGenerationTest()
        {
            double[] x = new double[5];
            double[][] y = new double[5][];
            for (int i = 0; i < x.Length; i++) 
            {
                x[i] = i;
                y[i] = new double[] { i * i, 1 };
            }
            V1DataArray dataItems = new V1DataArray("data", DateTime.Now, x, arrayGenerate);
            dataItems.Should().NotBeNull();
            dataItems.grid.Should().BeEquivalentTo(x);
            dataItems.field.Should().BeEquivalentTo(y);

        }

        [Fact]
        public void splineCalculationTest()
        {
            double[] x = new double[5];
            double[][] y = new double[5][];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = i;
                y[i] = new double[] { i * i, 1 };
            }
            V1DataArray knots = new V1DataArray("data", DateTime.Now, x, arrayGenerate);

            SplineData spline = new SplineData(knots, 3, 1000, new double[] { 0, 4 }, 9);
            spline.Should().NotBeNull();
            spline.calculate(0.0001);

            spline.results.Should().BeInAscendingOrder(new Comparer());
            double[] diff = new double[5];
            for (int i = 0; i < diff.Length; i++)
            {
                diff[i] = Math.Abs(spline.supportResults[i*2][1] - y[i][0]);
            }
           
            bool same = true;
            for (int i = 0; i < diff.Length; i++)
            {
                if (diff[i] >= 1)
                    same = false;
            }
            same.Should().BeTrue();
        }

        private class Comparer : IComparer<SplineDataItem>
        {
            public int Compare(SplineDataItem? x, SplineDataItem? y)
            {
                return Math.Sign(x.y - y.y);
            }
        }
    }
}