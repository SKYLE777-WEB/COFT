using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x02000083 RID: 131
	[ComVisible(true)]
	public class CapmonsterSolver : RucaptchaSolver
	{
		// Token: 0x0600068A RID: 1674 RVA: 0x00023EE4 File Offset: 0x000220E4
		public CapmonsterSolver(string host = "127.0.0.3:80")
		{
			base.Host = host;
			base.IsApiKeyRequired = false;
		}
	}
}
