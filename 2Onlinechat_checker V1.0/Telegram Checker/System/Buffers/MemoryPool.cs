using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x0200003C RID: 60
	[ComVisible(true)]
	public abstract class MemoryPool<T> : IDisposable
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600028C RID: 652 RVA: 0x000111E4 File Offset: 0x0000F3E4
		public static MemoryPool<T> Shared
		{
			get
			{
				return MemoryPool<T>.s_shared;
			}
		}

		// Token: 0x0600028D RID: 653
		public abstract IMemoryOwner<T> Rent(int minBufferSize = -1);

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600028E RID: 654
		public abstract int MaxBufferSize { get; }

		// Token: 0x06000290 RID: 656 RVA: 0x000111F4 File Offset: 0x0000F3F4
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000291 RID: 657
		protected abstract void Dispose(bool disposing);

		// Token: 0x04000125 RID: 293
		private static readonly MemoryPool<T> s_shared = new ArrayMemoryPool<T>();
	}
}
