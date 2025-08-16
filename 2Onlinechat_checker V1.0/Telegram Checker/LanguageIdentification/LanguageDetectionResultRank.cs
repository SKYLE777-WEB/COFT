using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LanguageIdentification
{
	// Token: 0x0200001B RID: 27
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class LanguageDetectionResultRank : ILanguageDetectionResultRank, IEnumerable<ILanguageDetectionResult>, IEnumerable, IDisposable
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00007BA8 File Offset: 0x00005DA8
		internal float[] Confidences { get; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00007BB0 File Offset: 0x00005DB0
		public int Count
		{
			get
			{
				return this.Confidences.Length;
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00007BBC File Offset: 0x00005DBC
		public LanguageDetectionResultRank(string[] languageClasses, float[] confidences, IFixedSizeArrayPool<float> confidenceArrayPool)
		{
			LanguageDetectionResultRank <>4__this = this;
			this.Confidences = confidences;
			this._confidenceArrayPool = confidenceArrayPool;
			this._results = confidences.Select((float item, int index) => new RankLanguageDetectionResult(<>4__this, languageClasses[index], index));
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00007C10 File Offset: 0x00005E10
		internal float NormalizeConfidenceAsProbability(int index)
		{
			this.ThrowIfDisposed();
			return LanguageDetectionResult.NormalizeConfidenceAsProbability(index, this.Confidences);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007C24 File Offset: 0x00005E24
		public IEnumerator<ILanguageDetectionResult> GetEnumerator()
		{
			this.ThrowIfDisposed();
			return this._results.GetEnumerator();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007C38 File Offset: 0x00005E38
		IEnumerator IEnumerable.GetEnumerator()
		{
			this.ThrowIfDisposed();
			return this.GetEnumerator();
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007C48 File Offset: 0x00005E48
		~LanguageDetectionResultRank()
		{
			this.InternalDispose();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007C78 File Offset: 0x00005E78
		public void Dispose()
		{
			this.InternalDispose();
			GC.SuppressFinalize(this);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007C88 File Offset: 0x00005E88
		private void InternalDispose()
		{
			if (!this._isDisposed)
			{
				this._isDisposed = true;
				IFixedSizeArrayPool<float> confidenceArrayPool = this._confidenceArrayPool;
				if (confidenceArrayPool == null)
				{
					return;
				}
				confidenceArrayPool.Return(this.Confidences);
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007CB8 File Offset: 0x00005EB8
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException("LanguageDetectionResultRank");
			}
		}

		// Token: 0x040000A4 RID: 164
		[Nullable(2)]
		private readonly IFixedSizeArrayPool<float> _confidenceArrayPool;

		// Token: 0x040000A5 RID: 165
		private readonly IEnumerable<ILanguageDetectionResult> _results;

		// Token: 0x040000A6 RID: 166
		private bool _isDisposed;
	}
}
