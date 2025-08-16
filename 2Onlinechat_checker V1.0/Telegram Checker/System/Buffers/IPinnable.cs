using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x02000043 RID: 67
	[ComVisible(true)]
	public interface IPinnable
	{
		// Token: 0x060002DD RID: 733
		MemoryHandle Pin(int elementIndex);

		// Token: 0x060002DE RID: 734
		void Unpin();
	}
}
