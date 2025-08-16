using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000E5 RID: 229
	internal static class BufferUtils
	{
		// Token: 0x06000C83 RID: 3203 RVA: 0x0004F674 File Offset: 0x0004D874
		public static char[] RentBuffer(IArrayPool<char> bufferPool, int minSize)
		{
			if (bufferPool == null)
			{
				return new char[minSize];
			}
			return bufferPool.Rent(minSize);
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x0004F68C File Offset: 0x0004D88C
		public static void ReturnBuffer(IArrayPool<char> bufferPool, char[] buffer)
		{
			if (bufferPool != null)
			{
				bufferPool.Return(buffer);
			}
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0004F69C File Offset: 0x0004D89C
		public static char[] EnsureBufferSize(IArrayPool<char> bufferPool, int size, char[] buffer)
		{
			if (bufferPool == null)
			{
				return new char[size];
			}
			if (buffer != null)
			{
				bufferPool.Return(buffer);
			}
			return bufferPool.Rent(size);
		}
	}
}
