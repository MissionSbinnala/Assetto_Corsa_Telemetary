using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;
using Telemetry.Tools;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SlickLibrary;

namespace Telemetry.UserControls
{
    public class StintRecordViewModel: INotifyPropertyChanged
    {
        private bool isActive;
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
        public string Name { get; set; }
        public ICommand Select { get; }
        public StintRecordViewModel() { 
            IsActive = false;
            Name = "Finally Binded!";
            Select = new RelayCommand(OnButtonClick); 
        }


        private void OnButtonClick()
        {
            IsActive = !IsActive;
            Debug.WriteLine(IsActive.ToString());
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
