using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000182 RID: 386
	internal class XCommentWrapper : XObjectWrapper
	{
		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06001418 RID: 5144 RVA: 0x0006EB8C File Offset: 0x0006CD8C
		private XComment Text
		{
			get
			{
				return (XComment)base.WrappedNode;
			}
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x0006EB9C File Offset: 0x0006CD9C
		public XCommentWrapper(XComment text)
			: base(text)
		{
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x0600141A RID: 5146 RVA: 0x0006EBA8 File Offset: 0x0006CDA8
		// (set) Token: 0x0600141B RID: 5147 RVA: 0x0006EBB8 File Offset: 0x0006CDB8
		public override string Value
		{
			get
			{
				return this.Text.Value;
			}
			set
			{
				this.Text.Value = value;
			}
		}

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x0600141C RID: 5148 RVA: 0x0006EBC8 File Offset: 0x0006CDC8
		public override IXmlNode ParentNode
		{
			get
			{
				if (this.Text.Parent == null)
				{
					return null;
				}
				return XContainerWrapper.WrapNode(this.Text.Parent);
			}
		}
	}
}
