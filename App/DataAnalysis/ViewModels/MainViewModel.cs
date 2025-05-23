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
using System.ComponentModel;

namespace FluentChartApp.ViewModels
{
    public class MainViewModel
    {
        public CurveCollection Collection { get; } = new();
        public ObservableCollection<RectangularSection> LapRange { get; set; } = [];

        public MainViewModel() { }

        public ObservableCollection<Axis> XAxes { get; set; } = [new Axis
        {
            Name="Lap",
            MinLimit = 0,
        }];
        public ObservableCollection<Axis> YAxes { get; set; } = [new Axis
        {
            MaxLimit = 100,
        },new Axis{
            Position = LiveChartsCore.Measure.AxisPosition.End,
        }];
        public void Change(bool visible)
        {
            YAxes[1].IsVisible = visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
            var data = File.ReadAllLines(filePath);
            Collection.AddStint(new Stint(data, out _));
        }

        public void ExportChartDataToCsv(string filePath)
        {
            File.WriteAllLines(filePath, Collection.CurrentStint?.SaveToFile() ?? []);
        }
    }
}