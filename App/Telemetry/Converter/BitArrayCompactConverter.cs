using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Telemetry.Converter
{
    public class BitArrayCompactConverter : JsonConverter<BitArray>
    {
        public override BitArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string base64 = reader.GetString();
            byte[] bytes = Convert.FromBase64String(base64);
            return new BitArray(bytes);
        }

        public override void Write(Utf8JsonWriter writer, BitArray value, JsonSerializerOptions options)
        {
            byte[] bytes = new byte[(value.Length + 7) / 8];
            value.CopyTo(bytes, 0);
            string base64 = Convert.ToBase64String(bytes);
            writer.WriteStringValue(base64);
        }
    }

}
