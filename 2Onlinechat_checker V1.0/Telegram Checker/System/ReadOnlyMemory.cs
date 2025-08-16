using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x0200002C RID: 44
	[DebuggerTypeProxy(typeof(MemoryDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	[ComVisible(true)]
	public readonly struct ReadOnlyMemory<T>
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x0000B36C File Offset: 0x0000956C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory(T[] array)
		{
			if (array == null)
			{
				this = default(ReadOnlyMemory<T>);
				return;
			}
			this._object = array;
			this._index = 0;
			this._length = array.Length;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000B394 File Offset: 0x00009594
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory(T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				this = default(ReadOnlyMemory<T>);
				return;
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			this._object = array;
			this._index = start;
			this._length = length;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000B3F4 File Offset: 0x000095F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ReadOnlyMemory(object obj, int start, int length)
		{
			this._object = obj;
			this._index = start;
			this._length = length;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000B40C File Offset: 0x0000960C
		public static implicit operator ReadOnlyMemory<T>(T[] array)
		{
			return new ReadOnlyMemory<T>(array);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000B414 File Offset: 0x00009614
		public static implicit operator ReadOnlyMemory<T>(ArraySegment<T> segment)
		{
			return new ReadOnlyMemory<T>(segment.Array, segment.Offset, segment.Count);
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x0000B440 File Offset: 0x00009640
		public static ReadOnlyMemory<T> Empty
		{
			get
			{
				return default(ReadOnlyMemory<T>);
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000B45C File Offset: 0x0000965C
		public int Length
		{
			get
			{
				return this._length & int.MaxValue;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000B46C File Offset: 0x0000966C
		public bool IsEmpty
		{
			get
			{
				return (this._length & int.MaxValue) == 0;
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000B480 File Offset: 0x00009680
		public override string ToString()
		{
			if (!(typeof(T) == typeof(char)))
			{
				return string.Format("System.ReadOnlyMemory<{0}>[{1}]", typeof(T).Name, this._length & int.MaxValue);
			}
			string text;
			if ((text = this._object as string) == null)
			{
				return this.Span.ToString();
			}
			return text.Substring(this._index, this._length & int.MaxValue);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000B51C File Offset: 0x0000971C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory<T> Slice(int start)
		{
			int length = this._length;
			int num = length & int.MaxValue;
			if (start > num)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new ReadOnlyMemory<T>(this._object, this._index + start, length - start);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000B560 File Offset: 0x00009760
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory<T> Slice(int start, int length)
		{
			int length2 = this._length;
			int num = this._length & int.MaxValue;
			if (start > num || length > num - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new ReadOnlyMemory<T>(this._object, this._index + start, length | (length2 & int.MinValue));
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000B5B8 File Offset: 0x000097B8
		public ReadOnlySpan<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (this._index < 0)
				{
					return ((MemoryManager<T>)this._object).GetSpan().Slice(this._index & int.MaxValue, this._length);
				}
				string text;
				if (typeof(T) == typeof(char) && (text = this._object as string) != null)
				{
					return new ReadOnlySpan<T>(Unsafe.As<Pinnable<T>>(text), MemoryExtensions.StringAdjustment, text.Length).Slice(this._index, this._length);
				}
				if (this._object != null)
				{
					return new ReadOnlySpan<T>((T[])this._object, this._index, this._length & int.MaxValue);
				}
				return default(ReadOnlySpan<T>);
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000B698 File Offset: 0x00009898
		public void CopyTo(Memory<T> destination)
		{
			this.Span.CopyTo(destination.Span);
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000B6C0 File Offset: 0x000098C0
		public bool TryCopyTo(Memory<T> destination)
		{
			return this.Span.TryCopyTo(destination.Span);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000B6E8 File Offset: 0x000098E8
		public unsafe MemoryHandle Pin()
		{
			if (this._index < 0)
			{
				return ((MemoryManager<T>)this._object).Pin(this._index & int.MaxValue);
			}
			string text;
			if (typeof(T) == typeof(char) && (text = this._object as string) != null)
			{
				GCHandle gchandle = GCHandle.Alloc(text, GCHandleType.Pinned);
				void* ptr = Unsafe.Add<T>((void*)gchandle.AddrOfPinnedObject(), this._index);
				return new MemoryHandle(ptr, gchandle, null);
			}
			T[] array;
			if ((array = this._object as T[]) == null)
			{
				return default(MemoryHandle);
			}
			if (this._length < 0)
			{
				void* ptr2 = Unsafe.Add<T>(Unsafe.AsPointer<T>(MemoryMarshal.GetReference<T>(array)), this._index);
				return new MemoryHandle(ptr2, default(GCHandle), null);
			}
			GCHandle gchandle2 = GCHandle.Alloc(array, GCHandleType.Pinned);
			void* ptr3 = Unsafe.Add<T>((void*)gchandle2.AddrOfPinnedObject(), this._index);
			return new MemoryHandle(ptr3, gchandle2, null);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000B800 File Offset: 0x00009A00
		public T[] ToArray()
		{
			return this.Span.ToArray();
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000B820 File Offset: 0x00009A20
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			if (obj is ReadOnlyMemory<T>)
			{
				ReadOnlyMemory<T> readOnlyMemory = (ReadOnlyMemory<T>)obj;
				return this.Equals(readOnlyMemory);
			}
			if (obj is Memory<T>)
			{
				Memory<T> memory = (Memory<T>)obj;
				return this.Equals(memory);
			}
			return false;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000B870 File Offset: 0x00009A70
		public bool Equals(ReadOnlyMemory<T> other)
		{
			return this._object == other._object && this._index == other._index && this._length == other._length;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000B8A4 File Offset: 0x00009AA4
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			if (this._object == null)
			{
				return 0;
			}
			return ReadOnlyMemory<T>.CombineHashCodes(this._object.GetHashCode(), this._index.GetHashCode(), this._length.GetHashCode());
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000B8F0 File Offset: 0x00009AF0
		private static int CombineHashCodes(int left, int right)
		{
			return ((left << 5) + left) ^ right;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000B8FC File Offset: 0x00009AFC
		private static int CombineHashCodes(int h1, int h2, int h3)
		{
			return ReadOnlyMemory<T>.CombineHashCodes(ReadOnlyMemory<T>.CombineHashCodes(h1, h2), h3);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000B90C File Offset: 0x00009B0C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal object GetObjectStartLength(out int start, out int length)
		{
			start = this._index;
			length = this._length;
			return this._object;
		}

		// Token: 0x0400010B RID: 267
		private readonly object _object;

		// Token: 0x0400010C RID: 268
		private readonly int _index;

		// Token: 0x0400010D RID: 269
		private readonly int _length;

		// Token: 0x0400010E RID: 270
		internal const int RemoveFlagsBitMask = 2147483647;
	}
}
