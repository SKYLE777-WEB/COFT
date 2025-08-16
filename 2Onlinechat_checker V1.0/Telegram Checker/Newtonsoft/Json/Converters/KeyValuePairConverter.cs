using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000170 RID: 368
	public class KeyValuePairConverter : JsonConverter
	{
		// Token: 0x06001387 RID: 4999 RVA: 0x0006D980 File Offset: 0x0006BB80
		private static ReflectionObject InitializeReflectionObject(Type t)
		{
			Type[] genericArguments = t.GetGenericArguments();
			Type type = ((IList<Type>)genericArguments)[0];
			Type type2 = ((IList<Type>)genericArguments)[1];
			return ReflectionObject.Create(t, t.GetConstructor(new Type[] { type, type2 }), new string[] { "Key", "Value" });
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x0006D9D8 File Offset: 0x0006BBD8
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get(value.GetType());
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			writer.WriteStartObject();
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Key") : "Key");
			serializer.Serialize(writer, reflectionObject.GetValue(value, "Key"), reflectionObject.GetType("Key"));
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Value") : "Value");
			serializer.Serialize(writer, reflectionObject.GetValue(value, "Value"), reflectionObject.GetType("Value"));
			writer.WriteEndObject();
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x0006DA90 File Offset: 0x0006BC90
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Null)
			{
				object obj = null;
				object obj2 = null;
				reader.ReadAndAssert();
				Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
				ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get(type);
				JsonContract jsonContract = serializer.ContractResolver.ResolveContract(reflectionObject.GetType("Key"));
				JsonContract jsonContract2 = serializer.ContractResolver.ResolveContract(reflectionObject.GetType("Value"));
				while (reader.TokenType == JsonToken.PropertyName)
				{
					string text = reader.Value.ToString();
					if (string.Equals(text, "Key", StringComparison.OrdinalIgnoreCase))
					{
						reader.ReadForTypeAndAssert(jsonContract, false);
						obj = serializer.Deserialize(reader, jsonContract.UnderlyingType);
					}
					else if (string.Equals(text, "Value", StringComparison.OrdinalIgnoreCase))
					{
						reader.ReadForTypeAndAssert(jsonContract2, false);
						obj2 = serializer.Deserialize(reader, jsonContract2.UnderlyingType);
					}
					else
					{
						reader.Skip();
					}
					reader.ReadAndAssert();
				}
				return reflectionObject.Creator(new object[] { obj, obj2 });
			}
			if (!ReflectionUtils.IsNullableType(objectType))
			{
				throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
			}
			return null;
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x0006DBCC File Offset: 0x0006BDCC
		public override bool CanConvert(Type objectType)
		{
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			return type.IsValueType() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(KeyValuePair<, >);
		}

		// Token: 0x040006FE RID: 1790
		private const string KeyName = "Key";

		// Token: 0x040006FF RID: 1791
		private const string ValueName = "Value";

		// Token: 0x04000700 RID: 1792
		private static readonly ThreadSafeStore<Type, ReflectionObject> ReflectionObjectPerType = new ThreadSafeStore<Type, ReflectionObject>(new Func<Type, ReflectionObject>(KeyValuePairConverter.InitializeReflectionObject));
	}
}
