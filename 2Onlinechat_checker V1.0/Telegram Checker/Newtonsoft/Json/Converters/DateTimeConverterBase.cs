using System;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200016A RID: 362
	public abstract class DateTimeConverterBase : JsonConverter
	{
		// Token: 0x06001364 RID: 4964 RVA: 0x0006CA64 File Offset: 0x0006AC64
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime) || objectType == typeof(DateTime?) || (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?));
		}
	}
}
