using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	// Token: 0x02000039 RID: 57
	internal sealed class ArrayMemoryPool<T> : MemoryPool<T>
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600027E RID: 638 RVA: 0x00010F4C File Offset: 0x0000F14C
		public sealed override int MaxBufferSize
		{
			get
			{
				return int.MaxValue;
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00010F54 File Offset: 0x0000F154
		public sealed override IMemoryOwner<T> Rent(int minimumBufferSize = -1)
		{
			if (minimumBufferSize == -1)
			{
				minimumBufferSize = 1 + 4095 / Unsafe.SizeOf<T>();
			}
			else if (minimumBufferSize > 2147483647)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.minimumBufferSize);
			}
			return new ArrayMemoryPool<T>.ArrayMemoryPoolBuffer(minimumBufferSize);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00010F88 File Offset: 0x0000F188
		protected sealed override void Dispose(bool disposing)
		{
		}

		// Token: 0x04000124 RID: 292
		private const int s_maxBufferSize = 2147483647;

		// Token: 0x020001C2 RID: 450
		private sealed class ArrayMemoryPoolBuffer : IMemoryOwner<T>, IDisposable
		{
			// Token: 0x06001553 RID: 5459 RVA: 0x000759D8 File Offset: 0x00073BD8
			public ArrayMemoryPoolBuffer(int size)
			{
				this._array = ArrayPool<T>.Shared.Rent(size);
			}

			// Token: 0x170003DB RID: 987
			// (get) Token: 0x06001554 RID: 5460 RVA: 0x000759F4 File Offset: 0x00073BF4
			public Memory<T> Memory
			{
				get
				{
					T[] array = this._array;
					if (array == null)
					{
						ThrowHelper.ThrowObjectDisposedException_ArrayMemoryPoolBuffer();
					}
					return new Memory<T>(array);
				}
			}

			// Token: 0x06001555 RID: 5461 RVA: 0x00075A20 File Offset: 0x00073C20
			public void Dispose()
			{
				T[] array = this._array;
				if (array != null)
				{
					this._array = null;
					ArrayPool<T>.Shared.Return(array, false);
				}
			}

			// Token: 0x04000801 RID: 2049
			private T[] _array;
		}
	}
}
