using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000102 RID: 258
	public class DiagnosticsTraceWriter : ITraceWriter
	{
		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000D99 RID: 3481 RVA: 0x000551A0 File Offset: 0x000533A0
		// (set) Token: 0x06000D9A RID: 3482 RVA: 0x000551A8 File Offset: 0x000533A8
		public TraceLevel LevelFilter { get; set; }

		// Token: 0x06000D9B RID: 3483 RVA: 0x000551B4 File Offset: 0x000533B4
		private TraceEventType GetTraceEventType(TraceLevel level)
		{
			switch (level)
			{
			case TraceLevel.Error:
				return TraceEventType.Error;
			case TraceLevel.Warning:
				return TraceEventType.Warning;
			case TraceLevel.Info:
				return TraceEventType.Information;
			case TraceLevel.Verbose:
				return TraceEventType.Verbose;
			default:
				throw new ArgumentOutOfRangeException("level");
			}
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x000551E8 File Offset: 0x000533E8
		public void Trace(TraceLevel level, string message, Exception ex)
		{
			if (level == TraceLevel.Off)
			{
				return;
			}
			TraceEventCache traceEventCache = new TraceEventCache();
			TraceEventType traceEventType = this.GetTraceEventType(level);
			foreach (object obj in global::System.Diagnostics.Trace.Listeners)
			{
				TraceListener traceListener = (TraceListener)obj;
				if (!traceListener.IsThreadSafe)
				{
					TraceListener traceListener2 = traceListener;
					lock (traceListener2)
					{
						traceListener.TraceEvent(traceEventCache, "Newtonsoft.Json", traceEventType, 0, message);
						goto IL_007D;
					}
					goto IL_006E;
				}
				goto IL_006E;
				IL_007D:
				if (global::System.Diagnostics.Trace.AutoFlush)
				{
					traceListener.Flush();
					continue;
				}
				continue;
				IL_006E:
				traceListener.TraceEvent(traceEventCache, "Newtonsoft.Json", traceEventType, 0, message);
				goto IL_007D;
			}
		}
	}
}
