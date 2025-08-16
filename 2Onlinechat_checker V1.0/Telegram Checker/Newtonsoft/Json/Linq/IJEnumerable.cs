using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000141 RID: 321
	public interface IJEnumerable<out T> : IEnumerable<T>, IEnumerable where T : JToken
	{
		// Token: 0x17000307 RID: 775
		IJEnumerable<JToken> this[object key] { get; }
	}
}
