namespace DiffManager.Common.Converters
{
    using System;

    using Newtonsoft.Json;

    public class Base64FileJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value is string readerValue ? Convert.FromBase64String(readerValue) : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToBase64String(value as byte[] ?? new byte[0]));
        }
    }
}
