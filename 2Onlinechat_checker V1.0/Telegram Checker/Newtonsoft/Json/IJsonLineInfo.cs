using System;

namespace Newtonsoft.Json
{
	// Token: 0x0200009F RID: 159
	public interface IJsonLineInfo
	{
		// Token: 0x0600076E RID: 1902
		bool HasLineInfo();

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600076F RID: 1903
		int LineNumber { get; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000770 RID: 1904
		int LinePosition { get; }
	}
}
