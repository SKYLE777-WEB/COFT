using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000BA RID: 186
	public abstract class JsonWriter : IDisposable
	{
		// Token: 0x06000A8E RID: 2702 RVA: 0x000455FC File Offset: 0x000437FC
		internal Task AutoCompleteAsync(JsonToken tokenBeingWritten, CancellationToken cancellationToken)
		{
			JsonWriter.State currentState = this._currentState;
			JsonWriter.State state = JsonWriter.StateArray[(int)tokenBeingWritten][(int)currentState];
			if (state == JsonWriter.State.Error)
			{
				throw JsonWriterException.Create(this, "Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), currentState.ToString()), null);
			}
			this._currentState = state;
			if (this._formatting == Formatting.Indented)
			{
				switch (currentState)
				{
				case JsonWriter.State.Start:
					goto IL_0115;
				case JsonWriter.State.Property:
					return this.WriteIndentSpaceAsync(cancellationToken);
				case JsonWriter.State.Object:
					if (tokenBeingWritten == JsonToken.PropertyName)
					{
						return this.AutoCompleteAsync(cancellationToken);
					}
					if (tokenBeingWritten != JsonToken.Comment)
					{
						return this.WriteValueDelimiterAsync(cancellationToken);
					}
					goto IL_0115;
				case JsonWriter.State.ArrayStart:
				case JsonWriter.State.ConstructorStart:
					return this.WriteIndentAsync(cancellationToken);
				case JsonWriter.State.Array:
				case JsonWriter.State.Constructor:
					if (tokenBeingWritten != JsonToken.Comment)
					{
						return this.AutoCompleteAsync(cancellationToken);
					}
					return this.WriteIndentAsync(cancellationToken);
				}
				if (tokenBeingWritten == JsonToken.PropertyName)
				{
					return this.WriteIndentAsync(cancellationToken);
				}
			}
			else if (tokenBeingWritten != JsonToken.Comment)
			{
				switch (currentState)
				{
				case JsonWriter.State.Object:
				case JsonWriter.State.Array:
				case JsonWriter.State.Constructor:
					return this.WriteValueDelimiterAsync(cancellationToken);
				}
			}
			IL_0115:
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x00045728 File Offset: 0x00043928
		private async Task AutoCompleteAsync(CancellationToken cancellationToken)
		{
			await this.WriteValueDelimiterAsync(cancellationToken).ConfigureAwait(false);
			await this.WriteIndentAsync(cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0004577C File Offset: 0x0004397C
		public virtual Task CloseAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.Close();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x0004579C File Offset: 0x0004399C
		public virtual Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.Flush();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x000457BC File Offset: 0x000439BC
		protected virtual Task WriteEndAsync(JsonToken token, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteEnd(token);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x000457E0 File Offset: 0x000439E0
		protected virtual Task WriteIndentAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteIndent();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x00045800 File Offset: 0x00043A00
		protected virtual Task WriteValueDelimiterAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValueDelimiter();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00045820 File Offset: 0x00043A20
		protected virtual Task WriteIndentSpaceAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteIndentSpace();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x00045840 File Offset: 0x00043A40
		public virtual Task WriteRawAsync(string json, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteRaw(json);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x00045864 File Offset: 0x00043A64
		public virtual Task WriteEndAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteEnd();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x00045884 File Offset: 0x00043A84
		internal Task WriteEndInternalAsync(CancellationToken cancellationToken)
		{
			JsonContainerType jsonContainerType = this.Peek();
			switch (jsonContainerType)
			{
			case JsonContainerType.Object:
				return this.WriteEndObjectAsync(cancellationToken);
			case JsonContainerType.Array:
				return this.WriteEndArrayAsync(cancellationToken);
			case JsonContainerType.Constructor:
				return this.WriteEndConstructorAsync(cancellationToken);
			default:
				if (cancellationToken.IsCancellationRequested)
				{
					return cancellationToken.FromCanceled();
				}
				throw JsonWriterException.Create(this, "Unexpected type when writing end: " + jsonContainerType, null);
			}
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x000458F8 File Offset: 0x00043AF8
		internal async Task InternalWriteEndAsync(JsonContainerType type, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int levelsToComplete = this.CalculateLevelsToComplete(type);
			while (levelsToComplete-- > 0)
			{
				JsonToken token = this.GetCloseTokenForType(this.Pop());
				if (this._currentState == JsonWriter.State.Property)
				{
					await this.WriteNullAsync(cancellationToken).ConfigureAwait(false);
				}
				if (this._formatting == Formatting.Indented && this._currentState != JsonWriter.State.ObjectStart && this._currentState != JsonWriter.State.ArrayStart)
				{
					await this.WriteIndentAsync(cancellationToken).ConfigureAwait(false);
				}
				await this.WriteEndAsync(token, cancellationToken).ConfigureAwait(false);
				this.UpdateCurrentState();
			}
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x00045954 File Offset: 0x00043B54
		public virtual Task WriteEndArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteEndArray();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x00045974 File Offset: 0x00043B74
		public virtual Task WriteEndConstructorAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteEndConstructor();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x00045994 File Offset: 0x00043B94
		public virtual Task WriteEndObjectAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteEndObject();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x000459B4 File Offset: 0x00043BB4
		public virtual Task WriteNullAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteNull();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x000459D4 File Offset: 0x00043BD4
		public virtual Task WritePropertyNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WritePropertyName(name);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x000459F8 File Offset: 0x00043BF8
		public virtual Task WritePropertyNameAsync(string name, bool escape, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WritePropertyName(name, escape);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00045A1C File Offset: 0x00043C1C
		internal Task InternalWritePropertyNameAsync(string name, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this._currentPosition.PropertyName = name;
			return this.AutoCompleteAsync(JsonToken.PropertyName, cancellationToken);
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00045A48 File Offset: 0x00043C48
		public virtual Task WriteStartArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteStartArray();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00045A68 File Offset: 0x00043C68
		internal async Task InternalWriteStartAsync(JsonToken token, JsonContainerType container, CancellationToken cancellationToken)
		{
			this.UpdateScopeWithFinishedValue();
			await this.AutoCompleteAsync(token, cancellationToken).ConfigureAwait(false);
			this.Push(container);
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x00045ACC File Offset: 0x00043CCC
		public virtual Task WriteCommentAsync(string text, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteComment(text);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x00045AF0 File Offset: 0x00043CF0
		internal Task InternalWriteCommentAsync(CancellationToken cancellationToken)
		{
			return this.AutoCompleteAsync(JsonToken.Comment, cancellationToken);
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00045AFC File Offset: 0x00043CFC
		public virtual Task WriteRawValueAsync(string json, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteRawValue(json);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x00045B20 File Offset: 0x00043D20
		public virtual Task WriteStartConstructorAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteStartConstructor(name);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00045B44 File Offset: 0x00043D44
		public virtual Task WriteStartObjectAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteStartObject();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x00045B64 File Offset: 0x00043D64
		public Task WriteTokenAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return this.WriteTokenAsync(reader, true, cancellationToken);
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x00045B70 File Offset: 0x00043D70
		public Task WriteTokenAsync(JsonReader reader, bool writeChildren, CancellationToken cancellationToken = default(CancellationToken))
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			return this.WriteTokenAsync(reader, writeChildren, true, true, cancellationToken);
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00045B88 File Offset: 0x00043D88
		public Task WriteTokenAsync(JsonToken token, CancellationToken cancellationToken = default(CancellationToken))
		{
			return this.WriteTokenAsync(token, null, cancellationToken);
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00045B94 File Offset: 0x00043D94
		public Task WriteTokenAsync(JsonToken token, object value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			switch (token)
			{
			case JsonToken.None:
				return AsyncUtils.CompletedTask;
			case JsonToken.StartObject:
				return this.WriteStartObjectAsync(cancellationToken);
			case JsonToken.StartArray:
				return this.WriteStartArrayAsync(cancellationToken);
			case JsonToken.StartConstructor:
				ValidationUtils.ArgumentNotNull(value, "value");
				return this.WriteStartConstructorAsync(value.ToString(), cancellationToken);
			case JsonToken.PropertyName:
				ValidationUtils.ArgumentNotNull(value, "value");
				return this.WritePropertyNameAsync(value.ToString(), cancellationToken);
			case JsonToken.Comment:
				return this.WriteCommentAsync((value != null) ? value.ToString() : null, cancellationToken);
			case JsonToken.Raw:
				return this.WriteRawValueAsync((value != null) ? value.ToString() : null, cancellationToken);
			case JsonToken.Integer:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (!(value is BigInteger))
				{
					return this.WriteValueAsync(Convert.ToInt64(value, CultureInfo.InvariantCulture), cancellationToken);
				}
				return this.WriteValueAsync((BigInteger)value, cancellationToken);
			case JsonToken.Float:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (value is decimal)
				{
					return this.WriteValueAsync((decimal)value, cancellationToken);
				}
				if (value is double)
				{
					return this.WriteValueAsync((double)value, cancellationToken);
				}
				if (value is float)
				{
					return this.WriteValueAsync((float)value, cancellationToken);
				}
				return this.WriteValueAsync(Convert.ToDouble(value, CultureInfo.InvariantCulture), cancellationToken);
			case JsonToken.String:
				ValidationUtils.ArgumentNotNull(value, "value");
				return this.WriteValueAsync(value.ToString(), cancellationToken);
			case JsonToken.Boolean:
				ValidationUtils.ArgumentNotNull(value, "value");
				return this.WriteValueAsync(Convert.ToBoolean(value, CultureInfo.InvariantCulture), cancellationToken);
			case JsonToken.Null:
				return this.WriteNullAsync(cancellationToken);
			case JsonToken.Undefined:
				return this.WriteUndefinedAsync(cancellationToken);
			case JsonToken.EndObject:
				return this.WriteEndObjectAsync(cancellationToken);
			case JsonToken.EndArray:
				return this.WriteEndArrayAsync(cancellationToken);
			case JsonToken.EndConstructor:
				return this.WriteEndConstructorAsync(cancellationToken);
			case JsonToken.Date:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (value is DateTimeOffset)
				{
					return this.WriteValueAsync((DateTimeOffset)value, cancellationToken);
				}
				return this.WriteValueAsync(Convert.ToDateTime(value, CultureInfo.InvariantCulture), cancellationToken);
			case JsonToken.Bytes:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (value is Guid)
				{
					return this.WriteValueAsync((Guid)value, cancellationToken);
				}
				return this.WriteValueAsync((byte[])value, cancellationToken);
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("token", token, "Unexpected token type.");
			}
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x00045E0C File Offset: 0x0004400C
		internal virtual async Task WriteTokenAsync(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments, CancellationToken cancellationToken)
		{
			int initialDepth = this.CalculateWriteTokenInitialDepth(reader);
			bool flag;
			do
			{
				if (writeDateConstructorAsDate && reader.TokenType == JsonToken.StartConstructor && string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
				{
					await this.WriteConstructorDateAsync(reader, cancellationToken).ConfigureAwait(false);
				}
				else if (writeComments || reader.TokenType != JsonToken.Comment)
				{
					await this.WriteTokenAsync(reader.TokenType, reader.Value, cancellationToken).ConfigureAwait(false);
				}
				flag = initialDepth - 1 < reader.Depth - (JsonTokenUtils.IsEndToken(reader.TokenType) ? 1 : 0) && writeChildren;
				if (flag)
				{
					flag = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			while (flag);
			if (initialDepth < this.CalculateWriteTokenFinalDepth(reader))
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading token.", null);
			}
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x00045E80 File Offset: 0x00044080
		internal async Task WriteTokenSyncReadingAsync(JsonReader reader, CancellationToken cancellationToken)
		{
			int initialDepth = this.CalculateWriteTokenInitialDepth(reader);
			bool flag;
			do
			{
				if (reader.TokenType == JsonToken.StartConstructor && string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
				{
					this.WriteConstructorDate(reader);
				}
				else
				{
					this.WriteToken(reader.TokenType, reader.Value);
				}
				flag = initialDepth - 1 < reader.Depth - (JsonTokenUtils.IsEndToken(reader.TokenType) ? 1 : 0);
				if (flag)
				{
					flag = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			while (flag);
			if (initialDepth < this.CalculateWriteTokenFinalDepth(reader))
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading token.", null);
			}
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x00045EDC File Offset: 0x000440DC
		private async Task WriteConstructorDateAsync(JsonReader reader, CancellationToken cancellationToken)
		{
			if (!(await reader.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.Integer)
			{
				throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected Integer, got " + reader.TokenType, null);
			}
			DateTime date = DateTimeUtils.ConvertJavaScriptTicksToDateTime((long)reader.Value);
			if (!(await reader.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected EndConstructor, got " + reader.TokenType, null);
			}
			await this.WriteValueAsync(date, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x00045F38 File Offset: 0x00044138
		public virtual Task WriteValueAsync(bool value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00045F5C File Offset: 0x0004415C
		public virtual Task WriteValueAsync(bool? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x00045F80 File Offset: 0x00044180
		public virtual Task WriteValueAsync(byte value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x00045FA4 File Offset: 0x000441A4
		public virtual Task WriteValueAsync(byte? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x00045FC8 File Offset: 0x000441C8
		public virtual Task WriteValueAsync(byte[] value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x00045FEC File Offset: 0x000441EC
		public virtual Task WriteValueAsync(char value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00046010 File Offset: 0x00044210
		public virtual Task WriteValueAsync(char? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x00046034 File Offset: 0x00044234
		public virtual Task WriteValueAsync(DateTime value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x00046058 File Offset: 0x00044258
		public virtual Task WriteValueAsync(DateTime? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0004607C File Offset: 0x0004427C
		public virtual Task WriteValueAsync(DateTimeOffset value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x000460A0 File Offset: 0x000442A0
		public virtual Task WriteValueAsync(DateTimeOffset? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x000460C4 File Offset: 0x000442C4
		public virtual Task WriteValueAsync(decimal value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x000460E8 File Offset: 0x000442E8
		public virtual Task WriteValueAsync(decimal? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x0004610C File Offset: 0x0004430C
		public virtual Task WriteValueAsync(double value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x00046130 File Offset: 0x00044330
		public virtual Task WriteValueAsync(double? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x00046154 File Offset: 0x00044354
		public virtual Task WriteValueAsync(float value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x00046178 File Offset: 0x00044378
		public virtual Task WriteValueAsync(float? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x0004619C File Offset: 0x0004439C
		public virtual Task WriteValueAsync(Guid value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x000461C0 File Offset: 0x000443C0
		public virtual Task WriteValueAsync(Guid? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x000461E4 File Offset: 0x000443E4
		public virtual Task WriteValueAsync(int value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x00046208 File Offset: 0x00044408
		public virtual Task WriteValueAsync(int? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0004622C File Offset: 0x0004442C
		public virtual Task WriteValueAsync(long value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x00046250 File Offset: 0x00044450
		public virtual Task WriteValueAsync(long? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x00046274 File Offset: 0x00044474
		public virtual Task WriteValueAsync(object value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x00046298 File Offset: 0x00044498
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(sbyte value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x000462BC File Offset: 0x000444BC
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(sbyte? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x000462E0 File Offset: 0x000444E0
		public virtual Task WriteValueAsync(short value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x00046304 File Offset: 0x00044504
		public virtual Task WriteValueAsync(short? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x00046328 File Offset: 0x00044528
		public virtual Task WriteValueAsync(string value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0004634C File Offset: 0x0004454C
		public virtual Task WriteValueAsync(TimeSpan value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x00046370 File Offset: 0x00044570
		public virtual Task WriteValueAsync(TimeSpan? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x00046394 File Offset: 0x00044594
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(uint value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x000463B8 File Offset: 0x000445B8
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(uint? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x000463DC File Offset: 0x000445DC
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(ulong value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x00046400 File Offset: 0x00044600
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(ulong? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x00046424 File Offset: 0x00044624
		public virtual Task WriteValueAsync(Uri value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x00046448 File Offset: 0x00044648
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(ushort value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x0004646C File Offset: 0x0004466C
		[CLSCompliant(false)]
		public virtual Task WriteValueAsync(ushort? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteValue(value);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x00046490 File Offset: 0x00044690
		public virtual Task WriteUndefinedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteUndefined();
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x000464B0 File Offset: 0x000446B0
		public virtual Task WriteWhitespaceAsync(string ws, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.WriteWhitespace(ws);
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x000464D4 File Offset: 0x000446D4
		internal Task InternalWriteValueAsync(JsonToken token, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			this.UpdateScopeWithFinishedValue();
			return this.AutoCompleteAsync(token, cancellationToken);
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x000464F8 File Offset: 0x000446F8
		protected Task SetWriteStateAsync(JsonToken token, object value, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			switch (token)
			{
			case JsonToken.StartObject:
				return this.InternalWriteStartAsync(token, JsonContainerType.Object, cancellationToken);
			case JsonToken.StartArray:
				return this.InternalWriteStartAsync(token, JsonContainerType.Array, cancellationToken);
			case JsonToken.StartConstructor:
				return this.InternalWriteStartAsync(token, JsonContainerType.Constructor, cancellationToken);
			case JsonToken.PropertyName:
				if (!(value is string))
				{
					throw new ArgumentException("A name is required when setting property name state.", "value");
				}
				return this.InternalWritePropertyNameAsync((string)value, cancellationToken);
			case JsonToken.Comment:
				return this.InternalWriteCommentAsync(cancellationToken);
			case JsonToken.Raw:
				return AsyncUtils.CompletedTask;
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return this.InternalWriteValueAsync(token, cancellationToken);
			case JsonToken.EndObject:
				return this.InternalWriteEndAsync(JsonContainerType.Object, cancellationToken);
			case JsonToken.EndArray:
				return this.InternalWriteEndAsync(JsonContainerType.Array, cancellationToken);
			case JsonToken.EndConstructor:
				return this.InternalWriteEndAsync(JsonContainerType.Constructor, cancellationToken);
			default:
				throw new ArgumentOutOfRangeException("token");
			}
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x000465F0 File Offset: 0x000447F0
		internal static Task WriteValueAsync(JsonWriter writer, PrimitiveTypeCode typeCode, object value, CancellationToken cancellationToken)
		{
			switch (typeCode)
			{
			case PrimitiveTypeCode.Char:
				return writer.WriteValueAsync((char)value, cancellationToken);
			case PrimitiveTypeCode.CharNullable:
				return writer.WriteValueAsync((value == null) ? null : new char?((char)value), cancellationToken);
			case PrimitiveTypeCode.Boolean:
				return writer.WriteValueAsync((bool)value, cancellationToken);
			case PrimitiveTypeCode.BooleanNullable:
				return writer.WriteValueAsync((value == null) ? null : new bool?((bool)value), cancellationToken);
			case PrimitiveTypeCode.SByte:
				return writer.WriteValueAsync((sbyte)value, cancellationToken);
			case PrimitiveTypeCode.SByteNullable:
				return writer.WriteValueAsync((value == null) ? null : new sbyte?((sbyte)value), cancellationToken);
			case PrimitiveTypeCode.Int16:
				return writer.WriteValueAsync((short)value, cancellationToken);
			case PrimitiveTypeCode.Int16Nullable:
				return writer.WriteValueAsync((value == null) ? null : new short?((short)value), cancellationToken);
			case PrimitiveTypeCode.UInt16:
				return writer.WriteValueAsync((ushort)value, cancellationToken);
			case PrimitiveTypeCode.UInt16Nullable:
				return writer.WriteValueAsync((value == null) ? null : new ushort?((ushort)value), cancellationToken);
			case PrimitiveTypeCode.Int32:
				return writer.WriteValueAsync((int)value, cancellationToken);
			case PrimitiveTypeCode.Int32Nullable:
				return writer.WriteValueAsync((value == null) ? null : new int?((int)value), cancellationToken);
			case PrimitiveTypeCode.Byte:
				return writer.WriteValueAsync((byte)value, cancellationToken);
			case PrimitiveTypeCode.ByteNullable:
				return writer.WriteValueAsync((value == null) ? null : new byte?((byte)value), cancellationToken);
			case PrimitiveTypeCode.UInt32:
				return writer.WriteValueAsync((uint)value, cancellationToken);
			case PrimitiveTypeCode.UInt32Nullable:
				return writer.WriteValueAsync((value == null) ? null : new uint?((uint)value), cancellationToken);
			case PrimitiveTypeCode.Int64:
				return writer.WriteValueAsync((long)value, cancellationToken);
			case PrimitiveTypeCode.Int64Nullable:
				return writer.WriteValueAsync((value == null) ? null : new long?((long)value), cancellationToken);
			case PrimitiveTypeCode.UInt64:
				return writer.WriteValueAsync((ulong)value, cancellationToken);
			case PrimitiveTypeCode.UInt64Nullable:
				return writer.WriteValueAsync((value == null) ? null : new ulong?((ulong)value), cancellationToken);
			case PrimitiveTypeCode.Single:
				return writer.WriteValueAsync((float)value, cancellationToken);
			case PrimitiveTypeCode.SingleNullable:
				return writer.WriteValueAsync((value == null) ? null : new float?((float)value), cancellationToken);
			case PrimitiveTypeCode.Double:
				return writer.WriteValueAsync((double)value, cancellationToken);
			case PrimitiveTypeCode.DoubleNullable:
				return writer.WriteValueAsync((value == null) ? null : new double?((double)value), cancellationToken);
			case PrimitiveTypeCode.DateTime:
				return writer.WriteValueAsync((DateTime)value, cancellationToken);
			case PrimitiveTypeCode.DateTimeNullable:
				return writer.WriteValueAsync((value == null) ? null : new DateTime?((DateTime)value), cancellationToken);
			case PrimitiveTypeCode.DateTimeOffset:
				return writer.WriteValueAsync((DateTimeOffset)value, cancellationToken);
			case PrimitiveTypeCode.DateTimeOffsetNullable:
				return writer.WriteValueAsync((value == null) ? null : new DateTimeOffset?((DateTimeOffset)value), cancellationToken);
			case PrimitiveTypeCode.Decimal:
				return writer.WriteValueAsync((decimal)value, cancellationToken);
			case PrimitiveTypeCode.DecimalNullable:
				return writer.WriteValueAsync((value == null) ? null : new decimal?((decimal)value), cancellationToken);
			case PrimitiveTypeCode.Guid:
				return writer.WriteValueAsync((Guid)value, cancellationToken);
			case PrimitiveTypeCode.GuidNullable:
				return writer.WriteValueAsync((value == null) ? null : new Guid?((Guid)value), cancellationToken);
			case PrimitiveTypeCode.TimeSpan:
				return writer.WriteValueAsync((TimeSpan)value, cancellationToken);
			case PrimitiveTypeCode.TimeSpanNullable:
				return writer.WriteValueAsync((value == null) ? null : new TimeSpan?((TimeSpan)value), cancellationToken);
			case PrimitiveTypeCode.BigInteger:
				return writer.WriteValueAsync((BigInteger)value, cancellationToken);
			case PrimitiveTypeCode.BigIntegerNullable:
				return writer.WriteValueAsync((value == null) ? null : new BigInteger?((BigInteger)value), cancellationToken);
			case PrimitiveTypeCode.Uri:
				return writer.WriteValueAsync((Uri)value, cancellationToken);
			case PrimitiveTypeCode.String:
				return writer.WriteValueAsync((string)value, cancellationToken);
			case PrimitiveTypeCode.Bytes:
				return writer.WriteValueAsync((byte[])value, cancellationToken);
			case PrimitiveTypeCode.DBNull:
				return writer.WriteNullAsync(cancellationToken);
			default:
			{
				IConvertible convertible = value as IConvertible;
				if (convertible != null)
				{
					TypeInformation typeInformation = ConvertUtils.GetTypeInformation(convertible);
					PrimitiveTypeCode primitiveTypeCode = ((typeInformation.TypeCode == PrimitiveTypeCode.Object) ? PrimitiveTypeCode.String : typeInformation.TypeCode);
					Type type = ((typeInformation.TypeCode == PrimitiveTypeCode.Object) ? typeof(string) : typeInformation.Type);
					object obj = convertible.ToType(type, CultureInfo.InvariantCulture);
					return JsonWriter.WriteValueAsync(writer, primitiveTypeCode, obj, cancellationToken);
				}
				throw JsonWriter.CreateUnsupportedTypeException(writer, value);
			}
			}
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x00046B30 File Offset: 0x00044D30
		internal static JsonWriter.State[][] BuildStateArray()
		{
			List<JsonWriter.State[]> list = JsonWriter.StateArrayTempate.ToList<JsonWriter.State[]>();
			JsonWriter.State[] array = JsonWriter.StateArrayTempate[0];
			JsonWriter.State[] array2 = JsonWriter.StateArrayTempate[7];
			foreach (object obj in EnumUtils.GetValues(typeof(JsonToken)))
			{
				JsonToken jsonToken = (JsonToken)obj;
				if (list.Count <= (int)jsonToken)
				{
					if (jsonToken - JsonToken.Integer <= 5 || jsonToken - JsonToken.Date <= 1)
					{
						list.Add(array2);
					}
					else
					{
						list.Add(array);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000ADC RID: 2780 RVA: 0x00046CBC File Offset: 0x00044EBC
		// (set) Token: 0x06000ADD RID: 2781 RVA: 0x00046CC4 File Offset: 0x00044EC4
		public bool CloseOutput { get; set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000ADE RID: 2782 RVA: 0x00046CD0 File Offset: 0x00044ED0
		// (set) Token: 0x06000ADF RID: 2783 RVA: 0x00046CD8 File Offset: 0x00044ED8
		public bool AutoCompleteOnClose { get; set; }

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x00046CE4 File Offset: 0x00044EE4
		protected internal int Top
		{
			get
			{
				int num = ((this._stack != null) ? this._stack.Count : 0);
				if (this.Peek() != JsonContainerType.None)
				{
					num++;
				}
				return num;
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x00046D24 File Offset: 0x00044F24
		public WriteState WriteState
		{
			get
			{
				switch (this._currentState)
				{
				case JsonWriter.State.Start:
					return WriteState.Start;
				case JsonWriter.State.Property:
					return WriteState.Property;
				case JsonWriter.State.ObjectStart:
				case JsonWriter.State.Object:
					return WriteState.Object;
				case JsonWriter.State.ArrayStart:
				case JsonWriter.State.Array:
					return WriteState.Array;
				case JsonWriter.State.ConstructorStart:
				case JsonWriter.State.Constructor:
					return WriteState.Constructor;
				case JsonWriter.State.Closed:
					return WriteState.Closed;
				case JsonWriter.State.Error:
					return WriteState.Error;
				default:
					throw JsonWriterException.Create(this, "Invalid state: " + this._currentState, null);
				}
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000AE2 RID: 2786 RVA: 0x00046D9C File Offset: 0x00044F9C
		internal string ContainerPath
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None || this._stack == null)
				{
					return string.Empty;
				}
				return JsonPosition.BuildPath(this._stack, null);
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x00046DE4 File Offset: 0x00044FE4
		public string Path
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				JsonPosition? jsonPosition = ((this._currentState != JsonWriter.State.ArrayStart && this._currentState != JsonWriter.State.ConstructorStart && this._currentState != JsonWriter.State.ObjectStart) ? new JsonPosition?(this._currentPosition) : null);
				return JsonPosition.BuildPath(this._stack, jsonPosition);
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000AE4 RID: 2788 RVA: 0x00046E60 File Offset: 0x00045060
		// (set) Token: 0x06000AE5 RID: 2789 RVA: 0x00046E68 File Offset: 0x00045068
		public Formatting Formatting
		{
			get
			{
				return this._formatting;
			}
			set
			{
				if (value < Formatting.None || value > Formatting.Indented)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._formatting = value;
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x00046E8C File Offset: 0x0004508C
		// (set) Token: 0x06000AE7 RID: 2791 RVA: 0x00046E94 File Offset: 0x00045094
		public DateFormatHandling DateFormatHandling
		{
			get
			{
				return this._dateFormatHandling;
			}
			set
			{
				if (value < DateFormatHandling.IsoDateFormat || value > DateFormatHandling.MicrosoftDateFormat)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._dateFormatHandling = value;
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x00046EB8 File Offset: 0x000450B8
		// (set) Token: 0x06000AE9 RID: 2793 RVA: 0x00046EC0 File Offset: 0x000450C0
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

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000AEA RID: 2794 RVA: 0x00046EE4 File Offset: 0x000450E4
		// (set) Token: 0x06000AEB RID: 2795 RVA: 0x00046EEC File Offset: 0x000450EC
		public StringEscapeHandling StringEscapeHandling
		{
			get
			{
				return this._stringEscapeHandling;
			}
			set
			{
				if (value < StringEscapeHandling.Default || value > StringEscapeHandling.EscapeHtml)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._stringEscapeHandling = value;
				this.OnStringEscapeHandlingChanged();
			}
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x00046F14 File Offset: 0x00045114
		internal virtual void OnStringEscapeHandlingChanged()
		{
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000AED RID: 2797 RVA: 0x00046F18 File Offset: 0x00045118
		// (set) Token: 0x06000AEE RID: 2798 RVA: 0x00046F20 File Offset: 0x00045120
		public FloatFormatHandling FloatFormatHandling
		{
			get
			{
				return this._floatFormatHandling;
			}
			set
			{
				if (value < FloatFormatHandling.String || value > FloatFormatHandling.DefaultValue)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._floatFormatHandling = value;
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000AEF RID: 2799 RVA: 0x00046F44 File Offset: 0x00045144
		// (set) Token: 0x06000AF0 RID: 2800 RVA: 0x00046F4C File Offset: 0x0004514C
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

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x00046F58 File Offset: 0x00045158
		// (set) Token: 0x06000AF2 RID: 2802 RVA: 0x00046F6C File Offset: 0x0004516C
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

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00046F78 File Offset: 0x00045178
		protected JsonWriter()
		{
			this._currentState = JsonWriter.State.Start;
			this._formatting = Formatting.None;
			this._dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			this.CloseOutput = true;
			this.AutoCompleteOnClose = true;
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00046FA4 File Offset: 0x000451A4
		internal void UpdateScopeWithFinishedValue()
		{
			if (this._currentPosition.HasIndex)
			{
				this._currentPosition.Position = this._currentPosition.Position + 1;
			}
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00046FC8 File Offset: 0x000451C8
		private void Push(JsonContainerType value)
		{
			if (this._currentPosition.Type != JsonContainerType.None)
			{
				if (this._stack == null)
				{
					this._stack = new List<JsonPosition>();
				}
				this._stack.Add(this._currentPosition);
			}
			this._currentPosition = new JsonPosition(value);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x0004701C File Offset: 0x0004521C
		private JsonContainerType Pop()
		{
			ref JsonPosition currentPosition = this._currentPosition;
			if (this._stack != null && this._stack.Count > 0)
			{
				this._currentPosition = this._stack[this._stack.Count - 1];
				this._stack.RemoveAt(this._stack.Count - 1);
			}
			else
			{
				this._currentPosition = default(JsonPosition);
			}
			return currentPosition.Type;
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x0004709C File Offset: 0x0004529C
		private JsonContainerType Peek()
		{
			return this._currentPosition.Type;
		}

		// Token: 0x06000AF8 RID: 2808
		public abstract void Flush();

		// Token: 0x06000AF9 RID: 2809 RVA: 0x000470AC File Offset: 0x000452AC
		public virtual void Close()
		{
			if (this.AutoCompleteOnClose)
			{
				this.AutoCompleteAll();
			}
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x000470C0 File Offset: 0x000452C0
		public virtual void WriteStartObject()
		{
			this.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x000470CC File Offset: 0x000452CC
		public virtual void WriteEndObject()
		{
			this.InternalWriteEnd(JsonContainerType.Object);
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x000470D8 File Offset: 0x000452D8
		public virtual void WriteStartArray()
		{
			this.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x000470E4 File Offset: 0x000452E4
		public virtual void WriteEndArray()
		{
			this.InternalWriteEnd(JsonContainerType.Array);
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x000470F0 File Offset: 0x000452F0
		public virtual void WriteStartConstructor(string name)
		{
			this.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x000470FC File Offset: 0x000452FC
		public virtual void WriteEndConstructor()
		{
			this.InternalWriteEnd(JsonContainerType.Constructor);
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x00047108 File Offset: 0x00045308
		public virtual void WritePropertyName(string name)
		{
			this.InternalWritePropertyName(name);
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x00047114 File Offset: 0x00045314
		public virtual void WritePropertyName(string name, bool escape)
		{
			this.WritePropertyName(name);
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00047120 File Offset: 0x00045320
		public virtual void WriteEnd()
		{
			this.WriteEnd(this.Peek());
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00047130 File Offset: 0x00045330
		public void WriteToken(JsonReader reader)
		{
			this.WriteToken(reader, true);
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x0004713C File Offset: 0x0004533C
		public void WriteToken(JsonReader reader, bool writeChildren)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this.WriteToken(reader, writeChildren, true, true);
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00047154 File Offset: 0x00045354
		public void WriteToken(JsonToken token, object value)
		{
			switch (token)
			{
			case JsonToken.None:
				return;
			case JsonToken.StartObject:
				this.WriteStartObject();
				return;
			case JsonToken.StartArray:
				this.WriteStartArray();
				return;
			case JsonToken.StartConstructor:
				ValidationUtils.ArgumentNotNull(value, "value");
				this.WriteStartConstructor(value.ToString());
				return;
			case JsonToken.PropertyName:
				ValidationUtils.ArgumentNotNull(value, "value");
				this.WritePropertyName(value.ToString());
				return;
			case JsonToken.Comment:
				this.WriteComment((value != null) ? value.ToString() : null);
				return;
			case JsonToken.Raw:
				this.WriteRawValue((value != null) ? value.ToString() : null);
				return;
			case JsonToken.Integer:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (value is BigInteger)
				{
					this.WriteValue((BigInteger)value);
					return;
				}
				this.WriteValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
				return;
			case JsonToken.Float:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (value is decimal)
				{
					this.WriteValue((decimal)value);
					return;
				}
				if (value is double)
				{
					this.WriteValue((double)value);
					return;
				}
				if (value is float)
				{
					this.WriteValue((float)value);
					return;
				}
				this.WriteValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
				return;
			case JsonToken.String:
				ValidationUtils.ArgumentNotNull(value, "value");
				this.WriteValue(value.ToString());
				return;
			case JsonToken.Boolean:
				ValidationUtils.ArgumentNotNull(value, "value");
				this.WriteValue(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
				return;
			case JsonToken.Null:
				this.WriteNull();
				return;
			case JsonToken.Undefined:
				this.WriteUndefined();
				return;
			case JsonToken.EndObject:
				this.WriteEndObject();
				return;
			case JsonToken.EndArray:
				this.WriteEndArray();
				return;
			case JsonToken.EndConstructor:
				this.WriteEndConstructor();
				return;
			case JsonToken.Date:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (value is DateTimeOffset)
				{
					this.WriteValue((DateTimeOffset)value);
					return;
				}
				this.WriteValue(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
				return;
			case JsonToken.Bytes:
				ValidationUtils.ArgumentNotNull(value, "value");
				if (value is Guid)
				{
					this.WriteValue((Guid)value);
					return;
				}
				this.WriteValue((byte[])value);
				return;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("token", token, "Unexpected token type.");
			}
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x0004739C File Offset: 0x0004559C
		public void WriteToken(JsonToken token)
		{
			this.WriteToken(token, null);
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x000473A8 File Offset: 0x000455A8
		internal virtual void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments)
		{
			int num = this.CalculateWriteTokenInitialDepth(reader);
			do
			{
				if (writeDateConstructorAsDate && reader.TokenType == JsonToken.StartConstructor && string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
				{
					this.WriteConstructorDate(reader);
				}
				else if (writeComments || reader.TokenType != JsonToken.Comment)
				{
					this.WriteToken(reader.TokenType, reader.Value);
				}
			}
			while (num - 1 < reader.Depth - (JsonTokenUtils.IsEndToken(reader.TokenType) ? 1 : 0) && writeChildren && reader.Read());
			if (num < this.CalculateWriteTokenFinalDepth(reader))
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading token.", null);
			}
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00047470 File Offset: 0x00045670
		private int CalculateWriteTokenInitialDepth(JsonReader reader)
		{
			JsonToken tokenType = reader.TokenType;
			if (tokenType == JsonToken.None)
			{
				return -1;
			}
			if (!JsonTokenUtils.IsStartToken(tokenType))
			{
				return reader.Depth + 1;
			}
			return reader.Depth;
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x000474AC File Offset: 0x000456AC
		private int CalculateWriteTokenFinalDepth(JsonReader reader)
		{
			JsonToken tokenType = reader.TokenType;
			if (tokenType == JsonToken.None)
			{
				return -1;
			}
			if (!JsonTokenUtils.IsEndToken(tokenType))
			{
				return reader.Depth;
			}
			return reader.Depth - 1;
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x000474E8 File Offset: 0x000456E8
		private void WriteConstructorDate(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.Integer)
			{
				throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected Integer, got " + reader.TokenType, null);
			}
			DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime((long)reader.Value);
			if (!reader.Read())
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected EndConstructor, got " + reader.TokenType, null);
			}
			this.WriteValue(dateTime);
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00047594 File Offset: 0x00045794
		private void WriteEnd(JsonContainerType type)
		{
			switch (type)
			{
			case JsonContainerType.Object:
				this.WriteEndObject();
				return;
			case JsonContainerType.Array:
				this.WriteEndArray();
				return;
			case JsonContainerType.Constructor:
				this.WriteEndConstructor();
				return;
			default:
				throw JsonWriterException.Create(this, "Unexpected type when writing end: " + type, null);
			}
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x000475EC File Offset: 0x000457EC
		private void AutoCompleteAll()
		{
			while (this.Top > 0)
			{
				this.WriteEnd();
			}
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x00047604 File Offset: 0x00045804
		private JsonToken GetCloseTokenForType(JsonContainerType type)
		{
			switch (type)
			{
			case JsonContainerType.Object:
				return JsonToken.EndObject;
			case JsonContainerType.Array:
				return JsonToken.EndArray;
			case JsonContainerType.Constructor:
				return JsonToken.EndConstructor;
			default:
				throw JsonWriterException.Create(this, "No close token for type: " + type, null);
			}
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x00047640 File Offset: 0x00045840
		private void AutoCompleteClose(JsonContainerType type)
		{
			int num = this.CalculateLevelsToComplete(type);
			for (int i = 0; i < num; i++)
			{
				JsonToken closeTokenForType = this.GetCloseTokenForType(this.Pop());
				if (this._currentState == JsonWriter.State.Property)
				{
					this.WriteNull();
				}
				if (this._formatting == Formatting.Indented && this._currentState != JsonWriter.State.ObjectStart && this._currentState != JsonWriter.State.ArrayStart)
				{
					this.WriteIndent();
				}
				this.WriteEnd(closeTokenForType);
				this.UpdateCurrentState();
			}
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x000476C0 File Offset: 0x000458C0
		private int CalculateLevelsToComplete(JsonContainerType type)
		{
			int num = 0;
			if (this._currentPosition.Type == type)
			{
				num = 1;
			}
			else
			{
				int num2 = this.Top - 2;
				for (int i = num2; i >= 0; i--)
				{
					int num3 = num2 - i;
					if (this._stack[num3].Type == type)
					{
						num = i + 2;
						break;
					}
				}
			}
			if (num == 0)
			{
				throw JsonWriterException.Create(this, "No token to close.", null);
			}
			return num;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x0004773C File Offset: 0x0004593C
		private void UpdateCurrentState()
		{
			JsonContainerType jsonContainerType = this.Peek();
			switch (jsonContainerType)
			{
			case JsonContainerType.None:
				this._currentState = JsonWriter.State.Start;
				return;
			case JsonContainerType.Object:
				this._currentState = JsonWriter.State.Object;
				return;
			case JsonContainerType.Array:
				this._currentState = JsonWriter.State.Array;
				return;
			case JsonContainerType.Constructor:
				this._currentState = JsonWriter.State.Array;
				return;
			default:
				throw JsonWriterException.Create(this, "Unknown JsonType: " + jsonContainerType, null);
			}
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x000477A8 File Offset: 0x000459A8
		protected virtual void WriteEnd(JsonToken token)
		{
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x000477AC File Offset: 0x000459AC
		protected virtual void WriteIndent()
		{
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x000477B0 File Offset: 0x000459B0
		protected virtual void WriteValueDelimiter()
		{
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x000477B4 File Offset: 0x000459B4
		protected virtual void WriteIndentSpace()
		{
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x000477B8 File Offset: 0x000459B8
		internal void AutoComplete(JsonToken tokenBeingWritten)
		{
			JsonWriter.State state = JsonWriter.StateArray[(int)tokenBeingWritten][(int)this._currentState];
			if (state == JsonWriter.State.Error)
			{
				throw JsonWriterException.Create(this, "Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), this._currentState.ToString()), null);
			}
			if ((this._currentState == JsonWriter.State.Object || this._currentState == JsonWriter.State.Array || this._currentState == JsonWriter.State.Constructor) && tokenBeingWritten != JsonToken.Comment)
			{
				this.WriteValueDelimiter();
			}
			if (this._formatting == Formatting.Indented)
			{
				if (this._currentState == JsonWriter.State.Property)
				{
					this.WriteIndentSpace();
				}
				if (this._currentState == JsonWriter.State.Array || this._currentState == JsonWriter.State.ArrayStart || this._currentState == JsonWriter.State.Constructor || this._currentState == JsonWriter.State.ConstructorStart || (tokenBeingWritten == JsonToken.PropertyName && this._currentState != JsonWriter.State.Start))
				{
					this.WriteIndent();
				}
			}
			this._currentState = state;
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x000478B8 File Offset: 0x00045AB8
		public virtual void WriteNull()
		{
			this.InternalWriteValue(JsonToken.Null);
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x000478C4 File Offset: 0x00045AC4
		public virtual void WriteUndefined()
		{
			this.InternalWriteValue(JsonToken.Undefined);
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x000478D0 File Offset: 0x00045AD0
		public virtual void WriteRaw(string json)
		{
			this.InternalWriteRaw();
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x000478D8 File Offset: 0x00045AD8
		public virtual void WriteRawValue(string json)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(JsonToken.Undefined);
			this.WriteRaw(json);
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x000478F0 File Offset: 0x00045AF0
		public virtual void WriteValue(string value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x000478FC File Offset: 0x00045AFC
		public virtual void WriteValue(int value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x00047908 File Offset: 0x00045B08
		[CLSCompliant(false)]
		public virtual void WriteValue(uint value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x00047914 File Offset: 0x00045B14
		public virtual void WriteValue(long value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x00047920 File Offset: 0x00045B20
		[CLSCompliant(false)]
		public virtual void WriteValue(ulong value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x0004792C File Offset: 0x00045B2C
		public virtual void WriteValue(float value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x00047938 File Offset: 0x00045B38
		public virtual void WriteValue(double value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x00047944 File Offset: 0x00045B44
		public virtual void WriteValue(bool value)
		{
			this.InternalWriteValue(JsonToken.Boolean);
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x00047950 File Offset: 0x00045B50
		public virtual void WriteValue(short value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x0004795C File Offset: 0x00045B5C
		[CLSCompliant(false)]
		public virtual void WriteValue(ushort value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x00047968 File Offset: 0x00045B68
		public virtual void WriteValue(char value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x00047974 File Offset: 0x00045B74
		public virtual void WriteValue(byte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x00047980 File Offset: 0x00045B80
		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x0004798C File Offset: 0x00045B8C
		public virtual void WriteValue(decimal value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00047998 File Offset: 0x00045B98
		public virtual void WriteValue(DateTime value)
		{
			this.InternalWriteValue(JsonToken.Date);
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x000479A4 File Offset: 0x00045BA4
		public virtual void WriteValue(DateTimeOffset value)
		{
			this.InternalWriteValue(JsonToken.Date);
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x000479B0 File Offset: 0x00045BB0
		public virtual void WriteValue(Guid value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x000479BC File Offset: 0x00045BBC
		public virtual void WriteValue(TimeSpan value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x000479C8 File Offset: 0x00045BC8
		public virtual void WriteValue(int? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x000479EC File Offset: 0x00045BEC
		[CLSCompliant(false)]
		public virtual void WriteValue(uint? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x00047A10 File Offset: 0x00045C10
		public virtual void WriteValue(long? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x00047A34 File Offset: 0x00045C34
		[CLSCompliant(false)]
		public virtual void WriteValue(ulong? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x00047A58 File Offset: 0x00045C58
		public virtual void WriteValue(float? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x00047A7C File Offset: 0x00045C7C
		public virtual void WriteValue(double? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x00047AA0 File Offset: 0x00045CA0
		public virtual void WriteValue(bool? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x00047AC4 File Offset: 0x00045CC4
		public virtual void WriteValue(short? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x00047AE8 File Offset: 0x00045CE8
		[CLSCompliant(false)]
		public virtual void WriteValue(ushort? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x00047B0C File Offset: 0x00045D0C
		public virtual void WriteValue(char? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x00047B30 File Offset: 0x00045D30
		public virtual void WriteValue(byte? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x00047B54 File Offset: 0x00045D54
		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x00047B78 File Offset: 0x00045D78
		public virtual void WriteValue(decimal? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x00047B9C File Offset: 0x00045D9C
		public virtual void WriteValue(DateTime? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x00047BC0 File Offset: 0x00045DC0
		public virtual void WriteValue(DateTimeOffset? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x00047BE4 File Offset: 0x00045DE4
		public virtual void WriteValue(Guid? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x00047C08 File Offset: 0x00045E08
		public virtual void WriteValue(TimeSpan? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x00047C2C File Offset: 0x00045E2C
		public virtual void WriteValue(byte[] value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.InternalWriteValue(JsonToken.Bytes);
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x00047C44 File Offset: 0x00045E44
		public virtual void WriteValue(Uri value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.InternalWriteValue(JsonToken.String);
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x00047C64 File Offset: 0x00045E64
		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			if (value is BigInteger)
			{
				throw JsonWriter.CreateUnsupportedTypeException(this, value);
			}
			JsonWriter.WriteValue(this, ConvertUtils.GetTypeCode(value.GetType()), value);
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x00047C98 File Offset: 0x00045E98
		public virtual void WriteComment(string text)
		{
			this.InternalWriteComment();
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x00047CA0 File Offset: 0x00045EA0
		public virtual void WriteWhitespace(string ws)
		{
			this.InternalWriteWhitespace(ws);
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x00047CAC File Offset: 0x00045EAC
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x00047CBC File Offset: 0x00045EBC
		protected virtual void Dispose(bool disposing)
		{
			if (this._currentState != JsonWriter.State.Closed && disposing)
			{
				this.Close();
			}
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x00047CD8 File Offset: 0x00045ED8
		internal static void WriteValue(JsonWriter writer, PrimitiveTypeCode typeCode, object value)
		{
			switch (typeCode)
			{
			case PrimitiveTypeCode.Char:
				writer.WriteValue((char)value);
				return;
			case PrimitiveTypeCode.CharNullable:
				writer.WriteValue((value == null) ? null : new char?((char)value));
				return;
			case PrimitiveTypeCode.Boolean:
				writer.WriteValue((bool)value);
				return;
			case PrimitiveTypeCode.BooleanNullable:
				writer.WriteValue((value == null) ? null : new bool?((bool)value));
				return;
			case PrimitiveTypeCode.SByte:
				writer.WriteValue((sbyte)value);
				return;
			case PrimitiveTypeCode.SByteNullable:
				writer.WriteValue((value == null) ? null : new sbyte?((sbyte)value));
				return;
			case PrimitiveTypeCode.Int16:
				writer.WriteValue((short)value);
				return;
			case PrimitiveTypeCode.Int16Nullable:
				writer.WriteValue((value == null) ? null : new short?((short)value));
				return;
			case PrimitiveTypeCode.UInt16:
				writer.WriteValue((ushort)value);
				return;
			case PrimitiveTypeCode.UInt16Nullable:
				writer.WriteValue((value == null) ? null : new ushort?((ushort)value));
				return;
			case PrimitiveTypeCode.Int32:
				writer.WriteValue((int)value);
				return;
			case PrimitiveTypeCode.Int32Nullable:
				writer.WriteValue((value == null) ? null : new int?((int)value));
				return;
			case PrimitiveTypeCode.Byte:
				writer.WriteValue((byte)value);
				return;
			case PrimitiveTypeCode.ByteNullable:
				writer.WriteValue((value == null) ? null : new byte?((byte)value));
				return;
			case PrimitiveTypeCode.UInt32:
				writer.WriteValue((uint)value);
				return;
			case PrimitiveTypeCode.UInt32Nullable:
				writer.WriteValue((value == null) ? null : new uint?((uint)value));
				return;
			case PrimitiveTypeCode.Int64:
				writer.WriteValue((long)value);
				return;
			case PrimitiveTypeCode.Int64Nullable:
				writer.WriteValue((value == null) ? null : new long?((long)value));
				return;
			case PrimitiveTypeCode.UInt64:
				writer.WriteValue((ulong)value);
				return;
			case PrimitiveTypeCode.UInt64Nullable:
				writer.WriteValue((value == null) ? null : new ulong?((ulong)value));
				return;
			case PrimitiveTypeCode.Single:
				writer.WriteValue((float)value);
				return;
			case PrimitiveTypeCode.SingleNullable:
				writer.WriteValue((value == null) ? null : new float?((float)value));
				return;
			case PrimitiveTypeCode.Double:
				writer.WriteValue((double)value);
				return;
			case PrimitiveTypeCode.DoubleNullable:
				writer.WriteValue((value == null) ? null : new double?((double)value));
				return;
			case PrimitiveTypeCode.DateTime:
				writer.WriteValue((DateTime)value);
				return;
			case PrimitiveTypeCode.DateTimeNullable:
				writer.WriteValue((value == null) ? null : new DateTime?((DateTime)value));
				return;
			case PrimitiveTypeCode.DateTimeOffset:
				writer.WriteValue((DateTimeOffset)value);
				return;
			case PrimitiveTypeCode.DateTimeOffsetNullable:
				writer.WriteValue((value == null) ? null : new DateTimeOffset?((DateTimeOffset)value));
				return;
			case PrimitiveTypeCode.Decimal:
				writer.WriteValue((decimal)value);
				return;
			case PrimitiveTypeCode.DecimalNullable:
				writer.WriteValue((value == null) ? null : new decimal?((decimal)value));
				return;
			case PrimitiveTypeCode.Guid:
				writer.WriteValue((Guid)value);
				return;
			case PrimitiveTypeCode.GuidNullable:
				writer.WriteValue((value == null) ? null : new Guid?((Guid)value));
				return;
			case PrimitiveTypeCode.TimeSpan:
				writer.WriteValue((TimeSpan)value);
				return;
			case PrimitiveTypeCode.TimeSpanNullable:
				writer.WriteValue((value == null) ? null : new TimeSpan?((TimeSpan)value));
				return;
			case PrimitiveTypeCode.BigInteger:
				writer.WriteValue((BigInteger)value);
				return;
			case PrimitiveTypeCode.BigIntegerNullable:
				writer.WriteValue((value == null) ? null : new BigInteger?((BigInteger)value));
				return;
			case PrimitiveTypeCode.Uri:
				writer.WriteValue((Uri)value);
				return;
			case PrimitiveTypeCode.String:
				writer.WriteValue((string)value);
				return;
			case PrimitiveTypeCode.Bytes:
				writer.WriteValue((byte[])value);
				return;
			case PrimitiveTypeCode.DBNull:
				writer.WriteNull();
				return;
			default:
			{
				IConvertible convertible = value as IConvertible;
				if (convertible != null)
				{
					TypeInformation typeInformation = ConvertUtils.GetTypeInformation(convertible);
					PrimitiveTypeCode primitiveTypeCode = ((typeInformation.TypeCode == PrimitiveTypeCode.Object) ? PrimitiveTypeCode.String : typeInformation.TypeCode);
					Type type = ((typeInformation.TypeCode == PrimitiveTypeCode.Object) ? typeof(string) : typeInformation.Type);
					object obj = convertible.ToType(type, CultureInfo.InvariantCulture);
					JsonWriter.WriteValue(writer, primitiveTypeCode, obj);
					return;
				}
				throw JsonWriter.CreateUnsupportedTypeException(writer, value);
			}
			}
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x000481F0 File Offset: 0x000463F0
		private static JsonWriterException CreateUnsupportedTypeException(JsonWriter writer, object value)
		{
			return JsonWriterException.Create(writer, "Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), null);
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x00048210 File Offset: 0x00046410
		protected void SetWriteState(JsonToken token, object value)
		{
			switch (token)
			{
			case JsonToken.StartObject:
				this.InternalWriteStart(token, JsonContainerType.Object);
				return;
			case JsonToken.StartArray:
				this.InternalWriteStart(token, JsonContainerType.Array);
				return;
			case JsonToken.StartConstructor:
				this.InternalWriteStart(token, JsonContainerType.Constructor);
				return;
			case JsonToken.PropertyName:
				if (!(value is string))
				{
					throw new ArgumentException("A name is required when setting property name state.", "value");
				}
				this.InternalWritePropertyName((string)value);
				return;
			case JsonToken.Comment:
				this.InternalWriteComment();
				return;
			case JsonToken.Raw:
				this.InternalWriteRaw();
				return;
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Undefined:
			case JsonToken.Date:
			case JsonToken.Bytes:
				this.InternalWriteValue(token);
				return;
			case JsonToken.EndObject:
				this.InternalWriteEnd(JsonContainerType.Object);
				return;
			case JsonToken.EndArray:
				this.InternalWriteEnd(JsonContainerType.Array);
				return;
			case JsonToken.EndConstructor:
				this.InternalWriteEnd(JsonContainerType.Constructor);
				return;
			default:
				throw new ArgumentOutOfRangeException("token");
			}
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x000482F0 File Offset: 0x000464F0
		internal void InternalWriteEnd(JsonContainerType container)
		{
			this.AutoCompleteClose(container);
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x000482FC File Offset: 0x000464FC
		internal void InternalWritePropertyName(string name)
		{
			this._currentPosition.PropertyName = name;
			this.AutoComplete(JsonToken.PropertyName);
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x00048314 File Offset: 0x00046514
		internal void InternalWriteRaw()
		{
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x00048318 File Offset: 0x00046518
		internal void InternalWriteStart(JsonToken token, JsonContainerType container)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(token);
			this.Push(container);
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x00048330 File Offset: 0x00046530
		internal void InternalWriteValue(JsonToken token)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(token);
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x00048340 File Offset: 0x00046540
		internal void InternalWriteWhitespace(string ws)
		{
			if (ws != null && !StringUtils.IsWhiteSpace(ws))
			{
				throw JsonWriterException.Create(this, "Only white space characters should be used.", null);
			}
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x00048360 File Offset: 0x00046560
		internal void InternalWriteComment()
		{
			this.AutoComplete(JsonToken.Comment);
		}

		// Token: 0x04000432 RID: 1074
		private static readonly JsonWriter.State[][] StateArray = JsonWriter.BuildStateArray();

		// Token: 0x04000433 RID: 1075
		internal static readonly JsonWriter.State[][] StateArrayTempate = new JsonWriter.State[][]
		{
			new JsonWriter.State[]
			{
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			},
			new JsonWriter.State[]
			{
				JsonWriter.State.ObjectStart,
				JsonWriter.State.ObjectStart,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.ObjectStart,
				JsonWriter.State.ObjectStart,
				JsonWriter.State.ObjectStart,
				JsonWriter.State.ObjectStart,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			},
			new JsonWriter.State[]
			{
				JsonWriter.State.ArrayStart,
				JsonWriter.State.ArrayStart,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.ArrayStart,
				JsonWriter.State.ArrayStart,
				JsonWriter.State.ArrayStart,
				JsonWriter.State.ArrayStart,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			},
			new JsonWriter.State[]
			{
				JsonWriter.State.ConstructorStart,
				JsonWriter.State.ConstructorStart,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.ConstructorStart,
				JsonWriter.State.ConstructorStart,
				JsonWriter.State.ConstructorStart,
				JsonWriter.State.ConstructorStart,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			},
			new JsonWriter.State[]
			{
				JsonWriter.State.Property,
				JsonWriter.State.Error,
				JsonWriter.State.Property,
				JsonWriter.State.Property,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			},
			new JsonWriter.State[]
			{
				JsonWriter.State.Start,
				JsonWriter.State.Property,
				JsonWriter.State.ObjectStart,
				JsonWriter.State.Object,
				JsonWriter.State.ArrayStart,
				JsonWriter.State.Array,
				JsonWriter.State.Constructor,
				JsonWriter.State.Constructor,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			},
			new JsonWriter.State[]
			{
				JsonWriter.State.Start,
				JsonWriter.State.Property,
				JsonWriter.State.ObjectStart,
				JsonWriter.State.Object,
				JsonWriter.State.ArrayStart,
				JsonWriter.State.Array,
				JsonWriter.State.Constructor,
				JsonWriter.State.Constructor,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			},
			new JsonWriter.State[]
			{
				JsonWriter.State.Start,
				JsonWriter.State.Object,
				JsonWriter.State.Error,
				JsonWriter.State.Error,
				JsonWriter.State.Array,
				JsonWriter.State.Array,
				JsonWriter.State.Constructor,
				JsonWriter.State.Constructor,
				JsonWriter.State.Error,
				JsonWriter.State.Error
			}
		};

		// Token: 0x04000434 RID: 1076
		private List<JsonPosition> _stack;

		// Token: 0x04000435 RID: 1077
		private JsonPosition _currentPosition;

		// Token: 0x04000436 RID: 1078
		private JsonWriter.State _currentState;

		// Token: 0x04000437 RID: 1079
		private Formatting _formatting;

		// Token: 0x0400043A RID: 1082
		private DateFormatHandling _dateFormatHandling;

		// Token: 0x0400043B RID: 1083
		private DateTimeZoneHandling _dateTimeZoneHandling;

		// Token: 0x0400043C RID: 1084
		private StringEscapeHandling _stringEscapeHandling;

		// Token: 0x0400043D RID: 1085
		private FloatFormatHandling _floatFormatHandling;

		// Token: 0x0400043E RID: 1086
		private string _dateFormatString;

		// Token: 0x0400043F RID: 1087
		private CultureInfo _culture;

		// Token: 0x0200022D RID: 557
		internal enum State
		{
			// Token: 0x04000A59 RID: 2649
			Start,
			// Token: 0x04000A5A RID: 2650
			Property,
			// Token: 0x04000A5B RID: 2651
			ObjectStart,
			// Token: 0x04000A5C RID: 2652
			Object,
			// Token: 0x04000A5D RID: 2653
			ArrayStart,
			// Token: 0x04000A5E RID: 2654
			Array,
			// Token: 0x04000A5F RID: 2655
			ConstructorStart,
			// Token: 0x04000A60 RID: 2656
			Constructor,
			// Token: 0x04000A61 RID: 2657
			Closed,
			// Token: 0x04000A62 RID: 2658
			Error
		}
	}
}
