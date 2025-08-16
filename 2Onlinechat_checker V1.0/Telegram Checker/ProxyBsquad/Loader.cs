using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxyBsquad
{
	// Token: 0x02000005 RID: 5
	internal class Loader
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000064D8 File Offset: 0x000046D8
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000064E0 File Offset: 0x000046E0
		public string basep { get; set; }

		// Token: 0x06000043 RID: 67 RVA: 0x000064EC File Offset: 0x000046EC
		public string LoadLines()
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Текстовые файлы(*.txt) | *.txt";
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					this.basep = openFileDialog.FileName;
				}
				else
				{
					MessageBox.Show("Файл не выбран", "error");
					this.basep = null;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "error");
			}
			return this.basep;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00006570 File Offset: 0x00004770
		public async Task GetProxyLines(string[] p_lines)
		{
			for (int i = 0; i < p_lines.Length; i++)
			{
				string text = Regex.Match(p_lines[i], "^(?:\\d+\\.\\d+\\.\\d+\\.\\d+:\\d+|\\S+:\\d+:\\S+:\\S+|\\S+:\\d+)$").ToString();
				if (!string.IsNullOrEmpty(text))
				{
					this.prx_queue.Enqueue(text);
				}
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000065C0 File Offset: 0x000047C0
		public async Task GetLines(string[] lines)
		{
			string text = "(?<!https:\\/\\/)(http:\\/\\/)?(t\\.me\\/|@|telegram\\.me\\/)(\\w+)\\s*";
			for (int i = 0; i < lines.Length; i++)
			{
				lines[i] = Regex.Replace(lines[i], text, "https://t.me/$3").Trim();
			}
			foreach (string text2 in new HashSet<string>(lines).ToArray<string>())
			{
				if (!Regex.IsMatch(text2, "^https:\\/\\/[^ ]+[^!\\\"#$%&'()*+,-./:;<=>?@[\\]^_{|}~^ ]+$"))
				{
					Stata.IncorrectLines++;
					this.IncorrectLines(text2);
				}
				else if (!string.IsNullOrEmpty(text2) && text2.Contains("joinchat"))
				{
					Stata.IncorrectLines++;
					this.IncorrectLines(text2);
				}
				else if (!string.IsNullOrEmpty(text2) && text2.Contains("https://t.me/") && text2.Length > 16)
				{
					this.queue.Enqueue(text2);
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00006610 File Offset: 0x00004810
		public void IncorrectLines(string line)
		{
			using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Functions.folderPath, "incorrect_urls.txt"), true))
			{
				streamWriter.WriteLine(line);
			}
		}

		// Token: 0x04000064 RID: 100
		public readonly ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

		// Token: 0x04000065 RID: 101
		public readonly ConcurrentQueue<string> prx_queue = new ConcurrentQueue<string>();

		// Token: 0x04000066 RID: 102
		public readonly ConcurrentQueue<string> tbot_queue = new ConcurrentQueue<string>();

		// Token: 0x04000067 RID: 103
		private readonly string dirPath = Directory.GetCurrentDirectory();
	}
}
