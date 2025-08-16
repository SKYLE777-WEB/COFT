using System;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x02000032 RID: 50
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class Pinnable<T>
	{
		// Token: 0x04000119 RID: 281
		public T Data;
	}
}
