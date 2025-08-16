using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x0200015F RID: 351
	internal class BooleanQueryExpression : QueryExpression
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x0600132D RID: 4909 RVA: 0x0006BBC0 File Offset: 0x00069DC0
		// (set) Token: 0x0600132E RID: 4910 RVA: 0x0006BBC8 File Offset: 0x00069DC8
		public object Left { get; set; }

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x0600132F RID: 4911 RVA: 0x0006BBD4 File Offset: 0x00069DD4
		// (set) Token: 0x06001330 RID: 4912 RVA: 0x0006BBDC File Offset: 0x00069DDC
		public object Right { get; set; }

		// Token: 0x06001331 RID: 4913 RVA: 0x0006BBE8 File Offset: 0x00069DE8
		private IEnumerable<JToken> GetResult(JToken root, JToken t, object o)
		{
			JToken jtoken = o as JToken;
			if (jtoken != null)
			{
				return new JToken[] { jtoken };
			}
			List<PathFilter> list = o as List<PathFilter>;
			if (list != null)
			{
				return JPath.Evaluate(list, root, t, false);
			}
			return CollectionUtils.ArrayEmpty<JToken>();
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0006BC30 File Offset: 0x00069E30
		public override bool IsMatch(JToken root, JToken t)
		{
			if (base.Operator == QueryOperator.Exists)
			{
				return this.GetResult(root, t, this.Left).Any<JToken>();
			}
			using (IEnumerator<JToken> enumerator = this.GetResult(root, t, this.Left).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					IEnumerable<JToken> result = this.GetResult(root, t, this.Right);
					ICollection<JToken> collection = (result as ICollection<JToken>) ?? result.ToList<JToken>();
					do
					{
						JToken jtoken = enumerator.Current;
						foreach (JToken jtoken2 in collection)
						{
							if (this.MatchTokens(jtoken, jtoken2))
							{
								return true;
							}
						}
					}
					while (enumerator.MoveNext());
				}
			}
			return false;
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x0006BD2C File Offset: 0x00069F2C
		private bool MatchTokens(JToken leftResult, JToken rightResult)
		{
			JValue jvalue = leftResult as JValue;
			JValue jvalue2 = rightResult as JValue;
			if (jvalue != null && jvalue2 != null)
			{
				switch (base.Operator)
				{
				case QueryOperator.Equals:
					if (this.EqualsWithStringCoercion(jvalue, jvalue2))
					{
						return true;
					}
					break;
				case QueryOperator.NotEquals:
					if (!this.EqualsWithStringCoercion(jvalue, jvalue2))
					{
						return true;
					}
					break;
				case QueryOperator.Exists:
					return true;
				case QueryOperator.LessThan:
					if (jvalue.CompareTo(jvalue2) < 0)
					{
						return true;
					}
					break;
				case QueryOperator.LessThanOrEquals:
					if (jvalue.CompareTo(jvalue2) <= 0)
					{
						return true;
					}
					break;
				case QueryOperator.GreaterThan:
					if (jvalue.CompareTo(jvalue2) > 0)
					{
						return true;
					}
					break;
				case QueryOperator.GreaterThanOrEquals:
					if (jvalue.CompareTo(jvalue2) >= 0)
					{
						return true;
					}
					break;
				}
			}
			else
			{
				QueryOperator @operator = base.Operator;
				if (@operator - QueryOperator.NotEquals <= 1)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x0006BDF8 File Offset: 0x00069FF8
		private bool EqualsWithStringCoercion(JValue value, JValue queryValue)
		{
			if (value.Equals(queryValue))
			{
				return true;
			}
			if (queryValue.Type != JTokenType.String)
			{
				return false;
			}
			string text = (string)queryValue.Value;
			string text2;
			switch (value.Type)
			{
			case JTokenType.Date:
			{
				using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
				{
					if (value.Value is DateTimeOffset)
					{
						DateTimeUtils.WriteDateTimeOffsetString(stringWriter, (DateTimeOffset)value.Value, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
					}
					else
					{
						DateTimeUtils.WriteDateTimeString(stringWriter, (DateTime)value.Value, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
					}
					text2 = stringWriter.ToString();
					goto IL_00FA;
				}
				break;
			}
			case JTokenType.Raw:
				return false;
			case JTokenType.Bytes:
				break;
			case JTokenType.Guid:
			case JTokenType.TimeSpan:
				text2 = value.Value.ToString();
				goto IL_00FA;
			case JTokenType.Uri:
				text2 = ((Uri)value.Value).OriginalString;
				goto IL_00FA;
			default:
				return false;
			}
			text2 = Convert.ToBase64String((byte[])value.Value);
			IL_00FA:
			return string.Equals(text2, text, StringComparison.Ordinal);
		}
	}
}
