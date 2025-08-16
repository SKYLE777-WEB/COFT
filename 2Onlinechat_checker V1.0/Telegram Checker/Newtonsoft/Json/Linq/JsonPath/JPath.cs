using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	// Token: 0x0200015A RID: 346
	internal class JPath
	{
		// Token: 0x1700035B RID: 859
		// (get) Token: 0x0600130C RID: 4876 RVA: 0x0006A728 File Offset: 0x00068928
		public List<PathFilter> Filters { get; }

		// Token: 0x0600130D RID: 4877 RVA: 0x0006A730 File Offset: 0x00068930
		public JPath(string expression)
		{
			ValidationUtils.ArgumentNotNull(expression, "expression");
			this._expression = expression;
			this.Filters = new List<PathFilter>();
			this.ParseMain();
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0006A75C File Offset: 0x0006895C
		private void ParseMain()
		{
			int num = this._currentIndex;
			this.EatWhitespace();
			if (this._expression.Length == this._currentIndex)
			{
				return;
			}
			if (this._expression[this._currentIndex] == '$')
			{
				if (this._expression.Length == 1)
				{
					return;
				}
				char c = this._expression[this._currentIndex + 1];
				if (c == '.' || c == '[')
				{
					this._currentIndex++;
					num = this._currentIndex;
				}
			}
			if (!this.ParsePath(this.Filters, num, false))
			{
				int currentIndex = this._currentIndex;
				this.EatWhitespace();
				if (this._currentIndex < this._expression.Length)
				{
					throw new JsonException("Unexpected character while parsing path: " + this._expression[currentIndex].ToString());
				}
			}
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0006A850 File Offset: 0x00068A50
		private bool ParsePath(List<PathFilter> filters, int currentPartStartIndex, bool query)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			while (this._currentIndex < this._expression.Length && !flag4)
			{
				char c = this._expression[this._currentIndex];
				if (c <= ')')
				{
					if (c != ' ')
					{
						if (c != '(')
						{
							if (c != ')')
							{
								goto IL_01A8;
							}
							goto IL_00DD;
						}
					}
					else
					{
						if (this._currentIndex < this._expression.Length)
						{
							flag4 = true;
							continue;
						}
						continue;
					}
				}
				else
				{
					if (c == '.')
					{
						if (this._currentIndex > currentPartStartIndex)
						{
							string text = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
							if (text == "*")
							{
								text = null;
							}
							filters.Add(JPath.CreatePathFilter(text, flag));
							flag = false;
						}
						if (this._currentIndex + 1 < this._expression.Length && this._expression[this._currentIndex + 1] == '.')
						{
							flag = true;
							this._currentIndex++;
						}
						this._currentIndex++;
						currentPartStartIndex = this._currentIndex;
						flag2 = false;
						flag3 = true;
						continue;
					}
					if (c != '[')
					{
						if (c != ']')
						{
							goto IL_01A8;
						}
						goto IL_00DD;
					}
				}
				if (this._currentIndex > currentPartStartIndex)
				{
					string text2 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
					if (text2 == "*")
					{
						text2 = null;
					}
					filters.Add(JPath.CreatePathFilter(text2, flag));
					flag = false;
				}
				filters.Add(this.ParseIndexer(c, flag));
				this._currentIndex++;
				currentPartStartIndex = this._currentIndex;
				flag2 = true;
				flag3 = false;
				continue;
				IL_00DD:
				flag4 = true;
				continue;
				IL_01A8:
				if (query && (c == '=' || c == '<' || c == '!' || c == '>' || c == '|' || c == '&'))
				{
					flag4 = true;
				}
				else
				{
					if (flag2)
					{
						throw new JsonException("Unexpected character following indexer: " + c.ToString());
					}
					this._currentIndex++;
				}
			}
			bool flag5 = this._currentIndex == this._expression.Length;
			if (this._currentIndex > currentPartStartIndex)
			{
				string text3 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex).TrimEnd(new char[0]);
				if (text3 == "*")
				{
					text3 = null;
				}
				filters.Add(JPath.CreatePathFilter(text3, flag));
			}
			else if (flag3 && (flag5 || query))
			{
				throw new JsonException("Unexpected end while parsing path.");
			}
			return flag5;
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x0006AB18 File Offset: 0x00068D18
		private static PathFilter CreatePathFilter(string member, bool scan)
		{
			if (!scan)
			{
				return new FieldFilter
				{
					Name = member
				};
			}
			return new ScanFilter
			{
				Name = member
			};
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0006AB3C File Offset: 0x00068D3C
		private PathFilter ParseIndexer(char indexerOpenChar, bool scan)
		{
			this._currentIndex++;
			char c = ((indexerOpenChar == '[') ? ']' : ')');
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] == '\'')
			{
				return this.ParseQuotedField(c, scan);
			}
			if (this._expression[this._currentIndex] == '?')
			{
				return this.ParseQuery(c, scan);
			}
			return this.ParseArrayIndexer(c);
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0006ABC8 File Offset: 0x00068DC8
		private PathFilter ParseArrayIndexer(char indexerCloseChar)
		{
			int num = this._currentIndex;
			int? num2 = null;
			List<int> list = null;
			int num3 = 0;
			int? num4 = null;
			int? num5 = null;
			int? num6 = null;
			while (this._currentIndex < this._expression.Length)
			{
				char c = this._expression[this._currentIndex];
				if (c == ' ')
				{
					num2 = new int?(this._currentIndex);
					this.EatWhitespace();
				}
				else if (c == indexerCloseChar)
				{
					int num7 = (num2 ?? this._currentIndex) - num;
					if (list != null)
					{
						if (num7 == 0)
						{
							throw new JsonException("Array index expected.");
						}
						int num8 = Convert.ToInt32(this._expression.Substring(num, num7), CultureInfo.InvariantCulture);
						list.Add(num8);
						return new ArrayMultipleIndexFilter
						{
							Indexes = list
						};
					}
					else
					{
						if (num3 > 0)
						{
							if (num7 > 0)
							{
								int num9 = Convert.ToInt32(this._expression.Substring(num, num7), CultureInfo.InvariantCulture);
								if (num3 == 1)
								{
									num5 = new int?(num9);
								}
								else
								{
									num6 = new int?(num9);
								}
							}
							return new ArraySliceFilter
							{
								Start = num4,
								End = num5,
								Step = num6
							};
						}
						if (num7 == 0)
						{
							throw new JsonException("Array index expected.");
						}
						int num10 = Convert.ToInt32(this._expression.Substring(num, num7), CultureInfo.InvariantCulture);
						return new ArrayIndexFilter
						{
							Index = new int?(num10)
						};
					}
				}
				else if (c == ',')
				{
					int num11 = (num2 ?? this._currentIndex) - num;
					if (num11 == 0)
					{
						throw new JsonException("Array index expected.");
					}
					if (list == null)
					{
						list = new List<int>();
					}
					string text = this._expression.Substring(num, num11);
					list.Add(Convert.ToInt32(text, CultureInfo.InvariantCulture));
					this._currentIndex++;
					this.EatWhitespace();
					num = this._currentIndex;
					num2 = null;
				}
				else if (c == '*')
				{
					this._currentIndex++;
					this.EnsureLength("Path ended with open indexer.");
					this.EatWhitespace();
					if (this._expression[this._currentIndex] != indexerCloseChar)
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
					}
					return new ArrayIndexFilter();
				}
				else if (c == ':')
				{
					int num12 = (num2 ?? this._currentIndex) - num;
					if (num12 > 0)
					{
						int num13 = Convert.ToInt32(this._expression.Substring(num, num12), CultureInfo.InvariantCulture);
						if (num3 == 0)
						{
							num4 = new int?(num13);
						}
						else if (num3 == 1)
						{
							num5 = new int?(num13);
						}
						else
						{
							num6 = new int?(num13);
						}
					}
					num3++;
					this._currentIndex++;
					this.EatWhitespace();
					num = this._currentIndex;
					num2 = null;
				}
				else
				{
					if (!char.IsDigit(c) && c != '-')
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
					}
					if (num2 != null)
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
					}
					this._currentIndex++;
				}
			}
			throw new JsonException("Path ended with open indexer.");
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0006AF74 File Offset: 0x00069174
		private void EatWhitespace()
		{
			while (this._currentIndex < this._expression.Length && this._expression[this._currentIndex] == ' ')
			{
				this._currentIndex++;
			}
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0006AFB4 File Offset: 0x000691B4
		private PathFilter ParseQuery(char indexerCloseChar, bool scan)
		{
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			if (this._expression[this._currentIndex] != '(')
			{
				throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex].ToString());
			}
			this._currentIndex++;
			QueryExpression queryExpression = this.ParseExpression(scan);
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] != indexerCloseChar)
			{
				throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex].ToString());
			}
			if (!scan)
			{
				return new QueryFilter
				{
					Expression = queryExpression
				};
			}
			return new QueryScanFilter
			{
				Expression = queryExpression
			};
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x0006B0B4 File Offset: 0x000692B4
		private bool TryParseExpression(bool scan, out List<PathFilter> expressionPath)
		{
			if (this._expression[this._currentIndex] == '$')
			{
				expressionPath = new List<PathFilter>();
				expressionPath.Add(RootFilter.Instance);
			}
			else
			{
				if (this._expression[this._currentIndex] != '@')
				{
					expressionPath = null;
					return false;
				}
				expressionPath = new List<PathFilter>();
			}
			this._currentIndex++;
			if (this.ParsePath(expressionPath, this._currentIndex, true))
			{
				throw new JsonException("Path ended with open query.");
			}
			return true;
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x0006B14C File Offset: 0x0006934C
		private JsonException CreateUnexpectedCharacterException()
		{
			return new JsonException("Unexpected character while parsing path query: " + this._expression[this._currentIndex].ToString());
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x0006B188 File Offset: 0x00069388
		private object ParseSide(bool scan)
		{
			this.EatWhitespace();
			List<PathFilter> list;
			if (this.TryParseExpression(scan, out list))
			{
				this.EatWhitespace();
				this.EnsureLength("Path ended with open query.");
				return list;
			}
			object obj;
			if (this.TryParseValue(out obj))
			{
				this.EatWhitespace();
				this.EnsureLength("Path ended with open query.");
				return new JValue(obj);
			}
			throw this.CreateUnexpectedCharacterException();
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x0006B1EC File Offset: 0x000693EC
		private QueryExpression ParseExpression(bool scan)
		{
			QueryExpression queryExpression = null;
			CompositeExpression compositeExpression = null;
			while (this._currentIndex < this._expression.Length)
			{
				object obj = this.ParseSide(scan);
				object obj2 = null;
				QueryOperator queryOperator;
				if (this._expression[this._currentIndex] == ')' || this._expression[this._currentIndex] == '|' || this._expression[this._currentIndex] == '&')
				{
					queryOperator = QueryOperator.Exists;
				}
				else
				{
					queryOperator = this.ParseOperator();
					obj2 = this.ParseSide(scan);
				}
				BooleanQueryExpression booleanQueryExpression = new BooleanQueryExpression
				{
					Left = obj,
					Operator = queryOperator,
					Right = obj2
				};
				if (this._expression[this._currentIndex] == ')')
				{
					if (compositeExpression != null)
					{
						compositeExpression.Expressions.Add(booleanQueryExpression);
						return queryExpression;
					}
					return booleanQueryExpression;
				}
				else
				{
					if (this._expression[this._currentIndex] == '&')
					{
						if (!this.Match("&&"))
						{
							throw this.CreateUnexpectedCharacterException();
						}
						if (compositeExpression == null || compositeExpression.Operator != QueryOperator.And)
						{
							CompositeExpression compositeExpression2 = new CompositeExpression
							{
								Operator = QueryOperator.And
							};
							if (compositeExpression != null)
							{
								compositeExpression.Expressions.Add(compositeExpression2);
							}
							compositeExpression = compositeExpression2;
							if (queryExpression == null)
							{
								queryExpression = compositeExpression;
							}
						}
						compositeExpression.Expressions.Add(booleanQueryExpression);
					}
					if (this._expression[this._currentIndex] == '|')
					{
						if (!this.Match("||"))
						{
							throw this.CreateUnexpectedCharacterException();
						}
						if (compositeExpression == null || compositeExpression.Operator != QueryOperator.Or)
						{
							CompositeExpression compositeExpression3 = new CompositeExpression
							{
								Operator = QueryOperator.Or
							};
							if (compositeExpression != null)
							{
								compositeExpression.Expressions.Add(compositeExpression3);
							}
							compositeExpression = compositeExpression3;
							if (queryExpression == null)
							{
								queryExpression = compositeExpression;
							}
						}
						compositeExpression.Expressions.Add(booleanQueryExpression);
					}
				}
			}
			throw new JsonException("Path ended with open query.");
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x0006B3D4 File Offset: 0x000695D4
		private bool TryParseValue(out object value)
		{
			char c = this._expression[this._currentIndex];
			if (c == '\'')
			{
				value = this.ReadQuotedString();
				return true;
			}
			if (char.IsDigit(c) || c == '-')
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(c);
				this._currentIndex++;
				while (this._currentIndex < this._expression.Length)
				{
					c = this._expression[this._currentIndex];
					if (c == ' ' || c == ')')
					{
						string text = stringBuilder.ToString();
						if (text.IndexOfAny(new char[] { '.', 'E', 'e' }) != -1)
						{
							double num;
							bool flag = double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num);
							value = num;
							return flag;
						}
						long num2;
						bool flag2 = long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2);
						value = num2;
						return flag2;
					}
					else
					{
						stringBuilder.Append(c);
						this._currentIndex++;
					}
				}
			}
			else if (c == 't')
			{
				if (this.Match("true"))
				{
					value = true;
					return true;
				}
			}
			else if (c == 'f')
			{
				if (this.Match("false"))
				{
					value = false;
					return true;
				}
			}
			else if (c == 'n' && this.Match("null"))
			{
				value = null;
				return true;
			}
			value = null;
			return false;
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x0006B54C File Offset: 0x0006974C
		private string ReadQuotedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this._currentIndex++;
			while (this._currentIndex < this._expression.Length)
			{
				char c = this._expression[this._currentIndex];
				if (c == '\\' && this._currentIndex + 1 < this._expression.Length)
				{
					this._currentIndex++;
					if (this._expression[this._currentIndex] == '\'')
					{
						stringBuilder.Append('\'');
					}
					else
					{
						if (this._expression[this._currentIndex] != '\\')
						{
							throw new JsonException("Unknown escape character: \\" + this._expression[this._currentIndex].ToString());
						}
						stringBuilder.Append('\\');
					}
					this._currentIndex++;
				}
				else
				{
					if (c == '\'')
					{
						this._currentIndex++;
						return stringBuilder.ToString();
					}
					this._currentIndex++;
					stringBuilder.Append(c);
				}
			}
			throw new JsonException("Path ended with an open string.");
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x0006B694 File Offset: 0x00069894
		private bool Match(string s)
		{
			int num = this._currentIndex;
			foreach (char c in s)
			{
				if (num >= this._expression.Length || this._expression[num] != c)
				{
					return false;
				}
				num++;
			}
			this._currentIndex = num;
			return true;
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x0006B700 File Offset: 0x00069900
		private QueryOperator ParseOperator()
		{
			if (this._currentIndex + 1 >= this._expression.Length)
			{
				throw new JsonException("Path ended with open query.");
			}
			if (this.Match("=="))
			{
				return QueryOperator.Equals;
			}
			if (this.Match("!=") || this.Match("<>"))
			{
				return QueryOperator.NotEquals;
			}
			if (this.Match("<="))
			{
				return QueryOperator.LessThanOrEquals;
			}
			if (this.Match("<"))
			{
				return QueryOperator.LessThan;
			}
			if (this.Match(">="))
			{
				return QueryOperator.GreaterThanOrEquals;
			}
			if (this.Match(">"))
			{
				return QueryOperator.GreaterThan;
			}
			throw new JsonException("Could not read query operator.");
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x0006B7BC File Offset: 0x000699BC
		private PathFilter ParseQuotedField(char indexerCloseChar, bool scan)
		{
			List<string> list = null;
			while (this._currentIndex < this._expression.Length)
			{
				string text = this.ReadQuotedString();
				this.EatWhitespace();
				this.EnsureLength("Path ended with open indexer.");
				if (this._expression[this._currentIndex] == indexerCloseChar)
				{
					if (list == null)
					{
						return JPath.CreatePathFilter(text, scan);
					}
					list.Add(text);
					if (!scan)
					{
						return new FieldMultipleFilter
						{
							Names = list
						};
					}
					return new ScanMultipleFilter
					{
						Names = list
					};
				}
				else
				{
					if (this._expression[this._currentIndex] != ',')
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex].ToString());
					}
					this._currentIndex++;
					this.EatWhitespace();
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text);
				}
			}
			throw new JsonException("Path ended with open indexer.");
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0006B8C8 File Offset: 0x00069AC8
		private void EnsureLength(string message)
		{
			if (this._currentIndex >= this._expression.Length)
			{
				throw new JsonException(message);
			}
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x0006B8E8 File Offset: 0x00069AE8
		internal IEnumerable<JToken> Evaluate(JToken root, JToken t, bool errorWhenNoMatch)
		{
			return JPath.Evaluate(this.Filters, root, t, errorWhenNoMatch);
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x0006B8F8 File Offset: 0x00069AF8
		internal static IEnumerable<JToken> Evaluate(List<PathFilter> filters, JToken root, JToken t, bool errorWhenNoMatch)
		{
			IEnumerable<JToken> enumerable = new JToken[] { t };
			foreach (PathFilter pathFilter in filters)
			{
				enumerable = pathFilter.ExecuteFilter(root, enumerable, errorWhenNoMatch);
			}
			return enumerable;
		}

		// Token: 0x040006D7 RID: 1751
		private readonly string _expression;

		// Token: 0x040006D9 RID: 1753
		private int _currentIndex;
	}
}
