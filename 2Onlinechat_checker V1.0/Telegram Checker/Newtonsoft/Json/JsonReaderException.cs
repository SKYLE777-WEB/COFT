using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	// Token: 0x020000B0 RID: 176
	[Serializable]
	public class JsonReaderException : JsonException
	{
		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000872 RID: 2162 RVA: 0x0003B93C File Offset: 0x00039B3C
		public int LineNumber { get; }

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x0003B944 File Offset: 0x00039B44
		public int LinePosition { get; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000874 RID: 2164 RVA: 0x0003B94C File Offset: 0x00039B4C
		public string Path { get; }

		// Token: 0x06000875 RID: 2165 RVA: 0x0003B954 File Offset: 0x00039B54
		public JsonReaderException()
		{
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0003B95C File Offset: 0x00039B5C
		public JsonReaderException(string message)
			: base(message)
		{
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0003B968 File Offset: 0x00039B68
		public JsonReaderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x0003B974 File Offset: 0x00039B74
		public JsonReaderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x0003B980 File Offset: 0x00039B80
		public JsonReaderException(string message, string path, int lineNumber, int linePosition, Exception innerException)
			: base(message, innerException)
		{
			this.Path = path;
			this.LineNumber = lineNumber;
			this.LinePosition = linePosition;
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x0003B9A4 File Offset: 0x00039BA4
		internal static JsonReaderException Create(JsonReader reader, string message)
		{
			return JsonReaderException.Create(reader, message, null);
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x0003B9B0 File Offset: 0x00039BB0
		internal static JsonReaderException Create(JsonReader reader, string message, Exception ex)
		{
			return JsonReaderException.Create(reader as IJsonLineInfo, reader.Path, message, ex);
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x0003B9C8 File Offset: 0x00039BC8
		internal static JsonReaderException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
		{
			message = JsonPosition.FormatMessage(lineInfo, path, message);
			int num;
			int num2;
			if (lineInfo != null && lineInfo.HasLineInfo())
			{
				num = lineInfo.LineNumber;
				num2 = lineInfo.LinePosition;
			}
			else
			{
				num = 0;
				num2 = 0;
			}
			return new JsonReaderException(message, path, num, num2, ex);
		}
	}
}
