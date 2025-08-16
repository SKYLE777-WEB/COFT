using System;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x02000138 RID: 312
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public class JsonSchemaResolver
	{
		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06001085 RID: 4229 RVA: 0x00061CC4 File Offset: 0x0005FEC4
		// (set) Token: 0x06001086 RID: 4230 RVA: 0x00061CCC File Offset: 0x0005FECC
		public IList<JsonSchema> LoadedSchemas { get; protected set; }

		// Token: 0x06001087 RID: 4231 RVA: 0x00061CD8 File Offset: 0x0005FED8
		public JsonSchemaResolver()
		{
			this.LoadedSchemas = new List<JsonSchema>();
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x00061CEC File Offset: 0x0005FEEC
		public virtual JsonSchema GetSchema(string reference)
		{
			JsonSchema jsonSchema = this.LoadedSchemas.SingleOrDefault((JsonSchema s) => string.Equals(s.Id, reference, StringComparison.Ordinal));
			if (jsonSchema == null)
			{
				jsonSchema = this.LoadedSchemas.SingleOrDefault((JsonSchema s) => string.Equals(s.Location, reference, StringComparison.Ordinal));
			}
			return jsonSchema;
		}
	}
}
