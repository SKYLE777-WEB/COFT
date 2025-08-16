using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x02000159 RID: 345
	internal class FieldMultipleFilter : PathFilter
	{
		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06001308 RID: 4872 RVA: 0x0006A6EC File Offset: 0x000688EC
		// (set) Token: 0x06001309 RID: 4873 RVA: 0x0006A6F4 File Offset: 0x000688F4
		public List<string> Names { get; set; }

		// Token: 0x0600130A RID: 4874 RVA: 0x0006A700 File Offset: 0x00068900
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				JObject o = t as JObject;
				if (o != null)
				{
					foreach (string name in this.Names)
					{
						JToken jtoken = o[name];
						if (jtoken != null)
						{
							yield return jtoken;
						}
						if (errorWhenNoMatch)
						{
							throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, name));
						}
						name = null;
					}
					List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", this.Names.Select((string n) => "'" + n + "'")), t.GetType().Name));
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
