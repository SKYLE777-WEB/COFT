using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000AF RID: 175
	public abstract class JsonReader : IDisposable
	{
		// Token: 0x0600081F RID: 2079 RVA: 0x0003A11C File Offset: 0x0003831C
		public virtual Task<bool> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<bool>() ?? this.Read().ToAsync();
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0003A138 File Offset: 0x00038338
		public async Task SkipAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this.TokenType == JsonToken.PropertyName)
			{
				await this.ReadAsync(cancellationToken).ConfigureAwait(false);
			}
			if (JsonTokenUtils.IsStartToken(this.TokenType))
			{
				int depth = this.Depth;
				while (await this.ReadAsync(cancellationToken).ConfigureAwait(false) && depth < this.Depth)
				{
				}
			}
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0003A18C File Offset: 0x0003838C
		internal async Task ReaderReadAndAssertAsync(CancellationToken cancellationToken)
		{
			if (!(await this.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw this.CreateUnexpectedEndException();
			}
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0003A1E0 File Offset: 0x000383E0
		public virtual Task<bool?> ReadAsBooleanAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<bool?>() ?? Task.FromResult<bool?>(this.ReadAsBoolean());
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x0003A1FC File Offset: 0x000383FC
		public virtual Task<byte[]> ReadAsBytesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<byte[]>() ?? Task.FromResult<byte[]>(this.ReadAsBytes());
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x0003A218 File Offset: 0x00038418
		internal async Task<byte[]> ReadArrayIntoByteArrayAsync(CancellationToken cancellationToken)
		{
			List<byte> buffer = new List<byte>();
			do
			{
				if (!(await this.ReadAsync(cancellationToken).ConfigureAwait(false)))
				{
					this.SetToken(JsonToken.None);
				}
			}
			while (!this.ReadArrayElementIntoByteArrayReportDone(buffer));
			byte[] array = buffer.ToArray();
			this.SetToken(JsonToken.Bytes, array, false);
			return array;
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x0003A26C File Offset: 0x0003846C
		public virtual Task<DateTime?> ReadAsDateTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<DateTime?>() ?? Task.FromResult<DateTime?>(this.ReadAsDateTime());
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0003A288 File Offset: 0x00038488
		public virtual Task<DateTimeOffset?> ReadAsDateTimeOffsetAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<DateTimeOffset?>() ?? Task.FromResult<DateTimeOffset?>(this.ReadAsDateTimeOffset());
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0003A2A4 File Offset: 0x000384A4
		public virtual Task<decimal?> ReadAsDecimalAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<decimal?>() ?? Task.FromResult<decimal?>(this.ReadAsDecimal());
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0003A2C0 File Offset: 0x000384C0
		public virtual Task<double?> ReadAsDoubleAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult<double?>(this.ReadAsDouble());
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0003A2D0 File Offset: 0x000384D0
		public virtual Task<int?> ReadAsInt32Async(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<int?>() ?? Task.FromResult<int?>(this.ReadAsInt32());
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0003A2EC File Offset: 0x000384EC
		public virtual Task<string> ReadAsStringAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return cancellationToken.CancelIfRequestedAsync<string>() ?? Task.FromResult<string>(this.ReadAsString());
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0003A308 File Offset: 0x00038508
		internal async Task<bool> ReadAndMoveToContentAsync(CancellationToken cancellationToken)
		{
			bool flag = await this.ReadAsync(cancellationToken).ConfigureAwait(false);
			if (flag)
			{
				flag = await this.MoveToContentAsync(cancellationToken).ConfigureAwait(false);
			}
			return flag;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0003A35C File Offset: 0x0003855C
		internal Task<bool> MoveToContentAsync(CancellationToken cancellationToken)
		{
			JsonToken tokenType = this.TokenType;
			if (tokenType == JsonToken.None || tokenType == JsonToken.Comment)
			{
				return this.MoveToContentFromNonContentAsync(cancellationToken);
			}
			return AsyncUtils.True;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0003A390 File Offset: 0x00038590
		private async Task<bool> MoveToContentFromNonContentAsync(CancellationToken cancellationToken)
		{
			while (await this.ReadAsync(cancellationToken).ConfigureAwait(false))
			{
				JsonToken tokenType = this.TokenType;
				if (tokenType != JsonToken.None && tokenType != JsonToken.Comment)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x0003A3E4 File Offset: 0x000385E4
		protected JsonReader.State CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600082F RID: 2095 RVA: 0x0003A3EC File Offset: 0x000385EC
		// (set) Token: 0x06000830 RID: 2096 RVA: 0x0003A3F4 File Offset: 0x000385F4
		public bool CloseInput { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000831 RID: 2097 RVA: 0x0003A400 File Offset: 0x00038600
		// (set) Token: 0x06000832 RID: 2098 RVA: 0x0003A408 File Offset: 0x00038608
		public bool SupportMultipleContent { get; set; }

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000833 RID: 2099 RVA: 0x0003A414 File Offset: 0x00038614
		// (set) Token: 0x06000834 RID: 2100 RVA: 0x0003A41C File Offset: 0x0003861C
		public virtual char QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			protected internal set
			{
				this._quoteChar = value;
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000835 RID: 2101 RVA: 0x0003A428 File Offset: 0x00038628
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x0003A430 File Offset: 0x00038630
		public DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				return this._dateTimeZoneHandling;
			}
			set
			{
				if (value < DateTimeZoneHandling.Local || value > DateTimeZoneHandling.RoundtripKind)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._dateTimeZoneHandling = value;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000837 RID: 2103 RVA: 0x0003A454 File Offset: 0x00038654
		// (set) Token: 0x06000838 RID: 2104 RVA: 0x0003A45C File Offset: 0x0003865C
		public DateParseHandling DateParseHandling
		{
			get
			{
				return this._dateParseHandling;
			}
			set
			{
				if (value < DateParseHandling.None || value > DateParseHandling.DateTimeOffset)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._dateParseHandling = value;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x0003A480 File Offset: 0x00038680
		// (set) Token: 0x0600083A RID: 2106 RVA: 0x0003A488 File Offset: 0x00038688
		public FloatParseHandling FloatParseHandling
		{
			get
			{
				return this._floatParseHandling;
			}
			set
			{
				if (value < FloatParseHandling.Double || value > FloatParseHandling.Decimal)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._floatParseHandling = value;
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x0003A4AC File Offset: 0x000386AC
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x0003A4B4 File Offset: 0x000386B4
		public string DateFormatString
		{
			get
			{
				return this._dateFormatString;
			}
			set
			{
				this._dateFormatString = value;
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x0600083D RID: 2109 RVA: 0x0003A4C0 File Offset: 0x000386C0
		// (set) Token: 0x0600083E RID: 2110 RVA: 0x0003A4C8 File Offset: 0x000386C8
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
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600083F RID: 2111 RVA: 0x0003A514 File Offset: 0x00038714
		public virtual JsonToken TokenType
		{
			get
			{
				return this._tokenType;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000840 RID: 2112 RVA: 0x0003A51C File Offset: 0x0003871C
		public virtual object Value
		{
			get
			{
				return this._value;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000841 RID: 2113 RVA: 0x0003A524 File Offset: 0x00038724
		public virtual Type ValueType
		{
			get
			{
				object value = this._value;
				if (value == null)
				{
					return null;
				}
				return value.GetType();
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000842 RID: 2114 RVA: 0x0003A53C File Offset: 0x0003873C
		public virtual int Depth
		{
			get
			{
				int num = ((this._stack != null) ? this._stack.Count : 0);
				if (JsonTokenUtils.IsStartToken(this.TokenType) || this._currentPosition.Type == JsonContainerType.None)
				{
					return num;
				}
				return num + 1;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000843 RID: 2115 RVA: 0x0003A590 File Offset: 0x00038790
		public virtual string Path
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				JsonPosition? jsonPosition = ((this._currentState != JsonReader.State.ArrayStart && this._currentState != JsonReader.State.ConstructorStart && this._currentState != JsonReader.State.ObjectStart) ? new JsonPosition?(this._currentPosition) : null);
				return JsonPosition.BuildPath(this._stack, jsonPosition);
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x0003A610 File Offset: 0x00038810
		// (set) Token: 0x06000845 RID: 2117 RVA: 0x0003A624 File Offset: 0x00038824
		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? CultureInfo.InvariantCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0003A630 File Offset: 0x00038830
		internal JsonPosition GetPosition(int depth)
		{
			if (this._stack != null && depth < this._stack.Count)
			{
				return this._stack[depth];
			}
			return this._currentPosition;
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0003A664 File Offset: 0x00038864
		protected JsonReader()
		{
			this._currentState = JsonReader.State.Start;
			this._dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			this._dateParseHandling = DateParseHandling.DateTime;
			this._floatParseHandling = FloatParseHandling.Double;
			this.CloseInput = true;
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0003A690 File Offset: 0x00038890
		private void Push(JsonContainerType value)
		{
			this.UpdateScopeWithFinishedValue();
			if (this._currentPosition.Type == JsonContainerType.None)
			{
				this._currentPosition = new JsonPosition(value);
				return;
			}
			if (this._stack == null)
			{
				this._stack = new List<JsonPosition>();
			}
			this._stack.Add(this._currentPosition);
			this._currentPosition = new JsonPosition(value);
			if (this._maxDepth != null && this.Depth + 1 > this._maxDepth && !this._hasExceededMaxDepth)
			{
				this._hasExceededMaxDepth = true;
				throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith(CultureInfo.InvariantCulture, this._maxDepth));
			}
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x0003A768 File Offset: 0x00038968
		private JsonContainerType Pop()
		{
			JsonPosition jsonPosition;
			if (this._stack != null && this._stack.Count > 0)
			{
				jsonPosition = this._currentPosition;
				this._currentPosition = this._stack[this._stack.Count - 1];
				this._stack.RemoveAt(this._stack.Count - 1);
			}
			else
			{
				jsonPosition = this._currentPosition;
				this._currentPosition = default(JsonPosition);
			}
			if (this._maxDepth != null && this.Depth <= this._maxDepth)
			{
				this._hasExceededMaxDepth = false;
			}
			return jsonPosition.Type;
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x0003A834 File Offset: 0x00038A34
		private JsonContainerType Peek()
		{
			return this._currentPosition.Type;
		}

		// Token: 0x0600084B RID: 2123
		public abstract bool Read();

		// Token: 0x0600084C RID: 2124 RVA: 0x0003A844 File Offset: 0x00038A44
		public virtual int? ReadAsInt32()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
				case JsonToken.Integer:
				case JsonToken.Float:
					if (!(this.Value is int))
					{
						this.SetToken(JsonToken.Integer, Convert.ToInt32(this.Value, CultureInfo.InvariantCulture), false);
					}
					return new int?((int)this.Value);
				case JsonToken.String:
				{
					string text = (string)this.Value;
					return this.ReadInt32String(text);
				}
				case JsonToken.Null:
				case JsonToken.EndArray:
					goto IL_003A;
				}
				throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
			}
			IL_003A:
			return null;
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x0003A908 File Offset: 0x00038B08
		internal int? ReadInt32String(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			int num;
			if (int.TryParse(s, NumberStyles.Integer, this.Culture, out num))
			{
				this.SetToken(JsonToken.Integer, num, false);
				return new int?(num);
			}
			this.SetToken(JsonToken.String, s, false);
			throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x0003A984 File Offset: 0x00038B84
		public virtual string ReadAsString()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken <= JsonToken.String)
			{
				if (contentToken != JsonToken.None)
				{
					if (contentToken != JsonToken.String)
					{
						goto IL_0040;
					}
					return (string)this.Value;
				}
			}
			else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
			{
				goto IL_0040;
			}
			return null;
			IL_0040:
			if (JsonTokenUtils.IsPrimitiveToken(contentToken) && this.Value != null)
			{
				IFormattable formattable = this.Value as IFormattable;
				string text;
				if (formattable != null)
				{
					text = formattable.ToString(null, this.Culture);
				}
				else
				{
					Uri uri = this.Value as Uri;
					text = ((uri != null) ? uri.OriginalString : this.Value.ToString());
				}
				this.SetToken(JsonToken.String, text, false);
				return text;
			}
			throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0003AA68 File Offset: 0x00038C68
		public virtual byte[] ReadAsBytes()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken <= JsonToken.String)
			{
				switch (contentToken)
				{
				case JsonToken.None:
					break;
				case JsonToken.StartObject:
				{
					this.ReadIntoWrappedTypeObject();
					byte[] array = this.ReadAsBytes();
					this.ReaderReadAndAssert();
					if (this.TokenType != JsonToken.EndObject)
					{
						throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
					}
					this.SetToken(JsonToken.Bytes, array, false);
					return array;
				}
				case JsonToken.StartArray:
					return this.ReadArrayIntoByteArray();
				default:
				{
					if (contentToken != JsonToken.String)
					{
						goto IL_013A;
					}
					string text = (string)this.Value;
					byte[] array2;
					Guid guid;
					if (text.Length == 0)
					{
						array2 = CollectionUtils.ArrayEmpty<byte>();
					}
					else if (ConvertUtils.TryConvertGuid(text, out guid))
					{
						array2 = guid.ToByteArray();
					}
					else
					{
						array2 = Convert.FromBase64String(text);
					}
					this.SetToken(JsonToken.Bytes, array2, false);
					return array2;
				}
				}
			}
			else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
			{
				if (contentToken != JsonToken.Bytes)
				{
					goto IL_013A;
				}
				if (this.ValueType == typeof(Guid))
				{
					byte[] array3 = ((Guid)this.Value).ToByteArray();
					this.SetToken(JsonToken.Bytes, array3, false);
					return array3;
				}
				return (byte[])this.Value;
			}
			return null;
			IL_013A:
			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x0003ABD0 File Offset: 0x00038DD0
		internal byte[] ReadArrayIntoByteArray()
		{
			List<byte> list = new List<byte>();
			do
			{
				if (!this.Read())
				{
					this.SetToken(JsonToken.None);
				}
			}
			while (!this.ReadArrayElementIntoByteArrayReportDone(list));
			byte[] array = list.ToArray();
			this.SetToken(JsonToken.Bytes, array, false);
			return array;
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0003AC14 File Offset: 0x00038E14
		private bool ReadArrayElementIntoByteArrayReportDone(List<byte> buffer)
		{
			JsonToken tokenType = this.TokenType;
			if (tokenType <= JsonToken.Comment)
			{
				if (tokenType == JsonToken.None)
				{
					throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
				}
				if (tokenType == JsonToken.Comment)
				{
					return false;
				}
			}
			else
			{
				if (tokenType == JsonToken.Integer)
				{
					buffer.Add(Convert.ToByte(this.Value, CultureInfo.InvariantCulture));
					return false;
				}
				if (tokenType == JsonToken.EndArray)
				{
					return true;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x0003ACA4 File Offset: 0x00038EA4
		public virtual double? ReadAsDouble()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
				case JsonToken.Integer:
				case JsonToken.Float:
					if (!(this.Value is double))
					{
						double num;
						if (this.Value is BigInteger)
						{
							num = (double)((BigInteger)this.Value);
						}
						else
						{
							num = Convert.ToDouble(this.Value, CultureInfo.InvariantCulture);
						}
						this.SetToken(JsonToken.Float, num, false);
					}
					return new double?((double)this.Value);
				case JsonToken.String:
					return this.ReadDoubleString((string)this.Value);
				case JsonToken.Null:
				case JsonToken.EndArray:
					goto IL_003A;
				}
				throw JsonReaderException.Create(this, "Error reading double. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
			}
			IL_003A:
			return null;
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0003AD90 File Offset: 0x00038F90
		internal double? ReadDoubleString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			double num;
			if (double.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, this.Culture, out num))
			{
				this.SetToken(JsonToken.Float, num, false);
				return new double?(num);
			}
			this.SetToken(JsonToken.String, s, false);
			throw JsonReaderException.Create(this, "Could not convert string to double: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x0003AE10 File Offset: 0x00039010
		public virtual bool? ReadAsBoolean()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
				case JsonToken.Integer:
				case JsonToken.Float:
				{
					bool flag;
					if (this.Value is BigInteger)
					{
						flag = (BigInteger)this.Value != 0L;
					}
					else
					{
						flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
					}
					this.SetToken(JsonToken.Boolean, flag, false);
					return new bool?(flag);
				}
				case JsonToken.String:
					return this.ReadBooleanString((string)this.Value);
				case JsonToken.Boolean:
					return new bool?((bool)this.Value);
				case JsonToken.Null:
				case JsonToken.EndArray:
					goto IL_003A;
				}
				throw JsonReaderException.Create(this, "Error reading boolean. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
			}
			IL_003A:
			return null;
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x0003AEF4 File Offset: 0x000390F4
		internal bool? ReadBooleanString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			bool flag;
			if (bool.TryParse(s, out flag))
			{
				this.SetToken(JsonToken.Boolean, flag, false);
				return new bool?(flag);
			}
			this.SetToken(JsonToken.String, s, false);
			throw JsonReaderException.Create(this, "Could not convert string to boolean: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x0003AF68 File Offset: 0x00039168
		public virtual decimal? ReadAsDecimal()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
				case JsonToken.Integer:
				case JsonToken.Float:
					if (!(this.Value is decimal))
					{
						decimal num;
						if (this.Value is BigInteger)
						{
							num = (decimal)((BigInteger)this.Value);
						}
						else
						{
							num = Convert.ToDecimal(this.Value, CultureInfo.InvariantCulture);
						}
						this.SetToken(JsonToken.Float, num, false);
					}
					return new decimal?((decimal)this.Value);
				case JsonToken.String:
					return this.ReadDecimalString((string)this.Value);
				case JsonToken.Null:
				case JsonToken.EndArray:
					goto IL_003A;
				}
				throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
			}
			IL_003A:
			return null;
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x0003B050 File Offset: 0x00039250
		internal decimal? ReadDecimalString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			decimal num;
			if (decimal.TryParse(s, NumberStyles.Number, this.Culture, out num))
			{
				this.SetToken(JsonToken.Float, num, false);
				return new decimal?(num);
			}
			this.SetToken(JsonToken.String, s, false);
			throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x0003B0CC File Offset: 0x000392CC
		public virtual DateTime? ReadAsDateTime()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken <= JsonToken.String)
			{
				if (contentToken != JsonToken.None)
				{
					if (contentToken != JsonToken.String)
					{
						goto IL_009F;
					}
					string text = (string)this.Value;
					return this.ReadDateTimeString(text);
				}
			}
			else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
			{
				if (contentToken != JsonToken.Date)
				{
					goto IL_009F;
				}
				if (this.Value is DateTimeOffset)
				{
					this.SetToken(JsonToken.Date, ((DateTimeOffset)this.Value).DateTime, false);
				}
				return new DateTime?((DateTime)this.Value);
			}
			return null;
			IL_009F:
			throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x0003B19C File Offset: 0x0003939C
		internal DateTime? ReadDateTimeString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			DateTime dateTime;
			if (DateTimeUtils.TryParseDateTime(s, this.DateTimeZoneHandling, this._dateFormatString, this.Culture, out dateTime))
			{
				dateTime = DateTimeUtils.EnsureDateTime(dateTime, this.DateTimeZoneHandling);
				this.SetToken(JsonToken.Date, dateTime, false);
				return new DateTime?(dateTime);
			}
			if (DateTime.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out dateTime))
			{
				dateTime = DateTimeUtils.EnsureDateTime(dateTime, this.DateTimeZoneHandling);
				this.SetToken(JsonToken.Date, dateTime, false);
				return new DateTime?(dateTime);
			}
			throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x0003B260 File Offset: 0x00039460
		public virtual DateTimeOffset? ReadAsDateTimeOffset()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken <= JsonToken.String)
			{
				if (contentToken != JsonToken.None)
				{
					if (contentToken != JsonToken.String)
					{
						goto IL_009C;
					}
					string text = (string)this.Value;
					return this.ReadDateTimeOffsetString(text);
				}
			}
			else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
			{
				if (contentToken != JsonToken.Date)
				{
					goto IL_009C;
				}
				if (this.Value is DateTime)
				{
					this.SetToken(JsonToken.Date, new DateTimeOffset((DateTime)this.Value), false);
				}
				return new DateTimeOffset?((DateTimeOffset)this.Value);
			}
			return null;
			IL_009C:
			throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x0003B328 File Offset: 0x00039528
		internal DateTimeOffset? ReadDateTimeOffsetString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			DateTimeOffset dateTimeOffset;
			if (DateTimeUtils.TryParseDateTimeOffset(s, this._dateFormatString, this.Culture, out dateTimeOffset))
			{
				this.SetToken(JsonToken.Date, dateTimeOffset, false);
				return new DateTimeOffset?(dateTimeOffset);
			}
			if (DateTimeOffset.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out dateTimeOffset))
			{
				this.SetToken(JsonToken.Date, dateTimeOffset, false);
				return new DateTimeOffset?(dateTimeOffset);
			}
			this.SetToken(JsonToken.String, s, false);
			throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x0003B3D8 File Offset: 0x000395D8
		internal void ReaderReadAndAssert()
		{
			if (!this.Read())
			{
				throw this.CreateUnexpectedEndException();
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x0003B3EC File Offset: 0x000395EC
		internal JsonReaderException CreateUnexpectedEndException()
		{
			return JsonReaderException.Create(this, "Unexpected end when reading JSON.");
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x0003B3FC File Offset: 0x000395FC
		internal void ReadIntoWrappedTypeObject()
		{
			this.ReaderReadAndAssert();
			if (this.Value != null && this.Value.ToString() == "$type")
			{
				this.ReaderReadAndAssert();
				if (this.Value != null && this.Value.ToString().StartsWith("System.Byte[]", StringComparison.Ordinal))
				{
					this.ReaderReadAndAssert();
					if (this.Value.ToString() == "$value")
					{
						return;
					}
				}
			}
			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x0003B4A0 File Offset: 0x000396A0
		public void Skip()
		{
			if (this.TokenType == JsonToken.PropertyName)
			{
				this.Read();
			}
			if (JsonTokenUtils.IsStartToken(this.TokenType))
			{
				int depth = this.Depth;
				while (this.Read() && depth < this.Depth)
				{
				}
			}
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x0003B4F0 File Offset: 0x000396F0
		protected void SetToken(JsonToken newToken)
		{
			this.SetToken(newToken, null, true);
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x0003B4FC File Offset: 0x000396FC
		protected void SetToken(JsonToken newToken, object value)
		{
			this.SetToken(newToken, value, true);
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x0003B508 File Offset: 0x00039708
		protected void SetToken(JsonToken newToken, object value, bool updateIndex)
		{
			this._tokenType = newToken;
			this._value = value;
			switch (newToken)
			{
			case JsonToken.StartObject:
				this._currentState = JsonReader.State.ObjectStart;
				this.Push(JsonContainerType.Object);
				return;
			case JsonToken.StartArray:
				this._currentState = JsonReader.State.ArrayStart;
				this.Push(JsonContainerType.Array);
				return;
			case JsonToken.StartConstructor:
				this._currentState = JsonReader.State.ConstructorStart;
				this.Push(JsonContainerType.Constructor);
				return;
			case JsonToken.PropertyName:
				this._currentState = JsonReader.State.Property;
				this._currentPosition.PropertyName = (string)value;
				return;
			case JsonToken.Comment:
				break;
			case JsonToken.Raw:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				this.SetPostValueState(updateIndex);
				break;
			case JsonToken.EndObject:
				this.ValidateEnd(JsonToken.EndObject);
				return;
			case JsonToken.EndArray:
				this.ValidateEnd(JsonToken.EndArray);
				return;
			case JsonToken.EndConstructor:
				this.ValidateEnd(JsonToken.EndConstructor);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x0003B5E0 File Offset: 0x000397E0
		internal void SetPostValueState(bool updateIndex)
		{
			if (this.Peek() != JsonContainerType.None)
			{
				this._currentState = JsonReader.State.PostValue;
			}
			else
			{
				this.SetFinished();
			}
			if (updateIndex)
			{
				this.UpdateScopeWithFinishedValue();
			}
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x0003B60C File Offset: 0x0003980C
		private void UpdateScopeWithFinishedValue()
		{
			if (this._currentPosition.HasIndex)
			{
				this._currentPosition.Position = this._currentPosition.Position + 1;
			}
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x0003B630 File Offset: 0x00039830
		private void ValidateEnd(JsonToken endToken)
		{
			JsonContainerType jsonContainerType = this.Pop();
			if (this.GetTypeForCloseToken(endToken) != jsonContainerType)
			{
				throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, jsonContainerType));
			}
			if (this.Peek() != JsonContainerType.None)
			{
				this._currentState = JsonReader.State.PostValue;
				return;
			}
			this.SetFinished();
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x0003B690 File Offset: 0x00039890
		protected void SetStateBasedOnCurrent()
		{
			JsonContainerType jsonContainerType = this.Peek();
			switch (jsonContainerType)
			{
			case JsonContainerType.None:
				this.SetFinished();
				return;
			case JsonContainerType.Object:
				this._currentState = JsonReader.State.Object;
				return;
			case JsonContainerType.Array:
				this._currentState = JsonReader.State.Array;
				return;
			case JsonContainerType.Constructor:
				this._currentState = JsonReader.State.Constructor;
				return;
			default:
				throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContainerType));
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x0003B700 File Offset: 0x00039900
		private void SetFinished()
		{
			if (this.SupportMultipleContent)
			{
				this._currentState = JsonReader.State.Start;
				return;
			}
			this._currentState = JsonReader.State.Finished;
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x0003B720 File Offset: 0x00039920
		private JsonContainerType GetTypeForCloseToken(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				return JsonContainerType.Object;
			case JsonToken.EndArray:
				return JsonContainerType.Array;
			case JsonToken.EndConstructor:
				return JsonContainerType.Constructor;
			default:
				throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
			}
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x0003B760 File Offset: 0x00039960
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x0003B770 File Offset: 0x00039970
		protected virtual void Dispose(bool disposing)
		{
			if (this._currentState != JsonReader.State.Closed && disposing)
			{
				this.Close();
			}
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0003B78C File Offset: 0x0003998C
		public virtual void Close()
		{
			this._currentState = JsonReader.State.Closed;
			this._tokenType = JsonToken.None;
			this._value = null;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0003B7A4 File Offset: 0x000399A4
		internal void ReadAndAssert()
		{
			if (!this.Read())
			{
				throw JsonSerializationException.Create(this, "Unexpected end when reading JSON.");
			}
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x0003B7C0 File Offset: 0x000399C0
		internal void ReadForTypeAndAssert(JsonContract contract, bool hasConverter)
		{
			if (!this.ReadForType(contract, hasConverter))
			{
				throw JsonSerializationException.Create(this, "Unexpected end when reading JSON.");
			}
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x0003B7DC File Offset: 0x000399DC
		internal bool ReadForType(JsonContract contract, bool hasConverter)
		{
			if (hasConverter)
			{
				return this.Read();
			}
			switch ((contract != null) ? contract.InternalReadType : ReadType.Read)
			{
			case ReadType.Read:
				return this.ReadAndMoveToContent();
			case ReadType.ReadAsInt32:
				this.ReadAsInt32();
				break;
			case ReadType.ReadAsBytes:
				this.ReadAsBytes();
				break;
			case ReadType.ReadAsString:
				this.ReadAsString();
				break;
			case ReadType.ReadAsDecimal:
				this.ReadAsDecimal();
				break;
			case ReadType.ReadAsDateTime:
				this.ReadAsDateTime();
				break;
			case ReadType.ReadAsDateTimeOffset:
				this.ReadAsDateTimeOffset();
				break;
			case ReadType.ReadAsDouble:
				this.ReadAsDouble();
				break;
			case ReadType.ReadAsBoolean:
				this.ReadAsBoolean();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return this.TokenType > JsonToken.None;
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0003B8B4 File Offset: 0x00039AB4
		internal bool ReadAndMoveToContent()
		{
			return this.Read() && this.MoveToContent();
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0003B8CC File Offset: 0x00039ACC
		internal bool MoveToContent()
		{
			JsonToken jsonToken = this.TokenType;
			while (jsonToken == JsonToken.None || jsonToken == JsonToken.Comment)
			{
				if (!this.Read())
				{
					return false;
				}
				jsonToken = this.TokenType;
			}
			return true;
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x0003B908 File Offset: 0x00039B08
		private JsonToken GetContentToken()
		{
			while (this.Read())
			{
				JsonToken tokenType = this.TokenType;
				if (tokenType != JsonToken.Comment)
				{
					return tokenType;
				}
			}
			this.SetToken(JsonToken.None);
			return JsonToken.None;
		}

		// Token: 0x0400038F RID: 911
		private JsonToken _tokenType;

		// Token: 0x04000390 RID: 912
		private object _value;

		// Token: 0x04000391 RID: 913
		internal char _quoteChar;

		// Token: 0x04000392 RID: 914
		internal JsonReader.State _currentState;

		// Token: 0x04000393 RID: 915
		private JsonPosition _currentPosition;

		// Token: 0x04000394 RID: 916
		private CultureInfo _culture;

		// Token: 0x04000395 RID: 917
		private DateTimeZoneHandling _dateTimeZoneHandling;

		// Token: 0x04000396 RID: 918
		private int? _maxDepth;

		// Token: 0x04000397 RID: 919
		private bool _hasExceededMaxDepth;

		// Token: 0x04000398 RID: 920
		internal DateParseHandling _dateParseHandling;

		// Token: 0x04000399 RID: 921
		internal FloatParseHandling _floatParseHandling;

		// Token: 0x0400039A RID: 922
		private string _dateFormatString;

		// Token: 0x0400039B RID: 923
		private List<JsonPosition> _stack;

		// Token: 0x020001EB RID: 491
		protected internal enum State
		{
			// Token: 0x04000892 RID: 2194
			Start,
			// Token: 0x04000893 RID: 2195
			Complete,
			// Token: 0x04000894 RID: 2196
			Property,
			// Token: 0x04000895 RID: 2197
			ObjectStart,
			// Token: 0x04000896 RID: 2198
			Object,
			// Token: 0x04000897 RID: 2199
			ArrayStart,
			// Token: 0x04000898 RID: 2200
			Array,
			// Token: 0x04000899 RID: 2201
			Closed,
			// Token: 0x0400089A RID: 2202
			PostValue,
			// Token: 0x0400089B RID: 2203
			ConstructorStart,
			// Token: 0x0400089C RID: 2204
			Constructor,
			// Token: 0x0400089D RID: 2205
			Error,
			// Token: 0x0400089E RID: 2206
			Finished
		}
	}
}
