using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Win32;
using SlickLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telemetry.Data;
using Telemetry.Tools;
using static System.Net.Mime.MediaTypeNames;

namespace Telemetry.ViewModel
{
    public class UDPViewModel : INotifyPropertyChanged
    {
        public UDPReceiver UDPreceiver { get; set; } = new UDPReceiver();
        public StintData CurrentStint { get; set; } = new StintData();
        public ObservableCollection<LineSeries<ObservablePoint>> AllLines { get; } = [];
        public ObservableCollection<StintData> AllStints { get; set; } = [];
        public TrackData CurrentTrack { get; set; }
        public CarData CurrentCar { get; set; }
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

        public UDPViewModel()
        {
            foreach (var line in CurrentStint.Lines)
                if (line.IsVisible)
                    AllLines.Add(line);
            AllStints.Add(CurrentStint);
            UDPreceiver.OnDataReceived += HandleUDPData;
            UDPreceiver.OnSessionReceived += HandleSessionData;
            UDPButton = new RelayCommand(OnUDPButtonClick);
            Refresh = new RelayCommand(OnRefreshClick);
            ChangeVisiblity = new RelayCommand<LineSeries<ObservablePoint>>(OnVisiblityChange);
            Save = new RelayCommand(OnSaveClick);
            Load = new RelayCommand(OnLoadClick);
            SelectAll = new RelayCommand<StintData>(OnSelectAllClick);
        }



        #region udpLogic
        public void HandleUDPData(string msg)
        {
            var items = msg.Split(',');
            if (items[0] == "Point") CurrentStint.UDPAddPoint(items);
            else if (items[0] == "Lap") items[0] = "bb";
            else if (items[0] == "Pit") items[0] = "cc";
            else Debug.WriteLine($"Other Data:{msg}");//throw new Exception("Data Type Error!!");
        }
        public bool HandleSessionData(string session)
        {
            bool result = false;
            try
            {
                var sections = session.Split(';');
                CurrentTrack = new TrackData(sections[0], double.Parse(sections[1]), int.Parse(sections[2]));
                CurrentCar = new CarData(sections[3], sections[4]);
                OnPropertyChanged(nameof(CurrentCar));
                OnPropertyChanged(nameof(CurrentTrack));
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                result = false;
                IsActive = false;
            }
            return result;
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
            if (File.Exists($"./Data/{CurrentTrack.TrackName}/{CurrentCar.CarName}/") == false)
                Directory.CreateDirectory($"./Data/{CurrentTrack.TrackName}/{CurrentCar.CarName}/");
            var time = DateTime.Now;
            CurrentStint.DateTimeString = time.ToString("yyyy-MM-dd HH-mm-ss");
            File.WriteAllText($"./Data/{CurrentTrack.TrackName}/{CurrentCar.CarName}/{time.ToString("HH-mm-ss yyyy-MM-dd")}.txt", CurrentStint.ToString());
            StintSaved?.Invoke();
        }
        public event Action? StintSaved;

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


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
