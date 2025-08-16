using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x0200014E RID: 334
	public class JTokenEqualityComparer : IEqualityComparer<JToken>
	{
		// Token: 0x06001279 RID: 4729 RVA: 0x00068564 File Offset: 0x00066764
		public bool Equals(JToken x, JToken y)
		{
			return JToken.DeepEquals(x, y);
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x00068570 File Offset: 0x00066770
		public int GetHashCode(JToken obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetDeepHashCode();
		}
	}
}
