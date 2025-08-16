using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000EF RID: 239
	internal class ReflectionMember
	{
		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x000509CC File Offset: 0x0004EBCC
		// (set) Token: 0x06000CC7 RID: 3271 RVA: 0x000509D4 File Offset: 0x0004EBD4
		public Type MemberType { get; set; }

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x000509E0 File Offset: 0x0004EBE0
		// (set) Token: 0x06000CC9 RID: 3273 RVA: 0x000509E8 File Offset: 0x0004EBE8
		public Func<object, object> Getter { get; set; }

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000CCA RID: 3274 RVA: 0x000509F4 File Offset: 0x0004EBF4
		// (set) Token: 0x06000CCB RID: 3275 RVA: 0x000509FC File Offset: 0x0004EBFC
		public Action<object, object> Setter { get; set; }
	}
}
