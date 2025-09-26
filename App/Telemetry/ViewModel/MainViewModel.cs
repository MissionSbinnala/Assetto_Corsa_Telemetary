using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telemetry.Data;
using Telemetry.Pages;
using Telemetry.Tools;
using Telemetry.UserControls;
using SlickLibrary;

namespace Telemetry.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Frame Frame { get; set; }
        public List<Page> Pages { get; set; } = new List<Page>();
        public StintPageViewModel StintModel { get; set; } = new StintPageViewModel();
        public UDPViewModel UDPModel { get; set; } = new UDPViewModel();


        public MainViewModel(Frame frame)
        {
            Frame = frame;
            Pages.Add(new StintPage());
            Pages.Add(new UDPPage());
            Frame.Navigate(Pages[0]);
            Pages[0].DataContext = StintModel;
            Pages[1].DataContext = UDPModel;
            SwitchToPageZeroCommand = new RelayCommand(SwitchToPageZero);
            SwitchToPageOneCommand = new RelayCommand(SwitchToPageOne);
            UDPModel.StintSaved += StintModel.RefreshStints;


            //Testing
            Test = new RelayCommand(OnTestClick);
        }

        #region Pages
        public ICommand SwitchToPageZeroCommand { get; set; }
        public void SwitchToPageZero() => Frame.Navigate(Pages[0]);
        public ICommand SwitchToPageOneCommand { get; set; }
        public void SwitchToPageOne()
        {
            Frame.Navigate(Pages[1]);
        }
        #endregion

        public Action RefreshStints;

        public ICommand Test { get; set; }
        private void OnTestClick()
        {
            Debug.WriteLine("OK!");
        }

        #region RangeChangerDeprecated
        //float[] ranges = [0.125f, 0.25f, 0.5f, 1, 1.5f, 2, 3, 4, 5, 6];
        //public ICommand IncreaseRange { get; private set; }
        //public ICommand DecreaseRange { get; private set; }
        //public void OnRangeIncrease() => RangeIndex++;
        //public void OnRangeDecrease() => RangeIndex--;
        //In Construction:
        //IncreaseRange = new RelayCommand(OnRangeIncrease);
        //DecreaseRange = new RelayCommand(OnRangeDecrease);
        #endregion


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
