using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Leaf.xNet
{
	// Token: 0x0200005D RID: 93
	[ComVisible(true)]
	public static class Http
	{
		// Token: 0x06000476 RID: 1142 RVA: 0x0001C344 File Offset: 0x0001A544
		public static string ToQueryString(IEnumerable<KeyValuePair<string, string>> parameters, bool valuesUnescaped = false, bool keysUnescaped = false)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in parameters)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Key))
				{
					stringBuilder.Append(keysUnescaped ? keyValuePair.Key : Uri.EscapeDataString(keyValuePair.Key));
					stringBuilder.Append('=');
					stringBuilder.Append(valuesUnescaped ? keyValuePair.Value : Uri.EscapeDataString(keyValuePair.Value ?? string.Empty));
					stringBuilder.Append('&');
				}
			}
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001C444 File Offset: 0x0001A644
		public static string DetermineMediaType(string extension)
		{
			return MimeMapping.GetMimeMapping(extension);
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x0001C44C File Offset: 0x0001A64C
		public static string IEUserAgent()
		{
			string text = Http.RandomWindowsVersion();
			string text2;
			string text3;
			string text4;
			string text5;
			if (text.Contains("NT 5.1"))
			{
				text2 = "9.0";
				text3 = "5.0";
				text4 = "5.0";
				text5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729";
			}
			else if (text.Contains("NT 6.0"))
			{
				text2 = "9.0";
				text3 = "5.0";
				text4 = "5.0";
				text5 = ".NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.30729";
			}
			else
			{
				int num = Randomizer.Instance.Next(3);
				if (num != 0)
				{
					if (num != 1)
					{
						text2 = "11.0";
						text4 = "7.0";
						text3 = "5.0";
					}
					else
					{
						text2 = "10.6";
						text4 = "6.0";
						text3 = "5.0";
					}
				}
				else
				{
					text2 = "10.0";
					text4 = "6.0";
					text3 = "5.0";
				}
				text5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E";
			}
			return string.Concat(new string[]
			{
				"Mozilla/", text3, " (compatible; MSIE ", text2, "; ", text, "; Trident/", text4, "; ", text5,
				")"
			});
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0001C57C File Offset: 0x0001A77C
		public static string OperaUserAgent()
		{
			string text;
			string text2;
			switch (Randomizer.Instance.Next(4))
			{
			case 0:
				text = "12.16";
				text2 = "2.12.388";
				break;
			case 1:
				text = "12.14";
				text2 = "2.12.388";
				break;
			case 2:
				text = "12.02";
				text2 = "2.10.289";
				break;
			default:
				text = "12.00";
				text2 = "2.10.181";
				break;
			}
			return string.Concat(new string[]
			{
				"Opera/9.80 (",
				Http.RandomWindowsVersion(),
				"); U) Presto/",
				text2,
				" Version/",
				text
			});
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0001C624 File Offset: 0x0001A824
		public static string ChromeUserAgent()
		{
			int num = Randomizer.Instance.Next(62, 70);
			int num2 = Randomizer.Instance.Next(2100, 3538);
			int num3 = Randomizer.Instance.Next(170);
			return "Mozilla/5.0 (" + Http.RandomWindowsVersion() + ") AppleWebKit/537.36 (KHTML, like Gecko) " + string.Format("Chrome/{0}.0.{1}.{2} Safari/537.36", num, num2, num3);
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0001C69C File Offset: 0x0001A89C
		public static string FirefoxUserAgent()
		{
			byte b = Http.FirefoxVersions[Randomizer.Instance.Next(Http.FirefoxVersions.Length - 1)];
			return string.Format("Mozilla/5.0 ({0}; rv:{1}.0) Gecko/20100101 Firefox/{2}.0", Http.RandomWindowsVersion(), b, b);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0001C6E4 File Offset: 0x0001A8E4
		public static string OperaMiniUserAgent()
		{
			int num = Randomizer.Instance.Next(3);
			string text;
			string text2;
			string text3;
			string text4;
			if (num != 0)
			{
				if (num != 1)
				{
					text = "Android";
					text2 = "7.5.54678";
					text3 = "12.02";
					text4 = "2.10.289";
				}
				else
				{
					text = "J2ME/MIDP";
					text2 = "7.1.23511";
					text3 = "12.00";
					text4 = "2.10.181";
				}
			}
			else
			{
				text = "iOS";
				text2 = "7.0.73345";
				text3 = "11.62";
				text4 = "2.10.229";
			}
			return string.Concat(new string[] { "Opera/9.80 (", text, "; Opera Mini/", text2, "/28.2555; U; ru) Presto/", text4, " Version/", text3 });
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x0001C7A4 File Offset: 0x0001A9A4
		public static string RandomUserAgent()
		{
			int num = Randomizer.Instance.Next(99) + 1;
			if (num >= 1 && num <= 70)
			{
				return Http.ChromeUserAgent();
			}
			if (num > 70 && num <= 85)
			{
				return Http.FirefoxUserAgent();
			}
			if (num > 85 && num <= 91)
			{
				return Http.IEUserAgent();
			}
			if (num > 91 && num <= 96)
			{
				return Http.OperaUserAgent();
			}
			return Http.OperaMiniUserAgent();
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0001C820 File Offset: 0x0001AA20
		private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001C824 File Offset: 0x0001AA24
		private static string RandomWindowsVersion()
		{
			string text = "Windows NT ";
			int num = Randomizer.Instance.Next(99) + 1;
			if (num >= 1 && num <= 45)
			{
				text += "10.0";
			}
			else if (num > 45 && num <= 80)
			{
				text += "6.1";
			}
			else if (num > 80 && num <= 95)
			{
				text += "6.3";
			}
			else
			{
				text += "6.2";
			}
			if (Randomizer.Instance.NextDouble() <= 0.65)
			{
				text += ((Randomizer.Instance.NextDouble() <= 0.5) ? "; WOW64" : "; Win64; x64");
			}
			return text;
		}

		// Token: 0x040001A7 RID: 423
		public const string NewLine = "\r\n";

		// Token: 0x040001A8 RID: 424
		public static readonly RemoteCertificateValidationCallback AcceptAllCertificationsCallback = new RemoteCertificateValidationCallback(Http.AcceptAllCertifications);

		// Token: 0x040001A9 RID: 425
		internal static readonly Dictionary<HttpHeader, string> Headers = new Dictionary<HttpHeader, string>
		{
			{
				HttpHeader.Accept,
				"Accept"
			},
			{
				HttpHeader.AcceptCharset,
				"Accept-Charset"
			},
			{
				HttpHeader.AcceptLanguage,
				"Accept-Language"
			},
			{
				HttpHeader.AcceptDatetime,
				"Accept-Datetime"
			},
			{
				HttpHeader.CacheControl,
				"Cache-Control"
			},
			{
				HttpHeader.ContentType,
				"Content-Type"
			},
			{
				HttpHeader.Date,
				"Date"
			},
			{
				HttpHeader.Expect,
				"Expect"
			},
			{
				HttpHeader.From,
				"From"
			},
			{
				HttpHeader.IfMatch,
				"If-Match"
			},
			{
				HttpHeader.IfModifiedSince,
				"If-Modified-Since"
			},
			{
				HttpHeader.IfNoneMatch,
				"If-None-Match"
			},
			{
				HttpHeader.IfRange,
				"If-Range"
			},
			{
				HttpHeader.IfUnmodifiedSince,
				"If-Unmodified-Since"
			},
			{
				HttpHeader.MaxForwards,
				"Max-Forwards"
			},
			{
				HttpHeader.Pragma,
				"Pragma"
			},
			{
				HttpHeader.Range,
				"Range"
			},
			{
				HttpHeader.Referer,
				"Referer"
			},
			{
				HttpHeader.Origin,
				"Origin"
			},
			{
				HttpHeader.Upgrade,
				"Upgrade"
			},
			{
				HttpHeader.UpgradeInsecureRequests,
				"Upgrade-Insecure-Requests"
			},
			{
				HttpHeader.UserAgent,
				"User-Agent"
			},
			{
				HttpHeader.Via,
				"Via"
			},
			{
				HttpHeader.Warning,
				"Warning"
			},
			{
				HttpHeader.DNT,
				"DNT"
			},
			{
				HttpHeader.AccessControlAllowOrigin,
				"Access-Control-Allow-Origin"
			},
			{
				HttpHeader.AcceptRanges,
				"Accept-Ranges"
			},
			{
				HttpHeader.Age,
				"Age"
			},
			{
				HttpHeader.Allow,
				"Allow"
			},
			{
				HttpHeader.ContentEncoding,
				"Content-Encoding"
			},
			{
				HttpHeader.ContentLanguage,
				"Content-Language"
			},
			{
				HttpHeader.ContentLength,
				"Content-Length"
			},
			{
				HttpHeader.ContentLocation,
				"Content-Location"
			},
			{
				HttpHeader.ContentMD5,
				"Content-MD5"
			},
			{
				HttpHeader.ContentDisposition,
				"Content-Disposition"
			},
			{
				HttpHeader.ContentRange,
				"Content-Range"
			},
			{
				HttpHeader.ETag,
				"ETag"
			},
			{
				HttpHeader.Expires,
				"Expires"
			},
			{
				HttpHeader.LastModified,
				"Last-Modified"
			},
			{
				HttpHeader.Link,
				"Link"
			},
			{
				HttpHeader.Location,
				"Location"
			},
			{
				HttpHeader.P3P,
				"P3P"
			},
			{
				HttpHeader.Refresh,
				"Refresh"
			},
			{
				HttpHeader.RetryAfter,
				"Retry-After"
			},
			{
				HttpHeader.Server,
				"Server"
			},
			{
				HttpHeader.TransferEncoding,
				"Transfer-Encoding"
			}
		};

		// Token: 0x040001AA RID: 426
		private static readonly byte[] FirefoxVersions = new byte[] { 64, 63, 62, 60, 58, 52, 51, 46, 45 };
	}
}
