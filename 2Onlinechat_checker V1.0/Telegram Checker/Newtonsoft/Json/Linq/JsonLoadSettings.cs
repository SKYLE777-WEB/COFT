using System;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x0200014B RID: 331
	public class JsonLoadSettings
	{
		// Token: 0x060011D2 RID: 4562 RVA: 0x00065AB8 File Offset: 0x00063CB8
		public JsonLoadSettings()
		{
			this._lineInfoHandling = LineInfoHandling.Load;
			this._commentHandling = CommentHandling.Ignore;
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x060011D3 RID: 4563 RVA: 0x00065AD0 File Offset: 0x00063CD0
		// (set) Token: 0x060011D4 RID: 4564 RVA: 0x00065AD8 File Offset: 0x00063CD8
		public CommentHandling CommentHandling
		{
			get
			{
				return this._commentHandling;
			}
			set
			{
				if (value < CommentHandling.Ignore || value > CommentHandling.Load)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._commentHandling = value;
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x060011D5 RID: 4565 RVA: 0x00065AFC File Offset: 0x00063CFC
		// (set) Token: 0x060011D6 RID: 4566 RVA: 0x00065B04 File Offset: 0x00063D04
		public LineInfoHandling LineInfoHandling
		{
			get
			{
				return this._lineInfoHandling;
			}
			set
			{
				if (value < LineInfoHandling.Ignore || value > LineInfoHandling.Load)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lineInfoHandling = value;
			}
		}

		// Token: 0x04000698 RID: 1688
		private CommentHandling _commentHandling;

		// Token: 0x04000699 RID: 1689
		private LineInfoHandling _lineInfoHandling;
	}
}
