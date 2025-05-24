using FluentChartApp.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using FluentChartApp.Dispatcher;
using FluentChartApp.Tool;
using FluentChartApp.Data;

namespace FluentChartApp
{
    public partial class MainWindow : Window
    {
        public static MainViewModel viewModel { get; set; } = new MainViewModel();

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_LAYERED = 0x00080000;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out aPoint lpPoint);

        struct aPoint
        {
            public int X;
            public int Y;
        }

        private IntPtr hwnd;

        DispatcherTimer mouseCheckTimer;


        private UdpReceiver receiver;

        public MainWindow()
        {
            DataContext = viewModel;

            InitializeComponent();
            viewModel.chart = MyChart;

            DataDispatcher.Register();
            receiver = new UdpReceiver();
            receiver.OnDataReceived += HandleUdpData;
            receiver.Start(9999); // 监听端口
            MyChart.TooltipPosition = TooltipPosition.Hidden;

            mouseCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            mouseCheckTimer.Tick += CheckMousePosition;
            mouseCheckTimer.Start();


        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            hwnd = new WindowInteropHelper(this).Handle;
            //EnableClickThrough();  // 默认穿透
        }

        private void EnableClickThrough()
        {
            int style = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        /*PreviewMouseLeftButtonDown="DragArea_MouseLeftButtonDown"
     IsHitTestVisible="True" MouseEnter="DragArea_MouseEnter"
MouseLeave="DragArea_MouseLeave"*/

        private void DisableClickThrough()
        {
            int style = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, (style & ~WS_EX_TRANSPARENT) | WS_EX_LAYERED);
        }


        private void CheckMousePosition(object sender, EventArgs e)
        {
            // 获取当前鼠标位置（屏幕坐标）
            GetCursorPos(out aPoint mousePos);

            // 获取我们窗口中可交互控件的屏幕坐标范围
            var relativeTo = MyChart; // 比如是你定义的 Border
            var transform = relativeTo.TransformToAncestor(this);
            var point = transform.Transform(new Point(0, 0));
            var screenPos = PointToScreen(point);

            var areaRect = new Rect(screenPos.X, screenPos.Y, relativeTo.ActualWidth, relativeTo.ActualHeight);

            if (areaRect.Contains(new Point(mousePos.X, mousePos.Y)))
            {
                DisableClickThrough(); // 鼠标在可交互区
            }
            else
            {
                EnableClickThrough(); // 鼠标不在交互区，恢复穿透
            }
        }

        private void DragArea_MouseEnter(object sender, MouseEventArgs e)
        {
            DisableClickThrough();  // 允许鼠标
        }

        private void DragArea_MouseLeave(object sender, MouseEventArgs e)
        {
            EnableClickThrough();   // 恢复穿透
        }

        private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();        // 拖动窗口
        }






        void HandleUdpData(string msg)
        {
            var data = DataDispatcher.Deserialize(msg);
            if (data == null) { return; }
            FluentChartApp.App.Current.Dispatcher.Invoke(() =>
            {
                switch (data)
                {
                    case TickData tick:
                        viewModel.AddPoint(tick);
                        viewModel.LiveXAxis(tick);
                        break;
                    case LapData lap:
                        viewModel.AddRange(lap);
                        break;
                    case PitData pit:
                        
                        break;
                }

            });
        }

        int i = 0;

        void SaveFile(object sender, RoutedEventArgs e)
        {
            MainViewModel? a = DataContext as MainViewModel;
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = "chart_data.csv"
            };

            if (dialog.ShowDialog() == true)
            {

                a?.ExportChartDataToCsv(dialog.FileName);
                MessageBox.Show("导出成功！");
            }
        }

        public void LoadFile(object sender, RoutedEventArgs e)
        {
            MainViewModel? a = DataContext as MainViewModel;
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                CheckFileExists = true,
            };
            if (dialog.ShowDialog() == true)
            {
                a?.ImportChartDataFromCsv(dialog.FileName);
            }
        }

        private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                MyChart.TooltipPosition = TooltipPosition.Auto;
            }
        }

        private void Chart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                MyChart.TooltipPosition = TooltipPosition.Hidden;
            }
        }
    }
}