using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000152 RID: 338
	public class JValue : JToken, IEquatable<JValue>, IFormattable, IComparable, IComparable<JValue>, IConvertible
	{
		// Token: 0x060012B5 RID: 4789 RVA: 0x00069060 File Offset: 0x00067260
		public override Task WriteToAsync(JsonWriter writer, CancellationToken cancellationToken, params JsonConverter[] converters)
		{
			if (converters != null && converters.Length != 0 && this._value != null)
			{
				JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter(converters, this._value.GetType());
				if (matchingConverter != null && matchingConverter.CanWrite)
				{
					matchingConverter.WriteJson(writer, this._value, JsonSerializer.CreateDefault());
					return AsyncUtils.CompletedTask;
				}
			}
			switch (this._valueType)
			{
			case JTokenType.Comment:
			{
				object value = this._value;
				return writer.WriteCommentAsync((value != null) ? value.ToString() : null, cancellationToken);
			}
			case JTokenType.Integer:
				if (this._value is int)
				{
					return writer.WriteValueAsync((int)this._value, cancellationToken);
				}
				if (this._value is long)
				{
					return writer.WriteValueAsync((long)this._value, cancellationToken);
				}
				if (this._value is ulong)
				{
					return writer.WriteValueAsync((ulong)this._value, cancellationToken);
				}
				if (this._value is BigInteger)
				{
					return writer.WriteValueAsync((BigInteger)this._value, cancellationToken);
				}
				return writer.WriteValueAsync(Convert.ToInt64(this._value, CultureInfo.InvariantCulture), cancellationToken);
			case JTokenType.Float:
				if (this._value is decimal)
				{
					return writer.WriteValueAsync((decimal)this._value, cancellationToken);
				}
				if (this._value is double)
				{
					return writer.WriteValueAsync((double)this._value, cancellationToken);
				}
				if (this._value is float)
				{
					return writer.WriteValueAsync((float)this._value, cancellationToken);
				}
				return writer.WriteValueAsync(Convert.ToDouble(this._value, CultureInfo.InvariantCulture), cancellationToken);
			case JTokenType.String:
			{
				object value2 = this._value;
				return writer.WriteValueAsync((value2 != null) ? value2.ToString() : null, cancellationToken);
			}
			case JTokenType.Boolean:
				return writer.WriteValueAsync(Convert.ToBoolean(this._value, CultureInfo.InvariantCulture), cancellationToken);
			case JTokenType.Null:
				return writer.WriteNullAsync(cancellationToken);
			case JTokenType.Undefined:
				return writer.WriteUndefinedAsync(cancellationToken);
			case JTokenType.Date:
				if (this._value is DateTimeOffset)
				{
					return writer.WriteValueAsync((DateTimeOffset)this._value, cancellationToken);
				}
				return writer.WriteValueAsync(Convert.ToDateTime(this._value, CultureInfo.InvariantCulture), cancellationToken);
			case JTokenType.Raw:
			{
				object value3 = this._value;
				return writer.WriteRawValueAsync((value3 != null) ? value3.ToString() : null, cancellationToken);
			}
			case JTokenType.Bytes:
				return writer.WriteValueAsync((byte[])this._value, cancellationToken);
			case JTokenType.Guid:
				return writer.WriteValueAsync((this._value != null) ? ((Guid?)this._value) : null, cancellationToken);
			case JTokenType.Uri:
				return writer.WriteValueAsync((Uri)this._value, cancellationToken);
			case JTokenType.TimeSpan:
				return writer.WriteValueAsync((this._value != null) ? ((TimeSpan?)this._value) : null, cancellationToken);
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", this._valueType, "Unexpected token type.");
			}
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x00069394 File Offset: 0x00067594
		internal JValue(object value, JTokenType type)
		{
			this._value = value;
			this._valueType = type;
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x000693AC File Offset: 0x000675AC
		public JValue(JValue other)
			: this(other.Value, other.Type)
		{
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x000693C0 File Offset: 0x000675C0
		public JValue(long value)
			: this(value, JTokenType.Integer)
		{
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x000693D0 File Offset: 0x000675D0
		public JValue(decimal value)
			: this(value, JTokenType.Float)
		{
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x000693E0 File Offset: 0x000675E0
		public JValue(char value)
			: this(value, JTokenType.String)
		{
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x000693F0 File Offset: 0x000675F0
		[CLSCompliant(false)]
		public JValue(ulong value)
			: this(value, JTokenType.Integer)
		{
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x00069400 File Offset: 0x00067600
		public JValue(double value)
			: this(value, JTokenType.Float)
		{
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x00069410 File Offset: 0x00067610
		public JValue(float value)
			: this(value, JTokenType.Float)
		{
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x00069420 File Offset: 0x00067620
		public JValue(DateTime value)
			: this(value, JTokenType.Date)
		{
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x00069430 File Offset: 0x00067630
		public JValue(DateTimeOffset value)
			: this(value, JTokenType.Date)
		{
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x00069440 File Offset: 0x00067640
		public JValue(bool value)
			: this(value, JTokenType.Boolean)
		{
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x00069450 File Offset: 0x00067650
		public JValue(string value)
			: this(value, JTokenType.String)
		{
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0006945C File Offset: 0x0006765C
		public JValue(Guid value)
			: this(value, JTokenType.Guid)
		{
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0006946C File Offset: 0x0006766C
		public JValue(Uri value)
			: this(value, (value != null) ? JTokenType.Uri : JTokenType.Null)
		{
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0006948C File Offset: 0x0006768C
		public JValue(TimeSpan value)
			: this(value, JTokenType.TimeSpan)
		{
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x0006949C File Offset: 0x0006769C
		public JValue(object value)
			: this(value, JValue.GetValueType(null, value))
		{
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x000694C4 File Offset: 0x000676C4
		internal override bool DeepEquals(JToken node)
		{
			JValue jvalue = node as JValue;
			return jvalue != null && (jvalue == this || JValue.ValuesEquals(this, jvalue));
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060012C7 RID: 4807 RVA: 0x000694F4 File Offset: 0x000676F4
		public override bool HasValues
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x000694F8 File Offset: 0x000676F8
		private static int CompareBigInteger(BigInteger i1, object i2)
		{
			int num = i1.CompareTo(ConvertUtils.ToBigInteger(i2));
			if (num != 0)
			{
				return num;
			}
			if (i2 is decimal)
			{
				decimal num2 = (decimal)i2;
				return 0m.CompareTo(Math.Abs(num2 - Math.Truncate(num2)));
			}
			if (i2 is double || i2 is float)
			{
				double num3 = Convert.ToDouble(i2, CultureInfo.InvariantCulture);
				return 0.0.CompareTo(Math.Abs(num3 - Math.Truncate(num3)));
			}
			return num;
		}

		// Token: 0x060012C9 RID: 4809 RVA: 0x00069594 File Offset: 0x00067794
		internal static int Compare(JTokenType valueType, object objA, object objB)
		{
			if (objA == objB)
			{
				return 0;
			}
			if (objB == null)
			{
				return 1;
			}
			if (objA == null)
			{
				return -1;
			}
			switch (valueType)
			{
			case JTokenType.Comment:
			case JTokenType.String:
			case JTokenType.Raw:
			{
				string text = Convert.ToString(objA, CultureInfo.InvariantCulture);
				string text2 = Convert.ToString(objB, CultureInfo.InvariantCulture);
				return string.CompareOrdinal(text, text2);
			}
			case JTokenType.Integer:
				if (objA is BigInteger)
				{
					return JValue.CompareBigInteger((BigInteger)objA, objB);
				}
				if (objB is BigInteger)
				{
					return -JValue.CompareBigInteger((BigInteger)objB, objA);
				}
				if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
				{
					return Convert.ToDecimal(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
				}
				if (objA is float || objB is float || objA is double || objB is double)
				{
					return JValue.CompareFloat(objA, objB);
				}
				return Convert.ToInt64(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));
			case JTokenType.Float:
				if (objA is BigInteger)
				{
					return JValue.CompareBigInteger((BigInteger)objA, objB);
				}
				if (objB is BigInteger)
				{
					return -JValue.CompareBigInteger((BigInteger)objB, objA);
				}
				if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
				{
					return Convert.ToDecimal(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
				}
				return JValue.CompareFloat(objA, objB);
			case JTokenType.Boolean:
			{
				bool flag = Convert.ToBoolean(objA, CultureInfo.InvariantCulture);
				bool flag2 = Convert.ToBoolean(objB, CultureInfo.InvariantCulture);
				return flag.CompareTo(flag2);
			}
			case JTokenType.Date:
			{
				if (objA is DateTime)
				{
					DateTime dateTime = (DateTime)objA;
					DateTime dateTime2;
					if (objB is DateTimeOffset)
					{
						dateTime2 = ((DateTimeOffset)objB).DateTime;
					}
					else
					{
						dateTime2 = Convert.ToDateTime(objB, CultureInfo.InvariantCulture);
					}
					return dateTime.CompareTo(dateTime2);
				}
				DateTimeOffset dateTimeOffset = (DateTimeOffset)objA;
				DateTimeOffset dateTimeOffset2;
				if (objB is DateTimeOffset)
				{
					dateTimeOffset2 = (DateTimeOffset)objB;
				}
				else
				{
					dateTimeOffset2 = new DateTimeOffset(Convert.ToDateTime(objB, CultureInfo.InvariantCulture));
				}
				return dateTimeOffset.CompareTo(dateTimeOffset2);
			}
			case JTokenType.Bytes:
			{
				byte[] array = objB as byte[];
				if (array == null)
				{
					throw new ArgumentException("Object must be of type byte[].");
				}
				return MiscellaneousUtils.ByteArrayCompare(objA as byte[], array);
			}
			case JTokenType.Guid:
			{
				if (!(objB is Guid))
				{
					throw new ArgumentException("Object must be of type Guid.");
				}
				Guid guid = (Guid)objA;
				Guid guid2 = (Guid)objB;
				return guid.CompareTo(guid2);
			}
			case JTokenType.Uri:
			{
				Uri uri = objB as Uri;
				if (uri == null)
				{
					throw new ArgumentException("Object must be of type Uri.");
				}
				Uri uri2 = (Uri)objA;
				return Comparer<string>.Default.Compare(uri2.ToString(), uri.ToString());
			}
			case JTokenType.TimeSpan:
			{
				if (!(objB is TimeSpan))
				{
					throw new ArgumentException("Object must be of type TimeSpan.");
				}
				TimeSpan timeSpan = (TimeSpan)objA;
				TimeSpan timeSpan2 = (TimeSpan)objB;
				return timeSpan.CompareTo(timeSpan2);
			}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", valueType, "Unexpected value type: {0}".FormatWith(CultureInfo.InvariantCulture, valueType));
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x00069908 File Offset: 0x00067B08
		private static int CompareFloat(object objA, object objB)
		{
			double num = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
			double num2 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
			if (MathUtils.ApproxEquals(num, num2))
			{
				return 0;
			}
			return num.CompareTo(num2);
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x00069948 File Offset: 0x00067B48
		private static bool Operation(ExpressionType operation, object objA, object objB, out object result)
		{
			if ((objA is string || objB is string) && (operation == ExpressionType.Add || operation == ExpressionType.AddAssign))
			{
				result = ((objA != null) ? objA.ToString() : null) + ((objB != null) ? objB.ToString() : null);
				return true;
			}
			if (objA is BigInteger || objB is BigInteger)
			{
				if (objA == null || objB == null)
				{
					result = null;
					return true;
				}
				BigInteger bigInteger = ConvertUtils.ToBigInteger(objA);
				BigInteger bigInteger2 = ConvertUtils.ToBigInteger(objB);
				if (operation <= ExpressionType.Subtract)
				{
					if (operation <= ExpressionType.Divide)
					{
						if (operation != ExpressionType.Add)
						{
							if (operation != ExpressionType.Divide)
							{
								goto IL_048F;
							}
							goto IL_0120;
						}
					}
					else
					{
						if (operation == ExpressionType.Multiply)
						{
							goto IL_0110;
						}
						if (operation != ExpressionType.Subtract)
						{
							goto IL_048F;
						}
						goto IL_0100;
					}
				}
				else if (operation <= ExpressionType.DivideAssign)
				{
					if (operation != ExpressionType.AddAssign)
					{
						if (operation != ExpressionType.DivideAssign)
						{
							goto IL_048F;
						}
						goto IL_0120;
					}
				}
				else
				{
					if (operation == ExpressionType.MultiplyAssign)
					{
						goto IL_0110;
					}
					if (operation != ExpressionType.SubtractAssign)
					{
						goto IL_048F;
					}
					goto IL_0100;
				}
				result = bigInteger + bigInteger2;
				return true;
				IL_0100:
				result = bigInteger - bigInteger2;
				return true;
				IL_0110:
				result = bigInteger * bigInteger2;
				return true;
				IL_0120:
				result = bigInteger / bigInteger2;
				return true;
			}
			else if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
			{
				if (objA == null || objB == null)
				{
					result = null;
					return true;
				}
				decimal num = Convert.ToDecimal(objA, CultureInfo.InvariantCulture);
				decimal num2 = Convert.ToDecimal(objB, CultureInfo.InvariantCulture);
				if (operation <= ExpressionType.Subtract)
				{
					if (operation <= ExpressionType.Divide)
					{
						if (operation != ExpressionType.Add)
						{
							if (operation != ExpressionType.Divide)
							{
								goto IL_048F;
							}
							goto IL_021F;
						}
					}
					else
					{
						if (operation == ExpressionType.Multiply)
						{
							goto IL_020F;
						}
						if (operation != ExpressionType.Subtract)
						{
							goto IL_048F;
						}
						goto IL_01FF;
					}
				}
				else if (operation <= ExpressionType.DivideAssign)
				{
					if (operation != ExpressionType.AddAssign)
					{
						if (operation != ExpressionType.DivideAssign)
						{
							goto IL_048F;
						}
						goto IL_021F;
					}
				}
				else
				{
					if (operation == ExpressionType.MultiplyAssign)
					{
						goto IL_020F;
					}
					if (operation != ExpressionType.SubtractAssign)
					{
						goto IL_048F;
					}
					goto IL_01FF;
				}
				result = num + num2;
				return true;
				IL_01FF:
				result = num - num2;
				return true;
				IL_020F:
				result = num * num2;
				return true;
				IL_021F:
				result = num / num2;
				return true;
			}
			else if (objA is float || objB is float || objA is double || objB is double)
			{
				if (objA == null || objB == null)
				{
					result = null;
					return true;
				}
				double num3 = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
				double num4 = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
				if (operation <= ExpressionType.Subtract)
				{
					if (operation <= ExpressionType.Divide)
					{
						if (operation != ExpressionType.Add)
						{
							if (operation != ExpressionType.Divide)
							{
								goto IL_048F;
							}
							goto IL_031A;
						}
					}
					else
					{
						if (operation == ExpressionType.Multiply)
						{
							goto IL_030C;
						}
						if (operation != ExpressionType.Subtract)
						{
							goto IL_048F;
						}
						goto IL_02FE;
					}
				}
				else if (operation <= ExpressionType.DivideAssign)
				{
					if (operation != ExpressionType.AddAssign)
					{
						if (operation != ExpressionType.DivideAssign)
						{
							goto IL_048F;
						}
						goto IL_031A;
					}
				}
				else
				{
					if (operation == ExpressionType.MultiplyAssign)
					{
						goto IL_030C;
					}
					if (operation != ExpressionType.SubtractAssign)
					{
						goto IL_048F;
					}
					goto IL_02FE;
				}
				result = num3 + num4;
				return true;
				IL_02FE:
				result = num3 - num4;
				return true;
				IL_030C:
				result = num3 * num4;
				return true;
				IL_031A:
				result = num3 / num4;
				return true;
			}
			else if (objA is int || objA is uint || objA is long || objA is short || objA is ushort || objA is sbyte || objA is byte || objB is int || objB is uint || objB is long || objB is short || objB is ushort || objB is sbyte || objB is byte)
			{
				if (objA == null || objB == null)
				{
					result = null;
					return true;
				}
				long num5 = Convert.ToInt64(objA, CultureInfo.InvariantCulture);
				long num6 = Convert.ToInt64(objB, CultureInfo.InvariantCulture);
				if (operation <= ExpressionType.Subtract)
				{
					if (operation <= ExpressionType.Divide)
					{
						if (operation != ExpressionType.Add)
						{
							if (operation != ExpressionType.Divide)
							{
								goto IL_048F;
							}
							goto IL_0481;
						}
					}
					else
					{
						if (operation == ExpressionType.Multiply)
						{
							goto IL_0473;
						}
						if (operation != ExpressionType.Subtract)
						{
							goto IL_048F;
						}
						goto IL_0465;
					}
				}
				else if (operation <= ExpressionType.DivideAssign)
				{
					if (operation != ExpressionType.AddAssign)
					{
						if (operation != ExpressionType.DivideAssign)
						{
							goto IL_048F;
						}
						goto IL_0481;
					}
				}
				else
				{
					if (operation == ExpressionType.MultiplyAssign)
					{
						goto IL_0473;
					}
					if (operation != ExpressionType.SubtractAssign)
					{
						goto IL_048F;
					}
					goto IL_0465;
				}
				result = num5 + num6;
				return true;
				IL_0465:
				result = num5 - num6;
				return true;
				IL_0473:
				result = num5 * num6;
				return true;
				IL_0481:
				result = num5 / num6;
				return true;
			}
			IL_048F:
			result = null;
			return false;
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x00069DEC File Offset: 0x00067FEC
		internal override JToken CloneToken()
		{
			return new JValue(this);
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x00069DF4 File Offset: 0x00067FF4
		public static JValue CreateComment(string value)
		{
			return new JValue(value, JTokenType.Comment);
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x00069E00 File Offset: 0x00068000
		public static JValue CreateString(string value)
		{
			return new JValue(value, JTokenType.String);
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x00069E0C File Offset: 0x0006800C
		public static JValue CreateNull()
		{
			return new JValue(null, JTokenType.Null);
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x00069E18 File Offset: 0x00068018
		public static JValue CreateUndefined()
		{
			return new JValue(null, JTokenType.Undefined);
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x00069E24 File Offset: 0x00068024
		private static JTokenType GetValueType(JTokenType? current, object value)
		{
			if (value == null)
			{
				return JTokenType.Null;
			}
			if (value == DBNull.Value)
			{
				return JTokenType.Null;
			}
			if (value is string)
			{
				return JValue.GetStringValueType(current);
			}
			if (value is long || value is int || value is short || value is sbyte || value is ulong || value is uint || value is ushort || value is byte)
			{
				return JTokenType.Integer;
			}
			if (value is Enum)
			{
				return JTokenType.Integer;
			}
			if (value is BigInteger)
			{
				return JTokenType.Integer;
			}
			if (value is double || value is float || value is decimal)
			{
				return JTokenType.Float;
			}
			if (value is DateTime)
			{
				return JTokenType.Date;
			}
			if (value is DateTimeOffset)
			{
				return JTokenType.Date;
			}
			if (value is byte[])
			{
				return JTokenType.Bytes;
			}
			if (value is bool)
			{
				return JTokenType.Boolean;
			}
			if (value is Guid)
			{
				return JTokenType.Guid;
			}
			if (value is Uri)
			{
				return JTokenType.Uri;
			}
			if (value is TimeSpan)
			{
				return JTokenType.TimeSpan;
			}
			throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x00069F74 File Offset: 0x00068174
		private static JTokenType GetStringValueType(JTokenType? current)
		{
			if (current == null)
			{
				return JTokenType.String;
			}
			JTokenType valueOrDefault = current.GetValueOrDefault();
			if (valueOrDefault == JTokenType.Comment || valueOrDefault == JTokenType.String || valueOrDefault == JTokenType.Raw)
			{
				return current.GetValueOrDefault();
			}
			return JTokenType.String;
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060012D3 RID: 4819 RVA: 0x00069FBC File Offset: 0x000681BC
		public override JTokenType Type
		{
			get
			{
				return this._valueType;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x00069FC4 File Offset: 0x000681C4
		// (set) Token: 0x060012D5 RID: 4821 RVA: 0x00069FCC File Offset: 0x000681CC
		public new object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				object value2 = this._value;
				Type type = ((value2 != null) ? value2.GetType() : null);
				Type type2 = ((value != null) ? value.GetType() : null);
				if (type != type2)
				{
					this._valueType = JValue.GetValueType(new JTokenType?(this._valueType), value);
				}
				this._value = value;
			}
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0006A034 File Offset: 0x00068234
		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			if (converters != null && converters.Length != 0 && this._value != null)
			{
				JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter(converters, this._value.GetType());
				if (matchingConverter != null && matchingConverter.CanWrite)
				{
					matchingConverter.WriteJson(writer, this._value, JsonSerializer.CreateDefault());
					return;
				}
			}
			switch (this._valueType)
			{
			case JTokenType.Comment:
			{
				object value = this._value;
				writer.WriteComment((value != null) ? value.ToString() : null);
				return;
			}
			case JTokenType.Integer:
				if (this._value is int)
				{
					writer.WriteValue((int)this._value);
					return;
				}
				if (this._value is long)
				{
					writer.WriteValue((long)this._value);
					return;
				}
				if (this._value is ulong)
				{
					writer.WriteValue((ulong)this._value);
					return;
				}
				if (this._value is BigInteger)
				{
					writer.WriteValue((BigInteger)this._value);
					return;
				}
				writer.WriteValue(Convert.ToInt64(this._value, CultureInfo.InvariantCulture));
				return;
			case JTokenType.Float:
				if (this._value is decimal)
				{
					writer.WriteValue((decimal)this._value);
					return;
				}
				if (this._value is double)
				{
					writer.WriteValue((double)this._value);
					return;
				}
				if (this._value is float)
				{
					writer.WriteValue((float)this._value);
					return;
				}
				writer.WriteValue(Convert.ToDouble(this._value, CultureInfo.InvariantCulture));
				return;
			case JTokenType.String:
			{
				object value2 = this._value;
				writer.WriteValue((value2 != null) ? value2.ToString() : null);
				return;
			}
			case JTokenType.Boolean:
				writer.WriteValue(Convert.ToBoolean(this._value, CultureInfo.InvariantCulture));
				return;
			case JTokenType.Null:
				writer.WriteNull();
				return;
			case JTokenType.Undefined:
				writer.WriteUndefined();
				return;
			case JTokenType.Date:
				if (this._value is DateTimeOffset)
				{
					writer.WriteValue((DateTimeOffset)this._value);
					return;
				}
				writer.WriteValue(Convert.ToDateTime(this._value, CultureInfo.InvariantCulture));
				return;
			case JTokenType.Raw:
			{
				object value3 = this._value;
				writer.WriteRawValue((value3 != null) ? value3.ToString() : null);
				return;
			}
			case JTokenType.Bytes:
				writer.WriteValue((byte[])this._value);
				return;
			case JTokenType.Guid:
				writer.WriteValue((this._value != null) ? ((Guid?)this._value) : null);
				return;
			case JTokenType.Uri:
				writer.WriteValue((Uri)this._value);
				return;
			case JTokenType.TimeSpan:
				writer.WriteValue((this._value != null) ? ((TimeSpan?)this._value) : null);
				return;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", this._valueType, "Unexpected token type.");
			}
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0006A34C File Offset: 0x0006854C
		internal override int GetDeepHashCode()
		{
			int num = ((this._value != null) ? this._value.GetHashCode() : 0);
			int valueType = (int)this._valueType;
			return valueType.GetHashCode() ^ num;
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x0006A38C File Offset: 0x0006858C
		private static bool ValuesEquals(JValue v1, JValue v2)
		{
			return v1 == v2 || (v1._valueType == v2._valueType && JValue.Compare(v1._valueType, v1._value, v2._value) == 0);
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0006A3C4 File Offset: 0x000685C4
		public bool Equals(JValue other)
		{
			return other != null && JValue.ValuesEquals(this, other);
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0006A3D8 File Offset: 0x000685D8
		public override bool Equals(object obj)
		{
			return this.Equals(obj as JValue);
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0006A3E8 File Offset: 0x000685E8
		public override int GetHashCode()
		{
			if (this._value == null)
			{
				return 0;
			}
			return this._value.GetHashCode();
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0006A404 File Offset: 0x00068604
		public override string ToString()
		{
			if (this._value == null)
			{
				return string.Empty;
			}
			return this._value.ToString();
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x0006A424 File Offset: 0x00068624
		public string ToString(string format)
		{
			return this.ToString(format, CultureInfo.CurrentCulture);
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x0006A434 File Offset: 0x00068634
		public string ToString(IFormatProvider formatProvider)
		{
			return this.ToString(null, formatProvider);
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0006A440 File Offset: 0x00068640
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (this._value == null)
			{
				return string.Empty;
			}
			IFormattable formattable = this._value as IFormattable;
			if (formattable != null)
			{
				return formattable.ToString(format, formatProvider);
			}
			return this._value.ToString();
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x0006A488 File Offset: 0x00068688
		protected override DynamicMetaObject GetMetaObject(Expression parameter)
		{
			return new DynamicProxyMetaObject<JValue>(parameter, this, new JValue.JValueDynamicProxy());
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0006A498 File Offset: 0x00068698
		int IComparable.CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			JValue jvalue = obj as JValue;
			object obj2 = ((jvalue != null) ? jvalue.Value : obj);
			return JValue.Compare(this._valueType, this._value, obj2);
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0006A4E0 File Offset: 0x000686E0
		public int CompareTo(JValue obj)
		{
			if (obj == null)
			{
				return 1;
			}
			return JValue.Compare(this._valueType, this._value, obj._value);
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0006A504 File Offset: 0x00068704
		TypeCode IConvertible.GetTypeCode()
		{
			if (this._value == null)
			{
				return TypeCode.Empty;
			}
			IConvertible convertible = this._value as IConvertible;
			if (convertible == null)
			{
				return TypeCode.Object;
			}
			return convertible.GetTypeCode();
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0006A53C File Offset: 0x0006873C
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return (bool)this;
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0006A544 File Offset: 0x00068744
		char IConvertible.ToChar(IFormatProvider provider)
		{
			return (char)this;
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0006A54C File Offset: 0x0006874C
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return (sbyte)this;
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x0006A554 File Offset: 0x00068754
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return (byte)this;
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0006A55C File Offset: 0x0006875C
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return (short)this;
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0006A564 File Offset: 0x00068764
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return (ushort)this;
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x0006A56C File Offset: 0x0006876C
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return (int)this;
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x0006A574 File Offset: 0x00068774
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this;
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x0006A57C File Offset: 0x0006877C
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return (long)this;
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x0006A584 File Offset: 0x00068784
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this;
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0006A58C File Offset: 0x0006878C
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return (float)this;
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0006A598 File Offset: 0x00068798
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return (double)this;
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x0006A5A4 File Offset: 0x000687A4
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return (decimal)this;
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0006A5AC File Offset: 0x000687AC
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return (DateTime)this;
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0006A5B4 File Offset: 0x000687B4
		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return base.ToObject(conversionType);
		}

		// Token: 0x040006C6 RID: 1734
		private JTokenType _valueType;

		// Token: 0x040006C7 RID: 1735
		private object _value;

		// Token: 0x020002A6 RID: 678
		private class JValueDynamicProxy : DynamicProxy<JValue>
		{
			// Token: 0x060017E7 RID: 6119 RVA: 0x00086FF8 File Offset: 0x000851F8
			public override bool TryConvert(JValue instance, ConvertBinder binder, out object result)
			{
				if (binder.Type == typeof(JValue) || binder.Type == typeof(JToken))
				{
					result = instance;
					return true;
				}
				object value = instance.Value;
				if (value == null)
				{
					result = null;
					return ReflectionUtils.IsNullable(binder.Type);
				}
				result = ConvertUtils.Convert(value, CultureInfo.InvariantCulture, binder.Type);
				return true;
			}

			// Token: 0x060017E8 RID: 6120 RVA: 0x00087074 File Offset: 0x00085274
			public override bool TryBinaryOperation(JValue instance, BinaryOperationBinder binder, object arg, out object result)
			{
				JValue jvalue = arg as JValue;
				object obj = ((jvalue != null) ? jvalue.Value : arg);
				ExpressionType operation = binder.Operation;
				if (operation <= ExpressionType.NotEqual)
				{
					if (operation <= ExpressionType.LessThanOrEqual)
					{
						if (operation != ExpressionType.Add)
						{
							switch (operation)
							{
							case ExpressionType.Divide:
								break;
							case ExpressionType.Equal:
								result = JValue.Compare(instance.Type, instance.Value, obj) == 0;
								return true;
							case ExpressionType.ExclusiveOr:
							case ExpressionType.Invoke:
							case ExpressionType.Lambda:
							case ExpressionType.LeftShift:
								goto IL_01A2;
							case ExpressionType.GreaterThan:
								result = JValue.Compare(instance.Type, instance.Value, obj) > 0;
								return true;
							case ExpressionType.GreaterThanOrEqual:
								result = JValue.Compare(instance.Type, instance.Value, obj) >= 0;
								return true;
							case ExpressionType.LessThan:
								result = JValue.Compare(instance.Type, instance.Value, obj) < 0;
								return true;
							case ExpressionType.LessThanOrEqual:
								result = JValue.Compare(instance.Type, instance.Value, obj) <= 0;
								return true;
							default:
								goto IL_01A2;
							}
						}
					}
					else if (operation != ExpressionType.Multiply)
					{
						if (operation != ExpressionType.NotEqual)
						{
							goto IL_01A2;
						}
						result = JValue.Compare(instance.Type, instance.Value, obj) != 0;
						return true;
					}
				}
				else if (operation <= ExpressionType.AddAssign)
				{
					if (operation != ExpressionType.Subtract && operation != ExpressionType.AddAssign)
					{
						goto IL_01A2;
					}
				}
				else if (operation != ExpressionType.DivideAssign && operation != ExpressionType.MultiplyAssign && operation != ExpressionType.SubtractAssign)
				{
					goto IL_01A2;
				}
				if (JValue.Operation(binder.Operation, instance.Value, obj, out result))
				{
					result = new JValue(result);
					return true;
				}
				IL_01A2:
				result = null;
				return false;
			}
		}
	}
}
