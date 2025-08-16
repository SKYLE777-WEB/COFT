using System;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
	// Token: 0x02000091 RID: 145
	internal class ConstantHelper
	{
		// Token: 0x060006C7 RID: 1735 RVA: 0x00024AAC File Offset: 0x00022CAC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte GetByteWithAllBitsSet()
		{
			byte b = 0;
			*(&b) = byte.MaxValue;
			return b;
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00024ACC File Offset: 0x00022CCC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static sbyte GetSByteWithAllBitsSet()
		{
			sbyte b = 0;
			*(&b) = -1;
			return b;
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x00024AE8 File Offset: 0x00022CE8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ushort GetUInt16WithAllBitsSet()
		{
			ushort num = 0;
			*(&num) = ushort.MaxValue;
			return num;
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x00024B08 File Offset: 0x00022D08
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static short GetInt16WithAllBitsSet()
		{
			short num = 0;
			*(&num) = -1;
			return num;
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00024B24 File Offset: 0x00022D24
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint GetUInt32WithAllBitsSet()
		{
			uint num = 0U;
			*(&num) = uint.MaxValue;
			return num;
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00024B40 File Offset: 0x00022D40
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetInt32WithAllBitsSet()
		{
			int num = 0;
			*(&num) = -1;
			return num;
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x00024B5C File Offset: 0x00022D5C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ulong GetUInt64WithAllBitsSet()
		{
			ulong num = 0UL;
			*(&num) = ulong.MaxValue;
			return num;
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x00024B78 File Offset: 0x00022D78
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static long GetInt64WithAllBitsSet()
		{
			long num = 0L;
			*(&num) = -1L;
			return num;
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00024B94 File Offset: 0x00022D94
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float GetSingleWithAllBitsSet()
		{
			float num = 0f;
			*(int*)(&num) = -1;
			return num;
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x00024BB4 File Offset: 0x00022DB4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static double GetDoubleWithAllBitsSet()
		{
			double num = 0.0;
			*(long*)(&num) = -1L;
			return num;
		}
	}
}
