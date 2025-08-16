using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leaf.xNet.Services.Captcha
{
	// Token: 0x02000082 RID: 130
	[ComVisible(true)]
	public interface ICaptchaSolver
	{
		// Token: 0x17000141 RID: 321
		// (get) Token: 0x0600067B RID: 1659
		// (set) Token: 0x0600067C RID: 1660
		uint UploadRetries { get; set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x0600067D RID: 1661
		// (set) Token: 0x0600067E RID: 1662
		uint StatusRetries { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x0600067F RID: 1663
		// (set) Token: 0x06000680 RID: 1664
		TimeSpan UploadDelayOnNoSlotAvailable { get; set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000681 RID: 1665
		// (set) Token: 0x06000682 RID: 1666
		TimeSpan StatusDelayOnNotReady { get; set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000683 RID: 1667
		// (set) Token: 0x06000684 RID: 1668
		TimeSpan BeforeStatusCheckingDelay { get; set; }

		// Token: 0x06000685 RID: 1669
		string SolveImage(string imageUrl, CancellationToken cancelToken = default(CancellationToken));

		// Token: 0x06000686 RID: 1670
		string SolveImage(byte[] imageBytes, CancellationToken cancelToken = default(CancellationToken));

		// Token: 0x06000687 RID: 1671
		string SolveImage(Stream imageStream, CancellationToken cancelToken = default(CancellationToken));

		// Token: 0x06000688 RID: 1672
		string SolveImageFromBase64(string imageBase64, CancellationToken cancelToken = default(CancellationToken));

		// Token: 0x06000689 RID: 1673
		string SolveRecaptcha(string pageUrl, string siteKey, CancellationToken cancelToken = default(CancellationToken));
	}
}
