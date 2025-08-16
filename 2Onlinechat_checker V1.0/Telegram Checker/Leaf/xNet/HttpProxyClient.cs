using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Leaf.xNet
{
	// Token: 0x02000072 RID: 114
	[ComVisible(true)]
	public sealed class HttpProxyClient : ProxyClient
	{
		// Token: 0x060005DE RID: 1502 RVA: 0x000217D0 File Offset: 0x0001F9D0
		public HttpProxyClient()
			: this(null, 8080)
		{
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x000217E0 File Offset: 0x0001F9E0
		public HttpProxyClient(string host, int port = 8080)
			: this(host, port, string.Empty, string.Empty)
		{
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x000217F4 File Offset: 0x0001F9F4
		public HttpProxyClient(string host, int port, string username, string password)
			: base(ProxyType.HTTP, host, port, username, password)
		{
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x00021804 File Offset: 0x0001FA04
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x0002180C File Offset: 0x0001FA0C
		public static string ProtocolVersion { get; set; } = "1.1";

		// Token: 0x060005E3 RID: 1507 RVA: 0x00021814 File Offset: 0x0001FA14
		public new static HttpProxyClient Parse(string proxyAddress)
		{
			return ProxyClient.Parse(ProxyType.HTTP, proxyAddress) as HttpProxyClient;
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x00021824 File Offset: 0x0001FA24
		public static bool TryParse(string proxyAddress, out HttpProxyClient result)
		{
			ProxyClient proxyClient;
			if (!ProxyClient.TryParse(ProxyType.HTTP, proxyAddress, out proxyClient))
			{
				result = null;
				return false;
			}
			result = proxyClient as HttpProxyClient;
			return true;
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x00021854 File Offset: 0x0001FA54
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
			if (destinationPort == 80)
			{
				return tcpClient2;
			}
			HttpStatusCode httpStatusCode;
			try
			{
				NetworkStream stream = tcpClient2.GetStream();
				this.SendConnectionCommand(stream, destinationHost, destinationPort);
				httpStatusCode = this.ReceiveResponse(stream);
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
			if (httpStatusCode == HttpStatusCode.OK)
			{
				return tcpClient2;
			}
			tcpClient2.Close();
			throw new ProxyException(string.Format(Resources.ProxyException_ReceivedWrongStatusCode, httpStatusCode, this.ToString()), this, null);
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00021948 File Offset: 0x0001FB48
		private string GenerateAuthorizationHeader()
		{
			if (string.IsNullOrEmpty(this._username) && string.IsNullOrEmpty(this._password))
			{
				return string.Empty;
			}
			string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(this._username + ":" + this._password));
			return "Proxy-Authorization: Basic " + text + "\r\n";
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x000219B8 File Offset: 0x0001FBB8
		private void SendConnectionCommand(Stream nStream, string destinationHost, int destinationPort)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("CONNECT {0}:{1} HTTP/{2}\r\n", destinationHost, destinationPort, HttpProxyClient.ProtocolVersion);
			stringBuilder.AppendFormat(this.GenerateAuthorizationHeader(), new object[0]);
			stringBuilder.Append("Host: ");
			stringBuilder.AppendLine(destinationHost);
			stringBuilder.AppendLine("Proxy-Connection: Keep-Alive");
			stringBuilder.AppendLine();
			byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			nStream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00021A40 File Offset: 0x0001FC40
		private HttpStatusCode ReceiveResponse(NetworkStream nStream)
		{
			byte[] array = new byte[50];
			StringBuilder stringBuilder = new StringBuilder();
			this.WaitData(nStream);
			do
			{
				int num = nStream.Read(array, 0, 50);
				stringBuilder.Append(Encoding.ASCII.GetString(array, 0, num));
			}
			while (nStream.DataAvailable);
			string text = stringBuilder.ToString();
			if (text.Length == 0)
			{
				throw base.NewProxyException(Resources.ProxyException_ReceivedEmptyResponse, null);
			}
			string text2 = text.Substring(" ", "\r\n", 0, StringComparison.Ordinal, null);
			if (text2 == null)
			{
				throw base.NewProxyException(Resources.ProxyException_ReceivedWrongResponse, null);
			}
			int num2 = text2.IndexOf(' ');
			if (num2 == -1)
			{
				throw base.NewProxyException(Resources.ProxyException_ReceivedWrongResponse, null);
			}
			string text3 = text2.Substring(0, num2);
			if (text3.Length == 0)
			{
				throw base.NewProxyException(Resources.ProxyException_ReceivedWrongResponse, null);
			}
			HttpStatusCode httpStatusCode;
			if (!Enum.TryParse<HttpStatusCode>(text3, out httpStatusCode))
			{
				return HttpStatusCode.InvalidStatusCode;
			}
			return httpStatusCode;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00021B20 File Offset: 0x0001FD20
		private void WaitData(NetworkStream nStream)
		{
			int num = 0;
			int num2 = ((nStream.ReadTimeout < 10) ? 10 : nStream.ReadTimeout);
			while (!nStream.DataAvailable)
			{
				if (num >= num2)
				{
					throw base.NewProxyException(Resources.ProxyException_WaitDataTimeout, null);
				}
				num += 10;
				Thread.Sleep(10);
			}
		}

		// Token: 0x04000288 RID: 648
		private const int BufferSize = 50;

		// Token: 0x04000289 RID: 649
		private const int DefaultPort = 8080;
	}
}
