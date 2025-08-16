using System;

namespace Leaf.xNet
{
	// Token: 0x02000071 RID: 113
	internal static class ProxyHelper
	{
		// Token: 0x060005DD RID: 1501 RVA: 0x0002172C File Offset: 0x0001F92C
		public static ProxyClient CreateProxyClient(ProxyType proxyType, string host = null, int port = 0, string username = null, string password = null)
		{
			switch (proxyType)
			{
			case ProxyType.HTTP:
				if (port != 0)
				{
					return new HttpProxyClient(host, port, username, password);
				}
				return new HttpProxyClient(host, 8080);
			case ProxyType.Socks4:
				if (port != 0)
				{
					return new Socks4ProxyClient(host, port, username);
				}
				return new Socks4ProxyClient(host, 1080);
			case ProxyType.Socks4A:
				if (port != 0)
				{
					return new Socks4AProxyClient(host, port, username);
				}
				return new Socks4AProxyClient(host, 1080);
			case ProxyType.Socks5:
				if (port != 0)
				{
					return new Socks5ProxyClient(host, port, username, password);
				}
				return new Socks5ProxyClient(host, 1080);
			default:
				throw new InvalidOperationException();
			}
		}
	}
}
