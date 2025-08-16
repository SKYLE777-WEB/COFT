using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000DF RID: 223
	internal class EnumValue<T> where T : struct
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x0004E5FC File Offset: 0x0004C7FC
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x0004E604 File Offset: 0x0004C804
		public T Value
		{
			get
			{
				return this._value;
			}
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x0004E60C File Offset: 0x0004C80C
		public EnumValue(string name, T value)
		{
			this._name = name;
			this._value = value;
		}

		// Token: 0x040004EF RID: 1263
		private readonly string _name;

		// Token: 0x040004F0 RID: 1264
		private readonly T _value;
	}
}
