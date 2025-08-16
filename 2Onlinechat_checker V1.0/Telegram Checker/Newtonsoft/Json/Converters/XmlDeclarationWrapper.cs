using System;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000176 RID: 374
	internal class XmlDeclarationWrapper : XmlNodeWrapper, IXmlDeclaration, IXmlNode
	{
		// Token: 0x060013B6 RID: 5046 RVA: 0x0006E4BC File Offset: 0x0006C6BC
		public XmlDeclarationWrapper(XmlDeclaration declaration)
			: base(declaration)
		{
			this._declaration = declaration;
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x060013B7 RID: 5047 RVA: 0x0006E4CC File Offset: 0x0006C6CC
		public string Version
		{
			get
			{
				return this._declaration.Version;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x060013B8 RID: 5048 RVA: 0x0006E4DC File Offset: 0x0006C6DC
		// (set) Token: 0x060013B9 RID: 5049 RVA: 0x0006E4EC File Offset: 0x0006C6EC
		public string Encoding
		{
			get
			{
				return this._declaration.Encoding;
			}
			set
			{
				this._declaration.Encoding = value;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x060013BA RID: 5050 RVA: 0x0006E4FC File Offset: 0x0006C6FC
		// (set) Token: 0x060013BB RID: 5051 RVA: 0x0006E50C File Offset: 0x0006C70C
		public string Standalone
		{
			get
			{
				return this._declaration.Standalone;
			}
			set
			{
				this._declaration.Standalone = value;
			}
		}

		// Token: 0x04000707 RID: 1799
		private readonly XmlDeclaration _declaration;
	}
}
