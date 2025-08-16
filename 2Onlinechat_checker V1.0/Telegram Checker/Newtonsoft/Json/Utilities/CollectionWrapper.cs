using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000CD RID: 205
	internal class CollectionWrapper<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IWrappedCollection, IList, ICollection
	{
		// Token: 0x06000B82 RID: 2946 RVA: 0x00048ED8 File Offset: 0x000470D8
		public CollectionWrapper(IList list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			ICollection<T> collection = list as ICollection<T>;
			if (collection != null)
			{
				this._genericCollection = collection;
				return;
			}
			this._list = list;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x00048F18 File Offset: 0x00047118
		public CollectionWrapper(ICollection<T> list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");
			this._genericCollection = list;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x00048F34 File Offset: 0x00047134
		public virtual void Add(T item)
		{
			if (this._genericCollection != null)
			{
				this._genericCollection.Add(item);
				return;
			}
			this._list.Add(item);
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x00048F60 File Offset: 0x00047160
		public virtual void Clear()
		{
			if (this._genericCollection != null)
			{
				this._genericCollection.Clear();
				return;
			}
			this._list.Clear();
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x00048F84 File Offset: 0x00047184
		public virtual bool Contains(T item)
		{
			if (this._genericCollection != null)
			{
				return this._genericCollection.Contains(item);
			}
			return this._list.Contains(item);
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x00048FB0 File Offset: 0x000471B0
		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			if (this._genericCollection != null)
			{
				this._genericCollection.CopyTo(array, arrayIndex);
				return;
			}
			this._list.CopyTo(array, arrayIndex);
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000B88 RID: 2952 RVA: 0x00048FD8 File Offset: 0x000471D8
		public virtual int Count
		{
			get
			{
				if (this._genericCollection != null)
				{
					return this._genericCollection.Count;
				}
				return this._list.Count;
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000B89 RID: 2953 RVA: 0x00048FFC File Offset: 0x000471FC
		public virtual bool IsReadOnly
		{
			get
			{
				if (this._genericCollection != null)
				{
					return this._genericCollection.IsReadOnly;
				}
				return this._list.IsReadOnly;
			}
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x00049020 File Offset: 0x00047220
		public virtual bool Remove(T item)
		{
			if (this._genericCollection != null)
			{
				return this._genericCollection.Remove(item);
			}
			bool flag = this._list.Contains(item);
			if (flag)
			{
				this._list.Remove(item);
			}
			return flag;
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x00049074 File Offset: 0x00047274
		public virtual IEnumerator<T> GetEnumerator()
		{
			IEnumerable<T> genericCollection = this._genericCollection;
			return (genericCollection ?? this._list.Cast<T>()).GetEnumerator();
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x000490A4 File Offset: 0x000472A4
		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerable genericCollection = this._genericCollection;
			return (genericCollection ?? this._list).GetEnumerator();
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x000490D0 File Offset: 0x000472D0
		int IList.Add(object value)
		{
			CollectionWrapper<T>.VerifyValueType(value);
			this.Add((T)((object)value));
			return this.Count - 1;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x000490EC File Offset: 0x000472EC
		bool IList.Contains(object value)
		{
			return CollectionWrapper<T>.IsCompatibleObject(value) && this.Contains((T)((object)value));
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x00049108 File Offset: 0x00047308
		int IList.IndexOf(object value)
		{
			if (this._genericCollection != null)
			{
				throw new InvalidOperationException("Wrapped ICollection<T> does not support IndexOf.");
			}
			if (CollectionWrapper<T>.IsCompatibleObject(value))
			{
				return this._list.IndexOf((T)((object)value));
			}
			return -1;
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x00049144 File Offset: 0x00047344
		void IList.RemoveAt(int index)
		{
			if (this._genericCollection != null)
			{
				throw new InvalidOperationException("Wrapped ICollection<T> does not support RemoveAt.");
			}
			this._list.RemoveAt(index);
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x00049168 File Offset: 0x00047368
		void IList.Insert(int index, object value)
		{
			if (this._genericCollection != null)
			{
				throw new InvalidOperationException("Wrapped ICollection<T> does not support Insert.");
			}
			CollectionWrapper<T>.VerifyValueType(value);
			this._list.Insert(index, (T)((object)value));
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000B92 RID: 2962 RVA: 0x000491A0 File Offset: 0x000473A0
		bool IList.IsFixedSize
		{
			get
			{
				if (this._genericCollection != null)
				{
					return this._genericCollection.IsReadOnly;
				}
				return this._list.IsFixedSize;
			}
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x000491C4 File Offset: 0x000473C4
		void IList.Remove(object value)
		{
			if (CollectionWrapper<T>.IsCompatibleObject(value))
			{
				this.Remove((T)((object)value));
			}
		}

		// Token: 0x170001F9 RID: 505
		object IList.this[int index]
		{
			get
			{
				if (this._genericCollection != null)
				{
					throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
				}
				return this._list[index];
			}
			set
			{
				if (this._genericCollection != null)
				{
					throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
				}
				CollectionWrapper<T>.VerifyValueType(value);
				this._list[index] = (T)((object)value);
			}
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x0004923C File Offset: 0x0004743C
		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			this.CopyTo((T[])array, arrayIndex);
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000B97 RID: 2967 RVA: 0x0004924C File Offset: 0x0004744C
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000B98 RID: 2968 RVA: 0x00049250 File Offset: 0x00047450
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

		// Token: 0x06000B99 RID: 2969 RVA: 0x00049278 File Offset: 0x00047478
		private static void VerifyValueType(object value)
		{
			if (!CollectionWrapper<T>.IsCompatibleObject(value))
			{
				throw new ArgumentException("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.".FormatWith(CultureInfo.InvariantCulture, value, typeof(T)), "value");
			}
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x000492AC File Offset: 0x000474AC
		private static bool IsCompatibleObject(object value)
		{
			return value is T || (value == null && (!typeof(T).IsValueType() || ReflectionUtils.IsNullableType(typeof(T))));
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000B9B RID: 2971 RVA: 0x000492EC File Offset: 0x000474EC
		public object UnderlyingCollection
		{
			get
			{
				return this._genericCollection ?? this._list;
			}
		}

		// Token: 0x04000483 RID: 1155
		private readonly IList _list;

		// Token: 0x04000484 RID: 1156
		private readonly ICollection<T> _genericCollection;

		// Token: 0x04000485 RID: 1157
		private object _syncRoot;
	}
}
