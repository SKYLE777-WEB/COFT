using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x02000132 RID: 306
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	[Serializable]
	public class JsonSchemaException : JsonException
	{
		// Token: 0x170002DF RID: 735
		// (get) Token: 0x0600101E RID: 4126 RVA: 0x000605F0 File Offset: 0x0005E7F0
		public int LineNumber { get; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x0600101F RID: 4127 RVA: 0x000605F8 File Offset: 0x0005E7F8
		public int LinePosition { get; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06001020 RID: 4128 RVA: 0x00060600 File Offset: 0x0005E800
		public string Path { get; }

		// Token: 0x06001021 RID: 4129 RVA: 0x00060608 File Offset: 0x0005E808
		public JsonSchemaException()
		{
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x00060610 File Offset: 0x0005E810
		public JsonSchemaException(string message)
			: base(message)
		{
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x0006061C File Offset: 0x0005E81C
		public JsonSchemaException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x00060628 File Offset: 0x0005E828
		public JsonSchemaException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x00060634 File Offset: 0x0005E834
		internal JsonSchemaException(string message, Exception innerException, string path, int lineNumber, int linePosition)
			: base(message, innerException)
		{
			this.Path = path;
			this.LineNumber = lineNumber;
			this.LinePosition = linePosition;
		}
	}
}
