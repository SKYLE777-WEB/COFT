using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000190 RID: 400
	internal class BsonEmpty : BsonToken
	{
		// Token: 0x060014AD RID: 5293 RVA: 0x000724D0 File Offset: 0x000706D0
		private BsonEmpty(BsonType type)
		{
			this.Type = type;
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x000724E0 File Offset: 0x000706E0
		public override BsonType Type { get; }

		// Token: 0x0400073C RID: 1852
		public static readonly BsonToken Null = new BsonEmpty(BsonType.Null);

		// Token: 0x0400073D RID: 1853
		public static readonly BsonToken Undefined = new BsonEmpty(BsonType.Undefined);
	}
}
