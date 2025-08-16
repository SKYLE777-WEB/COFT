using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200017F RID: 383
	internal class XDocumentTypeWrapper : XObjectWrapper, IXmlDocumentType, IXmlNode
	{
		// Token: 0x060013FB RID: 5115 RVA: 0x0006E8FC File Offset: 0x0006CAFC
		public XDocumentTypeWrapper(XDocumentType documentType)
			: base(documentType)
		{
			this._documentType = documentType;
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x0006E90C File Offset: 0x0006CB0C
		public string Name
		{
			get
			{
				return this._documentType.Name;
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x0006E91C File Offset: 0x0006CB1C
		public string System
		{
			get
			{
				return this._documentType.SystemId;
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x060013FE RID: 5118 RVA: 0x0006E92C File Offset: 0x0006CB2C
		public string Public
		{
			get
			{
				return this._documentType.PublicId;
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x0006E93C File Offset: 0x0006CB3C
		public string InternalSubset
		{
			get
			{
				return this._documentType.InternalSubset;
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06001400 RID: 5120 RVA: 0x0006E94C File Offset: 0x0006CB4C
		public override string LocalName
		{
			get
			{
				return "DOCTYPE";
			}
		}

		// Token: 0x0400070D RID: 1805
		private readonly XDocumentType _documentType;
	}
}
