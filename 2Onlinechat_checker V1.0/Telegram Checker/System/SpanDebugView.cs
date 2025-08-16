using System;
using System.Diagnostics;

namespace System
{
	// Token: 0x0200002F RID: 47
	internal sealed class SpanDebugView<T>
	{
		// Token: 0x06000210 RID: 528 RVA: 0x0000C600 File Offset: 0x0000A800
		public SpanDebugView(Span<T> span)
		{
			this._array = span.ToArray();
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000C618 File Offset: 0x0000A818
		public SpanDebugView(ReadOnlySpan<T> span)
		{
			this._array = span.ToArray();
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000C630 File Offset: 0x0000A830
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._array;
			}
		}

		// Token: 0x04000115 RID: 277
		private readonly T[] _array;
	}
}
