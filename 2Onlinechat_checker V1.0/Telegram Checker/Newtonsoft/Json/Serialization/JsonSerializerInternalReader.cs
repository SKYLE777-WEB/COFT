using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200011F RID: 287
	internal class JsonSerializerInternalReader : JsonSerializerInternalBase
	{
		// Token: 0x06000E96 RID: 3734 RVA: 0x000573D8 File Offset: 0x000555D8
		public JsonSerializerInternalReader(JsonSerializer serializer)
			: base(serializer)
		{
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x000573E4 File Offset: 0x000555E4
		public void Populate(JsonReader reader, object target)
		{
			ValidationUtils.ArgumentNotNull(target, "target");
			Type type = target.GetType();
			JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(type);
			if (!reader.MoveToContent())
			{
				throw JsonSerializationException.Create(reader, "No JSON content found.");
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				if (jsonContract.ContractType == JsonContractType.Array)
				{
					JsonArrayContract jsonArrayContract = (JsonArrayContract)jsonContract;
					IList list;
					if (!jsonArrayContract.ShouldCreateWrapper)
					{
						list = (IList)target;
					}
					else
					{
						IList list2 = jsonArrayContract.CreateWrapper(target);
						list = list2;
					}
					this.PopulateList(list, reader, jsonArrayContract, null, null);
					return;
				}
				throw JsonSerializationException.Create(reader, "Cannot populate JSON array onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
			}
			else
			{
				if (reader.TokenType != JsonToken.StartObject)
				{
					throw JsonSerializationException.Create(reader, "Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				reader.ReadAndAssert();
				string text = null;
				if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
				{
					reader.ReadAndAssert();
					object value = reader.Value;
					text = ((value != null) ? value.ToString() : null);
					reader.ReadAndAssert();
				}
				if (jsonContract.ContractType == JsonContractType.Dictionary)
				{
					JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)jsonContract;
					IDictionary dictionary;
					if (!jsonDictionaryContract.ShouldCreateWrapper)
					{
						dictionary = (IDictionary)target;
					}
					else
					{
						IDictionary dictionary2 = jsonDictionaryContract.CreateWrapper(target);
						dictionary = dictionary2;
					}
					this.PopulateDictionary(dictionary, reader, jsonDictionaryContract, null, text);
					return;
				}
				if (jsonContract.ContractType == JsonContractType.Object)
				{
					this.PopulateObject(target, reader, (JsonObjectContract)jsonContract, null, text);
					return;
				}
				throw JsonSerializationException.Create(reader, "Cannot populate JSON object onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
			}
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x000575A0 File Offset: 0x000557A0
		private JsonContract GetContractSafe(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return this.Serializer._contractResolver.ResolveContract(type);
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x000575C4 File Offset: 0x000557C4
		public object Deserialize(JsonReader reader, Type objectType, bool checkAdditionalContent)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			JsonContract contractSafe = this.GetContractSafe(objectType);
			object obj;
			try
			{
				JsonConverter converter = this.GetConverter(contractSafe, null, null, null);
				if (reader.TokenType == JsonToken.None && !reader.ReadForType(contractSafe, converter != null))
				{
					if (contractSafe != null && !contractSafe.IsNullable)
					{
						throw JsonSerializationException.Create(reader, "No JSON content found and type '{0}' is not nullable.".FormatWith(CultureInfo.InvariantCulture, contractSafe.UnderlyingType));
					}
					obj = null;
				}
				else
				{
					object obj2;
					if (converter != null && converter.CanRead)
					{
						obj2 = this.DeserializeConvertable(converter, reader, objectType, null);
					}
					else
					{
						obj2 = this.CreateValueInternal(reader, objectType, contractSafe, null, null, null, null);
					}
					if (checkAdditionalContent)
					{
						while (reader.Read())
						{
							if (reader.TokenType != JsonToken.Comment)
							{
								throw JsonSerializationException.Create(reader, "Additional text found in JSON string after finishing deserializing object.");
							}
						}
					}
					obj = obj2;
				}
			}
			catch (Exception ex)
			{
				if (!base.IsErrorHandled(null, contractSafe, null, reader as IJsonLineInfo, reader.Path, ex))
				{
					base.ClearErrorContext();
					throw;
				}
				this.HandleError(reader, false, 0);
				obj = null;
			}
			return obj;
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x000576EC File Offset: 0x000558EC
		private JsonSerializerProxy GetInternalSerializer()
		{
			if (this.InternalSerializer == null)
			{
				this.InternalSerializer = new JsonSerializerProxy(this);
			}
			return this.InternalSerializer;
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x0005770C File Offset: 0x0005590C
		private JToken CreateJToken(JsonReader reader, JsonContract contract)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (contract != null)
			{
				if (contract.UnderlyingType == typeof(JRaw))
				{
					return JRaw.Create(reader);
				}
				if (reader.TokenType == JsonToken.Null && !(contract.UnderlyingType == typeof(JValue)) && !(contract.UnderlyingType == typeof(JToken)))
				{
					return null;
				}
			}
			JToken token;
			using (JTokenWriter jtokenWriter = new JTokenWriter())
			{
				jtokenWriter.WriteToken(reader);
				token = jtokenWriter.Token;
			}
			return token;
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x000577C8 File Offset: 0x000559C8
		private JToken CreateJObject(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			using (JTokenWriter jtokenWriter = new JTokenWriter())
			{
				jtokenWriter.WriteStartObject();
				for (;;)
				{
					if (reader.TokenType == JsonToken.PropertyName)
					{
						string text = (string)reader.Value;
						if (!reader.ReadAndMoveToContent())
						{
							goto IL_0083;
						}
						if (!this.CheckPropertyName(reader, text))
						{
							jtokenWriter.WritePropertyName(text);
							jtokenWriter.WriteToken(reader, true, true, false);
						}
					}
					else if (reader.TokenType != JsonToken.Comment)
					{
						break;
					}
					if (!reader.Read())
					{
						goto IL_0083;
					}
				}
				jtokenWriter.WriteEndObject();
				return jtokenWriter.Token;
				IL_0083:
				throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
			}
			JToken jtoken;
			return jtoken;
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00057884 File Offset: 0x00055A84
		private object CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue)
		{
			if (contract != null && contract.ContractType == JsonContractType.Linq)
			{
				return this.CreateJToken(reader, contract);
			}
			for (;;)
			{
				switch (reader.TokenType)
				{
				case JsonToken.StartObject:
					goto IL_0073;
				case JsonToken.StartArray:
					goto IL_0085;
				case JsonToken.StartConstructor:
					goto IL_00F0;
				case JsonToken.Comment:
					if (!reader.Read())
					{
						goto Block_7;
					}
					continue;
				case JsonToken.Raw:
					goto IL_013C;
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.Boolean:
				case JsonToken.Date:
				case JsonToken.Bytes:
					goto IL_0094;
				case JsonToken.String:
					goto IL_00A9;
				case JsonToken.Null:
				case JsonToken.Undefined:
					goto IL_010C;
				}
				break;
			}
			goto IL_014D;
			IL_0073:
			return this.CreateObject(reader, objectType, contract, member, containerContract, containerMember, existingValue);
			IL_0085:
			return this.CreateList(reader, objectType, contract, member, existingValue, null);
			IL_0094:
			return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
			IL_00A9:
			string text = (string)reader.Value;
			if (JsonSerializerInternalReader.CoerceEmptyStringToNull(objectType, contract, text))
			{
				return null;
			}
			if (objectType == typeof(byte[]))
			{
				return Convert.FromBase64String(text);
			}
			return this.EnsureType(reader, text, CultureInfo.InvariantCulture, contract, objectType);
			IL_00F0:
			string text2 = reader.Value.ToString();
			return this.EnsureType(reader, text2, CultureInfo.InvariantCulture, contract, objectType);
			IL_010C:
			if (objectType == typeof(DBNull))
			{
				return DBNull.Value;
			}
			return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
			IL_013C:
			return new JRaw((string)reader.Value);
			IL_014D:
			throw JsonSerializationException.Create(reader, "Unexpected token while deserializing object: " + reader.TokenType);
			Block_7:
			throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x00057A14 File Offset: 0x00055C14
		private static bool CoerceEmptyStringToNull(Type objectType, JsonContract contract, string s)
		{
			return string.IsNullOrEmpty(s) && objectType != null && objectType != typeof(string) && objectType != typeof(object) && contract != null && contract.IsNullable;
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x00057A74 File Offset: 0x00055C74
		internal string GetExpectedDescription(JsonContract contract)
		{
			switch (contract.ContractType)
			{
			case JsonContractType.Object:
			case JsonContractType.Dictionary:
			case JsonContractType.Dynamic:
			case JsonContractType.Serializable:
				return "JSON object (e.g. {\"name\":\"value\"})";
			case JsonContractType.Array:
				return "JSON array (e.g. [1,2,3])";
			case JsonContractType.Primitive:
				return "JSON primitive value (e.g. string, number, boolean, null)";
			case JsonContractType.String:
				return "JSON string value";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00057AD4 File Offset: 0x00055CD4
		private JsonConverter GetConverter(JsonContract contract, JsonConverter memberConverter, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			JsonConverter jsonConverter = null;
			if (memberConverter != null)
			{
				jsonConverter = memberConverter;
			}
			else if (((containerProperty != null) ? containerProperty.ItemConverter : null) != null)
			{
				jsonConverter = containerProperty.ItemConverter;
			}
			else if (((containerContract != null) ? containerContract.ItemConverter : null) != null)
			{
				jsonConverter = containerContract.ItemConverter;
			}
			else if (contract != null)
			{
				JsonConverter matchingConverter;
				if (contract.Converter != null)
				{
					jsonConverter = contract.Converter;
				}
				else if ((matchingConverter = this.Serializer.GetMatchingConverter(contract.UnderlyingType)) != null)
				{
					jsonConverter = matchingConverter;
				}
				else if (contract.InternalConverter != null)
				{
					jsonConverter = contract.InternalConverter;
				}
			}
			return jsonConverter;
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x00057B8C File Offset: 0x00055D8C
		private object CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue)
		{
			Type type = objectType;
			string text;
			if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.Ignore)
			{
				reader.ReadAndAssert();
				text = null;
			}
			else if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead)
			{
				JTokenReader jtokenReader = reader as JTokenReader;
				if (jtokenReader == null)
				{
					jtokenReader = (JTokenReader)JToken.ReadFrom(reader).CreateReader();
					jtokenReader.Culture = reader.Culture;
					jtokenReader.DateFormatString = reader.DateFormatString;
					jtokenReader.DateParseHandling = reader.DateParseHandling;
					jtokenReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
					jtokenReader.FloatParseHandling = reader.FloatParseHandling;
					jtokenReader.SupportMultipleContent = reader.SupportMultipleContent;
					jtokenReader.ReadAndAssert();
					reader = jtokenReader;
				}
				object obj;
				if (this.ReadMetadataPropertiesToken(jtokenReader, ref type, ref contract, member, containerContract, containerMember, existingValue, out obj, out text))
				{
					return obj;
				}
			}
			else
			{
				reader.ReadAndAssert();
				object obj2;
				if (this.ReadMetadataProperties(reader, ref type, ref contract, member, containerContract, containerMember, existingValue, out obj2, out text))
				{
					return obj2;
				}
			}
			if (this.HasNoDefinedType(contract))
			{
				return this.CreateJObject(reader);
			}
			switch (contract.ContractType)
			{
			case JsonContractType.Object:
			{
				bool flag = false;
				JsonObjectContract jsonObjectContract = (JsonObjectContract)contract;
				object obj3;
				if (existingValue != null && (type == objectType || type.IsAssignableFrom(existingValue.GetType())))
				{
					obj3 = existingValue;
				}
				else
				{
					obj3 = this.CreateNewObject(reader, jsonObjectContract, member, containerMember, text, out flag);
				}
				if (flag)
				{
					return obj3;
				}
				return this.PopulateObject(obj3, reader, jsonObjectContract, member, text);
			}
			case JsonContractType.Primitive:
			{
				JsonPrimitiveContract jsonPrimitiveContract = (JsonPrimitiveContract)contract;
				if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
				{
					reader.ReadAndAssert();
					if (reader.TokenType == JsonToken.StartObject)
					{
						throw JsonSerializationException.Create(reader, "Unexpected token when deserializing primitive value: " + reader.TokenType);
					}
					object obj4 = this.CreateValueInternal(reader, type, jsonPrimitiveContract, member, null, null, existingValue);
					reader.ReadAndAssert();
					return obj4;
				}
				break;
			}
			case JsonContractType.Dictionary:
			{
				JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)contract;
				object obj5;
				if (existingValue == null)
				{
					bool flag2;
					IDictionary dictionary = this.CreateNewDictionary(reader, jsonDictionaryContract, out flag2);
					if (flag2)
					{
						if (text != null)
						{
							throw JsonSerializationException.Create(reader, "Cannot preserve reference to readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
						}
						if (contract.OnSerializingCallbacks.Count > 0)
						{
							throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
						}
						if (contract.OnErrorCallbacks.Count > 0)
						{
							throw JsonSerializationException.Create(reader, "Cannot call OnError on readonly list, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
						}
						if (!jsonDictionaryContract.HasParameterizedCreatorInternal)
						{
							throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size dictionary: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
						}
					}
					this.PopulateDictionary(dictionary, reader, jsonDictionaryContract, member, text);
					if (flag2)
					{
						return (jsonDictionaryContract.OverrideCreator ?? jsonDictionaryContract.ParameterizedCreator)(new object[] { dictionary });
					}
					if (dictionary is IWrappedDictionary)
					{
						return ((IWrappedDictionary)dictionary).UnderlyingDictionary;
					}
					obj5 = dictionary;
				}
				else
				{
					IDictionary dictionary2;
					if (!jsonDictionaryContract.ShouldCreateWrapper)
					{
						dictionary2 = (IDictionary)existingValue;
					}
					else
					{
						IDictionary dictionary3 = jsonDictionaryContract.CreateWrapper(existingValue);
						dictionary2 = dictionary3;
					}
					obj5 = this.PopulateDictionary(dictionary2, reader, jsonDictionaryContract, member, text);
				}
				return obj5;
			}
			case JsonContractType.Dynamic:
			{
				JsonDynamicContract jsonDynamicContract = (JsonDynamicContract)contract;
				return this.CreateDynamic(reader, jsonDynamicContract, member, text);
			}
			case JsonContractType.Serializable:
			{
				JsonISerializableContract jsonISerializableContract = (JsonISerializableContract)contract;
				return this.CreateISerializable(reader, jsonISerializableContract, member, text);
			}
			}
			string text2 = "Cannot deserialize the current JSON object (e.g. {{\"name\":\"value\"}}) into type '{0}' because the type requires a {1} to deserialize correctly." + Environment.NewLine + "To fix this error either change the JSON to a {1} or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer, not a collection type like an array or List<T>) that can be deserialized from a JSON object. JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object." + Environment.NewLine;
			text2 = text2.FormatWith(CultureInfo.InvariantCulture, type, this.GetExpectedDescription(contract));
			throw JsonSerializationException.Create(reader, text2);
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x00057F70 File Offset: 0x00056170
		private bool ReadMetadataPropertiesToken(JTokenReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue, out object newValue, out string id)
		{
			id = null;
			newValue = null;
			if (reader.TokenType == JsonToken.StartObject)
			{
				JObject jobject = (JObject)reader.CurrentToken;
				JToken jtoken = jobject["$ref"];
				if (jtoken != null)
				{
					if (jtoken.Type != JTokenType.String && jtoken.Type != JTokenType.Null)
					{
						throw JsonSerializationException.Create(jtoken, jtoken.Path, "JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"), null);
					}
					JToken parent = jtoken.Parent;
					JToken jtoken2 = null;
					if (parent.Next != null)
					{
						jtoken2 = parent.Next;
					}
					else if (parent.Previous != null)
					{
						jtoken2 = parent.Previous;
					}
					string text = (string)jtoken;
					if (text != null)
					{
						if (jtoken2 != null)
						{
							throw JsonSerializationException.Create(jtoken2, jtoken2.Path, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"), null);
						}
						newValue = this.Serializer.GetReferenceResolver().ResolveReference(this, text);
						if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
						{
							this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, text, newValue.GetType())), null);
						}
						reader.Skip();
						return true;
					}
				}
				JToken jtoken3 = jobject["$type"];
				if (jtoken3 != null)
				{
					string text2 = (string)jtoken3;
					JsonReader jsonReader = jtoken3.CreateReader();
					jsonReader.ReadAndAssert();
					this.ResolveTypeName(jsonReader, ref objectType, ref contract, member, containerContract, containerMember, text2);
					if (jobject["$value"] != null)
					{
						for (;;)
						{
							reader.ReadAndAssert();
							if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "$value")
							{
								break;
							}
							reader.ReadAndAssert();
							reader.Skip();
						}
						return false;
					}
				}
				JToken jtoken4 = jobject["$id"];
				if (jtoken4 != null)
				{
					id = (string)jtoken4;
				}
				JToken jtoken5 = jobject["$values"];
				if (jtoken5 != null)
				{
					JsonReader jsonReader2 = jtoken5.CreateReader();
					jsonReader2.ReadAndAssert();
					newValue = this.CreateList(jsonReader2, objectType, contract, member, existingValue, id);
					reader.Skip();
					return true;
				}
			}
			reader.ReadAndAssert();
			return false;
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x000581B8 File Offset: 0x000563B8
		private bool ReadMetadataProperties(JsonReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue, out object newValue, out string id)
		{
			id = null;
			newValue = null;
			if (reader.TokenType == JsonToken.PropertyName)
			{
				string text = reader.Value.ToString();
				if (text.Length > 0 && text[0] == '$')
				{
					string text2;
					for (;;)
					{
						text = reader.Value.ToString();
						bool flag;
						if (string.Equals(text, "$ref", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
							{
								break;
							}
							object value = reader.Value;
							text2 = ((value != null) ? value.ToString() : null);
							reader.ReadAndAssert();
							if (text2 != null)
							{
								goto Block_7;
							}
							flag = true;
						}
						else if (string.Equals(text, "$type", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							string text3 = reader.Value.ToString();
							this.ResolveTypeName(reader, ref objectType, ref contract, member, containerContract, containerMember, text3);
							reader.ReadAndAssert();
							flag = true;
						}
						else if (string.Equals(text, "$id", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							object value2 = reader.Value;
							id = ((value2 != null) ? value2.ToString() : null);
							reader.ReadAndAssert();
							flag = true;
						}
						else
						{
							if (string.Equals(text, "$values", StringComparison.Ordinal))
							{
								goto Block_14;
							}
							flag = false;
						}
						if (!flag || reader.TokenType != JsonToken.PropertyName)
						{
							return false;
						}
					}
					throw JsonSerializationException.Create(reader, "JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
					Block_7:
					if (reader.TokenType == JsonToken.PropertyName)
					{
						throw JsonSerializationException.Create(reader, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
					}
					newValue = this.Serializer.GetReferenceResolver().ResolveReference(this, text2);
					if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
					{
						this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, text2, newValue.GetType())), null);
					}
					return true;
					Block_14:
					reader.ReadAndAssert();
					object obj = this.CreateList(reader, objectType, contract, member, existingValue, id);
					reader.ReadAndAssert();
					newValue = obj;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x000583E4 File Offset: 0x000565E4
		private void ResolveTypeName(JsonReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, string qualifiedTypeName)
		{
			if ((((member != null) ? member.TypeNameHandling : null) ?? ((containerContract != null) ? containerContract.ItemTypeNameHandling : null) ?? ((containerMember != null) ? containerMember.ItemTypeNameHandling : null) ?? this.Serializer._typeNameHandling) != TypeNameHandling.None)
			{
				TypeNameKey typeNameKey = ReflectionUtils.SplitFullyQualifiedTypeName(qualifiedTypeName);
				Type type;
				try
				{
					type = this.Serializer._serializationBinder.BindToType(typeNameKey.AssemblyName, typeNameKey.TypeName);
				}
				catch (Exception ex)
				{
					throw JsonSerializationException.Create(reader, "Error resolving type specified in JSON '{0}'.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName), ex);
				}
				if (type == null)
				{
					throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' was not resolved.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName));
				}
				if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
				{
					this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved type '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName, type)), null);
				}
				if (objectType != null && objectType != typeof(IDynamicMetaObjectProvider) && !objectType.IsAssignableFrom(type))
				{
					throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.AssemblyQualifiedName, objectType.AssemblyQualifiedName));
				}
				objectType = type;
				contract = this.GetContractSafe(type);
			}
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x000585D0 File Offset: 0x000567D0
		private JsonArrayContract EnsureArrayContract(JsonReader reader, Type objectType, JsonContract contract)
		{
			if (contract == null)
			{
				throw JsonSerializationException.Create(reader, "Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			JsonArrayContract jsonArrayContract = contract as JsonArrayContract;
			if (jsonArrayContract == null)
			{
				string text = "Cannot deserialize the current JSON array (e.g. [1,2,3]) into type '{0}' because the type requires a {1} to deserialize correctly." + Environment.NewLine + "To fix this error either change the JSON to a {1} or change the deserialized type to an array or a type that implements a collection interface (e.g. ICollection, IList) like List<T> that can be deserialized from a JSON array. JsonArrayAttribute can also be added to the type to force it to deserialize from a JSON array." + Environment.NewLine;
				text = text.FormatWith(CultureInfo.InvariantCulture, objectType, this.GetExpectedDescription(contract));
				throw JsonSerializationException.Create(reader, text);
			}
			return jsonArrayContract;
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x00058640 File Offset: 0x00056840
		private object CreateList(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue, string id)
		{
			if (this.HasNoDefinedType(contract))
			{
				return this.CreateJToken(reader, contract);
			}
			JsonArrayContract jsonArrayContract = this.EnsureArrayContract(reader, objectType, contract);
			object obj;
			if (existingValue == null)
			{
				bool flag;
				IList list = this.CreateNewList(reader, jsonArrayContract, out flag);
				if (flag)
				{
					if (id != null)
					{
						throw JsonSerializationException.Create(reader, "Cannot preserve reference to array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
					if (contract.OnSerializingCallbacks.Count > 0)
					{
						throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
					if (contract.OnErrorCallbacks.Count > 0)
					{
						throw JsonSerializationException.Create(reader, "Cannot call OnError on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
					if (!jsonArrayContract.HasParameterizedCreatorInternal && !jsonArrayContract.IsArray)
					{
						throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size list: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
				}
				if (!jsonArrayContract.IsMultidimensionalArray)
				{
					this.PopulateList(list, reader, jsonArrayContract, member, id);
				}
				else
				{
					this.PopulateMultidimensionalArray(list, reader, jsonArrayContract, member, id);
				}
				if (flag)
				{
					if (jsonArrayContract.IsMultidimensionalArray)
					{
						list = CollectionUtils.ToMultidimensionalArray(list, jsonArrayContract.CollectionItemType, contract.CreatedType.GetArrayRank());
					}
					else
					{
						if (!jsonArrayContract.IsArray)
						{
							return (jsonArrayContract.OverrideCreator ?? jsonArrayContract.ParameterizedCreator)(new object[] { list });
						}
						Array array = Array.CreateInstance(jsonArrayContract.CollectionItemType, list.Count);
						list.CopyTo(array, 0);
						list = array;
					}
				}
				else if (list is IWrappedCollection)
				{
					return ((IWrappedCollection)list).UnderlyingCollection;
				}
				obj = list;
			}
			else
			{
				if (!jsonArrayContract.CanDeserialize)
				{
					throw JsonSerializationException.Create(reader, "Cannot populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.CreatedType));
				}
				IList list2;
				if (!jsonArrayContract.ShouldCreateWrapper)
				{
					list2 = (IList)existingValue;
				}
				else
				{
					IList list3 = jsonArrayContract.CreateWrapper(existingValue);
					list2 = list3;
				}
				obj = this.PopulateList(list2, reader, jsonArrayContract, member, id);
			}
			return obj;
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x0005885C File Offset: 0x00056A5C
		private bool HasNoDefinedType(JsonContract contract)
		{
			return contract == null || contract.UnderlyingType == typeof(object) || contract.ContractType == JsonContractType.Linq || contract.UnderlyingType == typeof(IDynamicMetaObjectProvider);
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x000588B0 File Offset: 0x00056AB0
		private object EnsureType(JsonReader reader, object value, CultureInfo culture, JsonContract contract, Type targetType)
		{
			if (targetType == null)
			{
				return value;
			}
			if (ReflectionUtils.GetObjectType(value) != targetType)
			{
				if (value == null && contract.IsNullable)
				{
					return null;
				}
				try
				{
					if (!contract.IsConvertable)
					{
						return ConvertUtils.ConvertOrCast(value, culture, contract.NonNullableUnderlyingType);
					}
					JsonPrimitiveContract jsonPrimitiveContract = (JsonPrimitiveContract)contract;
					if (contract.IsEnum)
					{
						if (value is string)
						{
							return Enum.Parse(contract.NonNullableUnderlyingType, value.ToString(), true);
						}
						if (ConvertUtils.IsInteger(jsonPrimitiveContract.TypeCode))
						{
							return Enum.ToObject(contract.NonNullableUnderlyingType, value);
						}
					}
					if (value is BigInteger)
					{
						return ConvertUtils.FromBigInteger((BigInteger)value, contract.NonNullableUnderlyingType);
					}
					return Convert.ChangeType(value, contract.NonNullableUnderlyingType, culture);
				}
				catch (Exception ex)
				{
					throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(value), targetType), ex);
				}
				return value;
			}
			return value;
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x000589E0 File Offset: 0x00056BE0
		private bool SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, object target)
		{
			bool flag;
			object value;
			JsonContract jsonContract;
			bool flag2;
			if (this.CalculatePropertyDetails(property, ref propertyConverter, containerContract, containerProperty, reader, target, out flag, out value, out jsonContract, out flag2))
			{
				return false;
			}
			object obj;
			if (propertyConverter != null && propertyConverter.CanRead)
			{
				if (!flag2 && target != null && property.Readable)
				{
					value = property.ValueProvider.GetValue(target);
				}
				obj = this.DeserializeConvertable(propertyConverter, reader, property.PropertyType, value);
			}
			else
			{
				obj = this.CreateValueInternal(reader, property.PropertyType, jsonContract, property, containerContract, containerProperty, flag ? value : null);
			}
			if ((!flag || obj != value) && this.ShouldSetPropertyValue(property, obj))
			{
				property.ValueProvider.SetValue(target, obj);
				if (property.SetIsSpecified != null)
				{
					if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
					{
						this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "IsSpecified for property '{0}' on {1} set to true.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType)), null);
					}
					property.SetIsSpecified(target, true);
				}
				return true;
			}
			return flag;
		}

		// Token: 0x06000EAA RID: 3754 RVA: 0x00058B24 File Offset: 0x00056D24
		private bool CalculatePropertyDetails(JsonProperty property, ref JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, object target, out bool useExistingValue, out object currentValue, out JsonContract propertyContract, out bool gottenCurrentValue)
		{
			currentValue = null;
			useExistingValue = false;
			propertyContract = null;
			gottenCurrentValue = false;
			if (property.Ignored)
			{
				return true;
			}
			JsonToken tokenType = reader.TokenType;
			if (property.PropertyContract == null)
			{
				property.PropertyContract = this.GetContractSafe(property.PropertyType);
			}
			if (property.ObjectCreationHandling.GetValueOrDefault(this.Serializer._objectCreationHandling) != ObjectCreationHandling.Replace && (tokenType == JsonToken.StartArray || tokenType == JsonToken.StartObject) && property.Readable)
			{
				currentValue = property.ValueProvider.GetValue(target);
				gottenCurrentValue = true;
				if (currentValue != null)
				{
					propertyContract = this.GetContractSafe(currentValue.GetType());
					useExistingValue = !propertyContract.IsReadOnlyOrFixedSize && !propertyContract.UnderlyingType.IsValueType();
				}
			}
			if (!property.Writable && !useExistingValue)
			{
				return true;
			}
			if (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) == NullValueHandling.Ignore && tokenType == JsonToken.Null)
			{
				return true;
			}
			if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) && !this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) && JsonTokenUtils.IsPrimitiveToken(tokenType) && MiscellaneousUtils.ValueEquals(reader.Value, property.GetResolvedDefaultValue()))
			{
				return true;
			}
			if (currentValue == null)
			{
				propertyContract = property.PropertyContract;
			}
			else
			{
				propertyContract = this.GetContractSafe(currentValue.GetType());
				if (propertyContract != property.PropertyContract)
				{
					propertyConverter = this.GetConverter(propertyContract, property.MemberConverter, containerContract, containerProperty);
				}
			}
			return false;
		}

		// Token: 0x06000EAB RID: 3755 RVA: 0x00058CF8 File Offset: 0x00056EF8
		private void AddReference(JsonReader reader, string id, object value)
		{
			try
			{
				if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
				{
					this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Read object reference Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, id, value.GetType())), null);
				}
				this.Serializer.GetReferenceResolver().AddReference(this, id, value);
			}
			catch (Exception ex)
			{
				throw JsonSerializationException.Create(reader, "Error reading object reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, id), ex);
			}
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x00058D98 File Offset: 0x00056F98
		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x00058DA0 File Offset: 0x00056FA0
		private bool ShouldSetPropertyValue(JsonProperty property, object value)
		{
			return (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) != NullValueHandling.Ignore || value != null) && (!this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) || this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) || !MiscellaneousUtils.ValueEquals(value, property.GetResolvedDefaultValue())) && property.Writable;
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x00058E44 File Offset: 0x00057044
		private IList CreateNewList(JsonReader reader, JsonArrayContract contract, out bool createdFromNonDefaultCreator)
		{
			if (!contract.CanDeserialize)
			{
				throw JsonSerializationException.Create(reader, "Cannot create and populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.CreatedType));
			}
			if (contract.OverrideCreator != null)
			{
				if (contract.HasParameterizedCreator)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryCollection();
				}
				object obj = contract.OverrideCreator(new object[0]);
				if (contract.ShouldCreateWrapper)
				{
					obj = contract.CreateWrapper(obj);
				}
				createdFromNonDefaultCreator = false;
				return (IList)obj;
			}
			else
			{
				if (contract.IsReadOnlyOrFixedSize)
				{
					createdFromNonDefaultCreator = true;
					IList list = contract.CreateTemporaryCollection();
					if (contract.ShouldCreateWrapper)
					{
						list = contract.CreateWrapper(list);
					}
					return list;
				}
				if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
				{
					object obj2 = contract.DefaultCreator();
					if (contract.ShouldCreateWrapper)
					{
						obj2 = contract.CreateWrapper(obj2);
					}
					createdFromNonDefaultCreator = false;
					return (IList)obj2;
				}
				if (contract.HasParameterizedCreatorInternal)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryCollection();
				}
				if (!contract.IsInstantiable)
				{
					throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x00058F9C File Offset: 0x0005719C
		private IDictionary CreateNewDictionary(JsonReader reader, JsonDictionaryContract contract, out bool createdFromNonDefaultCreator)
		{
			if (contract.OverrideCreator != null)
			{
				if (contract.HasParameterizedCreator)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryDictionary();
				}
				createdFromNonDefaultCreator = false;
				return (IDictionary)contract.OverrideCreator(new object[0]);
			}
			else
			{
				if (contract.IsReadOnlyOrFixedSize)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryDictionary();
				}
				if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
				{
					object obj = contract.DefaultCreator();
					if (contract.ShouldCreateWrapper)
					{
						obj = contract.CreateWrapper(obj);
					}
					createdFromNonDefaultCreator = false;
					return (IDictionary)obj;
				}
				if (contract.HasParameterizedCreatorInternal)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryDictionary();
				}
				if (!contract.IsInstantiable)
				{
					throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				throw JsonSerializationException.Create(reader, "Unable to find a default constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x000590A4 File Offset: 0x000572A4
		private void OnDeserializing(JsonReader reader, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnDeserializing(value, this.Serializer._context);
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x00059118 File Offset: 0x00057318
		private void OnDeserialized(JsonReader reader, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnDeserialized(value, this.Serializer._context);
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0005918C File Offset: 0x0005738C
		private object PopulateDictionary(IDictionary dictionary, JsonReader reader, JsonDictionaryContract contract, JsonProperty containerProperty, string id)
		{
			IWrappedDictionary wrappedDictionary = dictionary as IWrappedDictionary;
			object obj = ((wrappedDictionary != null) ? wrappedDictionary.UnderlyingDictionary : dictionary);
			if (id != null)
			{
				this.AddReference(reader, id, obj);
			}
			this.OnDeserializing(reader, contract, obj);
			int depth = reader.Depth;
			if (contract.KeyContract == null)
			{
				contract.KeyContract = this.GetContractSafe(contract.DictionaryKeyType);
			}
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.GetContractSafe(contract.DictionaryValueType);
			}
			JsonConverter jsonConverter = contract.ItemConverter ?? this.GetConverter(contract.ItemContract, null, contract, containerProperty);
			JsonPrimitiveContract jsonPrimitiveContract = contract.KeyContract as JsonPrimitiveContract;
			PrimitiveTypeCode primitiveTypeCode = ((jsonPrimitiveContract != null) ? jsonPrimitiveContract.TypeCode : PrimitiveTypeCode.Empty);
			bool flag = false;
			for (;;)
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType != JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.Comment)
					{
						if (tokenType != JsonToken.EndObject)
						{
							break;
						}
						goto IL_02A4;
					}
				}
				else
				{
					object obj2 = reader.Value;
					if (!this.CheckPropertyName(reader, obj2.ToString()))
					{
						try
						{
							try
							{
								DateTime dateTime;
								if (primitiveTypeCode - PrimitiveTypeCode.DateTime > 1)
								{
									DateTimeOffset dateTimeOffset;
									if (primitiveTypeCode - PrimitiveTypeCode.DateTimeOffset > 1)
									{
										obj2 = this.EnsureType(reader, obj2, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
									}
									else if (DateTimeUtils.TryParseDateTimeOffset(obj2.ToString(), reader.DateFormatString, reader.Culture, out dateTimeOffset))
									{
										obj2 = dateTimeOffset;
									}
									else
									{
										obj2 = this.EnsureType(reader, obj2, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
									}
								}
								else if (DateTimeUtils.TryParseDateTime(obj2.ToString(), reader.DateTimeZoneHandling, reader.DateFormatString, reader.Culture, out dateTime))
								{
									obj2 = dateTime;
								}
								else
								{
									obj2 = this.EnsureType(reader, obj2, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
								}
							}
							catch (Exception ex)
							{
								throw JsonSerializationException.Create(reader, "Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object.".FormatWith(CultureInfo.InvariantCulture, reader.Value, contract.DictionaryKeyType), ex);
							}
							if (!reader.ReadForType(contract.ItemContract, jsonConverter != null))
							{
								throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
							}
							object obj3;
							if (jsonConverter != null && jsonConverter.CanRead)
							{
								obj3 = this.DeserializeConvertable(jsonConverter, reader, contract.DictionaryValueType, null);
							}
							else
							{
								obj3 = this.CreateValueInternal(reader, contract.DictionaryValueType, contract.ItemContract, null, contract, containerProperty, null);
							}
							dictionary[obj2] = obj3;
							goto IL_02C8;
						}
						catch (Exception ex2)
						{
							if (base.IsErrorHandled(obj, contract, obj2, reader as IJsonLineInfo, reader.Path, ex2))
							{
								this.HandleError(reader, true, depth);
								goto IL_02C8;
							}
							throw;
						}
						goto IL_02A4;
					}
				}
				IL_02C8:
				if (flag || !reader.Read())
				{
					goto IL_02DA;
				}
				continue;
				IL_02A4:
				flag = true;
				goto IL_02C8;
			}
			throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
			IL_02DA:
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, obj, "Unexpected end when deserializing object.");
			}
			this.OnDeserialized(reader, contract, obj);
			return obj;
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x000594C8 File Offset: 0x000576C8
		private object PopulateMultidimensionalArray(IList list, JsonReader reader, JsonArrayContract contract, JsonProperty containerProperty, string id)
		{
			int arrayRank = contract.UnderlyingType.GetArrayRank();
			if (id != null)
			{
				this.AddReference(reader, id, list);
			}
			this.OnDeserializing(reader, contract, list);
			JsonContract contractSafe = this.GetContractSafe(contract.CollectionItemType);
			JsonConverter converter = this.GetConverter(contractSafe, null, contract, containerProperty);
			int? num = null;
			Stack<IList> stack = new Stack<IList>();
			stack.Push(list);
			IList list2 = list;
			bool flag = false;
			for (;;)
			{
				int depth = reader.Depth;
				JsonToken jsonToken;
				if (stack.Count == arrayRank)
				{
					try
					{
						if (reader.ReadForType(contractSafe, converter != null))
						{
							jsonToken = reader.TokenType;
							if (jsonToken == JsonToken.EndArray)
							{
								stack.Pop();
								list2 = stack.Peek();
								num = null;
							}
							else
							{
								object obj;
								if (converter != null && converter.CanRead)
								{
									obj = this.DeserializeConvertable(converter, reader, contract.CollectionItemType, null);
								}
								else
								{
									obj = this.CreateValueInternal(reader, contract.CollectionItemType, contractSafe, null, contract, containerProperty, null);
								}
								list2.Add(obj);
							}
							goto IL_023D;
						}
						goto IL_0244;
					}
					catch (Exception ex)
					{
						JsonPosition position = reader.GetPosition(depth);
						if (!base.IsErrorHandled(list, contract, position.Position, reader as IJsonLineInfo, reader.Path, ex))
						{
							throw;
						}
						this.HandleError(reader, true, depth);
						if (num != null && num == position.Position)
						{
							throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
						}
						num = new int?(position.Position);
						goto IL_023D;
					}
					goto IL_01A2;
				}
				goto IL_01A2;
				IL_023D:
				if (flag)
				{
					goto IL_0244;
				}
				continue;
				IL_01A2:
				if (!reader.Read())
				{
					goto IL_0244;
				}
				jsonToken = reader.TokenType;
				if (jsonToken == JsonToken.StartArray)
				{
					IList list3 = new List<object>();
					list2.Add(list3);
					stack.Push(list3);
					list2 = list3;
					goto IL_023D;
				}
				if (jsonToken == JsonToken.Comment)
				{
					goto IL_023D;
				}
				if (jsonToken != JsonToken.EndArray)
				{
					break;
				}
				stack.Pop();
				if (stack.Count > 0)
				{
					list2 = stack.Peek();
					goto IL_023D;
				}
				flag = true;
				goto IL_023D;
			}
			throw JsonSerializationException.Create(reader, "Unexpected token when deserializing multidimensional array: " + reader.TokenType);
			IL_0244:
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, list, "Unexpected end when deserializing array.");
			}
			this.OnDeserialized(reader, contract, list);
			return list;
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x00059748 File Offset: 0x00057948
		private void ThrowUnexpectedEndException(JsonReader reader, JsonContract contract, object currentObject, string message)
		{
			try
			{
				throw JsonSerializationException.Create(reader, message);
			}
			catch (Exception ex)
			{
				if (!base.IsErrorHandled(currentObject, contract, null, reader as IJsonLineInfo, reader.Path, ex))
				{
					throw;
				}
				this.HandleError(reader, false, 0);
			}
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x000597A0 File Offset: 0x000579A0
		private object PopulateList(IList list, JsonReader reader, JsonArrayContract contract, JsonProperty containerProperty, string id)
		{
			IWrappedCollection wrappedCollection = list as IWrappedCollection;
			object obj = ((wrappedCollection != null) ? wrappedCollection.UnderlyingCollection : list);
			if (id != null)
			{
				this.AddReference(reader, id, obj);
			}
			if (list.IsFixedSize)
			{
				reader.Skip();
				return obj;
			}
			this.OnDeserializing(reader, contract, obj);
			int depth = reader.Depth;
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.GetContractSafe(contract.CollectionItemType);
			}
			JsonConverter converter = this.GetConverter(contract.ItemContract, null, contract, containerProperty);
			int? num = null;
			bool flag = false;
			do
			{
				try
				{
					if (!reader.ReadForType(contract.ItemContract, converter != null))
					{
						break;
					}
					JsonToken tokenType = reader.TokenType;
					if (tokenType == JsonToken.EndArray)
					{
						flag = true;
					}
					else
					{
						object obj2;
						if (converter != null && converter.CanRead)
						{
							obj2 = this.DeserializeConvertable(converter, reader, contract.CollectionItemType, null);
						}
						else
						{
							obj2 = this.CreateValueInternal(reader, contract.CollectionItemType, contract.ItemContract, null, contract, containerProperty, null);
						}
						list.Add(obj2);
					}
				}
				catch (Exception ex)
				{
					JsonPosition position = reader.GetPosition(depth);
					if (!base.IsErrorHandled(obj, contract, position.Position, reader as IJsonLineInfo, reader.Path, ex))
					{
						throw;
					}
					this.HandleError(reader, true, depth);
					if (num != null && num == position.Position)
					{
						throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
					}
					num = new int?(position.Position);
				}
			}
			while (!flag);
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, obj, "Unexpected end when deserializing array.");
			}
			this.OnDeserialized(reader, contract, obj);
			return obj;
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x00059990 File Offset: 0x00057B90
		private object CreateISerializable(JsonReader reader, JsonISerializableContract contract, JsonProperty member, string id)
		{
			Type underlyingType = contract.UnderlyingType;
			if (!JsonTypeReflector.FullyTrusted)
			{
				string text = "Type '{0}' implements ISerializable but cannot be deserialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data." + Environment.NewLine + "To fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true." + Environment.NewLine;
				text = text.FormatWith(CultureInfo.InvariantCulture, underlyingType);
				throw JsonSerializationException.Create(reader, text);
			}
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Deserializing {0} using ISerializable constructor.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new JsonFormatterConverter(this, contract, member));
			bool flag = false;
			string text2;
			for (;;)
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType != JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.Comment)
					{
						if (tokenType != JsonToken.EndObject)
						{
							break;
						}
						flag = true;
					}
				}
				else
				{
					text2 = reader.Value.ToString();
					if (!reader.Read())
					{
						goto Block_7;
					}
					serializationInfo.AddValue(text2, JToken.ReadFrom(reader));
				}
				if (flag || !reader.Read())
				{
					goto IL_0142;
				}
			}
			throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
			Block_7:
			throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text2));
			IL_0142:
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, serializationInfo, "Unexpected end when deserializing object.");
			}
			if (contract.ISerializableCreator == null)
			{
				throw JsonSerializationException.Create(reader, "ISerializable type '{0}' does not have a valid constructor. To correctly implement ISerializable a constructor that takes SerializationInfo and StreamingContext parameters should be present.".FormatWith(CultureInfo.InvariantCulture, underlyingType));
			}
			object obj = contract.ISerializableCreator(new object[]
			{
				serializationInfo,
				this.Serializer._context
			});
			if (id != null)
			{
				this.AddReference(reader, id, obj);
			}
			this.OnDeserializing(reader, contract, obj);
			this.OnDeserialized(reader, contract, obj);
			return obj;
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x00059B6C File Offset: 0x00057D6C
		internal object CreateISerializableItem(JToken token, Type type, JsonISerializableContract contract, JsonProperty member)
		{
			JsonContract contractSafe = this.GetContractSafe(type);
			JsonConverter converter = this.GetConverter(contractSafe, null, contract, member);
			JsonReader jsonReader = token.CreateReader();
			jsonReader.ReadAndAssert();
			object obj;
			if (converter != null && converter.CanRead)
			{
				obj = this.DeserializeConvertable(converter, jsonReader, type, null);
			}
			else
			{
				obj = this.CreateValueInternal(jsonReader, type, contractSafe, null, contract, member, null);
			}
			return obj;
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x00059BD0 File Offset: 0x00057DD0
		private object CreateDynamic(JsonReader reader, JsonDynamicContract contract, JsonProperty member, string id)
		{
			if (!contract.IsInstantiable)
			{
				throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				IDynamicMetaObjectProvider dynamicMetaObjectProvider = (IDynamicMetaObjectProvider)contract.DefaultCreator();
				if (id != null)
				{
					this.AddReference(reader, id, dynamicMetaObjectProvider);
				}
				this.OnDeserializing(reader, contract, dynamicMetaObjectProvider);
				int depth = reader.Depth;
				bool flag = false;
				for (;;)
				{
					JsonToken tokenType = reader.TokenType;
					if (tokenType == JsonToken.PropertyName)
					{
						string text = reader.Value.ToString();
						try
						{
							if (!reader.Read())
							{
								throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
							}
							JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(text);
							if (closestMatchProperty != null && closestMatchProperty.Writable && !closestMatchProperty.Ignored)
							{
								if (closestMatchProperty.PropertyContract == null)
								{
									closestMatchProperty.PropertyContract = this.GetContractSafe(closestMatchProperty.PropertyType);
								}
								JsonConverter converter = this.GetConverter(closestMatchProperty.PropertyContract, closestMatchProperty.MemberConverter, null, null);
								if (!this.SetPropertyValue(closestMatchProperty, converter, null, member, reader, dynamicMetaObjectProvider))
								{
									reader.Skip();
								}
							}
							else
							{
								Type type = (JsonTokenUtils.IsPrimitiveToken(reader.TokenType) ? reader.ValueType : typeof(IDynamicMetaObjectProvider));
								JsonContract contractSafe = this.GetContractSafe(type);
								JsonConverter converter2 = this.GetConverter(contractSafe, null, null, member);
								object obj;
								if (converter2 != null && converter2.CanRead)
								{
									obj = this.DeserializeConvertable(converter2, reader, type, null);
								}
								else
								{
									obj = this.CreateValueInternal(reader, type, contractSafe, null, null, member, null);
								}
								contract.TrySetMember(dynamicMetaObjectProvider, text, obj);
							}
							goto IL_024F;
						}
						catch (Exception ex)
						{
							if (base.IsErrorHandled(dynamicMetaObjectProvider, contract, text, reader as IJsonLineInfo, reader.Path, ex))
							{
								this.HandleError(reader, true, depth);
								goto IL_024F;
							}
							throw;
						}
						goto IL_022C;
					}
					if (tokenType != JsonToken.EndObject)
					{
						break;
					}
					goto IL_022C;
					IL_024F:
					if (flag || !reader.Read())
					{
						goto IL_0260;
					}
					continue;
					IL_022C:
					flag = true;
					goto IL_024F;
				}
				throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
				IL_0260:
				if (!flag)
				{
					this.ThrowUnexpectedEndException(reader, contract, dynamicMetaObjectProvider, "Unexpected end when deserializing object.");
				}
				this.OnDeserialized(reader, contract, dynamicMetaObjectProvider);
				return dynamicMetaObjectProvider;
			}
			throw JsonSerializationException.Create(reader, "Unable to find a default constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x00059E78 File Offset: 0x00058078
		private object CreateObjectUsingCreatorWithParameters(JsonReader reader, JsonObjectContract contract, JsonProperty containerProperty, ObjectConstructor<object> creator, string id)
		{
			ValidationUtils.ArgumentNotNull(creator, "creator");
			bool flag = contract.HasRequiredOrDefaultValueProperties || this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate);
			Type underlyingType = contract.UnderlyingType;
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				string text = string.Join(", ", contract.CreatorParameters.Select((JsonProperty p) => p.PropertyName));
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Deserializing {0} using creator with parameters: {1}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType, text)), null);
			}
			List<JsonSerializerInternalReader.CreatorPropertyContext> list = this.ResolvePropertyAndCreatorValues(contract, containerProperty, reader, underlyingType);
			if (flag)
			{
				using (IEnumerator<JsonProperty> enumerator = contract.Properties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JsonProperty property = enumerator.Current;
						if (list.All((JsonSerializerInternalReader.CreatorPropertyContext p) => p.Property != property))
						{
							list.Add(new JsonSerializerInternalReader.CreatorPropertyContext
							{
								Property = property,
								Name = property.PropertyName,
								Presence = new JsonSerializerInternalReader.PropertyPresence?(JsonSerializerInternalReader.PropertyPresence.None)
							});
						}
					}
				}
			}
			object[] array = new object[contract.CreatorParameters.Count];
			foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext in list)
			{
				if (flag && creatorPropertyContext.Property != null && creatorPropertyContext.Presence == null)
				{
					object value = creatorPropertyContext.Value;
					JsonSerializerInternalReader.PropertyPresence propertyPresence;
					if (value == null)
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Null;
					}
					else if (value is string)
					{
						propertyPresence = (JsonSerializerInternalReader.CoerceEmptyStringToNull(creatorPropertyContext.Property.PropertyType, creatorPropertyContext.Property.PropertyContract, (string)value) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value);
					}
					else
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Value;
					}
					creatorPropertyContext.Presence = new JsonSerializerInternalReader.PropertyPresence?(propertyPresence);
				}
				JsonProperty jsonProperty = creatorPropertyContext.ConstructorProperty;
				if (jsonProperty == null && creatorPropertyContext.Property != null)
				{
					jsonProperty = contract.CreatorParameters.ForgivingCaseSensitiveFind((JsonProperty p) => p.PropertyName, creatorPropertyContext.Property.UnderlyingName);
				}
				if (jsonProperty != null && !jsonProperty.Ignored)
				{
					if (flag && (creatorPropertyContext.Presence == JsonSerializerInternalReader.PropertyPresence.None || creatorPropertyContext.Presence == JsonSerializerInternalReader.PropertyPresence.Null))
					{
						if (jsonProperty.PropertyContract == null)
						{
							jsonProperty.PropertyContract = this.GetContractSafe(jsonProperty.PropertyType);
						}
						if (this.HasFlag(jsonProperty.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate))
						{
							creatorPropertyContext.Value = this.EnsureType(reader, jsonProperty.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, jsonProperty.PropertyContract, jsonProperty.PropertyType);
						}
					}
					int num = contract.CreatorParameters.IndexOf(jsonProperty);
					array[num] = creatorPropertyContext.Value;
					creatorPropertyContext.Used = true;
				}
			}
			object obj = creator(array);
			if (id != null)
			{
				this.AddReference(reader, id, obj);
			}
			this.OnDeserializing(reader, contract, obj);
			foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext2 in list)
			{
				if (!creatorPropertyContext2.Used && creatorPropertyContext2.Property != null && !creatorPropertyContext2.Property.Ignored && !(creatorPropertyContext2.Presence == JsonSerializerInternalReader.PropertyPresence.None))
				{
					JsonProperty property2 = creatorPropertyContext2.Property;
					object value2 = creatorPropertyContext2.Value;
					if (this.ShouldSetPropertyValue(property2, value2))
					{
						property2.ValueProvider.SetValue(obj, value2);
						creatorPropertyContext2.Used = true;
					}
					else if (!property2.Writable && value2 != null)
					{
						JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(property2.PropertyType);
						if (jsonContract.ContractType != JsonContractType.Array)
						{
							goto IL_0558;
						}
						JsonArrayContract jsonArrayContract = (JsonArrayContract)jsonContract;
						if (jsonArrayContract.CanDeserialize)
						{
							object value3 = property2.ValueProvider.GetValue(obj);
							if (value3 != null)
							{
								IList list2;
								if (!jsonArrayContract.ShouldCreateWrapper)
								{
									list2 = (IList)value3;
								}
								else
								{
									IList list3 = jsonArrayContract.CreateWrapper(value3);
									list2 = list3;
								}
								IList list4 = list2;
								IEnumerable enumerable;
								if (!jsonArrayContract.ShouldCreateWrapper)
								{
									enumerable = (IList)value2;
								}
								else
								{
									IList list3 = jsonArrayContract.CreateWrapper(value2);
									enumerable = list3;
								}
								using (IEnumerator enumerator3 = enumerable.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										object obj2 = enumerator3.Current;
										list4.Add(obj2);
									}
									goto IL_062E;
								}
								goto IL_0558;
							}
						}
						IL_062E:
						creatorPropertyContext2.Used = true;
						continue;
						IL_0558:
						if (jsonContract.ContractType != JsonContractType.Dictionary)
						{
							goto IL_062E;
						}
						JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)jsonContract;
						if (jsonDictionaryContract.IsReadOnlyOrFixedSize)
						{
							goto IL_062E;
						}
						object value4 = property2.ValueProvider.GetValue(obj);
						if (value4 != null)
						{
							IDictionary dictionary;
							if (!jsonDictionaryContract.ShouldCreateWrapper)
							{
								dictionary = (IDictionary)value4;
							}
							else
							{
								IDictionary dictionary2 = jsonDictionaryContract.CreateWrapper(value4);
								dictionary = dictionary2;
							}
							IDictionary dictionary3 = dictionary;
							IDictionary dictionary4;
							if (!jsonDictionaryContract.ShouldCreateWrapper)
							{
								dictionary4 = (IDictionary)value2;
							}
							else
							{
								IDictionary dictionary2 = jsonDictionaryContract.CreateWrapper(value2);
								dictionary4 = dictionary2;
							}
							using (IDictionaryEnumerator enumerator4 = dictionary4.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									DictionaryEntry entry = enumerator4.Entry;
									dictionary3[entry.Key] = entry.Value;
								}
							}
							goto IL_062E;
						}
						goto IL_062E;
					}
				}
			}
			if (contract.ExtensionDataSetter != null)
			{
				foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext3 in list)
				{
					if (!creatorPropertyContext3.Used)
					{
						contract.ExtensionDataSetter(obj, creatorPropertyContext3.Name, creatorPropertyContext3.Value);
					}
				}
			}
			if (flag)
			{
				foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext4 in list)
				{
					if (creatorPropertyContext4.Property != null)
					{
						this.EndProcessProperty(obj, reader, contract, reader.Depth, creatorPropertyContext4.Property, creatorPropertyContext4.Presence.GetValueOrDefault(), !creatorPropertyContext4.Used);
					}
				}
			}
			this.OnDeserialized(reader, contract, obj);
			return obj;
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0005A668 File Offset: 0x00058868
		private object DeserializeConvertable(JsonConverter converter, JsonReader reader, Type objectType, object existingValue)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, objectType, converter.GetType())), null);
			}
			object obj = converter.ReadJson(reader, objectType, existingValue, this.GetInternalSerializer());
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, objectType, converter.GetType())), null);
			}
			return obj;
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x0005A72C File Offset: 0x0005892C
		private List<JsonSerializerInternalReader.CreatorPropertyContext> ResolvePropertyAndCreatorValues(JsonObjectContract contract, JsonProperty containerProperty, JsonReader reader, Type objectType)
		{
			List<JsonSerializerInternalReader.CreatorPropertyContext> list = new List<JsonSerializerInternalReader.CreatorPropertyContext>();
			bool flag = false;
			string text;
			for (;;)
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType != JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.Comment)
					{
						if (tokenType != JsonToken.EndObject)
						{
							break;
						}
						flag = true;
					}
				}
				else
				{
					text = reader.Value.ToString();
					JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext = new JsonSerializerInternalReader.CreatorPropertyContext
					{
						Name = reader.Value.ToString(),
						ConstructorProperty = contract.CreatorParameters.GetClosestMatchProperty(text),
						Property = contract.Properties.GetClosestMatchProperty(text)
					};
					list.Add(creatorPropertyContext);
					JsonProperty jsonProperty = creatorPropertyContext.ConstructorProperty ?? creatorPropertyContext.Property;
					if (jsonProperty != null && !jsonProperty.Ignored)
					{
						if (jsonProperty.PropertyContract == null)
						{
							jsonProperty.PropertyContract = this.GetContractSafe(jsonProperty.PropertyType);
						}
						JsonConverter converter = this.GetConverter(jsonProperty.PropertyContract, jsonProperty.MemberConverter, contract, containerProperty);
						if (!reader.ReadForType(jsonProperty.PropertyContract, converter != null))
						{
							goto Block_8;
						}
						if (converter != null && converter.CanRead)
						{
							creatorPropertyContext.Value = this.DeserializeConvertable(converter, reader, jsonProperty.PropertyType, null);
						}
						else
						{
							creatorPropertyContext.Value = this.CreateValueInternal(reader, jsonProperty.PropertyType, jsonProperty.PropertyContract, jsonProperty, contract, containerProperty, null);
						}
					}
					else
					{
						if (!reader.Read())
						{
							goto Block_11;
						}
						if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
						{
							this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}.".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType)), null);
						}
						if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
						{
							goto Block_14;
						}
						if (contract.ExtensionDataSetter != null)
						{
							creatorPropertyContext.Value = this.ReadExtensionDataValue(contract, containerProperty, reader);
						}
						else
						{
							reader.Skip();
						}
					}
				}
				if (flag || !reader.Read())
				{
					goto IL_0260;
				}
			}
			throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
			Block_8:
			throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
			Block_11:
			throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
			Block_14:
			throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, objectType.Name));
			IL_0260:
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, null, "Unexpected end when deserializing object.");
			}
			return list;
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x0005A9B4 File Offset: 0x00058BB4
		public object CreateNewObject(JsonReader reader, JsonObjectContract objectContract, JsonProperty containerMember, JsonProperty containerProperty, string id, out bool createdFromNonDefaultCreator)
		{
			object obj = null;
			if (objectContract.OverrideCreator != null)
			{
				if (objectContract.CreatorParameters.Count > 0)
				{
					createdFromNonDefaultCreator = true;
					return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.OverrideCreator, id);
				}
				obj = objectContract.OverrideCreator(CollectionUtils.ArrayEmpty<object>());
			}
			else if (objectContract.DefaultCreator != null && (!objectContract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor || objectContract.ParameterizedCreator == null))
			{
				obj = objectContract.DefaultCreator();
			}
			else if (objectContract.ParameterizedCreator != null)
			{
				createdFromNonDefaultCreator = true;
				return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.ParameterizedCreator, id);
			}
			if (obj != null)
			{
				createdFromNonDefaultCreator = false;
				return obj;
			}
			if (!objectContract.IsInstantiable)
			{
				throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
			}
			throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.".FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x0005AAC4 File Offset: 0x00058CC4
		private object PopulateObject(object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, string id)
		{
			this.OnDeserializing(reader, contract, newObject);
			Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary;
			if (!contract.HasRequiredOrDefaultValueProperties && !this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate))
			{
				dictionary = null;
			}
			else
			{
				dictionary = contract.Properties.ToDictionary((JsonProperty m) => m, (JsonProperty m) => JsonSerializerInternalReader.PropertyPresence.None);
			}
			Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary2 = dictionary;
			if (id != null)
			{
				this.AddReference(reader, id, newObject);
			}
			int depth = reader.Depth;
			bool flag = false;
			for (;;)
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType != JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.Comment)
					{
						if (tokenType != JsonToken.EndObject)
						{
							break;
						}
						goto IL_02AB;
					}
				}
				else
				{
					string text = reader.Value.ToString();
					if (!this.CheckPropertyName(reader, text))
					{
						try
						{
							JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(text);
							if (closestMatchProperty != null)
							{
								if (closestMatchProperty.Ignored || !this.ShouldDeserialize(reader, closestMatchProperty, newObject))
								{
									if (reader.Read())
									{
										this.SetPropertyPresence(reader, closestMatchProperty, dictionary2);
										this.SetExtensionData(contract, member, reader, text, newObject);
									}
								}
								else
								{
									if (closestMatchProperty.PropertyContract == null)
									{
										closestMatchProperty.PropertyContract = this.GetContractSafe(closestMatchProperty.PropertyType);
									}
									JsonConverter converter = this.GetConverter(closestMatchProperty.PropertyContract, closestMatchProperty.MemberConverter, contract, member);
									if (!reader.ReadForType(closestMatchProperty.PropertyContract, converter != null))
									{
										throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
									}
									this.SetPropertyPresence(reader, closestMatchProperty, dictionary2);
									if (!this.SetPropertyValue(closestMatchProperty, converter, contract, member, reader, newObject))
									{
										this.SetExtensionData(contract, member, reader, text, newObject);
									}
								}
								goto IL_02CE;
							}
							if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
							{
								this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType)), null);
							}
							if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
							{
								throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType.Name));
							}
							if (!reader.Read())
							{
								goto IL_02CE;
							}
							this.SetExtensionData(contract, member, reader, text, newObject);
							goto IL_02CE;
						}
						catch (Exception ex)
						{
							if (base.IsErrorHandled(newObject, contract, text, reader as IJsonLineInfo, reader.Path, ex))
							{
								this.HandleError(reader, true, depth - 1);
								goto IL_02CE;
							}
							throw;
						}
						goto IL_02AB;
					}
				}
				IL_02CE:
				if (flag || !reader.Read())
				{
					goto IL_02DF;
				}
				continue;
				IL_02AB:
				flag = true;
				goto IL_02CE;
			}
			throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
			IL_02DF:
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, newObject, "Unexpected end when deserializing object.");
			}
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<JsonProperty, JsonSerializerInternalReader.PropertyPresence> keyValuePair in dictionary2)
				{
					JsonProperty key = keyValuePair.Key;
					JsonSerializerInternalReader.PropertyPresence value = keyValuePair.Value;
					this.EndProcessProperty(newObject, reader, contract, depth, key, value, true);
				}
			}
			this.OnDeserialized(reader, contract, newObject);
			return newObject;
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0005AE5C File Offset: 0x0005905C
		private bool ShouldDeserialize(JsonReader reader, JsonProperty property, object target)
		{
			if (property.ShouldDeserialize == null)
			{
				return true;
			}
			bool flag = property.ShouldDeserialize(target);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, reader.Path, "ShouldDeserialize result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, flag)), null);
			}
			return flag;
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x0005AEE0 File Offset: 0x000590E0
		private bool CheckPropertyName(JsonReader reader, string memberName)
		{
			if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead && (memberName == "$id" || memberName == "$ref" || memberName == "$type" || memberName == "$values"))
			{
				reader.Skip();
				return true;
			}
			return false;
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x0005AF4C File Offset: 0x0005914C
		private void SetExtensionData(JsonObjectContract contract, JsonProperty member, JsonReader reader, string memberName, object o)
		{
			if (contract.ExtensionDataSetter != null)
			{
				try
				{
					object obj = this.ReadExtensionDataValue(contract, member, reader);
					contract.ExtensionDataSetter(o, memberName, obj);
					return;
				}
				catch (Exception ex)
				{
					throw JsonSerializationException.Create(reader, "Error setting value in extension data for type '{0}'.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType), ex);
				}
			}
			reader.Skip();
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0005AFB8 File Offset: 0x000591B8
		private object ReadExtensionDataValue(JsonObjectContract contract, JsonProperty member, JsonReader reader)
		{
			object obj;
			if (contract.ExtensionDataIsJToken)
			{
				obj = JToken.ReadFrom(reader);
			}
			else
			{
				obj = this.CreateValueInternal(reader, null, null, null, contract, member, null);
			}
			return obj;
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0005AFF0 File Offset: 0x000591F0
		private void EndProcessProperty(object newObject, JsonReader reader, JsonObjectContract contract, int initialDepth, JsonProperty property, JsonSerializerInternalReader.PropertyPresence presence, bool setDefaultValue)
		{
			if (presence == JsonSerializerInternalReader.PropertyPresence.None || presence == JsonSerializerInternalReader.PropertyPresence.Null)
			{
				try
				{
					Required required = (property.Ignored ? Required.Default : (property._required ?? contract.ItemRequired ?? Required.Default));
					if (presence != JsonSerializerInternalReader.PropertyPresence.None)
					{
						if (presence == JsonSerializerInternalReader.PropertyPresence.Null)
						{
							if (required == Required.Always)
							{
								throw JsonSerializationException.Create(reader, "Required property '{0}' expects a value but got null.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
							}
							if (required == Required.DisallowNull)
							{
								throw JsonSerializationException.Create(reader, "Required property '{0}' expects a non-null value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
							}
						}
					}
					else
					{
						if (required == Required.AllowNull || required == Required.Always)
						{
							throw JsonSerializationException.Create(reader, "Required property '{0}' not found in JSON.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
						}
						if (setDefaultValue && !property.Ignored)
						{
							if (property.PropertyContract == null)
							{
								property.PropertyContract = this.GetContractSafe(property.PropertyType);
							}
							if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) && property.Writable)
							{
								property.ValueProvider.SetValue(newObject, this.EnsureType(reader, property.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, property.PropertyContract, property.PropertyType));
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (!base.IsErrorHandled(newObject, contract, property.PropertyName, reader as IJsonLineInfo, reader.Path, ex))
					{
						throw;
					}
					this.HandleError(reader, true, initialDepth);
				}
			}
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0005B1DC File Offset: 0x000593DC
		private void SetPropertyPresence(JsonReader reader, JsonProperty property, Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> requiredProperties)
		{
			if (property != null && requiredProperties != null)
			{
				JsonToken tokenType = reader.TokenType;
				JsonSerializerInternalReader.PropertyPresence propertyPresence;
				if (tokenType != JsonToken.String)
				{
					if (tokenType - JsonToken.Null > 1)
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Value;
					}
					else
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Null;
					}
				}
				else
				{
					propertyPresence = (JsonSerializerInternalReader.CoerceEmptyStringToNull(property.PropertyType, property.PropertyContract, (string)reader.Value) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value);
				}
				requiredProperties[property] = propertyPresence;
			}
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0005B258 File Offset: 0x00059458
		private void HandleError(JsonReader reader, bool readPastError, int initialDepth)
		{
			base.ClearErrorContext();
			if (readPastError)
			{
				reader.Skip();
				while (reader.Depth > initialDepth + 1 && reader.Read())
				{
				}
			}
		}

		// Token: 0x02000279 RID: 633
		internal enum PropertyPresence
		{
			// Token: 0x04000B47 RID: 2887
			None,
			// Token: 0x04000B48 RID: 2888
			Null,
			// Token: 0x04000B49 RID: 2889
			Value
		}

		// Token: 0x0200027A RID: 634
		internal class CreatorPropertyContext
		{
			// Token: 0x04000B4A RID: 2890
			public string Name;

			// Token: 0x04000B4B RID: 2891
			public JsonProperty Property;

			// Token: 0x04000B4C RID: 2892
			public JsonProperty ConstructorProperty;

			// Token: 0x04000B4D RID: 2893
			public JsonSerializerInternalReader.PropertyPresence? Presence;

			// Token: 0x04000B4E RID: 2894
			public object Value;

			// Token: 0x04000B4F RID: 2895
			public bool Used;
		}
	}
}
