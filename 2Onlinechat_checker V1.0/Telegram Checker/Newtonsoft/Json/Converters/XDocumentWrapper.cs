using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000180 RID: 384
	internal class XDocumentWrapper : XContainerWrapper, IXmlDocument, IXmlNode
	{
		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06001401 RID: 5121 RVA: 0x0006E954 File Offset: 0x0006CB54
		private XDocument Document
		{
			get
			{
				return (XDocument)base.WrappedNode;
			}
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x0006E964 File Offset: 0x0006CB64
		public XDocumentWrapper(XDocument document)
			: base(document)
		{
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x0006E970 File Offset: 0x0006CB70
		public override List<IXmlNode> ChildNodes
		{
			get
			{
				List<IXmlNode> childNodes = base.ChildNodes;
				if (this.Document.Declaration != null && (childNodes.Count == 0 || childNodes[0].NodeType != XmlNodeType.XmlDeclaration))
				{
					childNodes.Insert(0, new XDeclarationWrapper(this.Document.Declaration));
				}
				return childNodes;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06001404 RID: 5124 RVA: 0x0006E9D0 File Offset: 0x0006CBD0
		protected override bool HasChildNodes
		{
			get
			{
				return base.HasChildNodes || this.Document.Declaration != null;
			}
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0006E9F0 File Offset: 0x0006CBF0
		public IXmlNode CreateComment(string text)
		{
			return new XObjectWrapper(new XComment(text));
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0006EA00 File Offset: 0x0006CC00
		public IXmlNode CreateTextNode(string text)
		{
			return new XObjectWrapper(new XText(text));
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0006EA10 File Offset: 0x0006CC10
		public IXmlNode CreateCDataSection(string data)
		{
			return new XObjectWrapper(new XCData(data));
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0006EA20 File Offset: 0x0006CC20
		public IXmlNode CreateWhitespace(string text)
		{
			return new XObjectWrapper(new XText(text));
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0006EA30 File Offset: 0x0006CC30
		public IXmlNode CreateSignificantWhitespace(string text)
		{
			return new XObjectWrapper(new XText(text));
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0006EA40 File Offset: 0x0006CC40
		public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new XDeclarationWrapper(new XDeclaration(version, encoding, standalone));
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0006EA50 File Offset: 0x0006CC50
		public IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XDocumentTypeWrapper(new XDocumentType(name, publicId, systemId, internalSubset));
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0006EA64 File Offset: 0x0006CC64
		public IXmlNode CreateProcessingInstruction(string target, string data)
		{
			return new XProcessingInstructionWrapper(new XProcessingInstruction(target, data));
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0006EA74 File Offset: 0x0006CC74
		public IXmlElement CreateElement(string elementName)
		{
			return new XElementWrapper(new XElement(elementName));
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0006EA88 File Offset: 0x0006CC88
		public IXmlElement CreateElement(string qualifiedName, string namespaceUri)
		{
			return new XElementWrapper(new XElement(XName.Get(MiscellaneousUtils.GetLocalName(qualifiedName), namespaceUri)));
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0006EAA0 File Offset: 0x0006CCA0
		public IXmlNode CreateAttribute(string name, string value)
		{
			return new XAttributeWrapper(new XAttribute(name, value));
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0006EAB4 File Offset: 0x0006CCB4
		public IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value)
		{
			return new XAttributeWrapper(new XAttribute(XName.Get(MiscellaneousUtils.GetLocalName(qualifiedName), namespaceUri), value));
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06001411 RID: 5137 RVA: 0x0006EAD0 File Offset: 0x0006CCD0
		public IXmlElement DocumentElement
		{
			get
			{
				if (this.Document.Root == null)
				{
					return null;
				}
				return new XElementWrapper(this.Document.Root);
			}
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x0006EAF4 File Offset: 0x0006CCF4
		public override IXmlNode AppendChild(IXmlNode newChild)
		{
			XDeclarationWrapper xdeclarationWrapper = newChild as XDeclarationWrapper;
			if (xdeclarationWrapper != null)
			{
				this.Document.Declaration = xdeclarationWrapper.Declaration;
				return xdeclarationWrapper;
			}
			return base.AppendChild(newChild);
		}
	}
}
