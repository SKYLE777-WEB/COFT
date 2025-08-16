using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x0200003D RID: 61
	[ComVisible(true)]
	public enum OperationStatus
	{
		// Token: 0x04000127 RID: 295
		Done,
		// Token: 0x04000128 RID: 296
		DestinationTooSmall,
		// Token: 0x04000129 RID: 297
		NeedMoreData,
		// Token: 0x0400012A RID: 298
		InvalidData
	}
}
