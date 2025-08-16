using System;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x02000064 RID: 100
	[ComVisible(true)]
	public enum HttpStatusCode
	{
		// Token: 0x0400023E RID: 574
		None,
		// Token: 0x0400023F RID: 575
		Continue = 100,
		// Token: 0x04000240 RID: 576
		SwitchingProtocols,
		// Token: 0x04000241 RID: 577
		OK = 200,
		// Token: 0x04000242 RID: 578
		Created,
		// Token: 0x04000243 RID: 579
		Accepted,
		// Token: 0x04000244 RID: 580
		NonAuthoritativeInformation,
		// Token: 0x04000245 RID: 581
		NoContent,
		// Token: 0x04000246 RID: 582
		ResetContent,
		// Token: 0x04000247 RID: 583
		PartialContent,
		// Token: 0x04000248 RID: 584
		MultipleChoices = 300,
		// Token: 0x04000249 RID: 585
		Ambiguous = 300,
		// Token: 0x0400024A RID: 586
		MovedPermanently,
		// Token: 0x0400024B RID: 587
		Moved = 301,
		// Token: 0x0400024C RID: 588
		Found,
		// Token: 0x0400024D RID: 589
		Redirect = 302,
		// Token: 0x0400024E RID: 590
		SeeOther,
		// Token: 0x0400024F RID: 591
		RedirectMethod = 303,
		// Token: 0x04000250 RID: 592
		NotModified,
		// Token: 0x04000251 RID: 593
		UseProxy,
		// Token: 0x04000252 RID: 594
		Unused,
		// Token: 0x04000253 RID: 595
		TemporaryRedirect,
		// Token: 0x04000254 RID: 596
		RedirectKeepVerb = 307,
		// Token: 0x04000255 RID: 597
		BadRequest = 400,
		// Token: 0x04000256 RID: 598
		Unauthorized,
		// Token: 0x04000257 RID: 599
		PaymentRequired,
		// Token: 0x04000258 RID: 600
		Forbidden,
		// Token: 0x04000259 RID: 601
		NotFound,
		// Token: 0x0400025A RID: 602
		MethodNotAllowed,
		// Token: 0x0400025B RID: 603
		NotAcceptable,
		// Token: 0x0400025C RID: 604
		ProxyAuthenticationRequired,
		// Token: 0x0400025D RID: 605
		RequestTimeout,
		// Token: 0x0400025E RID: 606
		Conflict,
		// Token: 0x0400025F RID: 607
		Gone,
		// Token: 0x04000260 RID: 608
		LengthRequired,
		// Token: 0x04000261 RID: 609
		PreconditionFailed,
		// Token: 0x04000262 RID: 610
		RequestEntityTooLarge,
		// Token: 0x04000263 RID: 611
		RequestUriTooLong,
		// Token: 0x04000264 RID: 612
		UnsupportedMediaType,
		// Token: 0x04000265 RID: 613
		RequestedRangeNotSatisfiable,
		// Token: 0x04000266 RID: 614
		ExpectationFailed,
		// Token: 0x04000267 RID: 615
		UpgradeRequired = 426,
		// Token: 0x04000268 RID: 616
		PreconditionRequired = 428,
		// Token: 0x04000269 RID: 617
		TooManyRequests,
		// Token: 0x0400026A RID: 618
		RequestHeaderFieldsTooLarge = 431,
		// Token: 0x0400026B RID: 619
		UnavailableForLegalReasons = 451,
		// Token: 0x0400026C RID: 620
		InternalServerError = 500,
		// Token: 0x0400026D RID: 621
		NotImplemented,
		// Token: 0x0400026E RID: 622
		BadGateway,
		// Token: 0x0400026F RID: 623
		ServiceUnavailable,
		// Token: 0x04000270 RID: 624
		GatewayTimeout,
		// Token: 0x04000271 RID: 625
		HttpVersionNotSupported,
		// Token: 0x04000272 RID: 626
		NetworkAuthenticationRequired = 511,
		// Token: 0x04000273 RID: 627
		InvalidStatusCode = 555
	}
}
