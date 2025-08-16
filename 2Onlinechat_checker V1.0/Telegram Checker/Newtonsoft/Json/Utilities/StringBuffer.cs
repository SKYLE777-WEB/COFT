using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F3 RID: 243
	internal struct StringBuffer
	{
		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000D06 RID: 3334 RVA: 0x00052084 File Offset: 0x00050284
		// (set) Token: 0x06000D07 RID: 3335 RVA: 0x0005208C File Offset: 0x0005028C
		public int Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000D08 RID: 3336 RVA: 0x00052098 File Offset: 0x00050298
		public bool IsEmpty
		{
			get
			{
				return this._buffer == null;
			}
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x000520A4 File Offset: 0x000502A4
		public StringBuffer(IArrayPool<char> bufferPool, int initalSize)
		{
			this = new StringBuffer(BufferUtils.RentBuffer(bufferPool, initalSize));
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x000520B4 File Offset: 0x000502B4
		private StringBuffer(char[] buffer)
		{
			this._buffer = buffer;
			this._position = 0;
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x000520C4 File Offset: 0x000502C4
		public void Append(IArrayPool<char> bufferPool, char value)
		{
			if (this._position == this._buffer.Length)
			{
				this.EnsureSize(bufferPool, 1);
			}
			char[] buffer = this._buffer;
			int position = this._position;
			this._position = position + 1;
			buffer[position] = value;
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x0005210C File Offset: 0x0005030C
		public void Append(IArrayPool<char> bufferPool, char[] buffer, int startIndex, int count)
		{
			if (this._position + count >= this._buffer.Length)
			{
				this.EnsureSize(bufferPool, count);
			}
			Array.Copy(buffer, startIndex, this._buffer, this._position, count);
			this._position += count;
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x00052160 File Offset: 0x00050360
		public void Clear(IArrayPool<char> bufferPool)
		{
			if (this._buffer != null)
			{
				BufferUtils.ReturnBuffer(bufferPool, this._buffer);
				this._buffer = null;
			}
			this._position = 0;
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x00052188 File Offset: 0x00050388
		private void EnsureSize(IArrayPool<char> bufferPool, int appendLength)
		{
			char[] array = BufferUtils.RentBuffer(bufferPool, (this._position + appendLength) * 2);
			if (this._buffer != null)
			{
				Array.Copy(this._buffer, array, this._position);
				BufferUtils.ReturnBuffer(bufferPool, this._buffer);
			}
			this._buffer = array;
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x000521DC File Offset: 0x000503DC
		public override string ToString()
		{
			return this.ToString(0, this._position);
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x000521EC File Offset: 0x000503EC
		public string ToString(int start, int length)
		{
			return new string(this._buffer, start, length);
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000D11 RID: 3345 RVA: 0x000521FC File Offset: 0x000503FC
		public char[] InternalBuffer
		{
			get
			{
				return this._buffer;
			}
		}

		// Token: 0x0400052E RID: 1326
		private char[] _buffer;

		// Token: 0x0400052F RID: 1327
		private int _position;
	}
}
