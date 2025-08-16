using System;
using System.Diagnostics;

namespace System.Buffers
{
	// Token: 0x02000040 RID: 64
	internal sealed class ReadOnlySequenceDebugView<T>
	{
		// Token: 0x060002C5 RID: 709 RVA: 0x000122D0 File Offset: 0x000104D0
		public ReadOnlySequenceDebugView(ReadOnlySequence<T> sequence)
		{
			this._array = (in sequence).ToArray<T>();
			int num = 0;
			foreach (ReadOnlyMemory<T> readOnlyMemory in sequence)
			{
				num++;
			}
			ReadOnlyMemory<T>[] array = new ReadOnlyMemory<T>[num];
			int num2 = 0;
			foreach (ReadOnlyMemory<T> readOnlyMemory2 in sequence)
			{
				array[num2] = readOnlyMemory2;
				num2++;
			}
			this._segments = new ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments
			{
				Segments = array
			};
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x0001236C File Offset: 0x0001056C
		public ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments BufferSegments
		{
			get
			{
				return this._segments;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x00012374 File Offset: 0x00010574
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._array;
			}
		}

		// Token: 0x04000138 RID: 312
		private readonly T[] _array;

		// Token: 0x04000139 RID: 313
		private readonly ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments _segments;

		// Token: 0x020001C5 RID: 453
		[DebuggerDisplay("Count: {Segments.Length}", Name = "Segments")]
		public struct ReadOnlySequenceDebugViewSegments
		{
			// Token: 0x170003DD RID: 989
			// (get) Token: 0x06001559 RID: 5465 RVA: 0x00075AB0 File Offset: 0x00073CB0
			// (set) Token: 0x0600155A RID: 5466 RVA: 0x00075AB8 File Offset: 0x00073CB8
			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public ReadOnlyMemory<T>[] Segments { get; set; }
		}
	}
}
