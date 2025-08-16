using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000115 RID: 277
	public class JsonDictionaryContract : JsonContainerContract
	{
		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000E03 RID: 3587 RVA: 0x00056178 File Offset: 0x00054378
		// (set) Token: 0x06000E04 RID: 3588 RVA: 0x00056180 File Offset: 0x00054380
		public Func<string, string> DictionaryKeyResolver { get; set; }

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x0005618C File Offset: 0x0005438C
		public Type DictionaryKeyType { get; }

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000E06 RID: 3590 RVA: 0x00056194 File Offset: 0x00054394
		public Type DictionaryValueType { get; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000E07 RID: 3591 RVA: 0x0005619C File Offset: 0x0005439C
		// (set) Token: 0x06000E08 RID: 3592 RVA: 0x000561A4 File Offset: 0x000543A4
		internal JsonContract KeyContract { get; set; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x000561B0 File Offset: 0x000543B0
		internal bool ShouldCreateWrapper { get; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x000561B8 File Offset: 0x000543B8
		internal ObjectConstructor<object> ParameterizedCreator
		{
			get
			{
				if (this._parameterizedCreator == null)
				{
					this._parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(this._parameterizedConstructor);
				}
				return this._parameterizedCreator;
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x000561E4 File Offset: 0x000543E4
		// (set) Token: 0x06000E0C RID: 3596 RVA: 0x000561EC File Offset: 0x000543EC
		public ObjectConstructor<object> OverrideCreator
		{
			get
			{
				return this._overrideCreator;
			}
			set
			{
				this._overrideCreator = value;
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000E0D RID: 3597 RVA: 0x000561F8 File Offset: 0x000543F8
		// (set) Token: 0x06000E0E RID: 3598 RVA: 0x00056200 File Offset: 0x00054400
		public bool HasParameterizedCreator { get; set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06000E0F RID: 3599 RVA: 0x0005620C File Offset: 0x0005440C
		internal bool HasParameterizedCreatorInternal
		{
			get
			{
				return this.HasParameterizedCreator || this._parameterizedCreator != null || this._parameterizedConstructor != null;
			}
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x00056234 File Offset: 0x00054434
		public JsonDictionaryContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Dictionary;
			Type type;
			Type type2;
			if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<, >), out this._genericCollectionDefinitionType))
			{
				type = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				type2 = this._genericCollectionDefinitionType.GetGenericArguments()[1];
				if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IDictionary<, >)))
				{
					base.CreatedType = typeof(Dictionary<, >).MakeGenericType(new Type[] { type, type2 });
				}
				this.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyDictionary<, >));
			}
			else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IReadOnlyDictionary<, >), out this._genericCollectionDefinitionType))
			{
				type = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				type2 = this._genericCollectionDefinitionType.GetGenericArguments()[1];
				if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IReadOnlyDictionary<, >)))
				{
					base.CreatedType = typeof(ReadOnlyDictionary<, >).MakeGenericType(new Type[] { type, type2 });
				}
				this.IsReadOnlyOrFixedSize = true;
			}
			else
			{
				ReflectionUtils.GetDictionaryKeyValueTypes(base.UnderlyingType, out type, out type2);
				if (base.UnderlyingType == typeof(IDictionary))
				{
					base.CreatedType = typeof(Dictionary<object, object>);
				}
			}
			if (type != null && type2 != null)
			{
				this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(base.CreatedType, typeof(KeyValuePair<, >).MakeGenericType(new Type[] { type, type2 }), typeof(IDictionary<, >).MakeGenericType(new Type[] { type, type2 }));
				if (!this.HasParameterizedCreatorInternal && underlyingType.Name == "FSharpMap`2")
				{
					FSharpUtils.EnsureInitialized(underlyingType.Assembly());
					this._parameterizedCreator = FSharpUtils.CreateMap(type, type2);
				}
			}
			this.ShouldCreateWrapper = !typeof(IDictionary).IsAssignableFrom(base.CreatedType);
			this.DictionaryKeyType = type;
			this.DictionaryValueType = type2;
			Type type3;
			ObjectConstructor<object> objectConstructor;
			if (ImmutableCollectionsUtils.TryBuildImmutableForDictionaryContract(underlyingType, this.DictionaryKeyType, this.DictionaryValueType, out type3, out objectConstructor))
			{
				base.CreatedType = type3;
				this._parameterizedCreator = objectConstructor;
				this.IsReadOnlyOrFixedSize = true;
			}
		}

		// Token: 0x06000E11 RID: 3601 RVA: 0x000564A4 File Offset: 0x000546A4
		internal IWrappedDictionary CreateWrapper(object dictionary)
		{
			if (this._genericWrapperCreator == null)
			{
				this._genericWrapperType = typeof(DictionaryWrapper<, >).MakeGenericType(new Type[] { this.DictionaryKeyType, this.DictionaryValueType });
				ConstructorInfo constructor = this._genericWrapperType.GetConstructor(new Type[] { this._genericCollectionDefinitionType });
				this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
			}
			return (IWrappedDictionary)this._genericWrapperCreator(new object[] { dictionary });
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x00056534 File Offset: 0x00054734
		internal IDictionary CreateTemporaryDictionary()
		{
			if (this._genericTemporaryDictionaryCreator == null)
			{
				Type type = typeof(Dictionary<, >).MakeGenericType(new Type[]
				{
					this.DictionaryKeyType ?? typeof(object),
					this.DictionaryValueType ?? typeof(object)
				});
				this._genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type);
			}
			return (IDictionary)this._genericTemporaryDictionaryCreator();
		}

		// Token: 0x04000593 RID: 1427
		private readonly Type _genericCollectionDefinitionType;

		// Token: 0x04000594 RID: 1428
		private Type _genericWrapperType;

		// Token: 0x04000595 RID: 1429
		private ObjectConstructor<object> _genericWrapperCreator;

		// Token: 0x04000596 RID: 1430
		private Func<object> _genericTemporaryDictionaryCreator;

		// Token: 0x04000598 RID: 1432
		private readonly ConstructorInfo _parameterizedConstructor;

		// Token: 0x04000599 RID: 1433
		private ObjectConstructor<object> _overrideCreator;

		// Token: 0x0400059A RID: 1434
		private ObjectConstructor<object> _parameterizedCreator;
	}
}
