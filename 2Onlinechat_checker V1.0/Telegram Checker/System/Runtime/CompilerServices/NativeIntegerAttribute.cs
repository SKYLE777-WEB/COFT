using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000052 RID: 82
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	[System.Runtime.CompilerServices.Unsafe.Embedded]
	[CompilerGenerated]
	internal sealed class NativeIntegerAttribute : Attribute
	{
		// Token: 0x060003D3 RID: 979 RVA: 0x0001A7D4 File Offset: 0x000189D4
		public NativeIntegerAttribute()
		{
			this.TransformFlags = new bool[] { true };
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0001A7EC File Offset: 0x000189EC
		public NativeIntegerAttribute(bool[] A_0)
		{
			this.TransformFlags = A_0;
		}

		// Token: 0x0400018D RID: 397
		public readonly bool[] TransformFlags;
	}
}
