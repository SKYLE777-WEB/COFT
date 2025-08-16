using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000A7 RID: 167
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	public sealed class JsonDictionaryAttribute : JsonContainerAttribute
	{
		// Token: 0x060007E2 RID: 2018 RVA: 0x00039A00 File Offset: 0x00037C00
		public JsonDictionaryAttribute()
		{
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00039A08 File Offset: 0x00037C08
		public JsonDictionaryAttribute(string id)
			: base(id)
		{
		}
	}
}
