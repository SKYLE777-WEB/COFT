using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x02000059 RID: 89
	[ComVisible(true)]
	public static class StringExtensions
	{
		// Token: 0x06000426 RID: 1062 RVA: 0x0001AF7C File Offset: 0x0001917C
		public static string[] SubstringsOrEmpty(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
		{
			if (string.IsNullOrEmpty(self))
			{
				return new string[0];
			}
			if (string.IsNullOrEmpty(left))
			{
				throw new ArgumentNullException("left");
			}
			if (string.IsNullOrEmpty(right))
			{
				throw new ArgumentNullException("right");
			}
			if (startIndex < 0 || startIndex >= self.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			int num = startIndex;
			int num2 = limit;
			List<string> list = new List<string>();
			for (;;)
			{
				if (limit > 0)
				{
					num2--;
					if (num2 < 0)
					{
						break;
					}
				}
				int num3 = self.IndexOf(left, num, comparison);
				if (num3 == -1)
				{
					break;
				}
				int num4 = num3 + left.Length;
				int num5 = self.IndexOf(right, num4, comparison);
				if (num5 == -1)
				{
					break;
				}
				int num6 = num5 - num4;
				list.Add(self.Substring(num4, num6));
				num = num5 + right.Length;
			}
			return list.ToArray();
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0001B064 File Offset: 0x00019264
		public static string[] Substrings(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0, string[] fallback = null)
		{
			string[] array = self.SubstringsOrEmpty(left, right, startIndex, comparison, limit);
			if (array.Length == 0)
			{
				return fallback;
			}
			return array;
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x0001B090 File Offset: 0x00019290
		public static string[] SubstringsEx(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
		{
			string[] array = self.SubstringsOrEmpty(left, right, startIndex, comparison, limit);
			if (array.Length == 0)
			{
				throw new SubstringException(string.Concat(new string[] { "Substrings not found. Left: \"", left, "\". Right: \"", right, "\"." }));
			}
			return array;
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0001B0E8 File Offset: 0x000192E8
		public static string Substring(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string fallback = null)
		{
			if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) || startIndex < 0 || startIndex >= self.Length)
			{
				return fallback;
			}
			int num = self.IndexOf(left, startIndex, comparison);
			if (num == -1)
			{
				return fallback;
			}
			int num2 = num + left.Length;
			int num3 = self.IndexOf(right, num2, comparison);
			if (num3 == -1)
			{
				return fallback;
			}
			return self.Substring(num2, num3 - num2);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x0001B170 File Offset: 0x00019370
		public static string SubstringOrEmpty(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
		{
			return self.Substring(left, right, startIndex, comparison, string.Empty);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x0001B184 File Offset: 0x00019384
		public static string SubstringEx(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
		{
			string text = self.Substring(left, right, startIndex, comparison, null);
			if (text == null)
			{
				throw new SubstringException(string.Concat(new string[] { "Substring not found. Left: \"", left, "\". Right: \"", right, "\"." }));
			}
			return text;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0001B1DC File Offset: 0x000193DC
		public static string SubstringLast(this string self, string right, string left, int startIndex = -1, StringComparison comparison = StringComparison.Ordinal, string notFoundValue = null)
		{
			if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left) || startIndex < -1 || startIndex >= self.Length)
			{
				return notFoundValue;
			}
			if (startIndex == -1)
			{
				startIndex = self.Length - 1;
			}
			int num = self.LastIndexOf(right, startIndex, comparison);
			if (num == -1 || num == 0)
			{
				return notFoundValue;
			}
			int num2 = self.LastIndexOf(left, num - 1, comparison);
			if (num2 == -1 || num - num2 == 1)
			{
				return notFoundValue;
			}
			int num3 = num2 + left.Length;
			return self.Substring(num3, num - num3);
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0001B284 File Offset: 0x00019484
		public static string SubstringLastOrEmpty(this string self, string right, string left, int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
		{
			return self.SubstringLast(right, left, startIndex, comparison, string.Empty);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001B298 File Offset: 0x00019498
		public static string SubstringLastEx(this string self, string right, string left, int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
		{
			string text = self.SubstringLast(right, left, startIndex, comparison, null);
			if (text == null)
			{
				throw new SubstringException(string.Concat(new string[] { "StringBetween not found. Right: \"", right, "\". Left: \"", left, "\"." }));
			}
			return text;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x0001B2F0 File Offset: 0x000194F0
		public static bool ContainsInsensitive(this string self, string value)
		{
			return self.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}
