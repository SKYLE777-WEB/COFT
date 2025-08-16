using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x0200002E RID: 46
	[DebuggerTypeProxy(typeof(SpanDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	[DebuggerTypeProxy(typeof(SpanDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	[ComVisible(true)]
	public readonly ref struct Span<T>
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x0000BDEC File Offset: 0x00009FEC
		public int Length
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000BDF4 File Offset: 0x00009FF4
		public bool IsEmpty
		{
			get
			{
				return this._length == 0;
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000BE00 File Offset: 0x0000A000
		public static bool operator !=(Span<T> left, Span<T> right)
		{
			return !(left == right);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000BE0C File Offset: 0x0000A00C
		[Obsolete("Equals() on Span will always throw an exception. Use == instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException(SR.NotSupported_CannotCallEqualsOnSpan);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000BE18 File Offset: 0x0000A018
		[Obsolete("GetHashCode() on Span will always throw an exception.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			throw new NotSupportedException(SR.NotSupported_CannotCallGetHashCodeOnSpan);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000BE24 File Offset: 0x0000A024
		public static implicit operator Span<T>(T[] array)
		{
			return new Span<T>(array);
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000BE2C File Offset: 0x0000A02C
		public static implicit operator Span<T>(ArraySegment<T> segment)
		{
			return new Span<T>(segment.Array, segment.Offset, segment.Count);
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000BE58 File Offset: 0x0000A058
		public static Span<T> Empty
		{
			get
			{
				return default(Span<T>);
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000BE74 File Offset: 0x0000A074
		public Span<T>.Enumerator GetEnumerator()
		{
			return new Span<T>.Enumerator(this);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000BE84 File Offset: 0x0000A084
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span(T[] array)
		{
			if (array == null)
			{
				this = default(Span<T>);
				return;
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			this._length = array.Length;
			this._pinnable = Unsafe.As<Pinnable<T>>(array);
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000BEF8 File Offset: 0x0000A0F8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Span<T> Create(T[] array, int start)
		{
			if (array == null)
			{
				if (start != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				return default(Span<T>);
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			if (start > array.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.Add(start);
			int num = array.Length - start;
			return new Span<T>(Unsafe.As<Pinnable<T>>(array), intPtr, num);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000BF88 File Offset: 0x0000A188
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span(T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				this = default(Span<T>);
				return;
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._length = length;
			this._pinnable = Unsafe.As<Pinnable<T>>(array);
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.Add(start);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000C02C File Offset: 0x0000A22C
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Span(void* pointer, int length)
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

		// Token: 0x06000200 RID: 512 RVA: 0x0000C080 File Offset: 0x0000A280
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Span(Pinnable<T> pinnable, IntPtr byteOffset, int length)
		{
			this._length = length;
			this._pinnable = pinnable;
			this._byteOffset = byteOffset;
		}

		// Token: 0x17000052 RID: 82
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

		// Token: 0x06000202 RID: 514 RVA: 0x0000C0FC File Offset: 0x0000A2FC
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ref T GetPinnableReference()
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

		// Token: 0x06000203 RID: 515 RVA: 0x0000C158 File Offset: 0x0000A358
		public unsafe void Clear()
		{
			int length = this._length;
			if (length == 0)
			{
				return;
			}
			UIntPtr uintPtr = (UIntPtr)((ulong)length * (ulong)((long)Unsafe.SizeOf<T>()));
			if ((Unsafe.SizeOf<T>() & (sizeof(IntPtr) - 1)) != 0)
			{
				if (this._pinnable == null)
				{
					byte* ptr = (byte*)this._byteOffset.ToPointer();
					SpanHelpers.ClearLessThanPointerSized(ptr, uintPtr);
					return;
				}
				ref byte ptr2 = ref Unsafe.As<T, byte>(Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset));
				SpanHelpers.ClearLessThanPointerSized(ref ptr2, uintPtr);
				return;
			}
			else
			{
				if (SpanHelpers.IsReferenceOrContainsReferences<T>())
				{
					UIntPtr uintPtr2 = (UIntPtr)((ulong)((long)(length * Unsafe.SizeOf<T>() / sizeof(IntPtr))));
					ref IntPtr ptr3 = ref Unsafe.As<T, IntPtr>(this.DangerousGetPinnableReference());
					SpanHelpers.ClearPointerSizedWithReferences(ref ptr3, uintPtr2);
					return;
				}
				ref byte ptr4 = ref Unsafe.As<T, byte>(this.DangerousGetPinnableReference());
				SpanHelpers.ClearPointerSizedWithoutReferences(ref ptr4, uintPtr);
				return;
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000C230 File Offset: 0x0000A430
		public unsafe void Fill(T value)
		{
			int length = this._length;
			if (length == 0)
			{
				return;
			}
			if (Unsafe.SizeOf<T>() != 1)
			{
				ref T ptr = ref this.DangerousGetPinnableReference();
				int i;
				for (i = 0; i < (length & -8); i += 8)
				{
					*Unsafe.Add<T>(ref ptr, i) = value;
					*Unsafe.Add<T>(ref ptr, i + 1) = value;
					*Unsafe.Add<T>(ref ptr, i + 2) = value;
					*Unsafe.Add<T>(ref ptr, i + 3) = value;
					*Unsafe.Add<T>(ref ptr, i + 4) = value;
					*Unsafe.Add<T>(ref ptr, i + 5) = value;
					*Unsafe.Add<T>(ref ptr, i + 6) = value;
					*Unsafe.Add<T>(ref ptr, i + 7) = value;
				}
				if (i < (length & -4))
				{
					*Unsafe.Add<T>(ref ptr, i) = value;
					*Unsafe.Add<T>(ref ptr, i + 1) = value;
					*Unsafe.Add<T>(ref ptr, i + 2) = value;
					*Unsafe.Add<T>(ref ptr, i + 3) = value;
					i += 4;
				}
				while (i < length)
				{
					*Unsafe.Add<T>(ref ptr, i) = value;
					i++;
				}
				return;
			}
			byte b = *Unsafe.As<T, byte>(ref value);
			if (this._pinnable == null)
			{
				Unsafe.InitBlockUnaligned(this._byteOffset.ToPointer(), b, (uint)length);
				return;
			}
			ref byte ptr2 = ref Unsafe.As<T, byte>(Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset));
			Unsafe.InitBlockUnaligned(ref ptr2, b, (uint)length);
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000C3C4 File Offset: 0x0000A5C4
		public void CopyTo(Span<T> destination)
		{
			if (!this.TryCopyTo(destination))
			{
				ThrowHelper.ThrowArgumentException_DestinationTooShort();
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000C3D8 File Offset: 0x0000A5D8
		public bool TryCopyTo(Span<T> destination)
		{
			int length = this._length;
			int length2 = destination._length;
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

		// Token: 0x06000207 RID: 519 RVA: 0x0000C424 File Offset: 0x0000A624
		public static bool operator ==(Span<T> left, Span<T> right)
		{
			return left._length == right._length && Unsafe.AreSame<T>(left.DangerousGetPinnableReference(), right.DangerousGetPinnableReference());
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000C44C File Offset: 0x0000A64C
		public static implicit operator ReadOnlySpan<T>(Span<T> span)
		{
			return new ReadOnlySpan<T>(span._pinnable, span._byteOffset, span._length);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000C468 File Offset: 0x0000A668
		public unsafe override string ToString()
		{
			if (typeof(T) == typeof(char))
			{
				fixed (char* ptr = Unsafe.As<T, char>(this.DangerousGetPinnableReference()))
				{
					char* ptr2 = ptr;
					return new string(ptr2, 0, this._length);
				}
			}
			return string.Format("System.Span<{0}>[{1}]", typeof(T).Name, this._length);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000C4D8 File Offset: 0x0000A6D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<T> Slice(int start)
		{
			if (start > this._length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add(start);
			int num = this._length - start;
			return new Span<T>(this._pinnable, intPtr, num);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000C520 File Offset: 0x0000A720
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<T> Slice(int start, int length)
		{
			if (start > this._length || length > this._length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add(start);
			return new Span<T>(this._pinnable, intPtr, length);
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000C56C File Offset: 0x0000A76C
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

		// Token: 0x0600020D RID: 525 RVA: 0x0000C5A8 File Offset: 0x0000A7A8
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

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000C5F0 File Offset: 0x0000A7F0
		internal Pinnable<T> Pinnable
		{
			get
			{
				return this._pinnable;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0000C5F8 File Offset: 0x0000A7F8
		internal IntPtr ByteOffset
		{
			get
			{
				return this._byteOffset;
			}
		}

		// Token: 0x04000112 RID: 274
		private readonly Pinnable<T> _pinnable;

		// Token: 0x04000113 RID: 275
		private readonly IntPtr _byteOffset;

		// Token: 0x04000114 RID: 276
		private readonly int _length;

		// Token: 0x020001BB RID: 443
		public ref struct Enumerator
		{
			// Token: 0x06001544 RID: 5444 RVA: 0x000757B8 File Offset: 0x000739B8
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Enumerator(Span<T> span)
			{
				this._span = span;
				this._index = -1;
			}

			// Token: 0x06001545 RID: 5445 RVA: 0x000757C8 File Offset: 0x000739C8
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

			// Token: 0x170003D8 RID: 984
			// (get) Token: 0x06001546 RID: 5446 RVA: 0x00075800 File Offset: 0x00073A00
			public ref T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this._span[this._index];
				}
			}

			// Token: 0x040007F4 RID: 2036
			private readonly Span<T> _span;

			// Token: 0x040007F5 RID: 2037
			private int _index;
		}
	}
}
