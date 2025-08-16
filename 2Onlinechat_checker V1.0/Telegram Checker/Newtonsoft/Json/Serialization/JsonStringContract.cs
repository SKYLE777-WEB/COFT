using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000122 RID: 290
	public class JsonStringContract : JsonPrimitiveContract
	{
		// Token: 0x06000F2F RID: 3887 RVA: 0x0005D588 File Offset: 0x0005B788
		public JsonStringContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.String;
		}
	}
}
