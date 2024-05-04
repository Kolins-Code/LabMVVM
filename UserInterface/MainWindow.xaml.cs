using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UserInterfaceLogic;


namespace UserInterface
{
    public partial class MainWindow : Window, IUserInterfaceRealization
    {
        ViewData source { get; set; }

        public static RoutedCommand CalculateCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();

            source = new ViewData(this);
            DataContext = source;
            
            Binding bindingInterval = new Binding();
            bindingInterval.Path = new PropertyPath("interval");
            bindingInterval.Converter = new IntervalConverter();
            bindingInterval.ValidatesOnDataErrors = true;
            TextBox_interval.SetBinding(TextBox.TextProperty, bindingInterval);
        }

        public string getFilePathByDialog()
        {
            Microsoft.Win32.SaveFileDialog ofd = new Microsoft.Win32.SaveFileDialog();
            ofd.Filter = " *.bin| *.bin";
            if (ofd.ShowDialog() == true)
            {
                return ofd.FileName;
            }
            return "";
        }

        public void showErrorDialog(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}