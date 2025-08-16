using System;
using System.Runtime.CompilerServices;

namespace System
{
	// Token: 0x0200008E RID: 142
	internal static class MathF
	{
		// Token: 0x060006B0 RID: 1712 RVA: 0x000248A4 File Offset: 0x00022AA4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Abs(float x)
		{
			return Math.Abs(x);
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x000248AC File Offset: 0x00022AAC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Acos(float x)
		{
			return (float)Math.Acos((double)x);
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x000248B8 File Offset: 0x00022AB8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cos(float x)
		{
			return (float)Math.Cos((double)x);
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x000248C4 File Offset: 0x00022AC4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float IEEERemainder(float x, float y)
		{
			return (float)Math.IEEERemainder((double)x, (double)y);
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x000248D0 File Offset: 0x00022AD0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Pow(float x, float y)
		{
			return (float)Math.Pow((double)x, (double)y);
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x000248DC File Offset: 0x00022ADC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sin(float x)
		{
			return (float)Math.Sin((double)x);
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x000248E8 File Offset: 0x00022AE8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt((double)x);
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x000248F4 File Offset: 0x00022AF4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Tan(float x)
		{
			return (float)Math.Tan((double)x);
		}

		// Token: 0x040002F1 RID: 753
		public const float PI = 3.1415927f;
	}
}
