using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentChartApp.Tool
{
    public static class CurveFactory
    {
        private static readonly SKColor[] ColorPalette = new SKColor[]
        {
            SKColors.Red,
            SKColors.Blue,
            SKColors.Green,
            SKColors.Orange,
            SKColors.Purple,
            SKColors.Crimson,
            SKColors.Cyan,
            SKColors.Magenta,
            SKColors.Yellow,
            SKColors.Orchid,
            SKColors.Purple,
            SKColors.Aqua,
            SKColors.Aquamarine,
            SKColors.Brown,
            SKColors.Maroon,
            SKColors.Chartreuse,
        };
        private static int colorIndex = 0;

        public static LineSeries<ObservablePoint> CreateNewCurve(string name, int axis = 0)
        {
            var strokeColor = ColorPalette[colorIndex % ColorPalette.Length];
            colorIndex++; // 循环使用颜色列表

            return new LineSeries<ObservablePoint>
            {
                Name = name,
                Values = new ObservableCollection<ObservablePoint>(),
                Stroke = new SolidColorPaint(strokeColor, 2), // 设置线宽为2，可选
                //Fill = null, // 可选：不使用填充
                GeometrySize = 0,
                YToolTipLabelFormatter = point => $"{point.Coordinate}",
                Tag = name.Split("-")[0],
                ScalesYAt = axis,
            };
        }

    }
}
