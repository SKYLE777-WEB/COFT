using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x0200018D RID: 397
	internal abstract class BsonToken
	{
		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x0600149D RID: 5277
		public abstract BsonType Type { get; }

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x0600149E RID: 5278 RVA: 0x000723E4 File Offset: 0x000705E4
		// (set) Token: 0x0600149F RID: 5279 RVA: 0x000723EC File Offset: 0x000705EC
		public BsonToken Parent { get; set; }

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x000723F8 File Offset: 0x000705F8
		// (set) Token: 0x060014A1 RID: 5281 RVA: 0x00072400 File Offset: 0x00070600
		public int CalculatedSize { get; set; }
	}
}
