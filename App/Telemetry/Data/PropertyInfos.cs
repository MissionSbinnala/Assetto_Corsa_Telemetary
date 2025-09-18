using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telemetry.Tools;

namespace Telemetry.Data
{
    public static class PropertyInfos
    {
        public static PropertyInfo[] DataPointInfo { get; set; } = typeof(DataPoint)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new
            {
                Prop = p,
                Order = p.GetCustomAttribute<OrderAttribute>()?.Index ?? int.MaxValue
            })
            .Where(p => p.Order >= 3 && p.Order <= 21)
            .OrderBy(p => p.Order).Select(p => p.Prop).ToArray();

    }
}
