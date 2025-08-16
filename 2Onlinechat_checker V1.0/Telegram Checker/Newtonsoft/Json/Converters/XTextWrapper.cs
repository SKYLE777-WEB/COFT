using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000181 RID: 385
	internal class XTextWrapper : XObjectWrapper
	{
		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06001413 RID: 5139 RVA: 0x0006EB2C File Offset: 0x0006CD2C
		private XText Text
		{
			get
			{
				return (XText)base.WrappedNode;
			}
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x0006EB3C File Offset: 0x0006CD3C
		public XTextWrapper(XText text)
			: base(text)
		{
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06001415 RID: 5141 RVA: 0x0006EB48 File Offset: 0x0006CD48
		// (set) Token: 0x06001416 RID: 5142 RVA: 0x0006EB58 File Offset: 0x0006CD58
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

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06001417 RID: 5143 RVA: 0x0006EB68 File Offset: 0x0006CD68
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
