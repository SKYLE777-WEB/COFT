using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000B8 RID: 184
	public enum JsonToken
	{
		// Token: 0x04000419 RID: 1049
		None,
		// Token: 0x0400041A RID: 1050
		StartObject,
		// Token: 0x0400041B RID: 1051
		StartArray,
		// Token: 0x0400041C RID: 1052
		StartConstructor,
		// Token: 0x0400041D RID: 1053
		PropertyName,
		// Token: 0x0400041E RID: 1054
		Comment,
		// Token: 0x0400041F RID: 1055
		Raw,
		// Token: 0x04000420 RID: 1056
		Integer,
		// Token: 0x04000421 RID: 1057
		Float,
		// Token: 0x04000422 RID: 1058
		String,
		// Token: 0x04000423 RID: 1059
		Boolean,
		// Token: 0x04000424 RID: 1060
		Null,
		// Token: 0x04000425 RID: 1061
		Undefined,
		// Token: 0x04000426 RID: 1062
		EndObject,
		// Token: 0x04000427 RID: 1063
		EndArray,
		// Token: 0x04000428 RID: 1064
		EndConstructor,
		// Token: 0x04000429 RID: 1065
		Date,
		// Token: 0x0400042A RID: 1066
		Bytes
	}
}
