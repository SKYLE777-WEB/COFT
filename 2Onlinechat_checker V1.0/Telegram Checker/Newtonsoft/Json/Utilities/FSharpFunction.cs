using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000E1 RID: 225
	internal class FSharpFunction
	{
		// Token: 0x06000C5D RID: 3165 RVA: 0x0004EDF0 File Offset: 0x0004CFF0
		public FSharpFunction(object instance, MethodCall<object, object> invoker)
		{
			this._instance = instance;
			this._invoker = invoker;
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0004EE08 File Offset: 0x0004D008
		public object Invoke(params object[] args)
		{
			return this._invoker(this._instance, args);
		}

		// Token: 0x040004F2 RID: 1266
		private readonly object _instance;

		// Token: 0x040004F3 RID: 1267
		private readonly MethodCall<object, object> _invoker;
	}
}
