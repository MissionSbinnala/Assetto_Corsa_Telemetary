using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Tools
{
    public static class ExtensionMethods
    {
        public static void AddPoint(this LineSeries<ObservablePoint> line, float x, float y) => (line.Values as ObservableCollection<ObservablePoint>)?.Add(new ObservablePoint(x, y));
    }
}
