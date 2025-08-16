using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000CF RID: 207
	internal class TypeInformation
	{
		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000B9C RID: 2972 RVA: 0x00049304 File Offset: 0x00047504
		// (set) Token: 0x06000B9D RID: 2973 RVA: 0x0004930C File Offset: 0x0004750C
		public Type Type { get; set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000B9E RID: 2974 RVA: 0x00049318 File Offset: 0x00047518
		// (set) Token: 0x06000B9F RID: 2975 RVA: 0x00049320 File Offset: 0x00047520
		public PrimitiveTypeCode TypeCode { get; set; }
	}
}
