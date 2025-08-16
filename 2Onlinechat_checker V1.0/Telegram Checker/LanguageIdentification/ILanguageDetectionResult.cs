using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageIdentification
{
	// Token: 0x02000013 RID: 19
	[NullableContext(1)]
	[ComVisible(true)]
	public interface ILanguageDetectionResult : IDisposable
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A2 RID: 162
		float Confidence { get; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A3 RID: 163
		string LanguageCode { get; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A4 RID: 164
		float NormalizeConfidence { get; }
	}
}
