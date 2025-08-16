using System;
using System.Runtime.CompilerServices;

namespace LanguageIdentification
{
	// Token: 0x0200001C RID: 28
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PooledConfidenceLanguageDetectionResult : ILanguageDetectionResult, IDisposable
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00007CD0 File Offset: 0x00005ED0
		public float Confidence { get; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00007CD8 File Offset: 0x00005ED8
		public string LanguageCode { get; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x00007CE0 File Offset: 0x00005EE0
		public float NormalizeConfidence
		{
			get
			{
				float? normalizeConfidence = this._normalizeConfidence;
				if (normalizeConfidence == null)
				{
					return this.GetConfidenceValue();
				}
				return normalizeConfidence.GetValueOrDefault();
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00007D14 File Offset: 0x00005F14
		public PooledConfidenceLanguageDetectionResult(string languageCode, int index, float[] confidences, IFixedSizeArrayPool<float> confidenceArrayPool)
		{
			if (languageCode == null)
			{
				throw new ArgumentNullException("languageCode");
			}
			this.LanguageCode = languageCode;
			if (confidences == null)
			{
				throw new ArgumentNullException("confidences");
			}
			this._confidences = confidences;
			if (confidenceArrayPool == null)
			{
				throw new ArgumentNullException("confidenceArrayPool");
			}
			this._confidenceArrayPool = confidenceArrayPool;
			this._index = index;
			this.Confidence = confidences[index];
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00007D88 File Offset: 0x00005F88
		private float GetConfidenceValue()
		{
			this.ThrowIfDisposed();
			float? num = (this._normalizeConfidence = new float?(LanguageDetectionResult.NormalizeConfidenceAsProbability(this._index, this._confidences)));
			return num.Value;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00007DC8 File Offset: 0x00005FC8
		public override string ToString()
		{
			return LanguageDetectionResult.ToString(this);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00007DD0 File Offset: 0x00005FD0
		~PooledConfidenceLanguageDetectionResult()
		{
			this.InternalDispose();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00007E00 File Offset: 0x00006000
		public void Dispose()
		{
			this.InternalDispose();
			GC.SuppressFinalize(this);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00007E10 File Offset: 0x00006010
		private void InternalDispose()
		{
			if (!this._isDisposed)
			{
				this._isDisposed = true;
				if (this._confidenceArrayPool != null && this._confidences != null)
				{
					this._confidenceArrayPool.Return(this._confidences);
				}
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00007E4C File Offset: 0x0000604C
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException("LanguageDetectionResultRank");
			}
		}

		// Token: 0x040000A8 RID: 168
		private readonly IFixedSizeArrayPool<float> _confidenceArrayPool;

		// Token: 0x040000A9 RID: 169
		private readonly float[] _confidences;

		// Token: 0x040000AA RID: 170
		private readonly int _index;

		// Token: 0x040000AB RID: 171
		private bool _isDisposed;

		// Token: 0x040000AC RID: 172
		private float? _normalizeConfidence;
	}
}
