using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Leaf.xNet
{
	// Token: 0x02000063 RID: 99
	[ComVisible(true)]
	public sealed class HttpResponse
	{
		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x0001F5AC File Offset: 0x0001D7AC
		public Dictionary<string, string> MiddleHeaders
		{
			get
			{
				Dictionary<string, string> dictionary;
				if ((dictionary = this._middleHeaders) == null)
				{
					dictionary = (this._middleHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
				}
				return dictionary;
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x0001F5E0 File Offset: 0x0001D7E0
		// (set) Token: 0x06000556 RID: 1366 RVA: 0x0001F5E8 File Offset: 0x0001D7E8
		public bool HasError { get; private set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x0001F5F4 File Offset: 0x0001D7F4
		// (set) Token: 0x06000558 RID: 1368 RVA: 0x0001F5FC File Offset: 0x0001D7FC
		public bool MessageBodyLoaded { get; private set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000559 RID: 1369 RVA: 0x0001F608 File Offset: 0x0001D808
		public bool IsOK
		{
			get
			{
				return this.StatusCode == HttpStatusCode.OK;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x0001F618 File Offset: 0x0001D818
		public bool HasRedirect
		{
			get
			{
				int statusCode = (int)this.StatusCode;
				return (statusCode >= 300 && statusCode < 400) || this._headers.ContainsKey("Location") || this._headers.ContainsKey("Redirect-Location");
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x0001F670 File Offset: 0x0001D870
		public bool HasExternalRedirect
		{
			get
			{
				return this.HasRedirect && this.RedirectAddress != null && !this.RedirectAddress.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase) && !this.RedirectAddress.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x0001F6D4 File Offset: 0x0001D8D4
		// (set) Token: 0x0600055D RID: 1373 RVA: 0x0001F6DC File Offset: 0x0001D8DC
		public int ReconnectCount { get; internal set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x0001F6E8 File Offset: 0x0001D8E8
		// (set) Token: 0x0600055F RID: 1375 RVA: 0x0001F6F0 File Offset: 0x0001D8F0
		public Uri Address { get; private set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x0001F6FC File Offset: 0x0001D8FC
		// (set) Token: 0x06000561 RID: 1377 RVA: 0x0001F704 File Offset: 0x0001D904
		public HttpMethod Method { get; private set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x0001F710 File Offset: 0x0001D910
		// (set) Token: 0x06000563 RID: 1379 RVA: 0x0001F718 File Offset: 0x0001D918
		public Version ProtocolVersion { get; private set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x0001F724 File Offset: 0x0001D924
		// (set) Token: 0x06000565 RID: 1381 RVA: 0x0001F72C File Offset: 0x0001D92C
		public HttpStatusCode StatusCode { get; private set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x0001F738 File Offset: 0x0001D938
		// (set) Token: 0x06000567 RID: 1383 RVA: 0x0001F740 File Offset: 0x0001D940
		public Uri RedirectAddress { get; private set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x0001F74C File Offset: 0x0001D94C
		// (set) Token: 0x06000569 RID: 1385 RVA: 0x0001F754 File Offset: 0x0001D954
		public Encoding CharacterSet { get; private set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x0001F760 File Offset: 0x0001D960
		// (set) Token: 0x0600056B RID: 1387 RVA: 0x0001F768 File Offset: 0x0001D968
		public long ContentLength { get; private set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x0001F774 File Offset: 0x0001D974
		// (set) Token: 0x0600056D RID: 1389 RVA: 0x0001F77C File Offset: 0x0001D97C
		public string ContentType { get; private set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x0001F788 File Offset: 0x0001D988
		public string Location
		{
			get
			{
				return this["Location"];
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x0001F798 File Offset: 0x0001D998
		// (set) Token: 0x06000570 RID: 1392 RVA: 0x0001F7A0 File Offset: 0x0001D9A0
		public CookieStorage Cookies { get; private set; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x0001F7AC File Offset: 0x0001D9AC
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x0001F7B4 File Offset: 0x0001D9B4
		public int? KeepAliveTimeout { get; private set; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x0001F7C0 File Offset: 0x0001D9C0
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x0001F7C8 File Offset: 0x0001D9C8
		public int? MaximumKeepAliveRequests { get; private set; }

		// Token: 0x17000116 RID: 278
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
				if (!this._headers.TryGetValue(headerName, out empty))
				{
					empty = string.Empty;
				}
				return empty;
			}
		}

		// Token: 0x17000117 RID: 279
		public string this[HttpHeader header]
		{
			get
			{
				return this[Http.Headers[header]];
			}
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x0001F83C File Offset: 0x0001DA3C
		internal HttpResponse(HttpRequest request)
		{
			this._request = request;
			this.ContentLength = -1L;
			this.ContentType = string.Empty;
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x0001F870 File Offset: 0x0001DA70
		public byte[] ToBytes()
		{
			if (this.HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (this.MessageBodyLoaded)
			{
				return new byte[0];
			}
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.SetLength((this.ContentLength == -1L) ? 0L : this.ContentLength);
				try
				{
					foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
					{
						memoryStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
					}
				}
				catch (Exception ex)
				{
					this.HasError = true;
					if (ex is IOException || ex is InvalidOperationException)
					{
						throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
					}
					throw;
				}
				if (this.ConnectionClosed())
				{
					HttpRequest request = this._request;
					if (request != null)
					{
						request.Dispose();
					}
				}
				this.MessageBodyLoaded = true;
				array = memoryStream.ToArray();
			}
			return array;
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0001F9B0 File Offset: 0x0001DBB0
		public override string ToString()
		{
			if (this.HasError)
			{
				return string.Empty;
			}
			if (this.MessageBodyLoaded)
			{
				return this._loadedMessageBody;
			}
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.SetLength((this.ContentLength == -1L) ? 0L : this.ContentLength);
			try
			{
				foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
				{
					memoryStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
				}
			}
			catch (Exception ex)
			{
				this.HasError = true;
				if (ex is IOException || ex is InvalidOperationException)
				{
					throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
				}
				throw;
			}
			if (this.ConnectionClosed())
			{
				this._request.Dispose();
			}
			this.MessageBodyLoaded = true;
			this._loadedMessageBody = this.CharacterSet.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			memoryStream.Dispose();
			return this._loadedMessageBody;
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0001FAE4 File Offset: 0x0001DCE4
		public void ToFile(string path)
		{
			if (this.HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (this.MessageBodyLoaded)
			{
				return;
			}
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
					{
						fileStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
					}
				}
			}
			catch (ArgumentException ex)
			{
				throw ExceptionHelper.WrongPath("path", ex);
			}
			catch (NotSupportedException ex2)
			{
				throw ExceptionHelper.WrongPath("path", ex2);
			}
			catch (Exception ex3)
			{
				this.HasError = true;
				if (ex3 is IOException || ex3 is InvalidOperationException)
				{
					throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex3);
				}
				throw;
			}
			if (this.ConnectionClosed())
			{
				this._request.Dispose();
			}
			this.MessageBodyLoaded = true;
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x0001FC34 File Offset: 0x0001DE34
		public MemoryStream ToMemoryStream()
		{
			if (this.HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (this.MessageBodyLoaded)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.SetLength((this.ContentLength == -1L) ? 0L : this.ContentLength);
			try
			{
				foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
				{
					memoryStream.Write(bytesWrapper.Value, 0, bytesWrapper.Length);
				}
			}
			catch (Exception ex)
			{
				this.HasError = true;
				if (ex is IOException || ex is InvalidOperationException)
				{
					throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
				}
				throw;
			}
			if (this.ConnectionClosed())
			{
				this._request.Dispose();
			}
			this.MessageBodyLoaded = true;
			memoryStream.Position = 0L;
			return memoryStream;
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x0001FD44 File Offset: 0x0001DF44
		public void None()
		{
			if (this.HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (this.MessageBodyLoaded)
			{
				return;
			}
			if (this.ConnectionClosed())
			{
				this._request.Dispose();
			}
			else
			{
				try
				{
					foreach (HttpResponse.BytesWrapper bytesWrapper in this.GetMessageBodySource())
					{
					}
				}
				catch (Exception ex)
				{
					this.HasError = true;
					if (ex is IOException || ex is InvalidOperationException)
					{
						throw this.NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
					}
					throw;
				}
			}
			this.MessageBodyLoaded = true;
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0001FE18 File Offset: 0x0001E018
		public bool ContainsCookie(string url, string name)
		{
			return this.Cookies != null && this.Cookies.Contains(url, name);
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x0001FE34 File Offset: 0x0001E034
		public bool ContainsCookie(Uri uri, string name)
		{
			return this.Cookies != null && this.Cookies.Contains(uri, name);
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0001FE50 File Offset: 0x0001E050
		public bool ContainsCookie(string name)
		{
			return this.Cookies != null && this.Cookies.Contains((this.HasRedirect && !this.HasExternalRedirect) ? this.RedirectAddress : this.Address, name);
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x0001FEA4 File Offset: 0x0001E0A4
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
			return this._headers.ContainsKey(headerName);
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0001FEDC File Offset: 0x0001E0DC
		public bool ContainsHeader(HttpHeader header)
		{
			return this.ContainsHeader(Http.Headers[header]);
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x0001FEF0 File Offset: 0x0001E0F0
		public Dictionary<string, string>.Enumerator EnumerateHeaders()
		{
			return this._headers.GetEnumerator();
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0001FF00 File Offset: 0x0001E100
		internal long LoadResponse(HttpMethod method, bool trackMiddleHeaders)
		{
			this.Method = method;
			this.Address = this._request.Address;
			this.HasError = false;
			this.MessageBodyLoaded = false;
			this.KeepAliveTimeout = null;
			this.MaximumKeepAliveRequests = null;
			if (trackMiddleHeaders && this._headers.Count > 0)
			{
				foreach (string text in this._headers.Keys)
				{
					this.MiddleHeaders[text] = this._headers[text];
				}
			}
			this._headers.Clear();
			if (this._request.UseCookies)
			{
				this.Cookies = ((this._request.Cookies != null && !this._request.Cookies.IsLocked) ? this._request.Cookies : new CookieStorage(false, null, this._request.IgnoreInvalidCookie));
			}
			if (this._receiverHelper == null)
			{
				this._receiverHelper = new HttpResponse.ReceiverHelper(this._request.TcpClient.ReceiveBufferSize);
			}
			this._receiverHelper.Init(this._request.ClientStream);
			try
			{
				this.ReceiveStartingLine();
				this.ReceiveHeaders();
				this.RedirectAddress = this.GetLocation();
				this.CharacterSet = this.GetCharacterSet();
				this.ContentLength = this.GetContentLength();
				this.ContentType = this.GetContentType();
				this.KeepAliveTimeout = this.GetKeepAliveTimeout();
				this.MaximumKeepAliveRequests = this.GetKeepAliveMax();
			}
			catch (Exception ex)
			{
				this.HasError = true;
				if (ex is IOException)
				{
					throw this.NewHttpException(Resources.HttpException_FailedReceiveResponse, ex);
				}
				throw;
			}
			if (this.ContentLength == 0L || this.Method == HttpMethod.HEAD || this.StatusCode == HttpStatusCode.Continue || this.StatusCode == HttpStatusCode.NoContent || this.StatusCode == HttpStatusCode.NotModified)
			{
				this._loadedMessageBody = string.Empty;
				this.MessageBodyLoaded = true;
			}
			long num = (long)this._receiverHelper.Position;
			if (this.ContentLength > 0L)
			{
				num += this.ContentLength;
			}
			return num;
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00020174 File Offset: 0x0001E374
		private void ReceiveStartingLine()
		{
			string text;
			for (;;)
			{
				text = this._receiverHelper.ReadLine();
				if (text.Length == 0)
				{
					break;
				}
				if (text != "\r\n")
				{
					goto Block_1;
				}
			}
			HttpException ex = this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse, null);
			ex.EmptyMessageBody = true;
			throw ex;
			Block_1:
			string text2 = text.Substring("HTTP/", " ", 0, StringComparison.Ordinal, null);
			string text3 = text.Substring(" ", " ", 0, StringComparison.Ordinal, null);
			if (string.IsNullOrEmpty(text3))
			{
				text3 = text.Substring(" ", "\r\n", 0, StringComparison.Ordinal, null);
			}
			if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3))
			{
				throw this.NewHttpException(Resources.HttpException_ReceivedEmptyResponse, null);
			}
			this.ProtocolVersion = Version.Parse(text2);
			this.StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), text3);
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00020250 File Offset: 0x0001E450
		private void ReceiveHeaders()
		{
			string text;
			for (;;)
			{
				text = this._receiverHelper.ReadLine();
				if (text == "\r\n")
				{
					break;
				}
				int num = text.IndexOf(':');
				if (num == -1)
				{
					goto Block_1;
				}
				string text2 = text.Substring(0, num);
				string text3 = text.Substring(num + 1).Trim(new char[] { ' ', '\t', '\r', '\n' });
				if (text2.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
				{
					this.ParseCookieFromHeader(text3);
				}
				else
				{
					this._headers[text2] = text3;
				}
			}
			return;
			Block_1:
			string text4 = string.Format(Resources.HttpException_WrongHeader, text, this.Address.Host);
			throw this.NewHttpException(text4, null);
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x00020308 File Offset: 0x0001E508
		private void ParseCookieFromHeader(string headerValue)
		{
			if (!this._request.UseCookies)
			{
				return;
			}
			this.Cookies.Set(this._request.Address, headerValue);
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00020344 File Offset: 0x0001E544
		private IEnumerable<HttpResponse.BytesWrapper> GetMessageBodySource()
		{
			if (!this._headers.ContainsKey("Content-Encoding") || !this._headers["Content-Encoding"].Equals("gzip", StringComparison.OrdinalIgnoreCase))
			{
				return this.GetMessageBodySourceStd();
			}
			return this.GetMessageBodySourceZip();
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00020398 File Offset: 0x0001E598
		private IEnumerable<HttpResponse.BytesWrapper> GetMessageBodySourceStd()
		{
			if (this._headers.ContainsKey("Transfer-Encoding"))
			{
				return this.ReceiveMessageBodyChunked();
			}
			if (this.ContentLength == -1L)
			{
				return HttpResponse.ReceiveMessageBody(this._request.ClientStream);
			}
			return this.ReceiveMessageBody(this.ContentLength);
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x000203F0 File Offset: 0x0001E5F0
		private IEnumerable<HttpResponse.BytesWrapper> GetMessageBodySourceZip()
		{
			if (this._headers.ContainsKey("Transfer-Encoding"))
			{
				return this.ReceiveMessageBodyChunkedZip();
			}
			if (this.ContentLength != -1L)
			{
				return this.ReceiveMessageBodyZip(this.ContentLength);
			}
			HttpResponse.ZipWrapperStream zipWrapperStream = new HttpResponse.ZipWrapperStream(this._request.ClientStream, this._receiverHelper);
			return HttpResponse.ReceiveMessageBody(this.GetZipStream(zipWrapperStream));
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x0002045C File Offset: 0x0001E65C
		private static byte[] GetResponse(Stream stream)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				array = memoryStream.ToArray();
			}
			return array;
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x000204A0 File Offset: 0x0001E6A0
		private static IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBody(Stream stream)
		{
			HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
			byte[] response = HttpResponse.GetResponse(stream);
			bytesWrapper.Value = response;
			bytesWrapper.Length = response.Length;
			return new HttpResponse.BytesWrapper[] { bytesWrapper };
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x000204D8 File Offset: 0x0001E6D8
		private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBody(long contentLength)
		{
			Stream stream = this._request.ClientStream;
			HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
			int bufferSize = this._request.TcpClient.ReceiveBufferSize;
			byte[] buffer = new byte[bufferSize];
			bytesWrapper.Value = buffer;
			int totalBytesRead = 0;
			while ((long)totalBytesRead != contentLength)
			{
				int num = (this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, 0, bufferSize) : stream.Read(buffer, 0, bufferSize));
				if (num == 0)
				{
					this.WaitData();
				}
				else
				{
					totalBytesRead += num;
					bytesWrapper.Length = num;
					yield return bytesWrapper;
				}
			}
			yield break;
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x000204F0 File Offset: 0x0001E6F0
		private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBodyChunked()
		{
			Stream stream = this._request.ClientStream;
			HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
			int bufferSize = this._request.TcpClient.ReceiveBufferSize;
			byte[] buffer = new byte[bufferSize];
			bytesWrapper.Value = buffer;
			for (;;)
			{
				string text = this._receiverHelper.ReadLine();
				if (!(text == "\r\n"))
				{
					text = text.Trim(new char[] { ' ', '\r', '\n' });
					if (text == string.Empty)
					{
						break;
					}
					int totalBytesRead = 0;
					int blockLength;
					try
					{
						blockLength = Convert.ToInt32(text, 16);
					}
					catch (Exception ex)
					{
						if (ex is FormatException || ex is OverflowException)
						{
							throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, text), ex);
						}
						throw;
					}
					if (blockLength == 0)
					{
						goto Block_4;
					}
					while (totalBytesRead != blockLength)
					{
						int num = blockLength - totalBytesRead;
						if (num > bufferSize)
						{
							num = bufferSize;
						}
						int num2 = (this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, 0, num) : stream.Read(buffer, 0, num));
						if (num2 == 0)
						{
							this.WaitData();
						}
						else
						{
							totalBytesRead += num2;
							bytesWrapper.Length = num2;
							yield return bytesWrapper;
						}
					}
				}
			}
			yield break;
			Block_4:
			yield break;
			yield break;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00020500 File Offset: 0x0001E700
		private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBodyZip(long contentLength)
		{
			HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
			HttpResponse.ZipWrapperStream streamWrapper = new HttpResponse.ZipWrapperStream(this._request.ClientStream, this._receiverHelper);
			using (Stream stream = this.GetZipStream(streamWrapper))
			{
				int bufferSize = this._request.TcpClient.ReceiveBufferSize;
				byte[] buffer = new byte[bufferSize];
				bytesWrapper.Value = buffer;
				for (;;)
				{
					int num = stream.Read(buffer, 0, bufferSize);
					if (num == 0)
					{
						if ((long)streamWrapper.TotalBytesRead == contentLength)
						{
							break;
						}
						this.WaitData();
					}
					else
					{
						bytesWrapper.Length = num;
						yield return bytesWrapper;
					}
				}
				yield break;
			}
			yield break;
			yield break;
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00020518 File Offset: 0x0001E718
		private IEnumerable<HttpResponse.BytesWrapper> ReceiveMessageBodyChunkedZip()
		{
			HttpResponse.BytesWrapper bytesWrapper = new HttpResponse.BytesWrapper();
			HttpResponse.ZipWrapperStream streamWrapper = new HttpResponse.ZipWrapperStream(this._request.ClientStream, this._receiverHelper);
			using (Stream stream = this.GetZipStream(streamWrapper))
			{
				int bufferSize = this._request.TcpClient.ReceiveBufferSize;
				byte[] buffer = new byte[bufferSize];
				bytesWrapper.Value = buffer;
				for (;;)
				{
					string text = this._receiverHelper.ReadLine();
					if (!(text == "\r\n"))
					{
						text = text.Trim(new char[] { ' ', '\r', '\n' });
						if (text == string.Empty)
						{
							break;
						}
						int blockLength;
						try
						{
							blockLength = Convert.ToInt32(text, 16);
						}
						catch (Exception ex)
						{
							if (ex is FormatException || ex is OverflowException)
							{
								throw this.NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, text), ex);
							}
							throw;
						}
						if (blockLength == 0)
						{
							goto Block_7;
						}
						streamWrapper.TotalBytesRead = 0;
						streamWrapper.LimitBytesRead = blockLength;
						for (;;)
						{
							int num = stream.Read(buffer, 0, bufferSize);
							if (num == 0)
							{
								if (streamWrapper.TotalBytesRead == blockLength)
								{
									break;
								}
								this.WaitData();
							}
							else
							{
								bytesWrapper.Length = num;
								yield return bytesWrapper;
							}
						}
					}
				}
				yield break;
				Block_7:
				yield break;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00020528 File Offset: 0x0001E728
		private bool ConnectionClosed()
		{
			return (this._headers.ContainsKey("Connection") && this._headers["Connection"].Equals("close", StringComparison.OrdinalIgnoreCase)) || (this._headers.ContainsKey("Proxy-Connection") && this._headers["Proxy-Connection"].Equals("close", StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x000205A4 File Offset: 0x0001E7A4
		private int? GetKeepAliveTimeout()
		{
			if (!this._headers.ContainsKey("Keep-Alive"))
			{
				return null;
			}
			string text = this._headers["Keep-Alive"];
			Match match = HttpResponse.KeepAliveTimeoutRegex.Match(text);
			if (match.Success)
			{
				return new int?(int.Parse(match.Groups["value"].Value) * 1000);
			}
			return null;
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x0002062C File Offset: 0x0001E82C
		private int? GetKeepAliveMax()
		{
			if (!this._headers.ContainsKey("Keep-Alive"))
			{
				return null;
			}
			string text = this._headers["Keep-Alive"];
			Match match = HttpResponse.KeepAliveMaxRegex.Match(text);
			if (match.Success)
			{
				return new int?(int.Parse(match.Groups["value"].Value));
			}
			return null;
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x000206B0 File Offset: 0x0001E8B0
		private Uri GetLocation()
		{
			string text;
			if (!this._headers.TryGetValue("Location", out text))
			{
				this._headers.TryGetValue("Redirect-Location", out text);
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			Uri uri;
			Uri.TryCreate(this._request.Address, text, out uri);
			return uri;
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x00020710 File Offset: 0x0001E910
		private Encoding GetCharacterSet()
		{
			if (!this._headers.ContainsKey("Content-Type"))
			{
				return this._request.CharacterSet ?? Encoding.Default;
			}
			string text = this._headers["Content-Type"];
			Match match = HttpResponse.ContentCharsetRegex.Match(text);
			if (!match.Success)
			{
				return this._request.CharacterSet ?? Encoding.Default;
			}
			Group group = match.Groups["value"];
			Encoding encoding;
			try
			{
				encoding = Encoding.GetEncoding(group.Value);
			}
			catch (ArgumentException)
			{
				encoding = this._request.CharacterSet ?? Encoding.Default;
			}
			return encoding;
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x000207DC File Offset: 0x0001E9DC
		private long GetContentLength()
		{
			string text = Http.Headers[HttpHeader.ContentLength];
			if (!this._headers.ContainsKey(text))
			{
				return -1L;
			}
			long num;
			if (!long.TryParse(this._headers[text], out num))
			{
				throw new FormatException("Invalid response header \"" + text + "\" value");
			}
			return num;
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00020840 File Offset: 0x0001EA40
		private string GetContentType()
		{
			string text = Http.Headers[HttpHeader.ContentType];
			if (!this._headers.ContainsKey(text))
			{
				return string.Empty;
			}
			string text2 = this._headers[text];
			int num = text2.IndexOf(';');
			if (num != -1)
			{
				text2 = text2.Substring(0, num);
			}
			return text2;
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0002089C File Offset: 0x0001EA9C
		private void WaitData()
		{
			int num = 0;
			int num2 = ((this._request.TcpClient.ReceiveTimeout < 10) ? 10 : this._request.TcpClient.ReceiveTimeout);
			while (!this._request.ClientNetworkStream.DataAvailable)
			{
				if (num >= num2)
				{
					throw this.NewHttpException(Resources.HttpException_WaitDataTimeout, null);
				}
				num += 10;
				Thread.Sleep(10);
			}
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x00020918 File Offset: 0x0001EB18
		private Stream GetZipStream(Stream stream)
		{
			string text = this._headers[Http.Headers[HttpHeader.ContentEncoding]].ToLower();
			if (text != null)
			{
				if (text == "gzip")
				{
					return new GZipStream(stream, CompressionMode.Decompress, true);
				}
				if (text == "deflate")
				{
					return new DeflateStream(stream, CompressionMode.Decompress, true);
				}
			}
			throw new InvalidOperationException(string.Format(Resources.InvalidOperationException_NotSupportedEncodingFormat, text));
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x00020994 File Offset: 0x0001EB94
		private static bool FindSignature(byte[] source, int sourceLength, byte[] signature)
		{
			int num = sourceLength - signature.Length + 1;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < signature.Length; j++)
				{
					char c = (char)source[j + i];
					if (char.IsLetter(c))
					{
						c = char.ToLower(c);
					}
					if ((byte)c != signature[j])
					{
						break;
					}
					if (j == signature.Length - 1)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x00020A00 File Offset: 0x0001EC00
		private HttpException NewHttpException(string message, Exception innerException = null)
		{
			return new HttpException(string.Format(message, this.Address.Host), HttpExceptionStatus.ReceiveFailure, HttpStatusCode.None, innerException);
		}

		// Token: 0x04000225 RID: 549
		private static readonly byte[] OpenHtmlSignature = Encoding.ASCII.GetBytes("<html");

		// Token: 0x04000226 RID: 550
		private static readonly byte[] CloseHtmlSignature = Encoding.ASCII.GetBytes("</html>");

		// Token: 0x04000227 RID: 551
		private static readonly Regex KeepAliveTimeoutRegex = new Regex("timeout(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000228 RID: 552
		private static readonly Regex KeepAliveMaxRegex = new Regex("max(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000229 RID: 553
		private static readonly Regex ContentCharsetRegex = new Regex("charset(|\\s+)=(|\\s+)(?<value>[a-z,0-9,-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x0400022A RID: 554
		private readonly HttpRequest _request;

		// Token: 0x0400022B RID: 555
		private HttpResponse.ReceiverHelper _receiverHelper;

		// Token: 0x0400022C RID: 556
		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0400022D RID: 557
		private Dictionary<string, string> _middleHeaders;

		// Token: 0x0400022E RID: 558
		private string _loadedMessageBody;

		// Token: 0x020001D7 RID: 471
		private sealed class BytesWrapper
		{
			// Token: 0x170003E6 RID: 998
			// (get) Token: 0x06001573 RID: 5491 RVA: 0x00075F28 File Offset: 0x00074128
			// (set) Token: 0x06001574 RID: 5492 RVA: 0x00075F30 File Offset: 0x00074130
			public int Length { get; set; }

			// Token: 0x170003E7 RID: 999
			// (get) Token: 0x06001575 RID: 5493 RVA: 0x00075F3C File Offset: 0x0007413C
			// (set) Token: 0x06001576 RID: 5494 RVA: 0x00075F44 File Offset: 0x00074144
			public byte[] Value { get; set; }
		}

		// Token: 0x020001D8 RID: 472
		private sealed class ReceiverHelper
		{
			// Token: 0x170003E8 RID: 1000
			// (get) Token: 0x06001578 RID: 5496 RVA: 0x00075F58 File Offset: 0x00074158
			public bool HasData
			{
				get
				{
					return this.Length - this.Position != 0;
				}
			}

			// Token: 0x170003E9 RID: 1001
			// (get) Token: 0x06001579 RID: 5497 RVA: 0x00075F6C File Offset: 0x0007416C
			// (set) Token: 0x0600157A RID: 5498 RVA: 0x00075F74 File Offset: 0x00074174
			private int Length { get; set; }

			// Token: 0x170003EA RID: 1002
			// (get) Token: 0x0600157B RID: 5499 RVA: 0x00075F80 File Offset: 0x00074180
			// (set) Token: 0x0600157C RID: 5500 RVA: 0x00075F88 File Offset: 0x00074188
			public int Position { get; private set; }

			// Token: 0x0600157D RID: 5501 RVA: 0x00075F94 File Offset: 0x00074194
			public ReceiverHelper(int bufferSize)
			{
				this._bufferSize = bufferSize;
				this._buffer = new byte[this._bufferSize];
			}

			// Token: 0x0600157E RID: 5502 RVA: 0x00075FC4 File Offset: 0x000741C4
			public void Init(Stream stream)
			{
				this._stream = stream;
				this._linePosition = 0;
				this.Length = 0;
				this.Position = 0;
			}

			// Token: 0x0600157F RID: 5503 RVA: 0x00075FE4 File Offset: 0x000741E4
			public string ReadLine()
			{
				this._linePosition = 0;
				for (;;)
				{
					if (this.Position == this.Length)
					{
						this.Position = 0;
						this.Length = this._stream.Read(this._buffer, 0, this._bufferSize);
						if (this.Length == 0)
						{
							break;
						}
					}
					byte[] buffer = this._buffer;
					int num = this.Position;
					this.Position = num + 1;
					byte b = buffer[num];
					byte[] lineBuffer = this._lineBuffer;
					num = this._linePosition;
					this._linePosition = num + 1;
					lineBuffer[num] = b;
					if (b == 10)
					{
						break;
					}
					if (this._linePosition == this._lineBuffer.Length)
					{
						byte[] array = new byte[this._lineBuffer.Length * 2];
						this._lineBuffer.CopyTo(array, 0);
						this._lineBuffer = array;
					}
				}
				return Encoding.ASCII.GetString(this._lineBuffer, 0, this._linePosition);
			}

			// Token: 0x06001580 RID: 5504 RVA: 0x000760CC File Offset: 0x000742CC
			public int Read(byte[] buffer, int index, int length)
			{
				int num = this.Length - this.Position;
				if (num > length)
				{
					num = length;
				}
				Array.Copy(this._buffer, this.Position, buffer, index, num);
				this.Position += num;
				return num;
			}

			// Token: 0x04000839 RID: 2105
			private const int InitialLineSize = 1000;

			// Token: 0x0400083A RID: 2106
			private Stream _stream;

			// Token: 0x0400083B RID: 2107
			private readonly byte[] _buffer;

			// Token: 0x0400083C RID: 2108
			private readonly int _bufferSize;

			// Token: 0x0400083D RID: 2109
			private int _linePosition;

			// Token: 0x0400083E RID: 2110
			private byte[] _lineBuffer = new byte[1000];
		}

		// Token: 0x020001D9 RID: 473
		private sealed class ZipWrapperStream : Stream
		{
			// Token: 0x170003EB RID: 1003
			// (get) Token: 0x06001581 RID: 5505 RVA: 0x00076118 File Offset: 0x00074318
			// (set) Token: 0x06001582 RID: 5506 RVA: 0x00076120 File Offset: 0x00074320
			private int BytesRead { get; set; }

			// Token: 0x170003EC RID: 1004
			// (get) Token: 0x06001583 RID: 5507 RVA: 0x0007612C File Offset: 0x0007432C
			// (set) Token: 0x06001584 RID: 5508 RVA: 0x00076134 File Offset: 0x00074334
			public int TotalBytesRead { get; set; }

			// Token: 0x170003ED RID: 1005
			// (get) Token: 0x06001585 RID: 5509 RVA: 0x00076140 File Offset: 0x00074340
			// (set) Token: 0x06001586 RID: 5510 RVA: 0x00076148 File Offset: 0x00074348
			public int LimitBytesRead { private get; set; }

			// Token: 0x170003EE RID: 1006
			// (get) Token: 0x06001587 RID: 5511 RVA: 0x00076154 File Offset: 0x00074354
			public override bool CanRead
			{
				get
				{
					return this._baseStream.CanRead;
				}
			}

			// Token: 0x170003EF RID: 1007
			// (get) Token: 0x06001588 RID: 5512 RVA: 0x00076164 File Offset: 0x00074364
			public override bool CanSeek
			{
				get
				{
					return this._baseStream.CanSeek;
				}
			}

			// Token: 0x170003F0 RID: 1008
			// (get) Token: 0x06001589 RID: 5513 RVA: 0x00076174 File Offset: 0x00074374
			public override bool CanTimeout
			{
				get
				{
					return this._baseStream.CanTimeout;
				}
			}

			// Token: 0x170003F1 RID: 1009
			// (get) Token: 0x0600158A RID: 5514 RVA: 0x00076184 File Offset: 0x00074384
			public override bool CanWrite
			{
				get
				{
					return this._baseStream.CanWrite;
				}
			}

			// Token: 0x170003F2 RID: 1010
			// (get) Token: 0x0600158B RID: 5515 RVA: 0x00076194 File Offset: 0x00074394
			public override long Length
			{
				get
				{
					return this._baseStream.Length;
				}
			}

			// Token: 0x170003F3 RID: 1011
			// (get) Token: 0x0600158C RID: 5516 RVA: 0x000761A4 File Offset: 0x000743A4
			// (set) Token: 0x0600158D RID: 5517 RVA: 0x000761B4 File Offset: 0x000743B4
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

			// Token: 0x0600158E RID: 5518 RVA: 0x000761C4 File Offset: 0x000743C4
			public ZipWrapperStream(Stream baseStream, HttpResponse.ReceiverHelper receiverHelper)
			{
				this._baseStream = baseStream;
				this._receiverHelper = receiverHelper;
			}

			// Token: 0x0600158F RID: 5519 RVA: 0x000761DC File Offset: 0x000743DC
			public override void Flush()
			{
				this._baseStream.Flush();
			}

			// Token: 0x06001590 RID: 5520 RVA: 0x000761EC File Offset: 0x000743EC
			public override void SetLength(long value)
			{
				this._baseStream.SetLength(value);
			}

			// Token: 0x06001591 RID: 5521 RVA: 0x000761FC File Offset: 0x000743FC
			public override long Seek(long offset, SeekOrigin origin)
			{
				return this._baseStream.Seek(offset, origin);
			}

			// Token: 0x06001592 RID: 5522 RVA: 0x0007620C File Offset: 0x0007440C
			public override int Read(byte[] buffer, int offset, int count)
			{
				if (this.LimitBytesRead != 0)
				{
					int num = this.LimitBytesRead - this.TotalBytesRead;
					if (num == 0)
					{
						return 0;
					}
					if (num > buffer.Length)
					{
						num = buffer.Length;
					}
					this.BytesRead = (this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, offset, num) : this._baseStream.Read(buffer, offset, num));
				}
				else
				{
					this.BytesRead = (this._receiverHelper.HasData ? this._receiverHelper.Read(buffer, offset, count) : this._baseStream.Read(buffer, offset, count));
				}
				this.TotalBytesRead += this.BytesRead;
				return this.BytesRead;
			}

			// Token: 0x06001593 RID: 5523 RVA: 0x000762D8 File Offset: 0x000744D8
			public override void Write(byte[] buffer, int offset, int count)
			{
				this._baseStream.Write(buffer, offset, count);
			}

			// Token: 0x04000841 RID: 2113
			private readonly Stream _baseStream;

			// Token: 0x04000842 RID: 2114
			private readonly HttpResponse.ReceiverHelper _receiverHelper;
		}
	}
}
