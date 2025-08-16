using System;
using System.Diagnostics;

namespace System
{
	// Token: 0x0200002A RID: 42
	internal sealed class MemoryDebugView<T>
	{
		// Token: 0x0600016D RID: 365 RVA: 0x000095DC File Offset: 0x000077DC
		public MemoryDebugView(Memory<T> memory)
		{
			this._memory = memory;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x000095F0 File Offset: 0x000077F0
		public MemoryDebugView(ReadOnlyMemory<T> memory)
		{
			this._memory = memory;
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00009600 File Offset: 0x00007800
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._memory.ToArray();
			}
		}

		// Token: 0x04000109 RID: 265
		private readonly ReadOnlyMemory<T> _memory;
	}
}
