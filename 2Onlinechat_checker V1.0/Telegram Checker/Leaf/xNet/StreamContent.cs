using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x0200006A RID: 106
	[ComVisible(true)]
	public class StreamContent : HttpContent
	{
		// Token: 0x060005B8 RID: 1464 RVA: 0x00021290 File Offset: 0x0001F490
		public StreamContent(Stream contentStream, int bufferSize = 32768)
		{
			if (contentStream == null)
			{
				throw new ArgumentNullException("contentStream");
			}
			if (!contentStream.CanRead || !contentStream.CanSeek)
			{
				throw new ArgumentException(Resources.ArgumentException_CanNotReadOrSeek, "contentStream");
			}
			if (bufferSize < 1)
			{
				throw ExceptionHelper.CanNotBeLess<int>("bufferSize", 1);
			}
			this.ContentStream = contentStream;
			this.BufferSize = bufferSize;
			this.InitialStreamPosition = this.ContentStream.Position;
			this.MimeContentType = "application/octet-stream";
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x0002131C File Offset: 0x0001F51C
		protected StreamContent()
		{
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x00021324 File Offset: 0x0001F524
		public override long CalculateContentLength()
		{
			this.ThrowIfDisposed();
			return this.ContentStream.Length;
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x00021338 File Offset: 0x0001F538
		public override void WriteTo(Stream stream)
		{
			this.ThrowIfDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.ContentStream.Position = this.InitialStreamPosition;
			byte[] array = new byte[this.BufferSize];
			for (;;)
			{
				int num = this.ContentStream.Read(array, 0, array.Length);
				if (num == 0)
				{
					break;
				}
				stream.Write(array, 0, num);
			}
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x000213A0 File Offset: 0x0001F5A0
		protected override void Dispose(bool disposing)
		{
			if (!disposing || this.ContentStream == null)
			{
				return;
			}
			this.ContentStream.Dispose();
			this.ContentStream = null;
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x000213C8 File Offset: 0x0001F5C8
		private void ThrowIfDisposed()
		{
			if (this.ContentStream == null)
			{
				throw new ObjectDisposedException("StreamContent");
			}
		}

		// Token: 0x0400027E RID: 638
		protected Stream ContentStream;

		// Token: 0x0400027F RID: 639
		protected int BufferSize;

		// Token: 0x04000280 RID: 640
		protected long InitialStreamPosition;
	}
}
