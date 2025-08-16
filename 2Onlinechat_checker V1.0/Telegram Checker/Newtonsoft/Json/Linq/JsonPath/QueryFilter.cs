using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000160 RID: 352
	internal class QueryFilter : PathFilter
	{
		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06001336 RID: 4918 RVA: 0x0006BF20 File Offset: 0x0006A120
		// (set) Token: 0x06001337 RID: 4919 RVA: 0x0006BF28 File Offset: 0x0006A128
		public QueryExpression Expression { get; set; }

		// Token: 0x06001338 RID: 4920 RVA: 0x0006BF34 File Offset: 0x0006A134
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken jtoken in current)
			{
				foreach (JToken jtoken2 in ((IEnumerable<JToken>)jtoken))
				{
					if (this.Expression.IsMatch(root, jtoken2))
					{
						yield return jtoken2;
					}
				}
				IEnumerator<JToken> enumerator2 = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
