using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000156 RID: 342
	internal class ArrayMultipleIndexFilter : PathFilter
	{
		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060012F7 RID: 4855 RVA: 0x0006A5FC File Offset: 0x000687FC
		// (set) Token: 0x060012F8 RID: 4856 RVA: 0x0006A604 File Offset: 0x00068804
		public List<int> Indexes { get; set; }

		// Token: 0x060012F9 RID: 4857 RVA: 0x0006A610 File Offset: 0x00068810
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				foreach (int num in this.Indexes)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(t, errorWhenNoMatch, num);
					if (tokenIndex != null)
					{
						yield return tokenIndex;
					}
				}
				List<int>.Enumerator enumerator2 = default(List<int>.Enumerator);
				t = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
