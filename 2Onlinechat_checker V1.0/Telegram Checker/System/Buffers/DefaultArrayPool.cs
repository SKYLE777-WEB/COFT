using System;
using System.Diagnostics;
using System.Threading;

namespace System.Buffers
{
	// Token: 0x0200008B RID: 139
	internal sealed class DefaultArrayPool<T> : ArrayPool<T>
	{
		// Token: 0x060006A9 RID: 1705 RVA: 0x00024594 File Offset: 0x00022794
		internal DefaultArrayPool()
			: this(1048576, 50)
		{
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x000245A4 File Offset: 0x000227A4
		internal DefaultArrayPool(int maxArrayLength, int maxArraysPerBucket)
		{
			if (maxArrayLength <= 0)
			{
				throw new ArgumentOutOfRangeException("maxArrayLength");
			}
			if (maxArraysPerBucket <= 0)
			{
				throw new ArgumentOutOfRangeException("maxArraysPerBucket");
			}
			if (maxArrayLength > 1073741824)
			{
				maxArrayLength = 1073741824;
			}
			else if (maxArrayLength < 16)
			{
				maxArrayLength = 16;
			}
			int id = this.Id;
			int num = Utilities.SelectBucketIndex(maxArrayLength);
			DefaultArrayPool<T>.Bucket[] array = new DefaultArrayPool<T>.Bucket[num + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DefaultArrayPool<T>.Bucket(Utilities.GetMaxSizeForBucket(i), maxArraysPerBucket, id);
			}
			this._buckets = array;
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x00024648 File Offset: 0x00022848
		private int Id
		{
			get
			{
				return this.GetHashCode();
			}
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x00024650 File Offset: 0x00022850
		public override T[] Rent(int minimumLength)
		{
			if (minimumLength < 0)
			{
				throw new ArgumentOutOfRangeException("minimumLength");
			}
			if (minimumLength == 0)
			{
				T[] array;
				if ((array = DefaultArrayPool<T>.s_emptyArray) == null)
				{
					array = (DefaultArrayPool<T>.s_emptyArray = new T[0]);
				}
				return array;
			}
			ArrayPoolEventSource log = ArrayPoolEventSource.Log;
			int num = Utilities.SelectBucketIndex(minimumLength);
			T[] array2;
			if (num < this._buckets.Length)
			{
				int num2 = num;
				for (;;)
				{
					array2 = this._buckets[num2].Rent();
					if (array2 != null)
					{
						break;
					}
					if (++num2 >= this._buckets.Length || num2 == num + 2)
					{
						goto IL_00B3;
					}
				}
				if (log.IsEnabled())
				{
					log.BufferRented(array2.GetHashCode(), array2.Length, this.Id, this._buckets[num2].Id);
				}
				return array2;
				IL_00B3:
				array2 = new T[this._buckets[num]._bufferLength];
			}
			else
			{
				array2 = new T[minimumLength];
			}
			if (log.IsEnabled())
			{
				int hashCode = array2.GetHashCode();
				int num3 = -1;
				log.BufferRented(hashCode, array2.Length, this.Id, num3);
				log.BufferAllocated(hashCode, array2.Length, this.Id, num3, (num >= this._buckets.Length) ? ArrayPoolEventSource.BufferAllocatedReason.OverMaximumSize : ArrayPoolEventSource.BufferAllocatedReason.PoolExhausted);
			}
			return array2;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0002478C File Offset: 0x0002298C
		public override void Return(T[] array, bool clearArray = false)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length == 0)
			{
				return;
			}
			int num = Utilities.SelectBucketIndex(array.Length);
			if (num < this._buckets.Length)
			{
				if (clearArray)
				{
					Array.Clear(array, 0, array.Length);
				}
				this._buckets[num].Return(array);
			}
			ArrayPoolEventSource log = ArrayPoolEventSource.Log;
			if (log.IsEnabled())
			{
				log.BufferReturned(array.GetHashCode(), array.Length, this.Id);
			}
		}

		// Token: 0x040002ED RID: 749
		private const int DefaultMaxArrayLength = 1048576;

		// Token: 0x040002EE RID: 750
		private const int DefaultMaxNumberOfArraysPerBucket = 50;

		// Token: 0x040002EF RID: 751
		private static T[] s_emptyArray;

		// Token: 0x040002F0 RID: 752
		private readonly DefaultArrayPool<T>.Bucket[] _buckets;

		// Token: 0x020001E9 RID: 489
		private sealed class Bucket
		{
			// Token: 0x060015E1 RID: 5601 RVA: 0x000770D4 File Offset: 0x000752D4
			internal Bucket(int bufferLength, int numberOfBuffers, int poolId)
			{
				this._lock = new SpinLock(Debugger.IsAttached);
				this._buffers = new T[numberOfBuffers][];
				this._bufferLength = bufferLength;
				this._poolId = poolId;
			}

			// Token: 0x17000405 RID: 1029
			// (get) Token: 0x060015E2 RID: 5602 RVA: 0x00077108 File Offset: 0x00075308
			internal int Id
			{
				get
				{
					return this.GetHashCode();
				}
			}

			// Token: 0x060015E3 RID: 5603 RVA: 0x00077110 File Offset: 0x00075310
			internal T[] Rent()
			{
				T[][] buffers = this._buffers;
				T[] array = null;
				bool flag = false;
				bool flag2 = false;
				try
				{
					this._lock.Enter(ref flag);
					if (this._index < buffers.Length)
					{
						array = buffers[this._index];
						T[][] array2 = buffers;
						int index = this._index;
						this._index = index + 1;
						array2[index] = null;
						flag2 = array == null;
					}
				}
				finally
				{
					if (flag)
					{
						this._lock.Exit(false);
					}
				}
				if (flag2)
				{
					array = new T[this._bufferLength];
					ArrayPoolEventSource log = ArrayPoolEventSource.Log;
					if (log.IsEnabled())
					{
						log.BufferAllocated(array.GetHashCode(), this._bufferLength, this._poolId, this.Id, ArrayPoolEventSource.BufferAllocatedReason.Pooled);
					}
				}
				return array;
			}

			// Token: 0x060015E4 RID: 5604 RVA: 0x000771DC File Offset: 0x000753DC
			internal void Return(T[] array)
			{
				if (array.Length != this._bufferLength)
				{
					throw new ArgumentException(System.Buffers157539.SR.ArgumentException_BufferNotFromPool, "array");
				}
				bool flag = false;
				try
				{
					this._lock.Enter(ref flag);
					if (this._index != 0)
					{
						T[][] buffers = this._buffers;
						int num = this._index - 1;
						this._index = num;
						buffers[num] = array;
					}
				}
				finally
				{
					if (flag)
					{
						this._lock.Exit(false);
					}
				}
			}

			// Token: 0x0400088A RID: 2186
			internal readonly int _bufferLength;

			// Token: 0x0400088B RID: 2187
			private readonly T[][] _buffers;

			// Token: 0x0400088C RID: 2188
			private readonly int _poolId;

			// Token: 0x0400088D RID: 2189
			private SpinLock _lock;

			// Token: 0x0400088E RID: 2190
			private int _index;
		}
	}
}
