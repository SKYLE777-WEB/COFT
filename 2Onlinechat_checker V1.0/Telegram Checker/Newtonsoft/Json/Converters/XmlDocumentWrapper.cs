using System;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000174 RID: 372
	internal class XmlDocumentWrapper : XmlNodeWrapper, IXmlDocument, IXmlNode
	{
		// Token: 0x060013A4 RID: 5028 RVA: 0x0006E320 File Offset: 0x0006C520
		public XmlDocumentWrapper(XmlDocument document)
			: base(document)
		{
			this._document = document;
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x0006E330 File Offset: 0x0006C530
		public IXmlNode CreateComment(string data)
		{
			return new XmlNodeWrapper(this._document.CreateComment(data));
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x0006E344 File Offset: 0x0006C544
		public IXmlNode CreateTextNode(string text)
		{
			return new XmlNodeWrapper(this._document.CreateTextNode(text));
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x0006E358 File Offset: 0x0006C558
		public IXmlNode CreateCDataSection(string data)
		{
			return new XmlNodeWrapper(this._document.CreateCDataSection(data));
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x0006E36C File Offset: 0x0006C56C
		public IXmlNode CreateWhitespace(string text)
		{
			return new XmlNodeWrapper(this._document.CreateWhitespace(text));
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x0006E380 File Offset: 0x0006C580
		public IXmlNode CreateSignificantWhitespace(string text)
		{
			return new XmlNodeWrapper(this._document.CreateSignificantWhitespace(text));
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x0006E394 File Offset: 0x0006C594
		public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new XmlDeclarationWrapper(this._document.CreateXmlDeclaration(version, encoding, standalone));
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x0006E3AC File Offset: 0x0006C5AC
		public IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XmlDocumentTypeWrapper(this._document.CreateDocumentType(name, publicId, systemId, null));
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x0006E3C4 File Offset: 0x0006C5C4
		public IXmlNode CreateProcessingInstruction(string target, string data)
		{
			return new XmlNodeWrapper(this._document.CreateProcessingInstruction(target, data));
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x0006E3D8 File Offset: 0x0006C5D8
		public IXmlElement CreateElement(string elementName)
		{
			return new XmlElementWrapper(this._document.CreateElement(elementName));
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x0006E3EC File Offset: 0x0006C5EC
		public IXmlElement CreateElement(string qualifiedName, string namespaceUri)
		{
			return new XmlElementWrapper(this._document.CreateElement(qualifiedName, namespaceUri));
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x0006E400 File Offset: 0x0006C600
		public IXmlNode CreateAttribute(string name, string value)
		{
			return new XmlNodeWrapper(this._document.CreateAttribute(name))
			{
				Value = value
			};
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x0006E41C File Offset: 0x0006C61C
		public IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value)
		{
			return new XmlNodeWrapper(this._document.CreateAttribute(qualifiedName, namespaceUri))
			{
				Value = value
			};
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x060013B1 RID: 5041 RVA: 0x0006E438 File Offset: 0x0006C638
		public IXmlElement DocumentElement
		{
			get
			{
				if (this._document.DocumentElement == null)
				{
					return null;
				}
				return new XmlElementWrapper(this._document.DocumentElement);
			}
		}

		// Token: 0x04000705 RID: 1797
		private readonly XmlDocument _document;
	}
}
