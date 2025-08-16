using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000164 RID: 356
	internal class ScanMultipleFilter : PathFilter
	{
		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x0006BFEC File Offset: 0x0006A1EC
		// (set) Token: 0x06001346 RID: 4934 RVA: 0x0006BFF4 File Offset: 0x0006A1F4
		public List<string> Names { get; set; }

		// Token: 0x06001347 RID: 4935 RVA: 0x0006C000 File Offset: 0x0006A200
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken c in current)
			{
				JToken value = c;
				JContainer jcontainer = c as JContainer;
				for (;;)
				{
					value = PathFilter.GetNextScanValue(c, jcontainer, value);
					if (value == null)
					{
						break;
					}
					JProperty e = value as JProperty;
					if (e != null)
					{
						foreach (string text in this.Names)
						{
							if (e.Name == text)
							{
								yield return e.Value;
							}
						}
						List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
					}
					jcontainer = value as JContainer;
					e = null;
				}
				value = null;
				c = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
