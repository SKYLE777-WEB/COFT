using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000196 RID: 406
	internal class BsonProperty
	{
		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x060014C2 RID: 5314 RVA: 0x000725FC File Offset: 0x000707FC
		// (set) Token: 0x060014C3 RID: 5315 RVA: 0x00072604 File Offset: 0x00070804
		public BsonString Name { get; set; }

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x060014C4 RID: 5316 RVA: 0x00072610 File Offset: 0x00070810
		// (set) Token: 0x060014C5 RID: 5317 RVA: 0x00072618 File Offset: 0x00070818
		public BsonToken Value { get; set; }
	}
}
