using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leaf.xNet;
using TelegramCH;
using TelegramCH.Properties;

namespace ProxyBsquad
{
	// Token: 0x02000002 RID: 2
	public partial class Form1 : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public Form1()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x000020DC File Offset: 0x000002DC
		// (set) Token: 0x06000003 RID: 3 RVA: 0x000020E4 File Offset: 0x000002E4
		public string lic_key { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020F0 File Offset: 0x000002F0
		// (set) Token: 0x06000005 RID: 5 RVA: 0x000020F8 File Offset: 0x000002F8
		private string useproxy { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002104 File Offset: 0x00000304
		// (set) Token: 0x06000007 RID: 7 RVA: 0x0000210C File Offset: 0x0000030C
		public string membercount { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002118 File Offset: 0x00000318
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002120 File Offset: 0x00000320
		public string message { get; set; }

		// Token: 0x0600000A RID: 10 RVA: 0x0000212C File Offset: 0x0000032C
		private async Task<string> Req(string line, string proxy)
		{
			using (HttpRequest req = new HttpRequest())
			{
				int num = 0;
				try
				{
					req.Proxy = ProxyClient.Parse(this.type, proxy);
					req.AllowAutoRedirect = false;
					req.Reconnect = true;
					req.ReconnectDelay = 500;
					req.ReconnectLimit = 1;
					req.UserAgentRandomize();
					req.ConnectTimeout = 10000;
					req.KeepAlive = true;
					req.KeepAliveTimeout = 10000;
					object obj = req.Get(line ?? "", null);
					this.useproxy = "no_change";
					return obj.ToString();
				}
				catch
				{
					num = 1;
				}
				if (num == 1)
				{
					if (this.load.prx_queue.Count > 0)
					{
						string text = "";
						this.load.prx_queue.TryDequeue(out text);
						if (this.asocks_count.Text != "0")
						{
							this.asocks_count.Invoke(new Action(delegate
							{
								this.asocks_count.Text = this.load.prx_queue.Count.ToString();
							}));
						}
						else
						{
							this.proxy_count.Invoke(new Action(delegate
							{
								this.proxy_count.Text = this.load.prx_queue.Count.ToString();
							}));
						}
						Stata.ErrorProxy++;
						this.error_proxy.Invoke(new Action(delegate
						{
							this.error_proxy.Text = Stata.ErrorProxy.ToString();
						}));
						return await this.Req(line, text);
					}
					return "proxy_empty";
				}
			}
			HttpRequest req = null;
			string text2;
			return text2;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002184 File Offset: 0x00000384
		private async Task Botreq(string line, string proxy, string tb)
		{
			using (HttpRequest req = new HttpRequest())
			{
				req.IgnoreProtocolErrors = true;
				req.AllowAutoRedirect = false;
				req.Reconnect = false;
				req.UserAgentRandomize();
				req.ConnectTimeout = 10000;
				req.KeepAlive = true;
				req.KeepAliveTimeout = 10000;
				int num = 0;
				try
				{
					HttpResponse httpResponse = req.Get("https://" + tb + "/getChat?chat_id=" + line.Replace("https://t.me/", "@"), null);
					if (httpResponse.IsOK)
					{
						this.stata.testbot = httpResponse.ToString();
					}
					else if (HttpStatusCode.BadRequest == httpResponse.StatusCode)
					{
						this.stata.testbot = "not found";
					}
					else if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests && int.Parse(Regex.Match(httpResponse.ToString(), "(?<=\"retry_after\":).*?(?=})").ToString()) > 1)
					{
						MessageBox.Show("BAN");
						return;
					}
				}
				catch
				{
					num = 1;
				}
				if (num == 1 && this.load.prx_queue.Count > 0)
				{
					string text;
					this.load.prx_queue.TryDequeue(out text);
					Stata.ErrorProxy++;
					this.error_proxy.Invoke(new Action(delegate
					{
						this.error_proxy.Text = Stata.ErrorProxy.ToString();
					}));
					await this.Botreq(line, text, tb);
				}
			}
			HttpRequest req = null;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021DC File Offset: 0x000003DC
		private void start_Click(object sender, EventArgs e)
		{
			if (!this.radioButton1.Checked && !this.radioButton2.Checked && !this.radioButton3.Checked)
			{
				MessageBox.Show("Тип Proxy не выбран", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.load.prx_queue.Count == 0)
			{
				MessageBox.Show("Список Proxy - пуст !", "Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.load.queue.Count == 0)
			{
				MessageBox.Show("Список URLS - пуст !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.thr.Value > this.load.queue.Count)
			{
				this.thr.Value = this.load.queue.Count;
			}
			this.PrxType();
			List<Thread> threads = new List<Thread>();
			int num = 0;
			while (num < this.thr.Value)
			{
				Thread thread = new Thread(delegate
				{
					this.DoWork(this.cts.Token);
				});
				threads.Add(thread);
				thread.Start();
				num++;
			}
			this.start.Enabled = false;
			new Thread(delegate
			{
				foreach (Thread thread2 in threads)
				{
					thread2.Join();
				}
				MessageBox.Show("Work completed", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				this.start.Invoke(new Action(delegate
				{
					this.start.Enabled = true;
				}));
			}).Start();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002354 File Offset: 0x00000554
		private void ClearStata()
		{
			this.listView1.Items.Clear();
			this.listView3.Items.Clear();
			this.stata.ClearStata();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002390 File Offset: 0x00000590
		private async void loader_chat_Click(object sender, EventArgs e)
		{
			this.path = this.functions.CreateFolder();
			string text = this.load.LoadLines();
			if (text != null && File.Exists(text))
			{
				string[] array = (from line in File.ReadAllLines(text)
					select line.ToLower()).Distinct<string>().ToArray<string>();
				await this.load.GetLines(array);
				this.url_count.Text = this.load.queue.Count.ToString();
				if (Stata.IncorrectLines > 0)
				{
					this.incorrect_lines.Text = Stata.IncorrectLines.ToString();
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000023CC File Offset: 0x000005CC
		private void PrxType()
		{
			if (this.radioButton1.Checked)
			{
				this.type = ProxyType.HTTP;
				return;
			}
			if (this.radioButton2.Checked)
			{
				this.type = ProxyType.Socks4;
				return;
			}
			if (this.radioButton3.Checked)
			{
				this.type = ProxyType.Socks5;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002424 File Offset: 0x00000624
		private async void DoWork(CancellationToken token)
		{
			Form1.<>c__DisplayClass30_0 CS$<>8__locals1 = new Form1.<>c__DisplayClass30_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.line = "";
			string prx = "";
			while (this.load.queue.Count > 0 && this.load.prx_queue.Count > 0)
			{
				if (this.useproxy == "no_change")
				{
					this.load.queue.TryDequeue(out CS$<>8__locals1.line);
				}
				else
				{
					this.load.queue.TryDequeue(out CS$<>8__locals1.line);
					this.load.prx_queue.TryDequeue(out prx);
					if (this.asocks_count.Text != "0")
					{
						Control control = this.asocks_count;
						Action action;
						if ((action = CS$<>8__locals1.<>9__0) == null)
						{
							action = (CS$<>8__locals1.<>9__0 = delegate
							{
								CS$<>8__locals1.<>4__this.asocks_count.Text = CS$<>8__locals1.<>4__this.load.prx_queue.Count.ToString();
							});
						}
						control.Invoke(action);
					}
					else
					{
						Control control2 = this.proxy_count;
						Action action2;
						if ((action2 = CS$<>8__locals1.<>9__1) == null)
						{
							action2 = (CS$<>8__locals1.<>9__1 = delegate
							{
								CS$<>8__locals1.<>4__this.proxy_count.Text = CS$<>8__locals1.<>4__this.load.prx_queue.Count.ToString();
							});
						}
						control2.Invoke(action2);
					}
				}
				string text = await this.Req(CS$<>8__locals1.line, prx);
				object obj = Form1.block;
				lock (obj)
				{
					this.tresp = text.ToString();
					this.stata.Title = Regex.Match(this.tresp, "(?<=og:title\" content=\").*?(?=\")").ToString();
					this.stata.Description = Regex.Match(this.tresp, "(?<=\"tgme_page_description\"\\ dir=\"auto\">).*?(?=</div>)").ToString();
					this.stata.Online = Regex.Match(this.tresp, "(?<=members,\\ ).*?(?=</div>)").ToString();
					if (string.IsNullOrEmpty(this.stata.Online))
					{
						this.stata.No_Online = Regex.Match(this.tresp, "(?<=tgme_page_extra\">).*?(?=</div>)").ToString();
					}
					this.stata.Member_Count = Regex.Match(this.tresp, "(?<=\"tgme_page_extra\">).*?(?=,)").ToString();
					if (this.tresp == "proxy_empty")
					{
						break;
					}
					if (this.tresp.Contains("member"))
					{
						if (!string.IsNullOrEmpty(this.stata.Description))
						{
							this.stata.lang_chat = this.functions.LngDetector(this.stata.Description);
						}
						else
						{
							this.stata.lang_chat = "-";
						}
						if (this.stata.Member_Count == "" && this.stata.Online == "")
						{
							Control control3 = this.listView1;
							MethodInvoker methodInvoker;
							if ((methodInvoker = CS$<>8__locals1.<>9__2) == null)
							{
								Form1.<>c__DisplayClass30_0 CS$<>8__locals2 = CS$<>8__locals1;
								MethodInvoker methodInvoker2 = delegate
								{
									CS$<>8__locals1.<>4__this.listView1.Items.Add(CS$<>8__locals1.<>4__this.stata.Title).SubItems.AddRange(new string[]
									{
										CS$<>8__locals1.line,
										CS$<>8__locals1.<>4__this.stata.No_Online,
										"-",
										CS$<>8__locals1.<>4__this.stata.lang_chat
									});
								};
								CS$<>8__locals2.<>9__2 = methodInvoker2;
								methodInvoker = methodInvoker2;
							}
							control3.BeginInvoke(methodInvoker);
						}
						else
						{
							Control control4 = this.listView1;
							MethodInvoker methodInvoker3;
							if ((methodInvoker3 = CS$<>8__locals1.<>9__3) == null)
							{
								Form1.<>c__DisplayClass30_0 CS$<>8__locals3 = CS$<>8__locals1;
								MethodInvoker methodInvoker4 = delegate
								{
									CS$<>8__locals1.<>4__this.listView1.Items.Add(CS$<>8__locals1.<>4__this.stata.Title).SubItems.AddRange(new string[]
									{
										CS$<>8__locals1.line,
										CS$<>8__locals1.<>4__this.stata.Member_Count,
										CS$<>8__locals1.<>4__this.stata.Online,
										CS$<>8__locals1.<>4__this.stata.lang_chat
									});
								};
								CS$<>8__locals3.<>9__3 = methodInvoker4;
								methodInvoker3 = methodInvoker4;
							}
							control4.BeginInvoke(methodInvoker3);
						}
						Stata.WorkChats++;
						Control control5 = this.work_chats;
						Action action3;
						if ((action3 = CS$<>8__locals1.<>9__4) == null)
						{
							action3 = (CS$<>8__locals1.<>9__4 = delegate
							{
								CS$<>8__locals1.<>4__this.work_chats.Text = Stata.WorkChats.ToString();
							});
						}
						control5.Invoke(action3);
						if (this.stata.Online == "" && this.stata.Member_Count == "")
						{
							this.Write("Work_Chats", CS$<>8__locals1.line, this.stata.lang_chat);
							this.Write("Work_Chats_Statistics", CS$<>8__locals1.line + " | " + this.stata.No_Online + " | -", this.stata.lang_chat);
						}
						else
						{
							this.Write("Work_Chats", CS$<>8__locals1.line, this.stata.lang_chat);
							this.Write("Work_Chats_Statistics", string.Concat(new string[]
							{
								CS$<>8__locals1.line,
								" | ",
								this.stata.Member_Count,
								" | ",
								this.stata.Online
							}), this.stata.lang_chat);
						}
					}
					else if (this.tresp.Contains("subscriber</div>") || this.tresp.Contains("subscribers</div>"))
					{
						this.stata.Member_Count_Channel = Regex.Match(this.tresp, "(?<=\"tgme_page_extra\">).*?(?=</div>)").ToString();
						if (!string.IsNullOrEmpty(this.stata.Description))
						{
							this.stata.lang_channel = this.functions.LngDetector(this.stata.Description);
						}
						else
						{
							this.stata.lang_channel = "-";
						}
						Control control6 = this.listView3;
						MethodInvoker methodInvoker5;
						if ((methodInvoker5 = CS$<>8__locals1.<>9__5) == null)
						{
							Form1.<>c__DisplayClass30_0 CS$<>8__locals4 = CS$<>8__locals1;
							MethodInvoker methodInvoker6 = delegate
							{
								CS$<>8__locals1.<>4__this.listView3.Items.Add(CS$<>8__locals1.<>4__this.stata.Title).SubItems.AddRange(new string[]
								{
									CS$<>8__locals1.line,
									CS$<>8__locals1.<>4__this.stata.Member_Count_Channel,
									CS$<>8__locals1.<>4__this.stata.lang_channel
								});
							};
							CS$<>8__locals4.<>9__5 = methodInvoker6;
							methodInvoker5 = methodInvoker6;
						}
						control6.BeginInvoke(methodInvoker5);
						Stata.WorkChannels++;
						Control control7 = this.work_channels;
						Action action4;
						if ((action4 = CS$<>8__locals1.<>9__6) == null)
						{
							action4 = (CS$<>8__locals1.<>9__6 = delegate
							{
								CS$<>8__locals1.<>4__this.work_channels.Text = Stata.WorkChannels.ToString();
							});
						}
						control7.Invoke(action4);
						this.Write("Work_Channels", CS$<>8__locals1.line + " | " + this.stata.Member_Count_Channel, this.stata.lang_channel);
					}
					else if (this.tresp.Contains("tgme_username_link"))
					{
						Stata.BadURLS++;
						Control control8 = this.bad_links;
						Action action5;
						if ((action5 = CS$<>8__locals1.<>9__7) == null)
						{
							action5 = (CS$<>8__locals1.<>9__7 = delegate
							{
								CS$<>8__locals1.<>4__this.bad_links.Text = Stata.BadURLS.ToString();
							});
						}
						control8.Invoke(action5);
						this.Write("bad_links", CS$<>8__locals1.line, "");
					}
					else if (!this.tresp.Contains("tgme_username_link") && this.tresp.Contains("Send Message"))
					{
						Stata.TgUsers++;
						Control control9 = this.tg_users;
						Action action6;
						if ((action6 = CS$<>8__locals1.<>9__8) == null)
						{
							action6 = (CS$<>8__locals1.<>9__8 = delegate
							{
								CS$<>8__locals1.<>4__this.tg_users.Text = Stata.TgUsers.ToString();
							});
						}
						control9.Invoke(action6);
						this.Write("Telegram_Users", CS$<>8__locals1.line, "");
					}
					else if (this.tresp != "proxy_empty")
					{
						Stata.UnknownError++;
						Control control10 = this.unknown_error;
						Action action7;
						if ((action7 = CS$<>8__locals1.<>9__9) == null)
						{
							action7 = (CS$<>8__locals1.<>9__9 = delegate
							{
								CS$<>8__locals1.<>4__this.unknown_error.Text = Stata.UnknownError.ToString();
							});
						}
						control10.Invoke(action7);
						this.Write("Unknown error", CS$<>8__locals1.line, "");
					}
				}
				if (token.IsCancellationRequested)
				{
					break;
				}
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002468 File Offset: 0x00000668
		private void Write(string chatname, string line, string lng)
		{
			if (lng == "")
			{
				using (StreamWriter streamWriter = new StreamWriter(Path.Combine(this.path, chatname + ".txt"), true))
				{
					streamWriter.WriteLine(line);
					return;
				}
			}
			using (StreamWriter streamWriter2 = new StreamWriter(Path.Combine(this.path, string.Concat(new string[] { "TG [ ", lng, " ]\\", chatname, ".txt" })), true))
			{
				streamWriter2.WriteLine(line);
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002530 File Offset: 0x00000730
		private async void proxy_Click(object sender, EventArgs e)
		{
			string text = this.load.LoadLines();
			if (text != null && File.Exists(text))
			{
				string[] array = File.ReadAllLines(text).ToArray<string>();
				await this.load.GetProxyLines(array);
				this.proxy_count.Text = this.load.prx_queue.Count.ToString();
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000256C File Offset: 0x0000076C
		private void button1_Click(object sender, EventArgs e)
		{
			string searchText = this.searcher.Text.ToLower();
			foreach (object obj in this.listView1.Items)
			{
				((ListViewItem)obj).ForeColor = this.listView1.ForeColor;
			}
			IEnumerable<ListViewItem> enumerable = this.listView1.Items.Cast<ListViewItem>();
			Func<ListViewItem, bool> <>9__0;
			Func<ListViewItem, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (ListViewItem item) => item.Text.ToLower().Contains(searchText));
			}
			foreach (ListViewItem listViewItem in enumerable.Where(func))
			{
				listViewItem.ForeColor = Color.Red;
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002674 File Offset: 0x00000874
		private void pictureBox1_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://asocks.com/");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "OK");
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000026B8 File Offset: 0x000008B8
		private void Team()
		{
			Color color = Color.White;
			for (;;)
			{
				Thread.Sleep(700);
				Color color2 = Color.FromArgb(this.rnd.Next(256), this.rnd.Next(256), this.rnd.Next(256));
				for (int i = 0; i <= 100; i++)
				{
					int r = (int)color.R + (int)(color2.R - color.R) * i / 100;
					int g = (int)color.G + (int)(color2.G - color.G) * i / 100;
					int b = (int)color.B + (int)(color2.B - color.B) * i / 100;
					try
					{
						this.label9.BeginInvoke(new MethodInvoker(delegate
						{
							this.label9.ForeColor = Color.FromArgb(r, g, b);
						}));
					}
					catch
					{
					}
					Thread.Sleep(10);
				}
				color = color2;
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000027D4 File Offset: 0x000009D4
		private async void Form1_Load(object sender, EventArgs e)
		{
			this.radioButton1.Checked = true;
			await Task.WhenAll(new Task[] { Task.Run(delegate
			{
				this.Team();
			}) });
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002810 File Offset: 0x00000A10
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002854 File Offset: 0x00000A54
		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				this.cts.Cancel();
				this.cts.Dispose();
				this.cts = new CancellationTokenSource();
			}
			catch (Exception ex)
			{
				this.message = ex.Message;
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000028AC File Offset: 0x00000AAC
		private void pictureBox2_Click(object sender, EventArgs e)
		{
			try
			{
				
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "OK");
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000028F0 File Offset: 0x00000AF0
		private static Task Inizialize()
		{
			Form1.<Inizialize>d__41 <Inizialize>d__;
			<Inizialize>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Inizialize>d__.<>1__state = -1;
			<Inizialize>d__.<>t__builder.Start<Form1.<Inizialize>d__41>(ref <Inizialize>d__);
			return <Inizialize>d__.<>t__builder.Task;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002930 File Offset: 0x00000B30
		private void open_result_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(Functions.folderPath))
			{
				Process.Start("explorer.exe", Functions.folderPath);
				return;
			}
			MessageBox.Show("Папка с результатами появится после загрузки базы!", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002968 File Offset: 0x00000B68
		private async void button3_Click(object sender, EventArgs e)
		{
			int Width = ((Form.ActiveForm.Size.Width == 985) ? 1680 : 985);
			int Height = 586;
			for (int i = 0; i <= 10; i++)
			{
				int num = base.Size.Width + (Width - base.Size.Width) * i / 10;
				int num2 = base.Size.Height + (Height - base.Size.Height) * i / 10;
				base.Size = new Size(num, num2);
				await Task.Run(delegate
				{
					Thread.Sleep(15);
				});
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000029A4 File Offset: 0x00000BA4
		private async void TestCheck()
		{
			while (this.load.queue.Count > 0)
			{
				Form1.<>c__DisplayClass44_0 CS$<>8__locals1 = new Form1.<>c__DisplayClass44_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.line = "";
				string text = "";
				this.load.queue.TryDequeue(out CS$<>8__locals1.line);
				if (Stata.BpsLimit < 20)
				{
					this.label14.Invoke(new Action(delegate
					{
						this.label14.Text = "api.telegram.org/bot5321688653:AAEI2yqGrOA_-sRZ3xaqutrexraSgFa0AnA";
					}));
					await this.Botreq(CS$<>8__locals1.line, text, "api.telegram.org/bot5321688653:AAEI2yqGrOA_-sRZ3xaqutrexraSgFa0AnA");
					if (this.stata.testbot == "not found")
					{
						break;
					}
				}
				else
				{
					this.label14.Invoke(new Action(delegate
					{
						this.label14.Text = "api.telegram.org/bot5229864731:AAEV0jOLrI_tfLx-WLBXsih1ys_6gsK9KBg";
					}));
					await this.Botreq(CS$<>8__locals1.line, text, "api.telegram.org/bot5229864731:AAEV0jOLrI_tfLx-WLBXsih1ys_6gsK9KBg");
					if (this.stata.testbot == "not found")
					{
						break;
					}
					if (Stata.BpsLimit > 39)
					{
						Thread.Sleep(5000);
						Stata.BpsLimit = 0;
					}
				}
				object obj = Form1.block;
				lock (obj)
				{
					string text2 = this.stata.testbot.Replace("true", "+").Replace("false", "-");
					CS$<>8__locals1.title = Regex.Unescape(Regex.Match(text2, "(?<=\"title\":\").*?(?=\")").ToString());
					CS$<>8__locals1.message = Regex.Match(text2, "(?<=\"can_send_messages\":).*?(?=,)").ToString();
					CS$<>8__locals1.media = Regex.Match(text2, "(?<=\"can_send_photos\":).*?(?=,)").ToString();
					this.listView3.BeginInvoke(new MethodInvoker(delegate
					{
						CS$<>8__locals1.<>4__this.listView4.Items.Add(CS$<>8__locals1.line).SubItems.AddRange(new string[] { CS$<>8__locals1.title, CS$<>8__locals1.message, CS$<>8__locals1.media });
					}));
					Stata.WorkChats++;
					Stata.BpsLimit++;
					this.label19.Invoke(new Action(delegate
					{
						this.label19.Text = Stata.WorkChats.ToString();
					}));
					this.label5.Invoke(new Action(delegate
					{
						this.label5.Text = Stata.BpsLimit.ToString();
					}));
				}
				CS$<>8__locals1 = null;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000029E0 File Offset: 0x00000BE0
		private void button8_Click(object sender, EventArgs e)
		{
			if (Stata.LoadUrls > 0)
			{
				string text = Path.Combine(this.path, "Остаток.txt");
				using (IEnumerator<string> enumerator = this.load.queue.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text2 = enumerator.Current;
						using (StreamWriter streamWriter = new StreamWriter(text, true))
						{
							streamWriter.WriteLine(text2);
						}
					}
					return;
				}
			}
			MessageBox.Show("Нечего сохранять !", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002A90 File Offset: 0x00000C90
		private async Task AsocksProxy()
		{
			MatchCollection matchCollection = Regex.Matches(await this.asocks.AsocksGetPoxy(this.asocks_url_proxy.Text), "(\\d{1,3}\\.){3}\\d{1,3}:(\\d+)");
			int count = matchCollection.Count;
			if (count > 0)
			{
				foreach (object obj in matchCollection)
				{
					string value = ((Match)obj).Value;
					this.load.prx_queue.Enqueue(value);
				}
			}
			this.asocks_count.Text = this.load.prx_queue.Count.ToString();
			if (count > 0)
			{
				this.proxy.Enabled = false;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002AD8 File Offset: 0x00000CD8
		private async void button10_Click(object sender, EventArgs e)
		{
			await this.AsocksProxy();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002B14 File Offset: 0x00000D14
		private void button6_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x04000002 RID: 2
		private Random rnd = new Random();

		// Token: 0x04000003 RID: 3
		private CancellationTokenSource cts = new CancellationTokenSource();

		// Token: 0x04000004 RID: 4
		private static object block = new object();

		// Token: 0x04000005 RID: 5
		private ProxyType type;

		// Token: 0x04000007 RID: 7
		private string path = "";

		// Token: 0x04000008 RID: 8
		private string tresp = "";

		// Token: 0x04000009 RID: 9
		private string[] tgb = new string[] { "api.telegram.org/bot5359876700:AAE8gbqn7HhajDmQ4WuzRJ3L4c_JEcfVNlE" };

		// Token: 0x0400000C RID: 12
		private static readonly string[] AppFilter = new string[]
		{
			"SFRUUERlYnVnZ2VyU3Zj", "SFRUUERlYnVnZ2VyVUk=", "aHR0cCBhbmFseXplciBzdGFuZC1hbG9uZQ==", "ZmlkZGxlcg==", "ZWZmZXRlY2ggaHR0cCBzbmlmZmVy", "ZmlyZXNoZWVw", "SUVXYXRjaCBQcm9mZXNzaW9uYWw=", "ZHVtcGNhcA==", "d2lyZXNoYXJr", "d2lyZXNoYXJrIHBvcnRhYmxl",
			"c3lzaW50ZXJuYWxzIHRjcHZpZXc=", "aHR0cCBkZWJ1Z2dlcg==", "TmV0d29ya01pbmVy", "TmV0d29ya1RyYWZmaWNWaWV3", "SFRUUE5ldHdvcmtTbmlmZmVy", "dGNwZHVtcA==", "aW50ZXJjZXB0ZXI=", "SW50ZXJjZXB0ZXItTkc=", "Y29kZWNyYWNrZXI=", "eDMyZGJn",
			"eDY0ZGJn", "b2xseWRiZw==", "aWRh", "Y2hhcmxlcw==", "ZG5zcHk=", "c2ltcGxlYXNzZW1ibHk=", "cGVlaw==", "cHJvY2Vzc2hhY2tlcg==", "c2N5bGxhX3g4Ng==", "c2N5bGxhX3g2NA==",
			"c2N5bGxh", "aWRhdTY0", "aWRhdQ==", "aWRhcQ==", "aWRhcTY0", "aWRhdw==", "aWRhdzY0", "aWRhZw==", "aWRhZzY0", "SW1wb3J0UkVD",
			"SU1NVU5JVFlERUJVR0dFUg==", "TWVnYUR1bXBlcg==", "Q29kZUJyb3dzZXI=", "cmVzaGFja2Vy"
		};

		// Token: 0x0400000D RID: 13
		private readonly Loader load = new Loader();

		// Token: 0x0400000E RID: 14
		private readonly Functions functions = new Functions();

		// Token: 0x0400000F RID: 15
		private readonly Stata stata = new Stata();

		// Token: 0x04000010 RID: 16
		private readonly Perfecto perf = new Perfecto();

		// Token: 0x04000011 RID: 17
		private readonly Asocks asocks = new Asocks();
	}
}
