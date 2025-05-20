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
    public CurveData(LineSeries<ObservablePoint> curve) => Curve = curve;
    public CurveData(string name) => Curve = CurveFactory.CreateNewCurve(name);
    public void AddPoint(double x, double y) => (Curve.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y));
    public double GetY(int i) => (Curve.Values as ObservableCollection<ObservablePoint>)?[i].Y ?? 0;
}