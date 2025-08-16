using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x0200005F RID: 95
	[ComVisible(true)]
	public enum HttpExceptionStatus
	{
		// Token: 0x040001AF RID: 431
		Other,
		// Token: 0x040001B0 RID: 432
		ProtocolError,
		// Token: 0x040001B1 RID: 433
		ConnectFailure,
		// Token: 0x040001B2 RID: 434
		SendFailure,
		// Token: 0x040001B3 RID: 435
		ReceiveFailure
	}
}
