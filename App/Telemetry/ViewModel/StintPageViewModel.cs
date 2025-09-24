using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using SlickLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Telemetry.Data;
using Telemetry.Tools;

namespace Telemetry.ViewModel
{
    public class StintPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<LineSeries<ObservablePoint>> _generics = [];
        public ObservableCollection<LineSeries<ObservablePoint>> Generics { get => _generics; set { _generics = value; OnPropertyChanged(); } }
        public ObservableCollection<LineSeries<ObservablePoint>> Pedals { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> Wears { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> TempsFL { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> TempsFR { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> TempsRL { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> TempsRR { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> WheelFL { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> WheelFR { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> WheelRL { get; set; } = [];
        public ObservableCollection<LineSeries<ObservablePoint>> WheelRR { get; set; } = [];
        public StintPageViewModel() { }

        public ICommand Test => new RelayCommand(TestFunc);
        StintData data;


        public void TestFunc()
        {
            ReadStint("S:\\Projects\\Assetto_Corsa_Telemetary\\App\\Telemetry\\bin\\Debug\\net8.0-windows7.0\\Data\\Temp1.txt");
            Debug.WriteLine(data.ToString().Substring(0, 100));
        }

        public void ReadStint(string path)
        {
            using (var reader = new StreamReader(path))
            {
                data = new StintData(reader);
                ClearData();
                GetLinesFromRecord(data);

            }
        }

        public void GetLinesFromRecord(StintData data)
        {
            var lines = data.Lines;
            Generics.AddRange(lines.Where(line => generic.Contains(line.Name)));//Speed
            Pedals.AddRange(lines.Where(line => pedals.Contains(line.Name)));
            Wears.AddRange(lines.Where(line => wear.Any(p=>line.Name.StartsWith(p))));
            TempsFL.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FL"))));
            TempsFR.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FR"))));
            TempsRL.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RL"))));
            TempsRR.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RR"))));
            WheelFL.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FL"))));
            WheelFR.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FR"))));
            WheelRL.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RL"))));
            WheelRR.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RR"))));
        }
        private void ClearData()
        {
            Generics.Clear();
            Pedals.Clear();
            TempsFL.Clear();
            TempsFR.Clear();
            TempsRL.Clear();
            TempsRR.Clear();
            WheelFL.Clear();
            WheelFR.Clear();
            WheelRL.Clear();
            WheelRR.Clear();
        }
        #region TestSection
        public Axis[] XAxis { get; set; } = [new Axis()];
        #endregion



        static HashSet<string> generic = ["Speed", "Gear", "RPM", "SteerAngle", "SurfaceGrip"];
        static HashSet<string> pedals = ["Throttle", "Brake", "Clutch"];
        static HashSet<string> temps = ["CoreTemp", "TyreTempI", "TyreTempM", "TyreTempO"];
        static HashSet<string> wheel = ["AngularSpeed"];
        static HashSet<string> wear = ["TyreGrip"];

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}