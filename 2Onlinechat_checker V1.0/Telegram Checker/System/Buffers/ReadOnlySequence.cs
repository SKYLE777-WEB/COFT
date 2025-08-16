using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x0200003E RID: 62
	[DebuggerTypeProxy(typeof(ReadOnlySequenceDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	[ComVisible(true)]
	public readonly struct ReadOnlySequence<T>
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000293 RID: 659 RVA: 0x00011210 File Offset: 0x0000F410
		public long Length
		{
			get
			{
				return this.GetLength();
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000294 RID: 660 RVA: 0x00011218 File Offset: 0x0000F418
		public bool IsEmpty
		{
			get
			{
				return this.Length == 0L;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000295 RID: 661 RVA: 0x00011224 File Offset: 0x0000F424
		public bool IsSingleSegment
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._sequenceStart.GetObject() == this._sequenceEnd.GetObject();
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000296 RID: 662 RVA: 0x00011240 File Offset: 0x0000F440
		public ReadOnlyMemory<T> First
		{
			get
			{
				return this.GetFirstBuffer();
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000297 RID: 663 RVA: 0x00011248 File Offset: 0x0000F448
		public SequencePosition Start
		{
			get
			{
				return this._sequenceStart;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000298 RID: 664 RVA: 0x00011250 File Offset: 0x0000F450
		public SequencePosition End
		{
			get
			{
				return this._sequenceEnd;
			}
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00011258 File Offset: 0x0000F458
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ReadOnlySequence(object startSegment, int startIndexAndFlags, object endSegment, int endIndexAndFlags)
		{
			this._sequenceStart = new SequencePosition(startSegment, startIndexAndFlags);
			this._sequenceEnd = new SequencePosition(endSegment, endIndexAndFlags);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00011278 File Offset: 0x0000F478
		public ReadOnlySequence(ReadOnlySequenceSegment<T> startSegment, int startIndex, ReadOnlySequenceSegment<T> endSegment, int endIndex)
		{
			if (startSegment == null || endSegment == null || (startSegment != endSegment && startSegment.RunningIndex > endSegment.RunningIndex) || startSegment.Memory.Length < startIndex || endSegment.Memory.Length < endIndex || (startSegment == endSegment && endIndex < startIndex))
			{
				ThrowHelper.ThrowArgumentValidationException<T>(startSegment, startIndex, endSegment);
			}
			this._sequenceStart = new SequencePosition(startSegment, ReadOnlySequence.SegmentToSequenceStart(startIndex));
			this._sequenceEnd = new SequencePosition(endSegment, ReadOnlySequence.SegmentToSequenceEnd(endIndex));
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00011314 File Offset: 0x0000F514
		public ReadOnlySequence(T[] array)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			this._sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(0));
			this._sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(array.Length));
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0001134C File Offset: 0x0000F54C
		public ReadOnlySequence(T[] array, int start, int length)
		{
			if (array == null || start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentValidationException(array, start);
			}
			this._sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(start));
			this._sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(start + length));
		}

		// Token: 0x0600029D RID: 669 RVA: 0x000113A4 File Offset: 0x0000F5A4
		public ReadOnlySequence(ReadOnlyMemory<T> memory)
		{
			MemoryManager<T> memoryManager;
			int num;
			int num2;
			if (MemoryMarshal.TryGetMemoryManager<T, MemoryManager<T>>(memory, out memoryManager, out num, out num2))
			{
				this._sequenceStart = new SequencePosition(memoryManager, ReadOnlySequence.MemoryManagerToSequenceStart(num));
				this._sequenceEnd = new SequencePosition(memoryManager, ReadOnlySequence.MemoryManagerToSequenceEnd(num + num2));
				return;
			}
			ArraySegment<T> arraySegment;
			if (MemoryMarshal.TryGetArray<T>(memory, out arraySegment))
			{
				T[] array = arraySegment.Array;
				int offset = arraySegment.Offset;
				this._sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(offset));
				this._sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(offset + arraySegment.Count));
				return;
			}
			if (typeof(T) == typeof(char))
			{
				string text;
				int num3;
				if (!MemoryMarshal.TryGetString((ReadOnlyMemory<char>)memory, out text, out num3, out num2))
				{
					ThrowHelper.ThrowInvalidOperationException();
				}
				this._sequenceStart = new SequencePosition(text, ReadOnlySequence.StringToSequenceStart(num3));
				this._sequenceEnd = new SequencePosition(text, ReadOnlySequence.StringToSequenceEnd(num3 + num2));
				return;
			}
			ThrowHelper.ThrowInvalidOperationException();
			this._sequenceStart = default(SequencePosition);
			this._sequenceEnd = default(SequencePosition);
		}

		// Token: 0x0600029E RID: 670 RVA: 0x000114C4 File Offset: 0x0000F6C4
		public ReadOnlySequence<T> Slice(long start, long length)
		{
			if (start < 0L || length < 0L)
			{
				ThrowHelper.ThrowStartOrEndArgumentValidationException(start);
			}
			int num = ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			int index = ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			object @object = this._sequenceStart.GetObject();
			object object2 = this._sequenceEnd.GetObject();
			SequencePosition sequencePosition;
			SequencePosition sequencePosition2;
			if (@object != object2)
			{
				ReadOnlySequenceSegment<T> readOnlySequenceSegment = (ReadOnlySequenceSegment<T>)@object;
				int num2 = readOnlySequenceSegment.Memory.Length - num;
				if ((long)num2 > start)
				{
					num += (int)start;
					sequencePosition = new SequencePosition(@object, num);
					sequencePosition2 = ReadOnlySequence<T>.GetEndPosition(readOnlySequenceSegment, @object, num, object2, index, length);
				}
				else
				{
					if (num2 < 0)
					{
						ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
					}
					sequencePosition = ReadOnlySequence<T>.SeekMultiSegment(readOnlySequenceSegment.Next, object2, index, start - (long)num2, ExceptionArgument.start);
					int index2 = ReadOnlySequence<T>.GetIndex(in sequencePosition);
					object object3 = sequencePosition.GetObject();
					if (object3 != object2)
					{
						sequencePosition2 = ReadOnlySequence<T>.GetEndPosition((ReadOnlySequenceSegment<T>)object3, object3, index2, object2, index, length);
					}
					else
					{
						if ((long)(index - index2) < length)
						{
							ThrowHelper.ThrowStartOrEndArgumentValidationException(0L);
						}
						sequencePosition2 = new SequencePosition(object3, index2 + (int)length);
					}
				}
			}
			else
			{
				if ((long)(index - num) < start)
				{
					ThrowHelper.ThrowStartOrEndArgumentValidationException(-1L);
				}
				num += (int)start;
				sequencePosition = new SequencePosition(@object, num);
				if ((long)(index - num) < length)
				{
					ThrowHelper.ThrowStartOrEndArgumentValidationException(0L);
				}
				sequencePosition2 = new SequencePosition(@object, num + (int)length);
			}
			return this.SliceImpl(in sequencePosition, in sequencePosition2);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0001162C File Offset: 0x0000F82C
		public ReadOnlySequence<T> Slice(long start, SequencePosition end)
		{
			if (start < 0L)
			{
				ThrowHelper.ThrowStartOrEndArgumentValidationException(start);
			}
			uint index = (uint)ReadOnlySequence<T>.GetIndex(in end);
			object @object = end.GetObject();
			uint index2 = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			object object2 = this._sequenceStart.GetObject();
			uint index3 = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			object object3 = this._sequenceEnd.GetObject();
			if (object2 == object3)
			{
				if (!ReadOnlySequence<T>.InRange(index, index2, index3))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
				}
				if ((ulong)(index - index2) < (ulong)start)
				{
					ThrowHelper.ThrowStartOrEndArgumentValidationException(-1L);
				}
			}
			else
			{
				ReadOnlySequenceSegment<T> readOnlySequenceSegment = (ReadOnlySequenceSegment<T>)object2;
				ulong num = (ulong)(readOnlySequenceSegment.RunningIndex + (long)((ulong)index2));
				ulong num2 = (ulong)(((ReadOnlySequenceSegment<T>)@object).RunningIndex + (long)((ulong)index));
				if (!ReadOnlySequence<T>.InRange(num2, num, (ulong)(((ReadOnlySequenceSegment<T>)object3).RunningIndex + (long)((ulong)index3))))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
				}
				if (num + (ulong)start > num2)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				int num3 = readOnlySequenceSegment.Memory.Length - (int)index2;
				if ((long)num3 <= start)
				{
					if (num3 < 0)
					{
						ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
					}
					SequencePosition sequencePosition = ReadOnlySequence<T>.SeekMultiSegment(readOnlySequenceSegment.Next, @object, (int)index, start - (long)num3, ExceptionArgument.start);
					return this.SliceImpl(in sequencePosition, in end);
				}
			}
			SequencePosition sequencePosition2 = new SequencePosition(object2, (int)(index2 + (uint)((int)start)));
			return this.SliceImpl(in sequencePosition2, in end);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00011778 File Offset: 0x0000F978
		public ReadOnlySequence<T> Slice(SequencePosition start, long length)
		{
			uint index = (uint)ReadOnlySequence<T>.GetIndex(in start);
			object @object = start.GetObject();
			uint index2 = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			object object2 = this._sequenceStart.GetObject();
			uint index3 = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			object object3 = this._sequenceEnd.GetObject();
			if (object2 == object3)
			{
				if (!ReadOnlySequence<T>.InRange(index, index2, index3))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
				}
				if (length < 0L)
				{
					ThrowHelper.ThrowStartOrEndArgumentValidationException(0L);
				}
				if ((ulong)(index3 - index) < (ulong)length)
				{
					ThrowHelper.ThrowStartOrEndArgumentValidationException(0L);
				}
			}
			else
			{
				ReadOnlySequenceSegment<T> readOnlySequenceSegment = (ReadOnlySequenceSegment<T>)@object;
				ulong num = (ulong)(readOnlySequenceSegment.RunningIndex + (long)((ulong)index));
				ulong num2 = (ulong)(((ReadOnlySequenceSegment<T>)object2).RunningIndex + (long)((ulong)index2));
				ulong num3 = (ulong)(((ReadOnlySequenceSegment<T>)object3).RunningIndex + (long)((ulong)index3));
				if (!ReadOnlySequence<T>.InRange(num, num2, num3))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
				}
				if (length < 0L)
				{
					ThrowHelper.ThrowStartOrEndArgumentValidationException(0L);
				}
				if (num + (ulong)length > num3)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
				}
				int num4 = readOnlySequenceSegment.Memory.Length - (int)index;
				if ((long)num4 < length)
				{
					if (num4 < 0)
					{
						ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
					}
					SequencePosition sequencePosition = ReadOnlySequence<T>.SeekMultiSegment(readOnlySequenceSegment.Next, object3, (int)index3, length - (long)num4, ExceptionArgument.length);
					return this.SliceImpl(in start, in sequencePosition);
				}
			}
			SequencePosition sequencePosition2 = new SequencePosition(@object, (int)(index + (uint)((int)length)));
			return this.SliceImpl(in start, in sequencePosition2);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x000118DC File Offset: 0x0000FADC
		public ReadOnlySequence<T> Slice(int start, int length)
		{
			return this.Slice((long)start, (long)length);
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x000118E8 File Offset: 0x0000FAE8
		public ReadOnlySequence<T> Slice(int start, SequencePosition end)
		{
			return this.Slice((long)start, end);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x000118F4 File Offset: 0x0000FAF4
		public ReadOnlySequence<T> Slice(SequencePosition start, int length)
		{
			return this.Slice(start, (long)length);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00011900 File Offset: 0x0000FB00
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySequence<T> Slice(SequencePosition start, SequencePosition end)
		{
			this.BoundsCheck((uint)ReadOnlySequence<T>.GetIndex(in start), start.GetObject(), (uint)ReadOnlySequence<T>.GetIndex(in end), end.GetObject());
			return this.SliceImpl(in start, in end);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00011940 File Offset: 0x0000FB40
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySequence<T> Slice(SequencePosition start)
		{
			this.BoundsCheck(in start);
			return this.SliceImpl(in start, in this._sequenceEnd);
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00011958 File Offset: 0x0000FB58
		public ReadOnlySequence<T> Slice(long start)
		{
			if (start < 0L)
			{
				ThrowHelper.ThrowStartOrEndArgumentValidationException(start);
			}
			if (start == 0L)
			{
				return this;
			}
			SequencePosition sequencePosition = this.Seek(in this._sequenceStart, in this._sequenceEnd, start, ExceptionArgument.start);
			return this.SliceImpl(in sequencePosition, in this._sequenceEnd);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x000119A8 File Offset: 0x0000FBA8
		public unsafe override string ToString()
		{
			if (typeof(T) == typeof(char))
			{
				ReadOnlySequence<T> readOnlySequence = this;
				ReadOnlySequence<char> readOnlySequence2 = *Unsafe.As<ReadOnlySequence<T>, ReadOnlySequence<char>>(ref readOnlySequence);
				string text;
				int num;
				int num2;
				if (SequenceMarshal.TryGetString(readOnlySequence2, out text, out num, out num2))
				{
					return text.Substring(num, num2);
				}
				if (this.Length < 2147483647L)
				{
					return new string((in readOnlySequence2).ToArray<char>());
				}
			}
			return string.Format("System.Buffers.ReadOnlySequence<{0}>[{1}]", typeof(T).Name, this.Length);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00011A48 File Offset: 0x0000FC48
		public ReadOnlySequence<T>.Enumerator GetEnumerator()
		{
			return new ReadOnlySequence<T>.Enumerator(in this);
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00011A50 File Offset: 0x0000FC50
		public SequencePosition GetPosition(long offset)
		{
			return this.GetPosition(offset, this._sequenceStart);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00011A60 File Offset: 0x0000FC60
		public SequencePosition GetPosition(long offset, SequencePosition origin)
		{
			if (offset < 0L)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_OffsetOutOfRange();
			}
			return this.Seek(in origin, in this._sequenceEnd, offset, ExceptionArgument.offset);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00011A80 File Offset: 0x0000FC80
		public bool TryGet(ref SequencePosition position, out ReadOnlyMemory<T> memory, bool advance = true)
		{
			SequencePosition sequencePosition;
			bool flag = this.TryGetBuffer(in position, out memory, out sequencePosition);
			if (advance)
			{
				position = sequencePosition;
			}
			return flag;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00011AAC File Offset: 0x0000FCAC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool TryGetBuffer(in SequencePosition position, out ReadOnlyMemory<T> memory, out SequencePosition next)
		{
			object @object = position.GetObject();
			next = default(SequencePosition);
			if (@object == null)
			{
				memory = default(ReadOnlyMemory<T>);
				return false;
			}
			ReadOnlySequence<T>.SequenceType sequenceType = this.GetSequenceType();
			object object2 = this._sequenceEnd.GetObject();
			int index = ReadOnlySequence<T>.GetIndex(in position);
			int index2 = ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			if (sequenceType == ReadOnlySequence<T>.SequenceType.MultiSegment)
			{
				ReadOnlySequenceSegment<T> readOnlySequenceSegment = (ReadOnlySequenceSegment<T>)@object;
				if (readOnlySequenceSegment != object2)
				{
					ReadOnlySequenceSegment<T> next2 = readOnlySequenceSegment.Next;
					if (next2 == null)
					{
						ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
					}
					next = new SequencePosition(next2, 0);
					memory = readOnlySequenceSegment.Memory.Slice(index);
				}
				else
				{
					memory = readOnlySequenceSegment.Memory.Slice(index, index2 - index);
				}
			}
			else
			{
				if (@object != object2)
				{
					ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
				}
				if (sequenceType == ReadOnlySequence<T>.SequenceType.Array)
				{
					memory = new ReadOnlyMemory<T>((T[])@object, index, index2 - index);
				}
				else if (typeof(T) == typeof(char) && sequenceType == ReadOnlySequence<T>.SequenceType.String)
				{
					memory = (ReadOnlyMemory<T>)((string)@object).AsMemory(index, index2 - index);
				}
				else
				{
					memory = ((MemoryManager<T>)@object).Memory.Slice(index, index2 - index);
				}
			}
			return true;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00011C14 File Offset: 0x0000FE14
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ReadOnlyMemory<T> GetFirstBuffer()
		{
			object @object = this._sequenceStart.GetObject();
			if (@object == null)
			{
				return default(ReadOnlyMemory<T>);
			}
			int num = this._sequenceStart.GetInteger();
			int integer = this._sequenceEnd.GetInteger();
			bool flag = @object != this._sequenceEnd.GetObject();
			if (num >= 0)
			{
				if (integer < 0)
				{
					if (flag)
					{
						ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
					}
					return new ReadOnlyMemory<T>((T[])@object, num, (integer & int.MaxValue) - num);
				}
				ReadOnlyMemory<T> memory = ((ReadOnlySequenceSegment<T>)@object).Memory;
				if (flag)
				{
					return memory.Slice(num);
				}
				return memory.Slice(num, integer - num);
			}
			else
			{
				if (flag)
				{
					ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
				}
				if (typeof(T) == typeof(char) && integer < 0)
				{
					return (ReadOnlyMemory<T>)((string)@object).AsMemory(num & int.MaxValue, integer - num);
				}
				num &= int.MaxValue;
				return ((MemoryManager<T>)@object).Memory.Slice(num, integer - num);
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00011D3C File Offset: 0x0000FF3C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SequencePosition Seek(in SequencePosition start, in SequencePosition end, long offset, ExceptionArgument argument)
		{
			int index = ReadOnlySequence<T>.GetIndex(in start);
			int index2 = ReadOnlySequence<T>.GetIndex(in end);
			object @object = start.GetObject();
			object object2 = end.GetObject();
			if (@object != object2)
			{
				ReadOnlySequenceSegment<T> readOnlySequenceSegment = (ReadOnlySequenceSegment<T>)@object;
				int num = readOnlySequenceSegment.Memory.Length - index;
				if ((long)num <= offset)
				{
					if (num < 0)
					{
						ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
					}
					return ReadOnlySequence<T>.SeekMultiSegment(readOnlySequenceSegment.Next, object2, index2, offset - (long)num, argument);
				}
			}
			else if ((long)(index2 - index) < offset)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(argument);
			}
			return new SequencePosition(@object, index + (int)offset);
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00011DD4 File Offset: 0x0000FFD4
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static SequencePosition SeekMultiSegment(ReadOnlySequenceSegment<T> currentSegment, object endObject, int endIndex, long offset, ExceptionArgument argument)
		{
			while (currentSegment != null && currentSegment != endObject)
			{
				int length = currentSegment.Memory.Length;
				if ((long)length > offset)
				{
					IL_0049:
					return new SequencePosition(currentSegment, (int)offset);
				}
				offset -= (long)length;
				currentSegment = currentSegment.Next;
			}
			if (currentSegment == null || (long)endIndex < offset)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(argument);
				goto IL_0049;
			}
			goto IL_0049;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00011E38 File Offset: 0x00010038
		private void BoundsCheck(in SequencePosition position)
		{
			uint index = (uint)ReadOnlySequence<T>.GetIndex(in position);
			uint index2 = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			uint index3 = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			object @object = this._sequenceStart.GetObject();
			object object2 = this._sequenceEnd.GetObject();
			if (@object == object2)
			{
				if (!ReadOnlySequence<T>.InRange(index, index2, index3))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
					return;
				}
			}
			else
			{
				ulong num = (ulong)(((ReadOnlySequenceSegment<T>)@object).RunningIndex + (long)((ulong)index2));
				if (!ReadOnlySequence<T>.InRange((ulong)(((ReadOnlySequenceSegment<T>)position.GetObject()).RunningIndex + (long)((ulong)index)), num, (ulong)(((ReadOnlySequenceSegment<T>)object2).RunningIndex + (long)((ulong)index3))))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
				}
			}
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00011EE0 File Offset: 0x000100E0
		private void BoundsCheck(uint sliceStartIndex, object sliceStartObject, uint sliceEndIndex, object sliceEndObject)
		{
			uint index = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			uint index2 = (uint)ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			object @object = this._sequenceStart.GetObject();
			object object2 = this._sequenceEnd.GetObject();
			if (@object == object2)
			{
				if (sliceStartObject != sliceEndObject || sliceStartObject != @object || sliceStartIndex > sliceEndIndex || sliceStartIndex < index || sliceEndIndex > index2)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
					return;
				}
			}
			else
			{
				ulong num = (ulong)(((ReadOnlySequenceSegment<T>)sliceStartObject).RunningIndex + (long)((ulong)sliceStartIndex));
				ulong num2 = (ulong)(((ReadOnlySequenceSegment<T>)sliceEndObject).RunningIndex + (long)((ulong)sliceEndIndex));
				if (num > num2)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
				}
				if (num < (ulong)(((ReadOnlySequenceSegment<T>)@object).RunningIndex + (long)((ulong)index)) || num2 > (ulong)(((ReadOnlySequenceSegment<T>)object2).RunningIndex + (long)((ulong)index2)))
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
				}
			}
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00011FB0 File Offset: 0x000101B0
		private static SequencePosition GetEndPosition(ReadOnlySequenceSegment<T> startSegment, object startObject, int startIndex, object endObject, int endIndex, long length)
		{
			int num = startSegment.Memory.Length - startIndex;
			if ((long)num > length)
			{
				return new SequencePosition(startObject, startIndex + (int)length);
			}
			if (num < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
			}
			return ReadOnlySequence<T>.SeekMultiSegment(startSegment.Next, endObject, endIndex, length - (long)num, ExceptionArgument.length);
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00012008 File Offset: 0x00010208
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ReadOnlySequence<T>.SequenceType GetSequenceType()
		{
			return (ReadOnlySequence<T>.SequenceType)(-(ReadOnlySequence<T>.SequenceType)(2 * (this._sequenceStart.GetInteger() >> 31) + (this._sequenceEnd.GetInteger() >> 31)));
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0001202C File Offset: 0x0001022C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetIndex(in SequencePosition position)
		{
			return position.GetInteger() & int.MaxValue;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0001203C File Offset: 0x0001023C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ReadOnlySequence<T> SliceImpl(in SequencePosition start, in SequencePosition end)
		{
			return new ReadOnlySequence<T>(start.GetObject(), ReadOnlySequence<T>.GetIndex(in start) | (this._sequenceStart.GetInteger() & int.MinValue), end.GetObject(), ReadOnlySequence<T>.GetIndex(in end) | (this._sequenceEnd.GetInteger() & int.MinValue));
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00012090 File Offset: 0x00010290
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private long GetLength()
		{
			int index = ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			int index2 = ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			object @object = this._sequenceStart.GetObject();
			object object2 = this._sequenceEnd.GetObject();
			if (@object != object2)
			{
				ReadOnlySequenceSegment<T> readOnlySequenceSegment = (ReadOnlySequenceSegment<T>)@object;
				ReadOnlySequenceSegment<T> readOnlySequenceSegment2 = (ReadOnlySequenceSegment<T>)object2;
				return readOnlySequenceSegment2.RunningIndex + (long)index2 - (readOnlySequenceSegment.RunningIndex + (long)index);
			}
			return (long)(index2 - index);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00012104 File Offset: 0x00010304
		internal bool TryGetReadOnlySequenceSegment(out ReadOnlySequenceSegment<T> startSegment, out int startIndex, out ReadOnlySequenceSegment<T> endSegment, out int endIndex)
		{
			object @object = this._sequenceStart.GetObject();
			if (@object == null || this.GetSequenceType() != ReadOnlySequence<T>.SequenceType.MultiSegment)
			{
				startSegment = null;
				startIndex = 0;
				endSegment = null;
				endIndex = 0;
				return false;
			}
			startSegment = (ReadOnlySequenceSegment<T>)@object;
			startIndex = ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			endSegment = (ReadOnlySequenceSegment<T>)this._sequenceEnd.GetObject();
			endIndex = ReadOnlySequence<T>.GetIndex(in this._sequenceEnd);
			return true;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00012178 File Offset: 0x00010378
		internal bool TryGetArray(out ArraySegment<T> segment)
		{
			if (this.GetSequenceType() != ReadOnlySequence<T>.SequenceType.Array)
			{
				segment = default(ArraySegment<T>);
				return false;
			}
			int index = ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			segment = new ArraySegment<T>((T[])this._sequenceStart.GetObject(), index, ReadOnlySequence<T>.GetIndex(in this._sequenceEnd) - index);
			return true;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x000121D4 File Offset: 0x000103D4
		internal bool TryGetString(out string text, out int start, out int length)
		{
			if (typeof(T) != typeof(char) || this.GetSequenceType() != ReadOnlySequence<T>.SequenceType.String)
			{
				start = 0;
				length = 0;
				text = null;
				return false;
			}
			start = ReadOnlySequence<T>.GetIndex(in this._sequenceStart);
			length = ReadOnlySequence<T>.GetIndex(in this._sequenceEnd) - start;
			text = (string)this._sequenceStart.GetObject();
			return true;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0001224C File Offset: 0x0001044C
		private static bool InRange(uint value, uint start, uint end)
		{
			return value - start <= end - start;
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0001225C File Offset: 0x0001045C
		private static bool InRange(ulong value, ulong start, ulong end)
		{
			return value - start <= end - start;
		}

		// Token: 0x0400012B RID: 299
		private readonly SequencePosition _sequenceStart;

		// Token: 0x0400012C RID: 300
		private readonly SequencePosition _sequenceEnd;

		// Token: 0x0400012D RID: 301
		public static readonly ReadOnlySequence<T> Empty = new ReadOnlySequence<T>(SpanHelpers.PerTypeValues<T>.EmptyArray);

		// Token: 0x020001C3 RID: 451
		public struct Enumerator
		{
			// Token: 0x06001556 RID: 5462 RVA: 0x00075A54 File Offset: 0x00073C54
			public Enumerator(in ReadOnlySequence<T> sequence)
			{
				this._currentMemory = default(ReadOnlyMemory<T>);
				this._next = sequence.Start;
				this._sequence = sequence;
			}

			// Token: 0x170003DC RID: 988
			// (get) Token: 0x06001557 RID: 5463 RVA: 0x00075A7C File Offset: 0x00073C7C
			public ReadOnlyMemory<T> Current
			{
				get
				{
					return this._currentMemory;
				}
			}

			// Token: 0x06001558 RID: 5464 RVA: 0x00075A84 File Offset: 0x00073C84
			public bool MoveNext()
			{
				return this._next.GetObject() != null && this._sequence.TryGet(ref this._next, out this._currentMemory, true);
			}

			// Token: 0x04000802 RID: 2050
			private readonly ReadOnlySequence<T> _sequence;

			// Token: 0x04000803 RID: 2051
			private SequencePosition _next;

			// Token: 0x04000804 RID: 2052
			private ReadOnlyMemory<T> _currentMemory;
		}

		// Token: 0x020001C4 RID: 452
		private enum SequenceType
		{
			// Token: 0x04000806 RID: 2054
			MultiSegment,
			// Token: 0x04000807 RID: 2055
			Array,
			// Token: 0x04000808 RID: 2056
			MemoryManager,
			// Token: 0x04000809 RID: 2057
			String,
			// Token: 0x0400080A RID: 2058
			Empty
		}
	}
}
