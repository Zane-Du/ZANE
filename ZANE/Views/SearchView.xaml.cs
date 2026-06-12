using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZANE.ViewModels;

namespace ZANE.Views
{
    /// <summary>
    /// SearchView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchView : Window
    {
        //public SearchView(SearchViewModel searchViewModel)
        //{
        //    InitializeComponent();
        //    this.DataContext = searchViewModel;

        //}


        public SearchView(MainWindowViewModel searchViewModel)
        {
            InitializeComponent();
            this.DataContext = searchViewModel;

        }
    }
}
