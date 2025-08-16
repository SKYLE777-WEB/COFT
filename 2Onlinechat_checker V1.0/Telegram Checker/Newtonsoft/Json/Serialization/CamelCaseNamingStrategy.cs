using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x020000FB RID: 251
	public class CamelCaseNamingStrategy : NamingStrategy
	{
		// Token: 0x06000D43 RID: 3395 RVA: 0x00052AF4 File Offset: 0x00050CF4
		public CamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
		{
			base.ProcessDictionaryKeys = processDictionaryKeys;
			base.OverrideSpecifiedNames = overrideSpecifiedNames;
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x00052B0C File Offset: 0x00050D0C
		public CamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames, bool processExtensionDataNames)
			: this(processDictionaryKeys, overrideSpecifiedNames)
		{
			base.ProcessExtensionDataNames = processExtensionDataNames;
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x00052B20 File Offset: 0x00050D20
		public CamelCaseNamingStrategy()
		{
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x00052B28 File Offset: 0x00050D28
		protected override string ResolvePropertyName(string name)
		{
			return StringUtils.ToCamelCase(name);
		}
	}
}
