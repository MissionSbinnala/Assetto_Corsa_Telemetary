using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Data
{
    public class LapData : INotifyPropertyChanged
    {
        public ObservableCollection<DataPoint> Points { get; set; } = [];
        public int Lap { get; set; } = 0;
        public double LapTime { get; set; }
        public string TrackName { get; set; } = "";
        public string CarName { get; set; } = "";
        public byte Flags { get; set; }     //76543210  0:IsPitLap 1:Valid
        public bool IsPitLap
        {
            get => (Flags & (1 << 0)) != 0;
            set
            {
                if (value) Flags |= (1 << 0);  // 置位
                else Flags &= 0b11111110; // 清位
            }
        }      // Position 0 => IsPitLap  
        public bool Valid
        {
            get => (Flags & (1 << 1)) != 0;
            set
            {
                if (value) Flags |= (1 << 1);
                else Flags &= 0b11111101;
            }
        }         // Position 1 => Valid

        public LapData() { }
        public LapData(string trackName, string carName, int lap)
        {
            TrackName = trackName;
            CarName = carName;
            Lap = lap;
        }                   //New Lap Constructor
        public LapData(StreamReader stream, string trackName, string carName)
        {
            TrackName = trackName;
            CarName = carName;
            Lap = int.Parse((stream.ReadLine() ?? throw new Exception("Stint Ends Early!"))[4..].Trim(','));
            LapTime = Double.Parse((stream.ReadLine() ?? throw new Exception("Stint Ends Early!"))[8..].Trim(','));
            Flags = byte.Parse((stream.ReadLine() ?? throw new Exception($"Stint Ends Early at Lap {Lap}!"))[6..].Trim(','));
            if ((stream.ReadLine() ?? throw new Exception($"Stint Ends Early at Lap {Lap}!")).Trim(',') is not "Points Start") throw new Exception("No Points Start!");
            while (true)
            {
                var data = stream.ReadLine() ?? throw new Exception($"Stint Ends Early at Lap {Lap}!");
                if (data.Trim(',') is "Points End") break;
                var point = new DataPoint(data.Split(','));
                if (point.InPitLine) IsPitLap = true;
                if (point.Valid) Valid = true;
                Points.Add(point);
            }
            if ((stream.ReadLine() ?? throw new Exception($"Stint Ends Early at Lap {Lap}!")).Trim(',') is not "Lap Ends") throw new Exception($"Lap {Lap} Not Closed Properly!");
        }       //File Constrctor
        public void AddPoint(DataPoint point)
        {
            if (point.InPitLine) IsPitLap = true;
            if (point.Valid) Valid = true;
            Points.Add(point);
            //OnPropertyChanged(nameof(Points));
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Lap Starts");
            sb.AppendLine($"Lap:{Lap.ToString()}");
            sb.AppendLine($"LapTime:{LapTime.ToString()}");
            sb.AppendLine($"Flags:{Flags}");
            sb.AppendLine("Points Start");
            foreach (var item in Points)
                sb.AppendLine(item.ToString());
            sb.AppendLine("Points End");
            sb.Append("Lap Ends");
            return sb.ToString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
