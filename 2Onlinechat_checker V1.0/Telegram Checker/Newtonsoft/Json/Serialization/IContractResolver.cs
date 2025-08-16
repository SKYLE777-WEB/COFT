using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000108 RID: 264
	public interface IContractResolver
	{
		// Token: 0x06000DB2 RID: 3506
		JsonContract ResolveContract(Type type);
	}
}
