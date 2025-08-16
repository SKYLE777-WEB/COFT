using System;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000121 RID: 289
	internal class JsonSerializerProxy : JsonSerializer
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000EEC RID: 3820 RVA: 0x0005D0CC File Offset: 0x0005B2CC
		// (remove) Token: 0x06000EED RID: 3821 RVA: 0x0005D0DC File Offset: 0x0005B2DC
		public override event EventHandler<ErrorEventArgs> Error
		{
			add
			{
				this._serializer.Error += value;
			}
			remove
			{
				this._serializer.Error -= value;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000EEE RID: 3822 RVA: 0x0005D0EC File Offset: 0x0005B2EC
		// (set) Token: 0x06000EEF RID: 3823 RVA: 0x0005D0FC File Offset: 0x0005B2FC
		public override IReferenceResolver ReferenceResolver
		{
			get
			{
				return this._serializer.ReferenceResolver;
			}
			set
			{
				this._serializer.ReferenceResolver = value;
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0005D10C File Offset: 0x0005B30C
		// (set) Token: 0x06000EF1 RID: 3825 RVA: 0x0005D11C File Offset: 0x0005B31C
		public override ITraceWriter TraceWriter
		{
			get
			{
				return this._serializer.TraceWriter;
			}
			set
			{
				this._serializer.TraceWriter = value;
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x0005D12C File Offset: 0x0005B32C
		// (set) Token: 0x06000EF3 RID: 3827 RVA: 0x0005D13C File Offset: 0x0005B33C
		public override IEqualityComparer EqualityComparer
		{
			get
			{
				return this._serializer.EqualityComparer;
			}
			set
			{
				this._serializer.EqualityComparer = value;
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x0005D14C File Offset: 0x0005B34C
		public override JsonConverterCollection Converters
		{
			get
			{
				return this._serializer.Converters;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0005D15C File Offset: 0x0005B35C
		// (set) Token: 0x06000EF6 RID: 3830 RVA: 0x0005D16C File Offset: 0x0005B36C
		public override DefaultValueHandling DefaultValueHandling
		{
			get
			{
				return this._serializer.DefaultValueHandling;
			}
			set
			{
				this._serializer.DefaultValueHandling = value;
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x0005D17C File Offset: 0x0005B37C
		// (set) Token: 0x06000EF8 RID: 3832 RVA: 0x0005D18C File Offset: 0x0005B38C
		public override IContractResolver ContractResolver
		{
			get
			{
				return this._serializer.ContractResolver;
			}
			set
			{
				this._serializer.ContractResolver = value;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x0005D19C File Offset: 0x0005B39C
		// (set) Token: 0x06000EFA RID: 3834 RVA: 0x0005D1AC File Offset: 0x0005B3AC
		public override MissingMemberHandling MissingMemberHandling
		{
			get
			{
				return this._serializer.MissingMemberHandling;
			}
			set
			{
				this._serializer.MissingMemberHandling = value;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000EFB RID: 3835 RVA: 0x0005D1BC File Offset: 0x0005B3BC
		// (set) Token: 0x06000EFC RID: 3836 RVA: 0x0005D1CC File Offset: 0x0005B3CC
		public override NullValueHandling NullValueHandling
		{
			get
			{
				return this._serializer.NullValueHandling;
			}
			set
			{
				this._serializer.NullValueHandling = value;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000EFD RID: 3837 RVA: 0x0005D1DC File Offset: 0x0005B3DC
		// (set) Token: 0x06000EFE RID: 3838 RVA: 0x0005D1EC File Offset: 0x0005B3EC
		public override ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				return this._serializer.ObjectCreationHandling;
			}
			set
			{
				this._serializer.ObjectCreationHandling = value;
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000EFF RID: 3839 RVA: 0x0005D1FC File Offset: 0x0005B3FC
		// (set) Token: 0x06000F00 RID: 3840 RVA: 0x0005D20C File Offset: 0x0005B40C
		public override ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				return this._serializer.ReferenceLoopHandling;
			}
			set
			{
				this._serializer.ReferenceLoopHandling = value;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000F01 RID: 3841 RVA: 0x0005D21C File Offset: 0x0005B41C
		// (set) Token: 0x06000F02 RID: 3842 RVA: 0x0005D22C File Offset: 0x0005B42C
		public override PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				return this._serializer.PreserveReferencesHandling;
			}
			set
			{
				this._serializer.PreserveReferencesHandling = value;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06000F03 RID: 3843 RVA: 0x0005D23C File Offset: 0x0005B43C
		// (set) Token: 0x06000F04 RID: 3844 RVA: 0x0005D24C File Offset: 0x0005B44C
		public override TypeNameHandling TypeNameHandling
		{
			get
			{
				return this._serializer.TypeNameHandling;
			}
			set
			{
				this._serializer.TypeNameHandling = value;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000F05 RID: 3845 RVA: 0x0005D25C File Offset: 0x0005B45C
		// (set) Token: 0x06000F06 RID: 3846 RVA: 0x0005D26C File Offset: 0x0005B46C
		public override MetadataPropertyHandling MetadataPropertyHandling
		{
			get
			{
				return this._serializer.MetadataPropertyHandling;
			}
			set
			{
				this._serializer.MetadataPropertyHandling = value;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000F07 RID: 3847 RVA: 0x0005D27C File Offset: 0x0005B47C
		// (set) Token: 0x06000F08 RID: 3848 RVA: 0x0005D28C File Offset: 0x0005B48C
		[Obsolete("TypeNameAssemblyFormat is obsolete. Use TypeNameAssemblyFormatHandling instead.")]
		public override FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				return this._serializer.TypeNameAssemblyFormat;
			}
			set
			{
				this._serializer.TypeNameAssemblyFormat = value;
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06000F09 RID: 3849 RVA: 0x0005D29C File Offset: 0x0005B49C
		// (set) Token: 0x06000F0A RID: 3850 RVA: 0x0005D2AC File Offset: 0x0005B4AC
		public override TypeNameAssemblyFormatHandling TypeNameAssemblyFormatHandling
		{
			get
			{
				return this._serializer.TypeNameAssemblyFormatHandling;
			}
			set
			{
				this._serializer.TypeNameAssemblyFormatHandling = value;
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000F0B RID: 3851 RVA: 0x0005D2BC File Offset: 0x0005B4BC
		// (set) Token: 0x06000F0C RID: 3852 RVA: 0x0005D2CC File Offset: 0x0005B4CC
		public override ConstructorHandling ConstructorHandling
		{
			get
			{
				return this._serializer.ConstructorHandling;
			}
			set
			{
				this._serializer.ConstructorHandling = value;
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000F0D RID: 3853 RVA: 0x0005D2DC File Offset: 0x0005B4DC
		// (set) Token: 0x06000F0E RID: 3854 RVA: 0x0005D2EC File Offset: 0x0005B4EC
		[Obsolete("Binder is obsolete. Use SerializationBinder instead.")]
		public override SerializationBinder Binder
		{
			get
			{
				return this._serializer.Binder;
			}
			set
			{
				this._serializer.Binder = value;
			}
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000F0F RID: 3855 RVA: 0x0005D2FC File Offset: 0x0005B4FC
		// (set) Token: 0x06000F10 RID: 3856 RVA: 0x0005D30C File Offset: 0x0005B50C
		public override ISerializationBinder SerializationBinder
		{
			get
			{
				return this._serializer.SerializationBinder;
			}
			set
			{
				this._serializer.SerializationBinder = value;
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000F11 RID: 3857 RVA: 0x0005D31C File Offset: 0x0005B51C
		// (set) Token: 0x06000F12 RID: 3858 RVA: 0x0005D32C File Offset: 0x0005B52C
		public override StreamingContext Context
		{
			get
			{
				return this._serializer.Context;
			}
			set
			{
				this._serializer.Context = value;
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000F13 RID: 3859 RVA: 0x0005D33C File Offset: 0x0005B53C
		// (set) Token: 0x06000F14 RID: 3860 RVA: 0x0005D34C File Offset: 0x0005B54C
		public override Formatting Formatting
		{
			get
			{
				return this._serializer.Formatting;
			}
			set
			{
				this._serializer.Formatting = value;
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0005D35C File Offset: 0x0005B55C
		// (set) Token: 0x06000F16 RID: 3862 RVA: 0x0005D36C File Offset: 0x0005B56C
		public override DateFormatHandling DateFormatHandling
		{
			get
			{
				return this._serializer.DateFormatHandling;
			}
			set
			{
				this._serializer.DateFormatHandling = value;
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06000F17 RID: 3863 RVA: 0x0005D37C File Offset: 0x0005B57C
		// (set) Token: 0x06000F18 RID: 3864 RVA: 0x0005D38C File Offset: 0x0005B58C
		public override DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				return this._serializer.DateTimeZoneHandling;
			}
			set
			{
				this._serializer.DateTimeZoneHandling = value;
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06000F19 RID: 3865 RVA: 0x0005D39C File Offset: 0x0005B59C
		// (set) Token: 0x06000F1A RID: 3866 RVA: 0x0005D3AC File Offset: 0x0005B5AC
		public override DateParseHandling DateParseHandling
		{
			get
			{
				return this._serializer.DateParseHandling;
			}
			set
			{
				this._serializer.DateParseHandling = value;
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06000F1B RID: 3867 RVA: 0x0005D3BC File Offset: 0x0005B5BC
		// (set) Token: 0x06000F1C RID: 3868 RVA: 0x0005D3CC File Offset: 0x0005B5CC
		public override FloatFormatHandling FloatFormatHandling
		{
			get
			{
				return this._serializer.FloatFormatHandling;
			}
			set
			{
				this._serializer.FloatFormatHandling = value;
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000F1D RID: 3869 RVA: 0x0005D3DC File Offset: 0x0005B5DC
		// (set) Token: 0x06000F1E RID: 3870 RVA: 0x0005D3EC File Offset: 0x0005B5EC
		public override FloatParseHandling FloatParseHandling
		{
			get
			{
				return this._serializer.FloatParseHandling;
			}
			set
			{
				this._serializer.FloatParseHandling = value;
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000F1F RID: 3871 RVA: 0x0005D3FC File Offset: 0x0005B5FC
		// (set) Token: 0x06000F20 RID: 3872 RVA: 0x0005D40C File Offset: 0x0005B60C
		public override StringEscapeHandling StringEscapeHandling
		{
			get
			{
				return this._serializer.StringEscapeHandling;
			}
			set
			{
				this._serializer.StringEscapeHandling = value;
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06000F21 RID: 3873 RVA: 0x0005D41C File Offset: 0x0005B61C
		// (set) Token: 0x06000F22 RID: 3874 RVA: 0x0005D42C File Offset: 0x0005B62C
		public override string DateFormatString
		{
			get
			{
				return this._serializer.DateFormatString;
			}
			set
			{
				this._serializer.DateFormatString = value;
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000F23 RID: 3875 RVA: 0x0005D43C File Offset: 0x0005B63C
		// (set) Token: 0x06000F24 RID: 3876 RVA: 0x0005D44C File Offset: 0x0005B64C
		public override CultureInfo Culture
		{
			get
			{
				return this._serializer.Culture;
			}
			set
			{
				this._serializer.Culture = value;
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0005D45C File Offset: 0x0005B65C
		// (set) Token: 0x06000F26 RID: 3878 RVA: 0x0005D46C File Offset: 0x0005B66C
		public override int? MaxDepth
		{
			get
			{
				return this._serializer.MaxDepth;
			}
			set
			{
				this._serializer.MaxDepth = value;
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000F27 RID: 3879 RVA: 0x0005D47C File Offset: 0x0005B67C
		// (set) Token: 0x06000F28 RID: 3880 RVA: 0x0005D48C File Offset: 0x0005B68C
		public override bool CheckAdditionalContent
		{
			get
			{
				return this._serializer.CheckAdditionalContent;
			}
			set
			{
				this._serializer.CheckAdditionalContent = value;
			}
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x0005D49C File Offset: 0x0005B69C
		internal JsonSerializerInternalBase GetInternalSerializer()
		{
			if (this._serializerReader != null)
			{
				return this._serializerReader;
			}
			return this._serializerWriter;
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x0005D4B8 File Offset: 0x0005B6B8
		public JsonSerializerProxy(JsonSerializerInternalReader serializerReader)
		{
			ValidationUtils.ArgumentNotNull(serializerReader, "serializerReader");
			this._serializerReader = serializerReader;
			this._serializer = serializerReader.Serializer;
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x0005D4E0 File Offset: 0x0005B6E0
		public JsonSerializerProxy(JsonSerializerInternalWriter serializerWriter)
		{
			ValidationUtils.ArgumentNotNull(serializerWriter, "serializerWriter");
			this._serializerWriter = serializerWriter;
			this._serializer = serializerWriter.Serializer;
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0005D508 File Offset: 0x0005B708
		internal override object DeserializeInternal(JsonReader reader, Type objectType)
		{
			if (this._serializerReader != null)
			{
				return this._serializerReader.Deserialize(reader, objectType, false);
			}
			return this._serializer.Deserialize(reader, objectType);
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0005D534 File Offset: 0x0005B734
		internal override void PopulateInternal(JsonReader reader, object target)
		{
			if (this._serializerReader != null)
			{
				this._serializerReader.Populate(reader, target);
				return;
			}
			this._serializer.Populate(reader, target);
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0005D55C File Offset: 0x0005B75C
		internal override void SerializeInternal(JsonWriter jsonWriter, object value, Type rootType)
		{
			if (this._serializerWriter != null)
			{
				this._serializerWriter.Serialize(jsonWriter, value, rootType);
				return;
			}
			this._serializer.Serialize(jsonWriter, value);
		}

		// Token: 0x040005DD RID: 1501
		private readonly JsonSerializerInternalReader _serializerReader;

		// Token: 0x040005DE RID: 1502
		private readonly JsonSerializerInternalWriter _serializerWriter;

		// Token: 0x040005DF RID: 1503
		private readonly JsonSerializer _serializer;
	}
}
