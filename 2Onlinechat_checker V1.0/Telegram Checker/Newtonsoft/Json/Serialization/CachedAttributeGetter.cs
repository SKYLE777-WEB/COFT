using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x020000FA RID: 250
	internal static class CachedAttributeGetter<T> where T : Attribute
	{
		// Token: 0x06000D41 RID: 3393 RVA: 0x00052ACC File Offset: 0x00050CCC
		public static T GetAttribute(object type)
		{
			return CachedAttributeGetter<T>.TypeAttributeCache.Get(type);
		}

		// Token: 0x0400053B RID: 1339
		private static readonly ThreadSafeStore<object, T> TypeAttributeCache = new ThreadSafeStore<object, T>(new Func<object, T>(JsonTypeReflector.GetAttribute<T>));
	}
}
