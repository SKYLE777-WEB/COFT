using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200010B RID: 267
	public interface ITraceWriter
	{
		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000DB9 RID: 3513
		TraceLevel LevelFilter { get; }

		// Token: 0x06000DBA RID: 3514
		void Trace(TraceLevel level, string message, Exception ex);
	}
}
