using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x02000068 RID: 104
	[ComVisible(true)]
	public abstract class HttpContent : IDisposable
	{
		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00020C6C File Offset: 0x0001EE6C
		// (set) Token: 0x060005A6 RID: 1446 RVA: 0x00020C74 File Offset: 0x0001EE74
		public string ContentType
		{
			get
			{
				return this.MimeContentType;
			}
			set
			{
				this.MimeContentType = value ?? string.Empty;
			}
		}

		// Token: 0x060005A7 RID: 1447
		public abstract long CalculateContentLength();

		// Token: 0x060005A8 RID: 1448
		public abstract void WriteTo(Stream stream);

		// Token: 0x060005A9 RID: 1449 RVA: 0x00020C8C File Offset: 0x0001EE8C
		public void Dispose()
		{
			this.Dispose(true);
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x00020C98 File Offset: 0x0001EE98
		protected virtual void Dispose(bool disposing)
		{
		}

		// Token: 0x04000277 RID: 631
		protected string MimeContentType = string.Empty;
	}
}
