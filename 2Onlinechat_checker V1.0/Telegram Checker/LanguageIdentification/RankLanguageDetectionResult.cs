using System;
using System.Runtime.CompilerServices;

namespace LanguageIdentification
{
	// Token: 0x0200001D RID: 29
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class RankLanguageDetectionResult : ILanguageDetectionResult, IDisposable
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00007E64 File Offset: 0x00006064
		public float Confidence { get; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00007E6C File Offset: 0x0000606C
		public string LanguageCode { get; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00007E74 File Offset: 0x00006074
		public float NormalizeConfidence
		{
			get
			{
				float? normalizeConfidence = this._normalizeConfidence;
				if (normalizeConfidence == null)
				{
					float? num = (this._normalizeConfidence = new float?(this._detectionResultRank.NormalizeConfidenceAsProbability(this._index)));
					return num.Value;
				}
				return normalizeConfidence.GetValueOrDefault();
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00007EC8 File Offset: 0x000060C8
		public RankLanguageDetectionResult(LanguageDetectionResultRank detectionResultRank, string languageCode, int index)
		{
			if (detectionResultRank == null)
			{
				throw new ArgumentNullException("detectionResultRank");
			}
			this._detectionResultRank = detectionResultRank;
			if (languageCode == null)
			{
				throw new ArgumentNullException("languageCode");
			}
			this.LanguageCode = languageCode;
			this.Confidence = detectionResultRank.Confidences[index];
			this._index = index;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00007F28 File Offset: 0x00006128
		public void Dispose()
		{
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00007F2C File Offset: 0x0000612C
		public override string ToString()
		{
			return LanguageDetectionResult.ToString(this);
		}

		// Token: 0x040000AF RID: 175
		private readonly LanguageDetectionResultRank _detectionResultRank;

		// Token: 0x040000B0 RID: 176
		private readonly int _index;

		// Token: 0x040000B1 RID: 177
		private float? _normalizeConfidence;
	}
}
