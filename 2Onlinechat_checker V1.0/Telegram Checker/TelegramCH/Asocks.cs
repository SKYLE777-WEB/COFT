using System;
using System.Threading.Tasks;
using Leaf.xNet;

namespace TelegramCH
{
	// Token: 0x02000008 RID: 8
	internal class Asocks
	{
		// Token: 0x06000075 RID: 117 RVA: 0x00006860 File Offset: 0x00004A60
		public async Task AsocksWhitelist(string api_key, string ip)
		{
			using (HttpRequest httpRequest = new HttpRequest())
			{
				try
				{
					string text = "{\"ip\": \"" + ip + "\", \"description\": \"TelegramChecker\"}";
					httpRequest.Post("https://api.asocks.com/v2/whitelist/add?apikey=" + api_key, text, "application/json");
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000068B0 File Offset: 0x00004AB0
		public async Task<string> AsocksGetPlan(string api_key)
		{
			string text;
			using (HttpRequest httpRequest = new HttpRequest())
			{
				httpRequest.ConnectTimeout = 10000;
				text = httpRequest.Get("https://api.asocks.com/v2/plan/info?apikey=" + api_key, null).ToString();
			}
			return text;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000068F8 File Offset: 0x00004AF8
		public async Task<string> AsocksGetPoxy(string url)
		{
			string text;
			using (HttpRequest httpRequest = new HttpRequest())
			{
				httpRequest.ConnectTimeout = 10000;
				text = httpRequest.Get(url ?? "", null).ToString();
			}
			return text;
		}
	}
}
