using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Leaf.xNet
{
	// Token: 0x0200006B RID: 107
	[ComVisible(true)]
	public class StringContent : BytesContent
	{
		// Token: 0x060005BE RID: 1470 RVA: 0x000213E0 File Offset: 0x0001F5E0
		public StringContent(string content)
			: this(content, Encoding.UTF8)
		{
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x000213F0 File Offset: 0x0001F5F0
		public StringContent(string content, Encoding encoding)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			byte[] array = ((encoding != null) ? encoding.GetBytes(content) : null);
			if (array == null)
			{
				throw new ArgumentNullException("encoding");
			}
			this.Content = array;
			this.Offset = 0;
			this.Count = this.Content.Length;
			this.MimeContentType = "text/plain";
		}
	}
}
