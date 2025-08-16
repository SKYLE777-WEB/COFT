using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000AD RID: 173
	internal struct JsonPosition
	{
		// Token: 0x060007F6 RID: 2038 RVA: 0x00039B14 File Offset: 0x00037D14
		public JsonPosition(JsonContainerType type)
		{
			this.Type = type;
			this.HasIndex = JsonPosition.TypeHasIndex(type);
			this.Position = -1;
			this.PropertyName = null;
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x00039B38 File Offset: 0x00037D38
		internal int CalculateLength()
		{
			JsonContainerType type = this.Type;
			if (type == JsonContainerType.Object)
			{
				return this.PropertyName.Length + 5;
			}
			if (type - JsonContainerType.Array > 1)
			{
				throw new ArgumentOutOfRangeException("Type");
			}
			return MathUtils.IntLength((ulong)((long)this.Position)) + 2;
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x00039B8C File Offset: 0x00037D8C
		internal void WriteTo(StringBuilder sb)
		{
			JsonContainerType type = this.Type;
			if (type != JsonContainerType.Object)
			{
				if (type - JsonContainerType.Array > 1)
				{
					return;
				}
				sb.Append('[');
				sb.Append(this.Position);
				sb.Append(']');
				return;
			}
			else
			{
				string propertyName = this.PropertyName;
				if (propertyName.IndexOfAny(JsonPosition.SpecialCharacters) != -1)
				{
					sb.Append("['");
					sb.Append(propertyName);
					sb.Append("']");
					return;
				}
				if (sb.Length > 0)
				{
					sb.Append('.');
				}
				sb.Append(propertyName);
				return;
			}
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x00039C2C File Offset: 0x00037E2C
		internal static bool TypeHasIndex(JsonContainerType type)
		{
			return type == JsonContainerType.Array || type == JsonContainerType.Constructor;
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x00039C3C File Offset: 0x00037E3C
		internal static string BuildPath(List<JsonPosition> positions, JsonPosition? currentPosition)
		{
			int num = 0;
			if (positions != null)
			{
				for (int i = 0; i < positions.Count; i++)
				{
					num += positions[i].CalculateLength();
				}
			}
			if (currentPosition != null)
			{
				num += currentPosition.GetValueOrDefault().CalculateLength();
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			if (positions != null)
			{
				foreach (JsonPosition jsonPosition in positions)
				{
					jsonPosition.WriteTo(stringBuilder);
				}
			}
			if (currentPosition != null)
			{
				currentPosition.GetValueOrDefault().WriteTo(stringBuilder);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00039D10 File Offset: 0x00037F10
		internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
		{
			if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
			{
				message = message.Trim();
				if (!message.EndsWith('.'))
				{
					message += ".";
				}
				message += " ";
			}
			message += "Path '{0}'".FormatWith(CultureInfo.InvariantCulture, path);
			if (lineInfo != null && lineInfo.HasLineInfo())
			{
				message += ", line {0}, position {1}".FormatWith(CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition);
			}
			message += ".";
			return message;
		}

		// Token: 0x0400037A RID: 890
		private static readonly char[] SpecialCharacters = new char[] { '.', ' ', '[', ']', '(', ')' };

		// Token: 0x0400037B RID: 891
		internal JsonContainerType Type;

		// Token: 0x0400037C RID: 892
		internal int Position;

		// Token: 0x0400037D RID: 893
		internal string PropertyName;

		// Token: 0x0400037E RID: 894
		internal bool HasIndex;
	}
}
