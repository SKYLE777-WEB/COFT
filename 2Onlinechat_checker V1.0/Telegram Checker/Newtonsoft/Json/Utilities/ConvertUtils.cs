using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D1 RID: 209
	internal static class ConvertUtils
	{
		// Token: 0x06000BA1 RID: 2977 RVA: 0x00049334 File Offset: 0x00047534
		public static PrimitiveTypeCode GetTypeCode(Type t)
		{
			bool flag;
			return ConvertUtils.GetTypeCode(t, out flag);
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x00049350 File Offset: 0x00047550
		public static PrimitiveTypeCode GetTypeCode(Type t, out bool isEnum)
		{
			PrimitiveTypeCode primitiveTypeCode;
			if (ConvertUtils.TypeCodeMap.TryGetValue(t, out primitiveTypeCode))
			{
				isEnum = false;
				return primitiveTypeCode;
			}
			if (t.IsEnum())
			{
				isEnum = true;
				return ConvertUtils.GetTypeCode(Enum.GetUnderlyingType(t));
			}
			if (ReflectionUtils.IsNullableType(t))
			{
				Type underlyingType = Nullable.GetUnderlyingType(t);
				if (underlyingType.IsEnum())
				{
					Type type = typeof(Nullable<>).MakeGenericType(new Type[] { Enum.GetUnderlyingType(underlyingType) });
					isEnum = true;
					return ConvertUtils.GetTypeCode(type);
				}
			}
			isEnum = false;
			return PrimitiveTypeCode.Object;
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x000493DC File Offset: 0x000475DC
		public static TypeInformation GetTypeInformation(IConvertible convertable)
		{
			return ConvertUtils.PrimitiveTypeCodes[(int)convertable.GetTypeCode()];
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x000493F0 File Offset: 0x000475F0
		public static bool IsConvertible(Type t)
		{
			return typeof(IConvertible).IsAssignableFrom(t);
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x00049404 File Offset: 0x00047604
		public static TimeSpan ParseTimeSpan(string input)
		{
			return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x00049414 File Offset: 0x00047614
		private static Func<object, object> CreateCastConverter(ConvertUtils.TypeConvertKey t)
		{
			MethodInfo methodInfo = t.TargetType.GetMethod("op_Implicit", new Type[] { t.InitialType });
			if (methodInfo == null)
			{
				methodInfo = t.TargetType.GetMethod("op_Explicit", new Type[] { t.InitialType });
			}
			if (methodInfo == null)
			{
				return null;
			}
			MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
			return (object o) => call(null, new object[] { o });
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x000494A8 File Offset: 0x000476A8
		internal static BigInteger ToBigInteger(object value)
		{
			if (value is BigInteger)
			{
				return (BigInteger)value;
			}
			string text = value as string;
			if (text != null)
			{
				return BigInteger.Parse(text, CultureInfo.InvariantCulture);
			}
			if (value is float)
			{
				return new BigInteger((float)value);
			}
			if (value is double)
			{
				return new BigInteger((double)value);
			}
			if (value is decimal)
			{
				return new BigInteger((decimal)value);
			}
			if (value is int)
			{
				return new BigInteger((int)value);
			}
			if (value is long)
			{
				return new BigInteger((long)value);
			}
			if (value is uint)
			{
				return new BigInteger((uint)value);
			}
			if (value is ulong)
			{
				return new BigInteger((ulong)value);
			}
			byte[] array = value as byte[];
			if (array != null)
			{
				return new BigInteger(array);
			}
			throw new InvalidCastException("Cannot convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x000495B4 File Offset: 0x000477B4
		public static object FromBigInteger(BigInteger i, Type targetType)
		{
			if (targetType == typeof(decimal))
			{
				return (decimal)i;
			}
			if (targetType == typeof(double))
			{
				return (double)i;
			}
			if (targetType == typeof(float))
			{
				return (float)i;
			}
			if (targetType == typeof(ulong))
			{
				return (ulong)i;
			}
			if (targetType == typeof(bool))
			{
				return i != 0L;
			}
			object obj;
			try
			{
				obj = global::System.Convert.ChangeType((long)i, targetType, CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Can not convert from BigInteger to {0}.".FormatWith(CultureInfo.InvariantCulture, targetType), ex);
			}
			return obj;
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x000496B0 File Offset: 0x000478B0
		public static object Convert(object initialValue, CultureInfo culture, Type targetType)
		{
			object obj;
			switch (ConvertUtils.TryConvertInternal(initialValue, culture, targetType, out obj))
			{
			case ConvertUtils.ConvertResult.Success:
				return obj;
			case ConvertUtils.ConvertResult.CannotConvertNull:
				throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
			case ConvertUtils.ConvertResult.NotInstantiableType:
				throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, targetType), "targetType");
			case ConvertUtils.ConvertResult.NoValidConversion:
				throw new InvalidOperationException("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
			default:
				throw new InvalidOperationException("Unexpected conversion result.");
			}
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x00049748 File Offset: 0x00047948
		private static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object value)
		{
			bool flag;
			try
			{
				if (ConvertUtils.TryConvertInternal(initialValue, culture, targetType, out value) == ConvertUtils.ConvertResult.Success)
				{
					flag = true;
				}
				else
				{
					value = null;
					flag = false;
				}
			}
			catch
			{
				value = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x00049790 File Offset: 0x00047990
		private static ConvertUtils.ConvertResult TryConvertInternal(object initialValue, CultureInfo culture, Type targetType, out object value)
		{
			if (initialValue == null)
			{
				throw new ArgumentNullException("initialValue");
			}
			if (ReflectionUtils.IsNullableType(targetType))
			{
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			Type type = initialValue.GetType();
			if (targetType == type)
			{
				value = initialValue;
				return ConvertUtils.ConvertResult.Success;
			}
			if (ConvertUtils.IsConvertible(initialValue.GetType()) && ConvertUtils.IsConvertible(targetType))
			{
				if (targetType.IsEnum())
				{
					if (initialValue is string)
					{
						value = Enum.Parse(targetType, initialValue.ToString(), true);
						return ConvertUtils.ConvertResult.Success;
					}
					if (ConvertUtils.IsInteger(initialValue))
					{
						value = Enum.ToObject(targetType, initialValue);
						return ConvertUtils.ConvertResult.Success;
					}
				}
				value = global::System.Convert.ChangeType(initialValue, targetType, culture);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue is DateTime && targetType == typeof(DateTimeOffset))
			{
				value = new DateTimeOffset((DateTime)initialValue);
				return ConvertUtils.ConvertResult.Success;
			}
			byte[] array = initialValue as byte[];
			if (array != null && targetType == typeof(Guid))
			{
				value = new Guid(array);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue is Guid && targetType == typeof(byte[]))
			{
				value = ((Guid)initialValue).ToByteArray();
				return ConvertUtils.ConvertResult.Success;
			}
			string text = initialValue as string;
			if (text != null)
			{
				if (targetType == typeof(Guid))
				{
					value = new Guid(text);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(Uri))
				{
					value = new Uri(text, UriKind.RelativeOrAbsolute);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(TimeSpan))
				{
					value = ConvertUtils.ParseTimeSpan(text);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(byte[]))
				{
					value = global::System.Convert.FromBase64String(text);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType == typeof(Version))
				{
					Version version;
					if (ConvertUtils.VersionTryParse(text, out version))
					{
						value = version;
						return ConvertUtils.ConvertResult.Success;
					}
					value = null;
					return ConvertUtils.ConvertResult.NoValidConversion;
				}
				else if (typeof(Type).IsAssignableFrom(targetType))
				{
					value = Type.GetType(text, true);
					return ConvertUtils.ConvertResult.Success;
				}
			}
			if (targetType == typeof(BigInteger))
			{
				value = ConvertUtils.ToBigInteger(initialValue);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue is BigInteger)
			{
				value = ConvertUtils.FromBigInteger((BigInteger)initialValue, targetType);
				return ConvertUtils.ConvertResult.Success;
			}
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (converter != null && converter.CanConvertTo(targetType))
			{
				value = converter.ConvertTo(null, culture, initialValue, targetType);
				return ConvertUtils.ConvertResult.Success;
			}
			TypeConverter converter2 = TypeDescriptor.GetConverter(targetType);
			if (converter2 != null && converter2.CanConvertFrom(type))
			{
				value = converter2.ConvertFrom(null, culture, initialValue);
				return ConvertUtils.ConvertResult.Success;
			}
			if (initialValue == DBNull.Value)
			{
				if (ReflectionUtils.IsNullable(targetType))
				{
					value = ConvertUtils.EnsureTypeAssignable(null, type, targetType);
					return ConvertUtils.ConvertResult.Success;
				}
				value = null;
				return ConvertUtils.ConvertResult.CannotConvertNull;
			}
			else
			{
				INullable nullable = initialValue as INullable;
				if (nullable != null)
				{
					value = ConvertUtils.EnsureTypeAssignable(ConvertUtils.ToValue(nullable), type, targetType);
					return ConvertUtils.ConvertResult.Success;
				}
				if (targetType.IsInterface() || targetType.IsGenericTypeDefinition() || targetType.IsAbstract())
				{
					value = null;
					return ConvertUtils.ConvertResult.NotInstantiableType;
				}
				value = null;
				return ConvertUtils.ConvertResult.NoValidConversion;
			}
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x00049AC0 File Offset: 0x00047CC0
		public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
		{
			if (targetType == typeof(object))
			{
				return initialValue;
			}
			if (initialValue == null && ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			object obj;
			if (ConvertUtils.TryConvert(initialValue, culture, targetType, out obj))
			{
				return obj;
			}
			return ConvertUtils.EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x00049B1C File Offset: 0x00047D1C
		private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
		{
			Type type = ((value != null) ? value.GetType() : null);
			if (value != null)
			{
				if (targetType.IsAssignableFrom(type))
				{
					return value;
				}
				Func<object, object> func = ConvertUtils.CastConverters.Get(new ConvertUtils.TypeConvertKey(type, targetType));
				if (func != null)
				{
					return func(value);
				}
			}
			else if (ReflectionUtils.IsNullable(targetType))
			{
				return null;
			}
			throw new ArgumentException("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, ((initialType != null) ? initialType.ToString() : null) ?? "{null}", targetType));
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x00049BB4 File Offset: 0x00047DB4
		public static object ToValue(INullable nullableValue)
		{
			if (nullableValue == null)
			{
				return null;
			}
			if (nullableValue is SqlInt32)
			{
				return ConvertUtils.ToValue((SqlInt32)nullableValue);
			}
			if (nullableValue is SqlInt64)
			{
				return ConvertUtils.ToValue((SqlInt64)nullableValue);
			}
			if (nullableValue is SqlBoolean)
			{
				return ConvertUtils.ToValue((SqlBoolean)nullableValue);
			}
			if (nullableValue is SqlString)
			{
				return ConvertUtils.ToValue((SqlString)nullableValue);
			}
			if (nullableValue is SqlDateTime)
			{
				return ConvertUtils.ToValue((SqlDateTime)nullableValue);
			}
			throw new ArgumentException("Unsupported INullable type: {0}".FormatWith(CultureInfo.InvariantCulture, nullableValue.GetType()));
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x00049C74 File Offset: 0x00047E74
		public static bool VersionTryParse(string input, out Version result)
		{
			return Version.TryParse(input, out result);
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x00049C80 File Offset: 0x00047E80
		public static bool IsInteger(object value)
		{
			switch (ConvertUtils.GetTypeCode(value.GetType()))
			{
			case PrimitiveTypeCode.SByte:
			case PrimitiveTypeCode.Int16:
			case PrimitiveTypeCode.UInt16:
			case PrimitiveTypeCode.Int32:
			case PrimitiveTypeCode.Byte:
			case PrimitiveTypeCode.UInt32:
			case PrimitiveTypeCode.Int64:
			case PrimitiveTypeCode.UInt64:
				return true;
			}
			return false;
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x00049CEC File Offset: 0x00047EEC
		public static ParseResult Int32TryParse(char[] chars, int start, int length, out int value)
		{
			value = 0;
			if (length == 0)
			{
				return ParseResult.Invalid;
			}
			bool flag = chars[start] == '-';
			if (flag)
			{
				if (length == 1)
				{
					return ParseResult.Invalid;
				}
				start++;
				length--;
			}
			int num = start + length;
			if (length > 10 || (length == 10 && chars[start] - '0' > '\u0002'))
			{
				for (int i = start; i < num; i++)
				{
					int num2 = (int)(chars[i] - '0');
					if (num2 < 0 || num2 > 9)
					{
						return ParseResult.Invalid;
					}
				}
				return ParseResult.Overflow;
			}
			for (int j = start; j < num; j++)
			{
				int num3 = (int)(chars[j] - '0');
				if (num3 < 0 || num3 > 9)
				{
					return ParseResult.Invalid;
				}
				int num4 = 10 * value - num3;
				if (num4 > value)
				{
					for (j++; j < num; j++)
					{
						num3 = (int)(chars[j] - '0');
						if (num3 < 0 || num3 > 9)
						{
							return ParseResult.Invalid;
						}
					}
					return ParseResult.Overflow;
				}
				value = num4;
			}
			if (!flag)
			{
				if (value == -2147483648)
				{
					return ParseResult.Overflow;
				}
				value = -value;
			}
			return ParseResult.Success;
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x00049E08 File Offset: 0x00048008
		public static ParseResult Int64TryParse(char[] chars, int start, int length, out long value)
		{
			value = 0L;
			if (length == 0)
			{
				return ParseResult.Invalid;
			}
			bool flag = chars[start] == '-';
			if (flag)
			{
				if (length == 1)
				{
					return ParseResult.Invalid;
				}
				start++;
				length--;
			}
			int num = start + length;
			if (length > 19)
			{
				for (int i = start; i < num; i++)
				{
					int num2 = (int)(chars[i] - '0');
					if (num2 < 0 || num2 > 9)
					{
						return ParseResult.Invalid;
					}
				}
				return ParseResult.Overflow;
			}
			for (int j = start; j < num; j++)
			{
				int num3 = (int)(chars[j] - '0');
				if (num3 < 0 || num3 > 9)
				{
					return ParseResult.Invalid;
				}
				long num4 = 10L * value - (long)num3;
				if (num4 > value)
				{
					for (j++; j < num; j++)
					{
						num3 = (int)(chars[j] - '0');
						if (num3 < 0 || num3 > 9)
						{
							return ParseResult.Invalid;
						}
					}
					return ParseResult.Overflow;
				}
				value = num4;
			}
			if (!flag)
			{
				if (value == -9223372036854775808L)
				{
					return ParseResult.Overflow;
				}
				value = -value;
			}
			return ParseResult.Success;
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x00049F18 File Offset: 0x00048118
		public static ParseResult DecimalTryParse(char[] chars, int start, int length, out decimal value)
		{
			value = 0m;
			if (length == 0)
			{
				return ParseResult.Invalid;
			}
			bool flag = chars[start] == '-';
			if (flag)
			{
				if (length == 1)
				{
					return ParseResult.Invalid;
				}
				start++;
				length--;
			}
			int i = start;
			int num = start + length;
			int num2 = num;
			int num3 = num;
			int num4 = 0;
			ulong num5 = 0UL;
			ulong num6 = 0UL;
			int num7 = 0;
			int num8 = 0;
			bool? flag2 = null;
			bool? flag3 = null;
			while (i < num)
			{
				char c = chars[i];
				if (c == '.')
				{
					goto IL_0086;
				}
				if (c != 'E' && c != 'e')
				{
					if (c < '0' || c > '9')
					{
						return ParseResult.Invalid;
					}
					if (i == start && c == '0')
					{
						i++;
						if (i != num)
						{
							c = chars[i];
							if (c == '.')
							{
								goto IL_0086;
							}
							if (c != 'e' && c != 'E')
							{
								return ParseResult.Invalid;
							}
							goto IL_00AC;
						}
					}
					if (num7 < 29)
					{
						if (num7 == 28)
						{
							bool? flag4 = flag3;
							bool flag6;
							if (flag4 == null)
							{
								flag3 = new bool?(num5 > 7922816251426433759UL || (num5 == 7922816251426433759UL && (num6 > 354395033UL || (num6 == 354395033UL && c > '5'))));
								bool? flag5 = flag3;
								flag6 = flag5.GetValueOrDefault();
							}
							else
							{
								flag6 = flag4.GetValueOrDefault();
							}
							if (flag6)
							{
								goto IL_0277;
							}
						}
						if (num7 < 19)
						{
							num5 = num5 * 10UL + (ulong)((long)(c - '0'));
						}
						else
						{
							num6 = num6 * 10UL + (ulong)((long)(c - '0'));
						}
						num7++;
						goto IL_0299;
					}
					IL_0277:
					if (flag2 == null)
					{
						flag2 = new bool?(c >= '5');
					}
					num8++;
					goto IL_0299;
				}
				IL_00AC:
				if (i == start)
				{
					return ParseResult.Invalid;
				}
				if (i == num2)
				{
					return ParseResult.Invalid;
				}
				i++;
				if (i == num)
				{
					return ParseResult.Invalid;
				}
				if (num2 < num)
				{
					num3 = i - 1;
				}
				c = chars[i];
				bool flag7 = false;
				if (c != '+')
				{
					if (c == '-')
					{
						flag7 = true;
						i++;
					}
				}
				else
				{
					i++;
				}
				while (i < num)
				{
					c = chars[i];
					if (c < '0' || c > '9')
					{
						return ParseResult.Invalid;
					}
					int num9 = 10 * num4 + (int)(c - '0');
					if (num4 < num9)
					{
						num4 = num9;
					}
					i++;
				}
				if (flag7)
				{
					num4 = -num4;
				}
				IL_0299:
				i++;
				continue;
				IL_0086:
				if (i == start)
				{
					return ParseResult.Invalid;
				}
				if (i + 1 == num)
				{
					return ParseResult.Invalid;
				}
				if (num2 != num)
				{
					return ParseResult.Invalid;
				}
				num2 = i + 1;
				goto IL_0299;
			}
			num4 += num8;
			num4 -= num3 - num2;
			if (num7 <= 19)
			{
				value = num5;
			}
			else
			{
				value = num5 * ConvertUtils.DecimalFactors[num7 - 20] + num6;
			}
			if (num4 > 0)
			{
				num7 += num4;
				if (num7 > 29)
				{
					return ParseResult.Overflow;
				}
				if (num7 == 29)
				{
					if (num4 > 1)
					{
						value *= ConvertUtils.DecimalFactors[num4 - 2];
						if (value > 7922816251426433759354395033m)
						{
							return ParseResult.Overflow;
						}
					}
					value *= 10m;
				}
				else
				{
					value *= ConvertUtils.DecimalFactors[num4 - 1];
				}
			}
			else
			{
				if (flag2 == true && num4 >= -28)
				{
					value += 1m;
				}
				if (num4 < 0)
				{
					if (num7 + num4 + 28 <= 0)
					{
						value = 0m;
						return ParseResult.Success;
					}
					if (num4 >= -28)
					{
						value /= ConvertUtils.DecimalFactors[-num4 - 1];
					}
					else
					{
						decimal[] decimalFactors = ConvertUtils.DecimalFactors;
						value /= decimalFactors[27];
						value /= decimalFactors[-num4 - 29];
					}
				}
			}
			if (flag)
			{
				value = -value;
			}
			return ParseResult.Success;
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000BB4 RID: 2996 RVA: 0x0004A3C0 File Offset: 0x000485C0
		private static decimal[] DecimalFactors
		{
			get
			{
				decimal[] array = ConvertUtils._decimalFactors;
				if (array == null)
				{
					array = new decimal[28];
					decimal num = 1m;
					for (int i = 0; i < array.Length; i++)
					{
						num = (array[i] = num * 10m);
					}
					ConvertUtils._decimalFactors = array;
				}
				return array;
			}
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x0004A41C File Offset: 0x0004861C
		public static bool TryConvertGuid(string s, out Guid g)
		{
			return Guid.TryParseExact(s, "D", out g);
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0004A42C File Offset: 0x0004862C
		public static bool TryHexTextToInt(char[] text, int start, int end, out int value)
		{
			value = 0;
			for (int i = start; i < end; i++)
			{
				char c = text[i];
				int num;
				if (c <= '9' && c >= '0')
				{
					num = (int)(c - '0');
				}
				else if (c <= 'F' && c >= 'A')
				{
					num = (int)(c - '7');
				}
				else
				{
					if (c > 'f' || c < 'a')
					{
						value = 0;
						return false;
					}
					num = (int)(c - 'W');
				}
				value += num << (end - 1 - i) * 4;
			}
			return true;
		}

		// Token: 0x040004B8 RID: 1208
		private static readonly Dictionary<Type, PrimitiveTypeCode> TypeCodeMap = new Dictionary<Type, PrimitiveTypeCode>
		{
			{
				typeof(char),
				PrimitiveTypeCode.Char
			},
			{
				typeof(char?),
				PrimitiveTypeCode.CharNullable
			},
			{
				typeof(bool),
				PrimitiveTypeCode.Boolean
			},
			{
				typeof(bool?),
				PrimitiveTypeCode.BooleanNullable
			},
			{
				typeof(sbyte),
				PrimitiveTypeCode.SByte
			},
			{
				typeof(sbyte?),
				PrimitiveTypeCode.SByteNullable
			},
			{
				typeof(short),
				PrimitiveTypeCode.Int16
			},
			{
				typeof(short?),
				PrimitiveTypeCode.Int16Nullable
			},
			{
				typeof(ushort),
				PrimitiveTypeCode.UInt16
			},
			{
				typeof(ushort?),
				PrimitiveTypeCode.UInt16Nullable
			},
			{
				typeof(int),
				PrimitiveTypeCode.Int32
			},
			{
				typeof(int?),
				PrimitiveTypeCode.Int32Nullable
			},
			{
				typeof(byte),
				PrimitiveTypeCode.Byte
			},
			{
				typeof(byte?),
				PrimitiveTypeCode.ByteNullable
			},
			{
				typeof(uint),
				PrimitiveTypeCode.UInt32
			},
			{
				typeof(uint?),
				PrimitiveTypeCode.UInt32Nullable
			},
			{
				typeof(long),
				PrimitiveTypeCode.Int64
			},
			{
				typeof(long?),
				PrimitiveTypeCode.Int64Nullable
			},
			{
				typeof(ulong),
				PrimitiveTypeCode.UInt64
			},
			{
				typeof(ulong?),
				PrimitiveTypeCode.UInt64Nullable
			},
			{
				typeof(float),
				PrimitiveTypeCode.Single
			},
			{
				typeof(float?),
				PrimitiveTypeCode.SingleNullable
			},
			{
				typeof(double),
				PrimitiveTypeCode.Double
			},
			{
				typeof(double?),
				PrimitiveTypeCode.DoubleNullable
			},
			{
				typeof(DateTime),
				PrimitiveTypeCode.DateTime
			},
			{
				typeof(DateTime?),
				PrimitiveTypeCode.DateTimeNullable
			},
			{
				typeof(DateTimeOffset),
				PrimitiveTypeCode.DateTimeOffset
			},
			{
				typeof(DateTimeOffset?),
				PrimitiveTypeCode.DateTimeOffsetNullable
			},
			{
				typeof(decimal),
				PrimitiveTypeCode.Decimal
			},
			{
				typeof(decimal?),
				PrimitiveTypeCode.DecimalNullable
			},
			{
				typeof(Guid),
				PrimitiveTypeCode.Guid
			},
			{
				typeof(Guid?),
				PrimitiveTypeCode.GuidNullable
			},
			{
				typeof(TimeSpan),
				PrimitiveTypeCode.TimeSpan
			},
			{
				typeof(TimeSpan?),
				PrimitiveTypeCode.TimeSpanNullable
			},
			{
				typeof(BigInteger),
				PrimitiveTypeCode.BigInteger
			},
			{
				typeof(BigInteger?),
				PrimitiveTypeCode.BigIntegerNullable
			},
			{
				typeof(Uri),
				PrimitiveTypeCode.Uri
			},
			{
				typeof(string),
				PrimitiveTypeCode.String
			},
			{
				typeof(byte[]),
				PrimitiveTypeCode.Bytes
			},
			{
				typeof(DBNull),
				PrimitiveTypeCode.DBNull
			}
		};

		// Token: 0x040004B9 RID: 1209
		private static readonly TypeInformation[] PrimitiveTypeCodes = new TypeInformation[]
		{
			new TypeInformation
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.Empty
			},
			new TypeInformation
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.Object
			},
			new TypeInformation
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.DBNull
			},
			new TypeInformation
			{
				Type = typeof(bool),
				TypeCode = PrimitiveTypeCode.Boolean
			},
			new TypeInformation
			{
				Type = typeof(char),
				TypeCode = PrimitiveTypeCode.Char
			},
			new TypeInformation
			{
				Type = typeof(sbyte),
				TypeCode = PrimitiveTypeCode.SByte
			},
			new TypeInformation
			{
				Type = typeof(byte),
				TypeCode = PrimitiveTypeCode.Byte
			},
			new TypeInformation
			{
				Type = typeof(short),
				TypeCode = PrimitiveTypeCode.Int16
			},
			new TypeInformation
			{
				Type = typeof(ushort),
				TypeCode = PrimitiveTypeCode.UInt16
			},
			new TypeInformation
			{
				Type = typeof(int),
				TypeCode = PrimitiveTypeCode.Int32
			},
			new TypeInformation
			{
				Type = typeof(uint),
				TypeCode = PrimitiveTypeCode.UInt32
			},
			new TypeInformation
			{
				Type = typeof(long),
				TypeCode = PrimitiveTypeCode.Int64
			},
			new TypeInformation
			{
				Type = typeof(ulong),
				TypeCode = PrimitiveTypeCode.UInt64
			},
			new TypeInformation
			{
				Type = typeof(float),
				TypeCode = PrimitiveTypeCode.Single
			},
			new TypeInformation
			{
				Type = typeof(double),
				TypeCode = PrimitiveTypeCode.Double
			},
			new TypeInformation
			{
				Type = typeof(decimal),
				TypeCode = PrimitiveTypeCode.Decimal
			},
			new TypeInformation
			{
				Type = typeof(DateTime),
				TypeCode = PrimitiveTypeCode.DateTime
			},
			new TypeInformation
			{
				Type = typeof(object),
				TypeCode = PrimitiveTypeCode.Empty
			},
			new TypeInformation
			{
				Type = typeof(string),
				TypeCode = PrimitiveTypeCode.String
			}
		};

		// Token: 0x040004BA RID: 1210
		private static readonly ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>> CastConverters = new ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>>(new Func<ConvertUtils.TypeConvertKey, Func<object, object>>(ConvertUtils.CreateCastConverter));

		// Token: 0x040004BB RID: 1211
		private static decimal[] _decimalFactors;

		// Token: 0x02000237 RID: 567
		internal struct TypeConvertKey : IEquatable<ConvertUtils.TypeConvertKey>
		{
			// Token: 0x1700040E RID: 1038
			// (get) Token: 0x0600168C RID: 5772 RVA: 0x000821A4 File Offset: 0x000803A4
			public Type InitialType
			{
				get
				{
					return this._initialType;
				}
			}

			// Token: 0x1700040F RID: 1039
			// (get) Token: 0x0600168D RID: 5773 RVA: 0x000821AC File Offset: 0x000803AC
			public Type TargetType
			{
				get
				{
					return this._targetType;
				}
			}

			// Token: 0x0600168E RID: 5774 RVA: 0x000821B4 File Offset: 0x000803B4
			public TypeConvertKey(Type initialType, Type targetType)
			{
				this._initialType = initialType;
				this._targetType = targetType;
			}

			// Token: 0x0600168F RID: 5775 RVA: 0x000821C4 File Offset: 0x000803C4
			public override int GetHashCode()
			{
				return this._initialType.GetHashCode() ^ this._targetType.GetHashCode();
			}

			// Token: 0x06001690 RID: 5776 RVA: 0x000821E0 File Offset: 0x000803E0
			public override bool Equals(object obj)
			{
				return obj is ConvertUtils.TypeConvertKey && this.Equals((ConvertUtils.TypeConvertKey)obj);
			}

			// Token: 0x06001691 RID: 5777 RVA: 0x000821FC File Offset: 0x000803FC
			public bool Equals(ConvertUtils.TypeConvertKey other)
			{
				return this._initialType == other._initialType && this._targetType == other._targetType;
			}

			// Token: 0x04000A9F RID: 2719
			private readonly Type _initialType;

			// Token: 0x04000AA0 RID: 2720
			private readonly Type _targetType;
		}

		// Token: 0x02000238 RID: 568
		internal enum ConvertResult
		{
			// Token: 0x04000AA2 RID: 2722
			Success,
			// Token: 0x04000AA3 RID: 2723
			CannotConvertNull,
			// Token: 0x04000AA4 RID: 2724
			NotInstantiableType,
			// Token: 0x04000AA5 RID: 2725
			NoValidConversion
		}
	}
}
