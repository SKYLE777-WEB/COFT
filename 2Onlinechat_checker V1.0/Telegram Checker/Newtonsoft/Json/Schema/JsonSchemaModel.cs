using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x02000134 RID: 308
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	internal class JsonSchemaModel
	{
		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x0600103C RID: 4156 RVA: 0x0006112C File Offset: 0x0005F32C
		// (set) Token: 0x0600103D RID: 4157 RVA: 0x00061134 File Offset: 0x0005F334
		public bool Required { get; set; }

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600103E RID: 4158 RVA: 0x00061140 File Offset: 0x0005F340
		// (set) Token: 0x0600103F RID: 4159 RVA: 0x00061148 File Offset: 0x0005F348
		public JsonSchemaType Type { get; set; }

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06001040 RID: 4160 RVA: 0x00061154 File Offset: 0x0005F354
		// (set) Token: 0x06001041 RID: 4161 RVA: 0x0006115C File Offset: 0x0005F35C
		public int? MinimumLength { get; set; }

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06001042 RID: 4162 RVA: 0x00061168 File Offset: 0x0005F368
		// (set) Token: 0x06001043 RID: 4163 RVA: 0x00061170 File Offset: 0x0005F370
		public int? MaximumLength { get; set; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06001044 RID: 4164 RVA: 0x0006117C File Offset: 0x0005F37C
		// (set) Token: 0x06001045 RID: 4165 RVA: 0x00061184 File Offset: 0x0005F384
		public double? DivisibleBy { get; set; }

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06001046 RID: 4166 RVA: 0x00061190 File Offset: 0x0005F390
		// (set) Token: 0x06001047 RID: 4167 RVA: 0x00061198 File Offset: 0x0005F398
		public double? Minimum { get; set; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06001048 RID: 4168 RVA: 0x000611A4 File Offset: 0x0005F3A4
		// (set) Token: 0x06001049 RID: 4169 RVA: 0x000611AC File Offset: 0x0005F3AC
		public double? Maximum { get; set; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600104A RID: 4170 RVA: 0x000611B8 File Offset: 0x0005F3B8
		// (set) Token: 0x0600104B RID: 4171 RVA: 0x000611C0 File Offset: 0x0005F3C0
		public bool ExclusiveMinimum { get; set; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600104C RID: 4172 RVA: 0x000611CC File Offset: 0x0005F3CC
		// (set) Token: 0x0600104D RID: 4173 RVA: 0x000611D4 File Offset: 0x0005F3D4
		public bool ExclusiveMaximum { get; set; }

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x0600104E RID: 4174 RVA: 0x000611E0 File Offset: 0x0005F3E0
		// (set) Token: 0x0600104F RID: 4175 RVA: 0x000611E8 File Offset: 0x0005F3E8
		public int? MinimumItems { get; set; }

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06001050 RID: 4176 RVA: 0x000611F4 File Offset: 0x0005F3F4
		// (set) Token: 0x06001051 RID: 4177 RVA: 0x000611FC File Offset: 0x0005F3FC
		public int? MaximumItems { get; set; }

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06001052 RID: 4178 RVA: 0x00061208 File Offset: 0x0005F408
		// (set) Token: 0x06001053 RID: 4179 RVA: 0x00061210 File Offset: 0x0005F410
		public IList<string> Patterns { get; set; }

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06001054 RID: 4180 RVA: 0x0006121C File Offset: 0x0005F41C
		// (set) Token: 0x06001055 RID: 4181 RVA: 0x00061224 File Offset: 0x0005F424
		public IList<JsonSchemaModel> Items { get; set; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06001056 RID: 4182 RVA: 0x00061230 File Offset: 0x0005F430
		// (set) Token: 0x06001057 RID: 4183 RVA: 0x00061238 File Offset: 0x0005F438
		public IDictionary<string, JsonSchemaModel> Properties { get; set; }

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06001058 RID: 4184 RVA: 0x00061244 File Offset: 0x0005F444
		// (set) Token: 0x06001059 RID: 4185 RVA: 0x0006124C File Offset: 0x0005F44C
		public IDictionary<string, JsonSchemaModel> PatternProperties { get; set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600105A RID: 4186 RVA: 0x00061258 File Offset: 0x0005F458
		// (set) Token: 0x0600105B RID: 4187 RVA: 0x00061260 File Offset: 0x0005F460
		public JsonSchemaModel AdditionalProperties { get; set; }

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x0600105C RID: 4188 RVA: 0x0006126C File Offset: 0x0005F46C
		// (set) Token: 0x0600105D RID: 4189 RVA: 0x00061274 File Offset: 0x0005F474
		public JsonSchemaModel AdditionalItems { get; set; }

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x0600105E RID: 4190 RVA: 0x00061280 File Offset: 0x0005F480
		// (set) Token: 0x0600105F RID: 4191 RVA: 0x00061288 File Offset: 0x0005F488
		public bool PositionalItemsValidation { get; set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06001060 RID: 4192 RVA: 0x00061294 File Offset: 0x0005F494
		// (set) Token: 0x06001061 RID: 4193 RVA: 0x0006129C File Offset: 0x0005F49C
		public bool AllowAdditionalProperties { get; set; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06001062 RID: 4194 RVA: 0x000612A8 File Offset: 0x0005F4A8
		// (set) Token: 0x06001063 RID: 4195 RVA: 0x000612B0 File Offset: 0x0005F4B0
		public bool AllowAdditionalItems { get; set; }

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06001064 RID: 4196 RVA: 0x000612BC File Offset: 0x0005F4BC
		// (set) Token: 0x06001065 RID: 4197 RVA: 0x000612C4 File Offset: 0x0005F4C4
		public bool UniqueItems { get; set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06001066 RID: 4198 RVA: 0x000612D0 File Offset: 0x0005F4D0
		// (set) Token: 0x06001067 RID: 4199 RVA: 0x000612D8 File Offset: 0x0005F4D8
		public IList<JToken> Enum { get; set; }

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06001068 RID: 4200 RVA: 0x000612E4 File Offset: 0x0005F4E4
		// (set) Token: 0x06001069 RID: 4201 RVA: 0x000612EC File Offset: 0x0005F4EC
		public JsonSchemaType Disallow { get; set; }

		// Token: 0x0600106A RID: 4202 RVA: 0x000612F8 File Offset: 0x0005F4F8
		public JsonSchemaModel()
		{
			this.Type = JsonSchemaType.Any;
			this.AllowAdditionalProperties = true;
			this.AllowAdditionalItems = true;
			this.Required = false;
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x0006132C File Offset: 0x0005F52C
		public static JsonSchemaModel Create(IList<JsonSchema> schemata)
		{
			JsonSchemaModel jsonSchemaModel = new JsonSchemaModel();
			foreach (JsonSchema jsonSchema in schemata)
			{
				JsonSchemaModel.Combine(jsonSchemaModel, jsonSchema);
			}
			return jsonSchemaModel;
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x00061384 File Offset: 0x0005F584
		private static void Combine(JsonSchemaModel model, JsonSchema schema)
		{
			model.Required = model.Required || (schema.Required ?? false);
			model.Type &= schema.Type ?? JsonSchemaType.Any;
			model.MinimumLength = MathUtils.Max(model.MinimumLength, schema.MinimumLength);
			model.MaximumLength = MathUtils.Min(model.MaximumLength, schema.MaximumLength);
			model.DivisibleBy = MathUtils.Max(model.DivisibleBy, schema.DivisibleBy);
			model.Minimum = MathUtils.Max(model.Minimum, schema.Minimum);
			model.Maximum = MathUtils.Max(model.Maximum, schema.Maximum);
			model.ExclusiveMinimum = model.ExclusiveMinimum || (schema.ExclusiveMinimum ?? false);
			model.ExclusiveMaximum = model.ExclusiveMaximum || (schema.ExclusiveMaximum ?? false);
			model.MinimumItems = MathUtils.Max(model.MinimumItems, schema.MinimumItems);
			model.MaximumItems = MathUtils.Min(model.MaximumItems, schema.MaximumItems);
			model.PositionalItemsValidation = model.PositionalItemsValidation || schema.PositionalItemsValidation;
			model.AllowAdditionalProperties = model.AllowAdditionalProperties && schema.AllowAdditionalProperties;
			model.AllowAdditionalItems = model.AllowAdditionalItems && schema.AllowAdditionalItems;
			model.UniqueItems = model.UniqueItems || schema.UniqueItems;
			if (schema.Enum != null)
			{
				if (model.Enum == null)
				{
					model.Enum = new List<JToken>();
				}
				model.Enum.AddRangeDistinct(schema.Enum, JToken.EqualityComparer);
			}
			model.Disallow |= schema.Disallow ?? JsonSchemaType.None;
			if (schema.Pattern != null)
			{
				if (model.Patterns == null)
				{
					model.Patterns = new List<string>();
				}
				model.Patterns.AddDistinct(schema.Pattern);
			}
		}
	}
}
