using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json
{
	// Token: 0x020000A8 RID: 168
	[Serializable]
	public class JsonException : Exception
	{
		// Token: 0x060007E4 RID: 2020 RVA: 0x00039A14 File Offset: 0x00037C14
		public JsonException()
		{
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x00039A1C File Offset: 0x00037C1C
		public JsonException(string message)
			: base(message)
		{
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x00039A28 File Offset: 0x00037C28
		public JsonException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x00039A34 File Offset: 0x00037C34
		public JsonException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x00039A40 File Offset: 0x00037C40
		internal static JsonException Create(IJsonLineInfo lineInfo, string path, string message)
		{
			message = JsonPosition.FormatMessage(lineInfo, path, message);
			return new JsonException(message);
		}
	}
}
