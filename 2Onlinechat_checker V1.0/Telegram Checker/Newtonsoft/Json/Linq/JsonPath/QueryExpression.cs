using System;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x0200015D RID: 349
	internal abstract class QueryExpression
	{
		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06001325 RID: 4901 RVA: 0x0006BA9C File Offset: 0x00069C9C
		// (set) Token: 0x06001326 RID: 4902 RVA: 0x0006BAA4 File Offset: 0x00069CA4
		public QueryOperator Operator { get; set; }

		// Token: 0x06001327 RID: 4903
		public abstract bool IsMatch(JToken root, JToken t);
	}
}
