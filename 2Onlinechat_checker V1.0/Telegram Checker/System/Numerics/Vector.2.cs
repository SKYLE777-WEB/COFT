using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
	// Token: 0x02000094 RID: 148
	[Intrinsic]
	[ComVisible(true)]
	public static class Vector
	{
		// Token: 0x06000712 RID: 1810 RVA: 0x00037F94 File Offset: 0x00036194
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static void Widen(Vector<byte> source, out Vector<ushort> low, out Vector<ushort> high)
		{
			int count = Vector<byte>.Count;
			ushort* ptr;
			checked
			{
				ptr = stackalloc ushort[unchecked((UIntPtr)(count / 2)) * 2];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (ushort)source[i];
			}
			ushort* ptr2;
			checked
			{
				ptr2 = stackalloc ushort[unchecked((UIntPtr)(count / 2)) * 2];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr2[j] = (ushort)source[j + count / 2];
			}
			low = new Vector<ushort>((void*)ptr);
			high = new Vector<ushort>((void*)ptr2);
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x00038020 File Offset: 0x00036220
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static void Widen(Vector<ushort> source, out Vector<uint> low, out Vector<uint> high)
		{
			int count = Vector<ushort>.Count;
			uint* ptr;
			checked
			{
				ptr = stackalloc uint[unchecked((UIntPtr)(count / 2)) * 4];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (uint)source[i];
			}
			uint* ptr2;
			checked
			{
				ptr2 = stackalloc uint[unchecked((UIntPtr)(count / 2)) * 4];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr2[j] = (uint)source[j + count / 2];
			}
			low = new Vector<uint>((void*)ptr);
			high = new Vector<uint>((void*)ptr2);
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x000380AC File Offset: 0x000362AC
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static void Widen(Vector<uint> source, out Vector<ulong> low, out Vector<ulong> high)
		{
			int count = Vector<uint>.Count;
			ulong* ptr;
			checked
			{
				ptr = stackalloc ulong[unchecked((UIntPtr)(count / 2)) * 8];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (ulong)source[i];
			}
			ulong* ptr2;
			checked
			{
				ptr2 = stackalloc ulong[unchecked((UIntPtr)(count / 2)) * 8];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr2[j] = (ulong)source[j + count / 2];
			}
			low = new Vector<ulong>((void*)ptr);
			high = new Vector<ulong>((void*)ptr2);
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0003813C File Offset: 0x0003633C
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static void Widen(Vector<sbyte> source, out Vector<short> low, out Vector<short> high)
		{
			int count = Vector<sbyte>.Count;
			short* ptr;
			checked
			{
				ptr = stackalloc short[unchecked((UIntPtr)(count / 2)) * 2];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (short)source[i];
			}
			short* ptr2;
			checked
			{
				ptr2 = stackalloc short[unchecked((UIntPtr)(count / 2)) * 2];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr2[j] = (short)source[j + count / 2];
			}
			low = new Vector<short>((void*)ptr);
			high = new Vector<short>((void*)ptr2);
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x000381C8 File Offset: 0x000363C8
		[Intrinsic]
		public unsafe static void Widen(Vector<short> source, out Vector<int> low, out Vector<int> high)
		{
			int count = Vector<short>.Count;
			int* ptr;
			checked
			{
				ptr = stackalloc int[unchecked((UIntPtr)(count / 2)) * 4];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (int)source[i];
			}
			int* ptr2;
			checked
			{
				ptr2 = stackalloc int[unchecked((UIntPtr)(count / 2)) * 4];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr2[j] = (int)source[j + count / 2];
			}
			low = new Vector<int>((void*)ptr);
			high = new Vector<int>((void*)ptr2);
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x00038254 File Offset: 0x00036454
		[Intrinsic]
		public unsafe static void Widen(Vector<int> source, out Vector<long> low, out Vector<long> high)
		{
			int count = Vector<int>.Count;
			long* ptr;
			checked
			{
				ptr = stackalloc long[unchecked((UIntPtr)(count / 2)) * 8];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (long)source[i];
			}
			long* ptr2;
			checked
			{
				ptr2 = stackalloc long[unchecked((UIntPtr)(count / 2)) * 8];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr2[j] = (long)source[j + count / 2];
			}
			low = new Vector<long>((void*)ptr);
			high = new Vector<long>((void*)ptr2);
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x000382E4 File Offset: 0x000364E4
		[Intrinsic]
		public unsafe static void Widen(Vector<float> source, out Vector<double> low, out Vector<double> high)
		{
			int count = Vector<float>.Count;
			double* ptr;
			checked
			{
				ptr = stackalloc double[unchecked((UIntPtr)(count / 2)) * 8];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (double)source[i];
			}
			double* ptr2;
			checked
			{
				ptr2 = stackalloc double[unchecked((UIntPtr)(count / 2)) * 8];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr2[j] = (double)source[j + count / 2];
			}
			low = new Vector<double>((void*)ptr);
			high = new Vector<double>((void*)ptr2);
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x00038374 File Offset: 0x00036574
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<byte> Narrow(Vector<ushort> low, Vector<ushort> high)
		{
			int count = Vector<byte>.Count;
			byte* ptr = stackalloc byte[(UIntPtr)count];
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (byte)low[i];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr[j + count / 2] = (byte)high[j];
			}
			return new Vector<byte>((void*)ptr);
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x000383D8 File Offset: 0x000365D8
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<ushort> Narrow(Vector<uint> low, Vector<uint> high)
		{
			int count = Vector<ushort>.Count;
			ushort* ptr;
			checked
			{
				ptr = stackalloc ushort[unchecked((UIntPtr)count) * 2];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (ushort)low[i];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr[j + count / 2] = (ushort)high[j];
			}
			return new Vector<ushort>((void*)ptr);
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00038444 File Offset: 0x00036644
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<uint> Narrow(Vector<ulong> low, Vector<ulong> high)
		{
			int count = Vector<uint>.Count;
			uint* ptr;
			checked
			{
				ptr = stackalloc uint[unchecked((UIntPtr)count) * 4];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (uint)low[i];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr[j + count / 2] = (uint)high[j];
			}
			return new Vector<uint>((void*)ptr);
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x000384B0 File Offset: 0x000366B0
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<sbyte> Narrow(Vector<short> low, Vector<short> high)
		{
			int count = Vector<sbyte>.Count;
			sbyte* ptr = stackalloc sbyte[(UIntPtr)count];
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (sbyte)low[i];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr[j + count / 2] = (sbyte)high[j];
			}
			return new Vector<sbyte>((void*)ptr);
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x00038514 File Offset: 0x00036714
		[Intrinsic]
		public unsafe static Vector<short> Narrow(Vector<int> low, Vector<int> high)
		{
			int count = Vector<short>.Count;
			short* ptr;
			checked
			{
				ptr = stackalloc short[unchecked((UIntPtr)count) * 2];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (short)low[i];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr[j + count / 2] = (short)high[j];
			}
			return new Vector<short>((void*)ptr);
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x00038580 File Offset: 0x00036780
		[Intrinsic]
		public unsafe static Vector<int> Narrow(Vector<long> low, Vector<long> high)
		{
			int count = Vector<int>.Count;
			int* ptr;
			checked
			{
				ptr = stackalloc int[unchecked((UIntPtr)count) * 4];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (int)low[i];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr[j + count / 2] = (int)high[j];
			}
			return new Vector<int>((void*)ptr);
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x000385EC File Offset: 0x000367EC
		[Intrinsic]
		public unsafe static Vector<float> Narrow(Vector<double> low, Vector<double> high)
		{
			int count = Vector<float>.Count;
			float* ptr;
			checked
			{
				ptr = stackalloc float[unchecked((UIntPtr)count) * 4];
			}
			for (int i = 0; i < count / 2; i++)
			{
				ptr[i] = (float)low[i];
			}
			for (int j = 0; j < count / 2; j++)
			{
				ptr[j + count / 2] = (float)high[j];
			}
			return new Vector<float>((void*)ptr);
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00038658 File Offset: 0x00036858
		[Intrinsic]
		public unsafe static Vector<float> ConvertToSingle(Vector<int> value)
		{
			int count = Vector<float>.Count;
			float* ptr;
			checked
			{
				ptr = stackalloc float[unchecked((UIntPtr)count) * 4];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = (float)value[i];
			}
			return new Vector<float>((void*)ptr);
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x0003869C File Offset: 0x0003689C
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<float> ConvertToSingle(Vector<uint> value)
		{
			int count = Vector<float>.Count;
			float* ptr;
			checked
			{
				ptr = stackalloc float[unchecked((UIntPtr)count) * 4];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = value[i];
			}
			return new Vector<float>((void*)ptr);
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x000386E0 File Offset: 0x000368E0
		[Intrinsic]
		public unsafe static Vector<double> ConvertToDouble(Vector<long> value)
		{
			int count = Vector<double>.Count;
			double* ptr;
			checked
			{
				ptr = stackalloc double[unchecked((UIntPtr)count) * 8];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = (double)value[i];
			}
			return new Vector<double>((void*)ptr);
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00038724 File Offset: 0x00036924
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<double> ConvertToDouble(Vector<ulong> value)
		{
			int count = Vector<double>.Count;
			double* ptr;
			checked
			{
				ptr = stackalloc double[unchecked((UIntPtr)count) * 8];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = value[i];
			}
			return new Vector<double>((void*)ptr);
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00038768 File Offset: 0x00036968
		[Intrinsic]
		public unsafe static Vector<int> ConvertToInt32(Vector<float> value)
		{
			int count = Vector<int>.Count;
			int* ptr;
			checked
			{
				ptr = stackalloc int[unchecked((UIntPtr)count) * 4];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = (int)value[i];
			}
			return new Vector<int>((void*)ptr);
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x000387AC File Offset: 0x000369AC
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<uint> ConvertToUInt32(Vector<float> value)
		{
			int count = Vector<uint>.Count;
			uint* ptr;
			checked
			{
				ptr = stackalloc uint[unchecked((UIntPtr)count) * 4];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = (uint)value[i];
			}
			return new Vector<uint>((void*)ptr);
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x000387F0 File Offset: 0x000369F0
		[Intrinsic]
		public unsafe static Vector<long> ConvertToInt64(Vector<double> value)
		{
			int count = Vector<long>.Count;
			long* ptr;
			checked
			{
				ptr = stackalloc long[unchecked((UIntPtr)count) * 8];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = (long)value[i];
			}
			return new Vector<long>((void*)ptr);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00038834 File Offset: 0x00036A34
		[CLSCompliant(false)]
		[Intrinsic]
		public unsafe static Vector<ulong> ConvertToUInt64(Vector<double> value)
		{
			int count = Vector<ulong>.Count;
			ulong* ptr;
			checked
			{
				ptr = stackalloc ulong[unchecked((UIntPtr)count) * 8];
			}
			for (int i = 0; i < count; i++)
			{
				ptr[i] = (ulong)value[i];
			}
			return new Vector<ulong>((void*)ptr);
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x00038878 File Offset: 0x00036A78
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<float> ConditionalSelect(Vector<int> condition, Vector<float> left, Vector<float> right)
		{
			return Vector<float>.ConditionalSelect((Vector<float>)condition, left, right);
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x00038888 File Offset: 0x00036A88
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<double> ConditionalSelect(Vector<long> condition, Vector<double> left, Vector<double> right)
		{
			return Vector<double>.ConditionalSelect((Vector<double>)condition, left, right);
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x00038898 File Offset: 0x00036A98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> ConditionalSelect<T>(Vector<T> condition, Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.ConditionalSelect(condition, left, right);
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x000388A4 File Offset: 0x00036AA4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Equals<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.Equals(left, right);
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x000388B0 File Offset: 0x00036AB0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> Equals(Vector<float> left, Vector<float> right)
		{
			return (Vector<int>)Vector<float>.Equals(left, right);
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x000388C0 File Offset: 0x00036AC0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> Equals(Vector<int> left, Vector<int> right)
		{
			return Vector<int>.Equals(left, right);
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x000388CC File Offset: 0x00036ACC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> Equals(Vector<double> left, Vector<double> right)
		{
			return (Vector<long>)Vector<double>.Equals(left, right);
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x000388DC File Offset: 0x00036ADC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> Equals(Vector<long> left, Vector<long> right)
		{
			return Vector<long>.Equals(left, right);
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x000388E8 File Offset: 0x00036AE8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsAll<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left == right;
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x000388F4 File Offset: 0x00036AF4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualsAny<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return !Vector<T>.Equals(left, right).Equals(Vector<T>.Zero);
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0003891C File Offset: 0x00036B1C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> LessThan<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.LessThan(left, right);
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00038928 File Offset: 0x00036B28
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> LessThan(Vector<float> left, Vector<float> right)
		{
			return (Vector<int>)Vector<float>.LessThan(left, right);
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00038938 File Offset: 0x00036B38
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> LessThan(Vector<int> left, Vector<int> right)
		{
			return Vector<int>.LessThan(left, right);
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00038944 File Offset: 0x00036B44
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> LessThan(Vector<double> left, Vector<double> right)
		{
			return (Vector<long>)Vector<double>.LessThan(left, right);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x00038954 File Offset: 0x00036B54
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> LessThan(Vector<long> left, Vector<long> right)
		{
			return Vector<long>.LessThan(left, right);
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00038960 File Offset: 0x00036B60
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanAll<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return ((Vector<int>)Vector<T>.LessThan(left, right)).Equals(Vector<int>.AllOnes);
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0003898C File Offset: 0x00036B8C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanAny<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return !((Vector<int>)Vector<T>.LessThan(left, right)).Equals(Vector<int>.Zero);
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x000389BC File Offset: 0x00036BBC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> LessThanOrEqual<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.LessThanOrEqual(left, right);
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x000389C8 File Offset: 0x00036BC8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> LessThanOrEqual(Vector<float> left, Vector<float> right)
		{
			return (Vector<int>)Vector<float>.LessThanOrEqual(left, right);
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x000389D8 File Offset: 0x00036BD8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> LessThanOrEqual(Vector<int> left, Vector<int> right)
		{
			return Vector<int>.LessThanOrEqual(left, right);
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x000389E4 File Offset: 0x00036BE4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> LessThanOrEqual(Vector<long> left, Vector<long> right)
		{
			return Vector<long>.LessThanOrEqual(left, right);
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x000389F0 File Offset: 0x00036BF0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> LessThanOrEqual(Vector<double> left, Vector<double> right)
		{
			return (Vector<long>)Vector<double>.LessThanOrEqual(left, right);
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00038A00 File Offset: 0x00036C00
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqualAll<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return ((Vector<int>)Vector<T>.LessThanOrEqual(left, right)).Equals(Vector<int>.AllOnes);
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x00038A2C File Offset: 0x00036C2C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool LessThanOrEqualAny<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return !((Vector<int>)Vector<T>.LessThanOrEqual(left, right)).Equals(Vector<int>.Zero);
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x00038A5C File Offset: 0x00036C5C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> GreaterThan<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.GreaterThan(left, right);
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x00038A68 File Offset: 0x00036C68
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> GreaterThan(Vector<float> left, Vector<float> right)
		{
			return (Vector<int>)Vector<float>.GreaterThan(left, right);
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x00038A78 File Offset: 0x00036C78
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> GreaterThan(Vector<int> left, Vector<int> right)
		{
			return Vector<int>.GreaterThan(left, right);
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x00038A84 File Offset: 0x00036C84
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> GreaterThan(Vector<double> left, Vector<double> right)
		{
			return (Vector<long>)Vector<double>.GreaterThan(left, right);
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x00038A94 File Offset: 0x00036C94
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> GreaterThan(Vector<long> left, Vector<long> right)
		{
			return Vector<long>.GreaterThan(left, right);
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x00038AA0 File Offset: 0x00036CA0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GreaterThanAll<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return ((Vector<int>)Vector<T>.GreaterThan(left, right)).Equals(Vector<int>.AllOnes);
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00038ACC File Offset: 0x00036CCC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GreaterThanAny<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return !((Vector<int>)Vector<T>.GreaterThan(left, right)).Equals(Vector<int>.Zero);
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00038AFC File Offset: 0x00036CFC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> GreaterThanOrEqual<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.GreaterThanOrEqual(left, right);
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00038B08 File Offset: 0x00036D08
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> GreaterThanOrEqual(Vector<float> left, Vector<float> right)
		{
			return (Vector<int>)Vector<float>.GreaterThanOrEqual(left, right);
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00038B18 File Offset: 0x00036D18
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> GreaterThanOrEqual(Vector<int> left, Vector<int> right)
		{
			return Vector<int>.GreaterThanOrEqual(left, right);
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00038B24 File Offset: 0x00036D24
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> GreaterThanOrEqual(Vector<long> left, Vector<long> right)
		{
			return Vector<long>.GreaterThanOrEqual(left, right);
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00038B30 File Offset: 0x00036D30
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> GreaterThanOrEqual(Vector<double> left, Vector<double> right)
		{
			return (Vector<long>)Vector<double>.GreaterThanOrEqual(left, right);
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00038B40 File Offset: 0x00036D40
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GreaterThanOrEqualAll<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return ((Vector<int>)Vector<T>.GreaterThanOrEqual(left, right)).Equals(Vector<int>.AllOnes);
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00038B6C File Offset: 0x00036D6C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GreaterThanOrEqualAny<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return !((Vector<int>)Vector<T>.GreaterThanOrEqual(left, right)).Equals(Vector<int>.Zero);
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x0600074E RID: 1870 RVA: 0x00038B9C File Offset: 0x00036D9C
		public static bool IsHardwareAccelerated
		{
			[Intrinsic]
			get
			{
				return false;
			}
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00038BA0 File Offset: 0x00036DA0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Abs<T>(Vector<T> value) where T : struct
		{
			return Vector<T>.Abs(value);
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00038BA8 File Offset: 0x00036DA8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Min<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.Min(left, right);
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00038BB4 File Offset: 0x00036DB4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Max<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.Max(left, right);
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00038BC0 File Offset: 0x00036DC0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Dot<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return Vector<T>.DotProduct(left, right);
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00038BCC File Offset: 0x00036DCC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> SquareRoot<T>(Vector<T> value) where T : struct
		{
			return Vector<T>.SquareRoot(value);
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00038BD4 File Offset: 0x00036DD4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Add<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left + right;
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x00038BE0 File Offset: 0x00036DE0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Subtract<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left - right;
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00038BEC File Offset: 0x00036DEC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Multiply<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left * right;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00038BF8 File Offset: 0x00036DF8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Multiply<T>(Vector<T> left, T right) where T : struct
		{
			return left * right;
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x00038C04 File Offset: 0x00036E04
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Multiply<T>(T left, Vector<T> right) where T : struct
		{
			return left * right;
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00038C10 File Offset: 0x00036E10
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Divide<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left / right;
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00038C1C File Offset: 0x00036E1C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Negate<T>(Vector<T> value) where T : struct
		{
			return -value;
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00038C24 File Offset: 0x00036E24
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> BitwiseAnd<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left & right;
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00038C30 File Offset: 0x00036E30
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> BitwiseOr<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left | right;
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00038C3C File Offset: 0x00036E3C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> OnesComplement<T>(Vector<T> value) where T : struct
		{
			return ~value;
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x00038C44 File Offset: 0x00036E44
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> Xor<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left ^ right;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x00038C50 File Offset: 0x00036E50
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<T> AndNot<T>(Vector<T> left, Vector<T> right) where T : struct
		{
			return left & ~right;
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x00038C60 File Offset: 0x00036E60
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<byte> AsVectorByte<T>(Vector<T> value) where T : struct
		{
			return (Vector<byte>)value;
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00038C68 File Offset: 0x00036E68
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<sbyte> AsVectorSByte<T>(Vector<T> value) where T : struct
		{
			return (Vector<sbyte>)value;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00038C70 File Offset: 0x00036E70
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<ushort> AsVectorUInt16<T>(Vector<T> value) where T : struct
		{
			return (Vector<ushort>)value;
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00038C78 File Offset: 0x00036E78
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<short> AsVectorInt16<T>(Vector<T> value) where T : struct
		{
			return (Vector<short>)value;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00038C80 File Offset: 0x00036E80
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<uint> AsVectorUInt32<T>(Vector<T> value) where T : struct
		{
			return (Vector<uint>)value;
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00038C88 File Offset: 0x00036E88
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<int> AsVectorInt32<T>(Vector<T> value) where T : struct
		{
			return (Vector<int>)value;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00038C90 File Offset: 0x00036E90
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<ulong> AsVectorUInt64<T>(Vector<T> value) where T : struct
		{
			return (Vector<ulong>)value;
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00038C98 File Offset: 0x00036E98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<long> AsVectorInt64<T>(Vector<T> value) where T : struct
		{
			return (Vector<long>)value;
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00038CA0 File Offset: 0x00036EA0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<float> AsVectorSingle<T>(Vector<T> value) where T : struct
		{
			return (Vector<float>)value;
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00038CA8 File Offset: 0x00036EA8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector<double> AsVectorDouble<T>(Vector<T> value) where T : struct
		{
			return (Vector<double>)value;
		}
	}
}
