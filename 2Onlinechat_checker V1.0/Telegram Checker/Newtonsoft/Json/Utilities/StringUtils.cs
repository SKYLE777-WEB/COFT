using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F6 RID: 246
	internal static class StringUtils
	{
		// Token: 0x06000D1B RID: 3355 RVA: 0x00052360 File Offset: 0x00050560
		public static string FormatWith(this string format, IFormatProvider provider, object arg0)
		{
			return format.FormatWith(provider, new object[] { arg0 });
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x00052374 File Offset: 0x00050574
		public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1)
		{
			return format.FormatWith(provider, new object[] { arg0, arg1 });
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0005238C File Offset: 0x0005058C
		public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2)
		{
			return format.FormatWith(provider, new object[] { arg0, arg1, arg2 });
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x000523A8 File Offset: 0x000505A8
		public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2, object arg3)
		{
			return format.FormatWith(provider, new object[] { arg0, arg1, arg2, arg3 });
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x000523CC File Offset: 0x000505CC
		private static string FormatWith(this string format, IFormatProvider provider, params object[] args)
		{
			ValidationUtils.ArgumentNotNull(format, "format");
			return string.Format(provider, format, args);
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x000523E4 File Offset: 0x000505E4
		public static bool IsWhiteSpace(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsWhiteSpace(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0005243C File Offset: 0x0005063C
		public static StringWriter CreateStringWriter(int capacity)
		{
			return new StringWriter(new StringBuilder(capacity), CultureInfo.InvariantCulture);
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x00052450 File Offset: 0x00050650
		public static void ToCharAsUnicode(char c, char[] buffer)
		{
			buffer[0] = '\\';
			buffer[1] = 'u';
			buffer[2] = MathUtils.IntToHex((int)((c >> 12) & '\u000f'));
			buffer[3] = MathUtils.IntToHex((int)((c >> 8) & '\u000f'));
			buffer[4] = MathUtils.IntToHex((int)((c >> 4) & '\u000f'));
			buffer[5] = MathUtils.IntToHex((int)(c & '\u000f'));
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x000524A4 File Offset: 0x000506A4
		public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}
			IEnumerable<TSource> enumerable = source.Where((TSource s) => string.Equals(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase));
			if (enumerable.Count<TSource>() <= 1)
			{
				return enumerable.SingleOrDefault<TSource>();
			}
			return source.Where((TSource s) => string.Equals(valueSelector(s), testValue, StringComparison.Ordinal)).SingleOrDefault<TSource>();
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x00052530 File Offset: 0x00050730
		public static string ToCamelCase(string s)
		{
			if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
			{
				return s;
			}
			char[] array = s.ToCharArray();
			int num = 0;
			while (num < array.Length && (num != 1 || char.IsUpper(array[num])))
			{
				bool flag = num + 1 < array.Length;
				if (num > 0 && flag && !char.IsUpper(array[num + 1]))
				{
					break;
				}
				char c = char.ToLower(array[num], CultureInfo.InvariantCulture);
				array[num] = c;
				num++;
			}
			return new string(array);
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x000525C8 File Offset: 0x000507C8
		public static string ToSnakeCase(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringUtils.SnakeCaseState snakeCaseState = StringUtils.SnakeCaseState.Start;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == ' ')
				{
					if (snakeCaseState != StringUtils.SnakeCaseState.Start)
					{
						snakeCaseState = StringUtils.SnakeCaseState.NewWord;
					}
				}
				else if (char.IsUpper(s[i]))
				{
					switch (snakeCaseState)
					{
					case StringUtils.SnakeCaseState.Lower:
					case StringUtils.SnakeCaseState.NewWord:
						stringBuilder.Append('_');
						break;
					case StringUtils.SnakeCaseState.Upper:
					{
						bool flag = i + 1 < s.Length;
						if (i > 0 && flag)
						{
							char c = s[i + 1];
							if (!char.IsUpper(c) && c != '_')
							{
								stringBuilder.Append('_');
							}
						}
						break;
					}
					}
					char c2 = char.ToLower(s[i], CultureInfo.InvariantCulture);
					stringBuilder.Append(c2);
					snakeCaseState = StringUtils.SnakeCaseState.Upper;
				}
				else if (s[i] == '_')
				{
					stringBuilder.Append('_');
					snakeCaseState = StringUtils.SnakeCaseState.Start;
				}
				else
				{
					if (snakeCaseState == StringUtils.SnakeCaseState.NewWord)
					{
						stringBuilder.Append('_');
					}
					stringBuilder.Append(s[i]);
					snakeCaseState = StringUtils.SnakeCaseState.Lower;
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x00052700 File Offset: 0x00050900
		public static bool IsHighSurrogate(char c)
		{
			return char.IsHighSurrogate(c);
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x00052708 File Offset: 0x00050908
		public static bool IsLowSurrogate(char c)
		{
			return char.IsLowSurrogate(c);
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x00052710 File Offset: 0x00050910
		public static bool StartsWith(this string source, char value)
		{
			return source.Length > 0 && source[0] == value;
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x0005272C File Offset: 0x0005092C
		public static bool EndsWith(this string source, char value)
		{
			return source.Length > 0 && source[source.Length - 1] == value;
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x00052750 File Offset: 0x00050950
		public static string Trim(this string s, int start, int length)
		{
			if (s == null)
			{
				throw new ArgumentNullException();
			}
			if (start < 0)
			{
				throw new ArgumentOutOfRangeException("start");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = start + length - 1;
			if (num >= s.Length)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			while (start < num)
			{
				if (!char.IsWhiteSpace(s[start]))
				{
					IL_0081:
					while (num >= start && char.IsWhiteSpace(s[num]))
					{
						num--;
					}
					return s.Substring(start, num - start + 1);
				}
				start++;
			}
			goto IL_0081;
		}

		// Token: 0x04000533 RID: 1331
		public const string CarriageReturnLineFeed = "\r\n";

		// Token: 0x04000534 RID: 1332
		public const string Empty = "";

		// Token: 0x04000535 RID: 1333
		public const char CarriageReturn = '\r';

		// Token: 0x04000536 RID: 1334
		public const char LineFeed = '\n';

		// Token: 0x04000537 RID: 1335
		public const char Tab = '\t';

		// Token: 0x0200026A RID: 618
		internal enum SnakeCaseState
		{
			// Token: 0x04000B29 RID: 2857
			Start,
			// Token: 0x04000B2A RID: 2858
			Lower,
			// Token: 0x04000B2B RID: 2859
			Upper,
			// Token: 0x04000B2C RID: 2860
			NewWord
		}
	}
}
