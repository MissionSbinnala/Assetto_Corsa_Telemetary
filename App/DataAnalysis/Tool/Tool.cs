using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// 对任意 IList<T> 进行二分查找，返回值的索引或插入点。
        /// </summary>
        public static int BinarySearch<T>(
            this IList<T>? list,
            T value,
            IComparer<T>? comparer = null)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            comparer ??= Comparer<T>.Default;

            int low = 0, high = list.Count - 1;

            while (low <= high)
            {
                int mid = low + ((high - low) >> 1);
                int cmp = comparer.Compare(list[mid], value);
                if (cmp == 0) return mid;
                if (cmp < 0) low = mid + 1;
                else high = mid - 1;
            }

            return ~low; // 插入点
        }

    }
}
