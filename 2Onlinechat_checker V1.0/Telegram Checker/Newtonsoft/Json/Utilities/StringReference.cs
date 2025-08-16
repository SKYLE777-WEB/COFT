using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F4 RID: 244
	internal struct StringReference
	{
		// Token: 0x17000225 RID: 549
		public char this[int i]
		{
			get
			{
				return this._chars[i];
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000D13 RID: 3347 RVA: 0x00052210 File Offset: 0x00050410
		public char[] Chars
		{
			get
			{
				return this._chars;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000D14 RID: 3348 RVA: 0x00052218 File Offset: 0x00050418
		public int StartIndex
		{
			get
			{
				return this._startIndex;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000D15 RID: 3349 RVA: 0x00052220 File Offset: 0x00050420
		public int Length
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x00052228 File Offset: 0x00050428
		public StringReference(char[] chars, int startIndex, int length)
		{
			this._chars = chars;
			this._startIndex = startIndex;
			this._length = length;
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x00052240 File Offset: 0x00050440
		public override string ToString()
		{
			return new string(this._chars, this._startIndex, this._length);
		}

		// Token: 0x04000530 RID: 1328
		private readonly char[] _chars;

		// Token: 0x04000531 RID: 1329
		private readonly int _startIndex;

		// Token: 0x04000532 RID: 1330
		private readonly int _length;
	}
}
