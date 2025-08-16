using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x0200018E RID: 398
	internal class BsonObject : BsonToken, IEnumerable<BsonProperty>, IEnumerable
	{
		// Token: 0x060014A3 RID: 5283 RVA: 0x00072414 File Offset: 0x00070614
		public void Add(string name, BsonToken token)
		{
			this._children.Add(new BsonProperty
			{
				Name = new BsonString(name, false),
				Value = token
			});
			token.Parent = this;
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x060014A4 RID: 5284 RVA: 0x00072450 File Offset: 0x00070650
		public override BsonType Type
		{
			get
			{
				return BsonType.Object;
			}
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x00072454 File Offset: 0x00070654
		public IEnumerator<BsonProperty> GetEnumerator()
		{
			return this._children.GetEnumerator();
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x00072468 File Offset: 0x00070668
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0400073A RID: 1850
		private readonly List<BsonProperty> _children = new List<BsonProperty>();
	}
}
