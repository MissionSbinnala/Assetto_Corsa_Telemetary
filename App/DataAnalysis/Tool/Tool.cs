using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentChartApp.Tool
{
    public static class Tool
    {
        public static double Round(this double value, int num) => Math.Round(value, num);

        public static double TimeParse(this string timeStr)
        {
            if (TimeSpan.TryParseExact(timeStr, @"mm\:ss\.fff", null, out TimeSpan time))
                return time.Minutes * 60 + time.Seconds + (time.Milliseconds / 1000);
            else
                Console.WriteLine("时间格式错误");
            return -1;
        }

        public static string TimeParse(this double seconds) => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss\.fff");
    }
}
