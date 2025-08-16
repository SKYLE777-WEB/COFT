using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000155 RID: 341
	internal class ArrayIndexFilter : PathFilter
	{
		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060012F3 RID: 4851 RVA: 0x0006A5C0 File Offset: 0x000687C0
		// (set) Token: 0x060012F4 RID: 4852 RVA: 0x0006A5C8 File Offset: 0x000687C8
		public int? Index { get; set; }

		// Token: 0x060012F5 RID: 4853 RVA: 0x0006A5D4 File Offset: 0x000687D4
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				if (this.Index != null)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(t, errorWhenNoMatch, this.Index.GetValueOrDefault());
					if (tokenIndex != null)
					{
						yield return tokenIndex;
					}
				}
				else if (t is JArray || t is JConstructor)
				{
					foreach (JToken jtoken in ((IEnumerable<JToken>)t))
					{
						yield return jtoken;
					}
					IEnumerator<JToken> enumerator2 = null;
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
				}
				t = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
