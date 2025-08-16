using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200010A RID: 266
	public interface ISerializationBinder
	{
		// Token: 0x06000DB7 RID: 3511
		Type BindToType(string assemblyName, string typeName);

		// Token: 0x06000DB8 RID: 3512
		void BindToName(Type serializedType, out string assemblyName, out string typeName);
	}
}
