using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Leaf.xNet.Services.Captcha;

namespace Leaf.xNet
{
	// Token: 0x02000062 RID: 98
	[ComVisible(true)]
	public sealed class HttpRequest : IDisposable
	{
		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x0001C9F8 File Offset: 0x0001ABF8
		// (set) Token: 0x0600048B RID: 1163 RVA: 0x0001CA00 File Offset: 0x0001AC00
		public static Version ProtocolVersion { get; set; } = new Version(1, 1);

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x0001CA08 File Offset: 0x0001AC08
		// (set) Token: 0x0600048D RID: 1165 RVA: 0x0001CA10 File Offset: 0x0001AC10
		public static bool DisableProxyForLocalAddress { get; set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x0001CA18 File Offset: 0x0001AC18
		// (set) Token: 0x0600048F RID: 1167 RVA: 0x0001CA20 File Offset: 0x0001AC20
		public static ProxyClient GlobalProxy { get; set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000490 RID: 1168 RVA: 0x0001CA28 File Offset: 0x0001AC28
		// (remove) Token: 0x06000491 RID: 1169 RVA: 0x0001CA44 File Offset: 0x0001AC44
		public event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged
		{
			add
			{
				this._uploadProgressChangedHandler = (EventHandler<UploadProgressChangedEventArgs>)Delegate.Combine(this._uploadProgressChangedHandler, value);
			}
			remove
			{
				this._uploadProgressChangedHandler = (EventHandler<UploadProgressChangedEventArgs>)Delegate.Remove(this._uploadProgressChangedHandler, value);
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000492 RID: 1170 RVA: 0x0001CA60 File Offset: 0x0001AC60
		// (remove) Token: 0x06000493 RID: 1171 RVA: 0x0001CA7C File Offset: 0x0001AC7C
		public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
		{
			add
			{
				this._downloadProgressChangedHandler = (EventHandler<DownloadProgressChangedEventArgs>)Delegate.Combine(this._downloadProgressChangedHandler, value);
			}
			remove
			{
				this._downloadProgressChangedHandler = (EventHandler<DownloadProgressChangedEventArgs>)Delegate.Remove(this._downloadProgressChangedHandler, value);
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x0001CA98 File Offset: 0x0001AC98
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x0001CAA0 File Offset: 0x0001ACA0
		public Uri BaseAddress { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x0001CAAC File Offset: 0x0001ACAC
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x0001CAB4 File Offset: 0x0001ACB4
		public Uri Address { get; private set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x0001CAC0 File Offset: 0x0001ACC0
		// (set) Token: 0x06000499 RID: 1177 RVA: 0x0001CAC8 File Offset: 0x0001ACC8
		public HttpResponse Response { get; private set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x0001CAD4 File Offset: 0x0001ACD4
		// (set) Token: 0x0600049B RID: 1179 RVA: 0x0001CADC File Offset: 0x0001ACDC
		public ProxyClient Proxy { get; set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x0001CAE8 File Offset: 0x0001ACE8
		// (set) Token: 0x0600049D RID: 1181 RVA: 0x0001CAF0 File Offset: 0x0001ACF0
		public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600049E RID: 1182 RVA: 0x0001CAFC File Offset: 0x0001ACFC
		// (set) Token: 0x0600049F RID: 1183 RVA: 0x0001CB04 File Offset: 0x0001AD04
		public RemoteCertificateValidationCallback SslCertificateValidatorCallback { get; set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x0001CB10 File Offset: 0x0001AD10
		// (set) Token: 0x060004A1 RID: 1185 RVA: 0x0001CB18 File Offset: 0x0001AD18
		public bool AllowEmptyHeaderValues { get; set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060004A2 RID: 1186 RVA: 0x0001CB24 File Offset: 0x0001AD24
		// (set) Token: 0x060004A3 RID: 1187 RVA: 0x0001CB2C File Offset: 0x0001AD2C
		public bool KeepTemporaryHeadersOnRedirect { get; set; } = true;

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060004A4 RID: 1188 RVA: 0x0001CB38 File Offset: 0x0001AD38
		// (set) Token: 0x060004A5 RID: 1189 RVA: 0x0001CB40 File Offset: 0x0001AD40
		public bool EnableMiddleHeaders { get; set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060004A6 RID: 1190 RVA: 0x0001CB4C File Offset: 0x0001AD4C
		// (set) Token: 0x060004A7 RID: 1191 RVA: 0x0001CB54 File Offset: 0x0001AD54
		public string AcceptEncoding { get; set; } = "gzip,deflate";

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x0001CB60 File Offset: 0x0001AD60
		// (set) Token: 0x060004A9 RID: 1193 RVA: 0x0001CB68 File Offset: 0x0001AD68
		public bool IgnoreInvalidCookie { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x0001CB74 File Offset: 0x0001AD74
		// (set) Token: 0x060004AB RID: 1195 RVA: 0x0001CB7C File Offset: 0x0001AD7C
		public bool AllowAutoRedirect { get; set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x0001CB88 File Offset: 0x0001AD88
		// (set) Token: 0x060004AD RID: 1197 RVA: 0x0001CBA0 File Offset: 0x0001ADA0
		public bool ManualMode
		{
			get
			{
				return !this.AllowAutoRedirect && this.IgnoreProtocolErrors;
			}
			set
			{
				if (value)
				{
					this._tempAllowAutoRedirect = this.AllowAutoRedirect;
					this._tempIgnoreProtocolErrors = this.IgnoreProtocolErrors;
					this.AllowAutoRedirect = false;
					this.IgnoreProtocolErrors = true;
					return;
				}
				this.AllowAutoRedirect = this._tempAllowAutoRedirect;
				this.IgnoreProtocolErrors = this._tempIgnoreProtocolErrors;
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x0001CBF8 File Offset: 0x0001ADF8
		// (set) Token: 0x060004AF RID: 1199 RVA: 0x0001CC00 File Offset: 0x0001AE00
		public int MaximumAutomaticRedirections
		{
			get
			{
				return this._maximumAutomaticRedirections;
			}
			set
			{
				if (value < 1)
				{
					throw ExceptionHelper.CanNotBeLess<int>("MaximumAutomaticRedirections", 1);
				}
				this._maximumAutomaticRedirections = value;
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x0001CC1C File Offset: 0x0001AE1C
		// (set) Token: 0x060004B1 RID: 1201 RVA: 0x0001CC24 File Offset: 0x0001AE24
		public bool CookieSingleHeader { get; set; } = true;

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0001CC30 File Offset: 0x0001AE30
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x0001CC38 File Offset: 0x0001AE38
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

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x0001CC54 File Offset: 0x0001AE54
		// (set) Token: 0x060004B5 RID: 1205 RVA: 0x0001CC5C File Offset: 0x0001AE5C
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

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x0001CC78 File Offset: 0x0001AE78
		// (set) Token: 0x060004B7 RID: 1207 RVA: 0x0001CC80 File Offset: 0x0001AE80
		public bool IgnoreProtocolErrors { get; set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060004B8 RID: 1208 RVA: 0x0001CC8C File Offset: 0x0001AE8C
		// (set) Token: 0x060004B9 RID: 1209 RVA: 0x0001CC94 File Offset: 0x0001AE94
		public bool KeepAlive { get; set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060004BA RID: 1210 RVA: 0x0001CCA0 File Offset: 0x0001AEA0
		// (set) Token: 0x060004BB RID: 1211 RVA: 0x0001CCA8 File Offset: 0x0001AEA8
		public int KeepAliveTimeout
		{
			get
			{
				return this._keepAliveTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess<int>("KeepAliveTimeout", 0);
				}
				this._keepAliveTimeout = value;
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060004BC RID: 1212 RVA: 0x0001CCC4 File Offset: 0x0001AEC4
		// (set) Token: 0x060004BD RID: 1213 RVA: 0x0001CCCC File Offset: 0x0001AECC
		public int MaximumKeepAliveRequests
		{
			get
			{
				return this._maximumKeepAliveRequests;
			}
			set
			{
				if (value < 1)
				{
					throw ExceptionHelper.CanNotBeLess<int>("MaximumKeepAliveRequests", 1);
				}
				this._maximumKeepAliveRequests = value;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x0001CCE8 File Offset: 0x0001AEE8
		// (set) Token: 0x060004BF RID: 1215 RVA: 0x0001CCF0 File Offset: 0x0001AEF0
		public bool Reconnect { get; set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060004C0 RID: 1216 RVA: 0x0001CCFC File Offset: 0x0001AEFC
		// (set) Token: 0x060004C1 RID: 1217 RVA: 0x0001CD04 File Offset: 0x0001AF04
		public int ReconnectLimit
		{
			get
			{
				return this._reconnectLimit;
			}
			set
			{
				if (value < 1)
				{
					throw ExceptionHelper.CanNotBeLess<int>("ReconnectLimit", 1);
				}
				this._reconnectLimit = value;
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060004C2 RID: 1218 RVA: 0x0001CD20 File Offset: 0x0001AF20
		// (set) Token: 0x060004C3 RID: 1219 RVA: 0x0001CD28 File Offset: 0x0001AF28
		public int ReconnectDelay
		{
			get
			{
				return this._reconnectDelay;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess<int>("ReconnectDelay", 0);
				}
				this._reconnectDelay = value;
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060004C4 RID: 1220 RVA: 0x0001CD44 File Offset: 0x0001AF44
		// (set) Token: 0x060004C5 RID: 1221 RVA: 0x0001CD4C File Offset: 0x0001AF4C
		public CultureInfo Culture { get; set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0001CD58 File Offset: 0x0001AF58
		// (set) Token: 0x060004C7 RID: 1223 RVA: 0x0001CD60 File Offset: 0x0001AF60
		public Encoding CharacterSet { get; set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060004C8 RID: 1224 RVA: 0x0001CD6C File Offset: 0x0001AF6C
		// (set) Token: 0x060004C9 RID: 1225 RVA: 0x0001CD74 File Offset: 0x0001AF74
		public bool EnableEncodingContent { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060004CA RID: 1226 RVA: 0x0001CD80 File Offset: 0x0001AF80
		// (set) Token: 0x060004CB RID: 1227 RVA: 0x0001CD88 File Offset: 0x0001AF88
		public string Username { get; set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x0001CD94 File Offset: 0x0001AF94
		// (set) Token: 0x060004CD RID: 1229 RVA: 0x0001CD9C File Offset: 0x0001AF9C
		public string Password { get; set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060004CE RID: 1230 RVA: 0x0001CDA8 File Offset: 0x0001AFA8
		// (set) Token: 0x060004CF RID: 1231 RVA: 0x0001CDB8 File Offset: 0x0001AFB8
		public string UserAgent
		{
			get
			{
				return this["User-Agent"];
			}
			set
			{
				this["User-Agent"] = value;
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001CDC8 File Offset: 0x0001AFC8
		public void UserAgentRandomize()
		{
			this.UserAgent = Http.RandomUserAgent();
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060004D1 RID: 1233 RVA: 0x0001CDD8 File Offset: 0x0001AFD8
		// (set) Token: 0x060004D2 RID: 1234 RVA: 0x0001CDE8 File Offset: 0x0001AFE8
		public string Referer
		{
			get
			{
				return this["Referer"];
			}
			set
			{
				this["Referer"] = value;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060004D3 RID: 1235 RVA: 0x0001CDF8 File Offset: 0x0001AFF8
		// (set) Token: 0x060004D4 RID: 1236 RVA: 0x0001CE08 File Offset: 0x0001B008
		public string Authorization
		{
			get
			{
				return this["Authorization"];
			}
			set
			{
				this["Authorization"] = value;
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060004D5 RID: 1237 RVA: 0x0001CE18 File Offset: 0x0001B018
		// (set) Token: 0x060004D6 RID: 1238 RVA: 0x0001CE20 File Offset: 0x0001B020
		public CookieStorage Cookies { get; set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x0001CE2C File Offset: 0x0001B02C
		// (set) Token: 0x060004D8 RID: 1240 RVA: 0x0001CE34 File Offset: 0x0001B034
		public bool UseCookies { get; set; } = true;

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x0001CE40 File Offset: 0x0001B040
		// (set) Token: 0x060004DA RID: 1242 RVA: 0x0001CE48 File Offset: 0x0001B048
		public ICaptchaSolver CaptchaSolver { get; set; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x0001CE54 File Offset: 0x0001B054
		// (set) Token: 0x060004DC RID: 1244 RVA: 0x0001CE5C File Offset: 0x0001B05C
		internal TcpClient TcpClient { get; private set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060004DD RID: 1245 RVA: 0x0001CE68 File Offset: 0x0001B068
		// (set) Token: 0x060004DE RID: 1246 RVA: 0x0001CE70 File Offset: 0x0001B070
		internal Stream ClientStream { get; private set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060004DF RID: 1247 RVA: 0x0001CE7C File Offset: 0x0001B07C
		// (set) Token: 0x060004E0 RID: 1248 RVA: 0x0001CE84 File Offset: 0x0001B084
		internal NetworkStream ClientNetworkStream { get; private set; }

		// Token: 0x17000100 RID: 256
		public string this[string headerName]
		{
			get
			{
				if (headerName == null)
				{
					throw new ArgumentNullException("headerName");
				}
				if (headerName.Length == 0)
				{
					throw ExceptionHelper.EmptyString("headerName");
				}
				string empty;
				if (!this._permanentHeaders.TryGetValue(headerName, out empty))
				{
					empty = string.Empty;
				}
				return empty;
			}
			set
			{
				if (headerName == null)
				{
					throw new ArgumentNullException("headerName");
				}
				if (headerName.Length == 0)
				{
					throw ExceptionHelper.EmptyString("headerName");
				}
				if (string.IsNullOrEmpty(value))
				{
					this._permanentHeaders.Remove(headerName);
					return;
				}
				this._permanentHeaders[headerName] = value;
			}
		}

		// Token: 0x17000101 RID: 257
		public string this[HttpHeader header]
		{
			get
			{
				return this[Http.Headers[header]];
			}
			set
			{
				this[Http.Headers[header]] = value;
			}
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0001CF6C File Offset: 0x0001B16C
		public HttpRequest()
		{
			this.Init();
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0001D004 File Offset: 0x0001B204
		static HttpRequest()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(HttpRequest.ServerCertificateValidationCallback));
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0001D03C File Offset: 0x0001B23C
		private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return true;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001D040 File Offset: 0x0001B240
		public HttpRequest(string baseAddress)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (baseAddress.Length == 0)
			{
				throw ExceptionHelper.EmptyString("baseAddress");
			}
			if (!baseAddress.StartsWith("http"))
			{
				baseAddress = "http://" + baseAddress;
			}
			Uri uri = new Uri(baseAddress);
			if (!uri.IsAbsoluteUri)
			{
				throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, "baseAddress");
			}
			this.BaseAddress = uri;
			this.Init();
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0001D144 File Offset: 0x0001B344
		public HttpRequest(Uri baseAddress)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (!baseAddress.IsAbsoluteUri)
			{
				throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, "baseAddress");
			}
			this.BaseAddress = baseAddress;
			this.Init();
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001D214 File Offset: 0x0001B414
		public HttpResponse Get(string address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				address = new UriBuilder(address)
				{
					Query = urlParams.Query
				}.Uri.AbsoluteUri;
			}
			return this.Raw(HttpMethod.GET, address, null);
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0001D254 File Offset: 0x0001B454
		public HttpResponse Get(Uri address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				address = new UriBuilder(address)
				{
					Query = urlParams.Query
				}.Uri;
			}
			return this.Raw(HttpMethod.GET, address, null);
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001D280 File Offset: 0x0001B480
		public HttpResponse Head(string address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				address = new UriBuilder(address)
				{
					Query = urlParams.Query
				}.Uri.AbsoluteUri;
			}
			return this.Raw(HttpMethod.HEAD, address, null);
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001D2C0 File Offset: 0x0001B4C0
		public HttpResponse Head(Uri address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				address = new UriBuilder(address)
				{
					Query = urlParams.Query
				}.Uri;
			}
			return this.Raw(HttpMethod.HEAD, address, null);
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001D2EC File Offset: 0x0001B4EC
		public HttpResponse Options(string address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				address = new UriBuilder(address)
				{
					Query = urlParams.Query
				}.Uri.AbsoluteUri;
			}
			return this.Raw(HttpMethod.OPTIONS, address, null);
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0001D32C File Offset: 0x0001B52C
		public HttpResponse Options(Uri address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				address = new UriBuilder(address)
				{
					Query = urlParams.Query
				}.Uri;
			}
			return this.Raw(HttpMethod.OPTIONS, address, null);
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0001D358 File Offset: 0x0001B558
		public HttpResponse Post(string address)
		{
			return this.Raw(HttpMethod.POST, address, null);
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0001D364 File Offset: 0x0001B564
		public HttpResponse Post(Uri address)
		{
			return this.Raw(HttpMethod.POST, address, null);
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0001D370 File Offset: 0x0001B570
		public HttpResponse Post(string address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.POST, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001D394 File Offset: 0x0001B594
		public HttpResponse Post(Uri address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.POST, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0001D3B8 File Offset: 0x0001B5B8
		public HttpResponse Post(string address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.POST, address, stringContent);
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0001D430 File Offset: 0x0001B630
		public HttpResponse Post(Uri address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.POST, address, stringContent);
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001D4A8 File Offset: 0x0001B6A8
		public HttpResponse Post(string address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.POST, address, bytesContent);
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0001D508 File Offset: 0x0001B708
		public HttpResponse Post(Uri address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.POST, address, bytesContent);
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001D568 File Offset: 0x0001B768
		public HttpResponse Post(string address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.POST, address, streamContent);
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001D5D0 File Offset: 0x0001B7D0
		public HttpResponse Post(Uri address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.POST, address, streamContent);
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x0001D638 File Offset: 0x0001B838
		public HttpResponse Post(string address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.POST, address, new FileContent(path, 32768));
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0001D674 File Offset: 0x0001B874
		public HttpResponse Post(Uri address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.POST, address, new FileContent(path, 32768));
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0001D6B0 File Offset: 0x0001B8B0
		public HttpResponse Post(string address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.POST, address, content);
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001D6CC File Offset: 0x0001B8CC
		public HttpResponse Post(Uri address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.POST, address, content);
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0001D6E8 File Offset: 0x0001B8E8
		public HttpResponse Raw(HttpMethod method, string address, HttpContent content = null)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length == 0)
			{
				throw ExceptionHelper.EmptyString("address");
			}
			Uri uri = new Uri(address, UriKind.RelativeOrAbsolute);
			return this.Raw(method, uri, content);
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0001D734 File Offset: 0x0001B934
		public HttpResponse Raw(HttpMethod method, Uri address, HttpContent content = null)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!address.IsAbsoluteUri)
			{
				address = HttpRequest.GetRequestAddress(this.BaseAddress, address);
			}
			if (content == null && this._temporaryMultipartContent != null)
			{
				content = this._temporaryMultipartContent;
			}
			HttpResponse httpResponse;
			try
			{
				httpResponse = this.Request(method, address, content);
			}
			finally
			{
				if (content != null)
				{
					content.Dispose();
				}
				this.ClearRequestData(false);
			}
			return httpResponse;
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0001D7C0 File Offset: 0x0001B9C0
		public HttpRequest AddHeader(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0 && !this.AllowEmptyHeaderValues)
			{
				throw ExceptionHelper.EmptyString("value");
			}
			if (this._temporaryHeaders == null)
			{
				this._temporaryHeaders = new Dictionary<string, string>();
			}
			this._temporaryHeaders[name] = value;
			return this;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0001D850 File Offset: 0x0001BA50
		public HttpRequest AddXmlHttpRequestHeader()
		{
			return this.AddHeader("X-Requested-With", "XMLHttpRequest");
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0001D864 File Offset: 0x0001BA64
		public HttpRequest AddHeader(HttpHeader header, string value)
		{
			this.AddHeader(Http.Headers[header], value);
			return this;
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0001D87C File Offset: 0x0001BA7C
		public void Close()
		{
			this.Dispose();
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0001D884 File Offset: 0x0001BA84
		public void Dispose()
		{
			this.Dispose(true);
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0001D890 File Offset: 0x0001BA90
		public bool ContainsCookie(string url, string name)
		{
			return this.UseCookies && this.Cookies != null && this.Cookies.Contains(url, name);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001D8B8 File Offset: 0x0001BAB8
		public bool ContainsHeader(string headerName)
		{
			if (headerName == null)
			{
				throw new ArgumentNullException("headerName");
			}
			if (headerName.Length == 0)
			{
				throw ExceptionHelper.EmptyString("headerName");
			}
			return this._permanentHeaders.ContainsKey(headerName);
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001D8F0 File Offset: 0x0001BAF0
		public bool ContainsHeader(HttpHeader header)
		{
			return this.ContainsHeader(Http.Headers[header]);
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001D904 File Offset: 0x0001BB04
		public Dictionary<string, string>.Enumerator EnumerateHeaders()
		{
			return this._permanentHeaders.GetEnumerator();
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0001D914 File Offset: 0x0001BB14
		public void ClearAllHeaders()
		{
			this._permanentHeaders.Clear();
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x0001D924 File Offset: 0x0001BB24
		public HttpResponse Patch(string address)
		{
			return this.Raw(HttpMethod.PATCH, address, null);
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0001D930 File Offset: 0x0001BB30
		public HttpResponse Patch(Uri address)
		{
			return this.Raw(HttpMethod.PATCH, address, null);
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0001D93C File Offset: 0x0001BB3C
		public HttpResponse Patch(string address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.PATCH, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x0001D960 File Offset: 0x0001BB60
		public HttpResponse Patch(Uri address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.PATCH, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0001D984 File Offset: 0x0001BB84
		public HttpResponse Patch(string address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PATCH, address, stringContent);
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0001D9FC File Offset: 0x0001BBFC
		public HttpResponse Patch(Uri address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PATCH, address, stringContent);
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001DA74 File Offset: 0x0001BC74
		public HttpResponse Patch(string address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PATCH, address, bytesContent);
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0001DAD4 File Offset: 0x0001BCD4
		public HttpResponse Patch(Uri address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PATCH, address, bytesContent);
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0001DB34 File Offset: 0x0001BD34
		public HttpResponse Patch(string address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PATCH, address, streamContent);
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0001DB9C File Offset: 0x0001BD9C
		public HttpResponse Patch(Uri address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PATCH, address, streamContent);
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0001DC04 File Offset: 0x0001BE04
		public HttpResponse Patch(string address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.PATCH, address, new FileContent(path, 32768));
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0001DC40 File Offset: 0x0001BE40
		public HttpResponse Patch(Uri address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.PATCH, address, new FileContent(path, 32768));
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0001DC7C File Offset: 0x0001BE7C
		public HttpResponse Patch(string address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.PATCH, address, content);
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0001DC98 File Offset: 0x0001BE98
		public HttpResponse Patch(Uri address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.PATCH, address, content);
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001DCB4 File Offset: 0x0001BEB4
		public HttpResponse Put(string address)
		{
			return this.Raw(HttpMethod.PUT, address, null);
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0001DCC0 File Offset: 0x0001BEC0
		public HttpResponse Put(Uri address)
		{
			return this.Raw(HttpMethod.PUT, address, null);
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001DCCC File Offset: 0x0001BECC
		public HttpResponse Put(string address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.PUT, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001DCF0 File Offset: 0x0001BEF0
		public HttpResponse Put(Uri address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.PUT, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001DD14 File Offset: 0x0001BF14
		public HttpResponse Put(string address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PUT, address, stringContent);
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001DD8C File Offset: 0x0001BF8C
		public HttpResponse Put(Uri address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PUT, address, stringContent);
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001DE04 File Offset: 0x0001C004
		public HttpResponse Put(string address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PUT, address, bytesContent);
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001DE64 File Offset: 0x0001C064
		public HttpResponse Put(Uri address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PUT, address, bytesContent);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001DEC4 File Offset: 0x0001C0C4
		public HttpResponse Put(string address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PUT, address, streamContent);
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001DF2C File Offset: 0x0001C12C
		public HttpResponse Put(Uri address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.PUT, address, streamContent);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001DF94 File Offset: 0x0001C194
		public HttpResponse Put(string address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.PUT, address, new FileContent(path, 32768));
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001DFD0 File Offset: 0x0001C1D0
		public HttpResponse Put(Uri address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.PUT, address, new FileContent(path, 32768));
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001E00C File Offset: 0x0001C20C
		public HttpResponse Put(string address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.PUT, address, content);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001E028 File Offset: 0x0001C228
		public HttpResponse Put(Uri address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.PUT, address, content);
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001E044 File Offset: 0x0001C244
		public HttpResponse Delete(string address)
		{
			return this.Raw(HttpMethod.DELETE, address, null);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0001E050 File Offset: 0x0001C250
		public HttpResponse Delete(Uri address)
		{
			return this.Raw(HttpMethod.DELETE, address, null);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0001E05C File Offset: 0x0001C25C
		public HttpResponse Delete(string address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.DELETE, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0001E080 File Offset: 0x0001C280
		public HttpResponse Delete(Uri address, RequestParams reqParams)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return this.Raw(HttpMethod.DELETE, address, new FormUrlEncodedContent(reqParams));
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0001E0A4 File Offset: 0x0001C2A4
		public HttpResponse Delete(string address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.DELETE, address, stringContent);
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0001E11C File Offset: 0x0001C31C
		public HttpResponse Delete(Uri address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent stringContent = new StringContent(str)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.DELETE, address, stringContent);
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0001E194 File Offset: 0x0001C394
		public HttpResponse Delete(string address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.DELETE, address, bytesContent);
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0001E1F4 File Offset: 0x0001C3F4
		public HttpResponse Delete(Uri address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent bytesContent = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.DELETE, address, bytesContent);
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0001E254 File Offset: 0x0001C454
		public HttpResponse Delete(string address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.DELETE, address, streamContent);
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001E2BC File Offset: 0x0001C4BC
		public HttpResponse Delete(Uri address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent streamContent = new StreamContent(stream, 32768)
			{
				ContentType = contentType
			};
			return this.Raw(HttpMethod.DELETE, address, streamContent);
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0001E324 File Offset: 0x0001C524
		public HttpResponse Delete(string address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.DELETE, address, new FileContent(path, 32768));
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0001E360 File Offset: 0x0001C560
		public HttpResponse Delete(Uri address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return this.Raw(HttpMethod.DELETE, address, new FileContent(path, 32768));
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0001E39C File Offset: 0x0001C59C
		public HttpResponse Delete(string address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.DELETE, address, content);
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001E3B8 File Offset: 0x0001C5B8
		public HttpResponse Delete(Uri address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return this.Raw(HttpMethod.DELETE, address, content);
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0001E3D4 File Offset: 0x0001C5D4
		private void Dispose(bool disposing)
		{
			if (!disposing || this.TcpClient == null)
			{
				return;
			}
			this.TcpClient.Close();
			this.TcpClient = null;
			Stream clientStream = this.ClientStream;
			if (clientStream != null)
			{
				clientStream.Dispose();
			}
			this.ClientStream = null;
			NetworkStream clientNetworkStream = this.ClientNetworkStream;
			if (clientNetworkStream != null)
			{
				clientNetworkStream.Dispose();
			}
			this.ClientNetworkStream = null;
			this._keepAliveRequestCount = 0;
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001E44C File Offset: 0x0001C64C
		private void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
		{
			EventHandler<UploadProgressChangedEventArgs> uploadProgressChangedHandler = this._uploadProgressChangedHandler;
			if (uploadProgressChangedHandler == null)
			{
				return;
			}
			uploadProgressChangedHandler(this, e);
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x0001E464 File Offset: 0x0001C664
		private void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
		{
			EventHandler<DownloadProgressChangedEventArgs> downloadProgressChangedHandler = this._downloadProgressChangedHandler;
			if (downloadProgressChangedHandler == null)
			{
				return;
			}
			downloadProgressChangedHandler(this, e);
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x0001E47C File Offset: 0x0001C67C
		private void Init()
		{
			this.KeepAlive = true;
			this.AllowAutoRedirect = true;
			this._tempAllowAutoRedirect = this.AllowAutoRedirect;
			this.EnableEncodingContent = true;
			this.Response = new HttpResponse(this);
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0001E4BC File Offset: 0x0001C6BC
		private static Uri GetRequestAddress(Uri baseAddress, Uri address)
		{
			Uri uri;
			if (baseAddress == null)
			{
				uri = new UriBuilder(address.OriginalString).Uri;
			}
			else
			{
				Uri.TryCreate(baseAddress, address, out uri);
			}
			return uri;
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001E4FC File Offset: 0x0001C6FC
		private HttpResponse Request(HttpMethod method, Uri address, HttpContent content)
		{
			for (;;)
			{
				this._method = method;
				this._content = content;
				this.CloseConnectionIfNeeded();
				Uri address2 = this.Address;
				this.Address = address;
				bool flag;
				try
				{
					flag = this.TryCreateConnectionOrUseExisting(address, address2);
				}
				catch (HttpException)
				{
					if (this.CanReconnect)
					{
						return this.ReconnectAfterFail();
					}
					throw;
				}
				if (flag)
				{
					this._keepAliveRequestCount = 1;
				}
				else
				{
					this._keepAliveRequestCount++;
				}
				try
				{
					this.SendRequestData(address, method);
				}
				catch (SecurityException ex)
				{
					throw this.NewHttpException(Resources.HttpException_FailedSendRequest, ex, HttpExceptionStatus.SendFailure);
				}
				catch (IOException ex2)
				{
					if (this.CanReconnect)
					{
						return this.ReconnectAfterFail();
					}
					throw this.NewHttpException(Resources.HttpException_FailedSendRequest, ex2, HttpExceptionStatus.SendFailure);
				}
				try
				{
					this.ReceiveResponseHeaders(method);
				}
				catch (HttpException ex3)
				{
					if (this.CanReconnect)
					{
						return this.ReconnectAfterFail();
					}
					if (this.KeepAlive && !this._keepAliveReconnected && !flag && ex3.EmptyMessageBody)
					{
						return this.KeepAliveReconnect();
					}
					throw;
				}
				this.Response.ReconnectCount = this._reconnectCount;
				this._reconnectCount = 0;
				this._keepAliveReconnected = false;
				this._whenConnectionIdle = DateTime.Now;
				if (!this.IgnoreProtocolErrors)
				{
					this.CheckStatusCode(this.Response.StatusCode);
				}
				if (!this.AllowAutoRedirect || !this.Response.HasRedirect)
				{
					goto IL_01CF;
				}
				int num = this._redirectionCount + 1;
				this._redirectionCount = num;
				if (num > this._maximumAutomaticRedirections)
				{
					break;
				}
				if (this.Response.HasExternalRedirect)
				{
					goto Block_8;
				}
				this.ClearRequestData(true);
				method = HttpMethod.GET;
				address = this.Response.RedirectAddress;
				content = null;
			}
			throw this.NewHttpException(Resources.HttpException_LimitRedirections, null, HttpExceptionStatus.Other);
			Block_8:
			return this.Response;
			IL_01CF:
			this._redirectionCount = 0;
			return this.Response;
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x0001E71C File Offset: 0x0001C91C
		private void CloseConnectionIfNeeded()
		{
			if (this.TcpClient == null || this.ClientStream == null || this.Response.HasError || this.Response.MessageBodyLoaded)
			{
				return;
			}
			try
			{
				this.Response.None();
			}
			catch (HttpException)
			{
				this.Dispose();
			}
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001E798 File Offset: 0x0001C998
		private bool TryCreateConnectionOrUseExisting(Uri address, Uri previousAddress)
		{
			ProxyClient proxy = this.GetProxy();
			bool flag = this.TcpClient != null;
			bool flag2 = !object.Equals(this._currentProxy, proxy);
			bool flag3 = previousAddress == null || previousAddress.Port != address.Port || previousAddress.Host != address.Host || previousAddress.Scheme != address.Scheme;
			bool flag4 = this.Response.ContainsHeader("Connection") && this.Response["Connection"] == "close";
			if (flag && !flag2 && !flag3 && !this.Response.HasError && !this.KeepAliveLimitIsReached() && !flag4)
			{
				return false;
			}
			this._currentProxy = proxy;
			this.Dispose();
			this.CreateConnection(address);
			return true;
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0001E894 File Offset: 0x0001CA94
		private bool KeepAliveLimitIsReached()
		{
			if (!this.KeepAlive)
			{
				return false;
			}
			int num = this.Response.MaximumKeepAliveRequests ?? this._maximumKeepAliveRequests;
			if (this._keepAliveRequestCount >= num)
			{
				return true;
			}
			int num2 = this.Response.KeepAliveTimeout ?? this._keepAliveTimeout;
			return this._whenConnectionIdle.AddMilliseconds((double)num2) < DateTime.Now;
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001E930 File Offset: 0x0001CB30
		private void SendRequestData(Uri uri, HttpMethod method)
		{
			long num = 0L;
			string text = string.Empty;
			if (HttpRequest.CanContainsRequestBody(method) && this._content != null)
			{
				text = this._content.ContentType;
				num = this._content.CalculateContentLength();
			}
			string text2 = this.GenerateStartingLine(method);
			string text3 = this.GenerateHeaders(uri, method, num, text);
			byte[] bytes = Encoding.ASCII.GetBytes(text2);
			byte[] bytes2 = Encoding.ASCII.GetBytes(text3);
			this._bytesSent = 0L;
			this._totalBytesSent = (long)(bytes.Length + bytes2.Length) + num;
			this.ClientStream.Write(bytes, 0, bytes.Length);
			this.ClientStream.Write(bytes2, 0, bytes2.Length);
			if (this._content != null && num > 0L)
			{
				this._content.WriteTo(this.ClientStream);
			}
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001EA10 File Offset: 0x0001CC10
		private void ReceiveResponseHeaders(HttpMethod method)
		{
			this._canReportBytesReceived = false;
			this._bytesReceived = 0L;
			this._totalBytesReceived = this.Response.LoadResponse(method, this.EnableMiddleHeaders);
			this._canReportBytesReceived = true;
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x0001EA50 File Offset: 0x0001CC50
		private bool CanReconnect
		{
			get
			{
				return this.Reconnect && this._reconnectCount < this._reconnectLimit;
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0001EA70 File Offset: 0x0001CC70
		private HttpResponse ReconnectAfterFail()
		{
			this.Dispose();
			Thread.Sleep(this._reconnectDelay);
			this._reconnectCount++;
			return this.Request(this._method, this.Address, this._content);
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001EAB8 File Offset: 0x0001CCB8
		private HttpResponse KeepAliveReconnect()
		{
			this.Dispose();
			this._keepAliveReconnected = true;
			return this.Request(this._method, this.Address, this._content);
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001EAF0 File Offset: 0x0001CCF0
		private void CheckStatusCode(HttpStatusCode statusCode)
		{
			if (statusCode >= HttpStatusCode.BadRequest && statusCode < HttpStatusCode.InternalServerError)
			{
				throw new HttpException(string.Format(Resources.HttpException_ClientError, (int)statusCode), HttpExceptionStatus.ProtocolError, this.Response.StatusCode, null);
			}
			if (statusCode >= HttpStatusCode.InternalServerError)
			{
				throw new HttpException(string.Format(Resources.HttpException_SeverError, (int)statusCode), HttpExceptionStatus.ProtocolError, this.Response.StatusCode, null);
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001EB6C File Offset: 0x0001CD6C
		private static bool CanContainsRequestBody(HttpMethod method)
		{
			return method == HttpMethod.POST || method == HttpMethod.PUT || method == HttpMethod.PATCH || method == HttpMethod.DELETE;
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001EB8C File Offset: 0x0001CD8C
		private ProxyClient GetProxy()
		{
			if (!HttpRequest.DisableProxyForLocalAddress)
			{
				return this.Proxy ?? HttpRequest.GlobalProxy;
			}
			try
			{
				IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
				IPAddress[] hostAddresses = Dns.GetHostAddresses(this.Address.Host);
				for (int i = 0; i < hostAddresses.Length; i++)
				{
					if (hostAddresses[i].Equals(ipaddress))
					{
						return null;
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is SocketException || ex is ArgumentException)
				{
					throw this.NewHttpException(Resources.HttpException_FailedGetHostAddresses, ex, HttpExceptionStatus.Other);
				}
				throw;
			}
			return this.Proxy ?? HttpRequest.GlobalProxy;
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001EC54 File Offset: 0x0001CE54
		private TcpClient CreateTcpConnection(string host, int port)
		{
			TcpClient tcpClient;
			if (this._currentProxy == null)
			{
				tcpClient = new TcpClient();
				Exception connectException = null;
				ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
				try
				{
					tcpClient.BeginConnect(host, port, delegate(IAsyncResult ar)
					{
						try
						{
							tcpClient.EndConnect(ar);
						}
						catch (Exception ex3)
						{
							connectException = ex3;
						}
						connectDoneEvent.Set();
					}, tcpClient);
				}
				catch (Exception ex)
				{
					tcpClient.Close();
					if (ex is SocketException || ex is SecurityException)
					{
						throw this.NewHttpException(Resources.HttpException_FailedConnect, ex, HttpExceptionStatus.ConnectFailure);
					}
					throw;
				}
				if (!connectDoneEvent.Wait(this._connectTimeout))
				{
					tcpClient.Close();
					throw this.NewHttpException(Resources.HttpException_ConnectTimeout, null, HttpExceptionStatus.ConnectFailure);
				}
				if (connectException != null)
				{
					tcpClient.Close();
					if (connectException is SocketException)
					{
						throw this.NewHttpException(Resources.HttpException_FailedConnect, connectException, HttpExceptionStatus.ConnectFailure);
					}
					throw connectException;
				}
				else
				{
					if (!tcpClient.Connected)
					{
						tcpClient.Close();
						throw this.NewHttpException(Resources.HttpException_FailedConnect, null, HttpExceptionStatus.ConnectFailure);
					}
					tcpClient.SendTimeout = this._readWriteTimeout;
					tcpClient.ReceiveTimeout = this._readWriteTimeout;
				}
			}
			else
			{
				try
				{
					tcpClient = this._currentProxy.CreateConnection(host, port, null);
				}
				catch (ProxyException ex2)
				{
					throw this.NewHttpException(Resources.HttpException_FailedConnect, ex2, HttpExceptionStatus.ConnectFailure);
				}
			}
			return tcpClient;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001EDF0 File Offset: 0x0001CFF0
		private void CreateConnection(Uri address)
		{
			this.TcpClient = this.CreateTcpConnection(address.Host, address.Port);
			this.ClientNetworkStream = this.TcpClient.GetStream();
			if (address.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					SslStream sslStream = ((this.SslCertificateValidatorCallback == null) ? new SslStream(this.ClientNetworkStream, false, Http.AcceptAllCertificationsCallback) : new SslStream(this.ClientNetworkStream, false, this.SslCertificateValidatorCallback));
					sslStream.AuthenticateAsClient(address.Host, new X509CertificateCollection(), this.SslProtocols, false);
					this.ClientStream = sslStream;
					goto IL_00CA;
				}
				catch (Exception ex)
				{
					if (ex is IOException || ex is AuthenticationException)
					{
						throw this.NewHttpException(Resources.HttpException_FailedSslConnect, ex, HttpExceptionStatus.ConnectFailure);
					}
					throw;
				}
			}
			this.ClientStream = this.ClientNetworkStream;
			IL_00CA:
			if (this._uploadProgressChangedHandler == null && this._downloadProgressChangedHandler == null)
			{
				return;
			}
			HttpRequest.HttpWrapperStream httpWrapperStream = new HttpRequest.HttpWrapperStream(this.ClientStream, this.TcpClient.SendBufferSize);
			if (this._uploadProgressChangedHandler != null)
			{
				httpWrapperStream.BytesWriteCallback = new Action<int>(this.ReportBytesSent);
			}
			if (this._downloadProgressChangedHandler != null)
			{
				httpWrapperStream.BytesReadCallback = new Action<int>(this.ReportBytesReceived);
			}
			this.ClientStream = httpWrapperStream;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001EF48 File Offset: 0x0001D148
		private string GenerateStartingLine(HttpMethod method)
		{
			string text = ((this._currentProxy != null && this._currentProxy.Type == ProxyType.HTTP && this._currentProxy.AbsoluteUriInStartingLine) ? this.Address.AbsoluteUri : this.Address.PathAndQuery);
			return string.Format("{0} {1} HTTP/{2}\r\n", method, text, HttpRequest.ProtocolVersion);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001EFBC File Offset: 0x0001D1BC
		private string GenerateHeaders(Uri uri, HttpMethod method, long contentLength = 0L, string contentType = null)
		{
			Dictionary<string, string> dictionary = this.GenerateCommonHeaders(method, contentLength, contentType);
			HttpRequest.MergeHeaders(dictionary, this._permanentHeaders);
			if (this._temporaryHeaders != null && this._temporaryHeaders.Count > 0)
			{
				HttpRequest.MergeHeaders(dictionary, this._temporaryHeaders);
			}
			if (!this.UseCookies)
			{
				return this.ToHeadersString(dictionary);
			}
			if (this.Cookies == null)
			{
				this.Cookies = new CookieStorage(false, null, this.IgnoreInvalidCookie);
				return this.ToHeadersString(dictionary);
			}
			if (this.Cookies.Count == 0 || dictionary.ContainsKey("Cookie"))
			{
				return this.ToHeadersString(dictionary);
			}
			string cookieHeader = this.Cookies.GetCookieHeader(uri);
			if (!string.IsNullOrEmpty(cookieHeader))
			{
				dictionary["Cookie"] = cookieHeader;
			}
			return this.ToHeadersString(dictionary);
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001F098 File Offset: 0x0001D298
		private Dictionary<string, string> GenerateCommonHeaders(HttpMethod method, long contentLength = 0L, string contentType = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			dictionary["Host"] = (this.Address.IsDefaultPort ? this.Address.Host : string.Format("{0}:{1}", this.Address.Host, this.Address.Port));
			Dictionary<string, string> dictionary2 = dictionary;
			HttpProxyClient httpProxyClient = null;
			if (this._currentProxy != null && this._currentProxy.Type == ProxyType.HTTP)
			{
				httpProxyClient = this._currentProxy as HttpProxyClient;
			}
			if (httpProxyClient != null)
			{
				dictionary2["Proxy-Connection"] = (this.KeepAlive ? "keep-alive" : "close");
				if (!string.IsNullOrEmpty(httpProxyClient.Username) || !string.IsNullOrEmpty(httpProxyClient.Password))
				{
					dictionary2["Proxy-Authorization"] = HttpRequest.GetProxyAuthorizationHeader(httpProxyClient);
				}
			}
			else
			{
				dictionary2["Connection"] = (this.KeepAlive ? "keep-alive" : "close");
			}
			if (!string.IsNullOrEmpty(this.Username) || !string.IsNullOrEmpty(this.Password))
			{
				dictionary2["Authorization"] = this.GetAuthorizationHeader();
			}
			if (this.EnableEncodingContent)
			{
				dictionary2["Accept-Encoding"] = this.AcceptEncoding;
			}
			if (this.Culture != null)
			{
				dictionary2["Accept-Language"] = this.GetLanguageHeader();
			}
			if (this.CharacterSet != null)
			{
				dictionary2["Accept-Charset"] = this.GetCharsetHeader();
			}
			if (!HttpRequest.CanContainsRequestBody(method))
			{
				return dictionary2;
			}
			if (contentLength > 0L)
			{
				dictionary2["Content-Type"] = contentType;
			}
			dictionary2["Content-Length"] = contentLength.ToString();
			return dictionary2;
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001F26C File Offset: 0x0001D46C
		private string GetAuthorizationHeader()
		{
			string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Username + ":" + this.Password));
			return "Basic " + text;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0001F2B0 File Offset: 0x0001D4B0
		private static string GetProxyAuthorizationHeader(ProxyClient httpProxy)
		{
			string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(httpProxy.Username + ":" + httpProxy.Password));
			return "Basic " + text;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001F2F4 File Offset: 0x0001D4F4
		private string GetLanguageHeader()
		{
			CultureInfo culture = this.Culture;
			string text = ((culture != null) ? culture.Name : null) ?? CultureInfo.CurrentCulture.Name;
			if (!text.StartsWith("en"))
			{
				return text + "," + text.Substring(0, 2) + ";q=0.8,en-US;q=0.6,en;q=0.4";
			}
			return text;
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001F35C File Offset: 0x0001D55C
		private string GetCharsetHeader()
		{
			if (object.Equals(this.CharacterSet, Encoding.UTF8))
			{
				return "utf-8;q=0.7,*;q=0.3";
			}
			Encoding characterSet = this.CharacterSet;
			return (((characterSet != null) ? characterSet.WebName : null) ?? Encoding.Default.WebName) + ",utf-8;q=0.7,*;q=0.3";
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001F3BC File Offset: 0x0001D5BC
		private static void MergeHeaders(IDictionary<string, string> destination, Dictionary<string, string> source)
		{
			foreach (KeyValuePair<string, string> keyValuePair in source)
			{
				destination[keyValuePair.Key] = keyValuePair.Value;
			}
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0001F420 File Offset: 0x0001D620
		private string ToHeadersString(Dictionary<string, string> headers)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in headers)
			{
				if (keyValuePair.Key != "Cookie" || this.CookieSingleHeader)
				{
					stringBuilder.AppendFormat("{0}: {1}\r\n", keyValuePair.Key, keyValuePair.Value);
				}
				else
				{
					foreach (string text in keyValuePair.Value.Split(new string[] { "; " }, StringSplitOptions.None))
					{
						stringBuilder.AppendFormat("Cookie: {0}\r\n", text);
					}
				}
			}
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0001F50C File Offset: 0x0001D70C
		private void ReportBytesSent(int bytesSent)
		{
			this._bytesSent += (long)bytesSent;
			this.OnUploadProgressChanged(new UploadProgressChangedEventArgs(this._bytesSent, this._totalBytesSent));
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0001F534 File Offset: 0x0001D734
		private void ReportBytesReceived(int bytesReceived)
		{
			this._bytesReceived += (long)bytesReceived;
			if (this._canReportBytesReceived)
			{
				this.OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(this._bytesReceived, this._totalBytesReceived));
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0001F568 File Offset: 0x0001D768
		private void ClearRequestData(bool redirect)
		{
			this._content = null;
			this._temporaryMultipartContent = null;
			if (!redirect || !this.KeepTemporaryHeadersOnRedirect)
			{
				this._temporaryHeaders = null;
			}
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0001F590 File Offset: 0x0001D790
		private HttpException NewHttpException(string message, Exception innerException = null, HttpExceptionStatus status = HttpExceptionStatus.Other)
		{
			return new HttpException(string.Format(message, this.Address.Host), status, HttpStatusCode.None, innerException);
		}

		// Token: 0x040001EF RID: 495
		private ProxyClient _currentProxy;

		// Token: 0x040001F0 RID: 496
		private int _redirectionCount;

		// Token: 0x040001F1 RID: 497
		private int _maximumAutomaticRedirections = 5;

		// Token: 0x040001F2 RID: 498
		private int _connectTimeout = 9000;

		// Token: 0x040001F3 RID: 499
		private int _readWriteTimeout = 30000;

		// Token: 0x040001F4 RID: 500
		private DateTime _whenConnectionIdle;

		// Token: 0x040001F5 RID: 501
		private int _keepAliveTimeout = 30000;

		// Token: 0x040001F6 RID: 502
		private int _maximumKeepAliveRequests = 100;

		// Token: 0x040001F7 RID: 503
		private int _keepAliveRequestCount;

		// Token: 0x040001F8 RID: 504
		private bool _keepAliveReconnected;

		// Token: 0x040001F9 RID: 505
		private int _reconnectLimit = 3;

		// Token: 0x040001FA RID: 506
		private int _reconnectDelay = 100;

		// Token: 0x040001FB RID: 507
		private int _reconnectCount;

		// Token: 0x040001FC RID: 508
		private HttpMethod _method;

		// Token: 0x040001FD RID: 509
		private HttpContent _content;

		// Token: 0x040001FE RID: 510
		private readonly Dictionary<string, string> _permanentHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040001FF RID: 511
		private Dictionary<string, string> _temporaryHeaders;

		// Token: 0x04000200 RID: 512
		private MultipartContent _temporaryMultipartContent;

		// Token: 0x04000201 RID: 513
		private long _bytesSent;

		// Token: 0x04000202 RID: 514
		private long _totalBytesSent;

		// Token: 0x04000203 RID: 515
		private long _bytesReceived;

		// Token: 0x04000204 RID: 516
		private long _totalBytesReceived;

		// Token: 0x04000205 RID: 517
		private bool _canReportBytesReceived;

		// Token: 0x04000206 RID: 518
		private EventHandler<UploadProgressChangedEventArgs> _uploadProgressChangedHandler;

		// Token: 0x04000207 RID: 519
		private EventHandler<DownloadProgressChangedEventArgs> _downloadProgressChangedHandler;

		// Token: 0x04000208 RID: 520
		private bool _tempAllowAutoRedirect;

		// Token: 0x04000209 RID: 521
		private bool _tempIgnoreProtocolErrors;

		// Token: 0x020001D5 RID: 469
		private sealed class HttpWrapperStream : Stream
		{
			// Token: 0x170003DE RID: 990
			// (get) Token: 0x06001560 RID: 5472 RVA: 0x00075D38 File Offset: 0x00073F38
			// (set) Token: 0x06001561 RID: 5473 RVA: 0x00075D40 File Offset: 0x00073F40
			public Action<int> BytesReadCallback { private get; set; }

			// Token: 0x170003DF RID: 991
			// (get) Token: 0x06001562 RID: 5474 RVA: 0x00075D4C File Offset: 0x00073F4C
			// (set) Token: 0x06001563 RID: 5475 RVA: 0x00075D54 File Offset: 0x00073F54
			public Action<int> BytesWriteCallback { private get; set; }

			// Token: 0x170003E0 RID: 992
			// (get) Token: 0x06001564 RID: 5476 RVA: 0x00075D60 File Offset: 0x00073F60
			public override bool CanRead
			{
				get
				{
					return this._baseStream.CanRead;
				}
			}

			// Token: 0x170003E1 RID: 993
			// (get) Token: 0x06001565 RID: 5477 RVA: 0x00075D70 File Offset: 0x00073F70
			public override bool CanSeek
			{
				get
				{
					return this._baseStream.CanSeek;
				}
			}

			// Token: 0x170003E2 RID: 994
			// (get) Token: 0x06001566 RID: 5478 RVA: 0x00075D80 File Offset: 0x00073F80
			public override bool CanTimeout
			{
				get
				{
					return this._baseStream.CanTimeout;
				}
			}

			// Token: 0x170003E3 RID: 995
			// (get) Token: 0x06001567 RID: 5479 RVA: 0x00075D90 File Offset: 0x00073F90
			public override bool CanWrite
			{
				get
				{
					return this._baseStream.CanWrite;
				}
			}

			// Token: 0x170003E4 RID: 996
			// (get) Token: 0x06001568 RID: 5480 RVA: 0x00075DA0 File Offset: 0x00073FA0
			public override long Length
			{
				get
				{
					return this._baseStream.Length;
				}
			}

			// Token: 0x170003E5 RID: 997
			// (get) Token: 0x06001569 RID: 5481 RVA: 0x00075DB0 File Offset: 0x00073FB0
			// (set) Token: 0x0600156A RID: 5482 RVA: 0x00075DC0 File Offset: 0x00073FC0
			public override long Position
			{
				get
				{
					return this._baseStream.Position;
				}
				set
				{
					this._baseStream.Position = value;
				}
			}

			// Token: 0x0600156B RID: 5483 RVA: 0x00075DD0 File Offset: 0x00073FD0
			public HttpWrapperStream(Stream baseStream, int sendBufferSize)
			{
				this._baseStream = baseStream;
				this._sendBufferSize = sendBufferSize;
			}

			// Token: 0x0600156C RID: 5484 RVA: 0x00075DE8 File Offset: 0x00073FE8
			public override void Flush()
			{
			}

			// Token: 0x0600156D RID: 5485 RVA: 0x00075DEC File Offset: 0x00073FEC
			public override void SetLength(long value)
			{
				this._baseStream.SetLength(value);
			}

			// Token: 0x0600156E RID: 5486 RVA: 0x00075DFC File Offset: 0x00073FFC
			public override long Seek(long offset, SeekOrigin origin)
			{
				return this._baseStream.Seek(offset, origin);
			}

			// Token: 0x0600156F RID: 5487 RVA: 0x00075E0C File Offset: 0x0007400C
			public override int Read(byte[] buffer, int offset, int count)
			{
				int num = this._baseStream.Read(buffer, offset, count);
				Action<int> bytesReadCallback = this.BytesReadCallback;
				if (bytesReadCallback != null)
				{
					bytesReadCallback(num);
				}
				return num;
			}

			// Token: 0x06001570 RID: 5488 RVA: 0x00075E48 File Offset: 0x00074048
			public override void Write(byte[] buffer, int offset, int count)
			{
				if (this.BytesWriteCallback == null)
				{
					this._baseStream.Write(buffer, offset, count);
					return;
				}
				int num = 0;
				while (count > 0)
				{
					int num2;
					if (count >= this._sendBufferSize)
					{
						num2 = this._sendBufferSize;
						this._baseStream.Write(buffer, num, num2);
						num += this._sendBufferSize;
						count -= this._sendBufferSize;
					}
					else
					{
						num2 = count;
						this._baseStream.Write(buffer, num, num2);
						count = 0;
					}
					this.BytesWriteCallback(num2);
				}
			}

			// Token: 0x04000830 RID: 2096
			private readonly Stream _baseStream;

			// Token: 0x04000831 RID: 2097
			private readonly int _sendBufferSize;
		}
	}
}
