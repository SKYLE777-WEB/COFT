using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LanguageIdentification
{
	// Token: 0x02000014 RID: 20
	[ComVisible(true)]
	public interface ILanguageDetectionResultRank : IEnumerable<ILanguageDetectionResult>, IEnumerable, IDisposable
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A5 RID: 165
		int Count { get; }
	}
}
