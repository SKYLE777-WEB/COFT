using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	// Token: 0x02000038 RID: 56
	[ComVisible(true)]
	public static class MemoryMarshal
	{
		// Token: 0x0600026D RID: 621 RVA: 0x000109C4 File Offset: 0x0000EBC4
		public static bool TryGetArray<T>(ReadOnlyMemory<T> memory, out ArraySegment<T> segment)
		{
			int num;
			int num2;
			object objectStartLength = memory.GetObjectStartLength(out num, out num2);
			T[] array;
			if (num < 0)
			{
				ArraySegment<T> arraySegment;
				if (((MemoryManager<T>)objectStartLength).TryGetArray(out arraySegment))
				{
					segment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset + (num & int.MaxValue), num2);
					return true;
				}
			}
			else if ((array = objectStartLength as T[]) != null)
			{
				segment = new ArraySegment<T>(array, num, num2 & int.MaxValue);
				return true;
			}
			if ((num2 & 2147483647) == 0)
			{
				segment = new ArraySegment<T>(SpanHelpers.PerTypeValues<T>.EmptyArray);
				return true;
			}
			segment = default(ArraySegment<T>);
			return false;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00010A6C File Offset: 0x0000EC6C
		public static bool TryGetMemoryManager<T, TManager>(ReadOnlyMemory<T> memory, out TManager manager) where TManager : MemoryManager<T>
		{
			int num;
			int num2;
			manager = memory.GetObjectStartLength(out num, out num2) as TManager;
			return manager != null;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00010AA8 File Offset: 0x0000ECA8
		public static bool TryGetMemoryManager<T, TManager>(ReadOnlyMemory<T> memory, out TManager manager, out int start, out int length) where TManager : MemoryManager<T>
		{
			manager = memory.GetObjectStartLength(out start, out length) as TManager;
			start &= int.MaxValue;
			if (manager == null)
			{
				start = 0;
				length = 0;
				return false;
			}
			return true;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00010AF8 File Offset: 0x0000ECF8
		public unsafe static IEnumerable<T> ToEnumerable<T>(ReadOnlyMemory<T> memory)
		{
			int num;
			for (int i = 0; i < memory.Length; i = num + 1)
			{
				yield return *memory.Span[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00010B08 File Offset: 0x0000ED08
		public static bool TryGetString(ReadOnlyMemory<char> memory, out string text, out int start, out int length)
		{
			int num;
			int num2;
			string text2;
			if ((text2 = memory.GetObjectStartLength(out num, out num2) as string) != null)
			{
				text = text2;
				start = num;
				length = num2;
				return true;
			}
			text = null;
			start = 0;
			length = 0;
			return false;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00010B48 File Offset: 0x0000ED48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Read<T>(ReadOnlySpan<byte> source) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if (Unsafe.SizeOf<T>() > source.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
			}
			return Unsafe.ReadUnaligned<T>(MemoryMarshal.GetReference<byte>(source));
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00010B88 File Offset: 0x0000ED88
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryRead<T>(ReadOnlySpan<byte> source, out T value) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if ((long)Unsafe.SizeOf<T>() > (long)((ulong)source.Length))
			{
				value = default(T);
				return false;
			}
			value = Unsafe.ReadUnaligned<T>(MemoryMarshal.GetReference<byte>(source));
			return true;
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00010BE0 File Offset: 0x0000EDE0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<T>(Span<byte> destination, ref T value) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if (Unsafe.SizeOf<T>() > destination.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
			}
			Unsafe.WriteUnaligned<T>(MemoryMarshal.GetReference<byte>(destination), value);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00010C34 File Offset: 0x0000EE34
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWrite<T>(Span<byte> destination, ref T value) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if ((long)Unsafe.SizeOf<T>() > (long)((ulong)destination.Length))
			{
				return false;
			}
			Unsafe.WriteUnaligned<T>(MemoryMarshal.GetReference<byte>(destination), value);
			return true;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00010C88 File Offset: 0x0000EE88
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Memory<T> CreateFromPinnedArray<T>(T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				return default(Memory<T>);
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return new Memory<T>(array, start, length | int.MinValue);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00010D14 File Offset: 0x0000EF14
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<byte> AsBytes<T>(Span<T> span) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			int num = checked(span.Length * Unsafe.SizeOf<T>());
			return new Span<byte>(Unsafe.As<Pinnable<byte>>(span.Pinnable), span.ByteOffset, num);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00010D68 File Offset: 0x0000EF68
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<byte> AsBytes<T>(ReadOnlySpan<T> span) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			int num = checked(span.Length * Unsafe.SizeOf<T>());
			return new ReadOnlySpan<byte>(Unsafe.As<Pinnable<byte>>(span.Pinnable), span.ByteOffset, num);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00010DBC File Offset: 0x0000EFBC
		public unsafe static Memory<T> AsMemory<T>(ReadOnlyMemory<T> memory)
		{
			return *Unsafe.As<ReadOnlyMemory<T>, Memory<T>>(ref memory);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00010DCC File Offset: 0x0000EFCC
		public static ref T GetReference<T>(Span<T> span)
		{
			if (span.Pinnable == null)
			{
				return Unsafe.AsRef<T>(span.ByteOffset.ToPointer());
			}
			return Unsafe.AddByteOffset<T>(ref span.Pinnable.Data, span.ByteOffset);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00010E18 File Offset: 0x0000F018
		public static ref T GetReference<T>(ReadOnlySpan<T> span)
		{
			if (span.Pinnable == null)
			{
				return Unsafe.AsRef<T>(span.ByteOffset.ToPointer());
			}
			return Unsafe.AddByteOffset<T>(ref span.Pinnable.Data, span.ByteOffset);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00010E64 File Offset: 0x0000F064
		public static Span<TTo> Cast<TFrom, TTo>(Span<TFrom> span) where TFrom : struct where TTo : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<TFrom>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));
			}
			if (SpanHelpers.IsReferenceOrContainsReferences<TTo>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));
			}
			checked
			{
				int num = (int)(unchecked((long)span.Length) * unchecked((long)Unsafe.SizeOf<TFrom>()) / unchecked((long)Unsafe.SizeOf<TTo>()));
				return new Span<TTo>(Unsafe.As<Pinnable<TTo>>(span.Pinnable), span.ByteOffset, num);
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00010ED8 File Offset: 0x0000F0D8
		public static ReadOnlySpan<TTo> Cast<TFrom, TTo>(ReadOnlySpan<TFrom> span) where TFrom : struct where TTo : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<TFrom>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));
			}
			if (SpanHelpers.IsReferenceOrContainsReferences<TTo>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));
			}
			checked
			{
				int num = (int)(unchecked((long)span.Length) * unchecked((long)Unsafe.SizeOf<TFrom>()) / unchecked((long)Unsafe.SizeOf<TTo>()));
				return new ReadOnlySpan<TTo>(Unsafe.As<Pinnable<TTo>>(span.Pinnable), span.ByteOffset, num);
			}
		}
	}
}
