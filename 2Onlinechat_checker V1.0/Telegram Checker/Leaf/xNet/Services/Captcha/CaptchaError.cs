using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x0200007E RID: 126
	[ComVisible(true)]
	public enum CaptchaError
	{
		// Token: 0x040002D5 RID: 725
		Unknown,
		// Token: 0x040002D6 RID: 726
		CustomMessage,
		// Token: 0x040002D7 RID: 727
		InvalidApiKey,
		// Token: 0x040002D8 RID: 728
		EmptyResponse
	}
}
