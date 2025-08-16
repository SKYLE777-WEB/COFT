using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000184 RID: 388
	internal class XContainerWrapper : XObjectWrapper
	{
		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06001422 RID: 5154 RVA: 0x0006EC38 File Offset: 0x0006CE38
		private XContainer Container
		{
			get
			{
				return (XContainer)base.WrappedNode;
			}
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x0006EC48 File Offset: 0x0006CE48
		public XContainerWrapper(XContainer container)
			: base(container)
		{
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06001424 RID: 5156 RVA: 0x0006EC54 File Offset: 0x0006CE54
		public override List<IXmlNode> ChildNodes
		{
			get
			{
				if (this._childNodes == null)
				{
					if (!this.HasChildNodes)
					{
						this._childNodes = XmlNodeConverter.EmptyChildNodes;
					}
					else
					{
						this._childNodes = new List<IXmlNode>();
						foreach (XNode xnode in this.Container.Nodes())
						{
							this._childNodes.Add(XContainerWrapper.WrapNode(xnode));
						}
					}
				}
				return this._childNodes;
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x0006ECF0 File Offset: 0x0006CEF0
		protected virtual bool HasChildNodes
		{
			get
			{
				return this.Container.LastNode != null;
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06001426 RID: 5158 RVA: 0x0006ED00 File Offset: 0x0006CF00
		public override IXmlNode ParentNode
		{
			get
			{
				if (this.Container.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(this.Container.Parent);
			}
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x0006ED24 File Offset: 0x0006CF24
		internal static IXmlNode WrapNode(XObject node)
		{
			XDocument xdocument = node as XDocument;
			if (xdocument != null)
			{
				return new XDocumentWrapper(xdocument);
			}
			XElement xelement = node as XElement;
			if (xelement != null)
			{
				return new XElementWrapper(xelement);
			}
			XContainer xcontainer = node as XContainer;
			if (xcontainer != null)
			{
				return new XContainerWrapper(xcontainer);
			}
			XProcessingInstruction xprocessingInstruction = node as XProcessingInstruction;
			if (xprocessingInstruction != null)
			{
				return new XProcessingInstructionWrapper(xprocessingInstruction);
			}
			XText xtext = node as XText;
			if (xtext != null)
			{
				return new XTextWrapper(xtext);
			}
			XComment xcomment = node as XComment;
			if (xcomment != null)
			{
				return new XCommentWrapper(xcomment);
			}
			XAttribute xattribute = node as XAttribute;
			if (xattribute != null)
			{
				return new XAttributeWrapper(xattribute);
			}
			XDocumentType xdocumentType = node as XDocumentType;
			if (xdocumentType != null)
			{
				return new XDocumentTypeWrapper(xdocumentType);
			}
			return new XObjectWrapper(node);
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0006EDE8 File Offset: 0x0006CFE8
		public override IXmlNode AppendChild(IXmlNode newChild)
		{
			this.Container.Add(newChild.WrappedNode);
			this._childNodes = null;
			return newChild;
		}

		// Token: 0x0400070E RID: 1806
		private List<IXmlNode> _childNodes;
	}
}
