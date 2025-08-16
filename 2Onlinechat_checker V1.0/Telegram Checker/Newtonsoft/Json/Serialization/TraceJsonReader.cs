using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200012C RID: 300
	internal class TraceJsonReader : JsonReader, IJsonLineInfo
	{
		// Token: 0x06000F68 RID: 3944 RVA: 0x0005DFC4 File Offset: 0x0005C1C4
		public TraceJsonReader(JsonReader innerReader)
		{
			this._innerReader = innerReader;
			this._sw = new StringWriter(CultureInfo.InvariantCulture);
			this._sw.Write("Deserialized JSON: " + Environment.NewLine);
			this._textWriter = new JsonTextWriter(this._sw);
			this._textWriter.Formatting = Formatting.Indented;
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x0005E02C File Offset: 0x0005C22C
		public string GetDeserializedJsonMessage()
		{
			return this._sw.ToString();
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x0005E03C File Offset: 0x0005C23C
		public override bool Read()
		{
			bool flag = this._innerReader.Read();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return flag;
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x0005E06C File Offset: 0x0005C26C
		public override int? ReadAsInt32()
		{
			int? num = this._innerReader.ReadAsInt32();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return num;
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0005E09C File Offset: 0x0005C29C
		public override string ReadAsString()
		{
			string text = this._innerReader.ReadAsString();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return text;
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0005E0CC File Offset: 0x0005C2CC
		public override byte[] ReadAsBytes()
		{
			byte[] array = this._innerReader.ReadAsBytes();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return array;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0005E0FC File Offset: 0x0005C2FC
		public override decimal? ReadAsDecimal()
		{
			decimal? num = this._innerReader.ReadAsDecimal();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return num;
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0005E12C File Offset: 0x0005C32C
		public override double? ReadAsDouble()
		{
			double? num = this._innerReader.ReadAsDouble();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return num;
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0005E15C File Offset: 0x0005C35C
		public override bool? ReadAsBoolean()
		{
			bool? flag = this._innerReader.ReadAsBoolean();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return flag;
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x0005E18C File Offset: 0x0005C38C
		public override DateTime? ReadAsDateTime()
		{
			DateTime? dateTime = this._innerReader.ReadAsDateTime();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return dateTime;
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x0005E1BC File Offset: 0x0005C3BC
		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset? dateTimeOffset = this._innerReader.ReadAsDateTimeOffset();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return dateTimeOffset;
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06000F73 RID: 3955 RVA: 0x0005E1EC File Offset: 0x0005C3EC
		public override int Depth
		{
			get
			{
				return this._innerReader.Depth;
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06000F74 RID: 3956 RVA: 0x0005E1FC File Offset: 0x0005C3FC
		public override string Path
		{
			get
			{
				return this._innerReader.Path;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06000F75 RID: 3957 RVA: 0x0005E20C File Offset: 0x0005C40C
		// (set) Token: 0x06000F76 RID: 3958 RVA: 0x0005E21C File Offset: 0x0005C41C
		public override char QuoteChar
		{
			get
			{
				return this._innerReader.QuoteChar;
			}
			protected internal set
			{
				this._innerReader.QuoteChar = value;
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06000F77 RID: 3959 RVA: 0x0005E22C File Offset: 0x0005C42C
		public override JsonToken TokenType
		{
			get
			{
				return this._innerReader.TokenType;
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000F78 RID: 3960 RVA: 0x0005E23C File Offset: 0x0005C43C
		public override object Value
		{
			get
			{
				return this._innerReader.Value;
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000F79 RID: 3961 RVA: 0x0005E24C File Offset: 0x0005C44C
		public override Type ValueType
		{
			get
			{
				return this._innerReader.ValueType;
			}
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0005E25C File Offset: 0x0005C45C
		public override void Close()
		{
			this._innerReader.Close();
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0005E26C File Offset: 0x0005C46C
		bool IJsonLineInfo.HasLineInfo()
		{
			IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
			return jsonLineInfo != null && jsonLineInfo.HasLineInfo();
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000F7C RID: 3964 RVA: 0x0005E298 File Offset: 0x0005C498
		int IJsonLineInfo.LineNumber
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LineNumber;
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000F7D RID: 3965 RVA: 0x0005E2C4 File Offset: 0x0005C4C4
		int IJsonLineInfo.LinePosition
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LinePosition;
			}
		}

		// Token: 0x040005F5 RID: 1525
		private readonly JsonReader _innerReader;

		// Token: 0x040005F6 RID: 1526
		private readonly JsonTextWriter _textWriter;

		// Token: 0x040005F7 RID: 1527
		private readonly StringWriter _sw;
	}
}
