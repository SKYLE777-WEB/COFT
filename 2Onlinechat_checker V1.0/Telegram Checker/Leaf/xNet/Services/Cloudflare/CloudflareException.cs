using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Cloudflare
{
	// Token: 0x0200007C RID: 124
	[ComVisible(true)]
	[Serializable]
	public class CloudflareException : Exception
	{
		// Token: 0x06000655 RID: 1621 RVA: 0x00023AC8 File Offset: 0x00021CC8
		public CloudflareException(string message)
			: base(message)
		{
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00023AD4 File Offset: 0x00021CD4
		public CloudflareException(string message, Exception inner)
			: base(message, inner)
		{
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00023AE0 File Offset: 0x00021CE0
		public CloudflareException(int attempts)
			: this(attempts, string.Format("Clearance failed after {0} attempt(s).", attempts))
		{
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00023AFC File Offset: 0x00021CFC
		public CloudflareException(int attempts, string message)
			: base(message)
		{
			this.Attempts = attempts;
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00023B0C File Offset: 0x00021D0C
		public CloudflareException(int attempts, string message, Exception inner)
			: base(message, inner)
		{
			this.Attempts = attempts;
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x00023B20 File Offset: 0x00021D20
		public int Attempts { get; }
	}
}
