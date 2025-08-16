using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x020000FF RID: 255
	public class DefaultNamingStrategy : NamingStrategy
	{
		// Token: 0x06000D8A RID: 3466 RVA: 0x00054E0C File Offset: 0x0005300C
		protected override string ResolvePropertyName(string name)
		{
			return name;
		}
	}
}
