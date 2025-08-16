using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageIdentification
{
	// Token: 0x02000015 RID: 21
	[NullableContext(1)]
	[ComVisible(true)]
	public interface ILanguageIdentificationClassifier
	{
		// Token: 0x060000A6 RID: 166
		void Append(string text);

		// Token: 0x060000A7 RID: 167
		[NullableContext(0)]
		void Append(ReadOnlySpan<char> text);

		// Token: 0x060000A8 RID: 168
		void Append(byte[] buffer, int start, int length);

		// Token: 0x060000A9 RID: 169
		[NullableContext(0)]
		void Append(ReadOnlySpan<byte> buffer);

		// Token: 0x060000AA RID: 170
		ILanguageDetectionResult Classify();

		// Token: 0x060000AB RID: 171
		ILanguageDetectionResultRank CreateRank();

		// Token: 0x060000AC RID: 172
		IEnumerable<string> GetSupportedLanguages();

		// Token: 0x060000AD RID: 173
		void Reset();
	}
}
