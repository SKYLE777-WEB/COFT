using System;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Leaf.xNet
{
	// Token: 0x0200005A RID: 90
	[ComVisible(true)]
	public class AdvancedWebClient : WebClient
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000430 RID: 1072 RVA: 0x0001B300 File Offset: 0x00019500
		// (set) Token: 0x06000431 RID: 1073 RVA: 0x0001B308 File Offset: 0x00019508
		public int Timeout { get; set; } = 10000;

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x0001B314 File Offset: 0x00019514
		// (set) Token: 0x06000433 RID: 1075 RVA: 0x0001B31C File Offset: 0x0001951C
		public int ReadWriteTimeout { get; set; } = 10000;

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x0001B328 File Offset: 0x00019528
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x0001B330 File Offset: 0x00019530
		public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x0001B33C File Offset: 0x0001953C
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x0001B344 File Offset: 0x00019544
		public bool ServerCertificateValidation { get; set; }

		// Token: 0x06000438 RID: 1080 RVA: 0x0001B350 File Offset: 0x00019550
		protected override WebRequest GetWebRequest(Uri uri)
		{
			WebRequest webRequest = base.GetWebRequest(uri);
			if (webRequest == null)
			{
				throw new NullReferenceException("Null reference: unable to get instance of WebRequest in AdvancedWebClient.");
			}
			webRequest.Timeout = this.Timeout;
			HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
			httpWebRequest.ReadWriteTimeout = this.ReadWriteTimeout;
			httpWebRequest.AutomaticDecompression = this.DecompressionMethods;
			if (!this.ServerCertificateValidation)
			{
				HttpWebRequest httpWebRequest2 = httpWebRequest;
				httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true));
			}
			return webRequest;
		}
	}
}
