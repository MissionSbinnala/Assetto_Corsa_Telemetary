using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Telemetry.Tools;

namespace Telemetry.Data
{
    public class DataPoint
    {
        [Order(1)] public float RelativePosition { get; set; }  // 0.000 ~ 1.000
        [Order(2)] public int Lap { get; set; }
        [Axis(0)][Order(3)] public float Speed { get; set; }             // km/h
        [Axis(1)][Order(4)] public float Throttle { get; set; }          // 0~1
        [Axis(1)][Order(5)] public float Brake { get; set; }             // 0~1
        [Axis(1)][Order(6)] public float Clutch { get; set; }
        [Axis(0)][Order(7)] public float Fuel { get; set; }
        [Axis(0)][Order(8)] public byte Gear { get; set; }
        [Axis(0)][Order(9)] public ushort RPM { get; set; }
        [Axis(0)][Order(10)] public float SteerAngle { get; set; }        // -1~1
        [Axis(0)][Order(11)] public float GX { get; set; }
        [Axis(0)][Order(12)] public float GY { get; set; }
        [Axis(2)][Order(13)] public float[] TyreGrip { get; set; } = new float[4];
        [Axis(0)][Order(14)] public float[] CoreTemp { get; set; } = new float[4];
        [Axis(0)][Order(15)] public float[] TyreTempI { get; set; } = new float[4];
        [Axis(0)][Order(16)] public float[] TyreTempM { get; set; } = new float[4];
        [Axis(0)][Order(17)] public float[] TyreTempO { get; set; } = new float[4];
        [Axis(0)][Order(18)] public float[] AngularSpeed { get; set; } = new float[4];
        [Axis(0)][Order(19)] public float[] TyreLoad { get; set; } = new float[4];
        [Axis(0)][Order(20)] public float SurfaceGrip { get; set; }
        [Axis(0)][Order(21)] public byte Flags { get; set; }     //76543210  0:InPitLine 1:Valid 2:DRS
        public bool InPitLine
        {
            get => (Flags & (1 << 0)) != 0;
            set
            {
                if (value) Flags |= (1 << 0);  // 置位
                else Flags &= 0b11111110; // 清位
            }
        }   // Position 0 => InPitLine
        public bool Valid
        {
            get => (Flags & (1 << 1)) != 0;
            set
            {
                if (value) Flags |= (1 << 1);
                else Flags &= 0b11111101;
            }
        }       // Position 1 => Valid
        public bool DRS
        {
            get => (Flags & (1 << 2)) != 0;
            set
            {
                if (value) Flags |= (1 << 2);
                else Flags &= 0b11111011;
            }
        }         // Position 2 => DRS
        public float ImportanceScore { get; set; }  // 客户端计算，用于裁剪排序

        public float Position => Lap + RelativePosition;

