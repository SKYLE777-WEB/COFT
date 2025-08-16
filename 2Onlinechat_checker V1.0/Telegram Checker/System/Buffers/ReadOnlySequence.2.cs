using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	// Token: 0x0200003F RID: 63
	internal static class ReadOnlySequence
	{
		// Token: 0x060002BD RID: 701 RVA: 0x00012280 File Offset: 0x00010480
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SegmentToSequenceStart(int startIndex)
		{
			return startIndex | 0;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00012288 File Offset: 0x00010488
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SegmentToSequenceEnd(int endIndex)
		{
			return endIndex | 0;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x00012290 File Offset: 0x00010490
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ArrayToSequenceStart(int startIndex)
		{
			return startIndex | 0;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00012298 File Offset: 0x00010498
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ArrayToSequenceEnd(int endIndex)
		{
			return endIndex | int.MinValue;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x000122A4 File Offset: 0x000104A4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MemoryManagerToSequenceStart(int startIndex)
		{
			return startIndex | int.MinValue;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x000122B0 File Offset: 0x000104B0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MemoryManagerToSequenceEnd(int endIndex)
		{
			return endIndex | 0;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x000122B8 File Offset: 0x000104B8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int StringToSequenceStart(int startIndex)
		{
			return startIndex | int.MinValue;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x000122C4 File Offset: 0x000104C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int StringToSequenceEnd(int endIndex)
		{
			return endIndex | int.MinValue;
		}

		// Token: 0x0400012E RID: 302
		public const int FlagBitMask = -2147483648;

		// Token: 0x0400012F RID: 303
		public const int IndexBitMask = 2147483647;

		// Token: 0x04000130 RID: 304
		public const int SegmentStartMask = 0;

		// Token: 0x04000131 RID: 305
		public const int SegmentEndMask = 0;

		// Token: 0x04000132 RID: 306
		public const int ArrayStartMask = 0;

		// Token: 0x04000133 RID: 307
		public const int ArrayEndMask = -2147483648;

		// Token: 0x04000134 RID: 308
		public const int MemoryManagerStartMask = -2147483648;

		// Token: 0x04000135 RID: 309
		public const int MemoryManagerEndMask = 0;

		// Token: 0x04000136 RID: 310
		public const int StringStartMask = -2147483648;

		// Token: 0x04000137 RID: 311
		public const int StringEndMask = -2147483648;
	}
}
