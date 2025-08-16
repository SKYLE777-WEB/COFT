using System;
using System.Collections.Generic;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F7 RID: 247
	internal class ThreadSafeStore<TKey, TValue>
	{
		// Token: 0x06000D2B RID: 3371 RVA: 0x000527F4 File Offset: 0x000509F4
		public ThreadSafeStore(Func<TKey, TValue> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			this._creator = creator;
			this._store = new Dictionary<TKey, TValue>();
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x0005282C File Offset: 0x00050A2C
		public TValue Get(TKey key)
		{
			TValue tvalue;
			if (!this._store.TryGetValue(key, out tvalue))
			{
				return this.AddValue(key);
			}
			return tvalue;
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x0005285C File Offset: 0x00050A5C
		private TValue AddValue(TKey key)
		{
			TValue tvalue = this._creator(key);
			object @lock = this._lock;
			TValue tvalue3;
			lock (@lock)
			{
				if (this._store == null)
				{
					this._store = new Dictionary<TKey, TValue>();
					this._store[key] = tvalue;
				}
				else
				{
					TValue tvalue2;
					if (this._store.TryGetValue(key, out tvalue2))
					{
						return tvalue2;
					}
					Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._store);
					dictionary[key] = tvalue;
					Thread.MemoryBarrier();
					this._store = dictionary;
				}
				tvalue3 = tvalue;
			}
			return tvalue3;
		}

		// Token: 0x04000538 RID: 1336
		private readonly object _lock = new object();

		// Token: 0x04000539 RID: 1337
		private Dictionary<TKey, TValue> _store;

		// Token: 0x0400053A RID: 1338
		private readonly Func<TKey, TValue> _creator;
	}
}
