using System;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x0200014C RID: 332
	public class JsonMergeSettings
	{
		// Token: 0x1700033B RID: 827
		// (get) Token: 0x060011D7 RID: 4567 RVA: 0x00065B28 File Offset: 0x00063D28
		// (set) Token: 0x060011D8 RID: 4568 RVA: 0x00065B30 File Offset: 0x00063D30
		public MergeArrayHandling MergeArrayHandling
		{
			get
			{
				return this._mergeArrayHandling;
			}
			set
			{
				if (value < MergeArrayHandling.Concat || value > MergeArrayHandling.Merge)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._mergeArrayHandling = value;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x060011D9 RID: 4569 RVA: 0x00065B54 File Offset: 0x00063D54
		// (set) Token: 0x060011DA RID: 4570 RVA: 0x00065B5C File Offset: 0x00063D5C
		public MergeNullValueHandling MergeNullValueHandling
		{
			get
			{
				return this._mergeNullValueHandling;
			}
			set
			{
				if (value < MergeNullValueHandling.Ignore || value > MergeNullValueHandling.Merge)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._mergeNullValueHandling = value;
			}
		}

		// Token: 0x0400069A RID: 1690
		private MergeArrayHandling _mergeArrayHandling;

		// Token: 0x0400069B RID: 1691
		private MergeNullValueHandling _mergeNullValueHandling;
	}
}
