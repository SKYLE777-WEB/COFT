using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageIdentification
{
	// Token: 0x0200001A RID: 26
	[NullableContext(1)]
	[Nullable(0)]
	[ComVisible(true)]
	public sealed class LanguageDetectionResult : ILanguageDetectionResult, IDisposable
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00007AF0 File Offset: 0x00005CF0
		public float Confidence { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00007AF8 File Offset: 0x00005CF8
		public string LanguageCode { get; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00007B00 File Offset: 0x00005D00
		public float NormalizeConfidence { get; }

		// Token: 0x060000E4 RID: 228 RVA: 0x00007B08 File Offset: 0x00005D08
		public LanguageDetectionResult(string languageCode, float confidence, float normalizeConfidence)
		{
			if (languageCode == null)
			{
				throw new ArgumentNullException("languageCode");
			}
			this.LanguageCode = languageCode;
			this.Confidence = confidence;
			this.NormalizeConfidence = normalizeConfidence;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00007B38 File Offset: 0x00005D38
		public static float NormalizeConfidenceAsProbability(int index, float[] confidences)
		{
			float num = 0f;
			float num2 = confidences[index];
			for (int i = 0; i < confidences.Length; i++)
			{
				num += (float)Math.Exp((double)(confidences[i] - num2));
			}
			return 1f / num;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00007B7C File Offset: 0x00005D7C
		public static string ToString(ILanguageDetectionResult result)
		{
			return string.Format("[{0} - Confidence: {1}]", result.LanguageCode, result.NormalizeConfidence);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00007B9C File Offset: 0x00005D9C
		public void Dispose()
		{
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00007BA0 File Offset: 0x00005DA0
		public override string ToString()
		{
			return LanguageDetectionResult.ToString(this);
		}
	}
}
