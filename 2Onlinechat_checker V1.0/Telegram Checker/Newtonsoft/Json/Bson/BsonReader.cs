using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Bson
{
	// Token: 0x0200018C RID: 396
	[Obsolete("BSON reading and writing has been moved to its own package. See https://www.nuget.org/packages/Newtonsoft.Json.Bson for more details.")]
	public class BsonReader : JsonReader
	{
		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x0600147B RID: 5243 RVA: 0x000716C0 File Offset: 0x0006F8C0
		// (set) Token: 0x0600147C RID: 5244 RVA: 0x000716C8 File Offset: 0x0006F8C8
		[Obsolete("JsonNet35BinaryCompatibility will be removed in a future version of Json.NET.")]
		public bool JsonNet35BinaryCompatibility
		{
			get
			{
				return this._jsonNet35BinaryCompatibility;
			}
			set
			{
				this._jsonNet35BinaryCompatibility = value;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x0600147D RID: 5245 RVA: 0x000716D4 File Offset: 0x0006F8D4
		// (set) Token: 0x0600147E RID: 5246 RVA: 0x000716DC File Offset: 0x0006F8DC
		public bool ReadRootValueAsArray
		{
			get
			{
				return this._readRootValueAsArray;
			}
			set
			{
				this._readRootValueAsArray = value;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x0600147F RID: 5247 RVA: 0x000716E8 File Offset: 0x0006F8E8
		// (set) Token: 0x06001480 RID: 5248 RVA: 0x000716F0 File Offset: 0x0006F8F0
		public DateTimeKind DateTimeKindHandling
		{
			get
			{
				return this._dateTimeKindHandling;
			}
			set
			{
				this._dateTimeKindHandling = value;
			}
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x000716FC File Offset: 0x0006F8FC
		public BsonReader(Stream stream)
			: this(stream, false, DateTimeKind.Local)
		{
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x00071708 File Offset: 0x0006F908
		public BsonReader(BinaryReader reader)
			: this(reader, false, DateTimeKind.Local)
		{
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x00071714 File Offset: 0x0006F914
		public BsonReader(Stream stream, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
		{
			ValidationUtils.ArgumentNotNull(stream, "stream");
			this._reader = new BinaryReader(stream);
			this._stack = new List<BsonReader.ContainerContext>();
			this._readRootValueAsArray = readRootValueAsArray;
			this._dateTimeKindHandling = dateTimeKindHandling;
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x0007174C File Offset: 0x0006F94C
		public BsonReader(BinaryReader reader, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this._reader = reader;
			this._stack = new List<BsonReader.ContainerContext>();
			this._readRootValueAsArray = readRootValueAsArray;
			this._dateTimeKindHandling = dateTimeKindHandling;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00071780 File Offset: 0x0006F980
		private string ReadElement()
		{
			this._currentElementType = this.ReadType();
			return this.ReadString();
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x00071794 File Offset: 0x0006F994
		public override bool Read()
		{
			bool flag2;
			try
			{
				bool flag;
				switch (this._bsonReaderState)
				{
				case BsonReader.BsonReaderState.Normal:
					flag = this.ReadNormal();
					break;
				case BsonReader.BsonReaderState.ReferenceStart:
				case BsonReader.BsonReaderState.ReferenceRef:
				case BsonReader.BsonReaderState.ReferenceId:
					flag = this.ReadReference();
					break;
				case BsonReader.BsonReaderState.CodeWScopeStart:
				case BsonReader.BsonReaderState.CodeWScopeCode:
				case BsonReader.BsonReaderState.CodeWScopeScope:
				case BsonReader.BsonReaderState.CodeWScopeScopeObject:
				case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
					flag = this.ReadCodeWScope();
					break;
				default:
					throw JsonReaderException.Create(this, "Unexpected state: {0}".FormatWith(CultureInfo.InvariantCulture, this._bsonReaderState));
				}
				if (!flag)
				{
					base.SetToken(JsonToken.None);
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
			}
			catch (EndOfStreamException)
			{
				base.SetToken(JsonToken.None);
				flag2 = false;
			}
			return flag2;
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x00071858 File Offset: 0x0006FA58
		public override void Close()
		{
			base.Close();
			if (base.CloseInput)
			{
				BinaryReader reader = this._reader;
				if (reader == null)
				{
					return;
				}
				reader.Close();
			}
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x00071880 File Offset: 0x0006FA80
		private bool ReadCodeWScope()
		{
			switch (this._bsonReaderState)
			{
			case BsonReader.BsonReaderState.CodeWScopeStart:
				base.SetToken(JsonToken.PropertyName, "$code");
				this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeCode;
				return true;
			case BsonReader.BsonReaderState.CodeWScopeCode:
				this.ReadInt32();
				base.SetToken(JsonToken.String, this.ReadLengthString());
				this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScope;
				return true;
			case BsonReader.BsonReaderState.CodeWScopeScope:
			{
				if (base.CurrentState == JsonReader.State.PostValue)
				{
					base.SetToken(JsonToken.PropertyName, "$scope");
					return true;
				}
				base.SetToken(JsonToken.StartObject);
				this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeObject;
				BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
				this.PushContext(containerContext);
				containerContext.Length = this.ReadInt32();
				return true;
			}
			case BsonReader.BsonReaderState.CodeWScopeScopeObject:
			{
				bool flag = this.ReadNormal();
				if (flag && this.TokenType == JsonToken.EndObject)
				{
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeEnd;
				}
				return flag;
			}
			case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
				base.SetToken(JsonToken.EndObject);
				this._bsonReaderState = BsonReader.BsonReaderState.Normal;
				return true;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x00071968 File Offset: 0x0006FB68
		private bool ReadReference()
		{
			JsonReader.State currentState = base.CurrentState;
			if (currentState != JsonReader.State.Property)
			{
				if (currentState == JsonReader.State.ObjectStart)
				{
					base.SetToken(JsonToken.PropertyName, "$ref");
					this._bsonReaderState = BsonReader.BsonReaderState.ReferenceRef;
					return true;
				}
				if (currentState != JsonReader.State.PostValue)
				{
					throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + base.CurrentState);
				}
				if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
				{
					base.SetToken(JsonToken.PropertyName, "$id");
					this._bsonReaderState = BsonReader.BsonReaderState.ReferenceId;
					return true;
				}
				if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceId)
				{
					base.SetToken(JsonToken.EndObject);
					this._bsonReaderState = BsonReader.BsonReaderState.Normal;
					return true;
				}
				throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + this._bsonReaderState);
			}
			else
			{
				if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
				{
					base.SetToken(JsonToken.String, this.ReadLengthString());
					return true;
				}
				if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceId)
				{
					base.SetToken(JsonToken.Bytes, this.ReadBytes(12));
					return true;
				}
				throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + this._bsonReaderState);
			}
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x00071A7C File Offset: 0x0006FC7C
		private bool ReadNormal()
		{
			switch (base.CurrentState)
			{
			case JsonReader.State.Start:
			{
				JsonToken jsonToken = ((!this._readRootValueAsArray) ? JsonToken.StartObject : JsonToken.StartArray);
				BsonType bsonType = ((!this._readRootValueAsArray) ? BsonType.Object : BsonType.Array);
				base.SetToken(jsonToken);
				BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(bsonType);
				this.PushContext(containerContext);
				containerContext.Length = this.ReadInt32();
				return true;
			}
			case JsonReader.State.Complete:
			case JsonReader.State.Closed:
				return false;
			case JsonReader.State.Property:
				this.ReadType(this._currentElementType);
				return true;
			case JsonReader.State.ObjectStart:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.PostValue:
			{
				BsonReader.ContainerContext currentContext = this._currentContext;
				if (currentContext == null)
				{
					return false;
				}
				int num = currentContext.Length - 1;
				if (currentContext.Position < num)
				{
					if (currentContext.Type == BsonType.Array)
					{
						this.ReadElement();
						this.ReadType(this._currentElementType);
						return true;
					}
					base.SetToken(JsonToken.PropertyName, this.ReadElement());
					return true;
				}
				else
				{
					if (currentContext.Position != num)
					{
						throw JsonReaderException.Create(this, "Read past end of current container context.");
					}
					if (this.ReadByte() != 0)
					{
						throw JsonReaderException.Create(this, "Unexpected end of object byte value.");
					}
					this.PopContext();
					if (this._currentContext != null)
					{
						this.MovePosition(currentContext.Length);
					}
					JsonToken jsonToken2 = ((currentContext.Type == BsonType.Object) ? JsonToken.EndObject : JsonToken.EndArray);
					base.SetToken(jsonToken2);
					return true;
				}
				break;
			}
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
			case JsonReader.State.Error:
			case JsonReader.State.Finished:
				return false;
			}
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x00071BF8 File Offset: 0x0006FDF8
		private void PopContext()
		{
			this._stack.RemoveAt(this._stack.Count - 1);
			if (this._stack.Count == 0)
			{
				this._currentContext = null;
				return;
			}
			this._currentContext = this._stack[this._stack.Count - 1];
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x00071C58 File Offset: 0x0006FE58
		private void PushContext(BsonReader.ContainerContext newContext)
		{
			this._stack.Add(newContext);
			this._currentContext = newContext;
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x00071C70 File Offset: 0x0006FE70
		private byte ReadByte()
		{
			this.MovePosition(1);
			return this._reader.ReadByte();
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x00071C84 File Offset: 0x0006FE84
		private void ReadType(BsonType type)
		{
			switch (type)
			{
			case BsonType.Number:
			{
				double num = this.ReadDouble();
				if (this._floatParseHandling == FloatParseHandling.Decimal)
				{
					base.SetToken(JsonToken.Float, Convert.ToDecimal(num, CultureInfo.InvariantCulture));
					return;
				}
				base.SetToken(JsonToken.Float, num);
				return;
			}
			case BsonType.String:
			case BsonType.Symbol:
				base.SetToken(JsonToken.String, this.ReadLengthString());
				return;
			case BsonType.Object:
			{
				base.SetToken(JsonToken.StartObject);
				BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
				this.PushContext(containerContext);
				containerContext.Length = this.ReadInt32();
				return;
			}
			case BsonType.Array:
			{
				base.SetToken(JsonToken.StartArray);
				BsonReader.ContainerContext containerContext2 = new BsonReader.ContainerContext(BsonType.Array);
				this.PushContext(containerContext2);
				containerContext2.Length = this.ReadInt32();
				return;
			}
			case BsonType.Binary:
			{
				BsonBinaryType bsonBinaryType;
				byte[] array = this.ReadBinary(out bsonBinaryType);
				object obj = ((bsonBinaryType != BsonBinaryType.Uuid) ? array : new Guid(array));
				base.SetToken(JsonToken.Bytes, obj);
				return;
			}
			case BsonType.Undefined:
				base.SetToken(JsonToken.Undefined);
				return;
			case BsonType.Oid:
			{
				byte[] array2 = this.ReadBytes(12);
				base.SetToken(JsonToken.Bytes, array2);
				return;
			}
			case BsonType.Boolean:
			{
				bool flag = Convert.ToBoolean(this.ReadByte());
				base.SetToken(JsonToken.Boolean, flag);
				return;
			}
			case BsonType.Date:
			{
				DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime(this.ReadInt64());
				DateTimeKind dateTimeKindHandling = this.DateTimeKindHandling;
				DateTime dateTime2;
				if (dateTimeKindHandling != DateTimeKind.Unspecified)
				{
					if (dateTimeKindHandling != DateTimeKind.Local)
					{
						dateTime2 = dateTime;
					}
					else
					{
						dateTime2 = dateTime.ToLocalTime();
					}
				}
				else
				{
					dateTime2 = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
				}
				base.SetToken(JsonToken.Date, dateTime2);
				return;
			}
			case BsonType.Null:
				base.SetToken(JsonToken.Null);
				return;
			case BsonType.Regex:
			{
				string text = this.ReadString();
				string text2 = this.ReadString();
				string text3 = "/" + text + "/" + text2;
				base.SetToken(JsonToken.String, text3);
				return;
			}
			case BsonType.Reference:
				base.SetToken(JsonToken.StartObject);
				this._bsonReaderState = BsonReader.BsonReaderState.ReferenceStart;
				return;
			case BsonType.Code:
				base.SetToken(JsonToken.String, this.ReadLengthString());
				return;
			case BsonType.CodeWScope:
				base.SetToken(JsonToken.StartObject);
				this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeStart;
				return;
			case BsonType.Integer:
				base.SetToken(JsonToken.Integer, (long)this.ReadInt32());
				return;
			case BsonType.TimeStamp:
			case BsonType.Long:
				base.SetToken(JsonToken.Integer, this.ReadInt64());
				return;
			default:
				throw new ArgumentOutOfRangeException("type", "Unexpected BsonType value: " + type);
			}
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x00071EE8 File Offset: 0x000700E8
		private byte[] ReadBinary(out BsonBinaryType binaryType)
		{
			int num = this.ReadInt32();
			binaryType = (BsonBinaryType)this.ReadByte();
			if (binaryType == BsonBinaryType.BinaryOld && !this._jsonNet35BinaryCompatibility)
			{
				num = this.ReadInt32();
			}
			return this.ReadBytes(num);
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x00071F2C File Offset: 0x0007012C
		private string ReadString()
		{
			this.EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = num2;
				byte b;
				while (num3 < 128 && (b = this._reader.ReadByte()) > 0)
				{
					this._byteBuffer[num3++] = b;
				}
				num4 = num3 - num2;
				num += num4;
				if (num3 < 128 && stringBuilder == null)
				{
					break;
				}
				int lastFullCharStop = this.GetLastFullCharStop(num3 - 1);
				int chars = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(256);
				}
				stringBuilder.Append(this._charBuffer, 0, chars);
				if (lastFullCharStop < num4 - 1)
				{
					num2 = num4 - lastFullCharStop - 1;
					Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, num2);
				}
				else
				{
					if (num3 < 128)
					{
						goto Block_6;
					}
					num2 = 0;
				}
			}
			int chars2 = Encoding.UTF8.GetChars(this._byteBuffer, 0, num4, this._charBuffer, 0);
			this.MovePosition(num + 1);
			return new string(this._charBuffer, 0, chars2);
			Block_6:
			this.MovePosition(num + 1);
			return stringBuilder.ToString();
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x00072064 File Offset: 0x00070264
		private string ReadLengthString()
		{
			int num = this.ReadInt32();
			this.MovePosition(num);
			string @string = this.GetString(num - 1);
			this._reader.ReadByte();
			return @string;
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x00072098 File Offset: 0x00070298
		private string GetString(int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			this.EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = ((length - num > 128 - num2) ? (128 - num2) : (length - num));
				num4 = this._reader.Read(this._byteBuffer, num2, num3);
				if (num4 == 0)
				{
					break;
				}
				num += num4;
				num4 += num2;
				if (num4 == length)
				{
					goto Block_4;
				}
				int lastFullCharStop = this.GetLastFullCharStop(num4 - 1);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(length);
				}
				int chars = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
				stringBuilder.Append(this._charBuffer, 0, chars);
				if (lastFullCharStop < num4 - 1)
				{
					num2 = num4 - lastFullCharStop - 1;
					Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, num2);
				}
				else
				{
					num2 = 0;
				}
				if (num >= length)
				{
					goto Block_7;
				}
			}
			throw new EndOfStreamException("Unable to read beyond the end of the stream.");
			Block_4:
			int chars2 = Encoding.UTF8.GetChars(this._byteBuffer, 0, num4, this._charBuffer, 0);
			return new string(this._charBuffer, 0, chars2);
			Block_7:
			return stringBuilder.ToString();
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x000721CC File Offset: 0x000703CC
		private int GetLastFullCharStop(int start)
		{
			int i = start;
			int num = 0;
			while (i >= 0)
			{
				num = this.BytesInSequence(this._byteBuffer[i]);
				if (num == 0)
				{
					i--;
				}
				else
				{
					if (num != 1)
					{
						i--;
						break;
					}
					break;
				}
			}
			if (num == start - i)
			{
				return start;
			}
			return i;
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x00072224 File Offset: 0x00070424
		private int BytesInSequence(byte b)
		{
			if (b <= BsonReader.SeqRange1[1])
			{
				return 1;
			}
			if (b >= BsonReader.SeqRange2[0] && b <= BsonReader.SeqRange2[1])
			{
				return 2;
			}
			if (b >= BsonReader.SeqRange3[0] && b <= BsonReader.SeqRange3[1])
			{
				return 3;
			}
			if (b >= BsonReader.SeqRange4[0] && b <= BsonReader.SeqRange4[1])
			{
				return 4;
			}
			return 0;
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x0007229C File Offset: 0x0007049C
		private void EnsureBuffers()
		{
			if (this._byteBuffer == null)
			{
				this._byteBuffer = new byte[128];
			}
			if (this._charBuffer == null)
			{
				int maxCharCount = Encoding.UTF8.GetMaxCharCount(128);
				this._charBuffer = new char[maxCharCount];
			}
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x000722F0 File Offset: 0x000704F0
		private double ReadDouble()
		{
			this.MovePosition(8);
			return this._reader.ReadDouble();
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x00072304 File Offset: 0x00070504
		private int ReadInt32()
		{
			this.MovePosition(4);
			return this._reader.ReadInt32();
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x00072318 File Offset: 0x00070518
		private long ReadInt64()
		{
			this.MovePosition(8);
			return this._reader.ReadInt64();
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x0007232C File Offset: 0x0007052C
		private BsonType ReadType()
		{
			this.MovePosition(1);
			return (BsonType)this._reader.ReadSByte();
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x00072340 File Offset: 0x00070540
		private void MovePosition(int count)
		{
			this._currentContext.Position += count;
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x00072358 File Offset: 0x00070558
		private byte[] ReadBytes(int count)
		{
			this.MovePosition(count);
			return this._reader.ReadBytes(count);
		}

		// Token: 0x04000729 RID: 1833
		private const int MaxCharBytesSize = 128;

		// Token: 0x0400072A RID: 1834
		private static readonly byte[] SeqRange1 = new byte[] { 0, 127 };

		// Token: 0x0400072B RID: 1835
		private static readonly byte[] SeqRange2 = new byte[] { 194, 223 };

		// Token: 0x0400072C RID: 1836
		private static readonly byte[] SeqRange3 = new byte[] { 224, 239 };

		// Token: 0x0400072D RID: 1837
		private static readonly byte[] SeqRange4 = new byte[] { 240, 244 };

		// Token: 0x0400072E RID: 1838
		private readonly BinaryReader _reader;

		// Token: 0x0400072F RID: 1839
		private readonly List<BsonReader.ContainerContext> _stack;

		// Token: 0x04000730 RID: 1840
		private byte[] _byteBuffer;

		// Token: 0x04000731 RID: 1841
		private char[] _charBuffer;

		// Token: 0x04000732 RID: 1842
		private BsonType _currentElementType;

		// Token: 0x04000733 RID: 1843
		private BsonReader.BsonReaderState _bsonReaderState;

		// Token: 0x04000734 RID: 1844
		private BsonReader.ContainerContext _currentContext;

		// Token: 0x04000735 RID: 1845
		private bool _readRootValueAsArray;

		// Token: 0x04000736 RID: 1846
		private bool _jsonNet35BinaryCompatibility;

		// Token: 0x04000737 RID: 1847
		private DateTimeKind _dateTimeKindHandling;

		// Token: 0x020002B5 RID: 693
		private enum BsonReaderState
		{
			// Token: 0x04000C8B RID: 3211
			Normal,
			// Token: 0x04000C8C RID: 3212
			ReferenceStart,
			// Token: 0x04000C8D RID: 3213
			ReferenceRef,
			// Token: 0x04000C8E RID: 3214
			ReferenceId,
			// Token: 0x04000C8F RID: 3215
			CodeWScopeStart,
			// Token: 0x04000C90 RID: 3216
			CodeWScopeCode,
			// Token: 0x04000C91 RID: 3217
			CodeWScopeScope,
			// Token: 0x04000C92 RID: 3218
			CodeWScopeScopeObject,
			// Token: 0x04000C93 RID: 3219
			CodeWScopeScopeEnd
		}

		// Token: 0x020002B6 RID: 694
		private class ContainerContext
		{
			// Token: 0x0600184D RID: 6221 RVA: 0x00088EDC File Offset: 0x000870DC
			public ContainerContext(BsonType type)
			{
				this.Type = type;
			}

			// Token: 0x04000C94 RID: 3220
			public readonly BsonType Type;

			// Token: 0x04000C95 RID: 3221
			public int Length;

			// Token: 0x04000C96 RID: 3222
			public int Position;
		}
	}
}
