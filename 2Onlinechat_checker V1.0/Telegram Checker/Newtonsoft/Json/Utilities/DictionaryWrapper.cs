using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D6 RID: 214
	internal class DictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IWrappedDictionary, IDictionary, ICollection
	{
		// Token: 0x06000BE3 RID: 3043 RVA: 0x0004C120 File Offset: 0x0004A320
		public DictionaryWrapper(IDictionary dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			this._dictionary = dictionary;
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x0004C13C File Offset: 0x0004A33C
		public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			this._genericDictionary = dictionary;
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x0004C158 File Offset: 0x0004A358
		public DictionaryWrapper(IReadOnlyDictionary<TKey, TValue> dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
			this._readOnlyDictionary = dictionary;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0004C174 File Offset: 0x0004A374
		public void Add(TKey key, TValue value)
		{
			if (this._dictionary != null)
			{
				this._dictionary.Add(key, value);
				return;
			}
			if (this._genericDictionary != null)
			{
				this._genericDictionary.Add(key, value);
				return;
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x0004C1C8 File Offset: 0x0004A3C8
		public bool ContainsKey(TKey key)
		{
			if (this._dictionary != null)
			{
				return this._dictionary.Contains(key);
			}
			if (this._readOnlyDictionary != null)
			{
				return this._readOnlyDictionary.ContainsKey(key);
			}
			return this._genericDictionary.ContainsKey(key);
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000BE8 RID: 3048 RVA: 0x0004C21C File Offset: 0x0004A41C
		public ICollection<TKey> Keys
		{
			get
			{
				if (this._dictionary != null)
				{
					return this._dictionary.Keys.Cast<TKey>().ToList<TKey>();
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary.Keys.ToList<TKey>();
				}
				return this._genericDictionary.Keys;
			}
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x0004C278 File Offset: 0x0004A478
		public bool Remove(TKey key)
		{
			if (this._dictionary != null)
			{
				if (this._dictionary.Contains(key))
				{
					this._dictionary.Remove(key);
					return true;
				}
				return false;
			}
			else
			{
				if (this._readOnlyDictionary != null)
				{
					throw new NotSupportedException();
				}
				return this._genericDictionary.Remove(key);
			}
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0004C2DC File Offset: 0x0004A4DC
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (this._dictionary != null)
			{
				if (!this._dictionary.Contains(key))
				{
					value = default(TValue);
					return false;
				}
				value = (TValue)((object)this._dictionary[key]);
				return true;
			}
			else
			{
				if (this._readOnlyDictionary != null)
				{
					throw new NotSupportedException();
				}
				return this._genericDictionary.TryGetValue(key, out value);
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000BEB RID: 3051 RVA: 0x0004C354 File Offset: 0x0004A554
		public ICollection<TValue> Values
		{
			get
			{
				if (this._dictionary != null)
				{
					return this._dictionary.Values.Cast<TValue>().ToList<TValue>();
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary.Values.ToList<TValue>();
				}
				return this._genericDictionary.Values;
			}
		}

		// Token: 0x17000203 RID: 515
		public TValue this[TKey key]
		{
			get
			{
				if (this._dictionary != null)
				{
					return (TValue)((object)this._dictionary[key]);
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary[key];
				}
				return this._genericDictionary[key];
			}
			set
			{
				if (this._dictionary != null)
				{
					this._dictionary[key] = value;
					return;
				}
				if (this._readOnlyDictionary != null)
				{
					throw new NotSupportedException();
				}
				this._genericDictionary[key] = value;
			}
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x0004C45C File Offset: 0x0004A65C
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			if (this._dictionary != null)
			{
				((IList)this._dictionary).Add(item);
				return;
			}
			if (this._readOnlyDictionary != null)
			{
				throw new NotSupportedException();
			}
			IDictionary<TKey, TValue> genericDictionary = this._genericDictionary;
			if (genericDictionary == null)
			{
				return;
			}
			genericDictionary.Add(item);
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0004C4B8 File Offset: 0x0004A6B8
		public void Clear()
		{
			if (this._dictionary != null)
			{
				this._dictionary.Clear();
				return;
			}
			if (this._readOnlyDictionary != null)
			{
				throw new NotSupportedException();
			}
			this._genericDictionary.Clear();
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0004C4F0 File Offset: 0x0004A6F0
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (this._dictionary != null)
			{
				return ((IList)this._dictionary).Contains(item);
			}
			if (this._readOnlyDictionary != null)
			{
				return this._readOnlyDictionary.Contains(item);
			}
			return this._genericDictionary.Contains(item);
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0004C548 File Offset: 0x0004A748
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (this._dictionary != null)
			{
				using (IDictionaryEnumerator enumerator = this._dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry entry = enumerator.Entry;
						array[arrayIndex++] = new KeyValuePair<TKey, TValue>((TKey)((object)entry.Key), (TValue)((object)entry.Value));
					}
					return;
				}
			}
			if (this._readOnlyDictionary != null)
			{
				throw new NotSupportedException();
			}
			this._genericDictionary.CopyTo(array, arrayIndex);
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x0004C5F4 File Offset: 0x0004A7F4
		public int Count
		{
			get
			{
				if (this._dictionary != null)
				{
					return this._dictionary.Count;
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary.Count;
				}
				return this._genericDictionary.Count;
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000BF3 RID: 3059 RVA: 0x0004C630 File Offset: 0x0004A830
		public bool IsReadOnly
		{
			get
			{
				if (this._dictionary != null)
				{
					return this._dictionary.IsReadOnly;
				}
				return this._readOnlyDictionary != null || this._genericDictionary.IsReadOnly;
			}
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0004C664 File Offset: 0x0004A864
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (this._dictionary != null)
			{
				if (!this._dictionary.Contains(item.Key))
				{
					return true;
				}
				if (object.Equals(this._dictionary[item.Key], item.Value))
				{
					this._dictionary.Remove(item.Key);
					return true;
				}
				return false;
			}
			else
			{
				if (this._readOnlyDictionary != null)
				{
					throw new NotSupportedException();
				}
				return this._genericDictionary.Remove(item);
			}
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x0004C704 File Offset: 0x0004A904
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			if (this._dictionary != null)
			{
				return (from DictionaryEntry de in this._dictionary
					select new KeyValuePair<TKey, TValue>((TKey)((object)de.Key), (TValue)((object)de.Value))).GetEnumerator();
			}
			if (this._readOnlyDictionary != null)
			{
				return this._readOnlyDictionary.GetEnumerator();
			}
			return this._genericDictionary.GetEnumerator();
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0004C77C File Offset: 0x0004A97C
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0004C784 File Offset: 0x0004A984
		void IDictionary.Add(object key, object value)
		{
			if (this._dictionary != null)
			{
				this._dictionary.Add(key, value);
				return;
			}
			if (this._readOnlyDictionary != null)
			{
				throw new NotSupportedException();
			}
			this._genericDictionary.Add((TKey)((object)key), (TValue)((object)value));
		}

		// Token: 0x17000206 RID: 518
		object IDictionary.this[object key]
		{
			get
			{
				if (this._dictionary != null)
				{
					return this._dictionary[key];
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary[(TKey)((object)key)];
				}
				return this._genericDictionary[(TKey)((object)key)];
			}
			set
			{
				if (this._dictionary != null)
				{
					this._dictionary[key] = value;
					return;
				}
				if (this._readOnlyDictionary != null)
				{
					throw new NotSupportedException();
				}
				this._genericDictionary[(TKey)((object)key)] = (TValue)((object)value);
			}
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x0004C890 File Offset: 0x0004AA90
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			if (this._dictionary != null)
			{
				return this._dictionary.GetEnumerator();
			}
			if (this._readOnlyDictionary != null)
			{
				return new DictionaryWrapper<TKey, TValue>.DictionaryEnumerator<TKey, TValue>(this._readOnlyDictionary.GetEnumerator());
			}
			return new DictionaryWrapper<TKey, TValue>.DictionaryEnumerator<TKey, TValue>(this._genericDictionary.GetEnumerator());
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x0004C8F0 File Offset: 0x0004AAF0
		bool IDictionary.Contains(object key)
		{
			if (this._genericDictionary != null)
			{
				return this._genericDictionary.ContainsKey((TKey)((object)key));
			}
			if (this._readOnlyDictionary != null)
			{
				return this._readOnlyDictionary.ContainsKey((TKey)((object)key));
			}
			return this._dictionary.Contains(key);
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x0004C948 File Offset: 0x0004AB48
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this._genericDictionary == null && (this._readOnlyDictionary != null || this._dictionary.IsFixedSize);
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000BFD RID: 3069 RVA: 0x0004C970 File Offset: 0x0004AB70
		ICollection IDictionary.Keys
		{
			get
			{
				if (this._genericDictionary != null)
				{
					return this._genericDictionary.Keys.ToList<TKey>();
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary.Keys.ToList<TKey>();
				}
				return this._dictionary.Keys;
			}
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0004C9C4 File Offset: 0x0004ABC4
		public void Remove(object key)
		{
			if (this._dictionary != null)
			{
				this._dictionary.Remove(key);
				return;
			}
			if (this._readOnlyDictionary != null)
			{
				throw new NotSupportedException();
			}
			this._genericDictionary.Remove((TKey)((object)key));
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000BFF RID: 3071 RVA: 0x0004CA04 File Offset: 0x0004AC04
		ICollection IDictionary.Values
		{
			get
			{
				if (this._genericDictionary != null)
				{
					return this._genericDictionary.Values.ToList<TValue>();
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary.Values.ToList<TValue>();
				}
				return this._dictionary.Values;
			}
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0004CA58 File Offset: 0x0004AC58
		void ICollection.CopyTo(Array array, int index)
		{
			if (this._dictionary != null)
			{
				this._dictionary.CopyTo(array, index);
				return;
			}
			if (this._readOnlyDictionary != null)
			{
				throw new NotSupportedException();
			}
			this._genericDictionary.CopyTo((KeyValuePair<TKey, TValue>[])array, index);
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000C01 RID: 3073 RVA: 0x0004CA98 File Offset: 0x0004AC98
		bool ICollection.IsSynchronized
		{
			get
			{
				return this._dictionary != null && this._dictionary.IsSynchronized;
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000C02 RID: 3074 RVA: 0x0004CAB4 File Offset: 0x0004ACB4
		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000C03 RID: 3075 RVA: 0x0004CADC File Offset: 0x0004ACDC
		public object UnderlyingDictionary
		{
			get
			{
				if (this._dictionary != null)
				{
					return this._dictionary;
				}
				if (this._readOnlyDictionary != null)
				{
					return this._readOnlyDictionary;
				}
				return this._genericDictionary;
			}
		}

		// Token: 0x040004E5 RID: 1253
		private readonly IDictionary _dictionary;

		// Token: 0x040004E6 RID: 1254
		private readonly IDictionary<TKey, TValue> _genericDictionary;

		// Token: 0x040004E7 RID: 1255
		private readonly IReadOnlyDictionary<TKey, TValue> _readOnlyDictionary;

		// Token: 0x040004E8 RID: 1256
		private object _syncRoot;

		// Token: 0x0200023A RID: 570
		private struct DictionaryEnumerator<TEnumeratorKey, TEnumeratorValue> : IDictionaryEnumerator, IEnumerator
		{
			// Token: 0x06001694 RID: 5780 RVA: 0x00082248 File Offset: 0x00080448
			public DictionaryEnumerator(IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
			{
				ValidationUtils.ArgumentNotNull(e, "e");
				this._e = e;
			}

			// Token: 0x17000410 RID: 1040
			// (get) Token: 0x06001695 RID: 5781 RVA: 0x0008225C File Offset: 0x0008045C
			public DictionaryEntry Entry
			{
				get
				{
					return (DictionaryEntry)this.Current;
				}
			}

			// Token: 0x17000411 RID: 1041
			// (get) Token: 0x06001696 RID: 5782 RVA: 0x0008226C File Offset: 0x0008046C
			public object Key
			{
				get
				{
					return this.Entry.Key;
				}
			}

			// Token: 0x17000412 RID: 1042
			// (get) Token: 0x06001697 RID: 5783 RVA: 0x0008228C File Offset: 0x0008048C
			public object Value
			{
				get
				{
					return this.Entry.Value;
				}
			}

			// Token: 0x17000413 RID: 1043
			// (get) Token: 0x06001698 RID: 5784 RVA: 0x000822AC File Offset: 0x000804AC
			public object Current
			{
				get
				{
					KeyValuePair<TEnumeratorKey, TEnumeratorValue> keyValuePair = this._e.Current;
					object obj = keyValuePair.Key;
					keyValuePair = this._e.Current;
					return new DictionaryEntry(obj, keyValuePair.Value);
				}
			}

			// Token: 0x06001699 RID: 5785 RVA: 0x000822F8 File Offset: 0x000804F8
			public bool MoveNext()
			{
				return this._e.MoveNext();
			}

			// Token: 0x0600169A RID: 5786 RVA: 0x00082308 File Offset: 0x00080508
			public void Reset()
			{
				this._e.Reset();
			}

			// Token: 0x04000AA7 RID: 2727
			private readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;
		}
	}
}
