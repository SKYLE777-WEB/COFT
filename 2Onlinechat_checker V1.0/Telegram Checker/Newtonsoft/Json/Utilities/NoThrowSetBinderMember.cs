using System;
using System.Dynamic;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000DC RID: 220
	internal class NoThrowSetBinderMember : SetMemberBinder
	{
		// Token: 0x06000C3F RID: 3135 RVA: 0x0004DF78 File Offset: 0x0004C178
		public NoThrowSetBinderMember(SetMemberBinder innerBinder)
			: base(innerBinder.Name, innerBinder.IgnoreCase)
		{
			this._innerBinder = innerBinder;
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x0004DF94 File Offset: 0x0004C194
		public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
		{
			DynamicMetaObject dynamicMetaObject = this._innerBinder.Bind(target, new DynamicMetaObject[] { value });
			return new DynamicMetaObject(new NoThrowExpressionVisitor().Visit(dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
		}

		// Token: 0x040004EC RID: 1260
		private readonly SetMemberBinder _innerBinder;
	}
}
