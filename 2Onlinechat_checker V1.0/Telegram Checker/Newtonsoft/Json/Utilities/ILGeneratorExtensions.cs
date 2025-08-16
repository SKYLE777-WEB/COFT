using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000E3 RID: 227
	internal static class ILGeneratorExtensions
	{
		// Token: 0x06000C7A RID: 3194 RVA: 0x0004F1EC File Offset: 0x0004D3EC
		public static void PushInstance(this ILGenerator generator, Type type)
		{
			generator.Emit(OpCodes.Ldarg_0);
			if (type.IsValueType())
			{
				generator.Emit(OpCodes.Unbox, type);
				return;
			}
			generator.Emit(OpCodes.Castclass, type);
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x0004F220 File Offset: 0x0004D420
		public static void PushArrayInstance(this ILGenerator generator, int argsIndex, int arrayIndex)
		{
			generator.Emit(OpCodes.Ldarg, argsIndex);
			generator.Emit(OpCodes.Ldc_I4, arrayIndex);
			generator.Emit(OpCodes.Ldelem_Ref);
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x0004F248 File Offset: 0x0004D448
		public static void BoxIfNeeded(this ILGenerator generator, Type type)
		{
			if (type.IsValueType())
			{
				generator.Emit(OpCodes.Box, type);
				return;
			}
			generator.Emit(OpCodes.Castclass, type);
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x0004F270 File Offset: 0x0004D470
		public static void UnboxIfNeeded(this ILGenerator generator, Type type)
		{
			if (type.IsValueType())
			{
				generator.Emit(OpCodes.Unbox_Any, type);
				return;
			}
			generator.Emit(OpCodes.Castclass, type);
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x0004F298 File Offset: 0x0004D498
		public static void CallMethod(this ILGenerator generator, MethodInfo methodInfo)
		{
			if (methodInfo.IsFinal || !methodInfo.IsVirtual)
			{
				generator.Emit(OpCodes.Call, methodInfo);
				return;
			}
			generator.Emit(OpCodes.Callvirt, methodInfo);
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x0004F2CC File Offset: 0x0004D4CC
		public static void Return(this ILGenerator generator)
		{
			generator.Emit(OpCodes.Ret);
		}
	}
}
