using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000A3 RID: 163
	public static class JsonConvert
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x00038EFC File Offset: 0x000370FC
		// (set) Token: 0x06000792 RID: 1938 RVA: 0x00038F04 File Offset: 0x00037104
		public static Func<JsonSerializerSettings> DefaultSettings { get; set; }

		// Token: 0x06000793 RID: 1939 RVA: 0x00038F0C File Offset: 0x0003710C
		public static string ToString(DateTime value)
		{
			return JsonConvert.ToString(value, DateFormatHandling.IsoDateFormat, DateTimeZoneHandling.RoundtripKind);
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00038F18 File Offset: 0x00037118
		public static string ToString(DateTime value, DateFormatHandling format, DateTimeZoneHandling timeZoneHandling)
		{
			DateTime dateTime = DateTimeUtils.EnsureDateTime(value, timeZoneHandling);
			string text;
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				stringWriter.Write('"');
				DateTimeUtils.WriteDateTimeString(stringWriter, dateTime, format, null, CultureInfo.InvariantCulture);
				stringWriter.Write('"');
				text = stringWriter.ToString();
			}
			return text;
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x00038F80 File Offset: 0x00037180
		public static string ToString(DateTimeOffset value)
		{
			return JsonConvert.ToString(value, DateFormatHandling.IsoDateFormat);
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x00038F8C File Offset: 0x0003718C
		public static string ToString(DateTimeOffset value, DateFormatHandling format)
		{
			string text;
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
			{
				stringWriter.Write('"');
				DateTimeUtils.WriteDateTimeOffsetString(stringWriter, value, format, null, CultureInfo.InvariantCulture);
				stringWriter.Write('"');
				text = stringWriter.ToString();
			}
			return text;
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x00038FEC File Offset: 0x000371EC
		public static string ToString(bool value)
		{
			if (!value)
			{
				return JsonConvert.False;
			}
			return JsonConvert.True;
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x00039000 File Offset: 0x00037200
		public static string ToString(char value)
		{
			return JsonConvert.ToString(char.ToString(value));
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00039010 File Offset: 0x00037210
		public static string ToString(Enum value)
		{
			return value.ToString("D");
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00039020 File Offset: 0x00037220
		public static string ToString(int value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00039030 File Offset: 0x00037230
		public static string ToString(short value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00039040 File Offset: 0x00037240
		[CLSCompliant(false)]
		public static string ToString(ushort value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00039050 File Offset: 0x00037250
		[CLSCompliant(false)]
		public static string ToString(uint value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00039060 File Offset: 0x00037260
		public static string ToString(long value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00039070 File Offset: 0x00037270
		private static string ToStringInternal(BigInteger value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00039080 File Offset: 0x00037280
		[CLSCompliant(false)]
		public static string ToString(ulong value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x00039090 File Offset: 0x00037290
		public static string ToString(float value)
		{
			return JsonConvert.EnsureDecimalPlace((double)value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x000390AC File Offset: 0x000372AC
		internal static string ToString(float value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
		{
			return JsonConvert.EnsureFloatFormat((double)value, JsonConvert.EnsureDecimalPlace((double)value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x000390D0 File Offset: 0x000372D0
		private static string EnsureFloatFormat(double value, string text, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
		{
			if (floatFormatHandling == FloatFormatHandling.Symbol || (!double.IsInfinity(value) && !double.IsNaN(value)))
			{
				return text;
			}
			if (floatFormatHandling != FloatFormatHandling.DefaultValue)
			{
				return quoteChar.ToString() + text + quoteChar.ToString();
			}
			if (nullable)
			{
				return JsonConvert.Null;
			}
			return "0.0";
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x00039130 File Offset: 0x00037330
		public static string ToString(double value)
		{
			return JsonConvert.EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x0003914C File Offset: 0x0003734C
		internal static string ToString(double value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
		{
			return JsonConvert.EnsureFloatFormat(value, JsonConvert.EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x00039170 File Offset: 0x00037370
		private static string EnsureDecimalPlace(double value, string text)
		{
			if (double.IsNaN(value) || double.IsInfinity(value) || text.IndexOf('.') != -1 || text.IndexOf('E') != -1 || text.IndexOf('e') != -1)
			{
				return text;
			}
			return text + ".0";
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x000391D0 File Offset: 0x000373D0
		private static string EnsureDecimalPlace(string text)
		{
			if (text.IndexOf('.') != -1)
			{
				return text;
			}
			return text + ".0";
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x000391F0 File Offset: 0x000373F0
		public static string ToString(byte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x00039200 File Offset: 0x00037400
		[CLSCompliant(false)]
		public static string ToString(sbyte value)
		{
			return value.ToString(null, CultureInfo.InvariantCulture);
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00039210 File Offset: 0x00037410
		public static string ToString(decimal value)
		{
			return JsonConvert.EnsureDecimalPlace(value.ToString(null, CultureInfo.InvariantCulture));
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x00039224 File Offset: 0x00037424
		public static string ToString(Guid value)
		{
			return JsonConvert.ToString(value, '"');
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x00039230 File Offset: 0x00037430
		internal static string ToString(Guid value, char quoteChar)
		{
			string text = value.ToString("D", CultureInfo.InvariantCulture);
			string text2 = quoteChar.ToString(CultureInfo.InvariantCulture);
			return text2 + text + text2;
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x00039268 File Offset: 0x00037468
		public static string ToString(TimeSpan value)
		{
			return JsonConvert.ToString(value, '"');
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x00039274 File Offset: 0x00037474
		internal static string ToString(TimeSpan value, char quoteChar)
		{
			return JsonConvert.ToString(value.ToString(), quoteChar);
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x0003928C File Offset: 0x0003748C
		public static string ToString(Uri value)
		{
			if (value == null)
			{
				return JsonConvert.Null;
			}
			return JsonConvert.ToString(value, '"');
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x000392A8 File Offset: 0x000374A8
		internal static string ToString(Uri value, char quoteChar)
		{
			return JsonConvert.ToString(value.OriginalString, quoteChar);
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x000392B8 File Offset: 0x000374B8
		public static string ToString(string value)
		{
			return JsonConvert.ToString(value, '"');
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x000392C4 File Offset: 0x000374C4
		public static string ToString(string value, char delimiter)
		{
			return JsonConvert.ToString(value, delimiter, StringEscapeHandling.Default);
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x000392D0 File Offset: 0x000374D0
		public static string ToString(string value, char delimiter, StringEscapeHandling stringEscapeHandling)
		{
			if (delimiter != '"' && delimiter != '\'')
			{
				throw new ArgumentException("Delimiter must be a single or double quote.", "delimiter");
			}
			return JavaScriptUtils.ToEscapedJavaScriptString(value, delimiter, true, stringEscapeHandling);
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x000392FC File Offset: 0x000374FC
		public static string ToString(object value)
		{
			if (value == null)
			{
				return JsonConvert.Null;
			}
			switch (ConvertUtils.GetTypeCode(value.GetType()))
			{
			case PrimitiveTypeCode.Char:
				return JsonConvert.ToString((char)value);
			case PrimitiveTypeCode.Boolean:
				return JsonConvert.ToString((bool)value);
			case PrimitiveTypeCode.SByte:
				return JsonConvert.ToString((sbyte)value);
			case PrimitiveTypeCode.Int16:
				return JsonConvert.ToString((short)value);
			case PrimitiveTypeCode.UInt16:
				return JsonConvert.ToString((ushort)value);
			case PrimitiveTypeCode.Int32:
				return JsonConvert.ToString((int)value);
			case PrimitiveTypeCode.Byte:
				return JsonConvert.ToString((byte)value);
			case PrimitiveTypeCode.UInt32:
				return JsonConvert.ToString((uint)value);
			case PrimitiveTypeCode.Int64:
				return JsonConvert.ToString((long)value);
			case PrimitiveTypeCode.UInt64:
				return JsonConvert.ToString((ulong)value);
			case PrimitiveTypeCode.Single:
				return JsonConvert.ToString((float)value);
			case PrimitiveTypeCode.Double:
				return JsonConvert.ToString((double)value);
			case PrimitiveTypeCode.DateTime:
				return JsonConvert.ToString((DateTime)value);
			case PrimitiveTypeCode.DateTimeOffset:
				return JsonConvert.ToString((DateTimeOffset)value);
			case PrimitiveTypeCode.Decimal:
				return JsonConvert.ToString((decimal)value);
			case PrimitiveTypeCode.Guid:
				return JsonConvert.ToString((Guid)value);
			case PrimitiveTypeCode.TimeSpan:
				return JsonConvert.ToString((TimeSpan)value);
			case PrimitiveTypeCode.BigInteger:
				return JsonConvert.ToStringInternal((BigInteger)value);
			case PrimitiveTypeCode.Uri:
				return JsonConvert.ToString((Uri)value);
			case PrimitiveTypeCode.String:
				return JsonConvert.ToString((string)value);
			case PrimitiveTypeCode.DBNull:
				return JsonConvert.Null;
			}
			throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x000394E4 File Offset: 0x000376E4
		public static string SerializeObject(object value)
		{
			return JsonConvert.SerializeObject(value, null, null);
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x000394F0 File Offset: 0x000376F0
		public static string SerializeObject(object value, Formatting formatting)
		{
			return JsonConvert.SerializeObject(value, formatting, null);
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x000394FC File Offset: 0x000376FC
		public static string SerializeObject(object value, params JsonConverter[] converters)
		{
			object obj;
			if (converters == null || converters.Length == 0)
			{
				obj = null;
			}
			else
			{
				(obj = new JsonSerializerSettings()).Converters = converters;
			}
			JsonSerializerSettings jsonSerializerSettings = obj;
			return JsonConvert.SerializeObject(value, null, jsonSerializerSettings);
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x00039538 File Offset: 0x00037738
		public static string SerializeObject(object value, Formatting formatting, params JsonConverter[] converters)
		{
			object obj;
			if (converters == null || converters.Length == 0)
			{
				obj = null;
			}
			else
			{
				(obj = new JsonSerializerSettings()).Converters = converters;
			}
			JsonSerializerSettings jsonSerializerSettings = obj;
			return JsonConvert.SerializeObject(value, null, formatting, jsonSerializerSettings);
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x00039574 File Offset: 0x00037774
		public static string SerializeObject(object value, JsonSerializerSettings settings)
		{
			return JsonConvert.SerializeObject(value, null, settings);
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x00039580 File Offset: 0x00037780
		public static string SerializeObject(object value, Type type, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
			return JsonConvert.SerializeObjectInternal(value, type, jsonSerializer);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x000395A0 File Offset: 0x000377A0
		public static string SerializeObject(object value, Formatting formatting, JsonSerializerSettings settings)
		{
			return JsonConvert.SerializeObject(value, null, formatting, settings);
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x000395AC File Offset: 0x000377AC
		public static string SerializeObject(object value, Type type, Formatting formatting, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
			jsonSerializer.Formatting = formatting;
			return JsonConvert.SerializeObjectInternal(value, type, jsonSerializer);
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x000395D4 File Offset: 0x000377D4
		private static string SerializeObjectInternal(object value, Type type, JsonSerializer jsonSerializer)
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
			{
				jsonTextWriter.Formatting = jsonSerializer.Formatting;
				jsonSerializer.Serialize(jsonTextWriter, value, type);
			}
			return stringWriter.ToString();
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0003963C File Offset: 0x0003783C
		public static object DeserializeObject(string value)
		{
			return JsonConvert.DeserializeObject(value, null, null);
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x00039648 File Offset: 0x00037848
		public static object DeserializeObject(string value, JsonSerializerSettings settings)
		{
			return JsonConvert.DeserializeObject(value, null, settings);
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x00039654 File Offset: 0x00037854
		public static object DeserializeObject(string value, Type type)
		{
			return JsonConvert.DeserializeObject(value, type, null);
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00039660 File Offset: 0x00037860
		public static T DeserializeObject<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value, null);
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x0003966C File Offset: 0x0003786C
		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x00039674 File Offset: 0x00037874
		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject, JsonSerializerSettings settings)
		{
			return JsonConvert.DeserializeObject<T>(value, settings);
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x00039680 File Offset: 0x00037880
		public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
		{
			return (T)((object)JsonConvert.DeserializeObject(value, typeof(T), converters));
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x00039698 File Offset: 0x00037898
		public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
		{
			return (T)((object)JsonConvert.DeserializeObject(value, typeof(T), settings));
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x000396B0 File Offset: 0x000378B0
		public static object DeserializeObject(string value, Type type, params JsonConverter[] converters)
		{
			object obj;
			if (converters == null || converters.Length == 0)
			{
				obj = null;
			}
			else
			{
				(obj = new JsonSerializerSettings()).Converters = converters;
			}
			JsonSerializerSettings jsonSerializerSettings = obj;
			return JsonConvert.DeserializeObject(value, type, jsonSerializerSettings);
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x000396EC File Offset: 0x000378EC
		public static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
			if (!jsonSerializer.IsCheckAdditionalContentSet())
			{
				jsonSerializer.CheckAdditionalContent = true;
			}
			object obj;
			using (JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(value)))
			{
				obj = jsonSerializer.Deserialize(jsonTextReader, type);
			}
			return obj;
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x00039758 File Offset: 0x00037958
		public static void PopulateObject(string value, object target)
		{
			JsonConvert.PopulateObject(value, target, null);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00039764 File Offset: 0x00037964
		public static void PopulateObject(string value, object target, JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
			using (JsonReader jsonReader = new JsonTextReader(new StringReader(value)))
			{
				jsonSerializer.Populate(jsonReader, target);
				if (settings != null && settings.CheckAdditionalContent)
				{
					while (jsonReader.Read())
					{
						if (jsonReader.TokenType != JsonToken.Comment)
						{
							throw JsonSerializationException.Create(jsonReader, "Additional text found in JSON string after finishing deserializing object.");
						}
					}
				}
			}
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x000397E4 File Offset: 0x000379E4
		public static string SerializeXmlNode(XmlNode node)
		{
			return JsonConvert.SerializeXmlNode(node, Formatting.None);
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x000397F0 File Offset: 0x000379F0
		public static string SerializeXmlNode(XmlNode node, Formatting formatting)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00039818 File Offset: 0x00037A18
		public static string SerializeXmlNode(XmlNode node, Formatting formatting, bool omitRootObject)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter
			{
				OmitRootObject = omitRootObject
			};
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00039848 File Offset: 0x00037A48
		public static XmlDocument DeserializeXmlNode(string value)
		{
			return JsonConvert.DeserializeXmlNode(value, null);
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x00039854 File Offset: 0x00037A54
		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName)
		{
			return JsonConvert.DeserializeXmlNode(value, deserializeRootElementName, false);
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x00039860 File Offset: 0x00037A60
		public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			xmlNodeConverter.DeserializeRootElementName = deserializeRootElementName;
			xmlNodeConverter.WriteArrayAttribute = writeArrayAttribute;
			return (XmlDocument)JsonConvert.DeserializeObject(value, typeof(XmlDocument), new JsonConverter[] { xmlNodeConverter });
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x000398A4 File Offset: 0x00037AA4
		public static string SerializeXNode(XObject node)
		{
			return JsonConvert.SerializeXNode(node, Formatting.None);
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x000398B0 File Offset: 0x00037AB0
		public static string SerializeXNode(XObject node, Formatting formatting)
		{
			return JsonConvert.SerializeXNode(node, formatting, false);
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x000398BC File Offset: 0x00037ABC
		public static string SerializeXNode(XObject node, Formatting formatting, bool omitRootObject)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter
			{
				OmitRootObject = omitRootObject
			};
			return JsonConvert.SerializeObject(node, formatting, new JsonConverter[] { xmlNodeConverter });
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x000398EC File Offset: 0x00037AEC
		public static XDocument DeserializeXNode(string value)
		{
			return JsonConvert.DeserializeXNode(value, null);
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x000398F8 File Offset: 0x00037AF8
		public static XDocument DeserializeXNode(string value, string deserializeRootElementName)
		{
			return JsonConvert.DeserializeXNode(value, deserializeRootElementName, false);
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x00039904 File Offset: 0x00037B04
		public static XDocument DeserializeXNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
		{
			XmlNodeConverter xmlNodeConverter = new XmlNodeConverter();
			xmlNodeConverter.DeserializeRootElementName = deserializeRootElementName;
			xmlNodeConverter.WriteArrayAttribute = writeArrayAttribute;
			return (XDocument)JsonConvert.DeserializeObject(value, typeof(XDocument), new JsonConverter[] { xmlNodeConverter });
		}

		// Token: 0x04000368 RID: 872
		public static readonly string True = "true";

		// Token: 0x04000369 RID: 873
		public static readonly string False = "false";

		// Token: 0x0400036A RID: 874
		public static readonly string Null = "null";

		// Token: 0x0400036B RID: 875
		public static readonly string Undefined = "undefined";

		// Token: 0x0400036C RID: 876
		public static readonly string PositiveInfinity = "Infinity";

		// Token: 0x0400036D RID: 877
		public static readonly string NegativeInfinity = "-Infinity";

		// Token: 0x0400036E RID: 878
		public static readonly string NaN = "NaN";
	}
}
