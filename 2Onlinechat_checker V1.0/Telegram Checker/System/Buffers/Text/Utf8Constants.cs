using System;

namespace System.Buffers.Text
{
	// Token: 0x02000048 RID: 72
	internal static class Utf8Constants
	{
		// Token: 0x04000148 RID: 328
		public const byte Colon = 58;

		// Token: 0x04000149 RID: 329
		public const byte Comma = 44;

		// Token: 0x0400014A RID: 330
		public const byte Minus = 45;

		// Token: 0x0400014B RID: 331
		public const byte Period = 46;

		// Token: 0x0400014C RID: 332
		public const byte Plus = 43;

		// Token: 0x0400014D RID: 333
		public const byte Slash = 47;

		// Token: 0x0400014E RID: 334
		public const byte Space = 32;

		// Token: 0x0400014F RID: 335
		public const byte Hyphen = 45;

		// Token: 0x04000150 RID: 336
		public const byte Separator = 44;

		// Token: 0x04000151 RID: 337
		public const int GroupSize = 3;

		// Token: 0x04000152 RID: 338
		public static readonly TimeSpan s_nullUtcOffset = TimeSpan.MinValue;

		// Token: 0x04000153 RID: 339
		public const int DateTimeMaxUtcOffsetHours = 14;

		// Token: 0x04000154 RID: 340
		public const int DateTimeNumFractionDigits = 7;

		// Token: 0x04000155 RID: 341
		public const int MaxDateTimeFraction = 9999999;

		// Token: 0x04000156 RID: 342
		public const ulong BillionMaxUIntValue = 4294967295000000000UL;

		// Token: 0x04000157 RID: 343
		public const uint Billion = 1000000000U;
	}
}
