using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000B7 RID: 183
	public class JsonTextWriter : JsonWriter
	{
		// Token: 0x060009A2 RID: 2466 RVA: 0x000419D0 File Offset: 0x0003FBD0
		public override Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.FlushAsync(cancellationToken);
			}
			return this.DoFlushAsync(cancellationToken);
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x000419EC File Offset: 0x0003FBEC
		internal Task DoFlushAsync(CancellationToken cancellationToken)
		{
			return cancellationToken.CancelIfRequestedAsync() ?? this._writer.FlushAsync();
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00041A08 File Offset: 0x0003FC08
		protected override Task WriteValueDelimiterAsync(CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteValueDelimiterAsync(cancellationToken);
			}
			return this.DoWriteValueDelimiterAsync(cancellationToken);
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00041A24 File Offset: 0x0003FC24
		internal Task DoWriteValueDelimiterAsync(CancellationToken cancellationToken)
		{
			return this._writer.WriteAsync(',', cancellationToken);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00041A34 File Offset: 0x0003FC34
		protected override Task WriteEndAsync(JsonToken token, CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteEndAsync(token, cancellationToken);
			}
			return this.DoWriteEndAsync(token, cancellationToken);
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00041A54 File Offset: 0x0003FC54
		internal Task DoWriteEndAsync(JsonToken token, CancellationToken cancellationToken)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				return this._writer.WriteAsync('}', cancellationToken);
			case JsonToken.EndArray:
				return this._writer.WriteAsync(']', cancellationToken);
			case JsonToken.EndConstructor:
				return this._writer.WriteAsync(')', cancellationToken);
			default:
				throw JsonWriterException.Create(this, "Invalid JsonToken: " + token, null);
			}
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00041AC4 File Offset: 0x0003FCC4
		public override Task CloseAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.CloseAsync(cancellationToken);
			}
			return this.DoCloseAsync(cancellationToken);
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00041AE0 File Offset: 0x0003FCE0
		internal async Task DoCloseAsync(CancellationToken cancellationToken)
		{
			if (base.Top == 0)
			{
				cancellationToken.ThrowIfCancellationRequested();
			}
			while (base.Top > 0)
			{
				await this.WriteEndAsync(cancellationToken).ConfigureAwait(false);
			}
			this.CloseBufferAndWriter();
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00041B34 File Offset: 0x0003FD34
		public override Task WriteEndAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndAsync(cancellationToken);
			}
			return base.WriteEndInternalAsync(cancellationToken);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00041B50 File Offset: 0x0003FD50
		protected override Task WriteIndentAsync(CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteIndentAsync(cancellationToken);
			}
			return this.DoWriteIndentAsync(cancellationToken);
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00041B6C File Offset: 0x0003FD6C
		internal Task DoWriteIndentAsync(CancellationToken cancellationToken)
		{
			int num = base.Top * this._indentation;
			int num2 = this.SetIndentChars();
			if (num <= 12)
			{
				return this._writer.WriteAsync(this._indentChars, 0, num2 + num, cancellationToken);
			}
			return this.WriteIndentAsync(num, num2, cancellationToken);
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x00041BBC File Offset: 0x0003FDBC
		private async Task WriteIndentAsync(int currentIndentCount, int newLineLen, CancellationToken cancellationToken)
		{
			await this._writer.WriteAsync(this._indentChars, 0, newLineLen + Math.Min(currentIndentCount, 12), cancellationToken).ConfigureAwait(false);
			while ((currentIndentCount -= 12) > 0)
			{
				await this._writer.WriteAsync(this._indentChars, newLineLen, Math.Min(currentIndentCount, 12), cancellationToken).ConfigureAwait(false);
			}
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x00041C20 File Offset: 0x0003FE20
		private Task WriteValueInternalAsync(JsonToken token, string value, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(token, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this._writer.WriteAsync(value, cancellationToken);
			}
			return this.WriteValueInternalAsync(task, value, cancellationToken);
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x00041C60 File Offset: 0x0003FE60
		private async Task WriteValueInternalAsync(Task task, string value, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this._writer.WriteAsync(value, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00041CC4 File Offset: 0x0003FEC4
		protected override Task WriteIndentSpaceAsync(CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteIndentSpaceAsync(cancellationToken);
			}
			return this.DoWriteIndentSpaceAsync(cancellationToken);
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00041CE0 File Offset: 0x0003FEE0
		internal Task DoWriteIndentSpaceAsync(CancellationToken cancellationToken)
		{
			return this._writer.WriteAsync(' ', cancellationToken);
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x00041CF0 File Offset: 0x0003FEF0
		public override Task WriteRawAsync(string json, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteRawAsync(json, cancellationToken);
			}
			return this.DoWriteRawAsync(json, cancellationToken);
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x00041D10 File Offset: 0x0003FF10
		internal Task DoWriteRawAsync(string json, CancellationToken cancellationToken)
		{
			return this._writer.WriteAsync(json, cancellationToken);
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x00041D20 File Offset: 0x0003FF20
		public override Task WriteNullAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteNullAsync(cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x00041D3C File Offset: 0x0003FF3C
		internal Task DoWriteNullAsync(CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Null, JsonConvert.Null, cancellationToken);
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00041D4C File Offset: 0x0003FF4C
		private Task WriteDigitsAsync(ulong uvalue, bool negative, CancellationToken cancellationToken)
		{
			if ((uvalue <= 9UL) & !negative)
			{
				return this._writer.WriteAsync((char)(48UL + uvalue), cancellationToken);
			}
			int num = this.WriteNumberToBuffer(uvalue, negative);
			return this._writer.WriteAsync(this._writeBuffer, 0, num, cancellationToken);
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00041DA0 File Offset: 0x0003FFA0
		private Task WriteIntegerValueAsync(ulong uvalue, bool negative, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.Integer, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this.WriteDigitsAsync(uvalue, negative, cancellationToken);
			}
			return this.WriteIntegerValueAsync(task, uvalue, negative, cancellationToken);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00041DDC File Offset: 0x0003FFDC
		private async Task WriteIntegerValueAsync(Task task, ulong uvalue, bool negative, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this.WriteDigitsAsync(uvalue, negative, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00041E48 File Offset: 0x00040048
		internal Task WriteIntegerValueAsync(long value, CancellationToken cancellationToken)
		{
			bool flag = value < 0L;
			if (flag)
			{
				value = -value;
			}
			return this.WriteIntegerValueAsync((ulong)value, flag, cancellationToken);
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x00041E74 File Offset: 0x00040074
		internal Task WriteIntegerValueAsync(ulong uvalue, CancellationToken cancellationToken)
		{
			return this.WriteIntegerValueAsync(uvalue, false, cancellationToken);
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x00041E80 File Offset: 0x00040080
		private Task WriteEscapedStringAsync(string value, bool quote, CancellationToken cancellationToken)
		{
			return JavaScriptUtils.WriteEscapedJavaScriptStringAsync(this._writer, value, this._quoteChar, quote, this._charEscapeFlags, base.StringEscapeHandling, this, this._writeBuffer, cancellationToken);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x00041EB8 File Offset: 0x000400B8
		public override Task WritePropertyNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WritePropertyNameAsync(name, cancellationToken);
			}
			return this.DoWritePropertyNameAsync(name, cancellationToken);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00041ED8 File Offset: 0x000400D8
		internal Task DoWritePropertyNameAsync(string name, CancellationToken cancellationToken)
		{
			Task task = base.InternalWritePropertyNameAsync(name, cancellationToken);
			if (task.Status != TaskStatus.RanToCompletion)
			{
				return this.DoWritePropertyNameAsync(task, name, cancellationToken);
			}
			task = this.WriteEscapedStringAsync(name, this._quoteName, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this._writer.WriteAsync(':', cancellationToken);
			}
			return JavaScriptUtils.WriteCharAsync(task, this._writer, ':', cancellationToken);
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x00041F44 File Offset: 0x00040144
		private async Task DoWritePropertyNameAsync(Task task, string name, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this.WriteEscapedStringAsync(name, this._quoteName, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(':').ConfigureAwait(false);
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00041FA8 File Offset: 0x000401A8
		public override Task WritePropertyNameAsync(string name, bool escape, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WritePropertyNameAsync(name, escape, cancellationToken);
			}
			return this.DoWritePropertyNameAsync(name, escape, cancellationToken);
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x00041FC8 File Offset: 0x000401C8
		internal async Task DoWritePropertyNameAsync(string name, bool escape, CancellationToken cancellationToken)
		{
			await base.InternalWritePropertyNameAsync(name, cancellationToken).ConfigureAwait(false);
			if (escape)
			{
				await this.WriteEscapedStringAsync(name, this._quoteName, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				if (this._quoteName)
				{
					await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
				}
				await this._writer.WriteAsync(name, cancellationToken).ConfigureAwait(false);
				if (this._quoteName)
				{
					await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
				}
			}
			await this._writer.WriteAsync(':').ConfigureAwait(false);
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x0004202C File Offset: 0x0004022C
		public override Task WriteStartArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteStartArrayAsync(cancellationToken);
			}
			return this.DoWriteStartArrayAsync(cancellationToken);
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x00042048 File Offset: 0x00040248
		internal Task DoWriteStartArrayAsync(CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteStartAsync(JsonToken.StartArray, JsonContainerType.Array, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this._writer.WriteAsync('[', cancellationToken);
			}
			return this.DoWriteStartArrayAsync(task, cancellationToken);
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00042088 File Offset: 0x00040288
		internal async Task DoWriteStartArrayAsync(Task task, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this._writer.WriteAsync('[').ConfigureAwait(false);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x000420DC File Offset: 0x000402DC
		public override Task WriteStartObjectAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteStartObjectAsync(cancellationToken);
			}
			return this.DoWriteStartObjectAsync(cancellationToken);
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x000420F8 File Offset: 0x000402F8
		internal Task DoWriteStartObjectAsync(CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteStartAsync(JsonToken.StartObject, JsonContainerType.Object, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this._writer.WriteAsync('{', cancellationToken);
			}
			return this.DoWriteStartObjectAsync(task, cancellationToken);
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00042138 File Offset: 0x00040338
		internal async Task DoWriteStartObjectAsync(Task task, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this._writer.WriteAsync('{').ConfigureAwait(false);
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0004218C File Offset: 0x0004038C
		public override Task WriteStartConstructorAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteStartConstructorAsync(name, cancellationToken);
			}
			return this.DoWriteStartConstructorAsync(name, cancellationToken);
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x000421AC File Offset: 0x000403AC
		internal async Task DoWriteStartConstructorAsync(string name, CancellationToken cancellationToken)
		{
			await base.InternalWriteStartAsync(JsonToken.StartConstructor, JsonContainerType.Constructor, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync("new ", cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(name, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync('(').ConfigureAwait(false);
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x00042208 File Offset: 0x00040408
		public override Task WriteUndefinedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteUndefinedAsync(cancellationToken);
			}
			return this.DoWriteUndefinedAsync(cancellationToken);
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00042224 File Offset: 0x00040424
		internal Task DoWriteUndefinedAsync(CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.Undefined, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this._writer.WriteAsync(JsonConvert.Undefined, cancellationToken);
			}
			return this.DoWriteUndefinedAsync(task, cancellationToken);
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x00042268 File Offset: 0x00040468
		private async Task DoWriteUndefinedAsync(Task task, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this._writer.WriteAsync(JsonConvert.Undefined, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x000422C4 File Offset: 0x000404C4
		public override Task WriteWhitespaceAsync(string ws, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteWhitespaceAsync(ws, cancellationToken);
			}
			return this.DoWriteWhitespaceAsync(ws, cancellationToken);
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x000422E4 File Offset: 0x000404E4
		internal Task DoWriteWhitespaceAsync(string ws, CancellationToken cancellationToken)
		{
			base.InternalWriteWhitespace(ws);
			return this._writer.WriteAsync(ws, cancellationToken);
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x000422FC File Offset: 0x000404FC
		public override Task WriteValueAsync(bool value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x0004231C File Offset: 0x0004051C
		internal Task DoWriteValueAsync(bool value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Boolean, JsonConvert.ToString(value), cancellationToken);
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x00042330 File Offset: 0x00040530
		public override Task WriteValueAsync(bool? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x00042350 File Offset: 0x00040550
		internal Task DoWriteValueAsync(bool? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x00042374 File Offset: 0x00040574
		public override Task WriteValueAsync(byte value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)((ulong)value), cancellationToken);
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x00042394 File Offset: 0x00040594
		public override Task WriteValueAsync(byte? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x000423B4 File Offset: 0x000405B4
		internal Task DoWriteValueAsync(byte? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)((ulong)value.GetValueOrDefault()), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x000423DC File Offset: 0x000405DC
		public override Task WriteValueAsync(byte[] value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value != null)
			{
				return this.WriteValueNonNullAsync(value, cancellationToken);
			}
			return this.WriteNullAsync(cancellationToken);
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00042408 File Offset: 0x00040608
		internal async Task WriteValueNonNullAsync(byte[] value, CancellationToken cancellationToken)
		{
			await base.InternalWriteValueAsync(JsonToken.Bytes, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
			await this.Base64Encoder.EncodeAsync(value, 0, value.Length, cancellationToken).ConfigureAwait(false);
			await this.Base64Encoder.FlushAsync(cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x00042464 File Offset: 0x00040664
		public override Task WriteValueAsync(char value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x00042484 File Offset: 0x00040684
		internal Task DoWriteValueAsync(char value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.String, JsonConvert.ToString(value), cancellationToken);
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x00042498 File Offset: 0x00040698
		public override Task WriteValueAsync(char? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x000424B8 File Offset: 0x000406B8
		internal Task DoWriteValueAsync(char? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x000424DC File Offset: 0x000406DC
		public override Task WriteValueAsync(DateTime value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x000424FC File Offset: 0x000406FC
		internal async Task DoWriteValueAsync(DateTime value, CancellationToken cancellationToken)
		{
			await base.InternalWriteValueAsync(JsonToken.Date, cancellationToken).ConfigureAwait(false);
			value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
			if (string.IsNullOrEmpty(base.DateFormatString))
			{
				int num = this.WriteValueToBuffer(value);
				await this._writer.WriteAsync(this._writeBuffer, 0, num, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
				await this._writer.WriteAsync(value.ToString(base.DateFormatString, base.Culture), cancellationToken).ConfigureAwait(false);
				await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
			}
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00042558 File Offset: 0x00040758
		public override Task WriteValueAsync(DateTime? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00042578 File Offset: 0x00040778
		internal Task DoWriteValueAsync(DateTime? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x0004259C File Offset: 0x0004079C
		public override Task WriteValueAsync(DateTimeOffset value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x000425BC File Offset: 0x000407BC
		internal async Task DoWriteValueAsync(DateTimeOffset value, CancellationToken cancellationToken)
		{
			await base.InternalWriteValueAsync(JsonToken.Date, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrEmpty(base.DateFormatString))
			{
				int num = this.WriteValueToBuffer(value);
				await this._writer.WriteAsync(this._writeBuffer, 0, num, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
				await this._writer.WriteAsync(value.ToString(base.DateFormatString, base.Culture), cancellationToken).ConfigureAwait(false);
				await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
			}
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00042618 File Offset: 0x00040818
		public override Task WriteValueAsync(DateTimeOffset? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00042638 File Offset: 0x00040838
		internal Task DoWriteValueAsync(DateTimeOffset? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0004265C File Offset: 0x0004085C
		public override Task WriteValueAsync(decimal value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0004267C File Offset: 0x0004087C
		internal Task DoWriteValueAsync(decimal value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Float, JsonConvert.ToString(value), cancellationToken);
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0004268C File Offset: 0x0004088C
		public override Task WriteValueAsync(decimal? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x000426AC File Offset: 0x000408AC
		internal Task DoWriteValueAsync(decimal? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x000426D0 File Offset: 0x000408D0
		public override Task WriteValueAsync(double value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteValueAsync(value, false, cancellationToken);
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x000426F0 File Offset: 0x000408F0
		internal Task WriteValueAsync(double value, bool nullable, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Float, JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, nullable), cancellationToken);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0004271C File Offset: 0x0004091C
		public override Task WriteValueAsync(double? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value == null)
			{
				return this.WriteNullAsync(cancellationToken);
			}
			return this.WriteValueAsync(value.GetValueOrDefault(), true, cancellationToken);
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00042764 File Offset: 0x00040964
		public override Task WriteValueAsync(float value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteValueAsync(value, false, cancellationToken);
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00042784 File Offset: 0x00040984
		internal Task WriteValueAsync(float value, bool nullable, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Float, JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, nullable), cancellationToken);
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x000427B0 File Offset: 0x000409B0
		public override Task WriteValueAsync(float? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value == null)
			{
				return this.WriteNullAsync(cancellationToken);
			}
			return this.WriteValueAsync(value.GetValueOrDefault(), true, cancellationToken);
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x000427F8 File Offset: 0x000409F8
		public override Task WriteValueAsync(Guid value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00042818 File Offset: 0x00040A18
		internal async Task DoWriteValueAsync(Guid value, CancellationToken cancellationToken)
		{
			await base.InternalWriteValueAsync(JsonToken.String, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
			await this._writer.WriteAsync(value.ToString("D", CultureInfo.InvariantCulture), cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(this._quoteChar).ConfigureAwait(false);
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x00042874 File Offset: 0x00040A74
		public override Task WriteValueAsync(Guid? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x00042894 File Offset: 0x00040A94
		internal Task DoWriteValueAsync(Guid? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x000428B8 File Offset: 0x00040AB8
		public override Task WriteValueAsync(int value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)value, cancellationToken);
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x000428D8 File Offset: 0x00040AD8
		public override Task WriteValueAsync(int? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x000428F8 File Offset: 0x00040AF8
		internal Task DoWriteValueAsync(int? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x00042920 File Offset: 0x00040B20
		public override Task WriteValueAsync(long value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync(value, cancellationToken);
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x00042940 File Offset: 0x00040B40
		public override Task WriteValueAsync(long? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x00042960 File Offset: 0x00040B60
		internal Task DoWriteValueAsync(long? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00042984 File Offset: 0x00040B84
		internal Task WriteValueAsync(BigInteger value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Integer, value.ToString(CultureInfo.InvariantCulture), cancellationToken);
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x0004299C File Offset: 0x00040B9C
		public override Task WriteValueAsync(object value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value == null)
			{
				return this.WriteNullAsync(cancellationToken);
			}
			if (value is BigInteger)
			{
				return this.WriteValueAsync((BigInteger)value, cancellationToken);
			}
			return JsonWriter.WriteValueAsync(this, ConvertUtils.GetTypeCode(value.GetType()), value, cancellationToken);
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x000429FC File Offset: 0x00040BFC
		[CLSCompliant(false)]
		public override Task WriteValueAsync(sbyte value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)value, cancellationToken);
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00042A1C File Offset: 0x00040C1C
		[CLSCompliant(false)]
		public override Task WriteValueAsync(sbyte? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00042A3C File Offset: 0x00040C3C
		internal Task DoWriteValueAsync(sbyte? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00042A64 File Offset: 0x00040C64
		public override Task WriteValueAsync(short value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)value, cancellationToken);
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00042A84 File Offset: 0x00040C84
		public override Task WriteValueAsync(short? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00042AA4 File Offset: 0x00040CA4
		internal Task DoWriteValueAsync(short? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00042ACC File Offset: 0x00040CCC
		public override Task WriteValueAsync(string value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00042AEC File Offset: 0x00040CEC
		internal Task DoWriteValueAsync(string value, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.String, cancellationToken);
			if (task.Status != TaskStatus.RanToCompletion)
			{
				return this.DoWriteValueAsync(task, value, cancellationToken);
			}
			if (value != null)
			{
				return this.WriteEscapedStringAsync(value, true, cancellationToken);
			}
			return this._writer.WriteAsync(JsonConvert.Null, cancellationToken);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00042B40 File Offset: 0x00040D40
		private async Task DoWriteValueAsync(Task task, string value, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await ((value == null) ? this._writer.WriteAsync(JsonConvert.Null, cancellationToken) : this.WriteEscapedStringAsync(value, true, cancellationToken)).ConfigureAwait(false);
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00042BA4 File Offset: 0x00040DA4
		public override Task WriteValueAsync(TimeSpan value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00042BC4 File Offset: 0x00040DC4
		internal async Task DoWriteValueAsync(TimeSpan value, CancellationToken cancellationToken)
		{
			await base.InternalWriteValueAsync(JsonToken.String, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(this._quoteChar, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(value.ToString(null, CultureInfo.InvariantCulture), cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(this._quoteChar, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00042C20 File Offset: 0x00040E20
		public override Task WriteValueAsync(TimeSpan? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00042C40 File Offset: 0x00040E40
		internal Task DoWriteValueAsync(TimeSpan? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00042C64 File Offset: 0x00040E64
		[CLSCompliant(false)]
		public override Task WriteValueAsync(uint value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)((ulong)value), cancellationToken);
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00042C84 File Offset: 0x00040E84
		[CLSCompliant(false)]
		public override Task WriteValueAsync(uint? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00042CA4 File Offset: 0x00040EA4
		internal Task DoWriteValueAsync(uint? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)((ulong)value.GetValueOrDefault()), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00042CCC File Offset: 0x00040ECC
		[CLSCompliant(false)]
		public override Task WriteValueAsync(ulong value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync(value, cancellationToken);
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x00042CEC File Offset: 0x00040EEC
		[CLSCompliant(false)]
		public override Task WriteValueAsync(ulong? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00042D0C File Offset: 0x00040F0C
		internal Task DoWriteValueAsync(ulong? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00042D30 File Offset: 0x00040F30
		public override Task WriteValueAsync(Uri value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (!(value == null))
			{
				return this.WriteValueNotNullAsync(value, cancellationToken);
			}
			return this.WriteNullAsync(cancellationToken);
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00042D64 File Offset: 0x00040F64
		internal Task WriteValueNotNullAsync(Uri value, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.String, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this.WriteEscapedStringAsync(value.OriginalString, true, cancellationToken);
			}
			return this.WriteValueNotNullAsync(task, value, cancellationToken);
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00042DA4 File Offset: 0x00040FA4
		internal async Task WriteValueNotNullAsync(Task task, Uri value, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this.WriteEscapedStringAsync(value.OriginalString, true, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00042E08 File Offset: 0x00041008
		[CLSCompliant(false)]
		public override Task WriteValueAsync(ushort value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)((ulong)value), cancellationToken);
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00042E28 File Offset: 0x00041028
		[CLSCompliant(false)]
		public override Task WriteValueAsync(ushort? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x00042E48 File Offset: 0x00041048
		internal Task DoWriteValueAsync(ushort? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)((ulong)value.GetValueOrDefault()), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x00042E70 File Offset: 0x00041070
		public override Task WriteCommentAsync(string text, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteCommentAsync(text, cancellationToken);
			}
			return this.DoWriteCommentAsync(text, cancellationToken);
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00042E90 File Offset: 0x00041090
		internal async Task DoWriteCommentAsync(string text, CancellationToken cancellationToken)
		{
			await base.InternalWriteCommentAsync(cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync("/*", cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync(text, cancellationToken).ConfigureAwait(false);
			await this._writer.WriteAsync("*/", cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00042EEC File Offset: 0x000410EC
		public override Task WriteEndArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndArrayAsync(cancellationToken);
			}
			return base.InternalWriteEndAsync(JsonContainerType.Array, cancellationToken);
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00042F0C File Offset: 0x0004110C
		public override Task WriteEndConstructorAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndConstructorAsync(cancellationToken);
			}
			return base.InternalWriteEndAsync(JsonContainerType.Constructor, cancellationToken);
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00042F2C File Offset: 0x0004112C
		public override Task WriteEndObjectAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndObjectAsync(cancellationToken);
			}
			return base.InternalWriteEndAsync(JsonContainerType.Object, cancellationToken);
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00042F4C File Offset: 0x0004114C
		public override Task WriteRawValueAsync(string json, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteRawValueAsync(json, cancellationToken);
			}
			return this.DoWriteRawValueAsync(json, cancellationToken);
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00042F6C File Offset: 0x0004116C
		internal Task DoWriteRawValueAsync(string json, CancellationToken cancellationToken)
		{
			base.UpdateScopeWithFinishedValue();
			Task task = base.AutoCompleteAsync(JsonToken.Undefined, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this.WriteRawAsync(json, cancellationToken);
			}
			return this.DoWriteRawValueAsync(task, json, cancellationToken);
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00042FAC File Offset: 0x000411AC
		private async Task DoWriteRawValueAsync(Task task, string json, CancellationToken cancellationToken)
		{
			await task.ConfigureAwait(false);
			await this.WriteRawAsync(json, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x00043010 File Offset: 0x00041210
		internal char[] EnsureWriteBuffer(int length, int copyTo)
		{
			if (length < 35)
			{
				length = 35;
			}
			char[] writeBuffer = this._writeBuffer;
			if (writeBuffer == null)
			{
				return this._writeBuffer = BufferUtils.RentBuffer(this._arrayPool, length);
			}
			if (writeBuffer.Length >= length)
			{
				return writeBuffer;
			}
			char[] array = BufferUtils.RentBuffer(this._arrayPool, length);
			if (copyTo != 0)
			{
				Array.Copy(writeBuffer, array, copyTo);
			}
			BufferUtils.ReturnBuffer(this._arrayPool, writeBuffer);
			this._writeBuffer = array;
			return array;
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x0004308C File Offset: 0x0004128C
		private Base64Encoder Base64Encoder
		{
			get
			{
				if (this._base64Encoder == null)
				{
					this._base64Encoder = new Base64Encoder(this._writer);
				}
				return this._base64Encoder;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000A1C RID: 2588 RVA: 0x000430B0 File Offset: 0x000412B0
		// (set) Token: 0x06000A1D RID: 2589 RVA: 0x000430B8 File Offset: 0x000412B8
		public IArrayPool<char> ArrayPool
		{
			get
			{
				return this._arrayPool;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._arrayPool = value;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000A1E RID: 2590 RVA: 0x000430D4 File Offset: 0x000412D4
		// (set) Token: 0x06000A1F RID: 2591 RVA: 0x000430DC File Offset: 0x000412DC
		public int Indentation
		{
			get
			{
				return this._indentation;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Indentation value must be greater than 0.");
				}
				this._indentation = value;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000A20 RID: 2592 RVA: 0x000430F8 File Offset: 0x000412F8
		// (set) Token: 0x06000A21 RID: 2593 RVA: 0x00043100 File Offset: 0x00041300
		public char QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			set
			{
				if (value != '"' && value != '\'')
				{
					throw new ArgumentException("Invalid JavaScript string quote character. Valid quote characters are ' and \".");
				}
				this._quoteChar = value;
				this.UpdateCharEscapeFlags();
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000A22 RID: 2594 RVA: 0x0004312C File Offset: 0x0004132C
		// (set) Token: 0x06000A23 RID: 2595 RVA: 0x00043134 File Offset: 0x00041334
		public char IndentChar
		{
			get
			{
				return this._indentChar;
			}
			set
			{
				if (value != this._indentChar)
				{
					this._indentChar = value;
					this._indentChars = null;
				}
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000A24 RID: 2596 RVA: 0x00043150 File Offset: 0x00041350
		// (set) Token: 0x06000A25 RID: 2597 RVA: 0x00043158 File Offset: 0x00041358
		public bool QuoteName
		{
			get
			{
				return this._quoteName;
			}
			set
			{
				this._quoteName = value;
			}
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00043164 File Offset: 0x00041364
		public JsonTextWriter(TextWriter textWriter)
		{
			if (textWriter == null)
			{
				throw new ArgumentNullException("textWriter");
			}
			this._writer = textWriter;
			this._quoteChar = '"';
			this._quoteName = true;
			this._indentChar = ' ';
			this._indentation = 2;
			this.UpdateCharEscapeFlags();
			this._safeAsync = base.GetType() == typeof(JsonTextWriter);
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x000431D4 File Offset: 0x000413D4
		public override void Flush()
		{
			this._writer.Flush();
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x000431E4 File Offset: 0x000413E4
		public override void Close()
		{
			base.Close();
			this.CloseBufferAndWriter();
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x000431F4 File Offset: 0x000413F4
		private void CloseBufferAndWriter()
		{
			if (this._writeBuffer != null)
			{
				BufferUtils.ReturnBuffer(this._arrayPool, this._writeBuffer);
				this._writeBuffer = null;
			}
			if (base.CloseOutput)
			{
				TextWriter writer = this._writer;
				if (writer == null)
				{
					return;
				}
				writer.Close();
			}
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00043248 File Offset: 0x00041448
		public override void WriteStartObject()
		{
			base.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
			this._writer.Write('{');
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x00043260 File Offset: 0x00041460
		public override void WriteStartArray()
		{
			base.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
			this._writer.Write('[');
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00043278 File Offset: 0x00041478
		public override void WriteStartConstructor(string name)
		{
			base.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
			this._writer.Write("new ");
			this._writer.Write(name);
			this._writer.Write('(');
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x000432BC File Offset: 0x000414BC
		protected override void WriteEnd(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				this._writer.Write('}');
				return;
			case JsonToken.EndArray:
				this._writer.Write(']');
				return;
			case JsonToken.EndConstructor:
				this._writer.Write(')');
				return;
			default:
				throw JsonWriterException.Create(this, "Invalid JsonToken: " + token, null);
			}
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00043328 File Offset: 0x00041528
		public override void WritePropertyName(string name)
		{
			base.InternalWritePropertyName(name);
			this.WriteEscapedString(name, this._quoteName);
			this._writer.Write(':');
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0004334C File Offset: 0x0004154C
		public override void WritePropertyName(string name, bool escape)
		{
			base.InternalWritePropertyName(name);
			if (escape)
			{
				this.WriteEscapedString(name, this._quoteName);
			}
			else
			{
				if (this._quoteName)
				{
					this._writer.Write(this._quoteChar);
				}
				this._writer.Write(name);
				if (this._quoteName)
				{
					this._writer.Write(this._quoteChar);
				}
			}
			this._writer.Write(':');
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x000433D0 File Offset: 0x000415D0
		internal override void OnStringEscapeHandlingChanged()
		{
			this.UpdateCharEscapeFlags();
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x000433D8 File Offset: 0x000415D8
		private void UpdateCharEscapeFlags()
		{
			this._charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(base.StringEscapeHandling, this._quoteChar);
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x000433F4 File Offset: 0x000415F4
		protected override void WriteIndent()
		{
			int num = base.Top * this._indentation;
			int num2 = this.SetIndentChars();
			this._writer.Write(this._indentChars, 0, num2 + Math.Min(num, 12));
			while ((num -= 12) > 0)
			{
				this._writer.Write(this._indentChars, num2, Math.Min(num, 12));
			}
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00043460 File Offset: 0x00041660
		private int SetIndentChars()
		{
			string newLine = this._writer.NewLine;
			int length = newLine.Length;
			bool flag = this._indentChars != null && this._indentChars.Length == 12 + length;
			if (flag)
			{
				for (int num = 0; num != length; num++)
				{
					if (newLine[num] != this._indentChars[num])
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				this._indentChars = (newLine + new string(this._indentChar, 12)).ToCharArray();
			}
			return length;
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x000434FC File Offset: 0x000416FC
		protected override void WriteValueDelimiter()
		{
			this._writer.Write(',');
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x0004350C File Offset: 0x0004170C
		protected override void WriteIndentSpace()
		{
			this._writer.Write(' ');
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x0004351C File Offset: 0x0004171C
		private void WriteValueInternal(string value, JsonToken token)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x0004352C File Offset: 0x0004172C
		public override void WriteValue(object value)
		{
			if (value is BigInteger)
			{
				base.InternalWriteValue(JsonToken.Integer);
				this.WriteValueInternal(((BigInteger)value).ToString(CultureInfo.InvariantCulture), JsonToken.String);
				return;
			}
			base.WriteValue(value);
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x00043574 File Offset: 0x00041774
		public override void WriteNull()
		{
			base.InternalWriteValue(JsonToken.Null);
			this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0004358C File Offset: 0x0004178C
		public override void WriteUndefined()
		{
			base.InternalWriteValue(JsonToken.Undefined);
			this.WriteValueInternal(JsonConvert.Undefined, JsonToken.Undefined);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x000435A4 File Offset: 0x000417A4
		public override void WriteRaw(string json)
		{
			base.InternalWriteRaw();
			this._writer.Write(json);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x000435B8 File Offset: 0x000417B8
		public override void WriteValue(string value)
		{
			base.InternalWriteValue(JsonToken.String);
			if (value == null)
			{
				this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
				return;
			}
			this.WriteEscapedString(value, true);
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x000435E0 File Offset: 0x000417E0
		private void WriteEscapedString(string value, bool quote)
		{
			this.EnsureWriteBuffer();
			JavaScriptUtils.WriteEscapedJavaScriptString(this._writer, value, this._quoteChar, quote, this._charEscapeFlags, base.StringEscapeHandling, this._arrayPool, ref this._writeBuffer);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00043624 File Offset: 0x00041824
		public override void WriteValue(int value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((long)value);
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00043638 File Offset: 0x00041838
		[CLSCompliant(false)]
		public override void WriteValue(uint value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((long)((ulong)value));
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x0004364C File Offset: 0x0004184C
		public override void WriteValue(long value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue(value);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x0004365C File Offset: 0x0004185C
		[CLSCompliant(false)]
		public override void WriteValue(ulong value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue(value, false);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00043670 File Offset: 0x00041870
		public override void WriteValue(float value)
		{
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x000436A4 File Offset: 0x000418A4
		public override void WriteValue(float? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x000436F0 File Offset: 0x000418F0
		public override void WriteValue(double value)
		{
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x00043724 File Offset: 0x00041924
		public override void WriteValue(double? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00043770 File Offset: 0x00041970
		public override void WriteValue(bool value)
		{
			base.InternalWriteValue(JsonToken.Boolean);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Boolean);
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x00043788 File Offset: 0x00041988
		public override void WriteValue(short value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((long)value);
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x0004379C File Offset: 0x0004199C
		[CLSCompliant(false)]
		public override void WriteValue(ushort value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((long)((ulong)value));
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x000437B0 File Offset: 0x000419B0
		public override void WriteValue(char value)
		{
			base.InternalWriteValue(JsonToken.String);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.String);
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x000437C8 File Offset: 0x000419C8
		public override void WriteValue(byte value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((long)((ulong)value));
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x000437DC File Offset: 0x000419DC
		[CLSCompliant(false)]
		public override void WriteValue(sbyte value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((long)value);
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x000437F0 File Offset: 0x000419F0
		public override void WriteValue(decimal value)
		{
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Float);
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x00043808 File Offset: 0x00041A08
		public override void WriteValue(DateTime value)
		{
			base.InternalWriteValue(JsonToken.Date);
			value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
			if (string.IsNullOrEmpty(base.DateFormatString))
			{
				int num = this.WriteValueToBuffer(value);
				this._writer.Write(this._writeBuffer, 0, num);
				return;
			}
			this._writer.Write(this._quoteChar);
			this._writer.Write(value.ToString(base.DateFormatString, base.Culture));
			this._writer.Write(this._quoteChar);
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x0004389C File Offset: 0x00041A9C
		private int WriteValueToBuffer(DateTime value)
		{
			this.EnsureWriteBuffer();
			int num = 0;
			this._writeBuffer[num++] = this._quoteChar;
			num = DateTimeUtils.WriteDateTimeString(this._writeBuffer, num, value, null, value.Kind, base.DateFormatHandling);
			this._writeBuffer[num++] = this._quoteChar;
			return num;
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x00043900 File Offset: 0x00041B00
		public override void WriteValue(byte[] value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.Bytes);
			this._writer.Write(this._quoteChar);
			this.Base64Encoder.Encode(value, 0, value.Length);
			this.Base64Encoder.Flush();
			this._writer.Write(this._quoteChar);
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x00043964 File Offset: 0x00041B64
		public override void WriteValue(DateTimeOffset value)
		{
			base.InternalWriteValue(JsonToken.Date);
			if (string.IsNullOrEmpty(base.DateFormatString))
			{
				int num = this.WriteValueToBuffer(value);
				this._writer.Write(this._writeBuffer, 0, num);
				return;
			}
			this._writer.Write(this._quoteChar);
			this._writer.Write(value.ToString(base.DateFormatString, base.Culture));
			this._writer.Write(this._quoteChar);
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x000439EC File Offset: 0x00041BEC
		private int WriteValueToBuffer(DateTimeOffset value)
		{
			this.EnsureWriteBuffer();
			int num = 0;
			this._writeBuffer[num++] = this._quoteChar;
			num = DateTimeUtils.WriteDateTimeString(this._writeBuffer, num, (base.DateFormatHandling == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, base.DateFormatHandling);
			this._writeBuffer[num++] = this._quoteChar;
			return num;
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x00043A68 File Offset: 0x00041C68
		public override void WriteValue(Guid value)
		{
			base.InternalWriteValue(JsonToken.String);
			string text = value.ToString("D", CultureInfo.InvariantCulture);
			this._writer.Write(this._quoteChar);
			this._writer.Write(text);
			this._writer.Write(this._quoteChar);
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x00043AC4 File Offset: 0x00041CC4
		public override void WriteValue(TimeSpan value)
		{
			base.InternalWriteValue(JsonToken.String);
			string text = value.ToString(null, CultureInfo.InvariantCulture);
			this._writer.Write(this._quoteChar);
			this._writer.Write(text);
			this._writer.Write(this._quoteChar);
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x00043B1C File Offset: 0x00041D1C
		public override void WriteValue(Uri value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.String);
			this.WriteEscapedString(value.OriginalString, true);
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00043B58 File Offset: 0x00041D58
		public override void WriteComment(string text)
		{
			base.InternalWriteComment();
			this._writer.Write("/*");
			this._writer.Write(text);
			this._writer.Write("*/");
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x00043B9C File Offset: 0x00041D9C
		public override void WriteWhitespace(string ws)
		{
			base.InternalWriteWhitespace(ws);
			this._writer.Write(ws);
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00043BB4 File Offset: 0x00041DB4
		private void EnsureWriteBuffer()
		{
			if (this._writeBuffer == null)
			{
				this._writeBuffer = BufferUtils.RentBuffer(this._arrayPool, 35);
			}
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x00043BD4 File Offset: 0x00041DD4
		private void WriteIntegerValue(long value)
		{
			if (value >= 0L && value <= 9L)
			{
				this._writer.Write((char)(48L + value));
				return;
			}
			bool flag = value < 0L;
			this.WriteIntegerValue((ulong)(flag ? (-(ulong)value) : value), flag);
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x00043C24 File Offset: 0x00041E24
		private void WriteIntegerValue(ulong uvalue, bool negative)
		{
			if (!negative & (uvalue <= 9UL))
			{
				this._writer.Write((char)(48UL + uvalue));
				return;
			}
			int num = this.WriteNumberToBuffer(uvalue, negative);
			this._writer.Write(this._writeBuffer, 0, num);
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00043C78 File Offset: 0x00041E78
		private int WriteNumberToBuffer(ulong value, bool negative)
		{
			this.EnsureWriteBuffer();
			int num = MathUtils.IntLength(value);
			if (negative)
			{
				num++;
				this._writeBuffer[0] = '-';
			}
			int num2 = num;
			do
			{
				this._writeBuffer[--num2] = (char)(48UL + value % 10UL);
				value /= 10UL;
			}
			while (value != 0UL);
			return num;
		}

		// Token: 0x0400040C RID: 1036
		private readonly bool _safeAsync;

		// Token: 0x0400040D RID: 1037
		private const int IndentCharBufferSize = 12;

		// Token: 0x0400040E RID: 1038
		private readonly TextWriter _writer;

		// Token: 0x0400040F RID: 1039
		private Base64Encoder _base64Encoder;

		// Token: 0x04000410 RID: 1040
		private char _indentChar;

		// Token: 0x04000411 RID: 1041
		private int _indentation;

		// Token: 0x04000412 RID: 1042
		private char _quoteChar;

		// Token: 0x04000413 RID: 1043
		private bool _quoteName;

		// Token: 0x04000414 RID: 1044
		private bool[] _charEscapeFlags;

		// Token: 0x04000415 RID: 1045
		private char[] _writeBuffer;

		// Token: 0x04000416 RID: 1046
		private IArrayPool<char> _arrayPool;

		// Token: 0x04000417 RID: 1047
		private char[] _indentChars;
	}
}
