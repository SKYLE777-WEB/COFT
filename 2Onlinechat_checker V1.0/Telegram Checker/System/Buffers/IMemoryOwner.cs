using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x02000044 RID: 68
	[ComVisible(true)]
	public interface IMemoryOwner<T> : IDisposable
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060002DF RID: 735
		Memory<T> Memory { get; }
	}
}
