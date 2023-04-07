using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SCADA
{
    public class Settings
    {
        public static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Converters = new List<JsonConverter> { new IPAddressConverter(), new IPEndPointConverter() },
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
    }

    class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IPAddress);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue(((IPAddress)value).ToString());

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => IPAddress.Parse(JToken.Load(reader).Value<string>());
    }

    class IPEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IPEndPoint);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPEndPoint ep = (IPEndPoint)value;
            writer.WriteStartObject();
            writer.WritePropertyName("Address");
            serializer.Serialize(writer, ep.Address);
            writer.WritePropertyName("Port");
            writer.WriteValue(ep.Port);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
            int port = jo["Port"].Value<int>();
            return new IPEndPoint(address, port);
        }
    }
}
