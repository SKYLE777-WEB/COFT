using System;
using System.Globalization;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000151 RID: 337
	public class JTokenWriter : JsonWriter
	{
		// Token: 0x0600128B RID: 4747 RVA: 0x00068AE0 File Offset: 0x00066CE0
		internal override Task WriteTokenAsync(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments, CancellationToken cancellationToken)
		{
			if (reader is JTokenReader)
			{
				this.WriteToken(reader, writeChildren, writeDateConstructorAsDate, writeComments);
				return AsyncUtils.CompletedTask;
			}
			return base.WriteTokenSyncReadingAsync(reader, cancellationToken);
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x0600128C RID: 4748 RVA: 0x00068B08 File Offset: 0x00066D08
		public JToken CurrentToken
		{
			get
			{
				return this._current;
			}
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x0600128D RID: 4749 RVA: 0x00068B10 File Offset: 0x00066D10
		public JToken Token
		{
			get
			{
				if (this._token != null)
				{
					return this._token;
				}
				return this._value;
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x00068B2C File Offset: 0x00066D2C
		public JTokenWriter(JContainer container)
		{
			ValidationUtils.ArgumentNotNull(container, "container");
			this._token = container;
			this._parent = container;
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x00068B50 File Offset: 0x00066D50
		public JTokenWriter()
		{
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x00068B58 File Offset: 0x00066D58
		public override void Flush()
		{
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x00068B5C File Offset: 0x00066D5C
		public override void Close()
		{
			base.Close();
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x00068B64 File Offset: 0x00066D64
		public override void WriteStartObject()
		{
			base.WriteStartObject();
			this.AddParent(new JObject());
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x00068B78 File Offset: 0x00066D78
		private void AddParent(JContainer container)
		{
			if (this._parent == null)
			{
				this._token = container;
			}
			else
			{
				this._parent.AddAndSkipParentCheck(container);
			}
			this._parent = container;
			this._current = container;
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x00068BAC File Offset: 0x00066DAC
		private void RemoveParent()
		{
			this._current = this._parent;
			this._parent = this._parent.Parent;
			if (this._parent != null && this._parent.Type == JTokenType.Property)
			{
				this._parent = this._parent.Parent;
			}
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x00068C08 File Offset: 0x00066E08
		public override void WriteStartArray()
		{
			base.WriteStartArray();
			this.AddParent(new JArray());
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x00068C1C File Offset: 0x00066E1C
		public override void WriteStartConstructor(string name)
		{
			base.WriteStartConstructor(name);
			this.AddParent(new JConstructor(name));
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x00068C34 File Offset: 0x00066E34
		protected override void WriteEnd(JsonToken token)
		{
			this.RemoveParent();
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x00068C3C File Offset: 0x00066E3C
		public override void WritePropertyName(string name)
		{
			JObject jobject = this._parent as JObject;
			if (jobject != null)
			{
				jobject.Remove(name);
			}
			this.AddParent(new JProperty(name));
			base.WritePropertyName(name);
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00068C70 File Offset: 0x00066E70
		private void AddValue(object value, JsonToken token)
		{
			this.AddValue(new JValue(value), token);
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x00068C80 File Offset: 0x00066E80
		internal void AddValue(JValue value, JsonToken token)
		{
			if (this._parent != null)
			{
				this._parent.Add(value);
				this._current = this._parent.Last;
				if (this._parent.Type == JTokenType.Property)
				{
					this._parent = this._parent.Parent;
					return;
				}
			}
			else
			{
				this._value = value ?? JValue.CreateNull();
				this._current = this._value;
			}
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x00068CFC File Offset: 0x00066EFC
		public override void WriteValue(object value)
		{
			if (value is BigInteger)
			{
				base.InternalWriteValue(JsonToken.Integer);
				this.AddValue(value, JsonToken.Integer);
				return;
			}
			base.WriteValue(value);
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x00068D20 File Offset: 0x00066F20
		public override void WriteNull()
		{
			base.WriteNull();
			this.AddValue(null, JsonToken.Null);
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x00068D34 File Offset: 0x00066F34
		public override void WriteUndefined()
		{
			base.WriteUndefined();
			this.AddValue(null, JsonToken.Undefined);
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x00068D48 File Offset: 0x00066F48
		public override void WriteRaw(string json)
		{
			base.WriteRaw(json);
			this.AddValue(new JRaw(json), JsonToken.Raw);
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x00068D60 File Offset: 0x00066F60
		public override void WriteComment(string text)
		{
			base.WriteComment(text);
			this.AddValue(JValue.CreateComment(text), JsonToken.Comment);
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x00068D78 File Offset: 0x00066F78
		public override void WriteValue(string value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x00068D8C File Offset: 0x00066F8C
		public override void WriteValue(int value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x00068DA4 File Offset: 0x00066FA4
		[CLSCompliant(false)]
		public override void WriteValue(uint value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x00068DBC File Offset: 0x00066FBC
		public override void WriteValue(long value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x00068DD4 File Offset: 0x00066FD4
		[CLSCompliant(false)]
		public override void WriteValue(ulong value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x00068DEC File Offset: 0x00066FEC
		public override void WriteValue(float value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Float);
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x00068E04 File Offset: 0x00067004
		public override void WriteValue(double value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Float);
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x00068E1C File Offset: 0x0006701C
		public override void WriteValue(bool value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Boolean);
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x00068E34 File Offset: 0x00067034
		public override void WriteValue(short value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x00068E4C File Offset: 0x0006704C
		[CLSCompliant(false)]
		public override void WriteValue(ushort value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x00068E64 File Offset: 0x00067064
		public override void WriteValue(char value)
		{
			base.WriteValue(value);
			string text = value.ToString(CultureInfo.InvariantCulture);
			this.AddValue(text, JsonToken.String);
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00068E94 File Offset: 0x00067094
		public override void WriteValue(byte value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x00068EAC File Offset: 0x000670AC
		[CLSCompliant(false)]
		public override void WriteValue(sbyte value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x00068EC4 File Offset: 0x000670C4
		public override void WriteValue(decimal value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Float);
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x00068EDC File Offset: 0x000670DC
		public override void WriteValue(DateTime value)
		{
			base.WriteValue(value);
			value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
			this.AddValue(value, JsonToken.Date);
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x00068F04 File Offset: 0x00067104
		public override void WriteValue(DateTimeOffset value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Date);
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x00068F1C File Offset: 0x0006711C
		public override void WriteValue(byte[] value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Bytes);
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x00068F30 File Offset: 0x00067130
		public override void WriteValue(TimeSpan value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x00068F48 File Offset: 0x00067148
		public override void WriteValue(Guid value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x00068F60 File Offset: 0x00067160
		public override void WriteValue(Uri value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x00068F74 File Offset: 0x00067174
		internal override void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments)
		{
			JTokenReader jtokenReader = reader as JTokenReader;
			if (jtokenReader == null || !writeChildren || !writeDateConstructorAsDate || !writeComments)
			{
				base.WriteToken(reader, writeChildren, writeDateConstructorAsDate, writeComments);
				return;
			}
			if (jtokenReader.TokenType == JsonToken.None && !jtokenReader.Read())
			{
				return;
			}
			JToken jtoken = jtokenReader.CurrentToken.CloneToken();
			if (this._parent != null)
			{
				this._parent.Add(jtoken);
				this._current = this._parent.Last;
				if (this._parent.Type == JTokenType.Property)
				{
					this._parent = this._parent.Parent;
					base.InternalWriteValue(JsonToken.Null);
				}
			}
			else
			{
				this._current = jtoken;
				if (this._token == null && this._value == null)
				{
					this._token = jtoken as JContainer;
					this._value = jtoken as JValue;
				}
			}
			jtokenReader.Skip();
		}

		// Token: 0x040006C2 RID: 1730
		private JContainer _token;

		// Token: 0x040006C3 RID: 1731
		private JContainer _parent;

		// Token: 0x040006C4 RID: 1732
		private JValue _value;

		// Token: 0x040006C5 RID: 1733
		private JToken _current;
	}
}
