using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000189 RID: 393
	internal enum BsonBinaryType : byte
	{
		// Token: 0x0400071D RID: 1821
		Binary,
		// Token: 0x0400071E RID: 1822
		Function,
		// Token: 0x0400071F RID: 1823
		[Obsolete("This type has been deprecated in the BSON specification. Use Binary instead.")]
		BinaryOld,
		// Token: 0x04000720 RID: 1824
		[Obsolete("This type has been deprecated in the BSON specification. Use Uuid instead.")]
		UuidOld,
		// Token: 0x04000721 RID: 1825
		Uuid,
		// Token: 0x04000722 RID: 1826
		Md5,
		// Token: 0x04000723 RID: 1827
		UserDefined = 128
	}
}
