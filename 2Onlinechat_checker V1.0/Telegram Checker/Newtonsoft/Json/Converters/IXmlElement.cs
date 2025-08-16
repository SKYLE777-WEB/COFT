using System;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200017C RID: 380
	internal interface IXmlElement : IXmlNode
	{
		// Token: 0x060013E6 RID: 5094
		void SetAttributeNode(IXmlNode attribute);

		// Token: 0x060013E7 RID: 5095
		string GetPrefixOfNamespace(string namespaceUri);

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x060013E8 RID: 5096
		bool IsEmpty { get; }
	}
}
