using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.VisualElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telemetry.Tools;

namespace Telemetry.Data
{
    public class StintData : INotifyPropertyChanged
    {
        public string TrackName { get; set; } = "";
        public string CarName { get; set; } = "";
        public string DateTimeString { get; set; } = "";
        public string TyreCompound { get; set; } = "";
        public int StartLap { get; set; } = 0;
        public int TotalLaps { get; set; }
        public int MaskNum { get; set; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                    foreach (var line in Lines)
                        line.IsVisible = value;
                _isVisible = value;
            }
        }
        public ObservableCollection<LapData> Laps { get; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> Lines { get; private set; } = [];

        public List<double> Position { get; } = [];
        public List<double> Throttle { get; } = [];
        public List<double> Brake { get; } = [];

        #region PropertyInfo
        private bool _isVisible = false;
        private bool udpInitialized = false;
        #endregion
        public StintData()
        {
            foreach (var prop in PropertyInfos.DataPointInfo)
            {
                string name = prop.Name;
                if (prop.PropertyType == typeof(float[]))
                {
                    Lines.Add(GetNewLine(name + "FL"));
                    Lines.Add(GetNewLine(name + "FR"));
                    Lines.Add(GetNewLine(name + "RL"));
                    Lines.Add(GetNewLine(name + "RR"));
                    continue;
                }
                Lines.Add(GetNewLine(name));
            }
            //foreach (var line in Lines)
            //    if (line.Name?.Length < 8 || line.Name?.Substring(0, 8) is not "TyreGrip")
            //        line.IsVisible = false;
        }
        public StintData(StreamReader stream)
        {
            if ((stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!")).Trim(',') is not "Stint Starts") throw new FileFormatException("Stint Didn't Start!");
            TrackName = (stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!"))[6..].Trim(',');
            CarName = (stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!"))[4..].Trim(',');
            DateTimeString = (stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!"))[5..].Trim(',');
            TyreCompound = (stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!"))[5..].Trim(',');
            StartLap = int.Parse((stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!"))[9..].Trim(','));
            TotalLaps = int.Parse((stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!"))[5..].Trim(','));
            MaskNum = int.Parse((stream.ReadLine() ?? throw new FileFormatException("Stint Ends Early!"))[8..].Trim(','));
            while (true)
            {
                var data = stream.ReadLine() ?? throw new FileFormatException("Stint Didn't End Properly!");
                if (data is "Stint Ends") break;
                Laps.Add(new LapData(stream, TrackName, CarName));
            }
            if (Laps.Count != TotalLaps) Debug.WriteLine("LapNums Don't Match!");//throw new Exception("LapNums Don't Match!");
            foreach (var prop in PropertyInfos.DataPointInfo)
            {
                string name = prop.Name;
                var axis = prop.GetCustomAttribute<AxisAttribute>();
                if (prop.PropertyType == typeof(float[]))
                {
                    CreateTyreLineSet(prop, name, axis);
                }
                else
                    Lines.Add(new LineSeries<ObservablePoint>
                    {
                        Name = name,
                        Values = Laps
                        .SelectMany(lap => lap.Points
                            .Select(p => new ObservablePoint(p.Position, Convert.ToSingle(prop.GetValue(p)))))
                        .ToList(),
                        GeometrySize = 0,
                        LineSmoothness = 0,
                        IsVisible = true,
                        AnimationsSpeed = TimeSpan.Zero,
                        //ScalesYAt = axis.Axis,
                    });
                if (name == "Throttle")
                {
                    Position=Laps.SelectMany(lap => lap.Points.Select(p => (double)p.Position)).ToList();
                    Throttle=Laps.SelectMany(lap => lap.Points.Select(p => (double)(Convert.ToSingle(prop.GetValue(p))))).ToList();
                }
                if (name == "Brake")
                {
                    Position=Laps.SelectMany(lap => lap.Points.Select(p => (double)p.Position)).ToList();
                    Brake=Laps.SelectMany(lap => lap.Points.Select(p => (double)(Convert.ToSingle(prop.GetValue(p))))).ToList();
                }

            }
        }

        private void CreateTyreLineSet(PropertyInfo prop, string name, AxisAttribute? axis)
        {
            Lines.Add(new LineSeries<ObservablePoint>
            {
                Name = name + "FL",
                Values = Laps
                .SelectMany(lap => lap.Points
                    .Select(p => new ObservablePoint(p.Position, ((float[])prop.GetValue(p))[0])))
                .ToList(),
                GeometrySize = 0,
                LineSmoothness = 0,
                IsVisible = true,
                AnimationsSpeed = TimeSpan.Zero,
                //ScalesYAt = axis.Axis,
            });
            Lines.Add(new LineSeries<ObservablePoint>
            {
                Name = name + "FR",
                Values = Laps
                .SelectMany(lap => lap.Points
                    .Select(p => new ObservablePoint(p.Position, ((float[])prop.GetValue(p))[1])))
                .ToList(),
                GeometrySize = 0,
                LineSmoothness = 0,
                IsVisible = true,
                AnimationsSpeed = TimeSpan.Zero,
                //ScalesYAt = axis.Axis,
            });
            Lines.Add(new LineSeries<ObservablePoint>
            {
                Name = name + "RL",
                Values = Laps
                .SelectMany(lap => lap.Points
                    .Select(p => new ObservablePoint(p.Position, ((float[])prop.GetValue(p))[2])))
                .ToList(),
                GeometrySize = 0,
                LineSmoothness = 0,
                IsVisible = true,
                AnimationsSpeed = TimeSpan.Zero,
                //ScalesYAt = axis.Axis,
            });
            Lines.Add(new LineSeries<ObservablePoint>
            {
                Name = name + "RR",
                Values = Laps
                .SelectMany(lap => lap.Points
                    .Select(p => new ObservablePoint(p.Position, ((float[])prop.GetValue(p))[3])))
                .ToList(),
                GeometrySize = 0,
                LineSmoothness = 0,
                IsVisible = true,
                AnimationsSpeed = TimeSpan.Zero,
                //ScalesYAt = axis.Axis,
            });
        }

        public override string ToString()
        {
            StartLap = Laps[0].Lap;
            TotalLaps = Laps.Count;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Stint Starts");
            sb.AppendLine($"Track:{TrackName}");
            sb.AppendLine($"Car:{CarName}");
            sb.AppendLine($"Time:{DateTimeString}");
            sb.AppendLine($"Tyre:{TyreCompound}");
            sb.AppendLine($"StartLap:{StartLap}");
            sb.AppendLine($"Laps:{TotalLaps}");
            sb.AppendLine($"MaskNum:{MaskNum}");
            foreach (var item in Laps.Skip(StartLap))
                sb.AppendLine(item.ToString());
            sb.AppendLine("Stint Ends");
            return sb.ToString();
        }
        public void UDPAddPoint(string[] items)
        {
            var point = new DataPoint(items, 1);
            if (!udpInitialized) StartLap = point.Lap;
            if (point.Lap - StartLap > Laps.Count - 1)
                Laps.Add(new LapData(TrackName, CarName, point.Lap));
            Laps[point.Lap - StartLap].AddPoint(point);
            var props = PropertyInfos.DataPointInfo;
            int i = 3;
            foreach (var prop in props)
            {
                string name = prop.Name;
                if (prop.PropertyType == typeof(float[]))
                {
                    AddPointToLine(i - 3, name + "FL", float.Parse(items[1]) + float.Parse(items[2]), float.Parse(items[i]));
                    i++;
                    AddPointToLine(i - 3, name + "FR", float.Parse(items[1]) + float.Parse(items[2]), float.Parse(items[i]));
                    i++;
                    AddPointToLine(i - 3, name + "RL", float.Parse(items[1]) + float.Parse(items[2]), float.Parse(items[i]));
                    i++;
                    AddPointToLine(i - 3, name + "RR", float.Parse(items[1]) + float.Parse(items[2]), float.Parse(items[i]));
                    i++;
                    continue;
                }
                AddPointToLine(i - 3, name, float.Parse(items[1]) + float.Parse(items[2]), float.Parse(items[i]));
                i++;
            }
        }
        public (DataPoint, DataPoint) GetDataPoint(int lap, int index)
        {
            if (lap == 0) return (Laps[0].Points[0], Laps[1].Points[Laps[1].Points.Count - 1]);
            if (lap == TotalLaps - 1) return (Laps[TotalLaps - 2].Points[0], Laps[TotalLaps - 1].Points[Laps[1].Points.Count - 1]);
            return (Laps[lap - 2].Points[index], Laps[lap].Points[index]);
        }
        private LineSeries<ObservablePoint> GetNewLine(string name) => new LineSeries<ObservablePoint>
        {
            Name = name,
            Values = new ObservableCollection<ObservablePoint>(),
            GeometrySize = 0, // 可选：不显示点，只显示线
            LineSmoothness = 0,
        };
        private void AddPointToLine(int i, string name, float position, float data)
        {
            if (Lines[i].Name == name) Lines[i].AddPoint(position, data);
            else throw new Exception("Line Doesn't Match!");
        }

        public (float, float) GetMinMax(LineSeries<ObservablePoint> line, int lap, int index, double size, float maxValue = 100)
        {
            int leftIndex = lap * MaskNum + index - (int)(size / 2);
            int rightIndex = lap * MaskNum + index + (int)(size / 2);
            bool leftTrend = GetTrend(line, leftIndex);
            bool rightTrend = GetTrend(line, rightIndex);
            var left = (float)((line.Values as ObservableCollection<ObservablePoint>)?[leftIndex].Y ?? 0);
            var right = (float)((line.Values as ObservableCollection<ObservablePoint>)?[rightIndex].Y ?? 0);
            float min = 0, max = 0;

            if (leftTrend && rightTrend)
            {
                min = left;
                max = right;
            }
            else if (leftTrend && !rightTrend)
            {
                min = Math.Min(left, right);
                max = maxValue;
            }
            else if (!leftTrend && !rightTrend)
            {
                min = right;
                max = left;
            }
            else throw new Exception("Across the Stint!");
            return (min, max);
        }

        private bool GetTrend(LineSeries<ObservablePoint> line, int index)
            => (line.Values as ObservableCollection<ObservablePoint>)?[index].Y
            < (line.Values as ObservableCollection<ObservablePoint>)?[index + 1].Y;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
