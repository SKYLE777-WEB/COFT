using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	// Token: 0x020000BB RID: 187
	[Serializable]
	public class JsonWriterException : JsonException
	{
		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000B4E RID: 2894 RVA: 0x0004836C File Offset: 0x0004656C
		public string Path { get; }

		// Token: 0x06000B4F RID: 2895 RVA: 0x00048374 File Offset: 0x00046574
		public JsonWriterException()
		{
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x0004837C File Offset: 0x0004657C
		public JsonWriterException(string message)
			: base(message)
		{
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x00048388 File Offset: 0x00046588
		public JsonWriterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x00048394 File Offset: 0x00046594
		public JsonWriterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x000483A0 File Offset: 0x000465A0
		public JsonWriterException(string message, string path, Exception innerException)
			: base(message, innerException)
		{
			this.Path = path;
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x000483B4 File Offset: 0x000465B4
		internal static JsonWriterException Create(JsonWriter writer, string message, Exception ex)
		{
			return JsonWriterException.Create(writer.ContainerPath, message, ex);
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x000483C4 File Offset: 0x000465C4
		internal static JsonWriterException Create(string path, string message, Exception ex)
		{
			message = JsonPosition.FormatMessage(null, path, message);
			return new JsonWriterException(message, path, ex);
		}
	}
}
