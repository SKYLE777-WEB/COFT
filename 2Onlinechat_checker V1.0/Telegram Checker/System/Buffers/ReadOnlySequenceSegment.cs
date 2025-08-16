using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x02000041 RID: 65
	[ComVisible(true)]
	public abstract class ReadOnlySequenceSegment<T>
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0001237C File Offset: 0x0001057C
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x00012384 File Offset: 0x00010584
		public ReadOnlyMemory<T> Memory { get; protected set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002CA RID: 714 RVA: 0x00012390 File Offset: 0x00010590
		// (set) Token: 0x060002CB RID: 715 RVA: 0x00012398 File Offset: 0x00010598
		public ReadOnlySequenceSegment<T> Next { get; protected set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002CC RID: 716 RVA: 0x000123A4 File Offset: 0x000105A4
		// (set) Token: 0x060002CD RID: 717 RVA: 0x000123AC File Offset: 0x000105AC
		public long RunningIndex { get; protected set; }
	}
}
