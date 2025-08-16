using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000E2 RID: 226
	internal static class FSharpUtils
	{
		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000C5F RID: 3167 RVA: 0x0004EE1C File Offset: 0x0004D01C
		// (set) Token: 0x06000C60 RID: 3168 RVA: 0x0004EE24 File Offset: 0x0004D024
		public static Assembly FSharpCoreAssembly { get; private set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000C61 RID: 3169 RVA: 0x0004EE2C File Offset: 0x0004D02C
		// (set) Token: 0x06000C62 RID: 3170 RVA: 0x0004EE34 File Offset: 0x0004D034
		public static MethodCall<object, object> IsUnion { get; private set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000C63 RID: 3171 RVA: 0x0004EE3C File Offset: 0x0004D03C
		// (set) Token: 0x06000C64 RID: 3172 RVA: 0x0004EE44 File Offset: 0x0004D044
		public static MethodCall<object, object> GetUnionCases { get; private set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000C65 RID: 3173 RVA: 0x0004EE4C File Offset: 0x0004D04C
		// (set) Token: 0x06000C66 RID: 3174 RVA: 0x0004EE54 File Offset: 0x0004D054
		public static MethodCall<object, object> PreComputeUnionTagReader { get; private set; }

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000C67 RID: 3175 RVA: 0x0004EE5C File Offset: 0x0004D05C
		// (set) Token: 0x06000C68 RID: 3176 RVA: 0x0004EE64 File Offset: 0x0004D064
		public static MethodCall<object, object> PreComputeUnionReader { get; private set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000C69 RID: 3177 RVA: 0x0004EE6C File Offset: 0x0004D06C
		// (set) Token: 0x06000C6A RID: 3178 RVA: 0x0004EE74 File Offset: 0x0004D074
		public static MethodCall<object, object> PreComputeUnionConstructor { get; private set; }

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000C6B RID: 3179 RVA: 0x0004EE7C File Offset: 0x0004D07C
		// (set) Token: 0x06000C6C RID: 3180 RVA: 0x0004EE84 File Offset: 0x0004D084
		public static Func<object, object> GetUnionCaseInfoDeclaringType { get; private set; }

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000C6D RID: 3181 RVA: 0x0004EE8C File Offset: 0x0004D08C
		// (set) Token: 0x06000C6E RID: 3182 RVA: 0x0004EE94 File Offset: 0x0004D094
		public static Func<object, object> GetUnionCaseInfoName { get; private set; }

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000C6F RID: 3183 RVA: 0x0004EE9C File Offset: 0x0004D09C
		// (set) Token: 0x06000C70 RID: 3184 RVA: 0x0004EEA4 File Offset: 0x0004D0A4
		public static Func<object, object> GetUnionCaseInfoTag { get; private set; }

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000C71 RID: 3185 RVA: 0x0004EEAC File Offset: 0x0004D0AC
		// (set) Token: 0x06000C72 RID: 3186 RVA: 0x0004EEB4 File Offset: 0x0004D0B4
		public static MethodCall<object, object> GetUnionCaseInfoFields { get; private set; }

		// Token: 0x06000C73 RID: 3187 RVA: 0x0004EEBC File Offset: 0x0004D0BC
		public static void EnsureInitialized(Assembly fsharpCoreAssembly)
		{
			if (!FSharpUtils._initialized)
			{
				object @lock = FSharpUtils.Lock;
				lock (@lock)
				{
					if (!FSharpUtils._initialized)
					{
						FSharpUtils.FSharpCoreAssembly = fsharpCoreAssembly;
						Type type = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpType");
						MethodInfo methodWithNonPublicFallback = FSharpUtils.GetMethodWithNonPublicFallback(type, "IsUnion", BindingFlags.Static | BindingFlags.Public);
						FSharpUtils.IsUnion = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(methodWithNonPublicFallback);
						MethodInfo methodWithNonPublicFallback2 = FSharpUtils.GetMethodWithNonPublicFallback(type, "GetUnionCases", BindingFlags.Static | BindingFlags.Public);
						FSharpUtils.GetUnionCases = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(methodWithNonPublicFallback2);
						Type type2 = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpValue");
						FSharpUtils.PreComputeUnionTagReader = FSharpUtils.CreateFSharpFuncCall(type2, "PreComputeUnionTagReader");
						FSharpUtils.PreComputeUnionReader = FSharpUtils.CreateFSharpFuncCall(type2, "PreComputeUnionReader");
						FSharpUtils.PreComputeUnionConstructor = FSharpUtils.CreateFSharpFuncCall(type2, "PreComputeUnionConstructor");
						Type type3 = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.UnionCaseInfo");
						FSharpUtils.GetUnionCaseInfoName = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type3.GetProperty("Name"));
						FSharpUtils.GetUnionCaseInfoTag = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type3.GetProperty("Tag"));
						FSharpUtils.GetUnionCaseInfoDeclaringType = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type3.GetProperty("DeclaringType"));
						FSharpUtils.GetUnionCaseInfoFields = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(type3.GetMethod("GetFields"));
						FSharpUtils._ofSeq = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.ListModule").GetMethod("OfSeq");
						FSharpUtils._mapType = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.FSharpMap`2");
						Thread.MemoryBarrier();
						FSharpUtils._initialized = true;
					}
				}
			}
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x0004F050 File Offset: 0x0004D250
		private static MethodInfo GetMethodWithNonPublicFallback(Type type, string methodName, BindingFlags bindingFlags)
		{
			MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);
			if (methodInfo == null && (bindingFlags & BindingFlags.NonPublic) != BindingFlags.NonPublic)
			{
				methodInfo = type.GetMethod(methodName, bindingFlags | BindingFlags.NonPublic);
			}
			return methodInfo;
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x0004F090 File Offset: 0x0004D290
		private static MethodCall<object, object> CreateFSharpFuncCall(Type type, string methodName)
		{
			MethodInfo methodWithNonPublicFallback = FSharpUtils.GetMethodWithNonPublicFallback(type, methodName, BindingFlags.Static | BindingFlags.Public);
			MethodInfo method = methodWithNonPublicFallback.ReturnType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
			MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(methodWithNonPublicFallback);
			MethodCall<object, object> invoke = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object target, object[] args) => new FSharpFunction(call(target, args), invoke);
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x0004F0F0 File Offset: 0x0004D2F0
		public static ObjectConstructor<object> CreateSeq(Type t)
		{
			MethodInfo methodInfo = FSharpUtils._ofSeq.MakeGenericMethod(new Type[] { t });
			return JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(methodInfo);
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x0004F124 File Offset: 0x0004D324
		public static ObjectConstructor<object> CreateMap(Type keyType, Type valueType)
		{
			return (ObjectConstructor<object>)typeof(FSharpUtils).GetMethod("BuildMapCreator").MakeGenericMethod(new Type[] { keyType, valueType }).Invoke(null, null);
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x0004F168 File Offset: 0x0004D368
		public static ObjectConstructor<object> BuildMapCreator<TKey, TValue>()
		{
			ConstructorInfo constructor = FSharpUtils._mapType.MakeGenericType(new Type[]
			{
				typeof(TKey),
				typeof(TValue)
			}).GetConstructor(new Type[] { typeof(IEnumerable<Tuple<TKey, TValue>>) });
			ObjectConstructor<object> ctorDelegate = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
			return delegate(object[] args)
			{
				IEnumerable<Tuple<TKey, TValue>> enumerable = ((IEnumerable<KeyValuePair<TKey, TValue>>)args[0]).Select((KeyValuePair<TKey, TValue> kv) => new Tuple<TKey, TValue>(kv.Key, kv.Value));
				return ctorDelegate(new object[] { enumerable });
			};
		}

		// Token: 0x040004F4 RID: 1268
		private static readonly object Lock = new object();

		// Token: 0x040004F5 RID: 1269
		private static bool _initialized;

		// Token: 0x040004F6 RID: 1270
		private static MethodInfo _ofSeq;

		// Token: 0x040004F7 RID: 1271
		private static Type _mapType;

		// Token: 0x04000502 RID: 1282
		public const string FSharpSetTypeName = "FSharpSet`1";

		// Token: 0x04000503 RID: 1283
		public const string FSharpListTypeName = "FSharpList`1";

		// Token: 0x04000504 RID: 1284
		public const string FSharpMapTypeName = "FSharpMap`2";
	}
}
