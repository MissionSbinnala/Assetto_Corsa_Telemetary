using FluentChartApp.Tool;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace FluentChartApp.Data
{
    public class CurveCollection
    {
        public Stint? CurrentStint { get; set; } = new Stint(0);
        List<Stint> Session { get; set; } = [];
        public ObservableCollection<Stint> StintCollection { get; } = [];
        public Dictionary<string, CurveGroup> Category { get; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> AllCurves { get; } = [];
        public CurveCollection()
        {
            Session.Add(CurrentStint);
            AddStint(CurrentStint);
        }
        public void AddStint(Stint stint)
        {
            StintCollection.Add(stint);
            foreach (var curve in stint.Curves)
            {
                string tag = curve.Curve.Tag as string ?? "";
                if (!Category.ContainsKey(tag)) Category.Add(tag, new CurveGroup(tag));
                Category[tag].AddCurve(curve);
                AllCurves.Add(curve.Curve);
            }
        }
        public void RemoveStint(Stint stint)
        {
            StintCollection.Remove(stint);
            foreach (var curve in stint.Curves)
            {
                string tag = curve.Curve.Tag as string ?? "";
                Category[tag].RemoveCurve(curve);
                AllCurves.Remove(curve.Curve);
            }
        }
        public void StintStarts(double distance) => CurrentStint = new Stint(distance);
        public void StintEnds() => CurrentStint = null;
        public bool Availiable => CurrentStint is not null;

        public void ReadFile(string filePath)   //Currently Only Stint
        {
            var data = File.ReadAllLines(filePath);
            AddStint(new Stint(data, out _));
        }
    }

    public class Stint
    {
        public double StartDistance { get; set; }
        public int RecordDistance { get; }
        int i = 0;
        public ObservableCollection<CurveData> Curves { get; } = [];
        List<LapData> Laps { get; set; } = [];
        public Stint() { }
        public Stint(double startDistance) //Primary
        {
            StartDistance = startDistance;
            RecordDistance = ((int)startDistance / 10) * 10;
            InitializeCurves();
        }

        public Stint(string[] data, out int remain, int i = 0)
        {
            StartDistance = double.Parse(data[1]);
            RecordDistance = (int)StartDistance / 10 * 10;
            i += 2;
            while (data[i] != "Laps Ends")
            {
                Laps.Add(new LapData(data[i]));
                i++;
            }
            i += 2;
            var points = data.Skip(i);
            InitializeCurves();
            foreach (var point in points)
            {
                if (point == "Stint Ends") break;
                var y = point.Split(',');
                for (int j = 1; j < 7; j++) Curves[j - 1].AddPoint(double.Parse(y[0]), double.Parse(y[j]));
                i++;
            }
            if (data.Length > i) remain = i;
            else remain = -1;
        }

        public void AddPoint(TickData tick)
        {
            double x = tick.Lap.Round(4);

            Curves[0].AddPoint(x, tick.TyreWear[0].Round(4));
            Curves[1].AddPoint(x, tick.TyreWear[1].Round(4));
            Curves[2].AddPoint(x, tick.TyreWear[2].Round(4));
            Curves[3].AddPoint(x, tick.TyreWear[3].Round(4));
            Curves[4].AddPoint(x, tick.Fuel.Round(4));
            Curves[5].AddPoint(x, tick.Speed.Round(4));

            i++;
        }

        public string GetData(int i) => $"{Curves[0].GetX(i)},{Curves[0].GetY(i)},{Curves[1].GetY(i)},{Curves[2].GetY(i)},{Curves[3].GetY(i)},{Curves[4].GetY(i)},{Curves[5].GetY(i)}";

        public void AddLap(LapData lap) => Laps.Add(lap);

        public List<string> SaveToFile()
        {
            List<string> CSV = ["Stint Starts"];
            CSV.Add(StartDistance.ToString());
            foreach (var item in Laps) CSV.Add(item.ToString());
            CSV.Add("Laps Ends");

            CSV.Add("Lap,FL,FR,RL,RR,Fuel,Speed");
            double distance = RecordDistance;
            for (int j = 0; j < i; j++)
            {
                distance += 10;

                CSV.Add($"{GetData(j)}");
            }
            CSV.Add("Stint Ends");
            return CSV;
        }

        private void InitializeCurves()
        {
            Curves.Add(new CurveData("FL"));
            Curves.Add(new CurveData("FR"));
            Curves.Add(new CurveData("RL"));
            Curves.Add(new CurveData("RR"));
            Curves.Add(new CurveData("Fuel"));
            Curves.Add(new CurveData("Speed", 1));
        }
    }

    public class CurveGroup : INotifyPropertyChanged
    {
        public string GroupName { get; set; }

        public ObservableCollection<CurveData> Curves { get; set; } = [];

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

                    MainWindow.viewModel.Change(value);
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
        public void AddCurve(CurveData curve) => Curves.Add(curve);
        public void RemoveCurve(CurveData curve) => Curves.Remove(curve);
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
        public CurveData(string name, int axis = 0) => Curve = CurveFactory.CreateNewCurve(name, axis);
        public void AddPoint(double x, double y) => (Curve.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y));
        public double GetX(int i) => (Curve.Values as ObservableCollection<ObservablePoint>)?[i].X ?? 0;
        public double GetY(int i) => (Curve.Values as ObservableCollection<ObservablePoint>)?[i].Y ?? 0;
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataType { Tick, Lap, Pit }

    public interface IData
    {
        public DataType Type { get; set; }
    }

    public class TickData : IData
    {
        public DataType Type { get; set; }
        public double Speed { get; set; }
        public List<double> TyreWear { get; set; }
        public double Distance { get; set; }
        public double Lap { get; set; }
        public double Fuel { get; set; }
        public TickData() => Type = DataType.Tick;
    }

    public class LapData : IData
    {
        public DataType Type { get; set; }
        public int Lap { get; set; }
        public string LapTime { get; set; }
        public double LapTimeSeconds { get; set; }
        public double Distance { get; set; }
        public double LapDistance { get; set; }


        public LapData()
        {
            Type = DataType.Lap;
            Distance = 0;
            Lap = 0;
        }
        public LapData(string data)
        {
            var parts = data.Split(',');
            if (parts.Length == 5 &&
                    int.TryParse(parts[0], out int x) &&
                    double.TryParse(parts[2], out double y2) &&
                    double.TryParse(parts[3], out double y3) &&
                    double.TryParse(parts[4], out double y4)
                    )
            {
                Lap = x;
                LapTime = parts[1];
                LapTimeSeconds = y2;
                Distance = y3;
                LapDistance = y4;
            }
        }
        public LapData(LapData data)
        {
            Type = DataType.Lap;
            Lap = data.Lap;
            Distance = data.Distance;
            LapTime = data.LapTime;
            LapDistance = data.LapDistance;
            LapTimeSeconds = data.LapTimeSeconds;
        }
        public new string ToString() => $"{Lap},{LapTime},{LapTimeSeconds},{Distance},{LapDistance}";
    }

    public class PitData : IData
    {
        public DataType Type { get; set; }
        public PitStatus Status { get; set; }

        public PitData() => Type = DataType.Pit;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PitStatus { InPit, OutPit, OutPitBox, QuickPit }
}
