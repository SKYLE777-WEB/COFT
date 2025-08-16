using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Buffers
{
	// Token: 0x02000089 RID: 137
	[ComVisible(true)]
	public abstract class ArrayPool<T>
	{
		// Token: 0x1700014B RID: 331
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x0002433C File Offset: 0x0002253C
		public static ArrayPool<T> Shared
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Volatile.Read<ArrayPool<T>>(ref ArrayPool<T>.s_sharedInstance) ?? ArrayPool<T>.EnsureSharedCreated();
			}
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x00024354 File Offset: 0x00022554
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static ArrayPool<T> EnsureSharedCreated()
		{
			Interlocked.CompareExchange<ArrayPool<T>>(ref ArrayPool<T>.s_sharedInstance, ArrayPool<T>.Create(), null);
			return ArrayPool<T>.s_sharedInstance;
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0002436C File Offset: 0x0002256C
		public static ArrayPool<T> Create()
		{
			return new DefaultArrayPool<T>();
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x00024374 File Offset: 0x00022574
		public static ArrayPool<T> Create(int maxArrayLength, int maxArraysPerBucket)
		{
			return new DefaultArrayPool<T>(maxArrayLength, maxArraysPerBucket);
		}

		// Token: 0x060006A0 RID: 1696
		public abstract T[] Rent(int minimumLength);

		// Token: 0x060006A1 RID: 1697
		public abstract void Return(T[] array, bool clearArray = false);

		// Token: 0x040002EB RID: 747
		private static ArrayPool<T> s_sharedInstance;
	}
}
