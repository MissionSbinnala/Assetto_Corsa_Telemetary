#region CurveGroups
/* 
 CurveGroups.cs
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;
using System.ComponentModel;
using LiveChartsCore.SkiaSharpView;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FluentChartApp.Tool;
public class CurveGroups
{
    public LineSeries<ObservablePoint>[] Telemetry { get; } = new LineSeries<ObservablePoint>[4];
    public Dictionary<string, CurveGroup> Groups { get; } = new Dictionary<string, CurveGroup>();
    public ObservableCollection<ISeries> AllCurves { get; } = new ObservableCollection<ISeries>();

    public CurveGroups()
    {
        Telemetry[0] = CurveFactory.CreateNewCurve("FL");
        Telemetry[1] = CurveFactory.CreateNewCurve("FR");
        Telemetry[2] = CurveFactory.CreateNewCurve("RL");
        Telemetry[3] = CurveFactory.CreateNewCurve("RR");

        AddCurve("FL", Telemetry[0]);
        AddCurve("FR", Telemetry[1]);
        AddCurve("RL", Telemetry[2]);
        AddCurve("RR", Telemetry[3]);
    }

    public void AddCurve(string group, LineSeries<ObservablePoint> curve)
    {
        if (!Groups.ContainsKey(group)) Groups.Add(group, new CurveGroup(group));
        Groups[group].Curves.Add(new CurveData(curve));
        AllCurves.Add(curve);
    }
}



public class CurveGroup : INotifyPropertyChanged
{
    public string GroupName { get; set; }

    public ObservableCollection<CurveData> Curves { get; set; } = new();

    private bool _isVisible = true;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                foreach (var curve in Curves)
                    curve.Curve.IsVisible = value;

                OnPropertyChanged(nameof(IsVisible));
                VisibilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler VisibilityChanged;

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public CurveGroup() { }
    public CurveGroup(string name) { GroupName = name; }
}

public class CurveData
{
    public LineSeries<ObservablePoint> Curve { get; set; }
    public Brush StrokeBrush
    {
        get
        {
            // 下面假设你用的是 SolidColorPaint
            if (Curve.Stroke is SolidColorPaint solidColor)
            {
                var color = solidColor.Color;
                return new SolidColorBrush(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));
            }
            return Brushes.Transparent;
        }
    }
    public CurveData(LineSeries<ObservablePoint> curve)
    {
        Curve = curve;
    }
}
 */
#endregion

#region CurveSelectWidget
/*<UserControl x:Class="WpfApp1.CurveSelectWidget"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             MinHeight="100" MinWidth="150">
    <Border BorderBrush="LightGray" BorderThickness="1" Padding="5">
        <ScrollViewer VerticalScrollBarVisibility="Auto" HorizontalScrollBarVisibility="Disabled" MaxHeight="300">
            <ItemsControl ItemsSource="{Binding curveGroups.Groups.Values}">
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <CheckBox IsChecked="{Binding IsVisible, Mode=TwoWay}" Margin="5">
                            <StackPanel Orientation="Horizontal">
                                <ItemsControl ItemsSource="{Binding Curves}">
                                    <ItemsControl.ItemsPanel>
                                        <ItemsPanelTemplate>
                                            <StackPanel Orientation="Horizontal"/>
                                        </ItemsPanelTemplate>
                                    </ItemsControl.ItemsPanel>
                                    <ItemsControl.ItemTemplate>
                                        <DataTemplate>
                                            <Ellipse Width="12" Height="12" Fill="{Binding StrokeBrush}" Margin="0,-4,6,0"/>
                                        </DataTemplate>
                                    </ItemsControl.ItemTemplate>
                                </ItemsControl>
                                <TextBlock Text="{Binding  GroupName}" VerticalAlignment="Center"/>
                            </StackPanel>
                        </CheckBox>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>

        </ScrollViewer>
    </Border>
</UserControl>
*/
#endregion

/* MainViewModel
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

namespace FluentChartApp.ViewModels
{
    public class MainViewModel
    {
        public CurveGroups curveGroups { get; } = new();
        public ObservableCollection<RectangularSection> LapRange { get; set; }
        List<string> CSVHeadder = new List<string>() { "Distance,TyreWear_FL,TyreWear_FR,TyreWear_RL,TyreWear_RR" };

        public MainViewModel()
        {
            LapRange = new ObservableCollection<RectangularSection>();
            ImportChartDataFromCsv("A");
        }

        public void AddPoint(TickData tick)
        {
            double distance = tick.Distance.Round(0);
            FluentChartApp.App.Current.Dispatcher.Invoke(() =>
            {
                (curveGroups.Groups["FL"].Curves[0].Curve.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(distance, tick.TyreWear[0].Round(4)));
                (curveGroups.Groups["FR"].Curves[0].Curve.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(distance, tick.TyreWear[1].Round(4)));
                (curveGroups.Groups["RL"].Curves[0].Curve.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(distance, tick.TyreWear[2].Round(4)));
                (curveGroups.Groups["RR"].Curves[0].Curve.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(distance, tick.TyreWear[3].Round(4)));
                //(TireWearSeries[4].Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(distance, tick.Fuel.Round(2)));
            });
            string temp = $"{distance.ToString()},{tick.TyreWear[0].Round(4)},{tick.TyreWear[1].Round(4)},{tick.TyreWear[2].Round(4)},{tick.TyreWear[3].Round(4)},{tick.Fuel.Round(2)}";
            CSVHeadder.Add(temp);
        }

        public void AddRange(double start, double end, int lap)
        {
            var lapRange = new RectangularSection
            {
                Xi = start,     // 起点 X
                Xj = end,     // 终点 X，相等表示竖线
                Stroke = new SolidColorPaint(SKColors.Red, 2),
                Label = $"Lap {lap}", // 显示的标签
                LabelPaint = new SolidColorPaint(SKColors.Red),
                IsVisible = true,
            };
            LapRange.Add(lapRange);
        }

        public void AddCurves()
        {

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
            }

using LiveChartsCore.Defaults;
using System.Collections.ObjectModel;
using System.Windows.Shapes;

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

curveGroups.AddCurve("FL", FL);
curveGroups.AddCurve("FR", FR);
curveGroups.AddCurve("RL", RL);
curveGroups.AddCurve("RR", RR);
        }

        public void ExportChartDataToCsv(string filePath)
{
    File.WriteAllLines(filePath, CSVHeadder);
}

        //public void Add(ObservablePoint point) => (TireWearSeries[0].Values as ObservableCollection<ObservablePoint>)?.Add(point);
    }
}
 */