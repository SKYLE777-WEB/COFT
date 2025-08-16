using System;
using System.IO;
using System.Threading.Tasks;
using LanguageIdentification;
using Leaf.xNet;

namespace ProxyBsquad
{
	// Token: 0x02000004 RID: 4
	internal class Functions
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00006330 File Offset: 0x00004530
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00006338 File Offset: 0x00004538
		public static string folderPath { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00006340 File Offset: 0x00004540
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00006348 File Offset: 0x00004548
		public static string subFolderPath { get; set; }

		// Token: 0x0600003D RID: 61 RVA: 0x00006350 File Offset: 0x00004550
		public string CreateFolder()
		{
			string text = string.Format("Telegram Checker [{0:HH.mm.ss}]", DateTime.Now);
			string[] array = new string[] { "ru", "en", "uk", "-" };
			Functions.folderPath = Path.Combine(this.dirPath, text);
			if (!Directory.Exists(Functions.folderPath))
			{
				Directory.CreateDirectory(Functions.folderPath);
				foreach (string text2 in array)
				{
					string text3 = "TG [ " + text2 + " ]";
					Directory.CreateDirectory(Path.Combine(Functions.folderPath, text3));
				}
			}
			return Functions.folderPath;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00006410 File Offset: 0x00004610
		public async Task<string> DCA(string key)
		{
			string text;
			using (HttpRequest httpRequest = new HttpRequest())
			{
				try
				{
					httpRequest.AllowAutoRedirect = false;
					httpRequest.KeepAlive = true;
					httpRequest.UserAgentRandomize();
					httpRequest.ConnectTimeout = 10000;
					text = httpRequest.Get("https://squad1337.space/get?squadname=" + key, null).ToString();
				}
				catch
				{
					text = "error";
				}
			}
			return text;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00006458 File Offset: 0x00004658
		public string LngDetector(string desc)
		{
			LanguageIdentificationClassifier languageIdentificationClassifier = new LanguageIdentificationClassifier(new string[] { "ru", "en", "uk" });
			if (string.IsNullOrEmpty(desc))
			{
				return "unknown";
			}
			languageIdentificationClassifier.Append(desc);
			string languageCode = languageIdentificationClassifier.Classify().LanguageCode;
			if (string.IsNullOrEmpty(languageCode))
			{
				return "unknown";
			}
			return languageCode;
		}

		// Token: 0x04000061 RID: 97
		public readonly string dirPath = Directory.GetCurrentDirectory();
	}
}
