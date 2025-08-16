using System;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000167 RID: 359
	public abstract class CustomCreationConverter<T> : JsonConverter
	{
		// Token: 0x06001354 RID: 4948 RVA: 0x0006C3C4 File Offset: 0x0006A5C4
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x0006C3D0 File Offset: 0x0006A5D0
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			T t = this.Create(objectType);
			if (t == null)
			{
				throw new JsonSerializationException("No object created.");
			}
			serializer.Populate(reader, t);
			return t;
		}

		// Token: 0x06001356 RID: 4950
		public abstract T Create(Type objectType);

		// Token: 0x06001357 RID: 4951 RVA: 0x0006C424 File Offset: 0x0006A624
		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06001358 RID: 4952 RVA: 0x0006C438 File Offset: 0x0006A638
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}
	}
}
