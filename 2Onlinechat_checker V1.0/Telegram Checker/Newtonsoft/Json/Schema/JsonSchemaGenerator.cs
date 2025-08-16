using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x02000133 RID: 307
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public class JsonSchemaGenerator
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06001026 RID: 4134 RVA: 0x00060658 File Offset: 0x0005E858
		// (set) Token: 0x06001027 RID: 4135 RVA: 0x00060660 File Offset: 0x0005E860
		public UndefinedSchemaIdHandling UndefinedSchemaIdHandling { get; set; }

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06001028 RID: 4136 RVA: 0x0006066C File Offset: 0x0005E86C
		// (set) Token: 0x06001029 RID: 4137 RVA: 0x00060688 File Offset: 0x0005E888
		public IContractResolver ContractResolver
		{
			get
			{
				if (this._contractResolver == null)
				{
					return DefaultContractResolver.Instance;
				}
				return this._contractResolver;
			}
			set
			{
				this._contractResolver = value;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600102A RID: 4138 RVA: 0x00060694 File Offset: 0x0005E894
		private JsonSchema CurrentSchema
		{
			get
			{
				return this._currentSchema;
			}
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x0006069C File Offset: 0x0005E89C
		private void Push(JsonSchemaGenerator.TypeSchema typeSchema)
		{
			this._currentSchema = typeSchema.Schema;
			this._stack.Add(typeSchema);
			this._resolver.LoadedSchemas.Add(typeSchema.Schema);
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x000606DC File Offset: 0x0005E8DC
		private JsonSchemaGenerator.TypeSchema Pop()
		{
			JsonSchemaGenerator.TypeSchema typeSchema = this._stack[this._stack.Count - 1];
			this._stack.RemoveAt(this._stack.Count - 1);
			JsonSchemaGenerator.TypeSchema typeSchema2 = this._stack.LastOrDefault<JsonSchemaGenerator.TypeSchema>();
			if (typeSchema2 != null)
			{
				this._currentSchema = typeSchema2.Schema;
				return typeSchema;
			}
			this._currentSchema = null;
			return typeSchema;
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x00060744 File Offset: 0x0005E944
		public JsonSchema Generate(Type type)
		{
			return this.Generate(type, new JsonSchemaResolver(), false);
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x00060754 File Offset: 0x0005E954
		public JsonSchema Generate(Type type, JsonSchemaResolver resolver)
		{
			return this.Generate(type, resolver, false);
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00060760 File Offset: 0x0005E960
		public JsonSchema Generate(Type type, bool rootSchemaNullable)
		{
			return this.Generate(type, new JsonSchemaResolver(), rootSchemaNullable);
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x00060770 File Offset: 0x0005E970
		public JsonSchema Generate(Type type, JsonSchemaResolver resolver, bool rootSchemaNullable)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			ValidationUtils.ArgumentNotNull(resolver, "resolver");
			this._resolver = resolver;
			return this.GenerateInternal(type, (!rootSchemaNullable) ? Required.Always : Required.Default, false);
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x000607A4 File Offset: 0x0005E9A4
		private string GetTitle(Type type)
		{
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
			if (!string.IsNullOrEmpty((cachedAttribute != null) ? cachedAttribute.Title : null))
			{
				return cachedAttribute.Title;
			}
			return null;
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x000607E0 File Offset: 0x0005E9E0
		private string GetDescription(Type type)
		{
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
			if (!string.IsNullOrEmpty((cachedAttribute != null) ? cachedAttribute.Description : null))
			{
				return cachedAttribute.Description;
			}
			DescriptionAttribute attribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(type);
			if (attribute == null)
			{
				return null;
			}
			return attribute.Description;
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00060830 File Offset: 0x0005EA30
		private string GetTypeId(Type type, bool explicitOnly)
		{
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
			if (!string.IsNullOrEmpty((cachedAttribute != null) ? cachedAttribute.Id : null))
			{
				return cachedAttribute.Id;
			}
			if (explicitOnly)
			{
				return null;
			}
			UndefinedSchemaIdHandling undefinedSchemaIdHandling = this.UndefinedSchemaIdHandling;
			if (undefinedSchemaIdHandling == UndefinedSchemaIdHandling.UseTypeName)
			{
				return type.FullName;
			}
			if (undefinedSchemaIdHandling != UndefinedSchemaIdHandling.UseAssemblyQualifiedName)
			{
				return null;
			}
			return type.AssemblyQualifiedName;
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x0006089C File Offset: 0x0005EA9C
		private JsonSchema GenerateInternal(Type type, Required valueRequired, bool required)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			string typeId = this.GetTypeId(type, false);
			string typeId2 = this.GetTypeId(type, true);
			if (!string.IsNullOrEmpty(typeId))
			{
				JsonSchema schema = this._resolver.GetSchema(typeId);
				if (schema != null)
				{
					if (valueRequired != Required.Always && !JsonSchemaGenerator.HasFlag(schema.Type, JsonSchemaType.Null))
					{
						schema.Type |= JsonSchemaType.Null;
					}
					if (required && schema.Required != true)
					{
						schema.Required = new bool?(true);
					}
					return schema;
				}
			}
			if (this._stack.Any((JsonSchemaGenerator.TypeSchema tc) => tc.Type == type))
			{
				throw new JsonException("Unresolved circular reference for type '{0}'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property.".FormatWith(CultureInfo.InvariantCulture, type));
			}
			JsonContract jsonContract = this.ContractResolver.ResolveContract(type);
			bool flag = (jsonContract.Converter ?? jsonContract.InternalConverter) != null;
			this.Push(new JsonSchemaGenerator.TypeSchema(type, new JsonSchema()));
			if (typeId2 != null)
			{
				this.CurrentSchema.Id = typeId2;
			}
			if (required)
			{
				this.CurrentSchema.Required = new bool?(true);
			}
			this.CurrentSchema.Title = this.GetTitle(type);
			this.CurrentSchema.Description = this.GetDescription(type);
			if (flag)
			{
				this.CurrentSchema.Type = new JsonSchemaType?(JsonSchemaType.Any);
			}
			else
			{
				switch (jsonContract.ContractType)
				{
				case JsonContractType.Object:
					this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
					this.CurrentSchema.Id = this.GetTypeId(type, false);
					this.GenerateObjectSchema(type, (JsonObjectContract)jsonContract);
					goto IL_04F8;
				case JsonContractType.Array:
				{
					this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Array, valueRequired));
					this.CurrentSchema.Id = this.GetTypeId(type, false);
					JsonArrayAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>(type);
					bool flag2 = cachedAttribute == null || cachedAttribute.AllowNullItems;
					Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
					if (collectionItemType != null)
					{
						this.CurrentSchema.Items = new List<JsonSchema>();
						this.CurrentSchema.Items.Add(this.GenerateInternal(collectionItemType, (!flag2) ? Required.Always : Required.Default, false));
						goto IL_04F8;
					}
					goto IL_04F8;
				}
				case JsonContractType.Primitive:
				{
					this.CurrentSchema.Type = new JsonSchemaType?(this.GetJsonSchemaType(type, valueRequired));
					if (!(this.CurrentSchema.Type == JsonSchemaType.Integer) || !type.IsEnum() || type.IsDefined(typeof(FlagsAttribute), true))
					{
						goto IL_04F8;
					}
					this.CurrentSchema.Enum = new List<JToken>();
					using (IEnumerator<EnumValue<long>> enumerator = EnumUtils.GetNamesAndValues<long>(type).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							EnumValue<long> enumValue = enumerator.Current;
							JToken jtoken = JToken.FromObject(enumValue.Value);
							this.CurrentSchema.Enum.Add(jtoken);
						}
						goto IL_04F8;
					}
					break;
				}
				case JsonContractType.String:
					break;
				case JsonContractType.Dictionary:
				{
					this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
					Type type2;
					Type type3;
					ReflectionUtils.GetDictionaryKeyValueTypes(type, out type2, out type3);
					if (type2 != null && this.ContractResolver.ResolveContract(type2).ContractType == JsonContractType.Primitive)
					{
						this.CurrentSchema.AdditionalProperties = this.GenerateInternal(type3, Required.Default, false);
						goto IL_04F8;
					}
					goto IL_04F8;
				}
				case JsonContractType.Dynamic:
				case JsonContractType.Linq:
					this.CurrentSchema.Type = new JsonSchemaType?(JsonSchemaType.Any);
					goto IL_04F8;
				case JsonContractType.Serializable:
					this.CurrentSchema.Type = new JsonSchemaType?(this.AddNullType(JsonSchemaType.Object, valueRequired));
					this.CurrentSchema.Id = this.GetTypeId(type, false);
					this.GenerateISerializableContract(type, (JsonISerializableContract)jsonContract);
					goto IL_04F8;
				default:
					throw new JsonException("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContract));
				}
				JsonSchemaType jsonSchemaType = ((!ReflectionUtils.IsNullable(jsonContract.UnderlyingType)) ? JsonSchemaType.String : this.AddNullType(JsonSchemaType.String, valueRequired));
				this.CurrentSchema.Type = new JsonSchemaType?(jsonSchemaType);
			}
			IL_04F8:
			return this.Pop().Schema;
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x00060DBC File Offset: 0x0005EFBC
		private JsonSchemaType AddNullType(JsonSchemaType type, Required valueRequired)
		{
			if (valueRequired != Required.Always)
			{
				return type | JsonSchemaType.Null;
			}
			return type;
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00060DCC File Offset: 0x0005EFCC
		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x00060DD4 File Offset: 0x0005EFD4
		private void GenerateObjectSchema(Type type, JsonObjectContract contract)
		{
			this.CurrentSchema.Properties = new Dictionary<string, JsonSchema>();
			foreach (JsonProperty jsonProperty in contract.Properties)
			{
				if (!jsonProperty.Ignored)
				{
					bool flag = jsonProperty.NullValueHandling == NullValueHandling.Ignore || this.HasFlag(jsonProperty.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) || jsonProperty.ShouldSerialize != null || jsonProperty.GetIsSpecified != null;
					JsonSchema jsonSchema = this.GenerateInternal(jsonProperty.PropertyType, jsonProperty.Required, !flag);
					if (jsonProperty.DefaultValue != null)
					{
						jsonSchema.Default = JToken.FromObject(jsonProperty.DefaultValue);
					}
					this.CurrentSchema.Properties.Add(jsonProperty.PropertyName, jsonSchema);
				}
			}
			if (type.IsSealed())
			{
				this.CurrentSchema.AllowAdditionalProperties = false;
			}
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x00060F08 File Offset: 0x0005F108
		private void GenerateISerializableContract(Type type, JsonISerializableContract contract)
		{
			this.CurrentSchema.AllowAdditionalProperties = true;
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00060F18 File Offset: 0x0005F118
		internal static bool HasFlag(JsonSchemaType? value, JsonSchemaType flag)
		{
			return value == null || (value & flag) == flag || (flag == JsonSchemaType.Integer && (value & JsonSchemaType.Float) == JsonSchemaType.Float);
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00060FE4 File Offset: 0x0005F1E4
		private JsonSchemaType GetJsonSchemaType(Type type, Required valueRequired)
		{
			JsonSchemaType jsonSchemaType = JsonSchemaType.None;
			if (valueRequired != Required.Always && ReflectionUtils.IsNullable(type))
			{
				jsonSchemaType = JsonSchemaType.Null;
				if (ReflectionUtils.IsNullableType(type))
				{
					type = Nullable.GetUnderlyingType(type);
				}
			}
			PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(type);
			switch (typeCode)
			{
			case PrimitiveTypeCode.Empty:
			case PrimitiveTypeCode.Object:
				return jsonSchemaType | JsonSchemaType.String;
			case PrimitiveTypeCode.Char:
				return jsonSchemaType | JsonSchemaType.String;
			case PrimitiveTypeCode.Boolean:
				return jsonSchemaType | JsonSchemaType.Boolean;
			case PrimitiveTypeCode.SByte:
			case PrimitiveTypeCode.Int16:
			case PrimitiveTypeCode.UInt16:
			case PrimitiveTypeCode.Int32:
			case PrimitiveTypeCode.Byte:
			case PrimitiveTypeCode.UInt32:
			case PrimitiveTypeCode.Int64:
			case PrimitiveTypeCode.UInt64:
			case PrimitiveTypeCode.BigInteger:
				return jsonSchemaType | JsonSchemaType.Integer;
			case PrimitiveTypeCode.Single:
			case PrimitiveTypeCode.Double:
			case PrimitiveTypeCode.Decimal:
				return jsonSchemaType | JsonSchemaType.Float;
			case PrimitiveTypeCode.DateTime:
			case PrimitiveTypeCode.DateTimeOffset:
				return jsonSchemaType | JsonSchemaType.String;
			case PrimitiveTypeCode.Guid:
			case PrimitiveTypeCode.TimeSpan:
			case PrimitiveTypeCode.Uri:
			case PrimitiveTypeCode.String:
			case PrimitiveTypeCode.Bytes:
				return jsonSchemaType | JsonSchemaType.String;
			case PrimitiveTypeCode.DBNull:
				return jsonSchemaType | JsonSchemaType.Null;
			}
			throw new JsonException("Unexpected type code '{0}' for type '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeCode, type));
		}

		// Token: 0x0400064A RID: 1610
		private IContractResolver _contractResolver;

		// Token: 0x0400064B RID: 1611
		private JsonSchemaResolver _resolver;

		// Token: 0x0400064C RID: 1612
		private readonly IList<JsonSchemaGenerator.TypeSchema> _stack = new List<JsonSchemaGenerator.TypeSchema>();

		// Token: 0x0400064D RID: 1613
		private JsonSchema _currentSchema;

		// Token: 0x02000282 RID: 642
		private class TypeSchema
		{
			// Token: 0x17000417 RID: 1047
			// (get) Token: 0x0600174A RID: 5962 RVA: 0x00083CB4 File Offset: 0x00081EB4
			public Type Type { get; }

			// Token: 0x17000418 RID: 1048
			// (get) Token: 0x0600174B RID: 5963 RVA: 0x00083CBC File Offset: 0x00081EBC
			public JsonSchema Schema { get; }

			// Token: 0x0600174C RID: 5964 RVA: 0x00083CC4 File Offset: 0x00081EC4
			public TypeSchema(Type type, JsonSchema schema)
			{
				ValidationUtils.ArgumentNotNull(type, "type");
				ValidationUtils.ArgumentNotNull(schema, "schema");
				this.Type = type;
				this.Schema = schema;
			}
		}
	}
}
