using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000DE RID: 222
	internal static class EnumUtils
	{
		// Token: 0x06000C44 RID: 3140 RVA: 0x0004E034 File Offset: 0x0004C234
		private static BidirectionalDictionary<string, string> InitializeEnumType(Type type)
		{
			BidirectionalDictionary<string, string> bidirectionalDictionary = new BidirectionalDictionary<string, string>(StringComparer.Ordinal, StringComparer.Ordinal);
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				string name = fieldInfo.Name;
				string text = (from EnumMemberAttribute a in fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)
					select a.Value).SingleOrDefault<string>() ?? fieldInfo.Name;
				string text2;
				if (bidirectionalDictionary.TryGetBySecond(text, out text2))
				{
					throw new InvalidOperationException("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, text, type.Name));
				}
				bidirectionalDictionary.Set(name, text);
			}
			return bidirectionalDictionary;
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x0004E10C File Offset: 0x0004C30C
		public static IList<T> GetFlagsValues<T>(T value) where T : struct
		{
			Type typeFromHandle = typeof(T);
			if (!typeFromHandle.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("Enum type {0} is not a set of flags.".FormatWith(CultureInfo.InvariantCulture, typeFromHandle));
			}
			Type underlyingType = Enum.GetUnderlyingType(value.GetType());
			ulong num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			IList<EnumValue<ulong>> namesAndValues = EnumUtils.GetNamesAndValues<T>();
			IList<T> list = new List<T>();
			foreach (EnumValue<ulong> enumValue in namesAndValues)
			{
				if ((num & enumValue.Value) == enumValue.Value && enumValue.Value != 0UL)
				{
					list.Add((T)((object)Convert.ChangeType(enumValue.Value, underlyingType, CultureInfo.CurrentCulture)));
				}
			}
			if (list.Count == 0)
			{
				if (namesAndValues.SingleOrDefault((EnumValue<ulong> v) => v.Value == 0UL) != null)
				{
					list.Add(default(T));
				}
			}
			return list;
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x0004E254 File Offset: 0x0004C454
		public static IList<EnumValue<ulong>> GetNamesAndValues<T>() where T : struct
		{
			return EnumUtils.GetNamesAndValues<ulong>(typeof(T));
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x0004E268 File Offset: 0x0004C468
		public static IList<EnumValue<TUnderlyingType>> GetNamesAndValues<TUnderlyingType>(Type enumType) where TUnderlyingType : struct
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (!enumType.IsEnum())
			{
				throw new ArgumentException("Type {0} is not an enum.".FormatWith(CultureInfo.InvariantCulture, enumType.Name), "enumType");
			}
			IList<object> values = EnumUtils.GetValues(enumType);
			IList<string> names = EnumUtils.GetNames(enumType);
			IList<EnumValue<TUnderlyingType>> list = new List<EnumValue<TUnderlyingType>>();
			for (int i = 0; i < values.Count; i++)
			{
				try
				{
					list.Add(new EnumValue<TUnderlyingType>(names[i], (TUnderlyingType)((object)Convert.ChangeType(values[i], typeof(TUnderlyingType), CultureInfo.CurrentCulture))));
				}
				catch (OverflowException ex)
				{
					throw new InvalidOperationException("Value from enum with the underlying type of {0} cannot be added to dictionary with a value type of {1}. Value was too large: {2}".FormatWith(CultureInfo.InvariantCulture, Enum.GetUnderlyingType(enumType), typeof(TUnderlyingType), Convert.ToUInt64(values[i], CultureInfo.InvariantCulture)), ex);
				}
			}
			return list;
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x0004E36C File Offset: 0x0004C56C
		public static IList<object> GetValues(Type enumType)
		{
			if (!enumType.IsEnum())
			{
				throw new ArgumentException("Type {0} is not an enum.".FormatWith(CultureInfo.InvariantCulture, enumType.Name), "enumType");
			}
			List<object> list = new List<object>();
			FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
			for (int i = 0; i < fields.Length; i++)
			{
				object value = fields[i].GetValue(enumType);
				list.Add(value);
			}
			return list;
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x0004E3E0 File Offset: 0x0004C5E0
		public static IList<string> GetNames(Type enumType)
		{
			if (!enumType.IsEnum())
			{
				throw new ArgumentException("Type {0} is not an enum.".FormatWith(CultureInfo.InvariantCulture, enumType.Name), "enumType");
			}
			List<string> list = new List<string>();
			foreach (FieldInfo fieldInfo in enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				list.Add(fieldInfo.Name);
			}
			return list;
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x0004E454 File Offset: 0x0004C654
		public static object ParseEnumName(string enumText, bool isNullable, bool disallowValue, Type t)
		{
			if (enumText == string.Empty && isNullable)
			{
				return null;
			}
			BidirectionalDictionary<string, string> bidirectionalDictionary = EnumUtils.EnumMemberNamesPerType.Get(t);
			string text;
			string text2;
			if (EnumUtils.TryResolvedEnumName(bidirectionalDictionary, enumText, out text))
			{
				text2 = text;
			}
			else if (enumText.IndexOf(',') != -1)
			{
				string[] array = enumText.Split(new char[] { ',' });
				for (int i = 0; i < array.Length; i++)
				{
					string text3 = array[i].Trim();
					array[i] = (EnumUtils.TryResolvedEnumName(bidirectionalDictionary, text3, out text) ? text : text3);
				}
				text2 = string.Join(", ", array);
			}
			else
			{
				text2 = enumText;
				int num;
				if (disallowValue && int.TryParse(text2, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out num))
				{
					throw new FormatException("Integer string '{0}' is not allowed.".FormatWith(CultureInfo.InvariantCulture, enumText));
				}
			}
			return Enum.Parse(t, text2, true);
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x0004E544 File Offset: 0x0004C744
		public static string ToEnumName(Type enumType, string enumText, bool camelCaseText)
		{
			BidirectionalDictionary<string, string> bidirectionalDictionary = EnumUtils.EnumMemberNamesPerType.Get(enumType);
			string[] array = enumText.Split(new char[] { ',' });
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				string text2;
				bidirectionalDictionary.TryGetByFirst(text, out text2);
				text2 = text2 ?? text;
				if (camelCaseText)
				{
					text2 = StringUtils.ToCamelCase(text2);
				}
				array[i] = text2;
			}
			return string.Join(", ", array);
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x0004E5CC File Offset: 0x0004C7CC
		private static bool TryResolvedEnumName(BidirectionalDictionary<string, string> map, string enumText, out string resolvedEnumName)
		{
			if (map.TryGetBySecond(enumText, out resolvedEnumName))
			{
				return true;
			}
			resolvedEnumName = null;
			return false;
		}

		// Token: 0x040004EE RID: 1262
		private static readonly ThreadSafeStore<Type, BidirectionalDictionary<string, string>> EnumMemberNamesPerType = new ThreadSafeStore<Type, BidirectionalDictionary<string, string>>(new Func<Type, BidirectionalDictionary<string, string>>(EnumUtils.InitializeEnumType));
	}
}
