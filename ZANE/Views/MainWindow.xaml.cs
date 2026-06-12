using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZANE.ViewModels;

namespace ZANE.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            this.DataContext = mainWindowViewModel;

            this.Closing += (s, e) =>
            {
                if (DataContext is MainWindowViewModel vm)
                    vm.Cleanup();
            };
        }
    }
}