using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Leaf.xNet
{
	// Token: 0x02000067 RID: 103
	[ComVisible(true)]
	public class FormUrlEncodedContent : BytesContent
	{
		// Token: 0x060005A2 RID: 1442 RVA: 0x00020BE8 File Offset: 0x0001EDE8
		public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content, bool valuesUnescaped = false, bool keysUnescaped = false)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			this.Init(Http.ToQueryString(content, valuesUnescaped, keysUnescaped));
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x00020C10 File Offset: 0x0001EE10
		public FormUrlEncodedContent(RequestParams rp)
		{
			if (rp == null)
			{
				throw new ArgumentNullException("rp");
			}
			this.Init(rp.Query);
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x00020C38 File Offset: 0x0001EE38
		private void Init(string content)
		{
			this.Content = Encoding.ASCII.GetBytes(content);
			this.Offset = 0;
			this.Count = this.Content.Length;
			this.MimeContentType = "application/x-www-form-urlencoded";
		}
	}
}
