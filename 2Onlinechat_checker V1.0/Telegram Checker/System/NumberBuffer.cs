using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
	// Token: 0x02000028 RID: 40
	internal ref struct NumberBuffer
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00008E04 File Offset: 0x00007004
		public Span<byte> Digits
		{
			get
			{
				return new Span<byte>(Unsafe.AsPointer<byte>(ref this._b0), 51);
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00008E18 File Offset: 0x00007018
		public unsafe byte* UnsafeDigits
		{
			get
			{
				return (byte*)Unsafe.AsPointer<byte>(ref this._b0);
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00008E28 File Offset: 0x00007028
		public int NumDigits
		{
			get
			{
				return this.Digits.IndexOf(0);
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00008E38 File Offset: 0x00007038
		[Conditional("DEBUG")]
		public void CheckConsistency()
		{
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00008E3C File Offset: 0x0000703C
		public unsafe override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			stringBuilder.Append('"');
			Span<byte> digits = this.Digits;
			for (int i = 0; i < 51; i++)
			{
				byte b = *digits[i];
				if (b == 0)
				{
					break;
				}
				stringBuilder.Append((char)b);
			}
			stringBuilder.Append('"');
			stringBuilder.Append(", Scale = " + this.Scale);
			stringBuilder.Append(", IsNegative   = " + this.IsNegative.ToString());
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		// Token: 0x040000CF RID: 207
		public int Scale;

		// Token: 0x040000D0 RID: 208
		public bool IsNegative;

		// Token: 0x040000D1 RID: 209
		public const int BufferSize = 51;

		// Token: 0x040000D2 RID: 210
		private byte _b0;

		// Token: 0x040000D3 RID: 211
		private byte _b1;

		// Token: 0x040000D4 RID: 212
		private byte _b2;

		// Token: 0x040000D5 RID: 213
		private byte _b3;

		// Token: 0x040000D6 RID: 214
		private byte _b4;

		// Token: 0x040000D7 RID: 215
		private byte _b5;

		// Token: 0x040000D8 RID: 216
		private byte _b6;

		// Token: 0x040000D9 RID: 217
		private byte _b7;

		// Token: 0x040000DA RID: 218
		private byte _b8;

		// Token: 0x040000DB RID: 219
		private byte _b9;

		// Token: 0x040000DC RID: 220
		private byte _b10;

		// Token: 0x040000DD RID: 221
		private byte _b11;

		// Token: 0x040000DE RID: 222
		private byte _b12;

		// Token: 0x040000DF RID: 223
		private byte _b13;

		// Token: 0x040000E0 RID: 224
		private byte _b14;

		// Token: 0x040000E1 RID: 225
		private byte _b15;

		// Token: 0x040000E2 RID: 226
		private byte _b16;

		// Token: 0x040000E3 RID: 227
		private byte _b17;

		// Token: 0x040000E4 RID: 228
		private byte _b18;

		// Token: 0x040000E5 RID: 229
		private byte _b19;

		// Token: 0x040000E6 RID: 230
		private byte _b20;

		// Token: 0x040000E7 RID: 231
		private byte _b21;

		// Token: 0x040000E8 RID: 232
		private byte _b22;

		// Token: 0x040000E9 RID: 233
		private byte _b23;

		// Token: 0x040000EA RID: 234
		private byte _b24;

		// Token: 0x040000EB RID: 235
		private byte _b25;

		// Token: 0x040000EC RID: 236
		private byte _b26;

		// Token: 0x040000ED RID: 237
		private byte _b27;

		// Token: 0x040000EE RID: 238
		private byte _b28;

		// Token: 0x040000EF RID: 239
		private byte _b29;

		// Token: 0x040000F0 RID: 240
		private byte _b30;

		// Token: 0x040000F1 RID: 241
		private byte _b31;

		// Token: 0x040000F2 RID: 242
		private byte _b32;

		// Token: 0x040000F3 RID: 243
		private byte _b33;

		// Token: 0x040000F4 RID: 244
		private byte _b34;

		// Token: 0x040000F5 RID: 245
		private byte _b35;

		// Token: 0x040000F6 RID: 246
		private byte _b36;

		// Token: 0x040000F7 RID: 247
		private byte _b37;

		// Token: 0x040000F8 RID: 248
		private byte _b38;

		// Token: 0x040000F9 RID: 249
		private byte _b39;

		// Token: 0x040000FA RID: 250
		private byte _b40;

		// Token: 0x040000FB RID: 251
		private byte _b41;

		// Token: 0x040000FC RID: 252
		private byte _b42;

		// Token: 0x040000FD RID: 253
		private byte _b43;

		// Token: 0x040000FE RID: 254
		private byte _b44;

		// Token: 0x040000FF RID: 255
		private byte _b45;

		// Token: 0x04000100 RID: 256
		private byte _b46;

		// Token: 0x04000101 RID: 257
		private byte _b47;

		// Token: 0x04000102 RID: 258
		private byte _b48;

		// Token: 0x04000103 RID: 259
		private byte _b49;

		// Token: 0x04000104 RID: 260
		private byte _b50;
	}
}
