using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000118 RID: 280
	public class JsonISerializableContract : JsonContainerContract
	{
		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000E2E RID: 3630 RVA: 0x000568B4 File Offset: 0x00054AB4
		// (set) Token: 0x06000E2F RID: 3631 RVA: 0x000568BC File Offset: 0x00054ABC
		public ObjectConstructor<object> ISerializableCreator { get; set; }

		// Token: 0x06000E30 RID: 3632 RVA: 0x000568C8 File Offset: 0x00054AC8
		public JsonISerializableContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Serializable;
		}
	}
}
