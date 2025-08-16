using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200011C RID: 284
	public class JsonProperty
	{
		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x00056CA0 File Offset: 0x00054EA0
		// (set) Token: 0x06000E4C RID: 3660 RVA: 0x00056CA8 File Offset: 0x00054EA8
		internal JsonContract PropertyContract { get; set; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000E4D RID: 3661 RVA: 0x00056CB4 File Offset: 0x00054EB4
		// (set) Token: 0x06000E4E RID: 3662 RVA: 0x00056CBC File Offset: 0x00054EBC
		public string PropertyName
		{
			get
			{
				return this._propertyName;
			}
			set
			{
				this._propertyName = value;
				this._skipPropertyNameEscape = !JavaScriptUtils.ShouldEscapeJavaScriptString(this._propertyName, JavaScriptUtils.HtmlCharEscapeFlags);
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000E4F RID: 3663 RVA: 0x00056CE0 File Offset: 0x00054EE0
		// (set) Token: 0x06000E50 RID: 3664 RVA: 0x00056CE8 File Offset: 0x00054EE8
		public Type DeclaringType { get; set; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000E51 RID: 3665 RVA: 0x00056CF4 File Offset: 0x00054EF4
		// (set) Token: 0x06000E52 RID: 3666 RVA: 0x00056CFC File Offset: 0x00054EFC
		public int? Order { get; set; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000E53 RID: 3667 RVA: 0x00056D08 File Offset: 0x00054F08
		// (set) Token: 0x06000E54 RID: 3668 RVA: 0x00056D10 File Offset: 0x00054F10
		public string UnderlyingName { get; set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000E55 RID: 3669 RVA: 0x00056D1C File Offset: 0x00054F1C
		// (set) Token: 0x06000E56 RID: 3670 RVA: 0x00056D24 File Offset: 0x00054F24
		public IValueProvider ValueProvider { get; set; }

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000E57 RID: 3671 RVA: 0x00056D30 File Offset: 0x00054F30
		// (set) Token: 0x06000E58 RID: 3672 RVA: 0x00056D38 File Offset: 0x00054F38
		public IAttributeProvider AttributeProvider { get; set; }

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000E59 RID: 3673 RVA: 0x00056D44 File Offset: 0x00054F44
		// (set) Token: 0x06000E5A RID: 3674 RVA: 0x00056D4C File Offset: 0x00054F4C
		public Type PropertyType
		{
			get
			{
				return this._propertyType;
			}
			set
			{
				if (this._propertyType != value)
				{
					this._propertyType = value;
					this._hasGeneratedDefaultValue = false;
				}
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000E5B RID: 3675 RVA: 0x00056D70 File Offset: 0x00054F70
		// (set) Token: 0x06000E5C RID: 3676 RVA: 0x00056D78 File Offset: 0x00054F78
		public JsonConverter Converter { get; set; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000E5D RID: 3677 RVA: 0x00056D84 File Offset: 0x00054F84
		// (set) Token: 0x06000E5E RID: 3678 RVA: 0x00056D8C File Offset: 0x00054F8C
		public JsonConverter MemberConverter { get; set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000E5F RID: 3679 RVA: 0x00056D98 File Offset: 0x00054F98
		// (set) Token: 0x06000E60 RID: 3680 RVA: 0x00056DA0 File Offset: 0x00054FA0
		public bool Ignored { get; set; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000E61 RID: 3681 RVA: 0x00056DAC File Offset: 0x00054FAC
		// (set) Token: 0x06000E62 RID: 3682 RVA: 0x00056DB4 File Offset: 0x00054FB4
		public bool Readable { get; set; }

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000E63 RID: 3683 RVA: 0x00056DC0 File Offset: 0x00054FC0
		// (set) Token: 0x06000E64 RID: 3684 RVA: 0x00056DC8 File Offset: 0x00054FC8
		public bool Writable { get; set; }

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000E65 RID: 3685 RVA: 0x00056DD4 File Offset: 0x00054FD4
		// (set) Token: 0x06000E66 RID: 3686 RVA: 0x00056DDC File Offset: 0x00054FDC
		public bool HasMemberAttribute { get; set; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000E67 RID: 3687 RVA: 0x00056DE8 File Offset: 0x00054FE8
		// (set) Token: 0x06000E68 RID: 3688 RVA: 0x00056E00 File Offset: 0x00055000
		public object DefaultValue
		{
			get
			{
				if (!this._hasExplicitDefaultValue)
				{
					return null;
				}
				return this._defaultValue;
			}
			set
			{
				this._hasExplicitDefaultValue = true;
				this._defaultValue = value;
			}
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x00056E10 File Offset: 0x00055010
		internal object GetResolvedDefaultValue()
		{
			if (this._propertyType == null)
			{
				return null;
			}
			if (!this._hasExplicitDefaultValue && !this._hasGeneratedDefaultValue)
			{
				this._defaultValue = ReflectionUtils.GetDefaultValue(this.PropertyType);
				this._hasGeneratedDefaultValue = true;
			}
			return this._defaultValue;
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000E6A RID: 3690 RVA: 0x00056E68 File Offset: 0x00055068
		// (set) Token: 0x06000E6B RID: 3691 RVA: 0x00056E98 File Offset: 0x00055098
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

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000E6C RID: 3692 RVA: 0x00056EA8 File Offset: 0x000550A8
		// (set) Token: 0x06000E6D RID: 3693 RVA: 0x00056EB0 File Offset: 0x000550B0
		public bool? IsReference { get; set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000E6E RID: 3694 RVA: 0x00056EBC File Offset: 0x000550BC
		// (set) Token: 0x06000E6F RID: 3695 RVA: 0x00056EC4 File Offset: 0x000550C4
		public NullValueHandling? NullValueHandling { get; set; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000E70 RID: 3696 RVA: 0x00056ED0 File Offset: 0x000550D0
		// (set) Token: 0x06000E71 RID: 3697 RVA: 0x00056ED8 File Offset: 0x000550D8
		public DefaultValueHandling? DefaultValueHandling { get; set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000E72 RID: 3698 RVA: 0x00056EE4 File Offset: 0x000550E4
		// (set) Token: 0x06000E73 RID: 3699 RVA: 0x00056EEC File Offset: 0x000550EC
		public ReferenceLoopHandling? ReferenceLoopHandling { get; set; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000E74 RID: 3700 RVA: 0x00056EF8 File Offset: 0x000550F8
		// (set) Token: 0x06000E75 RID: 3701 RVA: 0x00056F00 File Offset: 0x00055100
		public ObjectCreationHandling? ObjectCreationHandling { get; set; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000E76 RID: 3702 RVA: 0x00056F0C File Offset: 0x0005510C
		// (set) Token: 0x06000E77 RID: 3703 RVA: 0x00056F14 File Offset: 0x00055114
		public TypeNameHandling? TypeNameHandling { get; set; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000E78 RID: 3704 RVA: 0x00056F20 File Offset: 0x00055120
		// (set) Token: 0x06000E79 RID: 3705 RVA: 0x00056F28 File Offset: 0x00055128
		public Predicate<object> ShouldSerialize { get; set; }

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000E7A RID: 3706 RVA: 0x00056F34 File Offset: 0x00055134
		// (set) Token: 0x06000E7B RID: 3707 RVA: 0x00056F3C File Offset: 0x0005513C
		public Predicate<object> ShouldDeserialize { get; set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000E7C RID: 3708 RVA: 0x00056F48 File Offset: 0x00055148
		// (set) Token: 0x06000E7D RID: 3709 RVA: 0x00056F50 File Offset: 0x00055150
		public Predicate<object> GetIsSpecified { get; set; }

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000E7E RID: 3710 RVA: 0x00056F5C File Offset: 0x0005515C
		// (set) Token: 0x06000E7F RID: 3711 RVA: 0x00056F64 File Offset: 0x00055164
		public Action<object, object> SetIsSpecified { get; set; }

		// Token: 0x06000E80 RID: 3712 RVA: 0x00056F70 File Offset: 0x00055170
		public override string ToString()
		{
			return this.PropertyName;
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000E81 RID: 3713 RVA: 0x00056F78 File Offset: 0x00055178
		// (set) Token: 0x06000E82 RID: 3714 RVA: 0x00056F80 File Offset: 0x00055180
		public JsonConverter ItemConverter { get; set; }

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000E83 RID: 3715 RVA: 0x00056F8C File Offset: 0x0005518C
		// (set) Token: 0x06000E84 RID: 3716 RVA: 0x00056F94 File Offset: 0x00055194
		public bool? ItemIsReference { get; set; }

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000E85 RID: 3717 RVA: 0x00056FA0 File Offset: 0x000551A0
		// (set) Token: 0x06000E86 RID: 3718 RVA: 0x00056FA8 File Offset: 0x000551A8
		public TypeNameHandling? ItemTypeNameHandling { get; set; }

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000E87 RID: 3719 RVA: 0x00056FB4 File Offset: 0x000551B4
		// (set) Token: 0x06000E88 RID: 3720 RVA: 0x00056FBC File Offset: 0x000551BC
		public ReferenceLoopHandling? ItemReferenceLoopHandling { get; set; }

		// Token: 0x06000E89 RID: 3721 RVA: 0x00056FC8 File Offset: 0x000551C8
		internal void WritePropertyName(JsonWriter writer)
		{
			if (this._skipPropertyNameEscape)
			{
				writer.WritePropertyName(this.PropertyName, false);
				return;
			}
			writer.WritePropertyName(this.PropertyName);
		}

		// Token: 0x040005B2 RID: 1458
		internal Required? _required;

		// Token: 0x040005B3 RID: 1459
		internal bool _hasExplicitDefaultValue;

		// Token: 0x040005B4 RID: 1460
		private object _defaultValue;

		// Token: 0x040005B5 RID: 1461
		private bool _hasGeneratedDefaultValue;

		// Token: 0x040005B6 RID: 1462
		private string _propertyName;

		// Token: 0x040005B7 RID: 1463
		internal bool _skipPropertyNameEscape;

		// Token: 0x040005B8 RID: 1464
		private Type _propertyType;
	}
}
