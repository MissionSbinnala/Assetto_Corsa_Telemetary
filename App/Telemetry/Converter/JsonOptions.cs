using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections;
using System.Text.Json.Serialization;

namespace Telemetry.Converter
{
    public static class JsonOptions
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions();
        public static void Register()
        {
            options.Converters.Add(new BitArrayCompactConverter());
            options.Converters.Add(new TupleListConverter());
        }

    }
}
