using System;

namespace System
{
	// Token: 0x02000033 RID: 51
	internal static class NotImplemented
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00010688 File Offset: 0x0000E888
		internal static Exception ByDesign
		{
			get
			{
				return new NotImplementedException();
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00010690 File Offset: 0x0000E890
		internal static Exception ByDesignWithMessage(string message)
		{
			return new NotImplementedException(message);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00010698 File Offset: 0x0000E898
		internal static Exception ActiveIssue(string issue)
		{
			return new NotImplementedException();
		}
	}
}
