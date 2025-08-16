using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Leaf.xNet
{
	// Token: 0x02000077 RID: 119
	[ComVisible(true)]
	public class Socks4ProxyClient : ProxyClient
	{
		// Token: 0x06000616 RID: 1558 RVA: 0x0002257C File Offset: 0x0002077C
		public Socks4ProxyClient()
			: this(null, 1080)
		{
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0002258C File Offset: 0x0002078C
		public Socks4ProxyClient(string host, int port = 1080)
			: this(host, port, string.Empty)
		{
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x0002259C File Offset: 0x0002079C
		public Socks4ProxyClient(string host, int port, string username)
			: base(ProxyType.Socks4, host, port, username, null)
		{
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x000225AC File Offset: 0x000207AC
		public new static Socks4ProxyClient Parse(string proxyAddress)
		{
			return ProxyClient.Parse(ProxyType.Socks4, proxyAddress) as Socks4ProxyClient;
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x000225BC File Offset: 0x000207BC
		public static bool TryParse(string proxyAddress, out Socks4ProxyClient result)
		{
			ProxyClient proxyClient;
			if (!ProxyClient.TryParse(ProxyType.Socks4, proxyAddress, out proxyClient))
			{
				result = null;
				return false;
			}
			result = proxyClient as Socks4ProxyClient;
			return true;
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x000225EC File Offset: 0x000207EC
		public override TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null)
		{
			base.CheckState();
			if (destinationHost == null)
			{
				throw new ArgumentNullException("destinationHost");
			}
			if (destinationHost.Length == 0)
			{
				throw ExceptionHelper.EmptyString("destinationHost");
			}
			if (!ExceptionHelper.ValidateTcpPort(destinationPort))
			{
				throw ExceptionHelper.WrongTcpPort("destinationHost");
			}
			TcpClient tcpClient2 = tcpClient ?? base.CreateConnectionToProxy();
			try
			{
				this.SendCommand(tcpClient2.GetStream(), 1, destinationHost, destinationPort);
			}
			catch (Exception ex)
			{
				tcpClient2.Close();
				if (ex is IOException || ex is SocketException)
				{
					throw base.NewProxyException(Resources.ProxyException_Error, ex);
				}
				throw;
			}
			return tcpClient2;
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0002269C File Offset: 0x0002089C
		private void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
		{
			Array ipAddressBytes = this.GetIpAddressBytes(destinationHost);
			Array portBytes = Socks4ProxyClient.GetPortBytes(destinationPort);
			byte[] array = (string.IsNullOrEmpty(this._username) ? new byte[0] : Encoding.ASCII.GetBytes(this._username));
			byte[] array2 = new byte[9 + array.Length];
			array2[0] = 4;
			array2[1] = command;
			portBytes.CopyTo(array2, 2);
			ipAddressBytes.CopyTo(array2, 4);
			array.CopyTo(array2, 8);
			array2[8 + array.Length] = 0;
			nStream.Write(array2, 0, array2.Length);
			byte[] array3 = new byte[8];
			nStream.Read(array3, 0, array3.Length);
			byte b = array3[1];
			if (b != 90)
			{
				this.HandleCommandError(b);
			}
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0002274C File Offset: 0x0002094C
		private byte[] GetIpAddressBytes(string destinationHost)
		{
			IPAddress ipaddress;
			if (IPAddress.TryParse(destinationHost, out ipaddress))
			{
				return ipaddress.GetAddressBytes();
			}
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(destinationHost);
				if (hostAddresses.Length != 0)
				{
					ipaddress = hostAddresses[0];
				}
			}
			catch (Exception ex)
			{
				if (ex is SocketException || ex is ArgumentException)
				{
					throw new ProxyException(string.Format(Resources.ProxyException_FailedGetHostAddresses, destinationHost), this, ex);
				}
				throw;
			}
			return ipaddress.GetAddressBytes();
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x000227CC File Offset: 0x000209CC
		protected static byte[] GetPortBytes(int port)
		{
			return new byte[]
			{
				(byte)(port / 256),
				(byte)(port % 256)
			};
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x000227EC File Offset: 0x000209EC
		protected void HandleCommandError(byte command)
		{
			string text;
			switch (command)
			{
			case 91:
				text = Resources.Socks4_CommandReplyRequestRejectedOrFailed;
				break;
			case 92:
				text = Resources.Socks4_CommandReplyRequestRejectedCannotConnectToIdentd;
				break;
			case 93:
				text = Resources.Socks4_CommandReplyRequestRejectedDifferentIdentd;
				break;
			default:
				text = Resources.Socks_UnknownError;
				break;
			}
			throw new ProxyException(string.Format(Resources.ProxyException_CommandError, text, this.ToString()), this, null);
		}

		// Token: 0x0400029C RID: 668
		protected const int DefaultPort = 1080;

		// Token: 0x0400029D RID: 669
		protected const byte VersionNumber = 4;

		// Token: 0x0400029E RID: 670
		private const byte CommandConnect = 1;

		// Token: 0x0400029F RID: 671
		protected const byte CommandReplyRequestGranted = 90;

		// Token: 0x040002A0 RID: 672
		private const byte CommandReplyRequestRejectedOrFailed = 91;

		// Token: 0x040002A1 RID: 673
		private const byte CommandReplyRequestRejectedCannotConnectToIdentd = 92;

		// Token: 0x040002A2 RID: 674
		private const byte CommandReplyRequestRejectedDifferentIdentd = 93;
	}
}
