using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000B9 RID: 185
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public class JsonValidatingReader : JsonReader, IJsonLineInfo
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000A5A RID: 2650 RVA: 0x00043CD0 File Offset: 0x00041ED0
		// (remove) Token: 0x06000A5B RID: 2651 RVA: 0x00043D0C File Offset: 0x00041F0C
		public event ValidationEventHandler ValidationEventHandler;

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000A5C RID: 2652 RVA: 0x00043D48 File Offset: 0x00041F48
		public override object Value
		{
			get
			{
				return this._reader.Value;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000A5D RID: 2653 RVA: 0x00043D58 File Offset: 0x00041F58
		public override int Depth
		{
			get
			{
				return this._reader.Depth;
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000A5E RID: 2654 RVA: 0x00043D68 File Offset: 0x00041F68
		public override string Path
		{
			get
			{
				return this._reader.Path;
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000A5F RID: 2655 RVA: 0x00043D78 File Offset: 0x00041F78
		// (set) Token: 0x06000A60 RID: 2656 RVA: 0x00043D88 File Offset: 0x00041F88
		public override char QuoteChar
		{
			get
			{
				return this._reader.QuoteChar;
			}
			protected internal set
			{
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000A61 RID: 2657 RVA: 0x00043D8C File Offset: 0x00041F8C
		public override JsonToken TokenType
		{
			get
			{
				return this._reader.TokenType;
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000A62 RID: 2658 RVA: 0x00043D9C File Offset: 0x00041F9C
		public override Type ValueType
		{
			get
			{
				return this._reader.ValueType;
			}
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x00043DAC File Offset: 0x00041FAC
		private void Push(JsonValidatingReader.SchemaScope scope)
		{
			this._stack.Push(scope);
			this._currentScope = scope;
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00043DC4 File Offset: 0x00041FC4
		private JsonValidatingReader.SchemaScope Pop()
		{
			JsonValidatingReader.SchemaScope schemaScope = this._stack.Pop();
			this._currentScope = ((this._stack.Count != 0) ? this._stack.Peek() : null);
			return schemaScope;
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000A65 RID: 2661 RVA: 0x00043E08 File Offset: 0x00042008
		private IList<JsonSchemaModel> CurrentSchemas
		{
			get
			{
				return this._currentScope.Schemas;
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000A66 RID: 2662 RVA: 0x00043E18 File Offset: 0x00042018
		private IList<JsonSchemaModel> CurrentMemberSchemas
		{
			get
			{
				if (this._currentScope == null)
				{
					return new List<JsonSchemaModel>(new JsonSchemaModel[] { this._model });
				}
				if (this._currentScope.Schemas == null || this._currentScope.Schemas.Count == 0)
				{
					return JsonValidatingReader.EmptySchemaList;
				}
				switch (this._currentScope.TokenType)
				{
				case JTokenType.None:
					return this._currentScope.Schemas;
				case JTokenType.Object:
				{
					if (this._currentScope.CurrentPropertyName == null)
					{
						throw new JsonReaderException("CurrentPropertyName has not been set on scope.");
					}
					IList<JsonSchemaModel> list = new List<JsonSchemaModel>();
					foreach (JsonSchemaModel jsonSchemaModel in this.CurrentSchemas)
					{
						JsonSchemaModel jsonSchemaModel2;
						if (jsonSchemaModel.Properties != null && jsonSchemaModel.Properties.TryGetValue(this._currentScope.CurrentPropertyName, out jsonSchemaModel2))
						{
							list.Add(jsonSchemaModel2);
						}
						if (jsonSchemaModel.PatternProperties != null)
						{
							foreach (KeyValuePair<string, JsonSchemaModel> keyValuePair in jsonSchemaModel.PatternProperties)
							{
								if (Regex.IsMatch(this._currentScope.CurrentPropertyName, keyValuePair.Key))
								{
									list.Add(keyValuePair.Value);
								}
							}
						}
						if (list.Count == 0 && jsonSchemaModel.AllowAdditionalProperties && jsonSchemaModel.AdditionalProperties != null)
						{
							list.Add(jsonSchemaModel.AdditionalProperties);
						}
					}
					return list;
				}
				case JTokenType.Array:
				{
					IList<JsonSchemaModel> list2 = new List<JsonSchemaModel>();
					foreach (JsonSchemaModel jsonSchemaModel3 in this.CurrentSchemas)
					{
						if (!jsonSchemaModel3.PositionalItemsValidation)
						{
							if (jsonSchemaModel3.Items != null && jsonSchemaModel3.Items.Count > 0)
							{
								list2.Add(jsonSchemaModel3.Items[0]);
							}
						}
						else
						{
							if (jsonSchemaModel3.Items != null && jsonSchemaModel3.Items.Count > 0 && jsonSchemaModel3.Items.Count > this._currentScope.ArrayItemCount - 1)
							{
								list2.Add(jsonSchemaModel3.Items[this._currentScope.ArrayItemCount - 1]);
							}
							if (jsonSchemaModel3.AllowAdditionalItems && jsonSchemaModel3.AdditionalItems != null)
							{
								list2.Add(jsonSchemaModel3.AdditionalItems);
							}
						}
					}
					return list2;
				}
				case JTokenType.Constructor:
					return JsonValidatingReader.EmptySchemaList;
				default:
					throw new ArgumentOutOfRangeException("TokenType", "Unexpected token type: {0}".FormatWith(CultureInfo.InvariantCulture, this._currentScope.TokenType));
				}
			}
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00044120 File Offset: 0x00042320
		private void RaiseError(string message, JsonSchemaModel schema)
		{
			string text = (((IJsonLineInfo)this).HasLineInfo() ? (message + " Line {0}, position {1}.".FormatWith(CultureInfo.InvariantCulture, ((IJsonLineInfo)this).LineNumber, ((IJsonLineInfo)this).LinePosition)) : message);
			this.OnValidationEvent(new JsonSchemaException(text, null, this.Path, ((IJsonLineInfo)this).LineNumber, ((IJsonLineInfo)this).LinePosition));
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00044190 File Offset: 0x00042390
		private void OnValidationEvent(JsonSchemaException exception)
		{
			ValidationEventHandler validationEventHandler = this.ValidationEventHandler;
			if (validationEventHandler != null)
			{
				validationEventHandler(this, new ValidationEventArgs(exception));
				return;
			}
			throw exception;
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x000441C0 File Offset: 0x000423C0
		public JsonValidatingReader(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this._reader = reader;
			this._stack = new Stack<JsonValidatingReader.SchemaScope>();
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000A6A RID: 2666 RVA: 0x000441E8 File Offset: 0x000423E8
		// (set) Token: 0x06000A6B RID: 2667 RVA: 0x000441F0 File Offset: 0x000423F0
		public JsonSchema Schema
		{
			get
			{
				return this._schema;
			}
			set
			{
				if (this.TokenType != JsonToken.None)
				{
					throw new InvalidOperationException("Cannot change schema while validating JSON.");
				}
				this._schema = value;
				this._model = null;
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000A6C RID: 2668 RVA: 0x00044218 File Offset: 0x00042418
		public JsonReader Reader
		{
			get
			{
				return this._reader;
			}
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00044220 File Offset: 0x00042420
		public override void Close()
		{
			base.Close();
			if (base.CloseInput)
			{
				JsonReader reader = this._reader;
				if (reader == null)
				{
					return;
				}
				reader.Close();
			}
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x00044248 File Offset: 0x00042448
		private void ValidateNotDisallowed(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			JsonSchemaType? currentNodeSchemaType = this.GetCurrentNodeSchemaType();
			if (currentNodeSchemaType != null && JsonSchemaGenerator.HasFlag(new JsonSchemaType?(schema.Disallow), currentNodeSchemaType.GetValueOrDefault()))
			{
				this.RaiseError("Type {0} is disallowed.".FormatWith(CultureInfo.InvariantCulture, currentNodeSchemaType), schema);
			}
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x000442AC File Offset: 0x000424AC
		private JsonSchemaType? GetCurrentNodeSchemaType()
		{
			switch (this._reader.TokenType)
			{
			case JsonToken.StartObject:
				return new JsonSchemaType?(JsonSchemaType.Object);
			case JsonToken.StartArray:
				return new JsonSchemaType?(JsonSchemaType.Array);
			case JsonToken.Integer:
				return new JsonSchemaType?(JsonSchemaType.Integer);
			case JsonToken.Float:
				return new JsonSchemaType?(JsonSchemaType.Float);
			case JsonToken.String:
				return new JsonSchemaType?(JsonSchemaType.String);
			case JsonToken.Boolean:
				return new JsonSchemaType?(JsonSchemaType.Boolean);
			case JsonToken.Null:
				return new JsonSchemaType?(JsonSchemaType.Null);
			}
			return null;
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x00044340 File Offset: 0x00042540
		public override int? ReadAsInt32()
		{
			int? num = this._reader.ReadAsInt32();
			this.ValidateCurrentToken();
			return num;
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x00044354 File Offset: 0x00042554
		public override byte[] ReadAsBytes()
		{
			byte[] array = this._reader.ReadAsBytes();
			this.ValidateCurrentToken();
			return array;
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x00044368 File Offset: 0x00042568
		public override decimal? ReadAsDecimal()
		{
			decimal? num = this._reader.ReadAsDecimal();
			this.ValidateCurrentToken();
			return num;
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x0004437C File Offset: 0x0004257C
		public override double? ReadAsDouble()
		{
			double? num = this._reader.ReadAsDouble();
			this.ValidateCurrentToken();
			return num;
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x00044390 File Offset: 0x00042590
		public override bool? ReadAsBoolean()
		{
			bool? flag = this._reader.ReadAsBoolean();
			this.ValidateCurrentToken();
			return flag;
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x000443A4 File Offset: 0x000425A4
		public override string ReadAsString()
		{
			string text = this._reader.ReadAsString();
			this.ValidateCurrentToken();
			return text;
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x000443B8 File Offset: 0x000425B8
		public override DateTime? ReadAsDateTime()
		{
			DateTime? dateTime = this._reader.ReadAsDateTime();
			this.ValidateCurrentToken();
			return dateTime;
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x000443CC File Offset: 0x000425CC
		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset? dateTimeOffset = this._reader.ReadAsDateTimeOffset();
			this.ValidateCurrentToken();
			return dateTimeOffset;
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x000443E0 File Offset: 0x000425E0
		public override bool Read()
		{
			if (!this._reader.Read())
			{
				return false;
			}
			if (this._reader.TokenType == JsonToken.Comment)
			{
				return true;
			}
			this.ValidateCurrentToken();
			return true;
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x00044410 File Offset: 0x00042610
		private void ValidateCurrentToken()
		{
			if (this._model == null)
			{
				JsonSchemaModelBuilder jsonSchemaModelBuilder = new JsonSchemaModelBuilder();
				this._model = jsonSchemaModelBuilder.Build(this._schema);
				if (!JsonTokenUtils.IsStartToken(this._reader.TokenType))
				{
					this.Push(new JsonValidatingReader.SchemaScope(JTokenType.None, this.CurrentMemberSchemas));
				}
			}
			switch (this._reader.TokenType)
			{
			case JsonToken.None:
				return;
			case JsonToken.StartObject:
			{
				this.ProcessValue();
				IList<JsonSchemaModel> list = this.CurrentMemberSchemas.Where(new Func<JsonSchemaModel, bool>(this.ValidateObject)).ToList<JsonSchemaModel>();
				this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Object, list));
				this.WriteToken(this.CurrentSchemas);
				return;
			}
			case JsonToken.StartArray:
			{
				this.ProcessValue();
				IList<JsonSchemaModel> list2 = this.CurrentMemberSchemas.Where(new Func<JsonSchemaModel, bool>(this.ValidateArray)).ToList<JsonSchemaModel>();
				this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Array, list2));
				this.WriteToken(this.CurrentSchemas);
				return;
			}
			case JsonToken.StartConstructor:
				this.ProcessValue();
				this.Push(new JsonValidatingReader.SchemaScope(JTokenType.Constructor, null));
				this.WriteToken(this.CurrentSchemas);
				return;
			case JsonToken.PropertyName:
			{
				this.WriteToken(this.CurrentSchemas);
				using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentSchemas.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JsonSchemaModel jsonSchemaModel = enumerator.Current;
						this.ValidatePropertyName(jsonSchemaModel);
					}
					return;
				}
				break;
			}
			case JsonToken.Comment:
				goto IL_03F9;
			case JsonToken.Raw:
				break;
			case JsonToken.Integer:
			{
				this.ProcessValue();
				this.WriteToken(this.CurrentMemberSchemas);
				using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JsonSchemaModel jsonSchemaModel2 = enumerator.Current;
						this.ValidateInteger(jsonSchemaModel2);
					}
					return;
				}
				goto IL_01E8;
			}
			case JsonToken.Float:
				goto IL_01E8;
			case JsonToken.String:
				goto IL_023A;
			case JsonToken.Boolean:
				goto IL_028C;
			case JsonToken.Null:
				goto IL_02DE;
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				this.WriteToken(this.CurrentMemberSchemas);
				return;
			case JsonToken.EndObject:
				goto IL_0330;
			case JsonToken.EndArray:
				this.WriteToken(this.CurrentSchemas);
				foreach (JsonSchemaModel jsonSchemaModel3 in this.CurrentSchemas)
				{
					this.ValidateEndArray(jsonSchemaModel3);
				}
				this.Pop();
				return;
			case JsonToken.EndConstructor:
				this.WriteToken(this.CurrentSchemas);
				this.Pop();
				return;
			default:
				goto IL_03F9;
			}
			this.ProcessValue();
			return;
			IL_01E8:
			this.ProcessValue();
			this.WriteToken(this.CurrentMemberSchemas);
			using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JsonSchemaModel jsonSchemaModel4 = enumerator.Current;
					this.ValidateFloat(jsonSchemaModel4);
				}
				return;
			}
			IL_023A:
			this.ProcessValue();
			this.WriteToken(this.CurrentMemberSchemas);
			using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JsonSchemaModel jsonSchemaModel5 = enumerator.Current;
					this.ValidateString(jsonSchemaModel5);
				}
				return;
			}
			IL_028C:
			this.ProcessValue();
			this.WriteToken(this.CurrentMemberSchemas);
			using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JsonSchemaModel jsonSchemaModel6 = enumerator.Current;
					this.ValidateBoolean(jsonSchemaModel6);
				}
				return;
			}
			IL_02DE:
			this.ProcessValue();
			this.WriteToken(this.CurrentMemberSchemas);
			using (IEnumerator<JsonSchemaModel> enumerator = this.CurrentMemberSchemas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JsonSchemaModel jsonSchemaModel7 = enumerator.Current;
					this.ValidateNull(jsonSchemaModel7);
				}
				return;
			}
			IL_0330:
			this.WriteToken(this.CurrentSchemas);
			foreach (JsonSchemaModel jsonSchemaModel8 in this.CurrentSchemas)
			{
				this.ValidateEndObject(jsonSchemaModel8);
			}
			this.Pop();
			return;
			IL_03F9:
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x00044880 File Offset: 0x00042A80
		private void WriteToken(IList<JsonSchemaModel> schemas)
		{
			foreach (JsonValidatingReader.SchemaScope schemaScope in this._stack)
			{
				bool flag = schemaScope.TokenType == JTokenType.Array && schemaScope.IsUniqueArray && schemaScope.ArrayItemCount > 0;
				if (!flag)
				{
					if (!schemas.Any((JsonSchemaModel s) => s.Enum != null))
					{
						continue;
					}
				}
				if (schemaScope.CurrentItemWriter == null)
				{
					if (JsonTokenUtils.IsEndToken(this._reader.TokenType))
					{
						continue;
					}
					schemaScope.CurrentItemWriter = new JTokenWriter();
				}
				schemaScope.CurrentItemWriter.WriteToken(this._reader, false);
				if (schemaScope.CurrentItemWriter.Top == 0 && this._reader.TokenType != JsonToken.PropertyName)
				{
					JToken token = schemaScope.CurrentItemWriter.Token;
					schemaScope.CurrentItemWriter = null;
					if (flag)
					{
						if (schemaScope.UniqueArrayItems.Contains(token, JToken.EqualityComparer))
						{
							this.RaiseError("Non-unique array item at index {0}.".FormatWith(CultureInfo.InvariantCulture, schemaScope.ArrayItemCount - 1), schemaScope.Schemas.First((JsonSchemaModel s) => s.UniqueItems));
						}
						schemaScope.UniqueArrayItems.Add(token);
					}
					else if (schemas.Any((JsonSchemaModel s) => s.Enum != null))
					{
						foreach (JsonSchemaModel jsonSchemaModel in schemas)
						{
							if (jsonSchemaModel.Enum != null && !jsonSchemaModel.Enum.ContainsValue(token, JToken.EqualityComparer))
							{
								StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
								token.WriteTo(new JsonTextWriter(stringWriter), new JsonConverter[0]);
								this.RaiseError("Value {0} is not defined in enum.".FormatWith(CultureInfo.InvariantCulture, stringWriter.ToString()), jsonSchemaModel);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00044B00 File Offset: 0x00042D00
		private void ValidateEndObject(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			Dictionary<string, bool> requiredProperties = this._currentScope.RequiredProperties;
			if (requiredProperties != null)
			{
				if (requiredProperties.Values.Any((bool v) => !v))
				{
					IEnumerable<string> enumerable = from kv in requiredProperties
						where !kv.Value
						select kv.Key;
					this.RaiseError("Required properties are missing from object: {0}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", enumerable)), schema);
				}
			}
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00044BD0 File Offset: 0x00042DD0
		private void ValidateEndArray(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			int arrayItemCount = this._currentScope.ArrayItemCount;
			if (schema.MaximumItems != null && arrayItemCount > schema.MaximumItems)
			{
				this.RaiseError("Array item count {0} exceeds maximum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MaximumItems), schema);
			}
			if (schema.MinimumItems != null && arrayItemCount < schema.MinimumItems)
			{
				this.RaiseError("Array item count {0} is less than minimum count of {1}.".FormatWith(CultureInfo.InvariantCulture, arrayItemCount, schema.MinimumItems), schema);
			}
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x00044CB4 File Offset: 0x00042EB4
		private void ValidateNull(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Null))
			{
				return;
			}
			this.ValidateNotDisallowed(schema);
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x00044CD4 File Offset: 0x00042ED4
		private void ValidateBoolean(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Boolean))
			{
				return;
			}
			this.ValidateNotDisallowed(schema);
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x00044CF4 File Offset: 0x00042EF4
		private void ValidateString(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.String))
			{
				return;
			}
			this.ValidateNotDisallowed(schema);
			string text = this._reader.Value.ToString();
			if (schema.MaximumLength != null && text.Length > schema.MaximumLength)
			{
				this.RaiseError("String '{0}' exceeds maximum length of {1}.".FormatWith(CultureInfo.InvariantCulture, text, schema.MaximumLength), schema);
			}
			if (schema.MinimumLength != null && text.Length < schema.MinimumLength)
			{
				this.RaiseError("String '{0}' is less than minimum length of {1}.".FormatWith(CultureInfo.InvariantCulture, text, schema.MinimumLength), schema);
			}
			if (schema.Patterns != null)
			{
				foreach (string text2 in schema.Patterns)
				{
					if (!Regex.IsMatch(text, text2))
					{
						this.RaiseError("String '{0}' does not match regex pattern '{1}'.".FormatWith(CultureInfo.InvariantCulture, text, text2), schema);
					}
				}
			}
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x00044E60 File Offset: 0x00043060
		private void ValidateInteger(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Integer))
			{
				return;
			}
			this.ValidateNotDisallowed(schema);
			object value = this._reader.Value;
			if (schema.Maximum != null)
			{
				if (JValue.Compare(JTokenType.Integer, value, schema.Maximum) > 0)
				{
					this.RaiseError("Integer {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), schema);
				}
				if (schema.ExclusiveMaximum && JValue.Compare(JTokenType.Integer, value, schema.Maximum) == 0)
				{
					this.RaiseError("Integer {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Maximum), schema);
				}
			}
			if (schema.Minimum != null)
			{
				if (JValue.Compare(JTokenType.Integer, value, schema.Minimum) < 0)
				{
					this.RaiseError("Integer {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), schema);
				}
				if (schema.ExclusiveMinimum && JValue.Compare(JTokenType.Integer, value, schema.Minimum) == 0)
				{
					this.RaiseError("Integer {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, value, schema.Minimum), schema);
				}
			}
			if (schema.DivisibleBy != null)
			{
				bool flag;
				if (value is BigInteger)
				{
					BigInteger bigInteger = (BigInteger)value;
					if (!Math.Abs(schema.DivisibleBy.Value - Math.Truncate(schema.DivisibleBy.Value)).Equals(0.0))
					{
						flag = bigInteger != 0L;
					}
					else
					{
						flag = bigInteger % new BigInteger(schema.DivisibleBy.Value) != 0L;
					}
				}
				else
				{
					flag = !JsonValidatingReader.IsZero((double)Convert.ToInt64(value, CultureInfo.InvariantCulture) % schema.DivisibleBy.GetValueOrDefault());
				}
				if (flag)
				{
					this.RaiseError("Integer {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(value), schema.DivisibleBy), schema);
				}
			}
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x000450AC File Offset: 0x000432AC
		private void ProcessValue()
		{
			if (this._currentScope != null && this._currentScope.TokenType == JTokenType.Array)
			{
				JsonValidatingReader.SchemaScope currentScope = this._currentScope;
				int arrayItemCount = currentScope.ArrayItemCount;
				currentScope.ArrayItemCount = arrayItemCount + 1;
				foreach (JsonSchemaModel jsonSchemaModel in this.CurrentSchemas)
				{
					if (jsonSchemaModel != null && jsonSchemaModel.PositionalItemsValidation && !jsonSchemaModel.AllowAdditionalItems && (jsonSchemaModel.Items == null || this._currentScope.ArrayItemCount - 1 >= jsonSchemaModel.Items.Count))
					{
						this.RaiseError("Index {0} has not been defined and the schema does not allow additional items.".FormatWith(CultureInfo.InvariantCulture, this._currentScope.ArrayItemCount), jsonSchemaModel);
					}
				}
			}
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x00045198 File Offset: 0x00043398
		private void ValidateFloat(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			if (!this.TestType(schema, JsonSchemaType.Float))
			{
				return;
			}
			this.ValidateNotDisallowed(schema);
			double num = Convert.ToDouble(this._reader.Value, CultureInfo.InvariantCulture);
			if (schema.Maximum != null)
			{
				if (num > schema.Maximum)
				{
					this.RaiseError("Float {0} exceeds maximum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Maximum), schema);
				}
				if (schema.ExclusiveMaximum && num == schema.Maximum)
				{
					this.RaiseError("Float {0} equals maximum value of {1} and exclusive maximum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Maximum), schema);
				}
			}
			if (schema.Minimum != null)
			{
				if (num < schema.Minimum)
				{
					this.RaiseError("Float {0} is less than minimum value of {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Minimum), schema);
				}
				if (schema.ExclusiveMinimum && num == schema.Minimum)
				{
					this.RaiseError("Float {0} equals minimum value of {1} and exclusive minimum is true.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.Minimum), schema);
				}
			}
			if (schema.DivisibleBy != null && !JsonValidatingReader.IsZero(JsonValidatingReader.FloatingPointRemainder(num, schema.DivisibleBy.GetValueOrDefault())))
			{
				this.RaiseError("Float {0} is not evenly divisible by {1}.".FormatWith(CultureInfo.InvariantCulture, JsonConvert.ToString(num), schema.DivisibleBy), schema);
			}
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x000453A4 File Offset: 0x000435A4
		private static double FloatingPointRemainder(double dividend, double divisor)
		{
			return dividend - Math.Floor(dividend / divisor) * divisor;
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x000453B4 File Offset: 0x000435B4
		private static bool IsZero(double value)
		{
			return Math.Abs(value) < 4.440892098500626E-15;
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x000453C8 File Offset: 0x000435C8
		private void ValidatePropertyName(JsonSchemaModel schema)
		{
			if (schema == null)
			{
				return;
			}
			string text = Convert.ToString(this._reader.Value, CultureInfo.InvariantCulture);
			if (this._currentScope.RequiredProperties.ContainsKey(text))
			{
				this._currentScope.RequiredProperties[text] = true;
			}
			if (!schema.AllowAdditionalProperties && !this.IsPropertyDefinied(schema, text))
			{
				this.RaiseError("Property '{0}' has not been defined and the schema does not allow additional properties.".FormatWith(CultureInfo.InvariantCulture, text), schema);
			}
			this._currentScope.CurrentPropertyName = text;
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0004545C File Offset: 0x0004365C
		private bool IsPropertyDefinied(JsonSchemaModel schema, string propertyName)
		{
			if (schema.Properties != null && schema.Properties.ContainsKey(propertyName))
			{
				return true;
			}
			if (schema.PatternProperties != null)
			{
				foreach (string text in schema.PatternProperties.Keys)
				{
					if (Regex.IsMatch(propertyName, text))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x000454F0 File Offset: 0x000436F0
		private bool ValidateArray(JsonSchemaModel schema)
		{
			return schema == null || this.TestType(schema, JsonSchemaType.Array);
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x00045504 File Offset: 0x00043704
		private bool ValidateObject(JsonSchemaModel schema)
		{
			return schema == null || this.TestType(schema, JsonSchemaType.Object);
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x00045518 File Offset: 0x00043718
		private bool TestType(JsonSchemaModel currentSchema, JsonSchemaType currentType)
		{
			if (!JsonSchemaGenerator.HasFlag(new JsonSchemaType?(currentSchema.Type), currentType))
			{
				this.RaiseError("Invalid type. Expected {0} but got {1}.".FormatWith(CultureInfo.InvariantCulture, currentSchema.Type, currentType), currentSchema);
				return false;
			}
			return true;
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x0004556C File Offset: 0x0004376C
		bool IJsonLineInfo.HasLineInfo()
		{
			IJsonLineInfo jsonLineInfo = this._reader as IJsonLineInfo;
			return jsonLineInfo != null && jsonLineInfo.HasLineInfo();
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000A8B RID: 2699 RVA: 0x00045598 File Offset: 0x00043798
		int IJsonLineInfo.LineNumber
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._reader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LineNumber;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000A8C RID: 2700 RVA: 0x000455C4 File Offset: 0x000437C4
		int IJsonLineInfo.LinePosition
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._reader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LinePosition;
			}
		}

		// Token: 0x0400042B RID: 1067
		private readonly JsonReader _reader;

		// Token: 0x0400042C RID: 1068
		private readonly Stack<JsonValidatingReader.SchemaScope> _stack;

		// Token: 0x0400042D RID: 1069
		private JsonSchema _schema;

		// Token: 0x0400042E RID: 1070
		private JsonSchemaModel _model;

		// Token: 0x0400042F RID: 1071
		private JsonValidatingReader.SchemaScope _currentScope;

		// Token: 0x04000431 RID: 1073
		private static readonly IList<JsonSchemaModel> EmptySchemaList = new List<JsonSchemaModel>();

		// Token: 0x0200022B RID: 555
		private class SchemaScope
		{
			// Token: 0x17000406 RID: 1030
			// (get) Token: 0x06001663 RID: 5731 RVA: 0x00081070 File Offset: 0x0007F270
			// (set) Token: 0x06001664 RID: 5732 RVA: 0x00081078 File Offset: 0x0007F278
			public string CurrentPropertyName { get; set; }

			// Token: 0x17000407 RID: 1031
			// (get) Token: 0x06001665 RID: 5733 RVA: 0x00081084 File Offset: 0x0007F284
			// (set) Token: 0x06001666 RID: 5734 RVA: 0x0008108C File Offset: 0x0007F28C
			public int ArrayItemCount { get; set; }

			// Token: 0x17000408 RID: 1032
			// (get) Token: 0x06001667 RID: 5735 RVA: 0x00081098 File Offset: 0x0007F298
			public bool IsUniqueArray { get; }

			// Token: 0x17000409 RID: 1033
			// (get) Token: 0x06001668 RID: 5736 RVA: 0x000810A0 File Offset: 0x0007F2A0
			public IList<JToken> UniqueArrayItems { get; }

			// Token: 0x1700040A RID: 1034
			// (get) Token: 0x06001669 RID: 5737 RVA: 0x000810A8 File Offset: 0x0007F2A8
			// (set) Token: 0x0600166A RID: 5738 RVA: 0x000810B0 File Offset: 0x0007F2B0
			public JTokenWriter CurrentItemWriter { get; set; }

			// Token: 0x1700040B RID: 1035
			// (get) Token: 0x0600166B RID: 5739 RVA: 0x000810BC File Offset: 0x0007F2BC
			public IList<JsonSchemaModel> Schemas
			{
				get
				{
					return this._schemas;
				}
			}

			// Token: 0x1700040C RID: 1036
			// (get) Token: 0x0600166C RID: 5740 RVA: 0x000810C4 File Offset: 0x0007F2C4
			public Dictionary<string, bool> RequiredProperties
			{
				get
				{
					return this._requiredProperties;
				}
			}

			// Token: 0x1700040D RID: 1037
			// (get) Token: 0x0600166D RID: 5741 RVA: 0x000810CC File Offset: 0x0007F2CC
			public JTokenType TokenType
			{
				get
				{
					return this._tokenType;
				}
			}

			// Token: 0x0600166E RID: 5742 RVA: 0x000810D4 File Offset: 0x0007F2D4
			public SchemaScope(JTokenType tokenType, IList<JsonSchemaModel> schemas)
			{
				this._tokenType = tokenType;
				this._schemas = schemas;
				this._requiredProperties = schemas.SelectMany(new Func<JsonSchemaModel, IEnumerable<string>>(this.GetRequiredProperties)).Distinct<string>().ToDictionary((string p) => p, (string p) => false);
				if (tokenType == JTokenType.Array)
				{
					if (schemas.Any((JsonSchemaModel s) => s.UniqueItems))
					{
						this.IsUniqueArray = true;
						this.UniqueArrayItems = new List<JToken>();
					}
				}
			}

			// Token: 0x0600166F RID: 5743 RVA: 0x000811A8 File Offset: 0x0007F3A8
			private IEnumerable<string> GetRequiredProperties(JsonSchemaModel schema)
			{
				if (((schema != null) ? schema.Properties : null) == null)
				{
					return Enumerable.Empty<string>();
				}
				return from p in schema.Properties
					where p.Value.Required
					select p.Key;
			}

			// Token: 0x04000A49 RID: 2633
			private readonly JTokenType _tokenType;

			// Token: 0x04000A4A RID: 2634
			private readonly IList<JsonSchemaModel> _schemas;

			// Token: 0x04000A4B RID: 2635
			private readonly Dictionary<string, bool> _requiredProperties;
		}
	}
}
