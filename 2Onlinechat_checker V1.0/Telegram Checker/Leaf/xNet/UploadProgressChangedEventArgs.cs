using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x0200006D RID: 109
	[ComVisible(true)]
	public sealed class UploadProgressChangedEventArgs : EventArgs
	{
		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060005C4 RID: 1476 RVA: 0x000214A8 File Offset: 0x0001F6A8
		public long BytesSent { get; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x000214B0 File Offset: 0x0001F6B0
		public long TotalBytesToSend { get; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x000214B8 File Offset: 0x0001F6B8
		public double ProgressPercentage
		{
			get
			{
				return (double)this.BytesSent / (double)this.TotalBytesToSend * 100.0;
			}
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x000214D4 File Offset: 0x0001F6D4
		public UploadProgressChangedEventArgs(long bytesSent, long totalBytesToSend)
		{
			this.BytesSent = bytesSent;
			this.TotalBytesToSend = totalBytesToSend;
		}
	}
}
