using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x0200006C RID: 108
	[ComVisible(true)]
	public sealed class DownloadProgressChangedEventArgs : EventArgs
	{
		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x00021464 File Offset: 0x0001F664
		public long BytesReceived { get; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0002146C File Offset: 0x0001F66C
		public long TotalBytesToReceive { get; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060005C2 RID: 1474 RVA: 0x00021474 File Offset: 0x0001F674
		public double ProgressPercentage
		{
			get
			{
				return (double)this.BytesReceived / (double)this.TotalBytesToReceive * 100.0;
			}
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00021490 File Offset: 0x0001F690
		public DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive)
		{
			this.BytesReceived = bytesReceived;
			this.TotalBytesToReceive = totalBytesToReceive;
		}
	}
}
