using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x020000FC RID: 252
	internal struct ResolverContractKey : IEquatable<ResolverContractKey>
	{
		// Token: 0x06000D47 RID: 3399 RVA: 0x00052B30 File Offset: 0x00050D30
		public ResolverContractKey(Type resolverType, Type contractType)
		{
			this._resolverType = resolverType;
			this._contractType = contractType;
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x00052B40 File Offset: 0x00050D40
		public override int GetHashCode()
		{
			return this._resolverType.GetHashCode() ^ this._contractType.GetHashCode();
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x00052B5C File Offset: 0x00050D5C
		public override bool Equals(object obj)
		{
			return obj is ResolverContractKey && this.Equals((ResolverContractKey)obj);
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x00052B78 File Offset: 0x00050D78
		public bool Equals(ResolverContractKey other)
		{
			return this._resolverType == other._resolverType && this._contractType == other._contractType;
		}

		// Token: 0x0400053C RID: 1340
		private readonly Type _resolverType;

		// Token: 0x0400053D RID: 1341
		private readonly Type _contractType;
	}
}
