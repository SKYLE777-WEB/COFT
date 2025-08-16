using System;
using System.Collections.Generic;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200017D RID: 381
	internal interface IXmlNode
	{
		// Token: 0x17000388 RID: 904
		// (get) Token: 0x060013E9 RID: 5097
		XmlNodeType NodeType { get; }

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x060013EA RID: 5098
		string LocalName { get; }

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x060013EB RID: 5099
		List<IXmlNode> ChildNodes { get; }

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x060013EC RID: 5100
		List<IXmlNode> Attributes { get; }

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x060013ED RID: 5101
		IXmlNode ParentNode { get; }

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x060013EE RID: 5102
		// (set) Token: 0x060013EF RID: 5103
		string Value { get; set; }

		// Token: 0x060013F0 RID: 5104
		IXmlNode AppendChild(IXmlNode newChild);

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x060013F1 RID: 5105
		string NamespaceUri { get; }

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x060013F2 RID: 5106
		object WrappedNode { get; }
	}
}
