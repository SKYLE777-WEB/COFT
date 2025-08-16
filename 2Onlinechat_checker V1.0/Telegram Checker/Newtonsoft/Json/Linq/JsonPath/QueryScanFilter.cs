using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000161 RID: 353
	internal class QueryScanFilter : PathFilter
	{
		// Token: 0x17000361 RID: 865
		// (get) Token: 0x0600133A RID: 4922 RVA: 0x0006BF5C File Offset: 0x0006A15C
		// (set) Token: 0x0600133B RID: 4923 RVA: 0x0006BF64 File Offset: 0x0006A164
		public QueryExpression Expression { get; set; }

		// Token: 0x0600133C RID: 4924 RVA: 0x0006BF70 File Offset: 0x0006A170
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				JContainer jcontainer;
				if ((jcontainer = t as JContainer) != null)
				{
					foreach (JToken jtoken in jcontainer.DescendantsAndSelf())
					{
						if (this.Expression.IsMatch(root, jtoken))
						{
							yield return jtoken;
						}
					}
					IEnumerator<JToken> enumerator2 = null;
				}
				else if (this.Expression.IsMatch(root, t))
				{
					yield return t;
				}
				t = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
