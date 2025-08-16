using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200010D RID: 269
	public class JsonArrayContract : JsonContainerContract
	{
		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000DBD RID: 3517 RVA: 0x00055584 File Offset: 0x00053784
		public Type CollectionItemType { get; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000DBE RID: 3518 RVA: 0x0005558C File Offset: 0x0005378C
		public bool IsMultidimensionalArray { get; }

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000DBF RID: 3519 RVA: 0x00055594 File Offset: 0x00053794
		internal bool IsArray { get; }

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000DC0 RID: 3520 RVA: 0x0005559C File Offset: 0x0005379C
		internal bool ShouldCreateWrapper { get; }

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000DC1 RID: 3521 RVA: 0x000555A4 File Offset: 0x000537A4
		// (set) Token: 0x06000DC2 RID: 3522 RVA: 0x000555AC File Offset: 0x000537AC
		internal bool CanDeserialize { get; private set; }

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x000555B8 File Offset: 0x000537B8
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

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000DC4 RID: 3524 RVA: 0x000555E4 File Offset: 0x000537E4
		// (set) Token: 0x06000DC5 RID: 3525 RVA: 0x000555EC File Offset: 0x000537EC
		public ObjectConstructor<object> OverrideCreator
		{
			get
			{
				return this._overrideCreator;
			}
			set
			{
				this._overrideCreator = value;
				this.CanDeserialize = true;
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x000555FC File Offset: 0x000537FC
		// (set) Token: 0x06000DC7 RID: 3527 RVA: 0x00055604 File Offset: 0x00053804
		public bool HasParameterizedCreator { get; set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x00055610 File Offset: 0x00053810
		internal bool HasParameterizedCreatorInternal
		{
			get
			{
				return this.HasParameterizedCreator || this._parameterizedCreator != null || this._parameterizedConstructor != null;
			}
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x00055638 File Offset: 0x00053838
		public JsonArrayContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Array;
			this.IsArray = base.CreatedType.IsArray;
			bool flag;
			Type type;
			if (this.IsArray)
			{
				this.CollectionItemType = ReflectionUtils.GetCollectionItemType(base.UnderlyingType);
				this.IsReadOnlyOrFixedSize = true;
				this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(new Type[] { this.CollectionItemType });
				flag = true;
				this.IsMultidimensionalArray = this.IsArray && base.UnderlyingType.GetArrayRank() > 1;
			}
			else if (typeof(IList).IsAssignableFrom(underlyingType))
			{
				if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out this._genericCollectionDefinitionType))
				{
					this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				}
				else
				{
					this.CollectionItemType = ReflectionUtils.GetCollectionItemType(underlyingType);
				}
				if (underlyingType == typeof(IList))
				{
					base.CreatedType = typeof(List<object>);
				}
				if (this.CollectionItemType != null)
				{
					this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
				}
				this.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyCollection<>));
				flag = true;
			}
			else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out this._genericCollectionDefinitionType))
			{
				this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ICollection<>)) || ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IList<>)))
				{
					base.CreatedType = typeof(List<>).MakeGenericType(new Type[] { this.CollectionItemType });
				}
				if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ISet<>)))
				{
					base.CreatedType = typeof(HashSet<>).MakeGenericType(new Type[] { this.CollectionItemType });
				}
				this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
				flag = true;
				this.ShouldCreateWrapper = true;
			}
			else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IReadOnlyCollection<>), out type))
			{
				this.CollectionItemType = type.GetGenericArguments()[0];
				if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IReadOnlyCollection<>)) || ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IReadOnlyList<>)))
				{
					base.CreatedType = typeof(ReadOnlyCollection<>).MakeGenericType(new Type[] { this.CollectionItemType });
				}
				this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(new Type[] { this.CollectionItemType });
				this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(base.CreatedType, this.CollectionItemType);
				this.StoreFSharpListCreatorIfNecessary(underlyingType);
				this.IsReadOnlyOrFixedSize = true;
				flag = this.HasParameterizedCreatorInternal;
			}
			else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IEnumerable<>), out type))
			{
				this.CollectionItemType = type.GetGenericArguments()[0];
				if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IEnumerable<>)))
				{
					base.CreatedType = typeof(List<>).MakeGenericType(new Type[] { this.CollectionItemType });
				}
				this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
				this.StoreFSharpListCreatorIfNecessary(underlyingType);
				if (underlyingType.IsGenericType() && underlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					this._genericCollectionDefinitionType = type;
					this.IsReadOnlyOrFixedSize = false;
					this.ShouldCreateWrapper = false;
					flag = true;
				}
				else
				{
					this._genericCollectionDefinitionType = typeof(List<>).MakeGenericType(new Type[] { this.CollectionItemType });
					this.IsReadOnlyOrFixedSize = true;
					this.ShouldCreateWrapper = true;
					flag = this.HasParameterizedCreatorInternal;
				}
			}
			else
			{
				flag = false;
				this.ShouldCreateWrapper = true;
			}
			this.CanDeserialize = flag;
			Type type2;
			ObjectConstructor<object> objectConstructor;
			if (ImmutableCollectionsUtils.TryBuildImmutableForArrayContract(underlyingType, this.CollectionItemType, out type2, out objectConstructor))
			{
				base.CreatedType = type2;
				this._parameterizedCreator = objectConstructor;
				this.IsReadOnlyOrFixedSize = true;
				this.CanDeserialize = true;
			}
		}

		// Token: 0x06000DCA RID: 3530 RVA: 0x00055A80 File Offset: 0x00053C80
		internal IWrappedCollection CreateWrapper(object list)
		{
			if (this._genericWrapperCreator == null)
			{
				this._genericWrapperType = typeof(CollectionWrapper<>).MakeGenericType(new Type[] { this.CollectionItemType });
				Type type;
				if (ReflectionUtils.InheritsGenericDefinition(this._genericCollectionDefinitionType, typeof(List<>)) || this._genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					type = typeof(ICollection<>).MakeGenericType(new Type[] { this.CollectionItemType });
				}
				else
				{
					type = this._genericCollectionDefinitionType;
				}
				ConstructorInfo constructor = this._genericWrapperType.GetConstructor(new Type[] { type });
				this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
			}
			return (IWrappedCollection)this._genericWrapperCreator(new object[] { list });
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x00055B68 File Offset: 0x00053D68
		internal IList CreateTemporaryCollection()
		{
			if (this._genericTemporaryCollectionCreator == null)
			{
				Type type = ((this.IsMultidimensionalArray || this.CollectionItemType == null) ? typeof(object) : this.CollectionItemType);
				Type type2 = typeof(List<>).MakeGenericType(new Type[] { type });
				this._genericTemporaryCollectionCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type2);
			}
			return (IList)this._genericTemporaryCollectionCreator();
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x00055BF4 File Offset: 0x00053DF4
		private void StoreFSharpListCreatorIfNecessary(Type underlyingType)
		{
			if (!this.HasParameterizedCreatorInternal && underlyingType.Name == "FSharpList`1")
			{
				FSharpUtils.EnsureInitialized(underlyingType.Assembly());
				this._parameterizedCreator = FSharpUtils.CreateSeq(this.CollectionItemType);
			}
		}

		// Token: 0x0400055F RID: 1375
		private readonly Type _genericCollectionDefinitionType;

		// Token: 0x04000560 RID: 1376
		private Type _genericWrapperType;

		// Token: 0x04000561 RID: 1377
		private ObjectConstructor<object> _genericWrapperCreator;

		// Token: 0x04000562 RID: 1378
		private Func<object> _genericTemporaryCollectionCreator;

		// Token: 0x04000566 RID: 1382
		private readonly ConstructorInfo _parameterizedConstructor;

		// Token: 0x04000567 RID: 1383
		private ObjectConstructor<object> _parameterizedCreator;

		// Token: 0x04000568 RID: 1384
		private ObjectConstructor<object> _overrideCreator;
	}
}
