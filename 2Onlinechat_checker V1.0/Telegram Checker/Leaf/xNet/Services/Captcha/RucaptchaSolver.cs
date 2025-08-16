using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x02000084 RID: 132
	[ComVisible(true)]
	public class RucaptchaSolver : BaseCaptchaSolver
	{
		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x00023EFC File Offset: 0x000220FC
		// (set) Token: 0x0600068C RID: 1676 RVA: 0x00023F04 File Offset: 0x00022104
		public string Host { get; protected set; } = "rucaptcha.com";

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x00023F10 File Offset: 0x00022110
		// (set) Token: 0x0600068E RID: 1678 RVA: 0x00023F18 File Offset: 0x00022118
		public CaptchaProxy Proxy { get; set; }

		// Token: 0x0600068F RID: 1679 RVA: 0x00023F24 File Offset: 0x00022124
		public override string SolveRecaptcha(string pageUrl, string siteKey, CancellationToken cancelToken = default(CancellationToken))
		{
			base.ThrowIfApiKeyRequiredAndInvalid();
			if (string.IsNullOrEmpty(pageUrl))
			{
				throw new ArgumentException("Invalid argument: \"pageUrl\" = " + (pageUrl ?? "null") + " when called \"SolveRecaptcha\"", "pageUrl");
			}
			if (string.IsNullOrEmpty(siteKey))
			{
				throw new ArgumentException("Invalid argument: \"siteKey\" = " + (siteKey ?? "null") + " when called \"SolveRecaptcha\"", "siteKey");
			}
			NameValueCollection nameValueCollection = new NameValueCollection
			{
				{ "key", base.ApiKey },
				{ "method", "userrecaptcha" },
				{ "googlekey", siteKey },
				{ "pageurl", pageUrl }
			};
			CaptchaProxy captchaProxy = this.Proxy;
			if (captchaProxy.IsValid)
			{
				nameValueCollection.Add("proxy", this.Proxy.Address);
				NameValueCollection nameValueCollection2 = nameValueCollection;
				string text = "proxytype";
				captchaProxy = this.Proxy;
				nameValueCollection2.Add(text, captchaProxy.Type.ToString());
			}
			string text2 = "unknown";
			bool flag = true;
			int num = 0;
			while ((long)num < (long)((ulong)base.UploadRetries))
			{
				cancelToken.ThrowIfCancellationRequested();
				text2 = Encoding.UTF8.GetString(this.Http.UploadValues("http://" + this.Host + "/in.php", nameValueCollection));
				if (!text2.Contains("ERROR_NO_SLOT_AVAILABLE"))
				{
					flag = !text2.Contains("OK|");
					break;
				}
				base.Delay(base.UploadDelayOnNoSlotAvailable, cancelToken);
				num++;
			}
			if (flag)
			{
				throw new CaptchaException(text2);
			}
			string text3 = text2.Replace("OK|", "").Trim();
			flag = true;
			base.Delay(base.BeforeStatusCheckingDelay, cancelToken);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)base.StatusRetries))
			{
				text2 = this.Http.DownloadString(string.Concat(new string[] { "http://", this.Host, "/res.php?key=", base.ApiKey, "&action=get&id=", text3 }));
				if (!text2.Contains("CAPCHA_NOT_READY"))
				{
					flag = !text2.Contains("OK|");
					break;
				}
				base.Delay(base.StatusDelayOnNotReady, cancelToken);
				num2++;
			}
			cancelToken.ThrowIfCancellationRequested();
			if (flag)
			{
				throw new CaptchaException(text2);
			}
			string text4 = text2.Replace("OK|", "");
			if (string.IsNullOrEmpty(text4))
			{
				throw new CaptchaException(CaptchaError.EmptyResponse);
			}
			return text4;
		}
	}
}
