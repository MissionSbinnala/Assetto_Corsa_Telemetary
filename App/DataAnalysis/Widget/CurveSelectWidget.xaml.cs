using FluentChartApp;
using FluentChartApp.ViewModels;
using LiveChartsCore.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class CurveSelectWidget : UserControl
    {
        public CurveSelectWidget()
        {
            DataContext = MainWindow.viewModel;
            InitializeComponent();
        }
    }
}

