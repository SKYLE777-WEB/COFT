using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200011A RID: 282
	public class JsonObjectContract : JsonContainerContract
	{
		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000E32 RID: 3634 RVA: 0x000568E8 File Offset: 0x00054AE8
		// (set) Token: 0x06000E33 RID: 3635 RVA: 0x000568F0 File Offset: 0x00054AF0
		public MemberSerialization MemberSerialization { get; set; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000E34 RID: 3636 RVA: 0x000568FC File Offset: 0x00054AFC
		// (set) Token: 0x06000E35 RID: 3637 RVA: 0x00056904 File Offset: 0x00054B04
		public Required? ItemRequired { get; set; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000E36 RID: 3638 RVA: 0x00056910 File Offset: 0x00054B10
		public JsonPropertyCollection Properties { get; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000E37 RID: 3639 RVA: 0x00056918 File Offset: 0x00054B18
		public JsonPropertyCollection CreatorParameters
		{
			get
			{
				if (this._creatorParameters == null)
				{
					this._creatorParameters = new JsonPropertyCollection(base.UnderlyingType);
				}
				return this._creatorParameters;
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000E38 RID: 3640 RVA: 0x0005693C File Offset: 0x00054B3C
		// (set) Token: 0x06000E39 RID: 3641 RVA: 0x00056944 File Offset: 0x00054B44
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

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06000E3A RID: 3642 RVA: 0x00056950 File Offset: 0x00054B50
		// (set) Token: 0x06000E3B RID: 3643 RVA: 0x00056958 File Offset: 0x00054B58
		internal ObjectConstructor<object> ParameterizedCreator
		{
			get
			{
				return this._parameterizedCreator;
			}
			set
			{
				this._parameterizedCreator = value;
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06000E3C RID: 3644 RVA: 0x00056964 File Offset: 0x00054B64
		// (set) Token: 0x06000E3D RID: 3645 RVA: 0x0005696C File Offset: 0x00054B6C
		public ExtensionDataSetter ExtensionDataSetter { get; set; }

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000E3E RID: 3646 RVA: 0x00056978 File Offset: 0x00054B78
		// (set) Token: 0x06000E3F RID: 3647 RVA: 0x00056980 File Offset: 0x00054B80
		public ExtensionDataGetter ExtensionDataGetter { get; set; }

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000E40 RID: 3648 RVA: 0x0005698C File Offset: 0x00054B8C
		// (set) Token: 0x06000E41 RID: 3649 RVA: 0x00056994 File Offset: 0x00054B94
		public Type ExtensionDataValueType
		{
			get
			{
				return this._extensionDataValueType;
			}
			set
			{
				this._extensionDataValueType = value;
				this.ExtensionDataIsJToken = value != null && typeof(JToken).IsAssignableFrom(value);
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000E42 RID: 3650 RVA: 0x000569C8 File Offset: 0x00054BC8
		// (set) Token: 0x06000E43 RID: 3651 RVA: 0x000569D0 File Offset: 0x00054BD0
		public Func<string, string> ExtensionDataNameResolver { get; set; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000E44 RID: 3652 RVA: 0x000569DC File Offset: 0x00054BDC
		internal bool HasRequiredOrDefaultValueProperties
		{
			get
			{
				if (this._hasRequiredOrDefaultValueProperties == null)
				{
					this._hasRequiredOrDefaultValueProperties = new bool?(false);
					if (this.ItemRequired.GetValueOrDefault(Required.Default) != Required.Default)
					{
						this._hasRequiredOrDefaultValueProperties = new bool?(true);
					}
					else
					{
						foreach (JsonProperty jsonProperty in this.Properties)
						{
							if (jsonProperty.Required != Required.Default || (jsonProperty.DefaultValueHandling & DefaultValueHandling.Populate) == DefaultValueHandling.Populate)
							{
								this._hasRequiredOrDefaultValueProperties = new bool?(true);
								break;
							}
						}
					}
				}
				return this._hasRequiredOrDefaultValueProperties.GetValueOrDefault();
			}
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x00056AE8 File Offset: 0x00054CE8
		public JsonObjectContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Object;
			this.Properties = new JsonPropertyCollection(base.UnderlyingType);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00056B0C File Offset: 0x00054D0C
		[SecuritySafeCritical]
		internal object GetUninitializedObject()
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, this.NonNullableUnderlyingType));
			}
			return FormatterServices.GetUninitializedObject(this.NonNullableUnderlyingType);
		}

		// Token: 0x040005AA RID: 1450
		internal bool ExtensionDataIsJToken;

		// Token: 0x040005AB RID: 1451
		private bool? _hasRequiredOrDefaultValueProperties;

		// Token: 0x040005AC RID: 1452
		private ObjectConstructor<object> _overrideCreator;

		// Token: 0x040005AD RID: 1453
		private ObjectConstructor<object> _parameterizedCreator;

		// Token: 0x040005AE RID: 1454
		private JsonPropertyCollection _creatorParameters;

		// Token: 0x040005AF RID: 1455
		private Type _extensionDataValueType;
	}
}
