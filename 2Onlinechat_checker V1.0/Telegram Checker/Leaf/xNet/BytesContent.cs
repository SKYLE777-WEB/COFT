using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x02000065 RID: 101
	[ComVisible(true)]
	public class BytesContent : HttpContent
	{
		// Token: 0x0600059C RID: 1436 RVA: 0x00020A88 File Offset: 0x0001EC88
		public BytesContent(byte[] content)
			: this(content, 0, content.Length)
		{
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00020A98 File Offset: 0x0001EC98
		public BytesContent(byte[] content, int offset, int count)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (offset < 0)
			{
				throw ExceptionHelper.CanNotBeLess<int>("offset", 0);
			}
			if (offset > content.Length)
			{
				throw ExceptionHelper.CanNotBeGreater<int>("offset", content.Length);
			}
			if (count < 0)
			{
				throw ExceptionHelper.CanNotBeLess<int>("count", 0);
			}
			if (count > content.Length - offset)
			{
				throw ExceptionHelper.CanNotBeGreater<int>("count", content.Length - offset);
			}
			this.Content = content;
			this.Offset = offset;
			this.Count = count;
			this.MimeContentType = "application/octet-stream";
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x00020B38 File Offset: 0x0001ED38
		protected BytesContent()
		{
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x00020B40 File Offset: 0x0001ED40
		public override long CalculateContentLength()
		{
			return (long)this.Content.Length;
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x00020B4C File Offset: 0x0001ED4C
		public override void WriteTo(Stream stream)
		{
			stream.Write(this.Content, this.Offset, this.Count);
		}

		// Token: 0x04000274 RID: 628
		protected byte[] Content;

		// Token: 0x04000275 RID: 629
		protected int Offset;

		// Token: 0x04000276 RID: 630
		protected int Count;
	}
}
