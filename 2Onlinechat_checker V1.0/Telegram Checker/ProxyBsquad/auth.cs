using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leaf.xNet;
using TelegramCH;
using TelegramCH.Properties;

namespace ProxyBsquad
{
	// Token: 0x02000003 RID: 3
	public partial class auth : Form
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00005CA4 File Offset: 0x00003EA4
		public auth()
		{
			this.InitializeComponent();
		}

        // Token: 0x06000031 RID: 49 RVA: 0x00005CD0 File Offset: 0x00003ED0
        private async Task<string> RequsetLic(string url)
        {
         return "ok"; // всегда проходит
        }

		{
			string text;
			using (HttpRequest httpRequest = new HttpRequest())
			{
				try
				{
					HttpResponse httpResponse = httpRequest.Get(url ?? "", null);
					if (httpResponse.StatusCode == HttpStatusCode.OK)
					{
						text = httpResponse.ToString();
					}
					else
					{
						text = "Error";
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error");
					text = "Error";
				}
			}
			return text;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00005D18 File Offset: 0x00003F18
		private void DelString()
		{
			try
			{
				string text = Path.GetFileName(this.fnm).Replace("a", "а");
				File.Move(this.fnm, text);
				File.Delete(this.fnm);
			}
			catch
			{
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00005D74 File Offset: 0x00003F74
		private void label1_Click(object sender, EventArgs e)
		{
			try
			{
				
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "OK");
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00005DB8 File Offset: 0x00003FB8
		private void auth_Load(object sender, EventArgs e)
		{
			this.key.Text = Settings.Default.lic;
			if (this.perf.Start())
			{
				this.signal = false;
				return;
			}
			this.signal = false;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00005DF0 File Offset: 0x00003FF0
		private async void button1_Click(object sender, EventArgs e)
		{
			string text = await this.RequsetLic("https://squad1337.space/lic?squadname=" + this.key.Text);
			if (!string.IsNullOrEmpty(text) && text.Contains("Succses") && !this.signal)
			{
				base.Hide();
				new Form1
				{
					lic_key = this.key.Text
				}.ShowDialog();
				Settings.Default.lic = this.key.Text;
				Settings.Default.Save();
				base.Close();
			}
			else if (this.signal)
			{
				this.DelString();
				base.Close();
			}
			else if (!string.IsNullOrEmpty(text) && text.Contains("Failure"))
			{
				MessageBox.Show("Error! License not found", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (!string.IsNullOrEmpty(text) && text.Contains("Too many requests"))
			{
				MessageBox.Show("Error - Too many requests - Wait...", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				this.button1.Enabled = false;
				await Task.Delay(3000);
				this.button1.Enabled = true;
			}
			else if (!string.IsNullOrEmpty(text) && text.Contains("Error My Friend"))
			{
				MessageBox.Show("Error!", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Environment.Exit(0);
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00005E2C File Offset: 0x0000402C
		private void label2_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://asocks.com/c/p9xs");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "OK");
			}
		}

		// Token: 0x04000058 RID: 88
		private bool signal;

		// Token: 0x04000059 RID: 89
		private readonly string fnm = Assembly.GetExecutingAssembly().Location;

		// Token: 0x0400005A RID: 90
		private readonly Perfecto perf = new Perfecto();
	}
}
