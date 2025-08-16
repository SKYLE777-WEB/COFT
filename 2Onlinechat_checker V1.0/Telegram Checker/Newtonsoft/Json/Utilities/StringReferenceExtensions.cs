using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F5 RID: 245
	internal static class StringReferenceExtensions
	{
		// Token: 0x06000D18 RID: 3352 RVA: 0x0005225C File Offset: 0x0005045C
		public static int IndexOf(this StringReference s, char c, int startIndex, int length)
		{
			int num = Array.IndexOf<char>(s.Chars, c, s.StartIndex + startIndex, length);
			if (num == -1)
			{
				return -1;
			}
			return num - s.StartIndex;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x00052298 File Offset: 0x00050498
		public static bool StartsWith(this StringReference s, string text)
		{
			if (text.Length > s.Length)
			{
				return false;
			}
			char[] chars = s.Chars;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != chars[i + s.StartIndex])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x000522F4 File Offset: 0x000504F4
		public static bool EndsWith(this StringReference s, string text)
		{
			if (text.Length > s.Length)
			{
				return false;
			}
			char[] chars = s.Chars;
			int num = s.StartIndex + s.Length - text.Length;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != chars[i + num])
				{
					return false;
				}
			}
			return true;
		}
	}
}
