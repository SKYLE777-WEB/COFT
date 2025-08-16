using System;

namespace System.Numerics.Hashing
{
	// Token: 0x02000036 RID: 54
	internal static class HashHelpers
	{
		// Token: 0x06000267 RID: 615 RVA: 0x00010928 File Offset: 0x0000EB28
		public static int Combine(int h1, int h2)
		{
			uint num = (uint)((h1 << 5) | (int)((uint)h1 >> 27));
			return (int)((num + (uint)h1) ^ (uint)h2);
		}

		// Token: 0x04000123 RID: 291
		public static readonly int RandomSeed = Guid.NewGuid().GetHashCode();
	}
}
