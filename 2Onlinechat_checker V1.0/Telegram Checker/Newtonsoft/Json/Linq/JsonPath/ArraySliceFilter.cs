using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000157 RID: 343
	internal class ArraySliceFilter : PathFilter
	{
		// Token: 0x17000356 RID: 854
		// (get) Token: 0x060012FB RID: 4859 RVA: 0x0006A638 File Offset: 0x00068838
		// (set) Token: 0x060012FC RID: 4860 RVA: 0x0006A640 File Offset: 0x00068840
		public int? Start { get; set; }

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060012FD RID: 4861 RVA: 0x0006A64C File Offset: 0x0006884C
		// (set) Token: 0x060012FE RID: 4862 RVA: 0x0006A654 File Offset: 0x00068854
		public int? End { get; set; }

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x060012FF RID: 4863 RVA: 0x0006A660 File Offset: 0x00068860
		// (set) Token: 0x06001300 RID: 4864 RVA: 0x0006A668 File Offset: 0x00068868
		public int? Step { get; set; }

		// Token: 0x06001301 RID: 4865 RVA: 0x0006A674 File Offset: 0x00068874
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			if (this.Step == 0)
			{
				throw new JsonException("Step cannot be zero.");
			}
			foreach (JToken t in current)
			{
				JArray a = t as JArray;
				if (a != null)
				{
					int stepCount = this.Step ?? 1;
					int num = this.Start ?? ((stepCount > 0) ? 0 : (a.Count - 1));
					int stopIndex = this.End ?? ((stepCount > 0) ? a.Count : (-1));
					if (this.Start < 0)
					{
						num = a.Count + num;
					}
					if (this.End < 0)
					{
						stopIndex = a.Count + stopIndex;
					}
					num = Math.Max(num, (stepCount > 0) ? 0 : int.MinValue);
					num = Math.Min(num, (stepCount > 0) ? a.Count : (a.Count - 1));
					stopIndex = Math.Max(stopIndex, -1);
					stopIndex = Math.Min(stopIndex, a.Count);
					bool positiveStep = stepCount > 0;
					if (this.IsValid(num, stopIndex, positiveStep))
					{
						int i = num;
						while (this.IsValid(i, stopIndex, positiveStep))
						{
							yield return a[i];
							i += stepCount;
						}
					}
					else if (errorWhenNoMatch)
					{
						throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith(CultureInfo.InvariantCulture, (this.Start != null) ? this.Start.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*", (this.End != null) ? this.End.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) : "*"));
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Array slice is not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
				}
				a = null;
				t = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0006A694 File Offset: 0x00068894
		private bool IsValid(int index, int stopIndex, bool positiveStep)
		{
			if (positiveStep)
			{
				return index < stopIndex;
			}
			return index > stopIndex;
		}
	}
}
