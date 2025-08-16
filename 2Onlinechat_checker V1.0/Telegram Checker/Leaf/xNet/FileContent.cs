using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x02000066 RID: 102
	[ComVisible(true)]
	public class FileContent : StreamContent
	{
		// Token: 0x060005A1 RID: 1441 RVA: 0x00020B68 File Offset: 0x0001ED68
		public FileContent(string pathToContent, int bufferSize = 32768)
		{
			if (pathToContent == null)
			{
				throw new ArgumentNullException("pathToContent");
			}
			if (pathToContent.Length == 0)
			{
				throw ExceptionHelper.EmptyString("pathToContent");
			}
			if (bufferSize < 1)
			{
				throw ExceptionHelper.CanNotBeLess<int>("bufferSize", 1);
			}
			this.ContentStream = new FileStream(pathToContent, FileMode.Open, FileAccess.Read);
			this.BufferSize = bufferSize;
			this.InitialStreamPosition = 0L;
			this.MimeContentType = Http.DetermineMediaType(Path.GetExtension(pathToContent));
		}
	}
}
