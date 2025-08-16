using System;
using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
	// Token: 0x02000049 RID: 73
	internal static class FormattingHelpers
	{
		// Token: 0x060002FA RID: 762 RVA: 0x00012FA8 File Offset: 0x000111A8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char GetSymbolOrDefault(in StandardFormat format, char defaultSymbol)
		{
			char c = format.Symbol;
			if (c == '\0' && format.Precision == 0)
			{
				c = defaultSymbol;
			}
			return c;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00012FD4 File Offset: 0x000111D4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void FillWithAsciiZeros(Span<byte> buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				*buffer[i] = 48;
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00013008 File Offset: 0x00011208
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteHexByte(byte value, Span<byte> buffer, int startingIndex = 0, FormattingHelpers.HexCasing casing = FormattingHelpers.HexCasing.Uppercase)
		{
			uint num = (uint)(((int)(value & 240) << 4) + (int)(value & 15) - 35209);
			uint num2 = (uint)((((-num & 28784U) >> 4) + num + (FormattingHelpers.HexCasing)47545U) | casing);
			*buffer[startingIndex + 1] = (byte)num2;
			*buffer[startingIndex] = (byte)(num2 >> 8);
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00013060 File Offset: 0x00011260
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteDigits(ulong value, Span<byte> buffer)
		{
			for (int i = buffer.Length - 1; i >= 1; i--)
			{
				ulong num = 48UL + value;
				value /= 10UL;
				*buffer[i] = (byte)(num - value * 10UL);
			}
			*buffer[0] = (byte)(48UL + value);
		}

		// Token: 0x060002FE RID: 766 RVA: 0x000130B8 File Offset: 0x000112B8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteDigitsWithGroupSeparator(ulong value, Span<byte> buffer)
		{
			int num = 0;
			for (int i = buffer.Length - 1; i >= 1; i--)
			{
				ulong num2 = 48UL + value;
				value /= 10UL;
				*buffer[i] = (byte)(num2 - value * 10UL);
				if (num == 2)
				{
					*buffer[--i] = 44;
					num = 0;
				}
				else
				{
					num++;
				}
			}
			*buffer[0] = (byte)(48UL + value);
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00013130 File Offset: 0x00011330
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteDigits(uint value, Span<byte> buffer)
		{
			for (int i = buffer.Length - 1; i >= 1; i--)
			{
				uint num = 48U + value;
				value /= 10U;
				*buffer[i] = (byte)(num - value * 10U);
			}
			*buffer[0] = (byte)(48U + value);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00013184 File Offset: 0x00011384
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteFourDecimalDigits(uint value, Span<byte> buffer, int startingIndex = 0)
		{
			uint num = 48U + value;
			value /= 10U;
			*buffer[startingIndex + 3] = (byte)(num - value * 10U);
			num = 48U + value;
			value /= 10U;
			*buffer[startingIndex + 2] = (byte)(num - value * 10U);
			num = 48U + value;
			value /= 10U;
			*buffer[startingIndex + 1] = (byte)(num - value * 10U);
			*buffer[startingIndex] = (byte)(48U + value);
		}

		// Token: 0x06000301 RID: 769 RVA: 0x000131FC File Offset: 0x000113FC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteTwoDecimalDigits(uint value, Span<byte> buffer, int startingIndex = 0)
		{
			uint num = 48U + value;
			value /= 10U;
			*buffer[startingIndex + 1] = (byte)(num - value * 10U);
			*buffer[startingIndex] = (byte)(48U + value);
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00013238 File Offset: 0x00011438
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong DivMod(ulong numerator, ulong denominator, out ulong modulo)
		{
			ulong num = numerator / denominator;
			modulo = numerator - num * denominator;
			return num;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00013258 File Offset: 0x00011458
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint DivMod(uint numerator, uint denominator, out uint modulo)
		{
			uint num = numerator / denominator;
			modulo = numerator - num * denominator;
			return num;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00013278 File Offset: 0x00011478
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountDecimalTrailingZeros(uint value, out uint valueWithoutTrailingZeros)
		{
			int num = 0;
			if (value != 0U)
			{
				for (;;)
				{
					uint num3;
					uint num2 = FormattingHelpers.DivMod(value, 10U, out num3);
					if (num3 != 0U)
					{
						break;
					}
					value = num2;
					num++;
				}
			}
			valueWithoutTrailingZeros = value;
			return num;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x000132B0 File Offset: 0x000114B0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountDigits(ulong value)
		{
			int num = 1;
			uint num2;
			if (value >= 10000000UL)
			{
				if (value >= 100000000000000UL)
				{
					num2 = (uint)(value / 100000000000000UL);
					num += 14;
				}
				else
				{
					num2 = (uint)(value / 10000000UL);
					num += 7;
				}
			}
			else
			{
				num2 = (uint)value;
			}
			if (num2 >= 10U)
			{
				if (num2 < 100U)
				{
					num++;
				}
				else if (num2 < 1000U)
				{
					num += 2;
				}
				else if (num2 < 10000U)
				{
					num += 3;
				}
				else if (num2 < 100000U)
				{
					num += 4;
				}
				else if (num2 < 1000000U)
				{
					num += 5;
				}
				else
				{
					num += 6;
				}
			}
			return num;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0001337C File Offset: 0x0001157C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountDigits(uint value)
		{
			int num = 1;
			if (value >= 100000U)
			{
				value /= 100000U;
				num += 5;
			}
			if (value >= 10U)
			{
				if (value < 100U)
				{
					num++;
				}
				else if (value < 1000U)
				{
					num += 2;
				}
				else if (value < 10000U)
				{
					num += 3;
				}
				else
				{
					num += 4;
				}
			}
			return num;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x000133F0 File Offset: 0x000115F0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountHexDigits(ulong value)
		{
			int num = 1;
			if (value > (ulong)(-1))
			{
				num += 8;
				value >>= 32;
			}
			if (value > 65535UL)
			{
				num += 4;
				value >>= 16;
			}
			if (value > 255UL)
			{
				num += 2;
				value >>= 8;
			}
			if (value > 15UL)
			{
				num++;
			}
			return num;
		}

		// Token: 0x04000158 RID: 344
		internal const string HexTableLower = "0123456789abcdef";

		// Token: 0x04000159 RID: 345
		internal const string HexTableUpper = "0123456789ABCDEF";

		// Token: 0x020001C6 RID: 454
		public enum HexCasing : uint
		{
			// Token: 0x0400080D RID: 2061
			Uppercase,
			// Token: 0x0400080E RID: 2062
			Lowercase = 8224U
		}
	}
}
