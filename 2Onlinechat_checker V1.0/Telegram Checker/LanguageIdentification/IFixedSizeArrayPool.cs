using System;
using System.Runtime.CompilerServices;

namespace LanguageIdentification
{
	// Token: 0x02000012 RID: 18
	[NullableContext(1)]
	internal interface IFixedSizeArrayPool<[Nullable(2)] T>
	{
		// Token: 0x060000A0 RID: 160
		T[] Rent();

		// Token: 0x060000A1 RID: 161
		void Return(T[] item);
	}
}
