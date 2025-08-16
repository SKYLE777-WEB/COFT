using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000197 RID: 407
	internal enum BsonType : sbyte
	{
		// Token: 0x0400074B RID: 1867
		Number = 1,
		// Token: 0x0400074C RID: 1868
		String,
		// Token: 0x0400074D RID: 1869
		Object,
		// Token: 0x0400074E RID: 1870
		Array,
		// Token: 0x0400074F RID: 1871
		Binary,
		// Token: 0x04000750 RID: 1872
		Undefined,
		// Token: 0x04000751 RID: 1873
		Oid,
		// Token: 0x04000752 RID: 1874
		Boolean,
		// Token: 0x04000753 RID: 1875
		Date,
		// Token: 0x04000754 RID: 1876
		Null,
		// Token: 0x04000755 RID: 1877
		Regex,
		// Token: 0x04000756 RID: 1878
		Reference,
		// Token: 0x04000757 RID: 1879
		Code,
		// Token: 0x04000758 RID: 1880
		Symbol,
		// Token: 0x04000759 RID: 1881
		CodeWScope,
		// Token: 0x0400075A RID: 1882
		Integer,
		// Token: 0x0400075B RID: 1883
		TimeStamp,
		// Token: 0x0400075C RID: 1884
		Long,
		// Token: 0x0400075D RID: 1885
		MinKey = -1,
		// Token: 0x0400075E RID: 1886
		MaxKey = 127
	}
}
