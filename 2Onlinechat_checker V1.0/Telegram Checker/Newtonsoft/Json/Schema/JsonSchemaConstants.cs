using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x02000131 RID: 305
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	internal static class JsonSchemaConstants
	{
		// Token: 0x04000625 RID: 1573
		public const string TypePropertyName = "type";

		// Token: 0x04000626 RID: 1574
		public const string PropertiesPropertyName = "properties";

		// Token: 0x04000627 RID: 1575
		public const string ItemsPropertyName = "items";

		// Token: 0x04000628 RID: 1576
		public const string AdditionalItemsPropertyName = "additionalItems";

		// Token: 0x04000629 RID: 1577
		public const string RequiredPropertyName = "required";

		// Token: 0x0400062A RID: 1578
		public const string PatternPropertiesPropertyName = "patternProperties";

		// Token: 0x0400062B RID: 1579
		public const string AdditionalPropertiesPropertyName = "additionalProperties";

		// Token: 0x0400062C RID: 1580
		public const string RequiresPropertyName = "requires";

		// Token: 0x0400062D RID: 1581
		public const string MinimumPropertyName = "minimum";

		// Token: 0x0400062E RID: 1582
		public const string MaximumPropertyName = "maximum";

		// Token: 0x0400062F RID: 1583
		public const string ExclusiveMinimumPropertyName = "exclusiveMinimum";

		// Token: 0x04000630 RID: 1584
		public const string ExclusiveMaximumPropertyName = "exclusiveMaximum";

		// Token: 0x04000631 RID: 1585
		public const string MinimumItemsPropertyName = "minItems";

		// Token: 0x04000632 RID: 1586
		public const string MaximumItemsPropertyName = "maxItems";

		// Token: 0x04000633 RID: 1587
		public const string PatternPropertyName = "pattern";

		// Token: 0x04000634 RID: 1588
		public const string MaximumLengthPropertyName = "maxLength";

		// Token: 0x04000635 RID: 1589
		public const string MinimumLengthPropertyName = "minLength";

		// Token: 0x04000636 RID: 1590
		public const string EnumPropertyName = "enum";

		// Token: 0x04000637 RID: 1591
		public const string ReadOnlyPropertyName = "readonly";

		// Token: 0x04000638 RID: 1592
		public const string TitlePropertyName = "title";

		// Token: 0x04000639 RID: 1593
		public const string DescriptionPropertyName = "description";

		// Token: 0x0400063A RID: 1594
		public const string FormatPropertyName = "format";

		// Token: 0x0400063B RID: 1595
		public const string DefaultPropertyName = "default";

		// Token: 0x0400063C RID: 1596
		public const string TransientPropertyName = "transient";

		// Token: 0x0400063D RID: 1597
		public const string DivisibleByPropertyName = "divisibleBy";

		// Token: 0x0400063E RID: 1598
		public const string HiddenPropertyName = "hidden";

		// Token: 0x0400063F RID: 1599
		public const string DisallowPropertyName = "disallow";

		// Token: 0x04000640 RID: 1600
		public const string ExtendsPropertyName = "extends";

		// Token: 0x04000641 RID: 1601
		public const string IdPropertyName = "id";

		// Token: 0x04000642 RID: 1602
		public const string UniqueItemsPropertyName = "uniqueItems";

		// Token: 0x04000643 RID: 1603
		public const string OptionValuePropertyName = "value";

		// Token: 0x04000644 RID: 1604
		public const string OptionLabelPropertyName = "label";

		// Token: 0x04000645 RID: 1605
		public static readonly IDictionary<string, JsonSchemaType> JsonSchemaTypeMapping = new Dictionary<string, JsonSchemaType>
		{
			{
				"string",
				JsonSchemaType.String
			},
			{
				"object",
				JsonSchemaType.Object
			},
			{
				"integer",
				JsonSchemaType.Integer
			},
			{
				"number",
				JsonSchemaType.Float
			},
			{
				"null",
				JsonSchemaType.Null
			},
			{
				"boolean",
				JsonSchemaType.Boolean
			},
			{
				"array",
				JsonSchemaType.Array
			},
			{
				"any",
				JsonSchemaType.Any
			}
		};
	}
}
