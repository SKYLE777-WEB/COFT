using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x0200015E RID: 350
	internal class CompositeExpression : QueryExpression
	{
		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06001329 RID: 4905 RVA: 0x0006BAB8 File Offset: 0x00069CB8
		// (set) Token: 0x0600132A RID: 4906 RVA: 0x0006BAC0 File Offset: 0x00069CC0
		public List<QueryExpression> Expressions { get; set; }

		// Token: 0x0600132B RID: 4907 RVA: 0x0006BACC File Offset: 0x00069CCC
		public CompositeExpression()
		{
			this.Expressions = new List<QueryExpression>();
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0006BAE0 File Offset: 0x00069CE0
		public override bool IsMatch(JToken root, JToken t)
		{
			QueryOperator @operator = base.Operator;
			if (@operator == QueryOperator.And)
			{
				using (List<QueryExpression>.Enumerator enumerator = this.Expressions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.IsMatch(root, t))
						{
							return false;
						}
					}
				}
				return true;
			}
			if (@operator != QueryOperator.Or)
			{
				throw new ArgumentOutOfRangeException();
			}
			using (List<QueryExpression>.Enumerator enumerator = this.Expressions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsMatch(root, t))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
