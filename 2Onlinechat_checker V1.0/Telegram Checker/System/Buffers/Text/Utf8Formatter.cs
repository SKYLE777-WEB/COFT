using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers.Text
{
	// Token: 0x0200004A RID: 74
	[ComVisible(true)]
	public static class Utf8Formatter
	{
		// Token: 0x06000308 RID: 776 RVA: 0x00013450 File Offset: 0x00011650
		public unsafe static bool TryFormat(bool value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			char symbolOrDefault = FormattingHelpers.GetSymbolOrDefault(in format, 'G');
			if (value)
			{
				if (symbolOrDefault == 'G')
				{
					if (!BinaryPrimitives.TryWriteUInt32BigEndian(destination, 1416787301U))
					{
						goto IL_009F;
					}
				}
				else
				{
					if (symbolOrDefault != 'l')
					{
						goto IL_00A4;
					}
					if (!BinaryPrimitives.TryWriteUInt32BigEndian(destination, 1953658213U))
					{
						goto IL_009F;
					}
				}
				bytesWritten = 4;
				return true;
			}
			if (symbolOrDefault == 'G')
			{
				if (4 >= destination.Length)
				{
					goto IL_009F;
				}
				BinaryPrimitives.WriteUInt32BigEndian(destination, 1180789875U);
			}
			else
			{
				if (symbolOrDefault != 'l')
				{
					goto IL_00A4;
				}
				if (4 >= destination.Length)
				{
					goto IL_009F;
				}
				BinaryPrimitives.WriteUInt32BigEndian(destination, 1717660787U);
			}
			*destination[4] = 101;
			bytesWritten = 5;
			return true;
			IL_009F:
			bytesWritten = 0;
			return false;
			IL_00A4:
			return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001350C File Offset: 0x0001170C
		public static bool TryFormat(DateTimeOffset value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			TimeSpan timeSpan = Utf8Constants.s_nullUtcOffset;
			char c = format.Symbol;
			if (format.IsDefault)
			{
				c = 'G';
				timeSpan = value.Offset;
			}
			if (c <= 'O')
			{
				if (c == 'G')
				{
					return Utf8Formatter.TryFormatDateTimeG(value.DateTime, timeSpan, destination, out bytesWritten);
				}
				if (c == 'O')
				{
					return Utf8Formatter.TryFormatDateTimeO(value.DateTime, value.Offset, destination, out bytesWritten);
				}
			}
			else
			{
				if (c == 'R')
				{
					return Utf8Formatter.TryFormatDateTimeR(value.UtcDateTime, destination, out bytesWritten);
				}
				if (c == 'l')
				{
					return Utf8Formatter.TryFormatDateTimeL(value.UtcDateTime, destination, out bytesWritten);
				}
			}
			return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000135C0 File Offset: 0x000117C0
		public static bool TryFormat(DateTime value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			char symbolOrDefault = FormattingHelpers.GetSymbolOrDefault(in format, 'G');
			if (symbolOrDefault <= 'O')
			{
				if (symbolOrDefault == 'G')
				{
					return Utf8Formatter.TryFormatDateTimeG(value, Utf8Constants.s_nullUtcOffset, destination, out bytesWritten);
				}
				if (symbolOrDefault == 'O')
				{
					return Utf8Formatter.TryFormatDateTimeO(value, Utf8Constants.s_nullUtcOffset, destination, out bytesWritten);
				}
			}
			else
			{
				if (symbolOrDefault == 'R')
				{
					return Utf8Formatter.TryFormatDateTimeR(value, destination, out bytesWritten);
				}
				if (symbolOrDefault == 'l')
				{
					return Utf8Formatter.TryFormatDateTimeL(value, destination, out bytesWritten);
				}
			}
			return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00013644 File Offset: 0x00011844
		private unsafe static bool TryFormatDateTimeG(DateTime value, TimeSpan offset, Span<byte> destination, out int bytesWritten)
		{
			int num = 19;
			if (offset != Utf8Constants.s_nullUtcOffset)
			{
				num += 7;
			}
			if (destination.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num;
			byte b = *destination[18];
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Month, destination, 0);
			*destination[2] = 47;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Day, destination, 3);
			*destination[5] = 47;
			FormattingHelpers.WriteFourDecimalDigits((uint)value.Year, destination, 6);
			*destination[10] = 32;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Hour, destination, 11);
			*destination[13] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Minute, destination, 14);
			*destination[16] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Second, destination, 17);
			if (offset != Utf8Constants.s_nullUtcOffset)
			{
				byte b2;
				if (offset < default(TimeSpan))
				{
					b2 = 45;
					offset = TimeSpan.FromTicks(-offset.Ticks);
				}
				else
				{
					b2 = 43;
				}
				FormattingHelpers.WriteTwoDecimalDigits((uint)offset.Minutes, destination, 24);
				*destination[23] = 58;
				FormattingHelpers.WriteTwoDecimalDigits((uint)offset.Hours, destination, 21);
				*destination[20] = b2;
				*destination[19] = 32;
			}
			return true;
		}

		// Token: 0x0600030C RID: 780 RVA: 0x000137A0 File Offset: 0x000119A0
		private unsafe static bool TryFormatDateTimeO(DateTime value, TimeSpan offset, Span<byte> destination, out int bytesWritten)
		{
			int num = 27;
			DateTimeKind dateTimeKind = DateTimeKind.Local;
			if (offset == Utf8Constants.s_nullUtcOffset)
			{
				dateTimeKind = value.Kind;
				if (dateTimeKind == DateTimeKind.Local)
				{
					offset = TimeZoneInfo.Local.GetUtcOffset(value);
					num += 6;
				}
				else if (dateTimeKind == DateTimeKind.Utc)
				{
					num++;
				}
			}
			else
			{
				num += 6;
			}
			if (destination.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num;
			byte b = *destination[26];
			FormattingHelpers.WriteFourDecimalDigits((uint)value.Year, destination, 0);
			*destination[4] = 45;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Month, destination, 5);
			*destination[7] = 45;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Day, destination, 8);
			*destination[10] = 84;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Hour, destination, 11);
			*destination[13] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Minute, destination, 14);
			*destination[16] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Second, destination, 17);
			*destination[19] = 46;
			FormattingHelpers.WriteDigits((uint)(value.Ticks % 10000000L), destination.Slice(20, 7));
			if (dateTimeKind == DateTimeKind.Local)
			{
				byte b2;
				if (offset < default(TimeSpan))
				{
					b2 = 45;
					offset = TimeSpan.FromTicks(-offset.Ticks);
				}
				else
				{
					b2 = 43;
				}
				FormattingHelpers.WriteTwoDecimalDigits((uint)offset.Minutes, destination, 31);
				*destination[30] = 58;
				FormattingHelpers.WriteTwoDecimalDigits((uint)offset.Hours, destination, 28);
				*destination[27] = b2;
			}
			else if (dateTimeKind == DateTimeKind.Utc)
			{
				*destination[27] = 90;
			}
			return true;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00013960 File Offset: 0x00011B60
		private unsafe static bool TryFormatDateTimeR(DateTime value, Span<byte> destination, out int bytesWritten)
		{
			if (28 >= destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			uint num = Utf8Formatter.DayAbbreviations[(int)value.DayOfWeek];
			*destination[0] = (byte)num;
			num >>= 8;
			*destination[1] = (byte)num;
			num >>= 8;
			*destination[2] = (byte)num;
			*destination[3] = 44;
			*destination[4] = 32;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Day, destination, 5);
			*destination[7] = 32;
			uint num2 = Utf8Formatter.MonthAbbreviations[value.Month - 1];
			*destination[8] = (byte)num2;
			num2 >>= 8;
			*destination[9] = (byte)num2;
			num2 >>= 8;
			*destination[10] = (byte)num2;
			*destination[11] = 32;
			FormattingHelpers.WriteFourDecimalDigits((uint)value.Year, destination, 12);
			*destination[16] = 32;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Hour, destination, 17);
			*destination[19] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Minute, destination, 20);
			*destination[22] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Second, destination, 23);
			*destination[25] = 32;
			*destination[26] = 71;
			*destination[27] = 77;
			*destination[28] = 84;
			bytesWritten = 29;
			return true;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00013AC8 File Offset: 0x00011CC8
		private unsafe static bool TryFormatDateTimeL(DateTime value, Span<byte> destination, out int bytesWritten)
		{
			if (28 >= destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			uint num = Utf8Formatter.DayAbbreviationsLowercase[(int)value.DayOfWeek];
			*destination[0] = (byte)num;
			num >>= 8;
			*destination[1] = (byte)num;
			num >>= 8;
			*destination[2] = (byte)num;
			*destination[3] = 44;
			*destination[4] = 32;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Day, destination, 5);
			*destination[7] = 32;
			uint num2 = Utf8Formatter.MonthAbbreviationsLowercase[value.Month - 1];
			*destination[8] = (byte)num2;
			num2 >>= 8;
			*destination[9] = (byte)num2;
			num2 >>= 8;
			*destination[10] = (byte)num2;
			*destination[11] = 32;
			FormattingHelpers.WriteFourDecimalDigits((uint)value.Year, destination, 12);
			*destination[16] = 32;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Hour, destination, 17);
			*destination[19] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Minute, destination, 20);
			*destination[22] = 58;
			FormattingHelpers.WriteTwoDecimalDigits((uint)value.Second, destination, 23);
			*destination[25] = 32;
			*destination[26] = 103;
			*destination[27] = 109;
			*destination[28] = 116;
			bytesWritten = 29;
			return true;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00013C30 File Offset: 0x00011E30
		public unsafe static bool TryFormat(decimal value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			if (format.IsDefault)
			{
				format = 'G';
			}
			char symbol = format.Symbol;
			switch (symbol)
			{
			case 'E':
				goto IL_00EF;
			case 'F':
				goto IL_00A2;
			case 'G':
				break;
			default:
				switch (symbol)
				{
				case 'e':
					goto IL_00EF;
				case 'f':
					goto IL_00A2;
				case 'g':
					break;
				default:
					return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
				}
				break;
			}
			if (format.Precision != 255)
			{
				throw new NotSupportedException(SR.Argument_GWithPrecisionNotSupported);
			}
			NumberBuffer numberBuffer = default(NumberBuffer);
			Number.DecimalToNumber(value, ref numberBuffer);
			if (*numberBuffer.Digits[0] == 0)
			{
				numberBuffer.IsNegative = false;
			}
			return Utf8Formatter.TryFormatDecimalG(ref numberBuffer, destination, out bytesWritten);
			IL_00A2:
			NumberBuffer numberBuffer2 = default(NumberBuffer);
			Number.DecimalToNumber(value, ref numberBuffer2);
			byte b = ((format.Precision == byte.MaxValue) ? 2 : format.Precision);
			Number.RoundNumber(ref numberBuffer2, numberBuffer2.Scale + (int)b);
			return Utf8Formatter.TryFormatDecimalF(ref numberBuffer2, destination, out bytesWritten, b);
			IL_00EF:
			NumberBuffer numberBuffer3 = default(NumberBuffer);
			Number.DecimalToNumber(value, ref numberBuffer3);
			byte b2 = ((format.Precision == byte.MaxValue) ? 6 : format.Precision);
			Number.RoundNumber(ref numberBuffer3, (int)(b2 + 1));
			return Utf8Formatter.TryFormatDecimalE(ref numberBuffer3, destination, out bytesWritten, b2, (byte)format.Symbol);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00013D88 File Offset: 0x00011F88
		private unsafe static bool TryFormatDecimalE(ref NumberBuffer number, Span<byte> destination, out int bytesWritten, byte precision, byte exponentSymbol)
		{
			int scale = number.Scale;
			ReadOnlySpan<byte> readOnlySpan = number.Digits;
			int num = (int)((number.IsNegative ? 1 : 0) + 1 + ((precision == 0) ? 0 : (precision + 1)) + 2 + 3);
			if (destination.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			int num2 = 0;
			int num3 = 0;
			if (number.IsNegative)
			{
				*destination[num2++] = 45;
			}
			byte b = *readOnlySpan[num3];
			int num4;
			if (b == 0)
			{
				*destination[num2++] = 48;
				num4 = 0;
			}
			else
			{
				*destination[num2++] = b;
				num3++;
				num4 = scale - 1;
			}
			if (precision > 0)
			{
				*destination[num2++] = 46;
				for (int i = 0; i < (int)precision; i++)
				{
					byte b2 = *readOnlySpan[num3];
					if (b2 == 0)
					{
						while (i++ < (int)precision)
						{
							*destination[num2++] = 48;
						}
						break;
					}
					*destination[num2++] = b2;
					num3++;
				}
			}
			*destination[num2++] = exponentSymbol;
			if (num4 >= 0)
			{
				*destination[num2++] = 43;
			}
			else
			{
				*destination[num2++] = 45;
				num4 = -num4;
			}
			*destination[num2++] = 48;
			*destination[num2++] = (byte)(num4 / 10 + 48);
			*destination[num2++] = (byte)(num4 % 10 + 48);
			bytesWritten = num;
			return true;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00013F3C File Offset: 0x0001213C
		private unsafe static bool TryFormatDecimalF(ref NumberBuffer number, Span<byte> destination, out int bytesWritten, byte precision)
		{
			int scale = number.Scale;
			ReadOnlySpan<byte> readOnlySpan = number.Digits;
			int num = (number.IsNegative ? 1 : 0) + ((scale <= 0) ? 1 : scale) + (int)((precision == 0) ? 0 : (precision + 1));
			if (destination.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			int i = 0;
			int num2 = 0;
			if (number.IsNegative)
			{
				*destination[num2++] = 45;
			}
			if (scale <= 0)
			{
				*destination[num2++] = 48;
			}
			else
			{
				while (i < scale)
				{
					byte b = *readOnlySpan[i];
					if (b == 0)
					{
						int num3 = scale - i;
						for (int j = 0; j < num3; j++)
						{
							*destination[num2++] = 48;
						}
						break;
					}
					*destination[num2++] = b;
					i++;
				}
			}
			if (precision > 0)
			{
				*destination[num2++] = 46;
				int k = 0;
				if (scale < 0)
				{
					int num4 = Math.Min((int)precision, -scale);
					for (int l = 0; l < num4; l++)
					{
						*destination[num2++] = 48;
					}
					k += num4;
				}
				while (k < (int)precision)
				{
					byte b2 = *readOnlySpan[i];
					if (b2 == 0)
					{
						while (k++ < (int)precision)
						{
							*destination[num2++] = 48;
						}
						break;
					}
					*destination[num2++] = b2;
					i++;
					k++;
				}
			}
			bytesWritten = num;
			return true;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x000140F4 File Offset: 0x000122F4
		private unsafe static bool TryFormatDecimalG(ref NumberBuffer number, Span<byte> destination, out int bytesWritten)
		{
			int scale = number.Scale;
			ReadOnlySpan<byte> readOnlySpan = number.Digits;
			int numDigits = number.NumDigits;
			bool flag = scale < numDigits;
			int num;
			if (flag)
			{
				num = numDigits + 1;
				if (scale <= 0)
				{
					num += 1 + -scale;
				}
			}
			else
			{
				num = ((scale <= 0) ? 1 : scale);
			}
			if (number.IsNegative)
			{
				num++;
			}
			if (destination.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			int i = 0;
			int num2 = 0;
			if (number.IsNegative)
			{
				*destination[num2++] = 45;
			}
			if (scale <= 0)
			{
				*destination[num2++] = 48;
			}
			else
			{
				while (i < scale)
				{
					byte b = *readOnlySpan[i];
					if (b == 0)
					{
						int num3 = scale - i;
						for (int j = 0; j < num3; j++)
						{
							*destination[num2++] = 48;
						}
						break;
					}
					*destination[num2++] = b;
					i++;
				}
			}
			if (flag)
			{
				*destination[num2++] = 46;
				if (scale < 0)
				{
					int num4 = -scale;
					for (int k = 0; k < num4; k++)
					{
						*destination[num2++] = 48;
					}
				}
				byte b2;
				while ((b2 = *readOnlySpan[i++]) != 0)
				{
					*destination[num2++] = b2;
				}
			}
			bytesWritten = num;
			return true;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001428C File Offset: 0x0001248C
		public static bool TryFormat(double value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatFloatingPoint<double>(value, destination, out bytesWritten, format);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00014298 File Offset: 0x00012498
		public static bool TryFormat(float value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatFloatingPoint<float>(value, destination, out bytesWritten, format);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x000142A4 File Offset: 0x000124A4
		private unsafe static bool TryFormatFloatingPoint<T>(T value, Span<byte> destination, out int bytesWritten, StandardFormat format) where T : IFormattable
		{
			if (format.IsDefault)
			{
				format = 'G';
			}
			char symbol = format.Symbol;
			switch (symbol)
			{
			case 'E':
			case 'F':
				goto IL_006F;
			case 'G':
				break;
			default:
				switch (symbol)
				{
				case 'e':
				case 'f':
					goto IL_006F;
				case 'g':
					break;
				default:
					return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
				}
				break;
			}
			if (format.Precision != 255)
			{
				throw new NotSupportedException(SR.Argument_GWithPrecisionNotSupported);
			}
			IL_006F:
			string text = format.ToString();
			string text2 = value.ToString(text, CultureInfo.InvariantCulture);
			int length = text2.Length;
			if (length > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				*destination[i] = (byte)text2[i];
			}
			bytesWritten = length;
			return true;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0001438C File Offset: 0x0001258C
		public unsafe static bool TryFormat(Guid value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			char symbolOrDefault = FormattingHelpers.GetSymbolOrDefault(in format, 'D');
			int num;
			if (symbolOrDefault <= 'D')
			{
				if (symbolOrDefault == 'B')
				{
					num = -2139260122;
					goto IL_006C;
				}
				if (symbolOrDefault == 'D')
				{
					num = -2147483612;
					goto IL_006C;
				}
			}
			else
			{
				if (symbolOrDefault == 'N')
				{
					num = 32;
					goto IL_006C;
				}
				if (symbolOrDefault == 'P')
				{
					num = -2144786394;
					goto IL_006C;
				}
			}
			return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
			IL_006C:
			if ((int)((byte)num) > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = (int)((byte)num);
			num >>= 8;
			if ((byte)num != 0)
			{
				*destination[0] = (byte)num;
				destination = destination.Slice(1);
			}
			num >>= 8;
			Utf8Formatter.DecomposedGuid decomposedGuid = default(Utf8Formatter.DecomposedGuid);
			decomposedGuid.Guid = value;
			byte b = *destination[8];
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte03, destination, 0, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte02, destination, 2, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte01, destination, 4, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte00, destination, 6, FormattingHelpers.HexCasing.Lowercase);
			if (num < 0)
			{
				*destination[8] = 45;
				destination = destination.Slice(9);
			}
			else
			{
				destination = destination.Slice(8);
			}
			byte b2 = *destination[4];
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte05, destination, 0, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte04, destination, 2, FormattingHelpers.HexCasing.Lowercase);
			if (num < 0)
			{
				*destination[4] = 45;
				destination = destination.Slice(5);
			}
			else
			{
				destination = destination.Slice(4);
			}
			byte b3 = *destination[4];
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte07, destination, 0, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte06, destination, 2, FormattingHelpers.HexCasing.Lowercase);
			if (num < 0)
			{
				*destination[4] = 45;
				destination = destination.Slice(5);
			}
			else
			{
				destination = destination.Slice(4);
			}
			byte b4 = *destination[4];
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte08, destination, 0, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte09, destination, 2, FormattingHelpers.HexCasing.Lowercase);
			if (num < 0)
			{
				*destination[4] = 45;
				destination = destination.Slice(5);
			}
			else
			{
				destination = destination.Slice(4);
			}
			byte b5 = *destination[11];
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte10, destination, 0, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte11, destination, 2, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte12, destination, 4, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte13, destination, 6, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte14, destination, 8, FormattingHelpers.HexCasing.Lowercase);
			FormattingHelpers.WriteHexByte(decomposedGuid.Byte15, destination, 10, FormattingHelpers.HexCasing.Lowercase);
			if ((byte)num != 0)
			{
				*destination[12] = (byte)num;
			}
			return true;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00014670 File Offset: 0x00012870
		public static bool TryFormat(byte value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatUInt64((ulong)value, destination, out bytesWritten, format);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0001467C File Offset: 0x0001287C
		[CLSCompliant(false)]
		public static bool TryFormat(sbyte value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatInt64((long)value, 255UL, destination, out bytesWritten, format);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00014690 File Offset: 0x00012890
		[CLSCompliant(false)]
		public static bool TryFormat(ushort value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatUInt64((ulong)value, destination, out bytesWritten, format);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0001469C File Offset: 0x0001289C
		public static bool TryFormat(short value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatInt64((long)value, 65535UL, destination, out bytesWritten, format);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x000146B0 File Offset: 0x000128B0
		[CLSCompliant(false)]
		public static bool TryFormat(uint value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatUInt64((ulong)value, destination, out bytesWritten, format);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x000146BC File Offset: 0x000128BC
		public static bool TryFormat(int value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatInt64((long)value, (ulong)(-1), destination, out bytesWritten, format);
		}

		// Token: 0x0600031D RID: 797 RVA: 0x000146CC File Offset: 0x000128CC
		[CLSCompliant(false)]
		public static bool TryFormat(ulong value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatUInt64(value, destination, out bytesWritten, format);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x000146D8 File Offset: 0x000128D8
		public static bool TryFormat(long value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			return Utf8Formatter.TryFormatInt64(value, ulong.MaxValue, destination, out bytesWritten, format);
		}

		// Token: 0x0600031F RID: 799 RVA: 0x000146E8 File Offset: 0x000128E8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatInt64(long value, ulong mask, Span<byte> destination, out int bytesWritten, StandardFormat format)
		{
			if (format.IsDefault)
			{
				return Utf8Formatter.TryFormatInt64Default(value, destination, out bytesWritten);
			}
			char symbol = format.Symbol;
			if (symbol <= 'X')
			{
				if (symbol <= 'G')
				{
					if (symbol == 'D')
					{
						goto IL_00B0;
					}
					if (symbol != 'G')
					{
						goto IL_00F6;
					}
				}
				else
				{
					if (symbol == 'N')
					{
						goto IL_00C0;
					}
					if (symbol != 'X')
					{
						goto IL_00F6;
					}
					return Utf8Formatter.TryFormatUInt64X((ulong)(value & (long)mask), format.Precision, false, destination, out bytesWritten);
				}
			}
			else if (symbol <= 'g')
			{
				if (symbol == 'd')
				{
					goto IL_00B0;
				}
				if (symbol != 'g')
				{
					goto IL_00F6;
				}
			}
			else
			{
				if (symbol == 'n')
				{
					goto IL_00C0;
				}
				if (symbol != 'x')
				{
					goto IL_00F6;
				}
				return Utf8Formatter.TryFormatUInt64X((ulong)(value & (long)mask), format.Precision, true, destination, out bytesWritten);
			}
			if (format.HasPrecision)
			{
				throw new NotSupportedException(SR.Argument_GWithPrecisionNotSupported);
			}
			return Utf8Formatter.TryFormatInt64D(value, format.Precision, destination, out bytesWritten);
			IL_00B0:
			return Utf8Formatter.TryFormatInt64D(value, format.Precision, destination, out bytesWritten);
			IL_00C0:
			return Utf8Formatter.TryFormatInt64N(value, format.Precision, destination, out bytesWritten);
			IL_00F6:
			return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
		}

		// Token: 0x06000320 RID: 800 RVA: 0x000147F8 File Offset: 0x000129F8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatInt64D(long value, byte precision, Span<byte> destination, out int bytesWritten)
		{
			bool flag = false;
			if (value < 0L)
			{
				flag = true;
				value = -value;
			}
			return Utf8Formatter.TryFormatUInt64D((ulong)value, precision, destination, flag, out bytesWritten);
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00014824 File Offset: 0x00012A24
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatInt64Default(long value, Span<byte> destination, out int bytesWritten)
		{
			if (value < 10L)
			{
				return Utf8Formatter.TryFormatUInt32SingleDigit((uint)value, destination, out bytesWritten);
			}
			if (IntPtr.Size == 8)
			{
				return Utf8Formatter.TryFormatInt64MultipleDigits(value, destination, out bytesWritten);
			}
			if (value <= 2147483647L && value >= -2147483648L)
			{
				return Utf8Formatter.TryFormatInt32MultipleDigits((int)value, destination, out bytesWritten);
			}
			if (value <= 4294967295000000000L && value >= -4294967295000000000L)
			{
				if (value >= 0L)
				{
					return Utf8Formatter.TryFormatUInt64LessThanBillionMaxUInt((ulong)value, destination, out bytesWritten);
				}
				return Utf8Formatter.TryFormatInt64MoreThanNegativeBillionMaxUInt(-value, destination, out bytesWritten);
			}
			else
			{
				if (value >= 0L)
				{
					return Utf8Formatter.TryFormatUInt64MoreThanBillionMaxUInt((ulong)value, destination, out bytesWritten);
				}
				return Utf8Formatter.TryFormatInt64LessThanNegativeBillionMaxUInt(-value, destination, out bytesWritten);
			}
		}

		// Token: 0x06000322 RID: 802 RVA: 0x000148D4 File Offset: 0x00012AD4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatInt32Default(int value, Span<byte> destination, out int bytesWritten)
		{
			if (value < 10)
			{
				return Utf8Formatter.TryFormatUInt32SingleDigit((uint)value, destination, out bytesWritten);
			}
			return Utf8Formatter.TryFormatInt32MultipleDigits(value, destination, out bytesWritten);
		}

		// Token: 0x06000323 RID: 803 RVA: 0x000148F0 File Offset: 0x00012AF0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool TryFormatInt32MultipleDigits(int value, Span<byte> destination, out int bytesWritten)
		{
			if (value >= 0)
			{
				return Utf8Formatter.TryFormatUInt32MultipleDigits((uint)value, destination, out bytesWritten);
			}
			value = -value;
			int num = FormattingHelpers.CountDigits((uint)value);
			if (num >= destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			*destination[0] = 45;
			bytesWritten = num + 1;
			FormattingHelpers.WriteDigits((uint)value, destination.Slice(1, num));
			return true;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00014950 File Offset: 0x00012B50
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool TryFormatInt64MultipleDigits(long value, Span<byte> destination, out int bytesWritten)
		{
			if (value >= 0L)
			{
				return Utf8Formatter.TryFormatUInt64MultipleDigits((ulong)value, destination, out bytesWritten);
			}
			value = -value;
			int num = FormattingHelpers.CountDigits((ulong)value);
			if (num >= destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			*destination[0] = 45;
			bytesWritten = num + 1;
			FormattingHelpers.WriteDigits((ulong)value, destination.Slice(1, num));
			return true;
		}

		// Token: 0x06000325 RID: 805 RVA: 0x000149B0 File Offset: 0x00012BB0
		private unsafe static bool TryFormatInt64MoreThanNegativeBillionMaxUInt(long value, Span<byte> destination, out int bytesWritten)
		{
			uint num = (uint)(value / 1000000000L);
			uint num2 = (uint)(value - (long)((ulong)(num * 1000000000U)));
			int num3 = FormattingHelpers.CountDigits(num);
			int num4 = num3 + 9;
			if (num4 >= destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			*destination[0] = 45;
			bytesWritten = num4 + 1;
			FormattingHelpers.WriteDigits(num, destination.Slice(1, num3));
			FormattingHelpers.WriteDigits(num2, destination.Slice(num3 + 1, 9));
			return true;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00014A28 File Offset: 0x00012C28
		private unsafe static bool TryFormatInt64LessThanNegativeBillionMaxUInt(long value, Span<byte> destination, out int bytesWritten)
		{
			ulong num = (ulong)(value / 1000000000L);
			uint num2 = (uint)(value - (long)(num * 1000000000UL));
			uint num3 = (uint)(num / 1000000000UL);
			uint num4 = (uint)(num - (ulong)(num3 * 1000000000U));
			int num5 = FormattingHelpers.CountDigits(num3);
			int num6 = num5 + 18;
			if (num6 >= destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			*destination[0] = 45;
			bytesWritten = num6 + 1;
			FormattingHelpers.WriteDigits(num3, destination.Slice(1, num5));
			FormattingHelpers.WriteDigits(num4, destination.Slice(num5 + 1, 9));
			FormattingHelpers.WriteDigits(num2, destination.Slice(num5 + 1 + 9, 9));
			return true;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00014AD4 File Offset: 0x00012CD4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatInt64N(long value, byte precision, Span<byte> destination, out int bytesWritten)
		{
			bool flag = false;
			if (value < 0L)
			{
				flag = true;
				value = -value;
			}
			return Utf8Formatter.TryFormatUInt64N((ulong)value, precision, destination, flag, out bytesWritten);
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00014B00 File Offset: 0x00012D00
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatUInt64(ulong value, Span<byte> destination, out int bytesWritten, StandardFormat format)
		{
			if (format.IsDefault)
			{
				return Utf8Formatter.TryFormatUInt64Default(value, destination, out bytesWritten);
			}
			char symbol = format.Symbol;
			if (symbol <= 'X')
			{
				if (symbol <= 'G')
				{
					if (symbol == 'D')
					{
						goto IL_00B1;
					}
					if (symbol != 'G')
					{
						goto IL_00F5;
					}
				}
				else
				{
					if (symbol == 'N')
					{
						goto IL_00C2;
					}
					if (symbol != 'X')
					{
						goto IL_00F5;
					}
					return Utf8Formatter.TryFormatUInt64X(value, format.Precision, false, destination, out bytesWritten);
				}
			}
			else if (symbol <= 'g')
			{
				if (symbol == 'd')
				{
					goto IL_00B1;
				}
				if (symbol != 'g')
				{
					goto IL_00F5;
				}
			}
			else
			{
				if (symbol == 'n')
				{
					goto IL_00C2;
				}
				if (symbol != 'x')
				{
					goto IL_00F5;
				}
				return Utf8Formatter.TryFormatUInt64X(value, format.Precision, true, destination, out bytesWritten);
			}
			if (format.HasPrecision)
			{
				throw new NotSupportedException(SR.Argument_GWithPrecisionNotSupported);
			}
			return Utf8Formatter.TryFormatUInt64D(value, format.Precision, destination, false, out bytesWritten);
			IL_00B1:
			return Utf8Formatter.TryFormatUInt64D(value, format.Precision, destination, false, out bytesWritten);
			IL_00C2:
			return Utf8Formatter.TryFormatUInt64N(value, format.Precision, destination, false, out bytesWritten);
			IL_00F5:
			return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00014C0C File Offset: 0x00012E0C
		private unsafe static bool TryFormatUInt64D(ulong value, byte precision, Span<byte> destination, bool insertNegationSign, out int bytesWritten)
		{
			int num = FormattingHelpers.CountDigits(value);
			int num2 = (int)((precision == byte.MaxValue) ? 0 : precision) - num;
			if (num2 < 0)
			{
				num2 = 0;
			}
			int num3 = num + num2;
			if (insertNegationSign)
			{
				num3++;
			}
			if (num3 > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num3;
			if (insertNegationSign)
			{
				*destination[0] = 45;
				destination = destination.Slice(1);
			}
			if (num2 > 0)
			{
				FormattingHelpers.FillWithAsciiZeros(destination.Slice(0, num2));
			}
			FormattingHelpers.WriteDigits(value, destination.Slice(num2, num));
			return true;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00014CA8 File Offset: 0x00012EA8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatUInt64Default(ulong value, Span<byte> destination, out int bytesWritten)
		{
			if (value < 10UL)
			{
				return Utf8Formatter.TryFormatUInt32SingleDigit((uint)value, destination, out bytesWritten);
			}
			if (IntPtr.Size == 8)
			{
				return Utf8Formatter.TryFormatUInt64MultipleDigits(value, destination, out bytesWritten);
			}
			if (value <= (ulong)(-1))
			{
				return Utf8Formatter.TryFormatUInt32MultipleDigits((uint)value, destination, out bytesWritten);
			}
			if (value <= 4294967295000000000UL)
			{
				return Utf8Formatter.TryFormatUInt64LessThanBillionMaxUInt(value, destination, out bytesWritten);
			}
			return Utf8Formatter.TryFormatUInt64MoreThanBillionMaxUInt(value, destination, out bytesWritten);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00014D14 File Offset: 0x00012F14
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatUInt32Default(uint value, Span<byte> destination, out int bytesWritten)
		{
			if (value < 10U)
			{
				return Utf8Formatter.TryFormatUInt32SingleDigit(value, destination, out bytesWritten);
			}
			return Utf8Formatter.TryFormatUInt32MultipleDigits(value, destination, out bytesWritten);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00014D30 File Offset: 0x00012F30
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool TryFormatUInt32SingleDigit(uint value, Span<byte> destination, out int bytesWritten)
		{
			if (destination.Length == 0)
			{
				bytesWritten = 0;
				return false;
			}
			*destination[0] = (byte)(48U + value);
			bytesWritten = 1;
			return true;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00014D58 File Offset: 0x00012F58
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatUInt32MultipleDigits(uint value, Span<byte> destination, out int bytesWritten)
		{
			int num = FormattingHelpers.CountDigits(value);
			if (num > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num;
			FormattingHelpers.WriteDigits(value, destination.Slice(0, num));
			return true;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00014D98 File Offset: 0x00012F98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool TryFormatUInt64SingleDigit(ulong value, Span<byte> destination, out int bytesWritten)
		{
			if (destination.Length == 0)
			{
				bytesWritten = 0;
				return false;
			}
			*destination[0] = (byte)(48UL + value);
			bytesWritten = 1;
			return true;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00014DC0 File Offset: 0x00012FC0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryFormatUInt64MultipleDigits(ulong value, Span<byte> destination, out int bytesWritten)
		{
			int num = FormattingHelpers.CountDigits(value);
			if (num > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num;
			FormattingHelpers.WriteDigits(value, destination.Slice(0, num));
			return true;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00014E00 File Offset: 0x00013000
		private static bool TryFormatUInt64LessThanBillionMaxUInt(ulong value, Span<byte> destination, out int bytesWritten)
		{
			uint num = (uint)(value / 1000000000UL);
			uint num2 = (uint)(value - (ulong)(num * 1000000000U));
			int num3 = FormattingHelpers.CountDigits(num);
			int num4 = num3 + 9;
			if (num4 > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num4;
			FormattingHelpers.WriteDigits(num, destination.Slice(0, num3));
			FormattingHelpers.WriteDigits(num2, destination.Slice(num3, 9));
			return true;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00014E68 File Offset: 0x00013068
		private static bool TryFormatUInt64MoreThanBillionMaxUInt(ulong value, Span<byte> destination, out int bytesWritten)
		{
			ulong num = value / 1000000000UL;
			uint num2 = (uint)(value - num * 1000000000UL);
			uint num3 = (uint)(num / 1000000000UL);
			uint num4 = (uint)(num - (ulong)(num3 * 1000000000U));
			int num5 = FormattingHelpers.CountDigits(num3);
			int num6 = num5 + 18;
			if (num6 > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num6;
			FormattingHelpers.WriteDigits(num3, destination.Slice(0, num5));
			FormattingHelpers.WriteDigits(num4, destination.Slice(num5, 9));
			FormattingHelpers.WriteDigits(num2, destination.Slice(num5 + 9, 9));
			return true;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00014F00 File Offset: 0x00013100
		private unsafe static bool TryFormatUInt64N(ulong value, byte precision, Span<byte> destination, bool insertNegationSign, out int bytesWritten)
		{
			int num = FormattingHelpers.CountDigits(value);
			int num2 = (num - 1) / 3;
			int num3 = (int)((precision == byte.MaxValue) ? 2 : precision);
			int num4 = num + num2;
			if (num3 > 0)
			{
				num4 += num3 + 1;
			}
			if (insertNegationSign)
			{
				num4++;
			}
			if (num4 > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num4;
			if (insertNegationSign)
			{
				*destination[0] = 45;
				destination = destination.Slice(1);
			}
			FormattingHelpers.WriteDigitsWithGroupSeparator(value, destination.Slice(0, num + num2));
			if (num3 > 0)
			{
				*destination[num + num2] = 46;
				FormattingHelpers.FillWithAsciiZeros(destination.Slice(num + num2 + 1, num3));
			}
			return true;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00014FB8 File Offset: 0x000131B8
		private unsafe static bool TryFormatUInt64X(ulong value, byte precision, bool useLower, Span<byte> destination, out int bytesWritten)
		{
			int num = FormattingHelpers.CountHexDigits(value);
			int num2 = ((precision == byte.MaxValue) ? num : Math.Max((int)precision, num));
			if (destination.Length < num2)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num2;
			string text = (useLower ? "0123456789abcdef" : "0123456789ABCDEF");
			while (--num2 < destination.Length)
			{
				*destination[num2] = (byte)text[(int)value & 15];
				value >>= 4;
			}
			return true;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00015044 File Offset: 0x00013244
		public unsafe static bool TryFormat(TimeSpan value, Span<byte> destination, out int bytesWritten, StandardFormat format = default(StandardFormat))
		{
			char c = FormattingHelpers.GetSymbolOrDefault(in format, 'c');
			if (c <= 'T')
			{
				if (c == 'G')
				{
					goto IL_004E;
				}
				if (c != 'T')
				{
					goto IL_0047;
				}
			}
			else
			{
				if (c == 'c' || c == 'g')
				{
					goto IL_004E;
				}
				if (c != 't')
				{
					goto IL_0047;
				}
			}
			c = 'c';
			goto IL_004E;
			IL_0047:
			return ThrowHelper.TryFormatThrowFormatException(out bytesWritten);
			IL_004E:
			int num = 8;
			long num2 = value.Ticks;
			uint num3;
			ulong num4;
			if (num2 < 0L)
			{
				num2 = -num2;
				if (num2 < 0L)
				{
					num3 = 4775808U;
					num4 = 922337203685UL;
					goto IL_00A0;
				}
			}
			ulong num5;
			num4 = FormattingHelpers.DivMod((ulong)Math.Abs(value.Ticks), 10000000UL, out num5);
			num3 = (uint)num5;
			IL_00A0:
			int num6 = 0;
			if (c == 'c')
			{
				if (num3 != 0U)
				{
					num6 = 7;
				}
			}
			else if (c == 'G')
			{
				num6 = 7;
			}
			else if (num3 != 0U)
			{
				num6 = 7 - FormattingHelpers.CountDecimalTrailingZeros(num3, out num3);
			}
			if (num6 != 0)
			{
				num += num6 + 1;
			}
			ulong num7 = 0UL;
			ulong num8 = 0UL;
			if (num4 > 0UL)
			{
				num7 = FormattingHelpers.DivMod(num4, 60UL, out num8);
			}
			ulong num9 = 0UL;
			ulong num10 = 0UL;
			if (num7 > 0UL)
			{
				num9 = FormattingHelpers.DivMod(num7, 60UL, out num10);
			}
			uint num11 = 0U;
			uint num12 = 0U;
			if (num9 > 0UL)
			{
				num11 = FormattingHelpers.DivMod((uint)num9, 24U, out num12);
			}
			int num13 = 2;
			if (num12 < 10U && c == 'g')
			{
				num13--;
				num--;
			}
			int num14 = 0;
			if (num11 == 0U)
			{
				if (c == 'G')
				{
					num += 2;
					num14 = 1;
				}
			}
			else
			{
				num14 = FormattingHelpers.CountDigits(num11);
				num += num14 + 1;
			}
			if (value.Ticks < 0L)
			{
				num++;
			}
			if (destination.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = num;
			int num15 = 0;
			if (value.Ticks < 0L)
			{
				*destination[num15++] = 45;
			}
			if (num14 > 0)
			{
				FormattingHelpers.WriteDigits(num11, destination.Slice(num15, num14));
				num15 += num14;
				*destination[num15++] = ((c == 'c') ? 46 : 58);
			}
			FormattingHelpers.WriteDigits(num12, destination.Slice(num15, num13));
			num15 += num13;
			*destination[num15++] = 58;
			FormattingHelpers.WriteDigits((uint)num10, destination.Slice(num15, 2));
			num15 += 2;
			*destination[num15++] = 58;
			FormattingHelpers.WriteDigits((uint)num8, destination.Slice(num15, 2));
			num15 += 2;
			if (num6 > 0)
			{
				*destination[num15++] = 46;
				FormattingHelpers.WriteDigits(num3, destination.Slice(num15, num6));
				num15 += num6;
			}
			return true;
		}

		// Token: 0x0400015A RID: 346
		private const byte TimeMarker = 84;

		// Token: 0x0400015B RID: 347
		private const byte UtcMarker = 90;

		// Token: 0x0400015C RID: 348
		private const byte GMT1 = 71;

		// Token: 0x0400015D RID: 349
		private const byte GMT2 = 77;

		// Token: 0x0400015E RID: 350
		private const byte GMT3 = 84;

		// Token: 0x0400015F RID: 351
		private const byte GMT1Lowercase = 103;

		// Token: 0x04000160 RID: 352
		private const byte GMT2Lowercase = 109;

		// Token: 0x04000161 RID: 353
		private const byte GMT3Lowercase = 116;

		// Token: 0x04000162 RID: 354
		private static readonly uint[] DayAbbreviations = new uint[] { 7238995U, 7237453U, 6649172U, 6579543U, 7694420U, 6910534U, 7627091U };

		// Token: 0x04000163 RID: 355
		private static readonly uint[] DayAbbreviationsLowercase = new uint[] { 7239027U, 7237485U, 6649204U, 6579575U, 7694452U, 6910566U, 7627123U };

		// Token: 0x04000164 RID: 356
		private static readonly uint[] MonthAbbreviations = new uint[]
		{
			7233866U, 6448454U, 7496013U, 7499841U, 7954765U, 7238986U, 7107914U, 6780225U, 7365971U, 7627599U,
			7761742U, 6513988U
		};

		// Token: 0x04000165 RID: 357
		private static readonly uint[] MonthAbbreviationsLowercase = new uint[]
		{
			7233898U, 6448486U, 7496045U, 7499873U, 7954797U, 7239018U, 7107946U, 6780257U, 7366003U, 7627631U,
			7761774U, 6514020U
		};

		// Token: 0x04000166 RID: 358
		private const byte OpenBrace = 123;

		// Token: 0x04000167 RID: 359
		private const byte CloseBrace = 125;

		// Token: 0x04000168 RID: 360
		private const byte OpenParen = 40;

		// Token: 0x04000169 RID: 361
		private const byte CloseParen = 41;

		// Token: 0x0400016A RID: 362
		private const byte Dash = 45;

		// Token: 0x020001C7 RID: 455
		[StructLayout(LayoutKind.Explicit)]
		private struct DecomposedGuid
		{
			// Token: 0x0400080F RID: 2063
			[FieldOffset(0)]
			public Guid Guid;

			// Token: 0x04000810 RID: 2064
			[FieldOffset(0)]
			public byte Byte00;

			// Token: 0x04000811 RID: 2065
			[FieldOffset(1)]
			public byte Byte01;

			// Token: 0x04000812 RID: 2066
			[FieldOffset(2)]
			public byte Byte02;

			// Token: 0x04000813 RID: 2067
			[FieldOffset(3)]
			public byte Byte03;

			// Token: 0x04000814 RID: 2068
			[FieldOffset(4)]
			public byte Byte04;

			// Token: 0x04000815 RID: 2069
			[FieldOffset(5)]
			public byte Byte05;

			// Token: 0x04000816 RID: 2070
			[FieldOffset(6)]
			public byte Byte06;

			// Token: 0x04000817 RID: 2071
			[FieldOffset(7)]
			public byte Byte07;

			// Token: 0x04000818 RID: 2072
			[FieldOffset(8)]
			public byte Byte08;

			// Token: 0x04000819 RID: 2073
			[FieldOffset(9)]
			public byte Byte09;

			// Token: 0x0400081A RID: 2074
			[FieldOffset(10)]
			public byte Byte10;

			// Token: 0x0400081B RID: 2075
			[FieldOffset(11)]
			public byte Byte11;

			// Token: 0x0400081C RID: 2076
			[FieldOffset(12)]
			public byte Byte12;

			// Token: 0x0400081D RID: 2077
			[FieldOffset(13)]
			public byte Byte13;

			// Token: 0x0400081E RID: 2078
			[FieldOffset(14)]
			public byte Byte14;

			// Token: 0x0400081F RID: 2079
			[FieldOffset(15)]
			public byte Byte15;
		}
	}
}
