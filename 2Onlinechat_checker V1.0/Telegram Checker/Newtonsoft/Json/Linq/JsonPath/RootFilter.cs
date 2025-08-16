using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000162 RID: 354
	internal class RootFilter : PathFilter
	{
		// Token: 0x0600133E RID: 4926 RVA: 0x0006BF98 File Offset: 0x0006A198
		private RootFilter()
		{
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x0006BFA0 File Offset: 0x0006A1A0
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			return new JToken[] { root };
		}

		// Token: 0x040006EB RID: 1771
		public static readonly RootFilter Instance = new RootFilter();
	}
}
