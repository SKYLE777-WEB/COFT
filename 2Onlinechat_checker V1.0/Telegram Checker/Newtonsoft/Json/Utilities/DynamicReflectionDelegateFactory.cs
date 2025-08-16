using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D9 RID: 217
	internal class DynamicReflectionDelegateFactory : ReflectionDelegateFactory
	{
		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000C2B RID: 3115 RVA: 0x0004D574 File Offset: 0x0004B774
		internal static DynamicReflectionDelegateFactory Instance
		{
			get
			{
				return DynamicReflectionDelegateFactory._instance;
			}
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x0004D57C File Offset: 0x0004B77C
		private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
		{
			if (owner.IsInterface())
			{
				return new DynamicMethod(name, returnType, parameterTypes, owner.Module, true);
			}
			return new DynamicMethod(name, returnType, parameterTypes, owner, true);
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x0004D5A4 File Offset: 0x0004B7A4
		public override ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
		{
			DynamicMethod dynamicMethod = DynamicReflectionDelegateFactory.CreateDynamicMethod(method.ToString(), typeof(object), new Type[] { typeof(object[]) }, method.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			this.GenerateCreateMethodCallIL(method, ilgenerator, 0);
			return (ObjectConstructor<object>)dynamicMethod.CreateDelegate(typeof(ObjectConstructor<object>));
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x0004D608 File Offset: 0x0004B808
		public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
		{
			DynamicMethod dynamicMethod = DynamicReflectionDelegateFactory.CreateDynamicMethod(method.ToString(), typeof(object), new Type[]
			{
				typeof(object),
				typeof(object[])
			}, method.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			this.GenerateCreateMethodCallIL(method, ilgenerator, 1);
			return (MethodCall<T, object>)dynamicMethod.CreateDelegate(typeof(MethodCall<T, object>));
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x0004D678 File Offset: 0x0004B878
		private void GenerateCreateMethodCallIL(MethodBase method, ILGenerator generator, int argsIndex)
		{
			ParameterInfo[] parameters = method.GetParameters();
			Label label = generator.DefineLabel();
			generator.Emit(OpCodes.Ldarg, argsIndex);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Ldc_I4, parameters.Length);
			generator.Emit(OpCodes.Beq, label);
			generator.Emit(OpCodes.Newobj, typeof(TargetParameterCountException).GetConstructor(ReflectionUtils.EmptyTypes));
			generator.Emit(OpCodes.Throw);
			generator.MarkLabel(label);
			if (!method.IsConstructor && !method.IsStatic)
			{
				generator.PushInstance(method.DeclaringType);
			}
			LocalBuilder localBuilder = generator.DeclareLocal(typeof(IConvertible));
			LocalBuilder localBuilder2 = generator.DeclareLocal(typeof(object));
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				Type type = parameterInfo.ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
					LocalBuilder localBuilder3 = generator.DeclareLocal(type);
					if (!parameterInfo.IsOut)
					{
						generator.PushArrayInstance(argsIndex, i);
						if (type.IsValueType())
						{
							Label label2 = generator.DefineLabel();
							Label label3 = generator.DefineLabel();
							generator.Emit(OpCodes.Brtrue_S, label2);
							generator.Emit(OpCodes.Ldloca_S, localBuilder3);
							generator.Emit(OpCodes.Initobj, type);
							generator.Emit(OpCodes.Br_S, label3);
							generator.MarkLabel(label2);
							generator.PushArrayInstance(argsIndex, i);
							generator.UnboxIfNeeded(type);
							generator.Emit(OpCodes.Stloc_S, localBuilder3);
							generator.MarkLabel(label3);
						}
						else
						{
							generator.UnboxIfNeeded(type);
							generator.Emit(OpCodes.Stloc_S, localBuilder3);
						}
					}
					generator.Emit(OpCodes.Ldloca_S, localBuilder3);
				}
				else if (type.IsValueType())
				{
					generator.PushArrayInstance(argsIndex, i);
					generator.Emit(OpCodes.Stloc_S, localBuilder2);
					Label label4 = generator.DefineLabel();
					Label label5 = generator.DefineLabel();
					generator.Emit(OpCodes.Ldloc_S, localBuilder2);
					generator.Emit(OpCodes.Brtrue_S, label4);
					LocalBuilder localBuilder4 = generator.DeclareLocal(type);
					generator.Emit(OpCodes.Ldloca_S, localBuilder4);
					generator.Emit(OpCodes.Initobj, type);
					generator.Emit(OpCodes.Ldloc_S, localBuilder4);
					generator.Emit(OpCodes.Br_S, label5);
					generator.MarkLabel(label4);
					if (type.IsPrimitive())
					{
						MethodInfo method2 = typeof(IConvertible).GetMethod("To" + type.Name, new Type[] { typeof(IFormatProvider) });
						if (method2 != null)
						{
							Label label6 = generator.DefineLabel();
							generator.Emit(OpCodes.Ldloc_S, localBuilder2);
							generator.Emit(OpCodes.Isinst, type);
							generator.Emit(OpCodes.Brtrue_S, label6);
							generator.Emit(OpCodes.Ldloc_S, localBuilder2);
							generator.Emit(OpCodes.Isinst, typeof(IConvertible));
							generator.Emit(OpCodes.Stloc_S, localBuilder);
							generator.Emit(OpCodes.Ldloc_S, localBuilder);
							generator.Emit(OpCodes.Brfalse_S, label6);
							generator.Emit(OpCodes.Ldloc_S, localBuilder);
							generator.Emit(OpCodes.Ldnull);
							generator.Emit(OpCodes.Callvirt, method2);
							generator.Emit(OpCodes.Br_S, label5);
							generator.MarkLabel(label6);
						}
					}
					generator.Emit(OpCodes.Ldloc_S, localBuilder2);
					generator.UnboxIfNeeded(type);
					generator.MarkLabel(label5);
				}
				else
				{
					generator.PushArrayInstance(argsIndex, i);
					generator.UnboxIfNeeded(type);
				}
			}
			if (method.IsConstructor)
			{
				generator.Emit(OpCodes.Newobj, (ConstructorInfo)method);
			}
			else
			{
				generator.CallMethod((MethodInfo)method);
			}
			Type type2 = (method.IsConstructor ? method.DeclaringType : ((MethodInfo)method).ReturnType);
			if (type2 != typeof(void))
			{
				generator.BoxIfNeeded(type2);
			}
			else
			{
				generator.Emit(OpCodes.Ldnull);
			}
			generator.Return();
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x0004DA88 File Offset: 0x0004BC88
		public override Func<T> CreateDefaultConstructor<T>(Type type)
		{
			DynamicMethod dynamicMethod = DynamicReflectionDelegateFactory.CreateDynamicMethod("Create" + type.FullName, typeof(T), ReflectionUtils.EmptyTypes, type);
			dynamicMethod.InitLocals = true;
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			this.GenerateCreateDefaultConstructorIL(type, ilgenerator, typeof(T));
			return (Func<T>)dynamicMethod.CreateDelegate(typeof(Func<T>));
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0004DAF4 File Offset: 0x0004BCF4
		private void GenerateCreateDefaultConstructorIL(Type type, ILGenerator generator, Type delegateType)
		{
			if (type.IsValueType())
			{
				generator.DeclareLocal(type);
				generator.Emit(OpCodes.Ldloc_0);
				if (type != delegateType)
				{
					generator.Emit(OpCodes.Box, type);
				}
			}
			else
			{
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ReflectionUtils.EmptyTypes, null);
				if (constructor == null)
				{
					throw new ArgumentException("Could not get constructor for {0}.".FormatWith(CultureInfo.InvariantCulture, type));
				}
				generator.Emit(OpCodes.Newobj, constructor);
			}
			generator.Return();
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x0004DB84 File Offset: 0x0004BD84
		public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
		{
			DynamicMethod dynamicMethod = DynamicReflectionDelegateFactory.CreateDynamicMethod("Get" + propertyInfo.Name, typeof(object), new Type[] { typeof(T) }, propertyInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			this.GenerateCreateGetPropertyIL(propertyInfo, ilgenerator);
			return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0004DBF0 File Offset: 0x0004BDF0
		private void GenerateCreateGetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException("Property '{0}' does not have a getter.".FormatWith(CultureInfo.InvariantCulture, propertyInfo.Name));
			}
			if (!getMethod.IsStatic)
			{
				generator.PushInstance(propertyInfo.DeclaringType);
			}
			generator.CallMethod(getMethod);
			generator.BoxIfNeeded(propertyInfo.PropertyType);
			generator.Return();
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x0004DC60 File Offset: 0x0004BE60
		public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
		{
			if (fieldInfo.IsLiteral)
			{
				object constantValue = fieldInfo.GetValue(null);
				return (T o) => constantValue;
			}
			DynamicMethod dynamicMethod = DynamicReflectionDelegateFactory.CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), new Type[] { typeof(object) }, fieldInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			this.GenerateCreateGetFieldIL(fieldInfo, ilgenerator);
			return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x0004DCF8 File Offset: 0x0004BEF8
		private void GenerateCreateGetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
		{
			if (!fieldInfo.IsStatic)
			{
				generator.PushInstance(fieldInfo.DeclaringType);
				generator.Emit(OpCodes.Ldfld, fieldInfo);
			}
			else
			{
				generator.Emit(OpCodes.Ldsfld, fieldInfo);
			}
			generator.BoxIfNeeded(fieldInfo.FieldType);
			generator.Return();
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0004DD50 File Offset: 0x0004BF50
		public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
		{
			DynamicMethod dynamicMethod = DynamicReflectionDelegateFactory.CreateDynamicMethod("Set" + fieldInfo.Name, null, new Type[]
			{
				typeof(T),
				typeof(object)
			}, fieldInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			DynamicReflectionDelegateFactory.GenerateCreateSetFieldIL(fieldInfo, ilgenerator);
			return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x0004DDC0 File Offset: 0x0004BFC0
		internal static void GenerateCreateSetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
		{
			if (!fieldInfo.IsStatic)
			{
				generator.PushInstance(fieldInfo.DeclaringType);
			}
			generator.Emit(OpCodes.Ldarg_1);
			generator.UnboxIfNeeded(fieldInfo.FieldType);
			if (!fieldInfo.IsStatic)
			{
				generator.Emit(OpCodes.Stfld, fieldInfo);
			}
			else
			{
				generator.Emit(OpCodes.Stsfld, fieldInfo);
			}
			generator.Return();
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x0004DE30 File Offset: 0x0004C030
		public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
		{
			DynamicMethod dynamicMethod = DynamicReflectionDelegateFactory.CreateDynamicMethod("Set" + propertyInfo.Name, null, new Type[]
			{
				typeof(T),
				typeof(object)
			}, propertyInfo.DeclaringType);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			DynamicReflectionDelegateFactory.GenerateCreateSetPropertyIL(propertyInfo, ilgenerator);
			return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x0004DEA0 File Offset: 0x0004C0A0
		internal static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			if (!setMethod.IsStatic)
			{
				generator.PushInstance(propertyInfo.DeclaringType);
			}
			generator.Emit(OpCodes.Ldarg_1);
			generator.UnboxIfNeeded(propertyInfo.PropertyType);
			generator.CallMethod(setMethod);
			generator.Return();
		}

		// Token: 0x040004EA RID: 1258
		private static readonly DynamicReflectionDelegateFactory _instance = new DynamicReflectionDelegateFactory();
	}
}
