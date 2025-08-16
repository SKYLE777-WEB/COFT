using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x02000085 RID: 133
	[ComVisible(true)]
	public class TwoCaptchaSolver : RucaptchaSolver
	{
		// Token: 0x06000691 RID: 1681 RVA: 0x000241C4 File Offset: 0x000223C4
		public TwoCaptchaSolver()
		{
			base.Host = "2captcha.com";
		}
	}
}
