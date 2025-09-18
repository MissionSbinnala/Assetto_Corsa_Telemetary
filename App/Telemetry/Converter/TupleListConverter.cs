using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace Telemetry.Converter
{
    public class TupleListConverter : JsonConverter<List<(double, double)>>
    {
        public override List<(double, double)> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string list = reader.GetString();
            list = list.Trim('[', ']');
            if (string.IsNullOrEmpty(list))
                return new List<(double, double)>();
            return list.Split(';').Select(s => ToTuple(s)).ToList();
        }

        private static (double, double) ToTuple(string tuple)
        {
            tuple = tuple.Trim('(', ')');
            if (string.IsNullOrWhiteSpace(tuple))
                return (0, 0);
            var select=tuple.Split(',')
                  .Select(s => double.Parse(s.Trim(), CultureInfo.InvariantCulture));
            return new(select.ElementAt(0), select.ElementAt(1));
        }

        public override void Write(Utf8JsonWriter writer, List<(double, double)> value, JsonSerializerOptions options)
        {
            var sb = new StringBuilder();
            sb.Append('[');
            foreach (var item in value)
            {
                sb.Append(item.ToString());
                sb.Append(';');
            }
            if (sb.Length > 0)
                sb.Length--;
            sb.Append(']');
            writer.WriteStringValue(sb.ToString());
        }
    }
}
