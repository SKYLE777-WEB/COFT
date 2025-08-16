using System;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000173 RID: 371
	public class VersionConverter : JsonConverter
	{
		// Token: 0x060013A0 RID: 5024 RVA: 0x0006E238 File Offset: 0x0006C438
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			if (value is Version)
			{
				writer.WriteValue(value.ToString());
				return;
			}
			throw new JsonSerializationException("Expected Version object value");
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x0006E26C File Offset: 0x0006C46C
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (reader.TokenType == JsonToken.String)
			{
				try
				{
					return new Version((string)reader.Value);
				}
				catch (Exception ex)
				{
					throw JsonSerializationException.Create(reader, "Error parsing version string: {0}".FormatWith(CultureInfo.InvariantCulture, reader.Value), ex);
				}
			}
			throw JsonSerializationException.Create(reader, "Unexpected token or value when parsing version. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x0006E304 File Offset: 0x0006C504
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Version);
		}
	}
}
