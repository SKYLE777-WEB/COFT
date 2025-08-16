using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq.JsonPath;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x0200014D RID: 333
	public abstract class JToken : IJEnumerable<JToken>, IEnumerable<JToken>, IEnumerable, IJsonLineInfo, ICloneable, IDynamicMetaObjectProvider
	{
		// Token: 0x060011DC RID: 4572 RVA: 0x00065B88 File Offset: 0x00063D88
		public virtual Task WriteToAsync(JsonWriter writer, CancellationToken cancellationToken, params JsonConverter[] converters)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x00065B90 File Offset: 0x00063D90
		public Task WriteToAsync(JsonWriter writer, params JsonConverter[] converters)
		{
			return this.WriteToAsync(writer, default(CancellationToken), converters);
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x00065BB4 File Offset: 0x00063DB4
		public static Task<JToken> ReadFromAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return JToken.ReadFromAsync(reader, null, cancellationToken);
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x00065BC0 File Offset: 0x00063DC0
		public static async Task<JToken> ReadFromAsync(JsonReader reader, JsonLoadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None && !(await ((settings != null && settings.CommentHandling == CommentHandling.Ignore) ? reader.ReadAndMoveToContentAsync(cancellationToken) : reader.ReadAsync(cancellationToken)).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
			}
			IJsonLineInfo lineInfo = reader as IJsonLineInfo;
			switch (reader.TokenType)
			{
			case JsonToken.StartObject:
				return await JObject.LoadAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			case JsonToken.StartArray:
				return await JArray.LoadAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			case JsonToken.StartConstructor:
				return await JConstructor.LoadAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			case JsonToken.PropertyName:
				return await JProperty.LoadAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			case JsonToken.Comment:
			{
				JValue jvalue = JValue.CreateComment(reader.Value.ToString());
				jvalue.SetLineInfo(lineInfo, settings);
				return jvalue;
			}
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Date:
			case JsonToken.Bytes:
			{
				JValue jvalue2 = new JValue(reader.Value);
				jvalue2.SetLineInfo(lineInfo, settings);
				return jvalue2;
			}
			case JsonToken.Null:
			{
				JValue jvalue3 = JValue.CreateNull();
				jvalue3.SetLineInfo(lineInfo, settings);
				return jvalue3;
			}
			case JsonToken.Undefined:
			{
				JValue jvalue4 = JValue.CreateUndefined();
				jvalue4.SetLineInfo(lineInfo, settings);
				return jvalue4;
			}
			}
			throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x00065C1C File Offset: 0x00063E1C
		public static Task<JToken> LoadAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return JToken.LoadAsync(reader, null, cancellationToken);
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x00065C28 File Offset: 0x00063E28
		public static Task<JToken> LoadAsync(JsonReader reader, JsonLoadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			return JToken.ReadFromAsync(reader, settings, cancellationToken);
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x060011E2 RID: 4578 RVA: 0x00065C34 File Offset: 0x00063E34
		public static JTokenEqualityComparer EqualityComparer
		{
			get
			{
				if (JToken._equalityComparer == null)
				{
					JToken._equalityComparer = new JTokenEqualityComparer();
				}
				return JToken._equalityComparer;
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x060011E3 RID: 4579 RVA: 0x00065C50 File Offset: 0x00063E50
		// (set) Token: 0x060011E4 RID: 4580 RVA: 0x00065C58 File Offset: 0x00063E58
		public JContainer Parent
		{
			[DebuggerStepThrough]
			get
			{
				return this._parent;
			}
			internal set
			{
				this._parent = value;
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x060011E5 RID: 4581 RVA: 0x00065C64 File Offset: 0x00063E64
		public JToken Root
		{
			get
			{
				JContainer jcontainer = this.Parent;
				if (jcontainer == null)
				{
					return this;
				}
				while (jcontainer.Parent != null)
				{
					jcontainer = jcontainer.Parent;
				}
				return jcontainer;
			}
		}

		// Token: 0x060011E6 RID: 4582
		internal abstract JToken CloneToken();

		// Token: 0x060011E7 RID: 4583
		internal abstract bool DeepEquals(JToken node);

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x060011E8 RID: 4584
		public abstract JTokenType Type { get; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x060011E9 RID: 4585
		public abstract bool HasValues { get; }

		// Token: 0x060011EA RID: 4586 RVA: 0x00065C94 File Offset: 0x00063E94
		public static bool DeepEquals(JToken t1, JToken t2)
		{
			return t1 == t2 || (t1 != null && t2 != null && t1.DeepEquals(t2));
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x060011EB RID: 4587 RVA: 0x00065CB4 File Offset: 0x00063EB4
		// (set) Token: 0x060011EC RID: 4588 RVA: 0x00065CBC File Offset: 0x00063EBC
		public JToken Next
		{
			get
			{
				return this._next;
			}
			internal set
			{
				this._next = value;
			}
		}

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x060011ED RID: 4589 RVA: 0x00065CC8 File Offset: 0x00063EC8
		// (set) Token: 0x060011EE RID: 4590 RVA: 0x00065CD0 File Offset: 0x00063ED0
		public JToken Previous
		{
			get
			{
				return this._previous;
			}
			internal set
			{
				this._previous = value;
			}
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x060011EF RID: 4591 RVA: 0x00065CDC File Offset: 0x00063EDC
		public string Path
		{
			get
			{
				if (this.Parent == null)
				{
					return string.Empty;
				}
				List<JsonPosition> list = new List<JsonPosition>();
				JToken jtoken = null;
				for (JToken jtoken2 = this; jtoken2 != null; jtoken2 = jtoken2.Parent)
				{
					JTokenType type = jtoken2.Type;
					if (type - JTokenType.Array > 1)
					{
						if (type == JTokenType.Property)
						{
							JProperty jproperty = (JProperty)jtoken2;
							List<JsonPosition> list2 = list;
							JsonPosition jsonPosition = new JsonPosition(JsonContainerType.Object)
							{
								PropertyName = jproperty.Name
							};
							list2.Add(jsonPosition);
						}
					}
					else if (jtoken != null)
					{
						int num = ((IList<JToken>)jtoken2).IndexOf(jtoken);
						List<JsonPosition> list3 = list;
						JsonPosition jsonPosition = new JsonPosition(JsonContainerType.Array)
						{
							Position = num
						};
						list3.Add(jsonPosition);
					}
					jtoken = jtoken2;
				}
				list.Reverse();
				return JsonPosition.BuildPath(list, null);
			}
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x00065DA0 File Offset: 0x00063FA0
		internal JToken()
		{
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x00065DA8 File Offset: 0x00063FA8
		public void AddAfterSelf(object content)
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			int num = this._parent.IndexOfItem(this);
			this._parent.AddInternal(num + 1, content, false);
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x00065DEC File Offset: 0x00063FEC
		public void AddBeforeSelf(object content)
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			int num = this._parent.IndexOfItem(this);
			this._parent.AddInternal(num, content, false);
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x00065E30 File Offset: 0x00064030
		public IEnumerable<JToken> Ancestors()
		{
			return this.GetAncestors(false);
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x00065E3C File Offset: 0x0006403C
		public IEnumerable<JToken> AncestorsAndSelf()
		{
			return this.GetAncestors(true);
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x00065E48 File Offset: 0x00064048
		internal IEnumerable<JToken> GetAncestors(bool self)
		{
			JToken current;
			for (current = (self ? this : this.Parent); current != null; current = current.Parent)
			{
				yield return current;
			}
			current = null;
			yield break;
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x00065E60 File Offset: 0x00064060
		public IEnumerable<JToken> AfterSelf()
		{
			if (this.Parent == null)
			{
				yield break;
			}
			JToken o;
			for (o = this.Next; o != null; o = o.Next)
			{
				yield return o;
			}
			o = null;
			yield break;
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x00065E70 File Offset: 0x00064070
		public IEnumerable<JToken> BeforeSelf()
		{
			JToken o;
			for (o = this.Parent.First; o != this; o = o.Next)
			{
				yield return o;
			}
			o = null;
			yield break;
		}

		// Token: 0x17000345 RID: 837
		public virtual JToken this[object key]
		{
			get
			{
				throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
			}
			set
			{
				throw new InvalidOperationException("Cannot set child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
			}
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x00065EB8 File Offset: 0x000640B8
		public virtual T Value<T>(object key)
		{
			JToken jtoken = this[key];
			if (jtoken != null)
			{
				return jtoken.Convert<JToken, T>();
			}
			return default(T);
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x060011FB RID: 4603 RVA: 0x00065EE8 File Offset: 0x000640E8
		public virtual JToken First
		{
			get
			{
				throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x060011FC RID: 4604 RVA: 0x00065F04 File Offset: 0x00064104
		public virtual JToken Last
		{
			get
			{
				throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
			}
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x00065F20 File Offset: 0x00064120
		public virtual JEnumerable<JToken> Children()
		{
			return JEnumerable<JToken>.Empty;
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x00065F28 File Offset: 0x00064128
		public JEnumerable<T> Children<T>() where T : JToken
		{
			return new JEnumerable<T>(this.Children().OfType<T>());
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x00065F40 File Offset: 0x00064140
		public virtual IEnumerable<T> Values<T>()
		{
			throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x00065F5C File Offset: 0x0006415C
		public void Remove()
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			this._parent.RemoveItem(this);
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x00065F84 File Offset: 0x00064184
		public void Replace(JToken value)
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			this._parent.ReplaceItem(this, value);
		}

		// Token: 0x06001202 RID: 4610
		public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

		// Token: 0x06001203 RID: 4611 RVA: 0x00065FAC File Offset: 0x000641AC
		public override string ToString()
		{
			return this.ToString(Formatting.Indented, new JsonConverter[0]);
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x00065FBC File Offset: 0x000641BC
		public string ToString(Formatting formatting, params JsonConverter[] converters)
		{
			string text;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				this.WriteTo(new JsonTextWriter(stringWriter)
				{
					Formatting = formatting
				}, converters);
				text = stringWriter.ToString();
			}
			return text;
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x00066014 File Offset: 0x00064214
		private static JValue EnsureValue(JToken value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value is JProperty)
			{
				value = ((JProperty)value).Value;
			}
			return value as JValue;
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x00066048 File Offset: 0x00064248
		private static string GetType(JToken token)
		{
			ValidationUtils.ArgumentNotNull(token, "token");
			if (token is JProperty)
			{
				token = ((JProperty)token).Value;
			}
			return token.Type.ToString();
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x00066090 File Offset: 0x00064290
		private static bool ValidateToken(JToken o, JTokenType[] validTypes, bool nullable)
		{
			return Array.IndexOf<JTokenType>(validTypes, o.Type) != -1 || (nullable && (o.Type == JTokenType.Null || o.Type == JTokenType.Undefined));
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x000660C8 File Offset: 0x000642C8
		public static explicit operator bool(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.BooleanTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return Convert.ToBoolean((int)((BigInteger)jvalue.Value));
			}
			return Convert.ToBoolean(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x00066148 File Offset: 0x00064348
		public static explicit operator DateTimeOffset(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.DateTimeTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is DateTimeOffset)
			{
				return (DateTimeOffset)jvalue.Value;
			}
			string text = jvalue.Value as string;
			if (text != null)
			{
				return DateTimeOffset.Parse(text, CultureInfo.InvariantCulture);
			}
			return new DateTimeOffset(Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x000661E4 File Offset: 0x000643E4
		public static explicit operator bool?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.BooleanTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new bool?(Convert.ToBoolean((int)((BigInteger)jvalue.Value)));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new bool?(Convert.ToBoolean(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00066294 File Offset: 0x00064494
		public static explicit operator long(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (long)((BigInteger)jvalue.Value);
			}
			return Convert.ToInt64(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x00066310 File Offset: 0x00064510
		public static explicit operator DateTime?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.DateTimeTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is DateTimeOffset)
			{
				return new DateTime?(((DateTimeOffset)jvalue.Value).DateTime);
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new DateTime?(Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x000663C0 File Offset: 0x000645C0
		public static explicit operator DateTimeOffset?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.DateTimeTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			if (jvalue.Value is DateTimeOffset)
			{
				return (DateTimeOffset?)jvalue.Value;
			}
			string text = jvalue.Value as string;
			if (text != null)
			{
				return new DateTimeOffset?(DateTimeOffset.Parse(text, CultureInfo.InvariantCulture));
			}
			return new DateTimeOffset?(new DateTimeOffset(Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture)));
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x00066488 File Offset: 0x00064688
		public static explicit operator decimal?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new decimal?((decimal)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new decimal?(Convert.ToDecimal(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00066534 File Offset: 0x00064734
		public static explicit operator double?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new double?((double)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new double?(Convert.ToDouble(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x000665E0 File Offset: 0x000647E0
		public static explicit operator char?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.CharTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new char?((char)(ushort)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new char?(Convert.ToChar(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x0006668C File Offset: 0x0006488C
		public static explicit operator int(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (int)((BigInteger)jvalue.Value);
			}
			return Convert.ToInt32(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x00066708 File Offset: 0x00064908
		public static explicit operator short(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (short)((BigInteger)jvalue.Value);
			}
			return Convert.ToInt16(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x00066784 File Offset: 0x00064984
		[CLSCompliant(false)]
		public static explicit operator ushort(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (ushort)((BigInteger)jvalue.Value);
			}
			return Convert.ToUInt16(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x00066800 File Offset: 0x00064A00
		[CLSCompliant(false)]
		public static explicit operator char(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.CharTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (char)(ushort)((BigInteger)jvalue.Value);
			}
			return Convert.ToChar(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x0006687C File Offset: 0x00064A7C
		public static explicit operator byte(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (byte)((BigInteger)jvalue.Value);
			}
			return Convert.ToByte(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x000668F8 File Offset: 0x00064AF8
		[CLSCompliant(false)]
		public static explicit operator sbyte(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (sbyte)((BigInteger)jvalue.Value);
			}
			return Convert.ToSByte(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x00066974 File Offset: 0x00064B74
		public static explicit operator int?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new int?((int)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new int?(Convert.ToInt32(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00066A20 File Offset: 0x00064C20
		public static explicit operator short?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new short?((short)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new short?(Convert.ToInt16(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x00066ACC File Offset: 0x00064CCC
		[CLSCompliant(false)]
		public static explicit operator ushort?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new ushort?((ushort)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new ushort?(Convert.ToUInt16(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x00066B78 File Offset: 0x00064D78
		public static explicit operator byte?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new byte?((byte)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new byte?(Convert.ToByte(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x00066C24 File Offset: 0x00064E24
		[CLSCompliant(false)]
		public static explicit operator sbyte?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new sbyte?((sbyte)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new sbyte?(Convert.ToSByte(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x00066CD0 File Offset: 0x00064ED0
		public static explicit operator DateTime(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.DateTimeTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is DateTimeOffset)
			{
				return ((DateTimeOffset)jvalue.Value).DateTime;
			}
			return Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00066D50 File Offset: 0x00064F50
		public static explicit operator long?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new long?((long)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new long?(Convert.ToInt64(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x00066DFC File Offset: 0x00064FFC
		public static explicit operator float?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new float?((float)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new float?(Convert.ToSingle(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x00066EA8 File Offset: 0x000650A8
		public static explicit operator decimal(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (decimal)((BigInteger)jvalue.Value);
			}
			return Convert.ToDecimal(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x00066F24 File Offset: 0x00065124
		[CLSCompliant(false)]
		public static explicit operator uint?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new uint?((uint)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new uint?(Convert.ToUInt32(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x00066FD0 File Offset: 0x000651D0
		[CLSCompliant(false)]
		public static explicit operator ulong?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return new ulong?((ulong)((BigInteger)jvalue.Value));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new ulong?(Convert.ToUInt64(jvalue.Value, CultureInfo.InvariantCulture));
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0006707C File Offset: 0x0006527C
		public static explicit operator double(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (double)((BigInteger)jvalue.Value);
			}
			return Convert.ToDouble(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x000670F8 File Offset: 0x000652F8
		public static explicit operator float(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (float)((BigInteger)jvalue.Value);
			}
			return Convert.ToSingle(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x00067174 File Offset: 0x00065374
		public static explicit operator string(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.StringTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to String.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			byte[] array = jvalue.Value as byte[];
			if (array != null)
			{
				return Convert.ToBase64String(array);
			}
			if (jvalue.Value is BigInteger)
			{
				return ((BigInteger)jvalue.Value).ToString(CultureInfo.InvariantCulture);
			}
			return Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x00067228 File Offset: 0x00065428
		[CLSCompliant(false)]
		public static explicit operator uint(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (uint)((BigInteger)jvalue.Value);
			}
			return Convert.ToUInt32(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x000672A4 File Offset: 0x000654A4
		[CLSCompliant(false)]
		public static explicit operator ulong(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is BigInteger)
			{
				return (ulong)((BigInteger)jvalue.Value);
			}
			return Convert.ToUInt64(jvalue.Value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x00067320 File Offset: 0x00065520
		public static explicit operator byte[](JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.BytesTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value is string)
			{
				return Convert.FromBase64String(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
			}
			if (jvalue.Value is BigInteger)
			{
				return ((BigInteger)jvalue.Value).ToByteArray();
			}
			byte[] array = jvalue.Value as byte[];
			if (array != null)
			{
				return array;
			}
			throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x000673EC File Offset: 0x000655EC
		public static explicit operator Guid(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.GuidTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			byte[] array = jvalue.Value as byte[];
			if (array != null)
			{
				return new Guid(array);
			}
			if (!(jvalue.Value is Guid))
			{
				return new Guid(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
			}
			return (Guid)jvalue.Value;
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x00067480 File Offset: 0x00065680
		public static explicit operator Guid?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.GuidTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			byte[] array = jvalue.Value as byte[];
			if (array != null)
			{
				return new Guid?(new Guid(array));
			}
			return new Guid?((jvalue.Value is Guid) ? ((Guid)jvalue.Value) : new Guid(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture)));
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x00067548 File Offset: 0x00065748
		public static explicit operator TimeSpan(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.TimeSpanTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (!(jvalue.Value is TimeSpan))
			{
				return ConvertUtils.ParseTimeSpan(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
			}
			return (TimeSpan)jvalue.Value;
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x000675C4 File Offset: 0x000657C4
		public static explicit operator TimeSpan?(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.TimeSpanTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new TimeSpan?((jvalue.Value is TimeSpan) ? ((TimeSpan)jvalue.Value) : ConvertUtils.ParseTimeSpan(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture)));
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x00067670 File Offset: 0x00065870
		public static explicit operator Uri(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.UriTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Uri.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			if (!(jvalue.Value is Uri))
			{
				return new Uri(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
			}
			return (Uri)jvalue.Value;
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x00067700 File Offset: 0x00065900
		private static BigInteger ToBigInteger(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.BigIntegerTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return ConvertUtils.ToBigInteger(jvalue.Value);
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x00067758 File Offset: 0x00065958
		private static BigInteger? ToBigIntegerNullable(JToken value)
		{
			JValue jvalue = JToken.EnsureValue(value);
			if (jvalue == null || !JToken.ValidateToken(jvalue, JToken.BigIntegerTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jvalue.Value == null)
			{
				return null;
			}
			return new BigInteger?(ConvertUtils.ToBigInteger(jvalue.Value));
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x000677C8 File Offset: 0x000659C8
		public static implicit operator JToken(bool value)
		{
			return new JValue(value);
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x000677D0 File Offset: 0x000659D0
		public static implicit operator JToken(DateTimeOffset value)
		{
			return new JValue(value);
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x000677D8 File Offset: 0x000659D8
		public static implicit operator JToken(byte value)
		{
			return new JValue((long)((ulong)value));
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x000677E4 File Offset: 0x000659E4
		public static implicit operator JToken(byte? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x000677F4 File Offset: 0x000659F4
		[CLSCompliant(false)]
		public static implicit operator JToken(sbyte value)
		{
			return new JValue((long)value);
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x00067800 File Offset: 0x00065A00
		[CLSCompliant(false)]
		public static implicit operator JToken(sbyte? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x00067810 File Offset: 0x00065A10
		public static implicit operator JToken(bool? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x00067820 File Offset: 0x00065A20
		public static implicit operator JToken(long value)
		{
			return new JValue(value);
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x00067828 File Offset: 0x00065A28
		public static implicit operator JToken(DateTime? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x00067838 File Offset: 0x00065A38
		public static implicit operator JToken(DateTimeOffset? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x00067848 File Offset: 0x00065A48
		public static implicit operator JToken(decimal? value)
		{
			return new JValue(value);
		}

		// Token: 0x0600123A RID: 4666 RVA: 0x00067858 File Offset: 0x00065A58
		public static implicit operator JToken(double? value)
		{
			return new JValue(value);
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x00067868 File Offset: 0x00065A68
		[CLSCompliant(false)]
		public static implicit operator JToken(short value)
		{
			return new JValue((long)value);
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x00067874 File Offset: 0x00065A74
		[CLSCompliant(false)]
		public static implicit operator JToken(ushort value)
		{
			return new JValue((long)((ulong)value));
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x00067880 File Offset: 0x00065A80
		public static implicit operator JToken(int value)
		{
			return new JValue((long)value);
		}

		// Token: 0x0600123E RID: 4670 RVA: 0x0006788C File Offset: 0x00065A8C
		public static implicit operator JToken(int? value)
		{
			return new JValue(value);
		}

		// Token: 0x0600123F RID: 4671 RVA: 0x0006789C File Offset: 0x00065A9C
		public static implicit operator JToken(DateTime value)
		{
			return new JValue(value);
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x000678A4 File Offset: 0x00065AA4
		public static implicit operator JToken(long? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x000678B4 File Offset: 0x00065AB4
		public static implicit operator JToken(float? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x000678C4 File Offset: 0x00065AC4
		public static implicit operator JToken(decimal value)
		{
			return new JValue(value);
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x000678CC File Offset: 0x00065ACC
		[CLSCompliant(false)]
		public static implicit operator JToken(short? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x000678DC File Offset: 0x00065ADC
		[CLSCompliant(false)]
		public static implicit operator JToken(ushort? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x000678EC File Offset: 0x00065AEC
		[CLSCompliant(false)]
		public static implicit operator JToken(uint? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x000678FC File Offset: 0x00065AFC
		[CLSCompliant(false)]
		public static implicit operator JToken(ulong? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x0006790C File Offset: 0x00065B0C
		public static implicit operator JToken(double value)
		{
			return new JValue(value);
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00067914 File Offset: 0x00065B14
		public static implicit operator JToken(float value)
		{
			return new JValue(value);
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x0006791C File Offset: 0x00065B1C
		public static implicit operator JToken(string value)
		{
			return new JValue(value);
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x00067924 File Offset: 0x00065B24
		[CLSCompliant(false)]
		public static implicit operator JToken(uint value)
		{
			return new JValue((long)((ulong)value));
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x00067930 File Offset: 0x00065B30
		[CLSCompliant(false)]
		public static implicit operator JToken(ulong value)
		{
			return new JValue(value);
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x00067938 File Offset: 0x00065B38
		public static implicit operator JToken(byte[] value)
		{
			return new JValue(value);
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x00067940 File Offset: 0x00065B40
		public static implicit operator JToken(Uri value)
		{
			return new JValue(value);
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x00067948 File Offset: 0x00065B48
		public static implicit operator JToken(TimeSpan value)
		{
			return new JValue(value);
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00067950 File Offset: 0x00065B50
		public static implicit operator JToken(TimeSpan? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x00067960 File Offset: 0x00065B60
		public static implicit operator JToken(Guid value)
		{
			return new JValue(value);
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x00067968 File Offset: 0x00065B68
		public static implicit operator JToken(Guid? value)
		{
			return new JValue(value);
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x00067978 File Offset: 0x00065B78
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<JToken>)this).GetEnumerator();
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00067980 File Offset: 0x00065B80
		IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
		{
			return this.Children().GetEnumerator();
		}

		// Token: 0x06001254 RID: 4692
		internal abstract int GetDeepHashCode();

		// Token: 0x17000348 RID: 840
		IJEnumerable<JToken> IJEnumerable<JToken>.this[object key]
		{
			get
			{
				return this[key];
			}
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x000679AC File Offset: 0x00065BAC
		public JsonReader CreateReader()
		{
			return new JTokenReader(this);
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x000679B4 File Offset: 0x00065BB4
		internal static JToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
			JToken token;
			using (JTokenWriter jtokenWriter = new JTokenWriter())
			{
				jsonSerializer.Serialize(jtokenWriter, o);
				token = jtokenWriter.Token;
			}
			return token;
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x00067A10 File Offset: 0x00065C10
		public static JToken FromObject(object o)
		{
			return JToken.FromObjectInternal(o, JsonSerializer.CreateDefault());
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x00067A20 File Offset: 0x00065C20
		public static JToken FromObject(object o, JsonSerializer jsonSerializer)
		{
			return JToken.FromObjectInternal(o, jsonSerializer);
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x00067A2C File Offset: 0x00065C2C
		public T ToObject<T>()
		{
			return (T)((object)this.ToObject(typeof(T)));
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00067A44 File Offset: 0x00065C44
		public object ToObject(Type objectType)
		{
			if (JsonConvert.DefaultSettings == null)
			{
				bool flag;
				PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(objectType, out flag);
				if (flag)
				{
					if (this.Type == JTokenType.String)
					{
						try
						{
							return this.ToObject(objectType, JsonSerializer.CreateDefault());
						}
						catch (Exception ex)
						{
							Type type = (objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType));
							throw new ArgumentException("Could not convert '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, (string)this, type.Name), ex);
						}
					}
					if (this.Type == JTokenType.Integer)
					{
						return Enum.ToObject(objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType), ((JValue)this).Value);
					}
				}
				switch (typeCode)
				{
				case PrimitiveTypeCode.Char:
					return (char)this;
				case PrimitiveTypeCode.CharNullable:
					return (char?)this;
				case PrimitiveTypeCode.Boolean:
					return (bool)this;
				case PrimitiveTypeCode.BooleanNullable:
					return (bool?)this;
				case PrimitiveTypeCode.SByte:
					return (sbyte)this;
				case PrimitiveTypeCode.SByteNullable:
					return (sbyte?)this;
				case PrimitiveTypeCode.Int16:
					return (short)this;
				case PrimitiveTypeCode.Int16Nullable:
					return (short?)this;
				case PrimitiveTypeCode.UInt16:
					return (ushort)this;
				case PrimitiveTypeCode.UInt16Nullable:
					return (ushort?)this;
				case PrimitiveTypeCode.Int32:
					return (int)this;
				case PrimitiveTypeCode.Int32Nullable:
					return (int?)this;
				case PrimitiveTypeCode.Byte:
					return (byte)this;
				case PrimitiveTypeCode.ByteNullable:
					return (byte?)this;
				case PrimitiveTypeCode.UInt32:
					return (uint)this;
				case PrimitiveTypeCode.UInt32Nullable:
					return (uint?)this;
				case PrimitiveTypeCode.Int64:
					return (long)this;
				case PrimitiveTypeCode.Int64Nullable:
					return (long?)this;
				case PrimitiveTypeCode.UInt64:
					return (ulong)this;
				case PrimitiveTypeCode.UInt64Nullable:
					return (ulong?)this;
				case PrimitiveTypeCode.Single:
					return (float)this;
				case PrimitiveTypeCode.SingleNullable:
					return (float?)this;
				case PrimitiveTypeCode.Double:
					return (double)this;
				case PrimitiveTypeCode.DoubleNullable:
					return (double?)this;
				case PrimitiveTypeCode.DateTime:
					return (DateTime)this;
				case PrimitiveTypeCode.DateTimeNullable:
					return (DateTime?)this;
				case PrimitiveTypeCode.DateTimeOffset:
					return (DateTimeOffset)this;
				case PrimitiveTypeCode.DateTimeOffsetNullable:
					return (DateTimeOffset?)this;
				case PrimitiveTypeCode.Decimal:
					return (decimal)this;
				case PrimitiveTypeCode.DecimalNullable:
					return (decimal?)this;
				case PrimitiveTypeCode.Guid:
					return (Guid)this;
				case PrimitiveTypeCode.GuidNullable:
					return (Guid?)this;
				case PrimitiveTypeCode.TimeSpan:
					return (TimeSpan)this;
				case PrimitiveTypeCode.TimeSpanNullable:
					return (TimeSpan?)this;
				case PrimitiveTypeCode.BigInteger:
					return JToken.ToBigInteger(this);
				case PrimitiveTypeCode.BigIntegerNullable:
					return JToken.ToBigIntegerNullable(this);
				case PrimitiveTypeCode.Uri:
					return (Uri)this;
				case PrimitiveTypeCode.String:
					return (string)this;
				}
			}
			return this.ToObject(objectType, JsonSerializer.CreateDefault());
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00067D7C File Offset: 0x00065F7C
		public T ToObject<T>(JsonSerializer jsonSerializer)
		{
			return (T)((object)this.ToObject(typeof(T), jsonSerializer));
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x00067D94 File Offset: 0x00065F94
		public object ToObject(Type objectType, JsonSerializer jsonSerializer)
		{
			ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
			object obj;
			using (JTokenReader jtokenReader = new JTokenReader(this))
			{
				obj = jsonSerializer.Deserialize(jtokenReader, objectType);
			}
			return obj;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x00067DE0 File Offset: 0x00065FE0
		public static JToken ReadFrom(JsonReader reader)
		{
			return JToken.ReadFrom(reader, null);
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x00067DEC File Offset: 0x00065FEC
		public static JToken ReadFrom(JsonReader reader, JsonLoadSettings settings)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			bool flag;
			if (reader.TokenType == JsonToken.None)
			{
				flag = ((settings != null && settings.CommentHandling == CommentHandling.Ignore) ? reader.ReadAndMoveToContent() : reader.Read());
			}
			else
			{
				flag = reader.TokenType != JsonToken.Comment || settings == null || settings.CommentHandling != CommentHandling.Ignore || reader.ReadAndMoveToContent();
			}
			if (!flag)
			{
				throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
			}
			IJsonLineInfo jsonLineInfo = reader as IJsonLineInfo;
			switch (reader.TokenType)
			{
			case JsonToken.StartObject:
				return JObject.Load(reader, settings);
			case JsonToken.StartArray:
				return JArray.Load(reader, settings);
			case JsonToken.StartConstructor:
				return JConstructor.Load(reader, settings);
			case JsonToken.PropertyName:
				return JProperty.Load(reader, settings);
			case JsonToken.Comment:
			{
				JValue jvalue = JValue.CreateComment(reader.Value.ToString());
				jvalue.SetLineInfo(jsonLineInfo, settings);
				return jvalue;
			}
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Date:
			case JsonToken.Bytes:
			{
				JValue jvalue2 = new JValue(reader.Value);
				jvalue2.SetLineInfo(jsonLineInfo, settings);
				return jvalue2;
			}
			case JsonToken.Null:
			{
				JValue jvalue3 = JValue.CreateNull();
				jvalue3.SetLineInfo(jsonLineInfo, settings);
				return jvalue3;
			}
			case JsonToken.Undefined:
			{
				JValue jvalue4 = JValue.CreateUndefined();
				jvalue4.SetLineInfo(jsonLineInfo, settings);
				return jvalue4;
			}
			}
			throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x00067F60 File Offset: 0x00066160
		public static JToken Parse(string json)
		{
			return JToken.Parse(json, null);
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x00067F6C File Offset: 0x0006616C
		public static JToken Parse(string json, JsonLoadSettings settings)
		{
			JToken jtoken2;
			using (JsonReader jsonReader = new JsonTextReader(new StringReader(json)))
			{
				JToken jtoken = JToken.Load(jsonReader, settings);
				while (jsonReader.Read())
				{
				}
				jtoken2 = jtoken;
			}
			return jtoken2;
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x00067FBC File Offset: 0x000661BC
		public static JToken Load(JsonReader reader, JsonLoadSettings settings)
		{
			return JToken.ReadFrom(reader, settings);
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x00067FC8 File Offset: 0x000661C8
		public static JToken Load(JsonReader reader)
		{
			return JToken.Load(reader, null);
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x00067FD4 File Offset: 0x000661D4
		internal void SetLineInfo(IJsonLineInfo lineInfo, JsonLoadSettings settings)
		{
			if (settings != null && settings.LineInfoHandling != LineInfoHandling.Load)
			{
				return;
			}
			if (lineInfo == null || !lineInfo.HasLineInfo())
			{
				return;
			}
			this.SetLineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x0006801C File Offset: 0x0006621C
		internal void SetLineInfo(int lineNumber, int linePosition)
		{
			this.AddAnnotation(new JToken.LineInfoAnnotation(lineNumber, linePosition));
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x0006802C File Offset: 0x0006622C
		bool IJsonLineInfo.HasLineInfo()
		{
			return this.Annotation<JToken.LineInfoAnnotation>() != null;
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06001267 RID: 4711 RVA: 0x00068038 File Offset: 0x00066238
		int IJsonLineInfo.LineNumber
		{
			get
			{
				JToken.LineInfoAnnotation lineInfoAnnotation = this.Annotation<JToken.LineInfoAnnotation>();
				if (lineInfoAnnotation != null)
				{
					return lineInfoAnnotation.LineNumber;
				}
				return 0;
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06001268 RID: 4712 RVA: 0x00068060 File Offset: 0x00066260
		int IJsonLineInfo.LinePosition
		{
			get
			{
				JToken.LineInfoAnnotation lineInfoAnnotation = this.Annotation<JToken.LineInfoAnnotation>();
				if (lineInfoAnnotation != null)
				{
					return lineInfoAnnotation.LinePosition;
				}
				return 0;
			}
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x00068088 File Offset: 0x00066288
		public JToken SelectToken(string path)
		{
			return this.SelectToken(path, false);
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x00068094 File Offset: 0x00066294
		public JToken SelectToken(string path, bool errorWhenNoMatch)
		{
			JPath jpath = new JPath(path);
			JToken jtoken = null;
			foreach (JToken jtoken2 in jpath.Evaluate(this, this, errorWhenNoMatch))
			{
				if (jtoken != null)
				{
					throw new JsonException("Path returned multiple tokens.");
				}
				jtoken = jtoken2;
			}
			return jtoken;
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x00068100 File Offset: 0x00066300
		public IEnumerable<JToken> SelectTokens(string path)
		{
			return this.SelectTokens(path, false);
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x0006810C File Offset: 0x0006630C
		public IEnumerable<JToken> SelectTokens(string path, bool errorWhenNoMatch)
		{
			return new JPath(path).Evaluate(this, this, errorWhenNoMatch);
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0006811C File Offset: 0x0006631C
		protected virtual DynamicMetaObject GetMetaObject(Expression parameter)
		{
			return new DynamicProxyMetaObject<JToken>(parameter, this, new DynamicProxy<JToken>());
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0006812C File Offset: 0x0006632C
		DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
		{
			return this.GetMetaObject(parameter);
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x00068138 File Offset: 0x00066338
		object ICloneable.Clone()
		{
			return this.DeepClone();
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x00068140 File Offset: 0x00066340
		public JToken DeepClone()
		{
			return this.CloneToken();
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x00068148 File Offset: 0x00066348
		public void AddAnnotation(object annotation)
		{
			if (annotation == null)
			{
				throw new ArgumentNullException("annotation");
			}
			if (this._annotations == null)
			{
				object obj;
				if (!(annotation is object[]))
				{
					obj = annotation;
				}
				else
				{
					(obj = new object[1])[0] = annotation;
				}
				this._annotations = obj;
				return;
			}
			object[] array = this._annotations as object[];
			if (array == null)
			{
				this._annotations = new object[] { this._annotations, annotation };
				return;
			}
			int num = 0;
			while (num < array.Length && array[num] != null)
			{
				num++;
			}
			if (num == array.Length)
			{
				Array.Resize<object>(ref array, num * 2);
				this._annotations = array;
			}
			array[num] = annotation;
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x000681FC File Offset: 0x000663FC
		public T Annotation<T>() where T : class
		{
			if (this._annotations != null)
			{
				object[] array = this._annotations as object[];
				if (array == null)
				{
					return this._annotations as T;
				}
				foreach (object obj in array)
				{
					if (obj == null)
					{
						break;
					}
					T t = obj as T;
					if (t != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0006827C File Offset: 0x0006647C
		public object Annotation(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this._annotations != null)
			{
				object[] array = this._annotations as object[];
				if (array == null)
				{
					if (type.IsInstanceOfType(this._annotations))
					{
						return this._annotations;
					}
				}
				else
				{
					foreach (object obj in array)
					{
						if (obj == null)
						{
							break;
						}
						if (type.IsInstanceOfType(obj))
						{
							return obj;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x00068304 File Offset: 0x00066504
		public IEnumerable<T> Annotations<T>() where T : class
		{
			if (this._annotations == null)
			{
				yield break;
			}
			object[] annotations = this._annotations as object[];
			if (annotations != null)
			{
				int num;
				for (int i = 0; i < annotations.Length; i = num + 1)
				{
					object obj = annotations[i];
					if (obj == null)
					{
						break;
					}
					T t = obj as T;
					if (t != null)
					{
						yield return t;
					}
					num = i;
				}
				yield break;
			}
			T t2 = this._annotations as T;
			if (t2 == null)
			{
				yield break;
			}
			yield return t2;
			yield break;
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x00068314 File Offset: 0x00066514
		public IEnumerable<object> Annotations(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this._annotations == null)
			{
				yield break;
			}
			object[] annotations = this._annotations as object[];
			if (annotations != null)
			{
				int num;
				for (int i = 0; i < annotations.Length; i = num + 1)
				{
					object obj = annotations[i];
					if (obj == null)
					{
						break;
					}
					if (type.IsInstanceOfType(obj))
					{
						yield return obj;
					}
					num = i;
				}
				yield break;
			}
			if (!type.IsInstanceOfType(this._annotations))
			{
				yield break;
			}
			yield return this._annotations;
			yield break;
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0006832C File Offset: 0x0006652C
		public void RemoveAnnotations<T>() where T : class
		{
			if (this._annotations != null)
			{
				object[] array = this._annotations as object[];
				if (array == null)
				{
					if (this._annotations is T)
					{
						this._annotations = null;
						return;
					}
				}
				else
				{
					int i = 0;
					int j = 0;
					while (i < array.Length)
					{
						object obj = array[i];
						if (obj == null)
						{
							break;
						}
						if (!(obj is T))
						{
							array[j++] = obj;
						}
						i++;
					}
					if (j != 0)
					{
						while (j < i)
						{
							array[j++] = null;
						}
						return;
					}
					this._annotations = null;
				}
			}
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x000683C4 File Offset: 0x000665C4
		public void RemoveAnnotations(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this._annotations != null)
			{
				object[] array = this._annotations as object[];
				if (array == null)
				{
					if (type.IsInstanceOfType(this._annotations))
					{
						this._annotations = null;
						return;
					}
				}
				else
				{
					int i = 0;
					int j = 0;
					while (i < array.Length)
					{
						object obj = array[i];
						if (obj == null)
						{
							break;
						}
						if (!type.IsInstanceOfType(obj))
						{
							array[j++] = obj;
						}
						i++;
					}
					if (j != 0)
					{
						while (j < i)
						{
							array[j++] = null;
						}
						return;
					}
					this._annotations = null;
				}
			}
		}

		// Token: 0x0400069C RID: 1692
		private static JTokenEqualityComparer _equalityComparer;

		// Token: 0x0400069D RID: 1693
		private JContainer _parent;

		// Token: 0x0400069E RID: 1694
		private JToken _previous;

		// Token: 0x0400069F RID: 1695
		private JToken _next;

		// Token: 0x040006A0 RID: 1696
		private object _annotations;

		// Token: 0x040006A1 RID: 1697
		private static readonly JTokenType[] BooleanTypes = new JTokenType[]
		{
			JTokenType.Integer,
			JTokenType.Float,
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.Boolean
		};

		// Token: 0x040006A2 RID: 1698
		private static readonly JTokenType[] NumberTypes = new JTokenType[]
		{
			JTokenType.Integer,
			JTokenType.Float,
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.Boolean
		};

		// Token: 0x040006A3 RID: 1699
		private static readonly JTokenType[] BigIntegerTypes = new JTokenType[]
		{
			JTokenType.Integer,
			JTokenType.Float,
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.Boolean,
			JTokenType.Bytes
		};

		// Token: 0x040006A4 RID: 1700
		private static readonly JTokenType[] StringTypes = new JTokenType[]
		{
			JTokenType.Date,
			JTokenType.Integer,
			JTokenType.Float,
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.Boolean,
			JTokenType.Bytes,
			JTokenType.Guid,
			JTokenType.TimeSpan,
			JTokenType.Uri
		};

		// Token: 0x040006A5 RID: 1701
		private static readonly JTokenType[] GuidTypes = new JTokenType[]
		{
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.Guid,
			JTokenType.Bytes
		};

		// Token: 0x040006A6 RID: 1702
		private static readonly JTokenType[] TimeSpanTypes = new JTokenType[]
		{
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.TimeSpan
		};

		// Token: 0x040006A7 RID: 1703
		private static readonly JTokenType[] UriTypes = new JTokenType[]
		{
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.Uri
		};

		// Token: 0x040006A8 RID: 1704
		private static readonly JTokenType[] CharTypes = new JTokenType[]
		{
			JTokenType.Integer,
			JTokenType.Float,
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw
		};

		// Token: 0x040006A9 RID: 1705
		private static readonly JTokenType[] DateTimeTypes = new JTokenType[]
		{
			JTokenType.Date,
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw
		};

		// Token: 0x040006AA RID: 1706
		private static readonly JTokenType[] BytesTypes = new JTokenType[]
		{
			JTokenType.Bytes,
			JTokenType.String,
			JTokenType.Comment,
			JTokenType.Raw,
			JTokenType.Integer
		};

		// Token: 0x0200029F RID: 671
		private class LineInfoAnnotation
		{
			// Token: 0x060017BC RID: 6076 RVA: 0x0008644C File Offset: 0x0008464C
			public LineInfoAnnotation(int lineNumber, int linePosition)
			{
				this.LineNumber = lineNumber;
				this.LinePosition = linePosition;
			}

			// Token: 0x04000BEB RID: 3051
			internal readonly int LineNumber;

			// Token: 0x04000BEC RID: 3052
			internal readonly int LinePosition;
		}
	}
}
