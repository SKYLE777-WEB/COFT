using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x0200016B RID: 363
	public class DiscriminatedUnionConverter : JsonConverter
	{
		// Token: 0x06001366 RID: 4966 RVA: 0x0006CAD8 File Offset: 0x0006ACD8
		private static Type CreateUnionTypeLookup(Type t)
		{
			MethodCall<object, object> getUnionCases = FSharpUtils.GetUnionCases;
			object obj = null;
			object[] array = new object[2];
			array[0] = t;
			object obj2 = ((object[])getUnionCases(obj, array)).First<object>();
			return (Type)FSharpUtils.GetUnionCaseInfoDeclaringType(obj2);
		}

		// Token: 0x06001367 RID: 4967 RVA: 0x0006CB1C File Offset: 0x0006AD1C
		private static DiscriminatedUnionConverter.Union CreateUnion(Type t)
		{
			DiscriminatedUnionConverter.Union union = new DiscriminatedUnionConverter.Union();
			DiscriminatedUnionConverter.Union union2 = union;
			MethodCall<object, object> preComputeUnionTagReader = FSharpUtils.PreComputeUnionTagReader;
			object obj = null;
			object[] array = new object[2];
			array[0] = t;
			union2.TagReader = (FSharpFunction)preComputeUnionTagReader(obj, array);
			union.Cases = new List<DiscriminatedUnionConverter.UnionCase>();
			MethodCall<object, object> getUnionCases = FSharpUtils.GetUnionCases;
			object obj2 = null;
			object[] array2 = new object[2];
			array2[0] = t;
			foreach (object obj3 in (object[])getUnionCases(obj2, array2))
			{
				DiscriminatedUnionConverter.UnionCase unionCase = new DiscriminatedUnionConverter.UnionCase();
				unionCase.Tag = (int)FSharpUtils.GetUnionCaseInfoTag(obj3);
				unionCase.Name = (string)FSharpUtils.GetUnionCaseInfoName(obj3);
				unionCase.Fields = (PropertyInfo[])FSharpUtils.GetUnionCaseInfoFields(obj3, new object[0]);
				DiscriminatedUnionConverter.UnionCase unionCase2 = unionCase;
				MethodCall<object, object> preComputeUnionReader = FSharpUtils.PreComputeUnionReader;
				object obj4 = null;
				object[] array4 = new object[2];
				array4[0] = obj3;
				unionCase2.FieldReader = (FSharpFunction)preComputeUnionReader(obj4, array4);
				DiscriminatedUnionConverter.UnionCase unionCase3 = unionCase;
				MethodCall<object, object> preComputeUnionConstructor = FSharpUtils.PreComputeUnionConstructor;
				object obj5 = null;
				object[] array5 = new object[2];
				array5[0] = obj3;
				unionCase3.Constructor = (FSharpFunction)preComputeUnionConstructor(obj5, array5);
				union.Cases.Add(unionCase);
			}
			return union;
		}

		// Token: 0x06001368 RID: 4968 RVA: 0x0006CC34 File Offset: 0x0006AE34
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			Type type = DiscriminatedUnionConverter.UnionTypeLookupCache.Get(value.GetType());
			DiscriminatedUnionConverter.Union union = DiscriminatedUnionConverter.UnionCache.Get(type);
			int tag = (int)union.TagReader.Invoke(new object[] { value });
			DiscriminatedUnionConverter.UnionCase unionCase = union.Cases.Single((DiscriminatedUnionConverter.UnionCase c) => c.Tag == tag);
			writer.WriteStartObject();
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Case") : "Case");
			writer.WriteValue(unionCase.Name);
			if (unionCase.Fields != null && unionCase.Fields.Length != 0)
			{
				object[] array = (object[])unionCase.FieldReader.Invoke(new object[] { value });
				writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Fields") : "Fields");
				writer.WriteStartArray();
				foreach (object obj in array)
				{
					serializer.Serialize(writer, obj);
				}
				writer.WriteEndArray();
			}
			writer.WriteEndObject();
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0006CD70 File Offset: 0x0006AF70
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			DiscriminatedUnionConverter.UnionCase unionCase = null;
			string caseName = null;
			JArray jarray = null;
			reader.ReadAndAssert();
			Func<DiscriminatedUnionConverter.UnionCase, bool> <>9__0;
			while (reader.TokenType == JsonToken.PropertyName)
			{
				string text = reader.Value.ToString();
				if (string.Equals(text, "Case", StringComparison.OrdinalIgnoreCase))
				{
					reader.ReadAndAssert();
					DiscriminatedUnionConverter.Union union = DiscriminatedUnionConverter.UnionCache.Get(objectType);
					caseName = reader.Value.ToString();
					IEnumerable<DiscriminatedUnionConverter.UnionCase> cases = union.Cases;
					Func<DiscriminatedUnionConverter.UnionCase, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (DiscriminatedUnionConverter.UnionCase c) => c.Name == caseName);
					}
					unionCase = cases.SingleOrDefault(func);
					if (unionCase == null)
					{
						throw JsonSerializationException.Create(reader, "No union type found with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, caseName));
					}
				}
				else
				{
					if (!string.Equals(text, "Fields", StringComparison.OrdinalIgnoreCase))
					{
						throw JsonSerializationException.Create(reader, "Unexpected property '{0}' found when reading union.".FormatWith(CultureInfo.InvariantCulture, text));
					}
					reader.ReadAndAssert();
					if (reader.TokenType != JsonToken.StartArray)
					{
						throw JsonSerializationException.Create(reader, "Union fields must been an array.");
					}
					jarray = (JArray)JToken.ReadFrom(reader);
				}
				reader.ReadAndAssert();
			}
			if (unionCase == null)
			{
				throw JsonSerializationException.Create(reader, "No '{0}' property with union name found.".FormatWith(CultureInfo.InvariantCulture, "Case"));
			}
			object[] array = new object[unionCase.Fields.Length];
			if (unionCase.Fields.Length != 0 && jarray == null)
			{
				throw JsonSerializationException.Create(reader, "No '{0}' property with union fields found.".FormatWith(CultureInfo.InvariantCulture, "Fields"));
			}
			if (jarray != null)
			{
				if (unionCase.Fields.Length != jarray.Count)
				{
					throw JsonSerializationException.Create(reader, "The number of field values does not match the number of properties defined by union '{0}'.".FormatWith(CultureInfo.InvariantCulture, caseName));
				}
				for (int i = 0; i < jarray.Count; i++)
				{
					JToken jtoken = jarray[i];
					PropertyInfo propertyInfo = unionCase.Fields[i];
					array[i] = jtoken.ToObject(propertyInfo.PropertyType, serializer);
				}
			}
			object[] array2 = new object[] { array };
			return unionCase.Constructor.Invoke(array2);
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0006CF98 File Offset: 0x0006B198
		public override bool CanConvert(Type objectType)
		{
			if (typeof(IEnumerable).IsAssignableFrom(objectType))
			{
				return false;
			}
			object[] customAttributes = objectType.GetCustomAttributes(true);
			bool flag = false;
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i].GetType();
				if (type.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute")
				{
					FSharpUtils.EnsureInitialized(type.Assembly());
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			MethodCall<object, object> isUnion = FSharpUtils.IsUnion;
			object obj = null;
			object[] array2 = new object[2];
			array2[0] = objectType;
			return (bool)isUnion(obj, array2);
		}

		// Token: 0x040006F1 RID: 1777
		private const string CasePropertyName = "Case";

		// Token: 0x040006F2 RID: 1778
		private const string FieldsPropertyName = "Fields";

		// Token: 0x040006F3 RID: 1779
		private static readonly ThreadSafeStore<Type, DiscriminatedUnionConverter.Union> UnionCache = new ThreadSafeStore<Type, DiscriminatedUnionConverter.Union>(new Func<Type, DiscriminatedUnionConverter.Union>(DiscriminatedUnionConverter.CreateUnion));

		// Token: 0x040006F4 RID: 1780
		private static readonly ThreadSafeStore<Type, Type> UnionTypeLookupCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(DiscriminatedUnionConverter.CreateUnionTypeLookup));

		// Token: 0x020002B1 RID: 689
		internal class Union
		{
			// Token: 0x17000440 RID: 1088
			// (get) Token: 0x06001845 RID: 6213 RVA: 0x00088E84 File Offset: 0x00087084
			// (set) Token: 0x06001846 RID: 6214 RVA: 0x00088E8C File Offset: 0x0008708C
			public FSharpFunction TagReader { get; set; }

			// Token: 0x04000C80 RID: 3200
			public List<DiscriminatedUnionConverter.UnionCase> Cases;
		}

		// Token: 0x020002B2 RID: 690
		internal class UnionCase
		{
			// Token: 0x04000C82 RID: 3202
			public int Tag;

			// Token: 0x04000C83 RID: 3203
			public string Name;

			// Token: 0x04000C84 RID: 3204
			public PropertyInfo[] Fields;

			// Token: 0x04000C85 RID: 3205
			public FSharpFunction FieldReader;

			// Token: 0x04000C86 RID: 3206
			public FSharpFunction Constructor;
		}
	}
}
