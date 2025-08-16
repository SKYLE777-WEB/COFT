using System;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D5 RID: 213
	internal interface IWrappedDictionary : IDictionary, ICollection, IEnumerable
	{
		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000BE2 RID: 3042
		object UnderlyingDictionary { get; }
	}
}
