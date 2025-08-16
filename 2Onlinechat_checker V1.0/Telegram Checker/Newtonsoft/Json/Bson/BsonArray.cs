using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x0200018F RID: 399
	internal class BsonArray : BsonToken, IEnumerable<BsonToken>, IEnumerable
	{
		// Token: 0x060014A8 RID: 5288 RVA: 0x00072484 File Offset: 0x00070684
		public void Add(BsonToken token)
		{
			this._children.Add(token);
			token.Parent = this;
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x060014A9 RID: 5289 RVA: 0x0007249C File Offset: 0x0007069C
		public override BsonType Type
		{
			get
			{
				return BsonType.Array;
			}
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x000724A0 File Offset: 0x000706A0
		public IEnumerator<BsonToken> GetEnumerator()
		{
			return this._children.GetEnumerator();
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x000724B4 File Offset: 0x000706B4
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0400073B RID: 1851
		private readonly List<BsonToken> _children = new List<BsonToken>();
	}
}
