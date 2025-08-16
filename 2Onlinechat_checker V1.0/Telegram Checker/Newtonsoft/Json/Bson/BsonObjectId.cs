using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x0200018B RID: 395
	[Obsolete("BSON reading and writing has been moved to its own package. See https://www.nuget.org/packages/Newtonsoft.Json.Bson for more details.")]
	public class BsonObjectId
	{
		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06001479 RID: 5241 RVA: 0x00071684 File Offset: 0x0006F884
		public byte[] Value { get; }

		// Token: 0x0600147A RID: 5242 RVA: 0x0007168C File Offset: 0x0006F88C
		public BsonObjectId(byte[] value)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			if (value.Length != 12)
			{
				throw new ArgumentException("An ObjectId must be 12 bytes", "value");
			}
			this.Value = value;
		}
	}
}
