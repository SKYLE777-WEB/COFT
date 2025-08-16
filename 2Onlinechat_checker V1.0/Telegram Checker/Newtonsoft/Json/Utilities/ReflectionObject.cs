using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F0 RID: 240
	internal class ReflectionObject
	{
		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000CCD RID: 3277 RVA: 0x00050A10 File Offset: 0x0004EC10
		public ObjectConstructor<object> Creator { get; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000CCE RID: 3278 RVA: 0x00050A18 File Offset: 0x0004EC18
		public IDictionary<string, ReflectionMember> Members { get; }

		// Token: 0x06000CCF RID: 3279 RVA: 0x00050A20 File Offset: 0x0004EC20
		private ReflectionObject(ObjectConstructor<object> creator)
		{
			this.Members = new Dictionary<string, ReflectionMember>();
			this.Creator = creator;
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x00050A3C File Offset: 0x0004EC3C
		public object GetValue(object target, string member)
		{
			return this.Members[member].Getter(target);
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x00050A64 File Offset: 0x0004EC64
		public void SetValue(object target, string member, object value)
		{
			this.Members[member].Setter(target, value);
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x00050A90 File Offset: 0x0004EC90
		public Type GetType(string member)
		{
			return this.Members[member].MemberType;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x00050AA4 File Offset: 0x0004ECA4
		public static ReflectionObject Create(Type t, params string[] memberNames)
		{
			return ReflectionObject.Create(t, null, memberNames);
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x00050AB0 File Offset: 0x0004ECB0
		public static ReflectionObject Create(Type t, MethodBase creator, params string[] memberNames)
		{
			ReflectionDelegateFactory reflectionDelegateFactory = JsonTypeReflector.ReflectionDelegateFactory;
			ObjectConstructor<object> objectConstructor = null;
			if (creator != null)
			{
				objectConstructor = reflectionDelegateFactory.CreateParameterizedConstructor(creator);
			}
			else if (ReflectionUtils.HasDefaultConstructor(t, false))
			{
				Func<object> ctor = reflectionDelegateFactory.CreateDefaultConstructor<object>(t);
				objectConstructor = (object[] args) => ctor();
			}
			ReflectionObject reflectionObject = new ReflectionObject(objectConstructor);
			int i = 0;
			while (i < memberNames.Length)
			{
				string text = memberNames[i];
				MemberInfo[] member = t.GetMember(text, BindingFlags.Instance | BindingFlags.Public);
				if (member.Length != 1)
				{
					throw new ArgumentException("Expected a single member with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, text));
				}
				MemberInfo memberInfo = member.Single<MemberInfo>();
				ReflectionMember reflectionMember = new ReflectionMember();
				MemberTypes memberTypes = memberInfo.MemberType();
				if (memberTypes == MemberTypes.Field)
				{
					goto IL_00C0;
				}
				if (memberTypes != MemberTypes.Method)
				{
					if (memberTypes == MemberTypes.Property)
					{
						goto IL_00C0;
					}
					throw new ArgumentException("Unexpected member type '{0}' for member '{1}'.".FormatWith(CultureInfo.InvariantCulture, memberInfo.MemberType(), memberInfo.Name));
				}
				else
				{
					MethodInfo methodInfo = (MethodInfo)memberInfo;
					if (methodInfo.IsPublic)
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (parameters.Length == 0 && methodInfo.ReturnType != typeof(void))
						{
							MethodCall<object, object> call2 = reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
							reflectionMember.Getter = (object target) => call2(target, new object[0]);
						}
						else if (parameters.Length == 1 && methodInfo.ReturnType == typeof(void))
						{
							MethodCall<object, object> call = reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
							reflectionMember.Setter = delegate(object target, object arg)
							{
								call(target, new object[] { arg });
							};
						}
					}
				}
				IL_01EA:
				if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
				{
					reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
				}
				if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
				{
					reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
				}
				reflectionMember.MemberType = ReflectionUtils.GetMemberUnderlyingType(memberInfo);
				reflectionObject.Members[text] = reflectionMember;
				i++;
				continue;
				IL_00C0:
				if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
				{
					reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
				}
				if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
				{
					reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
					goto IL_01EA;
				}
				goto IL_01EA;
			}
			return reflectionObject;
		}
	}
}
