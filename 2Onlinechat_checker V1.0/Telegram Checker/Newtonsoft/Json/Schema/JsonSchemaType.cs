using System;

namespace Newtonsoft.Json.Schema
{
	// Token: 0x02000139 RID: 313
	[Flags]
	[Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public enum JsonSchemaType
	{
		// Token: 0x04000671 RID: 1649
		None = 0,
		// Token: 0x04000672 RID: 1650
		String = 1,
		// Token: 0x04000673 RID: 1651
		Float = 2,
		// Token: 0x04000674 RID: 1652
		Integer = 4,
		// Token: 0x04000675 RID: 1653
		Boolean = 8,
		// Token: 0x04000676 RID: 1654
		Object = 16,
		// Token: 0x04000677 RID: 1655
		Array = 32,
		// Token: 0x04000678 RID: 1656
		Null = 64,
		// Token: 0x04000679 RID: 1657
		Any = 127
	}
}
