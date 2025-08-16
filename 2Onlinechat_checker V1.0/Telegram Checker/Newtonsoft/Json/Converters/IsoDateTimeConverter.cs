using System;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200016E RID: 366
	public class IsoDateTimeConverter : DateTimeConverterBase
	{
		// Token: 0x17000366 RID: 870
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x0006D47C File Offset: 0x0006B67C
		// (set) Token: 0x0600137C RID: 4988 RVA: 0x0006D484 File Offset: 0x0006B684
		public DateTimeStyles DateTimeStyles
		{
			get
			{
				return this._dateTimeStyles;
			}
			set
			{
				this._dateTimeStyles = value;
			}
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x0006D490 File Offset: 0x0006B690
		// (set) Token: 0x0600137E RID: 4990 RVA: 0x0006D4A4 File Offset: 0x0006B6A4
		public string DateTimeFormat
		{
			get
			{
				return this._dateTimeFormat ?? string.Empty;
			}
			set
			{
				this._dateTimeFormat = (string.IsNullOrEmpty(value) ? null : value);
			}
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x0600137F RID: 4991 RVA: 0x0006D4C0 File Offset: 0x0006B6C0
		// (set) Token: 0x06001380 RID: 4992 RVA: 0x0006D4D4 File Offset: 0x0006B6D4
		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? CultureInfo.CurrentCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		// Token: 0x06001381 RID: 4993 RVA: 0x0006D4E0 File Offset: 0x0006B6E0
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string text;
			if (value is DateTime)
			{
				DateTime dateTime = (DateTime)value;
				if ((this._dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (this._dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
				{
					dateTime = dateTime.ToUniversalTime();
				}
				text = dateTime.ToString(this._dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", this.Culture);
			}
			else
			{
				if (!(value is DateTimeOffset))
				{
					throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith(CultureInfo.InvariantCulture, ReflectionUtils.GetObjectType(value)));
				}
				DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
				if ((this._dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (this._dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
				{
					dateTimeOffset = dateTimeOffset.ToUniversalTime();
				}
				text = dateTimeOffset.ToString(this._dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", this.Culture);
			}
			writer.WriteValue(text);
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x0006D5D4 File Offset: 0x0006B7D4
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = ReflectionUtils.IsNullableType(objectType);
			if (reader.TokenType == JsonToken.Null)
			{
				if (!ReflectionUtils.IsNullableType(objectType))
				{
					throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
				}
				return null;
			}
			else
			{
				Type type = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
				if (reader.TokenType == JsonToken.Date)
				{
					if (type == typeof(DateTimeOffset))
					{
						if (!(reader.Value is DateTimeOffset))
						{
							return new DateTimeOffset((DateTime)reader.Value);
						}
						return reader.Value;
					}
					else
					{
						if (reader.Value is DateTimeOffset)
						{
							return ((DateTimeOffset)reader.Value).DateTime;
						}
						return reader.Value;
					}
				}
				else
				{
					if (reader.TokenType != JsonToken.String)
					{
						throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
					}
					string text = reader.Value.ToString();
					if (string.IsNullOrEmpty(text) && flag)
					{
						return null;
					}
					if (type == typeof(DateTimeOffset))
					{
						if (!string.IsNullOrEmpty(this._dateTimeFormat))
						{
							return DateTimeOffset.ParseExact(text, this._dateTimeFormat, this.Culture, this._dateTimeStyles);
						}
						return DateTimeOffset.Parse(text, this.Culture, this._dateTimeStyles);
					}
					else
					{
						if (!string.IsNullOrEmpty(this._dateTimeFormat))
						{
							return DateTime.ParseExact(text, this._dateTimeFormat, this.Culture, this._dateTimeStyles);
						}
						return DateTime.Parse(text, this.Culture, this._dateTimeStyles);
					}
				}
			}
		}

		// Token: 0x040006FA RID: 1786
		private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

		// Token: 0x040006FB RID: 1787
		private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;

		// Token: 0x040006FC RID: 1788
		private string _dateTimeFormat;

		// Token: 0x040006FD RID: 1789
		private CultureInfo _culture;
	}
}
