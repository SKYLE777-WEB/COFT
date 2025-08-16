using System;
using System.Dynamic;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000DB RID: 219
	internal class NoThrowGetBinderMember : GetMemberBinder
	{
		// Token: 0x06000C3D RID: 3133 RVA: 0x0004DF1C File Offset: 0x0004C11C
		public NoThrowGetBinderMember(GetMemberBinder innerBinder)
			: base(innerBinder.Name, innerBinder.IgnoreCase)
		{
			this._innerBinder = innerBinder;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x0004DF38 File Offset: 0x0004C138
		public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			DynamicMetaObject dynamicMetaObject = this._innerBinder.Bind(target, CollectionUtils.ArrayEmpty<DynamicMetaObject>());
			return new DynamicMetaObject(new NoThrowExpressionVisitor().Visit(dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
		}

		// Token: 0x040004EB RID: 1259
		private readonly GetMemberBinder _innerBinder;
	}
}
