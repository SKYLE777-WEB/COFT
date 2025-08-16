using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000A4 RID: 164
	public abstract class JsonConverter
	{
		// Token: 0x060007D7 RID: 2007
		public abstract void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);

		// Token: 0x060007D8 RID: 2008
		public abstract object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);

		// Token: 0x060007D9 RID: 2009
		public abstract bool CanConvert(Type objectType);

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060007DA RID: 2010 RVA: 0x000399A0 File Offset: 0x00037BA0
		public virtual bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x000399A4 File Offset: 0x00037BA4
		public virtual bool CanWrite
		{
			get
			{
				return true;
			}
		}
	}
}
