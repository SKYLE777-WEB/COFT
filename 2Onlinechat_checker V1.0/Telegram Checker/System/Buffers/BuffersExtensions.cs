using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x0200003A RID: 58
	[ComVisible(true)]
	public static class BuffersExtensions
	{
		// Token: 0x06000282 RID: 642 RVA: 0x00010F94 File Offset: 0x0000F194
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SequencePosition? PositionOf<T>(this ReadOnlySequence<T> source, T value) where T : object, IEquatable<T>
		{
			if (!source.IsSingleSegment)
			{
				return BuffersExtensions.PositionOfMultiSegment<T>(in source, value);
			}
			int num = source.First.Span.IndexOf(value);
			if (num != -1)
			{
				return new SequencePosition?(source.GetPosition((long)num));
			}
			return null;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00010FEC File Offset: 0x0000F1EC
		private static SequencePosition? PositionOfMultiSegment<T>(in ReadOnlySequence<T> source, T value) where T : object, IEquatable<T>
		{
			SequencePosition start = source.Start;
			SequencePosition sequencePosition = start;
			ReadOnlyMemory<T> readOnlyMemory;
			while (source.TryGet(ref start, out readOnlyMemory, true))
			{
				int num = readOnlyMemory.Span.IndexOf(value);
				if (num != -1)
				{
					return new SequencePosition?(source.GetPosition((long)num, sequencePosition));
				}
				if (start.GetObject() == null)
				{
					break;
				}
				sequencePosition = start;
			}
			return null;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00011054 File Offset: 0x0000F254
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this ReadOnlySequence<T> source, Span<T> destination)
		{
			if (source.Length > (long)destination.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.destination);
			}
			if (source.IsSingleSegment)
			{
				source.First.Span.CopyTo(destination);
				return;
			}
			BuffersExtensions.CopyToMultiSegment<T>(in source, destination);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000110A8 File Offset: 0x0000F2A8
		private static void CopyToMultiSegment<T>(in ReadOnlySequence<T> sequence, Span<T> destination)
		{
			SequencePosition start = sequence.Start;
			ReadOnlyMemory<T> readOnlyMemory;
			while (sequence.TryGet(ref start, out readOnlyMemory, true))
			{
				ReadOnlySpan<T> span = readOnlyMemory.Span;
				span.CopyTo(destination);
				if (start.GetObject() == null)
				{
					break;
				}
				destination = destination.Slice(span.Length);
			}
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00011100 File Offset: 0x0000F300
		public static T[] ToArray<T>(this ReadOnlySequence<T> sequence)
		{
			T[] array = new T[sequence.Length];
			(in sequence).CopyTo(array);
			return array;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0001112C File Offset: 0x0000F32C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<T>(this IBufferWriter<T> writer, ReadOnlySpan<T> value)
		{
			Span<T> span = writer.GetSpan(0);
			if (value.Length <= span.Length)
			{
				value.CopyTo(span);
				writer.Advance(value.Length);
				return;
			}
			BuffersExtensions.WriteMultiSegment<T>(writer, in value, span);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00011178 File Offset: 0x0000F378
		private static void WriteMultiSegment<T>(IBufferWriter<T> writer, in ReadOnlySpan<T> source, Span<T> destination)
		{
			ReadOnlySpan<T> readOnlySpan = source;
			for (;;)
			{
				int num = Math.Min(destination.Length, readOnlySpan.Length);
				readOnlySpan.Slice(0, num).CopyTo(destination);
				writer.Advance(num);
				readOnlySpan = readOnlySpan.Slice(num);
				if (readOnlySpan.Length <= 0)
				{
					break;
				}
				destination = writer.GetSpan(readOnlySpan.Length);
			}
		}
	}
}
