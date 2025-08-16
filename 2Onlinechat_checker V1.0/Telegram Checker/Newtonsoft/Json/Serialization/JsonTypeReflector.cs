using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000123 RID: 291
	internal static class JsonTypeReflector
	{
		// Token: 0x06000F30 RID: 3888 RVA: 0x0005D598 File Offset: 0x0005B798
		public static T GetCachedAttribute<T>(object attributeProvider) where T : Attribute
		{
			return CachedAttributeGetter<T>.GetAttribute(attributeProvider);
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0005D5A0 File Offset: 0x0005B7A0
		public static bool CanTypeDescriptorConvertString(Type type, out TypeConverter typeConverter)
		{
			typeConverter = TypeDescriptor.GetConverter(type);
			if (typeConverter != null)
			{
				Type type2 = typeConverter.GetType();
				if (!string.Equals(type2.FullName, "System.ComponentModel.ComponentConverter", StringComparison.Ordinal) && !string.Equals(type2.FullName, "System.ComponentModel.ReferenceConverter", StringComparison.Ordinal) && !string.Equals(type2.FullName, "System.Windows.Forms.Design.DataSourceConverter", StringComparison.Ordinal) && type2 != typeof(TypeConverter))
				{
					return typeConverter.CanConvertTo(typeof(string));
				}
			}
			return false;
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0005D634 File Offset: 0x0005B834
		public static DataContractAttribute GetDataContractAttribute(Type type)
		{
			Type type2 = type;
			while (type2 != null)
			{
				DataContractAttribute attribute = CachedAttributeGetter<DataContractAttribute>.GetAttribute(type2);
				if (attribute != null)
				{
					return attribute;
				}
				type2 = type2.BaseType();
			}
			return null;
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0005D66C File Offset: 0x0005B86C
		public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType() == MemberTypes.Field)
			{
				return CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfo);
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
			DataMemberAttribute dataMemberAttribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo);
			if (dataMemberAttribute == null && propertyInfo.IsVirtual())
			{
				Type type = propertyInfo.DeclaringType;
				while (dataMemberAttribute == null && type != null)
				{
					PropertyInfo propertyInfo2 = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(type, propertyInfo);
					if (propertyInfo2 != null && propertyInfo2.IsVirtual())
					{
						dataMemberAttribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo2);
					}
					type = type.BaseType();
				}
			}
			return dataMemberAttribute;
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0005D700 File Offset: 0x0005B900
		public static MemberSerialization GetObjectMemberSerialization(Type objectType, bool ignoreSerializableAttribute)
		{
			JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(objectType);
			if (cachedAttribute != null)
			{
				return cachedAttribute.MemberSerialization;
			}
			if (JsonTypeReflector.GetDataContractAttribute(objectType) != null)
			{
				return MemberSerialization.OptIn;
			}
			if (!ignoreSerializableAttribute && JsonTypeReflector.IsSerializable(objectType))
			{
				return MemberSerialization.Fields;
			}
			return MemberSerialization.OptOut;
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0005D748 File Offset: 0x0005B948
		public static JsonConverter GetJsonConverter(object attributeProvider)
		{
			JsonConverterAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonConverterAttribute>(attributeProvider);
			if (cachedAttribute != null)
			{
				Func<object[], object> func = JsonTypeReflector.CreatorCache.Get(cachedAttribute.ConverterType);
				if (func != null)
				{
					return (JsonConverter)func(cachedAttribute.ConverterParameters);
				}
			}
			return null;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0005D790 File Offset: 0x0005B990
		public static JsonConverter CreateJsonConverterInstance(Type converterType, object[] converterArgs)
		{
			return (JsonConverter)JsonTypeReflector.CreatorCache.Get(converterType)(converterArgs);
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0005D7A8 File Offset: 0x0005B9A8
		public static NamingStrategy CreateNamingStrategyInstance(Type namingStrategyType, object[] converterArgs)
		{
			return (NamingStrategy)JsonTypeReflector.CreatorCache.Get(namingStrategyType)(converterArgs);
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0005D7C0 File Offset: 0x0005B9C0
		public static NamingStrategy GetContainerNamingStrategy(JsonContainerAttribute containerAttribute)
		{
			if (containerAttribute.NamingStrategyInstance == null)
			{
				if (containerAttribute.NamingStrategyType == null)
				{
					return null;
				}
				containerAttribute.NamingStrategyInstance = JsonTypeReflector.CreateNamingStrategyInstance(containerAttribute.NamingStrategyType, containerAttribute.NamingStrategyParameters);
			}
			return containerAttribute.NamingStrategyInstance;
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x0005D80C File Offset: 0x0005BA0C
		private static Func<object[], object> GetCreator(Type type)
		{
			Func<object> defaultConstructor = (ReflectionUtils.HasDefaultConstructor(type, false) ? JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type) : null);
			return delegate(object[] parameters)
			{
				object obj;
				try
				{
					if (parameters != null)
					{
						Type[] array = parameters.Select((object param) => param.GetType()).ToArray<Type>();
						ConstructorInfo constructor = type.GetConstructor(array);
						if (!(null != constructor))
						{
							throw new JsonException("No matching parameterized constructor found for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
						}
						obj = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor)(parameters);
					}
					else
					{
						if (defaultConstructor == null)
						{
							throw new JsonException("No parameterless constructor defined for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
						}
						obj = defaultConstructor();
					}
				}
				catch (Exception ex)
				{
					throw new JsonException("Error creating '{0}'.".FormatWith(CultureInfo.InvariantCulture, type), ex);
				}
				return obj;
			};
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0005D864 File Offset: 0x0005BA64
		private static Type GetAssociatedMetadataType(Type type)
		{
			return JsonTypeReflector.AssociatedMetadataTypesCache.Get(type);
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0005D874 File Offset: 0x0005BA74
		private static Type GetAssociateMetadataTypeFromAttribute(Type type)
		{
			foreach (Attribute attribute in ReflectionUtils.GetAttributes(type, null, true))
			{
				Type type2 = attribute.GetType();
				if (string.Equals(type2.FullName, "System.ComponentModel.DataAnnotations.MetadataTypeAttribute", StringComparison.Ordinal))
				{
					if (JsonTypeReflector._metadataTypeAttributeReflectionObject == null)
					{
						JsonTypeReflector._metadataTypeAttributeReflectionObject = ReflectionObject.Create(type2, new string[] { "MetadataClassType" });
					}
					return (Type)JsonTypeReflector._metadataTypeAttributeReflectionObject.GetValue(attribute, "MetadataClassType");
				}
			}
			return null;
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x0005D900 File Offset: 0x0005BB00
		private static T GetAttribute<T>(Type type) where T : Attribute
		{
			Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(type);
			T t;
			if (associatedMetadataType != null)
			{
				t = ReflectionUtils.GetAttribute<T>(associatedMetadataType, true);
				if (t != null)
				{
					return t;
				}
			}
			t = ReflectionUtils.GetAttribute<T>(type, true);
			if (t != null)
			{
				return t;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				t = ReflectionUtils.GetAttribute<T>(interfaces[i], true);
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0005D98C File Offset: 0x0005BB8C
		private static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
		{
			Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(memberInfo.DeclaringType);
			T t;
			if (associatedMetadataType != null)
			{
				MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(associatedMetadataType, memberInfo);
				if (memberInfoFromType != null)
				{
					t = ReflectionUtils.GetAttribute<T>(memberInfoFromType, true);
					if (t != null)
					{
						return t;
					}
				}
			}
			t = ReflectionUtils.GetAttribute<T>(memberInfo, true);
			if (t != null)
			{
				return t;
			}
			if (memberInfo.DeclaringType != null)
			{
				Type[] interfaces = memberInfo.DeclaringType.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					MemberInfo memberInfoFromType2 = ReflectionUtils.GetMemberInfoFromType(interfaces[i], memberInfo);
					if (memberInfoFromType2 != null)
					{
						t = ReflectionUtils.GetAttribute<T>(memberInfoFromType2, true);
						if (t != null)
						{
							return t;
						}
					}
				}
			}
			return default(T);
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x0005DA64 File Offset: 0x0005BC64
		public static bool IsNonSerializable(object provider)
		{
			return JsonTypeReflector.GetCachedAttribute<NonSerializedAttribute>(provider) != null;
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x0005DA70 File Offset: 0x0005BC70
		public static bool IsSerializable(object provider)
		{
			return JsonTypeReflector.GetCachedAttribute<SerializableAttribute>(provider) != null;
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0005DA7C File Offset: 0x0005BC7C
		public static T GetAttribute<T>(object provider) where T : Attribute
		{
			Type type = provider as Type;
			if (type != null)
			{
				return JsonTypeReflector.GetAttribute<T>(type);
			}
			MemberInfo memberInfo = provider as MemberInfo;
			if (memberInfo != null)
			{
				return JsonTypeReflector.GetAttribute<T>(memberInfo);
			}
			return ReflectionUtils.GetAttribute<T>(provider, true);
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000F41 RID: 3905 RVA: 0x0005DAC8 File Offset: 0x0005BCC8
		public static bool DynamicCodeGeneration
		{
			[SecuritySafeCritical]
			get
			{
				if (JsonTypeReflector._dynamicCodeGeneration == null)
				{
					try
					{
						new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
						new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess).Demand();
						new SecurityPermission(SecurityPermissionFlag.SkipVerification).Demand();
						new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
						new SecurityPermission(PermissionState.Unrestricted).Demand();
						JsonTypeReflector._dynamicCodeGeneration = new bool?(true);
					}
					catch (Exception)
					{
						JsonTypeReflector._dynamicCodeGeneration = new bool?(false);
					}
				}
				return JsonTypeReflector._dynamicCodeGeneration.GetValueOrDefault();
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000F42 RID: 3906 RVA: 0x0005DB58 File Offset: 0x0005BD58
		public static bool FullyTrusted
		{
			get
			{
				if (JsonTypeReflector._fullyTrusted == null)
				{
					AppDomain currentDomain = AppDomain.CurrentDomain;
					JsonTypeReflector._fullyTrusted = new bool?(currentDomain.IsHomogenous && currentDomain.IsFullyTrusted);
				}
				return JsonTypeReflector._fullyTrusted.GetValueOrDefault();
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0005DBAC File Offset: 0x0005BDAC
		public static ReflectionDelegateFactory ReflectionDelegateFactory
		{
			get
			{
				if (JsonTypeReflector.DynamicCodeGeneration)
				{
					return DynamicReflectionDelegateFactory.Instance;
				}
				return LateBoundReflectionDelegateFactory.Instance;
			}
		}

		// Token: 0x040005E0 RID: 1504
		private static bool? _dynamicCodeGeneration;

		// Token: 0x040005E1 RID: 1505
		private static bool? _fullyTrusted;

		// Token: 0x040005E2 RID: 1506
		public const string IdPropertyName = "$id";

		// Token: 0x040005E3 RID: 1507
		public const string RefPropertyName = "$ref";

		// Token: 0x040005E4 RID: 1508
		public const string TypePropertyName = "$type";

		// Token: 0x040005E5 RID: 1509
		public const string ValuePropertyName = "$value";

		// Token: 0x040005E6 RID: 1510
		public const string ArrayValuesPropertyName = "$values";

		// Token: 0x040005E7 RID: 1511
		public const string ShouldSerializePrefix = "ShouldSerialize";

		// Token: 0x040005E8 RID: 1512
		public const string SpecifiedPostfix = "Specified";

		// Token: 0x040005E9 RID: 1513
		private static readonly ThreadSafeStore<Type, Func<object[], object>> CreatorCache = new ThreadSafeStore<Type, Func<object[], object>>(new Func<Type, Func<object[], object>>(JsonTypeReflector.GetCreator));

		// Token: 0x040005EA RID: 1514
		private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(JsonTypeReflector.GetAssociateMetadataTypeFromAttribute));

		// Token: 0x040005EB RID: 1515
		private static ReflectionObject _metadataTypeAttributeReflectionObject;
	}
}
