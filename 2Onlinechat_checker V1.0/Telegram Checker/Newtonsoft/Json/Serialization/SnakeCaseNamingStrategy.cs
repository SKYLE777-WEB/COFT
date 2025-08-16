using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200012B RID: 299
	public class SnakeCaseNamingStrategy : NamingStrategy
	{
		// Token: 0x06000F64 RID: 3940 RVA: 0x0005DF88 File Offset: 0x0005C188
		public SnakeCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
		{
			base.ProcessDictionaryKeys = processDictionaryKeys;
			base.OverrideSpecifiedNames = overrideSpecifiedNames;
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x0005DFA0 File Offset: 0x0005C1A0
		public SnakeCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames, bool processExtensionDataNames)
			: this(processDictionaryKeys, overrideSpecifiedNames)
		{
			base.ProcessExtensionDataNames = processExtensionDataNames;
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0005DFB4 File Offset: 0x0005C1B4
		public SnakeCaseNamingStrategy()
		{
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0005DFBC File Offset: 0x0005C1BC
		protected override string ResolvePropertyName(string name)
		{
			return StringUtils.ToSnakeCase(name);
		}
	}
}
