using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices
{
	// Token: 0x0200004F RID: 79
	[ComVisible(true)]
	public static class Unsafe
	{
		// Token: 0x060003A7 RID: 935 RVA: 0x0001A62C File Offset: 0x0001882C
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T Read<T>(void* source)
		{
			return *(T*)source;
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x0001A634 File Offset: 0x00018834
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T ReadUnaligned<T>(void* source)
		{
			return *(T*)source;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0001A640 File Offset: 0x00018840
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadUnaligned<T>(ref byte source)
		{
			return source;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0001A64C File Offset: 0x0001884C
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Write<T>(void* destination, T value)
		{
			*(T*)destination = value;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0001A658 File Offset: 0x00018858
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUnaligned<T>(void* destination, T value)
		{
			*(T*)destination = value;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0001A664 File Offset: 0x00018864
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUnaligned<T>(ref byte destination, T value)
		{
			destination = value;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0001A670 File Offset: 0x00018870
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(void* destination, ref T source)
		{
			*(T*)destination = source;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0001A680 File Offset: 0x00018880
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(ref T destination, void* source)
		{
			destination = *(T*)source;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0001A690 File Offset: 0x00018890
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* AsPointer<T>(ref T value)
		{
			return (void*)(&value);
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0001A694 File Offset: 0x00018894
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SkipInit<T>(out T value)
		{
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0001A698 File Offset: 0x00018898
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SizeOf<T>()
		{
			return sizeof(T);
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0001A6A0 File Offset: 0x000188A0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlock(void* destination, void* source, uint byteCount)
		{
			cpblk(destination, source, byteCount);
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0001A6A8 File Offset: 0x000188A8
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
		{
			cpblk(ref destination, ref source, byteCount);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0001A6B0 File Offset: 0x000188B0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
		{
			cpblk(destination, source, byteCount);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0001A6B8 File Offset: 0x000188B8
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlockUnaligned(ref byte destination, ref byte source, uint byteCount)
		{
			cpblk(ref destination, ref source, byteCount);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0001A6C0 File Offset: 0x000188C0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlock(void* startAddress, byte value, uint byteCount)
		{
			initblk(startAddress, value, byteCount);
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0001A6C8 File Offset: 0x000188C8
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
		{
			initblk(ref startAddress, value, byteCount);
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0001A6D0 File Offset: 0x000188D0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
		{
			initblk(startAddress, value, byteCount);
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0001A6D8 File Offset: 0x000188D8
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
		{
			initblk(ref startAddress, value, byteCount);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0001A6E0 File Offset: 0x000188E0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T As<T>(object o) where T : class
		{
			return o;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0001A6E4 File Offset: 0x000188E4
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T AsRef<T>(void* source)
		{
			return ref *(T*)source;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0001A6F8 File Offset: 0x000188F8
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<T>([System.Runtime.CompilerServices.Unsafe99943.IsReadOnly] ref T source)
		{
			return ref source;
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0001A6FC File Offset: 0x000188FC
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref TTo As<TFrom, TTo>(ref TFrom source)
		{
			return ref source;
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0001A700 File Offset: 0x00018900
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Unbox<T>(object box) where T : struct
		{
			return ref (T)box;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0001A708 File Offset: 0x00018908
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, int elementOffset)
		{
			return (ref source) + (IntPtr)elementOffset * (IntPtr)sizeof(T);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0001A718 File Offset: 0x00018918
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Add<T>(void* source, int elementOffset)
		{
			return (void*)((byte*)source + (IntPtr)elementOffset * (IntPtr)sizeof(T));
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0001A728 File Offset: 0x00018928
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, IntPtr elementOffset)
		{
			return (ref source) + elementOffset * (IntPtr)sizeof(T);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0001A734 File Offset: 0x00018934
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, [NativeInteger] [NonVersionable] UIntPtr elementOffset)
		{
			return (ref source) + elementOffset * (UIntPtr)sizeof(T);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0001A740 File Offset: 0x00018940
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
		{
			return (ref source) + byteOffset;
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0001A748 File Offset: 0x00018948
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, [NonVersionable] [NativeInteger] UIntPtr byteOffset)
		{
			return (ref source) + byteOffset;
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0001A750 File Offset: 0x00018950
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, int elementOffset)
		{
			return (ref source) - (IntPtr)elementOffset * (IntPtr)sizeof(T);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0001A760 File Offset: 0x00018960
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Subtract<T>(void* source, int elementOffset)
		{
			return (void*)((byte*)source - (IntPtr)elementOffset * (IntPtr)sizeof(T));
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0001A770 File Offset: 0x00018970
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, IntPtr elementOffset)
		{
			return (ref source) - elementOffset * (IntPtr)sizeof(T);
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0001A77C File Offset: 0x0001897C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, [NonVersionable] [NativeInteger] UIntPtr elementOffset)
		{
			return (ref source) - elementOffset * (UIntPtr)sizeof(T);
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0001A788 File Offset: 0x00018988
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, IntPtr byteOffset)
		{
			return (ref source) - byteOffset;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0001A790 File Offset: 0x00018990
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, [NativeInteger] [NonVersionable] UIntPtr byteOffset)
		{
			return (ref source) - byteOffset;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0001A798 File Offset: 0x00018998
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr ByteOffset<T>(ref T origin, ref T target)
		{
			return (ref target) - (ref origin);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0001A7A0 File Offset: 0x000189A0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AreSame<T>(ref T left, ref T right)
		{
			return (ref left) == (ref right);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0001A7A8 File Offset: 0x000189A8
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
		{
			return (ref left) != (ref right);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0001A7B0 File Offset: 0x000189B0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressLessThan<T>(ref T left, ref T right)
		{
			return (ref left) < (ref right);
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0001A7B8 File Offset: 0x000189B8
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullRef<T>(ref T source)
		{
			return (ref source) == (UIntPtr)0;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0001A7C0 File Offset: 0x000189C0
		[NonVersionable]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T NullRef<T>()
		{
			return (UIntPtr)0;
		}
	}
}
