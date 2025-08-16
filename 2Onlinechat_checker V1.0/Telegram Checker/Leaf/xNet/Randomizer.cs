using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Leaf.xNet
{
	// Token: 0x02000055 RID: 85
	[ComVisible(true)]
	public static class Randomizer
	{
		// Token: 0x060003D9 RID: 985 RVA: 0x0001A82C File Offset: 0x00018A2C
		private static Random Generate()
		{
			byte[] array = new byte[4];
			Randomizer.Generator.GetBytes(array);
			return new Random(BitConverter.ToInt32(array, 0));
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060003DA RID: 986 RVA: 0x0001A85C File Offset: 0x00018A5C
		public static Random Instance
		{
			get
			{
				Random random;
				if ((random = Randomizer._rand) == null)
				{
					random = (Randomizer._rand = Randomizer.Generate());
				}
				return random;
			}
		}

		// Token: 0x0400018E RID: 398
		private static readonly RNGCryptoServiceProvider Generator = new RNGCryptoServiceProvider();

		// Token: 0x0400018F RID: 399
		[ThreadStatic]
		private static Random _rand;
	}
}
