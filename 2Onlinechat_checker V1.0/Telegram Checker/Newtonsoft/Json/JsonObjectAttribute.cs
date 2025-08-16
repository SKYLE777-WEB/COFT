using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000AB RID: 171
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
	public sealed class JsonObjectAttribute : JsonContainerAttribute
	{
		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060007EF RID: 2031 RVA: 0x00039A9C File Offset: 0x00037C9C
		// (set) Token: 0x060007F0 RID: 2032 RVA: 0x00039AA4 File Offset: 0x00037CA4
		public MemberSerialization MemberSerialization
		{
			get
			{
				return this._memberSerialization;
			}
			set
			{
				this._memberSerialization = value;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060007F1 RID: 2033 RVA: 0x00039AB0 File Offset: 0x00037CB0
		// (set) Token: 0x060007F2 RID: 2034 RVA: 0x00039AE0 File Offset: 0x00037CE0
		public Required ItemRequired
		{
			get
			{
				Required? itemRequired = this._itemRequired;
				if (itemRequired == null)
				{
					return Required.Default;
				}
				return itemRequired.GetValueOrDefault();
			}
			set
			{
				this._itemRequired = new Required?(value);
			}
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00039AF0 File Offset: 0x00037CF0
		public JsonObjectAttribute()
		{
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x00039AF8 File Offset: 0x00037CF8
		public JsonObjectAttribute(MemberSerialization memberSerialization)
		{
			this.MemberSerialization = memberSerialization;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x00039B08 File Offset: 0x00037D08
		public JsonObjectAttribute(string id)
			: base(id)
		{
		}

		// Token: 0x04000373 RID: 883
		private MemberSerialization _memberSerialization;

		// Token: 0x04000374 RID: 884
		internal Required? _itemRequired;
	}
}
