using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000116 RID: 278
	public class JsonDynamicContract : JsonContainerContract
	{
		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06000E13 RID: 3603 RVA: 0x000565BC File Offset: 0x000547BC
		public JsonPropertyCollection Properties { get; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06000E14 RID: 3604 RVA: 0x000565C4 File Offset: 0x000547C4
		// (set) Token: 0x06000E15 RID: 3605 RVA: 0x000565CC File Offset: 0x000547CC
		public Func<string, string> PropertyNameResolver { get; set; }

		// Token: 0x06000E16 RID: 3606 RVA: 0x000565D8 File Offset: 0x000547D8
		private static CallSite<Func<CallSite, object, object>> CreateCallSiteGetter(string name)
		{
			return CallSite<Func<CallSite, object, object>>.Create(new NoThrowGetBinderMember((GetMemberBinder)DynamicUtils.BinderWrapper.GetMember(name, typeof(DynamicUtils))));
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x000565FC File Offset: 0x000547FC
		private static CallSite<Func<CallSite, object, object, object>> CreateCallSiteSetter(string name)
		{
			return CallSite<Func<CallSite, object, object, object>>.Create(new NoThrowSetBinderMember((SetMemberBinder)DynamicUtils.BinderWrapper.SetMember(name, typeof(DynamicUtils))));
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x00056620 File Offset: 0x00054820
		public JsonDynamicContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Dynamic;
			this.Properties = new JsonPropertyCollection(base.UnderlyingType);
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x00056680 File Offset: 0x00054880
		internal bool TryGetMember(IDynamicMetaObjectProvider dynamicProvider, string name, out object value)
		{
			ValidationUtils.ArgumentNotNull(dynamicProvider, "dynamicProvider");
			CallSite<Func<CallSite, object, object>> callSite = this._callSiteGetters.Get(name);
			object obj = callSite.Target(callSite, dynamicProvider);
			if (obj != NoThrowExpressionVisitor.ErrorResult)
			{
				value = obj;
				return true;
			}
			value = null;
			return false;
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x000566CC File Offset: 0x000548CC
		internal bool TrySetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value)
		{
			ValidationUtils.ArgumentNotNull(dynamicProvider, "dynamicProvider");
			CallSite<Func<CallSite, object, object, object>> callSite = this._callSiteSetters.Get(name);
			return callSite.Target(callSite, dynamicProvider, value) != NoThrowExpressionVisitor.ErrorResult;
		}

		// Token: 0x0400059E RID: 1438
		private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>> _callSiteGetters = new ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>>(new Func<string, CallSite<Func<CallSite, object, object>>>(JsonDynamicContract.CreateCallSiteGetter));

		// Token: 0x0400059F RID: 1439
		private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>> _callSiteSetters = new ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>>(new Func<string, CallSite<Func<CallSite, object, object, object>>>(JsonDynamicContract.CreateCallSiteSetter));
	}
}
