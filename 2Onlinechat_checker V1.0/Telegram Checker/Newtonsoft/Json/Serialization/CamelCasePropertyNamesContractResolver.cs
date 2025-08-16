using System;
using System.Collections.Generic;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x020000FD RID: 253
	public class CamelCasePropertyNamesContractResolver : DefaultContractResolver
	{
		// Token: 0x06000D4B RID: 3403 RVA: 0x00052BA4 File Offset: 0x00050DA4
		public CamelCasePropertyNamesContractResolver()
		{
			base.NamingStrategy = new CamelCaseNamingStrategy
			{
				ProcessDictionaryKeys = true,
				OverrideSpecifiedNames = true
			};
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x00052BD4 File Offset: 0x00050DD4
		public override JsonContract ResolveContract(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ResolverContractKey resolverContractKey = new ResolverContractKey(base.GetType(), type);
			Dictionary<ResolverContractKey, JsonContract> dictionary = CamelCasePropertyNamesContractResolver._contractCache;
			JsonContract jsonContract;
			if (dictionary == null || !dictionary.TryGetValue(resolverContractKey, out jsonContract))
			{
				jsonContract = this.CreateContract(type);
				object typeContractCacheLock = CamelCasePropertyNamesContractResolver.TypeContractCacheLock;
				lock (typeContractCacheLock)
				{
					dictionary = CamelCasePropertyNamesContractResolver._contractCache;
					Dictionary<ResolverContractKey, JsonContract> dictionary2 = ((dictionary != null) ? new Dictionary<ResolverContractKey, JsonContract>(dictionary) : new Dictionary<ResolverContractKey, JsonContract>());
					dictionary2[resolverContractKey] = jsonContract;
					CamelCasePropertyNamesContractResolver._contractCache = dictionary2;
				}
			}
			return jsonContract;
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x00052C88 File Offset: 0x00050E88
		internal override PropertyNameTable GetNameTable()
		{
			return CamelCasePropertyNamesContractResolver.NameTable;
		}

		// Token: 0x0400053E RID: 1342
		private static readonly object TypeContractCacheLock = new object();

		// Token: 0x0400053F RID: 1343
		private static readonly PropertyNameTable NameTable = new PropertyNameTable();

		// Token: 0x04000540 RID: 1344
		private static Dictionary<ResolverContractKey, JsonContract> _contractCache;
	}
}
