using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200010E RID: 270
	public class JsonContainerContract : JsonContract
	{
		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x00055C34 File Offset: 0x00053E34
		// (set) Token: 0x06000DCE RID: 3534 RVA: 0x00055C3C File Offset: 0x00053E3C
		internal JsonContract ItemContract
		{
			get
			{
				return this._itemContract;
			}
			set
			{
				this._itemContract = value;
				if (this._itemContract != null)
				{
					this._finalItemContract = (this._itemContract.UnderlyingType.IsSealed() ? this._itemContract : null);
					return;
				}
				this._finalItemContract = null;
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00055C90 File Offset: 0x00053E90
		internal JsonContract FinalItemContract
		{
			get
			{
				return this._finalItemContract;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x00055C98 File Offset: 0x00053E98
		// (set) Token: 0x06000DD1 RID: 3537 RVA: 0x00055CA0 File Offset: 0x00053EA0
		public JsonConverter ItemConverter { get; set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x00055CAC File Offset: 0x00053EAC
		// (set) Token: 0x06000DD3 RID: 3539 RVA: 0x00055CB4 File Offset: 0x00053EB4
		public bool? ItemIsReference { get; set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000DD4 RID: 3540 RVA: 0x00055CC0 File Offset: 0x00053EC0
		// (set) Token: 0x06000DD5 RID: 3541 RVA: 0x00055CC8 File Offset: 0x00053EC8
		public ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x00055CD4 File Offset: 0x00053ED4
		// (set) Token: 0x06000DD7 RID: 3543 RVA: 0x00055CDC File Offset: 0x00053EDC
		public TypeNameHandling? ItemTypeNameHandling { get; set; }

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00055CE8 File Offset: 0x00053EE8
		internal JsonContainerContract(Type underlyingType)
			: base(underlyingType)
		{
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(underlyingType);
			if (cachedAttribute != null)
			{
				if (cachedAttribute.ItemConverterType != null)
				{
					this.ItemConverter = JsonTypeReflector.CreateJsonConverterInstance(cachedAttribute.ItemConverterType, cachedAttribute.ItemConverterParameters);
				}
				this.ItemIsReference = cachedAttribute._itemIsReference;
				this.ItemReferenceLoopHandling = cachedAttribute._itemReferenceLoopHandling;
				this.ItemTypeNameHandling = cachedAttribute._itemTypeNameHandling;
			}
		}

		// Token: 0x0400056A RID: 1386
		private JsonContract _itemContract;

		// Token: 0x0400056B RID: 1387
		private JsonContract _finalItemContract;
	}
}
