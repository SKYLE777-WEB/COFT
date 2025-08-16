using System;

namespace System.Numerics.Hashing
{
	// Token: 0x02000095 RID: 149
	internal static class System.Numerics.Vectors159800.HashHelpers
	{
		// Token: 0x0600076A RID: 1898 RVA: 0x00038CB0 File Offset: 0x00036EB0
		public static int Combine(int h1, int h2)
		{
			uint num = (uint)((h1 << 5) | (int)((uint)h1 >> 27));
			return (int)((num + (uint)h1) ^ (uint)h2);
		}

		// Token: 0x0400033B RID: 827
		public static readonly int RandomSeed = Guid.NewGuid().GetHashCode();
	}
}
