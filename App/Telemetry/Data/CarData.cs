using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Telemetry.Converter;

namespace Telemetry.Data
{
    public class CarData
    {
        public string TrackName { get; set; } = "";
        public string CarName { get; set; } = "";
        public string Temp { get; set; } = "";
        [JsonConverter(typeof(TupleListConverter))]
        public List<(double, double)> TyreCurves { get; set; }= [];
        public CarData() { }

        public CarData(string trackName, string carName, string t)
        {
            TrackName = trackName;
            CarName = carName;
            TyreCurves = [(1.2, 1.2), (1.2, 1.2), (1.2, 1.2)];
            Temp = t;

        }
        public bool SaveToFile()
        {
            var jsonString = JsonSerializer.Serialize(this);
            if (!Directory.Exists($"./Data/{TrackName}/{CarName}"))
                Directory.CreateDirectory($"./Data/{TrackName}/{CarName}");
            File.WriteAllText($"./Data/{TrackName}/{CarName}/CarData.json", jsonString);
            return true;
        }
        public static CarData ReadFromFile(string trackName, string carName)
        {
            if (!File.Exists($"./Data/{trackName}/TrackData.json"))
                throw new Exception("Track does not exist!");
            return JsonSerializer.Deserialize<CarData>(File.ReadAllText($"./Data/{trackName}/{carName}/CarData.json"), JsonOptions.options) ?? throw new Exception("Failed to load! Data corrupted!");
        }
        public void Print()
        {
            Debug.WriteLine(TrackName);
            Debug.WriteLine(CarName);
            foreach (var car in TyreCurves)
            {
                Debug.WriteLine(car.ToString());
            }
        }

    }
}
//$"./Data/{TrackName}/{CarName}/CarData.json"
