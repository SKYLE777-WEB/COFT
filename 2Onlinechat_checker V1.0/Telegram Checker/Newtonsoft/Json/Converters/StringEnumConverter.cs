using System;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000172 RID: 370
	public class StringEnumConverter : JsonConverter
	{
		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001397 RID: 5015 RVA: 0x0006E008 File Offset: 0x0006C208
		// (set) Token: 0x06001398 RID: 5016 RVA: 0x0006E010 File Offset: 0x0006C210
		public bool CamelCaseText { get; set; }

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06001399 RID: 5017 RVA: 0x0006E01C File Offset: 0x0006C21C
		// (set) Token: 0x0600139A RID: 5018 RVA: 0x0006E024 File Offset: 0x0006C224
		public bool AllowIntegerValues { get; set; }

		// Token: 0x0600139B RID: 5019 RVA: 0x0006E030 File Offset: 0x0006C230
		public StringEnumConverter()
		{
			this.AllowIntegerValues = true;
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x0006E040 File Offset: 0x0006C240
		public StringEnumConverter(bool camelCaseText)
			: this()
		{
			this.CamelCaseText = camelCaseText;
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x0006E050 File Offset: 0x0006C250
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Enum @enum = (Enum)value;
			string text = @enum.ToString("G");
			if (!char.IsNumber(text[0]) && text[0] != '-')
			{
				string text2 = EnumUtils.ToEnumName(@enum.GetType(), text, this.CamelCaseText);
				writer.WriteValue(text2);
				return;
			}
			if (!this.AllowIntegerValues)
			{
				throw JsonSerializationException.Create(null, writer.ContainerPath, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, text), null);
			}
			writer.WriteValue(value);
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x0006E0EC File Offset: 0x0006C2EC
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Null)
			{
				bool flag = ReflectionUtils.IsNullableType(objectType);
				Type type = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
				try
				{
					if (reader.TokenType == JsonToken.String)
					{
						return EnumUtils.ParseEnumName(reader.Value.ToString(), flag, !this.AllowIntegerValues, type);
					}
					if (reader.TokenType == JsonToken.Integer)
					{
						if (!this.AllowIntegerValues)
						{
							throw JsonSerializationException.Create(reader, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, reader.Value));
						}
						return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, type);
					}
				}
				catch (Exception ex)
				{
					throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(reader.Value), objectType), ex);
				}
				throw JsonSerializationException.Create(reader, "Unexpected token {0} when parsing enum.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			if (!ReflectionUtils.IsNullableType(objectType))
			{
				throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			return null;
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x0006E218 File Offset: 0x0006C418
		public override bool CanConvert(Type objectType)
		{
			return (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType).IsEnum();
		}
	}
}
