using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telemetry.Tools;
using Telemetry.ViewModel;
using SlickLibrary;

namespace Telemetry.UserControls
{
    /*public class ChartsViewModel : INotifyPropertyChanged
    {
        public MainViewModel MainModel { get; set; }
        public int _index = 0;
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    AllLines = MainModel.AllStints.Select(stint => stint.Lines[_index]);
                    OnPropertyChanged();
                }
            }
        }
        private IEnumerable<LineSeries<ObservablePoint>> allLines;
        public IEnumerable<LineSeries<ObservablePoint>> AllLines
        {
            get => allLines;
            set
            {
                allLines = value;
                OnPropertyChanged();
            }
        }

        public ChartsViewModel(MainViewModel viewModel)
        {
            MainModel = viewModel;
            AllLines = MainModel.AllStints.Select(stint => stint.Lines[_index]);
            Increase = new RelayCommand(IncreaseAction);
            Decrease = new RelayCommand(DecreaseAction);
            MainModel.PropertyChanged += OnStintsRefreshing;
        }

        public ICommand Increase { get; set; }
        public void IncreaseAction()
        {
            Index = (Index + 1) % 20;
            Debug.WriteLine("Increase" + Index);
        }

        public ICommand Decrease { get; set; }
        public void DecreaseAction()
        {
            Index = (Index - 1) % 20;
            Debug.WriteLine("Decrease" + Index);
        }

        public void OnStintsRefreshing(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if ((sender as MainViewModel).Equals(MainModel) && eventArgs.PropertyName.Equals(nameof(MainModel.AllStints)))
                AllLines = MainModel.AllStints.Select(stint => stint.Lines[_index]);
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }*/
}
