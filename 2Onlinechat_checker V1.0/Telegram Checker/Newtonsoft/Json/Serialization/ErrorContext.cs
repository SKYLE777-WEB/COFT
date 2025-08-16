using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000104 RID: 260
	public class ErrorContext
	{
		// Token: 0x06000DA1 RID: 3489 RVA: 0x000553DC File Offset: 0x000535DC
		internal ErrorContext(object originalObject, object member, string path, Exception error)
		{
			this.OriginalObject = originalObject;
			this.Member = member;
			this.Error = error;
			this.Path = path;
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000DA2 RID: 3490 RVA: 0x00055404 File Offset: 0x00053604
		// (set) Token: 0x06000DA3 RID: 3491 RVA: 0x0005540C File Offset: 0x0005360C
		internal bool Traced { get; set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x00055418 File Offset: 0x00053618
		public Exception Error { get; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000DA5 RID: 3493 RVA: 0x00055420 File Offset: 0x00053620
		public object OriginalObject { get; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x00055428 File Offset: 0x00053628
		public object Member { get; }

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x00055430 File Offset: 0x00053630
		public string Path { get; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x00055438 File Offset: 0x00053638
		// (set) Token: 0x06000DA9 RID: 3497 RVA: 0x00055440 File Offset: 0x00053640
		public bool Handled { get; set; }
	}
}
