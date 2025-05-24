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
using LiveChartsCore.SkiaSharpView.WPF;

namespace FluentChartApp.ViewModels
{
    public class MainViewModel
    {
        public CurveCollection Collection { get; } = new();
        public ObservableCollection<RectangularSection> LapRange { get; set; } = [];

        public MainViewModel() { }
        public CartesianChart chart;

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

        private bool _live = false;
        public bool Live
        {
            get => _live;
            set
            {
                _live = value;
                var points = Collection.CurrentStint?.Curves[0].Curve.Values as ObservableCollection<ObservablePoint>;
                if (!value) XAxes[0].MaxLimit = null;
                else if (points?.Count == 0 || points?[points?.Count - 1 ?? 0].X < 2) XAxes[0].MaxLimit = 2;
                else
                {
                    XAxes[0].MinLimit = points?[points?.Count - 1 ?? 0].X - 1;
                    XAxes[0].MaxLimit = points?[points?.Count - 1 ?? 0].X + 1;
                }
            }
        }
        public void LiveXAxis(TickData tick)
        {
            if (Live && tick.Lap > 1)
            {
                XAxes[0].MaxLimit = tick.Lap + 1;
                XAxes[0].MinLimit = tick.Lap - 1;
            }
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
            var data = File.ReadAllLines(filePath);
            Collection.AddStint(new Stint(data, out _));
        }

        public void ExportChartDataToCsv(string filePath)
        {
            File.WriteAllLines(filePath, Collection.CurrentStint?.SaveToFile() ?? []);
        }
    }
}