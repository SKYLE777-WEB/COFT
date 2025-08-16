using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Leaf.xNet.Services.Captcha;

namespace Leaf.xNet.Services.Cloudflare
{
	// Token: 0x0200007B RID: 123
	[ComVisible(true)]
	public static class CloudflareBypass
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x00023278 File Offset: 0x00021478
		// (set) Token: 0x06000641 RID: 1601 RVA: 0x00023280 File Offset: 0x00021480
		public static string DefaultAcceptLanguage { get; set; } = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x00023288 File Offset: 0x00021488
		// (set) Token: 0x06000643 RID: 1603 RVA: 0x00023290 File Offset: 0x00021490
		public static int MaxRetries { get; set; } = 4;

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x00023298 File Offset: 0x00021498
		// (set) Token: 0x06000645 RID: 1605 RVA: 0x000232A0 File Offset: 0x000214A0
		public static int DelayMilliseconds { get; set; } = 5000;

		// Token: 0x06000646 RID: 1606 RVA: 0x000232A8 File Offset: 0x000214A8
		[Obsolete("Not maintained in public Leaf.xNet anymore.\r\nYou can order private paid Leaf.xNet with support.\r\nTelegram: @kelog")]
		public static bool IsCloudflared(this HttpResponse response)
		{
			bool flag = response.StatusCode == HttpStatusCode.ServiceUnavailable || response.StatusCode == HttpStatusCode.Forbidden;
			bool flag2 = response[HttpHeader.Server].IndexOf("cloudflare", StringComparison.OrdinalIgnoreCase) != -1;
			return flag && flag2;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000232F8 File Offset: 0x000214F8
		[Obsolete("Not maintained in public Leaf.xNet anymore.\r\nYou can order private paid Leaf.xNet with support.\r\nTelegram: @kelog")]
		public static HttpResponse GetThroughCloudflare(this HttpRequest request, Uri uri, CloudflareBypass.DLog log = null, CancellationToken cancellationToken = default(CancellationToken), ICaptchaSolver captchaSolver = null)
		{
			if (!request.UseCookies)
			{
				throw new CloudflareException("[Cloudflare] Cookies must be enabled. Please set $UseCookies to true.");
			}
			if (string.IsNullOrEmpty(request.UserAgent))
			{
				request.UserAgent = Http.ChromeUserAgent();
			}
			if (log != null)
			{
				log("[Cloudflare] Checking availability at: " + uri.AbsoluteUri + " ...");
			}
			int num = 0;
			if (num >= CloudflareBypass.MaxRetries)
			{
				throw new CloudflareException(CloudflareBypass.MaxRetries, "[Cloudflare] ERROR. Rate limit reached.");
			}
			string text = string.Format(". Retry {0} / {1}.", num + 1, CloudflareBypass.MaxRetries);
			if (log != null)
			{
				log("[Cloudflare] Trying to bypass" + text);
			}
			HttpResponse httpResponse = request.ManualGet(uri, null, null);
			if (!httpResponse.IsCloudflared())
			{
				if (log != null)
				{
					log("[Cloudflare]  OK. Not found at: " + uri.AbsoluteUri);
				}
				return httpResponse;
			}
			foreach (object obj in request.Cookies.GetCookies(uri))
			{
				Cookie cookie = (Cookie)obj;
				if (!(cookie.Name != "cf_clearance"))
				{
					cookie.Expired = true;
					break;
				}
			}
			if (cancellationToken != default(CancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
			}
			if (CloudflareBypass.HasJsChallenge(httpResponse))
			{
				CloudflareBypass.SolveJsChallenge(ref httpResponse, request, uri, text, log, cancellationToken);
			}
			if (CloudflareBypass.HasRecaptchaChallenge(httpResponse))
			{
				CloudflareBypass.SolveRecaptchaChallenge(ref httpResponse, request, uri, text, log, cancellationToken);
			}
			if (httpResponse.IsCloudflared())
			{
				throw new CloudflareException(CloudflareBypass.HasAccessDeniedError(httpResponse) ? "Access denied. Try to use another IP address." : "Unknown challenge type");
			}
			return httpResponse;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x000234D4 File Offset: 0x000216D4
		[Obsolete("Not maintained in public Leaf.xNet anymore.\r\nYou can order private paid Leaf.xNet with support.\r\nTelegram: @kelog")]
		public static HttpResponse GetThroughCloudflare(this HttpRequest request, string url, CloudflareBypass.DLog log = null, CancellationToken cancellationToken = default(CancellationToken), ICaptchaSolver captchaSolver = null)
		{
			Uri uri = ((request.BaseAddress != null && url.StartsWith("/")) ? new Uri(request.BaseAddress, url) : new Uri(url));
			return request.GetThroughCloudflare(uri, log, cancellationToken, captchaSolver);
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0002352C File Offset: 0x0002172C
		private static bool IsChallengePassed(string tag, ref HttpResponse response, HttpRequest request, Uri uri, string retry, CloudflareBypass.DLog log)
		{
			HttpStatusCode statusCode = response.StatusCode;
			if (statusCode == HttpStatusCode.Found)
			{
				if (response.HasRedirect)
				{
					if (!response.ContainsCookie(uri, "cf_clearance"))
					{
						return false;
					}
					if (log != null)
					{
						log(string.Concat(new string[] { "[Cloudflare] Passed [", tag, "]. Trying to get the original response at: ", uri.AbsoluteUri, " ..." }));
					}
					bool ignoreProtocolErrors = request.IgnoreProtocolErrors;
					request.IgnoreProtocolErrors = true;
					request.AddCloudflareHeaders(uri);
					response = request.Get(response.RedirectAddress.AbsoluteUri, null);
					request.IgnoreProtocolErrors = ignoreProtocolErrors;
					if (response.IsCloudflared())
					{
						if (log != null)
						{
							log("[Cloudflare] ERROR [" + tag + "]. Unable to get he original response at: " + uri.AbsoluteUri);
						}
						return false;
					}
				}
				if (log != null)
				{
					log("[Cloudflare] OK [" + tag + "]. Done: " + uri.AbsoluteUri);
				}
				return true;
			}
			if (statusCode == HttpStatusCode.Forbidden || statusCode == HttpStatusCode.ServiceUnavailable)
			{
				return false;
			}
			if (log != null)
			{
				log(string.Format("{0}ERROR [{1}]. Status code : {2}{3}.", new object[] { "[Cloudflare] ", tag, response.StatusCode, retry }));
			}
			return false;
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00023690 File Offset: 0x00021890
		private static bool HasJsChallenge(HttpResponse response)
		{
			return response.ToString().ContainsInsensitive("jschl-answer");
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x000236A4 File Offset: 0x000218A4
		private static bool SolveJsChallenge(ref HttpResponse response, HttpRequest request, Uri uri, string retry, CloudflareBypass.DLog log, CancellationToken cancellationToken)
		{
			if (log != null)
			{
				log("[Cloudflare] Solving JS Challenge for URL: " + uri.AbsoluteUri + " ...");
			}
			response = CloudflareBypass.PassClearance(request, response, uri, log, cancellationToken);
			return CloudflareBypass.IsChallengePassed("JS", ref response, request, uri, retry, log);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x000236F8 File Offset: 0x000218F8
		private static Uri GetSolutionUri(HttpResponse response)
		{
			string text = response.ToString();
			string scheme = response.Address.Scheme;
			string host = response.Address.Host;
			int port = response.Address.Port;
			ChallengeSolution challengeSolution = ChallengeSolver.Solve(text, host, port);
			return new Uri(string.Format("{0}://{1}:{2}{3}", new object[] { scheme, host, port, challengeSolution.ClearanceQuery }));
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0002376C File Offset: 0x0002196C
		private static HttpResponse PassClearance(HttpRequest request, HttpResponse response, Uri refererUri, CloudflareBypass.DLog log, CancellationToken cancellationToken)
		{
			Uri solutionUri = CloudflareBypass.GetSolutionUri(response);
			CloudflareBypass.Delay(CloudflareBypass.DelayMilliseconds, log, cancellationToken);
			return request.ManualGet(solutionUri, refererUri, null);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x0002379C File Offset: 0x0002199C
		private static bool HasRecaptchaChallenge(HttpResponse response)
		{
			return response.ToString().IndexOf("<div class=\"g-recaptcha\">", StringComparison.OrdinalIgnoreCase) != -1;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000237B8 File Offset: 0x000219B8
		private static bool SolveRecaptchaChallenge(ref HttpResponse response, HttpRequest request, Uri uri, string retry, CloudflareBypass.DLog log, CancellationToken cancelToken)
		{
			if (log != null)
			{
				log("[Cloudflare] Solving Recaptcha Challenge for URL: " + uri.AbsoluteUri + " ...");
			}
			if (request.CaptchaSolver == null)
			{
				throw new CloudflareException("CaptchaSolver required");
			}
			string text = response.ToString();
			string text2 = text.Substring("data-sitekey=\"", "\"", 0, StringComparison.Ordinal, null);
			if (text2 == null)
			{
				throw new CloudflareException("Value of \"data-sitekey\" not found");
			}
			string text3 = text2;
			string text4 = text.Substring("name=\"s\" value=\"", "\"", 0, StringComparison.Ordinal, null);
			if (text4 == null)
			{
				throw new CloudflareException("Value of \"s\" not found");
			}
			string text5 = text4;
			string text6 = text.Substring("data-ray=\"", "\"", 0, StringComparison.Ordinal, null);
			if (text6 == null)
			{
				throw new CloudflareException("Ray Id not found");
			}
			string text7 = text6;
			string text8 = request.CaptchaSolver.SolveRecaptcha(uri.AbsoluteUri, text3, cancelToken);
			cancelToken.ThrowIfCancellationRequested();
			RequestParams requestParams = new RequestParams(false, false);
			requestParams["s"] = text5;
			requestParams["id"] = text7;
			requestParams["g-recaptcha-response"] = text8;
			RequestParams requestParams2 = requestParams;
			string text9 = text.Substring("'bf_challenge_id', '", "'", 0, StringComparison.Ordinal, null);
			if (text9 != null)
			{
				requestParams2.Add(new KeyValuePair<string, string>("bf_challenge_id", text9));
				requestParams2.Add(new KeyValuePair<string, string>("bf_execution_time", "4"));
				requestParams2.Add(new KeyValuePair<string, string>("bf_result_hash", string.Empty));
			}
			response = request.ManualGet(new Uri(uri, "/cdn-cgi/l/chk_captcha"), uri, requestParams2);
			return CloudflareBypass.IsChallengePassed("ReCaptcha", ref response, request, uri, retry, log);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00023944 File Offset: 0x00021B44
		private static bool HasAccessDeniedError(HttpResponse response)
		{
			string text = response.ToString();
			string text2 = text.Substring("class=\"cf-subheadline\">", "<", 0, StringComparison.Ordinal, null) ?? text.Substring("<title>", "</title>", 0, StringComparison.Ordinal, null);
			return !string.IsNullOrEmpty(text2) && text2.ContainsInsensitive("Access denied");
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x000239A4 File Offset: 0x00021BA4
		private static HttpResponse ManualGet(this HttpRequest request, Uri uri, Uri refererUri = null, RequestParams requestParams = null)
		{
			request.ManualMode = true;
			request.AddCloudflareHeaders(refererUri ?? uri);
			HttpResponse httpResponse = ((requestParams == null) ? request.Get(uri, null) : request.Get(uri, requestParams));
			request.ManualMode = false;
			return httpResponse;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000239F0 File Offset: 0x00021BF0
		private static void AddCloudflareHeaders(this HttpRequest request, Uri refererUri)
		{
			request.AddHeader(HttpHeader.Referer, refererUri.AbsoluteUri);
			request.AddHeader(HttpHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.AddHeader("Upgrade-Insecure-Requests", "1");
			if (!request.ContainsHeader(HttpHeader.AcceptLanguage))
			{
				request.AddHeader(HttpHeader.AcceptLanguage, CloudflareBypass.DefaultAcceptLanguage);
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00023A48 File Offset: 0x00021C48
		private static void Delay(int milliseconds, CloudflareBypass.DLog log, CancellationToken cancellationToken)
		{
			if (log != null)
			{
				log(string.Format("{0}: delay {1} ms...", "[Cloudflare] ", milliseconds));
			}
			if (cancellationToken == default(CancellationToken))
			{
				Thread.Sleep(milliseconds);
				return;
			}
			cancellationToken.WaitHandle.WaitOne(milliseconds);
			cancellationToken.ThrowIfCancellationRequested();
		}

		// Token: 0x040002C4 RID: 708
		private const string PublicVersionIsNotMaintained = "Not maintained in public Leaf.xNet anymore.\r\nYou can order private paid Leaf.xNet with support.\r\nTelegram: @kelog";

		// Token: 0x040002C5 RID: 709
		public const string CfClearanceCookie = "cf_clearance";

		// Token: 0x040002C9 RID: 713
		private const string LogPrefix = "[Cloudflare] ";

		// Token: 0x020001E4 RID: 484
		// (Invoke) Token: 0x060015DE RID: 5598
		public delegate void DLog(string message);
	}
}
