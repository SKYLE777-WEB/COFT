using System;
using System.Xml;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200017E RID: 382
	internal class XDeclarationWrapper : XObjectWrapper, IXmlDeclaration, IXmlNode
	{
		// Token: 0x17000390 RID: 912
		// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0006E890 File Offset: 0x0006CA90
		internal XDeclaration Declaration { get; }

		// Token: 0x060013F4 RID: 5108 RVA: 0x0006E898 File Offset: 0x0006CA98
		public XDeclarationWrapper(XDeclaration declaration)
			: base(null)
		{
			this.Declaration = declaration;
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x060013F5 RID: 5109 RVA: 0x0006E8A8 File Offset: 0x0006CAA8
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.XmlDeclaration;
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x0006E8AC File Offset: 0x0006CAAC
		public string Version
		{
			get
			{
				return this.Declaration.Version;
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x060013F7 RID: 5111 RVA: 0x0006E8BC File Offset: 0x0006CABC
		// (set) Token: 0x060013F8 RID: 5112 RVA: 0x0006E8CC File Offset: 0x0006CACC
		public string Encoding
		{
			get
			{
				return this.Declaration.Encoding;
			}
			set
			{
				this.Declaration.Encoding = value;
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x060013F9 RID: 5113 RVA: 0x0006E8DC File Offset: 0x0006CADC
		// (set) Token: 0x060013FA RID: 5114 RVA: 0x0006E8EC File Offset: 0x0006CAEC
		public string Standalone
		{
			get
			{
				return this.Declaration.Standalone;
			}
			set
			{
				this.Declaration.Standalone = value;
			}
		}
	}
}
