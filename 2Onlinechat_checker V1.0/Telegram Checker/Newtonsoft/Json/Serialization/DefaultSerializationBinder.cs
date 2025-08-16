using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000101 RID: 257
	public class DefaultSerializationBinder : SerializationBinder, ISerializationBinder
	{
		// Token: 0x06000D92 RID: 3474 RVA: 0x00054F0C File Offset: 0x0005310C
		public DefaultSerializationBinder()
		{
			this._typeCache = new ThreadSafeStore<TypeNameKey, Type>(new Func<TypeNameKey, Type>(this.GetTypeFromTypeNameKey));
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x00054F2C File Offset: 0x0005312C
		private Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey)
		{
			string assemblyName = typeNameKey.AssemblyName;
			string typeName = typeNameKey.TypeName;
			if (assemblyName == null)
			{
				return Type.GetType(typeName);
			}
			Assembly assembly = Assembly.LoadWithPartialName(assemblyName);
			if (assembly == null)
			{
				foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (assembly2.FullName == assemblyName || assembly2.GetName().Name == assemblyName)
					{
						assembly = assembly2;
						break;
					}
				}
			}
			if (assembly == null)
			{
				throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, assemblyName));
			}
			Type type = assembly.GetType(typeName);
			if (type == null)
			{
				if (typeName.IndexOf('`') >= 0)
				{
					try
					{
						type = this.GetGenericTypeFromTypeName(typeName, assembly);
					}
					catch (Exception ex)
					{
						throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeName, assembly.FullName), ex);
					}
				}
				if (type == null)
				{
					throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeName, assembly.FullName));
				}
			}
			return type;
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x00055074 File Offset: 0x00053274
		private Type GetGenericTypeFromTypeName(string typeName, Assembly assembly)
		{
			Type type = null;
			int num = typeName.IndexOf('[');
			if (num >= 0)
			{
				string text = typeName.Substring(0, num);
				Type type2 = assembly.GetType(text);
				if (type2 != null)
				{
					List<Type> list = new List<Type>();
					int num2 = 0;
					int num3 = 0;
					int num4 = typeName.Length - 1;
					for (int i = num + 1; i < num4; i++)
					{
						char c = typeName[i];
						if (c != '[')
						{
							if (c == ']')
							{
								num2--;
								if (num2 == 0)
								{
									TypeNameKey typeNameKey = ReflectionUtils.SplitFullyQualifiedTypeName(typeName.Substring(num3, i - num3));
									list.Add(this.GetTypeByName(typeNameKey));
								}
							}
						}
						else
						{
							if (num2 == 0)
							{
								num3 = i + 1;
							}
							num2++;
						}
					}
					type = type2.MakeGenericType(list.ToArray());
				}
			}
			return type;
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x0005515C File Offset: 0x0005335C
		private Type GetTypeByName(TypeNameKey typeNameKey)
		{
			return this._typeCache.Get(typeNameKey);
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x0005516C File Offset: 0x0005336C
		public override Type BindToType(string assemblyName, string typeName)
		{
			return this.GetTypeByName(new TypeNameKey(assemblyName, typeName));
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x0005517C File Offset: 0x0005337C
		public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = serializedType.Assembly.FullName;
			typeName = serializedType.FullName;
		}

		// Token: 0x0400054C RID: 1356
		internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();

		// Token: 0x0400054D RID: 1357
		private readonly ThreadSafeStore<TypeNameKey, Type> _typeCache;
	}
}
