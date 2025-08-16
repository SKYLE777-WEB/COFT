using System;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200017B RID: 379
	internal interface IXmlDocumentType : IXmlNode
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x060013E2 RID: 5090
		string Name { get; }

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x060013E3 RID: 5091
		string System { get; }

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x060013E4 RID: 5092
		string Public { get; }

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x060013E5 RID: 5093
		string InternalSubset { get; }
	}
}
