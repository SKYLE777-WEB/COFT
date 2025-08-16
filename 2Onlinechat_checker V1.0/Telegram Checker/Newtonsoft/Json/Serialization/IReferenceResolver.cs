using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000109 RID: 265
	public interface IReferenceResolver
	{
		// Token: 0x06000DB3 RID: 3507
		object ResolveReference(object context, string reference);

		// Token: 0x06000DB4 RID: 3508
		string GetReference(object context, object value);

		// Token: 0x06000DB5 RID: 3509
		bool IsReferenced(object context, object value);

		// Token: 0x06000DB6 RID: 3510
		void AddReference(object context, string reference, object value);
	}
}
