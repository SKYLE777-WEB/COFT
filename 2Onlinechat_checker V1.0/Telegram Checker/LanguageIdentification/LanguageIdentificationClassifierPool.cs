using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace LanguageIdentification
{
	// Token: 0x02000017 RID: 23
	[NullableContext(1)]
	[Nullable(0)]
	[ComVisible(true)]
	public class LanguageIdentificationClassifierPool
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x000073E4 File Offset: 0x000055E4
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x000073EC File Offset: 0x000055EC
		public static LanguageIdentificationClassifierPool Default { get; private set; } = new LanguageIdentificationClassifierPool(Environment.ProcessorCount);

		// Token: 0x060000C6 RID: 198 RVA: 0x000073F4 File Offset: 0x000055F4
		public LanguageIdentificationClassifierPool(int maxRetainCount)
		{
			if (maxRetainCount < 1)
			{
				throw new ArgumentOutOfRangeException("maxRetainCount", "min value is 1.");
			}
			this._maxRetainCount = maxRetainCount;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00007428 File Offset: 0x00005628
		public static void SetDefaultMaxRemainCount(int maxRemainCount)
		{
			LanguageIdentificationClassifierPool.Default = new LanguageIdentificationClassifierPool(maxRemainCount);
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00007438 File Offset: 0x00005638
		public void Clear()
		{
			ILanguageIdentificationClassifier languageIdentificationClassifier;
			while (this._items.TryTake(out languageIdentificationClassifier))
			{
				Interlocked.Decrement(ref this._count);
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000746C File Offset: 0x0000566C
		public ILanguageIdentificationClassifier Rent()
		{
			ILanguageIdentificationClassifier languageIdentificationClassifier;
			if (this._items.TryTake(out languageIdentificationClassifier))
			{
				Interlocked.Decrement(ref this._count);
				return languageIdentificationClassifier;
			}
			return this.CreateClassifier();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000074A4 File Offset: 0x000056A4
		public void Return(ILanguageIdentificationClassifier item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (Interlocked.Increment(ref this._count) <= this._maxRetainCount)
			{
				item.Reset();
				this._items.Add(item);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000074E0 File Offset: 0x000056E0
		protected virtual ILanguageIdentificationClassifier CreateClassifier()
		{
			return new LanguageIdentificationClassifier();
		}

		// Token: 0x04000093 RID: 147
		private readonly ConcurrentBag<ILanguageIdentificationClassifier> _items = new ConcurrentBag<ILanguageIdentificationClassifier>();

		// Token: 0x04000094 RID: 148
		private readonly int _maxRetainCount;

		// Token: 0x04000095 RID: 149
		private int _count;
	}
}
