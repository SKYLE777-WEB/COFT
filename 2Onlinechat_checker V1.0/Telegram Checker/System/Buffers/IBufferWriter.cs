using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x0200003B RID: 59
	[ComVisible(true)]
	public interface IBufferWriter<T>
	{
		// Token: 0x06000289 RID: 649
		void Advance(int count);

		// Token: 0x0600028A RID: 650
		Memory<T> GetMemory(int sizeHint = 0);

		// Token: 0x0600028B RID: 651
		Span<T> GetSpan(int sizeHint = 0);
	}
}
