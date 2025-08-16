using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000A0 RID: 160
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	public sealed class JsonArrayAttribute : JsonContainerAttribute
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x00038CFC File Offset: 0x00036EFC
		// (set) Token: 0x06000772 RID: 1906 RVA: 0x00038D04 File Offset: 0x00036F04
		public bool AllowNullItems
		{
			get
			{
				return this._allowNullItems;
			}
			set
			{
				this._allowNullItems = value;
			}
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00038D10 File Offset: 0x00036F10
		public JsonArrayAttribute()
		{
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00038D18 File Offset: 0x00036F18
		public JsonArrayAttribute(bool allowNullItems)
		{
			this._allowNullItems = allowNullItems;
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00038D28 File Offset: 0x00036F28
		public JsonArrayAttribute(string id)
			: base(id)
		{
		}

		// Token: 0x0400035A RID: 858
		private bool _allowNullItems;
	}
}
