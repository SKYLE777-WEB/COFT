using System;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json
{
	// Token: 0x020000A2 RID: 162
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	public abstract class JsonContainerAttribute : Attribute
	{
		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000777 RID: 1911 RVA: 0x00038D3C File Offset: 0x00036F3C
		// (set) Token: 0x06000778 RID: 1912 RVA: 0x00038D44 File Offset: 0x00036F44
		public string Id { get; set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000779 RID: 1913 RVA: 0x00038D50 File Offset: 0x00036F50
		// (set) Token: 0x0600077A RID: 1914 RVA: 0x00038D58 File Offset: 0x00036F58
		public string Title { get; set; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x00038D64 File Offset: 0x00036F64
		// (set) Token: 0x0600077C RID: 1916 RVA: 0x00038D6C File Offset: 0x00036F6C
		public string Description { get; set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x00038D78 File Offset: 0x00036F78
		// (set) Token: 0x0600077E RID: 1918 RVA: 0x00038D80 File Offset: 0x00036F80
		public Type ItemConverterType { get; set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x00038D8C File Offset: 0x00036F8C
		// (set) Token: 0x06000780 RID: 1920 RVA: 0x00038D94 File Offset: 0x00036F94
		public object[] ItemConverterParameters { get; set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000781 RID: 1921 RVA: 0x00038DA0 File Offset: 0x00036FA0
		// (set) Token: 0x06000782 RID: 1922 RVA: 0x00038DA8 File Offset: 0x00036FA8
		public Type NamingStrategyType
		{
			get
			{
				return this._namingStrategyType;
			}
			set
			{
				this._namingStrategyType = value;
				this.NamingStrategyInstance = null;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x00038DB8 File Offset: 0x00036FB8
		// (set) Token: 0x06000784 RID: 1924 RVA: 0x00038DC0 File Offset: 0x00036FC0
		public object[] NamingStrategyParameters
		{
			get
			{
				return this._namingStrategyParameters;
			}
			set
			{
				this._namingStrategyParameters = value;
				this.NamingStrategyInstance = null;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x00038DD0 File Offset: 0x00036FD0
		// (set) Token: 0x06000786 RID: 1926 RVA: 0x00038DD8 File Offset: 0x00036FD8
		internal NamingStrategy NamingStrategyInstance { get; set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000787 RID: 1927 RVA: 0x00038DE4 File Offset: 0x00036FE4
		// (set) Token: 0x06000788 RID: 1928 RVA: 0x00038E14 File Offset: 0x00037014
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

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000789 RID: 1929 RVA: 0x00038E24 File Offset: 0x00037024
		// (set) Token: 0x0600078A RID: 1930 RVA: 0x00038E54 File Offset: 0x00037054
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

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x00038E64 File Offset: 0x00037064
		// (set) Token: 0x0600078C RID: 1932 RVA: 0x00038E94 File Offset: 0x00037094
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

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x00038EA4 File Offset: 0x000370A4
		// (set) Token: 0x0600078E RID: 1934 RVA: 0x00038ED4 File Offset: 0x000370D4
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

		// Token: 0x0600078F RID: 1935 RVA: 0x00038EE4 File Offset: 0x000370E4
		protected JsonContainerAttribute()
		{
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x00038EEC File Offset: 0x000370EC
		protected JsonContainerAttribute(string id)
		{
			this.Id = id;
		}

		// Token: 0x04000361 RID: 865
		internal bool? _isReference;

		// Token: 0x04000362 RID: 866
		internal bool? _itemIsReference;

		// Token: 0x04000363 RID: 867
		internal ReferenceLoopHandling? _itemReferenceLoopHandling;

		// Token: 0x04000364 RID: 868
		internal TypeNameHandling? _itemTypeNameHandling;

		// Token: 0x04000365 RID: 869
		private Type _namingStrategyType;

		// Token: 0x04000366 RID: 870
		private object[] _namingStrategyParameters;
	}
}
