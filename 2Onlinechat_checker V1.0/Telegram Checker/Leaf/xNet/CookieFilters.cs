using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x0200005B RID: 91
	[ComVisible(true)]
	public static class CookieFilters
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x0001B410 File Offset: 0x00019610
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x0001B418 File Offset: 0x00019618
		public static bool Enabled { get; set; } = true;

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x0001B420 File Offset: 0x00019620
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x0001B428 File Offset: 0x00019628
		public static bool Trim { get; set; } = true;

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x0001B430 File Offset: 0x00019630
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x0001B438 File Offset: 0x00019638
		public static bool Path { get; set; } = true;

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x0001B440 File Offset: 0x00019640
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x0001B448 File Offset: 0x00019648
		public static bool CommaEndingValue { get; set; } = true;

		// Token: 0x06000442 RID: 1090 RVA: 0x0001B450 File Offset: 0x00019650
		public static string Filter(string rawCookie)
		{
			if (CookieFilters.Enabled)
			{
				return rawCookie.TrimWhitespace().FilterPath().FilterInvalidExpireYear()
					.FilterCommaEndingValue();
			}
			return rawCookie;
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x0001B474 File Offset: 0x00019674
		public static string FilterDomain(string domain)
		{
			if (string.IsNullOrWhiteSpace(domain))
			{
				return null;
			}
			domain = domain.Trim(new char[] { '\t', '\n', '\r', ' ' });
			bool flag = domain.Length > 1 && domain[0] == '.';
			bool flag2 = domain.IndexOf('.', 1) == -1;
			if (!flag || !flag2)
			{
				return domain;
			}
			return domain.Substring(1);
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0001B4E8 File Offset: 0x000196E8
		private static string TrimWhitespace(this string rawCookie)
		{
			if (CookieFilters.Trim)
			{
				return rawCookie.Trim();
			}
			return rawCookie;
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x0001B4FC File Offset: 0x000196FC
		private static string FilterPath(this string rawCookie)
		{
			if (!CookieFilters.Path)
			{
				return rawCookie;
			}
			int num = rawCookie.IndexOf("path=/", 0, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return rawCookie;
			}
			num += "path=/".Length;
			if (num >= rawCookie.Length - 1 || rawCookie[num] == ';')
			{
				return rawCookie;
			}
			int num2 = rawCookie.IndexOf(';', num);
			if (num2 == -1)
			{
				num2 = rawCookie.Length;
			}
			return rawCookie.Remove(num, num2 - num);
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x0001B580 File Offset: 0x00019780
		private static string FilterCommaEndingValue(this string rawCookie)
		{
			if (!CookieFilters.CommaEndingValue)
			{
				return rawCookie;
			}
			int num = rawCookie.IndexOf('=');
			if (num == -1 || num >= rawCookie.Length - 1)
			{
				return rawCookie;
			}
			int num2 = rawCookie.IndexOf(';', num + 1);
			if (num2 == -1)
			{
				num2 = rawCookie.Length - 1;
			}
			int num3 = num2 - 1;
			if (rawCookie[num3] == ',')
			{
				return rawCookie.Remove(num3, 1).Insert(num3, "%2C");
			}
			return rawCookie;
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x0001B600 File Offset: 0x00019800
		private static string FilterInvalidExpireYear(this string rawCookie)
		{
			int num = rawCookie.IndexOf("expires=", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return rawCookie;
			}
			num += "expires=".Length;
			int num2 = rawCookie.IndexOf(';', num);
			if (num2 == -1)
			{
				num2 = rawCookie.Length;
			}
			int num3 = rawCookie.Substring(num, num2 - num).IndexOf("9999", StringComparison.Ordinal);
			if (num3 == -1)
			{
				return rawCookie;
			}
			num3 += num + "9999".Length - 1;
			return rawCookie.Remove(num3, 1).Insert(num3, "8");
		}
	}
}
