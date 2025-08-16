using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000107 RID: 263
	public interface IAttributeProvider
	{
		// Token: 0x06000DB0 RID: 3504
		IList<Attribute> GetAttributes(bool inherit);

		// Token: 0x06000DB1 RID: 3505
		IList<Attribute> GetAttributes(Type attributeType, bool inherit);
	}
}
