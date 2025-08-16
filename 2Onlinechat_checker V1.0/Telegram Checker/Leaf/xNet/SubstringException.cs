using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x02000058 RID: 88
	[ComVisible(true)]
	public class SubstringException : Exception
	{
		// Token: 0x06000423 RID: 1059 RVA: 0x0001AF5C File Offset: 0x0001915C
		public SubstringException()
		{
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0001AF64 File Offset: 0x00019164
		public SubstringException(string message)
			: base(message)
		{
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x0001AF70 File Offset: 0x00019170
		public SubstringException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
