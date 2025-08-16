using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000194 RID: 404
	internal class BsonBinary : BsonValue
	{
		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x060014B9 RID: 5305 RVA: 0x00072584 File Offset: 0x00070784
		// (set) Token: 0x060014BA RID: 5306 RVA: 0x0007258C File Offset: 0x0007078C
		public BsonBinaryType BinaryType { get; set; }

		// Token: 0x060014BB RID: 5307 RVA: 0x00072598 File Offset: 0x00070798
		public BsonBinary(byte[] value, BsonBinaryType binaryType)
			: base(value, BsonType.Binary)
		{
			this.BinaryType = binaryType;
		}
	}
}
