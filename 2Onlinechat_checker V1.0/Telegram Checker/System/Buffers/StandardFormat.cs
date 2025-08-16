using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x02000042 RID: 66
	[ComVisible(true)]
	public readonly struct StandardFormat : IEquatable<StandardFormat>
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060002CF RID: 719 RVA: 0x000123C0 File Offset: 0x000105C0
		public char Symbol
		{
			get
			{
				return (char)this._format;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x000123C8 File Offset: 0x000105C8
		public byte Precision
		{
			get
			{
				return this._precision;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x000123D0 File Offset: 0x000105D0
		public bool HasPrecision
		{
			get
			{
				return this._precision != byte.MaxValue;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x000123E4 File Offset: 0x000105E4
		public bool IsDefault
		{
			get
			{
				return this._format == 0 && this._precision == 0;
			}
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x000123FC File Offset: 0x000105FC
		public StandardFormat(char symbol, byte precision = 255)
		{
			if (precision != 255 && precision > 99)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_PrecisionTooLarge();
			}
			if (symbol != (char)((byte)symbol))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_SymbolDoesNotFit();
			}
			this._format = (byte)symbol;
			this._precision = precision;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00012434 File Offset: 0x00010634
		public static implicit operator StandardFormat(char symbol)
		{
			return new StandardFormat(symbol, byte.MaxValue);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00012444 File Offset: 0x00010644
		public unsafe static StandardFormat Parse(ReadOnlySpan<char> format)
		{
			if (format.Length == 0)
			{
				return default(StandardFormat);
			}
			char c = (char)(*format[0]);
			byte b;
			if (format.Length == 1)
			{
				b = byte.MaxValue;
			}
			else
			{
				uint num = 0U;
				for (int i = 1; i < format.Length; i++)
				{
					uint num2 = (uint)(*format[i] - 48);
					if (num2 > 9U)
					{
						throw new FormatException(SR.Format(SR.Argument_CannotParsePrecision, 99));
					}
					num = num * 10U + num2;
					if (num > 99U)
					{
						throw new FormatException(SR.Format(SR.Argument_PrecisionTooLarge, 99));
					}
				}
				b = (byte)num;
			}
			return new StandardFormat(c, b);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00012508 File Offset: 0x00010708
		public static StandardFormat Parse(string format)
		{
			if (format != null)
			{
				return StandardFormat.Parse(format.AsSpan());
			}
			return default(StandardFormat);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00012534 File Offset: 0x00010734
		public override bool Equals(object obj)
		{
			if (obj is StandardFormat)
			{
				StandardFormat standardFormat = (StandardFormat)obj;
				return this.Equals(standardFormat);
			}
			return false;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00012564 File Offset: 0x00010764
		public override int GetHashCode()
		{
			return this._format.GetHashCode() ^ this._precision.GetHashCode();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00012594 File Offset: 0x00010794
		public bool Equals(StandardFormat other)
		{
			return this._format == other._format && this._precision == other._precision;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x000125B8 File Offset: 0x000107B8
		public unsafe override string ToString()
		{
			char* ptr = stackalloc char[(UIntPtr)8];
			int num = 0;
			char symbol = this.Symbol;
			if (symbol != '\0')
			{
				ptr[(IntPtr)(num++) * 2] = symbol;
				byte b = this.Precision;
				if (b != 255)
				{
					if (b >= 100)
					{
						ptr[(IntPtr)(num++) * 2] = (char)(48 + b / 100 % 10);
						b %= 100;
					}
					if (b >= 10)
					{
						ptr[(IntPtr)(num++) * 2] = (char)(48 + b / 10 % 10);
						b %= 10;
					}
					ptr[(IntPtr)(num++) * 2] = (char)(48 + b);
				}
			}
			return new string(ptr, 0, num);
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0001265C File Offset: 0x0001085C
		public static bool operator ==(StandardFormat left, StandardFormat right)
		{
			return left.Equals(right);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00012668 File Offset: 0x00010868
		public static bool operator !=(StandardFormat left, StandardFormat right)
		{
			return !left.Equals(right);
		}

		// Token: 0x0400013D RID: 317
		public const byte NoPrecision = 255;

		// Token: 0x0400013E RID: 318
		public const byte MaxPrecision = 99;

		// Token: 0x0400013F RID: 319
		private readonly byte _format;

		// Token: 0x04000140 RID: 320
		private readonly byte _precision;
	}
}
