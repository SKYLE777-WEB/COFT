using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000A9 RID: 169
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class JsonExtensionDataAttribute : Attribute
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x00039A54 File Offset: 0x00037C54
		// (set) Token: 0x060007EA RID: 2026 RVA: 0x00039A5C File Offset: 0x00037C5C
		public bool WriteData { get; set; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060007EB RID: 2027 RVA: 0x00039A68 File Offset: 0x00037C68
		// (set) Token: 0x060007EC RID: 2028 RVA: 0x00039A70 File Offset: 0x00037C70
		public bool ReadData { get; set; }

		// Token: 0x060007ED RID: 2029 RVA: 0x00039A7C File Offset: 0x00037C7C
		public JsonExtensionDataAttribute()
		{
			this.WriteData = true;
			this.ReadData = true;
		}
	}
}
