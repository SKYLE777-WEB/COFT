using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000158 RID: 344
	internal class FieldFilter : PathFilter
	{
		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06001304 RID: 4868 RVA: 0x0006A6B0 File Offset: 0x000688B0
		// (set) Token: 0x06001305 RID: 4869 RVA: 0x0006A6B8 File Offset: 0x000688B8
		public string Name { get; set; }

		// Token: 0x06001306 RID: 4870 RVA: 0x0006A6C4 File Offset: 0x000688C4
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				JObject o = t as JObject;
				if (o != null)
				{
					if (this.Name != null)
					{
						JToken jtoken = o[this.Name];
						if (jtoken != null)
						{
							yield return jtoken;
						}
						else if (errorWhenNoMatch)
						{
							throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, this.Name));
						}
					}
					else
					{
						foreach (KeyValuePair<string, JToken> keyValuePair in o)
						{
							yield return keyValuePair.Value;
						}
						IEnumerator<KeyValuePair<string, JToken>> enumerator2 = null;
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, this.Name ?? "*", t.GetType().Name));
				}
				o = null;
				t = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}
	}
}
