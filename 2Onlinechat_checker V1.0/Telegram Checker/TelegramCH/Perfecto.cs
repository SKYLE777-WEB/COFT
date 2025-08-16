using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Leaf.xNet;

namespace TelegramCH
{
	// Token: 0x02000009 RID: 9
	internal class Perfecto
	{
		// Token: 0x06000079 RID: 121
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool IsDebuggerPresent();

		// Token: 0x0600007A RID: 122 RVA: 0x00006948 File Offset: 0x00004B48
		public void What(string key)
		{
			Task.Delay(3000).Wait();
			for (;;)
			{
				using (HttpRequest httpRequest = new HttpRequest())
				{
					try
					{
						httpRequest.AllowAutoRedirect = false;
						httpRequest.KeepAlive = true;
						httpRequest.UserAgentRandomize();
						httpRequest.ConnectTimeout = 10000;
						string text = httpRequest.Get("https://squad1337.space/lic?squadname=" + key, null).ToString();
						if (string.IsNullOrEmpty(text) || !text.Contains("Succses"))
						{
							Environment.Exit(0);
						}
					}
					catch
					{
					}
				}
				Task.Delay(9000).Wait();
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00006A0C File Offset: 0x00004C0C
		public bool Start()
		{
			return Perfecto.IsDebuggerPresent();
		}
	}
}
