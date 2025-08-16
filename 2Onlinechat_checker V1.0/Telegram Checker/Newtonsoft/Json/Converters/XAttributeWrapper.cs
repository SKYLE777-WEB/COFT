using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000186 RID: 390
	internal class XAttributeWrapper : XObjectWrapper
	{
		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06001434 RID: 5172 RVA: 0x0006EE5C File Offset: 0x0006D05C
		private XAttribute Attribute
		{
			get
			{
				return (XAttribute)base.WrappedNode;
			}
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0006EE6C File Offset: 0x0006D06C
		public XAttributeWrapper(XAttribute attribute)
			: base(attribute)
		{
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06001436 RID: 5174 RVA: 0x0006EE78 File Offset: 0x0006D078
		// (set) Token: 0x06001437 RID: 5175 RVA: 0x0006EE88 File Offset: 0x0006D088
		public override string Value
		{
			get
			{
				return this.Attribute.Value;
			}
			set
			{
				this.Attribute.Value = value;
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06001438 RID: 5176 RVA: 0x0006EE98 File Offset: 0x0006D098
		public override string LocalName
		{
			get
			{
				return this.Attribute.Name.LocalName;
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06001439 RID: 5177 RVA: 0x0006EEAC File Offset: 0x0006D0AC
		public override string NamespaceUri
		{
			get
			{
				return this.Attribute.Name.NamespaceName;
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x0600143A RID: 5178 RVA: 0x0006EEC0 File Offset: 0x0006D0C0
		public override IXmlNode ParentNode
		{
			get
			{
				if (this.Attribute.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(this.Attribute.Parent);
			}
		}
	}
}
