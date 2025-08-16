using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x02000136 RID: 310
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	internal class JsonSchemaNode
	{
		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06001076 RID: 4214 RVA: 0x00061AF0 File Offset: 0x0005FCF0
		public string Id { get; }

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06001077 RID: 4215 RVA: 0x00061AF8 File Offset: 0x0005FCF8
		public ReadOnlyCollection<JsonSchema> Schemas { get; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06001078 RID: 4216 RVA: 0x00061B00 File Offset: 0x0005FD00
		public Dictionary<string, JsonSchemaNode> Properties { get; }

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06001079 RID: 4217 RVA: 0x00061B08 File Offset: 0x0005FD08
		public Dictionary<string, JsonSchemaNode> PatternProperties { get; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x0600107A RID: 4218 RVA: 0x00061B10 File Offset: 0x0005FD10
		public List<JsonSchemaNode> Items { get; }

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x0600107B RID: 4219 RVA: 0x00061B18 File Offset: 0x0005FD18
		// (set) Token: 0x0600107C RID: 4220 RVA: 0x00061B20 File Offset: 0x0005FD20
		public JsonSchemaNode AdditionalProperties { get; set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x0600107D RID: 4221 RVA: 0x00061B2C File Offset: 0x0005FD2C
		// (set) Token: 0x0600107E RID: 4222 RVA: 0x00061B34 File Offset: 0x0005FD34
		public JsonSchemaNode AdditionalItems { get; set; }

		// Token: 0x0600107F RID: 4223 RVA: 0x00061B40 File Offset: 0x0005FD40
		public JsonSchemaNode(JsonSchema schema)
		{
			this.Schemas = new ReadOnlyCollection<JsonSchema>(new JsonSchema[] { schema });
			this.Properties = new Dictionary<string, JsonSchemaNode>();
			this.PatternProperties = new Dictionary<string, JsonSchemaNode>();
			this.Items = new List<JsonSchemaNode>();
			this.Id = JsonSchemaNode.GetId(this.Schemas);
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x00061BA0 File Offset: 0x0005FDA0
		private JsonSchemaNode(JsonSchemaNode source, JsonSchema schema)
		{
			this.Schemas = new ReadOnlyCollection<JsonSchema>(source.Schemas.Union(new JsonSchema[] { schema }).ToList<JsonSchema>());
			this.Properties = new Dictionary<string, JsonSchemaNode>(source.Properties);
			this.PatternProperties = new Dictionary<string, JsonSchemaNode>(source.PatternProperties);
			this.Items = new List<JsonSchemaNode>(source.Items);
			this.AdditionalProperties = source.AdditionalProperties;
			this.AdditionalItems = source.AdditionalItems;
			this.Id = JsonSchemaNode.GetId(this.Schemas);
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x00061C38 File Offset: 0x0005FE38
		public JsonSchemaNode Combine(JsonSchema schema)
		{
			return new JsonSchemaNode(this, schema);
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x00061C44 File Offset: 0x0005FE44
		public static string GetId(IEnumerable<JsonSchema> schemata)
		{
			return string.Join("-", schemata.Select((JsonSchema s) => s.InternalId).OrderBy((string id) => id, StringComparer.Ordinal));
		}
	}
}
