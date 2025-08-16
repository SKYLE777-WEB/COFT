using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Leaf.xNet
{
	// Token: 0x02000070 RID: 112
	internal class ProgressStreamContent : StreamContent
	{
		// Token: 0x060005D2 RID: 1490 RVA: 0x00021578 File Offset: 0x0001F778
		public ProgressStreamContent(Stream stream, CancellationToken token)
			: this(new ProgressStreamContent.ProgressStream(stream, token))
		{
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x00021588 File Offset: 0x0001F788
		public ProgressStreamContent(Stream stream, int bufferSize)
			: this(new ProgressStreamContent.ProgressStream(stream, CancellationToken.None), bufferSize)
		{
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x0002159C File Offset: 0x0001F79C
		private ProgressStreamContent(ProgressStreamContent.ProgressStream stream)
		{
			this._totalBytesExpected = -1L;
			base..ctor(stream);
			this.Init(stream);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x000215B4 File Offset: 0x0001F7B4
		private ProgressStreamContent(ProgressStreamContent.ProgressStream stream, int bufferSize)
		{
			this._totalBytesExpected = -1L;
			base..ctor(stream, bufferSize);
			this.Init(stream);
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x000215D0 File Offset: 0x0001F7D0
		private void Init(ProgressStreamContent.ProgressStream stream)
		{
			stream.ReadCallback = new Action<long>(this.ReadBytes);
			this.Progress = delegate
			{
			};
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0002160C File Offset: 0x0001F80C
		private void Reset()
		{
			this._totalBytes = 0L;
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x00021618 File Offset: 0x0001F818
		private void ReadBytes(long bytes)
		{
			if (this._totalBytesExpected == -1L)
			{
				this._totalBytesExpected = base.Headers.ContentLength ?? (-1L);
			}
			long num;
			if (this._totalBytesExpected == -1L && this.TryComputeLength(out num))
			{
				this._totalBytesExpected = ((num == 0L) ? (-1L) : num);
			}
			this._totalBytesExpected = Math.Max(-1L, this._totalBytesExpected);
			this._totalBytes += bytes;
			this.Progress(bytes, this._totalBytes, this._totalBytesExpected);
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x000216CC File Offset: 0x0001F8CC
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x000216D4 File Offset: 0x0001F8D4
		public ProgressDelegate Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				ProgressDelegate progressDelegate = value;
				if (value == null && (progressDelegate = ProgressStreamContent.<>c.<>9__11_0) == null)
				{
					progressDelegate = (ProgressStreamContent.<>c.<>9__11_0 = delegate
					{
					});
				}
				this._progress = progressDelegate;
			}
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x00021708 File Offset: 0x0001F908
		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			this.Reset();
			return base.SerializeToStreamAsync(stream, context);
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00021718 File Offset: 0x0001F918
		protected override bool TryComputeLength(out long length)
		{
			bool flag = base.TryComputeLength(out length);
			this._totalBytesExpected = length;
			return flag;
		}

		// Token: 0x04000285 RID: 645
		private long _totalBytes;

		// Token: 0x04000286 RID: 646
		private long _totalBytesExpected;

		// Token: 0x04000287 RID: 647
		private ProgressDelegate _progress;

		// Token: 0x020001E0 RID: 480
		private class ProgressStream : Stream
		{
			// Token: 0x060015BB RID: 5563 RVA: 0x00076CC8 File Offset: 0x00074EC8
			public ProgressStream(Stream stream, CancellationToken token)
			{
				this.ParentStream = stream;
				this._token = token;
				this.ReadCallback = delegate
				{
				};
				this.WriteCallback = delegate
				{
				};
			}

			// Token: 0x170003FC RID: 1020
			// (get) Token: 0x060015BC RID: 5564 RVA: 0x00076D40 File Offset: 0x00074F40
			// (set) Token: 0x060015BD RID: 5565 RVA: 0x00076D48 File Offset: 0x00074F48
			public Action<long> ReadCallback { private get; set; }

			// Token: 0x170003FD RID: 1021
			// (get) Token: 0x060015BE RID: 5566 RVA: 0x00076D54 File Offset: 0x00074F54
			private Action<long> WriteCallback { get; }

			// Token: 0x170003FE RID: 1022
			// (get) Token: 0x060015BF RID: 5567 RVA: 0x00076D5C File Offset: 0x00074F5C
			private Stream ParentStream { get; }

			// Token: 0x170003FF RID: 1023
			// (get) Token: 0x060015C0 RID: 5568 RVA: 0x00076D64 File Offset: 0x00074F64
			public override bool CanRead
			{
				get
				{
					return this.ParentStream.CanRead;
				}
			}

			// Token: 0x17000400 RID: 1024
			// (get) Token: 0x060015C1 RID: 5569 RVA: 0x00076D74 File Offset: 0x00074F74
			public override bool CanSeek
			{
				get
				{
					return this.ParentStream.CanSeek;
				}
			}

			// Token: 0x17000401 RID: 1025
			// (get) Token: 0x060015C2 RID: 5570 RVA: 0x00076D84 File Offset: 0x00074F84
			public override bool CanWrite
			{
				get
				{
					return this.ParentStream.CanWrite;
				}
			}

			// Token: 0x17000402 RID: 1026
			// (get) Token: 0x060015C3 RID: 5571 RVA: 0x00076D94 File Offset: 0x00074F94
			public override bool CanTimeout
			{
				get
				{
					return this.ParentStream.CanTimeout;
				}
			}

			// Token: 0x17000403 RID: 1027
			// (get) Token: 0x060015C4 RID: 5572 RVA: 0x00076DA4 File Offset: 0x00074FA4
			public override long Length
			{
				get
				{
					return this.ParentStream.Length;
				}
			}

			// Token: 0x060015C5 RID: 5573 RVA: 0x00076DB4 File Offset: 0x00074FB4
			public override void Flush()
			{
				this.ParentStream.Flush();
			}

			// Token: 0x060015C6 RID: 5574 RVA: 0x00076DC4 File Offset: 0x00074FC4
			public override Task FlushAsync(CancellationToken cancellationToken)
			{
				return this.ParentStream.FlushAsync(cancellationToken);
			}

			// Token: 0x17000404 RID: 1028
			// (get) Token: 0x060015C7 RID: 5575 RVA: 0x00076DD4 File Offset: 0x00074FD4
			// (set) Token: 0x060015C8 RID: 5576 RVA: 0x00076DE4 File Offset: 0x00074FE4
			public override long Position
			{
				get
				{
					return this.ParentStream.Position;
				}
				set
				{
					this.ParentStream.Position = value;
				}
			}

			// Token: 0x060015C9 RID: 5577 RVA: 0x00076DF4 File Offset: 0x00074FF4
			public override int Read(byte[] buffer, int offset, int count)
			{
				this._token.ThrowIfCancellationRequested();
				int num = this.ParentStream.Read(buffer, offset, count);
				this.ReadCallback((long)num);
				return num;
			}

			// Token: 0x060015CA RID: 5578 RVA: 0x00076E30 File Offset: 0x00075030
			public override long Seek(long offset, SeekOrigin origin)
			{
				this._token.ThrowIfCancellationRequested();
				return this.ParentStream.Seek(offset, origin);
			}

			// Token: 0x060015CB RID: 5579 RVA: 0x00076E5C File Offset: 0x0007505C
			public override void SetLength(long value)
			{
				this._token.ThrowIfCancellationRequested();
				this.ParentStream.SetLength(value);
			}

			// Token: 0x060015CC RID: 5580 RVA: 0x00076E88 File Offset: 0x00075088
			public override void Write(byte[] buffer, int offset, int count)
			{
				this._token.ThrowIfCancellationRequested();
				this.ParentStream.Write(buffer, offset, count);
				this.WriteCallback((long)count);
			}

			// Token: 0x060015CD RID: 5581 RVA: 0x00076EC4 File Offset: 0x000750C4
			public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				this._token.ThrowIfCancellationRequested();
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._token, cancellationToken);
				int num = await this.ParentStream.ReadAsync(buffer, offset, count, cancellationTokenSource.Token);
				this.ReadCallback((long)num);
				return num;
			}

			// Token: 0x060015CE RID: 5582 RVA: 0x00076F30 File Offset: 0x00075130
			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				this._token.ThrowIfCancellationRequested();
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._token, cancellationToken);
				Task task = this.ParentStream.WriteAsync(buffer, offset, count, cancellationTokenSource.Token);
				this.WriteCallback((long)count);
				return task;
			}

			// Token: 0x060015CF RID: 5583 RVA: 0x00076F80 File Offset: 0x00075180
			protected override void Dispose(bool disposing)
			{
				if (this._disposed)
				{
					return;
				}
				if (disposing)
				{
					Stream parentStream = this.ParentStream;
					if (parentStream != null)
					{
						parentStream.Dispose();
					}
				}
				this._disposed = true;
			}

			// Token: 0x04000875 RID: 2165
			private readonly CancellationToken _token;

			// Token: 0x04000879 RID: 2169
			private bool _disposed;
		}
	}
}
