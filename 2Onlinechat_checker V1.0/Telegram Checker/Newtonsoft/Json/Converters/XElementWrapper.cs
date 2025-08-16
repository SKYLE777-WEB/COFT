using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000187 RID: 391
	internal class XElementWrapper : XContainerWrapper, IXmlElement, IXmlNode
	{
		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x0600143B RID: 5179 RVA: 0x0006EEE4 File Offset: 0x0006D0E4
		private XElement Element
		{
			get
			{
				return (XElement)base.WrappedNode;
			}
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x0006EEF4 File Offset: 0x0006D0F4
		public XElementWrapper(XElement element)
			: base(element)
		{
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x0006EF00 File Offset: 0x0006D100
		public void SetAttributeNode(IXmlNode attribute)
		{
			XObjectWrapper xobjectWrapper = (XObjectWrapper)attribute;
			this.Element.Add(xobjectWrapper.WrappedNode);
			this._attributes = null;
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x0600143E RID: 5182 RVA: 0x0006EF30 File Offset: 0x0006D130
		public override List<IXmlNode> Attributes
		{
			get
			{
				if (this._attributes == null)
				{
					if (!this.Element.HasAttributes && !this.HasImplicitNamespaceAttribute(this.NamespaceUri))
					{
						this._attributes = XmlNodeConverter.EmptyChildNodes;
					}
					else
					{
						this._attributes = new List<IXmlNode>();
						foreach (XAttribute xattribute in this.Element.Attributes())
						{
							this._attributes.Add(new XAttributeWrapper(xattribute));
						}
						string namespaceUri = this.NamespaceUri;
						if (this.HasImplicitNamespaceAttribute(namespaceUri))
						{
							this._attributes.Insert(0, new XAttributeWrapper(new XAttribute("xmlns", namespaceUri)));
						}
					}
				}
				return this._attributes;
			}
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0006F018 File Offset: 0x0006D218
		private bool HasImplicitNamespaceAttribute(string namespaceUri)
		{
			if (!string.IsNullOrEmpty(namespaceUri))
			{
				IXmlNode parentNode = this.ParentNode;
				if (namespaceUri != ((parentNode != null) ? parentNode.NamespaceUri : null) && string.IsNullOrEmpty(this.GetPrefixOfNamespace(namespaceUri)))
				{
					bool flag = false;
					if (this.Element.HasAttributes)
					{
						foreach (XAttribute xattribute in this.Element.Attributes())
						{
							if (xattribute.Name.LocalName == "xmlns" && string.IsNullOrEmpty(xattribute.Name.NamespaceName) && xattribute.Value == namespaceUri)
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x0006F108 File Offset: 0x0006D308
		public override IXmlNode AppendChild(IXmlNode newChild)
		{
			IXmlNode xmlNode = base.AppendChild(newChild);
			this._attributes = null;
			return xmlNode;
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06001441 RID: 5185 RVA: 0x0006F118 File Offset: 0x0006D318
		// (set) Token: 0x06001442 RID: 5186 RVA: 0x0006F128 File Offset: 0x0006D328
		public override string Value
		{
			get
			{
				return this.Element.Value;
			}
			set
			{
				this.Element.Value = value;
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06001443 RID: 5187 RVA: 0x0006F138 File Offset: 0x0006D338
		public override string LocalName
		{
			get
			{
				return this.Element.Name.LocalName;
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x0006F14C File Offset: 0x0006D34C
		public override string NamespaceUri
		{
			get
			{
				return this.Element.Name.NamespaceName;
			}
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x0006F160 File Offset: 0x0006D360
		public string GetPrefixOfNamespace(string namespaceUri)
		{
			return this.Element.GetPrefixOfNamespace(namespaceUri);
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06001446 RID: 5190 RVA: 0x0006F174 File Offset: 0x0006D374
		public bool IsEmpty
		{
			get
			{
				return this.Element.IsEmpty;
			}
		}

		// Token: 0x04000710 RID: 1808
		private List<IXmlNode> _attributes;
	}
}
