using System;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F8 RID: 248
	internal static class TypeExtensions
	{
		// Token: 0x06000D2E RID: 3374 RVA: 0x00052914 File Offset: 0x00050B14
		public static MethodInfo Method(this Delegate d)
		{
			return d.Method;
		}

		// Token: 0x06000D2F RID: 3375 RVA: 0x0005291C File Offset: 0x00050B1C
		public static MemberTypes MemberType(this MemberInfo memberInfo)
		{
			return memberInfo.MemberType;
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x00052924 File Offset: 0x00050B24
		public static bool ContainsGenericParameters(this Type type)
		{
			return type.ContainsGenericParameters;
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x0005292C File Offset: 0x00050B2C
		public static bool IsInterface(this Type type)
		{
			return type.IsInterface;
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x00052934 File Offset: 0x00050B34
		public static bool IsGenericType(this Type type)
		{
			return type.IsGenericType;
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x0005293C File Offset: 0x00050B3C
		public static bool IsGenericTypeDefinition(this Type type)
		{
			return type.IsGenericTypeDefinition;
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00052944 File Offset: 0x00050B44
		public static Type BaseType(this Type type)
		{
			return type.BaseType;
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x0005294C File Offset: 0x00050B4C
		public static Assembly Assembly(this Type type)
		{
			return type.Assembly;
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x00052954 File Offset: 0x00050B54
		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x0005295C File Offset: 0x00050B5C
		public static bool IsClass(this Type type)
		{
			return type.IsClass;
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x00052964 File Offset: 0x00050B64
		public static bool IsSealed(this Type type)
		{
			return type.IsSealed;
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x0005296C File Offset: 0x00050B6C
		public static bool IsAbstract(this Type type)
		{
			return type.IsAbstract;
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x00052974 File Offset: 0x00050B74
		public static bool IsVisible(this Type type)
		{
			return type.IsVisible;
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x0005297C File Offset: 0x00050B7C
		public static bool IsValueType(this Type type)
		{
			return type.IsValueType;
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x00052984 File Offset: 0x00050B84
		public static bool IsPrimitive(this Type type)
		{
			return type.IsPrimitive;
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x0005298C File Offset: 0x00050B8C
		public static bool AssignableToTypeName(this Type type, string fullTypeName, bool searchInterfaces, out Type match)
		{
			Type type2 = type;
			while (type2 != null)
			{
				if (string.Equals(type2.FullName, fullTypeName, StringComparison.Ordinal))
				{
					match = type2;
					return true;
				}
				type2 = type2.BaseType();
			}
			if (searchInterfaces)
			{
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					if (string.Equals(interfaces[i].Name, fullTypeName, StringComparison.Ordinal))
					{
						match = type;
						return true;
					}
				}
			}
			match = null;
			return false;
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x00052A0C File Offset: 0x00050C0C
		public static bool AssignableToTypeName(this Type type, string fullTypeName, bool searchInterfaces)
		{
			Type type2;
			return type.AssignableToTypeName(fullTypeName, searchInterfaces, out type2);
		}

		// Token: 0x06000D3F RID: 3391 RVA: 0x00052A28 File Offset: 0x00050C28
		public static bool ImplementInterface(this Type type, Type interfaceType)
		{
			Type type2 = type;
			while (type2 != null)
			{
				foreach (Type type3 in ((IEnumerable<Type>)type2.GetInterfaces()))
				{
					if (type3 == interfaceType || (type3 != null && type3.ImplementInterface(interfaceType)))
					{
						return true;
					}
				}
				type2 = type2.BaseType();
			}
			return false;
		}
	}
}
