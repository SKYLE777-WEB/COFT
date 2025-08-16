using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000AE RID: 174
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class JsonPropertyAttribute : Attribute
	{
		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x00039DE0 File Offset: 0x00037FE0
		// (set) Token: 0x060007FE RID: 2046 RVA: 0x00039DE8 File Offset: 0x00037FE8
		public Type ItemConverterType { get; set; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x00039DF4 File Offset: 0x00037FF4
		// (set) Token: 0x06000800 RID: 2048 RVA: 0x00039DFC File Offset: 0x00037FFC
		public object[] ItemConverterParameters { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x00039E08 File Offset: 0x00038008
		// (set) Token: 0x06000802 RID: 2050 RVA: 0x00039E10 File Offset: 0x00038010
		public Type NamingStrategyType { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x00039E1C File Offset: 0x0003801C
		// (set) Token: 0x06000804 RID: 2052 RVA: 0x00039E24 File Offset: 0x00038024
		public object[] NamingStrategyParameters { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000805 RID: 2053 RVA: 0x00039E30 File Offset: 0x00038030
		// (set) Token: 0x06000806 RID: 2054 RVA: 0x00039E60 File Offset: 0x00038060
		public NullValueHandling NullValueHandling
		{
			get
			{
				NullValueHandling? nullValueHandling = this._nullValueHandling;
				if (nullValueHandling == null)
				{
					return NullValueHandling.Include;
				}
				return nullValueHandling.GetValueOrDefault();
			}
			set
			{
				this._nullValueHandling = new NullValueHandling?(value);
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000807 RID: 2055 RVA: 0x00039E70 File Offset: 0x00038070
		// (set) Token: 0x06000808 RID: 2056 RVA: 0x00039EA0 File Offset: 0x000380A0
		public DefaultValueHandling DefaultValueHandling
		{
			get
			{
				DefaultValueHandling? defaultValueHandling = this._defaultValueHandling;
				if (defaultValueHandling == null)
				{
					return DefaultValueHandling.Include;
				}
				return defaultValueHandling.GetValueOrDefault();
			}
			set
			{
				this._defaultValueHandling = new DefaultValueHandling?(value);
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000809 RID: 2057 RVA: 0x00039EB0 File Offset: 0x000380B0
		// (set) Token: 0x0600080A RID: 2058 RVA: 0x00039EE0 File Offset: 0x000380E0
		public ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				ReferenceLoopHandling? referenceLoopHandling = this._referenceLoopHandling;
				if (referenceLoopHandling == null)
				{
					return ReferenceLoopHandling.Error;
				}
				return referenceLoopHandling.GetValueOrDefault();
			}
			set
			{
				this._referenceLoopHandling = new ReferenceLoopHandling?(value);
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x0600080B RID: 2059 RVA: 0x00039EF0 File Offset: 0x000380F0
		// (set) Token: 0x0600080C RID: 2060 RVA: 0x00039F20 File Offset: 0x00038120
		public ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				ObjectCreationHandling? objectCreationHandling = this._objectCreationHandling;
				if (objectCreationHandling == null)
				{
					return ObjectCreationHandling.Auto;
				}
				return objectCreationHandling.GetValueOrDefault();
			}
			set
			{
				this._objectCreationHandling = new ObjectCreationHandling?(value);
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x0600080D RID: 2061 RVA: 0x00039F30 File Offset: 0x00038130
		// (set) Token: 0x0600080E RID: 2062 RVA: 0x00039F60 File Offset: 0x00038160
		public TypeNameHandling TypeNameHandling
		{
			get
			{
				TypeNameHandling? typeNameHandling = this._typeNameHandling;
				if (typeNameHandling == null)
				{
					return TypeNameHandling.None;
				}
				return typeNameHandling.GetValueOrDefault();
			}
			set
			{
				this._typeNameHandling = new TypeNameHandling?(value);
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x0600080F RID: 2063 RVA: 0x00039F70 File Offset: 0x00038170
		// (set) Token: 0x06000810 RID: 2064 RVA: 0x00039FA0 File Offset: 0x000381A0
		public bool IsReference
		{
			get
			{
				return this._isReference ?? false;
			}
			set
			{
				this._isReference = new bool?(value);
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000811 RID: 2065 RVA: 0x00039FB0 File Offset: 0x000381B0
		// (set) Token: 0x06000812 RID: 2066 RVA: 0x00039FE0 File Offset: 0x000381E0
		public int Order
		{
			get
			{
				int? order = this._order;
				if (order == null)
				{
					return 0;
				}
				return order.GetValueOrDefault();
			}
			set
			{
				this._order = new int?(value);
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000813 RID: 2067 RVA: 0x00039FF0 File Offset: 0x000381F0
		// (set) Token: 0x06000814 RID: 2068 RVA: 0x0003A020 File Offset: 0x00038220
		public Required Required
		{
			get
			{
				Required? required = this._required;
				if (required == null)
				{
					return Required.Default;
				}
				return required.GetValueOrDefault();
			}
			set
			{
				this._required = new Required?(value);
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000815 RID: 2069 RVA: 0x0003A030 File Offset: 0x00038230
		// (set) Token: 0x06000816 RID: 2070 RVA: 0x0003A038 File Offset: 0x00038238
		public string PropertyName { get; set; }

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000817 RID: 2071 RVA: 0x0003A044 File Offset: 0x00038244
		// (set) Token: 0x06000818 RID: 2072 RVA: 0x0003A074 File Offset: 0x00038274
		public ReferenceLoopHandling ItemReferenceLoopHandling
		{
			get
			{
				ReferenceLoopHandling? itemReferenceLoopHandling = this._itemReferenceLoopHandling;
				if (itemReferenceLoopHandling == null)
				{
					return ReferenceLoopHandling.Error;
				}
				return itemReferenceLoopHandling.GetValueOrDefault();
			}
			set
			{
				this._itemReferenceLoopHandling = new ReferenceLoopHandling?(value);
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000819 RID: 2073 RVA: 0x0003A084 File Offset: 0x00038284
		// (set) Token: 0x0600081A RID: 2074 RVA: 0x0003A0B4 File Offset: 0x000382B4
		public TypeNameHandling ItemTypeNameHandling
		{
			get
			{
				TypeNameHandling? itemTypeNameHandling = this._itemTypeNameHandling;
				if (itemTypeNameHandling == null)
				{
					return TypeNameHandling.None;
				}
				return itemTypeNameHandling.GetValueOrDefault();
			}
			set
			{
				this._itemTypeNameHandling = new TypeNameHandling?(value);
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600081B RID: 2075 RVA: 0x0003A0C4 File Offset: 0x000382C4
		// (set) Token: 0x0600081C RID: 2076 RVA: 0x0003A0F4 File Offset: 0x000382F4
		public bool ItemIsReference
		{
			get
			{
				return this._itemIsReference ?? false;
			}
			set
			{
				this._itemIsReference = new bool?(value);
			}
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0003A104 File Offset: 0x00038304
		public JsonPropertyAttribute()
		{
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x0003A10C File Offset: 0x0003830C
		public JsonPropertyAttribute(string propertyName)
		{
			this.PropertyName = propertyName;
		}

		// Token: 0x0400037F RID: 895
		internal NullValueHandling? _nullValueHandling;

		// Token: 0x04000380 RID: 896
		internal DefaultValueHandling? _defaultValueHandling;

		// Token: 0x04000381 RID: 897
		internal ReferenceLoopHandling? _referenceLoopHandling;

		// Token: 0x04000382 RID: 898
		internal ObjectCreationHandling? _objectCreationHandling;

		// Token: 0x04000383 RID: 899
		internal TypeNameHandling? _typeNameHandling;

		// Token: 0x04000384 RID: 900
		internal bool? _isReference;

		// Token: 0x04000385 RID: 901
		internal int? _order;

		// Token: 0x04000386 RID: 902
		internal Required? _required;

		// Token: 0x04000387 RID: 903
		internal bool? _itemIsReference;

		// Token: 0x04000388 RID: 904
		internal ReferenceLoopHandling? _itemReferenceLoopHandling;

		// Token: 0x04000389 RID: 905
		internal TypeNameHandling? _itemTypeNameHandling;
	}
}
