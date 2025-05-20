using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Diagnostics;
using System.Windows.Media;
using LiveChartsCore.Painting;
using FluentChartApp.Tool;
using System.Windows.Shapes;
using FluentChartApp.Data;

namespace FluentChartApp.ViewModels
{
    public class MainViewModel
    {
        public CurveCollection Collection { get; } = new();
        public ObservableCollection<RectangularSection> LapRange { get; set; }

        public MainViewModel()
        {
            LapRange = new ObservableCollection<RectangularSection>();
            ImportChartDataFromCsv("A");
        }

        public void AddPoint(TickData tick)
        {
            Collection.CurrentStint?.AddPoint(tick);
        }

        public void AddRange(LapData lap)
        {
            var lapRange = new RectangularSection
            {
                Xi = lap.Distance,     // 起点 X
                Xj = lap.Distance,     // 终点 X，相等表示竖线
                Stroke = new SolidColorPaint(SKColors.Red, 2),
                Label = $"Lap {lap.Lap}", // 显示的标签
                LabelPaint = new SolidColorPaint(SKColors.Red),
                IsVisible = true,
            };
            LapRange.Add(lapRange);
            Collection.CurrentStint?.AddLap(lap);
        }

        public void ImportChartDataFromCsv(string filePath)
        {
            var FL = CurveFactory.CreateNewCurve("RecordedFL");
            var FR = CurveFactory.CreateNewCurve("RecordedFR");
            var RL = CurveFactory.CreateNewCurve("RecordedRL");
            var RR = CurveFactory.CreateNewCurve("RecordedRR");
            filePath = "C:\\Users\\76292\\Desktop\\chart_data1.csv";
            var lines = File.ReadAllLines(filePath);

            /*foreach (var line in lines.Skip(1)) // 跳过表头
            {
                var parts = line.Split(',');

                if (parts.Length == 5 &&
                    double.TryParse(parts[0], out double x) &&
                    double.TryParse(parts[1], out double y1)&&
                    double.TryParse(parts[2], out double y2)&&
                    double.TryParse(parts[3], out double y3)&& 
                    double.TryParse(parts[4], out double y4)
                    )
                {
                    (FL.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y1));
                    (FR.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y2));
                    (RL.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y3));
                    (RR.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y4));
                }
            }*/



            int samplingStep = 5; // 每 5 行取 1 行
            for (int i = 1; i < lines.Length; i += samplingStep) // 跳过表头 + 采样
            {
                var parts = lines[i].Split(',');

                if (parts.Length == 5 &&
                    double.TryParse(parts[0], out double x) &&
                    double.TryParse(parts[1], out double y1) &&
                    double.TryParse(parts[2], out double y2) &&
                    double.TryParse(parts[3], out double y3) &&
                    double.TryParse(parts[4], out double y4))
                {
                    (FL.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y1));
                    (FR.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y2));
                    (RL.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y3));
                    (RR.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y4));
                }
            }

            //collection.AddStint(new Stint(0));
        }

        public void ExportChartDataToCsv(string filePath)
        {
            File.WriteAllLines(filePath, Collection.CurrentStint?.SaveToFile() ?? []);
        }

        //public void Add(ObservablePoint point) => (TireWearSeries[0].Values as ObservableCollection<ObservablePoint>)?.Add(point);
    }
}