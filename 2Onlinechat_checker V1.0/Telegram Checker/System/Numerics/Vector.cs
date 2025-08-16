﻿using System;
using System.Globalization;
using System.Numerics.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Numerics
{
	// Token: 0x02000093 RID: 147
	[Intrinsic]
	[ComVisible(true)]
	public struct Vector<T> : IEquatable<Vector<T>>, IFormattable where T : struct
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x00024BE0 File Offset: 0x00022DE0
		public static int Count
		{
			[Intrinsic]
			get
			{
				return Vector<T>.s_count;
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060006D3 RID: 1747 RVA: 0x00024BE8 File Offset: 0x00022DE8
		public static Vector<T> Zero
		{
			[Intrinsic]
			get
			{
				return Vector<T>.s_zero;
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x00024BF0 File Offset: 0x00022DF0
		public static Vector<T> One
		{
			[Intrinsic]
			get
			{
				return Vector<T>.s_one;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x00024BF8 File Offset: 0x00022DF8
		internal static Vector<T> AllOnes
		{
			get
			{
				return Vector<T>.s_allOnes;
			}
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00024C00 File Offset: 0x00022E00
		private unsafe static int InitializeCount()
		{
			Vector<T>.VectorSizeHelper vectorSizeHelper;
			byte* ptr = &vectorSizeHelper._placeholder.register.byte_0;
			byte* ptr2 = &vectorSizeHelper._byte;
			int num = (int)((long)(ptr2 - ptr));
			int num2;
			if (typeof(T) == typeof(byte))
			{
				num2 = 1;
			}
			else if (typeof(T) == typeof(sbyte))
			{
				num2 = 1;
			}
			else if (typeof(T) == typeof(ushort))
			{
				num2 = 2;
			}
			else if (typeof(T) == typeof(short))
			{
				num2 = 2;
			}
			else if (typeof(T) == typeof(uint))
			{
				num2 = 4;
			}
			else if (typeof(T) == typeof(int))
			{
				num2 = 4;
			}
			else if (typeof(T) == typeof(ulong))
			{
				num2 = 8;
			}
			else if (typeof(T) == typeof(long))
			{
				num2 = 8;
			}
			else if (typeof(T) == typeof(float))
			{
				num2 = 4;
			}
			else
			{
				if (!(typeof(T) == typeof(double)))
				{
					throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
				}
				num2 = 8;
			}
			return num / num2;
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00024DC4 File Offset: 0x00022FC4
		[Intrinsic]
		public unsafe Vector(T value)
		{
			this = default(Vector<T>);
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					fixed (byte* ptr = &this.register.byte_0)
					{
						byte* ptr2 = ptr;
						for (int i = 0; i < Vector<T>.Count; i++)
						{
							ptr2[i] = (byte)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(sbyte))
				{
					fixed (sbyte* ptr3 = &this.register.sbyte_0)
					{
						sbyte* ptr4 = ptr3;
						for (int j = 0; j < Vector<T>.Count; j++)
						{
							ptr4[j] = (sbyte)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(ushort))
				{
					fixed (ushort* ptr5 = &this.register.uint16_0)
					{
						ushort* ptr6 = ptr5;
						for (int k = 0; k < Vector<T>.Count; k++)
						{
							ptr6[k] = (ushort)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(short))
				{
					fixed (short* ptr7 = &this.register.int16_0)
					{
						short* ptr8 = ptr7;
						for (int l = 0; l < Vector<T>.Count; l++)
						{
							ptr8[l] = (short)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(uint))
				{
					fixed (uint* ptr9 = &this.register.uint32_0)
					{
						uint* ptr10 = ptr9;
						for (int m = 0; m < Vector<T>.Count; m++)
						{
							ptr10[m] = (uint)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(int))
				{
					fixed (int* ptr11 = &this.register.int32_0)
					{
						int* ptr12 = ptr11;
						for (int n = 0; n < Vector<T>.Count; n++)
						{
							ptr12[n] = (int)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(ulong))
				{
					fixed (ulong* ptr13 = &this.register.uint64_0)
					{
						ulong* ptr14 = ptr13;
						for (int num = 0; num < Vector<T>.Count; num++)
						{
							ptr14[num] = (ulong)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(long))
				{
					fixed (long* ptr15 = &this.register.int64_0)
					{
						long* ptr16 = ptr15;
						for (int num2 = 0; num2 < Vector<T>.Count; num2++)
						{
							ptr16[num2] = (long)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(float))
				{
					fixed (float* ptr17 = &this.register.single_0)
					{
						float* ptr18 = ptr17;
						for (int num3 = 0; num3 < Vector<T>.Count; num3++)
						{
							ptr18[num3] = (float)((object)value);
						}
					}
					return;
				}
				if (typeof(T) == typeof(double))
				{
					fixed (double* ptr19 = &this.register.double_0)
					{
						double* ptr20 = ptr19;
						for (int num4 = 0; num4 < Vector<T>.Count; num4++)
						{
							ptr20[num4] = (double)((object)value);
						}
					}
					return;
				}
			}
			else
			{
				if (typeof(T) == typeof(byte))
				{
					this.register.byte_0 = (byte)((object)value);
					this.register.byte_1 = (byte)((object)value);
					this.register.byte_2 = (byte)((object)value);
					this.register.byte_3 = (byte)((object)value);
					this.register.byte_4 = (byte)((object)value);
					this.register.byte_5 = (byte)((object)value);
					this.register.byte_6 = (byte)((object)value);
					this.register.byte_7 = (byte)((object)value);
					this.register.byte_8 = (byte)((object)value);
					this.register.byte_9 = (byte)((object)value);
					this.register.byte_10 = (byte)((object)value);
					this.register.byte_11 = (byte)((object)value);
					this.register.byte_12 = (byte)((object)value);
					this.register.byte_13 = (byte)((object)value);
					this.register.byte_14 = (byte)((object)value);
					this.register.byte_15 = (byte)((object)value);
					return;
				}
				if (typeof(T) == typeof(sbyte))
				{
					this.register.sbyte_0 = (sbyte)((object)value);
					this.register.sbyte_1 = (sbyte)((object)value);
					this.register.sbyte_2 = (sbyte)((object)value);
					this.register.sbyte_3 = (sbyte)((object)value);
					this.register.sbyte_4 = (sbyte)((object)value);
					this.register.sbyte_5 = (sbyte)((object)value);
					this.register.sbyte_6 = (sbyte)((object)value);
					this.register.sbyte_7 = (sbyte)((object)value);
					this.register.sbyte_8 = (sbyte)((object)value);
					this.register.sbyte_9 = (sbyte)((object)value);
					this.register.sbyte_10 = (sbyte)((object)value);
					this.register.sbyte_11 = (sbyte)((object)value);
					this.register.sbyte_12 = (sbyte)((object)value);
					this.register.sbyte_13 = (sbyte)((object)value);
					this.register.sbyte_14 = (sbyte)((object)value);
					this.register.sbyte_15 = (sbyte)((object)value);
					return;
				}
				if (typeof(T) == typeof(ushort))
				{
					this.register.uint16_0 = (ushort)((object)value);
					this.register.uint16_1 = (ushort)((object)value);
					this.register.uint16_2 = (ushort)((object)value);
					this.register.uint16_3 = (ushort)((object)value);
					this.register.uint16_4 = (ushort)((object)value);
					this.register.uint16_5 = (ushort)((object)value);
					this.register.uint16_6 = (ushort)((object)value);
					this.register.uint16_7 = (ushort)((object)value);
					return;
				}
				if (typeof(T) == typeof(short))
				{
					this.register.int16_0 = (short)((object)value);
					this.register.int16_1 = (short)((object)value);
					this.register.int16_2 = (short)((object)value);
					this.register.int16_3 = (short)((object)value);
					this.register.int16_4 = (short)((object)value);
					this.register.int16_5 = (short)((object)value);
					this.register.int16_6 = (short)((object)value);
					this.register.int16_7 = (short)((object)value);
					return;
				}
				if (typeof(T) == typeof(uint))
				{
					this.register.uint32_0 = (uint)((object)value);
					this.register.uint32_1 = (uint)((object)value);
					this.register.uint32_2 = (uint)((object)value);
					this.register.uint32_3 = (uint)((object)value);
					return;
				}
				if (typeof(T) == typeof(int))
				{
					this.register.int32_0 = (int)((object)value);
					this.register.int32_1 = (int)((object)value);
					this.register.int32_2 = (int)((object)value);
					this.register.int32_3 = (int)((object)value);
					return;
				}
				if (typeof(T) == typeof(ulong))
				{
					this.register.uint64_0 = (ulong)((object)value);
					this.register.uint64_1 = (ulong)((object)value);
					return;
				}
				if (typeof(T) == typeof(long))
				{
					this.register.int64_0 = (long)((object)value);
					this.register.int64_1 = (long)((object)value);
					return;
				}
				if (typeof(T) == typeof(float))
				{
					this.register.single_0 = (float)((object)value);
					this.register.single_1 = (float)((object)value);
					this.register.single_2 = (float)((object)value);
					this.register.single_3 = (float)((object)value);
					return;
				}
				if (typeof(T) == typeof(double))
				{
					this.register.double_0 = (double)((object)value);
					this.register.double_1 = (double)((object)value);
				}
			}
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x00025874 File Offset: 0x00023A74
		[Intrinsic]
		public Vector(T[] values)
		{
			this = new Vector<T>(values, 0);
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x00025880 File Offset: 0x00023A80
		public unsafe Vector(T[] values, int index)
		{
			this = default(Vector<T>);
			if (values == null)
			{
				throw new NullReferenceException(System.Numerics.Vectors159800.SR.Arg_NullArgumentNullRef);
			}
			if (index < 0 || values.Length - index < Vector<T>.Count)
			{
				throw new IndexOutOfRangeException(System.Numerics.Vectors159800.SR.Format(System.Numerics.Vectors159800.SR.Arg_InsufficientNumberOfElements, Vector<T>.Count, "values"));
			}
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					fixed (byte* ptr = &this.register.byte_0)
					{
						byte* ptr2 = ptr;
						for (int i = 0; i < Vector<T>.Count; i++)
						{
							ptr2[i] = (byte)((object)values[i + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(sbyte))
				{
					fixed (sbyte* ptr3 = &this.register.sbyte_0)
					{
						sbyte* ptr4 = ptr3;
						for (int j = 0; j < Vector<T>.Count; j++)
						{
							ptr4[j] = (sbyte)((object)values[j + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(ushort))
				{
					fixed (ushort* ptr5 = &this.register.uint16_0)
					{
						ushort* ptr6 = ptr5;
						for (int k = 0; k < Vector<T>.Count; k++)
						{
							ptr6[k] = (ushort)((object)values[k + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(short))
				{
					fixed (short* ptr7 = &this.register.int16_0)
					{
						short* ptr8 = ptr7;
						for (int l = 0; l < Vector<T>.Count; l++)
						{
							ptr8[l] = (short)((object)values[l + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(uint))
				{
					fixed (uint* ptr9 = &this.register.uint32_0)
					{
						uint* ptr10 = ptr9;
						for (int m = 0; m < Vector<T>.Count; m++)
						{
							ptr10[m] = (uint)((object)values[m + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(int))
				{
					fixed (int* ptr11 = &this.register.int32_0)
					{
						int* ptr12 = ptr11;
						for (int n = 0; n < Vector<T>.Count; n++)
						{
							ptr12[n] = (int)((object)values[n + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(ulong))
				{
					fixed (ulong* ptr13 = &this.register.uint64_0)
					{
						ulong* ptr14 = ptr13;
						for (int num = 0; num < Vector<T>.Count; num++)
						{
							ptr14[num] = (ulong)((object)values[num + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(long))
				{
					fixed (long* ptr15 = &this.register.int64_0)
					{
						long* ptr16 = ptr15;
						for (int num2 = 0; num2 < Vector<T>.Count; num2++)
						{
							ptr16[num2] = (long)((object)values[num2 + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(float))
				{
					fixed (float* ptr17 = &this.register.single_0)
					{
						float* ptr18 = ptr17;
						for (int num3 = 0; num3 < Vector<T>.Count; num3++)
						{
							ptr18[num3] = (float)((object)values[num3 + index]);
						}
					}
					return;
				}
				if (typeof(T) == typeof(double))
				{
					fixed (double* ptr19 = &this.register.double_0)
					{
						double* ptr20 = ptr19;
						for (int num4 = 0; num4 < Vector<T>.Count; num4++)
						{
							ptr20[num4] = (double)((object)values[num4 + index]);
						}
					}
					return;
				}
			}
			else
			{
				if (typeof(T) == typeof(byte))
				{
					fixed (byte* ptr21 = &this.register.byte_0)
					{
						byte* ptr22 = ptr21;
						*ptr22 = (byte)((object)values[index]);
						ptr22[1] = (byte)((object)values[1 + index]);
						ptr22[2] = (byte)((object)values[2 + index]);
						ptr22[3] = (byte)((object)values[3 + index]);
						ptr22[4] = (byte)((object)values[4 + index]);
						ptr22[5] = (byte)((object)values[5 + index]);
						ptr22[6] = (byte)((object)values[6 + index]);
						ptr22[7] = (byte)((object)values[7 + index]);
						ptr22[8] = (byte)((object)values[8 + index]);
						ptr22[9] = (byte)((object)values[9 + index]);
						ptr22[10] = (byte)((object)values[10 + index]);
						ptr22[11] = (byte)((object)values[11 + index]);
						ptr22[12] = (byte)((object)values[12 + index]);
						ptr22[13] = (byte)((object)values[13 + index]);
						ptr22[14] = (byte)((object)values[14 + index]);
						ptr22[15] = (byte)((object)values[15 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(sbyte))
				{
					fixed (sbyte* ptr23 = &this.register.sbyte_0)
					{
						sbyte* ptr24 = ptr23;
						*ptr24 = (sbyte)((object)values[index]);
						ptr24[1] = (sbyte)((object)values[1 + index]);
						ptr24[2] = (sbyte)((object)values[2 + index]);
						ptr24[3] = (sbyte)((object)values[3 + index]);
						ptr24[4] = (sbyte)((object)values[4 + index]);
						ptr24[5] = (sbyte)((object)values[5 + index]);
						ptr24[6] = (sbyte)((object)values[6 + index]);
						ptr24[7] = (sbyte)((object)values[7 + index]);
						ptr24[8] = (sbyte)((object)values[8 + index]);
						ptr24[9] = (sbyte)((object)values[9 + index]);
						ptr24[10] = (sbyte)((object)values[10 + index]);
						ptr24[11] = (sbyte)((object)values[11 + index]);
						ptr24[12] = (sbyte)((object)values[12 + index]);
						ptr24[13] = (sbyte)((object)values[13 + index]);
						ptr24[14] = (sbyte)((object)values[14 + index]);
						ptr24[15] = (sbyte)((object)values[15 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(ushort))
				{
					fixed (ushort* ptr25 = &this.register.uint16_0)
					{
						ushort* ptr26 = ptr25;
						*ptr26 = (ushort)((object)values[index]);
						ptr26[1] = (ushort)((object)values[1 + index]);
						ptr26[2] = (ushort)((object)values[2 + index]);
						ptr26[3] = (ushort)((object)values[3 + index]);
						ptr26[4] = (ushort)((object)values[4 + index]);
						ptr26[5] = (ushort)((object)values[5 + index]);
						ptr26[6] = (ushort)((object)values[6 + index]);
						ptr26[7] = (ushort)((object)values[7 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(short))
				{
					fixed (short* ptr27 = &this.register.int16_0)
					{
						short* ptr28 = ptr27;
						*ptr28 = (short)((object)values[index]);
						ptr28[1] = (short)((object)values[1 + index]);
						ptr28[2] = (short)((object)values[2 + index]);
						ptr28[3] = (short)((object)values[3 + index]);
						ptr28[4] = (short)((object)values[4 + index]);
						ptr28[5] = (short)((object)values[5 + index]);
						ptr28[6] = (short)((object)values[6 + index]);
						ptr28[7] = (short)((object)values[7 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(uint))
				{
					fixed (uint* ptr29 = &this.register.uint32_0)
					{
						uint* ptr30 = ptr29;
						*ptr30 = (uint)((object)values[index]);
						ptr30[1] = (uint)((object)values[1 + index]);
						ptr30[2] = (uint)((object)values[2 + index]);
						ptr30[3] = (uint)((object)values[3 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(int))
				{
					fixed (int* ptr31 = &this.register.int32_0)
					{
						int* ptr32 = ptr31;
						*ptr32 = (int)((object)values[index]);
						ptr32[1] = (int)((object)values[1 + index]);
						ptr32[2] = (int)((object)values[2 + index]);
						ptr32[3] = (int)((object)values[3 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(ulong))
				{
					fixed (ulong* ptr33 = &this.register.uint64_0)
					{
						ulong* ptr34 = ptr33;
						*ptr34 = (ulong)((object)values[index]);
						ptr34[1] = (ulong)((object)values[1 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(long))
				{
					fixed (long* ptr35 = &this.register.int64_0)
					{
						long* ptr36 = ptr35;
						*ptr36 = (long)((object)values[index]);
						ptr36[1] = (long)((object)values[1 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(float))
				{
					fixed (float* ptr37 = &this.register.single_0)
					{
						float* ptr38 = ptr37;
						*ptr38 = (float)((object)values[index]);
						ptr38[1] = (float)((object)values[1 + index]);
						ptr38[2] = (float)((object)values[2 + index]);
						ptr38[3] = (float)((object)values[3 + index]);
					}
					return;
				}
				if (typeof(T) == typeof(double))
				{
					fixed (double* ptr39 = &this.register.double_0)
					{
						double* ptr40 = ptr39;
						*ptr40 = (double)((object)values[index]);
						ptr40[1] = (double)((object)values[1 + index]);
					}
				}
			}
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x0002655C File Offset: 0x0002475C
		internal unsafe Vector(void* dataPointer)
		{
			this = new Vector<T>(dataPointer, 0);
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x00026568 File Offset: 0x00024768
		internal unsafe Vector(void* dataPointer, int offset)
		{
			this = default(Vector<T>);
			if (typeof(T) == typeof(byte))
			{
				byte* ptr = (byte*)dataPointer + offset;
				fixed (byte* ptr2 = &this.register.byte_0)
				{
					byte* ptr3 = ptr2;
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr3[i] = ptr[i];
					}
				}
				return;
			}
			if (typeof(T) == typeof(sbyte))
			{
				sbyte* ptr4 = (sbyte*)((byte*)dataPointer + offset);
				fixed (sbyte* ptr5 = &this.register.sbyte_0)
				{
					sbyte* ptr6 = ptr5;
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr6[j] = ptr4[j];
					}
				}
				return;
			}
			if (typeof(T) == typeof(ushort))
			{
				ushort* ptr7 = (ushort*)((byte*)dataPointer + (IntPtr)offset * 2);
				fixed (ushort* ptr8 = &this.register.uint16_0)
				{
					ushort* ptr9 = ptr8;
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr9[k] = ptr7[k];
					}
				}
				return;
			}
			if (typeof(T) == typeof(short))
			{
				short* ptr10 = (short*)((byte*)dataPointer + (IntPtr)offset * 2);
				fixed (short* ptr11 = &this.register.int16_0)
				{
					short* ptr12 = ptr11;
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr12[l] = ptr10[l];
					}
				}
				return;
			}
			if (typeof(T) == typeof(uint))
			{
				uint* ptr13 = (uint*)((byte*)dataPointer + (IntPtr)offset * 4);
				fixed (uint* ptr14 = &this.register.uint32_0)
				{
					uint* ptr15 = ptr14;
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr15[m] = ptr13[m];
					}
				}
				return;
			}
			if (typeof(T) == typeof(int))
			{
				int* ptr16 = (int*)((byte*)dataPointer + (IntPtr)offset * 4);
				fixed (int* ptr17 = &this.register.int32_0)
				{
					int* ptr18 = ptr17;
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr18[n] = ptr16[n];
					}
				}
				return;
			}
			if (typeof(T) == typeof(ulong))
			{
				ulong* ptr19 = (ulong*)((byte*)dataPointer + (IntPtr)offset * 8);
				fixed (ulong* ptr20 = &this.register.uint64_0)
				{
					ulong* ptr21 = ptr20;
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr21[num] = ptr19[num];
					}
				}
				return;
			}
			if (typeof(T) == typeof(long))
			{
				long* ptr22 = (long*)((byte*)dataPointer + (IntPtr)offset * 8);
				fixed (long* ptr23 = &this.register.int64_0)
				{
					long* ptr24 = ptr23;
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr24[num2] = ptr22[num2];
					}
				}
				return;
			}
			if (typeof(T) == typeof(float))
			{
				float* ptr25 = (float*)((byte*)dataPointer + (IntPtr)offset * 4);
				fixed (float* ptr26 = &this.register.single_0)
				{
					float* ptr27 = ptr26;
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr27[num3] = ptr25[num3];
					}
				}
				return;
			}
			if (typeof(T) == typeof(double))
			{
				double* ptr28 = (double*)((byte*)dataPointer + (IntPtr)offset * 8);
				fixed (double* ptr29 = &this.register.double_0)
				{
					double* ptr30 = ptr29;
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr30[num4] = ptr28[num4];
					}
				}
				return;
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x00026990 File Offset: 0x00024B90
		private Vector(ref Register existingRegister)
		{
			this.register = existingRegister;
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x000269A0 File Offset: 0x00024BA0
		[Intrinsic]
		public void CopyTo(T[] destination)
		{
			this.CopyTo(destination, 0);
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x000269AC File Offset: 0x00024BAC
		[Intrinsic]
		public unsafe void CopyTo(T[] destination, int startIndex)
		{
			if (destination == null)
			{
				throw new NullReferenceException(System.Numerics.Vectors159800.SR.Arg_NullArgumentNullRef);
			}
			if (startIndex < 0 || startIndex >= destination.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", System.Numerics.Vectors159800.SR.Format(System.Numerics.Vectors159800.SR.Arg_ArgumentOutOfRangeException, startIndex));
			}
			if (destination.Length - startIndex < Vector<T>.Count)
			{
				throw new ArgumentException(System.Numerics.Vectors159800.SR.Format(System.Numerics.Vectors159800.SR.Arg_ElementsInSourceIsGreaterThanDestination, startIndex));
			}
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					byte[] array = (byte[])destination;
					byte[] array2;
					byte* ptr;
					if ((array2 = array) == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[startIndex + i] = (byte)((object)this[i]);
					}
					array2 = null;
					return;
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte[] array3 = (sbyte[])destination;
					sbyte[] array4;
					sbyte* ptr2;
					if ((array4 = array3) == null || array4.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array4[0];
					}
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[startIndex + j] = (sbyte)((object)this[j]);
					}
					array4 = null;
					return;
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort[] array5 = (ushort[])destination;
					ushort[] array6;
					ushort* ptr3;
					if ((array6 = array5) == null || array6.Length == 0)
					{
						ptr3 = null;
					}
					else
					{
						ptr3 = &array6[0];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[startIndex + k] = (ushort)((object)this[k]);
					}
					array6 = null;
					return;
				}
				if (typeof(T) == typeof(short))
				{
					short[] array7 = (short[])destination;
					short[] array8;
					short* ptr4;
					if ((array8 = array7) == null || array8.Length == 0)
					{
						ptr4 = null;
					}
					else
					{
						ptr4 = &array8[0];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[startIndex + l] = (short)((object)this[l]);
					}
					array8 = null;
					return;
				}
				if (typeof(T) == typeof(uint))
				{
					uint[] array9 = (uint[])destination;
					uint[] array10;
					uint* ptr5;
					if ((array10 = array9) == null || array10.Length == 0)
					{
						ptr5 = null;
					}
					else
					{
						ptr5 = &array10[0];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[startIndex + m] = (uint)((object)this[m]);
					}
					array10 = null;
					return;
				}
				if (typeof(T) == typeof(int))
				{
					int[] array11 = (int[])destination;
					int[] array12;
					int* ptr6;
					if ((array12 = array11) == null || array12.Length == 0)
					{
						ptr6 = null;
					}
					else
					{
						ptr6 = &array12[0];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[startIndex + n] = (int)((object)this[n]);
					}
					array12 = null;
					return;
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong[] array13 = (ulong[])destination;
					ulong[] array14;
					ulong* ptr7;
					if ((array14 = array13) == null || array14.Length == 0)
					{
						ptr7 = null;
					}
					else
					{
						ptr7 = &array14[0];
					}
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr7[startIndex + num] = (ulong)((object)this[num]);
					}
					array14 = null;
					return;
				}
				if (typeof(T) == typeof(long))
				{
					long[] array15 = (long[])destination;
					long[] array16;
					long* ptr8;
					if ((array16 = array15) == null || array16.Length == 0)
					{
						ptr8 = null;
					}
					else
					{
						ptr8 = &array16[0];
					}
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr8[startIndex + num2] = (long)((object)this[num2]);
					}
					array16 = null;
					return;
				}
				if (typeof(T) == typeof(float))
				{
					float[] array17 = (float[])destination;
					float[] array18;
					float* ptr9;
					if ((array18 = array17) == null || array18.Length == 0)
					{
						ptr9 = null;
					}
					else
					{
						ptr9 = &array18[0];
					}
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr9[startIndex + num3] = (float)((object)this[num3]);
					}
					array18 = null;
					return;
				}
				if (typeof(T) == typeof(double))
				{
					double[] array19 = (double[])destination;
					double[] array20;
					double* ptr10;
					if ((array20 = array19) == null || array20.Length == 0)
					{
						ptr10 = null;
					}
					else
					{
						ptr10 = &array20[0];
					}
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr10[startIndex + num4] = (double)((object)this[num4]);
					}
					array20 = null;
					return;
				}
			}
			else
			{
				if (typeof(T) == typeof(byte))
				{
					byte[] array21 = (byte[])destination;
					byte[] array2;
					byte* ptr11;
					if ((array2 = array21) == null || array2.Length == 0)
					{
						ptr11 = null;
					}
					else
					{
						ptr11 = &array2[0];
					}
					ptr11[startIndex] = this.register.byte_0;
					ptr11[startIndex + 1] = this.register.byte_1;
					ptr11[startIndex + 2] = this.register.byte_2;
					ptr11[startIndex + 3] = this.register.byte_3;
					ptr11[startIndex + 4] = this.register.byte_4;
					ptr11[startIndex + 5] = this.register.byte_5;
					ptr11[startIndex + 6] = this.register.byte_6;
					ptr11[startIndex + 7] = this.register.byte_7;
					ptr11[startIndex + 8] = this.register.byte_8;
					ptr11[startIndex + 9] = this.register.byte_9;
					ptr11[startIndex + 10] = this.register.byte_10;
					ptr11[startIndex + 11] = this.register.byte_11;
					ptr11[startIndex + 12] = this.register.byte_12;
					ptr11[startIndex + 13] = this.register.byte_13;
					ptr11[startIndex + 14] = this.register.byte_14;
					ptr11[startIndex + 15] = this.register.byte_15;
					array2 = null;
					return;
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte[] array22 = (sbyte[])destination;
					sbyte[] array4;
					sbyte* ptr12;
					if ((array4 = array22) == null || array4.Length == 0)
					{
						ptr12 = null;
					}
					else
					{
						ptr12 = &array4[0];
					}
					ptr12[startIndex] = this.register.sbyte_0;
					ptr12[startIndex + 1] = this.register.sbyte_1;
					ptr12[startIndex + 2] = this.register.sbyte_2;
					ptr12[startIndex + 3] = this.register.sbyte_3;
					ptr12[startIndex + 4] = this.register.sbyte_4;
					ptr12[startIndex + 5] = this.register.sbyte_5;
					ptr12[startIndex + 6] = this.register.sbyte_6;
					ptr12[startIndex + 7] = this.register.sbyte_7;
					ptr12[startIndex + 8] = this.register.sbyte_8;
					ptr12[startIndex + 9] = this.register.sbyte_9;
					ptr12[startIndex + 10] = this.register.sbyte_10;
					ptr12[startIndex + 11] = this.register.sbyte_11;
					ptr12[startIndex + 12] = this.register.sbyte_12;
					ptr12[startIndex + 13] = this.register.sbyte_13;
					ptr12[startIndex + 14] = this.register.sbyte_14;
					ptr12[startIndex + 15] = this.register.sbyte_15;
					array4 = null;
					return;
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort[] array23 = (ushort[])destination;
					ushort[] array6;
					ushort* ptr13;
					if ((array6 = array23) == null || array6.Length == 0)
					{
						ptr13 = null;
					}
					else
					{
						ptr13 = &array6[0];
					}
					ptr13[startIndex] = this.register.uint16_0;
					ptr13[startIndex + 1] = this.register.uint16_1;
					ptr13[startIndex + 2] = this.register.uint16_2;
					ptr13[startIndex + 3] = this.register.uint16_3;
					ptr13[startIndex + 4] = this.register.uint16_4;
					ptr13[startIndex + 5] = this.register.uint16_5;
					ptr13[startIndex + 6] = this.register.uint16_6;
					ptr13[startIndex + 7] = this.register.uint16_7;
					array6 = null;
					return;
				}
				if (typeof(T) == typeof(short))
				{
					short[] array24 = (short[])destination;
					short[] array8;
					short* ptr14;
					if ((array8 = array24) == null || array8.Length == 0)
					{
						ptr14 = null;
					}
					else
					{
						ptr14 = &array8[0];
					}
					ptr14[startIndex] = this.register.int16_0;
					ptr14[startIndex + 1] = this.register.int16_1;
					ptr14[startIndex + 2] = this.register.int16_2;
					ptr14[startIndex + 3] = this.register.int16_3;
					ptr14[startIndex + 4] = this.register.int16_4;
					ptr14[startIndex + 5] = this.register.int16_5;
					ptr14[startIndex + 6] = this.register.int16_6;
					ptr14[startIndex + 7] = this.register.int16_7;
					array8 = null;
					return;
				}
				if (typeof(T) == typeof(uint))
				{
					uint[] array25 = (uint[])destination;
					uint[] array10;
					uint* ptr15;
					if ((array10 = array25) == null || array10.Length == 0)
					{
						ptr15 = null;
					}
					else
					{
						ptr15 = &array10[0];
					}
					ptr15[startIndex] = this.register.uint32_0;
					ptr15[startIndex + 1] = this.register.uint32_1;
					ptr15[startIndex + 2] = this.register.uint32_2;
					ptr15[startIndex + 3] = this.register.uint32_3;
					array10 = null;
					return;
				}
				if (typeof(T) == typeof(int))
				{
					int[] array26 = (int[])destination;
					int[] array12;
					int* ptr16;
					if ((array12 = array26) == null || array12.Length == 0)
					{
						ptr16 = null;
					}
					else
					{
						ptr16 = &array12[0];
					}
					ptr16[startIndex] = this.register.int32_0;
					ptr16[startIndex + 1] = this.register.int32_1;
					ptr16[startIndex + 2] = this.register.int32_2;
					ptr16[startIndex + 3] = this.register.int32_3;
					array12 = null;
					return;
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong[] array27 = (ulong[])destination;
					ulong[] array14;
					ulong* ptr17;
					if ((array14 = array27) == null || array14.Length == 0)
					{
						ptr17 = null;
					}
					else
					{
						ptr17 = &array14[0];
					}
					ptr17[startIndex] = this.register.uint64_0;
					ptr17[startIndex + 1] = this.register.uint64_1;
					array14 = null;
					return;
				}
				if (typeof(T) == typeof(long))
				{
					long[] array28 = (long[])destination;
					long[] array16;
					long* ptr18;
					if ((array16 = array28) == null || array16.Length == 0)
					{
						ptr18 = null;
					}
					else
					{
						ptr18 = &array16[0];
					}
					ptr18[startIndex] = this.register.int64_0;
					ptr18[startIndex + 1] = this.register.int64_1;
					array16 = null;
					return;
				}
				if (typeof(T) == typeof(float))
				{
					float[] array29 = (float[])destination;
					float[] array18;
					float* ptr19;
					if ((array18 = array29) == null || array18.Length == 0)
					{
						ptr19 = null;
					}
					else
					{
						ptr19 = &array18[0];
					}
					ptr19[startIndex] = this.register.single_0;
					ptr19[startIndex + 1] = this.register.single_1;
					ptr19[startIndex + 2] = this.register.single_2;
					ptr19[startIndex + 3] = this.register.single_3;
					array18 = null;
					return;
				}
				if (typeof(T) == typeof(double))
				{
					double[] array30 = (double[])destination;
					double[] array20;
					double* ptr20;
					if ((array20 = array30) == null || array20.Length == 0)
					{
						ptr20 = null;
					}
					else
					{
						ptr20 = &array20[0];
					}
					ptr20[startIndex] = this.register.double_0;
					ptr20[startIndex + 1] = this.register.double_1;
					array20 = null;
				}
			}
		}

		// Token: 0x17000158 RID: 344
		public unsafe T this[int index]
		{
			[Intrinsic]
			get
			{
				if (index >= Vector<T>.Count || index < 0)
				{
					throw new IndexOutOfRangeException(System.Numerics.Vectors159800.SR.Format(System.Numerics.Vectors159800.SR.Arg_ArgumentOutOfRangeException, index));
				}
				if (typeof(T) == typeof(byte))
				{
					fixed (byte* ptr = &this.register.byte_0)
					{
						byte* ptr2 = ptr;
						return (T)((object)ptr2[index]);
					}
				}
				if (typeof(T) == typeof(sbyte))
				{
					fixed (sbyte* ptr3 = &this.register.sbyte_0)
					{
						sbyte* ptr4 = ptr3;
						return (T)((object)ptr4[index]);
					}
				}
				if (typeof(T) == typeof(ushort))
				{
					fixed (ushort* ptr5 = &this.register.uint16_0)
					{
						ushort* ptr6 = ptr5;
						return (T)((object)ptr6[index]);
					}
				}
				if (typeof(T) == typeof(short))
				{
					fixed (short* ptr7 = &this.register.int16_0)
					{
						short* ptr8 = ptr7;
						return (T)((object)ptr8[index]);
					}
				}
				if (typeof(T) == typeof(uint))
				{
					fixed (uint* ptr9 = &this.register.uint32_0)
					{
						uint* ptr10 = ptr9;
						return (T)((object)ptr10[index]);
					}
				}
				if (typeof(T) == typeof(int))
				{
					fixed (int* ptr11 = &this.register.int32_0)
					{
						int* ptr12 = ptr11;
						return (T)((object)ptr12[index]);
					}
				}
				if (typeof(T) == typeof(ulong))
				{
					fixed (ulong* ptr13 = &this.register.uint64_0)
					{
						ulong* ptr14 = ptr13;
						return (T)((object)ptr14[index]);
					}
				}
				if (typeof(T) == typeof(long))
				{
					fixed (long* ptr15 = &this.register.int64_0)
					{
						long* ptr16 = ptr15;
						return (T)((object)ptr16[index]);
					}
				}
				if (typeof(T) == typeof(float))
				{
					fixed (float* ptr17 = &this.register.single_0)
					{
						float* ptr18 = ptr17;
						return (T)((object)ptr18[index]);
					}
				}
				if (typeof(T) == typeof(double))
				{
					fixed (double* ptr19 = &this.register.double_0)
					{
						double* ptr20 = ptr19;
						return (T)((object)ptr20[index]);
					}
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x00027A44 File Offset: 0x00025C44
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is Vector<T> && this.Equals((Vector<T>)obj);
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x00027A60 File Offset: 0x00025C60
		[Intrinsic]
		public bool Equals(Vector<T> other)
		{
			if (Vector.IsHardwareAccelerated)
			{
				for (int i = 0; i < Vector<T>.Count; i++)
				{
					if (!Vector<T>.ScalarEquals(this[i], other[i]))
					{
						return false;
					}
				}
				return true;
			}
			if (typeof(T) == typeof(byte))
			{
				return this.register.byte_0 == other.register.byte_0 && this.register.byte_1 == other.register.byte_1 && this.register.byte_2 == other.register.byte_2 && this.register.byte_3 == other.register.byte_3 && this.register.byte_4 == other.register.byte_4 && this.register.byte_5 == other.register.byte_5 && this.register.byte_6 == other.register.byte_6 && this.register.byte_7 == other.register.byte_7 && this.register.byte_8 == other.register.byte_8 && this.register.byte_9 == other.register.byte_9 && this.register.byte_10 == other.register.byte_10 && this.register.byte_11 == other.register.byte_11 && this.register.byte_12 == other.register.byte_12 && this.register.byte_13 == other.register.byte_13 && this.register.byte_14 == other.register.byte_14 && this.register.byte_15 == other.register.byte_15;
			}
			if (typeof(T) == typeof(sbyte))
			{
				return this.register.sbyte_0 == other.register.sbyte_0 && this.register.sbyte_1 == other.register.sbyte_1 && this.register.sbyte_2 == other.register.sbyte_2 && this.register.sbyte_3 == other.register.sbyte_3 && this.register.sbyte_4 == other.register.sbyte_4 && this.register.sbyte_5 == other.register.sbyte_5 && this.register.sbyte_6 == other.register.sbyte_6 && this.register.sbyte_7 == other.register.sbyte_7 && this.register.sbyte_8 == other.register.sbyte_8 && this.register.sbyte_9 == other.register.sbyte_9 && this.register.sbyte_10 == other.register.sbyte_10 && this.register.sbyte_11 == other.register.sbyte_11 && this.register.sbyte_12 == other.register.sbyte_12 && this.register.sbyte_13 == other.register.sbyte_13 && this.register.sbyte_14 == other.register.sbyte_14 && this.register.sbyte_15 == other.register.sbyte_15;
			}
			if (typeof(T) == typeof(ushort))
			{
				return this.register.uint16_0 == other.register.uint16_0 && this.register.uint16_1 == other.register.uint16_1 && this.register.uint16_2 == other.register.uint16_2 && this.register.uint16_3 == other.register.uint16_3 && this.register.uint16_4 == other.register.uint16_4 && this.register.uint16_5 == other.register.uint16_5 && this.register.uint16_6 == other.register.uint16_6 && this.register.uint16_7 == other.register.uint16_7;
			}
			if (typeof(T) == typeof(short))
			{
				return this.register.int16_0 == other.register.int16_0 && this.register.int16_1 == other.register.int16_1 && this.register.int16_2 == other.register.int16_2 && this.register.int16_3 == other.register.int16_3 && this.register.int16_4 == other.register.int16_4 && this.register.int16_5 == other.register.int16_5 && this.register.int16_6 == other.register.int16_6 && this.register.int16_7 == other.register.int16_7;
			}
			if (typeof(T) == typeof(uint))
			{
				return this.register.uint32_0 == other.register.uint32_0 && this.register.uint32_1 == other.register.uint32_1 && this.register.uint32_2 == other.register.uint32_2 && this.register.uint32_3 == other.register.uint32_3;
			}
			if (typeof(T) == typeof(int))
			{
				return this.register.int32_0 == other.register.int32_0 && this.register.int32_1 == other.register.int32_1 && this.register.int32_2 == other.register.int32_2 && this.register.int32_3 == other.register.int32_3;
			}
			if (typeof(T) == typeof(ulong))
			{
				return this.register.uint64_0 == other.register.uint64_0 && this.register.uint64_1 == other.register.uint64_1;
			}
			if (typeof(T) == typeof(long))
			{
				return this.register.int64_0 == other.register.int64_0 && this.register.int64_1 == other.register.int64_1;
			}
			if (typeof(T) == typeof(float))
			{
				return this.register.single_0 == other.register.single_0 && this.register.single_1 == other.register.single_1 && this.register.single_2 == other.register.single_2 && this.register.single_3 == other.register.single_3;
			}
			if (typeof(T) == typeof(double))
			{
				return this.register.double_0 == other.register.double_0 && this.register.double_1 == other.register.double_1;
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x000282D8 File Offset: 0x000264D8
		public override int GetHashCode()
		{
			int num = 0;
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((byte)((object)this[i])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(sbyte))
				{
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((sbyte)((object)this[j])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(ushort))
				{
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((ushort)((object)this[k])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(short))
				{
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((short)((object)this[l])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(uint))
				{
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((uint)((object)this[m])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(int))
				{
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((int)((object)this[n])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(ulong))
				{
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((ulong)((object)this[num2])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(long))
				{
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((long)((object)this[num3])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(float))
				{
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((float)((object)this[num4])).GetHashCode());
					}
					return num;
				}
				if (typeof(T) == typeof(double))
				{
					for (int num5 = 0; num5 < Vector<T>.Count; num5++)
					{
						num = System.Numerics.Vectors159800.HashHelpers.Combine(num, ((double)((object)this[num5])).GetHashCode());
					}
					return num;
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				if (typeof(T) == typeof(byte))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_0.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_1.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_2.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_3.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_4.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_5.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_6.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_7.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_8.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_9.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_10.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_11.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_12.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_13.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_14.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.byte_15.GetHashCode());
				}
				if (typeof(T) == typeof(sbyte))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_0.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_1.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_2.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_3.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_4.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_5.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_6.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_7.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_8.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_9.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_10.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_11.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_12.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_13.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_14.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.sbyte_15.GetHashCode());
				}
				if (typeof(T) == typeof(ushort))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_0.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_1.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_2.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_3.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_4.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_5.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_6.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint16_7.GetHashCode());
				}
				if (typeof(T) == typeof(short))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_0.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_1.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_2.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_3.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_4.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_5.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_6.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int16_7.GetHashCode());
				}
				if (typeof(T) == typeof(uint))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint32_0.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint32_1.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint32_2.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint32_3.GetHashCode());
				}
				if (typeof(T) == typeof(int))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int32_0.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int32_1.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int32_2.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int32_3.GetHashCode());
				}
				if (typeof(T) == typeof(ulong))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint64_0.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.uint64_1.GetHashCode());
				}
				if (typeof(T) == typeof(long))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int64_0.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.int64_1.GetHashCode());
				}
				if (typeof(T) == typeof(float))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.single_0.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.single_1.GetHashCode());
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.single_2.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.single_3.GetHashCode());
				}
				if (typeof(T) == typeof(double))
				{
					num = System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.double_0.GetHashCode());
					return System.Numerics.Vectors159800.HashHelpers.Combine(num, this.register.double_1.GetHashCode());
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x00028DA8 File Offset: 0x00026FA8
		public override string ToString()
		{
			return this.ToString("G", CultureInfo.CurrentCulture);
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x00028DBC File Offset: 0x00026FBC
		public string ToString(string format)
		{
			return this.ToString(format, CultureInfo.CurrentCulture);
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00028DCC File Offset: 0x00026FCC
		public string ToString(string format, IFormatProvider formatProvider)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string numberGroupSeparator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
			stringBuilder.Append('<');
			for (int i = 0; i < Vector<T>.Count - 1; i++)
			{
				stringBuilder.Append(((IFormattable)((object)this[i])).ToString(format, formatProvider));
				stringBuilder.Append(numberGroupSeparator);
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(((IFormattable)((object)this[Vector<T>.Count - 1])).ToString(format, formatProvider));
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x00028E74 File Offset: 0x00027074
		public unsafe static Vector<T>operator +(Vector<T> left, Vector<T> right)
		{
			if (!Vector.IsHardwareAccelerated)
			{
				Vector<T> vector = default(Vector<T>);
				if (typeof(T) == typeof(byte))
				{
					vector.register.byte_0 = left.register.byte_0 + right.register.byte_0;
					vector.register.byte_1 = left.register.byte_1 + right.register.byte_1;
					vector.register.byte_2 = left.register.byte_2 + right.register.byte_2;
					vector.register.byte_3 = left.register.byte_3 + right.register.byte_3;
					vector.register.byte_4 = left.register.byte_4 + right.register.byte_4;
					vector.register.byte_5 = left.register.byte_5 + right.register.byte_5;
					vector.register.byte_6 = left.register.byte_6 + right.register.byte_6;
					vector.register.byte_7 = left.register.byte_7 + right.register.byte_7;
					vector.register.byte_8 = left.register.byte_8 + right.register.byte_8;
					vector.register.byte_9 = left.register.byte_9 + right.register.byte_9;
					vector.register.byte_10 = left.register.byte_10 + right.register.byte_10;
					vector.register.byte_11 = left.register.byte_11 + right.register.byte_11;
					vector.register.byte_12 = left.register.byte_12 + right.register.byte_12;
					vector.register.byte_13 = left.register.byte_13 + right.register.byte_13;
					vector.register.byte_14 = left.register.byte_14 + right.register.byte_14;
					vector.register.byte_15 = left.register.byte_15 + right.register.byte_15;
				}
				else if (typeof(T) == typeof(sbyte))
				{
					vector.register.sbyte_0 = left.register.sbyte_0 + right.register.sbyte_0;
					vector.register.sbyte_1 = left.register.sbyte_1 + right.register.sbyte_1;
					vector.register.sbyte_2 = left.register.sbyte_2 + right.register.sbyte_2;
					vector.register.sbyte_3 = left.register.sbyte_3 + right.register.sbyte_3;
					vector.register.sbyte_4 = left.register.sbyte_4 + right.register.sbyte_4;
					vector.register.sbyte_5 = left.register.sbyte_5 + right.register.sbyte_5;
					vector.register.sbyte_6 = left.register.sbyte_6 + right.register.sbyte_6;
					vector.register.sbyte_7 = left.register.sbyte_7 + right.register.sbyte_7;
					vector.register.sbyte_8 = left.register.sbyte_8 + right.register.sbyte_8;
					vector.register.sbyte_9 = left.register.sbyte_9 + right.register.sbyte_9;
					vector.register.sbyte_10 = left.register.sbyte_10 + right.register.sbyte_10;
					vector.register.sbyte_11 = left.register.sbyte_11 + right.register.sbyte_11;
					vector.register.sbyte_12 = left.register.sbyte_12 + right.register.sbyte_12;
					vector.register.sbyte_13 = left.register.sbyte_13 + right.register.sbyte_13;
					vector.register.sbyte_14 = left.register.sbyte_14 + right.register.sbyte_14;
					vector.register.sbyte_15 = left.register.sbyte_15 + right.register.sbyte_15;
				}
				else if (typeof(T) == typeof(ushort))
				{
					vector.register.uint16_0 = left.register.uint16_0 + right.register.uint16_0;
					vector.register.uint16_1 = left.register.uint16_1 + right.register.uint16_1;
					vector.register.uint16_2 = left.register.uint16_2 + right.register.uint16_2;
					vector.register.uint16_3 = left.register.uint16_3 + right.register.uint16_3;
					vector.register.uint16_4 = left.register.uint16_4 + right.register.uint16_4;
					vector.register.uint16_5 = left.register.uint16_5 + right.register.uint16_5;
					vector.register.uint16_6 = left.register.uint16_6 + right.register.uint16_6;
					vector.register.uint16_7 = left.register.uint16_7 + right.register.uint16_7;
				}
				else if (typeof(T) == typeof(short))
				{
					vector.register.int16_0 = left.register.int16_0 + right.register.int16_0;
					vector.register.int16_1 = left.register.int16_1 + right.register.int16_1;
					vector.register.int16_2 = left.register.int16_2 + right.register.int16_2;
					vector.register.int16_3 = left.register.int16_3 + right.register.int16_3;
					vector.register.int16_4 = left.register.int16_4 + right.register.int16_4;
					vector.register.int16_5 = left.register.int16_5 + right.register.int16_5;
					vector.register.int16_6 = left.register.int16_6 + right.register.int16_6;
					vector.register.int16_7 = left.register.int16_7 + right.register.int16_7;
				}
				else if (typeof(T) == typeof(uint))
				{
					vector.register.uint32_0 = left.register.uint32_0 + right.register.uint32_0;
					vector.register.uint32_1 = left.register.uint32_1 + right.register.uint32_1;
					vector.register.uint32_2 = left.register.uint32_2 + right.register.uint32_2;
					vector.register.uint32_3 = left.register.uint32_3 + right.register.uint32_3;
				}
				else if (typeof(T) == typeof(int))
				{
					vector.register.int32_0 = left.register.int32_0 + right.register.int32_0;
					vector.register.int32_1 = left.register.int32_1 + right.register.int32_1;
					vector.register.int32_2 = left.register.int32_2 + right.register.int32_2;
					vector.register.int32_3 = left.register.int32_3 + right.register.int32_3;
				}
				else if (typeof(T) == typeof(ulong))
				{
					vector.register.uint64_0 = left.register.uint64_0 + right.register.uint64_0;
					vector.register.uint64_1 = left.register.uint64_1 + right.register.uint64_1;
				}
				else if (typeof(T) == typeof(long))
				{
					vector.register.int64_0 = left.register.int64_0 + right.register.int64_0;
					vector.register.int64_1 = left.register.int64_1 + right.register.int64_1;
				}
				else if (typeof(T) == typeof(float))
				{
					vector.register.single_0 = left.register.single_0 + right.register.single_0;
					vector.register.single_1 = left.register.single_1 + right.register.single_1;
					vector.register.single_2 = left.register.single_2 + right.register.single_2;
					vector.register.single_3 = left.register.single_3 + right.register.single_3;
				}
				else if (typeof(T) == typeof(double))
				{
					vector.register.double_0 = left.register.double_0 + right.register.double_0;
					vector.register.double_1 = left.register.double_1 + right.register.double_1;
				}
				return vector;
			}
			if (typeof(T) == typeof(byte))
			{
				byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
				for (int i = 0; i < Vector<T>.Count; i++)
				{
					ptr[i] = (byte)((object)Vector<T>.ScalarAdd(left[i], right[i]));
				}
				return new Vector<T>((void*)ptr);
			}
			if (typeof(T) == typeof(sbyte))
			{
				sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
				for (int j = 0; j < Vector<T>.Count; j++)
				{
					ptr2[j] = (sbyte)((object)Vector<T>.ScalarAdd(left[j], right[j]));
				}
				return new Vector<T>((void*)ptr2);
			}
			if (typeof(T) == typeof(ushort))
			{
				ushort* ptr3;
				checked
				{
					ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int k = 0; k < Vector<T>.Count; k++)
				{
					ptr3[k] = (ushort)((object)Vector<T>.ScalarAdd(left[k], right[k]));
				}
				return new Vector<T>((void*)ptr3);
			}
			if (typeof(T) == typeof(short))
			{
				short* ptr4;
				checked
				{
					ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int l = 0; l < Vector<T>.Count; l++)
				{
					ptr4[l] = (short)((object)Vector<T>.ScalarAdd(left[l], right[l]));
				}
				return new Vector<T>((void*)ptr4);
			}
			if (typeof(T) == typeof(uint))
			{
				uint* ptr5;
				checked
				{
					ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int m = 0; m < Vector<T>.Count; m++)
				{
					ptr5[m] = (uint)((object)Vector<T>.ScalarAdd(left[m], right[m]));
				}
				return new Vector<T>((void*)ptr5);
			}
			if (typeof(T) == typeof(int))
			{
				int* ptr6;
				checked
				{
					ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int n = 0; n < Vector<T>.Count; n++)
				{
					ptr6[n] = (int)((object)Vector<T>.ScalarAdd(left[n], right[n]));
				}
				return new Vector<T>((void*)ptr6);
			}
			if (typeof(T) == typeof(ulong))
			{
				ulong* ptr7;
				checked
				{
					ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num = 0; num < Vector<T>.Count; num++)
				{
					ptr7[num] = (ulong)((object)Vector<T>.ScalarAdd(left[num], right[num]));
				}
				return new Vector<T>((void*)ptr7);
			}
			if (typeof(T) == typeof(long))
			{
				long* ptr8;
				checked
				{
					ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num2 = 0; num2 < Vector<T>.Count; num2++)
				{
					ptr8[num2] = (long)((object)Vector<T>.ScalarAdd(left[num2], right[num2]));
				}
				return new Vector<T>((void*)ptr8);
			}
			if (typeof(T) == typeof(float))
			{
				float* ptr9;
				checked
				{
					ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int num3 = 0; num3 < Vector<T>.Count; num3++)
				{
					ptr9[num3] = (float)((object)Vector<T>.ScalarAdd(left[num3], right[num3]));
				}
				return new Vector<T>((void*)ptr9);
			}
			if (typeof(T) == typeof(double))
			{
				double* ptr10;
				checked
				{
					ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num4 = 0; num4 < Vector<T>.Count; num4++)
				{
					ptr10[num4] = (double)((object)Vector<T>.ScalarAdd(left[num4], right[num4]));
				}
				return new Vector<T>((void*)ptr10);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x00029D9C File Offset: 0x00027F9C
		public unsafe static Vector<T>operator -(Vector<T> left, Vector<T> right)
		{
			if (!Vector.IsHardwareAccelerated)
			{
				Vector<T> vector = default(Vector<T>);
				if (typeof(T) == typeof(byte))
				{
					vector.register.byte_0 = left.register.byte_0 - right.register.byte_0;
					vector.register.byte_1 = left.register.byte_1 - right.register.byte_1;
					vector.register.byte_2 = left.register.byte_2 - right.register.byte_2;
					vector.register.byte_3 = left.register.byte_3 - right.register.byte_3;
					vector.register.byte_4 = left.register.byte_4 - right.register.byte_4;
					vector.register.byte_5 = left.register.byte_5 - right.register.byte_5;
					vector.register.byte_6 = left.register.byte_6 - right.register.byte_6;
					vector.register.byte_7 = left.register.byte_7 - right.register.byte_7;
					vector.register.byte_8 = left.register.byte_8 - right.register.byte_8;
					vector.register.byte_9 = left.register.byte_9 - right.register.byte_9;
					vector.register.byte_10 = left.register.byte_10 - right.register.byte_10;
					vector.register.byte_11 = left.register.byte_11 - right.register.byte_11;
					vector.register.byte_12 = left.register.byte_12 - right.register.byte_12;
					vector.register.byte_13 = left.register.byte_13 - right.register.byte_13;
					vector.register.byte_14 = left.register.byte_14 - right.register.byte_14;
					vector.register.byte_15 = left.register.byte_15 - right.register.byte_15;
				}
				else if (typeof(T) == typeof(sbyte))
				{
					vector.register.sbyte_0 = left.register.sbyte_0 - right.register.sbyte_0;
					vector.register.sbyte_1 = left.register.sbyte_1 - right.register.sbyte_1;
					vector.register.sbyte_2 = left.register.sbyte_2 - right.register.sbyte_2;
					vector.register.sbyte_3 = left.register.sbyte_3 - right.register.sbyte_3;
					vector.register.sbyte_4 = left.register.sbyte_4 - right.register.sbyte_4;
					vector.register.sbyte_5 = left.register.sbyte_5 - right.register.sbyte_5;
					vector.register.sbyte_6 = left.register.sbyte_6 - right.register.sbyte_6;
					vector.register.sbyte_7 = left.register.sbyte_7 - right.register.sbyte_7;
					vector.register.sbyte_8 = left.register.sbyte_8 - right.register.sbyte_8;
					vector.register.sbyte_9 = left.register.sbyte_9 - right.register.sbyte_9;
					vector.register.sbyte_10 = left.register.sbyte_10 - right.register.sbyte_10;
					vector.register.sbyte_11 = left.register.sbyte_11 - right.register.sbyte_11;
					vector.register.sbyte_12 = left.register.sbyte_12 - right.register.sbyte_12;
					vector.register.sbyte_13 = left.register.sbyte_13 - right.register.sbyte_13;
					vector.register.sbyte_14 = left.register.sbyte_14 - right.register.sbyte_14;
					vector.register.sbyte_15 = left.register.sbyte_15 - right.register.sbyte_15;
				}
				else if (typeof(T) == typeof(ushort))
				{
					vector.register.uint16_0 = left.register.uint16_0 - right.register.uint16_0;
					vector.register.uint16_1 = left.register.uint16_1 - right.register.uint16_1;
					vector.register.uint16_2 = left.register.uint16_2 - right.register.uint16_2;
					vector.register.uint16_3 = left.register.uint16_3 - right.register.uint16_3;
					vector.register.uint16_4 = left.register.uint16_4 - right.register.uint16_4;
					vector.register.uint16_5 = left.register.uint16_5 - right.register.uint16_5;
					vector.register.uint16_6 = left.register.uint16_6 - right.register.uint16_6;
					vector.register.uint16_7 = left.register.uint16_7 - right.register.uint16_7;
				}
				else if (typeof(T) == typeof(short))
				{
					vector.register.int16_0 = left.register.int16_0 - right.register.int16_0;
					vector.register.int16_1 = left.register.int16_1 - right.register.int16_1;
					vector.register.int16_2 = left.register.int16_2 - right.register.int16_2;
					vector.register.int16_3 = left.register.int16_3 - right.register.int16_3;
					vector.register.int16_4 = left.register.int16_4 - right.register.int16_4;
					vector.register.int16_5 = left.register.int16_5 - right.register.int16_5;
					vector.register.int16_6 = left.register.int16_6 - right.register.int16_6;
					vector.register.int16_7 = left.register.int16_7 - right.register.int16_7;
				}
				else if (typeof(T) == typeof(uint))
				{
					vector.register.uint32_0 = left.register.uint32_0 - right.register.uint32_0;
					vector.register.uint32_1 = left.register.uint32_1 - right.register.uint32_1;
					vector.register.uint32_2 = left.register.uint32_2 - right.register.uint32_2;
					vector.register.uint32_3 = left.register.uint32_3 - right.register.uint32_3;
				}
				else if (typeof(T) == typeof(int))
				{
					vector.register.int32_0 = left.register.int32_0 - right.register.int32_0;
					vector.register.int32_1 = left.register.int32_1 - right.register.int32_1;
					vector.register.int32_2 = left.register.int32_2 - right.register.int32_2;
					vector.register.int32_3 = left.register.int32_3 - right.register.int32_3;
				}
				else if (typeof(T) == typeof(ulong))
				{
					vector.register.uint64_0 = left.register.uint64_0 - right.register.uint64_0;
					vector.register.uint64_1 = left.register.uint64_1 - right.register.uint64_1;
				}
				else if (typeof(T) == typeof(long))
				{
					vector.register.int64_0 = left.register.int64_0 - right.register.int64_0;
					vector.register.int64_1 = left.register.int64_1 - right.register.int64_1;
				}
				else if (typeof(T) == typeof(float))
				{
					vector.register.single_0 = left.register.single_0 - right.register.single_0;
					vector.register.single_1 = left.register.single_1 - right.register.single_1;
					vector.register.single_2 = left.register.single_2 - right.register.single_2;
					vector.register.single_3 = left.register.single_3 - right.register.single_3;
				}
				else if (typeof(T) == typeof(double))
				{
					vector.register.double_0 = left.register.double_0 - right.register.double_0;
					vector.register.double_1 = left.register.double_1 - right.register.double_1;
				}
				return vector;
			}
			if (typeof(T) == typeof(byte))
			{
				byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
				for (int i = 0; i < Vector<T>.Count; i++)
				{
					ptr[i] = (byte)((object)Vector<T>.ScalarSubtract(left[i], right[i]));
				}
				return new Vector<T>((void*)ptr);
			}
			if (typeof(T) == typeof(sbyte))
			{
				sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
				for (int j = 0; j < Vector<T>.Count; j++)
				{
					ptr2[j] = (sbyte)((object)Vector<T>.ScalarSubtract(left[j], right[j]));
				}
				return new Vector<T>((void*)ptr2);
			}
			if (typeof(T) == typeof(ushort))
			{
				ushort* ptr3;
				checked
				{
					ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int k = 0; k < Vector<T>.Count; k++)
				{
					ptr3[k] = (ushort)((object)Vector<T>.ScalarSubtract(left[k], right[k]));
				}
				return new Vector<T>((void*)ptr3);
			}
			if (typeof(T) == typeof(short))
			{
				short* ptr4;
				checked
				{
					ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int l = 0; l < Vector<T>.Count; l++)
				{
					ptr4[l] = (short)((object)Vector<T>.ScalarSubtract(left[l], right[l]));
				}
				return new Vector<T>((void*)ptr4);
			}
			if (typeof(T) == typeof(uint))
			{
				uint* ptr5;
				checked
				{
					ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int m = 0; m < Vector<T>.Count; m++)
				{
					ptr5[m] = (uint)((object)Vector<T>.ScalarSubtract(left[m], right[m]));
				}
				return new Vector<T>((void*)ptr5);
			}
			if (typeof(T) == typeof(int))
			{
				int* ptr6;
				checked
				{
					ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int n = 0; n < Vector<T>.Count; n++)
				{
					ptr6[n] = (int)((object)Vector<T>.ScalarSubtract(left[n], right[n]));
				}
				return new Vector<T>((void*)ptr6);
			}
			if (typeof(T) == typeof(ulong))
			{
				ulong* ptr7;
				checked
				{
					ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num = 0; num < Vector<T>.Count; num++)
				{
					ptr7[num] = (ulong)((object)Vector<T>.ScalarSubtract(left[num], right[num]));
				}
				return new Vector<T>((void*)ptr7);
			}
			if (typeof(T) == typeof(long))
			{
				long* ptr8;
				checked
				{
					ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num2 = 0; num2 < Vector<T>.Count; num2++)
				{
					ptr8[num2] = (long)((object)Vector<T>.ScalarSubtract(left[num2], right[num2]));
				}
				return new Vector<T>((void*)ptr8);
			}
			if (typeof(T) == typeof(float))
			{
				float* ptr9;
				checked
				{
					ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int num3 = 0; num3 < Vector<T>.Count; num3++)
				{
					ptr9[num3] = (float)((object)Vector<T>.ScalarSubtract(left[num3], right[num3]));
				}
				return new Vector<T>((void*)ptr9);
			}
			if (typeof(T) == typeof(double))
			{
				double* ptr10;
				checked
				{
					ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num4 = 0; num4 < Vector<T>.Count; num4++)
				{
					ptr10[num4] = (double)((object)Vector<T>.ScalarSubtract(left[num4], right[num4]));
				}
				return new Vector<T>((void*)ptr10);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x0002ACC4 File Offset: 0x00028EC4
		public unsafe static Vector<T>operator *(Vector<T> left, Vector<T> right)
		{
			if (!Vector.IsHardwareAccelerated)
			{
				Vector<T> vector = default(Vector<T>);
				if (typeof(T) == typeof(byte))
				{
					vector.register.byte_0 = left.register.byte_0 * right.register.byte_0;
					vector.register.byte_1 = left.register.byte_1 * right.register.byte_1;
					vector.register.byte_2 = left.register.byte_2 * right.register.byte_2;
					vector.register.byte_3 = left.register.byte_3 * right.register.byte_3;
					vector.register.byte_4 = left.register.byte_4 * right.register.byte_4;
					vector.register.byte_5 = left.register.byte_5 * right.register.byte_5;
					vector.register.byte_6 = left.register.byte_6 * right.register.byte_6;
					vector.register.byte_7 = left.register.byte_7 * right.register.byte_7;
					vector.register.byte_8 = left.register.byte_8 * right.register.byte_8;
					vector.register.byte_9 = left.register.byte_9 * right.register.byte_9;
					vector.register.byte_10 = left.register.byte_10 * right.register.byte_10;
					vector.register.byte_11 = left.register.byte_11 * right.register.byte_11;
					vector.register.byte_12 = left.register.byte_12 * right.register.byte_12;
					vector.register.byte_13 = left.register.byte_13 * right.register.byte_13;
					vector.register.byte_14 = left.register.byte_14 * right.register.byte_14;
					vector.register.byte_15 = left.register.byte_15 * right.register.byte_15;
				}
				else if (typeof(T) == typeof(sbyte))
				{
					vector.register.sbyte_0 = left.register.sbyte_0 * right.register.sbyte_0;
					vector.register.sbyte_1 = left.register.sbyte_1 * right.register.sbyte_1;
					vector.register.sbyte_2 = left.register.sbyte_2 * right.register.sbyte_2;
					vector.register.sbyte_3 = left.register.sbyte_3 * right.register.sbyte_3;
					vector.register.sbyte_4 = left.register.sbyte_4 * right.register.sbyte_4;
					vector.register.sbyte_5 = left.register.sbyte_5 * right.register.sbyte_5;
					vector.register.sbyte_6 = left.register.sbyte_6 * right.register.sbyte_6;
					vector.register.sbyte_7 = left.register.sbyte_7 * right.register.sbyte_7;
					vector.register.sbyte_8 = left.register.sbyte_8 * right.register.sbyte_8;
					vector.register.sbyte_9 = left.register.sbyte_9 * right.register.sbyte_9;
					vector.register.sbyte_10 = left.register.sbyte_10 * right.register.sbyte_10;
					vector.register.sbyte_11 = left.register.sbyte_11 * right.register.sbyte_11;
					vector.register.sbyte_12 = left.register.sbyte_12 * right.register.sbyte_12;
					vector.register.sbyte_13 = left.register.sbyte_13 * right.register.sbyte_13;
					vector.register.sbyte_14 = left.register.sbyte_14 * right.register.sbyte_14;
					vector.register.sbyte_15 = left.register.sbyte_15 * right.register.sbyte_15;
				}
				else if (typeof(T) == typeof(ushort))
				{
					vector.register.uint16_0 = left.register.uint16_0 * right.register.uint16_0;
					vector.register.uint16_1 = left.register.uint16_1 * right.register.uint16_1;
					vector.register.uint16_2 = left.register.uint16_2 * right.register.uint16_2;
					vector.register.uint16_3 = left.register.uint16_3 * right.register.uint16_3;
					vector.register.uint16_4 = left.register.uint16_4 * right.register.uint16_4;
					vector.register.uint16_5 = left.register.uint16_5 * right.register.uint16_5;
					vector.register.uint16_6 = left.register.uint16_6 * right.register.uint16_6;
					vector.register.uint16_7 = left.register.uint16_7 * right.register.uint16_7;
				}
				else if (typeof(T) == typeof(short))
				{
					vector.register.int16_0 = left.register.int16_0 * right.register.int16_0;
					vector.register.int16_1 = left.register.int16_1 * right.register.int16_1;
					vector.register.int16_2 = left.register.int16_2 * right.register.int16_2;
					vector.register.int16_3 = left.register.int16_3 * right.register.int16_3;
					vector.register.int16_4 = left.register.int16_4 * right.register.int16_4;
					vector.register.int16_5 = left.register.int16_5 * right.register.int16_5;
					vector.register.int16_6 = left.register.int16_6 * right.register.int16_6;
					vector.register.int16_7 = left.register.int16_7 * right.register.int16_7;
				}
				else if (typeof(T) == typeof(uint))
				{
					vector.register.uint32_0 = left.register.uint32_0 * right.register.uint32_0;
					vector.register.uint32_1 = left.register.uint32_1 * right.register.uint32_1;
					vector.register.uint32_2 = left.register.uint32_2 * right.register.uint32_2;
					vector.register.uint32_3 = left.register.uint32_3 * right.register.uint32_3;
				}
				else if (typeof(T) == typeof(int))
				{
					vector.register.int32_0 = left.register.int32_0 * right.register.int32_0;
					vector.register.int32_1 = left.register.int32_1 * right.register.int32_1;
					vector.register.int32_2 = left.register.int32_2 * right.register.int32_2;
					vector.register.int32_3 = left.register.int32_3 * right.register.int32_3;
				}
				else if (typeof(T) == typeof(ulong))
				{
					vector.register.uint64_0 = left.register.uint64_0 * right.register.uint64_0;
					vector.register.uint64_1 = left.register.uint64_1 * right.register.uint64_1;
				}
				else if (typeof(T) == typeof(long))
				{
					vector.register.int64_0 = left.register.int64_0 * right.register.int64_0;
					vector.register.int64_1 = left.register.int64_1 * right.register.int64_1;
				}
				else if (typeof(T) == typeof(float))
				{
					vector.register.single_0 = left.register.single_0 * right.register.single_0;
					vector.register.single_1 = left.register.single_1 * right.register.single_1;
					vector.register.single_2 = left.register.single_2 * right.register.single_2;
					vector.register.single_3 = left.register.single_3 * right.register.single_3;
				}
				else if (typeof(T) == typeof(double))
				{
					vector.register.double_0 = left.register.double_0 * right.register.double_0;
					vector.register.double_1 = left.register.double_1 * right.register.double_1;
				}
				return vector;
			}
			if (typeof(T) == typeof(byte))
			{
				byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
				for (int i = 0; i < Vector<T>.Count; i++)
				{
					ptr[i] = (byte)((object)Vector<T>.ScalarMultiply(left[i], right[i]));
				}
				return new Vector<T>((void*)ptr);
			}
			if (typeof(T) == typeof(sbyte))
			{
				sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
				for (int j = 0; j < Vector<T>.Count; j++)
				{
					ptr2[j] = (sbyte)((object)Vector<T>.ScalarMultiply(left[j], right[j]));
				}
				return new Vector<T>((void*)ptr2);
			}
			if (typeof(T) == typeof(ushort))
			{
				ushort* ptr3;
				checked
				{
					ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int k = 0; k < Vector<T>.Count; k++)
				{
					ptr3[k] = (ushort)((object)Vector<T>.ScalarMultiply(left[k], right[k]));
				}
				return new Vector<T>((void*)ptr3);
			}
			if (typeof(T) == typeof(short))
			{
				short* ptr4;
				checked
				{
					ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int l = 0; l < Vector<T>.Count; l++)
				{
					ptr4[l] = (short)((object)Vector<T>.ScalarMultiply(left[l], right[l]));
				}
				return new Vector<T>((void*)ptr4);
			}
			if (typeof(T) == typeof(uint))
			{
				uint* ptr5;
				checked
				{
					ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int m = 0; m < Vector<T>.Count; m++)
				{
					ptr5[m] = (uint)((object)Vector<T>.ScalarMultiply(left[m], right[m]));
				}
				return new Vector<T>((void*)ptr5);
			}
			if (typeof(T) == typeof(int))
			{
				int* ptr6;
				checked
				{
					ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int n = 0; n < Vector<T>.Count; n++)
				{
					ptr6[n] = (int)((object)Vector<T>.ScalarMultiply(left[n], right[n]));
				}
				return new Vector<T>((void*)ptr6);
			}
			if (typeof(T) == typeof(ulong))
			{
				ulong* ptr7;
				checked
				{
					ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num = 0; num < Vector<T>.Count; num++)
				{
					ptr7[num] = (ulong)((object)Vector<T>.ScalarMultiply(left[num], right[num]));
				}
				return new Vector<T>((void*)ptr7);
			}
			if (typeof(T) == typeof(long))
			{
				long* ptr8;
				checked
				{
					ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num2 = 0; num2 < Vector<T>.Count; num2++)
				{
					ptr8[num2] = (long)((object)Vector<T>.ScalarMultiply(left[num2], right[num2]));
				}
				return new Vector<T>((void*)ptr8);
			}
			if (typeof(T) == typeof(float))
			{
				float* ptr9;
				checked
				{
					ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int num3 = 0; num3 < Vector<T>.Count; num3++)
				{
					ptr9[num3] = (float)((object)Vector<T>.ScalarMultiply(left[num3], right[num3]));
				}
				return new Vector<T>((void*)ptr9);
			}
			if (typeof(T) == typeof(double))
			{
				double* ptr10;
				checked
				{
					ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num4 = 0; num4 < Vector<T>.Count; num4++)
				{
					ptr10[num4] = (double)((object)Vector<T>.ScalarMultiply(left[num4], right[num4]));
				}
				return new Vector<T>((void*)ptr10);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0002BBEC File Offset: 0x00029DEC
		public static Vector<T>operator *(Vector<T> value, T factor)
		{
			if (Vector.IsHardwareAccelerated)
			{
				return new Vector<T>(factor) * value;
			}
			Vector<T> vector = default(Vector<T>);
			if (typeof(T) == typeof(byte))
			{
				vector.register.byte_0 = value.register.byte_0 * (byte)((object)factor);
				vector.register.byte_1 = value.register.byte_1 * (byte)((object)factor);
				vector.register.byte_2 = value.register.byte_2 * (byte)((object)factor);
				vector.register.byte_3 = value.register.byte_3 * (byte)((object)factor);
				vector.register.byte_4 = value.register.byte_4 * (byte)((object)factor);
				vector.register.byte_5 = value.register.byte_5 * (byte)((object)factor);
				vector.register.byte_6 = value.register.byte_6 * (byte)((object)factor);
				vector.register.byte_7 = value.register.byte_7 * (byte)((object)factor);
				vector.register.byte_8 = value.register.byte_8 * (byte)((object)factor);
				vector.register.byte_9 = value.register.byte_9 * (byte)((object)factor);
				vector.register.byte_10 = value.register.byte_10 * (byte)((object)factor);
				vector.register.byte_11 = value.register.byte_11 * (byte)((object)factor);
				vector.register.byte_12 = value.register.byte_12 * (byte)((object)factor);
				vector.register.byte_13 = value.register.byte_13 * (byte)((object)factor);
				vector.register.byte_14 = value.register.byte_14 * (byte)((object)factor);
				vector.register.byte_15 = value.register.byte_15 * (byte)((object)factor);
			}
			else if (typeof(T) == typeof(sbyte))
			{
				vector.register.sbyte_0 = value.register.sbyte_0 * (sbyte)((object)factor);
				vector.register.sbyte_1 = value.register.sbyte_1 * (sbyte)((object)factor);
				vector.register.sbyte_2 = value.register.sbyte_2 * (sbyte)((object)factor);
				vector.register.sbyte_3 = value.register.sbyte_3 * (sbyte)((object)factor);
				vector.register.sbyte_4 = value.register.sbyte_4 * (sbyte)((object)factor);
				vector.register.sbyte_5 = value.register.sbyte_5 * (sbyte)((object)factor);
				vector.register.sbyte_6 = value.register.sbyte_6 * (sbyte)((object)factor);
				vector.register.sbyte_7 = value.register.sbyte_7 * (sbyte)((object)factor);
				vector.register.sbyte_8 = value.register.sbyte_8 * (sbyte)((object)factor);
				vector.register.sbyte_9 = value.register.sbyte_9 * (sbyte)((object)factor);
				vector.register.sbyte_10 = value.register.sbyte_10 * (sbyte)((object)factor);
				vector.register.sbyte_11 = value.register.sbyte_11 * (sbyte)((object)factor);
				vector.register.sbyte_12 = value.register.sbyte_12 * (sbyte)((object)factor);
				vector.register.sbyte_13 = value.register.sbyte_13 * (sbyte)((object)factor);
				vector.register.sbyte_14 = value.register.sbyte_14 * (sbyte)((object)factor);
				vector.register.sbyte_15 = value.register.sbyte_15 * (sbyte)((object)factor);
			}
			else if (typeof(T) == typeof(ushort))
			{
				vector.register.uint16_0 = value.register.uint16_0 * (ushort)((object)factor);
				vector.register.uint16_1 = value.register.uint16_1 * (ushort)((object)factor);
				vector.register.uint16_2 = value.register.uint16_2 * (ushort)((object)factor);
				vector.register.uint16_3 = value.register.uint16_3 * (ushort)((object)factor);
				vector.register.uint16_4 = value.register.uint16_4 * (ushort)((object)factor);
				vector.register.uint16_5 = value.register.uint16_5 * (ushort)((object)factor);
				vector.register.uint16_6 = value.register.uint16_6 * (ushort)((object)factor);
				vector.register.uint16_7 = value.register.uint16_7 * (ushort)((object)factor);
			}
			else if (typeof(T) == typeof(short))
			{
				vector.register.int16_0 = value.register.int16_0 * (short)((object)factor);
				vector.register.int16_1 = value.register.int16_1 * (short)((object)factor);
				vector.register.int16_2 = value.register.int16_2 * (short)((object)factor);
				vector.register.int16_3 = value.register.int16_3 * (short)((object)factor);
				vector.register.int16_4 = value.register.int16_4 * (short)((object)factor);
				vector.register.int16_5 = value.register.int16_5 * (short)((object)factor);
				vector.register.int16_6 = value.register.int16_6 * (short)((object)factor);
				vector.register.int16_7 = value.register.int16_7 * (short)((object)factor);
			}
			else if (typeof(T) == typeof(uint))
			{
				vector.register.uint32_0 = value.register.uint32_0 * (uint)((object)factor);
				vector.register.uint32_1 = value.register.uint32_1 * (uint)((object)factor);
				vector.register.uint32_2 = value.register.uint32_2 * (uint)((object)factor);
				vector.register.uint32_3 = value.register.uint32_3 * (uint)((object)factor);
			}
			else if (typeof(T) == typeof(int))
			{
				vector.register.int32_0 = value.register.int32_0 * (int)((object)factor);
				vector.register.int32_1 = value.register.int32_1 * (int)((object)factor);
				vector.register.int32_2 = value.register.int32_2 * (int)((object)factor);
				vector.register.int32_3 = value.register.int32_3 * (int)((object)factor);
			}
			else if (typeof(T) == typeof(ulong))
			{
				vector.register.uint64_0 = value.register.uint64_0 * (ulong)((object)factor);
				vector.register.uint64_1 = value.register.uint64_1 * (ulong)((object)factor);
			}
			else if (typeof(T) == typeof(long))
			{
				vector.register.int64_0 = value.register.int64_0 * (long)((object)factor);
				vector.register.int64_1 = value.register.int64_1 * (long)((object)factor);
			}
			else if (typeof(T) == typeof(float))
			{
				vector.register.single_0 = value.register.single_0 * (float)((object)factor);
				vector.register.single_1 = value.register.single_1 * (float)((object)factor);
				vector.register.single_2 = value.register.single_2 * (float)((object)factor);
				vector.register.single_3 = value.register.single_3 * (float)((object)factor);
			}
			else if (typeof(T) == typeof(double))
			{
				vector.register.double_0 = value.register.double_0 * (double)((object)factor);
				vector.register.double_1 = value.register.double_1 * (double)((object)factor);
			}
			return vector;
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0002C6B4 File Offset: 0x0002A8B4
		public static Vector<T>operator *(T factor, Vector<T> value)
		{
			if (Vector.IsHardwareAccelerated)
			{
				return new Vector<T>(factor) * value;
			}
			Vector<T> vector = default(Vector<T>);
			if (typeof(T) == typeof(byte))
			{
				vector.register.byte_0 = value.register.byte_0 * (byte)((object)factor);
				vector.register.byte_1 = value.register.byte_1 * (byte)((object)factor);
				vector.register.byte_2 = value.register.byte_2 * (byte)((object)factor);
				vector.register.byte_3 = value.register.byte_3 * (byte)((object)factor);
				vector.register.byte_4 = value.register.byte_4 * (byte)((object)factor);
				vector.register.byte_5 = value.register.byte_5 * (byte)((object)factor);
				vector.register.byte_6 = value.register.byte_6 * (byte)((object)factor);
				vector.register.byte_7 = value.register.byte_7 * (byte)((object)factor);
				vector.register.byte_8 = value.register.byte_8 * (byte)((object)factor);
				vector.register.byte_9 = value.register.byte_9 * (byte)((object)factor);
				vector.register.byte_10 = value.register.byte_10 * (byte)((object)factor);
				vector.register.byte_11 = value.register.byte_11 * (byte)((object)factor);
				vector.register.byte_12 = value.register.byte_12 * (byte)((object)factor);
				vector.register.byte_13 = value.register.byte_13 * (byte)((object)factor);
				vector.register.byte_14 = value.register.byte_14 * (byte)((object)factor);
				vector.register.byte_15 = value.register.byte_15 * (byte)((object)factor);
			}
			else if (typeof(T) == typeof(sbyte))
			{
				vector.register.sbyte_0 = value.register.sbyte_0 * (sbyte)((object)factor);
				vector.register.sbyte_1 = value.register.sbyte_1 * (sbyte)((object)factor);
				vector.register.sbyte_2 = value.register.sbyte_2 * (sbyte)((object)factor);
				vector.register.sbyte_3 = value.register.sbyte_3 * (sbyte)((object)factor);
				vector.register.sbyte_4 = value.register.sbyte_4 * (sbyte)((object)factor);
				vector.register.sbyte_5 = value.register.sbyte_5 * (sbyte)((object)factor);
				vector.register.sbyte_6 = value.register.sbyte_6 * (sbyte)((object)factor);
				vector.register.sbyte_7 = value.register.sbyte_7 * (sbyte)((object)factor);
				vector.register.sbyte_8 = value.register.sbyte_8 * (sbyte)((object)factor);
				vector.register.sbyte_9 = value.register.sbyte_9 * (sbyte)((object)factor);
				vector.register.sbyte_10 = value.register.sbyte_10 * (sbyte)((object)factor);
				vector.register.sbyte_11 = value.register.sbyte_11 * (sbyte)((object)factor);
				vector.register.sbyte_12 = value.register.sbyte_12 * (sbyte)((object)factor);
				vector.register.sbyte_13 = value.register.sbyte_13 * (sbyte)((object)factor);
				vector.register.sbyte_14 = value.register.sbyte_14 * (sbyte)((object)factor);
				vector.register.sbyte_15 = value.register.sbyte_15 * (sbyte)((object)factor);
			}
			else if (typeof(T) == typeof(ushort))
			{
				vector.register.uint16_0 = value.register.uint16_0 * (ushort)((object)factor);
				vector.register.uint16_1 = value.register.uint16_1 * (ushort)((object)factor);
				vector.register.uint16_2 = value.register.uint16_2 * (ushort)((object)factor);
				vector.register.uint16_3 = value.register.uint16_3 * (ushort)((object)factor);
				vector.register.uint16_4 = value.register.uint16_4 * (ushort)((object)factor);
				vector.register.uint16_5 = value.register.uint16_5 * (ushort)((object)factor);
				vector.register.uint16_6 = value.register.uint16_6 * (ushort)((object)factor);
				vector.register.uint16_7 = value.register.uint16_7 * (ushort)((object)factor);
			}
			else if (typeof(T) == typeof(short))
			{
				vector.register.int16_0 = value.register.int16_0 * (short)((object)factor);
				vector.register.int16_1 = value.register.int16_1 * (short)((object)factor);
				vector.register.int16_2 = value.register.int16_2 * (short)((object)factor);
				vector.register.int16_3 = value.register.int16_3 * (short)((object)factor);
				vector.register.int16_4 = value.register.int16_4 * (short)((object)factor);
				vector.register.int16_5 = value.register.int16_5 * (short)((object)factor);
				vector.register.int16_6 = value.register.int16_6 * (short)((object)factor);
				vector.register.int16_7 = value.register.int16_7 * (short)((object)factor);
			}
			else if (typeof(T) == typeof(uint))
			{
				vector.register.uint32_0 = value.register.uint32_0 * (uint)((object)factor);
				vector.register.uint32_1 = value.register.uint32_1 * (uint)((object)factor);
				vector.register.uint32_2 = value.register.uint32_2 * (uint)((object)factor);
				vector.register.uint32_3 = value.register.uint32_3 * (uint)((object)factor);
			}
			else if (typeof(T) == typeof(int))
			{
				vector.register.int32_0 = value.register.int32_0 * (int)((object)factor);
				vector.register.int32_1 = value.register.int32_1 * (int)((object)factor);
				vector.register.int32_2 = value.register.int32_2 * (int)((object)factor);
				vector.register.int32_3 = value.register.int32_3 * (int)((object)factor);
			}
			else if (typeof(T) == typeof(ulong))
			{
				vector.register.uint64_0 = value.register.uint64_0 * (ulong)((object)factor);
				vector.register.uint64_1 = value.register.uint64_1 * (ulong)((object)factor);
			}
			else if (typeof(T) == typeof(long))
			{
				vector.register.int64_0 = value.register.int64_0 * (long)((object)factor);
				vector.register.int64_1 = value.register.int64_1 * (long)((object)factor);
			}
			else if (typeof(T) == typeof(float))
			{
				vector.register.single_0 = value.register.single_0 * (float)((object)factor);
				vector.register.single_1 = value.register.single_1 * (float)((object)factor);
				vector.register.single_2 = value.register.single_2 * (float)((object)factor);
				vector.register.single_3 = value.register.single_3 * (float)((object)factor);
			}
			else if (typeof(T) == typeof(double))
			{
				vector.register.double_0 = value.register.double_0 * (double)((object)factor);
				vector.register.double_1 = value.register.double_1 * (double)((object)factor);
			}
			return vector;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0002D17C File Offset: 0x0002B37C
		public unsafe static Vector<T>operator /(Vector<T> left, Vector<T> right)
		{
			if (!Vector.IsHardwareAccelerated)
			{
				Vector<T> vector = default(Vector<T>);
				if (typeof(T) == typeof(byte))
				{
					vector.register.byte_0 = left.register.byte_0 / right.register.byte_0;
					vector.register.byte_1 = left.register.byte_1 / right.register.byte_1;
					vector.register.byte_2 = left.register.byte_2 / right.register.byte_2;
					vector.register.byte_3 = left.register.byte_3 / right.register.byte_3;
					vector.register.byte_4 = left.register.byte_4 / right.register.byte_4;
					vector.register.byte_5 = left.register.byte_5 / right.register.byte_5;
					vector.register.byte_6 = left.register.byte_6 / right.register.byte_6;
					vector.register.byte_7 = left.register.byte_7 / right.register.byte_7;
					vector.register.byte_8 = left.register.byte_8 / right.register.byte_8;
					vector.register.byte_9 = left.register.byte_9 / right.register.byte_9;
					vector.register.byte_10 = left.register.byte_10 / right.register.byte_10;
					vector.register.byte_11 = left.register.byte_11 / right.register.byte_11;
					vector.register.byte_12 = left.register.byte_12 / right.register.byte_12;
					vector.register.byte_13 = left.register.byte_13 / right.register.byte_13;
					vector.register.byte_14 = left.register.byte_14 / right.register.byte_14;
					vector.register.byte_15 = left.register.byte_15 / right.register.byte_15;
				}
				else if (typeof(T) == typeof(sbyte))
				{
					vector.register.sbyte_0 = left.register.sbyte_0 / right.register.sbyte_0;
					vector.register.sbyte_1 = left.register.sbyte_1 / right.register.sbyte_1;
					vector.register.sbyte_2 = left.register.sbyte_2 / right.register.sbyte_2;
					vector.register.sbyte_3 = left.register.sbyte_3 / right.register.sbyte_3;
					vector.register.sbyte_4 = left.register.sbyte_4 / right.register.sbyte_4;
					vector.register.sbyte_5 = left.register.sbyte_5 / right.register.sbyte_5;
					vector.register.sbyte_6 = left.register.sbyte_6 / right.register.sbyte_6;
					vector.register.sbyte_7 = left.register.sbyte_7 / right.register.sbyte_7;
					vector.register.sbyte_8 = left.register.sbyte_8 / right.register.sbyte_8;
					vector.register.sbyte_9 = left.register.sbyte_9 / right.register.sbyte_9;
					vector.register.sbyte_10 = left.register.sbyte_10 / right.register.sbyte_10;
					vector.register.sbyte_11 = left.register.sbyte_11 / right.register.sbyte_11;
					vector.register.sbyte_12 = left.register.sbyte_12 / right.register.sbyte_12;
					vector.register.sbyte_13 = left.register.sbyte_13 / right.register.sbyte_13;
					vector.register.sbyte_14 = left.register.sbyte_14 / right.register.sbyte_14;
					vector.register.sbyte_15 = left.register.sbyte_15 / right.register.sbyte_15;
				}
				else if (typeof(T) == typeof(ushort))
				{
					vector.register.uint16_0 = left.register.uint16_0 / right.register.uint16_0;
					vector.register.uint16_1 = left.register.uint16_1 / right.register.uint16_1;
					vector.register.uint16_2 = left.register.uint16_2 / right.register.uint16_2;
					vector.register.uint16_3 = left.register.uint16_3 / right.register.uint16_3;
					vector.register.uint16_4 = left.register.uint16_4 / right.register.uint16_4;
					vector.register.uint16_5 = left.register.uint16_5 / right.register.uint16_5;
					vector.register.uint16_6 = left.register.uint16_6 / right.register.uint16_6;
					vector.register.uint16_7 = left.register.uint16_7 / right.register.uint16_7;
				}
				else if (typeof(T) == typeof(short))
				{
					vector.register.int16_0 = left.register.int16_0 / right.register.int16_0;
					vector.register.int16_1 = left.register.int16_1 / right.register.int16_1;
					vector.register.int16_2 = left.register.int16_2 / right.register.int16_2;
					vector.register.int16_3 = left.register.int16_3 / right.register.int16_3;
					vector.register.int16_4 = left.register.int16_4 / right.register.int16_4;
					vector.register.int16_5 = left.register.int16_5 / right.register.int16_5;
					vector.register.int16_6 = left.register.int16_6 / right.register.int16_6;
					vector.register.int16_7 = left.register.int16_7 / right.register.int16_7;
				}
				else if (typeof(T) == typeof(uint))
				{
					vector.register.uint32_0 = left.register.uint32_0 / right.register.uint32_0;
					vector.register.uint32_1 = left.register.uint32_1 / right.register.uint32_1;
					vector.register.uint32_2 = left.register.uint32_2 / right.register.uint32_2;
					vector.register.uint32_3 = left.register.uint32_3 / right.register.uint32_3;
				}
				else if (typeof(T) == typeof(int))
				{
					vector.register.int32_0 = left.register.int32_0 / right.register.int32_0;
					vector.register.int32_1 = left.register.int32_1 / right.register.int32_1;
					vector.register.int32_2 = left.register.int32_2 / right.register.int32_2;
					vector.register.int32_3 = left.register.int32_3 / right.register.int32_3;
				}
				else if (typeof(T) == typeof(ulong))
				{
					vector.register.uint64_0 = left.register.uint64_0 / right.register.uint64_0;
					vector.register.uint64_1 = left.register.uint64_1 / right.register.uint64_1;
				}
				else if (typeof(T) == typeof(long))
				{
					vector.register.int64_0 = left.register.int64_0 / right.register.int64_0;
					vector.register.int64_1 = left.register.int64_1 / right.register.int64_1;
				}
				else if (typeof(T) == typeof(float))
				{
					vector.register.single_0 = left.register.single_0 / right.register.single_0;
					vector.register.single_1 = left.register.single_1 / right.register.single_1;
					vector.register.single_2 = left.register.single_2 / right.register.single_2;
					vector.register.single_3 = left.register.single_3 / right.register.single_3;
				}
				else if (typeof(T) == typeof(double))
				{
					vector.register.double_0 = left.register.double_0 / right.register.double_0;
					vector.register.double_1 = left.register.double_1 / right.register.double_1;
				}
				return vector;
			}
			if (typeof(T) == typeof(byte))
			{
				byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
				for (int i = 0; i < Vector<T>.Count; i++)
				{
					ptr[i] = (byte)((object)Vector<T>.ScalarDivide(left[i], right[i]));
				}
				return new Vector<T>((void*)ptr);
			}
			if (typeof(T) == typeof(sbyte))
			{
				sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
				for (int j = 0; j < Vector<T>.Count; j++)
				{
					ptr2[j] = (sbyte)((object)Vector<T>.ScalarDivide(left[j], right[j]));
				}
				return new Vector<T>((void*)ptr2);
			}
			if (typeof(T) == typeof(ushort))
			{
				ushort* ptr3;
				checked
				{
					ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int k = 0; k < Vector<T>.Count; k++)
				{
					ptr3[k] = (ushort)((object)Vector<T>.ScalarDivide(left[k], right[k]));
				}
				return new Vector<T>((void*)ptr3);
			}
			if (typeof(T) == typeof(short))
			{
				short* ptr4;
				checked
				{
					ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
				}
				for (int l = 0; l < Vector<T>.Count; l++)
				{
					ptr4[l] = (short)((object)Vector<T>.ScalarDivide(left[l], right[l]));
				}
				return new Vector<T>((void*)ptr4);
			}
			if (typeof(T) == typeof(uint))
			{
				uint* ptr5;
				checked
				{
					ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int m = 0; m < Vector<T>.Count; m++)
				{
					ptr5[m] = (uint)((object)Vector<T>.ScalarDivide(left[m], right[m]));
				}
				return new Vector<T>((void*)ptr5);
			}
			if (typeof(T) == typeof(int))
			{
				int* ptr6;
				checked
				{
					ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int n = 0; n < Vector<T>.Count; n++)
				{
					ptr6[n] = (int)((object)Vector<T>.ScalarDivide(left[n], right[n]));
				}
				return new Vector<T>((void*)ptr6);
			}
			if (typeof(T) == typeof(ulong))
			{
				ulong* ptr7;
				checked
				{
					ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num = 0; num < Vector<T>.Count; num++)
				{
					ptr7[num] = (ulong)((object)Vector<T>.ScalarDivide(left[num], right[num]));
				}
				return new Vector<T>((void*)ptr7);
			}
			if (typeof(T) == typeof(long))
			{
				long* ptr8;
				checked
				{
					ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num2 = 0; num2 < Vector<T>.Count; num2++)
				{
					ptr8[num2] = (long)((object)Vector<T>.ScalarDivide(left[num2], right[num2]));
				}
				return new Vector<T>((void*)ptr8);
			}
			if (typeof(T) == typeof(float))
			{
				float* ptr9;
				checked
				{
					ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
				}
				for (int num3 = 0; num3 < Vector<T>.Count; num3++)
				{
					ptr9[num3] = (float)((object)Vector<T>.ScalarDivide(left[num3], right[num3]));
				}
				return new Vector<T>((void*)ptr9);
			}
			if (typeof(T) == typeof(double))
			{
				double* ptr10;
				checked
				{
					ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
				}
				for (int num4 = 0; num4 < Vector<T>.Count; num4++)
				{
					ptr10[num4] = (double)((object)Vector<T>.ScalarDivide(left[num4], right[num4]));
				}
				return new Vector<T>((void*)ptr10);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0002E0A4 File Offset: 0x0002C2A4
		public static Vector<T>operator -(Vector<T> value)
		{
			return Vector<T>.Zero - value;
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0002E0B4 File Offset: 0x0002C2B4
		[Intrinsic]
		public unsafe static Vector<T>operator &(Vector<T> left, Vector<T> right)
		{
			Vector<T> vector = default(Vector<T>);
			if (Vector.IsHardwareAccelerated)
			{
				long* ptr = &vector.register.int64_0;
				long* ptr2 = &left.register.int64_0;
				long* ptr3 = &right.register.int64_0;
				for (int i = 0; i < Vector<long>.Count; i++)
				{
					ptr[i] = ptr2[i] & ptr3[i];
				}
			}
			else
			{
				vector.register.int64_0 = left.register.int64_0 & right.register.int64_0;
				vector.register.int64_1 = left.register.int64_1 & right.register.int64_1;
			}
			return vector;
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x0002E180 File Offset: 0x0002C380
		[Intrinsic]
		public unsafe static Vector<T>operator |(Vector<T> left, Vector<T> right)
		{
			Vector<T> vector = default(Vector<T>);
			if (Vector.IsHardwareAccelerated)
			{
				long* ptr = &vector.register.int64_0;
				long* ptr2 = &left.register.int64_0;
				long* ptr3 = &right.register.int64_0;
				for (int i = 0; i < Vector<long>.Count; i++)
				{
					ptr[i] = ptr2[i] | ptr3[i];
				}
			}
			else
			{
				vector.register.int64_0 = left.register.int64_0 | right.register.int64_0;
				vector.register.int64_1 = left.register.int64_1 | right.register.int64_1;
			}
			return vector;
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x0002E24C File Offset: 0x0002C44C
		[Intrinsic]
		public unsafe static Vector<T>operator ^(Vector<T> left, Vector<T> right)
		{
			Vector<T> vector = default(Vector<T>);
			if (Vector.IsHardwareAccelerated)
			{
				long* ptr = &vector.register.int64_0;
				long* ptr2 = &left.register.int64_0;
				long* ptr3 = &right.register.int64_0;
				for (int i = 0; i < Vector<long>.Count; i++)
				{
					ptr[i] = ptr2[i] ^ ptr3[i];
				}
			}
			else
			{
				vector.register.int64_0 = left.register.int64_0 ^ right.register.int64_0;
				vector.register.int64_1 = left.register.int64_1 ^ right.register.int64_1;
			}
			return vector;
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x0002E318 File Offset: 0x0002C518
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T>operator ~(Vector<T> value)
		{
			return Vector<T>.s_allOnes ^ value;
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0002E328 File Offset: 0x0002C528
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector<T> left, Vector<T> right)
		{
			return left.Equals(right);
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0002E334 File Offset: 0x0002C534
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector<T> left, Vector<T> right)
		{
			return !(left == right);
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0002E340 File Offset: 0x0002C540
		[Intrinsic]
		public static explicit operator Vector<byte>(Vector<T> value)
		{
			return new Vector<byte>(ref value.register);
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0002E350 File Offset: 0x0002C550
		[CLSCompliant(false)]
		[Intrinsic]
		public static explicit operator Vector<sbyte>(Vector<T> value)
		{
			return new Vector<sbyte>(ref value.register);
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0002E360 File Offset: 0x0002C560
		[CLSCompliant(false)]
		[Intrinsic]
		public static explicit operator Vector<ushort>(Vector<T> value)
		{
			return new Vector<ushort>(ref value.register);
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0002E370 File Offset: 0x0002C570
		[Intrinsic]
		public static explicit operator Vector<short>(Vector<T> value)
		{
			return new Vector<short>(ref value.register);
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0002E380 File Offset: 0x0002C580
		[CLSCompliant(false)]
		[Intrinsic]
		public static explicit operator Vector<uint>(Vector<T> value)
		{
			return new Vector<uint>(ref value.register);
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0002E390 File Offset: 0x0002C590
		[Intrinsic]
		public static explicit operator Vector<int>(Vector<T> value)
		{
			return new Vector<int>(ref value.register);
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0002E3A0 File Offset: 0x0002C5A0
		[CLSCompliant(false)]
		[Intrinsic]
		public static explicit operator Vector<ulong>(Vector<T> value)
		{
			return new Vector<ulong>(ref value.register);
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0002E3B0 File Offset: 0x0002C5B0
		[Intrinsic]
		public static explicit operator Vector<long>(Vector<T> value)
		{
			return new Vector<long>(ref value.register);
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0002E3C0 File Offset: 0x0002C5C0
		[Intrinsic]
		public static explicit operator Vector<float>(Vector<T> value)
		{
			return new Vector<float>(ref value.register);
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0002E3D0 File Offset: 0x0002C5D0
		[Intrinsic]
		public static explicit operator Vector<double>(Vector<T> value)
		{
			return new Vector<double>(ref value.register);
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0002E3E0 File Offset: 0x0002C5E0
		[Intrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static Vector<T> Equals(Vector<T> left, Vector<T> right)
		{
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[i] = (Vector<T>.ScalarEquals(left[i], right[i]) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr);
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[j] = (Vector<T>.ScalarEquals(left[j], right[j]) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr2);
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort* ptr3;
					checked
					{
						ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[k] = (Vector<T>.ScalarEquals(left[k], right[k]) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr3);
				}
				if (typeof(T) == typeof(short))
				{
					short* ptr4;
					checked
					{
						ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[l] = (Vector<T>.ScalarEquals(left[l], right[l]) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr4);
				}
				if (typeof(T) == typeof(uint))
				{
					uint* ptr5;
					checked
					{
						ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[m] = (Vector<T>.ScalarEquals(left[m], right[m]) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					}
					return new Vector<T>((void*)ptr5);
				}
				if (typeof(T) == typeof(int))
				{
					int* ptr6;
					checked
					{
						ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[n] = (Vector<T>.ScalarEquals(left[n], right[n]) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr6);
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong* ptr7;
					checked
					{
						ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr7[num] = (Vector<T>.ScalarEquals(left[num], right[num]) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					}
					return new Vector<T>((void*)ptr7);
				}
				if (typeof(T) == typeof(long))
				{
					long* ptr8;
					checked
					{
						ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr8[num2] = (Vector<T>.ScalarEquals(left[num2], right[num2]) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					}
					return new Vector<T>((void*)ptr8);
				}
				if (typeof(T) == typeof(float))
				{
					float* ptr9;
					checked
					{
						ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr9[num3] = (Vector<T>.ScalarEquals(left[num3], right[num3]) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					}
					return new Vector<T>((void*)ptr9);
				}
				if (typeof(T) == typeof(double))
				{
					double* ptr10;
					checked
					{
						ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr10[num4] = (Vector<T>.ScalarEquals(left[num4], right[num4]) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					}
					return new Vector<T>((void*)ptr10);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				Register register = default(Register);
				if (typeof(T) == typeof(byte))
				{
					register.byte_0 = ((left.register.byte_0 == right.register.byte_0) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_1 = ((left.register.byte_1 == right.register.byte_1) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_2 = ((left.register.byte_2 == right.register.byte_2) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_3 = ((left.register.byte_3 == right.register.byte_3) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_4 = ((left.register.byte_4 == right.register.byte_4) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_5 = ((left.register.byte_5 == right.register.byte_5) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_6 = ((left.register.byte_6 == right.register.byte_6) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_7 = ((left.register.byte_7 == right.register.byte_7) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_8 = ((left.register.byte_8 == right.register.byte_8) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_9 = ((left.register.byte_9 == right.register.byte_9) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_10 = ((left.register.byte_10 == right.register.byte_10) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_11 = ((left.register.byte_11 == right.register.byte_11) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_12 = ((left.register.byte_12 == right.register.byte_12) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_13 = ((left.register.byte_13 == right.register.byte_13) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_14 = ((left.register.byte_14 == right.register.byte_14) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_15 = ((left.register.byte_15 == right.register.byte_15) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(sbyte))
				{
					register.sbyte_0 = ((left.register.sbyte_0 == right.register.sbyte_0) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_1 = ((left.register.sbyte_1 == right.register.sbyte_1) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_2 = ((left.register.sbyte_2 == right.register.sbyte_2) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_3 = ((left.register.sbyte_3 == right.register.sbyte_3) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_4 = ((left.register.sbyte_4 == right.register.sbyte_4) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_5 = ((left.register.sbyte_5 == right.register.sbyte_5) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_6 = ((left.register.sbyte_6 == right.register.sbyte_6) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_7 = ((left.register.sbyte_7 == right.register.sbyte_7) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_8 = ((left.register.sbyte_8 == right.register.sbyte_8) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_9 = ((left.register.sbyte_9 == right.register.sbyte_9) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_10 = ((left.register.sbyte_10 == right.register.sbyte_10) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_11 = ((left.register.sbyte_11 == right.register.sbyte_11) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_12 = ((left.register.sbyte_12 == right.register.sbyte_12) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_13 = ((left.register.sbyte_13 == right.register.sbyte_13) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_14 = ((left.register.sbyte_14 == right.register.sbyte_14) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_15 = ((left.register.sbyte_15 == right.register.sbyte_15) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(ushort))
				{
					register.uint16_0 = ((left.register.uint16_0 == right.register.uint16_0) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_1 = ((left.register.uint16_1 == right.register.uint16_1) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_2 = ((left.register.uint16_2 == right.register.uint16_2) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_3 = ((left.register.uint16_3 == right.register.uint16_3) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_4 = ((left.register.uint16_4 == right.register.uint16_4) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_5 = ((left.register.uint16_5 == right.register.uint16_5) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_6 = ((left.register.uint16_6 == right.register.uint16_6) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_7 = ((left.register.uint16_7 == right.register.uint16_7) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(short))
				{
					register.int16_0 = ((left.register.int16_0 == right.register.int16_0) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_1 = ((left.register.int16_1 == right.register.int16_1) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_2 = ((left.register.int16_2 == right.register.int16_2) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_3 = ((left.register.int16_3 == right.register.int16_3) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_4 = ((left.register.int16_4 == right.register.int16_4) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_5 = ((left.register.int16_5 == right.register.int16_5) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_6 = ((left.register.int16_6 == right.register.int16_6) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_7 = ((left.register.int16_7 == right.register.int16_7) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(uint))
				{
					register.uint32_0 = ((left.register.uint32_0 == right.register.uint32_0) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_1 = ((left.register.uint32_1 == right.register.uint32_1) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_2 = ((left.register.uint32_2 == right.register.uint32_2) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_3 = ((left.register.uint32_3 == right.register.uint32_3) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(int))
				{
					register.int32_0 = ((left.register.int32_0 == right.register.int32_0) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_1 = ((left.register.int32_1 == right.register.int32_1) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_2 = ((left.register.int32_2 == right.register.int32_2) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_3 = ((left.register.int32_3 == right.register.int32_3) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(ulong))
				{
					register.uint64_0 = ((left.register.uint64_0 == right.register.uint64_0) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					register.uint64_1 = ((left.register.uint64_1 == right.register.uint64_1) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(long))
				{
					register.int64_0 = ((left.register.int64_0 == right.register.int64_0) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					register.int64_1 = ((left.register.int64_1 == right.register.int64_1) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(float))
				{
					register.single_0 = ((left.register.single_0 == right.register.single_0) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_1 = ((left.register.single_1 == right.register.single_1) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_2 = ((left.register.single_2 == right.register.single_2) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_3 = ((left.register.single_3 == right.register.single_3) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(double))
				{
					register.double_0 = ((left.register.double_0 == right.register.double_0) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					register.double_1 = ((left.register.double_1 == right.register.double_1) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					return new Vector<T>(ref register);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0002F5FC File Offset: 0x0002D7FC
		[Intrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static Vector<T> LessThan(Vector<T> left, Vector<T> right)
		{
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[i] = (Vector<T>.ScalarLessThan(left[i], right[i]) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr);
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[j] = (Vector<T>.ScalarLessThan(left[j], right[j]) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr2);
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort* ptr3;
					checked
					{
						ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[k] = (Vector<T>.ScalarLessThan(left[k], right[k]) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr3);
				}
				if (typeof(T) == typeof(short))
				{
					short* ptr4;
					checked
					{
						ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[l] = (Vector<T>.ScalarLessThan(left[l], right[l]) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr4);
				}
				if (typeof(T) == typeof(uint))
				{
					uint* ptr5;
					checked
					{
						ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[m] = (Vector<T>.ScalarLessThan(left[m], right[m]) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					}
					return new Vector<T>((void*)ptr5);
				}
				if (typeof(T) == typeof(int))
				{
					int* ptr6;
					checked
					{
						ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[n] = (Vector<T>.ScalarLessThan(left[n], right[n]) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr6);
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong* ptr7;
					checked
					{
						ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr7[num] = (Vector<T>.ScalarLessThan(left[num], right[num]) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					}
					return new Vector<T>((void*)ptr7);
				}
				if (typeof(T) == typeof(long))
				{
					long* ptr8;
					checked
					{
						ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr8[num2] = (Vector<T>.ScalarLessThan(left[num2], right[num2]) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					}
					return new Vector<T>((void*)ptr8);
				}
				if (typeof(T) == typeof(float))
				{
					float* ptr9;
					checked
					{
						ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr9[num3] = (Vector<T>.ScalarLessThan(left[num3], right[num3]) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					}
					return new Vector<T>((void*)ptr9);
				}
				if (typeof(T) == typeof(double))
				{
					double* ptr10;
					checked
					{
						ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr10[num4] = (Vector<T>.ScalarLessThan(left[num4], right[num4]) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					}
					return new Vector<T>((void*)ptr10);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				Register register = default(Register);
				if (typeof(T) == typeof(byte))
				{
					register.byte_0 = ((left.register.byte_0 < right.register.byte_0) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_1 = ((left.register.byte_1 < right.register.byte_1) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_2 = ((left.register.byte_2 < right.register.byte_2) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_3 = ((left.register.byte_3 < right.register.byte_3) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_4 = ((left.register.byte_4 < right.register.byte_4) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_5 = ((left.register.byte_5 < right.register.byte_5) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_6 = ((left.register.byte_6 < right.register.byte_6) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_7 = ((left.register.byte_7 < right.register.byte_7) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_8 = ((left.register.byte_8 < right.register.byte_8) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_9 = ((left.register.byte_9 < right.register.byte_9) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_10 = ((left.register.byte_10 < right.register.byte_10) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_11 = ((left.register.byte_11 < right.register.byte_11) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_12 = ((left.register.byte_12 < right.register.byte_12) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_13 = ((left.register.byte_13 < right.register.byte_13) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_14 = ((left.register.byte_14 < right.register.byte_14) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_15 = ((left.register.byte_15 < right.register.byte_15) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(sbyte))
				{
					register.sbyte_0 = ((left.register.sbyte_0 < right.register.sbyte_0) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_1 = ((left.register.sbyte_1 < right.register.sbyte_1) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_2 = ((left.register.sbyte_2 < right.register.sbyte_2) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_3 = ((left.register.sbyte_3 < right.register.sbyte_3) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_4 = ((left.register.sbyte_4 < right.register.sbyte_4) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_5 = ((left.register.sbyte_5 < right.register.sbyte_5) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_6 = ((left.register.sbyte_6 < right.register.sbyte_6) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_7 = ((left.register.sbyte_7 < right.register.sbyte_7) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_8 = ((left.register.sbyte_8 < right.register.sbyte_8) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_9 = ((left.register.sbyte_9 < right.register.sbyte_9) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_10 = ((left.register.sbyte_10 < right.register.sbyte_10) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_11 = ((left.register.sbyte_11 < right.register.sbyte_11) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_12 = ((left.register.sbyte_12 < right.register.sbyte_12) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_13 = ((left.register.sbyte_13 < right.register.sbyte_13) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_14 = ((left.register.sbyte_14 < right.register.sbyte_14) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_15 = ((left.register.sbyte_15 < right.register.sbyte_15) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(ushort))
				{
					register.uint16_0 = ((left.register.uint16_0 < right.register.uint16_0) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_1 = ((left.register.uint16_1 < right.register.uint16_1) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_2 = ((left.register.uint16_2 < right.register.uint16_2) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_3 = ((left.register.uint16_3 < right.register.uint16_3) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_4 = ((left.register.uint16_4 < right.register.uint16_4) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_5 = ((left.register.uint16_5 < right.register.uint16_5) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_6 = ((left.register.uint16_6 < right.register.uint16_6) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_7 = ((left.register.uint16_7 < right.register.uint16_7) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(short))
				{
					register.int16_0 = ((left.register.int16_0 < right.register.int16_0) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_1 = ((left.register.int16_1 < right.register.int16_1) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_2 = ((left.register.int16_2 < right.register.int16_2) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_3 = ((left.register.int16_3 < right.register.int16_3) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_4 = ((left.register.int16_4 < right.register.int16_4) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_5 = ((left.register.int16_5 < right.register.int16_5) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_6 = ((left.register.int16_6 < right.register.int16_6) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_7 = ((left.register.int16_7 < right.register.int16_7) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(uint))
				{
					register.uint32_0 = ((left.register.uint32_0 < right.register.uint32_0) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_1 = ((left.register.uint32_1 < right.register.uint32_1) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_2 = ((left.register.uint32_2 < right.register.uint32_2) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_3 = ((left.register.uint32_3 < right.register.uint32_3) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(int))
				{
					register.int32_0 = ((left.register.int32_0 < right.register.int32_0) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_1 = ((left.register.int32_1 < right.register.int32_1) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_2 = ((left.register.int32_2 < right.register.int32_2) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_3 = ((left.register.int32_3 < right.register.int32_3) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(ulong))
				{
					register.uint64_0 = ((left.register.uint64_0 < right.register.uint64_0) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					register.uint64_1 = ((left.register.uint64_1 < right.register.uint64_1) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(long))
				{
					register.int64_0 = ((left.register.int64_0 < right.register.int64_0) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					register.int64_1 = ((left.register.int64_1 < right.register.int64_1) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(float))
				{
					register.single_0 = ((left.register.single_0 < right.register.single_0) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_1 = ((left.register.single_1 < right.register.single_1) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_2 = ((left.register.single_2 < right.register.single_2) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_3 = ((left.register.single_3 < right.register.single_3) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(double))
				{
					register.double_0 = ((left.register.double_0 < right.register.double_0) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					register.double_1 = ((left.register.double_1 < right.register.double_1) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					return new Vector<T>(ref register);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x00030818 File Offset: 0x0002EA18
		[Intrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static Vector<T> GreaterThan(Vector<T> left, Vector<T> right)
		{
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[i] = (Vector<T>.ScalarGreaterThan(left[i], right[i]) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr);
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[j] = (Vector<T>.ScalarGreaterThan(left[j], right[j]) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr2);
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort* ptr3;
					checked
					{
						ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[k] = (Vector<T>.ScalarGreaterThan(left[k], right[k]) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr3);
				}
				if (typeof(T) == typeof(short))
				{
					short* ptr4;
					checked
					{
						ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[l] = (Vector<T>.ScalarGreaterThan(left[l], right[l]) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr4);
				}
				if (typeof(T) == typeof(uint))
				{
					uint* ptr5;
					checked
					{
						ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[m] = (Vector<T>.ScalarGreaterThan(left[m], right[m]) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					}
					return new Vector<T>((void*)ptr5);
				}
				if (typeof(T) == typeof(int))
				{
					int* ptr6;
					checked
					{
						ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[n] = (Vector<T>.ScalarGreaterThan(left[n], right[n]) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					}
					return new Vector<T>((void*)ptr6);
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong* ptr7;
					checked
					{
						ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr7[num] = (Vector<T>.ScalarGreaterThan(left[num], right[num]) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					}
					return new Vector<T>((void*)ptr7);
				}
				if (typeof(T) == typeof(long))
				{
					long* ptr8;
					checked
					{
						ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr8[num2] = (Vector<T>.ScalarGreaterThan(left[num2], right[num2]) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					}
					return new Vector<T>((void*)ptr8);
				}
				if (typeof(T) == typeof(float))
				{
					float* ptr9;
					checked
					{
						ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr9[num3] = (Vector<T>.ScalarGreaterThan(left[num3], right[num3]) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					}
					return new Vector<T>((void*)ptr9);
				}
				if (typeof(T) == typeof(double))
				{
					double* ptr10;
					checked
					{
						ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr10[num4] = (Vector<T>.ScalarGreaterThan(left[num4], right[num4]) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					}
					return new Vector<T>((void*)ptr10);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				Register register = default(Register);
				if (typeof(T) == typeof(byte))
				{
					register.byte_0 = ((left.register.byte_0 > right.register.byte_0) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_1 = ((left.register.byte_1 > right.register.byte_1) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_2 = ((left.register.byte_2 > right.register.byte_2) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_3 = ((left.register.byte_3 > right.register.byte_3) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_4 = ((left.register.byte_4 > right.register.byte_4) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_5 = ((left.register.byte_5 > right.register.byte_5) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_6 = ((left.register.byte_6 > right.register.byte_6) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_7 = ((left.register.byte_7 > right.register.byte_7) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_8 = ((left.register.byte_8 > right.register.byte_8) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_9 = ((left.register.byte_9 > right.register.byte_9) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_10 = ((left.register.byte_10 > right.register.byte_10) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_11 = ((left.register.byte_11 > right.register.byte_11) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_12 = ((left.register.byte_12 > right.register.byte_12) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_13 = ((left.register.byte_13 > right.register.byte_13) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_14 = ((left.register.byte_14 > right.register.byte_14) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					register.byte_15 = ((left.register.byte_15 > right.register.byte_15) ? ConstantHelper.GetByteWithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(sbyte))
				{
					register.sbyte_0 = ((left.register.sbyte_0 > right.register.sbyte_0) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_1 = ((left.register.sbyte_1 > right.register.sbyte_1) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_2 = ((left.register.sbyte_2 > right.register.sbyte_2) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_3 = ((left.register.sbyte_3 > right.register.sbyte_3) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_4 = ((left.register.sbyte_4 > right.register.sbyte_4) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_5 = ((left.register.sbyte_5 > right.register.sbyte_5) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_6 = ((left.register.sbyte_6 > right.register.sbyte_6) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_7 = ((left.register.sbyte_7 > right.register.sbyte_7) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_8 = ((left.register.sbyte_8 > right.register.sbyte_8) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_9 = ((left.register.sbyte_9 > right.register.sbyte_9) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_10 = ((left.register.sbyte_10 > right.register.sbyte_10) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_11 = ((left.register.sbyte_11 > right.register.sbyte_11) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_12 = ((left.register.sbyte_12 > right.register.sbyte_12) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_13 = ((left.register.sbyte_13 > right.register.sbyte_13) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_14 = ((left.register.sbyte_14 > right.register.sbyte_14) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					register.sbyte_15 = ((left.register.sbyte_15 > right.register.sbyte_15) ? ConstantHelper.GetSByteWithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(ushort))
				{
					register.uint16_0 = ((left.register.uint16_0 > right.register.uint16_0) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_1 = ((left.register.uint16_1 > right.register.uint16_1) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_2 = ((left.register.uint16_2 > right.register.uint16_2) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_3 = ((left.register.uint16_3 > right.register.uint16_3) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_4 = ((left.register.uint16_4 > right.register.uint16_4) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_5 = ((left.register.uint16_5 > right.register.uint16_5) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_6 = ((left.register.uint16_6 > right.register.uint16_6) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					register.uint16_7 = ((left.register.uint16_7 > right.register.uint16_7) ? ConstantHelper.GetUInt16WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(short))
				{
					register.int16_0 = ((left.register.int16_0 > right.register.int16_0) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_1 = ((left.register.int16_1 > right.register.int16_1) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_2 = ((left.register.int16_2 > right.register.int16_2) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_3 = ((left.register.int16_3 > right.register.int16_3) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_4 = ((left.register.int16_4 > right.register.int16_4) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_5 = ((left.register.int16_5 > right.register.int16_5) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_6 = ((left.register.int16_6 > right.register.int16_6) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					register.int16_7 = ((left.register.int16_7 > right.register.int16_7) ? ConstantHelper.GetInt16WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(uint))
				{
					register.uint32_0 = ((left.register.uint32_0 > right.register.uint32_0) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_1 = ((left.register.uint32_1 > right.register.uint32_1) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_2 = ((left.register.uint32_2 > right.register.uint32_2) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					register.uint32_3 = ((left.register.uint32_3 > right.register.uint32_3) ? ConstantHelper.GetUInt32WithAllBitsSet() : 0U);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(int))
				{
					register.int32_0 = ((left.register.int32_0 > right.register.int32_0) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_1 = ((left.register.int32_1 > right.register.int32_1) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_2 = ((left.register.int32_2 > right.register.int32_2) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					register.int32_3 = ((left.register.int32_3 > right.register.int32_3) ? ConstantHelper.GetInt32WithAllBitsSet() : 0);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(ulong))
				{
					register.uint64_0 = ((left.register.uint64_0 > right.register.uint64_0) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					register.uint64_1 = ((left.register.uint64_1 > right.register.uint64_1) ? ConstantHelper.GetUInt64WithAllBitsSet() : 0UL);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(long))
				{
					register.int64_0 = ((left.register.int64_0 > right.register.int64_0) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					register.int64_1 = ((left.register.int64_1 > right.register.int64_1) ? ConstantHelper.GetInt64WithAllBitsSet() : 0L);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(float))
				{
					register.single_0 = ((left.register.single_0 > right.register.single_0) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_1 = ((left.register.single_1 > right.register.single_1) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_2 = ((left.register.single_2 > right.register.single_2) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					register.single_3 = ((left.register.single_3 > right.register.single_3) ? ConstantHelper.GetSingleWithAllBitsSet() : 0f);
					return new Vector<T>(ref register);
				}
				if (typeof(T) == typeof(double))
				{
					register.double_0 = ((left.register.double_0 > right.register.double_0) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					register.double_1 = ((left.register.double_1 > right.register.double_1) ? ConstantHelper.GetDoubleWithAllBitsSet() : 0.0);
					return new Vector<T>(ref register);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x00031A34 File Offset: 0x0002FC34
		[Intrinsic]
		internal static Vector<T> GreaterThanOrEqual(Vector<T> left, Vector<T> right)
		{
			return Vector<T>.Equals(left, right) | Vector<T>.GreaterThan(left, right);
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x00031A4C File Offset: 0x0002FC4C
		[Intrinsic]
		internal static Vector<T> LessThanOrEqual(Vector<T> left, Vector<T> right)
		{
			return Vector<T>.Equals(left, right) | Vector<T>.LessThan(left, right);
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x00031A64 File Offset: 0x0002FC64
		[Intrinsic]
		internal static Vector<T> ConditionalSelect(Vector<T> condition, Vector<T> left, Vector<T> right)
		{
			return (left & condition) | Vector.AndNot<T>(right, condition);
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x00031A7C File Offset: 0x0002FC7C
		[Intrinsic]
		internal unsafe static Vector<T> Abs(Vector<T> value)
		{
			if (typeof(T) == typeof(byte))
			{
				return value;
			}
			if (typeof(T) == typeof(ushort))
			{
				return value;
			}
			if (typeof(T) == typeof(uint))
			{
				return value;
			}
			if (typeof(T) == typeof(ulong))
			{
				return value;
			}
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(sbyte))
				{
					sbyte* ptr = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[i] = (sbyte)Math.Abs((sbyte)((object)value[i]));
					}
					return new Vector<T>((void*)ptr);
				}
				if (typeof(T) == typeof(short))
				{
					short* ptr2;
					checked
					{
						ptr2 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[j] = (short)Math.Abs((short)((object)value[j]));
					}
					return new Vector<T>((void*)ptr2);
				}
				if (typeof(T) == typeof(int))
				{
					int* ptr3;
					checked
					{
						ptr3 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[k] = (int)Math.Abs((int)((object)value[k]));
					}
					return new Vector<T>((void*)ptr3);
				}
				if (typeof(T) == typeof(long))
				{
					long* ptr4;
					checked
					{
						ptr4 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[l] = (long)Math.Abs((long)((object)value[l]));
					}
					return new Vector<T>((void*)ptr4);
				}
				if (typeof(T) == typeof(float))
				{
					float* ptr5;
					checked
					{
						ptr5 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[m] = (float)Math.Abs((float)((object)value[m]));
					}
					return new Vector<T>((void*)ptr5);
				}
				if (typeof(T) == typeof(double))
				{
					double* ptr6;
					checked
					{
						ptr6 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[n] = (double)Math.Abs((double)((object)value[n]));
					}
					return new Vector<T>((void*)ptr6);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				if (typeof(T) == typeof(sbyte))
				{
					value.register.sbyte_0 = Math.Abs(value.register.sbyte_0);
					value.register.sbyte_1 = Math.Abs(value.register.sbyte_1);
					value.register.sbyte_2 = Math.Abs(value.register.sbyte_2);
					value.register.sbyte_3 = Math.Abs(value.register.sbyte_3);
					value.register.sbyte_4 = Math.Abs(value.register.sbyte_4);
					value.register.sbyte_5 = Math.Abs(value.register.sbyte_5);
					value.register.sbyte_6 = Math.Abs(value.register.sbyte_6);
					value.register.sbyte_7 = Math.Abs(value.register.sbyte_7);
					value.register.sbyte_8 = Math.Abs(value.register.sbyte_8);
					value.register.sbyte_9 = Math.Abs(value.register.sbyte_9);
					value.register.sbyte_10 = Math.Abs(value.register.sbyte_10);
					value.register.sbyte_11 = Math.Abs(value.register.sbyte_11);
					value.register.sbyte_12 = Math.Abs(value.register.sbyte_12);
					value.register.sbyte_13 = Math.Abs(value.register.sbyte_13);
					value.register.sbyte_14 = Math.Abs(value.register.sbyte_14);
					value.register.sbyte_15 = Math.Abs(value.register.sbyte_15);
					return value;
				}
				if (typeof(T) == typeof(short))
				{
					value.register.int16_0 = Math.Abs(value.register.int16_0);
					value.register.int16_1 = Math.Abs(value.register.int16_1);
					value.register.int16_2 = Math.Abs(value.register.int16_2);
					value.register.int16_3 = Math.Abs(value.register.int16_3);
					value.register.int16_4 = Math.Abs(value.register.int16_4);
					value.register.int16_5 = Math.Abs(value.register.int16_5);
					value.register.int16_6 = Math.Abs(value.register.int16_6);
					value.register.int16_7 = Math.Abs(value.register.int16_7);
					return value;
				}
				if (typeof(T) == typeof(int))
				{
					value.register.int32_0 = Math.Abs(value.register.int32_0);
					value.register.int32_1 = Math.Abs(value.register.int32_1);
					value.register.int32_2 = Math.Abs(value.register.int32_2);
					value.register.int32_3 = Math.Abs(value.register.int32_3);
					return value;
				}
				if (typeof(T) == typeof(long))
				{
					value.register.int64_0 = Math.Abs(value.register.int64_0);
					value.register.int64_1 = Math.Abs(value.register.int64_1);
					return value;
				}
				if (typeof(T) == typeof(float))
				{
					value.register.single_0 = Math.Abs(value.register.single_0);
					value.register.single_1 = Math.Abs(value.register.single_1);
					value.register.single_2 = Math.Abs(value.register.single_2);
					value.register.single_3 = Math.Abs(value.register.single_3);
					return value;
				}
				if (typeof(T) == typeof(double))
				{
					value.register.double_0 = Math.Abs(value.register.double_0);
					value.register.double_1 = Math.Abs(value.register.double_1);
					return value;
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x00032284 File Offset: 0x00030484
		[Intrinsic]
		internal unsafe static Vector<T> Min(Vector<T> left, Vector<T> right)
		{
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[i] = (Vector<T>.ScalarLessThan(left[i], right[i]) ? ((byte)((object)left[i])) : ((byte)((object)right[i])));
					}
					return new Vector<T>((void*)ptr);
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[j] = (Vector<T>.ScalarLessThan(left[j], right[j]) ? ((sbyte)((object)left[j])) : ((sbyte)((object)right[j])));
					}
					return new Vector<T>((void*)ptr2);
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort* ptr3;
					checked
					{
						ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[k] = (Vector<T>.ScalarLessThan(left[k], right[k]) ? ((ushort)((object)left[k])) : ((ushort)((object)right[k])));
					}
					return new Vector<T>((void*)ptr3);
				}
				if (typeof(T) == typeof(short))
				{
					short* ptr4;
					checked
					{
						ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[l] = (Vector<T>.ScalarLessThan(left[l], right[l]) ? ((short)((object)left[l])) : ((short)((object)right[l])));
					}
					return new Vector<T>((void*)ptr4);
				}
				if (typeof(T) == typeof(uint))
				{
					uint* ptr5;
					checked
					{
						ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[m] = (Vector<T>.ScalarLessThan(left[m], right[m]) ? ((uint)((object)left[m])) : ((uint)((object)right[m])));
					}
					return new Vector<T>((void*)ptr5);
				}
				if (typeof(T) == typeof(int))
				{
					int* ptr6;
					checked
					{
						ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[n] = (Vector<T>.ScalarLessThan(left[n], right[n]) ? ((int)((object)left[n])) : ((int)((object)right[n])));
					}
					return new Vector<T>((void*)ptr6);
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong* ptr7;
					checked
					{
						ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr7[num] = (Vector<T>.ScalarLessThan(left[num], right[num]) ? ((ulong)((object)left[num])) : ((ulong)((object)right[num])));
					}
					return new Vector<T>((void*)ptr7);
				}
				if (typeof(T) == typeof(long))
				{
					long* ptr8;
					checked
					{
						ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr8[num2] = (Vector<T>.ScalarLessThan(left[num2], right[num2]) ? ((long)((object)left[num2])) : ((long)((object)right[num2])));
					}
					return new Vector<T>((void*)ptr8);
				}
				if (typeof(T) == typeof(float))
				{
					float* ptr9;
					checked
					{
						ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr9[num3] = (Vector<T>.ScalarLessThan(left[num3], right[num3]) ? ((float)((object)left[num3])) : ((float)((object)right[num3])));
					}
					return new Vector<T>((void*)ptr9);
				}
				if (typeof(T) == typeof(double))
				{
					double* ptr10;
					checked
					{
						ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr10[num4] = (Vector<T>.ScalarLessThan(left[num4], right[num4]) ? ((double)((object)left[num4])) : ((double)((object)right[num4])));
					}
					return new Vector<T>((void*)ptr10);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				Vector<T> vector = default(Vector<T>);
				if (typeof(T) == typeof(byte))
				{
					vector.register.byte_0 = ((left.register.byte_0 < right.register.byte_0) ? left.register.byte_0 : right.register.byte_0);
					vector.register.byte_1 = ((left.register.byte_1 < right.register.byte_1) ? left.register.byte_1 : right.register.byte_1);
					vector.register.byte_2 = ((left.register.byte_2 < right.register.byte_2) ? left.register.byte_2 : right.register.byte_2);
					vector.register.byte_3 = ((left.register.byte_3 < right.register.byte_3) ? left.register.byte_3 : right.register.byte_3);
					vector.register.byte_4 = ((left.register.byte_4 < right.register.byte_4) ? left.register.byte_4 : right.register.byte_4);
					vector.register.byte_5 = ((left.register.byte_5 < right.register.byte_5) ? left.register.byte_5 : right.register.byte_5);
					vector.register.byte_6 = ((left.register.byte_6 < right.register.byte_6) ? left.register.byte_6 : right.register.byte_6);
					vector.register.byte_7 = ((left.register.byte_7 < right.register.byte_7) ? left.register.byte_7 : right.register.byte_7);
					vector.register.byte_8 = ((left.register.byte_8 < right.register.byte_8) ? left.register.byte_8 : right.register.byte_8);
					vector.register.byte_9 = ((left.register.byte_9 < right.register.byte_9) ? left.register.byte_9 : right.register.byte_9);
					vector.register.byte_10 = ((left.register.byte_10 < right.register.byte_10) ? left.register.byte_10 : right.register.byte_10);
					vector.register.byte_11 = ((left.register.byte_11 < right.register.byte_11) ? left.register.byte_11 : right.register.byte_11);
					vector.register.byte_12 = ((left.register.byte_12 < right.register.byte_12) ? left.register.byte_12 : right.register.byte_12);
					vector.register.byte_13 = ((left.register.byte_13 < right.register.byte_13) ? left.register.byte_13 : right.register.byte_13);
					vector.register.byte_14 = ((left.register.byte_14 < right.register.byte_14) ? left.register.byte_14 : right.register.byte_14);
					vector.register.byte_15 = ((left.register.byte_15 < right.register.byte_15) ? left.register.byte_15 : right.register.byte_15);
					return vector;
				}
				if (typeof(T) == typeof(sbyte))
				{
					vector.register.sbyte_0 = ((left.register.sbyte_0 < right.register.sbyte_0) ? left.register.sbyte_0 : right.register.sbyte_0);
					vector.register.sbyte_1 = ((left.register.sbyte_1 < right.register.sbyte_1) ? left.register.sbyte_1 : right.register.sbyte_1);
					vector.register.sbyte_2 = ((left.register.sbyte_2 < right.register.sbyte_2) ? left.register.sbyte_2 : right.register.sbyte_2);
					vector.register.sbyte_3 = ((left.register.sbyte_3 < right.register.sbyte_3) ? left.register.sbyte_3 : right.register.sbyte_3);
					vector.register.sbyte_4 = ((left.register.sbyte_4 < right.register.sbyte_4) ? left.register.sbyte_4 : right.register.sbyte_4);
					vector.register.sbyte_5 = ((left.register.sbyte_5 < right.register.sbyte_5) ? left.register.sbyte_5 : right.register.sbyte_5);
					vector.register.sbyte_6 = ((left.register.sbyte_6 < right.register.sbyte_6) ? left.register.sbyte_6 : right.register.sbyte_6);
					vector.register.sbyte_7 = ((left.register.sbyte_7 < right.register.sbyte_7) ? left.register.sbyte_7 : right.register.sbyte_7);
					vector.register.sbyte_8 = ((left.register.sbyte_8 < right.register.sbyte_8) ? left.register.sbyte_8 : right.register.sbyte_8);
					vector.register.sbyte_9 = ((left.register.sbyte_9 < right.register.sbyte_9) ? left.register.sbyte_9 : right.register.sbyte_9);
					vector.register.sbyte_10 = ((left.register.sbyte_10 < right.register.sbyte_10) ? left.register.sbyte_10 : right.register.sbyte_10);
					vector.register.sbyte_11 = ((left.register.sbyte_11 < right.register.sbyte_11) ? left.register.sbyte_11 : right.register.sbyte_11);
					vector.register.sbyte_12 = ((left.register.sbyte_12 < right.register.sbyte_12) ? left.register.sbyte_12 : right.register.sbyte_12);
					vector.register.sbyte_13 = ((left.register.sbyte_13 < right.register.sbyte_13) ? left.register.sbyte_13 : right.register.sbyte_13);
					vector.register.sbyte_14 = ((left.register.sbyte_14 < right.register.sbyte_14) ? left.register.sbyte_14 : right.register.sbyte_14);
					vector.register.sbyte_15 = ((left.register.sbyte_15 < right.register.sbyte_15) ? left.register.sbyte_15 : right.register.sbyte_15);
					return vector;
				}
				if (typeof(T) == typeof(ushort))
				{
					vector.register.uint16_0 = ((left.register.uint16_0 < right.register.uint16_0) ? left.register.uint16_0 : right.register.uint16_0);
					vector.register.uint16_1 = ((left.register.uint16_1 < right.register.uint16_1) ? left.register.uint16_1 : right.register.uint16_1);
					vector.register.uint16_2 = ((left.register.uint16_2 < right.register.uint16_2) ? left.register.uint16_2 : right.register.uint16_2);
					vector.register.uint16_3 = ((left.register.uint16_3 < right.register.uint16_3) ? left.register.uint16_3 : right.register.uint16_3);
					vector.register.uint16_4 = ((left.register.uint16_4 < right.register.uint16_4) ? left.register.uint16_4 : right.register.uint16_4);
					vector.register.uint16_5 = ((left.register.uint16_5 < right.register.uint16_5) ? left.register.uint16_5 : right.register.uint16_5);
					vector.register.uint16_6 = ((left.register.uint16_6 < right.register.uint16_6) ? left.register.uint16_6 : right.register.uint16_6);
					vector.register.uint16_7 = ((left.register.uint16_7 < right.register.uint16_7) ? left.register.uint16_7 : right.register.uint16_7);
					return vector;
				}
				if (typeof(T) == typeof(short))
				{
					vector.register.int16_0 = ((left.register.int16_0 < right.register.int16_0) ? left.register.int16_0 : right.register.int16_0);
					vector.register.int16_1 = ((left.register.int16_1 < right.register.int16_1) ? left.register.int16_1 : right.register.int16_1);
					vector.register.int16_2 = ((left.register.int16_2 < right.register.int16_2) ? left.register.int16_2 : right.register.int16_2);
					vector.register.int16_3 = ((left.register.int16_3 < right.register.int16_3) ? left.register.int16_3 : right.register.int16_3);
					vector.register.int16_4 = ((left.register.int16_4 < right.register.int16_4) ? left.register.int16_4 : right.register.int16_4);
					vector.register.int16_5 = ((left.register.int16_5 < right.register.int16_5) ? left.register.int16_5 : right.register.int16_5);
					vector.register.int16_6 = ((left.register.int16_6 < right.register.int16_6) ? left.register.int16_6 : right.register.int16_6);
					vector.register.int16_7 = ((left.register.int16_7 < right.register.int16_7) ? left.register.int16_7 : right.register.int16_7);
					return vector;
				}
				if (typeof(T) == typeof(uint))
				{
					vector.register.uint32_0 = ((left.register.uint32_0 < right.register.uint32_0) ? left.register.uint32_0 : right.register.uint32_0);
					vector.register.uint32_1 = ((left.register.uint32_1 < right.register.uint32_1) ? left.register.uint32_1 : right.register.uint32_1);
					vector.register.uint32_2 = ((left.register.uint32_2 < right.register.uint32_2) ? left.register.uint32_2 : right.register.uint32_2);
					vector.register.uint32_3 = ((left.register.uint32_3 < right.register.uint32_3) ? left.register.uint32_3 : right.register.uint32_3);
					return vector;
				}
				if (typeof(T) == typeof(int))
				{
					vector.register.int32_0 = ((left.register.int32_0 < right.register.int32_0) ? left.register.int32_0 : right.register.int32_0);
					vector.register.int32_1 = ((left.register.int32_1 < right.register.int32_1) ? left.register.int32_1 : right.register.int32_1);
					vector.register.int32_2 = ((left.register.int32_2 < right.register.int32_2) ? left.register.int32_2 : right.register.int32_2);
					vector.register.int32_3 = ((left.register.int32_3 < right.register.int32_3) ? left.register.int32_3 : right.register.int32_3);
					return vector;
				}
				if (typeof(T) == typeof(ulong))
				{
					vector.register.uint64_0 = ((left.register.uint64_0 < right.register.uint64_0) ? left.register.uint64_0 : right.register.uint64_0);
					vector.register.uint64_1 = ((left.register.uint64_1 < right.register.uint64_1) ? left.register.uint64_1 : right.register.uint64_1);
					return vector;
				}
				if (typeof(T) == typeof(long))
				{
					vector.register.int64_0 = ((left.register.int64_0 < right.register.int64_0) ? left.register.int64_0 : right.register.int64_0);
					vector.register.int64_1 = ((left.register.int64_1 < right.register.int64_1) ? left.register.int64_1 : right.register.int64_1);
					return vector;
				}
				if (typeof(T) == typeof(float))
				{
					vector.register.single_0 = ((left.register.single_0 < right.register.single_0) ? left.register.single_0 : right.register.single_0);
					vector.register.single_1 = ((left.register.single_1 < right.register.single_1) ? left.register.single_1 : right.register.single_1);
					vector.register.single_2 = ((left.register.single_2 < right.register.single_2) ? left.register.single_2 : right.register.single_2);
					vector.register.single_3 = ((left.register.single_3 < right.register.single_3) ? left.register.single_3 : right.register.single_3);
					return vector;
				}
				if (typeof(T) == typeof(double))
				{
					vector.register.double_0 = ((left.register.double_0 < right.register.double_0) ? left.register.double_0 : right.register.double_0);
					vector.register.double_1 = ((left.register.double_1 < right.register.double_1) ? left.register.double_1 : right.register.double_1);
					return vector;
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00033AE4 File Offset: 0x00031CE4
		[Intrinsic]
		internal unsafe static Vector<T> Max(Vector<T> left, Vector<T> right)
		{
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[i] = (Vector<T>.ScalarGreaterThan(left[i], right[i]) ? ((byte)((object)left[i])) : ((byte)((object)right[i])));
					}
					return new Vector<T>((void*)ptr);
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[j] = (Vector<T>.ScalarGreaterThan(left[j], right[j]) ? ((sbyte)((object)left[j])) : ((sbyte)((object)right[j])));
					}
					return new Vector<T>((void*)ptr2);
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort* ptr3;
					checked
					{
						ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[k] = (Vector<T>.ScalarGreaterThan(left[k], right[k]) ? ((ushort)((object)left[k])) : ((ushort)((object)right[k])));
					}
					return new Vector<T>((void*)ptr3);
				}
				if (typeof(T) == typeof(short))
				{
					short* ptr4;
					checked
					{
						ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[l] = (Vector<T>.ScalarGreaterThan(left[l], right[l]) ? ((short)((object)left[l])) : ((short)((object)right[l])));
					}
					return new Vector<T>((void*)ptr4);
				}
				if (typeof(T) == typeof(uint))
				{
					uint* ptr5;
					checked
					{
						ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[m] = (Vector<T>.ScalarGreaterThan(left[m], right[m]) ? ((uint)((object)left[m])) : ((uint)((object)right[m])));
					}
					return new Vector<T>((void*)ptr5);
				}
				if (typeof(T) == typeof(int))
				{
					int* ptr6;
					checked
					{
						ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[n] = (Vector<T>.ScalarGreaterThan(left[n], right[n]) ? ((int)((object)left[n])) : ((int)((object)right[n])));
					}
					return new Vector<T>((void*)ptr6);
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong* ptr7;
					checked
					{
						ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr7[num] = (Vector<T>.ScalarGreaterThan(left[num], right[num]) ? ((ulong)((object)left[num])) : ((ulong)((object)right[num])));
					}
					return new Vector<T>((void*)ptr7);
				}
				if (typeof(T) == typeof(long))
				{
					long* ptr8;
					checked
					{
						ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr8[num2] = (Vector<T>.ScalarGreaterThan(left[num2], right[num2]) ? ((long)((object)left[num2])) : ((long)((object)right[num2])));
					}
					return new Vector<T>((void*)ptr8);
				}
				if (typeof(T) == typeof(float))
				{
					float* ptr9;
					checked
					{
						ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr9[num3] = (Vector<T>.ScalarGreaterThan(left[num3], right[num3]) ? ((float)((object)left[num3])) : ((float)((object)right[num3])));
					}
					return new Vector<T>((void*)ptr9);
				}
				if (typeof(T) == typeof(double))
				{
					double* ptr10;
					checked
					{
						ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr10[num4] = (Vector<T>.ScalarGreaterThan(left[num4], right[num4]) ? ((double)((object)left[num4])) : ((double)((object)right[num4])));
					}
					return new Vector<T>((void*)ptr10);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				Vector<T> vector = default(Vector<T>);
				if (typeof(T) == typeof(byte))
				{
					vector.register.byte_0 = ((left.register.byte_0 > right.register.byte_0) ? left.register.byte_0 : right.register.byte_0);
					vector.register.byte_1 = ((left.register.byte_1 > right.register.byte_1) ? left.register.byte_1 : right.register.byte_1);
					vector.register.byte_2 = ((left.register.byte_2 > right.register.byte_2) ? left.register.byte_2 : right.register.byte_2);
					vector.register.byte_3 = ((left.register.byte_3 > right.register.byte_3) ? left.register.byte_3 : right.register.byte_3);
					vector.register.byte_4 = ((left.register.byte_4 > right.register.byte_4) ? left.register.byte_4 : right.register.byte_4);
					vector.register.byte_5 = ((left.register.byte_5 > right.register.byte_5) ? left.register.byte_5 : right.register.byte_5);
					vector.register.byte_6 = ((left.register.byte_6 > right.register.byte_6) ? left.register.byte_6 : right.register.byte_6);
					vector.register.byte_7 = ((left.register.byte_7 > right.register.byte_7) ? left.register.byte_7 : right.register.byte_7);
					vector.register.byte_8 = ((left.register.byte_8 > right.register.byte_8) ? left.register.byte_8 : right.register.byte_8);
					vector.register.byte_9 = ((left.register.byte_9 > right.register.byte_9) ? left.register.byte_9 : right.register.byte_9);
					vector.register.byte_10 = ((left.register.byte_10 > right.register.byte_10) ? left.register.byte_10 : right.register.byte_10);
					vector.register.byte_11 = ((left.register.byte_11 > right.register.byte_11) ? left.register.byte_11 : right.register.byte_11);
					vector.register.byte_12 = ((left.register.byte_12 > right.register.byte_12) ? left.register.byte_12 : right.register.byte_12);
					vector.register.byte_13 = ((left.register.byte_13 > right.register.byte_13) ? left.register.byte_13 : right.register.byte_13);
					vector.register.byte_14 = ((left.register.byte_14 > right.register.byte_14) ? left.register.byte_14 : right.register.byte_14);
					vector.register.byte_15 = ((left.register.byte_15 > right.register.byte_15) ? left.register.byte_15 : right.register.byte_15);
					return vector;
				}
				if (typeof(T) == typeof(sbyte))
				{
					vector.register.sbyte_0 = ((left.register.sbyte_0 > right.register.sbyte_0) ? left.register.sbyte_0 : right.register.sbyte_0);
					vector.register.sbyte_1 = ((left.register.sbyte_1 > right.register.sbyte_1) ? left.register.sbyte_1 : right.register.sbyte_1);
					vector.register.sbyte_2 = ((left.register.sbyte_2 > right.register.sbyte_2) ? left.register.sbyte_2 : right.register.sbyte_2);
					vector.register.sbyte_3 = ((left.register.sbyte_3 > right.register.sbyte_3) ? left.register.sbyte_3 : right.register.sbyte_3);
					vector.register.sbyte_4 = ((left.register.sbyte_4 > right.register.sbyte_4) ? left.register.sbyte_4 : right.register.sbyte_4);
					vector.register.sbyte_5 = ((left.register.sbyte_5 > right.register.sbyte_5) ? left.register.sbyte_5 : right.register.sbyte_5);
					vector.register.sbyte_6 = ((left.register.sbyte_6 > right.register.sbyte_6) ? left.register.sbyte_6 : right.register.sbyte_6);
					vector.register.sbyte_7 = ((left.register.sbyte_7 > right.register.sbyte_7) ? left.register.sbyte_7 : right.register.sbyte_7);
					vector.register.sbyte_8 = ((left.register.sbyte_8 > right.register.sbyte_8) ? left.register.sbyte_8 : right.register.sbyte_8);
					vector.register.sbyte_9 = ((left.register.sbyte_9 > right.register.sbyte_9) ? left.register.sbyte_9 : right.register.sbyte_9);
					vector.register.sbyte_10 = ((left.register.sbyte_10 > right.register.sbyte_10) ? left.register.sbyte_10 : right.register.sbyte_10);
					vector.register.sbyte_11 = ((left.register.sbyte_11 > right.register.sbyte_11) ? left.register.sbyte_11 : right.register.sbyte_11);
					vector.register.sbyte_12 = ((left.register.sbyte_12 > right.register.sbyte_12) ? left.register.sbyte_12 : right.register.sbyte_12);
					vector.register.sbyte_13 = ((left.register.sbyte_13 > right.register.sbyte_13) ? left.register.sbyte_13 : right.register.sbyte_13);
					vector.register.sbyte_14 = ((left.register.sbyte_14 > right.register.sbyte_14) ? left.register.sbyte_14 : right.register.sbyte_14);
					vector.register.sbyte_15 = ((left.register.sbyte_15 > right.register.sbyte_15) ? left.register.sbyte_15 : right.register.sbyte_15);
					return vector;
				}
				if (typeof(T) == typeof(ushort))
				{
					vector.register.uint16_0 = ((left.register.uint16_0 > right.register.uint16_0) ? left.register.uint16_0 : right.register.uint16_0);
					vector.register.uint16_1 = ((left.register.uint16_1 > right.register.uint16_1) ? left.register.uint16_1 : right.register.uint16_1);
					vector.register.uint16_2 = ((left.register.uint16_2 > right.register.uint16_2) ? left.register.uint16_2 : right.register.uint16_2);
					vector.register.uint16_3 = ((left.register.uint16_3 > right.register.uint16_3) ? left.register.uint16_3 : right.register.uint16_3);
					vector.register.uint16_4 = ((left.register.uint16_4 > right.register.uint16_4) ? left.register.uint16_4 : right.register.uint16_4);
					vector.register.uint16_5 = ((left.register.uint16_5 > right.register.uint16_5) ? left.register.uint16_5 : right.register.uint16_5);
					vector.register.uint16_6 = ((left.register.uint16_6 > right.register.uint16_6) ? left.register.uint16_6 : right.register.uint16_6);
					vector.register.uint16_7 = ((left.register.uint16_7 > right.register.uint16_7) ? left.register.uint16_7 : right.register.uint16_7);
					return vector;
				}
				if (typeof(T) == typeof(short))
				{
					vector.register.int16_0 = ((left.register.int16_0 > right.register.int16_0) ? left.register.int16_0 : right.register.int16_0);
					vector.register.int16_1 = ((left.register.int16_1 > right.register.int16_1) ? left.register.int16_1 : right.register.int16_1);
					vector.register.int16_2 = ((left.register.int16_2 > right.register.int16_2) ? left.register.int16_2 : right.register.int16_2);
					vector.register.int16_3 = ((left.register.int16_3 > right.register.int16_3) ? left.register.int16_3 : right.register.int16_3);
					vector.register.int16_4 = ((left.register.int16_4 > right.register.int16_4) ? left.register.int16_4 : right.register.int16_4);
					vector.register.int16_5 = ((left.register.int16_5 > right.register.int16_5) ? left.register.int16_5 : right.register.int16_5);
					vector.register.int16_6 = ((left.register.int16_6 > right.register.int16_6) ? left.register.int16_6 : right.register.int16_6);
					vector.register.int16_7 = ((left.register.int16_7 > right.register.int16_7) ? left.register.int16_7 : right.register.int16_7);
					return vector;
				}
				if (typeof(T) == typeof(uint))
				{
					vector.register.uint32_0 = ((left.register.uint32_0 > right.register.uint32_0) ? left.register.uint32_0 : right.register.uint32_0);
					vector.register.uint32_1 = ((left.register.uint32_1 > right.register.uint32_1) ? left.register.uint32_1 : right.register.uint32_1);
					vector.register.uint32_2 = ((left.register.uint32_2 > right.register.uint32_2) ? left.register.uint32_2 : right.register.uint32_2);
					vector.register.uint32_3 = ((left.register.uint32_3 > right.register.uint32_3) ? left.register.uint32_3 : right.register.uint32_3);
					return vector;
				}
				if (typeof(T) == typeof(int))
				{
					vector.register.int32_0 = ((left.register.int32_0 > right.register.int32_0) ? left.register.int32_0 : right.register.int32_0);
					vector.register.int32_1 = ((left.register.int32_1 > right.register.int32_1) ? left.register.int32_1 : right.register.int32_1);
					vector.register.int32_2 = ((left.register.int32_2 > right.register.int32_2) ? left.register.int32_2 : right.register.int32_2);
					vector.register.int32_3 = ((left.register.int32_3 > right.register.int32_3) ? left.register.int32_3 : right.register.int32_3);
					return vector;
				}
				if (typeof(T) == typeof(ulong))
				{
					vector.register.uint64_0 = ((left.register.uint64_0 > right.register.uint64_0) ? left.register.uint64_0 : right.register.uint64_0);
					vector.register.uint64_1 = ((left.register.uint64_1 > right.register.uint64_1) ? left.register.uint64_1 : right.register.uint64_1);
					return vector;
				}
				if (typeof(T) == typeof(long))
				{
					vector.register.int64_0 = ((left.register.int64_0 > right.register.int64_0) ? left.register.int64_0 : right.register.int64_0);
					vector.register.int64_1 = ((left.register.int64_1 > right.register.int64_1) ? left.register.int64_1 : right.register.int64_1);
					return vector;
				}
				if (typeof(T) == typeof(float))
				{
					vector.register.single_0 = ((left.register.single_0 > right.register.single_0) ? left.register.single_0 : right.register.single_0);
					vector.register.single_1 = ((left.register.single_1 > right.register.single_1) ? left.register.single_1 : right.register.single_1);
					vector.register.single_2 = ((left.register.single_2 > right.register.single_2) ? left.register.single_2 : right.register.single_2);
					vector.register.single_3 = ((left.register.single_3 > right.register.single_3) ? left.register.single_3 : right.register.single_3);
					return vector;
				}
				if (typeof(T) == typeof(double))
				{
					vector.register.double_0 = ((left.register.double_0 > right.register.double_0) ? left.register.double_0 : right.register.double_0);
					vector.register.double_1 = ((left.register.double_1 > right.register.double_1) ? left.register.double_1 : right.register.double_1);
					return vector;
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00035344 File Offset: 0x00033544
		[Intrinsic]
		internal static T DotProduct(Vector<T> left, Vector<T> right)
		{
			if (Vector.IsHardwareAccelerated)
			{
				T t = default(T);
				for (int i = 0; i < Vector<T>.Count; i++)
				{
					t = Vector<T>.ScalarAdd(t, Vector<T>.ScalarMultiply(left[i], right[i]));
				}
				return t;
			}
			if (typeof(T) == typeof(byte))
			{
				byte b = 0;
				b += left.register.byte_0 * right.register.byte_0;
				b += left.register.byte_1 * right.register.byte_1;
				b += left.register.byte_2 * right.register.byte_2;
				b += left.register.byte_3 * right.register.byte_3;
				b += left.register.byte_4 * right.register.byte_4;
				b += left.register.byte_5 * right.register.byte_5;
				b += left.register.byte_6 * right.register.byte_6;
				b += left.register.byte_7 * right.register.byte_7;
				b += left.register.byte_8 * right.register.byte_8;
				b += left.register.byte_9 * right.register.byte_9;
				b += left.register.byte_10 * right.register.byte_10;
				b += left.register.byte_11 * right.register.byte_11;
				b += left.register.byte_12 * right.register.byte_12;
				b += left.register.byte_13 * right.register.byte_13;
				b += left.register.byte_14 * right.register.byte_14;
				b += left.register.byte_15 * right.register.byte_15;
				return (T)((object)b);
			}
			if (typeof(T) == typeof(sbyte))
			{
				sbyte b2 = 0;
				b2 += left.register.sbyte_0 * right.register.sbyte_0;
				b2 += left.register.sbyte_1 * right.register.sbyte_1;
				b2 += left.register.sbyte_2 * right.register.sbyte_2;
				b2 += left.register.sbyte_3 * right.register.sbyte_3;
				b2 += left.register.sbyte_4 * right.register.sbyte_4;
				b2 += left.register.sbyte_5 * right.register.sbyte_5;
				b2 += left.register.sbyte_6 * right.register.sbyte_6;
				b2 += left.register.sbyte_7 * right.register.sbyte_7;
				b2 += left.register.sbyte_8 * right.register.sbyte_8;
				b2 += left.register.sbyte_9 * right.register.sbyte_9;
				b2 += left.register.sbyte_10 * right.register.sbyte_10;
				b2 += left.register.sbyte_11 * right.register.sbyte_11;
				b2 += left.register.sbyte_12 * right.register.sbyte_12;
				b2 += left.register.sbyte_13 * right.register.sbyte_13;
				b2 += left.register.sbyte_14 * right.register.sbyte_14;
				b2 += left.register.sbyte_15 * right.register.sbyte_15;
				return (T)((object)b2);
			}
			if (typeof(T) == typeof(ushort))
			{
				ushort num = 0;
				num += left.register.uint16_0 * right.register.uint16_0;
				num += left.register.uint16_1 * right.register.uint16_1;
				num += left.register.uint16_2 * right.register.uint16_2;
				num += left.register.uint16_3 * right.register.uint16_3;
				num += left.register.uint16_4 * right.register.uint16_4;
				num += left.register.uint16_5 * right.register.uint16_5;
				num += left.register.uint16_6 * right.register.uint16_6;
				num += left.register.uint16_7 * right.register.uint16_7;
				return (T)((object)num);
			}
			if (typeof(T) == typeof(short))
			{
				short num2 = 0;
				num2 += left.register.int16_0 * right.register.int16_0;
				num2 += left.register.int16_1 * right.register.int16_1;
				num2 += left.register.int16_2 * right.register.int16_2;
				num2 += left.register.int16_3 * right.register.int16_3;
				num2 += left.register.int16_4 * right.register.int16_4;
				num2 += left.register.int16_5 * right.register.int16_5;
				num2 += left.register.int16_6 * right.register.int16_6;
				num2 += left.register.int16_7 * right.register.int16_7;
				return (T)((object)num2);
			}
			if (typeof(T) == typeof(uint))
			{
				uint num3 = 0U;
				num3 += left.register.uint32_0 * right.register.uint32_0;
				num3 += left.register.uint32_1 * right.register.uint32_1;
				num3 += left.register.uint32_2 * right.register.uint32_2;
				num3 += left.register.uint32_3 * right.register.uint32_3;
				return (T)((object)num3);
			}
			if (typeof(T) == typeof(int))
			{
				int num4 = 0;
				num4 += left.register.int32_0 * right.register.int32_0;
				num4 += left.register.int32_1 * right.register.int32_1;
				num4 += left.register.int32_2 * right.register.int32_2;
				num4 += left.register.int32_3 * right.register.int32_3;
				return (T)((object)num4);
			}
			if (typeof(T) == typeof(ulong))
			{
				ulong num5 = 0UL;
				num5 += left.register.uint64_0 * right.register.uint64_0;
				num5 += left.register.uint64_1 * right.register.uint64_1;
				return (T)((object)num5);
			}
			if (typeof(T) == typeof(long))
			{
				long num6 = 0L;
				num6 += left.register.int64_0 * right.register.int64_0;
				num6 += left.register.int64_1 * right.register.int64_1;
				return (T)((object)num6);
			}
			if (typeof(T) == typeof(float))
			{
				float num7 = 0f;
				num7 += left.register.single_0 * right.register.single_0;
				num7 += left.register.single_1 * right.register.single_1;
				num7 += left.register.single_2 * right.register.single_2;
				num7 += left.register.single_3 * right.register.single_3;
				return (T)((object)num7);
			}
			if (typeof(T) == typeof(double))
			{
				double num8 = 0.0;
				num8 += left.register.double_0 * right.register.double_0;
				num8 += left.register.double_1 * right.register.double_1;
				return (T)((object)num8);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00035CD8 File Offset: 0x00033ED8
		[Intrinsic]
		internal unsafe static Vector<T> SquareRoot(Vector<T> value)
		{
			if (Vector.IsHardwareAccelerated)
			{
				if (typeof(T) == typeof(byte))
				{
					byte* ptr = stackalloc byte[(UIntPtr)Vector<T>.Count];
					for (int i = 0; i < Vector<T>.Count; i++)
					{
						ptr[i] = (byte)Math.Sqrt((double)((byte)((object)value[i])));
					}
					return new Vector<T>((void*)ptr);
				}
				if (typeof(T) == typeof(sbyte))
				{
					sbyte* ptr2 = stackalloc sbyte[(UIntPtr)Vector<T>.Count];
					for (int j = 0; j < Vector<T>.Count; j++)
					{
						ptr2[j] = (sbyte)Math.Sqrt((double)((sbyte)((object)value[j])));
					}
					return new Vector<T>((void*)ptr2);
				}
				if (typeof(T) == typeof(ushort))
				{
					ushort* ptr3;
					checked
					{
						ptr3 = stackalloc ushort[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int k = 0; k < Vector<T>.Count; k++)
					{
						ptr3[k] = (ushort)Math.Sqrt((double)((ushort)((object)value[k])));
					}
					return new Vector<T>((void*)ptr3);
				}
				if (typeof(T) == typeof(short))
				{
					short* ptr4;
					checked
					{
						ptr4 = stackalloc short[unchecked((UIntPtr)Vector<T>.Count) * 2];
					}
					for (int l = 0; l < Vector<T>.Count; l++)
					{
						ptr4[l] = (short)Math.Sqrt((double)((short)((object)value[l])));
					}
					return new Vector<T>((void*)ptr4);
				}
				if (typeof(T) == typeof(uint))
				{
					uint* ptr5;
					checked
					{
						ptr5 = stackalloc uint[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int m = 0; m < Vector<T>.Count; m++)
					{
						ptr5[m] = (uint)Math.Sqrt((uint)((object)value[m]));
					}
					return new Vector<T>((void*)ptr5);
				}
				if (typeof(T) == typeof(int))
				{
					int* ptr6;
					checked
					{
						ptr6 = stackalloc int[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int n = 0; n < Vector<T>.Count; n++)
					{
						ptr6[n] = (int)Math.Sqrt((double)((int)((object)value[n])));
					}
					return new Vector<T>((void*)ptr6);
				}
				if (typeof(T) == typeof(ulong))
				{
					ulong* ptr7;
					checked
					{
						ptr7 = stackalloc ulong[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num = 0; num < Vector<T>.Count; num++)
					{
						ptr7[num] = (ulong)Math.Sqrt((ulong)((object)value[num]));
					}
					return new Vector<T>((void*)ptr7);
				}
				if (typeof(T) == typeof(long))
				{
					long* ptr8;
					checked
					{
						ptr8 = stackalloc long[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num2 = 0; num2 < Vector<T>.Count; num2++)
					{
						ptr8[num2] = (long)Math.Sqrt((double)((long)((object)value[num2])));
					}
					return new Vector<T>((void*)ptr8);
				}
				if (typeof(T) == typeof(float))
				{
					float* ptr9;
					checked
					{
						ptr9 = stackalloc float[unchecked((UIntPtr)Vector<T>.Count) * 4];
					}
					for (int num3 = 0; num3 < Vector<T>.Count; num3++)
					{
						ptr9[num3] = (float)Math.Sqrt((double)((float)((object)value[num3])));
					}
					return new Vector<T>((void*)ptr9);
				}
				if (typeof(T) == typeof(double))
				{
					double* ptr10;
					checked
					{
						ptr10 = stackalloc double[unchecked((UIntPtr)Vector<T>.Count) * 8];
					}
					for (int num4 = 0; num4 < Vector<T>.Count; num4++)
					{
						ptr10[num4] = Math.Sqrt((double)((object)value[num4]));
					}
					return new Vector<T>((void*)ptr10);
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
			else
			{
				if (typeof(T) == typeof(byte))
				{
					value.register.byte_0 = (byte)Math.Sqrt((double)value.register.byte_0);
					value.register.byte_1 = (byte)Math.Sqrt((double)value.register.byte_1);
					value.register.byte_2 = (byte)Math.Sqrt((double)value.register.byte_2);
					value.register.byte_3 = (byte)Math.Sqrt((double)value.register.byte_3);
					value.register.byte_4 = (byte)Math.Sqrt((double)value.register.byte_4);
					value.register.byte_5 = (byte)Math.Sqrt((double)value.register.byte_5);
					value.register.byte_6 = (byte)Math.Sqrt((double)value.register.byte_6);
					value.register.byte_7 = (byte)Math.Sqrt((double)value.register.byte_7);
					value.register.byte_8 = (byte)Math.Sqrt((double)value.register.byte_8);
					value.register.byte_9 = (byte)Math.Sqrt((double)value.register.byte_9);
					value.register.byte_10 = (byte)Math.Sqrt((double)value.register.byte_10);
					value.register.byte_11 = (byte)Math.Sqrt((double)value.register.byte_11);
					value.register.byte_12 = (byte)Math.Sqrt((double)value.register.byte_12);
					value.register.byte_13 = (byte)Math.Sqrt((double)value.register.byte_13);
					value.register.byte_14 = (byte)Math.Sqrt((double)value.register.byte_14);
					value.register.byte_15 = (byte)Math.Sqrt((double)value.register.byte_15);
					return value;
				}
				if (typeof(T) == typeof(sbyte))
				{
					value.register.sbyte_0 = (sbyte)Math.Sqrt((double)value.register.sbyte_0);
					value.register.sbyte_1 = (sbyte)Math.Sqrt((double)value.register.sbyte_1);
					value.register.sbyte_2 = (sbyte)Math.Sqrt((double)value.register.sbyte_2);
					value.register.sbyte_3 = (sbyte)Math.Sqrt((double)value.register.sbyte_3);
					value.register.sbyte_4 = (sbyte)Math.Sqrt((double)value.register.sbyte_4);
					value.register.sbyte_5 = (sbyte)Math.Sqrt((double)value.register.sbyte_5);
					value.register.sbyte_6 = (sbyte)Math.Sqrt((double)value.register.sbyte_6);
					value.register.sbyte_7 = (sbyte)Math.Sqrt((double)value.register.sbyte_7);
					value.register.sbyte_8 = (sbyte)Math.Sqrt((double)value.register.sbyte_8);
					value.register.sbyte_9 = (sbyte)Math.Sqrt((double)value.register.sbyte_9);
					value.register.sbyte_10 = (sbyte)Math.Sqrt((double)value.register.sbyte_10);
					value.register.sbyte_11 = (sbyte)Math.Sqrt((double)value.register.sbyte_11);
					value.register.sbyte_12 = (sbyte)Math.Sqrt((double)value.register.sbyte_12);
					value.register.sbyte_13 = (sbyte)Math.Sqrt((double)value.register.sbyte_13);
					value.register.sbyte_14 = (sbyte)Math.Sqrt((double)value.register.sbyte_14);
					value.register.sbyte_15 = (sbyte)Math.Sqrt((double)value.register.sbyte_15);
					return value;
				}
				if (typeof(T) == typeof(ushort))
				{
					value.register.uint16_0 = (ushort)Math.Sqrt((double)value.register.uint16_0);
					value.register.uint16_1 = (ushort)Math.Sqrt((double)value.register.uint16_1);
					value.register.uint16_2 = (ushort)Math.Sqrt((double)value.register.uint16_2);
					value.register.uint16_3 = (ushort)Math.Sqrt((double)value.register.uint16_3);
					value.register.uint16_4 = (ushort)Math.Sqrt((double)value.register.uint16_4);
					value.register.uint16_5 = (ushort)Math.Sqrt((double)value.register.uint16_5);
					value.register.uint16_6 = (ushort)Math.Sqrt((double)value.register.uint16_6);
					value.register.uint16_7 = (ushort)Math.Sqrt((double)value.register.uint16_7);
					return value;
				}
				if (typeof(T) == typeof(short))
				{
					value.register.int16_0 = (short)Math.Sqrt((double)value.register.int16_0);
					value.register.int16_1 = (short)Math.Sqrt((double)value.register.int16_1);
					value.register.int16_2 = (short)Math.Sqrt((double)value.register.int16_2);
					value.register.int16_3 = (short)Math.Sqrt((double)value.register.int16_3);
					value.register.int16_4 = (short)Math.Sqrt((double)value.register.int16_4);
					value.register.int16_5 = (short)Math.Sqrt((double)value.register.int16_5);
					value.register.int16_6 = (short)Math.Sqrt((double)value.register.int16_6);
					value.register.int16_7 = (short)Math.Sqrt((double)value.register.int16_7);
					return value;
				}
				if (typeof(T) == typeof(uint))
				{
					value.register.uint32_0 = (uint)Math.Sqrt(value.register.uint32_0);
					value.register.uint32_1 = (uint)Math.Sqrt(value.register.uint32_1);
					value.register.uint32_2 = (uint)Math.Sqrt(value.register.uint32_2);
					value.register.uint32_3 = (uint)Math.Sqrt(value.register.uint32_3);
					return value;
				}
				if (typeof(T) == typeof(int))
				{
					value.register.int32_0 = (int)Math.Sqrt((double)value.register.int32_0);
					value.register.int32_1 = (int)Math.Sqrt((double)value.register.int32_1);
					value.register.int32_2 = (int)Math.Sqrt((double)value.register.int32_2);
					value.register.int32_3 = (int)Math.Sqrt((double)value.register.int32_3);
					return value;
				}
				if (typeof(T) == typeof(ulong))
				{
					value.register.uint64_0 = (ulong)Math.Sqrt(value.register.uint64_0);
					value.register.uint64_1 = (ulong)Math.Sqrt(value.register.uint64_1);
					return value;
				}
				if (typeof(T) == typeof(long))
				{
					value.register.int64_0 = (long)Math.Sqrt((double)value.register.int64_0);
					value.register.int64_1 = (long)Math.Sqrt((double)value.register.int64_1);
					return value;
				}
				if (typeof(T) == typeof(float))
				{
					value.register.single_0 = (float)Math.Sqrt((double)value.register.single_0);
					value.register.single_1 = (float)Math.Sqrt((double)value.register.single_1);
					value.register.single_2 = (float)Math.Sqrt((double)value.register.single_2);
					value.register.single_3 = (float)Math.Sqrt((double)value.register.single_3);
					return value;
				}
				if (typeof(T) == typeof(double))
				{
					value.register.double_0 = Math.Sqrt(value.register.double_0);
					value.register.double_1 = Math.Sqrt(value.register.double_1);
					return value;
				}
				throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00036A28 File Offset: 0x00034C28
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ScalarEquals(T left, T right)
		{
			if (typeof(T) == typeof(byte))
			{
				return (byte)((object)left) == (byte)((object)right);
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (sbyte)((object)left) == (sbyte)((object)right);
			}
			if (typeof(T) == typeof(ushort))
			{
				return (ushort)((object)left) == (ushort)((object)right);
			}
			if (typeof(T) == typeof(short))
			{
				return (short)((object)left) == (short)((object)right);
			}
			if (typeof(T) == typeof(uint))
			{
				return (uint)((object)left) == (uint)((object)right);
			}
			if (typeof(T) == typeof(int))
			{
				return (int)((object)left) == (int)((object)right);
			}
			if (typeof(T) == typeof(ulong))
			{
				return (ulong)((object)left) == (ulong)((object)right);
			}
			if (typeof(T) == typeof(long))
			{
				return (long)((object)left) == (long)((object)right);
			}
			if (typeof(T) == typeof(float))
			{
				return (float)((object)left) == (float)((object)right);
			}
			if (typeof(T) == typeof(double))
			{
				return (double)((object)left) == (double)((object)right);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00036C6C File Offset: 0x00034E6C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ScalarLessThan(T left, T right)
		{
			if (typeof(T) == typeof(byte))
			{
				return (byte)((object)left) < (byte)((object)right);
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (sbyte)((object)left) < (sbyte)((object)right);
			}
			if (typeof(T) == typeof(ushort))
			{
				return (ushort)((object)left) < (ushort)((object)right);
			}
			if (typeof(T) == typeof(short))
			{
				return (short)((object)left) < (short)((object)right);
			}
			if (typeof(T) == typeof(uint))
			{
				return (uint)((object)left) < (uint)((object)right);
			}
			if (typeof(T) == typeof(int))
			{
				return (int)((object)left) < (int)((object)right);
			}
			if (typeof(T) == typeof(ulong))
			{
				return (ulong)((object)left) < (ulong)((object)right);
			}
			if (typeof(T) == typeof(long))
			{
				return (long)((object)left) < (long)((object)right);
			}
			if (typeof(T) == typeof(float))
			{
				return (float)((object)left) < (float)((object)right);
			}
			if (typeof(T) == typeof(double))
			{
				return (double)((object)left) < (double)((object)right);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x00036EB0 File Offset: 0x000350B0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ScalarGreaterThan(T left, T right)
		{
			if (typeof(T) == typeof(byte))
			{
				return (byte)((object)left) > (byte)((object)right);
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (sbyte)((object)left) > (sbyte)((object)right);
			}
			if (typeof(T) == typeof(ushort))
			{
				return (ushort)((object)left) > (ushort)((object)right);
			}
			if (typeof(T) == typeof(short))
			{
				return (short)((object)left) > (short)((object)right);
			}
			if (typeof(T) == typeof(uint))
			{
				return (uint)((object)left) > (uint)((object)right);
			}
			if (typeof(T) == typeof(int))
			{
				return (int)((object)left) > (int)((object)right);
			}
			if (typeof(T) == typeof(ulong))
			{
				return (ulong)((object)left) > (ulong)((object)right);
			}
			if (typeof(T) == typeof(long))
			{
				return (long)((object)left) > (long)((object)right);
			}
			if (typeof(T) == typeof(float))
			{
				return (float)((object)left) > (float)((object)right);
			}
			if (typeof(T) == typeof(double))
			{
				return (double)((object)left) > (double)((object)right);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x000370F4 File Offset: 0x000352F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T ScalarAdd(T left, T right)
		{
			if (typeof(T) == typeof(byte))
			{
				return (T)((object)((byte)((object)left) + (byte)((object)right)));
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (T)((object)((sbyte)((object)left) + (sbyte)((object)right)));
			}
			if (typeof(T) == typeof(ushort))
			{
				return (T)((object)((ushort)((object)left) + (ushort)((object)right)));
			}
			if (typeof(T) == typeof(short))
			{
				return (T)((object)((short)((object)left) + (short)((object)right)));
			}
			if (typeof(T) == typeof(uint))
			{
				return (T)((object)((uint)((object)left) + (uint)((object)right)));
			}
			if (typeof(T) == typeof(int))
			{
				return (T)((object)((int)((object)left) + (int)((object)right)));
			}
			if (typeof(T) == typeof(ulong))
			{
				return (T)((object)((ulong)((object)left) + (ulong)((object)right)));
			}
			if (typeof(T) == typeof(long))
			{
				return (T)((object)((long)((object)left) + (long)((object)right)));
			}
			if (typeof(T) == typeof(float))
			{
				return (T)((object)((float)((object)left) + (float)((object)right)));
			}
			if (typeof(T) == typeof(double))
			{
				return (T)((object)((double)((object)left) + (double)((object)right)));
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x00037398 File Offset: 0x00035598
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T ScalarSubtract(T left, T right)
		{
			if (typeof(T) == typeof(byte))
			{
				return (T)((object)((byte)((object)left) - (byte)((object)right)));
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (T)((object)((sbyte)((object)left) - (sbyte)((object)right)));
			}
			if (typeof(T) == typeof(ushort))
			{
				return (T)((object)((ushort)((object)left) - (ushort)((object)right)));
			}
			if (typeof(T) == typeof(short))
			{
				return (T)((object)((short)((object)left) - (short)((object)right)));
			}
			if (typeof(T) == typeof(uint))
			{
				return (T)((object)((uint)((object)left) - (uint)((object)right)));
			}
			if (typeof(T) == typeof(int))
			{
				return (T)((object)((int)((object)left) - (int)((object)right)));
			}
			if (typeof(T) == typeof(ulong))
			{
				return (T)((object)((ulong)((object)left) - (ulong)((object)right)));
			}
			if (typeof(T) == typeof(long))
			{
				return (T)((object)((long)((object)left) - (long)((object)right)));
			}
			if (typeof(T) == typeof(float))
			{
				return (T)((object)((float)((object)left) - (float)((object)right)));
			}
			if (typeof(T) == typeof(double))
			{
				return (T)((object)((double)((object)left) - (double)((object)right)));
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0003763C File Offset: 0x0003583C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T ScalarMultiply(T left, T right)
		{
			if (typeof(T) == typeof(byte))
			{
				return (T)((object)((byte)((object)left) * (byte)((object)right)));
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (T)((object)((sbyte)((object)left) * (sbyte)((object)right)));
			}
			if (typeof(T) == typeof(ushort))
			{
				return (T)((object)((ushort)((object)left) * (ushort)((object)right)));
			}
			if (typeof(T) == typeof(short))
			{
				return (T)((object)((short)((object)left) * (short)((object)right)));
			}
			if (typeof(T) == typeof(uint))
			{
				return (T)((object)((uint)((object)left) * (uint)((object)right)));
			}
			if (typeof(T) == typeof(int))
			{
				return (T)((object)((int)((object)left) * (int)((object)right)));
			}
			if (typeof(T) == typeof(ulong))
			{
				return (T)((object)((ulong)((object)left) * (ulong)((object)right)));
			}
			if (typeof(T) == typeof(long))
			{
				return (T)((object)((long)((object)left) * (long)((object)right)));
			}
			if (typeof(T) == typeof(float))
			{
				return (T)((object)((float)((object)left) * (float)((object)right)));
			}
			if (typeof(T) == typeof(double))
			{
				return (T)((object)((double)((object)left) * (double)((object)right)));
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x000378E0 File Offset: 0x00035AE0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T ScalarDivide(T left, T right)
		{
			if (typeof(T) == typeof(byte))
			{
				return (T)((object)((byte)((object)left) / (byte)((object)right)));
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (T)((object)((sbyte)((object)left) / (sbyte)((object)right)));
			}
			if (typeof(T) == typeof(ushort))
			{
				return (T)((object)((ushort)((object)left) / (ushort)((object)right)));
			}
			if (typeof(T) == typeof(short))
			{
				return (T)((object)((short)((object)left) / (short)((object)right)));
			}
			if (typeof(T) == typeof(uint))
			{
				return (T)((object)((uint)((object)left) / (uint)((object)right)));
			}
			if (typeof(T) == typeof(int))
			{
				return (T)((object)((int)((object)left) / (int)((object)right)));
			}
			if (typeof(T) == typeof(ulong))
			{
				return (T)((object)((ulong)((object)left) / (ulong)((object)right)));
			}
			if (typeof(T) == typeof(long))
			{
				return (T)((object)((long)((object)left) / (long)((object)right)));
			}
			if (typeof(T) == typeof(float))
			{
				return (T)((object)((float)((object)left) / (float)((object)right)));
			}
			if (typeof(T) == typeof(double))
			{
				return (T)((object)((double)((object)left) / (double)((object)right)));
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x00037B84 File Offset: 0x00035D84
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T GetOneValue()
		{
			if (typeof(T) == typeof(byte))
			{
				byte b = 1;
				return (T)((object)b);
			}
			if (typeof(T) == typeof(sbyte))
			{
				sbyte b2 = 1;
				return (T)((object)b2);
			}
			if (typeof(T) == typeof(ushort))
			{
				ushort num = 1;
				return (T)((object)num);
			}
			if (typeof(T) == typeof(short))
			{
				short num2 = 1;
				return (T)((object)num2);
			}
			if (typeof(T) == typeof(uint))
			{
				uint num3 = 1U;
				return (T)((object)num3);
			}
			if (typeof(T) == typeof(int))
			{
				int num4 = 1;
				return (T)((object)num4);
			}
			if (typeof(T) == typeof(ulong))
			{
				ulong num5 = 1UL;
				return (T)((object)num5);
			}
			if (typeof(T) == typeof(long))
			{
				long num6 = 1L;
				return (T)((object)num6);
			}
			if (typeof(T) == typeof(float))
			{
				float num7 = 1f;
				return (T)((object)num7);
			}
			if (typeof(T) == typeof(double))
			{
				double num8 = 1.0;
				return (T)((object)num8);
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00037D74 File Offset: 0x00035F74
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T GetAllBitsSetValue()
		{
			if (typeof(T) == typeof(byte))
			{
				return (T)((object)ConstantHelper.GetByteWithAllBitsSet());
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (T)((object)ConstantHelper.GetSByteWithAllBitsSet());
			}
			if (typeof(T) == typeof(ushort))
			{
				return (T)((object)ConstantHelper.GetUInt16WithAllBitsSet());
			}
			if (typeof(T) == typeof(short))
			{
				return (T)((object)ConstantHelper.GetInt16WithAllBitsSet());
			}
			if (typeof(T) == typeof(uint))
			{
				return (T)((object)ConstantHelper.GetUInt32WithAllBitsSet());
			}
			if (typeof(T) == typeof(int))
			{
				return (T)((object)ConstantHelper.GetInt32WithAllBitsSet());
			}
			if (typeof(T) == typeof(ulong))
			{
				return (T)((object)ConstantHelper.GetUInt64WithAllBitsSet());
			}
			if (typeof(T) == typeof(long))
			{
				return (T)((object)ConstantHelper.GetInt64WithAllBitsSet());
			}
			if (typeof(T) == typeof(float))
			{
				return (T)((object)ConstantHelper.GetSingleWithAllBitsSet());
			}
			if (typeof(T) == typeof(double))
			{
				return (T)((object)ConstantHelper.GetDoubleWithAllBitsSet());
			}
			throw new NotSupportedException(System.Numerics.Vectors159800.SR.Arg_TypeNotSupported);
		}

		// Token: 0x04000336 RID: 822
		private Register register;

		// Token: 0x04000337 RID: 823
		private static readonly int s_count = Vector<T>.InitializeCount();

		// Token: 0x04000338 RID: 824
		private static readonly Vector<T> s_zero = default(Vector<T>);

		// Token: 0x04000339 RID: 825
		private static readonly Vector<T> s_one = new Vector<T>(Vector<T>.GetOneValue());

		// Token: 0x0400033A RID: 826
		private static readonly Vector<T> s_allOnes = new Vector<T>(Vector<T>.GetAllBitsSetValue());

		// Token: 0x020001EA RID: 490
		private struct VectorSizeHelper
		{
			// Token: 0x0400088F RID: 2191
			internal Vector<T> _placeholder;

			// Token: 0x04000890 RID: 2192
			internal byte _byte;
		}
	}
}
