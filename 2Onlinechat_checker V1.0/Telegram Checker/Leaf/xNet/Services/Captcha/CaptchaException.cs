using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x0200007F RID: 127
	[ComVisible(true)]
	public class CaptchaException : Exception
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x00023D78 File Offset: 0x00021F78
		public override string Message
		{
			get
			{
				if (this.Error != CaptchaError.CustomMessage)
				{
					return this.Error.ToString();
				}
				return this._message;
			}
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x00023DA0 File Offset: 0x00021FA0
		public CaptchaException(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				this.Error = CaptchaError.Unknown;
				return;
			}
			this._message = message;
			this.Error = CaptchaError.CustomMessage;
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x00023DCC File Offset: 0x00021FCC
		public CaptchaException(CaptchaError error)
		{
			this.Error = error;
		}

		// Token: 0x040002D9 RID: 729
		public readonly CaptchaError Error;

		// Token: 0x040002DA RID: 730
		private readonly string _message;
	}
}
