using FluentAssertions;
using Moq;
using UserInterfaceLogic;

namespace UserInterfaceLogicTests
{
    public class UITests
    {
        [Fact]
        public void arrayCreationTest()
        {
            double[] x = new double[5];
            double[][] y = new double[5][];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = (double) i / (x.Length - 1);
                y[i] = new double[] { x[i] * x[i], 1 };
            }

            var ui = new Mock<IUserInterfaceRealization>();
            ViewData viewData = new ViewData(ui.Object);
            viewData.isNotUniform = false;
            viewData.dataSize = 5;
            viewData.functionType = 0;
            viewData.CreateArray();
            viewData.dataItems.grid.Should().BeEquivalentTo(x);
            viewData.dataItems.field.Should().BeEquivalentTo(y);
        }

        [Fact]
        public void SplineCalculationTest() 
        {
            var ui = new Mock<IUserInterfaceRealization>();
            ViewData viewData = new ViewData(ui.Object);
            viewData.isNotUniform = false;
            viewData.dataSize = 5;
            viewData.functionType = 0;
            viewData.splineGridSize = 3;
            viewData.valueGridSize = 10;
            viewData.StartCalculation(null);

            viewData.results.Should().BeInAscendingOrder(new Comparer());
            double[] diff = new double[5];
            for (int i = 0; i < diff.Length; i++)
            {
                diff[i] = Math.Abs(viewData.results[i][2] - viewData.results[i][1]);
            }
        
            bool same = true;
            for (int i = 0; i < diff.Length; i++)
            {
                if (diff[i] >= 1)
                    same = false;
            }
            same.Should().BeTrue();
        }

        private class Comparer : IComparer<double[]>
        {
            public int Compare(double[]? x, double[]? y)
            {
                return Math.Sign(x[2] - y[2]);
            }
        }

        [Fact]
        public void errorTest()
        {
            var ui = new Mock<IUserInterfaceRealization>();
            ViewData viewData = new ViewData(ui.Object);
            viewData.isNotUniform = false;
            viewData.dataSize = 5;
            viewData.functionType = 0;
            viewData.splineGridSize = 7; //точек сплайна больше, чем заданных
            viewData.valueGridSize = 10;
            viewData.StartCalculation(null);

            ui.Verify(r => r.showErrorDialog(It.IsAny<Exception>()), Times.Once);
        }
    }
}