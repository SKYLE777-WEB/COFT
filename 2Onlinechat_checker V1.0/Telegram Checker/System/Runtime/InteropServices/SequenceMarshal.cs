using System;
using System.Buffers;

namespace System.Runtime.InteropServices
{
	// Token: 0x02000037 RID: 55
	[ComVisible(true)]
	public static class SequenceMarshal
	{
		// Token: 0x06000269 RID: 617 RVA: 0x00010974 File Offset: 0x0000EB74
		public static bool TryGetReadOnlySequenceSegment<T>(ReadOnlySequence<T> sequence, out ReadOnlySequenceSegment<T> startSegment, out int startIndex, out ReadOnlySequenceSegment<T> endSegment, out int endIndex)
		{
			return sequence.TryGetReadOnlySequenceSegment(out startSegment, out startIndex, out endSegment, out endIndex);
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00010984 File Offset: 0x0000EB84
		public static bool TryGetArray<T>(ReadOnlySequence<T> sequence, out ArraySegment<T> segment)
		{
			return sequence.TryGetArray(out segment);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00010990 File Offset: 0x0000EB90
		public static bool TryGetReadOnlyMemory<T>(ReadOnlySequence<T> sequence, out ReadOnlyMemory<T> memory)
		{
			if (!sequence.IsSingleSegment)
			{
				memory = default(ReadOnlyMemory<T>);
				return false;
			}
			memory = sequence.First;
			return true;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x000109B8 File Offset: 0x0000EBB8
		internal static bool TryGetString(ReadOnlySequence<char> sequence, out string text, out int start, out int length)
		{
			return sequence.TryGetString(out text, out start, out length);
		}
	}
}
