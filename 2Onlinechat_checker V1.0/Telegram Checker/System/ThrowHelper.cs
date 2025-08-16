using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace System
{
	// Token: 0x02000024 RID: 36
	internal static class ThrowHelper
	{
		// Token: 0x06000110 RID: 272 RVA: 0x00008100 File Offset: 0x00006300
		internal static void ThrowArgumentNullException(ExceptionArgument argument)
		{
			throw ThrowHelper.CreateArgumentNullException(argument);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00008108 File Offset: 0x00006308
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentNullException(ExceptionArgument argument)
		{
			return new ArgumentNullException(argument.ToString());
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000811C File Offset: 0x0000631C
		internal static void ThrowArrayTypeMismatchException()
		{
			throw ThrowHelper.CreateArrayTypeMismatchException();
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00008124 File Offset: 0x00006324
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArrayTypeMismatchException()
		{
			return new ArrayTypeMismatchException();
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000812C File Offset: 0x0000632C
		internal static void ThrowArgumentException_InvalidTypeWithPointersNotSupported(Type type)
		{
			throw ThrowHelper.CreateArgumentException_InvalidTypeWithPointersNotSupported(type);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00008134 File Offset: 0x00006334
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_InvalidTypeWithPointersNotSupported(Type type)
		{
			return new ArgumentException(SR.Format(SR.Argument_InvalidTypeWithPointersNotSupported, type));
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00008148 File Offset: 0x00006348
		internal static void ThrowArgumentException_DestinationTooShort()
		{
			throw ThrowHelper.CreateArgumentException_DestinationTooShort();
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00008150 File Offset: 0x00006350
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_DestinationTooShort()
		{
			return new ArgumentException(SR.Argument_DestinationTooShort);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000815C File Offset: 0x0000635C
		internal static void ThrowIndexOutOfRangeException()
		{
			throw ThrowHelper.CreateIndexOutOfRangeException();
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00008164 File Offset: 0x00006364
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateIndexOutOfRangeException()
		{
			return new IndexOutOfRangeException();
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000816C File Offset: 0x0000636C
		internal static void ThrowArgumentOutOfRangeException()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00008174 File Offset: 0x00006374
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException()
		{
			return new ArgumentOutOfRangeException();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000817C File Offset: 0x0000637C
		internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException(argument);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00008184 File Offset: 0x00006384
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException(ExceptionArgument argument)
		{
			return new ArgumentOutOfRangeException(argument.ToString());
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00008198 File Offset: 0x00006398
		internal static void ThrowArgumentOutOfRangeException_PrecisionTooLarge()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_PrecisionTooLarge();
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000081A0 File Offset: 0x000063A0
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_PrecisionTooLarge()
		{
			return new ArgumentOutOfRangeException("precision", SR.Format(SR.Argument_PrecisionTooLarge, 99));
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000081C0 File Offset: 0x000063C0
		internal static void ThrowArgumentOutOfRangeException_SymbolDoesNotFit()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_SymbolDoesNotFit();
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000081C8 File Offset: 0x000063C8
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_SymbolDoesNotFit()
		{
			return new ArgumentOutOfRangeException("symbol", SR.Argument_BadFormatSpecifier);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x000081DC File Offset: 0x000063DC
		internal static void ThrowInvalidOperationException()
		{
			throw ThrowHelper.CreateInvalidOperationException();
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000081E4 File Offset: 0x000063E4
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException()
		{
			return new InvalidOperationException();
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000081EC File Offset: 0x000063EC
		internal static void ThrowInvalidOperationException_OutstandingReferences()
		{
			throw ThrowHelper.CreateInvalidOperationException_OutstandingReferences();
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000081F4 File Offset: 0x000063F4
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_OutstandingReferences()
		{
			return new InvalidOperationException(SR.OutstandingReferences);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00008200 File Offset: 0x00006400
		internal static void ThrowInvalidOperationException_UnexpectedSegmentType()
		{
			throw ThrowHelper.CreateInvalidOperationException_UnexpectedSegmentType();
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00008208 File Offset: 0x00006408
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_UnexpectedSegmentType()
		{
			return new InvalidOperationException(SR.UnexpectedSegmentType);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00008214 File Offset: 0x00006414
		internal static void ThrowInvalidOperationException_EndPositionNotReached()
		{
			throw ThrowHelper.CreateInvalidOperationException_EndPositionNotReached();
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000821C File Offset: 0x0000641C
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_EndPositionNotReached()
		{
			return new InvalidOperationException(SR.EndPositionNotReached);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00008228 File Offset: 0x00006428
		internal static void ThrowArgumentOutOfRangeException_PositionOutOfRange()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_PositionOutOfRange();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00008230 File Offset: 0x00006430
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_PositionOutOfRange()
		{
			return new ArgumentOutOfRangeException("position");
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000823C File Offset: 0x0000643C
		internal static void ThrowArgumentOutOfRangeException_OffsetOutOfRange()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_OffsetOutOfRange();
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00008244 File Offset: 0x00006444
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_OffsetOutOfRange()
		{
			return new ArgumentOutOfRangeException("offset");
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00008250 File Offset: 0x00006450
		internal static void ThrowObjectDisposedException_ArrayMemoryPoolBuffer()
		{
			throw ThrowHelper.CreateObjectDisposedException_ArrayMemoryPoolBuffer();
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00008258 File Offset: 0x00006458
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateObjectDisposedException_ArrayMemoryPoolBuffer()
		{
			return new ObjectDisposedException("ArrayMemoryPoolBuffer");
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00008264 File Offset: 0x00006464
		internal static void ThrowFormatException_BadFormatSpecifier()
		{
			throw ThrowHelper.CreateFormatException_BadFormatSpecifier();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000826C File Offset: 0x0000646C
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateFormatException_BadFormatSpecifier()
		{
			return new FormatException(SR.Argument_BadFormatSpecifier);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00008278 File Offset: 0x00006478
		internal static void ThrowArgumentException_OverlapAlignmentMismatch()
		{
			throw ThrowHelper.CreateArgumentException_OverlapAlignmentMismatch();
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00008280 File Offset: 0x00006480
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_OverlapAlignmentMismatch()
		{
			return new ArgumentException(SR.Argument_OverlapAlignmentMismatch);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000828C File Offset: 0x0000648C
		internal static void ThrowNotSupportedException()
		{
			throw ThrowHelper.CreateThrowNotSupportedException();
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00008294 File Offset: 0x00006494
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateThrowNotSupportedException()
		{
			return new NotSupportedException();
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000829C File Offset: 0x0000649C
		public static bool TryFormatThrowFormatException(out int bytesWritten)
		{
			bytesWritten = 0;
			ThrowHelper.ThrowFormatException_BadFormatSpecifier();
			return false;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000082A8 File Offset: 0x000064A8
		public static bool TryParseThrowFormatException<T>(out T value, out int bytesConsumed)
		{
			value = default(T);
			bytesConsumed = 0;
			ThrowHelper.ThrowFormatException_BadFormatSpecifier();
			return false;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000082BC File Offset: 0x000064BC
		public static void ThrowArgumentValidationException<T>(ReadOnlySequenceSegment<T> startSegment, int startIndex, ReadOnlySequenceSegment<T> endSegment)
		{
			throw ThrowHelper.CreateArgumentValidationException<T>(startSegment, startIndex, endSegment);
		}

		// Token: 0x06000139 RID: 313 RVA: 0x000082C8 File Offset: 0x000064C8
		private static Exception CreateArgumentValidationException<T>(ReadOnlySequenceSegment<T> startSegment, int startIndex, ReadOnlySequenceSegment<T> endSegment)
		{
			if (startSegment == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.startSegment);
			}
			if (endSegment == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.endSegment);
			}
			if (startSegment != endSegment && startSegment.RunningIndex > endSegment.RunningIndex)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endSegment);
			}
			if (startSegment.Memory.Length < startIndex)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.startIndex);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endIndex);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008338 File Offset: 0x00006538
		public static void ThrowArgumentValidationException(Array array, int start)
		{
			throw ThrowHelper.CreateArgumentValidationException(array, start);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008344 File Offset: 0x00006544
		private static Exception CreateArgumentValidationException(Array array, int start)
		{
			if (array == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.array);
			}
			if (start > array.Length)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00008370 File Offset: 0x00006570
		public static void ThrowStartOrEndArgumentValidationException(long start)
		{
			throw ThrowHelper.CreateStartOrEndArgumentValidationException(start);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00008378 File Offset: 0x00006578
		private static Exception CreateStartOrEndArgumentValidationException(long start)
		{
			if (start < 0L)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
		}
	}
}
