using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LanguageIdentification.Internal
{
	// Token: 0x0200001E RID: 30
	[NullableContext(1)]
	[Nullable(0)]
	internal class FixedSizeArrayPool<[Nullable(2)] T> : IFixedSizeArrayPool<T>
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00007F34 File Offset: 0x00006134
		internal int Count
		{
			get
			{
				return this._arrayQueue.Count;
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00007F44 File Offset: 0x00006144
		public FixedSizeArrayPool(int arraySize, int maxRetainSize)
		{
			if (arraySize < 1)
			{
				throw new ArgumentOutOfRangeException("arraySize", "Must be greater than 0.");
			}
			if (maxRetainSize < 1)
			{
				throw new ArgumentOutOfRangeException("maxRetainSize", "Must be greater than 0.");
			}
			this._arraySize = arraySize;
			this._maxRetainSize = maxRetainSize;
			this._currentSize = 0;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00007FAC File Offset: 0x000061AC
		public T[] Rent()
		{
			T[] array;
			if (this._arrayQueue.TryDequeue(out array))
			{
				Interlocked.Add(ref this._currentSize, -1);
				return array;
			}
			return new T[this._arraySize];
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00007FEC File Offset: 0x000061EC
		public void Return(T[] item)
		{
			if (item.Length != this._arraySize)
			{
				throw new ArgumentException("array size error.", "item");
			}
			if (this._currentSize < this._maxRetainSize)
			{
				Interlocked.Add(ref this._currentSize, 1);
				this._arrayQueue.Enqueue(item);
			}
		}

		// Token: 0x040000B4 RID: 180
		private readonly ConcurrentQueue<T[]> _arrayQueue = new ConcurrentQueue<T[]>();

		// Token: 0x040000B5 RID: 181
		private readonly int _arraySize;

		// Token: 0x040000B6 RID: 182
		private readonly int _maxRetainSize;

		// Token: 0x040000B7 RID: 183
		private volatile int _currentSize;
	}
}
