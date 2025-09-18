using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Data
{
    public static class GraphAxes
    {
        public static Axis AutoAxis = new Axis();
        public static Axis PedalAxis = new Axis { MaxLimit = 1, MinLimit = 0 };
        public static Axis GripAxis= new Axis { MaxLimit = 100,MinLimit = 0 };
        public static Axis[] Axes = [AutoAxis, PedalAxis, GripAxis];
    }
}
