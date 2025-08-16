using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x02000198 RID: 408
	[Obsolete("BSON reading and writing has been moved to its own package. See https://www.nuget.org/packages/Newtonsoft.Json.Bson for more details.")]
	public class BsonWriter : JsonWriter
	{
		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x060014C7 RID: 5319 RVA: 0x0007262C File Offset: 0x0007082C
		// (set) Token: 0x060014C8 RID: 5320 RVA: 0x0007263C File Offset: 0x0007083C
		public DateTimeKind DateTimeKindHandling
		{
			get
			{
				return this._writer.DateTimeKindHandling;
			}
			set
			{
				this._writer.DateTimeKindHandling = value;
			}
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x0007264C File Offset: 0x0007084C
		public BsonWriter(Stream stream)
		{
			ValidationUtils.ArgumentNotNull(stream, "stream");
			this._writer = new BsonBinaryWriter(new BinaryWriter(stream));
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x00072670 File Offset: 0x00070870
		public BsonWriter(BinaryWriter writer)
		{
			ValidationUtils.ArgumentNotNull(writer, "writer");
			this._writer = new BsonBinaryWriter(writer);
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x00072690 File Offset: 0x00070890
		public override void Flush()
		{
			this._writer.Flush();
		}

		// Token: 0x060014CC RID: 5324 RVA: 0x000726A0 File Offset: 0x000708A0
		protected override void WriteEnd(JsonToken token)
		{
			base.WriteEnd(token);
			this.RemoveParent();
			if (base.Top == 0)
			{
				this._writer.WriteToken(this._root);
			}
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x000726CC File Offset: 0x000708CC
		public override void WriteComment(string text)
		{
			throw JsonWriterException.Create(this, "Cannot write JSON comment as BSON.", null);
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x000726DC File Offset: 0x000708DC
		public override void WriteStartConstructor(string name)
		{
			throw JsonWriterException.Create(this, "Cannot write JSON constructor as BSON.", null);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x000726EC File Offset: 0x000708EC
		public override void WriteRaw(string json)
		{
			throw JsonWriterException.Create(this, "Cannot write raw JSON as BSON.", null);
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x000726FC File Offset: 0x000708FC
		public override void WriteRawValue(string json)
		{
			throw JsonWriterException.Create(this, "Cannot write raw JSON as BSON.", null);
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0007270C File Offset: 0x0007090C
		public override void WriteStartArray()
		{
			base.WriteStartArray();
			this.AddParent(new BsonArray());
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x00072720 File Offset: 0x00070920
		public override void WriteStartObject()
		{
			base.WriteStartObject();
			this.AddParent(new BsonObject());
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x00072734 File Offset: 0x00070934
		public override void WritePropertyName(string name)
		{
			base.WritePropertyName(name);
			this._propertyName = name;
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x00072744 File Offset: 0x00070944
		public override void Close()
		{
			base.Close();
			if (base.CloseOutput)
			{
				BsonBinaryWriter writer = this._writer;
				if (writer == null)
				{
					return;
				}
				writer.Close();
			}
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x0007276C File Offset: 0x0007096C
		private void AddParent(BsonToken container)
		{
			this.AddToken(container);
			this._parent = container;
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0007277C File Offset: 0x0007097C
		private void RemoveParent()
		{
			this._parent = this._parent.Parent;
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x00072790 File Offset: 0x00070990
		private void AddValue(object value, BsonType type)
		{
			this.AddToken(new BsonValue(value, type));
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x000727A0 File Offset: 0x000709A0
		internal void AddToken(BsonToken token)
		{
			if (this._parent != null)
			{
				BsonObject bsonObject = this._parent as BsonObject;
				if (bsonObject != null)
				{
					bsonObject.Add(this._propertyName, token);
					this._propertyName = null;
					return;
				}
				((BsonArray)this._parent).Add(token);
				return;
			}
			else
			{
				if (token.Type != BsonType.Object && token.Type != BsonType.Array)
				{
					throw JsonWriterException.Create(this, "Error writing {0} value. BSON must start with an Object or Array.".FormatWith(CultureInfo.InvariantCulture, token.Type), null);
				}
				this._parent = token;
				this._root = token;
				return;
			}
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00072840 File Offset: 0x00070A40
		public override void WriteValue(object value)
		{
			if (value is BigInteger)
			{
				base.SetWriteState(JsonToken.Integer, null);
				this.AddToken(new BsonBinary(((BigInteger)value).ToByteArray(), BsonBinaryType.Binary));
				return;
			}
			base.WriteValue(value);
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x00072888 File Offset: 0x00070A88
		public override void WriteNull()
		{
			base.WriteNull();
			this.AddToken(BsonEmpty.Null);
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0007289C File Offset: 0x00070A9C
		public override void WriteUndefined()
		{
			base.WriteUndefined();
			this.AddToken(BsonEmpty.Undefined);
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x000728B0 File Offset: 0x00070AB0
		public override void WriteValue(string value)
		{
			base.WriteValue(value);
			this.AddToken((value == null) ? BsonEmpty.Null : new BsonString(value, true));
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x000728D8 File Offset: 0x00070AD8
		public override void WriteValue(int value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Integer);
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x000728F0 File Offset: 0x00070AF0
		[CLSCompliant(false)]
		public override void WriteValue(uint value)
		{
			if (value > 2147483647U)
			{
				throw JsonWriterException.Create(this, "Value is too large to fit in a signed 32 bit integer. BSON does not support unsigned values.", null);
			}
			base.WriteValue(value);
			this.AddValue(value, BsonType.Integer);
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x00072920 File Offset: 0x00070B20
		public override void WriteValue(long value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Long);
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x00072938 File Offset: 0x00070B38
		[CLSCompliant(false)]
		public override void WriteValue(ulong value)
		{
			if (value > 9223372036854775807UL)
			{
				throw JsonWriterException.Create(this, "Value is too large to fit in a signed 64 bit integer. BSON does not support unsigned values.", null);
			}
			base.WriteValue(value);
			this.AddValue(value, BsonType.Long);
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x0007296C File Offset: 0x00070B6C
		public override void WriteValue(float value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Number);
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x00072984 File Offset: 0x00070B84
		public override void WriteValue(double value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Number);
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0007299C File Offset: 0x00070B9C
		public override void WriteValue(bool value)
		{
			base.WriteValue(value);
			this.AddToken(value ? BsonBoolean.True : BsonBoolean.False);
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x000729C0 File Offset: 0x00070BC0
		public override void WriteValue(short value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Integer);
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x000729D8 File Offset: 0x00070BD8
		[CLSCompliant(false)]
		public override void WriteValue(ushort value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Integer);
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x000729F0 File Offset: 0x00070BF0
		public override void WriteValue(char value)
		{
			base.WriteValue(value);
			string text = value.ToString(CultureInfo.InvariantCulture);
			this.AddToken(new BsonString(text, true));
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x00072A24 File Offset: 0x00070C24
		public override void WriteValue(byte value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Integer);
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x00072A3C File Offset: 0x00070C3C
		[CLSCompliant(false)]
		public override void WriteValue(sbyte value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Integer);
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x00072A54 File Offset: 0x00070C54
		public override void WriteValue(decimal value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Number);
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x00072A6C File Offset: 0x00070C6C
		public override void WriteValue(DateTime value)
		{
			base.WriteValue(value);
			value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
			this.AddValue(value, BsonType.Date);
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x00072A94 File Offset: 0x00070C94
		public override void WriteValue(DateTimeOffset value)
		{
			base.WriteValue(value);
			this.AddValue(value, BsonType.Date);
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x00072AAC File Offset: 0x00070CAC
		public override void WriteValue(byte[] value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.WriteValue(value);
			this.AddToken(new BsonBinary(value, BsonBinaryType.Binary));
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x00072AD0 File Offset: 0x00070CD0
		public override void WriteValue(Guid value)
		{
			base.WriteValue(value);
			this.AddToken(new BsonBinary(value.ToByteArray(), BsonBinaryType.Uuid));
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x00072AEC File Offset: 0x00070CEC
		public override void WriteValue(TimeSpan value)
		{
			base.WriteValue(value);
			this.AddToken(new BsonString(value.ToString(), true));
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x00072B10 File Offset: 0x00070D10
		public override void WriteValue(Uri value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.WriteValue(value);
			this.AddToken(new BsonString(value.ToString(), true));
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x00072B50 File Offset: 0x00070D50
		public void WriteObjectId(byte[] value)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			if (value.Length != 12)
			{
				throw JsonWriterException.Create(this, "An object id must be 12 bytes", null);
			}
			base.SetWriteState(JsonToken.Undefined, null);
			this.AddValue(value, BsonType.Oid);
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x00072B88 File Offset: 0x00070D88
		public void WriteRegex(string pattern, string options)
		{
			ValidationUtils.ArgumentNotNull(pattern, "pattern");
			base.SetWriteState(JsonToken.Undefined, null);
			this.AddToken(new BsonRegex(pattern, options));
		}

		// Token: 0x0400075F RID: 1887
		private readonly BsonBinaryWriter _writer;

		// Token: 0x04000760 RID: 1888
		private BsonToken _root;

		// Token: 0x04000761 RID: 1889
		private BsonToken _parent;

		// Token: 0x04000762 RID: 1890
		private string _propertyName;
	}
}
