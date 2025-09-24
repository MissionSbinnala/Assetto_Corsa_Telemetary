using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;

namespace Telemetry.ViewModel
{
    public class StintPageViewModel
    {
        public List<ISeries> Generics { get; set; } = new List<ISeries>();
        public List<ISeries> Pedals { get; set; } = new List<ISeries>();
        public List<ISeries> TempsFL { get; set; } = new List<ISeries>();
        public List<ISeries> TempsFR { get; set; } = new List<ISeries>();
        public List<ISeries> TempsRL { get; set; } = new List<ISeries>();
        public List<ISeries> TempsRR { get; set; } = new List<ISeries>();
        public List<ISeries> WheelFL { get; set; } = new List<ISeries>();
        public List<ISeries> WheelFR { get; set; } = new List<ISeries>();
        public List<ISeries> WheelRL { get; set; } = new List<ISeries>();
        public List<ISeries> WheelRR { get; set; } = new List<ISeries>();

        public StintPageViewModel(ObservableCollection<LineSeries<ObservablePoint>> lines)
        {
            Generics.AddRange(lines.Where(line => generic.Contains(line.Name)));//Speed
            Pedals.AddRange(lines.Where(line => pedals.Contains(line.Name)));
            TempsFL.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FL"))));
            TempsFR.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FR"))));
            TempsRL.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RL"))));
            TempsRR.AddRange(lines.Where(line => temps.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RR"))));
            WheelFL.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FL"))));
            WheelFR.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("FR"))));
            WheelRL.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RL"))));
            WheelRR.AddRange(lines.Where(line => wheel.Any(p => line.Name.StartsWith(p) && line.Name.EndsWith("RR"))));
        }

        static HashSet<string> generic = ["Speed", "Gear", "RPM", "SteerAngle", "SurfaceGrip", "TyreGrip"];
        static HashSet<string> pedals = ["Throttle", "Brake", "Clutch"];
        static HashSet<string> temps = ["CoreTemp", "TyreTempI", "TyreTempM", "TyreTempO"];
        static HashSet<string> wheel = [ "AngularSpeed"];
    }
}
