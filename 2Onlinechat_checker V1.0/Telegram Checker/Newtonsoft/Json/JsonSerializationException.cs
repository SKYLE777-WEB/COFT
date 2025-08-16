using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	// Token: 0x020000B2 RID: 178
	[Serializable]
	public class JsonSerializationException : JsonException
	{
		// Token: 0x0600087E RID: 2174 RVA: 0x0003BA20 File Offset: 0x00039C20
		public JsonSerializationException()
		{
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x0003BA28 File Offset: 0x00039C28
		public JsonSerializationException(string message)
			: base(message)
		{
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x0003BA34 File Offset: 0x00039C34
		public JsonSerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x0003BA40 File Offset: 0x00039C40
		public JsonSerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x0003BA4C File Offset: 0x00039C4C
		internal static JsonSerializationException Create(JsonReader reader, string message)
		{
			return JsonSerializationException.Create(reader, message, null);
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0003BA58 File Offset: 0x00039C58
		internal static JsonSerializationException Create(JsonReader reader, string message, Exception ex)
		{
			return JsonSerializationException.Create(reader as IJsonLineInfo, reader.Path, message, ex);
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x0003BA70 File Offset: 0x00039C70
		internal static JsonSerializationException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
		{
			message = JsonPosition.FormatMessage(lineInfo, path, message);
			return new JsonSerializationException(message, ex);
		}
	}
}
