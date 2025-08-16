using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	// Token: 0x020000B5 RID: 181
	public class JsonTextReader : JsonReader, IJsonLineInfo
	{
		// Token: 0x0600091E RID: 2334 RVA: 0x0003D524 File Offset: 0x0003B724
		public override Task<bool> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsync(cancellationToken);
			}
			return this.DoReadAsync(cancellationToken);
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0003D540 File Offset: 0x0003B740
		internal Task<bool> DoReadAsync(CancellationToken cancellationToken)
		{
			this.EnsureBuffer();
			Task<bool> task;
			for (;;)
			{
				switch (this._currentState)
				{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
					goto IL_004C;
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
					goto IL_0054;
				case JsonReader.State.PostValue:
					task = this.ParsePostValueAsync(false, cancellationToken);
					if (task.Status != TaskStatus.RanToCompletion)
					{
						goto IL_007F;
					}
					if (task.Result)
					{
						goto Block_3;
					}
					continue;
				case JsonReader.State.Finished:
					goto IL_0088;
				}
				break;
			}
			goto IL_0090;
			IL_004C:
			return this.ParseValueAsync(cancellationToken);
			IL_0054:
			return this.ParseObjectAsync(cancellationToken);
			Block_3:
			return AsyncUtils.True;
			IL_007F:
			return this.DoReadAsync(task, cancellationToken);
			IL_0088:
			return this.ReadFromFinishedAsync(cancellationToken);
			IL_0090:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x0003D604 File Offset: 0x0003B804
		private async Task<bool> DoReadAsync(Task<bool> task, CancellationToken cancellationToken)
		{
			bool flag;
			if (await task.ConfigureAwait(false))
			{
				flag = true;
			}
			else
			{
				flag = await this.DoReadAsync(cancellationToken).ConfigureAwait(false);
			}
			return flag;
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x0003D660 File Offset: 0x0003B860
		private async Task<bool> ParsePostValueAsync(bool ignoreComments, CancellationToken cancellationToken)
		{
			char currentChar;
			for (;;)
			{
				currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c <= ')')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_0313;
							case '\r':
								await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
								continue;
							default:
								goto IL_0313;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (!(await this.ReadDataAsync(false, cancellationToken).ConfigureAwait(false)))
							{
								break;
							}
							continue;
						}
					}
					else if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_0313;
						}
						goto IL_01A7;
					}
					this._charPos++;
					continue;
				}
				if (c <= '/')
				{
					if (c == ',')
					{
						goto IL_025A;
					}
					if (c == '/')
					{
						await this.ParseCommentAsync(!ignoreComments, cancellationToken).ConfigureAwait(false);
						if (!ignoreComments)
						{
							goto Block_13;
						}
						continue;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_0189;
					}
					if (c == '}')
					{
						goto IL_016B;
					}
				}
				IL_0313:
				if (!char.IsWhiteSpace(currentChar))
				{
					goto IL_0336;
				}
				this._charPos++;
			}
			this._currentState = JsonReader.State.Finished;
			return false;
			IL_016B:
			this._charPos++;
			base.SetToken(JsonToken.EndObject);
			return true;
			IL_0189:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_01A7:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			Block_13:
			return true;
			IL_025A:
			this._charPos++;
			base.SetStateBasedOnCurrent();
			return false;
			IL_0336:
			throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x0003D6BC File Offset: 0x0003B8BC
		private async Task<bool> ReadFromFinishedAsync(CancellationToken cancellationToken)
		{
			bool flag;
			if (await this.EnsureCharsAsync(0, false, cancellationToken).ConfigureAwait(false))
			{
				await this.EatWhitespaceAsync(cancellationToken).ConfigureAwait(false);
				if (this._isEndOfFile)
				{
					base.SetToken(JsonToken.None);
					flag = false;
				}
				else
				{
					if (this._chars[this._charPos] != '/')
					{
						throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
					}
					await this.ParseCommentAsync(true, cancellationToken).ConfigureAwait(false);
					flag = true;
				}
			}
			else
			{
				base.SetToken(JsonToken.None);
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x0003D710 File Offset: 0x0003B910
		private Task<int> ReadDataAsync(bool append, CancellationToken cancellationToken)
		{
			return this.ReadDataAsync(append, 0, cancellationToken);
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x0003D71C File Offset: 0x0003B91C
		private async Task<int> ReadDataAsync(bool append, int charsRequired, CancellationToken cancellationToken)
		{
			int num;
			if (this._isEndOfFile)
			{
				num = 0;
			}
			else
			{
				this.PrepareBufferForReadData(append, charsRequired);
				int num2 = await this._reader.ReadAsync(this._chars, this._charsUsed, this._chars.Length - this._charsUsed - 1, cancellationToken).ConfigureAwait(false);
				this._charsUsed += num2;
				if (num2 == 0)
				{
					this._isEndOfFile = true;
				}
				this._chars[this._charsUsed] = '\0';
				num = num2;
			}
			return num;
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x0003D780 File Offset: 0x0003B980
		private async Task<bool> ParseValueAsync(CancellationToken cancellationToken)
		{
			char currentChar;
			for (;;)
			{
				currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c <= 'N')
				{
					if (c <= ' ')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_0A23;
							case '\r':
								await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
								continue;
							default:
								if (c != ' ')
								{
									goto IL_0A23;
								}
								break;
							}
							this._charPos++;
							continue;
						}
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (!(await this.ReadDataAsync(false, cancellationToken).ConfigureAwait(false)))
						{
							break;
						}
						continue;
					}
					else if (c <= '/')
					{
						if (c == '"')
						{
							goto IL_0202;
						}
						switch (c)
						{
						case '\'':
							goto IL_0202;
						case ')':
							goto IL_0967;
						case ',':
							goto IL_0957;
						case '-':
							goto IL_065F;
						case '/':
							goto IL_07FA;
						}
					}
					else
					{
						if (c == 'I')
						{
							goto IL_05DC;
						}
						if (c == 'N')
						{
							goto IL_0559;
						}
					}
				}
				else if (c <= 'f')
				{
					if (c == '[')
					{
						goto IL_091C;
					}
					if (c == ']')
					{
						goto IL_0939;
					}
					if (c == 'f')
					{
						goto IL_030B;
					}
				}
				else if (c <= 't')
				{
					if (c == 'n')
					{
						goto IL_038C;
					}
					if (c == 't')
					{
						goto IL_028A;
					}
				}
				else
				{
					if (c == 'u')
					{
						goto IL_087D;
					}
					if (c == '{')
					{
						goto IL_08FF;
					}
				}
				IL_0A23:
				if (!char.IsWhiteSpace(currentChar))
				{
					goto IL_0A46;
				}
				this._charPos++;
			}
			return false;
			IL_0202:
			await this.ParseStringAsync(currentChar, ReadType.Read, cancellationToken).ConfigureAwait(false);
			return true;
			IL_028A:
			await this.ParseTrueAsync(cancellationToken).ConfigureAwait(false);
			return true;
			IL_030B:
			await this.ParseFalseAsync(cancellationToken).ConfigureAwait(false);
			return true;
			IL_038C:
			if (await this.EnsureCharsAsync(1, true, cancellationToken).ConfigureAwait(false))
			{
				char c = this._chars[this._charPos + 1];
				if (c != 'e')
				{
					if (c != 'u')
					{
						throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
					}
					await this.ParseNullAsync(cancellationToken).ConfigureAwait(false);
				}
				else
				{
					await this.ParseConstructorAsync(cancellationToken).ConfigureAwait(false);
				}
				return true;
			}
			this._charPos++;
			throw base.CreateUnexpectedEndException();
			IL_0559:
			await this.ParseNumberNaNAsync(ReadType.Read, cancellationToken).ConfigureAwait(false);
			return true;
			IL_05DC:
			await this.ParseNumberPositiveInfinityAsync(ReadType.Read, cancellationToken).ConfigureAwait(false);
			return true;
			IL_065F:
			if (await this.EnsureCharsAsync(1, true, cancellationToken).ConfigureAwait(false) && this._chars[this._charPos + 1] == 'I')
			{
				await this.ParseNumberNegativeInfinityAsync(ReadType.Read, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				await this.ParseNumberAsync(ReadType.Read, cancellationToken).ConfigureAwait(false);
			}
			return true;
			IL_07FA:
			await this.ParseCommentAsync(true, cancellationToken).ConfigureAwait(false);
			return true;
			IL_087D:
			await this.ParseUndefinedAsync(cancellationToken).ConfigureAwait(false);
			return true;
			IL_08FF:
			this._charPos++;
			base.SetToken(JsonToken.StartObject);
			return true;
			IL_091C:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return true;
			IL_0939:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_0957:
			base.SetToken(JsonToken.Undefined);
			return true;
			IL_0967:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_0A46:
			if (!char.IsNumber(currentChar) && currentChar != '-' && currentChar != '.')
			{
				throw this.CreateUnexpectedCharacterException(currentChar);
			}
			this.ParseNumber(ReadType.Read);
			return true;
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x0003D7D4 File Offset: 0x0003B9D4
		private async Task ReadStringIntoBufferAsync(char quote, CancellationToken cancellationToken)
		{
			int charPos = this._charPos;
			int initialPosition = this._charPos;
			int lastWritePosition = this._charPos;
			this._stringBuffer.Position = 0;
			char currentChar;
			for (;;)
			{
				char[] chars = this._chars;
				int num = charPos;
				charPos = num + 1;
				char c = chars[num];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						if (c != '\n')
						{
							if (c == '\r')
							{
								this._charPos = charPos - 1;
								await this.ProcessCarriageReturnAsync(true, cancellationToken).ConfigureAwait(false);
								charPos = this._charPos;
							}
						}
						else
						{
							this._charPos = charPos - 1;
							this.ProcessLineFeed();
							charPos = this._charPos;
						}
					}
					else if (this._charsUsed == charPos - 1)
					{
						num = charPos;
						charPos = num - 1;
						if (!(await this.ReadDataAsync(true, cancellationToken).ConfigureAwait(false)))
						{
							break;
						}
					}
				}
				else if (c != '"' && c != '\'')
				{
					if (c == '\\')
					{
						this._charPos = charPos;
						if (!(await this.EnsureCharsAsync(0, true, cancellationToken).ConfigureAwait(false)))
						{
							goto Block_10;
						}
						int escapeStartPos = charPos - 1;
						currentChar = this._chars[charPos];
						num = charPos;
						charPos = num + 1;
						c = currentChar;
						char writeChar;
						if (c <= '\\')
						{
							if (c <= '\'')
							{
								if (c != '"' && c != '\'')
								{
									goto Block_14;
								}
							}
							else if (c != '/')
							{
								if (c != '\\')
								{
									goto Block_16;
								}
								writeChar = '\\';
								goto IL_0613;
							}
							writeChar = currentChar;
						}
						else if (c <= 'f')
						{
							if (c != 'b')
							{
								if (c != 'f')
								{
									goto Block_19;
								}
								writeChar = '\f';
							}
							else
							{
								writeChar = '\b';
							}
						}
						else
						{
							if (c != 'n')
							{
								switch (c)
								{
								case 'r':
									writeChar = '\r';
									goto IL_0613;
								case 't':
									writeChar = '\t';
									goto IL_0613;
								case 'u':
									this._charPos = charPos;
									c = await this.ParseUnicodeAsync(cancellationToken).ConfigureAwait(false);
									writeChar = c;
									if (StringUtils.IsLowSurrogate(writeChar))
									{
										writeChar = '\ufffd';
									}
									else if (StringUtils.IsHighSurrogate(writeChar))
									{
										bool anotherHighSurrogate;
										do
										{
											anotherHighSurrogate = false;
											if (await this.EnsureCharsAsync(2, true, cancellationToken).ConfigureAwait(false) && this._chars[this._charPos] == '\\' && this._chars[this._charPos + 1] == 'u')
											{
												char highSurrogate = writeChar;
												this._charPos += 2;
												c = await this.ParseUnicodeAsync(cancellationToken).ConfigureAwait(false);
												writeChar = c;
												if (!StringUtils.IsLowSurrogate(writeChar))
												{
													if (StringUtils.IsHighSurrogate(writeChar))
													{
														highSurrogate = '\ufffd';
														anotherHighSurrogate = true;
													}
													else
													{
														highSurrogate = '\ufffd';
													}
												}
												this.EnsureBufferNotEmpty();
												this.WriteCharToBuffer(highSurrogate, lastWritePosition, escapeStartPos);
												lastWritePosition = this._charPos;
											}
											else
											{
												writeChar = '\ufffd';
											}
										}
										while (anotherHighSurrogate);
									}
									charPos = this._charPos;
									goto IL_0613;
								}
								goto Block_21;
							}
							writeChar = '\n';
						}
						IL_0613:
						this.EnsureBufferNotEmpty();
						this.WriteCharToBuffer(writeChar, lastWritePosition, escapeStartPos);
						lastWritePosition = charPos;
					}
				}
				else if (this._chars[charPos - 1] == quote)
				{
					goto Block_28;
				}
			}
			this._charPos = charPos;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_10:
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_14:
			Block_16:
			Block_19:
			Block_21:
			this._charPos = charPos;
			throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + currentChar.ToString()));
			Block_28:
			this.FinishReadStringIntoBuffer(charPos - 1, initialPosition, lastWritePosition);
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x0003D830 File Offset: 0x0003BA30
		private Task ProcessCarriageReturnAsync(bool append, CancellationToken cancellationToken)
		{
			this._charPos++;
			Task<bool> task = this.EnsureCharsAsync(1, append, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				this.SetNewLine(task.Result);
				return AsyncUtils.CompletedTask;
			}
			return this.ProcessCarriageReturnAsync(task);
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x0003D880 File Offset: 0x0003BA80
		private async Task ProcessCarriageReturnAsync(Task<bool> task)
		{
			this.SetNewLine(await task.ConfigureAwait(false));
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0003D8D4 File Offset: 0x0003BAD4
		private async Task<char> ParseUnicodeAsync(CancellationToken cancellationToken)
		{
			return this.ConvertUnicode(await this.EnsureCharsAsync(4, true, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x0003D928 File Offset: 0x0003BB28
		private Task<bool> EnsureCharsAsync(int relativePosition, bool append, CancellationToken cancellationToken)
		{
			if (this._charPos + relativePosition < this._charsUsed)
			{
				return AsyncUtils.True;
			}
			if (this._isEndOfFile)
			{
				return AsyncUtils.False;
			}
			return this.ReadCharsAsync(relativePosition, append, cancellationToken);
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0003D960 File Offset: 0x0003BB60
		private async Task<bool> ReadCharsAsync(int relativePosition, bool append, CancellationToken cancellationToken)
		{
			int charsRequired = this._charPos + relativePosition - this._charsUsed + 1;
			for (;;)
			{
				int num = await this.ReadDataAsync(append, charsRequired, cancellationToken).ConfigureAwait(false);
				if (num == 0)
				{
					break;
				}
				charsRequired -= num;
				if (charsRequired <= 0)
				{
					goto Block_2;
				}
			}
			return false;
			Block_2:
			return true;
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x0003D9C4 File Offset: 0x0003BBC4
		private async Task<bool> ParseObjectAsync(CancellationToken cancellationToken)
		{
			for (;;)
			{
				char currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c <= '\r')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_0272;
						case '\r':
							await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
							continue;
						default:
							goto IL_0272;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (!(await this.ReadDataAsync(false, cancellationToken).ConfigureAwait(false)))
						{
							break;
						}
						continue;
					}
				}
				else if (c != ' ')
				{
					if (c == '/')
					{
						goto IL_0153;
					}
					if (c != '}')
					{
						goto IL_0272;
					}
					goto IL_0135;
				}
				this._charPos++;
				continue;
				IL_0272:
				if (!char.IsWhiteSpace(currentChar))
				{
					goto IL_0295;
				}
				this._charPos++;
			}
			return false;
			IL_0135:
			base.SetToken(JsonToken.EndObject);
			this._charPos++;
			return true;
			IL_0153:
			await this.ParseCommentAsync(true, cancellationToken).ConfigureAwait(false);
			return true;
			IL_0295:
			return await this.ParsePropertyAsync(cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x0003DA18 File Offset: 0x0003BC18
		private async Task ParseCommentAsync(bool setToken, CancellationToken cancellationToken)
		{
			this._charPos++;
			if (!(await this.EnsureCharsAsync(1, false, cancellationToken).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			bool singlelineComment;
			if (this._chars[this._charPos] == '*')
			{
				singlelineComment = false;
			}
			else
			{
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				singlelineComment = true;
			}
			this._charPos++;
			int initialPosition = this._charPos;
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c <= '\n')
				{
					if (c != '\0')
					{
						if (c == '\n')
						{
							if (singlelineComment)
							{
								goto Block_16;
							}
							this.ProcessLineFeed();
							continue;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (!(await this.ReadDataAsync(true, cancellationToken).ConfigureAwait(false)))
						{
							break;
						}
						continue;
					}
				}
				else if (c != '\r')
				{
					if (c == '*')
					{
						this._charPos++;
						if (!singlelineComment && await this.EnsureCharsAsync(0, true, cancellationToken).ConfigureAwait(false) && this._chars[this._charPos] == '/')
						{
							goto Block_14;
						}
						continue;
					}
				}
				else
				{
					if (singlelineComment)
					{
						goto Block_15;
					}
					await this.ProcessCarriageReturnAsync(true, cancellationToken).ConfigureAwait(false);
					continue;
				}
				this._charPos++;
			}
			if (!singlelineComment)
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			this.EndComment(setToken, initialPosition, this._charPos);
			return;
			Block_14:
			this.EndComment(setToken, initialPosition, this._charPos - 1);
			this._charPos++;
			return;
			Block_15:
			this.EndComment(setToken, initialPosition, this._charPos);
			return;
			Block_16:
			this.EndComment(setToken, initialPosition, this._charPos);
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x0003DA74 File Offset: 0x0003BC74
		private async Task EatWhitespaceAsync(CancellationToken cancellationToken)
		{
			for (;;)
			{
				char currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c != '\0')
				{
					if (c != '\n')
					{
						if (c != '\r')
						{
							if (currentChar != ' ' && !char.IsWhiteSpace(currentChar))
							{
								break;
							}
							this._charPos++;
						}
						else
						{
							await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
						}
					}
					else
					{
						this.ProcessLineFeed();
					}
				}
				else if (this._charsUsed == this._charPos)
				{
					if (!(await this.ReadDataAsync(false, cancellationToken).ConfigureAwait(false)))
					{
						break;
					}
				}
				else
				{
					this._charPos++;
				}
			}
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x0003DAC8 File Offset: 0x0003BCC8
		private async Task ParseStringAsync(char quote, ReadType readType, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			this._charPos++;
			this.ShiftBufferIfNeeded();
			await this.ReadStringIntoBufferAsync(quote, cancellationToken).ConfigureAwait(false);
			this.ParseReadString(quote, readType);
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x0003DB2C File Offset: 0x0003BD2C
		private async Task<bool> MatchValueAsync(string value, CancellationToken cancellationToken)
		{
			return this.MatchValue(await this.EnsureCharsAsync(value.Length - 1, true, cancellationToken).ConfigureAwait(false), value);
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x0003DB88 File Offset: 0x0003BD88
		private async Task<bool> MatchValueWithTrailingSeparatorAsync(string value, CancellationToken cancellationToken)
		{
			bool flag;
			if (!(await this.MatchValueAsync(value, cancellationToken).ConfigureAwait(false)))
			{
				flag = false;
			}
			else if (!(await this.EnsureCharsAsync(0, false, cancellationToken).ConfigureAwait(false)))
			{
				flag = true;
			}
			else
			{
				flag = this.IsSeparator(this._chars[this._charPos]) || this._chars[this._charPos] == '\0';
			}
			return flag;
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x0003DBE4 File Offset: 0x0003BDE4
		private async Task MatchAndSetAsync(string value, JsonToken newToken, object tokenValue, CancellationToken cancellationToken)
		{
			if (await this.MatchValueWithTrailingSeparatorAsync(value, cancellationToken).ConfigureAwait(false))
			{
				base.SetToken(newToken, tokenValue);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing " + newToken.ToString().ToLowerInvariant() + " value.");
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x0003DC50 File Offset: 0x0003BE50
		private Task ParseTrueAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.True, JsonToken.Boolean, true, cancellationToken);
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0003DC68 File Offset: 0x0003BE68
		private Task ParseFalseAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.False, JsonToken.Boolean, false, cancellationToken);
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x0003DC80 File Offset: 0x0003BE80
		private Task ParseNullAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.Null, JsonToken.Null, null, cancellationToken);
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x0003DC94 File Offset: 0x0003BE94
		private async Task ParseConstructorAsync(CancellationToken cancellationToken)
		{
			if (!(await this.MatchValueWithTrailingSeparatorAsync("new", cancellationToken).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
			}
			await this.EatWhitespaceAsync(cancellationToken).ConfigureAwait(false);
			int initialPosition = this._charPos;
			char currentChar;
			for (;;)
			{
				currentChar = this._chars[this._charPos];
				if (currentChar == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_01E6;
					}
					if (!(await this.ReadDataAsync(true, cancellationToken).ConfigureAwait(false)))
					{
						break;
					}
				}
				else
				{
					if (!char.IsLetterOrDigit(currentChar))
					{
						goto IL_0228;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
			IL_01E6:
			int endPosition = this._charPos;
			this._charPos++;
			goto IL_0352;
			IL_0228:
			if (currentChar == '\r')
			{
				endPosition = this._charPos;
				await this.ProcessCarriageReturnAsync(true, cancellationToken).ConfigureAwait(false);
			}
			else if (currentChar == '\n')
			{
				endPosition = this._charPos;
				this.ProcessLineFeed();
			}
			else if (char.IsWhiteSpace(currentChar))
			{
				endPosition = this._charPos;
				this._charPos++;
			}
			else
			{
				if (currentChar != '(')
				{
					throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
				}
				endPosition = this._charPos;
			}
			IL_0352:
			this._stringReference = new StringReference(this._chars, initialPosition, endPosition - initialPosition);
			string constructorName = this._stringReference.ToString();
			await this.EatWhitespaceAsync(cancellationToken).ConfigureAwait(false);
			if (this._chars[this._charPos] != '(')
			{
				throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			this.ClearRecentString();
			base.SetToken(JsonToken.StartConstructor, constructorName);
			constructorName = null;
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x0003DCE8 File Offset: 0x0003BEE8
		private async Task<object> ParseNumberNaNAsync(ReadType readType, CancellationToken cancellationToken)
		{
			return this.ParseNumberNaN(readType, await this.MatchValueWithTrailingSeparatorAsync(JsonConvert.NaN, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x0003DD44 File Offset: 0x0003BF44
		private async Task<object> ParseNumberPositiveInfinityAsync(ReadType readType, CancellationToken cancellationToken)
		{
			return this.ParseNumberPositiveInfinity(readType, await this.MatchValueWithTrailingSeparatorAsync(JsonConvert.PositiveInfinity, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0003DDA0 File Offset: 0x0003BFA0
		private async Task<object> ParseNumberNegativeInfinityAsync(ReadType readType, CancellationToken cancellationToken)
		{
			return this.ParseNumberNegativeInfinity(readType, await this.MatchValueWithTrailingSeparatorAsync(JsonConvert.NegativeInfinity, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x0003DDFC File Offset: 0x0003BFFC
		private async Task ParseNumberAsync(ReadType readType, CancellationToken cancellationToken)
		{
			this.ShiftBufferIfNeeded();
			char firstChar = this._chars[this._charPos];
			int initialPosition = this._charPos;
			await this.ReadNumberIntoBufferAsync(cancellationToken).ConfigureAwait(false);
			this.ParseReadNumber(readType, firstChar, initialPosition);
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x0003DE58 File Offset: 0x0003C058
		private Task ParseUndefinedAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.Undefined, JsonToken.Undefined, null, cancellationToken);
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x0003DE6C File Offset: 0x0003C06C
		private async Task<bool> ParsePropertyAsync(CancellationToken cancellationToken)
		{
			char firstChar = this._chars[this._charPos];
			char quoteChar;
			if (firstChar == '"' || firstChar == '\'')
			{
				this._charPos++;
				quoteChar = firstChar;
				this.ShiftBufferIfNeeded();
				await this.ReadStringIntoBufferAsync(quoteChar, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				if (!this.ValidIdentifierChar(firstChar))
				{
					throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				quoteChar = '\0';
				this.ShiftBufferIfNeeded();
				await this.ParseUnquotedPropertyAsync(cancellationToken).ConfigureAwait(false);
			}
			string propertyName;
			if (this.NameTable != null)
			{
				propertyName = this.NameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length) ?? this._stringReference.ToString();
			}
			else
			{
				propertyName = this._stringReference.ToString();
			}
			await this.EatWhitespaceAsync(cancellationToken).ConfigureAwait(false);
			if (this._chars[this._charPos] != ':')
			{
				throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			base.SetToken(JsonToken.PropertyName, propertyName);
			this._quoteChar = quoteChar;
			this.ClearRecentString();
			return true;
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0003DEC0 File Offset: 0x0003C0C0
		private async Task ReadNumberIntoBufferAsync(CancellationToken cancellationToken)
		{
			int charPos = this._charPos;
			for (;;)
			{
				char currentChar = this._chars[charPos];
				if (currentChar == '\0')
				{
					this._charPos = charPos;
					if (this._charsUsed != charPos || !(await this.ReadDataAsync(true, cancellationToken).ConfigureAwait(false)))
					{
						break;
					}
				}
				else
				{
					if (this.ReadNumberCharIntoBuffer(currentChar, charPos))
					{
						break;
					}
					charPos++;
				}
			}
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0003DF14 File Offset: 0x0003C114
		private async Task ParseUnquotedPropertyAsync(CancellationToken cancellationToken)
		{
			int initialPosition = this._charPos;
			for (;;)
			{
				char currentChar = this._chars[this._charPos];
				if (currentChar == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_00D6;
					}
					if (!(await this.ReadDataAsync(true, cancellationToken).ConfigureAwait(false)))
					{
						break;
					}
				}
				else if (this.ReadUnquotedPropertyReportIfDone(currentChar, initialPosition))
				{
					return;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
			IL_00D6:
			this._stringReference = new StringReference(this._chars, initialPosition, this._charPos - initialPosition);
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0003DF68 File Offset: 0x0003C168
		private async Task<bool> ReadNullCharAsync(CancellationToken cancellationToken)
		{
			if (this._charsUsed == this._charPos)
			{
				if (!(await this.ReadDataAsync(false, cancellationToken).ConfigureAwait(false)))
				{
					this._isEndOfFile = true;
					return true;
				}
			}
			else
			{
				this._charPos++;
			}
			return false;
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0003DFBC File Offset: 0x0003C1BC
		private async Task HandleNullAsync(CancellationToken cancellationToken)
		{
			if (!(await this.EnsureCharsAsync(1, true, cancellationToken).ConfigureAwait(false)))
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			if (this._chars[this._charPos + 1] == 'u')
			{
				await this.ParseNullAsync(cancellationToken).ConfigureAwait(false);
				return;
			}
			this._charPos += 2;
			throw this.CreateUnexpectedCharacterException(this._chars[this._charPos - 1]);
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x0003E010 File Offset: 0x0003C210
		private async Task ReadFinishedAsync(CancellationToken cancellationToken)
		{
			if (await this.EnsureCharsAsync(0, false, cancellationToken).ConfigureAwait(false))
			{
				await this.EatWhitespaceAsync(cancellationToken).ConfigureAwait(false);
				if (this._isEndOfFile)
				{
					base.SetToken(JsonToken.None);
					return;
				}
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				await this.ParseCommentAsync(false, cancellationToken).ConfigureAwait(false);
			}
			base.SetToken(JsonToken.None);
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x0003E064 File Offset: 0x0003C264
		private async Task<object> ReadStringValueAsync(ReadType readType, CancellationToken cancellationToken)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_09EA;
			case JsonReader.State.PostValue:
				if (await this.ParsePostValueAsync(true, cancellationToken))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				await this.ReadFinishedAsync(cancellationToken).ConfigureAwait(false);
				return null;
			default:
				goto IL_09EA;
			}
			char currentChar;
			string expected;
			for (;;)
			{
				currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c <= 'I')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								goto IL_095C;
							case '\v':
							case '\f':
								goto IL_0931;
							case '\r':
								await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
								goto IL_095C;
							default:
								goto IL_0931;
							}
						}
						else
						{
							if (await this.ReadNullCharAsync(cancellationToken).ConfigureAwait(false))
							{
								break;
							}
							goto IL_095C;
						}
					}
					else
					{
						switch (c)
						{
						case ' ':
							break;
						case '!':
						case '#':
						case '$':
						case '%':
						case '&':
						case '(':
						case ')':
						case '*':
						case '+':
							goto IL_0931;
						case '"':
						case '\'':
							goto IL_02B4;
						case ',':
							this.ProcessValueComma();
							goto IL_095C;
						case '-':
							goto IL_034C;
						case '.':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							goto IL_0481;
						case '/':
							await this.ParseCommentAsync(false, cancellationToken).ConfigureAwait(false);
							goto IL_095C;
						default:
							if (c != 'I')
							{
								goto IL_0931;
							}
							goto IL_062B;
						}
					}
					this._charPos++;
				}
				else if (c <= ']')
				{
					if (c == 'N')
					{
						goto IL_06B1;
					}
					if (c != ']')
					{
						goto IL_0931;
					}
					goto IL_0844;
				}
				else
				{
					if (c == 'f')
					{
						goto IL_052F;
					}
					if (c == 'n')
					{
						goto IL_0737;
					}
					if (c != 't')
					{
						goto IL_0931;
					}
					goto IL_052F;
				}
				IL_095C:
				expected = null;
				continue;
				IL_0931:
				this._charPos++;
				if (!char.IsWhiteSpace(currentChar))
				{
					goto Block_24;
				}
				goto IL_095C;
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_02B4:
			await this.ParseStringAsync(currentChar, readType, cancellationToken).ConfigureAwait(false);
			return this.FinishReadQuotedStringValue(readType);
			IL_034C:
			if (await this.EnsureCharsAsync(1, true, cancellationToken).ConfigureAwait(false) && this._chars[this._charPos + 1] == 'I')
			{
				return this.ParseNumberNegativeInfinity(readType);
			}
			await this.ParseNumberAsync(readType, cancellationToken).ConfigureAwait(false);
			return this.Value;
			IL_0481:
			if (readType != ReadType.ReadAsString)
			{
				this._charPos++;
				throw this.CreateUnexpectedCharacterException(currentChar);
			}
			await this.ParseNumberAsync(ReadType.ReadAsString, cancellationToken).ConfigureAwait(false);
			return this.Value;
			IL_052F:
			if (readType != ReadType.ReadAsString)
			{
				this._charPos++;
				throw this.CreateUnexpectedCharacterException(currentChar);
			}
			expected = ((currentChar == 't') ? JsonConvert.True : JsonConvert.False);
			if (!(await this.MatchValueWithTrailingSeparatorAsync(expected, cancellationToken).ConfigureAwait(false)))
			{
				throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
			}
			base.SetToken(JsonToken.String, expected);
			return expected;
			IL_062B:
			return await this.ParseNumberPositiveInfinityAsync(readType, cancellationToken).ConfigureAwait(false);
			IL_06B1:
			return await this.ParseNumberNaNAsync(readType, cancellationToken).ConfigureAwait(false);
			IL_0737:
			await this.HandleNullAsync(cancellationToken).ConfigureAwait(false);
			return null;
			IL_0844:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(currentChar);
			Block_24:
			throw this.CreateUnexpectedCharacterException(currentChar);
			IL_09EA:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x0003E0C0 File Offset: 0x0003C2C0
		private async Task<object> ReadNumberValueAsync(ReadType readType, CancellationToken cancellationToken)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_0913;
			case JsonReader.State.PostValue:
				if (await this.ParsePostValueAsync(true, cancellationToken))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				await this.ReadFinishedAsync(cancellationToken).ConfigureAwait(false);
				return null;
			default:
				goto IL_0913;
			}
			char currentChar;
			for (;;)
			{
				currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c <= '9')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_0866;
						case '\r':
							await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
							continue;
						default:
							switch (c)
							{
							case ' ':
								break;
							case '!':
							case '#':
							case '$':
							case '%':
							case '&':
							case '(':
							case ')':
							case '*':
							case '+':
								goto IL_0866;
							case '"':
							case '\'':
								goto IL_0294;
							case ',':
								this.ProcessValueComma();
								continue;
							case '-':
								goto IL_04B9;
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								goto IL_0661;
							case '/':
								await this.ParseCommentAsync(false, cancellationToken).ConfigureAwait(false);
								continue;
							default:
								goto IL_0866;
							}
							break;
						}
						this._charPos++;
						continue;
					}
					if (await this.ReadNullCharAsync(cancellationToken).ConfigureAwait(false))
					{
						break;
					}
					continue;
				}
				else if (c <= 'N')
				{
					if (c == 'I')
					{
						goto IL_0433;
					}
					if (c == 'N')
					{
						goto IL_03AD;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_0779;
					}
					if (c == 'n')
					{
						goto IL_032C;
					}
				}
				IL_0866:
				this._charPos++;
				if (!char.IsWhiteSpace(currentChar))
				{
					goto Block_17;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_0294:
			await this.ParseStringAsync(currentChar, readType, cancellationToken).ConfigureAwait(false);
			return this.FinishReadQuotedNumber(readType);
			IL_032C:
			await this.HandleNullAsync(cancellationToken).ConfigureAwait(false);
			return null;
			IL_03AD:
			return await this.ParseNumberNaNAsync(readType, cancellationToken).ConfigureAwait(false);
			IL_0433:
			return await this.ParseNumberPositiveInfinityAsync(readType, cancellationToken).ConfigureAwait(false);
			IL_04B9:
			if (await this.EnsureCharsAsync(1, true, cancellationToken).ConfigureAwait(false) && this._chars[this._charPos + 1] == 'I')
			{
				return await this.ParseNumberNegativeInfinityAsync(readType, cancellationToken).ConfigureAwait(false);
			}
			await this.ParseNumberAsync(readType, cancellationToken).ConfigureAwait(false);
			return this.Value;
			IL_0661:
			await this.ParseNumberAsync(readType, cancellationToken).ConfigureAwait(false);
			return this.Value;
			IL_0779:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(currentChar);
			Block_17:
			throw this.CreateUnexpectedCharacterException(currentChar);
			IL_0913:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x0003E11C File Offset: 0x0003C31C
		public override Task<bool?> ReadAsBooleanAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsBooleanAsync(cancellationToken);
			}
			return this.DoReadAsBooleanAsync(cancellationToken);
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x0003E138 File Offset: 0x0003C338
		internal async Task<bool?> DoReadAsBooleanAsync(CancellationToken cancellationToken)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_0792;
			case JsonReader.State.PostValue:
				if (await this.ParsePostValueAsync(true, cancellationToken))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				await this.ReadFinishedAsync(cancellationToken).ConfigureAwait(false);
				return null;
			default:
				goto IL_0792;
			}
			char currentChar;
			for (;;)
			{
				currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c <= '9')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_06E1;
						case '\r':
							await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
							continue;
						default:
							switch (c)
							{
							case ' ':
								break;
							case '!':
							case '#':
							case '$':
							case '%':
							case '&':
							case '(':
							case ')':
							case '*':
							case '+':
								goto IL_06E1;
							case '"':
							case '\'':
								goto IL_028E;
							case ',':
								this.ProcessValueComma();
								continue;
							case '-':
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								goto IL_03B2;
							case '/':
								await this.ParseCommentAsync(false, cancellationToken).ConfigureAwait(false);
								continue;
							default:
								goto IL_06E1;
							}
							break;
						}
						this._charPos++;
						continue;
					}
					if (await this.ReadNullCharAsync(cancellationToken).ConfigureAwait(false))
					{
						break;
					}
					continue;
				}
				else if (c <= 'f')
				{
					if (c == ']')
					{
						goto IL_05F0;
					}
					if (c == 'f')
					{
						goto IL_0485;
					}
				}
				else
				{
					if (c == 'n')
					{
						goto IL_032C;
					}
					if (c == 't')
					{
						goto IL_0485;
					}
				}
				IL_06E1:
				this._charPos++;
				if (!char.IsWhiteSpace(currentChar))
				{
					goto Block_18;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_028E:
			await this.ParseStringAsync(currentChar, ReadType.Read, cancellationToken).ConfigureAwait(false);
			return base.ReadBooleanString(this._stringReference.ToString());
			IL_032C:
			await this.HandleNullAsync(cancellationToken).ConfigureAwait(false);
			return null;
			IL_03B2:
			await this.ParseNumberAsync(ReadType.Read, cancellationToken).ConfigureAwait(false);
			bool flag = ((!(this.Value is BigInteger)) ? Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture) : ((BigInteger)this.Value != 0L));
			base.SetToken(JsonToken.Boolean, flag, false);
			return new bool?(flag);
			IL_0485:
			bool isTrue = currentChar == 't';
			if (!(await this.MatchValueWithTrailingSeparatorAsync(isTrue ? JsonConvert.True : JsonConvert.False, cancellationToken).ConfigureAwait(false)))
			{
				throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
			}
			base.SetToken(JsonToken.Boolean, isTrue);
			return new bool?(isTrue);
			IL_05F0:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(currentChar);
			Block_18:
			throw this.CreateUnexpectedCharacterException(currentChar);
			IL_0792:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0003E18C File Offset: 0x0003C38C
		public override Task<byte[]> ReadAsBytesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsBytesAsync(cancellationToken);
			}
			return this.DoReadAsBytesAsync(cancellationToken);
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x0003E1A8 File Offset: 0x0003C3A8
		internal async Task<byte[]> DoReadAsBytesAsync(CancellationToken cancellationToken)
		{
			this.EnsureBuffer();
			bool isWrapped = false;
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_0796;
			case JsonReader.State.PostValue:
				if (await this.ParsePostValueAsync(true, cancellationToken))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				await this.ReadFinishedAsync(cancellationToken).ConfigureAwait(false);
				return null;
			default:
				goto IL_0796;
			}
			char currentChar;
			byte[] data;
			for (;;)
			{
				currentChar = this._chars[this._charPos];
				char c = currentChar;
				if (c <= '\'')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								goto IL_0708;
							case '\v':
							case '\f':
								goto IL_06DD;
							case '\r':
								await this.ProcessCarriageReturnAsync(false, cancellationToken).ConfigureAwait(false);
								goto IL_0708;
							default:
								goto IL_06DD;
							}
						}
						else
						{
							if (await this.ReadNullCharAsync(cancellationToken).ConfigureAwait(false))
							{
								break;
							}
							goto IL_0708;
						}
					}
					else if (c != ' ')
					{
						if (c != '"' && c != '\'')
						{
							goto IL_06DD;
						}
						goto IL_0258;
					}
					this._charPos++;
				}
				else if (c <= '[')
				{
					if (c != ',')
					{
						if (c != '/')
						{
							if (c != '[')
							{
								goto IL_06DD;
							}
							goto IL_0451;
						}
						else
						{
							await this.ParseCommentAsync(false, cancellationToken).ConfigureAwait(false);
						}
					}
					else
					{
						this.ProcessValueComma();
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_05F1;
					}
					if (c == 'n')
					{
						goto IL_04E6;
					}
					if (c != '{')
					{
						goto IL_06DD;
					}
					this._charPos++;
					base.SetToken(JsonToken.StartObject);
					await this.ReadIntoWrappedTypeObjectAsync(cancellationToken).ConfigureAwait(false);
					isWrapped = true;
				}
				IL_0708:
				data = null;
				continue;
				IL_06DD:
				this._charPos++;
				if (!char.IsWhiteSpace(currentChar))
				{
					goto Block_22;
				}
				goto IL_0708;
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_0258:
			await this.ParseStringAsync(currentChar, ReadType.ReadAsBytes, cancellationToken).ConfigureAwait(false);
			data = (byte[])this.Value;
			if (isWrapped)
			{
				await base.ReaderReadAndAssertAsync(cancellationToken).ConfigureAwait(false);
				if (this.TokenType != JsonToken.EndObject)
				{
					throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
				}
				base.SetToken(JsonToken.Bytes, data, false);
			}
			return data;
			IL_0451:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return await base.ReadArrayIntoByteArrayAsync(cancellationToken).ConfigureAwait(false);
			IL_04E6:
			await this.HandleNullAsync(cancellationToken).ConfigureAwait(false);
			return null;
			IL_05F1:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(currentChar);
			Block_22:
			throw this.CreateUnexpectedCharacterException(currentChar);
			IL_0796:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0003E1FC File Offset: 0x0003C3FC
		private async Task ReadIntoWrappedTypeObjectAsync(CancellationToken cancellationToken)
		{
			await base.ReaderReadAndAssertAsync(cancellationToken).ConfigureAwait(false);
			if (this.Value != null && this.Value.ToString() == "$type")
			{
				await base.ReaderReadAndAssertAsync(cancellationToken).ConfigureAwait(false);
				if (this.Value != null && this.Value.ToString().StartsWith("System.Byte[]", StringComparison.Ordinal))
				{
					await base.ReaderReadAndAssertAsync(cancellationToken).ConfigureAwait(false);
					if (this.Value.ToString() == "$value")
					{
						return;
					}
				}
			}
			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0003E250 File Offset: 0x0003C450
		public override Task<DateTime?> ReadAsDateTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDateTimeAsync(cancellationToken);
			}
			return this.DoReadAsDateTimeAsync(cancellationToken);
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x0003E26C File Offset: 0x0003C46C
		internal async Task<DateTime?> DoReadAsDateTimeAsync(CancellationToken cancellationToken)
		{
			return (DateTime?)(await this.ReadStringValueAsync(ReadType.ReadAsDateTime, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x0003E2C0 File Offset: 0x0003C4C0
		public override Task<DateTimeOffset?> ReadAsDateTimeOffsetAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDateTimeOffsetAsync(cancellationToken);
			}
			return this.DoReadAsDateTimeOffsetAsync(cancellationToken);
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x0003E2DC File Offset: 0x0003C4DC
		internal async Task<DateTimeOffset?> DoReadAsDateTimeOffsetAsync(CancellationToken cancellationToken)
		{
			return (DateTimeOffset?)(await this.ReadStringValueAsync(ReadType.ReadAsDateTimeOffset, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0003E330 File Offset: 0x0003C530
		public override Task<decimal?> ReadAsDecimalAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDecimalAsync(cancellationToken);
			}
			return this.DoReadAsDecimalAsync(cancellationToken);
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0003E34C File Offset: 0x0003C54C
		internal async Task<decimal?> DoReadAsDecimalAsync(CancellationToken cancellationToken)
		{
			return (decimal?)(await this.ReadNumberValueAsync(ReadType.ReadAsDecimal, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0003E3A0 File Offset: 0x0003C5A0
		public override Task<double?> ReadAsDoubleAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDoubleAsync(cancellationToken);
			}
			return this.DoReadAsDoubleAsync(cancellationToken);
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0003E3BC File Offset: 0x0003C5BC
		internal async Task<double?> DoReadAsDoubleAsync(CancellationToken cancellationToken)
		{
			return (double?)(await this.ReadNumberValueAsync(ReadType.ReadAsDouble, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0003E410 File Offset: 0x0003C610
		public override Task<int?> ReadAsInt32Async(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsInt32Async(cancellationToken);
			}
			return this.DoReadAsInt32Async(cancellationToken);
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0003E42C File Offset: 0x0003C62C
		internal async Task<int?> DoReadAsInt32Async(CancellationToken cancellationToken)
		{
			return (int?)(await this.ReadNumberValueAsync(ReadType.ReadAsInt32, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x0003E480 File Offset: 0x0003C680
		public override Task<string> ReadAsStringAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsStringAsync(cancellationToken);
			}
			return this.DoReadAsStringAsync(cancellationToken);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0003E49C File Offset: 0x0003C69C
		internal async Task<string> DoReadAsStringAsync(CancellationToken cancellationToken)
		{
			return (string)(await this.ReadStringValueAsync(ReadType.ReadAsString, cancellationToken).ConfigureAwait(false));
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0003E4F0 File Offset: 0x0003C6F0
		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this._reader = reader;
			this._lineNumber = 1;
			this._safeAsync = base.GetType() == typeof(JsonTextReader);
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000956 RID: 2390 RVA: 0x0003E544 File Offset: 0x0003C744
		// (set) Token: 0x06000957 RID: 2391 RVA: 0x0003E54C File Offset: 0x0003C74C
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

		// Token: 0x06000958 RID: 2392 RVA: 0x0003E568 File Offset: 0x0003C768
		private void EnsureBufferNotEmpty()
		{
			if (this._stringBuffer.IsEmpty)
			{
				this._stringBuffer = new StringBuffer(this._arrayPool, 1024);
			}
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x0003E590 File Offset: 0x0003C790
		private void SetNewLine(bool hasNextChar)
		{
			if (hasNextChar && this._chars[this._charPos] == '\n')
			{
				this._charPos++;
			}
			this.OnNewLine(this._charPos);
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x0003E5C8 File Offset: 0x0003C7C8
		private void OnNewLine(int pos)
		{
			this._lineNumber++;
			this._lineStartPos = pos;
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0003E5E0 File Offset: 0x0003C7E0
		private void ParseString(char quote, ReadType readType)
		{
			this._charPos++;
			this.ShiftBufferIfNeeded();
			this.ReadStringIntoBuffer(quote);
			this.ParseReadString(quote, readType);
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x0003E608 File Offset: 0x0003C808
		private void ParseReadString(char quote, ReadType readType)
		{
			base.SetPostValueState(true);
			switch (readType)
			{
			case ReadType.ReadAsInt32:
			case ReadType.ReadAsDecimal:
			case ReadType.ReadAsBoolean:
				return;
			case ReadType.ReadAsBytes:
			{
				byte[] array;
				Guid guid;
				if (this._stringReference.Length == 0)
				{
					array = CollectionUtils.ArrayEmpty<byte>();
				}
				else if (this._stringReference.Length == 36 && ConvertUtils.TryConvertGuid(this._stringReference.ToString(), out guid))
				{
					array = guid.ToByteArray();
				}
				else
				{
					array = Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				}
				base.SetToken(JsonToken.Bytes, array, false);
				return;
			}
			case ReadType.ReadAsString:
			{
				string text = this._stringReference.ToString();
				base.SetToken(JsonToken.String, text, false);
				this._quoteChar = quote;
				return;
			}
			}
			if (this._dateParseHandling != DateParseHandling.None)
			{
				DateParseHandling dateParseHandling;
				if (readType == ReadType.ReadAsDateTime)
				{
					dateParseHandling = DateParseHandling.DateTime;
				}
				else if (readType == ReadType.ReadAsDateTimeOffset)
				{
					dateParseHandling = DateParseHandling.DateTimeOffset;
				}
				else
				{
					dateParseHandling = this._dateParseHandling;
				}
				DateTimeOffset dateTimeOffset;
				if (dateParseHandling == DateParseHandling.DateTime)
				{
					DateTime dateTime;
					if (DateTimeUtils.TryParseDateTime(this._stringReference, base.DateTimeZoneHandling, base.DateFormatString, base.Culture, out dateTime))
					{
						base.SetToken(JsonToken.Date, dateTime, false);
						return;
					}
				}
				else if (DateTimeUtils.TryParseDateTimeOffset(this._stringReference, base.DateFormatString, base.Culture, out dateTimeOffset))
				{
					base.SetToken(JsonToken.Date, dateTimeOffset, false);
					return;
				}
			}
			base.SetToken(JsonToken.String, this._stringReference.ToString(), false);
			this._quoteChar = quote;
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0003E7B4 File Offset: 0x0003C9B4
		private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0003E7C8 File Offset: 0x0003C9C8
		private void ShiftBufferIfNeeded()
		{
			int num = this._chars.Length;
			if ((double)(num - this._charPos) <= (double)num * 0.1)
			{
				int num2 = this._charsUsed - this._charPos;
				if (num2 > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num2;
				this._chars[this._charsUsed] = '\0';
			}
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x0003E85C File Offset: 0x0003CA5C
		private int ReadData(bool append)
		{
			return this.ReadData(append, 0);
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x0003E868 File Offset: 0x0003CA68
		private void PrepareBufferForReadData(bool append, int charsRequired)
		{
			if (this._charsUsed + charsRequired >= this._chars.Length - 1)
			{
				if (append)
				{
					int num = Math.Max(this._chars.Length * 2, this._charsUsed + charsRequired + 1);
					char[] array = BufferUtils.RentBuffer(this._arrayPool, num);
					JsonTextReader.BlockCopyChars(this._chars, 0, array, 0, this._chars.Length);
					BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
					this._chars = array;
					return;
				}
				int num2 = this._charsUsed - this._charPos;
				if (num2 + charsRequired + 1 >= this._chars.Length)
				{
					char[] array2 = BufferUtils.RentBuffer(this._arrayPool, num2 + charsRequired + 1);
					if (num2 > 0)
					{
						JsonTextReader.BlockCopyChars(this._chars, this._charPos, array2, 0, num2);
					}
					BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
					this._chars = array2;
				}
				else if (num2 > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num2;
			}
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0003E998 File Offset: 0x0003CB98
		private int ReadData(bool append, int charsRequired)
		{
			if (this._isEndOfFile)
			{
				return 0;
			}
			this.PrepareBufferForReadData(append, charsRequired);
			int num = this._chars.Length - this._charsUsed - 1;
			int num2 = this._reader.Read(this._chars, this._charsUsed, num);
			this._charsUsed += num2;
			if (num2 == 0)
			{
				this._isEndOfFile = true;
			}
			this._chars[this._charsUsed] = '\0';
			return num2;
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0003EA14 File Offset: 0x0003CC14
		private bool EnsureChars(int relativePosition, bool append)
		{
			return this._charPos + relativePosition < this._charsUsed || this.ReadChars(relativePosition, append);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0003EA34 File Offset: 0x0003CC34
		private bool ReadChars(int relativePosition, bool append)
		{
			if (this._isEndOfFile)
			{
				return false;
			}
			int num = this._charPos + relativePosition - this._charsUsed + 1;
			int num2 = 0;
			do
			{
				int num3 = this.ReadData(append, num - num2);
				if (num3 == 0)
				{
					break;
				}
				num2 += num3;
			}
			while (num2 < num);
			return num2 >= num;
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x0003EA8C File Offset: 0x0003CC8C
		public override bool Read()
		{
			this.EnsureBuffer();
			for (;;)
			{
				switch (this._currentState)
				{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
					goto IL_004C;
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
					goto IL_0053;
				case JsonReader.State.PostValue:
					if (this.ParsePostValue(false))
					{
						return true;
					}
					continue;
				case JsonReader.State.Finished:
					goto IL_0065;
				}
				break;
			}
			goto IL_00DA;
			IL_004C:
			return this.ParseValue();
			IL_0053:
			return this.ParseObject();
			IL_0065:
			if (!this.EnsureChars(0, false))
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			this.EatWhitespace();
			if (this._isEndOfFile)
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			if (this._chars[this._charPos] == '/')
			{
				this.ParseComment(true);
				return true;
			}
			throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			IL_00DA:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0003EB98 File Offset: 0x0003CD98
		public override int? ReadAsInt32()
		{
			return (int?)this.ReadNumberValue(ReadType.ReadAsInt32);
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0003EBA8 File Offset: 0x0003CDA8
		public override DateTime? ReadAsDateTime()
		{
			return (DateTime?)this.ReadStringValue(ReadType.ReadAsDateTime);
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0003EBB8 File Offset: 0x0003CDB8
		public override string ReadAsString()
		{
			return (string)this.ReadStringValue(ReadType.ReadAsString);
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0003EBC8 File Offset: 0x0003CDC8
		public override byte[] ReadAsBytes()
		{
			this.EnsureBuffer();
			bool flag = false;
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_0265;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_0265;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= '\'')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_023C;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_023C;
							}
						}
						else
						{
							if (this.ReadNullChar())
							{
								break;
							}
							continue;
						}
					}
					else if (c != ' ')
					{
						if (c != '"' && c != '\'')
						{
							goto IL_023C;
						}
						goto IL_0117;
					}
					this._charPos++;
					continue;
				}
				if (c <= '[')
				{
					if (c == ',')
					{
						this.ProcessValueComma();
						continue;
					}
					if (c == '/')
					{
						this.ParseComment(false);
						continue;
					}
					if (c == '[')
					{
						goto IL_0193;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_01CE;
					}
					if (c == 'n')
					{
						goto IL_01AF;
					}
					if (c == '{')
					{
						this._charPos++;
						base.SetToken(JsonToken.StartObject);
						base.ReadIntoWrappedTypeObject();
						flag = true;
						continue;
					}
				}
				IL_023C:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_22;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_0117:
			this.ParseString(c, ReadType.ReadAsBytes);
			byte[] array = (byte[])this.Value;
			if (flag)
			{
				base.ReaderReadAndAssert();
				if (this.TokenType != JsonToken.EndObject)
				{
					throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
				}
				base.SetToken(JsonToken.Bytes, array, false);
			}
			return array;
			IL_0193:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return base.ReadArrayIntoByteArray();
			IL_01AF:
			this.HandleNull();
			return null;
			IL_01CE:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_22:
			throw this.CreateUnexpectedCharacterException(c);
			IL_0265:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0003EE60 File Offset: 0x0003D060
		private object ReadStringValue(ReadType readType)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_0308;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_0308;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= 'I')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_02DF;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_02DF;
							}
						}
						else
						{
							if (this.ReadNullChar())
							{
								break;
							}
							continue;
						}
					}
					else
					{
						switch (c)
						{
						case ' ':
							break;
						case '!':
						case '#':
						case '$':
						case '%':
						case '&':
						case '(':
						case ')':
						case '*':
						case '+':
							goto IL_02DF;
						case '"':
						case '\'':
							goto IL_016E;
						case ',':
							this.ProcessValueComma();
							continue;
						case '-':
							goto IL_017E;
						case '.':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							goto IL_01B7;
						case '/':
							this.ParseComment(false);
							continue;
						default:
							if (c != 'I')
							{
								goto IL_02DF;
							}
							goto IL_0242;
						}
					}
					this._charPos++;
					continue;
				}
				if (c <= ']')
				{
					if (c == 'N')
					{
						goto IL_024A;
					}
					if (c == ']')
					{
						goto IL_0271;
					}
				}
				else
				{
					if (c == 'f')
					{
						goto IL_01E2;
					}
					if (c == 'n')
					{
						goto IL_0252;
					}
					if (c == 't')
					{
						goto IL_01E2;
					}
				}
				IL_02DF:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_24;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_016E:
			this.ParseString(c, readType);
			return this.FinishReadQuotedStringValue(readType);
			IL_017E:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				return this.ParseNumberNegativeInfinity(readType);
			}
			this.ParseNumber(readType);
			return this.Value;
			IL_01B7:
			if (readType != ReadType.ReadAsString)
			{
				this._charPos++;
				throw this.CreateUnexpectedCharacterException(c);
			}
			this.ParseNumber(ReadType.ReadAsString);
			return this.Value;
			IL_01E2:
			if (readType != ReadType.ReadAsString)
			{
				this._charPos++;
				throw this.CreateUnexpectedCharacterException(c);
			}
			string text = ((c == 't') ? JsonConvert.True : JsonConvert.False);
			if (!this.MatchValueWithTrailingSeparator(text))
			{
				throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
			}
			base.SetToken(JsonToken.String, text);
			return text;
			IL_0242:
			return this.ParseNumberPositiveInfinity(readType);
			IL_024A:
			return this.ParseNumberNaN(readType);
			IL_0252:
			this.HandleNull();
			return null;
			IL_0271:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_24:
			throw this.CreateUnexpectedCharacterException(c);
			IL_0308:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0003F19C File Offset: 0x0003D39C
		private object FinishReadQuotedStringValue(ReadType readType)
		{
			switch (readType)
			{
			case ReadType.ReadAsBytes:
			case ReadType.ReadAsString:
				return this.Value;
			case ReadType.ReadAsDateTime:
				if (this.Value is DateTime)
				{
					return (DateTime)this.Value;
				}
				return base.ReadDateTimeString((string)this.Value);
			case ReadType.ReadAsDateTimeOffset:
				if (this.Value is DateTimeOffset)
				{
					return (DateTimeOffset)this.Value;
				}
				return base.ReadDateTimeOffsetString((string)this.Value);
			}
			throw new ArgumentOutOfRangeException("readType");
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0003F250 File Offset: 0x0003D450
		private JsonReaderException CreateUnexpectedCharacterException(char c)
		{
			return JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0003F270 File Offset: 0x0003D470
		public override bool? ReadAsBoolean()
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_02FC;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_02FC;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= '9')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_02CB;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							switch (c)
							{
							case ' ':
								break;
							case '!':
							case '#':
							case '$':
							case '%':
							case '&':
							case '(':
							case ')':
							case '*':
							case '+':
								goto IL_02CB;
							case '"':
							case '\'':
								goto IL_0161;
							case ',':
								this.ProcessValueComma();
								continue;
							case '-':
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								goto IL_0191;
							case '/':
								this.ParseComment(false);
								continue;
							default:
								goto IL_02CB;
							}
							break;
						}
						this._charPos++;
						continue;
					}
					if (this.ReadNullChar())
					{
						break;
					}
					continue;
				}
				else if (c <= 'f')
				{
					if (c == ']')
					{
						goto IL_0255;
					}
					if (c == 'f')
					{
						goto IL_01E7;
					}
				}
				else
				{
					if (c == 'n')
					{
						goto IL_0181;
					}
					if (c == 't')
					{
						goto IL_01E7;
					}
				}
				IL_02CB:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_18;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_0161:
			this.ParseString(c, ReadType.Read);
			return base.ReadBooleanString(this._stringReference.ToString());
			IL_0181:
			this.HandleNull();
			return null;
			IL_0191:
			this.ParseNumber(ReadType.Read);
			bool flag;
			if (this.Value is BigInteger)
			{
				flag = (BigInteger)this.Value != 0L;
			}
			else
			{
				flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
			}
			base.SetToken(JsonToken.Boolean, flag, false);
			return new bool?(flag);
			IL_01E7:
			bool flag2 = c == 't';
			string text = (flag2 ? JsonConvert.True : JsonConvert.False);
			if (!this.MatchValueWithTrailingSeparator(text))
			{
				throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
			}
			base.SetToken(JsonToken.Boolean, flag2);
			return new bool?(flag2);
			IL_0255:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_18:
			throw this.CreateUnexpectedCharacterException(c);
			IL_02FC:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0003F5A0 File Offset: 0x0003D7A0
		private void ProcessValueComma()
		{
			this._charPos++;
			if (this._currentState != JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.Undefined);
				object obj = this.CreateUnexpectedCharacterException(',');
				this._charPos--;
				throw obj;
			}
			base.SetStateBasedOnCurrent();
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0003F5F0 File Offset: 0x0003D7F0
		private object ReadNumberValue(ReadType readType)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_026E;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_026E;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= '9')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_0245;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							switch (c)
							{
							case ' ':
								break;
							case '!':
							case '#':
							case '$':
							case '%':
							case '&':
							case '(':
							case ')':
							case '*':
							case '+':
								goto IL_0245;
							case '"':
							case '\'':
								goto IL_0151;
							case ',':
								this.ProcessValueComma();
								continue;
							case '-':
								goto IL_0179;
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								goto IL_01B2;
							case '/':
								this.ParseComment(false);
								continue;
							default:
								goto IL_0245;
							}
							break;
						}
						this._charPos++;
						continue;
					}
					if (this.ReadNullChar())
					{
						break;
					}
					continue;
				}
				else if (c <= 'N')
				{
					if (c == 'I')
					{
						goto IL_0171;
					}
					if (c == 'N')
					{
						goto IL_0169;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_01D7;
					}
					if (c == 'n')
					{
						goto IL_0161;
					}
				}
				IL_0245:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_17;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_0151:
			this.ParseString(c, readType);
			return this.FinishReadQuotedNumber(readType);
			IL_0161:
			this.HandleNull();
			return null;
			IL_0169:
			return this.ParseNumberNaN(readType);
			IL_0171:
			return this.ParseNumberPositiveInfinity(readType);
			IL_0179:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				return this.ParseNumberNegativeInfinity(readType);
			}
			this.ParseNumber(readType);
			return this.Value;
			IL_01B2:
			this.ParseNumber(readType);
			return this.Value;
			IL_01D7:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_17:
			throw this.CreateUnexpectedCharacterException(c);
			IL_026E:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0003F890 File Offset: 0x0003DA90
		private object FinishReadQuotedNumber(ReadType readType)
		{
			if (readType == ReadType.ReadAsInt32)
			{
				return base.ReadInt32String(this._stringReference.ToString());
			}
			if (readType == ReadType.ReadAsDecimal)
			{
				return base.ReadDecimalString(this._stringReference.ToString());
			}
			if (readType != ReadType.ReadAsDouble)
			{
				throw new ArgumentOutOfRangeException("readType");
			}
			return base.ReadDoubleString(this._stringReference.ToString());
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0003F91C File Offset: 0x0003DB1C
		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			return (DateTimeOffset?)this.ReadStringValue(ReadType.ReadAsDateTimeOffset);
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x0003F92C File Offset: 0x0003DB2C
		public override decimal? ReadAsDecimal()
		{
			return (decimal?)this.ReadNumberValue(ReadType.ReadAsDecimal);
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x0003F93C File Offset: 0x0003DB3C
		public override double? ReadAsDouble()
		{
			return (double?)this.ReadNumberValue(ReadType.ReadAsDouble);
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0003F94C File Offset: 0x0003DB4C
		private void HandleNull()
		{
			if (!this.EnsureChars(1, true))
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			if (this._chars[this._charPos + 1] == 'u')
			{
				this.ParseNull();
				return;
			}
			this._charPos += 2;
			throw this.CreateUnexpectedCharacterException(this._chars[this._charPos - 1]);
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0003F9C0 File Offset: 0x0003DBC0
		private void ReadFinished()
		{
			if (this.EnsureChars(0, false))
			{
				this.EatWhitespace();
				if (this._isEndOfFile)
				{
					return;
				}
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				this.ParseComment(false);
			}
			base.SetToken(JsonToken.None);
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0003FA40 File Offset: 0x0003DC40
		private bool ReadNullChar()
		{
			if (this._charsUsed == this._charPos)
			{
				if (this.ReadData(false) == 0)
				{
					this._isEndOfFile = true;
					return true;
				}
			}
			else
			{
				this._charPos++;
			}
			return false;
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0003FA78 File Offset: 0x0003DC78
		private void EnsureBuffer()
		{
			if (this._chars == null)
			{
				this._chars = BufferUtils.RentBuffer(this._arrayPool, 1024);
				this._chars[0] = '\0';
			}
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0003FAA4 File Offset: 0x0003DCA4
		private void ReadStringIntoBuffer(char quote)
		{
			int num = this._charPos;
			int charPos = this._charPos;
			int num2 = this._charPos;
			this._stringBuffer.Position = 0;
			char c2;
			for (;;)
			{
				char c = this._chars[num++];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						if (c != '\n')
						{
							if (c == '\r')
							{
								this._charPos = num - 1;
								this.ProcessCarriageReturn(true);
								num = this._charPos;
							}
						}
						else
						{
							this._charPos = num - 1;
							this.ProcessLineFeed();
							num = this._charPos;
						}
					}
					else if (this._charsUsed == num - 1)
					{
						num--;
						if (this.ReadData(true) == 0)
						{
							break;
						}
					}
				}
				else if (c != '"' && c != '\'')
				{
					if (c == '\\')
					{
						this._charPos = num;
						if (!this.EnsureChars(0, true))
						{
							goto Block_10;
						}
						int num3 = num - 1;
						c2 = this._chars[num];
						num++;
						char c3;
						if (c2 <= '\\')
						{
							if (c2 <= '\'')
							{
								if (c2 != '"' && c2 != '\'')
								{
									goto Block_14;
								}
							}
							else if (c2 != '/')
							{
								if (c2 != '\\')
								{
									goto Block_16;
								}
								c3 = '\\';
								goto IL_02C6;
							}
							c3 = c2;
						}
						else if (c2 <= 'f')
						{
							if (c2 != 'b')
							{
								if (c2 != 'f')
								{
									goto Block_19;
								}
								c3 = '\f';
							}
							else
							{
								c3 = '\b';
							}
						}
						else
						{
							if (c2 != 'n')
							{
								switch (c2)
								{
								case 'r':
									c3 = '\r';
									goto IL_02C6;
								case 't':
									c3 = '\t';
									goto IL_02C6;
								case 'u':
									this._charPos = num;
									c3 = this.ParseUnicode();
									if (StringUtils.IsLowSurrogate(c3))
									{
										c3 = '\ufffd';
									}
									else if (StringUtils.IsHighSurrogate(c3))
									{
										bool flag;
										do
										{
											flag = false;
											if (this.EnsureChars(2, true) && this._chars[this._charPos] == '\\' && this._chars[this._charPos + 1] == 'u')
											{
												char c4 = c3;
												this._charPos += 2;
												c3 = this.ParseUnicode();
												if (!StringUtils.IsLowSurrogate(c3))
												{
													if (StringUtils.IsHighSurrogate(c3))
													{
														c4 = '\ufffd';
														flag = true;
													}
													else
													{
														c4 = '\ufffd';
													}
												}
												this.EnsureBufferNotEmpty();
												this.WriteCharToBuffer(c4, num2, num3);
												num2 = this._charPos;
											}
											else
											{
												c3 = '\ufffd';
											}
										}
										while (flag);
									}
									num = this._charPos;
									goto IL_02C6;
								}
								goto Block_21;
							}
							c3 = '\n';
						}
						IL_02C6:
						this.EnsureBufferNotEmpty();
						this.WriteCharToBuffer(c3, num2, num3);
						num2 = num;
					}
				}
				else if (this._chars[num - 1] == quote)
				{
					goto Block_28;
				}
			}
			this._charPos = num;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_10:
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_14:
			Block_16:
			Block_19:
			Block_21:
			this._charPos = num;
			throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + c2.ToString()));
			Block_28:
			this.FinishReadStringIntoBuffer(num - 1, charPos, num2);
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0003FDE8 File Offset: 0x0003DFE8
		private void FinishReadStringIntoBuffer(int charPos, int initialPosition, int lastWritePosition)
		{
			if (initialPosition == lastWritePosition)
			{
				this._stringReference = new StringReference(this._chars, initialPosition, charPos - initialPosition);
			}
			else
			{
				this.EnsureBufferNotEmpty();
				if (charPos > lastWritePosition)
				{
					this._stringBuffer.Append(this._arrayPool, this._chars, lastWritePosition, charPos - lastWritePosition);
				}
				this._stringReference = new StringReference(this._stringBuffer.InternalBuffer, 0, this._stringBuffer.Position);
			}
			this._charPos = charPos + 1;
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0003FE70 File Offset: 0x0003E070
		private void WriteCharToBuffer(char writeChar, int lastWritePosition, int writeToPosition)
		{
			if (writeToPosition > lastWritePosition)
			{
				this._stringBuffer.Append(this._arrayPool, this._chars, lastWritePosition, writeToPosition - lastWritePosition);
			}
			this._stringBuffer.Append(this._arrayPool, writeChar);
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0003FEA8 File Offset: 0x0003E0A8
		private char ConvertUnicode(bool enoughChars)
		{
			if (!enoughChars)
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing Unicode escape sequence.");
			}
			int num;
			if (ConvertUtils.TryHexTextToInt(this._chars, this._charPos, this._charPos + 4, out num))
			{
				char c = Convert.ToChar(num);
				this._charPos += 4;
				return c;
			}
			throw JsonReaderException.Create(this, "Invalid Unicode escape sequence: \\u{0}.".FormatWith(CultureInfo.InvariantCulture, new string(this._chars, this._charPos, 4)));
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0003FF28 File Offset: 0x0003E128
		private char ParseUnicode()
		{
			return this.ConvertUnicode(this.EnsureChars(4, true));
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x0003FF38 File Offset: 0x0003E138
		private void ReadNumberIntoBuffer()
		{
			int num = this._charPos;
			for (;;)
			{
				char c = this._chars[num];
				if (c == '\0')
				{
					this._charPos = num;
					if (this._charsUsed != num)
					{
						return;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					if (this.ReadNumberCharIntoBuffer(c, num))
					{
						return;
					}
					num++;
				}
			}
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0003FF90 File Offset: 0x0003E190
		private bool ReadNumberCharIntoBuffer(char currentChar, int charPos)
		{
			if (currentChar <= 'X')
			{
				switch (currentChar)
				{
				case '+':
				case '-':
				case '.':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
					break;
				case ',':
				case '/':
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '@':
					goto IL_00B9;
				default:
					if (currentChar != 'X')
					{
						goto IL_00B9;
					}
					break;
				}
			}
			else
			{
				switch (currentChar)
				{
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					break;
				default:
					if (currentChar != 'x')
					{
						goto IL_00B9;
					}
					break;
				}
			}
			return false;
			IL_00B9:
			this._charPos = charPos;
			if (char.IsWhiteSpace(currentChar) || currentChar == ',' || currentChar == '}' || currentChar == ']' || currentChar == ')' || currentChar == '/')
			{
				return true;
			}
			throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x000400B4 File Offset: 0x0003E2B4
		private void ClearRecentString()
		{
			this._stringBuffer.Position = 0;
			this._stringReference = default(StringReference);
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x000400D0 File Offset: 0x0003E2D0
		private bool ParsePostValue(bool ignoreComments)
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= ')')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_0161;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_0161;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (this.ReadData(false) == 0)
							{
								break;
							}
							continue;
						}
					}
					else if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_0161;
						}
						goto IL_00F7;
					}
					this._charPos++;
					continue;
				}
				if (c <= '/')
				{
					if (c == ',')
					{
						goto IL_0121;
					}
					if (c == '/')
					{
						this.ParseComment(!ignoreComments);
						if (!ignoreComments)
						{
							return true;
						}
						continue;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_00DF;
					}
					if (c == '}')
					{
						goto IL_00C7;
					}
				}
				IL_0161:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_017F;
				}
				this._charPos++;
			}
			this._currentState = JsonReader.State.Finished;
			return false;
			IL_00C7:
			this._charPos++;
			base.SetToken(JsonToken.EndObject);
			return true;
			IL_00DF:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_00F7:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_0121:
			this._charPos++;
			base.SetStateBasedOnCurrent();
			return false;
			IL_017F:
			throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x0004027C File Offset: 0x0003E47C
		private bool ParseObject()
		{
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_00D5;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							goto IL_00D5;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c != ' ')
				{
					if (c == '/')
					{
						goto IL_00A2;
					}
					if (c != '}')
					{
						goto IL_00D5;
					}
					goto IL_008A;
				}
				this._charPos++;
				continue;
				IL_00D5:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_00F3;
				}
				this._charPos++;
			}
			return false;
			IL_008A:
			base.SetToken(JsonToken.EndObject);
			this._charPos++;
			return true;
			IL_00A2:
			this.ParseComment(true);
			return true;
			IL_00F3:
			return this.ParseProperty();
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x00040388 File Offset: 0x0003E588
		private bool ParseProperty()
		{
			char c = this._chars[this._charPos];
			char c2;
			if (c == '"' || c == '\'')
			{
				this._charPos++;
				c2 = c;
				this.ShiftBufferIfNeeded();
				this.ReadStringIntoBuffer(c2);
			}
			else
			{
				if (!this.ValidIdentifierChar(c))
				{
					throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				c2 = '\0';
				this.ShiftBufferIfNeeded();
				this.ParseUnquotedProperty();
			}
			string text;
			if (this.NameTable != null)
			{
				text = this.NameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				if (text == null)
				{
					text = this._stringReference.ToString();
				}
			}
			else
			{
				text = this._stringReference.ToString();
			}
			this.EatWhitespace();
			if (this._chars[this._charPos] != ':')
			{
				throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			base.SetToken(JsonToken.PropertyName, text);
			this._quoteChar = c2;
			this.ClearRecentString();
			return true;
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x000404F0 File Offset: 0x0003E6F0
		private bool ValidIdentifierChar(char value)
		{
			return char.IsLetterOrDigit(value) || value == '_' || value == '$';
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0004050C File Offset: 0x0003E70C
		private void ParseUnquotedProperty()
		{
			int charPos = this._charPos;
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_0041;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else if (this.ReadUnquotedPropertyReportIfDone(c, charPos))
				{
					return;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
			IL_0041:
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x00040584 File Offset: 0x0003E784
		private bool ReadUnquotedPropertyReportIfDone(char currentChar, int initialPosition)
		{
			if (this.ValidIdentifierChar(currentChar))
			{
				this._charPos++;
				return false;
			}
			if (char.IsWhiteSpace(currentChar) || currentChar == ':')
			{
				this._stringReference = new StringReference(this._chars, initialPosition, this._charPos - initialPosition);
				return true;
			}
			throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x000405FC File Offset: 0x0003E7FC
		private bool ParseValue()
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= 'N')
				{
					if (c <= ' ')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_02A6;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								if (c != ' ')
								{
									goto IL_02A6;
								}
								break;
							}
							this._charPos++;
							continue;
						}
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
					else if (c <= '/')
					{
						if (c == '"')
						{
							goto IL_012E;
						}
						switch (c)
						{
						case '\'':
							goto IL_012E;
						case ')':
							goto IL_0264;
						case ',':
							goto IL_025A;
						case '-':
							goto IL_01CA;
						case '/':
							goto IL_0203;
						}
					}
					else
					{
						if (c == 'I')
						{
							goto IL_01C0;
						}
						if (c == 'N')
						{
							goto IL_01B6;
						}
					}
				}
				else if (c <= 'f')
				{
					if (c == '[')
					{
						goto IL_022B;
					}
					if (c == ']')
					{
						goto IL_0242;
					}
					if (c == 'f')
					{
						goto IL_0140;
					}
				}
				else if (c <= 't')
				{
					if (c == 'n')
					{
						goto IL_0148;
					}
					if (c == 't')
					{
						goto IL_0138;
					}
				}
				else
				{
					if (c == 'u')
					{
						goto IL_020C;
					}
					if (c == '{')
					{
						goto IL_0214;
					}
				}
				IL_02A6:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_02C4;
				}
				this._charPos++;
			}
			return false;
			IL_012E:
			this.ParseString(c, ReadType.Read);
			return true;
			IL_0138:
			this.ParseTrue();
			return true;
			IL_0140:
			this.ParseFalse();
			return true;
			IL_0148:
			if (this.EnsureChars(1, true))
			{
				char c2 = this._chars[this._charPos + 1];
				if (c2 == 'u')
				{
					this.ParseNull();
				}
				else
				{
					if (c2 != 'e')
					{
						throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
					}
					this.ParseConstructor();
				}
				return true;
			}
			this._charPos++;
			throw base.CreateUnexpectedEndException();
			IL_01B6:
			this.ParseNumberNaN(ReadType.Read);
			return true;
			IL_01C0:
			this.ParseNumberPositiveInfinity(ReadType.Read);
			return true;
			IL_01CA:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				this.ParseNumberNegativeInfinity(ReadType.Read);
			}
			else
			{
				this.ParseNumber(ReadType.Read);
			}
			return true;
			IL_0203:
			this.ParseComment(true);
			return true;
			IL_020C:
			this.ParseUndefined();
			return true;
			IL_0214:
			this._charPos++;
			base.SetToken(JsonToken.StartObject);
			return true;
			IL_022B:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return true;
			IL_0242:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_025A:
			base.SetToken(JsonToken.Undefined);
			return true;
			IL_0264:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_02C4:
			if (char.IsNumber(c) || c == '-' || c == '.')
			{
				this.ParseNumber(ReadType.Read);
				return true;
			}
			throw this.CreateUnexpectedCharacterException(c);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x000408FC File Offset: 0x0003EAFC
		private void ProcessLineFeed()
		{
			this._charPos++;
			this.OnNewLine(this._charPos);
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x00040918 File Offset: 0x0003EB18
		private void ProcessCarriageReturn(bool append)
		{
			this._charPos++;
			this.SetNewLine(this.EnsureChars(1, append));
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x00040938 File Offset: 0x0003EB38
		private void EatWhitespace()
		{
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c != '\0')
				{
					if (c != '\n')
					{
						if (c != '\r')
						{
							if (c != ' ' && !char.IsWhiteSpace(c))
							{
								return;
							}
							this._charPos++;
						}
						else
						{
							this.ProcessCarriageReturn(false);
						}
					}
					else
					{
						this.ProcessLineFeed();
					}
				}
				else if (this._charsUsed == this._charPos)
				{
					if (this.ReadData(false) == 0)
					{
						break;
					}
				}
				else
				{
					this._charPos++;
				}
			}
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x000409D4 File Offset: 0x0003EBD4
		private void ParseConstructor()
		{
			if (!this.MatchValueWithTrailingSeparator("new"))
			{
				throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
			}
			this.EatWhitespace();
			int charPos = this._charPos;
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_0057;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					if (!char.IsLetterOrDigit(c))
					{
						goto IL_008C;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
			IL_0057:
			int num = this._charPos;
			this._charPos++;
			goto IL_0116;
			IL_008C:
			if (c == '\r')
			{
				num = this._charPos;
				this.ProcessCarriageReturn(true);
			}
			else if (c == '\n')
			{
				num = this._charPos;
				this.ProcessLineFeed();
			}
			else if (char.IsWhiteSpace(c))
			{
				num = this._charPos;
				this._charPos++;
			}
			else
			{
				if (c != '(')
				{
					throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
				num = this._charPos;
			}
			IL_0116:
			this._stringReference = new StringReference(this._chars, charPos, num - charPos);
			string text = this._stringReference.ToString();
			this.EatWhitespace();
			if (this._chars[this._charPos] != '(')
			{
				throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			this.ClearRecentString();
			base.SetToken(JsonToken.StartConstructor, text);
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x00040B8C File Offset: 0x0003ED8C
		private void ParseNumber(ReadType readType)
		{
			this.ShiftBufferIfNeeded();
			char c = this._chars[this._charPos];
			int charPos = this._charPos;
			this.ReadNumberIntoBuffer();
			this.ParseReadNumber(readType, c, charPos);
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x00040BC8 File Offset: 0x0003EDC8
		private void ParseReadNumber(ReadType readType, char firstChar, int initialPosition)
		{
			base.SetPostValueState(true);
			this._stringReference = new StringReference(this._chars, initialPosition, this._charPos - initialPosition);
			bool flag = char.IsDigit(firstChar) && this._stringReference.Length == 1;
			bool flag2 = firstChar == '0' && this._stringReference.Length > 1 && this._stringReference.Chars[this._stringReference.StartIndex + 1] != '.' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'e' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E';
			JsonToken jsonToken;
			object obj;
			if (readType == ReadType.ReadAsString)
			{
				string text = this._stringReference.ToString();
				if (flag2)
				{
					try
					{
						if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							Convert.ToInt64(text, 16);
						}
						else
						{
							Convert.ToInt64(text, 8);
						}
						goto IL_0164;
					}
					catch (Exception ex)
					{
						throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text), ex);
					}
				}
				double num;
				if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
				{
					throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
				}
				IL_0164:
				jsonToken = JsonToken.String;
				obj = text;
			}
			else if (readType == ReadType.ReadAsInt32)
			{
				if (flag)
				{
					obj = (int)(firstChar - '0');
				}
				else
				{
					if (flag2)
					{
						string text2 = this._stringReference.ToString();
						try
						{
							obj = (text2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(text2, 16) : Convert.ToInt32(text2, 8));
							goto IL_028E;
						}
						catch (Exception ex2)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, text2), ex2);
						}
					}
					int num2;
					ParseResult parseResult = ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num2);
					if (parseResult == ParseResult.Success)
					{
						obj = num2;
					}
					else
					{
						if (parseResult == ParseResult.Overflow)
						{
							throw this.ThrowReaderError("JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
						}
						throw this.ThrowReaderError("Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
					}
				}
				IL_028E:
				jsonToken = JsonToken.Integer;
			}
			else if (readType == ReadType.ReadAsDecimal)
			{
				if (flag)
				{
					obj = firstChar - 48m;
				}
				else
				{
					if (flag2)
					{
						string text3 = this._stringReference.ToString();
						try
						{
							obj = Convert.ToDecimal(text3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text3, 16) : Convert.ToInt64(text3, 8));
							goto IL_0393;
						}
						catch (Exception ex3)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text3), ex3);
						}
					}
					decimal num3;
					if (ConvertUtils.DecimalTryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num3) != ParseResult.Success)
					{
						throw this.ThrowReaderError("Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
					}
					obj = num3;
				}
				IL_0393:
				jsonToken = JsonToken.Float;
			}
			else if (readType == ReadType.ReadAsDouble)
			{
				if (flag)
				{
					obj = (double)firstChar - 48.0;
				}
				else
				{
					if (flag2)
					{
						string text4 = this._stringReference.ToString();
						try
						{
							obj = Convert.ToDouble(text4.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text4, 16) : Convert.ToInt64(text4, 8));
							goto IL_048B;
						}
						catch (Exception ex4)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, text4), ex4);
						}
					}
					double num4;
					if (!double.TryParse(this._stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out num4))
					{
						throw this.ThrowReaderError("Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
					}
					obj = num4;
				}
				IL_048B:
				jsonToken = JsonToken.Float;
			}
			else if (flag)
			{
				obj = (long)((ulong)firstChar - 48UL);
				jsonToken = JsonToken.Integer;
			}
			else if (flag2)
			{
				string text5 = this._stringReference.ToString();
				try
				{
					obj = (text5.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text5, 16) : Convert.ToInt64(text5, 8));
				}
				catch (Exception ex5)
				{
					throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text5), ex5);
				}
				jsonToken = JsonToken.Integer;
			}
			else
			{
				long num5;
				ParseResult parseResult2 = ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num5);
				if (parseResult2 == ParseResult.Success)
				{
					obj = num5;
					jsonToken = JsonToken.Integer;
				}
				else if (parseResult2 == ParseResult.Overflow)
				{
					string text6 = this._stringReference.ToString();
					if (text6.Length > 380)
					{
						throw this.ThrowReaderError("JSON integer {0} is too large to parse.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
					}
					obj = JsonTextReader.BigIntegerParse(text6, CultureInfo.InvariantCulture);
					jsonToken = JsonToken.Integer;
				}
				else
				{
					if (this._floatParseHandling == FloatParseHandling.Decimal)
					{
						decimal num6;
						parseResult2 = ConvertUtils.DecimalTryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num6);
						if (parseResult2 != ParseResult.Success)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
						}
						obj = num6;
					}
					else
					{
						double num7;
						if (!double.TryParse(this._stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out num7))
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
						}
						obj = num7;
					}
					jsonToken = JsonToken.Float;
				}
			}
			this.ClearRecentString();
			base.SetToken(jsonToken, obj, false);
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x000412C4 File Offset: 0x0003F4C4
		private JsonReaderException ThrowReaderError(string message, Exception ex = null)
		{
			base.SetToken(JsonToken.Undefined, null, false);
			return JsonReaderException.Create(this, message, ex);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x000412D8 File Offset: 0x0003F4D8
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static object BigIntegerParse(string number, CultureInfo culture)
		{
			return BigInteger.Parse(number, culture);
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x000412E8 File Offset: 0x0003F4E8
		private void ParseComment(bool setToken)
		{
			this._charPos++;
			if (!this.EnsureChars(1, false))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			bool flag;
			if (this._chars[this._charPos] == '*')
			{
				flag = false;
			}
			else
			{
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				flag = true;
			}
			this._charPos++;
			int charPos = this._charPos;
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c <= '\n')
				{
					if (c != '\0')
					{
						if (c == '\n')
						{
							if (flag)
							{
								goto Block_16;
							}
							this.ProcessLineFeed();
							continue;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(true) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c != '\r')
				{
					if (c == '*')
					{
						this._charPos++;
						if (!flag && this.EnsureChars(0, true) && this._chars[this._charPos] == '/')
						{
							goto Block_14;
						}
						continue;
					}
				}
				else
				{
					if (flag)
					{
						goto Block_15;
					}
					this.ProcessCarriageReturn(true);
					continue;
				}
				this._charPos++;
			}
			if (!flag)
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			this.EndComment(setToken, charPos, this._charPos);
			return;
			Block_14:
			this.EndComment(setToken, charPos, this._charPos - 1);
			this._charPos++;
			return;
			Block_15:
			this.EndComment(setToken, charPos, this._charPos);
			return;
			Block_16:
			this.EndComment(setToken, charPos, this._charPos);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x000414C8 File Offset: 0x0003F6C8
		private void EndComment(bool setToken, int initialPosition, int endPosition)
		{
			if (setToken)
			{
				base.SetToken(JsonToken.Comment, new string(this._chars, initialPosition, endPosition - initialPosition));
			}
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x000414E8 File Offset: 0x0003F6E8
		private bool MatchValue(string value)
		{
			return this.MatchValue(this.EnsureChars(value.Length - 1, true), value);
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x00041500 File Offset: 0x0003F700
		private bool MatchValue(bool enoughChars, string value)
		{
			if (!enoughChars)
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (this._chars[this._charPos + i] != value[i])
				{
					this._charPos += i;
					return false;
				}
			}
			this._charPos += value.Length;
			return true;
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x00041580 File Offset: 0x0003F780
		private bool MatchValueWithTrailingSeparator(string value)
		{
			return this.MatchValue(value) && (!this.EnsureChars(0, false) || this.IsSeparator(this._chars[this._charPos]) || this._chars[this._charPos] == '\0');
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x000415D8 File Offset: 0x0003F7D8
		private bool IsSeparator(char c)
		{
			if (c <= ')')
			{
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				case '\v':
				case '\f':
					goto IL_00B6;
				default:
					if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_00B6;
						}
						if (base.CurrentState == JsonReader.State.Constructor || base.CurrentState == JsonReader.State.ConstructorStart)
						{
							return true;
						}
						return false;
					}
					break;
				}
				return true;
			}
			if (c <= '/')
			{
				if (c != ',')
				{
					if (c != '/')
					{
						goto IL_00B6;
					}
					if (!this.EnsureChars(1, false))
					{
						return false;
					}
					char c2 = this._chars[this._charPos + 1];
					return c2 == '*' || c2 == '/';
				}
			}
			else if (c != ']' && c != '}')
			{
				goto IL_00B6;
			}
			return true;
			IL_00B6:
			if (char.IsWhiteSpace(c))
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x000416B0 File Offset: 0x0003F8B0
		private void ParseTrue()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.True))
			{
				base.SetToken(JsonToken.Boolean, true);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x000416DC File Offset: 0x0003F8DC
		private void ParseNull()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Null))
			{
				base.SetToken(JsonToken.Null);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing null value.");
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x00041704 File Offset: 0x0003F904
		private void ParseUndefined()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Undefined))
			{
				base.SetToken(JsonToken.Undefined);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing undefined value.");
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x0004172C File Offset: 0x0003F92C
		private void ParseFalse()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.False))
			{
				base.SetToken(JsonToken.Boolean, false);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00041758 File Offset: 0x0003F958
		private object ParseNumberNegativeInfinity(ReadType readType)
		{
			return this.ParseNumberNegativeInfinity(readType, this.MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity));
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x0004176C File Offset: 0x0003F96C
		private object ParseNumberNegativeInfinity(ReadType readType, bool matched)
		{
			if (matched)
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.NegativeInfinity);
						return JsonConvert.NegativeInfinity;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_005C;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, double.NegativeInfinity);
					return double.NegativeInfinity;
				}
				IL_005C:
				throw JsonReaderException.Create(this, "Cannot read -Infinity value.");
			}
			throw JsonReaderException.Create(this, "Error parsing -Infinity value.");
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x000417F0 File Offset: 0x0003F9F0
		private object ParseNumberPositiveInfinity(ReadType readType)
		{
			return this.ParseNumberPositiveInfinity(readType, this.MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity));
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00041804 File Offset: 0x0003FA04
		private object ParseNumberPositiveInfinity(ReadType readType, bool matched)
		{
			if (matched)
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.PositiveInfinity);
						return JsonConvert.PositiveInfinity;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_005C;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, double.PositiveInfinity);
					return double.PositiveInfinity;
				}
				IL_005C:
				throw JsonReaderException.Create(this, "Cannot read Infinity value.");
			}
			throw JsonReaderException.Create(this, "Error parsing Infinity value.");
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00041888 File Offset: 0x0003FA88
		private object ParseNumberNaN(ReadType readType)
		{
			return this.ParseNumberNaN(readType, this.MatchValueWithTrailingSeparator(JsonConvert.NaN));
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x0004189C File Offset: 0x0003FA9C
		private object ParseNumberNaN(ReadType readType, bool matched)
		{
			if (matched)
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.NaN);
						return JsonConvert.NaN;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_005C;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, double.NaN);
					return double.NaN;
				}
				IL_005C:
				throw JsonReaderException.Create(this, "Cannot read NaN value.");
			}
			throw JsonReaderException.Create(this, "Error parsing NaN value.");
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00041920 File Offset: 0x0003FB20
		public override void Close()
		{
			base.Close();
			if (this._chars != null)
			{
				BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
				this._chars = null;
			}
			if (base.CloseInput)
			{
				TextReader reader = this._reader;
				if (reader != null)
				{
					reader.Close();
				}
			}
			this._stringBuffer.Clear(this._arrayPool);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00041990 File Offset: 0x0003FB90
		public bool HasLineInfo()
		{
			return true;
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060009A0 RID: 2464 RVA: 0x00041994 File Offset: 0x0003FB94
		public int LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start && this.LinePosition == 0 && this.TokenType != JsonToken.Comment)
				{
					return 0;
				}
				return this._lineNumber;
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060009A1 RID: 2465 RVA: 0x000419C0 File Offset: 0x0003FBC0
		public int LinePosition
		{
			get
			{
				return this._charPos - this._lineStartPos;
			}
		}

		// Token: 0x040003F4 RID: 1012
		private readonly bool _safeAsync;

		// Token: 0x040003F5 RID: 1013
		private const char UnicodeReplacementChar = '\ufffd';

		// Token: 0x040003F6 RID: 1014
		private const int MaximumJavascriptIntegerCharacterLength = 380;

		// Token: 0x040003F7 RID: 1015
		private readonly TextReader _reader;

		// Token: 0x040003F8 RID: 1016
		private char[] _chars;

		// Token: 0x040003F9 RID: 1017
		private int _charsUsed;

		// Token: 0x040003FA RID: 1018
		private int _charPos;

		// Token: 0x040003FB RID: 1019
		private int _lineStartPos;

		// Token: 0x040003FC RID: 1020
		private int _lineNumber;

		// Token: 0x040003FD RID: 1021
		private bool _isEndOfFile;

		// Token: 0x040003FE RID: 1022
		private StringBuffer _stringBuffer;

		// Token: 0x040003FF RID: 1023
		private StringReference _stringReference;

		// Token: 0x04000400 RID: 1024
		private IArrayPool<char> _arrayPool;

		// Token: 0x04000401 RID: 1025
		internal PropertyNameTable NameTable;
	}
}
