using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Leaf.xNet
{
	// Token: 0x0200005C RID: 92
	[ComVisible(true)]
	[Serializable]
	public class CookieStorage
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x0001B6AC File Offset: 0x000198AC
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x0001B6B4 File Offset: 0x000198B4
		public CookieContainer Container { get; private set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x0001B6C0 File Offset: 0x000198C0
		public int Count
		{
			get
			{
				return this.Container.Count;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x0001B6D0 File Offset: 0x000198D0
		// (set) Token: 0x0600044D RID: 1101 RVA: 0x0001B6D8 File Offset: 0x000198D8
		public bool IsLocked { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x0001B6E4 File Offset: 0x000198E4
		// (set) Token: 0x0600044F RID: 1103 RVA: 0x0001B6EC File Offset: 0x000198EC
		public static bool DefaultExpireBeforeSet { get; set; } = true;

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000450 RID: 1104 RVA: 0x0001B6F4 File Offset: 0x000198F4
		// (set) Token: 0x06000451 RID: 1105 RVA: 0x0001B6FC File Offset: 0x000198FC
		public bool ExpireBeforeSet { get; set; } = CookieStorage.DefaultExpireBeforeSet;

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x0001B708 File Offset: 0x00019908
		// (set) Token: 0x06000453 RID: 1107 RVA: 0x0001B710 File Offset: 0x00019910
		public bool EscapeValuesOnReceive { get; set; } = true;

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x0001B71C File Offset: 0x0001991C
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x0001B724 File Offset: 0x00019924
		public bool IgnoreInvalidCookie { get; set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x0001B730 File Offset: 0x00019930
		// (set) Token: 0x06000457 RID: 1111 RVA: 0x0001B738 File Offset: 0x00019938
		public bool IgnoreSetForExpiredCookies { get; set; } = true;

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x0001B744 File Offset: 0x00019944
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x0001B760 File Offset: 0x00019960
		public bool UnescapeValuesOnSend
		{
			get
			{
				if (this._unescapeValuesOnSendCustomized)
				{
					return this._unescapeValuesOnSend;
				}
				return this.EscapeValuesOnReceive;
			}
			set
			{
				this._unescapeValuesOnSendCustomized = true;
				this._unescapeValuesOnSend = value;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x0001B770 File Offset: 0x00019970
		private static BinaryFormatter Bf
		{
			get
			{
				BinaryFormatter binaryFormatter;
				if ((binaryFormatter = CookieStorage._binaryFormatter) == null)
				{
					binaryFormatter = (CookieStorage._binaryFormatter = new BinaryFormatter());
				}
				return binaryFormatter;
			}
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0001B78C File Offset: 0x0001998C
		public CookieStorage(bool isLocked = false, CookieContainer container = null, bool ignoreInvalidCookie = false)
		{
			this.IsLocked = isLocked;
			this.Container = container ?? new CookieContainer();
			this.IgnoreInvalidCookie = ignoreInvalidCookie;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0001B7E0 File Offset: 0x000199E0
		public void Add(Cookie cookie)
		{
			this.Container.Add(cookie);
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0001B7F0 File Offset: 0x000199F0
		public void Add(CookieCollection cookies)
		{
			this.Container.Add(cookies);
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x0001B800 File Offset: 0x00019A00
		public void Set(Cookie cookie)
		{
			cookie.Name = cookie.Name.Trim();
			cookie.Value = cookie.Value.Trim();
			if (this.ExpireBeforeSet)
			{
				this.ExpireIfExists(cookie);
			}
			this.Add(cookie);
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x0001B84C File Offset: 0x00019A4C
		public void Set(CookieCollection cookies)
		{
			if (this.ExpireBeforeSet)
			{
				foreach (object obj in cookies)
				{
					Cookie cookie = (Cookie)obj;
					this.ExpireIfExists(cookie);
				}
			}
			this.Add(cookies);
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001B8BC File Offset: 0x00019ABC
		public void Set(string name, string value, string domain, string path = "/")
		{
			Cookie cookie = new Cookie(name, value, path, domain);
			this.Set(cookie);
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x0001B8E0 File Offset: 0x00019AE0
		public void Set(Uri requestAddress, string rawCookie)
		{
			string[] array = rawCookie.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				return;
			}
			string[] array2 = array[0].Split(new char[] { '=' }, 2);
			if (array2.Length <= 1)
			{
				return;
			}
			array2[0] = array2[0].Trim();
			array2[1] = array2[1].Trim();
			if (this.IgnoreInvalidCookie && (string.IsNullOrEmpty(array2[0]) || array2[0][0] == '$' || array2[0].IndexOfAny(CookieStorage.ReservedChars) != -1))
			{
				return;
			}
			Cookie cookie = new Cookie(array2[0], (array2.Length < 2) ? string.Empty : (this.EscapeValuesOnReceive ? Uri.EscapeDataString(array2[1]) : array2[1]));
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new char[] { '=' }, 2);
				string text = array3[0].Trim().ToLower();
				string text2 = ((array3.Length < 2) ? null : array3[1].Trim());
				if (text != null)
				{
					if (!(text == "expires"))
					{
						if (!(text == "path"))
						{
							if (!(text == "domain"))
							{
								if (!(text == "secure"))
								{
									if (text == "httponly")
									{
										cookie.HttpOnly = true;
									}
								}
								else
								{
									cookie.Secure = true;
								}
							}
							else
							{
								string text3 = CookieFilters.FilterDomain(text2);
								if (text3 != null)
								{
									flag = true;
									cookie.Domain = text3;
								}
							}
						}
						else
						{
							cookie.Path = text2;
						}
					}
					else
					{
						DateTime dateTime;
						if (!DateTime.TryParse(text2, out dateTime) || dateTime.Year >= 9999)
						{
							dateTime = new DateTime(9998, 12, 31, 23, 59, 59, DateTimeKind.Local);
						}
						cookie.Expires = dateTime;
					}
				}
			}
			if (!flag)
			{
				if (string.IsNullOrEmpty(cookie.Path) || cookie.Path.StartsWith("/"))
				{
					cookie.Domain = requestAddress.Host;
				}
				else if (cookie.Path.Contains("."))
				{
					string path = cookie.Path;
					cookie.Domain = path;
					cookie.Path = null;
				}
			}
			if (this.IgnoreSetForExpiredCookies && cookie.Expired)
			{
				return;
			}
			this.Set(cookie);
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x0001BBB4 File Offset: 0x00019DB4
		public void Set(string requestAddress, string rawCookie)
		{
			this.Set(new Uri(requestAddress), rawCookie);
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0001BBC4 File Offset: 0x00019DC4
		private void ExpireIfExists(Uri uri, string cookieName)
		{
			foreach (object obj in this.Container.GetCookies(uri))
			{
				Cookie cookie = (Cookie)obj;
				if (cookie.Name == cookieName)
				{
					cookie.Expired = true;
				}
			}
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0001BC40 File Offset: 0x00019E40
		private void ExpireIfExists(Cookie cookie)
		{
			if (string.IsNullOrEmpty(cookie.Domain))
			{
				return;
			}
			string text = ((cookie.Domain[0] == '.') ? cookie.Domain.Substring(1) : cookie.Domain);
			Uri uri = new Uri((cookie.Secure ? "https://" : "http://") + text);
			this.ExpireIfExists(uri, cookie.Name);
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001BCC0 File Offset: 0x00019EC0
		public void Clear()
		{
			this.Container = new CookieContainer();
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0001BCD0 File Offset: 0x00019ED0
		public void Remove(string url)
		{
			this.Remove(new Uri(url));
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001BCE0 File Offset: 0x00019EE0
		public void Remove(Uri uri)
		{
			foreach (object obj in this.Container.GetCookies(uri))
			{
				((Cookie)obj).Expired = true;
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001BD48 File Offset: 0x00019F48
		public void Remove(string url, string name)
		{
			this.Remove(new Uri(url), name);
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x0001BD58 File Offset: 0x00019F58
		public void Remove(Uri uri, string name)
		{
			foreach (object obj in this.Container.GetCookies(uri))
			{
				Cookie cookie = (Cookie)obj;
				if (cookie.Name == name)
				{
					cookie.Expired = true;
				}
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0001BDD4 File Offset: 0x00019FD4
		public string GetCookieHeader(Uri uri)
		{
			string cookieHeader = this.Container.GetCookieHeader(uri);
			if (!this.UnescapeValuesOnSend)
			{
				return cookieHeader;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = cookieHeader.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[] { '=' }, 2);
				stringBuilder.Append(array2[0].Trim());
				stringBuilder.Append('=');
				stringBuilder.Append(Uri.UnescapeDataString(array2[1].Trim()));
				stringBuilder.Append("; ");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001BEA8 File Offset: 0x0001A0A8
		public string GetCookieHeader(string url)
		{
			return this.GetCookieHeader(new Uri(url));
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001BEB8 File Offset: 0x0001A0B8
		public CookieCollection GetCookies(Uri uri)
		{
			return this.Container.GetCookies(uri);
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001BEC8 File Offset: 0x0001A0C8
		public CookieCollection GetCookies(string url)
		{
			return this.GetCookies(new Uri(url));
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001BED8 File Offset: 0x0001A0D8
		public bool Contains(Uri uri, string cookieName)
		{
			return this.Container.Count > 0 && this.Container.GetCookies(uri)[cookieName] != null;
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0001BF14 File Offset: 0x0001A114
		public bool Contains(string url, string cookieName)
		{
			return this.Contains(new Uri(url), cookieName);
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001BF24 File Offset: 0x0001A124
		public void SaveToFile(string filePath, bool overwrite = true)
		{
			if (!overwrite && File.Exists(filePath))
			{
				throw new ArgumentException(string.Format(Resources.CookieStorage_SaveToFile_FileAlreadyExists, filePath), "filePath");
			}
			using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
			{
				CookieStorage.Bf.Serialize(fileStream, this);
			}
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0001BF90 File Offset: 0x0001A190
		public static CookieStorage LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("Файл с куками '$" + filePath + "' не найден", "filePath");
			}
			CookieStorage cookieStorage;
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
			{
				cookieStorage = (CookieStorage)CookieStorage.Bf.Deserialize(fileStream);
			}
			return cookieStorage;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001C000 File Offset: 0x0001A200
		public byte[] ToBytes()
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				CookieStorage.Bf.Serialize(memoryStream, this);
				array = memoryStream.ToArray();
			}
			return array;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0001C04C File Offset: 0x0001A24C
		public static CookieStorage FromBytes(byte[] bytes)
		{
			CookieStorage cookieStorage;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				cookieStorage = (CookieStorage)CookieStorage.Bf.Deserialize(memoryStream);
			}
			return cookieStorage;
		}

		// Token: 0x040001A3 RID: 419
		private bool _unescapeValuesOnSend;

		// Token: 0x040001A4 RID: 420
		private bool _unescapeValuesOnSendCustomized;

		// Token: 0x040001A5 RID: 421
		private static readonly char[] ReservedChars = new char[] { ' ', '\t', '\r', '\n', '=', ';', ',' };

		// Token: 0x040001A6 RID: 422
		private static BinaryFormatter _binaryFormatter;
	}
}
