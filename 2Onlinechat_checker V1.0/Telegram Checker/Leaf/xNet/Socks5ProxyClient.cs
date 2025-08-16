using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Leaf.xNet
{
	// Token: 0x02000078 RID: 120
	[ComVisible(true)]
	public sealed class Socks5ProxyClient : ProxyClient
	{
		// Token: 0x06000620 RID: 1568 RVA: 0x00022858 File Offset: 0x00020A58
		public Socks5ProxyClient()
			: this(null, 1080)
		{
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00022868 File Offset: 0x00020A68
		public Socks5ProxyClient(string host, int port = 1080)
			: this(host, port, string.Empty, string.Empty)
		{
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0002287C File Offset: 0x00020A7C
		public Socks5ProxyClient(string host, int port, string username, string password)
			: base(ProxyType.Socks5, host, port, username, password)
		{
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0002288C File Offset: 0x00020A8C
		public new static Socks5ProxyClient Parse(string proxyAddress)
		{
			return ProxyClient.Parse(ProxyType.Socks5, proxyAddress) as Socks5ProxyClient;
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0002289C File Offset: 0x00020A9C
		public static bool TryParse(string proxyAddress, out Socks5ProxyClient result)
		{
			ProxyClient proxyClient;
			if (!ProxyClient.TryParse(ProxyType.Socks5, proxyAddress, out proxyClient))
			{
				result = null;
				return false;
			}
			result = proxyClient as Socks5ProxyClient;
			return true;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x000228CC File Offset: 0x00020ACC
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
				NetworkStream stream = tcpClient2.GetStream();
				this.InitialNegotiation(stream);
				this.SendCommand(stream, 1, destinationHost, destinationPort);
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

		// Token: 0x06000626 RID: 1574 RVA: 0x00022988 File Offset: 0x00020B88
		private void InitialNegotiation(Stream nStream)
		{
			byte b = ((!string.IsNullOrEmpty(this._username) && !string.IsNullOrEmpty(this._password)) ? 2 : 0);
			byte[] array = new byte[] { 5, 1, b };
			nStream.Write(array, 0, array.Length);
			byte[] array2 = new byte[2];
			nStream.Read(array2, 0, array2.Length);
			byte b2 = array2[1];
			if (b == 2 && b2 == 2)
			{
				this.SendUsernameAndPassword(nStream);
				return;
			}
			if (b2 != 0)
			{
				this.HandleCommandError(b2);
			}
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00022A1C File Offset: 0x00020C1C
		private void SendUsernameAndPassword(Stream nStream)
		{
			byte[] array = (string.IsNullOrEmpty(this._username) ? new byte[0] : Encoding.ASCII.GetBytes(this._username));
			byte[] array2 = (string.IsNullOrEmpty(this._password) ? new byte[0] : Encoding.ASCII.GetBytes(this._password));
			byte[] array3 = new byte[array.Length + array2.Length + 3];
			array3[0] = 1;
			array3[1] = (byte)array.Length;
			array.CopyTo(array3, 2);
			array3[2 + array.Length] = (byte)array2.Length;
			array2.CopyTo(array3, 3 + array.Length);
			nStream.Write(array3, 0, array3.Length);
			byte[] array4 = new byte[2];
			nStream.Read(array4, 0, array4.Length);
			if (array4[1] != 0)
			{
				throw base.NewProxyException(Resources.ProxyException_Socks5_FailedAuthOn, null);
			}
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00022AF4 File Offset: 0x00020CF4
		private void SendCommand(Stream nStream, byte command, string destinationHost, int destinationPort)
		{
			byte addressType = this.GetAddressType(destinationHost);
			byte[] addressBytes = Socks5ProxyClient.GetAddressBytes(addressType, destinationHost);
			Array portBytes = Socks5ProxyClient.GetPortBytes(destinationPort);
			byte[] array = new byte[4 + addressBytes.Length + 2];
			array[0] = 5;
			array[1] = command;
			array[2] = 0;
			array[3] = addressType;
			addressBytes.CopyTo(array, 4);
			portBytes.CopyTo(array, 4 + addressBytes.Length);
			nStream.Write(array, 0, array.Length);
			byte[] array2 = new byte[255];
			nStream.Read(array2, 0, array2.Length);
			byte b = array2[1];
			if (b != 0)
			{
				this.HandleCommandError(b);
			}
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00022B84 File Offset: 0x00020D84
		private byte GetAddressType(string host)
		{
			IPAddress ipaddress;
			if (!IPAddress.TryParse(host, out ipaddress))
			{
				return 3;
			}
			AddressFamily addressFamily = ipaddress.AddressFamily;
			if (addressFamily == AddressFamily.InterNetwork)
			{
				return 1;
			}
			if (addressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ProxyException(string.Format(Resources.ProxyException_NotSupportedAddressType, host, Enum.GetName(typeof(AddressFamily), ipaddress.AddressFamily), this.ToString()), this, null);
			}
			return 4;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00022BF8 File Offset: 0x00020DF8
		private static byte[] GetAddressBytes(byte addressType, string host)
		{
			switch (addressType)
			{
			case 1:
			case 4:
				return IPAddress.Parse(host).GetAddressBytes();
			case 3:
			{
				byte[] array = new byte[host.Length + 1];
				array[0] = (byte)host.Length;
				Encoding.ASCII.GetBytes(host).CopyTo(array, 1);
				return array;
			}
			}
			return null;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00022C60 File Offset: 0x00020E60
		private static byte[] GetPortBytes(int port)
		{
			return new byte[]
			{
				(byte)(port / 256),
				(byte)(port % 256)
			};
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00022C80 File Offset: 0x00020E80
		private void HandleCommandError(byte command)
		{
			string text;
			switch (command)
			{
			case 1:
				text = Resources.Socks5_CommandReplyGeneralSocksServerFailure;
				break;
			case 2:
				text = Resources.Socks5_CommandReplyConnectionNotAllowedByRuleset;
				break;
			case 3:
				text = Resources.Socks5_CommandReplyNetworkUnreachable;
				break;
			case 4:
				text = Resources.Socks5_CommandReplyHostUnreachable;
				break;
			case 5:
				text = Resources.Socks5_CommandReplyConnectionRefused;
				break;
			case 6:
				text = Resources.Socks5_CommandReplyTTLExpired;
				break;
			case 7:
				text = Resources.Socks5_CommandReplyCommandNotSupported;
				break;
			case 8:
				text = Resources.Socks5_CommandReplyAddressTypeNotSupported;
				break;
			default:
				if (command == 255)
				{
					text = Resources.Socks5_AuthMethodReplyNoAcceptableMethods;
				}
				else
				{
					text = Resources.Socks_UnknownError;
				}
				break;
			}
			throw new ProxyException(string.Format(Resources.ProxyException_CommandError, text, this.ToString()), this, null);
		}

		// Token: 0x040002A3 RID: 675
		private const int DefaultPort = 1080;

		// Token: 0x040002A4 RID: 676
		private const byte VersionNumber = 5;

		// Token: 0x040002A5 RID: 677
		private const byte Reserved = 0;

		// Token: 0x040002A6 RID: 678
		private const byte AuthMethodNoAuthenticationRequired = 0;

		// Token: 0x040002A7 RID: 679
		private const byte AuthMethodUsernamePassword = 2;

		// Token: 0x040002A8 RID: 680
		private const byte AuthMethodReplyNoAcceptableMethods = 255;

		// Token: 0x040002A9 RID: 681
		private const byte CommandConnect = 1;

		// Token: 0x040002AA RID: 682
		private const byte CommandReplySucceeded = 0;

		// Token: 0x040002AB RID: 683
		private const byte CommandReplyGeneralSocksServerFailure = 1;

		// Token: 0x040002AC RID: 684
		private const byte CommandReplyConnectionNotAllowedByRuleset = 2;

		// Token: 0x040002AD RID: 685
		private const byte CommandReplyNetworkUnreachable = 3;

		// Token: 0x040002AE RID: 686
		private const byte CommandReplyHostUnreachable = 4;

		// Token: 0x040002AF RID: 687
		private const byte CommandReplyConnectionRefused = 5;

		// Token: 0x040002B0 RID: 688
		private const byte CommandReplyTTLExpired = 6;

		// Token: 0x040002B1 RID: 689
		private const byte CommandReplyCommandNotSupported = 7;

		// Token: 0x040002B2 RID: 690
		private const byte CommandReplyAddressTypeNotSupported = 8;

		// Token: 0x040002B3 RID: 691
		private const byte AddressTypeIPv4 = 1;

		// Token: 0x040002B4 RID: 692
		private const byte AddressTypeDomainName = 3;

		// Token: 0x040002B5 RID: 693
		private const byte AddressTypeIPv6 = 4;
	}
}
