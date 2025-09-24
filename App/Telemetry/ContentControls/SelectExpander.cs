using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Telemetry.Tools;
using LiveChartsCore.Measure;
using LiveChartsCore.Drawing;

namespace Telemetry.ContentControls
{
    public class SelectExpander : ContentControl
    {
        static SelectExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SelectExpander),
                new FrameworkPropertyMetadata(typeof(SelectExpander))
            );
        }

        // 你可以定义属性，比如是否展开、标题等
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SelectExpander), new PropertyMetadata(string.Empty));

        public Brush BackGround
        {
            get { return (Brush)GetValue(BackGroundProperty); }
            set { SetValue(BackGroundProperty, value); }
        }

        public static readonly DependencyProperty BackGroundProperty =
            DependencyProperty.Register("BackGround", typeof(Brush), typeof(SelectExpander), new PropertyMetadata(Brushes.Transparent));

        public ObservableCollection<LineSeries<ObservablePoint>> LineSource
        {
            get { return (ObservableCollection<LineSeries<ObservablePoint>>)GetValue(LineSourceProperty); }
            set { SetValue(LineSourceProperty, value); }
        }

        public static readonly DependencyProperty LineSourceProperty =
            DependencyProperty.Register("LineSource", typeof(ObservableCollection<LineSeries<ObservablePoint>>), typeof(SelectExpander), new PropertyMetadata(new ObservableCollection<LineSeries<ObservablePoint>>()));

        public double SetCornerRadius
        {
            set { SetValue(SetCornerRadiusProperty, new CornerRadius(value)); }
        }

        public static readonly DependencyProperty SetCornerRadiusProperty =
            DependencyProperty.Register("SetCornerRadius", typeof(double), typeof(SelectExpander), new PropertyMetadata(0.0));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SelectExpander), new PropertyMetadata(new CornerRadius(0)));
        // 你还可以定义其他属性，例如展现内容、命令等
    }

}
