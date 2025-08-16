using System;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200016F RID: 367
	public class JavaScriptDateTimeConverter : DateTimeConverterBase
	{
		// Token: 0x06001384 RID: 4996 RVA: 0x0006D7AC File Offset: 0x0006B9AC
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			long num;
			if (value is DateTime)
			{
				num = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(((DateTime)value).ToUniversalTime());
			}
			else
			{
				if (!(value is DateTimeOffset))
				{
					throw new JsonSerializationException("Expected date object value.");
				}
				num = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(((DateTimeOffset)value).ToUniversalTime().UtcDateTime);
			}
			writer.WriteStartConstructor("Date");
			writer.WriteValue(num);
			writer.WriteEndConstructor();
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x0006D830 File Offset: 0x0006BA30
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullable(objectType))
				{
					throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
				}
				return null;
			}
			else
			{
				if (reader.TokenType != JsonToken.StartConstructor || !string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
				{
					throw JsonSerializationException.Create(reader, "Unexpected token or value when parsing date. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
				}
				reader.Read();
				if (reader.TokenType != JsonToken.Integer)
				{
					throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected Integer, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime((long)reader.Value);
				reader.Read();
				if (reader.TokenType != JsonToken.EndConstructor)
				{
					throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected EndConstructor, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				if ((ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType) == typeof(DateTimeOffset))
				{
					return new DateTimeOffset(dateTime);
				}
				return dateTime;
			}
		}
	}
}
