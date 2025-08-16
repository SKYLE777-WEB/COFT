using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000165 RID: 357
	public class BinaryConverter : JsonConverter
	{
		// Token: 0x06001349 RID: 4937 RVA: 0x0006C020 File Offset: 0x0006A220
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			byte[] byteArray = this.GetByteArray(value);
			writer.WriteValue(byteArray);
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0006C050 File Offset: 0x0006A250
		private byte[] GetByteArray(object value)
		{
			if (value.GetType().FullName == "System.Data.Linq.Binary")
			{
				BinaryConverter.EnsureReflectionObject(value.GetType());
				return (byte[])BinaryConverter._reflectionObject.GetValue(value, "ToArray");
			}
			if (value is SqlBinary)
			{
				return ((SqlBinary)value).Value;
			}
			throw new JsonSerializationException("Unexpected value type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x0006C0D0 File Offset: 0x0006A2D0
		private static void EnsureReflectionObject(Type t)
		{
			if (BinaryConverter._reflectionObject == null)
			{
				BinaryConverter._reflectionObject = ReflectionObject.Create(t, t.GetConstructor(new Type[] { typeof(byte[]) }), new string[] { "ToArray" });
			}
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x0006C110 File Offset: 0x0006A310
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
				byte[] array;
				if (reader.TokenType == JsonToken.StartArray)
				{
					array = this.ReadByteArray(reader);
				}
				else
				{
					if (reader.TokenType != JsonToken.String)
					{
						throw JsonSerializationException.Create(reader, "Unexpected token parsing binary. Expected String or StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
					}
					array = Convert.FromBase64String(reader.Value.ToString());
				}
				Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
				if (type.FullName == "System.Data.Linq.Binary")
				{
					BinaryConverter.EnsureReflectionObject(type);
					return BinaryConverter._reflectionObject.Creator(new object[] { array });
				}
				if (type == typeof(SqlBinary))
				{
					return new SqlBinary(array);
				}
				throw JsonSerializationException.Create(reader, "Unexpected object type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x0006C234 File Offset: 0x0006A434
		private byte[] ReadByteArray(JsonReader reader)
		{
			List<byte> list = new List<byte>();
			while (reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					if (tokenType != JsonToken.Integer)
					{
						if (tokenType != JsonToken.EndArray)
						{
							throw JsonSerializationException.Create(reader, "Unexpected token when reading bytes: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
						}
						return list.ToArray();
					}
					else
					{
						list.Add(Convert.ToByte(reader.Value, CultureInfo.InvariantCulture));
					}
				}
			}
			throw JsonSerializationException.Create(reader, "Unexpected end when reading bytes.");
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x0006C2C8 File Offset: 0x0006A4C8
		public override bool CanConvert(Type objectType)
		{
			return objectType.FullName == "System.Data.Linq.Binary" || (objectType == typeof(SqlBinary) || objectType == typeof(SqlBinary?));
		}

		// Token: 0x040006EE RID: 1774
		private const string BinaryTypeName = "System.Data.Linq.Binary";

		// Token: 0x040006EF RID: 1775
		private const string BinaryToArrayName = "ToArray";

		// Token: 0x040006F0 RID: 1776
		private static ReflectionObject _reflectionObject;
	}
}
