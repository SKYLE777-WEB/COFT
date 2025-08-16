using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000C9 RID: 201
	internal class Base64Encoder
	{
		// Token: 0x06000B60 RID: 2912 RVA: 0x00048510 File Offset: 0x00046710
		public Base64Encoder(TextWriter writer)
		{
			ValidationUtils.ArgumentNotNull(writer, "writer");
			this._writer = writer;
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00048538 File Offset: 0x00046738
		private void ValidateEncode(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x00048594 File Offset: 0x00046794
		public void Encode(byte[] buffer, int index, int count)
		{
			this.ValidateEncode(buffer, index, count);
			if (this._leftOverBytesCount > 0)
			{
				if (this.FulfillFromLeftover(buffer, index, ref count))
				{
					return;
				}
				int num = Convert.ToBase64CharArray(this._leftOverBytes, 0, 3, this._charsLine, 0);
				this.WriteChars(this._charsLine, 0, num);
			}
			this.StoreLeftOverBytes(buffer, index, ref count);
			int num2 = index + count;
			int num3 = 57;
			while (index < num2)
			{
				if (index + num3 > num2)
				{
					num3 = num2 - index;
				}
				int num4 = Convert.ToBase64CharArray(buffer, index, num3, this._charsLine, 0);
				this.WriteChars(this._charsLine, 0, num4);
				index += num3;
			}
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x00048638 File Offset: 0x00046838
		private void StoreLeftOverBytes(byte[] buffer, int index, ref int count)
		{
			int num = count % 3;
			if (num > 0)
			{
				count -= num;
				if (this._leftOverBytes == null)
				{
					this._leftOverBytes = new byte[3];
				}
				for (int i = 0; i < num; i++)
				{
					this._leftOverBytes[i] = buffer[index + count + i];
				}
			}
			this._leftOverBytesCount = num;
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x00048698 File Offset: 0x00046898
		private bool FulfillFromLeftover(byte[] buffer, int index, ref int count)
		{
			int leftOverBytesCount = this._leftOverBytesCount;
			while (leftOverBytesCount < 3 && count > 0)
			{
				this._leftOverBytes[leftOverBytesCount++] = buffer[index++];
				count--;
			}
			if (count == 0 && leftOverBytesCount < 3)
			{
				this._leftOverBytesCount = leftOverBytesCount;
				return true;
			}
			return false;
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x000486F4 File Offset: 0x000468F4
		public void Flush()
		{
			if (this._leftOverBytesCount > 0)
			{
				int num = Convert.ToBase64CharArray(this._leftOverBytes, 0, this._leftOverBytesCount, this._charsLine, 0);
				this.WriteChars(this._charsLine, 0, num);
				this._leftOverBytesCount = 0;
			}
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x00048740 File Offset: 0x00046940
		private void WriteChars(char[] chars, int index, int count)
		{
			this._writer.Write(chars, index, count);
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x00048750 File Offset: 0x00046950
		public async Task EncodeAsync(byte[] buffer, int index, int count, CancellationToken cancellationToken)
		{
			this.ValidateEncode(buffer, index, count);
			if (this._leftOverBytesCount > 0)
			{
				if (this.FulfillFromLeftover(buffer, index, ref count))
				{
					return;
				}
				int num5 = Convert.ToBase64CharArray(this._leftOverBytes, 0, 3, this._charsLine, 0);
				await this.WriteCharsAsync(this._charsLine, 0, num5, cancellationToken).ConfigureAwait(false);
			}
			this.StoreLeftOverBytes(buffer, index, ref count);
			int num4 = index + count;
			int length = 57;
			while (index < num4)
			{
				if (index + length > num4)
				{
					length = num4 - index;
				}
				int num6 = Convert.ToBase64CharArray(buffer, index, length, this._charsLine, 0);
				await this.WriteCharsAsync(this._charsLine, 0, num6, cancellationToken).ConfigureAwait(false);
				index += length;
			}
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x000487BC File Offset: 0x000469BC
		private Task WriteCharsAsync(char[] chars, int index, int count, CancellationToken cancellationToken)
		{
			return this._writer.WriteAsync(chars, index, count, cancellationToken);
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x000487D0 File Offset: 0x000469D0
		public Task FlushAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return cancellationToken.FromCanceled();
			}
			if (this._leftOverBytesCount > 0)
			{
				int num = Convert.ToBase64CharArray(this._leftOverBytes, 0, this._leftOverBytesCount, this._charsLine, 0);
				this._leftOverBytesCount = 0;
				return this.WriteCharsAsync(this._charsLine, 0, num, cancellationToken);
			}
			return AsyncUtils.CompletedTask;
		}

		// Token: 0x04000479 RID: 1145
		private const int Base64LineSize = 76;

		// Token: 0x0400047A RID: 1146
		private const int LineSizeInBytes = 57;

		// Token: 0x0400047B RID: 1147
		private readonly char[] _charsLine = new char[76];

		// Token: 0x0400047C RID: 1148
		private readonly TextWriter _writer;

		// Token: 0x0400047D RID: 1149
		private byte[] _leftOverBytes;

		// Token: 0x0400047E RID: 1150
		private int _leftOverBytesCount;
	}
}
