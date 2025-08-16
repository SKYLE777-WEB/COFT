using System;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000179 RID: 377
	internal interface IXmlDocument : IXmlNode
	{
		// Token: 0x060013D0 RID: 5072
		IXmlNode CreateComment(string text);

		// Token: 0x060013D1 RID: 5073
		IXmlNode CreateTextNode(string text);

		// Token: 0x060013D2 RID: 5074
		IXmlNode CreateCDataSection(string data);

		// Token: 0x060013D3 RID: 5075
		IXmlNode CreateWhitespace(string text);

		// Token: 0x060013D4 RID: 5076
		IXmlNode CreateSignificantWhitespace(string text);

		// Token: 0x060013D5 RID: 5077
		IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone);

		// Token: 0x060013D6 RID: 5078
		IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset);

		// Token: 0x060013D7 RID: 5079
		IXmlNode CreateProcessingInstruction(string target, string data);

		// Token: 0x060013D8 RID: 5080
		IXmlElement CreateElement(string elementName);

		// Token: 0x060013D9 RID: 5081
		IXmlElement CreateElement(string qualifiedName, string namespaceUri);

		// Token: 0x060013DA RID: 5082
		IXmlNode CreateAttribute(string name, string value);

		// Token: 0x060013DB RID: 5083
		IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value);

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x060013DC RID: 5084
		IXmlElement DocumentElement { get; }
	}
}
