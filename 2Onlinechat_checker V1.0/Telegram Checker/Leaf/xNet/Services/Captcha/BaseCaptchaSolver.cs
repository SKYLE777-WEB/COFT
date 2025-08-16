using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x0200007D RID: 125
	[ComVisible(true)]
	public abstract class BaseCaptchaSolver : ICaptchaSolver, IDisposable
	{
		// Token: 0x17000137 RID: 311
		// (get) Token: 0x0600065B RID: 1627 RVA: 0x00023B28 File Offset: 0x00021D28
		// (set) Token: 0x0600065C RID: 1628 RVA: 0x00023B30 File Offset: 0x00021D30
		public string ApiKey { get; set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x0600065D RID: 1629 RVA: 0x00023B3C File Offset: 0x00021D3C
		// (set) Token: 0x0600065E RID: 1630 RVA: 0x00023B44 File Offset: 0x00021D44
		public bool IsApiKeyRequired { get; protected set; } = true;

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x0600065F RID: 1631 RVA: 0x00023B50 File Offset: 0x00021D50
		public bool IsApiKeyValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.ApiKey);
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x00023B60 File Offset: 0x00021D60
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x00023B68 File Offset: 0x00021D68
		public uint UploadRetries { get; set; } = 40U;

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x00023B74 File Offset: 0x00021D74
		// (set) Token: 0x06000663 RID: 1635 RVA: 0x00023B7C File Offset: 0x00021D7C
		public uint StatusRetries { get; set; } = 80U;

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x00023B88 File Offset: 0x00021D88
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x00023B90 File Offset: 0x00021D90
		public TimeSpan UploadDelayOnNoSlotAvailable { get; set; } = TimeSpan.FromSeconds(5.0);

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x00023B9C File Offset: 0x00021D9C
		// (set) Token: 0x06000667 RID: 1639 RVA: 0x00023BA4 File Offset: 0x00021DA4
		public TimeSpan StatusDelayOnNotReady { get; set; } = TimeSpan.FromSeconds(3.0);

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x00023BB0 File Offset: 0x00021DB0
		// (set) Token: 0x06000669 RID: 1641 RVA: 0x00023BB8 File Offset: 0x00021DB8
		public TimeSpan BeforeStatusCheckingDelay { get; set; } = TimeSpan.FromSeconds(3.0);

		// Token: 0x0600066A RID: 1642 RVA: 0x00023BC4 File Offset: 0x00021DC4
		public virtual string SolveImage(string imageUrl, CancellationToken cancelToken = default(CancellationToken))
		{
			throw this.NotImplemented("SolveImage", "string");
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x00023BD8 File Offset: 0x00021DD8
		public virtual string SolveImage(byte[] imageBytes, CancellationToken cancelToken = default(CancellationToken))
		{
			throw this.NotImplemented("SolveImage", "byte[]");
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x00023BEC File Offset: 0x00021DEC
		public virtual string SolveImage(Stream imageStream, CancellationToken cancelToken = default(CancellationToken))
		{
			throw this.NotImplemented("SolveImage", "Stream");
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00023C00 File Offset: 0x00021E00
		public string SolveImageFromBase64(string imageBase64, CancellationToken cancelToken = default(CancellationToken))
		{
			throw this.NotImplemented("SolveImageFromBase64", "string");
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x00023C14 File Offset: 0x00021E14
		public virtual string SolveRecaptcha(string pageUrl, string siteKey, CancellationToken cancelToken = default(CancellationToken))
		{
			throw this.NotImplemented("SolveRecaptcha", "string, string");
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00023C28 File Offset: 0x00021E28
		protected void ThrowIfApiKeyRequiredAndInvalid()
		{
			if (this.IsApiKeyRequired && !this.IsApiKeyValid)
			{
				throw new CaptchaException(CaptchaError.InvalidApiKey);
			}
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x00023C48 File Offset: 0x00021E48
		protected void Delay(TimeSpan delay, CancellationToken cancelToken)
		{
			if (cancelToken != CancellationToken.None)
			{
				cancelToken.WaitHandle.WaitOne(this.UploadDelayOnNoSlotAvailable);
				cancelToken.ThrowIfCancellationRequested();
				return;
			}
			Thread.Sleep(this.UploadDelayOnNoSlotAvailable);
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00023C90 File Offset: 0x00021E90
		private NotImplementedException NotImplemented(string method, string parameterType)
		{
			return new NotImplementedException(string.Concat(new string[]
			{
				"Method \"",
				method,
				"\"(",
				parameterType,
				") of ",
				base.GetType().Name,
				" isn't implemented"
			}));
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00023CE8 File Offset: 0x00021EE8
		public virtual void Dispose()
		{
			AdvancedWebClient http = this.Http;
			if (http == null)
			{
				return;
			}
			http.Dispose();
		}

		// Token: 0x040002D2 RID: 722
		public const string NameOfString = "string";

		// Token: 0x040002D3 RID: 723
		protected readonly AdvancedWebClient Http = new AdvancedWebClient();
	}
}
