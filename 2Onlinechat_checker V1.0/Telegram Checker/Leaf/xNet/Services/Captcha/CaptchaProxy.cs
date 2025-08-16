using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x02000081 RID: 129
	[ComVisible(true)]
	public struct CaptchaProxy
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000677 RID: 1655 RVA: 0x00023DDC File Offset: 0x00021FDC
		public bool IsValid
		{
			get
			{
				return !object.Equals(this, default(CaptchaProxy)) && !string.IsNullOrEmpty(this.Address);
			}
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00023E20 File Offset: 0x00022020
		public CaptchaProxy(CaptchaProxyType type, string address)
		{
			CaptchaProxy.Validate(type, address);
			this.Type = type;
			this.Address = address;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x00023E38 File Offset: 0x00022038
		public CaptchaProxy(string type, string address)
		{
			CaptchaProxyType captchaProxyType;
			if (!Enum.TryParse<CaptchaProxyType>(type.Trim().ToUpper(), out captchaProxyType))
			{
				throw new ArgumentException("Proxy type is invalid. Available: HTTP, HTTPS, SOCKS4, SOCKS5", "address");
			}
			CaptchaProxy.Validate(captchaProxyType, address);
			this.Type = captchaProxyType;
			this.Address = address;
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00023E88 File Offset: 0x00022088
		private static void Validate(CaptchaProxyType type, string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentException("CaptchaProxy should contain address", "address");
			}
			int num = address.IndexOf(':');
			if (num == -1 || address.Length - 1 - num < 2)
			{
				throw new ArgumentException("address should contain port", "address");
			}
		}

		// Token: 0x040002E0 RID: 736
		public readonly CaptchaProxyType Type;

		// Token: 0x040002E1 RID: 737
		public readonly string Address;
	}
}
