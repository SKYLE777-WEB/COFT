using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000E0 RID: 224
	internal class ExpressionReflectionDelegateFactory : ReflectionDelegateFactory
	{
		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x0004E624 File Offset: 0x0004C824
		internal static ReflectionDelegateFactory Instance
		{
			get
			{
				return ExpressionReflectionDelegateFactory._instance;
			}
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x0004E62C File Offset: 0x0004C82C
		public override ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
		{
			ValidationUtils.ArgumentNotNull(method, "method");
			Type typeFromHandle = typeof(object);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object[]), "args");
			Expression expression = this.BuildMethodCall(method, typeFromHandle, null, parameterExpression);
			return (ObjectConstructor<object>)Expression.Lambda(typeof(ObjectConstructor<object>), expression, new ParameterExpression[] { parameterExpression }).Compile();
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x0004E698 File Offset: 0x0004C898
		public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
		{
			ValidationUtils.ArgumentNotNull(method, "method");
			Type typeFromHandle = typeof(object);
			ParameterExpression parameterExpression = Expression.Parameter(typeFromHandle, "target");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object[]), "args");
			Expression expression = this.BuildMethodCall(method, typeFromHandle, parameterExpression, parameterExpression2);
			return (MethodCall<T, object>)Expression.Lambda(typeof(MethodCall<T, object>), expression, new ParameterExpression[] { parameterExpression, parameterExpression2 }).Compile();
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x0004E714 File Offset: 0x0004C914
		private Expression BuildMethodCall(MethodBase method, Type type, ParameterExpression targetParameterExpression, ParameterExpression argsParameterExpression)
		{
			ParameterInfo[] parameters = method.GetParameters();
			Expression[] array;
			IList<ExpressionReflectionDelegateFactory.ByRefParameter> list;
			if (parameters.Length == 0)
			{
				array = CollectionUtils.ArrayEmpty<Expression>();
				list = CollectionUtils.ArrayEmpty<ExpressionReflectionDelegateFactory.ByRefParameter>();
			}
			else
			{
				array = new Expression[parameters.Length];
				list = new List<ExpressionReflectionDelegateFactory.ByRefParameter>();
				for (int i = 0; i < parameters.Length; i++)
				{
					ParameterInfo parameterInfo = parameters[i];
					Type type2 = parameterInfo.ParameterType;
					bool flag = false;
					if (type2.IsByRef)
					{
						type2 = type2.GetElementType();
						flag = true;
					}
					Expression expression = Expression.Constant(i);
					Expression expression2 = Expression.ArrayIndex(argsParameterExpression, expression);
					Expression expression3 = this.EnsureCastExpression(expression2, type2, !flag);
					if (flag)
					{
						ParameterExpression parameterExpression = Expression.Variable(type2);
						list.Add(new ExpressionReflectionDelegateFactory.ByRefParameter
						{
							Value = expression3,
							Variable = parameterExpression,
							IsOut = parameterInfo.IsOut
						});
						expression3 = parameterExpression;
					}
					array[i] = expression3;
				}
			}
			Expression expression4;
			if (method.IsConstructor)
			{
				expression4 = Expression.New((ConstructorInfo)method, array);
			}
			else if (method.IsStatic)
			{
				expression4 = Expression.Call((MethodInfo)method, array);
			}
			else
			{
				expression4 = Expression.Call(this.EnsureCastExpression(targetParameterExpression, method.DeclaringType, false), (MethodInfo)method, array);
			}
			MethodInfo methodInfo = method as MethodInfo;
			if (methodInfo != null)
			{
				if (methodInfo.ReturnType != typeof(void))
				{
					expression4 = this.EnsureCastExpression(expression4, type, false);
				}
				else
				{
					expression4 = Expression.Block(expression4, Expression.Constant(null));
				}
			}
			else
			{
				expression4 = this.EnsureCastExpression(expression4, type, false);
			}
			if (list.Count > 0)
			{
				IList<ParameterExpression> list2 = new List<ParameterExpression>();
				IList<Expression> list3 = new List<Expression>();
				foreach (ExpressionReflectionDelegateFactory.ByRefParameter byRefParameter in list)
				{
					if (!byRefParameter.IsOut)
					{
						list3.Add(Expression.Assign(byRefParameter.Variable, byRefParameter.Value));
					}
					list2.Add(byRefParameter.Variable);
				}
				list3.Add(expression4);
				expression4 = Expression.Block(list2, list3);
			}
			return expression4;
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x0004E960 File Offset: 0x0004CB60
		public override Func<T> CreateDefaultConstructor<T>(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsAbstract())
			{
				return () => (T)((object)Activator.CreateInstance(type));
			}
			Func<T> func;
			try
			{
				Type typeFromHandle = typeof(T);
				Expression expression = Expression.New(type);
				expression = this.EnsureCastExpression(expression, typeFromHandle, false);
				func = (Func<T>)Expression.Lambda(typeof(Func<T>), expression, new ParameterExpression[0]).Compile();
			}
			catch
			{
				func = () => (T)((object)Activator.CreateInstance(type));
			}
			return func;
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x0004EA14 File Offset: 0x0004CC14
		public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
			Type typeFromHandle = typeof(T);
			Type typeFromHandle2 = typeof(object);
			ParameterExpression parameterExpression = Expression.Parameter(typeFromHandle, "instance");
			Expression expression;
			if (propertyInfo.GetGetMethod(true).IsStatic)
			{
				expression = Expression.MakeMemberAccess(null, propertyInfo);
			}
			else
			{
				expression = Expression.MakeMemberAccess(this.EnsureCastExpression(parameterExpression, propertyInfo.DeclaringType, false), propertyInfo);
			}
			expression = this.EnsureCastExpression(expression, typeFromHandle2, false);
			return (Func<T, object>)Expression.Lambda(typeof(Func<T, object>), expression, new ParameterExpression[] { parameterExpression }).Compile();
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x0004EAB4 File Offset: 0x0004CCB4
		public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "source");
			Expression expression;
			if (fieldInfo.IsStatic)
			{
				expression = Expression.Field(null, fieldInfo);
			}
			else
			{
				expression = Expression.Field(this.EnsureCastExpression(parameterExpression, fieldInfo.DeclaringType, false), fieldInfo);
			}
			expression = this.EnsureCastExpression(expression, typeof(object), false);
			return Expression.Lambda<Func<T, object>>(expression, new ParameterExpression[] { parameterExpression }).Compile();
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x0004EB3C File Offset: 0x0004CD3C
		public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
			if (fieldInfo.DeclaringType.IsValueType() || fieldInfo.IsInitOnly)
			{
				return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(fieldInfo);
			}
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "source");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "value");
			Expression expression;
			if (fieldInfo.IsStatic)
			{
				expression = Expression.Field(null, fieldInfo);
			}
			else
			{
				expression = Expression.Field(this.EnsureCastExpression(parameterExpression, fieldInfo.DeclaringType, false), fieldInfo);
			}
			Expression expression2 = this.EnsureCastExpression(parameterExpression2, expression.Type, false);
			BinaryExpression binaryExpression = Expression.Assign(expression, expression2);
			return (Action<T, object>)Expression.Lambda(typeof(Action<T, object>), binaryExpression, new ParameterExpression[] { parameterExpression, parameterExpression2 }).Compile();
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x0004EC18 File Offset: 0x0004CE18
		public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
			if (propertyInfo.DeclaringType.IsValueType())
			{
				return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(propertyInfo);
			}
			Type typeFromHandle = typeof(T);
			Type typeFromHandle2 = typeof(object);
			ParameterExpression parameterExpression = Expression.Parameter(typeFromHandle, "instance");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeFromHandle2, "value");
			Expression expression = this.EnsureCastExpression(parameterExpression2, propertyInfo.PropertyType, false);
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			Expression expression2;
			if (setMethod.IsStatic)
			{
				expression2 = Expression.Call(setMethod, expression);
			}
			else
			{
				expression2 = Expression.Call(this.EnsureCastExpression(parameterExpression, propertyInfo.DeclaringType, false), setMethod, new Expression[] { expression });
			}
			return (Action<T, object>)Expression.Lambda(typeof(Action<T, object>), expression2, new ParameterExpression[] { parameterExpression, parameterExpression2 }).Compile();
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x0004ECFC File Offset: 0x0004CEFC
		private Expression EnsureCastExpression(Expression expression, Type targetType, bool allowWidening = false)
		{
			Type type = expression.Type;
			if (type == targetType || (!type.IsValueType() && targetType.IsAssignableFrom(type)))
			{
				return expression;
			}
			if (targetType.IsValueType())
			{
				Expression expression2 = Expression.Unbox(expression, targetType);
				if (allowWidening && targetType.IsPrimitive())
				{
					MethodInfo method = typeof(Convert).GetMethod("To" + targetType.Name, new Type[] { typeof(object) });
					if (method != null)
					{
						expression2 = Expression.Condition(Expression.TypeIs(expression, targetType), expression2, Expression.Call(method, expression));
					}
				}
				return Expression.Condition(Expression.Equal(expression, Expression.Constant(null, typeof(object))), Expression.Default(targetType), expression2);
			}
			return Expression.Convert(expression, targetType);
		}

		// Token: 0x040004F1 RID: 1265
		private static readonly ExpressionReflectionDelegateFactory _instance = new ExpressionReflectionDelegateFactory();

		// Token: 0x0200024F RID: 591
		private class ByRefParameter
		{
			// Token: 0x04000ADC RID: 2780
			public Expression Value;

			// Token: 0x04000ADD RID: 2781
			public ParameterExpression Variable;

			// Token: 0x04000ADE RID: 2782
			public bool IsOut;
		}
	}
}
