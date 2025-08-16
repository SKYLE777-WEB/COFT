using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x02000080 RID: 128
	[ComVisible(true)]
	public enum CaptchaProxyType
	{
		// Token: 0x040002DC RID: 732
		HTTP,
		// Token: 0x040002DD RID: 733
		HTTPS,
		// Token: 0x040002DE RID: 734
		SOCKS4,
		// Token: 0x040002DF RID: 735
		SOCKS5
	}
}
