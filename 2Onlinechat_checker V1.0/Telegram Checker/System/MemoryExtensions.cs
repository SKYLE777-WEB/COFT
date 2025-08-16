using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x0200002B RID: 43
	[ComVisible(true)]
	public static class MemoryExtensions
	{
		// Token: 0x06000170 RID: 368 RVA: 0x00009610 File Offset: 0x00007810
		public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span)
		{
			return span.TrimStart().TrimEnd();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00009620 File Offset: 0x00007820
		public unsafe static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span)
		{
			int num = 0;
			while (num < span.Length && char.IsWhiteSpace((char)(*span[num])))
			{
				num++;
			}
			return span.Slice(num);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009664 File Offset: 0x00007864
		public unsafe static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span)
		{
			int num = span.Length - 1;
			while (num >= 0 && char.IsWhiteSpace((char)(*span[num])))
			{
				num--;
			}
			return span.Slice(0, num + 1);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000096AC File Offset: 0x000078AC
		public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, char trimChar)
		{
			return span.TrimStart(trimChar).TrimEnd(trimChar);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x000096BC File Offset: 0x000078BC
		public unsafe static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, char trimChar)
		{
			int num = 0;
			while (num < span.Length && *span[num] == (ushort)trimChar)
			{
				num++;
			}
			return span.Slice(num);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x000096FC File Offset: 0x000078FC
		public unsafe static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, char trimChar)
		{
			int num = span.Length - 1;
			while (num >= 0 && *span[num] == (ushort)trimChar)
			{
				num--;
			}
			return span.Slice(0, num + 1);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00009740 File Offset: 0x00007940
		public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
		{
			return span.TrimStart(trimChars).TrimEnd(trimChars);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00009750 File Offset: 0x00007950
		public unsafe static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
		{
			if (trimChars.IsEmpty)
			{
				return span.TrimStart();
			}
			int i = 0;
			IL_004F:
			while (i < span.Length)
			{
				for (int j = 0; j < trimChars.Length; j++)
				{
					if (*span[i] == *trimChars[j])
					{
						i++;
						goto IL_004F;
					}
				}
				break;
			}
			return span.Slice(i);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000097C4 File Offset: 0x000079C4
		public unsafe static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
		{
			if (trimChars.IsEmpty)
			{
				return span.TrimEnd();
			}
			int i = span.Length - 1;
			IL_0057:
			while (i >= 0)
			{
				for (int j = 0; j < trimChars.Length; j++)
				{
					if (*span[i] == *trimChars[j])
					{
						i--;
						goto IL_0057;
					}
				}
				break;
			}
			return span.Slice(0, i + 1);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000983C File Offset: 0x00007A3C
		public unsafe static bool IsWhiteSpace(this ReadOnlySpan<char> span)
		{
			for (int i = 0; i < span.Length; i++)
			{
				if (!char.IsWhiteSpace((char)(*span[i])))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00009878 File Offset: 0x00007A78
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int IndexOf<T>(this Span<T> span, T value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value), span.Length);
			}
			if (typeof(T) == typeof(char))
			{
				return SpanHelpers.IndexOf(Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, char>(ref value), span.Length);
			}
			return SpanHelpers.IndexOf<T>(MemoryMarshal.GetReference<T>(span), value, span.Length);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00009918 File Offset: 0x00007B18
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), value.Length);
			}
			return SpanHelpers.IndexOf<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(value), value.Length);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00009990 File Offset: 0x00007B90
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LastIndexOf<T>(this Span<T> span, T value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value), span.Length);
			}
			if (typeof(T) == typeof(char))
			{
				return SpanHelpers.LastIndexOf(Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, char>(ref value), span.Length);
			}
			return SpanHelpers.LastIndexOf<T>(MemoryMarshal.GetReference<T>(span), value, span.Length);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00009A30 File Offset: 0x00007C30
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this Span<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), value.Length);
			}
			return SpanHelpers.LastIndexOf<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(value), value.Length);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00009AA8 File Offset: 0x00007CA8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other) where T : object, IEquatable<T>
		{
			int length = span.Length;
			NUInt nuint;
			if (default(T) != null && MemoryExtensions.IsTypeComparableAsBytes<T>(out nuint))
			{
				return length == other.Length && SpanHelpers.SequenceEqual(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(other)), (NUInt)length * nuint);
			}
			return length == other.Length && SpanHelpers.SequenceEqual<T>(MemoryMarshal.GetReference<T>(span), MemoryMarshal.GetReference<T>(other), length);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009B38 File Offset: 0x00007D38
		public static int SequenceCompareTo<T>(this Span<T> span, ReadOnlySpan<T> other) where T : object, IComparable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.SequenceCompareTo(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(other)), other.Length);
			}
			if (typeof(T) == typeof(char))
			{
				return SpanHelpers.SequenceCompareTo(Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(other)), other.Length);
			}
			return SpanHelpers.SequenceCompareTo<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(other), other.Length);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00009BF8 File Offset: 0x00007DF8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int IndexOf<T>(this ReadOnlySpan<T> span, T value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value), span.Length);
			}
			if (typeof(T) == typeof(char))
			{
				return SpanHelpers.IndexOf(Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, char>(ref value), span.Length);
			}
			return SpanHelpers.IndexOf<T>(MemoryMarshal.GetReference<T>(span), value, span.Length);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00009C98 File Offset: 0x00007E98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), value.Length);
			}
			return SpanHelpers.IndexOf<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(value), value.Length);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00009D10 File Offset: 0x00007F10
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value), span.Length);
			}
			if (typeof(T) == typeof(char))
			{
				return SpanHelpers.LastIndexOf(Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, char>(ref value), span.Length);
			}
			return SpanHelpers.LastIndexOf<T>(MemoryMarshal.GetReference<T>(span), value, span.Length);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00009DB0 File Offset: 0x00007FB0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOf(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), value.Length);
			}
			return SpanHelpers.LastIndexOf<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(value), value.Length);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00009E28 File Offset: 0x00008028
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int IndexOfAny<T>(this Span<T> span, T value0, T value1) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), span.Length);
			}
			return SpanHelpers.IndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, span.Length);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00009E94 File Offset: 0x00008094
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int IndexOfAny<T>(this Span<T> span, T value0, T value1, T value2) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), *Unsafe.As<T, byte>(ref value2), span.Length);
			}
			return SpanHelpers.IndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, value2, span.Length);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00009F08 File Offset: 0x00008108
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny<T>(this Span<T> span, ReadOnlySpan<T> values) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(values)), values.Length);
			}
			return SpanHelpers.IndexOfAny<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(values), values.Length);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00009F80 File Offset: 0x00008180
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), span.Length);
			}
			return SpanHelpers.IndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, span.Length);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00009FEC File Offset: 0x000081EC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), *Unsafe.As<T, byte>(ref value2), span.Length);
			}
			return SpanHelpers.IndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, value2, span.Length);
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000A060 File Offset: 0x00008260
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.IndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(values)), values.Length);
			}
			return SpanHelpers.IndexOfAny<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(values), values.Length);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000A0D8 File Offset: 0x000082D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LastIndexOfAny<T>(this Span<T> span, T value0, T value1) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), span.Length);
			}
			return SpanHelpers.LastIndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, span.Length);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000A144 File Offset: 0x00008344
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LastIndexOfAny<T>(this Span<T> span, T value0, T value1, T value2) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), *Unsafe.As<T, byte>(ref value2), span.Length);
			}
			return SpanHelpers.LastIndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, value2, span.Length);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000A1B8 File Offset: 0x000083B8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOfAny<T>(this Span<T> span, ReadOnlySpan<T> values) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(values)), values.Length);
			}
			return SpanHelpers.LastIndexOfAny<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(values), values.Length);
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000A230 File Offset: 0x00008430
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), span.Length);
			}
			return SpanHelpers.LastIndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, span.Length);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000A29C File Offset: 0x0000849C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), *Unsafe.As<T, byte>(ref value0), *Unsafe.As<T, byte>(ref value1), *Unsafe.As<T, byte>(ref value2), span.Length);
			}
			return SpanHelpers.LastIndexOfAny<T>(MemoryMarshal.GetReference<T>(span), value0, value1, value2, span.Length);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000A310 File Offset: 0x00008510
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : object, IEquatable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.LastIndexOfAny(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(values)), values.Length);
			}
			return SpanHelpers.LastIndexOfAny<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(values), values.Length);
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000A388 File Offset: 0x00008588
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other) where T : object, IEquatable<T>
		{
			int length = span.Length;
			NUInt nuint;
			if (default(T) != null && MemoryExtensions.IsTypeComparableAsBytes<T>(out nuint))
			{
				return length == other.Length && SpanHelpers.SequenceEqual(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(other)), (NUInt)length * nuint);
			}
			return length == other.Length && SpanHelpers.SequenceEqual<T>(MemoryMarshal.GetReference<T>(span), MemoryMarshal.GetReference<T>(other), length);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000A418 File Offset: 0x00008618
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SequenceCompareTo<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other) where T : object, IComparable<T>
		{
			if (typeof(T) == typeof(byte))
			{
				return SpanHelpers.SequenceCompareTo(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(other)), other.Length);
			}
			if (typeof(T) == typeof(char))
			{
				return SpanHelpers.SequenceCompareTo(Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(span)), span.Length, Unsafe.As<T, char>(MemoryMarshal.GetReference<T>(other)), other.Length);
			}
			return SpanHelpers.SequenceCompareTo<T>(MemoryMarshal.GetReference<T>(span), span.Length, MemoryMarshal.GetReference<T>(other), other.Length);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000A4D8 File Offset: 0x000086D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			int length = value.Length;
			NUInt nuint;
			if (default(T) != null && MemoryExtensions.IsTypeComparableAsBytes<T>(out nuint))
			{
				return length <= span.Length && SpanHelpers.SequenceEqual(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), (NUInt)length * nuint);
			}
			return length <= span.Length && SpanHelpers.SequenceEqual<T>(MemoryMarshal.GetReference<T>(span), MemoryMarshal.GetReference<T>(value), length);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000A568 File Offset: 0x00008768
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			int length = value.Length;
			NUInt nuint;
			if (default(T) != null && MemoryExtensions.IsTypeComparableAsBytes<T>(out nuint))
			{
				return length <= span.Length && SpanHelpers.SequenceEqual(Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(span)), Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), (NUInt)length * nuint);
			}
			return length <= span.Length && SpanHelpers.SequenceEqual<T>(MemoryMarshal.GetReference<T>(span), MemoryMarshal.GetReference<T>(value), length);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000A5F8 File Offset: 0x000087F8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EndsWith<T>(this Span<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			int length = span.Length;
			int length2 = value.Length;
			NUInt nuint;
			if (default(T) != null && MemoryExtensions.IsTypeComparableAsBytes<T>(out nuint))
			{
				return length2 <= length && SpanHelpers.SequenceEqual(Unsafe.As<T, byte>(Unsafe.Add<T>(MemoryMarshal.GetReference<T>(span), length - length2)), Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), (NUInt)length2 * nuint);
			}
			return length2 <= length && SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(MemoryMarshal.GetReference<T>(span), length - length2), MemoryMarshal.GetReference<T>(value), length2);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000A694 File Offset: 0x00008894
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EndsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : object, IEquatable<T>
		{
			int length = span.Length;
			int length2 = value.Length;
			NUInt nuint;
			if (default(T) != null && MemoryExtensions.IsTypeComparableAsBytes<T>(out nuint))
			{
				return length2 <= length && SpanHelpers.SequenceEqual(Unsafe.As<T, byte>(Unsafe.Add<T>(MemoryMarshal.GetReference<T>(span), length - length2)), Unsafe.As<T, byte>(MemoryMarshal.GetReference<T>(value)), (NUInt)length2 * nuint);
			}
			return length2 <= length && SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(MemoryMarshal.GetReference<T>(span), length - length2), MemoryMarshal.GetReference<T>(value), length2);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000A730 File Offset: 0x00008930
		public unsafe static void Reverse<T>(this Span<T> span)
		{
			ref T reference = ref MemoryMarshal.GetReference<T>(span);
			int i = 0;
			int num = span.Length - 1;
			while (i < num)
			{
				T t = *Unsafe.Add<T>(ref reference, i);
				*Unsafe.Add<T>(ref reference, i) = *Unsafe.Add<T>(ref reference, num);
				*Unsafe.Add<T>(ref reference, num) = t;
				i++;
				num--;
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000A798 File Offset: 0x00008998
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this T[] array)
		{
			return new Span<T>(array);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000A7A0 File Offset: 0x000089A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this T[] array, int start, int length)
		{
			return new Span<T>(array, start, length);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000A7AC File Offset: 0x000089AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this ArraySegment<T> segment)
		{
			return new Span<T>(segment.Array, segment.Offset, segment.Count);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000A7D8 File Offset: 0x000089D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this ArraySegment<T> segment, int start)
		{
			if ((ulong)start > (ulong)((long)segment.Count))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new Span<T>(segment.Array, segment.Offset + start, segment.Count - start);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000A81C File Offset: 0x00008A1C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this ArraySegment<T> segment, int start, int length)
		{
			if ((ulong)start > (ulong)((long)segment.Count))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			if ((ulong)length > (ulong)((long)(segment.Count - start)))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
			}
			return new Span<T>(segment.Array, segment.Offset + start, length);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000A870 File Offset: 0x00008A70
		public static Memory<T> AsMemory<T>(this T[] array)
		{
			return new Memory<T>(array);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000A878 File Offset: 0x00008A78
		public static Memory<T> AsMemory<T>(this T[] array, int start)
		{
			return new Memory<T>(array, start);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000A884 File Offset: 0x00008A84
		public static Memory<T> AsMemory<T>(this T[] array, int start, int length)
		{
			return new Memory<T>(array, start, length);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000A890 File Offset: 0x00008A90
		public static Memory<T> AsMemory<T>(this ArraySegment<T> segment)
		{
			return new Memory<T>(segment.Array, segment.Offset, segment.Count);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000A8BC File Offset: 0x00008ABC
		public static Memory<T> AsMemory<T>(this ArraySegment<T> segment, int start)
		{
			if ((ulong)start > (ulong)((long)segment.Count))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new Memory<T>(segment.Array, segment.Offset + start, segment.Count - start);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000A900 File Offset: 0x00008B00
		public static Memory<T> AsMemory<T>(this ArraySegment<T> segment, int start, int length)
		{
			if ((ulong)start > (ulong)((long)segment.Count))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			if ((ulong)length > (ulong)((long)(segment.Count - start)))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
			}
			return new Memory<T>(segment.Array, segment.Offset + start, length);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000A954 File Offset: 0x00008B54
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this T[] source, Span<T> destination)
		{
			new ReadOnlySpan<T>(source).CopyTo(destination);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000A974 File Offset: 0x00008B74
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this T[] source, Memory<T> destination)
		{
			source.CopyTo(destination.Span);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000A984 File Offset: 0x00008B84
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps<T>(this Span<T> span, ReadOnlySpan<T> other)
		{
			return span.Overlaps(other);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000A994 File Offset: 0x00008B94
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps<T>(this Span<T> span, ReadOnlySpan<T> other, out int elementOffset)
		{
			return span.Overlaps(other, out elementOffset);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000A9A4 File Offset: 0x00008BA4
		public static bool Overlaps<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
		{
			if (span.IsEmpty || other.IsEmpty)
			{
				return false;
			}
			IntPtr intPtr = Unsafe.ByteOffset<T>(MemoryMarshal.GetReference<T>(span), MemoryMarshal.GetReference<T>(other));
			if (Unsafe.SizeOf<IntPtr>() == 4)
			{
				return (int)intPtr < span.Length * Unsafe.SizeOf<T>() || (int)intPtr > -(other.Length * Unsafe.SizeOf<T>());
			}
			return (long)intPtr < (long)span.Length * (long)Unsafe.SizeOf<T>() || (long)intPtr > -((long)other.Length * (long)Unsafe.SizeOf<T>());
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000AA54 File Offset: 0x00008C54
		public static bool Overlaps<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, out int elementOffset)
		{
			if (span.IsEmpty || other.IsEmpty)
			{
				elementOffset = 0;
				return false;
			}
			IntPtr intPtr = Unsafe.ByteOffset<T>(MemoryMarshal.GetReference<T>(span), MemoryMarshal.GetReference<T>(other));
			if (Unsafe.SizeOf<IntPtr>() == 4)
			{
				if ((int)intPtr < span.Length * Unsafe.SizeOf<T>() || (int)intPtr > -(other.Length * Unsafe.SizeOf<T>()))
				{
					if ((int)intPtr % Unsafe.SizeOf<T>() != 0)
					{
						ThrowHelper.ThrowArgumentException_OverlapAlignmentMismatch();
					}
					elementOffset = (int)intPtr / Unsafe.SizeOf<T>();
					return true;
				}
				elementOffset = 0;
				return false;
			}
			else
			{
				if ((long)intPtr < (long)span.Length * (long)Unsafe.SizeOf<T>() || (long)intPtr > -((long)other.Length * (long)Unsafe.SizeOf<T>()))
				{
					if ((long)intPtr % (long)Unsafe.SizeOf<T>() != 0L)
					{
						ThrowHelper.ThrowArgumentException_OverlapAlignmentMismatch();
					}
					elementOffset = (int)((long)intPtr / (long)Unsafe.SizeOf<T>());
					return true;
				}
				elementOffset = 0;
				return false;
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000AB60 File Offset: 0x00008D60
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T>(this Span<T> span, IComparable<T> comparable)
		{
			return span.BinarySearch(comparable);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000AB6C File Offset: 0x00008D6C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TComparable>(this Span<T> span, TComparable comparable) where TComparable : object, IComparable<T>
		{
			return span.BinarySearch(comparable);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000AB7C File Offset: 0x00008D7C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TComparer>(this Span<T> span, T value, TComparer comparer) where TComparer : object, IComparer<T>
		{
			return span.BinarySearch(value, comparer);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000AB8C File Offset: 0x00008D8C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T>(this ReadOnlySpan<T> span, IComparable<T> comparable)
		{
			return span.BinarySearch(comparable);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000AB98 File Offset: 0x00008D98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TComparable>(this ReadOnlySpan<T> span, TComparable comparable) where TComparable : object, IComparable<T>
		{
			return span.BinarySearch(comparable);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000ABA4 File Offset: 0x00008DA4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TComparer>(this ReadOnlySpan<T> span, T value, TComparer comparer) where TComparer : object, IComparer<T>
		{
			if (comparer == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparer);
			}
			SpanHelpers.ComparerComparable<T, TComparer> comparerComparable = new SpanHelpers.ComparerComparable<T, TComparer>(value, comparer);
			return span.BinarySearch(comparerComparable);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000ABD8 File Offset: 0x00008DD8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsTypeComparableAsBytes<T>(out NUInt size)
		{
			if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
			{
				size = (NUInt)1;
				return true;
			}
			if (typeof(T) == typeof(char) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
			{
				size = (NUInt)2;
				return true;
			}
			if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
			{
				size = (NUInt)4;
				return true;
			}
			if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
			{
				size = (NUInt)8;
				return true;
			}
			size = default(NUInt);
			return false;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000AD38 File Offset: 0x00008F38
		public static Span<T> AsSpan<T>(this T[] array, int start)
		{
			return Span<T>.Create(array, start);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000AD44 File Offset: 0x00008F44
		public static bool Contains(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
		{
			return span.IndexOf(value, comparisonType) >= 0;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000AD54 File Offset: 0x00008F54
		public static bool Equals(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, StringComparison comparisonType)
		{
			if (comparisonType == StringComparison.Ordinal)
			{
				return span.SequenceEqual(other);
			}
			if (comparisonType == StringComparison.OrdinalIgnoreCase)
			{
				return span.Length == other.Length && MemoryExtensions.EqualsOrdinalIgnoreCase(span, other);
			}
			return span.ToString().Equals(other.ToString(), comparisonType);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000ADB8 File Offset: 0x00008FB8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool EqualsOrdinalIgnoreCase(ReadOnlySpan<char> span, ReadOnlySpan<char> other)
		{
			return other.Length == 0 || MemoryExtensions.CompareToOrdinalIgnoreCase(span, other) == 0;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000ADD4 File Offset: 0x00008FD4
		public static int CompareTo(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, StringComparison comparisonType)
		{
			if (comparisonType == StringComparison.Ordinal)
			{
				return span.SequenceCompareTo(other);
			}
			if (comparisonType == StringComparison.OrdinalIgnoreCase)
			{
				return MemoryExtensions.CompareToOrdinalIgnoreCase(span, other);
			}
			return string.Compare(span.ToString(), other.ToString(), comparisonType);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000AE14 File Offset: 0x00009014
		private unsafe static int CompareToOrdinalIgnoreCase(ReadOnlySpan<char> strA, ReadOnlySpan<char> strB)
		{
			int num = Math.Min(strA.Length, strB.Length);
			int num2 = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(strA))
			{
				char* ptr = reference;
				fixed (char* reference2 = MemoryMarshal.GetReference<char>(strB))
				{
					char* ptr2 = reference2;
					char* ptr3 = ptr;
					char* ptr4 = ptr2;
					while (num != 0 && *ptr3 <= '\u007f' && *ptr4 <= '\u007f')
					{
						int num3 = (int)(*ptr3);
						int num4 = (int)(*ptr4);
						if (num3 == num4)
						{
							ptr3++;
							ptr4++;
							num--;
						}
						else
						{
							if (num3 - 97 <= 25)
							{
								num3 -= 32;
							}
							if (num4 - 97 <= 25)
							{
								num4 -= 32;
							}
							if (num3 != num4)
							{
								return num3 - num4;
							}
							ptr3++;
							ptr4++;
							num--;
						}
					}
					if (num == 0)
					{
						return strA.Length - strB.Length;
					}
					num2 -= num;
					return string.Compare(strA.Slice(num2).ToString(), strB.Slice(num2).ToString(), StringComparison.OrdinalIgnoreCase);
				}
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000AF34 File Offset: 0x00009134
		public static int IndexOf(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
		{
			if (comparisonType == StringComparison.Ordinal)
			{
				return span.IndexOf(value);
			}
			return span.ToString().IndexOf(value.ToString(), comparisonType);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000AF68 File Offset: 0x00009168
		public static int ToLower(this ReadOnlySpan<char> source, Span<char> destination, CultureInfo culture)
		{
			if (culture == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.culture);
			}
			if (destination.Length < source.Length)
			{
				return -1;
			}
			string text = source.ToString();
			string text2 = text.ToLower(culture);
			text2.AsSpan().CopyTo(destination);
			return source.Length;
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000AFC8 File Offset: 0x000091C8
		public static int ToLowerInvariant(this ReadOnlySpan<char> source, Span<char> destination)
		{
			return source.ToLower(destination, CultureInfo.InvariantCulture);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000AFD8 File Offset: 0x000091D8
		public static int ToUpper(this ReadOnlySpan<char> source, Span<char> destination, CultureInfo culture)
		{
			if (culture == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.culture);
			}
			if (destination.Length < source.Length)
			{
				return -1;
			}
			string text = source.ToString();
			string text2 = text.ToUpper(culture);
			text2.AsSpan().CopyTo(destination);
			return source.Length;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000B038 File Offset: 0x00009238
		public static int ToUpperInvariant(this ReadOnlySpan<char> source, Span<char> destination)
		{
			return source.ToUpper(destination, CultureInfo.InvariantCulture);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000B048 File Offset: 0x00009248
		public static bool EndsWith(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
		{
			if (comparisonType == StringComparison.Ordinal)
			{
				return span.EndsWith(value);
			}
			if (comparisonType == StringComparison.OrdinalIgnoreCase)
			{
				return value.Length <= span.Length && MemoryExtensions.EqualsOrdinalIgnoreCase(span.Slice(span.Length - value.Length), value);
			}
			string text = span.ToString();
			string text2 = value.ToString();
			return text.EndsWith(text2, comparisonType);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000B0C8 File Offset: 0x000092C8
		public static bool StartsWith(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
		{
			if (comparisonType == StringComparison.Ordinal)
			{
				return span.StartsWith(value);
			}
			if (comparisonType == StringComparison.OrdinalIgnoreCase)
			{
				return value.Length <= span.Length && MemoryExtensions.EqualsOrdinalIgnoreCase(span.Slice(0, value.Length), value);
			}
			string text = span.ToString();
			string text2 = value.ToString();
			return text.StartsWith(text2, comparisonType);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000B140 File Offset: 0x00009340
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<char> AsSpan(this string text)
		{
			if (text == null)
			{
				return default(ReadOnlySpan<char>);
			}
			return new ReadOnlySpan<char>(Unsafe.As<Pinnable<char>>(text), MemoryExtensions.StringAdjustment, text.Length);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000B178 File Offset: 0x00009378
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<char> AsSpan(this string text, int start)
		{
			if (text == null)
			{
				if (start != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				return default(ReadOnlySpan<char>);
			}
			if (start > text.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new ReadOnlySpan<char>(Unsafe.As<Pinnable<char>>(text), MemoryExtensions.StringAdjustment + start * 2, text.Length - start);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000B1D8 File Offset: 0x000093D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<char> AsSpan(this string text, int start, int length)
		{
			if (text == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				return default(ReadOnlySpan<char>);
			}
			if (start > text.Length || length > text.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new ReadOnlySpan<char>(Unsafe.As<Pinnable<char>>(text), MemoryExtensions.StringAdjustment + start * 2, length);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000B244 File Offset: 0x00009444
		public static ReadOnlyMemory<char> AsMemory(this string text)
		{
			if (text == null)
			{
				return default(ReadOnlyMemory<char>);
			}
			return new ReadOnlyMemory<char>(text, 0, text.Length);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000B274 File Offset: 0x00009474
		public static ReadOnlyMemory<char> AsMemory(this string text, int start)
		{
			if (text == null)
			{
				if (start != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				return default(ReadOnlyMemory<char>);
			}
			if (start > text.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new ReadOnlyMemory<char>(text, start, text.Length - start);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000B2C4 File Offset: 0x000094C4
		public static ReadOnlyMemory<char> AsMemory(this string text, int start, int length)
		{
			if (text == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				return default(ReadOnlyMemory<char>);
			}
			if (start > text.Length || length > text.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new ReadOnlyMemory<char>(text, start, length);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000B320 File Offset: 0x00009520
		private unsafe static IntPtr MeasureStringAdjustment()
		{
			string text = "a";
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				return Unsafe.ByteOffset<char>(ref Unsafe.As<Pinnable<char>>(text).Data, Unsafe.AsRef<char>((void*)ptr));
			}
		}

		// Token: 0x0400010A RID: 266
		internal static readonly IntPtr StringAdjustment = MemoryExtensions.MeasureStringAdjustment();
	}
}
