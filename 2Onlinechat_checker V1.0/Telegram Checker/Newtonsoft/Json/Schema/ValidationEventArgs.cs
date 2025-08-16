using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x0200013C RID: 316
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public class ValidationEventArgs : EventArgs
	{
		// Token: 0x06001090 RID: 4240 RVA: 0x0006251C File Offset: 0x0006071C
		internal ValidationEventArgs(JsonSchemaException ex)
		{
			ValidationUtils.ArgumentNotNull(ex, "ex");
			this._ex = ex;
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06001091 RID: 4241 RVA: 0x00062538 File Offset: 0x00060738
		public JsonSchemaException Exception
		{
			get
			{
				return this._ex;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x00062540 File Offset: 0x00060740
		public string Path
		{
			get
			{
				return this._ex.Path;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06001093 RID: 4243 RVA: 0x00062550 File Offset: 0x00060750
		public string Message
		{
			get
			{
				return this._ex.Message;
			}
		}

		// Token: 0x04000680 RID: 1664
		private readonly JsonSchemaException _ex;
	}
}
