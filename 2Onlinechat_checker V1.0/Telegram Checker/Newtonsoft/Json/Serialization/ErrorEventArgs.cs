using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000105 RID: 261
	public class ErrorEventArgs : EventArgs
	{
		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000DAA RID: 3498 RVA: 0x0005544C File Offset: 0x0005364C
		public object CurrentObject { get; }

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000DAB RID: 3499 RVA: 0x00055454 File Offset: 0x00053654
		public ErrorContext ErrorContext { get; }

		// Token: 0x06000DAC RID: 3500 RVA: 0x0005545C File Offset: 0x0005365C
		public ErrorEventArgs(object currentObject, ErrorContext errorContext)
		{
			this.CurrentObject = currentObject;
			this.ErrorContext = errorContext;
		}
	}
}
