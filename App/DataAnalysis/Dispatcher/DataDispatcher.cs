using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentChartApp.Data;

namespace FluentChartApp.Dispatcher
{
    public static class DataDispatcher
    {
        private static readonly Dictionary<DataType, Type> _dataMap = new();

        public static void Register()
        {
            Register<TickData>(DataType.Tick);
            Register<LapData>(DataType.Lap);
            Register<PitData>(DataType.Pit);
        }

        // 注册：data enum => 具体类
        public static void Register<T>(DataType data) where T : IData
        {
            _dataMap[data] = typeof(T);
        }

        // 反序列化：根据 enum 的值匹配类型
        public static IData? Deserialize(string json)
        {
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("Type", out var TypeProp))
                throw new Exception("缺少 'Type' 字段");

            // 解析枚举字符串
            var TypeStr = TypeProp.GetString();

            if (!Enum.TryParse<DataType>(TypeStr, ignoreCase: true, out var Type))
                throw new Exception($"无效的 Type 值: {TypeStr}");

            if (!_dataMap.TryGetValue(Type, out var type))
                throw new Exception($"未注册的 Type 类型: {Type}");

            return (IData?)JsonSerializer.Deserialize(json, type);
        }

        public static IData DataFactory(StreamReader reader)
        {
            string json = reader.ReadLine() ?? "";

            IData req = DataDispatcher.Deserialize(json);

            switch (req)
            {
                case TickData tick:
                    return tick;
                case PitData dir:
                    return dir;
                case LapData con:
                    return con;
                default: throw new Exception("Unknown Data!");
            }
        }
    }
}
