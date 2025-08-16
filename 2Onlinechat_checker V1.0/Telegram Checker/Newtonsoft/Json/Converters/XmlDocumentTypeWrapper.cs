using System;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000177 RID: 375
	internal class XmlDocumentTypeWrapper : XmlNodeWrapper, IXmlDocumentType, IXmlNode
	{
		// Token: 0x060013BC RID: 5052 RVA: 0x0006E51C File Offset: 0x0006C71C
		public XmlDocumentTypeWrapper(XmlDocumentType documentType)
			: base(documentType)
		{
			this._documentType = documentType;
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x060013BD RID: 5053 RVA: 0x0006E52C File Offset: 0x0006C72C
		public string Name
		{
			get
			{
				return this._documentType.Name;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x0006E53C File Offset: 0x0006C73C
		public string System
		{
			get
			{
				return this._documentType.SystemId;
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060013BF RID: 5055 RVA: 0x0006E54C File Offset: 0x0006C74C
		public string Public
		{
			get
			{
				return this._documentType.PublicId;
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x0006E55C File Offset: 0x0006C75C
		public string InternalSubset
		{
			get
			{
				return this._documentType.InternalSubset;
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060013C1 RID: 5057 RVA: 0x0006E56C File Offset: 0x0006C76C
		public override string LocalName
		{
			get
			{
				return "DOCTYPE";
			}
		}

		// Token: 0x04000708 RID: 1800
		private readonly XmlDocumentType _documentType;
	}
}
