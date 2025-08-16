using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000124 RID: 292
	public class MemoryTraceWriter : ITraceWriter
	{
		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000F45 RID: 3909 RVA: 0x0005DBF4 File Offset: 0x0005BDF4
		// (set) Token: 0x06000F46 RID: 3910 RVA: 0x0005DBFC File Offset: 0x0005BDFC
		public TraceLevel LevelFilter { get; set; }

		// Token: 0x06000F47 RID: 3911 RVA: 0x0005DC08 File Offset: 0x0005BE08
		public MemoryTraceWriter()
		{
			this.LevelFilter = TraceLevel.Verbose;
			this._traceMessages = new Queue<string>();
			this._lock = new object();
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x0005DC30 File Offset: 0x0005BE30
		public void Trace(TraceLevel level, string message, Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff", CultureInfo.InvariantCulture));
			stringBuilder.Append(" ");
			stringBuilder.Append(level.ToString("g"));
			stringBuilder.Append(" ");
			stringBuilder.Append(message);
			string text = stringBuilder.ToString();
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._traceMessages.Count >= 1000)
				{
					this._traceMessages.Dequeue();
				}
				this._traceMessages.Enqueue(text);
			}
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x0005DD00 File Offset: 0x0005BF00
		public IEnumerable<string> GetTraceMessages()
		{
			return this._traceMessages;
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0005DD08 File Offset: 0x0005BF08
		public override string ToString()
		{
			object @lock = this._lock;
			string text2;
			lock (@lock)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in this._traceMessages)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.Append(text);
				}
				text2 = stringBuilder.ToString();
			}
			return text2;
		}

		// Token: 0x040005EC RID: 1516
		private readonly Queue<string> _traceMessages;

		// Token: 0x040005ED RID: 1517
		private readonly object _lock;
	}
}
