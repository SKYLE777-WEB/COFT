using System;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200012D RID: 301
	internal class TraceJsonWriter : JsonWriter
	{
		// Token: 0x06000F7E RID: 3966 RVA: 0x0005E2F0 File Offset: 0x0005C4F0
		public TraceJsonWriter(JsonWriter innerWriter)
		{
			this._innerWriter = innerWriter;
			this._sw = new StringWriter(CultureInfo.InvariantCulture);
			this._sw.Write("Serialized JSON: " + Environment.NewLine);
			this._textWriter = new JsonTextWriter(this._sw);
			this._textWriter.Formatting = Formatting.Indented;
			this._textWriter.Culture = innerWriter.Culture;
			this._textWriter.DateFormatHandling = innerWriter.DateFormatHandling;
			this._textWriter.DateFormatString = innerWriter.DateFormatString;
			this._textWriter.DateTimeZoneHandling = innerWriter.DateTimeZoneHandling;
			this._textWriter.FloatFormatHandling = innerWriter.FloatFormatHandling;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0005E3AC File Offset: 0x0005C5AC
		public string GetSerializedJsonMessage()
		{
			return this._sw.ToString();
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0005E3BC File Offset: 0x0005C5BC
		public override void WriteValue(decimal value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0005E3E0 File Offset: 0x0005C5E0
		public override void WriteValue(decimal? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0005E42C File Offset: 0x0005C62C
		public override void WriteValue(bool value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0005E450 File Offset: 0x0005C650
		public override void WriteValue(bool? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x0005E49C File Offset: 0x0005C69C
		public override void WriteValue(byte value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x0005E4C0 File Offset: 0x0005C6C0
		public override void WriteValue(byte? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x0005E50C File Offset: 0x0005C70C
		public override void WriteValue(char value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x0005E530 File Offset: 0x0005C730
		public override void WriteValue(char? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x0005E57C File Offset: 0x0005C77C
		public override void WriteValue(byte[] value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value == null)
			{
				base.WriteUndefined();
				return;
			}
			base.WriteValue(value);
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0005E5AC File Offset: 0x0005C7AC
		public override void WriteValue(DateTime value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x0005E5D0 File Offset: 0x0005C7D0
		public override void WriteValue(DateTime? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x0005E61C File Offset: 0x0005C81C
		public override void WriteValue(DateTimeOffset value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x0005E640 File Offset: 0x0005C840
		public override void WriteValue(DateTimeOffset? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0005E68C File Offset: 0x0005C88C
		public override void WriteValue(double value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x0005E6B0 File Offset: 0x0005C8B0
		public override void WriteValue(double? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x0005E6FC File Offset: 0x0005C8FC
		public override void WriteUndefined()
		{
			this._textWriter.WriteUndefined();
			this._innerWriter.WriteUndefined();
			base.WriteUndefined();
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x0005E71C File Offset: 0x0005C91C
		public override void WriteNull()
		{
			this._textWriter.WriteNull();
			this._innerWriter.WriteNull();
			base.WriteUndefined();
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0005E73C File Offset: 0x0005C93C
		public override void WriteValue(float value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0005E760 File Offset: 0x0005C960
		public override void WriteValue(float? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x0005E7AC File Offset: 0x0005C9AC
		public override void WriteValue(Guid value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x0005E7D0 File Offset: 0x0005C9D0
		public override void WriteValue(Guid? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x0005E81C File Offset: 0x0005CA1C
		public override void WriteValue(int value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0005E840 File Offset: 0x0005CA40
		public override void WriteValue(int? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0005E88C File Offset: 0x0005CA8C
		public override void WriteValue(long value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0005E8B0 File Offset: 0x0005CAB0
		public override void WriteValue(long? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0005E8FC File Offset: 0x0005CAFC
		public override void WriteValue(object value)
		{
			if (value is BigInteger)
			{
				this._textWriter.WriteValue(value);
				this._innerWriter.WriteValue(value);
				base.InternalWriteValue(JsonToken.Integer);
				return;
			}
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value == null)
			{
				base.WriteUndefined();
				return;
			}
			base.WriteValue(value);
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0005E964 File Offset: 0x0005CB64
		public override void WriteValue(sbyte value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0005E988 File Offset: 0x0005CB88
		public override void WriteValue(sbyte? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0005E9D4 File Offset: 0x0005CBD4
		public override void WriteValue(short value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x0005E9F8 File Offset: 0x0005CBF8
		public override void WriteValue(short? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x0005EA44 File Offset: 0x0005CC44
		public override void WriteValue(string value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0005EA68 File Offset: 0x0005CC68
		public override void WriteValue(TimeSpan value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0005EA8C File Offset: 0x0005CC8C
		public override void WriteValue(TimeSpan? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x0005EAD8 File Offset: 0x0005CCD8
		public override void WriteValue(uint value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x0005EAFC File Offset: 0x0005CCFC
		public override void WriteValue(uint? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x0005EB48 File Offset: 0x0005CD48
		public override void WriteValue(ulong value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x0005EB6C File Offset: 0x0005CD6C
		public override void WriteValue(ulong? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x0005EBB8 File Offset: 0x0005CDB8
		public override void WriteValue(Uri value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value == null)
			{
				base.WriteUndefined();
				return;
			}
			base.WriteValue(value);
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0005EBEC File Offset: 0x0005CDEC
		public override void WriteValue(ushort value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			base.WriteValue(value);
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0005EC10 File Offset: 0x0005CE10
		public override void WriteValue(ushort? value)
		{
			this._textWriter.WriteValue(value);
			this._innerWriter.WriteValue(value);
			if (value != null)
			{
				base.WriteValue(value.GetValueOrDefault());
				return;
			}
			base.WriteUndefined();
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0005EC5C File Offset: 0x0005CE5C
		public override void WriteWhitespace(string ws)
		{
			this._textWriter.WriteWhitespace(ws);
			this._innerWriter.WriteWhitespace(ws);
			base.WriteWhitespace(ws);
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0005EC80 File Offset: 0x0005CE80
		public override void WriteComment(string text)
		{
			this._textWriter.WriteComment(text);
			this._innerWriter.WriteComment(text);
			base.WriteComment(text);
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0005ECA4 File Offset: 0x0005CEA4
		public override void WriteStartArray()
		{
			this._textWriter.WriteStartArray();
			this._innerWriter.WriteStartArray();
			base.WriteStartArray();
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0005ECC4 File Offset: 0x0005CEC4
		public override void WriteEndArray()
		{
			this._textWriter.WriteEndArray();
			this._innerWriter.WriteEndArray();
			base.WriteEndArray();
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x0005ECE4 File Offset: 0x0005CEE4
		public override void WriteStartConstructor(string name)
		{
			this._textWriter.WriteStartConstructor(name);
			this._innerWriter.WriteStartConstructor(name);
			base.WriteStartConstructor(name);
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0005ED08 File Offset: 0x0005CF08
		public override void WriteEndConstructor()
		{
			this._textWriter.WriteEndConstructor();
			this._innerWriter.WriteEndConstructor();
			base.WriteEndConstructor();
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0005ED28 File Offset: 0x0005CF28
		public override void WritePropertyName(string name)
		{
			this._textWriter.WritePropertyName(name);
			this._innerWriter.WritePropertyName(name);
			base.WritePropertyName(name);
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0005ED4C File Offset: 0x0005CF4C
		public override void WritePropertyName(string name, bool escape)
		{
			this._textWriter.WritePropertyName(name, escape);
			this._innerWriter.WritePropertyName(name, escape);
			base.WritePropertyName(name);
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x0005ED70 File Offset: 0x0005CF70
		public override void WriteStartObject()
		{
			this._textWriter.WriteStartObject();
			this._innerWriter.WriteStartObject();
			base.WriteStartObject();
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x0005ED90 File Offset: 0x0005CF90
		public override void WriteEndObject()
		{
			this._textWriter.WriteEndObject();
			this._innerWriter.WriteEndObject();
			base.WriteEndObject();
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0005EDB0 File Offset: 0x0005CFB0
		public override void WriteRawValue(string json)
		{
			this._textWriter.WriteRawValue(json);
			this._innerWriter.WriteRawValue(json);
			base.InternalWriteValue(JsonToken.Undefined);
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x0005EDD4 File Offset: 0x0005CFD4
		public override void WriteRaw(string json)
		{
			this._textWriter.WriteRaw(json);
			this._innerWriter.WriteRaw(json);
			base.WriteRaw(json);
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x0005EDF8 File Offset: 0x0005CFF8
		public override void Close()
		{
			this._textWriter.Close();
			this._innerWriter.Close();
			base.Close();
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x0005EE18 File Offset: 0x0005D018
		public override void Flush()
		{
			this._textWriter.Flush();
			this._innerWriter.Flush();
		}

		// Token: 0x040005F8 RID: 1528
		private readonly JsonWriter _innerWriter;

		// Token: 0x040005F9 RID: 1529
		private readonly JsonTextWriter _textWriter;

		// Token: 0x040005FA RID: 1530
		private readonly StringWriter _sw;
	}
}
