using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000B3 RID: 179
	public class JsonSerializer
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000885 RID: 2181 RVA: 0x0003BA84 File Offset: 0x00039C84
		// (remove) Token: 0x06000886 RID: 2182 RVA: 0x0003BAC0 File Offset: 0x00039CC0
		public virtual event EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> Error;

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000887 RID: 2183 RVA: 0x0003BAFC File Offset: 0x00039CFC
		// (set) Token: 0x06000888 RID: 2184 RVA: 0x0003BB04 File Offset: 0x00039D04
		public virtual IReferenceResolver ReferenceResolver
		{
			get
			{
				return this.GetReferenceResolver();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Reference resolver cannot be null.");
				}
				this._referenceResolver = value;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000889 RID: 2185 RVA: 0x0003BB24 File Offset: 0x00039D24
		// (set) Token: 0x0600088A RID: 2186 RVA: 0x0003BB7C File Offset: 0x00039D7C
		[Obsolete("Binder is obsolete. Use SerializationBinder instead.")]
		public virtual SerializationBinder Binder
		{
			get
			{
				if (this._serializationBinder == null)
				{
					return null;
				}
				SerializationBinder serializationBinder;
				if ((serializationBinder = this._serializationBinder as SerializationBinder) != null)
				{
					return serializationBinder;
				}
				SerializationBinderAdapter serializationBinderAdapter = this._serializationBinder as SerializationBinderAdapter;
				if (serializationBinderAdapter != null)
				{
					return serializationBinderAdapter.SerializationBinder;
				}
				throw new InvalidOperationException("Cannot get SerializationBinder because an ISerializationBinder was previously set.");
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Serialization binder cannot be null.");
				}
				this._serializationBinder = (value as ISerializationBinder) ?? new SerializationBinderAdapter(value);
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x0600088B RID: 2187 RVA: 0x0003BBB0 File Offset: 0x00039DB0
		// (set) Token: 0x0600088C RID: 2188 RVA: 0x0003BBB8 File Offset: 0x00039DB8
		public virtual ISerializationBinder SerializationBinder
		{
			get
			{
				return this._serializationBinder;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Serialization binder cannot be null.");
				}
				this._serializationBinder = value;
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x0600088D RID: 2189 RVA: 0x0003BBD8 File Offset: 0x00039DD8
		// (set) Token: 0x0600088E RID: 2190 RVA: 0x0003BBE0 File Offset: 0x00039DE0
		public virtual ITraceWriter TraceWriter
		{
			get
			{
				return this._traceWriter;
			}
			set
			{
				this._traceWriter = value;
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600088F RID: 2191 RVA: 0x0003BBEC File Offset: 0x00039DEC
		// (set) Token: 0x06000890 RID: 2192 RVA: 0x0003BBF4 File Offset: 0x00039DF4
		public virtual IEqualityComparer EqualityComparer
		{
			get
			{
				return this._equalityComparer;
			}
			set
			{
				this._equalityComparer = value;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000891 RID: 2193 RVA: 0x0003BC00 File Offset: 0x00039E00
		// (set) Token: 0x06000892 RID: 2194 RVA: 0x0003BC08 File Offset: 0x00039E08
		public virtual TypeNameHandling TypeNameHandling
		{
			get
			{
				return this._typeNameHandling;
			}
			set
			{
				if (value < TypeNameHandling.None || value > TypeNameHandling.Auto)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._typeNameHandling = value;
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000893 RID: 2195 RVA: 0x0003BC2C File Offset: 0x00039E2C
		// (set) Token: 0x06000894 RID: 2196 RVA: 0x0003BC34 File Offset: 0x00039E34
		[Obsolete("TypeNameAssemblyFormat is obsolete. Use TypeNameAssemblyFormatHandling instead.")]
		public virtual FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				return (FormatterAssemblyStyle)this._typeNameAssemblyFormatHandling;
			}
			set
			{
				if (value < FormatterAssemblyStyle.Simple || value > FormatterAssemblyStyle.Full)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._typeNameAssemblyFormatHandling = (TypeNameAssemblyFormatHandling)value;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000895 RID: 2197 RVA: 0x0003BC58 File Offset: 0x00039E58
		// (set) Token: 0x06000896 RID: 2198 RVA: 0x0003BC60 File Offset: 0x00039E60
		public virtual TypeNameAssemblyFormatHandling TypeNameAssemblyFormatHandling
		{
			get
			{
				return this._typeNameAssemblyFormatHandling;
			}
			set
			{
				if (value < TypeNameAssemblyFormatHandling.Simple || value > TypeNameAssemblyFormatHandling.Full)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._typeNameAssemblyFormatHandling = value;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000897 RID: 2199 RVA: 0x0003BC84 File Offset: 0x00039E84
		// (set) Token: 0x06000898 RID: 2200 RVA: 0x0003BC8C File Offset: 0x00039E8C
		public virtual PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				return this._preserveReferencesHandling;
			}
			set
			{
				if (value < PreserveReferencesHandling.None || value > PreserveReferencesHandling.All)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._preserveReferencesHandling = value;
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000899 RID: 2201 RVA: 0x0003BCB0 File Offset: 0x00039EB0
		// (set) Token: 0x0600089A RID: 2202 RVA: 0x0003BCB8 File Offset: 0x00039EB8
		public virtual ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				return this._referenceLoopHandling;
			}
			set
			{
				if (value < ReferenceLoopHandling.Error || value > ReferenceLoopHandling.Serialize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._referenceLoopHandling = value;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600089B RID: 2203 RVA: 0x0003BCDC File Offset: 0x00039EDC
		// (set) Token: 0x0600089C RID: 2204 RVA: 0x0003BCE4 File Offset: 0x00039EE4
		public virtual MissingMemberHandling MissingMemberHandling
		{
			get
			{
				return this._missingMemberHandling;
			}
			set
			{
				if (value < MissingMemberHandling.Ignore || value > MissingMemberHandling.Error)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._missingMemberHandling = value;
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x0600089D RID: 2205 RVA: 0x0003BD08 File Offset: 0x00039F08
		// (set) Token: 0x0600089E RID: 2206 RVA: 0x0003BD10 File Offset: 0x00039F10
		public virtual NullValueHandling NullValueHandling
		{
			get
			{
				return this._nullValueHandling;
			}
			set
			{
				if (value < NullValueHandling.Include || value > NullValueHandling.Ignore)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._nullValueHandling = value;
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x0003BD34 File Offset: 0x00039F34
		// (set) Token: 0x060008A0 RID: 2208 RVA: 0x0003BD3C File Offset: 0x00039F3C
		public virtual DefaultValueHandling DefaultValueHandling
		{
			get
			{
				return this._defaultValueHandling;
			}
			set
			{
				if (value < DefaultValueHandling.Include || value > DefaultValueHandling.IgnoreAndPopulate)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._defaultValueHandling = value;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x0003BD60 File Offset: 0x00039F60
		// (set) Token: 0x060008A2 RID: 2210 RVA: 0x0003BD68 File Offset: 0x00039F68
		public virtual ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				return this._objectCreationHandling;
			}
			set
			{
				if (value < ObjectCreationHandling.Auto || value > ObjectCreationHandling.Replace)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._objectCreationHandling = value;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x0003BD8C File Offset: 0x00039F8C
		// (set) Token: 0x060008A4 RID: 2212 RVA: 0x0003BD94 File Offset: 0x00039F94
		public virtual ConstructorHandling ConstructorHandling
		{
			get
			{
				return this._constructorHandling;
			}
			set
			{
				if (value < ConstructorHandling.Default || value > ConstructorHandling.AllowNonPublicDefaultConstructor)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._constructorHandling = value;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x0003BDB8 File Offset: 0x00039FB8
		// (set) Token: 0x060008A6 RID: 2214 RVA: 0x0003BDC0 File Offset: 0x00039FC0
		public virtual MetadataPropertyHandling MetadataPropertyHandling
		{
			get
			{
				return this._metadataPropertyHandling;
			}
			set
			{
				if (value < MetadataPropertyHandling.Default || value > MetadataPropertyHandling.Ignore)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._metadataPropertyHandling = value;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x0003BDE4 File Offset: 0x00039FE4
		public virtual JsonConverterCollection Converters
		{
			get
			{
				if (this._converters == null)
				{
					this._converters = new JsonConverterCollection();
				}
				return this._converters;
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060008A8 RID: 2216 RVA: 0x0003BE04 File Offset: 0x0003A004
		// (set) Token: 0x060008A9 RID: 2217 RVA: 0x0003BE0C File Offset: 0x0003A00C
		public virtual IContractResolver ContractResolver
		{
			get
			{
				return this._contractResolver;
			}
			set
			{
				this._contractResolver = value ?? DefaultContractResolver.Instance;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060008AA RID: 2218 RVA: 0x0003BE24 File Offset: 0x0003A024
		// (set) Token: 0x060008AB RID: 2219 RVA: 0x0003BE2C File Offset: 0x0003A02C
		public virtual StreamingContext Context
		{
			get
			{
				return this._context;
			}
			set
			{
				this._context = value;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060008AC RID: 2220 RVA: 0x0003BE38 File Offset: 0x0003A038
		// (set) Token: 0x060008AD RID: 2221 RVA: 0x0003BE68 File Offset: 0x0003A068
		public virtual Formatting Formatting
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

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x0003BE78 File Offset: 0x0003A078
		// (set) Token: 0x060008AF RID: 2223 RVA: 0x0003BEA8 File Offset: 0x0003A0A8
		public virtual DateFormatHandling DateFormatHandling
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

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060008B0 RID: 2224 RVA: 0x0003BEB8 File Offset: 0x0003A0B8
		// (set) Token: 0x060008B1 RID: 2225 RVA: 0x0003BEE8 File Offset: 0x0003A0E8
		public virtual DateTimeZoneHandling DateTimeZoneHandling
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

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060008B2 RID: 2226 RVA: 0x0003BEF8 File Offset: 0x0003A0F8
		// (set) Token: 0x060008B3 RID: 2227 RVA: 0x0003BF28 File Offset: 0x0003A128
		public virtual DateParseHandling DateParseHandling
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

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060008B4 RID: 2228 RVA: 0x0003BF38 File Offset: 0x0003A138
		// (set) Token: 0x060008B5 RID: 2229 RVA: 0x0003BF68 File Offset: 0x0003A168
		public virtual FloatParseHandling FloatParseHandling
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

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x0003BF78 File Offset: 0x0003A178
		// (set) Token: 0x060008B7 RID: 2231 RVA: 0x0003BFA8 File Offset: 0x0003A1A8
		public virtual FloatFormatHandling FloatFormatHandling
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

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x0003BFB8 File Offset: 0x0003A1B8
		// (set) Token: 0x060008B9 RID: 2233 RVA: 0x0003BFE8 File Offset: 0x0003A1E8
		public virtual StringEscapeHandling StringEscapeHandling
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

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060008BA RID: 2234 RVA: 0x0003BFF8 File Offset: 0x0003A1F8
		// (set) Token: 0x060008BB RID: 2235 RVA: 0x0003C00C File Offset: 0x0003A20C
		public virtual string DateFormatString
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

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060008BC RID: 2236 RVA: 0x0003C01C File Offset: 0x0003A21C
		// (set) Token: 0x060008BD RID: 2237 RVA: 0x0003C030 File Offset: 0x0003A230
		public virtual CultureInfo Culture
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

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060008BE RID: 2238 RVA: 0x0003C03C File Offset: 0x0003A23C
		// (set) Token: 0x060008BF RID: 2239 RVA: 0x0003C044 File Offset: 0x0003A244
		public virtual int? MaxDepth
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

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060008C0 RID: 2240 RVA: 0x0003C098 File Offset: 0x0003A298
		// (set) Token: 0x060008C1 RID: 2241 RVA: 0x0003C0C8 File Offset: 0x0003A2C8
		public virtual bool CheckAdditionalContent
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

		// Token: 0x060008C2 RID: 2242 RVA: 0x0003C0D8 File Offset: 0x0003A2D8
		internal bool IsCheckAdditionalContentSet()
		{
			return this._checkAdditionalContent != null;
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0003C0E8 File Offset: 0x0003A2E8
		public JsonSerializer()
		{
			this._referenceLoopHandling = ReferenceLoopHandling.Error;
			this._missingMemberHandling = MissingMemberHandling.Ignore;
			this._nullValueHandling = NullValueHandling.Include;
			this._defaultValueHandling = DefaultValueHandling.Include;
			this._objectCreationHandling = ObjectCreationHandling.Auto;
			this._preserveReferencesHandling = PreserveReferencesHandling.None;
			this._constructorHandling = ConstructorHandling.Default;
			this._typeNameHandling = TypeNameHandling.None;
			this._metadataPropertyHandling = MetadataPropertyHandling.Default;
			this._context = JsonSerializerSettings.DefaultContext;
			this._serializationBinder = DefaultSerializationBinder.Instance;
			this._culture = JsonSerializerSettings.DefaultCulture;
			this._contractResolver = DefaultContractResolver.Instance;
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x0003C16C File Offset: 0x0003A36C
		public static JsonSerializer Create()
		{
			return new JsonSerializer();
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x0003C174 File Offset: 0x0003A374
		public static JsonSerializer Create(JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create();
			if (settings != null)
			{
				JsonSerializer.ApplySerializerSettings(jsonSerializer, settings);
			}
			return jsonSerializer;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x0003C19C File Offset: 0x0003A39C
		public static JsonSerializer CreateDefault()
		{
			Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
			return JsonSerializer.Create((defaultSettings != null) ? defaultSettings() : null);
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x0003C1BC File Offset: 0x0003A3BC
		public static JsonSerializer CreateDefault(JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();
			if (settings != null)
			{
				JsonSerializer.ApplySerializerSettings(jsonSerializer, settings);
			}
			return jsonSerializer;
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0003C1E4 File Offset: 0x0003A3E4
		private static void ApplySerializerSettings(JsonSerializer serializer, JsonSerializerSettings settings)
		{
			if (!CollectionUtils.IsNullOrEmpty<JsonConverter>(settings.Converters))
			{
				for (int i = 0; i < settings.Converters.Count; i++)
				{
					serializer.Converters.Insert(i, settings.Converters[i]);
				}
			}
			if (settings._typeNameHandling != null)
			{
				serializer.TypeNameHandling = settings.TypeNameHandling;
			}
			if (settings._metadataPropertyHandling != null)
			{
				serializer.MetadataPropertyHandling = settings.MetadataPropertyHandling;
			}
			if (settings._typeNameAssemblyFormatHandling != null)
			{
				serializer.TypeNameAssemblyFormatHandling = settings.TypeNameAssemblyFormatHandling;
			}
			if (settings._preserveReferencesHandling != null)
			{
				serializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
			}
			if (settings._referenceLoopHandling != null)
			{
				serializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
			}
			if (settings._missingMemberHandling != null)
			{
				serializer.MissingMemberHandling = settings.MissingMemberHandling;
			}
			if (settings._objectCreationHandling != null)
			{
				serializer.ObjectCreationHandling = settings.ObjectCreationHandling;
			}
			if (settings._nullValueHandling != null)
			{
				serializer.NullValueHandling = settings.NullValueHandling;
			}
			if (settings._defaultValueHandling != null)
			{
				serializer.DefaultValueHandling = settings.DefaultValueHandling;
			}
			if (settings._constructorHandling != null)
			{
				serializer.ConstructorHandling = settings.ConstructorHandling;
			}
			if (settings._context != null)
			{
				serializer.Context = settings.Context;
			}
			if (settings._checkAdditionalContent != null)
			{
				serializer._checkAdditionalContent = settings._checkAdditionalContent;
			}
			if (settings.Error != null)
			{
				serializer.Error += settings.Error;
			}
			if (settings.ContractResolver != null)
			{
				serializer.ContractResolver = settings.ContractResolver;
			}
			if (settings.ReferenceResolverProvider != null)
			{
				serializer.ReferenceResolver = settings.ReferenceResolverProvider();
			}
			if (settings.TraceWriter != null)
			{
				serializer.TraceWriter = settings.TraceWriter;
			}
			if (settings.EqualityComparer != null)
			{
				serializer.EqualityComparer = settings.EqualityComparer;
			}
			if (settings.SerializationBinder != null)
			{
				serializer.SerializationBinder = settings.SerializationBinder;
			}
			if (settings._formatting != null)
			{
				serializer._formatting = settings._formatting;
			}
			if (settings._dateFormatHandling != null)
			{
				serializer._dateFormatHandling = settings._dateFormatHandling;
			}
			if (settings._dateTimeZoneHandling != null)
			{
				serializer._dateTimeZoneHandling = settings._dateTimeZoneHandling;
			}
			if (settings._dateParseHandling != null)
			{
				serializer._dateParseHandling = settings._dateParseHandling;
			}
			if (settings._dateFormatStringSet)
			{
				serializer._dateFormatString = settings._dateFormatString;
				serializer._dateFormatStringSet = settings._dateFormatStringSet;
			}
			if (settings._floatFormatHandling != null)
			{
				serializer._floatFormatHandling = settings._floatFormatHandling;
			}
			if (settings._floatParseHandling != null)
			{
				serializer._floatParseHandling = settings._floatParseHandling;
			}
			if (settings._stringEscapeHandling != null)
			{
				serializer._stringEscapeHandling = settings._stringEscapeHandling;
			}
			if (settings._culture != null)
			{
				serializer._culture = settings._culture;
			}
			if (settings._maxDepthSet)
			{
				serializer._maxDepth = settings._maxDepth;
				serializer._maxDepthSet = settings._maxDepthSet;
			}
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x0003C538 File Offset: 0x0003A738
		public void Populate(TextReader reader, object target)
		{
			this.Populate(new JsonTextReader(reader), target);
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x0003C548 File Offset: 0x0003A748
		public void Populate(JsonReader reader, object target)
		{
			this.PopulateInternal(reader, target);
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x0003C554 File Offset: 0x0003A754
		internal virtual void PopulateInternal(JsonReader reader, object target)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			ValidationUtils.ArgumentNotNull(target, "target");
			CultureInfo cultureInfo;
			DateTimeZoneHandling? dateTimeZoneHandling;
			DateParseHandling? dateParseHandling;
			FloatParseHandling? floatParseHandling;
			int? num;
			string text;
			this.SetupReader(reader, out cultureInfo, out dateTimeZoneHandling, out dateParseHandling, out floatParseHandling, out num, out text);
			TraceJsonReader traceJsonReader = ((this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose) ? new TraceJsonReader(reader) : null);
			new JsonSerializerInternalReader(this).Populate(traceJsonReader ?? reader, target);
			if (traceJsonReader != null)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader.GetDeserializedJsonMessage(), null);
			}
			this.ResetReader(reader, cultureInfo, dateTimeZoneHandling, dateParseHandling, floatParseHandling, num, text);
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x0003C5F8 File Offset: 0x0003A7F8
		public object Deserialize(JsonReader reader)
		{
			return this.Deserialize(reader, null);
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x0003C604 File Offset: 0x0003A804
		public object Deserialize(TextReader reader, Type objectType)
		{
			return this.Deserialize(new JsonTextReader(reader), objectType);
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x0003C614 File Offset: 0x0003A814
		public T Deserialize<T>(JsonReader reader)
		{
			return (T)((object)this.Deserialize(reader, typeof(T)));
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x0003C62C File Offset: 0x0003A82C
		public object Deserialize(JsonReader reader, Type objectType)
		{
			return this.DeserializeInternal(reader, objectType);
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x0003C638 File Offset: 0x0003A838
		internal virtual object DeserializeInternal(JsonReader reader, Type objectType)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			CultureInfo cultureInfo;
			DateTimeZoneHandling? dateTimeZoneHandling;
			DateParseHandling? dateParseHandling;
			FloatParseHandling? floatParseHandling;
			int? num;
			string text;
			this.SetupReader(reader, out cultureInfo, out dateTimeZoneHandling, out dateParseHandling, out floatParseHandling, out num, out text);
			TraceJsonReader traceJsonReader = ((this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose) ? new TraceJsonReader(reader) : null);
			object obj = new JsonSerializerInternalReader(this).Deserialize(traceJsonReader ?? reader, objectType, this.CheckAdditionalContent);
			if (traceJsonReader != null)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader.GetDeserializedJsonMessage(), null);
			}
			this.ResetReader(reader, cultureInfo, dateTimeZoneHandling, dateParseHandling, floatParseHandling, num, text);
			return obj;
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x0003C6D8 File Offset: 0x0003A8D8
		private void SetupReader(JsonReader reader, out CultureInfo previousCulture, out DateTimeZoneHandling? previousDateTimeZoneHandling, out DateParseHandling? previousDateParseHandling, out FloatParseHandling? previousFloatParseHandling, out int? previousMaxDepth, out string previousDateFormatString)
		{
			if (this._culture != null && !this._culture.Equals(reader.Culture))
			{
				previousCulture = reader.Culture;
				reader.Culture = this._culture;
			}
			else
			{
				previousCulture = null;
			}
			if (this._dateTimeZoneHandling != null && reader.DateTimeZoneHandling != this._dateTimeZoneHandling)
			{
				previousDateTimeZoneHandling = new DateTimeZoneHandling?(reader.DateTimeZoneHandling);
				reader.DateTimeZoneHandling = this._dateTimeZoneHandling.GetValueOrDefault();
			}
			else
			{
				previousDateTimeZoneHandling = null;
			}
			if (this._dateParseHandling != null && reader.DateParseHandling != this._dateParseHandling)
			{
				previousDateParseHandling = new DateParseHandling?(reader.DateParseHandling);
				reader.DateParseHandling = this._dateParseHandling.GetValueOrDefault();
			}
			else
			{
				previousDateParseHandling = null;
			}
			if (this._floatParseHandling != null && reader.FloatParseHandling != this._floatParseHandling)
			{
				previousFloatParseHandling = new FloatParseHandling?(reader.FloatParseHandling);
				reader.FloatParseHandling = this._floatParseHandling.GetValueOrDefault();
			}
			else
			{
				previousFloatParseHandling = null;
			}
			if (this._maxDepthSet && reader.MaxDepth != this._maxDepth)
			{
				previousMaxDepth = reader.MaxDepth;
				reader.MaxDepth = this._maxDepth;
			}
			else
			{
				previousMaxDepth = null;
			}
			if (this._dateFormatStringSet && reader.DateFormatString != this._dateFormatString)
			{
				previousDateFormatString = reader.DateFormatString;
				reader.DateFormatString = this._dateFormatString;
			}
			else
			{
				previousDateFormatString = null;
			}
			JsonTextReader jsonTextReader = reader as JsonTextReader;
			if (jsonTextReader != null)
			{
				DefaultContractResolver defaultContractResolver = this._contractResolver as DefaultContractResolver;
				if (defaultContractResolver != null)
				{
					jsonTextReader.NameTable = defaultContractResolver.GetNameTable();
				}
			}
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x0003C950 File Offset: 0x0003AB50
		private void ResetReader(JsonReader reader, CultureInfo previousCulture, DateTimeZoneHandling? previousDateTimeZoneHandling, DateParseHandling? previousDateParseHandling, FloatParseHandling? previousFloatParseHandling, int? previousMaxDepth, string previousDateFormatString)
		{
			if (previousCulture != null)
			{
				reader.Culture = previousCulture;
			}
			if (previousDateTimeZoneHandling != null)
			{
				reader.DateTimeZoneHandling = previousDateTimeZoneHandling.GetValueOrDefault();
			}
			if (previousDateParseHandling != null)
			{
				reader.DateParseHandling = previousDateParseHandling.GetValueOrDefault();
			}
			if (previousFloatParseHandling != null)
			{
				reader.FloatParseHandling = previousFloatParseHandling.GetValueOrDefault();
			}
			if (this._maxDepthSet)
			{
				reader.MaxDepth = previousMaxDepth;
			}
			if (this._dateFormatStringSet)
			{
				reader.DateFormatString = previousDateFormatString;
			}
			JsonTextReader jsonTextReader = reader as JsonTextReader;
			if (jsonTextReader != null)
			{
				jsonTextReader.NameTable = null;
			}
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x0003C9F4 File Offset: 0x0003ABF4
		public void Serialize(TextWriter textWriter, object value)
		{
			this.Serialize(new JsonTextWriter(textWriter), value);
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x0003CA04 File Offset: 0x0003AC04
		public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
		{
			this.SerializeInternal(jsonWriter, value, objectType);
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x0003CA10 File Offset: 0x0003AC10
		public void Serialize(TextWriter textWriter, object value, Type objectType)
		{
			this.Serialize(new JsonTextWriter(textWriter), value, objectType);
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x0003CA20 File Offset: 0x0003AC20
		public void Serialize(JsonWriter jsonWriter, object value)
		{
			this.SerializeInternal(jsonWriter, value, null);
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x0003CA2C File Offset: 0x0003AC2C
		internal virtual void SerializeInternal(JsonWriter jsonWriter, object value, Type objectType)
		{
			ValidationUtils.ArgumentNotNull(jsonWriter, "jsonWriter");
			Formatting? formatting = null;
			if (this._formatting != null && jsonWriter.Formatting != this._formatting)
			{
				formatting = new Formatting?(jsonWriter.Formatting);
				jsonWriter.Formatting = this._formatting.GetValueOrDefault();
			}
			DateFormatHandling? dateFormatHandling = null;
			if (this._dateFormatHandling != null && jsonWriter.DateFormatHandling != this._dateFormatHandling)
			{
				dateFormatHandling = new DateFormatHandling?(jsonWriter.DateFormatHandling);
				jsonWriter.DateFormatHandling = this._dateFormatHandling.GetValueOrDefault();
			}
			DateTimeZoneHandling? dateTimeZoneHandling = null;
			if (this._dateTimeZoneHandling != null && jsonWriter.DateTimeZoneHandling != this._dateTimeZoneHandling)
			{
				dateTimeZoneHandling = new DateTimeZoneHandling?(jsonWriter.DateTimeZoneHandling);
				jsonWriter.DateTimeZoneHandling = this._dateTimeZoneHandling.GetValueOrDefault();
			}
			FloatFormatHandling? floatFormatHandling = null;
			if (this._floatFormatHandling != null && jsonWriter.FloatFormatHandling != this._floatFormatHandling)
			{
				floatFormatHandling = new FloatFormatHandling?(jsonWriter.FloatFormatHandling);
				jsonWriter.FloatFormatHandling = this._floatFormatHandling.GetValueOrDefault();
			}
			StringEscapeHandling? stringEscapeHandling = null;
			if (this._stringEscapeHandling != null && jsonWriter.StringEscapeHandling != this._stringEscapeHandling)
			{
				stringEscapeHandling = new StringEscapeHandling?(jsonWriter.StringEscapeHandling);
				jsonWriter.StringEscapeHandling = this._stringEscapeHandling.GetValueOrDefault();
			}
			CultureInfo cultureInfo = null;
			if (this._culture != null && !this._culture.Equals(jsonWriter.Culture))
			{
				cultureInfo = jsonWriter.Culture;
				jsonWriter.Culture = this._culture;
			}
			string text = null;
			if (this._dateFormatStringSet && jsonWriter.DateFormatString != this._dateFormatString)
			{
				text = jsonWriter.DateFormatString;
				jsonWriter.DateFormatString = this._dateFormatString;
			}
			TraceJsonWriter traceJsonWriter = ((this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose) ? new TraceJsonWriter(jsonWriter) : null);
			new JsonSerializerInternalWriter(this).Serialize(traceJsonWriter ?? jsonWriter, value, objectType);
			if (traceJsonWriter != null)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonWriter.GetSerializedJsonMessage(), null);
			}
			if (formatting != null)
			{
				jsonWriter.Formatting = formatting.GetValueOrDefault();
			}
			if (dateFormatHandling != null)
			{
				jsonWriter.DateFormatHandling = dateFormatHandling.GetValueOrDefault();
			}
			if (dateTimeZoneHandling != null)
			{
				jsonWriter.DateTimeZoneHandling = dateTimeZoneHandling.GetValueOrDefault();
			}
			if (floatFormatHandling != null)
			{
				jsonWriter.FloatFormatHandling = floatFormatHandling.GetValueOrDefault();
			}
			if (stringEscapeHandling != null)
			{
				jsonWriter.StringEscapeHandling = stringEscapeHandling.GetValueOrDefault();
			}
			if (this._dateFormatStringSet)
			{
				jsonWriter.DateFormatString = text;
			}
			if (cultureInfo != null)
			{
				jsonWriter.Culture = cultureInfo;
			}
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x0003CDAC File Offset: 0x0003AFAC
		internal IReferenceResolver GetReferenceResolver()
		{
			if (this._referenceResolver == null)
			{
				this._referenceResolver = new DefaultReferenceResolver();
			}
			return this._referenceResolver;
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x0003CDCC File Offset: 0x0003AFCC
		internal JsonConverter GetMatchingConverter(Type type)
		{
			return JsonSerializer.GetMatchingConverter(this._converters, type);
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x0003CDDC File Offset: 0x0003AFDC
		internal static JsonConverter GetMatchingConverter(IList<JsonConverter> converters, Type objectType)
		{
			if (converters != null)
			{
				for (int i = 0; i < converters.Count; i++)
				{
					JsonConverter jsonConverter = converters[i];
					if (jsonConverter.CanConvert(objectType))
					{
						return jsonConverter;
					}
				}
			}
			return null;
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x0003CE20 File Offset: 0x0003B020
		internal void OnError(Newtonsoft.Json.Serialization.ErrorEventArgs e)
		{
			EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> error = this.Error;
			if (error == null)
			{
				return;
			}
			error(this, e);
		}

		// Token: 0x040003A1 RID: 929
		internal TypeNameHandling _typeNameHandling;

		// Token: 0x040003A2 RID: 930
		internal TypeNameAssemblyFormatHandling _typeNameAssemblyFormatHandling;

		// Token: 0x040003A3 RID: 931
		internal PreserveReferencesHandling _preserveReferencesHandling;

		// Token: 0x040003A4 RID: 932
		internal ReferenceLoopHandling _referenceLoopHandling;

		// Token: 0x040003A5 RID: 933
		internal MissingMemberHandling _missingMemberHandling;

		// Token: 0x040003A6 RID: 934
		internal ObjectCreationHandling _objectCreationHandling;

		// Token: 0x040003A7 RID: 935
		internal NullValueHandling _nullValueHandling;

		// Token: 0x040003A8 RID: 936
		internal DefaultValueHandling _defaultValueHandling;

		// Token: 0x040003A9 RID: 937
		internal ConstructorHandling _constructorHandling;

		// Token: 0x040003AA RID: 938
		internal MetadataPropertyHandling _metadataPropertyHandling;

		// Token: 0x040003AB RID: 939
		internal JsonConverterCollection _converters;

		// Token: 0x040003AC RID: 940
		internal IContractResolver _contractResolver;

		// Token: 0x040003AD RID: 941
		internal ITraceWriter _traceWriter;

		// Token: 0x040003AE RID: 942
		internal IEqualityComparer _equalityComparer;

		// Token: 0x040003AF RID: 943
		internal ISerializationBinder _serializationBinder;

		// Token: 0x040003B0 RID: 944
		internal StreamingContext _context;

		// Token: 0x040003B1 RID: 945
		private IReferenceResolver _referenceResolver;

		// Token: 0x040003B2 RID: 946
		private Formatting? _formatting;

		// Token: 0x040003B3 RID: 947
		private DateFormatHandling? _dateFormatHandling;

		// Token: 0x040003B4 RID: 948
		private DateTimeZoneHandling? _dateTimeZoneHandling;

		// Token: 0x040003B5 RID: 949
		private DateParseHandling? _dateParseHandling;

		// Token: 0x040003B6 RID: 950
		private FloatFormatHandling? _floatFormatHandling;

		// Token: 0x040003B7 RID: 951
		private FloatParseHandling? _floatParseHandling;

		// Token: 0x040003B8 RID: 952
		private StringEscapeHandling? _stringEscapeHandling;

		// Token: 0x040003B9 RID: 953
		private CultureInfo _culture;

		// Token: 0x040003BA RID: 954
		private int? _maxDepth;

		// Token: 0x040003BB RID: 955
		private bool _maxDepthSet;

		// Token: 0x040003BC RID: 956
		private bool? _checkAdditionalContent;

		// Token: 0x040003BD RID: 957
		private string _dateFormatString;

		// Token: 0x040003BE RID: 958
		private bool _dateFormatStringSet;
	}
}
