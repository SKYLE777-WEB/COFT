using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000193 RID: 403
	internal class BsonString : BsonValue
	{
		// Token: 0x170003CE RID: 974
		// (get) Token: 0x060014B5 RID: 5301 RVA: 0x00072554 File Offset: 0x00070754
		// (set) Token: 0x060014B6 RID: 5302 RVA: 0x0007255C File Offset: 0x0007075C
		public int ByteCount { get; set; }

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x060014B7 RID: 5303 RVA: 0x00072568 File Offset: 0x00070768
		public bool IncludeLength { get; }

		// Token: 0x060014B8 RID: 5304 RVA: 0x00072570 File Offset: 0x00070770
		public BsonString(object value, bool includeLength)
			: base(value, BsonType.String)
		{
			this.IncludeLength = includeLength;
		}
	}
}
