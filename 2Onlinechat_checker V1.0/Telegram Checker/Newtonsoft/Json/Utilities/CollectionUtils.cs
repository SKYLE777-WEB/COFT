using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000CB RID: 203
	internal static class CollectionUtils
	{
		// Token: 0x06000B70 RID: 2928 RVA: 0x0004896C File Offset: 0x00046B6C
		public static bool IsNullOrEmpty<T>(ICollection<T> collection)
		{
			return collection == null || collection.Count == 0;
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x00048980 File Offset: 0x00046B80
		public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
		{
			if (initial == null)
			{
				throw new ArgumentNullException("initial");
			}
			if (collection == null)
			{
				return;
			}
			foreach (T t in collection)
			{
				initial.Add(t);
			}
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x000489EC File Offset: 0x00046BEC
		public static bool IsDictionaryType(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			return typeof(IDictionary).IsAssignableFrom(type) || ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<, >)) || ReflectionUtils.ImplementsGenericDefinition(type, typeof(IReadOnlyDictionary<, >));
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x00048A50 File Offset: 0x00046C50
		public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType)
		{
			Type type = typeof(IList<>).MakeGenericType(new Type[] { collectionItemType });
			return CollectionUtils.ResolveEnumerableCollectionConstructor(collectionType, collectionItemType, type);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x00048A84 File Offset: 0x00046C84
		public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType, Type constructorArgumentType)
		{
			Type type = typeof(IEnumerable<>).MakeGenericType(new Type[] { collectionItemType });
			ConstructorInfo constructorInfo = null;
			foreach (ConstructorInfo constructorInfo2 in collectionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
			{
				IList<ParameterInfo> parameters = constructorInfo2.GetParameters();
				if (parameters.Count == 1)
				{
					Type parameterType = parameters[0].ParameterType;
					if (type == parameterType)
					{
						constructorInfo = constructorInfo2;
						break;
					}
					if (constructorInfo == null && parameterType.IsAssignableFrom(constructorArgumentType))
					{
						constructorInfo = constructorInfo2;
					}
				}
			}
			return constructorInfo;
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x00048B2C File Offset: 0x00046D2C
		public static bool AddDistinct<T>(this IList<T> list, T value)
		{
			return list.AddDistinct(value, EqualityComparer<T>.Default);
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x00048B3C File Offset: 0x00046D3C
		public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
		{
			if (list.ContainsValue(value, comparer))
			{
				return false;
			}
			list.Add(value);
			return true;
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x00048B58 File Offset: 0x00046D58
		public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			foreach (TSource tsource in source)
			{
				if (comparer.Equals(tsource, value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x00048BD8 File Offset: 0x00046DD8
		public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			bool flag = true;
			foreach (T t in values)
			{
				if (!list.AddDistinct(t, comparer))
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x00048C34 File Offset: 0x00046E34
		public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int num = 0;
			foreach (T t in collection)
			{
				if (predicate(t))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x00048C9C File Offset: 0x00046E9C
		public static bool Contains<T>(this List<T> list, T value, IEqualityComparer comparer)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (comparer.Equals(value, list[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x00048CE4 File Offset: 0x00046EE4
		public static int IndexOfReference<T>(this List<T> list, T item)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (item == list[i])
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x00048D24 File Offset: 0x00046F24
		private static IList<int> GetDimensions(IList values, int dimensionsCount)
		{
			IList<int> list = new List<int>();
			IList list2 = values;
			for (;;)
			{
				list.Add(list2.Count);
				if (list.Count == dimensionsCount || list2.Count == 0)
				{
					break;
				}
				IList list3 = list2[0] as IList;
				if (list3 == null)
				{
					break;
				}
				list2 = list3;
			}
			return list;
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x00048D78 File Offset: 0x00046F78
		private static void CopyFromJaggedToMultidimensionalArray(IList values, Array multidimensionalArray, int[] indices)
		{
			int num = indices.Length;
			if (num == multidimensionalArray.Rank)
			{
				multidimensionalArray.SetValue(CollectionUtils.JaggedArrayGetValue(values, indices), indices);
				return;
			}
			int length = multidimensionalArray.GetLength(num);
			if (((IList)CollectionUtils.JaggedArrayGetValue(values, indices)).Count != length)
			{
				throw new Exception("Cannot deserialize non-cubical array as multidimensional array.");
			}
			int[] array = new int[num + 1];
			for (int i = 0; i < num; i++)
			{
				array[i] = indices[i];
			}
			for (int j = 0; j < multidimensionalArray.GetLength(num); j++)
			{
				array[num] = j;
				CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, array);
			}
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x00048E18 File Offset: 0x00047018
		private static object JaggedArrayGetValue(IList values, int[] indices)
		{
			IList list = values;
			for (int i = 0; i < indices.Length; i++)
			{
				int num = indices[i];
				if (i == indices.Length - 1)
				{
					return list[num];
				}
				list = (IList)list[num];
			}
			return list;
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x00048E64 File Offset: 0x00047064
		public static Array ToMultidimensionalArray(IList values, Type type, int rank)
		{
			IList<int> dimensions = CollectionUtils.GetDimensions(values, rank);
			while (dimensions.Count < rank)
			{
				dimensions.Add(0);
			}
			Array array = Array.CreateInstance(type, dimensions.ToArray<int>());
			CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, array, CollectionUtils.ArrayEmpty<int>());
			return array;
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x00048EAC File Offset: 0x000470AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ArrayEmpty<T>()
		{
			T[] array = Enumerable.Empty<T>() as T[];
			return array ?? new T[0];
		}
	}
}
