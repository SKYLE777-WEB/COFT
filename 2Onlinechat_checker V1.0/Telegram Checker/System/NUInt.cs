using System;
using System.Runtime.CompilerServices;

namespace System
{
	// Token: 0x02000031 RID: 49
	internal struct NUInt
	{
		// Token: 0x06000242 RID: 578 RVA: 0x00010608 File Offset: 0x0000E808
		private NUInt(uint value)
		{
			this._value = value;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00010614 File Offset: 0x0000E814
		private NUInt(ulong value)
		{
			this._value = value;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00010620 File Offset: 0x0000E820
		public static implicit operator NUInt(uint value)
		{
			return new NUInt(value);
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00010628 File Offset: 0x0000E828
		public static implicit operator IntPtr(NUInt value)
		{
			return (IntPtr)value._value;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00010638 File Offset: 0x0000E838
		public static explicit operator NUInt(int value)
		{
			return new NUInt((uint)value);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00010640 File Offset: 0x0000E840
		public unsafe static explicit operator void*(NUInt value)
		{
			return value._value;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00010648 File Offset: 0x0000E848
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NUInt operator *(NUInt left, NUInt right)
		{
			if (sizeof(IntPtr) != 4)
			{
				return new NUInt(left._value * right._value);
			}
			return new NUInt(left._value * right._value);
		}

		// Token: 0x04000118 RID: 280
		private unsafe readonly void* _value;
	}
}
