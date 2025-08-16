using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x02000046 RID: 70
	[ComVisible(true)]
	public abstract class MemoryManager<T> : IMemoryOwner<T>, IDisposable, IPinnable
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x000126EC File Offset: 0x000108EC
		public virtual Memory<T> Memory
		{
			get
			{
				return new Memory<T>(this, this.GetSpan().Length);
			}
		}

		// Token: 0x060002E4 RID: 740
		public abstract Span<T> GetSpan();

		// Token: 0x060002E5 RID: 741
		public abstract MemoryHandle Pin(int elementIndex = 0);

		// Token: 0x060002E6 RID: 742
		public abstract void Unpin();

		// Token: 0x060002E7 RID: 743 RVA: 0x00012714 File Offset: 0x00010914
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected Memory<T> CreateMemory(int length)
		{
			return new Memory<T>(this, length);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00012720 File Offset: 0x00010920
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected Memory<T> CreateMemory(int start, int length)
		{
			return new Memory<T>(this, start, length);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0001272C File Offset: 0x0001092C
		protected internal virtual bool TryGetArray(out ArraySegment<T> segment)
		{
			segment = default(ArraySegment<T>);
			return false;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00012738 File Offset: 0x00010938
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060002EB RID: 747
		protected abstract void Dispose(bool disposing);
	}
}
