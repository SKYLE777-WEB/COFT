using System;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200017A RID: 378
	internal interface IXmlDeclaration : IXmlNode
	{
		// Token: 0x17000380 RID: 896
		// (get) Token: 0x060013DD RID: 5085
		string Version { get; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x060013DE RID: 5086
		// (set) Token: 0x060013DF RID: 5087
		string Encoding { get; set; }

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x060013E0 RID: 5088
		// (set) Token: 0x060013E1 RID: 5089
		string Standalone { get; set; }
	}
}
