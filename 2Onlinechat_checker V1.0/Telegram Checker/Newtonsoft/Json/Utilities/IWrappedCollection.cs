using System;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000CC RID: 204
	internal interface IWrappedCollection : IList, ICollection, IEnumerable
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000B81 RID: 2945
		object UnderlyingCollection { get; }
	}
}
