using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F9 RID: 249
	internal static class ValidationUtils
	{
		// Token: 0x06000D40 RID: 3392 RVA: 0x00052ABC File Offset: 0x00050CBC
		public static void ArgumentNotNull(object value, string parameterName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}
	}
}
