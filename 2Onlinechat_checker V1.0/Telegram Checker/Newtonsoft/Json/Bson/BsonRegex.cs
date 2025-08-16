using System;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000195 RID: 405
	internal class BsonRegex : BsonToken
	{
		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x000725AC File Offset: 0x000707AC
		// (set) Token: 0x060014BD RID: 5309 RVA: 0x000725B4 File Offset: 0x000707B4
		public BsonString Pattern { get; set; }

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x060014BE RID: 5310 RVA: 0x000725C0 File Offset: 0x000707C0
		// (set) Token: 0x060014BF RID: 5311 RVA: 0x000725C8 File Offset: 0x000707C8
		public BsonString Options { get; set; }

		// Token: 0x060014C0 RID: 5312 RVA: 0x000725D4 File Offset: 0x000707D4
		public BsonRegex(string pattern, string options)
		{
			this.Pattern = new BsonString(pattern, false);
			this.Options = new BsonString(options, false);
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x060014C1 RID: 5313 RVA: 0x000725F8 File Offset: 0x000707F8
		public override BsonType Type
		{
			get
			{
				return BsonType.Regex;
			}
		}
	}
}
