using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Leaf.xNet
{
	// Token: 0x02000074 RID: 116
	[ComVisible(true)]
	[Serializable]
	public sealed class ProxyException : NetException
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600060B RID: 1547 RVA: 0x000223E0 File Offset: 0x000205E0
		public ProxyClient ProxyClient { get; }

		// Token: 0x0600060C RID: 1548 RVA: 0x000223E8 File Offset: 0x000205E8
		public ProxyException()
			: this(Resources.ProxyException_Default, null)
		{
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x000223F8 File Offset: 0x000205F8
		public ProxyException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00022404 File Offset: 0x00020604
		public ProxyException(string message, ProxyClient proxyClient, Exception innerException = null)
			: base(message, innerException)
		{
			this.ProxyClient = proxyClient;
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00022418 File Offset: 0x00020618
		public ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
