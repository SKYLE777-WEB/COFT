using System;

namespace System
{
	// Token: 0x02000026 RID: 38
	internal static class DecimalDecCalc
	{
		// Token: 0x0600013E RID: 318 RVA: 0x00008390 File Offset: 0x00006590
		private static uint D32DivMod1E9(uint hi32, ref uint lo32)
		{
			ulong num = ((ulong)hi32 << 32) | (ulong)lo32;
			lo32 = (uint)(num / 1000000000UL);
			return (uint)(num % 1000000000UL);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000083C0 File Offset: 0x000065C0
		internal static uint DecDivMod1E9(ref MutableDecimal value)
		{
			return DecimalDecCalc.D32DivMod1E9(DecimalDecCalc.D32DivMod1E9(DecimalDecCalc.D32DivMod1E9(0U, ref value.High), ref value.Mid), ref value.Low);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000083E4 File Offset: 0x000065E4
		internal static void DecAddInt32(ref MutableDecimal value, uint i)
		{
			if (DecimalDecCalc.D32AddCarry(ref value.Low, i) && DecimalDecCalc.D32AddCarry(ref value.Mid, 1U))
			{
				DecimalDecCalc.D32AddCarry(ref value.High, 1U);
			}
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00008418 File Offset: 0x00006618
		private static bool D32AddCarry(ref uint value, uint i)
		{
			uint num = value;
			uint num2 = num + i;
			value = num2;
			return num2 < num || num2 < i;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00008440 File Offset: 0x00006640
		internal static void DecMul10(ref MutableDecimal value)
		{
			MutableDecimal mutableDecimal = value;
			DecimalDecCalc.DecShiftLeft(ref value);
			DecimalDecCalc.DecShiftLeft(ref value);
			DecimalDecCalc.DecAdd(ref value, mutableDecimal);
			DecimalDecCalc.DecShiftLeft(ref value);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00008474 File Offset: 0x00006674
		private static void DecShiftLeft(ref MutableDecimal value)
		{
			uint num = (((value.Low & 2147483648U) != 0U) ? 1U : 0U);
			uint num2 = (((value.Mid & 2147483648U) != 0U) ? 1U : 0U);
			value.Low <<= 1;
			value.Mid = (value.Mid << 1) | num;
			value.High = (value.High << 1) | num2;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000084E8 File Offset: 0x000066E8
		private static void DecAdd(ref MutableDecimal value, MutableDecimal d)
		{
			if (DecimalDecCalc.D32AddCarry(ref value.Low, d.Low) && DecimalDecCalc.D32AddCarry(ref value.Mid, 1U))
			{
				DecimalDecCalc.D32AddCarry(ref value.High, 1U);
			}
			if (DecimalDecCalc.D32AddCarry(ref value.Mid, d.Mid))
			{
				DecimalDecCalc.D32AddCarry(ref value.High, 1U);
			}
			DecimalDecCalc.D32AddCarry(ref value.High, d.High);
		}
	}
}
