using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace Leaf.xNet
{
	// Token: 0x02000073 RID: 115
	[ComVisible(true)]
	public abstract class ProxyClient : IEquatable<ProxyClient>
	{
		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00021B88 File Offset: 0x0001FD88
		public ProxyType Type
		{
			get
			{
				return this._type;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x00021B90 File Offset: 0x0001FD90
		public string Host { get; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x00021B98 File Offset: 0x0001FD98
		public int Port { get; } = 1;

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x00021BA0 File Offset: 0x0001FDA0
		// (set) Token: 0x060005EF RID: 1519 RVA: 0x00021BA8 File Offset: 0x0001FDA8
		public string Username
		{
			get
			{
				return this._username;
			}
			set
			{
				if (value != null && value.Length > 255)
				{
					throw new ArgumentOutOfRangeException("Username", string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, 255));
				}
				this._username = value;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x00021BE8 File Offset: 0x0001FDE8
		// (set) Token: 0x060005F1 RID: 1521 RVA: 0x00021BF0 File Offset: 0x0001FDF0
		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				if (value != null && value.Length > 255)
				{
					throw new ArgumentOutOfRangeException("Password", string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, 255));
				}
				this._password = value;
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x00021C30 File Offset: 0x0001FE30
		// (set) Token: 0x060005F3 RID: 1523 RVA: 0x00021C38 File Offset: 0x0001FE38
		public int ConnectTimeout
		{
			get
			{
				return this._connectTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess<int>("ConnectTimeout", 0);
				}
				this._connectTimeout = value;
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x00021C54 File Offset: 0x0001FE54
		// (set) Token: 0x060005F5 RID: 1525 RVA: 0x00021C5C File Offset: 0x0001FE5C
		public int ReadWriteTimeout
		{
			get
			{
				return this._readWriteTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess<int>("ReadWriteTimeout", 0);
				}
				this._readWriteTimeout = value;
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060005F6 RID: 1526 RVA: 0x00021C78 File Offset: 0x0001FE78
		// (set) Token: 0x060005F7 RID: 1527 RVA: 0x00021C80 File Offset: 0x0001FE80
		public bool AbsoluteUriInStartingLine { get; set; }

		// Token: 0x060005F8 RID: 1528 RVA: 0x00021C8C File Offset: 0x0001FE8C
		protected internal ProxyClient(ProxyType proxyType)
		{
			this._type = proxyType;
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00021CB8 File Offset: 0x0001FEB8
		protected internal ProxyClient(ProxyType proxyType, string address, int port)
		{
			this._type = proxyType;
			this.Host = address;
			this.Port = port;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x00021CF4 File Offset: 0x0001FEF4
		protected internal ProxyClient(ProxyType proxyType, string address, int port, string username, string password)
		{
			this._type = proxyType;
			this.Host = address;
			this.Port = port;
			this._username = username;
			this._password = password;
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060005FB RID: 1531 RVA: 0x00021D50 File Offset: 0x0001FF50
		public static HttpProxyClient DebugHttpProxy
		{
			get
			{
				if (ProxyClient._debugHttpProxy != null)
				{
					return ProxyClient._debugHttpProxy;
				}
				ProxyClient._debugHttpProxy = HttpProxyClient.Parse("127.0.0.1:8888");
				return ProxyClient._debugHttpProxy;
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x00021D78 File Offset: 0x0001FF78
		public static Socks5ProxyClient DebugSocksProxy
		{
			get
			{
				Socks5ProxyClient socks5ProxyClient;
				if ((socks5ProxyClient = ProxyClient._debugSocksProxy) == null)
				{
					socks5ProxyClient = (ProxyClient._debugSocksProxy = Socks5ProxyClient.Parse("127.0.0.1:8889"));
				}
				return socks5ProxyClient;
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00021D98 File Offset: 0x0001FF98
		public static ProxyClient Parse(ProxyType proxyType, string proxyAddress)
		{
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			if (proxyAddress.Length == 0)
			{
				throw ExceptionHelper.EmptyString("proxyAddress");
			}
			string[] array = proxyAddress.Split(new char[] { ':' });
			int num = 0;
			string text = array[0];
			if (array.Length >= 2)
			{
				try
				{
					num = int.Parse(array[1]);
				}
				catch (Exception ex)
				{
					if (ex is FormatException || ex is OverflowException)
					{
						throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort, ex);
					}
					throw;
				}
				if (!ExceptionHelper.ValidateTcpPort(num))
				{
					throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort);
				}
			}
			string text2 = null;
			string text3 = null;
			if (array.Length >= 3)
			{
				text2 = array[2];
			}
			if (array.Length >= 4)
			{
				text3 = array[3];
			}
			return ProxyHelper.CreateProxyClient(proxyType, text, num, text2, text3);
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00021E84 File Offset: 0x00020084
		public static ProxyClient Parse(string protoProxyAddress)
		{
			string[] array = protoProxyAddress.Split(new string[] { "://" }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2)
			{
				return null;
			}
			string text = array[0];
			if (!ProxyClient.ProxyProtocol.ContainsKey(text))
			{
				return null;
			}
			return ProxyClient.Parse(ProxyClient.ProxyProtocol[text], array[1]);
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00021EE8 File Offset: 0x000200E8
		public static bool TryParse(ProxyType proxyType, string proxyAddress, out ProxyClient result)
		{
			result = null;
			if (string.IsNullOrEmpty(proxyAddress))
			{
				return false;
			}
			string[] array = proxyAddress.Split(new char[] { ':' });
			int num = 0;
			string text = array[0];
			if (array.Length >= 2 && (!int.TryParse(array[1], out num) || !ExceptionHelper.ValidateTcpPort(num)))
			{
				return false;
			}
			string text2 = null;
			string text3 = null;
			if (array.Length >= 3)
			{
				text2 = array[2];
			}
			if (array.Length >= 4)
			{
				text3 = array[3];
			}
			try
			{
				result = ProxyHelper.CreateProxyClient(proxyType, text, num, text2, text3);
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00021FA4 File Offset: 0x000201A4
		public static bool TryParse(string protoProxyAddress, out ProxyClient result)
		{
			string[] array = protoProxyAddress.Split(new string[] { "://" }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2 || !ProxyClient.ProxyProtocol.ContainsKey(array[0]))
			{
				result = null;
				return false;
			}
			return ProxyClient.TryParse(ProxyClient.ProxyProtocol[array[0]], array[1], out result);
		}

		// Token: 0x06000601 RID: 1537
		public abstract TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null);

		// Token: 0x06000602 RID: 1538 RVA: 0x00022010 File Offset: 0x00020210
		public override string ToString()
		{
			return string.Format("{0}:{1}", this.Host, this.Port);
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x00022030 File Offset: 0x00020230
		public string ToExtendedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}:{1}", this.Host, this.Port);
			if (string.IsNullOrEmpty(this._username))
			{
				return stringBuilder.ToString();
			}
			stringBuilder.AppendFormat(":{0}", this._username);
			if (!string.IsNullOrEmpty(this._password))
			{
				stringBuilder.AppendFormat(":{0}", this._password);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x000220B8 File Offset: 0x000202B8
		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(this.Host))
			{
				return 0;
			}
			return this.Host.GetHashCode() ^ this.Port;
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x000220F0 File Offset: 0x000202F0
		public bool Equals(ProxyClient proxy)
		{
			return proxy != null && this.Host != null && this.Host.Equals(proxy.Host, StringComparison.OrdinalIgnoreCase) && this.Port == proxy.Port;
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0002213C File Offset: 0x0002033C
		public override bool Equals(object obj)
		{
			ProxyClient proxyClient = obj as ProxyClient;
			return proxyClient != null && this.Equals(proxyClient);
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00022164 File Offset: 0x00020364
		protected TcpClient CreateConnectionToProxy()
		{
			TcpClient tcpClient = new TcpClient();
			Exception connectException = null;
			ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
			try
			{
				tcpClient.BeginConnect(this.Host, this.Port, delegate(IAsyncResult ar)
				{
					if (tcpClient.Client == null)
					{
						return;
					}
					try
					{
						tcpClient.EndConnect(ar);
					}
					catch (Exception ex2)
					{
						connectException = ex2;
					}
					connectDoneEvent.Set();
				}, tcpClient);
			}
			catch (Exception ex)
			{
				tcpClient.Close();
				if (ex is SocketException || ex is SecurityException)
				{
					throw this.NewProxyException(Resources.ProxyException_FailedConnect, ex);
				}
				throw;
			}
			if (!connectDoneEvent.Wait(this._connectTimeout))
			{
				tcpClient.Close();
				throw this.NewProxyException(Resources.ProxyException_ConnectTimeout, null);
			}
			if (connectException != null)
			{
				tcpClient.Close();
				if (connectException is SocketException)
				{
					throw this.NewProxyException(Resources.ProxyException_FailedConnect, connectException);
				}
				throw connectException;
			}
			else
			{
				if (!tcpClient.Connected)
				{
					tcpClient.Close();
					throw this.NewProxyException(Resources.ProxyException_FailedConnect, null);
				}
				tcpClient.SendTimeout = this._readWriteTimeout;
				tcpClient.ReceiveTimeout = this._readWriteTimeout;
				return tcpClient;
			}
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x000222C4 File Offset: 0x000204C4
		protected void CheckState()
		{
			if (string.IsNullOrEmpty(this.Host))
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongHost);
			}
			if (!ExceptionHelper.ValidateTcpPort(this.Port))
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPort);
			}
			if (this._username != null && this._username.Length > 255)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongUsername);
			}
			if (this._password != null && this._password.Length > 255)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPassword);
			}
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00022364 File Offset: 0x00020564
		protected ProxyException NewProxyException(string message, Exception innerException = null)
		{
			return new ProxyException(string.Format(message, this.ToString()), this, innerException);
		}

		// Token: 0x0400028B RID: 651
		protected ProxyType _type;

		// Token: 0x0400028C RID: 652
		protected string _username;

		// Token: 0x0400028D RID: 653
		protected string _password;

		// Token: 0x0400028E RID: 654
		private int _connectTimeout = 9000;

		// Token: 0x0400028F RID: 655
		private int _readWriteTimeout = 30000;

		// Token: 0x04000293 RID: 659
		private static HttpProxyClient _debugHttpProxy;

		// Token: 0x04000294 RID: 660
		private static Socks5ProxyClient _debugSocksProxy;

		// Token: 0x04000295 RID: 661
		public static readonly Dictionary<string, ProxyType> ProxyProtocol = new Dictionary<string, ProxyType>
		{
			{
				"http",
				ProxyType.HTTP
			},
			{
				"https",
				ProxyType.HTTP
			},
			{
				"socks4",
				ProxyType.Socks4
			},
			{
				"socks4a",
				ProxyType.Socks4A
			},
			{
				"socks5",
				ProxyType.Socks5
			},
			{
				"socks",
				ProxyType.Socks5
			}
		};
	}
}
