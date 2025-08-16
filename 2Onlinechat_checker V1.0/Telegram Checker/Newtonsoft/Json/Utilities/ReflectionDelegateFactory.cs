using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000EE RID: 238
	internal abstract class ReflectionDelegateFactory
	{
		// Token: 0x06000CBC RID: 3260 RVA: 0x0005090C File Offset: 0x0004EB0C
		public Func<T, object> CreateGet<T>(MemberInfo memberInfo)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return this.CreateGet<T>(propertyInfo);
			}
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null)
			{
				return this.CreateGet<T>(fieldInfo);
			}
			throw new Exception("Could not create getter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x00050968 File Offset: 0x0004EB68
		public Action<T, object> CreateSet<T>(MemberInfo memberInfo)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return this.CreateSet<T>(propertyInfo);
			}
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null)
			{
				return this.CreateSet<T>(fieldInfo);
			}
			throw new Exception("Could not create setter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
		}

		// Token: 0x06000CBE RID: 3262
		public abstract MethodCall<T, object> CreateMethodCall<T>(MethodBase method);

		// Token: 0x06000CBF RID: 3263
		public abstract ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method);

		// Token: 0x06000CC0 RID: 3264
		public abstract Func<T> CreateDefaultConstructor<T>(Type type);

		// Token: 0x06000CC1 RID: 3265
		public abstract Func<T, object> CreateGet<T>(PropertyInfo propertyInfo);

		// Token: 0x06000CC2 RID: 3266
		public abstract Func<T, object> CreateGet<T>(FieldInfo fieldInfo);

		// Token: 0x06000CC3 RID: 3267
		public abstract Action<T, object> CreateSet<T>(FieldInfo fieldInfo);

		// Token: 0x06000CC4 RID: 3268
		public abstract Action<T, object> CreateSet<T>(PropertyInfo propertyInfo);
	}
}
