using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x0200015B RID: 347
	internal abstract class PathFilter
	{
		// Token: 0x06001321 RID: 4897
		public abstract IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch);

		// Token: 0x06001322 RID: 4898 RVA: 0x0006B95C File Offset: 0x00069B5C
		protected static JToken GetTokenIndex(JToken t, bool errorWhenNoMatch, int index)
		{
			JArray jarray = t as JArray;
			JConstructor jconstructor = t as JConstructor;
			if (jarray != null)
			{
				if (jarray.Count > index)
				{
					return jarray[index];
				}
				if (errorWhenNoMatch)
				{
					throw new JsonException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, index));
				}
				return null;
			}
			else if (jconstructor != null)
			{
				if (jconstructor.Count > index)
				{
					return jconstructor[index];
				}
				if (errorWhenNoMatch)
				{
					throw new JsonException("Index {0} outside the bounds of JConstructor.".FormatWith(CultureInfo.InvariantCulture, index));
				}
				return null;
			}
			else
			{
				if (errorWhenNoMatch)
				{
					throw new JsonException("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, index, t.GetType().Name));
				}
				return null;
			}
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x0006BA28 File Offset: 0x00069C28
		protected static JToken GetNextScanValue(JToken originalParent, JToken container, JToken value)
		{
			if (container != null && container.HasValues)
			{
				value = container.First;
			}
			else
			{
				while (value != null && value != originalParent && value == value.Parent.Last)
				{
					value = value.Parent;
				}
				if (value == null || value == originalParent)
				{
					return null;
				}
				value = value.Next;
			}
			return value;
		}
	}
}
