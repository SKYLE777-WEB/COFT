using System;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000E8 RID: 232
	internal class LateBoundReflectionDelegateFactory : ReflectionDelegateFactory
	{
		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000C95 RID: 3221 RVA: 0x0004FF78 File Offset: 0x0004E178
		internal static ReflectionDelegateFactory Instance
		{
			get
			{
				return LateBoundReflectionDelegateFactory._instance;
			}
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x0004FF80 File Offset: 0x0004E180
		public override ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
		{
			ValidationUtils.ArgumentNotNull(method, "method");
			ConstructorInfo c = method as ConstructorInfo;
			if (c != null)
			{
				return (object[] a) => c.Invoke(a);
			}
			return (object[] a) => method.Invoke(null, a);
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x0004FFEC File Offset: 0x0004E1EC
		public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
		{
			ValidationUtils.ArgumentNotNull(method, "method");
			ConstructorInfo c = method as ConstructorInfo;
			if (c != null)
			{
				return (T o, object[] a) => c.Invoke(a);
			}
			return (T o, object[] a) => method.Invoke(o, a);
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x00050058 File Offset: 0x0004E258
		public override Func<T> CreateDefaultConstructor<T>(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			if (type.IsValueType())
			{
				return () => (T)((object)Activator.CreateInstance(type));
			}
			ConstructorInfo constructorInfo = ReflectionUtils.GetDefaultConstructor(type, true);
			return () => (T)((object)constructorInfo.Invoke(null));
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x000500C4 File Offset: 0x0004E2C4
		public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
			return (T o) => propertyInfo.GetValue(o, null);
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x000500F0 File Offset: 0x0004E2F0
		public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
			return (T o) => fieldInfo.GetValue(o);
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0005011C File Offset: 0x0004E31C
		public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");
			return delegate(T o, object v)
			{
				fieldInfo.SetValue(o, v);
			};
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x00050148 File Offset: 0x0004E348
		public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
			return delegate(T o, object v)
			{
				propertyInfo.SetValue(o, v, null);
			};
		}

		// Token: 0x04000521 RID: 1313
		private static readonly LateBoundReflectionDelegateFactory _instance = new LateBoundReflectionDelegateFactory();
	}
}
