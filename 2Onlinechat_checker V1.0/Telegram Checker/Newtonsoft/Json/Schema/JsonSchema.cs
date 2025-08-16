using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x0200012F RID: 303
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public class JsonSchema
	{
		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000FBA RID: 4026 RVA: 0x0005EF2C File Offset: 0x0005D12C
		// (set) Token: 0x06000FBB RID: 4027 RVA: 0x0005EF34 File Offset: 0x0005D134
		public string Id { get; set; }

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000FBC RID: 4028 RVA: 0x0005EF40 File Offset: 0x0005D140
		// (set) Token: 0x06000FBD RID: 4029 RVA: 0x0005EF48 File Offset: 0x0005D148
		public string Title { get; set; }

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000FBE RID: 4030 RVA: 0x0005EF54 File Offset: 0x0005D154
		// (set) Token: 0x06000FBF RID: 4031 RVA: 0x0005EF5C File Offset: 0x0005D15C
		public bool? Required { get; set; }

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0005EF68 File Offset: 0x0005D168
		// (set) Token: 0x06000FC1 RID: 4033 RVA: 0x0005EF70 File Offset: 0x0005D170
		public bool? ReadOnly { get; set; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06000FC2 RID: 4034 RVA: 0x0005EF7C File Offset: 0x0005D17C
		// (set) Token: 0x06000FC3 RID: 4035 RVA: 0x0005EF84 File Offset: 0x0005D184
		public bool? Hidden { get; set; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000FC4 RID: 4036 RVA: 0x0005EF90 File Offset: 0x0005D190
		// (set) Token: 0x06000FC5 RID: 4037 RVA: 0x0005EF98 File Offset: 0x0005D198
		public bool? Transient { get; set; }

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x0005EFA4 File Offset: 0x0005D1A4
		// (set) Token: 0x06000FC7 RID: 4039 RVA: 0x0005EFAC File Offset: 0x0005D1AC
		public string Description { get; set; }

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x0005EFB8 File Offset: 0x0005D1B8
		// (set) Token: 0x06000FC9 RID: 4041 RVA: 0x0005EFC0 File Offset: 0x0005D1C0
		public JsonSchemaType? Type { get; set; }

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000FCA RID: 4042 RVA: 0x0005EFCC File Offset: 0x0005D1CC
		// (set) Token: 0x06000FCB RID: 4043 RVA: 0x0005EFD4 File Offset: 0x0005D1D4
		public string Pattern { get; set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000FCC RID: 4044 RVA: 0x0005EFE0 File Offset: 0x0005D1E0
		// (set) Token: 0x06000FCD RID: 4045 RVA: 0x0005EFE8 File Offset: 0x0005D1E8
		public int? MinimumLength { get; set; }

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000FCE RID: 4046 RVA: 0x0005EFF4 File Offset: 0x0005D1F4
		// (set) Token: 0x06000FCF RID: 4047 RVA: 0x0005EFFC File Offset: 0x0005D1FC
		public int? MaximumLength { get; set; }

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06000FD0 RID: 4048 RVA: 0x0005F008 File Offset: 0x0005D208
		// (set) Token: 0x06000FD1 RID: 4049 RVA: 0x0005F010 File Offset: 0x0005D210
		public double? DivisibleBy { get; set; }

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x0005F01C File Offset: 0x0005D21C
		// (set) Token: 0x06000FD3 RID: 4051 RVA: 0x0005F024 File Offset: 0x0005D224
		public double? Minimum { get; set; }

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x0005F030 File Offset: 0x0005D230
		// (set) Token: 0x06000FD5 RID: 4053 RVA: 0x0005F038 File Offset: 0x0005D238
		public double? Maximum { get; set; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000FD6 RID: 4054 RVA: 0x0005F044 File Offset: 0x0005D244
		// (set) Token: 0x06000FD7 RID: 4055 RVA: 0x0005F04C File Offset: 0x0005D24C
		public bool? ExclusiveMinimum { get; set; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x0005F058 File Offset: 0x0005D258
		// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x0005F060 File Offset: 0x0005D260
		public bool? ExclusiveMaximum { get; set; }

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x0005F06C File Offset: 0x0005D26C
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x0005F074 File Offset: 0x0005D274
		public int? MinimumItems { get; set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0005F080 File Offset: 0x0005D280
		// (set) Token: 0x06000FDD RID: 4061 RVA: 0x0005F088 File Offset: 0x0005D288
		public int? MaximumItems { get; set; }

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x0005F094 File Offset: 0x0005D294
		// (set) Token: 0x06000FDF RID: 4063 RVA: 0x0005F09C File Offset: 0x0005D29C
		public IList<JsonSchema> Items { get; set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x0005F0A8 File Offset: 0x0005D2A8
		// (set) Token: 0x06000FE1 RID: 4065 RVA: 0x0005F0B0 File Offset: 0x0005D2B0
		public bool PositionalItemsValidation { get; set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000FE2 RID: 4066 RVA: 0x0005F0BC File Offset: 0x0005D2BC
		// (set) Token: 0x06000FE3 RID: 4067 RVA: 0x0005F0C4 File Offset: 0x0005D2C4
		public JsonSchema AdditionalItems { get; set; }

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x0005F0D0 File Offset: 0x0005D2D0
		// (set) Token: 0x06000FE5 RID: 4069 RVA: 0x0005F0D8 File Offset: 0x0005D2D8
		public bool AllowAdditionalItems { get; set; }

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x0005F0E4 File Offset: 0x0005D2E4
		// (set) Token: 0x06000FE7 RID: 4071 RVA: 0x0005F0EC File Offset: 0x0005D2EC
		public bool UniqueItems { get; set; }

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000FE8 RID: 4072 RVA: 0x0005F0F8 File Offset: 0x0005D2F8
		// (set) Token: 0x06000FE9 RID: 4073 RVA: 0x0005F100 File Offset: 0x0005D300
		public IDictionary<string, JsonSchema> Properties { get; set; }

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x0005F10C File Offset: 0x0005D30C
		// (set) Token: 0x06000FEB RID: 4075 RVA: 0x0005F114 File Offset: 0x0005D314
		public JsonSchema AdditionalProperties { get; set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x0005F120 File Offset: 0x0005D320
		// (set) Token: 0x06000FED RID: 4077 RVA: 0x0005F128 File Offset: 0x0005D328
		public IDictionary<string, JsonSchema> PatternProperties { get; set; }

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000FEE RID: 4078 RVA: 0x0005F134 File Offset: 0x0005D334
		// (set) Token: 0x06000FEF RID: 4079 RVA: 0x0005F13C File Offset: 0x0005D33C
		public bool AllowAdditionalProperties { get; set; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0005F148 File Offset: 0x0005D348
		// (set) Token: 0x06000FF1 RID: 4081 RVA: 0x0005F150 File Offset: 0x0005D350
		public string Requires { get; set; }

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x0005F15C File Offset: 0x0005D35C
		// (set) Token: 0x06000FF3 RID: 4083 RVA: 0x0005F164 File Offset: 0x0005D364
		public IList<JToken> Enum { get; set; }

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x0005F170 File Offset: 0x0005D370
		// (set) Token: 0x06000FF5 RID: 4085 RVA: 0x0005F178 File Offset: 0x0005D378
		public JsonSchemaType? Disallow { get; set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x0005F184 File Offset: 0x0005D384
		// (set) Token: 0x06000FF7 RID: 4087 RVA: 0x0005F18C File Offset: 0x0005D38C
		public JToken Default { get; set; }

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x0005F198 File Offset: 0x0005D398
		// (set) Token: 0x06000FF9 RID: 4089 RVA: 0x0005F1A0 File Offset: 0x0005D3A0
		public IList<JsonSchema> Extends { get; set; }

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x0005F1AC File Offset: 0x0005D3AC
		// (set) Token: 0x06000FFB RID: 4091 RVA: 0x0005F1B4 File Offset: 0x0005D3B4
		public string Format { get; set; }

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000FFC RID: 4092 RVA: 0x0005F1C0 File Offset: 0x0005D3C0
		// (set) Token: 0x06000FFD RID: 4093 RVA: 0x0005F1C8 File Offset: 0x0005D3C8
		internal string Location { get; set; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x0005F1D4 File Offset: 0x0005D3D4
		internal string InternalId
		{
			get
			{
				return this._internalId;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000FFF RID: 4095 RVA: 0x0005F1DC File Offset: 0x0005D3DC
		// (set) Token: 0x06001000 RID: 4096 RVA: 0x0005F1E4 File Offset: 0x0005D3E4
		internal string DeferredReference { get; set; }

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x0005F1F0 File Offset: 0x0005D3F0
		// (set) Token: 0x06001002 RID: 4098 RVA: 0x0005F1F8 File Offset: 0x0005D3F8
		internal bool ReferencesResolved { get; set; }

		// Token: 0x06001003 RID: 4099 RVA: 0x0005F204 File Offset: 0x0005D404
		public JsonSchema()
		{
			this.AllowAdditionalProperties = true;
			this.AllowAdditionalItems = true;
		}

		// Token: 0x06001004 RID: 4100 RVA: 0x0005F244 File Offset: 0x0005D444
		public static JsonSchema Read(JsonReader reader)
		{
			return JsonSchema.Read(reader, new JsonSchemaResolver());
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x0005F254 File Offset: 0x0005D454
		public static JsonSchema Read(JsonReader reader, JsonSchemaResolver resolver)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			ValidationUtils.ArgumentNotNull(resolver, "resolver");
			return new JsonSchemaBuilder(resolver).Read(reader);
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0005F278 File Offset: 0x0005D478
		public static JsonSchema Parse(string json)
		{
			return JsonSchema.Parse(json, new JsonSchemaResolver());
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x0005F288 File Offset: 0x0005D488
		public static JsonSchema Parse(string json, JsonSchemaResolver resolver)
		{
			ValidationUtils.ArgumentNotNull(json, "json");
			JsonSchema jsonSchema;
			using (JsonReader jsonReader = new JsonTextReader(new StringReader(json)))
			{
				jsonSchema = JsonSchema.Read(jsonReader, resolver);
			}
			return jsonSchema;
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x0005F2D8 File Offset: 0x0005D4D8
		public void WriteTo(JsonWriter writer)
		{
			this.WriteTo(writer, new JsonSchemaResolver());
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x0005F2E8 File Offset: 0x0005D4E8
		public void WriteTo(JsonWriter writer, JsonSchemaResolver resolver)
		{
			ValidationUtils.ArgumentNotNull(writer, "writer");
			ValidationUtils.ArgumentNotNull(resolver, "resolver");
			new JsonSchemaWriter(writer, resolver).WriteSchema(this);
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x0005F310 File Offset: 0x0005D510
		public override string ToString()
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			this.WriteTo(new JsonTextWriter(stringWriter)
			{
				Formatting = Formatting.Indented
			});
			return stringWriter.ToString();
		}

		// Token: 0x0400061D RID: 1565
		private readonly string _internalId = Guid.NewGuid().ToString("N");
	}
}
