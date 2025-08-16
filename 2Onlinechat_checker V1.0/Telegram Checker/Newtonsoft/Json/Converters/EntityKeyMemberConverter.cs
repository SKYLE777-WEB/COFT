using System;
using System.Globalization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200016C RID: 364
	public class EntityKeyMemberConverter : JsonConverter
	{
		// Token: 0x0600136D RID: 4973 RVA: 0x0006D068 File Offset: 0x0006B268
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			EntityKeyMemberConverter.EnsureReflectionObject(value.GetType());
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			string text = (string)EntityKeyMemberConverter._reflectionObject.GetValue(value, "Key");
			object value2 = EntityKeyMemberConverter._reflectionObject.GetValue(value, "Value");
			Type type = ((value2 != null) ? value2.GetType() : null);
			writer.WriteStartObject();
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Key") : "Key");
			writer.WriteValue(text);
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Type") : "Type");
			writer.WriteValue((type != null) ? type.FullName : null);
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Value") : "Value");
			if (type != null)
			{
				string text2;
				if (JsonSerializerInternalWriter.TryConvertToString(value2, type, out text2))
				{
					writer.WriteValue(text2);
				}
				else
				{
					writer.WriteValue(value2);
				}
			}
			else
			{
				writer.WriteNull();
			}
			writer.WriteEndObject();
		}

		// Token: 0x0600136E RID: 4974 RVA: 0x0006D194 File Offset: 0x0006B394
		private static void ReadAndAssertProperty(JsonReader reader, string propertyName)
		{
			reader.ReadAndAssert();
			if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value.ToString(), propertyName, StringComparison.OrdinalIgnoreCase))
			{
				throw new JsonSerializationException("Expected JSON property '{0}'.".FormatWith(CultureInfo.InvariantCulture, propertyName));
			}
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x0006D1E4 File Offset: 0x0006B3E4
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			EntityKeyMemberConverter.EnsureReflectionObject(objectType);
			object obj = EntityKeyMemberConverter._reflectionObject.Creator(new object[0]);
			EntityKeyMemberConverter.ReadAndAssertProperty(reader, "Key");
			reader.ReadAndAssert();
			EntityKeyMemberConverter._reflectionObject.SetValue(obj, "Key", reader.Value.ToString());
			EntityKeyMemberConverter.ReadAndAssertProperty(reader, "Type");
			reader.ReadAndAssert();
			Type type = Type.GetType(reader.Value.ToString());
			EntityKeyMemberConverter.ReadAndAssertProperty(reader, "Value");
			reader.ReadAndAssert();
			EntityKeyMemberConverter._reflectionObject.SetValue(obj, "Value", serializer.Deserialize(reader, type));
			reader.ReadAndAssert();
			return obj;
		}

		// Token: 0x06001370 RID: 4976 RVA: 0x0006D290 File Offset: 0x0006B490
		private static void EnsureReflectionObject(Type objectType)
		{
			if (EntityKeyMemberConverter._reflectionObject == null)
			{
				EntityKeyMemberConverter._reflectionObject = ReflectionObject.Create(objectType, new string[] { "Key", "Value" });
			}
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x0006D2C0 File Offset: 0x0006B4C0
		public override bool CanConvert(Type objectType)
		{
			return objectType.AssignableToTypeName("System.Data.EntityKeyMember", false);
		}

		// Token: 0x040006F5 RID: 1781
		private const string EntityKeyMemberFullTypeName = "System.Data.EntityKeyMember";

		// Token: 0x040006F6 RID: 1782
		private const string KeyPropertyName = "Key";

		// Token: 0x040006F7 RID: 1783
		private const string TypePropertyName = "Type";

		// Token: 0x040006F8 RID: 1784
		private const string ValuePropertyName = "Value";

		// Token: 0x040006F9 RID: 1785
		private static ReflectionObject _reflectionObject;
	}
}
