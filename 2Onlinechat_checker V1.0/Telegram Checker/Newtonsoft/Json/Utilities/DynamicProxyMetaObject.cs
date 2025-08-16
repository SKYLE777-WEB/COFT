using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D8 RID: 216
	internal sealed class DynamicProxyMetaObject<T> : DynamicMetaObject
	{
		// Token: 0x06000C12 RID: 3090 RVA: 0x0004CB68 File Offset: 0x0004AD68
		internal DynamicProxyMetaObject(Expression expression, T value, DynamicProxy<T> proxy)
			: base(expression, BindingRestrictions.Empty, value)
		{
			this._proxy = proxy;
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0004CB84 File Offset: 0x0004AD84
		private bool IsOverridden(string method)
		{
			return ReflectionUtils.IsMethodOverridden(this._proxy.GetType(), typeof(DynamicProxy<T>), method);
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0004CBA4 File Offset: 0x0004ADA4
		public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			if (!this.IsOverridden("TryGetMember"))
			{
				return base.BindGetMember(binder);
			}
			return this.CallMethodWithResult("TryGetMember", binder, DynamicProxyMetaObject<T>.NoArgs, (DynamicMetaObject e) => binder.FallbackGetMember(this, e), null);
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0004CC0C File Offset: 0x0004AE0C
		public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			if (!this.IsOverridden("TrySetMember"))
			{
				return base.BindSetMember(binder, value);
			}
			return this.CallMethodReturnLast("TrySetMember", binder, DynamicProxyMetaObject<T>.GetArgs(new DynamicMetaObject[] { value }), (DynamicMetaObject e) => binder.FallbackSetMember(this, value, e));
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0004CC8C File Offset: 0x0004AE8C
		public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
		{
			if (!this.IsOverridden("TryDeleteMember"))
			{
				return base.BindDeleteMember(binder);
			}
			return this.CallMethodNoResult("TryDeleteMember", binder, DynamicProxyMetaObject<T>.NoArgs, (DynamicMetaObject e) => binder.FallbackDeleteMember(this, e));
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0004CCF0 File Offset: 0x0004AEF0
		public override DynamicMetaObject BindConvert(ConvertBinder binder)
		{
			if (!this.IsOverridden("TryConvert"))
			{
				return base.BindConvert(binder);
			}
			return this.CallMethodWithResult("TryConvert", binder, DynamicProxyMetaObject<T>.NoArgs, (DynamicMetaObject e) => binder.FallbackConvert(this, e), null);
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0004CD58 File Offset: 0x0004AF58
		public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			if (!this.IsOverridden("TryInvokeMember"))
			{
				return base.BindInvokeMember(binder, args);
			}
			DynamicProxyMetaObject<T>.Fallback fallback = (DynamicMetaObject e) => binder.FallbackInvokeMember(this, args, e);
			return this.BuildCallMethodWithResult("TryInvokeMember", binder, DynamicProxyMetaObject<T>.GetArgArray(args), this.BuildCallMethodWithResult("TryGetMember", new DynamicProxyMetaObject<T>.GetBinderAdapter(binder), DynamicProxyMetaObject<T>.NoArgs, fallback(null), (DynamicMetaObject e) => binder.FallbackInvoke(e, args, null)), null);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0004CE00 File Offset: 0x0004B000
		public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
		{
			if (!this.IsOverridden("TryCreateInstance"))
			{
				return base.BindCreateInstance(binder, args);
			}
			return this.CallMethodWithResult("TryCreateInstance", binder, DynamicProxyMetaObject<T>.GetArgArray(args), (DynamicMetaObject e) => binder.FallbackCreateInstance(this, args, e), null);
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0004CE78 File Offset: 0x0004B078
		public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
		{
			if (!this.IsOverridden("TryInvoke"))
			{
				return base.BindInvoke(binder, args);
			}
			return this.CallMethodWithResult("TryInvoke", binder, DynamicProxyMetaObject<T>.GetArgArray(args), (DynamicMetaObject e) => binder.FallbackInvoke(this, args, e), null);
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x0004CEF0 File Offset: 0x0004B0F0
		public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
		{
			if (!this.IsOverridden("TryBinaryOperation"))
			{
				return base.BindBinaryOperation(binder, arg);
			}
			return this.CallMethodWithResult("TryBinaryOperation", binder, DynamicProxyMetaObject<T>.GetArgs(new DynamicMetaObject[] { arg }), (DynamicMetaObject e) => binder.FallbackBinaryOperation(this, arg, e), null);
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x0004CF74 File Offset: 0x0004B174
		public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
		{
			if (!this.IsOverridden("TryUnaryOperation"))
			{
				return base.BindUnaryOperation(binder);
			}
			return this.CallMethodWithResult("TryUnaryOperation", binder, DynamicProxyMetaObject<T>.NoArgs, (DynamicMetaObject e) => binder.FallbackUnaryOperation(this, e), null);
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0004CFDC File Offset: 0x0004B1DC
		public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
		{
			if (!this.IsOverridden("TryGetIndex"))
			{
				return base.BindGetIndex(binder, indexes);
			}
			return this.CallMethodWithResult("TryGetIndex", binder, DynamicProxyMetaObject<T>.GetArgArray(indexes), (DynamicMetaObject e) => binder.FallbackGetIndex(this, indexes, e), null);
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0004D054 File Offset: 0x0004B254
		public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
		{
			if (!this.IsOverridden("TrySetIndex"))
			{
				return base.BindSetIndex(binder, indexes, value);
			}
			return this.CallMethodReturnLast("TrySetIndex", binder, DynamicProxyMetaObject<T>.GetArgArray(indexes, value), (DynamicMetaObject e) => binder.FallbackSetIndex(this, indexes, value, e));
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x0004D0E0 File Offset: 0x0004B2E0
		public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
		{
			if (!this.IsOverridden("TryDeleteIndex"))
			{
				return base.BindDeleteIndex(binder, indexes);
			}
			return this.CallMethodNoResult("TryDeleteIndex", binder, DynamicProxyMetaObject<T>.GetArgArray(indexes), (DynamicMetaObject e) => binder.FallbackDeleteIndex(this, indexes, e));
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000C20 RID: 3104 RVA: 0x0004D158 File Offset: 0x0004B358
		private static Expression[] NoArgs
		{
			get
			{
				return CollectionUtils.ArrayEmpty<Expression>();
			}
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0004D160 File Offset: 0x0004B360
		private static IEnumerable<Expression> GetArgs(params DynamicMetaObject[] args)
		{
			return args.Select(delegate(DynamicMetaObject arg)
			{
				Expression expression = arg.Expression;
				if (!expression.Type.IsValueType())
				{
					return expression;
				}
				return Expression.Convert(expression, typeof(object));
			});
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x0004D18C File Offset: 0x0004B38C
		private static Expression[] GetArgArray(DynamicMetaObject[] args)
		{
			return new NewArrayExpression[] { Expression.NewArrayInit(typeof(object), DynamicProxyMetaObject<T>.GetArgs(args)) };
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x0004D1AC File Offset: 0x0004B3AC
		private static Expression[] GetArgArray(DynamicMetaObject[] args, DynamicMetaObject value)
		{
			Expression expression = value.Expression;
			return new Expression[]
			{
				Expression.NewArrayInit(typeof(object), DynamicProxyMetaObject<T>.GetArgs(args)),
				expression.Type.IsValueType() ? Expression.Convert(expression, typeof(object)) : expression
			};
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x0004D20C File Offset: 0x0004B40C
		private static ConstantExpression Constant(DynamicMetaObjectBinder binder)
		{
			Type type = binder.GetType();
			while (!type.IsVisible())
			{
				type = type.BaseType();
			}
			return Expression.Constant(binder, type);
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x0004D240 File Offset: 0x0004B440
		private DynamicMetaObject CallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, IEnumerable<Expression> args, DynamicProxyMetaObject<T>.Fallback fallback, DynamicProxyMetaObject<T>.Fallback fallbackInvoke = null)
		{
			DynamicMetaObject dynamicMetaObject = fallback(null);
			return this.BuildCallMethodWithResult(methodName, binder, args, dynamicMetaObject, fallbackInvoke);
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x0004D268 File Offset: 0x0004B468
		private DynamicMetaObject BuildCallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, IEnumerable<Expression> args, DynamicMetaObject fallbackResult, DynamicProxyMetaObject<T>.Fallback fallbackInvoke)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), null);
			IList<Expression> list = new List<Expression>();
			list.Add(Expression.Convert(base.Expression, typeof(T)));
			list.Add(DynamicProxyMetaObject<T>.Constant(binder));
			list.AddRange(args);
			list.Add(parameterExpression);
			DynamicMetaObject dynamicMetaObject = new DynamicMetaObject(parameterExpression, BindingRestrictions.Empty);
			if (binder.ReturnType != typeof(object))
			{
				dynamicMetaObject = new DynamicMetaObject(Expression.Convert(dynamicMetaObject.Expression, binder.ReturnType), dynamicMetaObject.Restrictions);
			}
			if (fallbackInvoke != null)
			{
				dynamicMetaObject = fallbackInvoke(dynamicMetaObject);
			}
			return new DynamicMetaObject(Expression.Block(new ParameterExpression[] { parameterExpression }, new Expression[] { Expression.Condition(Expression.Call(Expression.Constant(this._proxy), typeof(DynamicProxy<T>).GetMethod(methodName), list), dynamicMetaObject.Expression, fallbackResult.Expression, binder.ReturnType) }), this.GetRestrictions().Merge(dynamicMetaObject.Restrictions).Merge(fallbackResult.Restrictions));
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x0004D38C File Offset: 0x0004B58C
		private DynamicMetaObject CallMethodReturnLast(string methodName, DynamicMetaObjectBinder binder, IEnumerable<Expression> args, DynamicProxyMetaObject<T>.Fallback fallback)
		{
			DynamicMetaObject dynamicMetaObject = fallback(null);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), null);
			IList<Expression> list = new List<Expression>();
			list.Add(Expression.Convert(base.Expression, typeof(T)));
			list.Add(DynamicProxyMetaObject<T>.Constant(binder));
			list.AddRange(args);
			list[list.Count - 1] = Expression.Assign(parameterExpression, list[list.Count - 1]);
			return new DynamicMetaObject(Expression.Block(new ParameterExpression[] { parameterExpression }, new Expression[] { Expression.Condition(Expression.Call(Expression.Constant(this._proxy), typeof(DynamicProxy<T>).GetMethod(methodName), list), parameterExpression, dynamicMetaObject.Expression, typeof(object)) }), this.GetRestrictions().Merge(dynamicMetaObject.Restrictions));
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x0004D474 File Offset: 0x0004B674
		private DynamicMetaObject CallMethodNoResult(string methodName, DynamicMetaObjectBinder binder, Expression[] args, DynamicProxyMetaObject<T>.Fallback fallback)
		{
			DynamicMetaObject dynamicMetaObject = fallback(null);
			IList<Expression> list = new List<Expression>();
			list.Add(Expression.Convert(base.Expression, typeof(T)));
			list.Add(DynamicProxyMetaObject<T>.Constant(binder));
			list.AddRange(args);
			return new DynamicMetaObject(Expression.Condition(Expression.Call(Expression.Constant(this._proxy), typeof(DynamicProxy<T>).GetMethod(methodName), list), Expression.Empty(), dynamicMetaObject.Expression, typeof(void)), this.GetRestrictions().Merge(dynamicMetaObject.Restrictions));
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x0004D514 File Offset: 0x0004B714
		private BindingRestrictions GetRestrictions()
		{
			if (base.Value != null || !base.HasValue)
			{
				return BindingRestrictions.GetTypeRestriction(base.Expression, base.LimitType);
			}
			return BindingRestrictions.GetInstanceRestriction(base.Expression, null);
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x0004D55C File Offset: 0x0004B75C
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return this._proxy.GetDynamicMemberNames((T)((object)base.Value));
		}

		// Token: 0x040004E9 RID: 1257
		private readonly DynamicProxy<T> _proxy;

		// Token: 0x0200023C RID: 572
		// (Invoke) Token: 0x0600169F RID: 5791
		private delegate DynamicMetaObject Fallback(DynamicMetaObject errorSuggestion);

		// Token: 0x0200023D RID: 573
		private sealed class GetBinderAdapter : GetMemberBinder
		{
			// Token: 0x060016A2 RID: 5794 RVA: 0x0008234C File Offset: 0x0008054C
			internal GetBinderAdapter(InvokeMemberBinder binder)
				: base(binder.Name, binder.IgnoreCase)
			{
			}

			// Token: 0x060016A3 RID: 5795 RVA: 0x00082360 File Offset: 0x00080560
			public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
			{
				throw new NotSupportedException();
			}
		}
	}
}
