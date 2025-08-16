using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000185 RID: 389
	internal class XObjectWrapper : IXmlNode
	{
		// Token: 0x06001429 RID: 5161 RVA: 0x0006EE04 File Offset: 0x0006D004
		public XObjectWrapper(XObject xmlObject)
		{
			this._xmlObject = xmlObject;
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x0006EE14 File Offset: 0x0006D014
		public object WrappedNode
		{
			get
			{
				return this._xmlObject;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x0600142B RID: 5163 RVA: 0x0006EE1C File Offset: 0x0006D01C
		public virtual XmlNodeType NodeType
		{
			get
			{
				return this._xmlObject.NodeType;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x0600142C RID: 5164 RVA: 0x0006EE2C File Offset: 0x0006D02C
		public virtual string LocalName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x0600142D RID: 5165 RVA: 0x0006EE30 File Offset: 0x0006D030
		public virtual List<IXmlNode> ChildNodes
		{
			get
			{
				return XmlNodeConverter.EmptyChildNodes;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x0600142E RID: 5166 RVA: 0x0006EE38 File Offset: 0x0006D038
		public virtual List<IXmlNode> Attributes
		{
			get
			{
				return XmlNodeConverter.EmptyChildNodes;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x0600142F RID: 5167 RVA: 0x0006EE40 File Offset: 0x0006D040
		public virtual IXmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06001430 RID: 5168 RVA: 0x0006EE44 File Offset: 0x0006D044
		// (set) Token: 0x06001431 RID: 5169 RVA: 0x0006EE48 File Offset: 0x0006D048
		public virtual string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x0006EE50 File Offset: 0x0006D050
		public virtual IXmlNode AppendChild(IXmlNode newChild)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06001433 RID: 5171 RVA: 0x0006EE58 File Offset: 0x0006D058
		public virtual string NamespaceUri
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0400070F RID: 1807
		private readonly XObject _xmlObject;
	}
}
