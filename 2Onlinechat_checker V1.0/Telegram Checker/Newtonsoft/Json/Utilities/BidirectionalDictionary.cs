using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000CA RID: 202
	internal class BidirectionalDictionary<TFirst, TSecond>
	{
		// Token: 0x06000B6A RID: 2922 RVA: 0x00048838 File Offset: 0x00046A38
		public BidirectionalDictionary()
			: this(EqualityComparer<TFirst>.Default, EqualityComparer<TSecond>.Default)
		{
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x0004884C File Offset: 0x00046A4C
		public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer)
			: this(firstEqualityComparer, secondEqualityComparer, "Duplicate item already exists for '{0}'.", "Duplicate item already exists for '{0}'.")
		{
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x00048860 File Offset: 0x00046A60
		public BidirectionalDictionary(IEqualityComparer<TFirst> firstEqualityComparer, IEqualityComparer<TSecond> secondEqualityComparer, string duplicateFirstErrorMessage, string duplicateSecondErrorMessage)
		{
			this._firstToSecond = new Dictionary<TFirst, TSecond>(firstEqualityComparer);
			this._secondToFirst = new Dictionary<TSecond, TFirst>(secondEqualityComparer);
			this._duplicateFirstErrorMessage = duplicateFirstErrorMessage;
			this._duplicateSecondErrorMessage = duplicateSecondErrorMessage;
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x00048890 File Offset: 0x00046A90
		public void Set(TFirst first, TSecond second)
		{
			TSecond tsecond;
			if (this._firstToSecond.TryGetValue(first, out tsecond) && !tsecond.Equals(second))
			{
				throw new ArgumentException(this._duplicateFirstErrorMessage.FormatWith(CultureInfo.InvariantCulture, first));
			}
			TFirst tfirst;
			if (this._secondToFirst.TryGetValue(second, out tfirst) && !tfirst.Equals(first))
			{
				throw new ArgumentException(this._duplicateSecondErrorMessage.FormatWith(CultureInfo.InvariantCulture, second));
			}
			this._firstToSecond.Add(first, second);
			this._secondToFirst.Add(second, first);
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0004894C File Offset: 0x00046B4C
		public bool TryGetByFirst(TFirst first, out TSecond second)
		{
			return this._firstToSecond.TryGetValue(first, out second);
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x0004895C File Offset: 0x00046B5C
		public bool TryGetBySecond(TSecond second, out TFirst first)
		{
			return this._secondToFirst.TryGetValue(second, out first);
		}

		// Token: 0x0400047F RID: 1151
		private readonly IDictionary<TFirst, TSecond> _firstToSecond;

		// Token: 0x04000480 RID: 1152
		private readonly IDictionary<TSecond, TFirst> _secondToFirst;

		// Token: 0x04000481 RID: 1153
		private readonly string _duplicateFirstErrorMessage;

		// Token: 0x04000482 RID: 1154
		private readonly string _duplicateSecondErrorMessage;
	}
}
