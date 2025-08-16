using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000191 RID: 401
	internal class BsonValue : BsonToken
	{
		// Token: 0x060014B0 RID: 5296 RVA: 0x00072504 File Offset: 0x00070704
		public BsonValue(object value, BsonType type)
		{
			this._value = value;
			this._type = type;
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x060014B1 RID: 5297 RVA: 0x0007251C File Offset: 0x0007071C
		public object Value
		{
			get
			{
				return this._value;
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x00072524 File Offset: 0x00070724
		public override BsonType Type
		{
			get
			{
				return this._type;
			}
		}

		// Token: 0x0400073F RID: 1855
		private readonly object _value;

		// Token: 0x04000740 RID: 1856
		private readonly BsonType _type;
	}
}
