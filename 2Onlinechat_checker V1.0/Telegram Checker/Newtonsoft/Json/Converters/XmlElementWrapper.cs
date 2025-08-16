using System;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000175 RID: 373
	internal class XmlElementWrapper : XmlNodeWrapper, IXmlElement, IXmlNode
	{
		// Token: 0x060013B2 RID: 5042 RVA: 0x0006E45C File Offset: 0x0006C65C
		public XmlElementWrapper(XmlElement element)
			: base(element)
		{
			this._element = element;
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x0006E46C File Offset: 0x0006C66C
		public void SetAttributeNode(IXmlNode attribute)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)attribute;
			this._element.SetAttributeNode((XmlAttribute)xmlNodeWrapper.WrappedNode);
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x0006E49C File Offset: 0x0006C69C
		public string GetPrefixOfNamespace(string namespaceUri)
		{
			return this._element.GetPrefixOfNamespace(namespaceUri);
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x060013B5 RID: 5045 RVA: 0x0006E4AC File Offset: 0x0006C6AC
		public bool IsEmpty
		{
			get
			{
				return this._element.IsEmpty;
			}
		}

		// Token: 0x04000706 RID: 1798
		private readonly XmlElement _element;
	}
}
