using System.ComponentModel;
using OxyPlot.Series;
using Lab3;
using OxyPlot;
using OxyPlot.Legends;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UserInterfaceLogic
{
    public class ViewData : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public double[] interval { get; set; }
        public int dataSize { get; set; }
        public bool isNotUniform { get; set; }
        public int functionType { get; set; }
        public int splineGridSize { get; set; }
        public int valueGridSize { get; set; }
        public double norm { get; set; }
        public int maxIterations { get; set; }
        public ObservableCollection<double[]> results { get; set; }
        public List<double[]> supportResults { get; set; }

        public SplineData spline { get; set; }
        public V1DataArray dataItems { get; set; }

        public PlotModel Graph { get; set; }

        public ICommand loadAndCalculateCommand { get; private set; }
        public ICommand saveCommand { get; private set; }
        public ICommand calculateCommand { get; private set; }

        private IUserInterfaceRealization ui;

        public string Error
        {
            get
            {
                return "Ошибка в данных";
            }
        }

        public string this[string property]
        {
            get
            {
                string message = string.Empty;
                switch (property)
                {
                    case "dataSize":
                        if (dataSize < 3)
                            message = "Должно быть задано минимум три точки аппроксимируемой функции";
                        break;
                    case "splineGridSize":
                        if (splineGridSize < 2 || splineGridSize > dataSize)
                            message = "Количество точек сетки сплайна не должно превышать количество точек сетки функции";
                        break;
                    case "valueGridSize":
                        if (valueGridSize <= 3)
                            message = "Должно быть задано минимум четыре точки, в которых будет вычислен сплайн";
                        break;
                    case "interval":
                        if (interval[1] <= interval[0])
                            message = "Концы отрезка заданы неправильно";
                        break;
                }
                return message;
            }
        }

        public ViewData(IUserInterfaceRealization ui)
        {
            interval = new double[2];
            interval[0] = 0;
            interval[1] = 1;
            norm = 0.0001;
            maxIterations = 1000;
            results = new ObservableCollection<double[]>();

            this.ui = ui;
            saveCommand = new RelayCommand(Save, _ => string.IsNullOrEmpty(this["interval"])
                                                      && string.IsNullOrEmpty(this["dataSize"]));
            loadAndCalculateCommand = new RelayCommand(Load, _ => true);
            calculateCommand = new RelayCommand(StartCalculation, _ => string.IsNullOrEmpty(this["interval"])
                                                                       && string.IsNullOrEmpty(this["dataSize"])
                                                                       && string.IsNullOrEmpty(this["splineGridSize"])
                                                                       && string.IsNullOrEmpty(this["valueGridSize"]));
        }

        private void arrayGenerate(double x, ref double y1, ref double y2)
        {
            switch (functionType)
            {
                case 0:
                    y1 = x * x;
                    break;
                case 1:
                    y1 = x * x * x;
                    break;
                case 2:
                    y1 = 1 / (x + 1);
                    break;
            }

            y2 = 1;
        }

        public void CreateArray()
        {
            double[] x = new double[dataSize];
            for (int i = 1; i < dataSize - 1; i++)
            {
                if (isNotUniform)
                {
                    Random rand = new Random();
                    x[i] = rand.NextDouble() * (interval[1] - interval[0]) + interval[0];
                }
                else
                {
                    x[i] = interval[0] + (interval[1] - interval[0]) / (dataSize - 1) * i;
                }

            }
            x[0] = interval[0];
            x[dataSize - 1] = interval[1];
            Array.Sort(x);

            dataItems = new V1DataArray("data", DateTime.Now, x, arrayGenerate);
        }

        public void Calculate()
        {
            double[] interval = new double[2] { dataItems.grid.Min(), dataItems.grid.Max() };
            spline = new SplineData(dataItems, splineGridSize, maxIterations, interval, valueGridSize);
            spline.calculate(norm);
            results.Clear();
            foreach (var item in spline.results)
            {
                results.Add(new double[] { item.x, item.y, item.ySplined });
            }
            supportResults = spline.supportResults;

            Graph = new PlotModel
            {
                Title = "Сплайн"
            };
            var splineGraph = new LineSeries()
            {
                Color = OxyColors.Red,
                Title = "Сглаживающий сплайн"
            };
            var dataGraph = new LineSeries()
            {
                Color = OxyColors.Transparent,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Blue,
                MarkerSize = 2,
                Title = "Заданные точки"

            };
            foreach (var point in supportResults)
            {
                splineGraph.Points.Add(new DataPoint(point[0], point[1]));
            }
            foreach (var point in dataItems)
            {
                dataGraph.Points.Add(new DataPoint(point.x, point.y1));
            }
            Graph.Series.Add(splineGraph);
            Graph.Series.Add(dataGraph);
            Graph.Legends.Add(new Legend()
            {
                LegendPosition = LegendPosition.TopCenter
            });

            if (PropertyChanged != null)
            {
                RaisePropertyChanged("supportResults");
                RaisePropertyChanged("Graph");
            }
            
        }

        public void Save(object sender)
        {
            try
            {
                CreateArray();
                string path = ui.getFilePathByDialog();
                dataItems.Save(path);
            } 
            catch (Exception ex)
            {
                ui.showErrorDialog(ex);
            }
            
        }

        public void Load(object sender)
        {
            try
            {
                string path = ui.getFilePathByDialog();
                V1DataArray? tmp = null;
                V1DataArray.Load(path, ref tmp);
                dataItems = tmp;
                Calculate();
            }
            catch (Exception ex)
            {
                ui.showErrorDialog(ex);
            }
        }

        public void StartCalculation(object sender)
        {
            try
            {
                CreateArray();
                Calculate();
            }
            catch (Exception ex)
            {
                ui.showErrorDialog(ex);
            }
        }


    }
}