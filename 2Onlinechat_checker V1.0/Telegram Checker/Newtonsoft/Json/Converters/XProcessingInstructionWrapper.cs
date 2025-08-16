using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000183 RID: 387
	internal class XProcessingInstructionWrapper : XObjectWrapper
	{
		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x0600141D RID: 5149 RVA: 0x0006EBEC File Offset: 0x0006CDEC
		private XProcessingInstruction ProcessingInstruction
		{
			get
			{
				return (XProcessingInstruction)base.WrappedNode;
			}
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x0006EBFC File Offset: 0x0006CDFC
		public XProcessingInstructionWrapper(XProcessingInstruction processingInstruction)
			: base(processingInstruction)
		{
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x0600141F RID: 5151 RVA: 0x0006EC08 File Offset: 0x0006CE08
		public override string LocalName
		{
			get
			{
				return this.ProcessingInstruction.Target;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06001420 RID: 5152 RVA: 0x0006EC18 File Offset: 0x0006CE18
		// (set) Token: 0x06001421 RID: 5153 RVA: 0x0006EC28 File Offset: 0x0006CE28
		public override string Value
		{
			get
			{
				return this.ProcessingInstruction.Data;
			}
			set
			{
				this.ProcessingInstruction.Data = value;
			}
		}
	}
}