        public DataPoint(float relative, int lap, float speed)
        {
            RelativePosition = relative;
            Lap = lap;
            Speed = speed;
        }   //Temp, Need to be removed
        public DataPoint(float relativePosition, int lap, float speed, float throttle, float brake, float clutch, byte gear, ushort rpm, float steerAngle, float gX, float gY, bool drs, float[] tyreGrip, float[] tyreTempI, float[] tyreTempM, float[] tyreTempO, float[] angularSpeed, float[] tyreLoad, bool inPitLine, bool valid, float surfaceGrip, float importanceScore)
        {
            RelativePosition = relativePosition;
            Lap = lap;
            Speed = speed;
            Throttle = throttle;
            Brake = brake;
            Clutch = clutch;
            Gear = gear;
            RPM = rpm;
            SteerAngle = steerAngle;
            GX = gX;
            GY = gY;
            DRS = drs;
            TyreGrip = tyreGrip;
            TyreTempI = tyreTempI;
            TyreTempM = tyreTempM;
            TyreTempO = tyreTempO;
            AngularSpeed = angularSpeed;
            TyreLoad = tyreLoad;
            InPitLine = inPitLine;
            Valid = valid;
            SurfaceGrip = surfaceGrip;
            ImportanceScore = importanceScore;
        }
        public DataPoint(string[] items, int i = 0)
        {
            if (items.Length < 42) throw new Exception("Point Data Error!");
            RelativePosition = float.Parse(items[i++]);
            Lap = int.Parse(items[i++]);
            Speed = float.Parse(items[i++]);
            Throttle = float.Parse(items[i++]);
            Brake = float.Parse(items[i++]);
            Clutch = float.Parse(items[i++]);
            Fuel = float.Parse(items[i++]);
            Gear = byte.Parse(items[i++]);
            RPM = ushort.Parse(items[i++]);
            SteerAngle = float.Parse(items[i++]);
            GX = float.Parse(items[i++]);
            GY = float.Parse(items[i++]);
            TyreGrip[0] = float.Parse(items[i++]);
            TyreGrip[1] = float.Parse(items[i++]);
            TyreGrip[2] = float.Parse(items[i++]);
            TyreGrip[3] = float.Parse(items[i++]);
            CoreTemp[0] = float.Parse(items[i++]);
            CoreTemp[1] = float.Parse(items[i++]);
            CoreTemp[2] = float.Parse(items[i++]);
            CoreTemp[3] = float.Parse(items[i++]);
            TyreTempI[0] = float.Parse(items[i++]);
            TyreTempI[1] = float.Parse(items[i++]);
            TyreTempI[2] = float.Parse(items[i++]);
            TyreTempI[3] = float.Parse(items[i++]);
            TyreTempM[0] = float.Parse(items[i++]);
            TyreTempM[1] = float.Parse(items[i++]);
            TyreTempM[2] = float.Parse(items[i++]);
            TyreTempM[3] = float.Parse(items[i++]);
            TyreTempO[0] = float.Parse(items[i++]);
            TyreTempO[1] = float.Parse(items[i++]);
            TyreTempO[2] = float.Parse(items[i++]);
            TyreTempO[3] = float.Parse(items[i++]);
            AngularSpeed[0] = float.Parse(items[i++]);
            AngularSpeed[1] = float.Parse(items[i++]);
            AngularSpeed[2] = float.Parse(items[i++]);
            AngularSpeed[3] = float.Parse(items[i++]);
            TyreLoad[0] = float.Parse(items[i++]);
            TyreLoad[1] = float.Parse(items[i++]);
            TyreLoad[2] = float.Parse(items[i++]);
            TyreLoad[3] = float.Parse(items[i++]);
            Flags = (byte)float.Parse(items[i++]);
            ImportanceScore = float.Parse(items[i++]);
        }
        public static string DataHead => "RelativePosition,Lap,Speed,Throttle,Break,Clutch,Fuel,Gear,RPM,SteerAngle,GX,GY,TyreGripFL,TyreGripFR,TyreGripRL,TyreGripRR,CoreTempFL,CoreTempFR,CoreTempRL,CoreTempRR,tyreTempIFL,tyreTempIFR,tyreTempIRL,tyreTempIRR,tyreTempMFL,tyreTempMFR,tyreTempMRL,tyreTempMRR,tyreTempOFL,tyreTempOFR,tyreTempORL,tyreTempORR,angularSpeedFL,angularSpeedFR,angularSpeedRL,angularSpeedRR,tyreLoadFL,tyreLoadFR,tyreLoadRL,tyreLoadRR,Flags,importanceScore";
        public override string ToString()
        {
            #region PropertyDeprecated
            //foreach (var prop in info)
            //{
            //    if (prop.PropertyType == typeof(float[]))
            //    {

            //    }
            //    else
            //    {
            //        var value = prop.GetValue(this);
            //        sb.Append(value);
            //        sb.Append(',');
            //    }
            //}
            //sb.Length--;
            #endregion
            StringBuilder sb = new StringBuilder();
            sb.Append(RelativePosition); sb.Append(',');
            sb.Append(Lap); sb.Append(',');
            sb.Append(Speed); sb.Append(',');
            sb.Append(Throttle); sb.Append(',');
            sb.Append(Brake); sb.Append(',');
            sb.Append(Clutch); sb.Append(',');
            sb.Append(Fuel); sb.Append(",");
            sb.Append(Gear); sb.Append(',');
            sb.Append(RPM); sb.Append(',');
            sb.Append(SteerAngle); sb.Append(',');
            sb.Append(GX); sb.Append(',');
            sb.Append(GY); sb.Append(',');
            sb.Append(TyreGrip[0]); sb.Append(',');
            sb.Append(TyreGrip[1]); sb.Append(',');
            sb.Append(TyreGrip[2]); sb.Append(',');
            sb.Append(TyreGrip[3]); sb.Append(',');
            sb.Append(CoreTemp[0]); sb.Append(',');
            sb.Append(CoreTemp[1]); sb.Append(',');
            sb.Append(CoreTemp[2]); sb.Append(',');
            sb.Append(CoreTemp[3]); sb.Append(',');
            sb.Append(TyreTempI[0]); sb.Append(',');
            sb.Append(TyreTempI[1]); sb.Append(',');
            sb.Append(TyreTempI[2]); sb.Append(',');
            sb.Append(TyreTempI[3]); sb.Append(',');
            sb.Append(TyreTempM[0]); sb.Append(',');
            sb.Append(TyreTempM[1]); sb.Append(',');
            sb.Append(TyreTempM[2]); sb.Append(',');
            sb.Append(TyreTempM[3]); sb.Append(',');
            sb.Append(TyreTempO[0]); sb.Append(',');
            sb.Append(TyreTempO[1]); sb.Append(',');
            sb.Append(TyreTempO[2]); sb.Append(',');
            sb.Append(TyreTempO[3]); sb.Append(',');
            sb.Append(AngularSpeed[0]); sb.Append(',');
            sb.Append(AngularSpeed[1]); sb.Append(',');
            sb.Append(AngularSpeed[2]); sb.Append(',');
            sb.Append(AngularSpeed[3]); sb.Append(',');
            sb.Append(TyreLoad[0]); sb.Append(',');
            sb.Append(TyreLoad[1]); sb.Append(',');
            sb.Append(TyreLoad[2]); sb.Append(',');
            sb.Append(TyreLoad[3]); sb.Append(',');
            sb.Append(Flags); sb.Append(',');
            sb.Append(ImportanceScore);
            return sb.ToString();
        }


        public static PropertyInfo[] info = typeof(DataPoint).GetProperties();
    }
}
