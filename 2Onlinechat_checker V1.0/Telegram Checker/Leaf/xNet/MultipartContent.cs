using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Leaf.xNet
{
	// Token: 0x02000069 RID: 105
	[ComVisible(true)]
	public class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
	{
		// Token: 0x060005AC RID: 1452 RVA: 0x00020CB0 File Offset: 0x0001EEB0
		public MultipartContent()
			: this("----------------" + MultipartContent.GetRandomString(16))
		{
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00020CCC File Offset: 0x0001EECC
		public MultipartContent(string boundary)
		{
			if (boundary == null)
			{
				throw new ArgumentNullException("boundary");
			}
			if (boundary.Length == 0)
			{
				throw ExceptionHelper.EmptyString("boundary");
			}
			if (boundary.Length > 70)
			{
				throw ExceptionHelper.CanNotBeGreater<int>("boundary", 70);
			}
			this._boundary = boundary;
			this.MimeContentType = "multipart/form-data; boundary=" + this._boundary;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00020D4C File Offset: 0x0001EF4C
		public void Add(HttpContent content, string name)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			MultipartContent.Element element = new MultipartContent.Element
			{
				Name = name,
				Content = content
			};
			this._elements.Add(element);
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x00020DB8 File Offset: 0x0001EFB8
		public void Add(HttpContent content, string name, string fileName)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			content.ContentType = Http.DetermineMediaType(Path.GetExtension(fileName));
			MultipartContent.Element element = new MultipartContent.Element
			{
				Name = name,
				FileName = fileName,
				Content = content
			};
			this._elements.Add(element);
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00020E4C File Offset: 0x0001F04C
		public void Add(HttpContent content, string name, string fileName, string contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			content.ContentType = contentType;
			MultipartContent.Element element = new MultipartContent.Element
			{
				Name = name,
				FileName = fileName,
				Content = content
			};
			this._elements.Add(element);
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00020EE8 File Offset: 0x0001F0E8
		public override long CalculateContentLength()
		{
			this.ThrowIfDisposed();
			long num = 0L;
			foreach (MultipartContent.Element element in this._elements)
			{
				num += element.Content.CalculateContentLength();
				if (element.IsFieldFile())
				{
					num += 72L;
					num += (long)element.Name.Length;
					num += (long)element.FileName.Length;
					num += (long)element.Content.ContentType.Length;
				}
				else
				{
					num += 43L;
					num += (long)element.Name.Length;
				}
				num += (long)(this._boundary.Length + 6);
			}
			num += (long)(this._boundary.Length + 6);
			return num;
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00020FD4 File Offset: 0x0001F1D4
		public override void WriteTo(Stream stream)
		{
			this.ThrowIfDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] bytes = Encoding.ASCII.GetBytes("\r\n");
			byte[] array = Encoding.ASCII.GetBytes("--" + this._boundary + "\r\n");
			foreach (MultipartContent.Element element in this._elements)
			{
				stream.Write(array, 0, array.Length);
				string text;
				if (element.IsFieldFile())
				{
					text = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", element.Name, element.FileName, element.Content.ContentType);
				}
				else
				{
					text = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n", element.Name);
				}
				byte[] bytes2 = Encoding.ASCII.GetBytes(text);
				stream.Write(bytes2, 0, bytes2.Length);
				element.Content.WriteTo(stream);
				stream.Write(bytes, 0, bytes.Length);
			}
			array = Encoding.ASCII.GetBytes("--" + this._boundary + "--\r\n");
			stream.Write(array, 0, array.Length);
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x00021120 File Offset: 0x0001F320
		public IEnumerator<HttpContent> GetEnumerator()
		{
			this.ThrowIfDisposed();
			return this._elements.Select((MultipartContent.Element e) => e.Content).GetEnumerator();
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0002115C File Offset: 0x0001F35C
		protected override void Dispose(bool disposing)
		{
			if (!disposing || this._elements == null)
			{
				return;
			}
			foreach (MultipartContent.Element element in this._elements)
			{
				element.Content.Dispose();
			}
			this._elements = null;
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x000211D0 File Offset: 0x0001F3D0
		IEnumerator IEnumerable.GetEnumerator()
		{
			this.ThrowIfDisposed();
			return this.GetEnumerator();
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x000211E0 File Offset: 0x0001F3E0
		private static string GetRandomString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				int num = Randomizer.Instance.Next(3);
				if (num != 0)
				{
					if (num != 1)
					{
						stringBuilder.Append((char)Randomizer.Instance.Next(65, 91));
					}
					else
					{
						stringBuilder.Append((char)Randomizer.Instance.Next(97, 123));
					}
				}
				else
				{
					stringBuilder.Append((char)Randomizer.Instance.Next(48, 58));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x00021278 File Offset: 0x0001F478
		private void ThrowIfDisposed()
		{
			if (this._elements == null)
			{
				throw new ObjectDisposedException("MultipartContent");
			}
		}

		// Token: 0x04000278 RID: 632
		private const int FieldTemplateSize = 43;

		// Token: 0x04000279 RID: 633
		private const int FieldFileTemplateSize = 72;

		// Token: 0x0400027A RID: 634
		private const string FieldTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n";

		// Token: 0x0400027B RID: 635
		private const string FieldFileTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

		// Token: 0x0400027C RID: 636
		private readonly string _boundary;

		// Token: 0x0400027D RID: 637
		private List<MultipartContent.Element> _elements = new List<MultipartContent.Element>();

		// Token: 0x020001DE RID: 478
		private sealed class Element
		{
			// Token: 0x060015B6 RID: 5558 RVA: 0x00076C98 File Offset: 0x00074E98
			public bool IsFieldFile()
			{
				return this.FileName != null;
			}

			// Token: 0x04000870 RID: 2160
			public string Name;

			// Token: 0x04000871 RID: 2161
			public string FileName;

			// Token: 0x04000872 RID: 2162
			public HttpContent Content;
		}
	}
}
