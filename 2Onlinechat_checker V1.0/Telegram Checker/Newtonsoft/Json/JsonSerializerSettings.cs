using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json
{
	// Token: 0x020000B4 RID: 180
	public class JsonSerializerSettings
	{
		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060008DC RID: 2268 RVA: 0x0003CE38 File Offset: 0x0003B038
		// (set) Token: 0x060008DD RID: 2269 RVA: 0x0003CE68 File Offset: 0x0003B068
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

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060008DE RID: 2270 RVA: 0x0003CE78 File Offset: 0x0003B078
		// (set) Token: 0x060008DF RID: 2271 RVA: 0x0003CEA8 File Offset: 0x0003B0A8
		public MissingMemberHandling MissingMemberHandling
		{
			get
			{
				MissingMemberHandling? missingMemberHandling = this._missingMemberHandling;
				if (missingMemberHandling == null)
				{
					return MissingMemberHandling.Ignore;
				}
				return missingMemberHandling.GetValueOrDefault();
			}
			set
			{
				this._missingMemberHandling = new MissingMemberHandling?(value);
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060008E0 RID: 2272 RVA: 0x0003CEB8 File Offset: 0x0003B0B8
		// (set) Token: 0x060008E1 RID: 2273 RVA: 0x0003CEE8 File Offset: 0x0003B0E8
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

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060008E2 RID: 2274 RVA: 0x0003CEF8 File Offset: 0x0003B0F8
		// (set) Token: 0x060008E3 RID: 2275 RVA: 0x0003CF28 File Offset: 0x0003B128
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

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060008E4 RID: 2276 RVA: 0x0003CF38 File Offset: 0x0003B138
		// (set) Token: 0x060008E5 RID: 2277 RVA: 0x0003CF68 File Offset: 0x0003B168
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

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060008E6 RID: 2278 RVA: 0x0003CF78 File Offset: 0x0003B178
		// (set) Token: 0x060008E7 RID: 2279 RVA: 0x0003CF80 File Offset: 0x0003B180
		public IList<JsonConverter> Converters { get; set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x0003CF8C File Offset: 0x0003B18C
		// (set) Token: 0x060008E9 RID: 2281 RVA: 0x0003CFBC File Offset: 0x0003B1BC
		public PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				PreserveReferencesHandling? preserveReferencesHandling = this._preserveReferencesHandling;
				if (preserveReferencesHandling == null)
				{
					return PreserveReferencesHandling.None;
				}
				return preserveReferencesHandling.GetValueOrDefault();
			}
			set
			{
				this._preserveReferencesHandling = new PreserveReferencesHandling?(value);
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060008EA RID: 2282 RVA: 0x0003CFCC File Offset: 0x0003B1CC
		// (set) Token: 0x060008EB RID: 2283 RVA: 0x0003CFFC File Offset: 0x0003B1FC
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

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x060008EC RID: 2284 RVA: 0x0003D00C File Offset: 0x0003B20C
		// (set) Token: 0x060008ED RID: 2285 RVA: 0x0003D03C File Offset: 0x0003B23C
		public MetadataPropertyHandling MetadataPropertyHandling
		{
			get
			{
				MetadataPropertyHandling? metadataPropertyHandling = this._metadataPropertyHandling;
				if (metadataPropertyHandling == null)
				{
					return MetadataPropertyHandling.Default;
				}
				return metadataPropertyHandling.GetValueOrDefault();
			}
			set
			{
				this._metadataPropertyHandling = new MetadataPropertyHandling?(value);
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x060008EE RID: 2286 RVA: 0x0003D04C File Offset: 0x0003B24C
		// (set) Token: 0x060008EF RID: 2287 RVA: 0x0003D054 File Offset: 0x0003B254
		[Obsolete("TypeNameAssemblyFormat is obsolete. Use TypeNameAssemblyFormatHandling instead.")]
		public FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				return (FormatterAssemblyStyle)this.TypeNameAssemblyFormatHandling;
			}
			set
			{
				this.TypeNameAssemblyFormatHandling = (TypeNameAssemblyFormatHandling)value;
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x0003D060 File Offset: 0x0003B260
		// (set) Token: 0x060008F1 RID: 2289 RVA: 0x0003D090 File Offset: 0x0003B290
		public TypeNameAssemblyFormatHandling TypeNameAssemblyFormatHandling
		{
			get
			{
				TypeNameAssemblyFormatHandling? typeNameAssemblyFormatHandling = this._typeNameAssemblyFormatHandling;
				if (typeNameAssemblyFormatHandling == null)
				{
					return TypeNameAssemblyFormatHandling.Simple;
				}
				return typeNameAssemblyFormatHandling.GetValueOrDefault();
			}
			set
			{
				this._typeNameAssemblyFormatHandling = new TypeNameAssemblyFormatHandling?(value);
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x060008F2 RID: 2290 RVA: 0x0003D0A0 File Offset: 0x0003B2A0
		// (set) Token: 0x060008F3 RID: 2291 RVA: 0x0003D0D0 File Offset: 0x0003B2D0
		public ConstructorHandling ConstructorHandling
		{
			get
			{
				ConstructorHandling? constructorHandling = this._constructorHandling;
				if (constructorHandling == null)
				{
					return ConstructorHandling.Default;
				}
				return constructorHandling.GetValueOrDefault();
			}
			set
			{
				this._constructorHandling = new ConstructorHandling?(value);
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x0003D0E0 File Offset: 0x0003B2E0
		// (set) Token: 0x060008F5 RID: 2293 RVA: 0x0003D0E8 File Offset: 0x0003B2E8
		public IContractResolver ContractResolver { get; set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x0003D0F4 File Offset: 0x0003B2F4
		// (set) Token: 0x060008F7 RID: 2295 RVA: 0x0003D0FC File Offset: 0x0003B2FC
		public IEqualityComparer EqualityComparer { get; set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x060008F8 RID: 2296 RVA: 0x0003D108 File Offset: 0x0003B308
		// (set) Token: 0x060008F9 RID: 2297 RVA: 0x0003D120 File Offset: 0x0003B320
		[Obsolete("ReferenceResolver property is obsolete. Use the ReferenceResolverProvider property to set the IReferenceResolver: settings.ReferenceResolverProvider = () => resolver")]
		public IReferenceResolver ReferenceResolver
		{
			get
			{
				Func<IReferenceResolver> referenceResolverProvider = this.ReferenceResolverProvider;
				if (referenceResolverProvider == null)
				{
					return null;
				}
				return referenceResolverProvider();
			}
			set
			{
				this.ReferenceResolverProvider = ((value != null) ? (() => value) : null);
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x0003D164 File Offset: 0x0003B364
		// (set) Token: 0x060008FB RID: 2299 RVA: 0x0003D16C File Offset: 0x0003B36C
		public Func<IReferenceResolver> ReferenceResolverProvider { get; set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x0003D178 File Offset: 0x0003B378
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x0003D180 File Offset: 0x0003B380
		public ITraceWriter TraceWriter { get; set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x060008FE RID: 2302 RVA: 0x0003D18C File Offset: 0x0003B38C
		// (set) Token: 0x060008FF RID: 2303 RVA: 0x0003D1D0 File Offset: 0x0003B3D0
		[Obsolete("Binder is obsolete. Use SerializationBinder instead.")]
		public SerializationBinder Binder
		{
			get
			{
				if (this.SerializationBinder == null)
				{
					return null;
				}
				SerializationBinderAdapter serializationBinderAdapter = this.SerializationBinder as SerializationBinderAdapter;
				if (serializationBinderAdapter != null)
				{
					return serializationBinderAdapter.SerializationBinder;
				}
				throw new InvalidOperationException("Cannot get SerializationBinder because an ISerializationBinder was previously set.");
			}
			set
			{
				this.SerializationBinder = ((value == null) ? null : new SerializationBinderAdapter(value));
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x0003D1EC File Offset: 0x0003B3EC
		// (set) Token: 0x06000901 RID: 2305 RVA: 0x0003D1F4 File Offset: 0x0003B3F4
		public ISerializationBinder SerializationBinder { get; set; }

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x0003D200 File Offset: 0x0003B400
		// (set) Token: 0x06000903 RID: 2307 RVA: 0x0003D208 File Offset: 0x0003B408
		public EventHandler<ErrorEventArgs> Error { get; set; }

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x0003D214 File Offset: 0x0003B414
		// (set) Token: 0x06000905 RID: 2309 RVA: 0x0003D248 File Offset: 0x0003B448
		public StreamingContext Context
		{
			get
			{
				StreamingContext? context = this._context;
				if (context == null)
				{
					return JsonSerializerSettings.DefaultContext;
				}
				return context.GetValueOrDefault();
			}
			set
			{
				this._context = new StreamingContext?(value);
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000906 RID: 2310 RVA: 0x0003D258 File Offset: 0x0003B458
		// (set) Token: 0x06000907 RID: 2311 RVA: 0x0003D26C File Offset: 0x0003B46C
		public string DateFormatString
		{
			get
			{
				return this._dateFormatString ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
			}
			set
			{
				this._dateFormatString = value;
				this._dateFormatStringSet = true;
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000908 RID: 2312 RVA: 0x0003D27C File Offset: 0x0003B47C
		// (set) Token: 0x06000909 RID: 2313 RVA: 0x0003D284 File Offset: 0x0003B484
		public int? MaxDepth
		{
			get
			{
				return this._maxDepth;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Value must be positive.", "value");
				}
				this._maxDepth = value;
				this._maxDepthSet = true;
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x0600090A RID: 2314 RVA: 0x0003D2D8 File Offset: 0x0003B4D8
		// (set) Token: 0x0600090B RID: 2315 RVA: 0x0003D308 File Offset: 0x0003B508
		public Formatting Formatting
		{
			get
			{
				Formatting? formatting = this._formatting;
				if (formatting == null)
				{
					return Formatting.None;
				}
				return formatting.GetValueOrDefault();
			}
			set
			{
				this._formatting = new Formatting?(value);
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600090C RID: 2316 RVA: 0x0003D318 File Offset: 0x0003B518
		// (set) Token: 0x0600090D RID: 2317 RVA: 0x0003D348 File Offset: 0x0003B548
		public DateFormatHandling DateFormatHandling
		{
			get
			{
				DateFormatHandling? dateFormatHandling = this._dateFormatHandling;
				if (dateFormatHandling == null)
				{
					return DateFormatHandling.IsoDateFormat;
				}
				return dateFormatHandling.GetValueOrDefault();
			}
			set
			{
				this._dateFormatHandling = new DateFormatHandling?(value);
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600090E RID: 2318 RVA: 0x0003D358 File Offset: 0x0003B558
		// (set) Token: 0x0600090F RID: 2319 RVA: 0x0003D388 File Offset: 0x0003B588
		public DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				DateTimeZoneHandling? dateTimeZoneHandling = this._dateTimeZoneHandling;
				if (dateTimeZoneHandling == null)
				{
					return DateTimeZoneHandling.RoundtripKind;
				}
				return dateTimeZoneHandling.GetValueOrDefault();
			}
			set
			{
				this._dateTimeZoneHandling = new DateTimeZoneHandling?(value);
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000910 RID: 2320 RVA: 0x0003D398 File Offset: 0x0003B598
		// (set) Token: 0x06000911 RID: 2321 RVA: 0x0003D3C8 File Offset: 0x0003B5C8
		public DateParseHandling DateParseHandling
		{
			get
			{
				DateParseHandling? dateParseHandling = this._dateParseHandling;
				if (dateParseHandling == null)
				{
					return DateParseHandling.DateTime;
				}
				return dateParseHandling.GetValueOrDefault();
			}
			set
			{
				this._dateParseHandling = new DateParseHandling?(value);
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000912 RID: 2322 RVA: 0x0003D3D8 File Offset: 0x0003B5D8
		// (set) Token: 0x06000913 RID: 2323 RVA: 0x0003D408 File Offset: 0x0003B608
		public FloatFormatHandling FloatFormatHandling
		{
			get
			{
				FloatFormatHandling? floatFormatHandling = this._floatFormatHandling;
				if (floatFormatHandling == null)
				{
					return FloatFormatHandling.String;
				}
				return floatFormatHandling.GetValueOrDefault();
			}
			set
			{
				this._floatFormatHandling = new FloatFormatHandling?(value);
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000914 RID: 2324 RVA: 0x0003D418 File Offset: 0x0003B618
		// (set) Token: 0x06000915 RID: 2325 RVA: 0x0003D448 File Offset: 0x0003B648
		public FloatParseHandling FloatParseHandling
		{
			get
			{
				FloatParseHandling? floatParseHandling = this._floatParseHandling;
				if (floatParseHandling == null)
				{
					return FloatParseHandling.Double;
				}
				return floatParseHandling.GetValueOrDefault();
			}
			set
			{
				this._floatParseHandling = new FloatParseHandling?(value);
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000916 RID: 2326 RVA: 0x0003D458 File Offset: 0x0003B658
		// (set) Token: 0x06000917 RID: 2327 RVA: 0x0003D488 File Offset: 0x0003B688
		public StringEscapeHandling StringEscapeHandling
		{
			get
			{
				StringEscapeHandling? stringEscapeHandling = this._stringEscapeHandling;
				if (stringEscapeHandling == null)
				{
					return StringEscapeHandling.Default;
				}
				return stringEscapeHandling.GetValueOrDefault();
			}
			set
			{
				this._stringEscapeHandling = new StringEscapeHandling?(value);
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000918 RID: 2328 RVA: 0x0003D498 File Offset: 0x0003B698
		// (set) Token: 0x06000919 RID: 2329 RVA: 0x0003D4AC File Offset: 0x0003B6AC
		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? JsonSerializerSettings.DefaultCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600091A RID: 2330 RVA: 0x0003D4B8 File Offset: 0x0003B6B8
		// (set) Token: 0x0600091B RID: 2331 RVA: 0x0003D4E8 File Offset: 0x0003B6E8
		public bool CheckAdditionalContent
		{
			get
			{
				return this._checkAdditionalContent ?? false;
			}
			set
			{
				this._checkAdditionalContent = new bool?(value);
			}
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0003D510 File Offset: 0x0003B710
		public JsonSerializerSettings()
		{
			this.Converters = new List<JsonConverter>();
		}

		// Token: 0x040003C0 RID: 960
		internal const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;

		// Token: 0x040003C1 RID: 961
		internal const MissingMemberHandling DefaultMissingMemberHandling = MissingMemberHandling.Ignore;

		// Token: 0x040003C2 RID: 962
		internal const NullValueHandling DefaultNullValueHandling = NullValueHandling.Include;

		// Token: 0x040003C3 RID: 963
		internal const DefaultValueHandling DefaultDefaultValueHandling = DefaultValueHandling.Include;

		// Token: 0x040003C4 RID: 964
		internal const ObjectCreationHandling DefaultObjectCreationHandling = ObjectCreationHandling.Auto;

		// Token: 0x040003C5 RID: 965
		internal const PreserveReferencesHandling DefaultPreserveReferencesHandling = PreserveReferencesHandling.None;

		// Token: 0x040003C6 RID: 966
		internal const ConstructorHandling DefaultConstructorHandling = ConstructorHandling.Default;

		// Token: 0x040003C7 RID: 967
		internal const TypeNameHandling DefaultTypeNameHandling = TypeNameHandling.None;

		// Token: 0x040003C8 RID: 968
		internal const MetadataPropertyHandling DefaultMetadataPropertyHandling = MetadataPropertyHandling.Default;

		// Token: 0x040003C9 RID: 969
		internal static readonly StreamingContext DefaultContext = default(StreamingContext);

		// Token: 0x040003CA RID: 970
		internal const Formatting DefaultFormatting = Formatting.None;

		// Token: 0x040003CB RID: 971
		internal const DateFormatHandling DefaultDateFormatHandling = DateFormatHandling.IsoDateFormat;

		// Token: 0x040003CC RID: 972
		internal const DateTimeZoneHandling DefaultDateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

		// Token: 0x040003CD RID: 973
		internal const DateParseHandling DefaultDateParseHandling = DateParseHandling.DateTime;

		// Token: 0x040003CE RID: 974
		internal const FloatParseHandling DefaultFloatParseHandling = FloatParseHandling.Double;

		// Token: 0x040003CF RID: 975
		internal const FloatFormatHandling DefaultFloatFormatHandling = FloatFormatHandling.String;

		// Token: 0x040003D0 RID: 976
		internal const StringEscapeHandling DefaultStringEscapeHandling = StringEscapeHandling.Default;

		// Token: 0x040003D1 RID: 977
		internal const TypeNameAssemblyFormatHandling DefaultTypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

		// Token: 0x040003D2 RID: 978
		internal static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;

		// Token: 0x040003D3 RID: 979
		internal const bool DefaultCheckAdditionalContent = false;

		// Token: 0x040003D4 RID: 980
		internal const string DefaultDateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

		// Token: 0x040003D5 RID: 981
		internal Formatting? _formatting;

		// Token: 0x040003D6 RID: 982
		internal DateFormatHandling? _dateFormatHandling;

		// Token: 0x040003D7 RID: 983
		internal DateTimeZoneHandling? _dateTimeZoneHandling;

		// Token: 0x040003D8 RID: 984
		internal DateParseHandling? _dateParseHandling;

		// Token: 0x040003D9 RID: 985
		internal FloatFormatHandling? _floatFormatHandling;

		// Token: 0x040003DA RID: 986
		internal FloatParseHandling? _floatParseHandling;

		// Token: 0x040003DB RID: 987
		internal StringEscapeHandling? _stringEscapeHandling;

		// Token: 0x040003DC RID: 988
		internal CultureInfo _culture;

		// Token: 0x040003DD RID: 989
		internal bool? _checkAdditionalContent;

		// Token: 0x040003DE RID: 990
		internal int? _maxDepth;

		// Token: 0x040003DF RID: 991
		internal bool _maxDepthSet;

		// Token: 0x040003E0 RID: 992
		internal string _dateFormatString;

		// Token: 0x040003E1 RID: 993
		internal bool _dateFormatStringSet;

		// Token: 0x040003E2 RID: 994
		internal TypeNameAssemblyFormatHandling? _typeNameAssemblyFormatHandling;

		// Token: 0x040003E3 RID: 995
		internal DefaultValueHandling? _defaultValueHandling;

		// Token: 0x040003E4 RID: 996
		internal PreserveReferencesHandling? _preserveReferencesHandling;

		// Token: 0x040003E5 RID: 997
		internal NullValueHandling? _nullValueHandling;

		// Token: 0x040003E6 RID: 998
		internal ObjectCreationHandling? _objectCreationHandling;

		// Token: 0x040003E7 RID: 999
		internal MissingMemberHandling? _missingMemberHandling;

		// Token: 0x040003E8 RID: 1000
		internal ReferenceLoopHandling? _referenceLoopHandling;

		// Token: 0x040003E9 RID: 1001
		internal StreamingContext? _context;

		// Token: 0x040003EA RID: 1002
		internal ConstructorHandling? _constructorHandling;

		// Token: 0x040003EB RID: 1003
		internal TypeNameHandling? _typeNameHandling;

		// Token: 0x040003EC RID: 1004
		internal MetadataPropertyHandling? _metadataPropertyHandling;
	}
}
