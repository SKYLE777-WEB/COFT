using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x02000030 RID: 48
	internal static class SpanHelpers
	{
		// Token: 0x06000213 RID: 531 RVA: 0x0000C638 File Offset: 0x0000A838
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TComparable>(this ReadOnlySpan<T> span, TComparable comparable) where TComparable : object, IComparable<T>
		{
			if (comparable == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparable);
			}
			return SpanHelpers.BinarySearch<T, TComparable>(MemoryMarshal.GetReference<T>(span), span.Length, comparable);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000C660 File Offset: 0x0000A860
		public unsafe static int BinarySearch<T, TComparable>(ref T spanStart, int length, TComparable comparable) where TComparable : object, IComparable<T>
		{
			int i = 0;
			int num = length - 1;
			while (i <= num)
			{
				int num2 = (int)((uint)(num + i) >> 1);
				int num3 = comparable.CompareTo(*Unsafe.Add<T>(ref spanStart, num2));
				if (num3 == 0)
				{
					return num2;
				}
				if (num3 > 0)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return ~i;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000C6C0 File Offset: 0x0000A8C0
		public static int IndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			byte b = value;
			ref byte ptr = ref Unsafe.Add<byte>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				int num4 = SpanHelpers.IndexOf(Unsafe.Add<byte>(ref searchSpace, num2), b, num3);
				if (num4 == -1)
				{
					return -1;
				}
				num2 += num4;
				if (SpanHelpers.SequenceEqual<byte>(Unsafe.Add<byte>(ref searchSpace, num2 + 1), ref ptr, num))
				{
					break;
				}
				num2++;
			}
			return num2;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000C738 File Offset: 0x0000A938
		public unsafe static int IndexOfAny(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.IndexOf(ref searchSpace, *Unsafe.Add<byte>(ref value, i), searchSpaceLength);
				if (num2 < num)
				{
					num = num2;
					searchSpaceLength = num2;
					if (num == 0)
					{
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000C788 File Offset: 0x0000A988
		public unsafe static int LastIndexOfAny(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.LastIndexOf(ref searchSpace, *Unsafe.Add<byte>(ref value, i), searchSpaceLength);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000C7CC File Offset: 0x0000A9CC
		public unsafe static int IndexOf(ref byte searchSpace, byte value, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			if (Vector.IsHardwareAccelerated && length >= Vector<byte>.Count * 2)
			{
				int num = Unsafe.AsPointer<byte>(ref searchSpace) & (Vector<byte>.Count - 1);
				intPtr2 = (IntPtr)((Vector<byte>.Count - num) & (Vector<byte>.Count - 1));
			}
			Vector<byte> vector2;
			for (;;)
			{
				if ((void*)intPtr2 < 8)
				{
					if ((void*)intPtr2 >= 4)
					{
						intPtr2 -= 4;
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
						{
							goto IL_0254;
						}
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
						{
							goto IL_025C;
						}
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
						{
							goto IL_026A;
						}
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
						{
							goto IL_0278;
						}
						intPtr += 4;
					}
					while ((void*)intPtr2 != null)
					{
						intPtr2 -= 1;
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
						{
							goto IL_0254;
						}
						intPtr += 1;
					}
					if (!Vector.IsHardwareAccelerated || (void*)intPtr >= length)
					{
						return -1;
					}
					intPtr2 = (IntPtr)((length - (void*)intPtr) & ~(Vector<byte>.Count - 1));
					Vector<byte> vector = SpanHelpers.GetVector(value);
					while ((void*)intPtr2 != (void*)intPtr)
					{
						vector2 = Vector.Equals<byte>(vector, Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr)));
						if (!Vector<byte>.Zero.Equals(vector2))
						{
							goto IL_0213;
						}
						intPtr += Vector<byte>.Count;
					}
					if ((void*)intPtr >= length)
					{
						return -1;
					}
					intPtr2 = (IntPtr)(length - (void*)intPtr);
				}
				else
				{
					intPtr2 -= 8;
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
					{
						goto IL_0254;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
					{
						goto IL_025C;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
					{
						goto IL_026A;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
					{
						goto IL_0278;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4))
					{
						goto IL_0286;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5))
					{
						goto IL_0294;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6))
					{
						goto IL_02A2;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7))
					{
						goto IL_02B0;
					}
					intPtr += 8;
				}
			}
			IL_0213:
			return (void*)intPtr + SpanHelpers.LocateFirstFoundByte(vector2);
			IL_0254:
			return (void*)intPtr;
			IL_025C:
			return (void*)(intPtr + 1);
			IL_026A:
			return (void*)(intPtr + 2);
			IL_0278:
			return (void*)(intPtr + 3);
			IL_0286:
			return (void*)(intPtr + 4);
			IL_0294:
			return (void*)(intPtr + 5);
			IL_02A2:
			return (void*)(intPtr + 6);
			IL_02B0:
			return (void*)(intPtr + 7);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000CA9C File Offset: 0x0000AC9C
		public static int LastIndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			byte b = value;
			ref byte ptr = ref Unsafe.Add<byte>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				num4 = SpanHelpers.LastIndexOf(ref searchSpace, b, num3);
				if (num4 == -1)
				{
					return -1;
				}
				if (SpanHelpers.SequenceEqual<byte>(Unsafe.Add<byte>(ref searchSpace, num4 + 1), ref ptr, num))
				{
					break;
				}
				num2 += num3 - num4;
			}
			return num4;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000CB0C File Offset: 0x0000AD0C
		public unsafe static int LastIndexOf(ref byte searchSpace, byte value, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			if (Vector.IsHardwareAccelerated && length >= Vector<byte>.Count * 2)
			{
				int num = Unsafe.AsPointer<byte>(ref searchSpace) & (Vector<byte>.Count - 1);
				intPtr2 = (IntPtr)(((length & (Vector<byte>.Count - 1)) + num) & (Vector<byte>.Count - 1));
			}
			Vector<byte> vector2;
			for (;;)
			{
				if ((void*)intPtr2 < 8)
				{
					if ((void*)intPtr2 >= 4)
					{
						intPtr2 -= 4;
						intPtr -= 4;
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
						{
							goto IL_028A;
						}
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
						{
							goto IL_027C;
						}
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
						{
							goto IL_026E;
						}
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
						{
							break;
						}
					}
					while ((void*)intPtr2 != null)
					{
						intPtr2 -= 1;
						intPtr -= 1;
						if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
						{
							goto IL_0266;
						}
					}
					if (!Vector.IsHardwareAccelerated || (void*)intPtr == null)
					{
						return -1;
					}
					intPtr2 = (IntPtr)((void*)intPtr & ~(Vector<byte>.Count - 1));
					Vector<byte> vector = SpanHelpers.GetVector(value);
					while ((void*)intPtr2 != Vector<byte>.Count - 1)
					{
						vector2 = Vector.Equals<byte>(vector, Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr - Vector<byte>.Count)));
						if (!Vector<byte>.Zero.Equals(vector2))
						{
							goto IL_022B;
						}
						intPtr -= Vector<byte>.Count;
						intPtr2 -= Vector<byte>.Count;
					}
					if ((void*)intPtr == null)
					{
						return -1;
					}
					intPtr2 = intPtr;
				}
				else
				{
					intPtr2 -= 8;
					intPtr -= 8;
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7))
					{
						goto IL_02C2;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6))
					{
						goto IL_02B4;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5))
					{
						goto IL_02A6;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4))
					{
						goto IL_0298;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3))
					{
						goto IL_028A;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2))
					{
						goto IL_027C;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1))
					{
						goto IL_026E;
					}
					if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
					{
						break;
					}
				}
			}
			goto IL_0266;
			IL_022B:
			return (int)intPtr - Vector<byte>.Count + SpanHelpers.LocateLastFoundByte(vector2);
			IL_0266:
			return (void*)intPtr;
			IL_026E:
			return (void*)(intPtr + 1);
			IL_027C:
			return (void*)(intPtr + 2);
			IL_028A:
			return (void*)(intPtr + 3);
			IL_0298:
			return (void*)(intPtr + 4);
			IL_02A6:
			return (void*)(intPtr + 5);
			IL_02B4:
			return (void*)(intPtr + 6);
			IL_02C2:
			return (void*)(intPtr + 7);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000CDEC File Offset: 0x0000AFEC
		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			if (Vector.IsHardwareAccelerated && length >= Vector<byte>.Count * 2)
			{
				int num = Unsafe.AsPointer<byte>(ref searchSpace) & (Vector<byte>.Count - 1);
				intPtr2 = (IntPtr)((Vector<byte>.Count - num) & (Vector<byte>.Count - 1));
			}
			Vector<byte> vector4;
			for (;;)
			{
				if ((void*)intPtr2 < 8)
				{
					if ((void*)intPtr2 >= 4)
					{
						intPtr2 -= 4;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_030E;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_0316;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_0324;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_0332;
						}
						intPtr += 4;
					}
					while ((void*)intPtr2 != null)
					{
						intPtr2 -= 1;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_030E;
						}
						intPtr += 1;
					}
					if (!Vector.IsHardwareAccelerated || (void*)intPtr >= length)
					{
						return -1;
					}
					intPtr2 = (IntPtr)((length - (void*)intPtr) & ~(Vector<byte>.Count - 1));
					Vector<byte> vector = SpanHelpers.GetVector(value0);
					Vector<byte> vector2 = SpanHelpers.GetVector(value1);
					while ((void*)intPtr2 != (void*)intPtr)
					{
						Vector<byte> vector3 = Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						vector4 = Vector.BitwiseOr<byte>(Vector.Equals<byte>(vector3, vector), Vector.Equals<byte>(vector3, vector2));
						if (!Vector<byte>.Zero.Equals(vector4))
						{
							goto IL_02CD;
						}
						intPtr += Vector<byte>.Count;
					}
					if ((void*)intPtr >= length)
					{
						return -1;
					}
					intPtr2 = (IntPtr)(length - (void*)intPtr);
				}
				else
				{
					intPtr2 -= 8;
					uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_030E;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0316;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0324;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0332;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0340;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_034E;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_035C;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_036A;
					}
					intPtr += 8;
				}
			}
			IL_02CD:
			return (void*)intPtr + SpanHelpers.LocateFirstFoundByte(vector4);
			IL_030E:
			return (void*)intPtr;
			IL_0316:
			return (void*)(intPtr + 1);
			IL_0324:
			return (void*)(intPtr + 2);
			IL_0332:
			return (void*)(intPtr + 3);
			IL_0340:
			return (void*)(intPtr + 4);
			IL_034E:
			return (void*)(intPtr + 5);
			IL_035C:
			return (void*)(intPtr + 6);
			IL_036A:
			return (void*)(intPtr + 7);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000D174 File Offset: 0x0000B374
		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, byte value2, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			if (Vector.IsHardwareAccelerated && length >= Vector<byte>.Count * 2)
			{
				int num = Unsafe.AsPointer<byte>(ref searchSpace) & (Vector<byte>.Count - 1);
				intPtr2 = (IntPtr)((Vector<byte>.Count - num) & (Vector<byte>.Count - 1));
			}
			Vector<byte> vector5;
			for (;;)
			{
				if ((void*)intPtr2 < 8)
				{
					if ((void*)intPtr2 >= 4)
					{
						intPtr2 -= 4;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03A2;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03AA;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03B8;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03C6;
						}
						intPtr += 4;
					}
					while ((void*)intPtr2 != null)
					{
						intPtr2 -= 1;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03A2;
						}
						intPtr += 1;
					}
					if (!Vector.IsHardwareAccelerated || (void*)intPtr >= length)
					{
						return -1;
					}
					intPtr2 = (IntPtr)((length - (void*)intPtr) & ~(Vector<byte>.Count - 1));
					Vector<byte> vector = SpanHelpers.GetVector(value0);
					Vector<byte> vector2 = SpanHelpers.GetVector(value1);
					Vector<byte> vector3 = SpanHelpers.GetVector(value2);
					while ((void*)intPtr2 != (void*)intPtr)
					{
						Vector<byte> vector4 = Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						vector5 = Vector.BitwiseOr<byte>(Vector.BitwiseOr<byte>(Vector.Equals<byte>(vector4, vector), Vector.Equals<byte>(vector4, vector2)), Vector.Equals<byte>(vector4, vector3));
						if (!Vector<byte>.Zero.Equals(vector5))
						{
							goto IL_035D;
						}
						intPtr += Vector<byte>.Count;
					}
					if ((void*)intPtr >= length)
					{
						return -1;
					}
					intPtr2 = (IntPtr)(length - (void*)intPtr);
				}
				else
				{
					intPtr2 -= 8;
					uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03A2;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03AA;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03B8;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03C6;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03D4;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03E2;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03F0;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03FE;
					}
					intPtr += 8;
				}
			}
			IL_035D:
			return (void*)intPtr + SpanHelpers.LocateFirstFoundByte(vector5);
			IL_03A2:
			return (void*)intPtr;
			IL_03AA:
			return (void*)(intPtr + 1);
			IL_03B8:
			return (void*)(intPtr + 2);
			IL_03C6:
			return (void*)(intPtr + 3);
			IL_03D4:
			return (void*)(intPtr + 4);
			IL_03E2:
			return (void*)(intPtr + 5);
			IL_03F0:
			return (void*)(intPtr + 6);
			IL_03FE:
			return (void*)(intPtr + 7);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000D590 File Offset: 0x0000B790
		public unsafe static int LastIndexOfAny(ref byte searchSpace, byte value0, byte value1, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			if (Vector.IsHardwareAccelerated && length >= Vector<byte>.Count * 2)
			{
				int num = Unsafe.AsPointer<byte>(ref searchSpace) & (Vector<byte>.Count - 1);
				intPtr2 = (IntPtr)(((length & (Vector<byte>.Count - 1)) + num) & (Vector<byte>.Count - 1));
			}
			Vector<byte> vector4;
			for (;;)
			{
				if ((void*)intPtr2 < 8)
				{
					if ((void*)intPtr2 >= 4)
					{
						intPtr2 -= 4;
						intPtr -= 4;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_0347;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_0339;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_032B;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2)
						{
							break;
						}
						if ((uint)value1 == num2)
						{
							break;
						}
					}
					while ((void*)intPtr2 != null)
					{
						intPtr2 -= 1;
						intPtr -= 1;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							goto IL_0323;
						}
					}
					if (!Vector.IsHardwareAccelerated || (void*)intPtr == null)
					{
						return -1;
					}
					intPtr2 = (IntPtr)((void*)intPtr & ~(Vector<byte>.Count - 1));
					Vector<byte> vector = SpanHelpers.GetVector(value0);
					Vector<byte> vector2 = SpanHelpers.GetVector(value1);
					while ((void*)intPtr2 != Vector<byte>.Count - 1)
					{
						Vector<byte> vector3 = Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr - Vector<byte>.Count));
						vector4 = Vector.BitwiseOr<byte>(Vector.Equals<byte>(vector3, vector), Vector.Equals<byte>(vector3, vector2));
						if (!Vector<byte>.Zero.Equals(vector4))
						{
							goto IL_02E5;
						}
						intPtr -= Vector<byte>.Count;
						intPtr2 -= Vector<byte>.Count;
					}
					if ((void*)intPtr == null)
					{
						return -1;
					}
					intPtr2 = intPtr;
				}
				else
				{
					intPtr2 -= 8;
					intPtr -= 8;
					uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_037F;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0371;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0363;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0355;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0347;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_0339;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						goto IL_032B;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
					if ((uint)value0 == num2 || (uint)value1 == num2)
					{
						break;
					}
				}
			}
			goto IL_0323;
			IL_02E5:
			return (int)intPtr - Vector<byte>.Count + SpanHelpers.LocateLastFoundByte(vector4);
			IL_0323:
			return (void*)intPtr;
			IL_032B:
			return (void*)(intPtr + 1);
			IL_0339:
			return (void*)(intPtr + 2);
			IL_0347:
			return (void*)(intPtr + 3);
			IL_0355:
			return (void*)(intPtr + 4);
			IL_0363:
			return (void*)(intPtr + 5);
			IL_0371:
			return (void*)(intPtr + 6);
			IL_037F:
			return (void*)(intPtr + 7);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000D930 File Offset: 0x0000BB30
		public unsafe static int LastIndexOfAny(ref byte searchSpace, byte value0, byte value1, byte value2, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			if (Vector.IsHardwareAccelerated && length >= Vector<byte>.Count * 2)
			{
				int num = Unsafe.AsPointer<byte>(ref searchSpace) & (Vector<byte>.Count - 1);
				intPtr2 = (IntPtr)(((length & (Vector<byte>.Count - 1)) + num) & (Vector<byte>.Count - 1));
			}
			Vector<byte> vector5;
			for (;;)
			{
				if ((void*)intPtr2 < 8)
				{
					if ((void*)intPtr2 >= 4)
					{
						intPtr2 -= 4;
						intPtr -= 4;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03DB;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03CD;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03BF;
						}
						num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2 || (uint)value1 == num2)
						{
							break;
						}
						if ((uint)value2 == num2)
						{
							break;
						}
					}
					while ((void*)intPtr2 != null)
					{
						intPtr2 -= 1;
						intPtr -= 1;
						uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
						if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
						{
							goto IL_03B7;
						}
					}
					if (!Vector.IsHardwareAccelerated || (void*)intPtr == null)
					{
						return -1;
					}
					intPtr2 = (IntPtr)((void*)intPtr & ~(Vector<byte>.Count - 1));
					Vector<byte> vector = SpanHelpers.GetVector(value0);
					Vector<byte> vector2 = SpanHelpers.GetVector(value1);
					Vector<byte> vector3 = SpanHelpers.GetVector(value2);
					while ((void*)intPtr2 != Vector<byte>.Count - 1)
					{
						Vector<byte> vector4 = Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr - Vector<byte>.Count));
						vector5 = Vector.BitwiseOr<byte>(Vector.BitwiseOr<byte>(Vector.Equals<byte>(vector4, vector), Vector.Equals<byte>(vector4, vector2)), Vector.Equals<byte>(vector4, vector3));
						if (!Vector<byte>.Zero.Equals(vector5))
						{
							goto IL_0377;
						}
						intPtr -= Vector<byte>.Count;
						intPtr2 -= Vector<byte>.Count;
					}
					if ((void*)intPtr == null)
					{
						return -1;
					}
					intPtr2 = intPtr;
				}
				else
				{
					intPtr2 -= 8;
					intPtr -= 8;
					uint num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 7));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_0413;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 6));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_0405;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 5));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03F7;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 4));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03E9;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 3));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03DB;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 2));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03CD;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + 1));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						goto IL_03BF;
					}
					num2 = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
					if ((uint)value0 == num2 || (uint)value1 == num2 || (uint)value2 == num2)
					{
						break;
					}
				}
			}
			goto IL_03B7;
			IL_0377:
			return (int)intPtr - Vector<byte>.Count + SpanHelpers.LocateLastFoundByte(vector5);
			IL_03B7:
			return (void*)intPtr;
			IL_03BF:
			return (void*)(intPtr + 1);
			IL_03CD:
			return (void*)(intPtr + 2);
			IL_03DB:
			return (void*)(intPtr + 3);
			IL_03E9:
			return (void*)(intPtr + 4);
			IL_03F7:
			return (void*)(intPtr + 5);
			IL_0405:
			return (void*)(intPtr + 6);
			IL_0413:
			return (void*)(intPtr + 7);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000DD64 File Offset: 0x0000BF64
		public unsafe static bool SequenceEqual(ref byte first, ref byte second, NUInt length)
		{
			if (!Unsafe.AreSame<byte>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)0;
				IntPtr intPtr2 = (IntPtr)((void*)length);
				if (Vector.IsHardwareAccelerated && (void*)intPtr2 >= Vector<byte>.Count)
				{
					intPtr2 -= Vector<byte>.Count;
					while ((void*)intPtr2 != (void*)intPtr)
					{
						if (Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref first, intPtr)) != Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref second, intPtr)))
						{
							return false;
						}
						intPtr += Vector<byte>.Count;
					}
					return Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) == Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref second, intPtr2));
				}
				if ((void*)intPtr2 >= sizeof(UIntPtr))
				{
					intPtr2 -= sizeof(UIntPtr);
					while ((void*)intPtr2 != (void*)intPtr)
					{
						if (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr)) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr)))
						{
							return false;
						}
						intPtr += sizeof(UIntPtr);
					}
					return Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) == Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr2));
				}
				while ((void*)intPtr2 != (void*)intPtr)
				{
					if (*Unsafe.AddByteOffset<byte>(ref first, intPtr) != *Unsafe.AddByteOffset<byte>(ref second, intPtr))
					{
						return false;
					}
					intPtr += 1;
				}
				return true;
			}
			return true;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000DEC8 File Offset: 0x0000C0C8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateFirstFoundByte(Vector<byte> match)
		{
			Vector<ulong> vector = Vector.AsVectorUInt64<byte>(match);
			ulong num = 0UL;
			int i;
			for (i = 0; i < Vector<ulong>.Count; i++)
			{
				num = vector[i];
				if (num != 0UL)
				{
					break;
				}
			}
			return i * 8 + SpanHelpers.LocateFirstFoundByte(num);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000DF10 File Offset: 0x0000C110
		public unsafe static int SequenceCompareTo(ref byte first, int firstLength, ref byte second, int secondLength)
		{
			if (!Unsafe.AreSame<byte>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)((firstLength < secondLength) ? firstLength : secondLength);
				IntPtr intPtr2 = (IntPtr)0;
				IntPtr intPtr3 = (IntPtr)((void*)intPtr);
				if (Vector.IsHardwareAccelerated && (void*)intPtr3 != Vector<byte>.Count)
				{
					intPtr3 -= Vector<byte>.Count;
					while ((void*)intPtr3 != (void*)intPtr2)
					{
						if (Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) != Unsafe.ReadUnaligned<Vector<byte>>(Unsafe.AddByteOffset<byte>(ref second, intPtr2)))
						{
							break;
						}
						intPtr2 += Vector<byte>.Count;
					}
				}
				else if ((void*)intPtr3 != sizeof(UIntPtr))
				{
					intPtr3 -= sizeof(UIntPtr);
					while ((void*)intPtr3 != (void*)intPtr2)
					{
						if (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr2)))
						{
							break;
						}
						intPtr2 += sizeof(UIntPtr);
					}
				}
				while ((void*)intPtr != (void*)intPtr2)
				{
					int num = Unsafe.AddByteOffset<byte>(ref first, intPtr2).CompareTo(*Unsafe.AddByteOffset<byte>(ref second, intPtr2));
					if (num != 0)
					{
						return num;
					}
					intPtr2 += 1;
				}
			}
			return firstLength - secondLength;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000E060 File Offset: 0x0000C260
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateLastFoundByte(Vector<byte> match)
		{
			Vector<ulong> vector = Vector.AsVectorUInt64<byte>(match);
			ulong num = 0UL;
			int i;
			for (i = Vector<ulong>.Count - 1; i >= 0; i--)
			{
				num = vector[i];
				if (num != 0UL)
				{
					break;
				}
			}
			return i * 8 + SpanHelpers.LocateLastFoundByte(num);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000E0AC File Offset: 0x0000C2AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateFirstFoundByte(ulong match)
		{
			ulong num = match ^ (match - 1UL);
			return (int)(num * 283686952306184UL >> 57);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000E0D4 File Offset: 0x0000C2D4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateLastFoundByte(ulong match)
		{
			int num = 7;
			while (match > 0UL)
			{
				match <<= 8;
				num--;
			}
			return num;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000E0FC File Offset: 0x0000C2FC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector<byte> GetVector(byte vectorByte)
		{
			return Vector.AsVectorByte<uint>(new Vector<uint>((uint)vectorByte * 16843009U));
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000E110 File Offset: 0x0000C310
		public unsafe static int SequenceCompareTo(ref char first, int firstLength, ref char second, int secondLength)
		{
			int num = firstLength - secondLength;
			if (!Unsafe.AreSame<char>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)((firstLength < secondLength) ? firstLength : secondLength);
				IntPtr intPtr2 = (IntPtr)0;
				if ((void*)intPtr >= sizeof(UIntPtr) / 2)
				{
					if (Vector.IsHardwareAccelerated && (void*)intPtr >= Vector<ushort>.Count)
					{
						IntPtr intPtr3 = intPtr - Vector<ushort>.Count;
						while (!(Unsafe.ReadUnaligned<Vector<ushort>>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) != Unsafe.ReadUnaligned<Vector<ushort>>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2)))))
						{
							intPtr2 += Vector<ushort>.Count;
							if ((void*)intPtr3 < (void*)intPtr2)
							{
								break;
							}
						}
					}
					while ((void*)intPtr >= (void*)(intPtr2 + sizeof(UIntPtr) / 2) && !(Unsafe.ReadUnaligned<UIntPtr>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2)))))
					{
						intPtr2 += sizeof(UIntPtr) / 2;
					}
				}
				if (sizeof(UIntPtr) > 4 && (void*)intPtr >= (void*)(intPtr2 + 2) && Unsafe.ReadUnaligned<int>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) == Unsafe.ReadUnaligned<int>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2))))
				{
					intPtr2 += 2;
				}
				while ((void*)intPtr2 < (void*)intPtr)
				{
					int num2 = Unsafe.Add<char>(ref first, intPtr2).CompareTo(*Unsafe.Add<char>(ref second, intPtr2));
					if (num2 != 0)
					{
						return num2;
					}
					intPtr2 += 1;
				}
			}
			return num;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000E2B8 File Offset: 0x0000C4B8
		public unsafe static int IndexOf(ref char searchSpace, char value, int length)
		{
			fixed (char* ptr = &searchSpace)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2;
				char* ptr4 = ptr3 + length;
				if (Vector.IsHardwareAccelerated && length >= Vector<ushort>.Count * 2)
				{
					int num = (ptr3 & (Unsafe.SizeOf<Vector<ushort>>() - 1)) / 2;
					length = (Vector<ushort>.Count - num) & (Vector<ushort>.Count - 1);
				}
				Vector<ushort> vector2;
				for (;;)
				{
					if (length < 4)
					{
						while (length > 0)
						{
							length--;
							if (*ptr3 == value)
							{
								goto IL_0145;
							}
							ptr3++;
						}
						if (!Vector.IsHardwareAccelerated || ptr3 >= ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr4 - ptr3) & (long)(~(long)(Vector<ushort>.Count - 1)));
						Vector<ushort> vector = new Vector<ushort>((ushort)value);
						while (length > 0)
						{
							vector2 = Vector.Equals<ushort>(vector, Unsafe.Read<Vector<ushort>>((void*)ptr3));
							if (!Vector<ushort>.Zero.Equals(vector2))
							{
								goto IL_010E;
							}
							ptr3 += Vector<ushort>.Count;
							length -= Vector<ushort>.Count;
						}
						if (ptr3 >= ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr4 - ptr3));
					}
					else
					{
						length -= 4;
						if (*ptr3 == value)
						{
							goto IL_0145;
						}
						if (ptr3[1] == value)
						{
							goto IL_0141;
						}
						if (ptr3[2] == value)
						{
							goto IL_013D;
						}
						if (ptr3[3] == value)
						{
							goto IL_0139;
						}
						ptr3 += 4;
					}
				}
				IL_010E:
				return (int)((long)(ptr3 - ptr2)) + SpanHelpers.LocateFirstFoundChar(vector2);
				IL_0139:
				ptr3++;
				IL_013D:
				ptr3++;
				IL_0141:
				ptr3++;
				IL_0145:
				return (int)((long)(ptr3 - ptr2));
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000E418 File Offset: 0x0000C618
		public unsafe static int LastIndexOf(ref char searchSpace, char value, int length)
		{
			fixed (char* ptr = &searchSpace)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + length;
				char* ptr4 = ptr2;
				if (Vector.IsHardwareAccelerated && length >= Vector<ushort>.Count * 2)
				{
					length = (ptr3 & (Unsafe.SizeOf<Vector<ushort>>() - 1)) / 2;
				}
				char* ptr5;
				Vector<ushort> vector2;
				for (;;)
				{
					if (length < 4)
					{
						while (length > 0)
						{
							length--;
							ptr3--;
							if (*ptr3 == value)
							{
								goto IL_0135;
							}
						}
						if (!Vector.IsHardwareAccelerated || ptr3 == ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr3 - ptr4) & (long)(~(long)(Vector<ushort>.Count - 1)));
						Vector<ushort> vector = new Vector<ushort>((ushort)value);
						while (length > 0)
						{
							ptr5 = ptr3 - Vector<ushort>.Count;
							vector2 = Vector.Equals<ushort>(vector, Unsafe.Read<Vector<ushort>>((void*)ptr5));
							if (!Vector<ushort>.Zero.Equals(vector2))
							{
								goto IL_0109;
							}
							ptr3 -= Vector<ushort>.Count;
							length -= Vector<ushort>.Count;
						}
						if (ptr3 == ptr4)
						{
							return -1;
						}
						length = (int)((long)(ptr3 - ptr4));
					}
					else
					{
						length -= 4;
						ptr3 -= 4;
						if (ptr3[3] == value)
						{
							goto IL_0151;
						}
						if (ptr3[2] == value)
						{
							goto IL_0147;
						}
						if (ptr3[1] == value)
						{
							goto IL_013D;
						}
						if (*ptr3 == value)
						{
							goto IL_0135;
						}
					}
				}
				IL_0109:
				return (int)((long)(ptr5 - ptr4)) + SpanHelpers.LocateLastFoundChar(vector2);
				IL_0135:
				return (int)((long)(ptr3 - ptr4));
				IL_013D:
				return (int)((long)(ptr3 - ptr4)) + 1;
				IL_0147:
				return (int)((long)(ptr3 - ptr4)) + 2;
				IL_0151:
				return (int)((long)(ptr3 - ptr4)) + 3;
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000E584 File Offset: 0x0000C784
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateFirstFoundChar(Vector<ushort> match)
		{
			Vector<ulong> vector = Vector.AsVectorUInt64<ushort>(match);
			ulong num = 0UL;
			int i;
			for (i = 0; i < Vector<ulong>.Count; i++)
			{
				num = vector[i];
				if (num != 0UL)
				{
					break;
				}
			}
			return i * 4 + SpanHelpers.LocateFirstFoundChar(num);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000E5CC File Offset: 0x0000C7CC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateFirstFoundChar(ulong match)
		{
			ulong num = match ^ (match - 1UL);
			return (int)(num * 4295098372UL >> 49);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000E5F4 File Offset: 0x0000C7F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateLastFoundChar(Vector<ushort> match)
		{
			Vector<ulong> vector = Vector.AsVectorUInt64<ushort>(match);
			ulong num = 0UL;
			int i;
			for (i = Vector<ulong>.Count - 1; i >= 0; i--)
			{
				num = vector[i];
				if (num != 0UL)
				{
					break;
				}
			}
			return i * 4 + SpanHelpers.LocateLastFoundChar(num);
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000E640 File Offset: 0x0000C840
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int LocateLastFoundChar(ulong match)
		{
			int num = 3;
			while (match > 0UL)
			{
				match <<= 16;
				num--;
			}
			return num;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000E668 File Offset: 0x0000C868
		public static int IndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : object, IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			T t = value;
			ref T ptr = ref Unsafe.Add<T>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				int num4 = SpanHelpers.IndexOf<T>(Unsafe.Add<T>(ref searchSpace, num2), t, num3);
				if (num4 == -1)
				{
					return -1;
				}
				num2 += num4;
				if (SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(ref searchSpace, num2 + 1), ref ptr, num))
				{
					break;
				}
				num2++;
			}
			return num2;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000E6E4 File Offset: 0x0000C8E4
		public unsafe static int IndexOf<T>(ref T searchSpace, T value, int length) where T : object, IEquatable<T>
		{
			IntPtr intPtr = (IntPtr)0;
			while (length >= 8)
			{
				length -= 8;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr)))
				{
					IL_020E:
					return (void*)intPtr;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 1)))
				{
					IL_0216:
					return (void*)(intPtr + 1);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 2)))
				{
					IL_0224:
					return (void*)(intPtr + 2);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 3)))
				{
					IL_0232:
					return (void*)(intPtr + 3);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 4)))
				{
					return (void*)(intPtr + 4);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 5)))
				{
					return (void*)(intPtr + 5);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 6)))
				{
					return (void*)(intPtr + 6);
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 7)))
				{
					return (void*)(intPtr + 7);
				}
				intPtr += 8;
			}
			if (length >= 4)
			{
				length -= 4;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr)))
				{
					goto IL_020E;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 1)))
				{
					goto IL_0216;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 2)))
				{
					goto IL_0224;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr + 3)))
				{
					goto IL_0232;
				}
				intPtr += 4;
			}
			while (length > 0)
			{
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, intPtr)))
				{
					goto IL_020E;
				}
				intPtr += 1;
				length--;
			}
			return -1;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000E96C File Offset: 0x0000CB6C
		public unsafe static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, int length) where T : object, IEquatable<T>
		{
			int i = 0;
			while (length - i >= 8)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02DD:
					return i + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02E1:
					return i + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02E5:
					return i + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 4);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 5);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 6);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 7);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 7;
				}
				i += 8;
			}
			if (length - i >= 4)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02DD;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02E1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02E5;
				}
				i += 4;
			}
			while (i < length)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000EC78 File Offset: 0x0000CE78
		public unsafe static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : object, IEquatable<T>
		{
			int i = 0;
			while (length - i >= 8)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03D7:
					return i + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03DB:
					return i + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03DF:
					return i + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 4);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 5);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 6);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 7);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 7;
				}
				i += 8;
			}
			if (length - i >= 4)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03D7;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03DB;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03DF;
				}
				i += 4;
			}
			while (i < length)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000F07C File Offset: 0x0000D27C
		public unsafe static int IndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : object, IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.IndexOf<T>(ref searchSpace, *Unsafe.Add<T>(ref value, i), searchSpaceLength);
				if (num2 < num)
				{
					num = num2;
					searchSpaceLength = num2;
					if (num == 0)
					{
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000F0D0 File Offset: 0x0000D2D0
		public static int LastIndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : object, IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			T t = value;
			ref T ptr = ref Unsafe.Add<T>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				num4 = SpanHelpers.LastIndexOf<T>(ref searchSpace, t, num3);
				if (num4 == -1)
				{
					return -1;
				}
				if (SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(ref searchSpace, num4 + 1), ref ptr, num))
				{
					break;
				}
				num2 += num3 - num4;
			}
			return num4;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000F144 File Offset: 0x0000D344
		public unsafe static int LastIndexOf<T>(ref T searchSpace, T value, int length) where T : object, IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 7)))
				{
					return length + 7;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 6)))
				{
					return length + 6;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 5)))
				{
					return length + 5;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 4)))
				{
					return length + 4;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 3)))
				{
					IL_01D1:
					return length + 3;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 2)))
				{
					IL_01CD:
					return length + 2;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 1)))
				{
					IL_01C9:
					return length + 1;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 3)))
				{
					goto IL_01D1;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 2)))
				{
					goto IL_01CD;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length + 1)))
				{
					goto IL_01C9;
				}
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				if (value.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			return -1;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000F33C File Offset: 0x0000D53C
		public unsafe static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, int length) where T : object, IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 7);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 7;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 6);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 5);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 4);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02E2:
					return length + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02DE:
					return length + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02DA:
					return length + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02E2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02DE;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02DA;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t))
				{
					return length;
				}
				if (value1.Equals(t))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				T t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
			}
			return -1;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000F644 File Offset: 0x0000D844
		public unsafe static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : object, IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 7);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 7;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 6);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 5);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 4);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03EF:
					return length + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03EA:
					return length + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03E5:
					return length + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03EF;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03EA;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03E5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
				if (value2.Equals(t))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				T t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length;
				}
			}
			return -1;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000FA5C File Offset: 0x0000DC5C
		public unsafe static int LastIndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : object, IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.LastIndexOf<T>(ref searchSpace, *Unsafe.Add<T>(ref value, i), searchSpaceLength);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000FAA4 File Offset: 0x0000DCA4
		public unsafe static bool SequenceEqual<T>(ref T first, ref T second, int length) where T : object, IEquatable<T>
		{
			if (!Unsafe.AreSame<T>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)0;
				while (length >= 8)
				{
					length -= 8;
					if (!Unsafe.Add<T>(ref first, intPtr).Equals(*Unsafe.Add<T>(ref second, intPtr)) || !Unsafe.Add<T>(ref first, intPtr + 1).Equals(*Unsafe.Add<T>(ref second, intPtr + 1)) || !Unsafe.Add<T>(ref first, intPtr + 2).Equals(*Unsafe.Add<T>(ref second, intPtr + 2)) || !Unsafe.Add<T>(ref first, intPtr + 3).Equals(*Unsafe.Add<T>(ref second, intPtr + 3)) || !Unsafe.Add<T>(ref first, intPtr + 4).Equals(*Unsafe.Add<T>(ref second, intPtr + 4)) || !Unsafe.Add<T>(ref first, intPtr + 5).Equals(*Unsafe.Add<T>(ref second, intPtr + 5)) || !Unsafe.Add<T>(ref first, intPtr + 6).Equals(*Unsafe.Add<T>(ref second, intPtr + 6)) || !Unsafe.Add<T>(ref first, intPtr + 7).Equals(*Unsafe.Add<T>(ref second, intPtr + 7)))
					{
						return false;
					}
					intPtr += 8;
				}
				if (length >= 4)
				{
					length -= 4;
					if (!Unsafe.Add<T>(ref first, intPtr).Equals(*Unsafe.Add<T>(ref second, intPtr)) || !Unsafe.Add<T>(ref first, intPtr + 1).Equals(*Unsafe.Add<T>(ref second, intPtr + 1)) || !Unsafe.Add<T>(ref first, intPtr + 2).Equals(*Unsafe.Add<T>(ref second, intPtr + 2)) || !Unsafe.Add<T>(ref first, intPtr + 3).Equals(*Unsafe.Add<T>(ref second, intPtr + 3)))
					{
						return false;
					}
					intPtr += 4;
				}
				while (length > 0)
				{
					if (!Unsafe.Add<T>(ref first, intPtr).Equals(*Unsafe.Add<T>(ref second, intPtr)))
					{
						return false;
					}
					intPtr += 1;
					length--;
				}
			}
			return true;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000FD50 File Offset: 0x0000DF50
		public unsafe static int SequenceCompareTo<T>(ref T first, int firstLength, ref T second, int secondLength) where T : object, IComparable<T>
		{
			int num = firstLength;
			if (num > secondLength)
			{
				num = secondLength;
			}
			for (int i = 0; i < num; i++)
			{
				int num2 = Unsafe.Add<T>(ref first, i).CompareTo(*Unsafe.Add<T>(ref second, i));
				if (num2 != 0)
				{
					return num2;
				}
			}
			return firstLength.CompareTo(secondLength);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000FDAC File Offset: 0x0000DFAC
		public unsafe static void CopyTo<T>(ref T dst, int dstLength, ref T src, int srcLength)
		{
			IntPtr intPtr = Unsafe.ByteOffset<T>(ref src, Unsafe.Add<T>(ref src, srcLength));
			IntPtr intPtr2 = Unsafe.ByteOffset<T>(ref dst, Unsafe.Add<T>(ref dst, dstLength));
			IntPtr intPtr3 = Unsafe.ByteOffset<T>(ref src, ref dst);
			if (!((sizeof(IntPtr) == 4) ? ((int)intPtr3 < (int)intPtr || (int)intPtr3 > -(int)intPtr2) : ((long)intPtr3 < (long)intPtr || (long)intPtr3 > -(long)intPtr2)) && !SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ref byte ptr = ref Unsafe.As<T, byte>(ref dst);
				ref byte ptr2 = ref Unsafe.As<T, byte>(ref src);
				ulong num = (ulong)(long)intPtr;
				uint num3;
				for (ulong num2 = 0UL; num2 < num; num2 += (ulong)num3)
				{
					num3 = ((num - num2 > (ulong)(-1)) ? uint.MaxValue : ((uint)(num - num2)));
					Unsafe.CopyBlock(Unsafe.Add<byte>(ref ptr, (IntPtr)((long)num2)), Unsafe.Add<byte>(ref ptr2, (IntPtr)((long)num2)), num3);
				}
				return;
			}
			bool flag = ((sizeof(IntPtr) == 4) ? ((int)intPtr3 > -(int)intPtr2) : ((long)intPtr3 > -(long)intPtr2));
			int num4 = (flag ? 1 : (-1));
			int num5 = (flag ? 0 : (srcLength - 1));
			int i;
			for (i = 0; i < (srcLength & -8); i += 8)
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				*Unsafe.Add<T>(ref dst, num5 + num4) = *Unsafe.Add<T>(ref src, num5 + num4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 2) = *Unsafe.Add<T>(ref src, num5 + num4 * 2);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 3) = *Unsafe.Add<T>(ref src, num5 + num4 * 3);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 4) = *Unsafe.Add<T>(ref src, num5 + num4 * 4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 5) = *Unsafe.Add<T>(ref src, num5 + num4 * 5);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 6) = *Unsafe.Add<T>(ref src, num5 + num4 * 6);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 7) = *Unsafe.Add<T>(ref src, num5 + num4 * 7);
				num5 += num4 * 8;
			}
			if (i < (srcLength & -4))
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				*Unsafe.Add<T>(ref dst, num5 + num4) = *Unsafe.Add<T>(ref src, num5 + num4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 2) = *Unsafe.Add<T>(ref src, num5 + num4 * 2);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 3) = *Unsafe.Add<T>(ref src, num5 + num4 * 3);
				num5 += num4 * 4;
				i += 4;
			}
			while (i < srcLength)
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				num5 += num4;
				i++;
			}
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00010110 File Offset: 0x0000E310
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static IntPtr Add<T>(this IntPtr start, int index)
		{
			if (sizeof(IntPtr) == 4)
			{
				uint num = (uint)(index * Unsafe.SizeOf<T>());
				return (IntPtr)((void*)((byte*)(void*)start + num));
			}
			ulong num2 = (ulong)((long)index * (long)Unsafe.SizeOf<T>());
			return (IntPtr)((void*)((byte*)(void*)start + num2));
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0001015C File Offset: 0x0000E35C
		public static bool IsReferenceOrContainsReferences<T>()
		{
			return SpanHelpers.PerTypeValues<T>.IsReferenceOrContainsReferences;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00010164 File Offset: 0x0000E364
		private static bool IsReferenceOrContainsReferencesCore(Type type)
		{
			if (type.GetTypeInfo().IsPrimitive)
			{
				return false;
			}
			if (!type.GetTypeInfo().IsValueType)
			{
				return true;
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (type.GetTypeInfo().IsEnum)
			{
				return false;
			}
			foreach (FieldInfo fieldInfo in type.GetTypeInfo().DeclaredFields)
			{
				if (!fieldInfo.IsStatic && SpanHelpers.IsReferenceOrContainsReferencesCore(fieldInfo.FieldType))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0001022C File Offset: 0x0000E42C
		public unsafe static void ClearLessThanPointerSized(byte* ptr, UIntPtr byteLength)
		{
			if (sizeof(UIntPtr) == 4)
			{
				Unsafe.InitBlockUnaligned((void*)ptr, 0, (uint)byteLength);
				return;
			}
			ulong num = (ulong)byteLength;
			uint num2 = (uint)(num & (ulong)(-1));
			Unsafe.InitBlockUnaligned((void*)ptr, 0, num2);
			num -= (ulong)num2;
			ptr += num2;
			while (num > 0UL)
			{
				num2 = ((num >= (ulong)(-1)) ? uint.MaxValue : ((uint)num));
				Unsafe.InitBlockUnaligned((void*)ptr, 0, num2);
				ptr += num2;
				num -= (ulong)num2;
			}
		}

		// Token: 0x0600023E RID: 574 RVA: 0x000102A8 File Offset: 0x0000E4A8
		public static void ClearLessThanPointerSized(ref byte b, UIntPtr byteLength)
		{
			if (sizeof(UIntPtr) == 4)
			{
				Unsafe.InitBlockUnaligned(ref b, 0, (uint)byteLength);
				return;
			}
			ulong num = (ulong)byteLength;
			uint num2 = (uint)(num & (ulong)(-1));
			Unsafe.InitBlockUnaligned(ref b, 0, num2);
			num -= (ulong)num2;
			long num3 = (long)((ulong)num2);
			while (num > 0UL)
			{
				num2 = ((num >= (ulong)(-1)) ? uint.MaxValue : ((uint)num));
				ref byte ptr = ref Unsafe.Add<byte>(ref b, (IntPtr)num3);
				Unsafe.InitBlockUnaligned(ref ptr, 0, num2);
				num3 += (long)((ulong)num2);
				num -= (ulong)num2;
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0001032C File Offset: 0x0000E52C
		public unsafe static void ClearPointerSizedWithoutReferences(ref byte b, UIntPtr byteLength)
		{
			IntPtr intPtr = IntPtr.Zero;
			while (intPtr.LessThanEqual(byteLength - sizeof(SpanHelpers.Reg64)))
			{
				*Unsafe.As<byte, SpanHelpers.Reg64>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg64);
				intPtr += sizeof(SpanHelpers.Reg64);
			}
			if (intPtr.LessThanEqual(byteLength - sizeof(SpanHelpers.Reg32)))
			{
				*Unsafe.As<byte, SpanHelpers.Reg32>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg32);
				intPtr += sizeof(SpanHelpers.Reg32);
			}
			if (intPtr.LessThanEqual(byteLength - sizeof(SpanHelpers.Reg16)))
			{
				*Unsafe.As<byte, SpanHelpers.Reg16>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg16);
				intPtr += sizeof(SpanHelpers.Reg16);
			}
			if (intPtr.LessThanEqual(byteLength - 8))
			{
				*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, intPtr)) = 0L;
				intPtr += 8;
			}
			if (sizeof(IntPtr) == 4 && intPtr.LessThanEqual(byteLength - 4))
			{
				*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, intPtr)) = 0;
				intPtr += 4;
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00010444 File Offset: 0x0000E644
		public unsafe static void ClearPointerSizedWithReferences(ref IntPtr ip, UIntPtr pointerSizeLength)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			while ((intPtr2 = intPtr + 8).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 1) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 2) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 3) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 4) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 5) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 6) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 7) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr2 = intPtr + 4).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 1) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 2) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 3) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr2 = intPtr + 2).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + 1) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr + 1).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr) = 0;
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000105D4 File Offset: 0x0000E7D4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool LessThanEqual(this IntPtr index, UIntPtr length)
		{
			if (sizeof(UIntPtr) != 4)
			{
				return (long)index <= (long)(ulong)length;
			}
			return (int)index <= (int)(uint)length;
		}

		// Token: 0x04000116 RID: 278
		private const ulong XorPowerOfTwoToHighByte = 283686952306184UL;

		// Token: 0x04000117 RID: 279
		private const ulong XorPowerOfTwoToHighChar = 4295098372UL;

		// Token: 0x020001BC RID: 444
		internal struct ComparerComparable<T, TComparer> : IComparable<T> where TComparer : object, IComparer<T>
		{
			// Token: 0x06001547 RID: 5447 RVA: 0x00075814 File Offset: 0x00073A14
			public ComparerComparable(T value, TComparer comparer)
			{
				this._value = value;
				this._comparer = comparer;
			}

			// Token: 0x06001548 RID: 5448 RVA: 0x00075824 File Offset: 0x00073A24
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CompareTo(T other)
			{
				TComparer comparer = this._comparer;
				return comparer.Compare(this._value, other);
			}

			// Token: 0x040007F6 RID: 2038
			private readonly T _value;

			// Token: 0x040007F7 RID: 2039
			private readonly TComparer _comparer;
		}

		// Token: 0x020001BD RID: 445
		private struct Reg64
		{
		}

		// Token: 0x020001BE RID: 446
		private struct Reg32
		{
		}

		// Token: 0x020001BF RID: 447
		private struct Reg16
		{
		}

		// Token: 0x020001C0 RID: 448
		public static class PerTypeValues<T>
		{
			// Token: 0x06001549 RID: 5449 RVA: 0x00075850 File Offset: 0x00073A50
			private static IntPtr MeasureArrayAdjustment()
			{
				T[] array = new T[1];
				return Unsafe.ByteOffset<T>(ref Unsafe.As<Pinnable<T>>(array).Data, ref array[0]);
			}

			// Token: 0x040007F8 RID: 2040
			public static readonly bool IsReferenceOrContainsReferences = SpanHelpers.IsReferenceOrContainsReferencesCore(typeof(T));

			// Token: 0x040007F9 RID: 2041
			public static readonly T[] EmptyArray = new T[0];

			// Token: 0x040007FA RID: 2042
			public static readonly IntPtr ArrayAdjustment = SpanHelpers.PerTypeValues<T>.MeasureArrayAdjustment();
		}
	}
}
