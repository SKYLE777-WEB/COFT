using System;

namespace Newtonsoft.Json
{
	// Token: 0x0200009E RID: 158
	public interface IArrayPool<T>
	{
		// Token: 0x0600076C RID: 1900
		T[] Rent(int minimumLength);

		// Token: 0x0600076D RID: 1901
		void Return(T[] array);
	}
}
