using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000163 RID: 355
	internal class ScanFilter : PathFilter
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06001341 RID: 4929 RVA: 0x0006BFB8 File Offset: 0x0006A1B8
		// (set) Token: 0x06001342 RID: 4930 RVA: 0x0006BFC0 File Offset: 0x0006A1C0
		public string Name { get; set; }

		// Token: 0x06001343 RID: 4931 RVA: 0x0006BFCC File Offset: 0x0006A1CC
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken c in current)
			{
				if (this.Name == null)
				{
					yield return c;
				}
				JToken value = c;
				JContainer jcontainer = c as JContainer;
				for (;;)
				{
					value = PathFilter.GetNextScanValue(c, jcontainer, value);
					if (value == null)
					{
						break;
					}
					JProperty jproperty = value as JProperty;
					if (jproperty != null)
					{
						if (jproperty.Name == this.Name)
						{
							yield return jproperty.Value;
						}
					}
					else if (this.Name == null)
					{
						yield return value;
					}
					jcontainer = value as JContainer;
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
