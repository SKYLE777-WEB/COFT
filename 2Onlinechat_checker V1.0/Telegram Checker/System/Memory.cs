using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x02000029 RID: 41
	[DebuggerTypeProxy(typeof(MemoryDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	[ComVisible(true)]
	public readonly struct Memory<T>
	{
		// Token: 0x06000154 RID: 340 RVA: 0x00008EE4 File Offset: 0x000070E4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Memory(T[] array)
		{
			if (array == null)
			{
				this = default(Memory<T>);
				return;
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			this._object = array;
			this._index = 0;
			this._length = array.Length;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00008F4C File Offset: 0x0000714C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Memory(T[] array, int start)
		{
			if (array == null)
			{
				if (start != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				this = default(Memory<T>);
				return;
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			if (start > array.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			this._object = array;
			this._index = start;
			this._length = array.Length - start;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00008FD0 File Offset: 0x000071D0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Memory(T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				this = default(Memory<T>);
				return;
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			this._object = array;
			this._index = start;
			this._length = length;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00009060 File Offset: 0x00007260
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Memory(MemoryManager<T> manager, int length)
		{
			if (length < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			this._object = manager;
			this._index = int.MinValue;
			this._length = length;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00009088 File Offset: 0x00007288
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Memory(MemoryManager<T> manager, int start, int length)
		{
			if (length < 0 || start < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			this._object = manager;
			this._index = start | int.MinValue;
			this._length = length;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x000090B8 File Offset: 0x000072B8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Memory(object obj, int start, int length)
		{
			this._object = obj;
			this._index = start;
			this._length = length;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x000090D0 File Offset: 0x000072D0
		public static implicit operator Memory<T>(T[] array)
		{
			return new Memory<T>(array);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000090D8 File Offset: 0x000072D8
		public static implicit operator Memory<T>(ArraySegment<T> segment)
		{
			return new Memory<T>(segment.Array, segment.Offset, segment.Count);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00009104 File Offset: 0x00007304
		public unsafe static implicit operator ReadOnlyMemory<T>(Memory<T> memory)
		{
			return *Unsafe.As<Memory<T>, ReadOnlyMemory<T>>(ref memory);
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00009114 File Offset: 0x00007314
		public static Memory<T> Empty
		{
			get
			{
				return default(Memory<T>);
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00009130 File Offset: 0x00007330
		public int Length
		{
			get
			{
				return this._length & int.MaxValue;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00009140 File Offset: 0x00007340
		public bool IsEmpty
		{
			get
			{
				return (this._length & int.MaxValue) == 0;
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00009154 File Offset: 0x00007354
		public override string ToString()
		{
			if (!(typeof(T) == typeof(char)))
			{
				return string.Format("System.Memory<{0}>[{1}]", typeof(T).Name, this._length & int.MaxValue);
			}
			string text;
			if ((text = this._object as string) == null)
			{
				return this.Span.ToString();
			}
			return text.Substring(this._index, this._length & int.MaxValue);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000091F0 File Offset: 0x000073F0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Memory<T> Slice(int start)
		{
			int length = this._length;
			int num = length & int.MaxValue;
			if (start > num)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new Memory<T>(this._object, this._index + start, length - start);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00009234 File Offset: 0x00007434
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Memory<T> Slice(int start, int length)
		{
			int length2 = this._length;
			int num = length2 & int.MaxValue;
			if (start > num || length > num - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return new Memory<T>(this._object, this._index + start, length | (length2 & int.MinValue));
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00009288 File Offset: 0x00007488
		public Span<T> Span
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
					return new Span<T>(Unsafe.As<Pinnable<T>>(text), MemoryExtensions.StringAdjustment, text.Length).Slice(this._index, this._length);
				}
				if (this._object != null)
				{
					return new Span<T>((T[])this._object, this._index, this._length & int.MaxValue);
				}
				return default(Span<T>);
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00009364 File Offset: 0x00007564
		public void CopyTo(Memory<T> destination)
		{
			this.Span.CopyTo(destination.Span);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000938C File Offset: 0x0000758C
		public bool TryCopyTo(Memory<T> destination)
		{
			return this.Span.TryCopyTo(destination.Span);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000093B4 File Offset: 0x000075B4
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

		// Token: 0x06000167 RID: 359 RVA: 0x000094CC File Offset: 0x000076CC
		public T[] ToArray()
		{
			return this.Span.ToArray();
		}

		// Token: 0x06000168 RID: 360 RVA: 0x000094EC File Offset: 0x000076EC
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			if (obj is ReadOnlyMemory<T>)
			{
				return ((ReadOnlyMemory<T>)obj).Equals(this);
			}
			if (obj is Memory<T>)
			{
				Memory<T> memory = (Memory<T>)obj;
				return this.Equals(memory);
			}
			return false;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00009540 File Offset: 0x00007740
		public bool Equals(Memory<T> other)
		{
			return this._object == other._object && this._index == other._index && this._length == other._length;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00009574 File Offset: 0x00007774
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			if (this._object == null)
			{
				return 0;
			}
			return Memory<T>.CombineHashCodes(this._object.GetHashCode(), this._index.GetHashCode(), this._length.GetHashCode());
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000095C0 File Offset: 0x000077C0
		private static int CombineHashCodes(int left, int right)
		{
			return ((left << 5) + left) ^ right;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000095CC File Offset: 0x000077CC
		private static int CombineHashCodes(int h1, int h2, int h3)
		{
			return Memory<T>.CombineHashCodes(Memory<T>.CombineHashCodes(h1, h2), h3);
		}

		// Token: 0x04000105 RID: 261
		private readonly object _object;

		// Token: 0x04000106 RID: 262
		private readonly int _index;

		// Token: 0x04000107 RID: 263
		private readonly int _length;

		// Token: 0x04000108 RID: 264
		private const int RemoveFlagsBitMask = 2147483647;
	}
}
