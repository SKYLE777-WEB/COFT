using System;
using System.Collections.Generic;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000178 RID: 376
	internal class XmlNodeWrapper : IXmlNode
	{
		// Token: 0x060013C2 RID: 5058 RVA: 0x0006E574 File Offset: 0x0006C774
		public XmlNodeWrapper(XmlNode node)
		{
			this._node = node;
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060013C3 RID: 5059 RVA: 0x0006E584 File Offset: 0x0006C784
		public object WrappedNode
		{
			get
			{
				return this._node;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x060013C4 RID: 5060 RVA: 0x0006E58C File Offset: 0x0006C78C
		public XmlNodeType NodeType
		{
			get
			{
				return this._node.NodeType;
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x060013C5 RID: 5061 RVA: 0x0006E59C File Offset: 0x0006C79C
		public virtual string LocalName
		{
			get
			{
				return this._node.LocalName;
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x060013C6 RID: 5062 RVA: 0x0006E5AC File Offset: 0x0006C7AC
		public List<IXmlNode> ChildNodes
		{
			get
			{
				if (this._childNodes == null)
				{
					if (!this._node.HasChildNodes)
					{
						this._childNodes = XmlNodeConverter.EmptyChildNodes;
					}
					else
					{
						this._childNodes = new List<IXmlNode>(this._node.ChildNodes.Count);
						foreach (object obj in this._node.ChildNodes)
						{
							XmlNode xmlNode = (XmlNode)obj;
							this._childNodes.Add(XmlNodeWrapper.WrapNode(xmlNode));
						}
					}
				}
				return this._childNodes;
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x060013C7 RID: 5063 RVA: 0x0006E66C File Offset: 0x0006C86C
		protected virtual bool HasChildNodes
		{
			get
			{
				return this._node.HasChildNodes;
			}
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x0006E67C File Offset: 0x0006C87C
		internal static IXmlNode WrapNode(XmlNode node)
		{
			XmlNodeType nodeType = node.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				return new XmlElementWrapper((XmlElement)node);
			}
			if (nodeType == XmlNodeType.DocumentType)
			{
				return new XmlDocumentTypeWrapper((XmlDocumentType)node);
			}
			if (nodeType != XmlNodeType.XmlDeclaration)
			{
				return new XmlNodeWrapper(node);
			}
			return new XmlDeclarationWrapper((XmlDeclaration)node);
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x060013C9 RID: 5065 RVA: 0x0006E6DC File Offset: 0x0006C8DC
		public List<IXmlNode> Attributes
		{
			get
			{
				if (this._attributes == null)
				{
					if (!this.HasAttributes)
					{
						this._attributes = XmlNodeConverter.EmptyChildNodes;
					}
					else
					{
						this._attributes = new List<IXmlNode>(this._node.Attributes.Count);
						foreach (object obj in this._node.Attributes)
						{
							XmlAttribute xmlAttribute = (XmlAttribute)obj;
							this._attributes.Add(XmlNodeWrapper.WrapNode(xmlAttribute));
						}
					}
				}
				return this._attributes;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x060013CA RID: 5066 RVA: 0x0006E794 File Offset: 0x0006C994
		private bool HasAttributes
		{
			get
			{
				XmlElement xmlElement = this._node as XmlElement;
				if (xmlElement != null)
				{
					return xmlElement.HasAttributes;
				}
				XmlAttributeCollection attributes = this._node.Attributes;
				return attributes != null && attributes.Count > 0;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x060013CB RID: 5067 RVA: 0x0006E7DC File Offset: 0x0006C9DC
		public IXmlNode ParentNode
		{
			get
			{
				XmlAttribute xmlAttribute = this._node as XmlAttribute;
				XmlNode xmlNode = ((xmlAttribute != null) ? xmlAttribute.OwnerElement : this._node.ParentNode);
				if (xmlNode == null)
				{
					return null;
				}
				return XmlNodeWrapper.WrapNode(xmlNode);
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x060013CC RID: 5068 RVA: 0x0006E824 File Offset: 0x0006CA24
		// (set) Token: 0x060013CD RID: 5069 RVA: 0x0006E834 File Offset: 0x0006CA34
		public string Value
		{
			get
			{
				return this._node.Value;
			}
			set
			{
				this._node.Value = value;
			}
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x0006E844 File Offset: 0x0006CA44
		public IXmlNode AppendChild(IXmlNode newChild)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
			this._node.AppendChild(xmlNodeWrapper._node);
			this._childNodes = null;
			this._attributes = null;
			return newChild;
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x060013CF RID: 5071 RVA: 0x0006E880 File Offset: 0x0006CA80
		public string NamespaceUri
		{
			get
			{
				return this._node.NamespaceURI;
			}
		}

		// Token: 0x04000709 RID: 1801
		private readonly XmlNode _node;

		// Token: 0x0400070A RID: 1802
		private List<IXmlNode> _childNodes;

		// Token: 0x0400070B RID: 1803
		private List<IXmlNode> _attributes;
	}
}
