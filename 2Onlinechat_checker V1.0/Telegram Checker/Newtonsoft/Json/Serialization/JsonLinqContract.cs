using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000119 RID: 281
	public class JsonLinqContract : JsonContract
	{
		// Token: 0x06000E31 RID: 3633 RVA: 0x000568D8 File Offset: 0x00054AD8
		public JsonLinqContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Linq;
		}
	}
}
