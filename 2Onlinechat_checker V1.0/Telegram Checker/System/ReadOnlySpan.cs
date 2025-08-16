using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x0200002D RID: 45
	[DebuggerTypeProxy(typeof(SpanDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	[DebuggerTypeProxy(typeof(SpanDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	[ComVisible(true)]
	public readonly ref struct ReadOnlySpan<T>
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000B924 File Offset: 0x00009B24
		public int Length
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001DB RID: 475 RVA: 0x0000B92C File Offset: 0x00009B2C
		public bool IsEmpty
		{
			get
			{
				return this._length == 0;
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000B938 File Offset: 0x00009B38
		public static bool operator !=(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
		{
			return !(left == right);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000B944 File Offset: 0x00009B44
		[Obsolete("Equals() on ReadOnlySpan will always throw an exception. Use == instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException(SR.NotSupported_CannotCallEqualsOnSpan);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000B950 File Offset: 0x00009B50
		[Obsolete("GetHashCode() on ReadOnlySpan will always throw an exception.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			throw new NotSupportedException(SR.NotSupported_CannotCallGetHashCodeOnSpan);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000B95C File Offset: 0x00009B5C
		public static implicit operator ReadOnlySpan<T>(T[] array)
		{
			return new ReadOnlySpan<T>(array);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000B964 File Offset: 0x00009B64
		public static implicit operator ReadOnlySpan<T>(ArraySegment<T> segment)
		{
			return new ReadOnlySpan<T>(segment.Array, segment.Offset, segment.Count);
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x0000B990 File Offset: 0x00009B90
		public static ReadOnlySpan<T> Empty
		{
			get
			{
				return default(ReadOnlySpan<T>);
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000B9AC File Offset: 0x00009BAC
		public ReadOnlySpan<T>.Enumerator GetEnumerator()
		{
			return new ReadOnlySpan<T>.Enumerator(this);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000B9BC File Offset: 0x00009BBC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan(T[] array)
		{
			if (array == null)
			{
				this = default(ReadOnlySpan<T>);
				return;
			}
			this._length = array.Length;
			this._pinnable = Unsafe.As<Pinnable<T>>(array);
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000B9EC File Offset: 0x00009BEC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan(T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				this = default(ReadOnlySpan<T>);
				return;
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._length = length;
			this._pinnable = Unsafe.As<Pinnable<T>>(array);
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.Add(start);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000BA5C File Offset: 0x00009C5C
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ReadOnlySpan(void* pointer, int length)
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if (length < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._length = length;
			this._pinnable = null;
			this._byteOffset = new IntPtr(pointer);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000BAB0 File Offset: 0x00009CB0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ReadOnlySpan(Pinnable<T> pinnable, IntPtr byteOffset, int length)
		{
			this._length = length;
			this._pinnable = pinnable;
			this._byteOffset = byteOffset;
		}

		// Token: 0x1700004C RID: 76
		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= this._length)
				{
					ThrowHelper.ThrowIndexOutOfRangeException();
				}
				if (this._pinnable == null)
				{
					return Unsafe.Add<T>(Unsafe.AsRef<T>(this._byteOffset.ToPointer()), index);
				}
				return Unsafe.Add<T>(Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset), index);
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000BB2C File Offset: 0x00009D2C
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly ref T GetPinnableReference()
		{
			if (this._length == 0)
			{
				return Unsafe.AsRef<T>(null);
			}
			if (this._pinnable == null)
			{
				return Unsafe.AsRef<T>(this._byteOffset.ToPointer());
			}
			return Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000BB88 File Offset: 0x00009D88
		public void CopyTo(Span<T> destination)
		{
			if (!this.TryCopyTo(destination))
			{
				ThrowHelper.ThrowArgumentException_DestinationTooShort();
			}
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000BB9C File Offset: 0x00009D9C
		public bool TryCopyTo(Span<T> destination)
		{
			int length = this._length;
			int length2 = destination.Length;
			if (length == 0)
			{
				return true;
			}
			if (length > length2)
			{
				return false;
			}
			ref T ptr = ref this.DangerousGetPinnableReference();
			ref T ptr2 = ref destination.DangerousGetPinnableReference();
			SpanHelpers.CopyTo<T>(ref ptr2, length2, ref ptr, length);
			return true;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000BBE8 File Offset: 0x00009DE8
		public static bool operator ==(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
		{
			return left._length == right._length && Unsafe.AreSame<T>(left.DangerousGetPinnableReference(), right.DangerousGetPinnableReference());
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000BC10 File Offset: 0x00009E10
		public unsafe override string ToString()
		{
			if (typeof(T) == typeof(char))
			{
				if (this._byteOffset == MemoryExtensions.StringAdjustment)
				{
					object obj = Unsafe.As<object>(this._pinnable);
					string text;
					if ((text = obj as string) != null && this._length == text.Length)
					{
						return text;
					}
				}
				fixed (char* ptr = Unsafe.As<T, char>(this.DangerousGetPinnableReference()))
				{
					char* ptr2 = ptr;
					return new string(ptr2, 0, this._length);
				}
			}
			return string.Format("System.ReadOnlySpan<{0}>[{1}]", typeof(T).Name, this._length);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000BCC4 File Offset: 0x00009EC4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<T> Slice(int start)
		{
			if (start > this._length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add(start);
			int num = this._length - start;
			return new ReadOnlySpan<T>(this._pinnable, intPtr, num);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000BD0C File Offset: 0x00009F0C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<T> Slice(int start, int length)
		{
			if (start > this._length || length > this._length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add(start);
			return new ReadOnlySpan<T>(this._pinnable, intPtr, length);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000BD58 File Offset: 0x00009F58
		public T[] ToArray()
		{
			if (this._length == 0)
			{
				return SpanHelpers.PerTypeValues<T>.EmptyArray;
			}
			T[] array = new T[this._length];
			this.CopyTo(array);
			return array;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000BD94 File Offset: 0x00009F94
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref T DangerousGetPinnableReference()
		{
			if (this._pinnable == null)
			{
				return Unsafe.AsRef<T>(this._byteOffset.ToPointer());
			}
			return Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset);
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000BDDC File Offset: 0x00009FDC
		internal Pinnable<T> Pinnable
		{
			get
			{
				return this._pinnable;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000BDE4 File Offset: 0x00009FE4
		internal IntPtr ByteOffset
		{
			get
			{
				return this._byteOffset;
			}
		}

		// Token: 0x0400010F RID: 271
		private readonly Pinnable<T> _pinnable;

		// Token: 0x04000110 RID: 272
		private readonly IntPtr _byteOffset;

		// Token: 0x04000111 RID: 273
		private readonly int _length;

		// Token: 0x020001BA RID: 442
		public ref struct Enumerator
		{
			// Token: 0x06001541 RID: 5441 RVA: 0x0007575C File Offset: 0x0007395C
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Enumerator(ReadOnlySpan<T> span)
			{
				this._span = span;
				this._index = -1;
			}

			// Token: 0x06001542 RID: 5442 RVA: 0x0007576C File Offset: 0x0007396C
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this._index + 1;
				if (num < this._span.Length)
				{
					this._index = num;
					return true;
				}
				return false;
			}

			// Token: 0x170003D7 RID: 983
			// (get) Token: 0x06001543 RID: 5443 RVA: 0x000757A4 File Offset: 0x000739A4
			public readonly ref T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this._span[this._index];
				}
			}

			// Token: 0x040007F2 RID: 2034
			private readonly ReadOnlySpan<T> _span;

			// Token: 0x040007F3 RID: 2035
			private int _index;
		}
	}
}
