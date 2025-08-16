using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000192 RID: 402
	internal class BsonBoolean : BsonValue
	{
		// Token: 0x060014B3 RID: 5299 RVA: 0x0007252C File Offset: 0x0007072C
		private BsonBoolean(bool value)
			: base(value, BsonType.Boolean)
		{
		}

		// Token: 0x04000741 RID: 1857
		public static readonly BsonBoolean False = new BsonBoolean(false);

		// Token: 0x04000742 RID: 1858
		public static readonly BsonBoolean True = new BsonBoolean(true);
	}
}
