using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Leaf.xNet
{
	// Token: 0x02000076 RID: 118
	[ComVisible(true)]
	public sealed class Socks4AProxyClient : Socks4ProxyClient
	{
		// Token: 0x06000610 RID: 1552 RVA: 0x00022424 File Offset: 0x00020624
		public Socks4AProxyClient()
			: this(null, 1080)
		{
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x00022434 File Offset: 0x00020634
		public Socks4AProxyClient(string host, int port = 1080)
			: this(host, port, string.Empty)
		{
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00022444 File Offset: 0x00020644
		public Socks4AProxyClient(string host, int port, string username)
			: base(host, port, username)
		{
			this._type = ProxyType.Socks4A;
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00022458 File Offset: 0x00020658
		public new static Socks4AProxyClient Parse(string proxyAddress)
		{
			return ProxyClient.Parse(ProxyType.Socks4A, proxyAddress) as Socks4AProxyClient;
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00022468 File Offset: 0x00020668
		public static bool TryParse(string proxyAddress, out Socks4AProxyClient result)
		{
			ProxyClient proxyClient;
			if (!ProxyClient.TryParse(ProxyType.Socks4A, proxyAddress, out proxyClient))
			{
				result = null;
				return false;
			}
			result = proxyClient as Socks4AProxyClient;
			return true;
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00022498 File Offset: 0x00020698
		internal void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
		{
			byte[] portBytes = Socks4ProxyClient.GetPortBytes(destinationPort);
			Array array = new byte[] { 0, 0, 0, 1 };
			byte[] array2 = (string.IsNullOrEmpty(this._username) ? new byte[0] : Encoding.ASCII.GetBytes(this._username));
			byte[] bytes = Encoding.ASCII.GetBytes(destinationHost);
			byte[] array3 = new byte[10 + array2.Length + bytes.Length];
			array3[0] = 4;
			array3[1] = command;
			portBytes.CopyTo(array3, 2);
			array.CopyTo(array3, 4);
			array2.CopyTo(array3, 8);
			array3[8 + array2.Length] = 0;
			bytes.CopyTo(array3, 9 + array2.Length);
			array3[9 + array2.Length + bytes.Length] = 0;
			nStream.Write(array3, 0, array3.Length);
			byte[] array4 = new byte[8];
			nStream.Read(array4, 0, 8);
			byte b = array4[1];
			if (b != 90)
			{
				base.HandleCommandError(b);
			}
		}
	}
}
