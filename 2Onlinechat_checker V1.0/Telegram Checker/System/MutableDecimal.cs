using System;

namespace System
{
	// Token: 0x02000034 RID: 52
	internal struct MutableDecimal
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600024D RID: 589 RVA: 0x000106A0 File Offset: 0x0000E8A0
		// (set) Token: 0x0600024E RID: 590 RVA: 0x000106B4 File Offset: 0x0000E8B4
		public bool IsNegative
		{
			get
			{
				return (this.Flags & 2147483648U) > 0U;
			}
			set
			{
				this.Flags = (this.Flags & 2147483647U) | (value ? 2147483648U : 0U);
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600024F RID: 591 RVA: 0x000106DC File Offset: 0x0000E8DC
		// (set) Token: 0x06000250 RID: 592 RVA: 0x000106E8 File Offset: 0x0000E8E8
		public int Scale
		{
			get
			{
				return (int)((byte)(this.Flags >> 16));
			}
			set
			{
				this.Flags = (this.Flags & 4278255615U) | (uint)((uint)value << 16);
			}
		}

		// Token: 0x0400011A RID: 282
		public uint Flags;

		// Token: 0x0400011B RID: 283
		public uint High;

		// Token: 0x0400011C RID: 284
		public uint Low;

		// Token: 0x0400011D RID: 285
		public uint Mid;

		// Token: 0x0400011E RID: 286
		private const uint SignMask = 2147483648U;

		// Token: 0x0400011F RID: 287
		private const uint ScaleMask = 16711680U;

		// Token: 0x04000120 RID: 288
		private const int ScaleShift = 16;
	}
}
