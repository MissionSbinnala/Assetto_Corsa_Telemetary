using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Tools
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AxisAttribute : Attribute
    {
        public int Axis { get; } = 0;
        public AxisAttribute(int axis) => Axis=axis;
    }
}

