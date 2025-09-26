using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
using Telemetry.ContentControls;
using Telemetry.Converter;
using Telemetry.Data;
using Telemetry.UserControls;
using Telemetry.ViewModel;
using Path = System.IO.Path;

namespace Telemetry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TrackData track;
        CarData car;
        public MainWindow()
        {
            InitializeComponent();
            JsonOptions.Register();

            LiveCharts.UseGPU = true;

            string trackName = "testTrack";
            string carName = "testCar";

            if (!(Directory.Exists($"./Data/{trackName}") && File.Exists($"./Data/{trackName}/TrackData.json")))
            {
                track = new TrackData("testTrack", 1000, 1000);
                track.SaveToFile();
            }
            else
                track = TrackData.ReadFromFile(trackName);
            if (!(Directory.Exists($"./Data/{trackName}/{carName}") && File.Exists($"./Data/{trackName}/{carName}/CarData.json")))
            {
                car = new CarData("testTrack", "testCar");
                car.SaveToFile();
            }
            else
                car = CarData.ReadFromFile(trackName, carName);

            var vw = new MainViewModel(MainFrame);

            //track.Print();
            //car.Print();
            //var data = new StintData();
            //for (int i = 0; i < 10; i++)
            //{
            //    var lap = new LapData();
            //    lap.Lap = i;
            //    for (float j = 0; j < 1000; j++)
            //        lap.Points.Add(new DataPoint(j / 1000, i, i * 1000 + j));
            //    data.Laps.Add(lap);
            //}
            //vw.Data.Add(data);
            //TestChart.Series = vw.Data[0].SpeedSeries;
            DataContext = vw;
            //TestChart.Series = vw.CurrentStint.TyreWearSeries;
            //TestChart.FindingStrategy = FindingStrategy.CompareOnlyXTakeClosest;
        }

    }
}