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
        public UDPReceiver UDPreceiver { get; set; } = new UDPReceiver();
        public StintData CurrentStint { get; set; } = new StintData();
        public ObservableCollection<LineSeries<ObservablePoint>> AllLines { get; } = [];
        public ObservableCollection<StintData> AllStints { get; set; } = [];
        public TrackData CurrentTrack { get; set; }
        public CarData CurrentCar { get; set; }
        public ChartsViewModel chartsViewModel { get; set; }
        private bool isActive = false;
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Page> Pages { get; set; } = new List<Page>();
        public Frame Frame { get; set; }


        public MainViewModel(Frame frame)
        {
            Frame = frame;
            foreach (var line in CurrentStint.Lines)
                if (line.IsVisible)
                    AllLines.Add(line);
            AllStints.Add(CurrentStint);
            UDPreceiver.OnDataReceived += HandleUDPData;
            UDPButton = new RelayCommand(OnUDPButtonClick);
            Refresh = new RelayCommand(OnRefreshClick);
            Test = new RelayCommand(OnTestClick);
            ChangeVisiblity = new RelayCommand<LineSeries<ObservablePoint>>(OnVisiblityChange);
            Save = new RelayCommand(OnSaveClick);
            Load = new RelayCommand(OnLoadClick);
            SelectAll = new RelayCommand<StintData>(OnSelectAllClick);


            //Testing
            chartsViewModel = new ChartsViewModel(this);
            Pages.Add(new StintPage());
            Pages.Add(new UDPPage());
            Frame.Navigate(Pages[0]);
            SwitchToPageZeroCommand = new RelayCommand(SwitchToPageZero);
            SwitchToPageOneCommand = new RelayCommand(SwitchToPageOne);
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

        #region udpLogic
        public void HandleUDPData(string msg)
        {
            var items = msg.Split(',');
            if (items[0] == "Point") CurrentStint.UDPAddPoint(items);
            else if (items[0] == "Lap") items[0] = "bb";
            else if (items[0] == "Pit") items[0] = "cc";
            else Debug.WriteLine($"Other Data:{msg}");//throw new Exception("Data Type Error!!");
        }

        public ICommand UDPButton { get; set; }
        private void OnUDPButtonClick()
        {
            IsActive = !IsActive;
            if (IsActive)
            {
                UDPreceiver.Start(9999);
                Debug.WriteLine("Start");
            }
            else
            {
                UDPreceiver.Stop();
                Debug.WriteLine("Stop");
            }
        }
        #endregion
        public ICommand Refresh { get; set; }
        float aaa = 0;
        private void OnRefreshClick()
        {
            //CurrentStint.Laps[0].Points.Add(new DataPoint(aaa, 0, 20));
            //aaa += 0.1f;
            //CurrentStint=new StintData();
            OnPropertyChanged(nameof(CurrentStint));
            Debug.WriteLine("Refresh");
        }

        public ICommand ChangeVisiblity { get; set; }
        private void OnVisiblityChange(LineSeries<ObservablePoint> series)
        {
            if (series == null)
                throw new ArgumentNullException(nameof(series));
            series.IsVisible = !series.IsVisible;
            if (series.IsVisible)
                AllLines.Add(series);
            else AllLines.Remove(series);
        }

        public ICommand Save { get; set; }
        private void OnSaveClick()
        {
            File.WriteAllText("./Data/Temp.txt", CurrentStint.ToString());
        }

        public ICommand Load { get; set; }
        private void OnLoadClick()
        {
            var fileFialog = new OpenFileDialog();
            fileFialog.ShowDialog();
            //using (FileStream fileStream = new FileStream("./Data/Temp.txt", FileMode.Open))
            using (Stream file = fileFialog.OpenFile())
            using (StreamReader reader = new StreamReader(file))
            {
                AllStints.Add(new StintData(reader));
                OnPropertyChanged(nameof(AllStints));
            }
        }

        public ICommand SelectAll { get; set; }
        private void OnSelectAllClick(StintData stint)
        {
            stint.IsVisible = !stint.IsVisible;
            if (stint.IsVisible)
            {
                foreach (var line in stint.Lines)
                    if (!AllLines.Contains(line))
                        AllLines.Add(line);
            }
            else
                foreach (var line in stint.Lines)
                    if (AllLines.Contains(line))
                        AllLines.Remove(line);
        }

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
