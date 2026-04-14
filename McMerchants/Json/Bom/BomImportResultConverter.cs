using McMerchantsLib.Models.Bom;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McMerchants.Json.Bom
{
    public class BomImportResultConverter : JsonConverter<BomImportResultDTO>
    {
        public override BomImportResultDTO Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, BomImportResultDTO value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("id", value.Bom.Id);
            writer.WriteString("name", value.Bom.Name);

            writer.WritePropertyName("rejected");
            WriteRejectedLines(writer, value.UnreadableLines);

            writer.WriteEndObject();
        }

        private static void WriteRejectedLines(Utf8JsonWriter writer, ICollection<Tuple<string, string>> rejects)
        {
            writer.WriteStartArray();

            foreach (var reject in rejects)
            {
                writer.WriteStartObject();
                writer.WriteString("line", reject.Item1);
                writer.WriteString("cause", reject.Item2);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
