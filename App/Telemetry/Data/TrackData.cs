using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using Telemetry.Converter;

namespace Telemetry.Data
{
    public class TrackData
    {
        public string TrackName { get; set; }
        public double TrackLength { get; set; }
        public int SamplingCount { get; set; }
        public double SamplingDelta { get; set; }
        public BitArray DefaultSamplingMask { get; set; }
        public bool IsSamplingMaskInitialized { get; set; }
        public TrackData() { }

        public TrackData(string name, double length, int count)
        {
            TrackName = name;
            TrackLength = length;
            SamplingCount = count;
            SamplingDelta = 1 / count;
            DefaultSamplingMask = new BitArray(SamplingCount);
            IsSamplingMaskInitialized = false;
        }

        public bool SaveToFile()
        {
            string jsonString = JsonSerializer.Serialize(this, JsonOptions.options);
            if (!File.Exists($"./Data/{TrackName}/TrackData.json"))
                Directory.CreateDirectory($"./Data/{TrackName}");
            File.WriteAllText($"./Data/{TrackName}/TrackData.json", jsonString);
            return true;
        }

        public static TrackData ReadFromFile(string trackName)
        {
            if (!File.Exists($"./Data/{trackName}/TrackData.json"))
                throw new Exception("Track does not exist!");
            return JsonSerializer.Deserialize<TrackData>(File.ReadAllText($"./Data/{trackName}/TrackData.json"), JsonOptions.options) ?? throw new Exception("Failed to load! Data corrupted!");
        }

        public void Print()
        {
            Debug.WriteLine(TrackName);
            Debug.WriteLine(TrackLength);
            Debug.WriteLine(SamplingCount);
            Debug.WriteLine(SamplingDelta);
            Debug.WriteLine(IsSamplingMaskInitialized);
        }
    }
}
