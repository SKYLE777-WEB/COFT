using System;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200012A RID: 298
	internal class SerializationBinderAdapter : ISerializationBinder
	{
		// Token: 0x06000F61 RID: 3937 RVA: 0x0005DF58 File Offset: 0x0005C158
		public SerializationBinderAdapter(SerializationBinder serializationBinder)
		{
			this.SerializationBinder = serializationBinder;
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0005DF68 File Offset: 0x0005C168
		public Type BindToType(string assemblyName, string typeName)
		{
			return this.SerializationBinder.BindToType(assemblyName, typeName);
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x0005DF78 File Offset: 0x0005C178
		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			this.SerializationBinder.BindToName(serializedType, out assemblyName, out typeName);
		}

		// Token: 0x040005F4 RID: 1524
		public readonly SerializationBinder SerializationBinder;
	}
}
