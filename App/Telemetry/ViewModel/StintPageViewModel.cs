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
        public ObservableCollection<LineSeries<ObservablePoint>> Generics { get; set; } = [];
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
        public FileNode CurrentTrack { get; set; }
        public FileNode CurrentCar { get; set; }
        public FileNode CurrentStint { get; set; }
        public List<FileNode> Tracks { get; set; } = [];
        public List<FileNode> Cars { get; set; } = [];
        public List<FileNode> Stints { get; set; } = [];

        public StintPageViewModel()
        {
            StintChoosingInit();
            RefreshStints();
        }

        public void RefreshStints()
        {
            var data = FileTreeBuilder.BuildSeparated("./Data", extend: false);
            Tracks = data.GetAllFolders().ToList();
            CurrentTrack = Tracks.First();
            Cars = CurrentTrack.GetAllFolders().ToList();
            CurrentCar = Cars.First();
            Stints = CurrentCar.GetAllFiles("txt").ToList();
            OnPropertyChanged(nameof(Tracks));
            OnPropertyChanged(nameof(Cars));
            OnPropertyChanged(nameof(Stints));
        }

        #region StintChoosing
        public void StintChoosingInit()
        {
            GetCar = new RelayCommand<FileNode>(GetCarFunc);
            GetStint = new RelayCommand<FileNode>(GetStintFunc);
            LoadStint = new RelayCommand<FileNode>(LoadStintFunc);
        }
        public ICommand GetCar { get; set; }
        public void GetCarFunc(FileNode node)
        {
            CurrentTrack = node;
            Cars = node.GetAllFolders().ToList();
            OnPropertyChanged(nameof(Cars));
            if (Cars.Count != 0) {                 
                CurrentCar = Cars.First();
                Stints = CurrentCar.GetAllFiles("txt").ToList();
            }
            else
            {
                CurrentCar = null;
                Stints = new List<FileNode>();
            }
            OnPropertyChanged(nameof(Stints));
        }
        public ICommand GetStint { get; set; }
        private void GetStintFunc(FileNode node)
        {
            CurrentCar = node;
            Stints = node.GetAllFiles("txt").ToList();
            OnPropertyChanged(nameof(Stints));
        }
        public ICommand LoadStint { get; set; }
        private void LoadStintFunc(FileNode node)
        {
            if (CurrentStint is not null&&CurrentStint.Equals(node))
                return;
            CurrentStint = node;
            StringBuilder sb = new StringBuilder();
            sb.Append("./Data/");
            sb.Append(CurrentTrack.Name);
            sb.Append("/");
            sb.Append(CurrentCar.Name);
            sb.Append("/");
            sb.Append(node.Name);
            ReadStint(sb.ToString());
        }
        #endregion

        public ICommand Test => new RelayCommand(TestFunc);
        StintData data;


        public void TestFunc()
        {
            ReadStint("./Data/testTrack/testCar/Temp1.txt");
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
            Wears.AddRange(lines.Where(line => wear.Any(p => line.Name.StartsWith(p))));
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